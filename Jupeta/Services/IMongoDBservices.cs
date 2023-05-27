using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;

namespace Jupeta.Services
{
    public interface IMongoDBservices
    {
        public List<UserReg> GetUsers();
        public UserReg GetUser(string email);
        public void AddUser(AddUserModel user);
        public Task<object> Login(UserLogin user);
        public string CreateToken(UserLogin user);
        public void AddProduct(AddProductModel product);
        public Products GetProductById(string id);
        public List<Products> GetAllProducts();
        public List<Products> GetAvailableProducts();
        public void AddToCart(string id, string userId);
        public (List<Carts> carts, decimal totalPrice) ViewCart(string id);

    }
}