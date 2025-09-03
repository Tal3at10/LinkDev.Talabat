using LinkDev.Talabat.APIs.Controllers.Base;
using LinkDev.Talabat.APIs.Controllers.Errors;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Buggy
{
    public class BuggyController : BaseApiController
    {
        // ✅ 400 Bad Request
        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]
        public IActionResult GetValidationError(int id) // ✅ 400
        {
           return Ok();
        }

        // ✅ 401 Unauthorized
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return StatusCode(401, new ApiResponse(401));
        }

        // ✅ 404 Not Found
        [HttpGet("notfound")]
        public IActionResult GetNotFound()
        {
            return NotFound(new ApiResponse(404, "Resource not found"));
        }

        // ✅ 500 Internal Server Error
        [HttpGet("servererror")]
        public IActionResult GetServerError()
        {
            throw new Exception();
        }

    }
}
