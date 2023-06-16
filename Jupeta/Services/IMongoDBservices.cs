using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;

namespace Jupeta.Services
{
    public interface IMongoDBservices
    {
        public List<UserReg> GetUsers();
        public UserReg GetUser(string email);
        public Task AddUser(AddUserModel user);
        public Task EditUser(EditUserModel user);
        public Task<object> Login(UserLogin user);
        public string CreateToken(UserLogin user);
        public Task AddProduct(AddProductModel product);
        public Task<Products> GetProductById(string id);
        public Task<List<Products>> GetAllProducts();
        public Task<List<Products>> GetAvailableProducts();
        public Task AddToCart(string id, string userId);
        public Task<(Carts carts, double totalPrice)> ViewCart(string id);
        public Task DeleteItem(string id, string userId);
        public Task CreateCategory(Categories model);

    }
}