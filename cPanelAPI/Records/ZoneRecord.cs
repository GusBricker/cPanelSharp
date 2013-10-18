using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace cPanelAPILib.Records
{
	[XmlRoot("record", IsNullable = false)]
	public class ZoneRecord
	{
		[XmlElement(ElementName = "name")]
		public string Name;

		[XmlElement(ElementName = "line")]
		public string Line;

		[XmlElement(ElementName = "address")]
		public string Address;

		[XmlElement(ElementName = "record")]
		public string Record;

		[XmlElement(ElementName = "ttl")]
		public string TTL;

		[XmlElement(ElementName = "type")]
		public string Type;

		public ZoneRecord ()
		{
			Name = string.Empty;
			Line = string.Empty;
			Address = string.Empty;
			Record = string.Empty;
			TTL = string.Empty;
			Type = string.Empty;
		}
	}
}

