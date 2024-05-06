// using CoolWebApi.Config;

// namespace CoolWebApi.Utils.Middleware
// {
//     public class MyMiddleware
//     {
//         private readonly RequestDelegate _next;
//         private readonly IUserPermisionConfig _config;


//         public MyMiddleware(RequestDelegate next, IUserPermisionConfig config)
//         {
//             _next = next;

//             _config = config;
//         }

//         public async Task InvokeAsync(HttpContext context)
//         {

//             _config.SetUserPermissionsSession(5);

//             await _next(context);
//         }
//     }
// }

