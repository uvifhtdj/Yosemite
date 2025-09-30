using System;
using System.IO;
using System.Net;
using System.Text;

namespace wrapper
{
    public class SimpleHttp
    {
        class WebClient2 : WebClient
        {
            public bool AllowAutoRedirect { get; set; }

            public WebClient2()
            {
                this.AllowAutoRedirect = true;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                HttpWebRequest req = request as HttpWebRequest;
                req.AllowAutoRedirect = this.AllowAutoRedirect;
                return base.GetWebResponse(request);
            }
        }
        public string GetHtml(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            return webClient.DownloadString(url);
        }

        public void DownloadFile(string url, string filenames)
        {
            var webClient = new WebClient2();
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            webClient.DownloadFile(url,filenames);
        }

        public string GetHtml(string url, Encoding encoding)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            byte[] array = webClient.DownloadData(url);
            return encoding.GetString(array);
        }

        public string GetLocation(string url)
        {
            WebClient2 webClient = new WebClient2();
            webClient.AllowAutoRedirect = false;
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            byte[] array = webClient.DownloadData(url);
            return webClient.ResponseHeaders["Location"].ToString();
        }

        public string PostJson(string url, string json)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            webClient.Headers["Content-Type"] = "application/json";
            byte[] array = webClient.UploadData(url, Encoding.UTF8.GetBytes(json));
            return Encoding.UTF8.GetString(array);
        }

        public long GetContentLength(string url)
        {
            long num;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
                httpWebRequest.Method = "HEAD";
                httpWebRequest.Timeout = 5000;
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        num = httpWebResponse.ContentLength;
                    }
                    else
                    {
                        num = -1L;
                    }
                }
            }
            catch
            {
                num = -1L;
            }
            return num;
        }

        public void DownloadFileWithProgress(string downloadUrl, string localFilePath, Action<long, long> updateProgress)
        {
            string text = localFilePath + ".tmp";
            long num = 0L;
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }
            if (File.Exists(text))
            {
                File.Delete(text);
            }
            long contentLength = this.GetContentLength(downloadUrl);
            using (FileStream fileStream = new FileStream(text, FileMode.Create))
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
                using (Stream responseStream = httpWebRequest.GetResponse().GetResponseStream())
                {
                    responseStream.CopyTo(fileStream);
                }
            }
            File.Copy(text, localFilePath, true);
            try
            {
                File.Delete(text);
            }
            catch
            {
            }
        }

        private const string DEFAULT_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
    }
}
