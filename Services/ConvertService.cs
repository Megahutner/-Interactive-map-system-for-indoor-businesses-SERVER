using Serilog;
using UWEServer.Entities;
using UWEServer.Responses;

namespace UWEServer.Services
{
    public static class ConvertService
    {
        public static ZoneDetailsResponse ConvertZoneToBasicData(Zone zone)
        {
            if (zone == null)
            {
                return null;
            }

            return new ZoneDetailsResponse()
            {
                Id = zone.Id,
                ZoneID = zone.ZoneId,
                Name = zone.Name,
                Height = zone.Height,
                Width = zone.Width,
                CreatedTime = zone.CreatedTime,
                UpdatedTime = zone.UpdatedTime
            };
        }

        public static CategoryResponse ConvertCategoryToBasicData(Category category)
        {
            if (category == null)
            {
                return null;
            }

            return new CategoryResponse()
            {
                Id = category.Id,
                Name = category.Name,
                CreatedTime = category.CreatedTime,
                UpdatedTime = category.UpdatedTime
            };
        }

        public static string ConverLocalDirectoryToUrl(string localDirectory, string mediaSettingLocalDirectory, string mediaSettingServerAddress)
        {
            try
            {
                if (localDirectory == null || localDirectory == "")
                {
                    return "";
                }
                var folderImages = mediaSettingLocalDirectory.Split(new char[] { '\\' });
                return $"{mediaSettingServerAddress}{folderImages[folderImages.Length - 1]}/{localDirectory}";
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "ConverLocalDirectoryToUrl");
                return "";
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> SaveFile(IFormFile file, string localDirectory, string directory, string fileName)
        {
            try
            {
                if (file == null)
                    return "";
                var returnPath = "";
                var localPath = "";
                if (file.Length > 0)
                {
                    var tmp = file.FileName.Split(new char[] { '.' });
                    returnPath = $"{directory}/{fileName}.{tmp[tmp.Length - 1]}";
                    localPath = $"{localDirectory}/{directory}";
                    if (!Directory.Exists($"{localPath}/"))
                        Directory.CreateDirectory($"{localPath}/");
                    int count = 0;
                    localPath = $"{localDirectory}/{returnPath}";
                    while (File.Exists(localPath))
                    {
                        count += 1;
                        returnPath = $"{directory}/{fileName}_{count}.{tmp[tmp.Length - 1]}";
                        localPath = $"{localDirectory}/{returnPath}";

                    }
                    using (var stream = File.Create(localPath))
                    {
                        await file.CopyToAsync(stream);
                        stream.Flush();
                    }
                }
                return returnPath;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "UploadFile fail");
                return "";
            }
        }
    }


    

}
