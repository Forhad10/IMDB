using System;
using System.Collections.Generic;

namespace IMDB.Business.DTOs
{
    // User Authentication DTOs
    public class UserSignupDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserSigninDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuthResponseDto
    {
        public UserResponseDto User { get; set; } = new UserResponseDto();
        public string Token { get; set; } = string.Empty;
    }

    // User Bookmark DTOs
    public class UserBookmarkDto
    {
        public long BookmarkId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime BookmarkedAt { get; set; }
    }

    public class AddBookmarkDto
    {
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }

    public class UserBookmarksResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<UserBookmarkDto> Data { get; set; } = new List<UserBookmarkDto>();
    }

    // User Rating History DTOs
    public class UserRatingHistoryDto
    {
        public long RatingHistoryId { get; set; }
        public string TitleId { get; set; } = string.Empty;
        public short Rating { get; set; }
        public DateTime RatedAt { get; set; }
    }

    public class AddRatingDto
    {
        public string TitleId { get; set; } = string.Empty;
        public short Rating { get; set; }
    }

    public class UpdateRatingDto
    {
        public short Rating { get; set; }
    }

    public class UserRatingHistoryResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<UserRatingHistoryDto> Data { get; set; } = new List<UserRatingHistoryDto>();
    }

    // Paginated Response Base
    public class PaginatedResponseDto<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
