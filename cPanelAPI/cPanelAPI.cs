using System;
using System.Text;
using System.Net;
using System.IO;
using System.Security;
using Utils.Logging;

namespace cPanelAPILib
{
	public class cPanelAPI
	{
		private static int SecurePort = 2083;
		private static int UnsecurePort = 2082;

		private string _url;
		private string _username;
		private string _password;
		private bool _useSecurity;

		public cPanelAPI (string url,
		                  string username,
		                  string password,
		                  bool secure)
		{
			_url = url;
			_username = username;
			_password = password;
			_useSecurity = secure;
		}

		public bool TryOperate (string module,
		                        string function,
		                        out string response,
		                        params ParameterPair[] pairs)
		{
			string apiCall;
			response = string.Empty;

			apiCall = ConstructAPICall (_url,
			                            _useSecurity,
			                            _username,
			                            module,
			                            function,
			                            pairs);


			NetworkCredential nc = new NetworkCredential ();
			nc.UserName = _username;
			nc.Password = _password;
			CredentialCache cc = new CredentialCache();
			cc.Add(new Uri(apiCall), "Basic", nc);

			Logger.Instance.Write (Logger.Severity.Debug,
			                       "Beginning request: ");
			Logger.Instance.Write (Logger.Severity.Debug, apiCall);


			WebRequest request = WebRequest.Create (apiCall);
			request.Credentials = cc;
			request.Timeout = 8000;
			request.PreAuthenticate = true;

			HttpWebResponse webResp = (HttpWebResponse)request.GetResponse();
			Logger.Instance.Write (Logger.Severity.Debug,
			                       "Response from cPanel request: ");
			Logger.Instance.Write (Logger.Severity.Debug, 
			                       webResp.StatusDescription);
			if (webResp.StatusDescription == "OK")
			{
				StreamReader reader = new StreamReader (webResp.GetResponseStream (), Encoding.UTF8);
				response = reader.ReadToEnd ();

				Logger.Instance.Write (Logger.Severity.Debug, response);

				return true;
			}
			return false;
		}


		private static string ConstructAPICall (string url,
		                                        bool secure,
		                                        string username,
		                                        string module,
		                                        string function,
		                                        params ParameterPair[] pairs)
		{
			StringBuilder sb = new StringBuilder ();

			if (secure == true)
			{
				sb.Append (@"https://");
			}
			else
			{
				sb.Append (@"http://");
			}

			sb.Append (url);
			sb.Append (":");
			if (secure == true)
			{
				sb.Append (SecurePort);
			}
			else
			{
				sb.Append (UnsecurePort);
			}

			sb.Append ("/xml-api/");
			sb.Append ("cpanel?cpanel_xmlapi_user=");
			sb.Append (username);
			sb.Append ("&cpanel_xmlapi_module=");
			sb.Append (module);
			sb.Append ("&cpanel_xmlapi_func=");
			sb.Append (function);
			sb.Append ("&cpanel_xmlapi_apiversion=2");
		
			for (int index=0; index<pairs.Length; index++)
			{
				ParameterPair pair = pairs[index];
				sb.Append ("&");
				sb.Append (pair.Combined);
			}

			return sb.ToString ();
		}
	}
}

