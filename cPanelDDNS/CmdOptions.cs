using System;
using CommandLine;
using CommandLine.Text;
using System.Security;
using System.Text;

namespace cPanelDDNS
{
	public class CmdOptions
	{
		[Option ('u', "username", Required = true,
		         HelpText = "cPanel username")]
		public string Username { get; set; }

		[Option('p', "password", Required = true,
		         HelpText = "cPanel password")]
		public string Password { get; set; }

		[Option ("url", Required = true,
		         HelpText = "cPanel URL, not including the http:// or https://")]
		public string URL { get; set; }

		[Option ("secure", Required = false,
		         HelpText = "Whether to use https or not, Recommend that this option is always specified!!")]
		public bool Secure { get; set; }

		[Option("domain", Required = true,
		        HelpText = "Root domain the subdomain is part of.")]
		public string Domain { get; set; }

		[Option("zone", Required = true,
		        HelpText = "Sub domain to lookup and check. Doesnt need the www at start")]
		public string Zone { get; set; }

		[Option("ip", Required = false,
		        HelpText = "IP Address to use, if not given, will auto determine current IP")]
		public string IP { get; set; }

		[HelpOption()]
		public string GetUsage()
		{
			return HelpText.AutoBuild (this,
				   (HelpText current) => HelpText.DefaultParsingErrorsHandler (this, current));
		}

//		public CmdOptions ()
//		{
//			Username = string.Empty;
//			Password = string.Empty;
//			URL = string.Empty;
////			Secure = true;
//			Domain = string.Empty;
//			Zone = string.Empty;
//			IP = string.Empty;
//		}
	}
}

