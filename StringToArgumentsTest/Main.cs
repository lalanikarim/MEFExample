using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StringToArgumentsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			MyClass1 c1 = new MyClass1();
			MyClass2 c2 = new MyClass2{ MyInt = 2, MyString = "MyString2"}; 
			Console.WriteLine(c1.MyMethod1("MyString1",2));
			Console.WriteLine(c1.MyMethod2(c2));
			Console.WriteLine(c1.MyMethod3("MyString2",c2,3));
			Console.WriteLine(c1.MyMethod4(DateTime.Now));
			
			NameValueCollection kvp1 = new NameValueCollection();
			kvp1.Add("myStr","MyString From String");
			kvp1.Add("myInt","20");
			
			NameValueCollection kvp2 = new NameValueCollection();
			kvp2.Add("myObj.MyString","MyString From String");
			kvp2.Add("myObj.MyInt","20");
			
			NameValueCollection kvp3 = new NameValueCollection();
			kvp3.Add("myObj.MyString","MyString From String");
			kvp3.Add("myObj.MyInt","20");
			kvp3.Add("myStr","MyString From String");
			kvp3.Add("myInt","20");
			
			NameValueCollection kvp4 = new NameValueCollection();
			kvp4.Add("myDt","01/02/2003");
			
			NameValueCollection kvp5 = new NameValueCollection();
			kvp5.Add("myStrArr[0]","myStr1");
			kvp5.Add("myStrArr[1]","myStr2");
			kvp5.Add("myStrArr[2]","myStr3");
			kvp5.Add("myStrArr[3]","myStr4");
			
			NameValueCollection kvp6 = new NameValueCollection();
			kvp6.Add("myClass2Arr[0].MyString","MyString1 From String");
			kvp6.Add("myClass2Arr[0].MyInt","20");
			kvp6.Add("myClass2Arr[1].MyString","MyString2 From String");
			kvp6.Add("myClass2Arr[1].MyInt","30");
			kvp6.Add("myClass2Arr[2].MyString","MyString3 From String");
			kvp6.Add("myClass2Arr[2].MyInt","40");
			
			Type myClass1Type = typeof(MyClass1);
			
			MethodInfo mi1 = myClass1Type.GetMethod("MyMethod1");
			MethodInfo mi2 = myClass1Type.GetMethod("MyMethod2");
			MethodInfo mi3 = myClass1Type.GetMethod("MyMethod3");
			MethodInfo mi4 = myClass1Type.GetMethod("MyMethod4");
			MethodInfo mi5 = myClass1Type.GetMethod("MyMethod5");
			MethodInfo mi6 = myClass1Type.GetMethod("MyMethod6");
			
			object[] objArr1 = GetParametersAsObjectArray(mi1,kvp1);
			object[] objArr2 = GetParametersAsObjectArray(mi2,kvp2);
			object[] objArr3 = GetParametersAsObjectArray(mi3,kvp3);
			object[] objArr4 = GetParametersAsObjectArray(mi4,kvp4);
			object[] objArr5 = GetParametersAsObjectArray(mi5,kvp5);
			object[] objArr6 = GetParametersAsObjectArray(mi6,kvp6);
			
			object mi1Return = mi1.Invoke(c1,objArr1);
			Console.WriteLine(mi1Return);
			object mi2Return = mi2.Invoke(c1,objArr2);
			Console.WriteLine(mi2Return);
			object mi3Return = mi3.Invoke(c1,objArr3);
			Console.WriteLine(mi3Return);
			object mi4Return = mi4.Invoke(c1,objArr4);
			Console.WriteLine(mi4Return);
			object mi5Return = mi5.Invoke(c1,objArr5);
			Console.WriteLine(mi5Return);
			object mi6Return = mi6.Invoke(c1,objArr6);
			Console.WriteLine(mi6Return);
		}

		public static object[] GetParametersAsObjectArray (MethodInfo mi, NameValueCollection kvpArr)
		{
			IList<object> result = new List<object>();
			
			ParameterInfo[] piArr = mi.GetParameters();
			
			foreach(ParameterInfo pi in piArr)
			{
				result.Add(GetObjectForParameter(pi.ParameterType,pi.Name,kvpArr));
			}
			return result.ToArray();
		}
		public static object GetObjectForParameter(Type paramType, string paramName,NameValueCollection kvpArr)
		{
			object result = null;
			
			if(paramType.IsArray)
			{
				Type elemType = paramType.GetElementType();
				
				var nvpKeySubset = from key in kvpArr.AllKeys
					where key.StartsWith(paramName + "[")
						select key;
				var nvpKeys = nvpKeySubset.ToArray();
				
				if(elemType.IsValueType || elemType == typeof(string))
				{
					result = Array.CreateInstance(elemType,nvpKeys.Length);
				
					for(int i = 0; i < nvpKeys.Length; i++)
					{
						NameValueCollection nvp = new NameValueCollection();
						nvp.Add(paramName,kvpArr[nvpKeys[i]]);
						(result as Array).SetValue(GetObjectForParameter(elemType,paramName,nvp),i);
					}
				}
				else
				{
					PropertyInfo[] piArr = elemType.GetProperties();
					IList<string> keyNames = new List<string>();
					foreach(string key in nvpKeys)
					{
						if(keyNames.Contains(key.Split('.')[0]))
							continue;
						keyNames.Add(key.Split('.')[0]);
					}
					result = Array.CreateInstance(elemType,keyNames.Count);
				
					for(int i = 0; i < keyNames.Count; i++)
					{
						NameValueCollection nvp = new NameValueCollection();
						foreach(PropertyInfo pi in piArr)
						{
							nvp.Add(paramName + "." + pi.Name,kvpArr[keyNames[i] + "." + pi.Name]);
						}
						(result as Array).SetValue(GetObjectForParameter(elemType,paramName,nvp),i);
					}
				}				
			}			
			else if(paramType.IsValueType || paramType == typeof(string))
			{
				foreach(string key in kvpArr.AllKeys)
				{					
					if(key == paramName)
					{
						result = ConvertFromString(paramType,kvpArr[key]);
						break;
					}
				}
			}
			else
			{
				result = Activator.CreateInstance(paramType);				
				PropertyInfo[] properties = paramType.GetProperties();				
				foreach(PropertyInfo pi in properties)
				{
					if(pi.PropertyType.IsValueType || pi.PropertyType == typeof(string))
					{
						foreach(string key in kvpArr)
						{
							if(key == paramName + "." + pi.Name)
							{
								object parameterValue = ConvertFromString (pi.PropertyType,kvpArr[key]);								
								paramType.GetProperty(pi.Name).SetValue(result,parameterValue,null);
								break;
							}
						}
					}
				}				
			}
			return result;
		}

		static object ConvertFromString (Type objType, string valueToConvert)
		{
			if(objType != typeof(string))
			{
				TypeConverter typeConv = TypeDescriptor.GetConverter(objType);
				return typeConv.ConvertFromString(valueToConvert);
			}
			else
			{
				return valueToConvert;
			}			
		}
	}
	public class MyClass1
	{
		public string MyMethod1(string myStr, int myInt)
		{
			return string.Format("myStr: {0} myInt: {1}",myStr,myInt);
		}
		public string MyMethod2(MyClass2 myObj)
		{
			return myObj.ToString();
		}
		public string MyMethod3(string myStr, MyClass2 myObj, int myInt)
		{
			return string.Format("myStr: {0} myObj: {1}, myInt: {2}",myStr,myObj,myInt);
		}
		public string MyMethod4(DateTime myDt)
		{
			return string.Format("myDt: {0}",myDt.ToLongDateString()); 
		}
		public string MyMethod5(string[] myStrArr)
		{
			StringBuilder sb = new StringBuilder();
			foreach(string myStr in myStrArr)
			{
				sb.AppendFormat("myStrArr[]: {0}\n",myStr);
			}
			return sb.ToString();
		}
		public string MyMethod6(MyClass2[] myClass2Arr)
		{
			StringBuilder sb = new StringBuilder();
			foreach(MyClass2 myClass2 in myClass2Arr)
			{
				sb.AppendFormat("myClass2[]: {0}\n",myClass2);
			}
			return sb.ToString();
		}
	}
	public class MyClass2
	{
		public string MyString{get;set;}
		public int MyInt{get;set;}
		public override string ToString ()
		{
			return string.Format ("[MyClass2: MyString={0}, MyInt={1}]", MyString, MyInt);
		}
	}
}
