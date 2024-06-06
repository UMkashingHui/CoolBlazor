using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using CoolWebApi.Config;
using CoolWebApi.Models.Identity;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Entities.Contracts;

namespace CoolWebApi.Utils.Contexts
{
    public class MongoIdentityDbContext
    {
        public readonly IMongoDatabase _db;
        private readonly string _usersCollectionName;
        private readonly string _roleClaimsCollectionName;
        private readonly string _userClaimsCollectionName;
        private readonly IOptions<MongoDbConfiguration> _config;
        private readonly ILogger<MongoIdentityDbContext> _logger;
        private readonly IStringLocalizer<MongoIdentityDbContext> _localizer;

        public MongoIdentityDbContext(IOptions<MongoDbConfiguration> config, ILogger<MongoIdentityDbContext> logger, IStringLocalizer<MongoIdentityDbContext> localizer)
        {
            _config = config;
            _logger = logger;
            _localizer = localizer;
            var client = new MongoClient(_config.Value.ConnectionString);
            _db = client.GetDatabase(_config.Value.DatabaseName);
            _usersCollectionName = _config.Value.UsersCollectionName;
            _roleClaimsCollectionName = _config.Value.RoleClaimsCollectionName;
            _userClaimsCollectionName = _config.Value.UserClaimsCollectionName;
        }

        public IMongoCollection<CoolBlazorRoleClaim> RoleClaims =>
        _db.GetCollection<CoolBlazorRoleClaim>(_roleClaimsCollectionName);

        public IMongoCollection<IdentityUserClaim<string>> UserClaims =>
        _db.GetCollection<IdentityUserClaim<string>>(_userClaimsCollectionName);
    }
}