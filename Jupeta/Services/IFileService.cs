using Amazon.S3.Model;
using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;

namespace Jupeta.Services
{
    public interface IFileService
    {
        //public Tuple<int, string> SaveImage(IFormFile ImageFile);
        //public bool DeleteImage(string ImageFileName);
        public Task<PutObjectResponse> UploadImage(Guid id, IFormFile ImageFile);
        public Task<GetObjectResponse?> GetImage(Guid id);
        public Task<DeleteObjectResponse?> DeleteImage(Guid id);

    }
}