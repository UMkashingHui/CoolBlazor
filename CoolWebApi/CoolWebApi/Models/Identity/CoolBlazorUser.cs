using System;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoolWebApi.Utils.Entities.Contracts;

namespace CoolWebApi.Models.Identity
{
    public class CoolBlazorUser : MongoUser, IAuditableEntity<ObjectId>
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsActive { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}

