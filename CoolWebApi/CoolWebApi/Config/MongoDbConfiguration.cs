using System;
namespace CoolWebApi.Config
{
	public class MongoDbConfiguration
	{
		public string UsersCollectionName { get; init; }
		public string RolesCollectionName { get; init; }
		public string RoleClaimsCollectionName { get; init; }
		public string UserClaimsCollectionName { get; init; }
		public string ConnectionString { get; init; }
		public string DatabaseName { get; init; }
	}
}

