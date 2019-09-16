using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Api.Login
{
    public class LoginInformation
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string senha { get; set; }
    }
}
