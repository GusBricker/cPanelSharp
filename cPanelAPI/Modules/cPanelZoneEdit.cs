using System;
using cPanelAPILib;
using cPanelAPILib.Records;

namespace cPanelAPILib.Modules
{
	public class cPanelZoneEditFunctions : cPanelModuleBase
	{
		public cPanelZoneEditFunctions (cPanelAPI api)
			: base (api)
		{
		}

		public bool FetchZone (string domain,
		                       string name,
		                       out FetchZoneResponse response)
		{
			string rawResponse;
			bool pass;
			cPanelResult<FetchZoneResponse> cResult;
			response = null;

			pass = _api.TryOperate ("ZoneEdit",
			                       "fetchzone",
			                       out rawResponse,
			                       new ParameterPair ("domain", domain),
			                       new ParameterPair ("name", name));

			if (pass == true)
			{
				pass = cPanelResult<FetchZoneResponse>.TryLoad (rawResponse, out cResult);
				if (cResult.Data.Status == "1")
				{
					response = cResult.Data;
				}
				else
				{
					pass = false;
				}
			}

			return pass;
		}

		public bool EditZoneCName (string domain,
				                   string alias,
		                           int line,
				                   out EditZoneResponse response)
		{
			string rawResponse;
			bool pass;
			cPanelResult<EditZoneResponse> cResult;
			response = null;

			pass = _api.TryOperate ("ZoneEdit",
			                       "edit_zone_record",
			                       out rawResponse,
			                       new ParameterPair ("domain", domain),
			                       new ParameterPair ("line", line),
			                       new ParameterPair ("type", "CNAME"),
			                       new ParameterPair ("cname", alias),
			                       new ParameterPair ("ttl", "60"));

			if (pass == true)
			{
				pass = cPanelResult<EditZoneResponse>.TryLoad (rawResponse, out cResult);
				if (cResult.Data.Status == "1")
				{
					response = cResult.Data;
				}
				else
				{
					pass = false;
				}
			}

			return pass;
		}

		public bool EditZoneARecord (string domain,
				                     string ipAddress,
			                         int line,
				                     out EditZoneResponse response)
		{
			string rawResponse;
			bool pass;
			cPanelResult<EditZoneResponse> cResult;
			response = null;

			pass = _api.TryOperate ("ZoneEdit",
			                       "edit_zone_record",
			                       out rawResponse,
			                       new ParameterPair ("domain", domain),
			                       new ParameterPair ("line", line),
			                       new ParameterPair ("type", "A"),
			                       new ParameterPair ("address", ipAddress),
			                       new ParameterPair ("ttl", "60"));

			if (pass == true)
			{
				pass = cPanelResult<EditZoneResponse>.TryLoad (rawResponse, out cResult);
				if (cResult.Data.Status == "1")
				{
					response = cResult.Data;
				}
				else
				{
					pass = false;
				}
			}

			return pass;
		}

	}
}

