using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public static class ImageHelper
    {
        public const string _TARGET_FOLDER_ROOT = "/Upload/";
        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        public static string SaveImage(string targetFolder, string base64String)
        {
            return SaveImage(targetFolder, Base64ToImage(base64String));
        }
        public static string SaveImage(string targetFolder, Image image, string fileName = "")
        {
            if (!System.IO.Directory.Exists(targetFolder))
            {
                System.IO.Directory.CreateDirectory(targetFolder);
            }
            if (image != null)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = RandomHelper.RandomString(15, false) + ".jpg";
                }
                string pathString = System.IO.Path.Combine(targetFolder, fileName);
                image.Save(pathString, System.Drawing.Imaging.ImageFormat.Jpeg);
                return pathString;
            }
            return "";
        }
    }
}
