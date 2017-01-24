using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CombatTrackerServer.Models.MongoDB
{
	public class MongoDBDataAccess
	{
		MongoClient _client;
		IMongoDatabase _db;
		IMongoCollection<BsonDocument> _collection;

		public MongoDBDataAccess()
		{
			_client = new MongoClient("mongodb://ec2-54-201-242-87.us-west-2.compute.amazonaws.com:27017");
			_db = _client.GetDatabase("CTData");
			_collection = _db.GetCollection<BsonDocument>("PlayerData");
		}

		public async Task<string> GetCharacters(string id)
		{
			FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("userId", id);
			PlayerDataModel pd = new PlayerDataModel(await _collection.Find(filter).FirstAsync());
			return pd.Characters.ToJson();
		}

		public async Task<string> GetCharacter(string id, int characterId)
		{
			FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("userId", id);
			PlayerDataModel pd = new PlayerDataModel(await _collection.Find(filter).FirstAsync());

			if(pd.Characters.Count > characterId)
				return pd.Characters[characterId].ToJson();

			return "";
		}

		public async Task<string> GetPlayerData(string id)
		{
			FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("userId", id);
			BsonDocument pd = await _collection.Find(filter).FirstAsync();
			pd.Remove("_id");
			pd.Remove("userId");
			return pd.ToJson();
		}

		public async Task<ReplaceOneResult> ReplacePlayerData(string id, string replacementData)
		{
			FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("userId", id);
			return await _collection.ReplaceOneAsync(filter, replacementData.ToBsonDocument());
		}

		public async Task CreatePlayerData(PlayerDataModel pd)
		{
			await _collection.InsertOneAsync(pd.ToBsonDocument());
		}

		//public async Task UpdatePlayerData(ObjectId id, PlayerDataModel pd)
		//{
		//	pd.Id = id;
		//	FilterDefinition<PlayerDataModel> filter = Builders<PlayerDataModel>.Filter.Where(x => x.Id == pd.Id);
		//	await _collection.ReplaceOneAsync(filter, pd);
		//}

		//public async Task RemovePlayerData(ObjectId id)
		//{
		//	FilterDefinition<PlayerDataModel> filter = Builders<PlayerDataModel>.Filter.Where(x => x.Id == id);
		//	await _collection.DeleteOneAsync(filter);
		//}
	}
}
