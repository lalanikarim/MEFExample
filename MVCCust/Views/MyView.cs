using System;
using MVCProvider;

namespace MVCCust
{
	public class MyView : IView
	{
		#region IView implementation
		public string Action {
			get {
				return "MyView";
			}
		}

		public string Template {
			get {
				return "MyViewTemplate.txt";
			}
		}
		#endregion
	}
}

