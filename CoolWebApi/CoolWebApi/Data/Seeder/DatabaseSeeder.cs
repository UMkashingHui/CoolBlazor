using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using CoolWebApi.Models.Identity;
using CoolWebApi.Utils.Constants.Role;
using CoolWebApi.Utils.Constants.Identity;
using CoolWebApi.Utils.Constants.Permission;
using CoolWebApi.Utils.Extensions;
using MongoDB.Driver;
using CoolWebApi.Data.Contexts;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Services.Identity.impl;

namespace CoolWebApi.Data.Seeder
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly MongoIdentityDbContext _dbContext;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localizer;
        private readonly UserManager<CoolBlazorUser> _userManager;
        private readonly RoleManager<CoolBlazorRole> _roleManager;

        public DatabaseSeeder(
            UserManager<CoolBlazorUser> userManager,
            RoleManager<CoolBlazorRole> roleManager,
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localizer,
            MongoIdentityDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _localizer = localizer;
            _dbContext = dbContext;
        }

        public void Initialize()
        {
            AddAdministrator();
            AddBasicUser();
            InitializeCollection();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new CoolBlazorRole(RoleConstants.AdministratorRole, _localizer["Administrator role with full permissions"]);
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                }
                // Check if User Exists
                var superUser = new CoolBlazorUser
                {
                    FirstName = "SuperUser",
                    LastName = "SuperUser",
                    Email = "SuperUser@163.com",
                    UserName = "SuperUser",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                    // UpdateClaimsAsync(superUser.Id.ToString());
                }
                // Add all Permissions
                foreach (var permission in Permissions.GetRegisteredPermissions())
                {
                    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                }
            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var basicRole = new CoolBlazorRole(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                    _logger.LogInformation(_localizer["Seeded Basic Role."]);
                }
                //Check if User Exists
                var basicUser = new CoolBlazorUser
                {
                    FirstName = "BasicUser",
                    LastName = "BasicUser",
                    Email = "basicuser@163.com",
                    UserName = "BasicUser",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                    _logger.LogInformation(_localizer["Seeded User with Basic Role."]);
                }
            }).GetAwaiter().GetResult();
        }

        public async void InitializeCollection()
        {
            await CreateCollection("RoleClaims");
            await CreateCollection("UserClaims");

            var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
            // Add all Permissions
            foreach (var permission in Permissions.GetRegisteredPermissions())
            {
                var filter = Builders<CoolBlazorRoleClaim>.Filter.Eq(r => r.Description, permission);
                if (_dbContext.RoleClaims.Find(filter).Count() == 0)
                {
                    var roleClaim = new CoolBlazorRoleClaim();
                    roleClaim.CreatedOn = DateTime.Now;
                    roleClaim.Description = permission;
                    roleClaim.Role = adminRoleInDb;
                    roleClaim.CreatedBy = "Administrator";
                    roleClaim.ClaimValue = permission;
                    roleClaim.ClaimValue = "Permission";
                    _dbContext.RoleClaims.InsertOne(roleClaim);
                }
            }
        }

        private async Task CreateCollection(string collectionName)
        {
            if (!CollectionExists(_dbContext._db, collectionName))
            {
                await _dbContext._db.CreateCollectionAsync(collectionName, new CreateCollectionOptions
                {
                    Capped = false
                    // MaxSize = 1024,
                    // MaxDocuments = 10,
                });
                _logger.LogInformation(_localizer[collectionName + "collection created!"]);
                Console.WriteLine("RoleClaims collection created!");
            }
        }

        private bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            return database.ListCollectionNames().ToList().Contains(collectionName);
        }

        private async void UpdateClaimsAsync(string userId)
        {
            CoolBlazorUser user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                CoolBlazorRole CoolBlazorRole = await _roleManager.FindByNameAsync(role);
                var roleClaims = _roleManager.GetClaimsAsync(CoolBlazorRole);
                await _userManager.AddClaimsAsync(user, roleClaims.Result);
            }
        }


    }
}