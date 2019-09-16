using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Master
{
    public class LocalNetwork
    {
        public const string Secret = "ciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1NTc5Mjk4ODcsImV4cCI6MTU1fhdsjhfeuyrejhdfj73333";

        public string mongoDB { get; set; }

        public string ensemble_usuario { get; set; }
        public string ensemble_senha { get; set; }
        public string ensemble_url { get; set; }

        public string mail_host { get; set; }
        public string mail_port { get; set; }
        public string mail_ssl { get; set; }
        public string mail_from { get; set; }
        public string mail_user { get; set; }
        public string mail_pass { get; set; }
        
        #region - (old) networks - 

        public string CacheLocation { get; set; }
        public List<string> ConfigurationHosts { get; set; }
        public List<string> PortalHosts { get; set; }

        public Hashtable hshLocalCache = new Hashtable();
        public Hashtable hshStats = new Hashtable();
        public List<string> lstStats = new List<string>();

        int idx_config = 0, count_config = 0,
            idx_portal = 0, count_portal = 0;

        public NetworkStats GetStats()
        {
            return GetStats(5)[0];
        }

        public List<NetworkStats> GetStats(int lastMinutes)
        {
            var dt = DateTime.Now;
            var ret = new List<NetworkStats>();

            for (int i = 0; i < lastMinutes; i++)
            {
                string tag = GetTimeTag(dt);
                var ns = hshStats[tag] as NetworkStats;

                if (ns == null)
                    ns = new NetworkStats();

                ret.Add ( new NetworkStats
                {
                    Date = dt.ToShortTimeString(),
                    requests = ns.requests,
                    requestsPerSecond = ns.requests > 0 ? ns.requests / 60 : 0,
                    bytes = ns.bytes,
                    kbytesPerSecond = ns.bytes > 0 ? (ns.bytes / 60) / 1024 : 0,
                    microCached = ns.microCached,
                    cachedRequests = ns.cachedRequests,
                    cachePct = ns.requests > 0 ? (ns.cachedRequests * 100) / ns.requests : 0
                });

                dt = dt.AddMinutes(-1);
            }

            return ret;
        }

        public string GetTimeTag(DateTime dt)
        {
            return dt.Hour.ToString() + dt.Minute.ToString();
        }

        public void UpdateRequestStat(int bytes, bool cached, bool micro)
        {
            string tag = GetTimeTag(DateTime.Now);

            if (!(hshStats[tag] is NetworkStats ns))
            {
                ns = new NetworkStats();
                hshStats[tag] = ns;

                lstStats.Add(tag);

                int maxMinutesStatistics = 5;

                if (lstStats.Count > maxMinutesStatistics)
                {
                    var indexEl = lstStats.Count - maxMinutesStatistics;

                    for (int i = 0; i < indexEl; i++)
                        hshStats[lstStats[i]] = null;

                    lstStats.RemoveRange(0, indexEl);
                }
            }

            if (cached)
                ns.cachedRequests++;

            if (micro)
                ns.microCached++;
            
            ns.requests++;

            ns.bytes += bytes;
        }

        public string GetHost(LocalNetworkTypes _type)
        {
            lock (this)
            {
                List<string> lst = null;
                int idx = 0, count = 0;

                switch (_type)
                {
                    case LocalNetworkTypes.Portal:
                        lst = PortalHosts;
                        idx = idx_portal;
                        if (count_portal == 0) count_portal = lst.Count();
                        count = count_portal;
                        break;
                }

                return ResolveHost(lst, ref idx, count);
            }
        }

        string ResolveHost(List<string> lst, ref int idx, int count)
        {
            if (count == 1) return lst[0];
            else
            {
                if (++idx >= count) idx = 0;
                return ConfigurationHosts[idx];
            }
        }

        #endregion
    }

    #region - code (old networking model) - 

    public enum LocalNetworkTypes
    {
        Portal = 1,
    }

    public class NetworkStats
    {
        public string Date;

        public int requests = 0, cachedRequests = 0, microCached = 0, bytes = 0, cachePct = 0;
        public int requestsPerSecond = 0, kbytesPerSecond;
    }

    #endregion
}
