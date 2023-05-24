using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;
using Jupeta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jupeta.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDBservices _db;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration config, IMongoDBservices db, ILogger<UserController> logger)
        {
            _configuration = config;
            _db = db;
            _logger = logger;
            _logger.LogInformation("User controller called ");
        }

        //[Authorize]
        [HttpGet]
        [Route("GetAllUsers")]
        public ActionResult<List<UserReg>> GetUsers()
        {
            _logger.LogInformation("Get all users method Starting.");
            return Ok(_db.GetUsers());
        }


        //[Authorize]
        [HttpGet]
        [Route("GetUserById")]
        public ActionResult<UserReg> GetUserById(string email)
        {
            _logger.LogInformation("Get user by Id method Starting.");
            var user = _db.GetUser(email);
            return Ok(user);
        }

        //[Authorize]
        [HttpPost]
        [Route("AddUser")]
        public ActionResult AddNewUser([FromBody] AddUserModel user)
        {
            _logger.LogInformation("Add user method Starting.");
            _db.AddUser(user);
            _logger.LogWarning($"User {user.Email} created successfully");
            return CreatedAtAction(nameof(GetUserById), new { email = user.Email }, _db.GetUser(user.Email));

        }



        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            _logger.LogInformation("Login method Starting.");
            if (user is not null)
            {
                var model = await _db.Login(user);
                return Ok(model);
            }
            return BadRequest("InValid Operation");
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public ActionResult<List<Products>> GetAllProducts()
        {
            _logger.LogInformation("Get all products method Starting.");
            return Ok(_db.GetAllProducts());
        }


        [HttpGet]
        [Route("GetProductById")]
        public ActionResult<UserReg> GetProductById(string id)
        {
            _logger.LogInformation("Get product by Id method Starting.");
            var product = _db.GetProductById(id);
            return Ok(product);
        }


        [HttpPost]
        [Route("AddProduct")]
        public IActionResult AddNewProduct([FromForm] AddProductModel product)
        {
            _logger.LogInformation("Add product method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.AddProduct(product);
            _logger.LogWarning($"product {product.ProductName} added successfully");
            return CreatedAtAction(nameof(GetProductById), product);


        }
    }
}
