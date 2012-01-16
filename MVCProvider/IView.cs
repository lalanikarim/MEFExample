using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web;

namespace MVCProvider
{
	[InheritedExport]
	public interface IView
	{
		string Action{get;}
		string Template{get;}
	}
}

