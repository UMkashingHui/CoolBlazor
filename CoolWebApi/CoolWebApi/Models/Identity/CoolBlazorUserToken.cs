using System;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoolWebApi.Models.Identity
{
    public class CoolBlazorUserToken : IdentityUserToken<ObjectId>
    {
        [BsonId]
        public virtual ObjectId Id { get; set; }

    }
}