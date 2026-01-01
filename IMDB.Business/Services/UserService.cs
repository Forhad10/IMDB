using Dapper;
using IMDB.Business.DTOs;
using IMDB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IMDB.Business.Services
{
    public class UserService
    {
        private readonly IMDBDbContext _context;
        private readonly IConfiguration _configuration;
        private IMDBDbContext context;

        public UserService(IMDBDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public UserService(IMDBDbContext context)
        {
            this.context = context;
        }

        public async Task<UserResponseDto> SignupAsync(UserSignupDto signupDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == signupDto.Email || u.Username == signupDto.Username);

            if (existingUser != null)
            {
                throw new ArgumentException("User with this email or username already exists");
            }


            var passwordHash = HashPassword(signupDto.Password);

            var user = new User
            {
                Username = signupDto.Username,
                Email = signupDto.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log the detailed inner exception
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new ArgumentException($"Database error: {innerException}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to save user: {ex.Message}");
            }

            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive.HasValue ? user.IsActive.Value : false,
                CreatedAt = user.CreatedAt.HasValue ? user.CreatedAt.Value : DateTime.Now
            };
        }

        public async Task<AuthResponseDto> SigninAsync(UserSigninDto signinDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == signinDto.Email);

            if (user == null)
            {
                throw new ArgumentException("Invalid email or password");
            }

            if (!user.IsActive.HasValue || !user.IsActive.Value)
            {
                throw new ArgumentException("Account is not active");
            }

            var passwordHash = HashPassword(signinDto.Password);
            if (user.PasswordHash != passwordHash)
            {
                throw new ArgumentException("Invalid email or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive.HasValue ? user.IsActive.Value : false,
                    CreatedAt = user.CreatedAt.HasValue ? user.CreatedAt.Value : DateTime.Now
                },
                Token = token
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiresMinutesStr = _configuration["Jwt:ExpiresMinutes"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("JWT Key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(int.TryParse(expiresMinutesStr, out var m) ? m : 60);

            var jwt = new JwtSecurityToken(
                issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
                audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// get  user bookmarks


        public async Task<UserBookmarksResponseDto> GetUserBookmarksAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (page - 1) * pageSize;

            // Single SQL query that returns data and count of returned rows
            var sql = @"SELECT
                       u.bookmark_id as BookmarkId,
                       u.title_id as TitleId,
					   t.primary_title as TitleName,
                       COALESCE(u.bookmarked_at, NOW()) as BookmarkedAt
                       FROM user_bookmarks u
					   inner join titles t on t.title_id=u.title_id
                       WHERE u.user_id = @UserId
                       ORDER BY u.bookmarked_at DESC
                       OFFSET @Offset ROWS
                       FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new { UserId = userId, Offset = offset, PageSize = pageSize };
            var data = await connection.QueryAsync<UserBookmarkDto>(sql, parameters);

            return new UserBookmarksResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = data.Count(),
                Data = data
            };
        }

        public async Task<UserBookmarkDto> AddBookmarkAsync(Guid userId, AddBookmarkDto bookmarkDto)
        {
            // Check if bookmark already exists
            var existingBookmark = await _context.UserBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId
                    && b.TitleId == bookmarkDto.TitleId);
            if (existingBookmark != null)
            {
                throw new ArgumentException("Bookmark already exists for this entity");
            }

            var bookmark = new UserBookmark
            {
                UserId = userId,
                TitleId = bookmarkDto.TitleId,
                BookmarkedAt = DateTime.Now
            };

            _context.UserBookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            // Get title name for the response
            var titleName = await _context.Titles
                .Where(t => t.TitleId == bookmarkDto.TitleId)
                .Select(t => t.PrimaryTitle)
                .FirstOrDefaultAsync() ?? string.Empty;

            return new UserBookmarkDto
            {
                BookmarkId = bookmark.BookmarkId,
                TitleId = bookmark.TitleId ?? string.Empty,
                TitleName = titleName,
                BookmarkedAt = bookmark.BookmarkedAt ?? DateTime.Now
            };
        }

        public async Task<bool> RemoveBookmarkAsync(Guid userId, string titleId)
        {
            var bookmark = await _context.UserBookmarks
                .FirstOrDefaultAsync(b => b.TitleId == titleId && b.UserId == userId);
            if (bookmark == null)
            {
                return false;
            }
            _context.UserBookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        //// Get  user rating history

        public async Task<UserRatingHistoryResponseDto> GetUserRatingHistoryAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (page - 1) * pageSize;

            // Single SQL query that returns data and count of returned rows
            var sql = @"SELECT
                       u.rating_history_id as RatingHistoryId,
                       u.title_id as TitleId,
					   t.primary_title as TitleName,
                       COALESCE(rating, 0) as Rating,
                       COALESCE(rated_at, NOW()) as RatedAt
                       FROM user_rating_history u
					   inner join titles t on t.title_id=u.title_id
                       WHERE user_id = @UserId
                       ORDER BY rated_at DESC
                       OFFSET @Offset ROWS
                       FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new { UserId = userId, Offset = offset, PageSize = pageSize };
            var data = await connection.QueryAsync<UserRatingHistoryDto>(sql, parameters);

            return new UserRatingHistoryResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = data.Count(),
                Data = data
            };
        }

        public async Task<UserRatingHistoryDto> AddOrUpdateRatingAsync(Guid userId, AddRatingDto ratingDto)
        {
            // Check if rating already exists
            var existingRating = await _context.UserRatingHistories
                .FirstOrDefaultAsync(r => r.UserId == userId && r.TitleId == ratingDto.TitleId);
            if (existingRating != null)
            {
                // Update existing rating
                existingRating.PreviousRating = existingRating.Rating; // Keep previous rating before updating
                existingRating.Rating = ratingDto.Rating;
                existingRating.RatedAt = DateTime.Now;
                _context.UserRatingHistories.Update(existingRating);
                await _context.SaveChangesAsync();

                // Update TitleRating with new average calculation
                await UpdateTitleRatingAsync(ratingDto.TitleId, userId, "update");

                return new UserRatingHistoryDto
                {
                    RatingHistoryId = existingRating.RatingHistoryId,
                    TitleId = existingRating.TitleId,
                    TitleName = await _context.Titles
                        .Where(t => t.TitleId == existingRating.TitleId)
                        .Select(t => t.PrimaryTitle)
                        .FirstOrDefaultAsync() ?? string.Empty,
                    Rating = existingRating.Rating ?? 0,
                    RatedAt = existingRating.RatedAt ?? DateTime.Now
                };
            }
            else
            {
                // Add new rating
                var ratingHistory = new UserRatingHistory
                {
                    UserId = userId,
                    TitleId = ratingDto.TitleId,
                    Rating = ratingDto.Rating,
                    PreviousRating = ratingDto.Rating,
                    RatedAt = DateTime.Now
                };
                _context.UserRatingHistories.Add(ratingHistory);
                await _context.SaveChangesAsync();

                // Update TitleRating with new average calculation
                await UpdateTitleRatingAsync(ratingDto.TitleId, userId, "add");

                return new UserRatingHistoryDto
                {
                    RatingHistoryId = ratingHistory.RatingHistoryId,
                    TitleId = ratingHistory.TitleId,
                    TitleName = await _context.Titles
                        .Where(t => t.TitleId == ratingHistory.TitleId)
                        .Select(t => t.PrimaryTitle)
                        .FirstOrDefaultAsync() ?? string.Empty,
                    Rating = ratingHistory.Rating ?? 0,
                    RatedAt = ratingHistory.RatedAt ?? DateTime.Now
                };
            }
        }

        private async Task UpdateTitleRatingAsync(string titleId, Guid userId, string action)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            // Execute stored procedure with titleId, userId, and action parameters
            var parameters = new
            {
                p_title_id = titleId,
                p_user_id = userId,
                p_action = action
            };
            try
            {
                await connection.ExecuteAsync("UpdateTitleRating", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
          
                Console.WriteLine($"Failed to update title rating for {titleId}, User: {userId}, Action: {action}. Error: {ex.Message}");
                throw; 
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> RemoveRatingAsync(Guid userId, string titleId)
        {
            try
            {
                var rating = await _context.UserRatingHistories
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.TitleId == titleId);
                if (rating == null)
                {
                    return false;
                }

                // Update TitleRating BEFORE removing the rating (stored procedure needs to find the user's rating)
                await UpdateTitleRatingAsync(titleId, userId, "remove");

                // Now remove the rating from database
                _context.UserRatingHistories.Remove(rating);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
       
                Console.WriteLine($"Failed to remove rating for TitleId: {titleId}, UserId: {userId}. Error: {ex.Message}");
                return false; 
            }
        }


        // UserService.cs
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
      
                throw;
            }
        }
    }
}
