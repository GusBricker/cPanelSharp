using System;
using System.Xml;
using System.Xml.Serialization;

namespace cPanelAPILib.Records
{
	public class FetchZoneResponse : DataRecordBase
	{
		[XmlElement(ElementName = "record", IsNullable = false)]
		public ZoneRecord Zone;

		public FetchZoneResponse () : base ()
		{
			Zone = new ZoneRecord ();
		}
	}
}

