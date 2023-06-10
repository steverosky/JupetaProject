using Amazon.S3;
using Amazon.S3.Model;

namespace Jupeta.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAmazonS3 _s3;
        private const string BucketName = "jupetaprojects3";

        public FileService(IWebHostEnvironment env, IAmazonS3 s3)
        {
            _env = env;
            _s3 = s3;
        }

        //Upload image to aws s3 bucket
        public async Task<PutObjectResponse> UploadImage(Guid id, IFormFile ImageFile)
        {
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = BucketName,
                Key = $"product_images/{id}",
                ContentType = ImageFile.ContentType,
                InputStream = ImageFile.OpenReadStream(),
                Metadata =
                {
                    ["x-amz-meta-originalname"] = ImageFile.FileName,
                    ["x-amz-meta-extension"] = Path.GetExtension(ImageFile.FileName)
                }
            };
            return await _s3.PutObjectAsync(putObjectRequest);
        }

        //Get image from aws s3 
        public async Task<GetObjectResponse?> GetImage(Guid id)
        {
            try
            {
                var getObjectRequest = new GetObjectRequest
                {

                    BucketName = BucketName,
                    Key = $"images/{id}"
                };

                return await _s3.GetObjectAsync(getObjectRequest);
            }
            catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
            {
                return null;
            }

        }


        //delete image from s3
        public async Task<DeleteObjectResponse> DeleteImage(Guid id)
        {
            var getDeleteRequest = new DeleteObjectRequest
            {

                BucketName = BucketName,
                Key = $"images/{id}"
            };

            return await _s3.DeleteObjectAsync(getDeleteRequest);
        }


        //save image uploaded to local storage
        //public Tuple<int, string> SaveImage(IFormFile ImageFile)
        //{
        //    try
        //    {
        //        var contentPath = _env.ContentRootPath;
        //        var path = Path.Combine(contentPath, "Uploads");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        // Check the allowed extenstions
        //        var ext = Path.GetExtension(ImageFile.FileName);
        //        var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
        //        if (!allowedExtensions.Contains(ext))
        //        {
        //            string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
        //            return new Tuple<int, string>(0, msg);
        //        }
        //        string uniqueString = Guid.NewGuid().ToString();
        //        // we are trying to create a unique filename here
        //        var newFileName = uniqueString + ext;
        //        var fileWithPath = Path.Combine(path, newFileName);
        //        var stream = new FileStream(fileWithPath, FileMode.Create);
        //        ImageFile.CopyTo(stream);
        //        stream.Close();
        //        return new Tuple<int, string>(1, newFileName);
        //    }
        //    catch (Exception)
        //    {
        //        return new Tuple<int, string>(0, "Error has occured");
        //    }
        //}

        //public bool DeleteImage(string imageFileName)
        //{
        //    try
        //    {
        //        var wwwPath = _env.WebRootPath;
        //        var path = Path.Combine(wwwPath, "Uploads\\", imageFileName);
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}


    }
}
