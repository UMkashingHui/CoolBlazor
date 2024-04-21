using System;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoolWebApi.Models.Identity;

namespace CoolWebApi.Models.Identity
{
    // public class CoolBlazorRoleClaim : IdentityRoleClaim<string>
    public class CoolBlazorRoleClaim
    {
        [BsonId]
        public virtual ObjectId Id { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Group")]
        public string Group { get; set; }

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; }

        [BsonElement("ClaimValue")]
        public string ClaimValue { get; set; }
        [BsonElement("ClaimType")]
        public string ClaimType { get; set; }

        [BsonElement("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [BsonElement("LastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [BsonElement("LastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }

        [BsonElement("RoleId")]
        public virtual ObjectId RoleId { get; set; }

        [BsonElement("Role")]
        public virtual CoolBlazorRole Role { get; set; }

        public CoolBlazorRoleClaim() : base()
        {
            Id = ObjectId.GenerateNewId();
        }

        public CoolBlazorRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}