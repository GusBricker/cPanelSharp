using System;
using System.Xml;
using System.Xml.Serialization;

namespace cPanelAPILib.Records
{
	public class EditZoneResponse : DataRecordBase
	{
		[XmlElement(ElementName = "name")]
		public string Name;

		public EditZoneResponse ()
		{
			Name = string.Empty;
		}
	}
}

