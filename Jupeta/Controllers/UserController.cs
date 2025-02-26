﻿using Jupeta.Models.DBModels;
using Jupeta.Models.RequestModels;
using Jupeta.Models.ResponseModels;
using Jupeta.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Security.Claims;


namespace Jupeta.Controllers
{
    //[HostAuthentication]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDBservices _db;
        private readonly ILogger<UserController> _logger;
        private readonly ICacheService _cachedb;
        private readonly IMongoClient _mongoClient;

        public UserController(IConfiguration config, IMongoDBservices db, ILogger<UserController> logger, ICacheService cachedb, IMongoClient mongoClient)
        {
            _configuration = config;
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger;
            _logger.LogInformation("User controller called ");
            _cachedb = cachedb;
            _mongoClient = mongoClient;
        }

        // [Authorize]
        [HttpGet]
        [Route("GetAllUsers")]
        public ActionResult<List<UserReg>> GetUsers()
        {
            if (!HttpContext.Session.IsAvailable)
            {
                return BadRequest("Login to continue");
            }

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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("AddUser")]
        public async Task<ActionResult> AddNewUser([FromBody] AddUserModel user)
        {
            _logger.LogInformation("Add user method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.AddUser(user);

                _logger.LogWarning($"User {user.Email} created successfully");
                return Ok(ResponseHandler.GetAppResponse(type, _db.GetUser(user.Email)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        //[Authorize]
        [HttpPut]
        [Route("EditProfile")]
        public async Task<ActionResult> EditProfile([FromBody] EditUserModel user)
        {
            _logger.LogInformation("Edit user method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.EditUser(user);

                _logger.LogInformation($"User {user.Email} edited successfully");
                return Ok(ResponseHandler.GetAppResponse(type, _db.GetUser(user.Email)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("TokenRefresh")]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("Refresh token method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                var model = await _db.Refresh();

                return Ok(ResponseHandler.GetAppResponse(type, model));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        //[Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<ActionResult<List<Products>>> GetAllProducts([FromQuery] PageParameters param)
        {
            _logger.LogInformation("Get all products method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                var data = await _db.GetAllProducts(param);

                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                else
                {
                    var metadata = new
                    {
                        data.TotalCount,
                        data.PageSize,
                        data.CurrentPage,
                        data.TotalPages,
                        data.NextPage,
                        data.PreviousPage
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("GetAvailableProducts")]
        public async Task<ActionResult<List<Products>>> GetAvailableProducts([FromQuery] PageParameters param)
        {
            _logger.LogInformation("Get all available products method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                var data = await _db.GetAvailableProducts(param);

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                else
                {
                    var metadata = new
                    {
                        data.TotalCount,
                        data.PageSize,
                        data.CurrentPage,
                        data.TotalPages,
                        data.NextPage,
                        data.PreviousPage
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("GetProductById")]
        public async Task<ActionResult<UserReg>> GetProductById(string id)
        {
            _logger.LogInformation("Get product by Id method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                Products data = await _db.GetProductById(id);
                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddNewProduct([FromForm] AddProductModel product)
        {
            _logger.LogInformation("Add product method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.AddProduct(product);

                _logger.LogWarning($"product {product.ProductName} added successfully");
                return Ok(ResponseHandler.GetAppResponse(type, product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory(Categories model)
        {
            _logger.LogInformation("Add categories method Starting.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.CreateCategory(model);

                _logger.LogWarning($"product {model.Name} added successfully");
                return Ok(ResponseHandler.GetAppResponse(type, model));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart(string productId, string userId)
        {
            _logger.LogInformation("Add to cart method Starting.");
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.AddToCart(productId, userId);

                _logger.LogWarning("product added successfully");
                return Ok(ResponseHandler.GetAppResponse(type, "Product added to cart successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [HttpGet]
        [Route("ViewCart")]
        public async Task<ActionResult> ViewCart(string userId)
        {
            ResponseType type = ResponseType.Success;
            _logger.LogInformation("View cart method Starting.");
            try
            {
                var (carts, totalPrice) = await _db.ViewCart(userId);

                if (carts is null)
                {
                    type = ResponseType.NotFound;
                }
                var responseData = new { Carts = carts, TotalPrice = totalPrice };
                return Ok(ResponseHandler.GetAppResponse(type, responseData));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpDelete]
        [Route("DeleteItemFromCart")]
        public async Task<ActionResult> DeleteItemFromCart(string productId, string userId)
        {
            _logger.LogInformation("Delete item from cart method Starting.");
            try
            {
                ResponseType type = ResponseType.Success;
                await _db.DeleteItem(productId, userId);

                _logger.LogWarning("product deleted successfully");
                return Ok(ResponseHandler.GetAppResponse(type, "Product deleted from cart successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpGet]
        [Route("SearchSort")]
        public async Task<ActionResult<List<Products>>> SearchSort(string? sortBy, string? keyword, bool isDescending, [FromQuery] PageParameters param)
        {
            _logger.LogInformation("Search products method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                var data = await _db.SearchSortBy(sortBy, keyword, isDescending, param);

                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                else
                {
                    var metadata = new
                    {
                        data.TotalCount,
                        data.PageSize,
                        data.CurrentPage,
                        data.TotalPages,
                        data.NextPage,
                        data.PreviousPage
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("GenerateOTPWithEmail")]
        public async Task<ActionResult> GenerateOTPWithEmail(string email)
        {
            _logger.LogInformation("Generate otp method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                await _db.GenerateOTPWithEmail(email);
                _logger.LogWarning("Otp generated successfully");
                return Ok(ResponseHandler.GetAppResponse(type, "Operation Completed Successfully"));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ValidateOTP")]
        public async Task<ActionResult> ValidateOTP(string otp, string email)
        {
            _logger.LogInformation("Validate otp method Starting.");
            ResponseType type = ResponseType.Success;
            try
            {
                var data = await _db.ValidateUserOTP(otp, email);
                if (data == false)
                {
                    type = ResponseType.Failure;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }


        [AllowAnonymous]
        [HttpGet("ExternalLogin/{provider}")]
        public IActionResult Login(string provider)
        {
            AuthenticationProperties props;
            switch (provider.ToLower())
            {
                case "google":
                    props = new AuthenticationProperties { RedirectUri = "/api/User/signin-google" };
                    return Challenge(props, GoogleDefaults.AuthenticationScheme);
                case "facebook":
                    props = new AuthenticationProperties { RedirectUri = "/api/User/signin-facebook" };
                    return Challenge(props, FacebookDefaults.AuthenticationScheme);
                case "apple":
                    props = new AuthenticationProperties { RedirectUri = "/api/User/signin-apple" };
                    return Challenge(props, "Apple");
                default:
                    return BadRequest("Invalid provider");
            }

        }

        [AllowAnonymous]
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            try
            {
                var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (response.Principal == null) return BadRequest();

                var name = response.Principal.FindFirstValue(ClaimTypes.Name);
                var givenName = response.Principal.FindFirstValue(ClaimTypes.GivenName);
                var surName = response.Principal.FindFirstValue(ClaimTypes.Surname);
                var email = response.Principal.FindFirstValue(ClaimTypes.Email);
                var ID = response.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var provider = response.Principal?.Identity?.AuthenticationType;

                // Get the physical path to the index.html file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "index.html");

                //Register user with the claims if not onboarded
                var IsEmail = await _db.UserExists(email!);
                if (!IsEmail)
                {
                    var userId = await _db.AddUserExternal(name!, email!);
                    await _db.AddToExtLogin(provider!, ID!, email!, userId!);

                    await _db.CreateToken(email!, userId);

                    return PhysicalFile(filePath, "text/html");
                }

                var user = _db.GetUser(email!);
                if (user?.Id != null)
                {
                    await _db.CreateToken(email!, user.Id);
                }
                // Return the file with a specified content type
                return PhysicalFile(filePath, "text/html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }

        }

        
        [AllowAnonymous]
        [HttpGet("signin-facebook")]
        public async Task<IActionResult> FacebookLoginCallback()
        {
            try
            {
                var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (response.Principal == null) return BadRequest();

                var name = response.Principal.FindFirstValue(ClaimTypes.Name);
                var givenName = response.Principal.FindFirstValue(ClaimTypes.GivenName);
                var surName = response.Principal.FindFirstValue(ClaimTypes.Surname);
                var email = response.Principal.FindFirstValue(ClaimTypes.Email);
                var ID = response.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var provider = response.Principal?.Identity?.AuthenticationType;

                // Get the physical path to the index.html file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "index.html");

                //Register user with the claims if not onboarded
                var IsEmail = await _db.UserExists(email!);
                if (!IsEmail)
                {
                    var userId = await _db.AddUserExternal(name!, email!);
                    await _db.AddToExtLogin(provider!, ID!, email!, userId!);

                    await _db.CreateToken(email!, userId);

                    return PhysicalFile(filePath, "text/html");
                }

                var user = _db.GetUser(email!);
                if (user?.Id != null)
                {
                    await _db.CreateToken(email!, user.Id);
                }
                // Return the file with a specified content type
                return PhysicalFile(filePath, "text/html");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        //[HttpPost, Authorize]
        //[Route("revoke")]
        //public IActionResult Revoke()
        //{
        //    var username = User.Identity.Name;
        //    var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
        //    if (user == null) return BadRequest();
        //    user.RefreshToken = null;
        //    _userContext.SaveChanges();
        //    return NoContent();
        //}
    }
}