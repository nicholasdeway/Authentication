using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autenticação.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}