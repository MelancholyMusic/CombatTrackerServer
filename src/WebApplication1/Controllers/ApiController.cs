using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CombatTrackerServer.Controllers
{
	[Authorize]
    public class ApiController : Controller
    {
		//
		// GET: /api/authtest
		public IActionResult AuthTest()
		{
			return Ok("You're authorized!");
		}
	}
}
