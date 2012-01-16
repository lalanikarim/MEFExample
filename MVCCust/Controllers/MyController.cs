using System;
using MVCProvider;
using System.Collections.Generic;
using System.Web;
using System.Text;
using Microsoft.CSharp;

namespace MVCCust
{
	public class MyController : IController
	{
		#region IController implementation
		public string Process (string[] args)
		{
			StringBuilder sb = new StringBuilder();
			foreach(string arg in args)
			{
				sb.AppendFormat("{0}<br/>",arg);
			}
			return sb.ToString();
		}
		
		public string TryMixed(string a, int x)
		{
			return string.Format("{0}<br/>{1}",a,x);
		}
		
		public string Name {
			get {
				return "MyController";
			}
		}
		#endregion
		
	}
}

