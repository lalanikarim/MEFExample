using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web;
using System.Web.SessionState;
using MVCProvider;
using System.Collections.Generic;


namespace MVCCust
{
	public class Global : System.Web.HttpApplication
	{
		[Import]
		protected Action<HttpApplication> ProcessRequest;
		
		protected virtual void Application_Start (Object sender, EventArgs e)
		{			
			DirectoryCatalog dirCatalog = new DirectoryCatalog(Server.MapPath("bin"),"*.dll");
			AssemblyCatalog asCatalog = new AssemblyCatalog(this.GetType().Assembly);
			AggregateCatalog catalog = new AggregateCatalog(dirCatalog,asCatalog);
			CompositionContainer container = new CompositionContainer(catalog);
			container.SatisfyImportsOnce(this);
		}
		
		protected virtual void Session_Start (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_BeginRequest (Object sender, EventArgs e)
		{
			ProcessRequest(this);			
		}
		
		protected virtual void Application_EndRequest (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_AuthenticateRequest (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_Error (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Session_End (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_End (Object sender, EventArgs e)
		{
		}
	}
}

