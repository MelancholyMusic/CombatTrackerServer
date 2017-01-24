using CombatTrackerServer.Models;
using CombatTrackerServer.Models.DAOModels;
using CombatTrackerServer.Models.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace CombatTrackerServer.Controllers
{
	[Authorize]
    [Produces("application/json")]
    public class PlayerDataController : Controller
    {
		private readonly MongoDBDataAccess _mongo;
		private readonly UserManager<ApplicationUser> _userManager;

		public PlayerDataController(MongoDBDataAccess mongo, UserManager<ApplicationUser> userManager)
		{
			_mongo = mongo;
			_userManager = userManager;
		}

		[HttpGet("api/playerdata")]
		public async Task<string> PlayerData()
		{
			ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			return await _mongo.GetPlayerData(user.Id);
		}

		[HttpPost("api/playerdata")]
		public async Task<IActionResult> PlayerData(PlayerDataReplaceModel model)
		{
			if(ModelState.IsValid)
			{
				ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
				ReplaceOneResult result = await _mongo.ReplacePlayerData(user.Id, "");

				if(result.IsAcknowledged)
				{
					return Ok();
				}
				else
				{
					return BadRequest();
				}
			}
			else
			{
				return BadRequest(ModelState.Values.ElementAt(0).Errors[0].ErrorMessage);
			}
		}

		[HttpGet("api/playerdata/characters/")]
		public async Task<string> Characters()
		{
			ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			return await _mongo.GetCharacters(user.Id);
		}

		[HttpGet("api/playerdata/characters/{id}")]
		public async Task<string> Character(int id)
		{
			ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			return await _mongo.GetCharacter(user.Id, id);
		}

		//[HttpPost]
		//public async Task<IActionResult> Create(PlayerDataModel pd)
		//{
		//	if(ModelState.IsValid)
		//	{
		//		await _mongo.CreatePlayerData(pd);
		//		return Ok();
		//	}

		//	return BadRequest("Malformed request");
		//}

		//[HttpPut("{id:length(24)}")]
		//public async Task<IActionResult> Update(string id, PlayerDataModel pd)
		//{
		//	ObjectId recId = new ObjectId(id);
		//	if(await _mongo.GetPlayerData(recId) == null)
		//		return NotFound();

		//	await _mongo.UpdatePlayerData(recId, pd);
		//	return Ok();
		//}

		//[HttpDelete("{id:length(24)}")]
		//public async Task<IActionResult> Delete(string id)
		//{
		//	ObjectId recId = new ObjectId(id);
		//	if(await _mongo.GetPlayerData(recId) == null)
		//		return NotFound();

		//	await _mongo.RemovePlayerData(recId);

		//	return Ok();
		//}
	}
}