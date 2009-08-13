using System;
using System.IO;
using System.Net;
using System.Text;

namespace dataextractcsharp
{
	class MainClass
	{
	    private static ICredentials credentials = null;

		public static void Main (string[] args)
		{
		    string remoteUrl = args[0];
		    credentials = new NetworkCredential(args[1], args[2]);
			HttpWebResponse response = CreateRemoteFile(remoteUrl);
            if(response.StatusCode == HttpStatusCode.Created) {
                string fileLocation = response.Headers["Location"];
				string downloadUrl = GetFileUrl(fileLocation);
                DownloadFile(downloadUrl);
			} 
		}

        private static HttpWebResponse CreateRemoteFile(string remoteUrl){
	        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(remoteUrl);
            request.Credentials = credentials;
	        request.Method = "POST";
	        request.ContentType = "application/x-www-form-urlencoded";
	        byte[] requestParameters = Encoding.UTF8.GetBytes("header_required=false");
	        request.ContentLength = requestParameters.Length;
	        Stream s = request.GetRequestStream();
	        if(s != null){
	            try{
	                s.Write(requestParameters, 0, requestParameters.Length);
	            }finally{
	                s.Close();
	            }
	        }
            return (HttpWebResponse)request.GetResponse();
	    }

	    private static string GetFileUrl(string fileLocation){
	        HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(fileLocation);
	        downloadRequest.Credentials = credentials;
	        downloadRequest.Method = "HEAD";
	        downloadRequest.AllowAutoRedirect = false;
	        HttpWebResponse downloadResponse = (HttpWebResponse)downloadRequest.GetResponse();
	        HttpStatusCode status = downloadResponse.StatusCode;
	        while(status == HttpStatusCode.Found){
	            return GetFileUrl(downloadResponse.Headers["Location"]);
	        }
            if(status != HttpStatusCode.OK){
                throw new ApplicationException("Unable to request file URL.");
            }
	        return fileLocation;
	    }

	    private static void DownloadFile(string downloadUrl){
	        WebClient wc = new WebClient();
	        wc.Credentials = credentials;
            wc.DownloadFile(downloadUrl, "some-file.csv");
	    }
	}
}
