using System.ComponentModel.DataAnnotations;

namespace CombatTrackerServer.Models.DAOModels
{
    public class PlayerDataReplaceModel
    {
		[Required]
		public string PlayerData { get; set; }
    }
}
