using IMDB.Business.DTOs;
using IMDB.Business.Services;
using IMDB.Data;
using IMDB.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMDB.Business.Tests
{
    public class TitleServiceTests : IDisposable
    {
        private readonly IMDBDbContext _context;
        private readonly TitleService _titleService;
        private readonly SqliteConnection _connection;

        public TitleServiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            _connection.CreateFunction("now", () => DateTime.UtcNow);

            var options = new DbContextOptionsBuilder<IMDBDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new IMDBDbContext(options);
            _context.Database.EnsureCreated();

            _titleService = new TitleService(_context);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Titles.Any())
            {
                _context.Titles.AddRange(
                    new Title
                    {
                        TitleId = "tt0000001",
                        TitleType = "movie",
                        PrimaryTitle = "The Shawshank Redemption",
                        OriginalTitle = "The Shawshank Redemption",
                        StartYear = "1994",
                        EndYear = null,
                        RuntimeMinutes = "142",
                        Genres = "Drama",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Title
                    {
                        TitleId = "tt0000002",
                        TitleType = "movie",
                        PrimaryTitle = "The Godfather",
                        OriginalTitle = "The Godfather",
                        StartYear = "1972",
                        EndYear = null,
                        RuntimeMinutes = "175",
                        Genres = "Crime,Drama",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Title
                    {
                        TitleId = "tt0000003",
                        TitleType = "tvSeries",
                        PrimaryTitle = "Breaking Bad",
                        OriginalTitle = "Breaking Bad",
                        StartYear = "2008",
                        EndYear = "2013",
                        RuntimeMinutes = "49",
                        Genres = "Crime,Drama,Thriller",
                        CreatedAt = DateTime.UtcNow
                    }
                );

                _context.TitleRatings.AddRange(
                    new TitleRating
                    {
                        TitleId = "tt0000001",
                        AverageRating = 9,
                        NumVotes = 25
                    },
                    new TitleRating
                    {
                        TitleId = "tt0000002",
                        AverageRating = 9,
                        NumVotes = 18
                    },
                    new TitleRating
                    {
                        TitleId = "tt0000003",
                        AverageRating = 9,
                        NumVotes = 19
                    }
                );

                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }

        /* ===== Title Object Tests ===== */
        [Fact]
        public void Title_Object_HasIdTitleTypeAndPrimaryTitle()
        {
            var title = new Title();
            Assert.Null(title.TitleId);
            Assert.Null(title.TitleType);
            Assert.Null(title.PrimaryTitle);
            Assert.Null(title.OriginalTitle);
            Assert.Null(title.StartYear);
            Assert.Null(title.EndYear);
            Assert.Null(title.RuntimeMinutes);
            Assert.Null(title.Genres);
        }

        /* ===== TitleRating Tests ===== */
        [Fact]
        public void TitleRating_Object_HasTitleIdAverageRatingAndNumVotes()
        {
            var rating = new TitleRating();
            Assert.Null(rating.TitleId);
            Assert.Equal(0, rating.AverageRating ?? 0);
            Assert.Equal(0, rating.NumVotes ?? 0);
        }

   

     
    }

    public class DropdownListServiceTests : IDisposable
    {
        private readonly IMDBDbContext _context;
        private readonly DropdownListService _dropdownService;
        private readonly SqliteConnection _connection;

        public DropdownListServiceTests()
        {
            // SQLite in-memory database
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<IMDBDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new IMDBDbContext(options);
            _context.Database.EnsureCreated();

            _dropdownService = new DropdownListService(_context);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add test actors
            if (!_context.Actors.Any())
            {
                var currentTime = DateTime.UtcNow;
                _context.Actors.AddRange(
                    new Actor 
                    { 
                        NameId = "nm0000001", 
                        PrimaryName = "Tom Hanks",
                        BirthYear = "1956",
                        CreatedAt = currentTime
                    },
                    new Actor 
                    { 
                        NameId = "nm0000002", 
                        PrimaryName = "Meryl Streep",
                        BirthYear = "1949",
                        CreatedAt = currentTime
                    }
                );
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetActorsForDropdownAsync_ShouldReturnAllActors()
        {
            // Act
            var result = await _dropdownService.GetActorsForDropdownAsync();
            var actors = result.ToList();

            // Assert
            Assert.NotNull(actors);
            Assert.Equal(2, actors.Count);
            Assert.Contains(actors, a => a.NameId == "nm0000001" && a.PrimaryName == "Tom Hanks");
            Assert.Contains(actors, a => a.NameId == "nm0000002" && a.PrimaryName == "Meryl Streep");
        }

        [Fact]
        public async Task GetActorsForDropdownAsync_ShouldReturnCorrectDtoProperties()
        {
            // Act
            var result = await _dropdownService.GetActorsForDropdownAsync();
            var actor = result.FirstOrDefault();

            // Assert
            Assert.NotNull(actor);
            Assert.NotNull(actor.NameId);
            Assert.NotNull(actor.PrimaryName);
        }

        [Fact]
        public async Task GetActorsForDropdownAsync_ShouldReturnEmptyList_WhenNoActorsExist()
        {
            // Arrange
            _context.Actors.RemoveRange(_context.Actors);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dropdownService.GetActorsForDropdownAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _context?.Dispose();
            }
        }
    }

    public class UserServiceTests : IDisposable
    {
        private readonly IMDBDbContext _context;
        private readonly UserService _userService;
        private readonly SqliteConnection _connection;

        public UserServiceTests()
        {
            // SQLite in-memory database
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<IMDBDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new IMDBDbContext(options);
            _context.Database.EnsureCreated();

            _userService = new UserService(_context);
        }

        [Fact]
        public async Task SignupAsync_ShouldCreateNewUser_WhenValidDataProvided()
        {
            // Arrange
            var signupDto = new UserSignupDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test@123"
            };

            // Act
            var result = await _userService.SignupAsync(signupDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(signupDto.Username, result.Username);
            Assert.Equal(signupDto.Email, result.Email);
            Assert.True(result.IsActive);
            Assert.NotEqual(Guid.Empty, result.UserId);

            // Verify user is saved in the database
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == signupDto.Email);
            Assert.NotNull(userInDb);
            Assert.Equal(signupDto.Username, userInDb.Username);
        }

        [Fact]
        public async Task SignupAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = "hashedpassword",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var signupDto = new UserSignupDto
            {
                Username = "newuser",
                Email = "existing@example.com", // Same email as existing user
                Password = "Test@123"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.SignupAsync(signupDto));
            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task SignInAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "Test@123";
            var testUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = TestHelpers.HashPassword(password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var signinDto = new UserSigninDto
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var result = await _userService.SigninAsync(signinDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.User);
            Assert.NotNull(result.Token);
            Assert.Equal(testUser.UserId, result.User.UserId);
            Assert.Equal(testUser.Username, result.User.Username);
            Assert.Equal(testUser.Email, result.User.Email);
        }

        [Fact]
        public async Task SignInAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var signinDto = new UserSigninDto
            {
                Email = "nonexistent@example.com",
                Password = "Test@123"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.SigninAsync(signinDto));
            Assert.Contains("Invalid email or password", exception.Message);
        }

        [Fact]
        public async Task SignInAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var testUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = TestHelpers.HashPassword("CorrectPassword"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var signinDto = new UserSigninDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.SigninAsync(signinDto));
            Assert.Contains("Invalid email or password", exception.Message);
        }

        [Fact]
        public async Task SignInAsync_ShouldThrowException_WhenAccountIsInactive()
        {
            // Arrange
            var password = "Test@123";
            var testUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "inactiveuser",
                Email = "inactive@example.com",
                PasswordHash = TestHelpers.HashPassword(password),
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var signinDto = new UserSigninDto
            {
                Email = "inactive@example.com",
                Password = password
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.SigninAsync(signinDto));
            Assert.Contains("Account is not active", exception.Message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _context?.Dispose();
            }
        }
    }

    /// <summary>
    /// Shared test utilities
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Hashes a password using SHA256 algorithm
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
