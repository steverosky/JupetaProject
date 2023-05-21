using Jupeta.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jupeta.Services
{
    public class MongoDBservices : IMongoDBservices
    {
        private readonly IMongoCollection<UserReg> _usersCollection;
        private readonly IMongoCollection<Products> _productsCollection;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MongoDBservices(IMongoDBSettings mongoSettings, IConfiguration config, IMongoClient mongoClient,
            IHttpContextAccessor httpContextAccessor)
        {
            //MongoClient client = new MongoClient(mongoSettings.ConnectionURI);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);
            _usersCollection = database.GetCollection<UserReg>(mongoSettings.CollectionName);
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }


        //Hash User password
        public static string CreatePasswordhash(string password)
        {
            if (password != null)
            {
                password = BCrypt.Net.BCrypt.HashPassword(password);

                return password;
            }
            else throw new Exception("invalid");

        }

        //get all Users
        public List<UserReg> GetUsers() => _usersCollection.Find(user => true).ToList();

        //Get User by Id
        public UserReg GetUser(string id) => _usersCollection.Find<UserReg>(user => user.Id == id).FirstOrDefault();

        //Add new User
        public UserReg AddUser(UserReg user)
        {
            user.PasswordHash = CreatePasswordhash(user.PasswordHash);
            _usersCollection.InsertOne(user);
            return user;
        }

        //Login 
        public async Task<object> Login(UserLogin user)
        {
            var dbUser = await _usersCollection.Find(x => x.Email == user.Email).FirstOrDefaultAsync();

            if (dbUser != null)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.PasswordHash, dbUser.PasswordHash);
                if (isPasswordValid)
                {
                    string token = CreateToken(user);
                    return new TokenResponse
                    {
                        Email = user.Email, 
                        Token = token 
                    };

                }
                else
                {
                    throw new Exception("Email or Password is Incorrect");
                }
            }
            throw new Exception("User Not Found");
        }


        //Create Token for authentication
        public string CreateToken(UserLogin user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("JwtConfig:Secret").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                 _config["JwtConfig:Issuer"],
                _config["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        //add new products
        public async Task<object> AddProdcut(Products product)
        {
            var roleclaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        }

    }
}
