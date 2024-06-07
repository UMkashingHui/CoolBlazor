﻿using CoolWebApi.Models.Requests.File;
using CoolWebApi.Utils.Extensions;
using CoolWebApi.Utils.Wrapper;

namespace CoolWebApi.Services.FileOperation.impl
{
    public class UploadService : IUploadService
    {
        public async Task<string> UploadAsync(UploadRequest request)
        {
            if (request.Data == null || string.IsNullOrEmpty(request.PathToSave)) return null;
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                // Macos Test Only
                var folder = request.UploadType.ToDescriptionString().Replace('\\', '/');
                bool exists = Directory.Exists(request.PathToSave);
                if (!exists)
                    Directory.CreateDirectory(request.PathToSave);
                var fileName = request.FileName.Trim('"');
                var fullPath = Path.Combine(request.PathToSave, fileName);
                var dbPath = Path.Combine(folder, fileName);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
                if (File.Exists(fullPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string numberPattern = " ({0})";

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
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

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}