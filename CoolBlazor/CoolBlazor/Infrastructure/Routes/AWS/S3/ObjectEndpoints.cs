namespace CoolBlazor.Infrastructure.Routes.AWS.S3
{
    public static class ObjectEndpoints
    {
        public static string GetAll = "api/aws/s3/object/";
        public static string Upload = "api/aws/s3/object/upload/";
        public static string Delete = "api/aws/s3/object/";
        public static string GetByKey(string bucketName, string key)
        {
            return $"api/identity/user/bucketName={bucketName}&key={key}";
        }
    }
}