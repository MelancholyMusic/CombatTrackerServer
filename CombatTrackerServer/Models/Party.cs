using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CombatTrackerServer.Models
{
	public class Party
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
	}
}