using System.ComponentModel;

namespace CoolBlazor.Infrastructure.Constants.Enums
{
    public enum UploadType : byte
    {
        [Description(@"Images\Products")]
        Product,

        [Description(@"wwwroot\images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }
}