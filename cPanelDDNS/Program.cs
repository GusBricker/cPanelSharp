using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using cPanelAPILib;
using cPanelAPILib.Modules;
using cPanelAPILib.Records;
using CommandLine;
using Utils.Logging;
using Utils.IP;

namespace cPanelDDNS
{
	class MainClass
	{
		private enum DDNSResult
		{
			Error,
			NotNeccessary,
			UpdateSuccess
		}

		private enum DDNSReturnCodes
		{
			NoUpdateNeccessary,
			UpdateSuccess,
			InvalidOptionsGiven,
			IPLookupFailure,
			GeneralError
		}

		public static void Main (string[] args)
		{
            string logPath = Path.GetTempPath();
            logPath = Path.Combine(logPath, "cPanel");
            logPath += DateTime.Now.ToString("yyyyMMdd-HHmmss");
            logPath += ".log";
            Logger.Setup(logPath);

			Logger.Instance.Write (Logger.Severity.Debug, "Starting up...");

			CmdOptions options = new CmdOptions ();
			if (CommandLine.Parser.Default.ParseArguments (args, options) == false)
			{
				Console.WriteLine ("Return codes: ");
				Console.WriteLine (BuildReturnString ());
				Environment.Exit ((int)DDNSReturnCodes.InvalidOptionsGiven);
			}

			// Check if we need to lookup our public IP on the internet
			if (string.IsNullOrEmpty (options.IP))
			{
				Logger.Instance.Write (Logger.Severity.Debug, 
				                       "No IP given on the command line, looking up");
				options.IP = PublicIP.Lookup ();

				if (options.IP == string.Empty)
				{
					Logger.Instance.Write (Logger.Severity.Error, "IP Address lookup failed");	
					Environment.Exit ((int)DDNSReturnCodes.IPLookupFailure);
				}

				Logger.Instance.Write (Logger.Severity.Debug, 
				                       "Got IP: " + options.IP);
			}
			                                                

			// Setup the cPanelAPI
			cPanelAPI api = new cPanelAPI (options.URL,
			                               options.Username,
			                               options.Password,
			                               options.Secure);

			// Update the domain A record only, point to our IP.
			DDNSResult result;
			result = UpdateDomainARecord (api,
			                              options.Domain,
			                              options.Zone,
			                              options.IP);

			if (result == DDNSResult.NotNeccessary)
			{
				Logger.Instance.Write (Logger.Severity.Debug, 
									   "IP address in record same as given IP, nothing to do");
				Environment.Exit ((int)DDNSReturnCodes.NoUpdateNeccessary);
			}
			else if (result == DDNSResult.UpdateSuccess)
			{
				Logger.Instance.Write (Logger.Severity.Debug,
				                       "Successfully updated zone " + options.Zone +
									   " on domain " + options.Domain + 
									   " to point to IP " + options.IP);
				Environment.Exit ((int)DDNSReturnCodes.UpdateSuccess);
			}
			else
			{
				Logger.Instance.Write (Logger.Severity.Error, "Updating zone record has failed");
				Environment.Exit ((int)DDNSReturnCodes.GeneralError);
			}
		}

		private static DDNSResult UpdateDomainARecord (cPanelAPI api,
			                                          string domain,
			                                          string zone,
			                                          string ip)
		{
			cPanelZoneEditFunctions funcs = new cPanelZoneEditFunctions (api);
			FetchZoneResponse zoneInfo;

			if (funcs.FetchZone (domain, zone, out zoneInfo) == true)
			{
				EditZoneResponse zoneResponse;
				int recordLine = -1;

				try
				{
					// Convert to int to make sure its a valid value
					recordLine = Convert.ToInt32 (zoneInfo.Zone.Line);
				}
				catch (Exception)
				{
					Logger.Instance.Write (Logger.Severity.Error, "Received record line is not an integer");
					return DDNSResult.Error; 
				}

				Logger.Instance.Write (Logger.Severity.Debug, "Given IP: " + ip);
				Logger.Instance.Write (Logger.Severity.Debug, "Zone IP: " + zoneInfo.Zone.Address);
				if (ip == zoneInfo.Zone.Address)
				{
					return DDNSResult.NotNeccessary;
				}

				if (funcs.EditZoneARecord (domain, ip, recordLine, out zoneResponse) == true)
				{
					if (zoneResponse.Status == "1")
					{
						return DDNSResult.UpdateSuccess;
					}
					else
					{
						return DDNSResult.Error;
					}
				}
			}

			return DDNSResult.Error;
		}

		private static string BuildReturnString ()
		{
			StringBuilder sb = new StringBuilder ();

			foreach (DDNSReturnCodes code in Enum.GetValues (typeof(DDNSReturnCodes)))
			{
				sb.Append ((int)code);
				sb.Append (": ");
				sb.Append (code.ToString ());
				sb.AppendLine ();
			}

			return sb.ToString ();
		}

//		public static bool UpdateDomainCName (cPanelAPI api,
//				                              string domain,
//				                              string name,
//				                              string alias)
//		{
//			bool pass = false;
//
//			cPanelResult<FetchZoneResponse> result;
//			pass = FetchZone (api,
//			                  domain,
//			                  name,
//			                  out result);
//
//			if (pass == true)
//			{
//				if (result.Data.Length != 1)
//				{
//					Console.WriteLine ("Expecting 1 result back, got: {0}", result.Data.Length);
//					return false;
//				}
//
//				if (result.Data [0].Status == "0")
//				{
//					Console.WriteLine ("Fetching zone failed: {0}", result.Data [0].StatusMessage);
//					return false;
//				}
//
//				if (result.Data[0].Zone.Length != 1)
//				{
//					Console.WriteLine ("Expecting 1 Zone Record back, got: {0}", result.Data [0].Zone.Length);
//					return false;
//				}
//
//				ZoneRecord zRec = result.Data [0].Zone [0];
//				pass = EditZoneCName (api,
//					                  domain,
//					                  alias,
//					                  zRec);
//			}
//
//			return pass;
//		}
//
//		public static bool UpdateDomainARecord (cPanelAPI api,
//				                                string domain,
//				                                string name,
//				                                string ipAddress)
//		{
//			bool pass = false;
//
//			cPanelResult result;
//			pass = FetchZone (api,
//			                  domain,
//			                  name,
//			                  out result);
//
//			if (pass == true)
//			{
//				if (result.Data.Length != 1)
//				{
//					Console.WriteLine ("Expecting 1 result back, got: {0}", result.Data.Length);
//					return false;
//				}
//
//				if (result.Data [0].Status == "0")
//				{
//					Console.WriteLine ("Fetching zone failed: {0}", result.Data [0].StatusMessage);
//					return false;
//				}
//
//				if (result.Data[0].Zone.Length != 1)
//				{
//					Console.WriteLine ("Expecting 1 Zone Record back, got: {0}", result.Data [0].Zone.Length);
//					return false;
//				}
//
//				ZoneRecord zRec = result.Data [0].Zone [0];
//				pass = EditZoneARecord (api,
//						                domain,
//						                ipAddress,
//						                zRec);
//			}
//
//			return pass;
//		}
//
//		public static bool FetchZone (cPanelAPI api, 
//		                              string domain,
//		                              string name,
//		                              out cPanelResult result)
//		{
//			string response;
//			bool pass;
//			result = null;
//
//			pass = api.TryOperate ("ZoneEdit",
//			                       "fetchzone",
//			                       out response,
//			                       new cPanelAPI.ParameterPair ("domain", domain),
//			                       new cPanelAPI.ParameterPair ("name", name));
//			if (pass == true)
//			{
//				pass = cPanelResult.TryLoad (response, out result);
//			}
//
//			return pass;
//		}
//
//		public static bool EditZoneCName (cPanelAPI api,
//				                          string domain,
//				                          string alias,
//				                          ZoneRecord record)
//		{
//			string response;
//			bool pass;
//
//			pass = api.TryOperate ("ZoneEdit",
//			                       "edit_zone_record",
//			                       out response,
//			                       new cPanelAPI.ParameterPair ("domain", domain),
//			                       new cPanelAPI.ParameterPair ("line", record.Line),
//			                       new cPanelAPI.ParameterPair ("type", "CNAME"),
//			                       new cPanelAPI.ParameterPair ("cname", alias),
//			                       new cPanelAPI.ParameterPair ("ttl", "60"));
//
//			if (pass == true)
//			{
//				cPanelResult fullResult;
//				pass = cPanelResult.TryLoad (response, out fullResult);
//			}
//
//			return pass;
//		}
//
//		public static bool EditZoneARecord (cPanelAPI api,
//				                            string domain,
//				                            string ipAddress,
//				                            ZoneRecord record)
//		{
//			string response;
//			bool pass;
//
//			pass = api.TryOperate ("ZoneEdit",
//			                       "edit_zone_record",
//			                       out response,
//			                       new cPanelAPI.ParameterPair ("domain", domain),
//			                       new cPanelAPI.ParameterPair ("line", record.Line),
//			                       new cPanelAPI.ParameterPair ("type", "A"),
//			                       new cPanelAPI.ParameterPair ("address", ipAddress),
//			                       new cPanelAPI.ParameterPair ("ttl", "60"));
//
//			if (pass == true)
//			{
//				cPanelResult fullResult;
//				pass = cPanelResult.TryLoad (response, out fullResult);
//			}
//
//			return pass;
//		}

	}
}
