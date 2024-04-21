using System;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoolWebApi.Models.Identity
{
    public class CoolBlazorUserRole : IdentityUserRole<ObjectId>
    {
        [BsonId]
        public virtual ObjectId Id { get; set; }

    }
}