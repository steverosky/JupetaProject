using Jupeta.Models;
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

        public UserController(IConfiguration config, IMongoDBservices db)
        {
            _configuration = config;
            _db = db;
        }

        //[Authorize]
        [HttpGet]
        [Route("GetAllUsers")]
        public ActionResult<List<UserReg>> GetUsers()
        {
            return Ok(_db.GetUsers());
        }


        //[Authorize]
        [HttpGet]
        [Route("GetUserById")]
        public ActionResult<UserReg> GetUserById(string id)
        {
            var user = _db.GetUser(id);
            return Ok(user);
        }

        //[Authorize]
        [HttpPost]
        [Route("AddUser")]
        public ActionResult<UserReg> Post([FromBody] UserReg user)
        {
            _db.AddUser(user);
            return CreatedAtAction(nameof(GetUserById), new {id = user.Id}, user);
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] UserLogin user)
        {
            if (user is not null)
            {
                var model = _db.Login(user);
                return Ok(model);
            }
            return BadRequest("InValid Operation");
        }
    }
}
