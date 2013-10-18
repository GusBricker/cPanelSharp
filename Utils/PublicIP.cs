using System;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Utils.IP
{
	public static class PublicIP
	{
		public static string Lookup()
		{
			WebRequest req = WebRequest.Create ("http://checkip.dyndns.org:8245");
			req.Timeout = 2000;

			HttpWebResponse webResp = (HttpWebResponse)req.GetResponse();
			StreamReader reader = new StreamReader (webResp.GetResponseStream (), Encoding.UTF8);
			string response = reader.ReadToEnd ();
		    response = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(response).Value;

		    return response;
		}
	}
}

