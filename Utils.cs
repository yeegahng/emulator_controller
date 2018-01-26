/*
 * Created by SharpDevelop.
 * User: ygsong
 * Date: 01/09/2018
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Documents;

namespace Emulator_Controller
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{        
		public static string ConvertHexStrToDecStr(string hexStr)
		{
			return (hexStr.Length > 0) ? (int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber)).ToString() : "";
		}
		
		public static string ConvertDecStrToHexStr(string decStr)
		{
			return (decStr.Length > 0) ? Convert.ToInt32(decStr).ToString("X") : "";
		}
		
		/*
		 * Returns AssemblyFileVersion as the application version.
		 */
		public static string GetApplicationVersion()
		{
			//get AssemblyVersion of current assembly
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			
			//get AssemblyFileVersion of current assembly file
			System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
			return fvi.FileVersion;
		}
		
		public static string GetApplicationFileVersion()
		{
			System.Reflection.AssemblyFileVersionAttribute attribute
				= (System.Reflection.AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyFileVersionAttribute));
			return attribute.Version;
		}
		
		public static string GetApplicationCompanyName()
		{
			System.Reflection.AssemblyCompanyAttribute attribute
				= (System.Reflection.AssemblyCompanyAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyCompanyAttribute));
			return attribute.Company;
		}
		
		public static string GetApplicationDescription()
		{
			System.Reflection.AssemblyDescriptionAttribute attribute
				= (System.Reflection.AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyDescriptionAttribute));
			return attribute.Description;
		}
		
		public static string GetApplicationCopyrightInfo()
		{
			System.Reflection.AssemblyCopyrightAttribute attribute
				= (System.Reflection.AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyCopyrightAttribute));
			return attribute.Copyright;
		}
		
		public static string GetEntryAssemblyVersion()
		{
			//<test> get AssemblyVersion of entry assembly			
			System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();
			Version assemblyVersion = assemblyName.Version;
			return assemblyVersion.ToString();
		}
		
		public static string GetEntryAssemblyFileVersion()
		{
			System.Reflection.AssemblyFileVersionAttribute attribute
				= (System.Reflection.AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetEntryAssembly(), typeof(System.Reflection.AssemblyFileVersionAttribute));
			return attribute.Version;
		}
	}
}
