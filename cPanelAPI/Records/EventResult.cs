using System;
using System.Xml;
using System.Xml.Serialization;

namespace cPanelAPILib.Records
{
	public class EventResult
	{
		[XmlElement(ElementName = "result")]
		public int Result;

		public EventResult () 
		{
			Result = 0;
		}
	}
}

