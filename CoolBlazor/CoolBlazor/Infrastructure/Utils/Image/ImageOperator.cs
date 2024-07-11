using CoolBlazor.Infrastructure.Constants.Enums;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Utils.Extensions;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Utils.Image
{
    public class ImageOperator
    {
        public ImageOperator()
        {

        }
        public async Task<Stream> IBrowerFile2Stream(IBrowserFile e)
        {
            // Change IBrowerFile to Stream
            var format = "image/jpg";
            var imageFile = await e.RequestImageFileAsync(format, 400, 400);
            long maxFileSize = 1024 * 1024 * 3; // 5 MB or whatever, don't just use max int
            var readStream = imageFile.OpenReadStream(maxFileSize);
            var buf = new byte[readStream.Length];
            var ms = new MemoryStream(buf);
            await readStream.CopyToAsync(ms);
            var buffer = ms.ToArray();
            return new MemoryStream(buffer);
        }

        public async Task<string> SaveImageByStreamLocally(SaveImageDataRequest request)
        {
            var streamData = request.FileData;
            var fileName = request.FileName;
            if (streamData != null)
            {
                // Macos/Linux Only
                var folder = UploadType.ProfilePicture.ToDescriptionString().Replace('\\', '/');
                // Macos/Linux Only
                // Save image
                if (string.IsNullOrEmpty(folder)) return null;
                if (streamData.Length > 0)
                {

                    bool exists = Directory.Exists(folder);
                    if (!exists)
                        Directory.CreateDirectory(folder);
                    var relativePath = Path.Combine(folder, fileName);
                    if (System.IO.File.Exists(relativePath))
                    {
                        relativePath = NextAvailableFilename(relativePath);
                    }
                    var nextFullPath = FullPathGenerator(relativePath);
                    using (var stream = new FileStream(nextFullPath, FileMode.Create))
                    {
                        streamData.CopyTo(stream);
                    }
                    return relativePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string FullPathGenerator(string relativePath)
        {
            // Get the absolute path of image
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
        }

        private static string numberPattern = " ({0})";

        private string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!System.IO.File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);

            if (!System.IO.File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (System.IO.File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (System.IO.File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }


}