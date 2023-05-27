using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;
using Jupeta.Models.ResponseModels;
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
            ResponseType type = ResponseType.Success;
            _logger.LogInformation("Get all users method Starting.");
            try
            {
                IEnumerable<UserReg> data = _db.GetUsers();

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }



        //[Authorize]
        [HttpGet]
        [Route("GetUserById")]
        public ActionResult<UserReg> GetUserById(string email)
        {
            ResponseType type = ResponseType.Success;
            _logger.LogInformation("Get user by Id method Starting.");
            try
            {
                UserReg data = _db.GetUser(email);
                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("AddUser")]
        public ActionResult AddNewUser([FromBody] AddUserModel user)
        {
            _logger.LogInformation("Add user method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                _db.AddUser(user);

                _logger.LogWarning($"User {user.Email} created successfully");
                return Ok(ResponseHandler.GetAppResponse(type, _db.GetUser(user.Email)));
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            _logger.LogInformation("Login method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                var model = await _db.Login(user);

                return Ok(ResponseHandler.GetAppResponse(type, model));
            }

            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }



        [HttpGet]
        [Route("GetAllProducts")]
        public ActionResult<List<Products>> GetAllProducts()
        {
            _logger.LogInformation("Get all products method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                IEnumerable<Products> data = _db.GetAllProducts();

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("GetAvailableProducts")]
        public ActionResult<List<Products>> GetAvailableProducts()
        {
            _logger.LogInformation("Get all available products method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                IEnumerable<Products> data = _db.GetAvailableProducts();

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("GetProductById")]
        public ActionResult<UserReg> GetProductById(string id)
        {
            _logger.LogInformation("Get product by Id method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                Products data = _db.GetProductById(id);
                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
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
            try
            {
                ResponseType type = ResponseType.Success;
                _db.AddProduct(product);

                _logger.LogWarning($"product {product.ProductName} added successfully");
                return Ok(ResponseHandler.GetAppResponse(type, product));
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpPost]
        [Route("AddToCart")]
        public IActionResult AddToCart(string productId, string userId)
        {
            _logger.LogInformation("Add to cart method Starting.");
            try
            {
                ResponseType type = ResponseType.Success;
                _db.AddToCart(productId, userId);

                _logger.LogWarning("product added successfully");
                return Ok(ResponseHandler.GetAppResponse(type, "Product added to cart successfully"));
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("ViewCart")]
        public ActionResult ViewCart(string userId)
        {
            ResponseType type = ResponseType.Success;
            _logger.LogInformation("View cart method Starting.");
            try
            {
                var (carts, totalPrice) = _db.ViewCart(userId);

                if (carts.Count == 0)
                {
                    type = ResponseType.NotFound;
                }
                var responseData = new { Carts = carts, TotalPrice = totalPrice };
                return Ok(ResponseHandler.GetAppResponse(type, responseData));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }
    }
}