using Jupeta.Models;

namespace Jupeta.Services
{
    public interface IMongoDBservices
    {
        public List<UserReg> GetUsers();
        public UserReg GetUser(string id);
        public UserReg AddUser(UserReg user);
        public Task<object> Login(UserLogin user);
        public string CreateToken(UserLogin user);
        public Products AddProdcut(Products product);
        public Products GetProductById(string id);



    }
}