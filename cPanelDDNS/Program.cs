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

namespace cPanelAPILib
{
	class MainClass
	{
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
				Console.WriteLine ("Invalid options given");
				Environment.Exit (1);
			}

			// Check if we need to lookup our public IP on the internet
			if (options.IP == string.Empty)
			{
				Logger.Instance.Write (Logger.Severity.Debug, 
				                       "No IP given on the command line, looking up");
				options.IP = PublicIP.Lookup ();

				if (options.IP == string.Empty)
				{
					Logger.Instance.Write (Logger.Severity.Error, "IP Address lookup failed");	
					Environment.Exit (3);
				}

				Logger.Instance.Write (Logger.Severity.Debug, 
				                       "Got IP: " + options.IP);
			}
			                                                

			// Setup the cPanelAPI
			bool pass;
			cPanelAPI api = new cPanelAPI (options.URL,
			                               options.Username,
			                               options.Password,
			                               options.Secure);

			// Update the domain A record only, point to our IP.
			pass = UpdateDomainARecord (api,
			                            options.Domain,
			                            options.Zone,
			                            options.IP);

			if (pass == true)
			{
				Logger.Instance.Write (Logger.Severity.Debug,
				                       "Successfully updated zone " + options.Zone +
									   " on domain " + options.Domain + 
									   " to point to IP " + options.IP);
			}
			else
			{
				Logger.Instance.Write (Logger.Severity.Error, "Updating zone record has failed");
				Environment.Exit (2);
			}
		}


		public static bool UpdateDomainARecord (cPanelAPI api,
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
					return false;
				}

				Logger.Instance.Write (Logger.Severity.Debug, "Given IP: " + ip);
				Logger.Instance.Write (Logger.Severity.Debug, "Zone IP: " + zoneInfo.Zone.Address);
				if (ip == zoneInfo.Zone.Address)
				{
					Logger.Instance.Write (Logger.Severity.Debug, "IP address in record same as given IP, nothing to do");
					return true;
				}

				if (funcs.EditZoneARecord (domain, ip, recordLine, out zoneResponse) == true)
				{
					return zoneResponse.Status == "1";
				}
			}

			return false;
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
