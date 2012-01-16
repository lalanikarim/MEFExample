using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web;
using MVCProvider;
using System.IO;
using System.Reflection;


namespace MVCCust
{
	public class MySite
	{
		
		public MySite ()
		{
		}
		
		[ImportMany]
		public IEnumerable<IController> Controllers{ get;set;}
		
		[ImportMany]
		public IEnumerable<IView> Views{get;set;}
		
		[Export]
		public void ProcessRequest(HttpApplication application)
		{
			string path = application.Request.AppRelativeCurrentExecutionFilePath;
			if(!File.Exists(application.Server.MapPath(path)))
			{
				IList<string> parts = new List<string>(path.Split(new char[]{'/','~'},StringSplitOptions.RemoveEmptyEntries));
				
				foreach(var controller in Controllers)
				{
					if(parts.Count > 0)
					{
						if(controller.Name == parts[0])
						{
							parts.RemoveAt(0);
							Type cType = controller.GetType();
							if(parts.Count > 0)
							{
								MethodInfo mi = cType.GetMethod(parts[0]);
								if(mi != null)
								{
									parts.RemoveAt(0);
									object[] args = ConvertToObjectArray(parts);
									object result = mi.Invoke(controller,args);
									application.Response.Write(result.ToString());
									application.Response.End();
								}
							}
							else
							{
								object[] args = ConvertToObjectArray(parts);
								MethodInfo defaultMi = cType.GetMethod("Process");
								
								object result = defaultMi.Invoke(controller,args);
								application.Response.Write(result.ToString());
								application.Response.End();
							}
							break;
						}
					}
				}
			}
		}
		
		private object[] ConvertToObjectArray(IList<string> list)
		{
			object[] array = new object[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
				array[i] = list[i];
			}
			return array;
		}
	}
}

