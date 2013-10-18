using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace cPanelAPILib.Records
{
	[XmlRoot("cpanelresult")]
	public class cPanelResult<TResponse>
	{
		[XmlElement(ElementName = "apiversion")]
		public string APIVersion;

		[XmlElement(ElementName = "data", IsNullable = false)]
		public TResponse Data; 

		[XmlElement(ElementName = "func")]
		public string Function;

		[XmlElement(ElementName = "module")]
		public string Module;

		[XmlArray("event")]
		[XmlArrayItem("result")]
		public string[] Results;

		public cPanelResult ()
		{
			APIVersion = string.Empty;
			Data = default (TResponse);
			Results = new string[0];
			Function = string.Empty;
			Module = string.Empty;
		}

		public static bool TrySave (out string str, cPanelResult<TResponse> res)
		{
			MemoryStream ms = new MemoryStream ();
			bool result = TrySave (ms, res);

			ms.Position = 0;
			StreamReader reader = new StreamReader (ms);
			str = reader.ReadToEnd ();

			return result;
		}

		public static bool TrySave (Stream s, cPanelResult<TResponse> res)
		{
			Type[] tArray = new Type[] { typeof (TResponse) };
			XmlSerializer ser = new XmlSerializer (res.GetType (), tArray);

			try
			{
				ser.Serialize (s, res);
			}
			catch (Exception e)
			{
				Console.WriteLine ("XML Serialization Failure: " + e.Message);
			}

			return true;
		}

		public static bool TryLoad (string str, out cPanelResult<TResponse> res)
		{
			MemoryStream ms = new MemoryStream ();
			StreamWriter writer = new StreamWriter (ms);
			writer.Write (str);
			writer.Flush ();
			ms.Position = 0;

			return TryLoad (ms, out res);
		}

		public static bool TryLoad (Stream s, out cPanelResult<TResponse> res)
		{
			res = new cPanelResult<TResponse>();
			Type[] tArray = new Type[] { typeof (TResponse) };

			try
			{
				XmlSerializer ser = new XmlSerializer (res.GetType (), tArray);
				ser.UnknownAttribute += new XmlAttributeEventHandler(XmlUnknownAttribute);
				ser.UnknownNode += new XmlNodeEventHandler(XmlUnknownNode);
				res = (cPanelResult<TResponse>) ser.Deserialize (s);
			}
			catch (Exception e)
			{
				Console.WriteLine ("XML Serialization Failure: " + e.Message);
				return false;
			}

			return true;
		}

		private static void XmlUnknownNode(object sender, XmlNodeEventArgs e)
		{
			Console.WriteLine("Unknown Node:" +   e.Name + "\t" + e.Text);
		}

		private static void XmlUnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Console.WriteLine("Unknown attribute " + 
			attr.Name + "='" + attr.Value + "'");
		}
	}
}

