using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace fuzzer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			JObject obj = new JObject ();
			obj ["method"] = "create";
			obj ["username"] = "fdsa'";
			obj ["password"] = "password";
		
			byte[] data = System.Text.Encoding.ASCII.GetBytes ("JSON=" + obj.ToString ());

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create ("http://127.0.0.1:8080/Vulnerable.ashx");
			req.Method = "POST";
			req.ContentLength = data.Length;
			req.ContentType = "application/x-www-form-urlencoded";

			req.GetRequestStream ().Write (data, 0, data.Length);

			string resp = string.Empty;
			WebResponse response;
			try {
				response = req.GetResponse ();
			} catch (WebException ex) {

				using (StreamReader rdr = new StreamReader(ex.Response.GetResponseStream()))
					resp = rdr.ReadToEnd();

				if (resp.Contains("syntax error"))
					Console.WriteLine("SQLi vector found");
				return;
			}

			using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
				resp = rdr.ReadToEnd();

			Console.WriteLine(resp);
		}
	}
}
