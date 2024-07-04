﻿namespace CoolBlazor.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = "api/identity/account/register";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
        public static string UpdateProfilePicture = "api/identity/account/profile-picture";

        public static string GetProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }

        // public static string UpdateProfilePicture(string userId)
        // {
        //     return $"api/identity/account/profile-picture/{userId}";
        // }
    }
}