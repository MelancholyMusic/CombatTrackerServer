using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CombatTrackerServer.Models
{
	public class PartyModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public bool Locked { get; set; }
		public int JoinedPlayers { get; set; }
		public int MaxPlayers { get; set; }
	}
}