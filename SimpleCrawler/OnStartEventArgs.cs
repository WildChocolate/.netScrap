using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCrawler
{
    public class OnStartEventArgs
    {
        public Uri Uri
        {
            get;
            set;
        }
        public OnStartEventArgs(Uri uri)
        {
            Uri = uri;
        }
    }
}
