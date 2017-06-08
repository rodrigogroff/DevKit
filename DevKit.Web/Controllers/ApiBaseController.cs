﻿using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : ApiController
	{
        [NonAction]
		public LoginInfo GetLoginInfo()
        {
            var strRequest = Request.GetQueryStringValue("login");

            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);

            return new LoginInfo { idUser = 0 };
        }
    }
}