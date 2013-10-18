using System;
using System.Xml;
using System.Xml.Serialization;

namespace cPanelAPILib.Records
{
	public class DataRecordBase
	{
		[XmlElement(ElementName = "serialnum")]
		[XmlElement(ElementName = "newserial")]
		public string SerialNumber;

		[XmlElement(ElementName = "status")]
		public string Status;

		[XmlElement(ElementName = "statusmsg")]
		public string StatusMessage;

		public DataRecordBase ()
		{
			SerialNumber = string.Empty;
			Status = string.Empty;
			StatusMessage = string.Empty;
		}
	}
}

