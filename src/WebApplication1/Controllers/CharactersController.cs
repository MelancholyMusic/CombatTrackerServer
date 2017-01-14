using CombatTrackerServer.Models;
using CombatTrackerServer.Models.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace CombatTrackerServer.Controllers
{
	[Authorize]
    [Produces("application/json")]
    public class CharactersController : Controller
    {
		private readonly MongoDBDataAccess _mongo;
		private readonly UserManager<ApplicationUser> _userManager;

		public CharactersController(MongoDBDataAccess mongo, UserManager<ApplicationUser> userManager)
		{
			_mongo = mongo;
			_userManager = userManager;
		}

		[HttpGet("api/characters/")]
		public async Task<string> Characters()
		{
			ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			return await _mongo.GetCharacters(user.Id);
		}

		[HttpGet("api/characters/{id}")]
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