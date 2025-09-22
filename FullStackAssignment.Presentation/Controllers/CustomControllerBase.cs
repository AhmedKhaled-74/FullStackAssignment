using Microsoft.AspNetCore.Mvc;

namespace FullStackAssignment.Bootstrapper.Controllers
{
    /// <summary>
    /// custom controller for controllerbase class
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomControllerBase : ControllerBase
    {
        protected ActionResult ExceptionHandel(Exception ex)
        {
            if (ex.InnerException != null)
                return StatusCode(500, ex.InnerException.Message);
            if (!string.IsNullOrEmpty(ex.Message))
                return StatusCode(500, ex.Message);
            if (ex is ArgumentNullException || ex is ArgumentException || ex is InvalidOperationException)
                return BadRequest(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}
