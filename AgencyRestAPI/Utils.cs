using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AgencyRestAPI
{
	internal class Utilities
	{
		public static string LogFileName
		{
			get
			{
				return "C:\\Coop B2B API Logs\\Coop B2B Logs - " + YYYYMMDD + ".txt";
			}
		}

		public static string DebugFileName
		{
			get
			{
				return "C:\\Coop B2B API Logs\\Debug - " + YYYYMMDD + ".txt";
			}
		}
		public static string LogFilePath
		{
			get
			{
				return "C:\\Coop B2B API Logs\\Coop B2B Logs" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
			}
		}

		private static string YYYYMMDD
		{
			get
			{
				return DateTime.Now.ToString("yyyy-MM-dd");
			}
		}

		public static string ValidateEntry(string entry)
		{
			string result = entry;
			try
			{
				entry = entry.Trim();
				char[] array = "'".ToCharArray();
				for (int i = 0; i < array.Length; i++)
				{
					char c = array[i];
					entry = entry.Replace(c.ToString(), "'" + c.ToString());
				}
				result = entry;
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string TitleCase(string sentence)
		{
			try
			{
				string[] array = sentence.Split(" ".ToCharArray());
				sentence = "";
				string[] array2 = array;
				foreach (string text in array2)
				{
					sentence = sentence + text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower() + " ";
				}
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
			}
			return sentence.Trim();
		}

		public static bool IsNumberValid(string number)
		{
			bool result = false;
			try
			{
				Convert.ToDouble(number);
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				LogDebugOnFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n the data [" + number + "]\n" + ex.Source + "\n" + ex.Message + "\n" + ex.StackTrace);
				//Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string FormatNumber(double number2Format)
		{
			return FormatNumber(number2Format.ToString());
		}

		public static string FormatNumber(string number2Format)
		{
			if (number2Format.Contains("(") || number2Format.Contains(")"))
			{
				number2Format = number2Format.Substring(1, number2Format.Length - 2);
			}
			string text = Convert.ToDouble(number2Format).ToString();
			string text2 = "";
			string text3 = "";
			bool flag = number2Format.ToString().Contains("-");
			if (flag)
			{
				text = text.Substring(1);
			}
			try
			{
				if (!text.Contains("."))
				{
					text += ".00";
				}
				int num = text.IndexOf(".");
				text3 = text.Substring(num + 1);
				if (text3.Length == 1)
				{
					text3 += "0";
				}
				else if (text3.Length > 2)
				{
					text3 = text3.Substring(0, 2);
				}
				int num2 = 0;
				for (int num3 = num - 1; num3 >= 0; num3--)
				{
					text2 = text.Substring(num3, 1) + text2;
					num2++;
					if (num2 == 3)
					{
						num2 = 0;
						if (num3 > 0)
						{
							text2 = "," + text2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
			}
			if (flag)
			{
				return "(" + text2 + "." + text3 + ")";
			}
			return text2 + "." + text3;
		}

		public static string FormatDate(DateTime date2Format)
		{
			string result = "";
			try
			{
				date2Format = date2Format.AddHours(-3.0);
				string text = date2Format.Year.ToString();
				string text2 = date2Format.Month.ToString();
				if (text2.Length == 1)
				{
					text2 = "0" + text2;
				}
				string text3 = date2Format.Day.ToString();
				if (text3.Length == 1)
				{
					text3 = "0" + text3;
				}
				string text4 = date2Format.Hour.ToString();
				if (text4.Length == 1)
				{
					text4 = "0" + text4;
				}
				string text5 = date2Format.Minute.ToString();
				if (text5.Length == 1)
				{
					text5 = "0" + text5;
				}
				string text6 = date2Format.Second.ToString();
				if (text6.Length == 1)
				{
					text6 = "0" + text6;
				}
				result = text + text2 + text3 + " " + text4 + ":" + text5 + ":" + text6;
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string FormatDateWithoutSymbols(DateTime date2Format)
		{
			string result = "";
			try
			{
				string text = date2Format.Year.ToString().Substring(2, 2);
				string text2 = date2Format.Month.ToString();
				if (text2.Length == 1)
				{
					text2 = "0" + text2;
				}
				string text3 = date2Format.Day.ToString();
				if (text3.Length == 1)
				{
					text3 = "0" + text3;
				}
				string text4 = date2Format.Hour.ToString();
				if (text4.Length == 1)
				{
					text4 = "0" + text4;
				}
				string text5 = date2Format.Minute.ToString();
				if (text5.Length == 1)
				{
					text5 = "0" + text5;
				}
				string text6 = date2Format.Second.ToString();
				if (text6.Length == 1)
				{
					text6 = "0" + text6;
				}
				result = text + text2 + text3 + text4 + text5 + text6;
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string FormatDate(DateTime date2Format, bool excludeTime)
		{
			string result = "";
			try
			{
				string str = date2Format.Year.ToString();
				string text = date2Format.Month.ToString();
				if (text.Length == 1)
				{
					text = "0" + text;
				}
				string text2 = date2Format.Day.ToString();
				if (text2.Length == 1)
				{
					text2 = "0" + text2;
				}
				if (!excludeTime)
				{
					result = FormatDate(date2Format);
					return result;
				}
				result = str + text + text2;
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string FormatDate(object date2Format)
		{
			string result = "";
			try
			{
				result = FormatDate((DateTime)date2Format);
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static string FormatDate(object date2Format, bool excludeTime)
		{
			string result = "";
			try
			{
				result = FormatDate((DateTime)date2Format, true);
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static int FormatDateDifference(string number2Format)
		{
			int result = 0;
			try
			{
				if (!number2Format.Contains("."))
				{
					return result;
				}
				result = Convert.ToInt32(number2Format.Substring(0, number2Format.IndexOf(".")));
				return result;
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
				return result;
			}
		}

		public static void Logexception(Exception ex)
		{
			LogDebugOnFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + ex.Source + "\n" + ex.Message + "\n" + ex.StackTrace);
			//LogDebugOnFile(ex.StackTrace);
		}

		public static void LogDebugOnFile(string clientRequest)
		{
			try
			{
				if (!File.Exists(DebugFileName))
				{
					File.Create(DebugFileName);
				}
				File.AppendAllText(DebugFileName, clientRequest + "\n");
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}
		public static void WriteLogOnFile(string jsonRequest)
		{
			try
			{
				if (!File.Exists(LogFilePath))
				{
					File.Create(LogFilePath);
				}
				File.AppendAllText(LogFilePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff") + "\t" + jsonRequest + "\n");
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}

		public static void LogEntryOnFile(string fileName, string clientRequest)
		{
			try
			{
				if (!File.Exists(fileName))
				{
					File.Create(fileName);
				}
				File.AppendAllText(fileName, clientRequest);
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}

		public static void SendEmailAlert(string alert)
		{
			try
			{
				//if (EmailAlertSent(alert))
				//{
				//	new DB().WriteDb("insert into [" + DB.companyDbName + "$ATM Alerts]([Date Time],[Subject],[Description],[Status])values( '" + FormatDate(DateTime.Now) + "', '" + ValidateEntry("ATM Bridge Failure".ToUpper()) + "', '" + ValidateEntry(alert) + "',0);");
				//}
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}

		public static bool EmailAlertSent(string alert)
		{
			bool result = false;
			try
			{
				//SqlDataReader sqlDataReader = new DB().ReadDb("select Description from [" + DB.companyDbName + "$ATM Alerts] where Description = '" + ValidateEntry(alert) + "' and [Status] = 0");
				//result = !sqlDataReader.HasRows;
				//sqlDataReader.Close();
				return result;
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
				return result;
			}
		}
	}
}