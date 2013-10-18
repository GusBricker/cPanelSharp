using System;
using System.Text;

namespace cPanelAPILib
{
	public class ParameterPair
	{
		public string Name { get; private set; }
		public string Value { get; private set; }
		public string Seperator { get; private set; }
		public string Combined
		{
			get
			{
				return Name + Seperator + Value;
			}
		}

		public ParameterPair (string name)
		{
			Name = name;
			Value = string.Empty;
			Seperator = string.Empty;
		}

		public ParameterPair (string name, string value)
		{
			Name = name;
			Value = value;
			Seperator = "=";
		}

		public ParameterPair (string name, int value)
			: this (name, Convert.ToString (value))
		{
		
		}

//		public ParameterPair (string name, params ParameterPair[] pairs)
//		{
//			Name = name;
//			Seperator = "=";
//
//			StringBuilder sb = new StringBuilder ();
//			sb.Append ('"');
//			for (int index=0; index<pairs.Length; index++)
//			{
//				ParameterPair pair = pairs[index];
//				sb.Append (pair.Combined);
//				if (index < (pairs.Length-1))
//				{
//					sb.Append ("&");
//				}
//			}
//			sb.Append ('"');
//
//			Value = sb.ToString ();
//		}
	}
}

