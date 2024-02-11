using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;
using Jupeta.Models.ResponseModels;

namespace Jupeta.Services
{
    public interface IMongoDBservices
    {
        public List<UserReg> GetUsers();
        public UserReg GetUser(string email);
        public Task AddUser(AddUserModel user);
        public Task EditUser(EditUserModel user);
        public Task<object> Login(UserLogin user);
        public Task<TokenResponse> CreateToken(string email, string Id);
        public Task<object> Refresh();
        public Task AddProduct(AddProductModel product);
        public Task<Products> GetProductById(string id);
        public Task<PagedList<Products>> GetAllProducts(PageParameters param);
        public Task<PagedList<Products>> GetAvailableProducts(PageParameters param);
        public Task AddToCart(string id, string userId);
        public Task<(Carts carts, double totalPrice)> ViewCart(string id);
        public Task DeleteItem(string id, string userId);
        public Task CreateCategory(Categories model);
        public Task<PagedList<Products>> SearchSortBy(string? sortBy, string? keyword, bool isDescending, PageParameters param);
        public Task GenerateOTPWithEmail(string email);
        public Task<bool> ValidateUserOTP(string userOTP, string email);
        public Task<bool> UserExists(string email);

        public Task<string> AddUserExternal(string name, string email);

    }
}