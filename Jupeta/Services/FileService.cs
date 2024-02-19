﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Jupeta.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAmazonS3 _s3;
        private readonly string BucketName;
        //private string keyName = DateTime.Now.ToString() + ".png";

        public FileService(IWebHostEnvironment env, IAmazonS3 s3)
        {
            _env = env;
            _s3 = s3;

            var region = RegionEndpoint.USEast1;
            BucketName = DotNetEnv.Env.GetString("S3_BUCKET_NAME");
            var accessKey = DotNetEnv.Env.GetString("AWS_ACCESS_KEY_ID");
            var secretKey = DotNetEnv.Env.GetString("AWS_SECRET_ACCESS_KEY");

            if (string.IsNullOrWhiteSpace(BucketName) || string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("AWS configuration values are missing or invalid.");
            }

            // Create AWS credentials using the access key and secret key
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            var s3Config = new AmazonS3Config()
            {
                RegionEndpoint = region
            };

            _s3 = new AmazonS3Client(credentials, s3Config);
        }



        //Upload image to aws s3 bucket
        public async Task<PutObjectResponse> UploadImage(Guid id, IFormFile ImageFile)
        {
            try
            {
                var putObjectRequest = new PutObjectRequest()
                {
                    BucketName = BucketName,
                    Key = $"product_images/{id}.png",
                    ContentType = ImageFile.ContentType,
                    InputStream = ImageFile.OpenReadStream(),
                    CannedACL = S3CannedACL.PublicRead,
                    Metadata =
                {
                    ["x-amz-meta-originalname"] = ImageFile.FileName,
                    ["x-amz-meta-extension"] = Path.GetExtension(ImageFile.FileName)
                }
                };
                return await _s3.PutObjectAsync(putObjectRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get image from aws s3 
        public async Task<GetObjectResponse?> GetImage(Guid id)
        {
            try
            {
                var getObjectRequest = new GetObjectRequest()
                {

                    BucketName = BucketName,
                    Key = $"images/{id}.png",
                };

                var result = await _s3.GetObjectAsync(getObjectRequest);
                return result;
            }
            catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
            {
                return null;
            }

        }


        //delete image from s3
        public async Task<DeleteObjectResponse?> DeleteImage(Guid id)
        {
            var getDeleteRequest = new DeleteObjectRequest()
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
