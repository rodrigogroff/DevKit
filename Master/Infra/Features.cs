
using System;

namespace Master
{
    public class FeatureState
    {
        public bool Execute { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CacheAutomaticRecycle
    {
        Critical = 0,
        Highest = 1,
        High = 2 ,
        Normal = 3,
        Low = 4,
        Lowest = 5
    }

    public class CachedLocalObject
    {
        public DateTime expires;
        public string cachedContent;
    }

    public class Features
    {
        public bool Cache { get; set; }
        public bool MicroCache { get; set; }        
        public FeatureState CreateAccount { get; set; }
        public FeatureState Authenticate { get; set; }
        public FeatureState CreateCategory { get; set; }
        public FeatureState CreateSubCategory { get; set; }
        public FeatureState CreateProduct { get; set; }
    }
}
