using System;
using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;

namespace CoolWebApi.Models.Identity
{
    public class CoolBlazorRole : MongoRole
    {
        public override ObjectId Id { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public virtual ICollection<CoolBlazorRoleClaim> RoleClaims { get; set; }


        public CoolBlazorRole(string roleName, string roleDescription) : base(roleName)
        {
            Description = roleDescription;
        }

        public CoolBlazorRole()
        {
        }
    }
}

