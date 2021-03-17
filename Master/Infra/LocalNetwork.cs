
namespace Master.Infra
{
    public class LocalNetwork
    {
        public const string Secret = "ciOiJIUzI1NiIsInR5cCI6IeyJ1bmlxdxxxWVfbmFtZSI6IjEiLCJuYmYiOjE1NTc5Mjk4ODcsImV4cCI6MTU1fhdsjhfeuyrejhdfj73333";
        public string sqlServer { get; set; }
        public string cacheServer { get; set; }
        public string _hostSmtp { get; set; }
        public string _emailSmtp { get; set; }
        public string _passwordSmtp { get; set; }
        public string _smtpPort { get; set; }
    }
}
