using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace MVCProvider
{
	[InheritedExport]
	public interface IController
	{
		string Name{get;}
		string Process(string[] args);
	}
}

