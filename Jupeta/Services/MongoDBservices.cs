using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;
using Jupeta.Models.ResponseModels;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jupeta.Services
{
    public class MongoDBservices : IMongoDBservices
    {
        private readonly IMongoCollection<UserReg> _users;
        private readonly IMongoCollection<Products> _products;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MongoDBservices(IMongoDBSettings mongoSettings, IConfiguration config, IMongoClient mongoClient,
            IHttpContextAccessor httpContextAccessor)
        {
            //MongoClient client = new MongoClient(mongoSettings.ConnectionURI);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);
            _users = database.GetCollection<UserReg>("users");
            _products = database.GetCollection<Products>("products");
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
        public List<UserReg> GetUsers() => _users.Find(user => true).ToList();

        //Get User by email
        public UserReg GetUser(string email) => _users.Find<UserReg>(user => user.Email == email).FirstOrDefault();

        //Add new User
        public void AddUser(AddUserModel user)
        {
            //check if email exists
            var IsEmail = _users.Find(p => p.Email == user.Email).FirstOrDefault();
            if (IsEmail is not null)
            {
                throw new Exception("User Already Exists");
            }
            var passwordHash = CreatePasswordhash(user.Password);

            UserReg dbTable = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = passwordHash,
                PhoneNumber= user.PhoneNumber,
                DateOfBirth= user.DateOfBirth,
                CreatedOn = DateTime.UtcNow
            };
            _users.InsertOne(dbTable);

        }

        //Login 
        public async Task<object> Login(UserLogin user)
        {
            var dbUser = await _users.Find(x => x.Email == user.Email).FirstOrDefaultAsync();

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
        public Products AddProdcut(Products product)
        {
            _products.InsertOne(product);
            return product;
        }


        //get product by id
        public Products GetProductById(string id) =>
            _products.Find(product => product.Id == id).FirstOrDefault();


        //get all products
        public List<Products> GetAllProducts() => _products.Find(p => true).ToList();
    }
}
