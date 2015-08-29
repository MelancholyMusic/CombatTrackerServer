using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace CombatTrackerServer.Controllers
{
    public class Class1 : ApiController
    {
        [HttpGet]
        [Route("activities")]
        public HttpResponseMessage GetActivity()
        {
            // Event doesn't exist, can't add activity.
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}