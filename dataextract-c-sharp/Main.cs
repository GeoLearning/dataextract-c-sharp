using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace dataextractcsharp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://pubservices.geolearning.com/dataextraction/geoproductmanagement/courses");
			request.Credentials = new NetworkCredential("bcarlson@geolearning.com", "Password1!");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			byte[] requestParameters = UTF8Encoding.UTF8.GetBytes("header_required=false");
			request.ContentLength = requestParameters.Length;
			Stream s = request.GetRequestStream();
			if(s != null) {
				try {
					s.Write(requestParameters, 0, requestParameters.Length);
				}finally {
					s.Close();
				}
			}
			
			HttpWebResponse response = (HttpWebResponse) request.GetResponse();
			if(response.StatusCode == HttpStatusCode.Created) {
				string fileLocation = response.Headers["Location"];
				HttpWebRequest downloadRequest = (HttpWebRequest) WebRequest.Create(fileLocation);
				downloadRequest.Credentials = request.Credentials;
				downloadRequest.Method = "HEAD";
				HttpWebResponse downloadResponse = (HttpWebResponse) downloadRequest.GetResponse();
				if(downloadResponse.StatusCode == HttpStatusCode.OK) {
					WebClient wc = new WebClient();
					wc.Credentials = request.Credentials;
					wc.DownloadFile(fileLocation, "some-file.csv");
				}
			} else {
				throw new Exception("Unable to create file! Status code: " + response.StatusCode);
			}
			
		}
	}
}
