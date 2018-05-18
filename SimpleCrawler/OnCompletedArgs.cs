using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCrawler
{
    public class OnCompletedArgs
    {
        public Uri Uri { get; private set; }

        public int ThreadId { get; private set; }
        public string PageSource { get;private set; }
        public long Milliseconds { get; private set; }
        public OnCompletedArgs(Uri uri, int threadId, long milliseconds, string pageSource)
        {
            Uri = uri;
            ThreadId = threadId;
            Milliseconds = milliseconds;
            PageSource = pageSource;
        }
    }
}
