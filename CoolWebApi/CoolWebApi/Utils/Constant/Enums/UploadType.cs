using System.ComponentModel;

namespace CoolWebApi.Utils.Constants.Enums
{
    public enum UploadType : byte
    {
        [Description(@"Images\Products")]
        Product,

        [Description(@"images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }
}