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
		PartyModel[] parties = new PartyModel[]
		{
			new PartyModel { Id = 1, Name = "Gurgle's Grumps", Category = "Groceries", Locked = false, JoinedPlayers = 3, MaxPlayers = 8 },
			new PartyModel { Id = 2, Name = "Lost Crusaders", Category = "Toys", Locked = true, JoinedPlayers = 1, MaxPlayers = 4 },
			new PartyModel { Id = 3, Name = "The Final Fighters", Category = "Hardware", Locked = false, JoinedPlayers = 5, MaxPlayers = 6 }
		};

		public IEnumerable<PartyModel> GetAllParties()
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
