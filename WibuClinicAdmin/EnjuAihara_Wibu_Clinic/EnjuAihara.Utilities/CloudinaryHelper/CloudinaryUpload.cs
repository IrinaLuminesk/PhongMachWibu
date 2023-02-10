using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace EnjuAihara.Utilities.CloudinaryHelper
{
    public class CloudinaryUpload
    {
        public static string[] fileExtension = { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
        public static string Upload(HttpPostedFileBase file)
        {
            if (file == null)
                return "Lỗi không có file";
            string extension = Path.GetExtension(file.FileName);
            if (CheckFileExtension(extension) == false)
                return "File ảnh không đúng định dạng";
            Cloudinary cloudinary = new Cloudinary(GetCloudinaryConfig());
            cloudinary.Api.Secure = true;
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.InputStream),
                Transformation = new Transformation().Width(800).Height(800)
            };
            try
            {
                var UploadResult = cloudinary.Upload(uploadParams);
                string UploadUrl = UploadResult.SecureUrl.ToString();
                return UploadUrl;

            }
            catch (Exception ex)
            {
                string error = string.Format("Đã xảy ra lỗi trong quá trình lưu ảnh\n{0}", ex.Message.ToString());
                return error;
            }
        }


        public static CloudinaryDotNet.Account GetCloudinaryConfig()
        {
            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account()
            {
                Cloud = WebConfigurationManager.AppSettings["Cloud_name"],
                ApiKey = WebConfigurationManager.AppSettings["api_key"],
                ApiSecret = WebConfigurationManager.AppSettings["api_secret"],
            };
            return account;
        }


        public static bool CheckFileExtension(string extension)
        {
            if (fileExtension.Contains(extension))
                return true;
            return false;
        }


   

    }

}
