using internship_task_backend.DataAccessLayer;
using internship_task_backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using System.Security.Cryptography.X509Certificates;
using static internship_task_backend.Model.AddLesson;
using static internship_task_backend.Model.GetLesson;
using static internship_task_backend.Model.LogIn;
using static internship_task_backend.Model.SignUp;


namespace internship_task_backend.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        
        public readonly IAuthDL _authDL;
        public AuthController(IAuthDL authDL)
        {
            _authDL = authDL;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            try
            {
                response = await _authDL.SignUp(request);

            }catch(Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return Ok(response);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInRequest request)
        {
            LogInResponse response = new LogInResponse();
            
            try
            {
                response = await _authDL.LogIn(request);
               

            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return Ok(response);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddLesson(AddLessonRequest request)
        {
            AddLessonResponse response = new AddLessonResponse();
            try
            {
                response = await _authDL.AddLesson(request);

            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetLesson()
        {
            GetLessonResponse response = new GetLessonResponse();
            try
            {
                response = await _authDL.GetLesson();
            }catch(Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return Ok(response);
        }

       
    }
}
