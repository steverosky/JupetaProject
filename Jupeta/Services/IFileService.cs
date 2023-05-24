namespace Jupeta.Services
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile ImageFile);
        public bool DeleteImage(string ImageFileName);
    }
}