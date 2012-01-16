using System;
using MVCProvider;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using Microsoft.CSharp;

namespace MVCCust
{
	public class YourController : IController
	{
		#region IController implementation
		public string Process (string[] args)
		{
			string result = string.Empty;
			using(StreamReader sr = new StreamReader("Views/MyViewTemplate.txt"))
			{
				result = sr.ReadToEnd();
				sr.Close();
			}
			return result;
		}

		public string Name {
			get {
				return "YourController";
			}
		}
		#endregion
		
	}
}

