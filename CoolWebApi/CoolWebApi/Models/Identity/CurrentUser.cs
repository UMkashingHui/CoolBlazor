namespace CoolWebApi.Models.Identity
{
    public static class CurrentUser
    {
        private static IHttpContextAccessor _httpContextAccessor;

        private static ISession _session => _httpContextAccessor.HttpContext.Session;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string UserId
        {
            get => _session == null ? "" : _session.GetString("CurrentUser_UserId");
            set => _session.SetString("CurrentUser_UserId", !string.IsNullOrEmpty(value) ? value : "");

        }

        public static string UserName
        {
            get => _session == null ? "" : _session.GetString("CurrentUser_UserName");
            set => _session.SetString("CurrentUser_UserName", !string.IsNullOrEmpty(value) ? value : "");
        }

    }
}