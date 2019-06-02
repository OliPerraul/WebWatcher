using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebWatcher
{
    public class URL
    {
        public string title;
        public string url;
        public string host;
        public long time;
        public string browser;
        public URL(string u, string t, long d, string b, string h)
        {
            this.url = u;
            this.title = t;
            this.time = d;
            this.browser = b;
            this.host = h;
        }

        public URL(string u, string t, string b, string h)
        {
            this.url = u;
            this.title = t;
            this.browser = b;
            this.host = h;
        }
    }
}