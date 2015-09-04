using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CombatTrackerServer.Models;

namespace CombatTrackerServer.Controllers
{
    public class PartiesController : ApiController
    {
		Party[] parties = new Party[]
		{
			new Party { Id = 1, Name = "Tomato Soup", Category = "Groceries" },
			new Party { Id = 2, Name = "Yo-yo", Category = "Toys" },
			new Party { Id = 3, Name = "Hammer", Category = "Hardware" }
		};

		public IEnumerable<Party> GetAllParties()
		{
			return parties;
		}

		public IHttpActionResult GetPartyById(int id)
		{
			var party = parties.FirstOrDefault((p) => p.Id == id);
			if(party == null)
			{
				return NotFound();
			}
			return Ok(party);
		}
	}
}
