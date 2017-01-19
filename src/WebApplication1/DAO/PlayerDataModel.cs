using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CombatTrackerServer.Models
{
	public class PlayerDataModel
	{
		public ObjectId Id { get; set; }

		[BsonElement("userId")]
		public string UserId { get; set; }

		[BsonElement("characters")]
		public BsonArray Characters { get; set; }

		public PlayerDataModel()
		{
			Characters = new BsonArray();
		}

		public PlayerDataModel(BsonDocument doc)
		{
			UserId = doc["userId"].AsString;
			Characters = doc["characters"].AsBsonArray;
		}
	}
}
