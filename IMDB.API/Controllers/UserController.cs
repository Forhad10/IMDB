using IMDB.Business.DTOs;
using IMDB.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IMDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupDto signupDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.SignupAsync(signupDto);

                return CreatedAtAction(nameof(Signup), new { id = user.UserId }, new
                {
                    message = "User created successfully",
                    user
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] UserSigninDto signinDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var authResponse = await _userService.SigninAsync(signinDto);

                return Ok(new
                {
                    message = "Sign in successful",
                    data = authResponse
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during sign in", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
            

                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = "An error occurred while deleting the user" });
            }
        }



        [HttpGet("{userId}/bookmarks")]
        public async Task<IActionResult> GetUserBookmarks(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var bookmarks = await _userService.GetUserBookmarksAsync(userId, page, pageSize);

                return Ok(new
                {
                    message = "Bookmarks retrieved successfully",
                    data = bookmarks
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving bookmarks", error = ex.Message });
            }
        }

        [HttpPost("{userId}/bookmarks")]
        public async Task<IActionResult> AddBookmark(Guid userId, [FromBody] AddBookmarkDto bookmarkDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bookmark = await _userService.AddBookmarkAsync(userId, bookmarkDto);

                return Ok(new
                {
                    message = "Bookmark added successfully",
                    data = bookmark
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding bookmark", error = ex.Message });
            }
        }

        [HttpDelete("{userId}/bookmarks/{titleId}")]
        public async Task<IActionResult> RemoveBookmark(Guid userId, string titleId)
        {
            try
            {
                var result = await _userService.RemoveBookmarkAsync(userId, titleId);

                if (!result)
                {
                    return NotFound(new { message = "Bookmark not found or already removed" });
                }

                return Ok(new
                {
                    message = "Bookmark removed successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing bookmark", error = ex.Message });
            }
        }



        [HttpPost("{userId}/ratings")]
        public async Task<IActionResult> AddOrUpdateRating(Guid userId, [FromBody] AddRatingDto ratingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var rating = await _userService.AddOrUpdateRatingAsync(userId, ratingDto);

                return Ok(new
                {
                    message = "Rating saved successfully",
                    data = rating
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving rating", error = ex.Message });
            }
        }

        [HttpDelete("{userId}/ratings/{titleId}")]
        public async Task<IActionResult> RemoveRating(Guid userId, string titleId)
        {
            try
            {
                var result = await _userService.RemoveRatingAsync(userId, titleId);

                if (!result)
                {
                    return NotFound(new { message = "Rating not found or already removed" });
                }

                return Ok(new
                {
                    message = "Rating removed successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing rating", error = ex.Message });
            }
        }

        [HttpGet("{userId}/ratings")]
        public async Task<IActionResult> GetUserRatingHistory(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var ratingHistory = await _userService.GetUserRatingHistoryAsync(userId, page, pageSize);

                return Ok(new
                {
                    message = "Rating history retrieved successfully",
                    data = ratingHistory
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving rating history", error = ex.Message });
            }
        }

    }
}
