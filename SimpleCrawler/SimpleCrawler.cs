using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCrawler
{
    public class SimpleCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;
        public event EventHandler<OnCompletedArgs> OnCompleted;
        public event EventHandler<Exception> OnError;

        public CookieContainer CookieContainer { get; set; }
        public async Task<string> Start(Uri uri, WebProxy proxy)
        {
            return await Task.Run(() => {
                var pageSource = string.Empty;
                try{
                    if (this.OnStart != null)
                    {
                        var watch = new Stopwatch();
                        watch.Start();
                        var request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Accept = "*/*";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.AllowAutoRedirect = false;
                        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.119 Safari/537.36";
                        request.Timeout = 5000;
                        request.KeepAlive = true;
                        request.Method = "GET";
                        if (proxy != null)
                        {
                            request.Proxy = proxy;
                        }
                        request.CookieContainer = this.CookieContainer;
                        request.ServicePoint.ConnectionLimit = int.MaxValue;
                        var response = (HttpWebResponse)request.GetResponse();
                        foreach (Cookie cookie in response.Cookies)
                        {
                            this.CookieContainer.Add(cookie);
                        }
                        var stream = response.GetResponseStream();
                        var reader = new StreamReader(stream);
                        pageSource = reader.ReadToEnd();
                        watch.Stop();
                        var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        var milliseconds = watch.ElapsedMilliseconds;
                        reader.Close();
                        stream.Close();
                        request.Abort();
                        response.Close();
                        if (this.OnCompleted != null)
                        {
                            this.OnCompleted(this, new OnCompletedArgs(uri, threadId, milliseconds, pageSource));
                        }
                    }
                }
                catch(Exception ex){
                    if(this.OnError!=null)
                        OnError(this, ex);
                }
                return pageSource;
            });
        }
    }
}
