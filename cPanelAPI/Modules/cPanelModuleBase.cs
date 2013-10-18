using System;
using cPanelAPILib;

namespace cPanelAPILib.Modules
{
	public abstract class cPanelModuleBase
	{
		protected cPanelAPI _api;

		public cPanelModuleBase (cPanelAPI api)
		{
			_api = api;
		}
	}
}

