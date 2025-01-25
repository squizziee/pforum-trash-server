using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PForum.API.Data;
using PForum.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PForum.API.Controllers
{
    [Route("api/application")]
	[ApiController]
	public class UltimateController : ControllerBase
	{
		private readonly DatabaseContext _db;
		private readonly IWebHostEnvironment _environment;
		private readonly IConfiguration _configuration;

		public UltimateController(
			DatabaseContext db, 
			IWebHostEnvironment webHostEnvironment,
			IConfiguration configuration)
		{
			_db = db;
			_environment = webHostEnvironment;
			_configuration = configuration;
		}

		private async Task<string> UploadImage(IFormFile file)
		{
			var newName = Guid.NewGuid().ToString();
			var ext = file.FileName.Split(".").Last();

			var uploadPath = Path.Combine(
				_environment.WebRootPath,
				"img",
				newName + "." + ext
			);

			using var stream = System.IO.File.Create(uploadPath);
			await file.CopyToAsync(stream);

			return $"https://localhost:7162/img/{newName}.{ext}";
		}

		private async Task<string> GenerateAccessToken(User user)
		{
			var claims = new Claim[] {
				new ("UserId", user.Id.ToString()),
				new (ClaimTypes.Email, user.Email),
				new (ClaimTypes.Role, user.Role.ToString()),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:AccessToken:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:AccessToken:Issuer"],
				audience: _configuration["Jwt:AccessToken:Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(double.Parse(_configuration["Jwt:AccessToken:ExpirationTimeInDays"]!)),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[HttpPost("auth/register")]
		public async Task<IActionResult> RegisterUser(
			[FromBody] RegisterRequest request
			)
		{
			var newUser = new User
			{
				Email = request.Email,
				Username = request.Username,
				Password = request.Password,
				Role = Domain.UserRole.Viewer,
			};

			try
			{
                _db.Users.Add(newUser);

                _db.SaveChanges();
            } catch
			{
				return BadRequest();
			}

			var token = await GenerateAccessToken(newUser);

			Response.Cookies.Append("accessToken", token, new CookieOptions
			{
                SameSite = SameSiteMode.Lax,
                //Secure = true
            });
			return Ok();
		}

		// extremely dumb but whatever
		[HttpPost("auth/login")]
		public async Task<IActionResult> LoginUser(
			[FromBody] LoginRequest request
			)
		{
			var tryFind = _db.Users.Where(u => u.Email == request.Email).FirstOrDefault();

			if (tryFind == null)
			{
				return NotFound();
			}

			if (tryFind.Password != request.Password)
			{
				return BadRequest();
			}

			var token = await GenerateAccessToken(tryFind);
            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                //Secure = true
            });

            return Ok();
		}

        [HttpPost("auth/logout")]
        public async Task<IActionResult> LogoutUser(
            )
        {
            Response.Cookies.Append("accessToken", "0000", new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                //Secure = true
            });

            return Ok();
        }

        [HttpPost("admin")]
        public async Task<IActionResult> RegisterAdmin(
            )
        {
            var newUser = new User
            {
                Email = "admin@pforum.com",
                Username = "boss",
                Password = "admin",
                Role = Domain.UserRole.Admin,
            };

            _db.Users.Add(newUser);

            _db.SaveChanges();

            var token = await GenerateAccessToken(newUser);

            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                //Secure = true
            });
            return Ok();
        }

        [HttpGet("auth/users")]
		//[Authorize]
		public async Task<IActionResult> GetUsers()
		{
			var result = _db.Users.ToList();

			return Ok(result);
		}

		private User? GetUserFromContext(HttpContext context)
		{
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
				return null;
            }

            var uid = identity.FindFirst("UserId");

            if (uid == null)
            {
                return null;
            }

            var guid = new Guid(uid.Value);

            var result = _db.Users
                .Where(u => u.Id == guid)
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

			return result;
        }

		[HttpGet("auth/profile")]
		[Authorize]
		public async Task<IActionResult> GetUserInfo()
		{
			var result = GetUserFromContext(HttpContext);

			if (result == null)
			{
				return NotFound();
			}

			return Ok(result);
		}

		[HttpGet("langtopics")]
		public async Task<IActionResult> GetLanguageTopics()
		{
			var result = _db.LanguageTopics.ToList();
			return Ok(result);
		}

		[HttpPost("langtopics")]
		[Authorize(Roles = "Admin")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> AddLanguageTopic(
			[FromForm] AddLanguageRequest request
			)
		{
			_db.LanguageTopics.Add(
				new LanguageTopic
				{
					LanguageName = request.LanguageName,
					LanguageDescription = request.LanguageDescription,
					LanguageLogoUrl = await UploadImage(request.LanguageLogo)
				}
			);

			_db.SaveChanges();

			return Ok();
		}

        [Authorize(Roles = "Admin")]
        [HttpPut("langtopics")]
		public async Task<IActionResult> UpdateLanguageTopic(
			[FromQuery] Guid Id,
			[FromForm] UpdateLanguageRequest request
			)
		{
			var toChange = _db.LanguageTopics.Where(lt => lt.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			toChange.LanguageName = request.LanguageName;
			toChange.LanguageDescription = request.LanguageDescription;
			if (request.LanguageLogo != null)
			{
                toChange.LanguageLogoUrl = await UploadImage(request.LanguageLogo);
            }
			

			_db.SaveChanges();

			return Ok();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("langtopics")]
		public async Task<IActionResult> DeleteLanguageTopic(
			[FromQuery] Guid Id
			)
		{
			var toChange = _db.LanguageTopics.Where(lt => lt.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			_db.LanguageTopics.Remove(toChange);

			_db.SaveChanges();

			return Ok();
		}



		[HttpGet("topics")]
		public async Task<IActionResult> GetTopics(

			[FromQuery] Guid LanguageTopicId)
		{
			var result = _db.Topics
				.Where(t => t.LanguageTopicId == LanguageTopicId)
				.AsEnumerable();

			return Ok(result);
		}

		[HttpPost("topics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTopic(
			[FromForm] AddTopicRequest request
			)
		{
			_db.Topics.Add(
				new Topic
				{
					TopicName = request.TopicName,
					TopicDescription = request.TopicDescription,
					LanguageTopicId = request.LanguageTopicId
                }
			);

			_db.SaveChanges();

			return Ok();
		}

		[HttpPut("topics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTopic(
			[FromQuery] Guid Id,
			[FromForm] UpdateTopicRequest request
			)
		{
			var toChange = _db.Topics.Where(t => t.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}
			
			toChange.TopicName = request.TopicName;
			toChange.TopicDescription = request.TopicDescription;

			_db.SaveChanges();

			return Ok();
		}

		[HttpDelete("topics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTopic(
			[FromQuery] Guid Id
			)
		{
			var toChange = _db.Topics.Where(t => t.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			_db.Topics.Remove(toChange);

			_db.SaveChanges();

			return Ok();
		}



		[HttpGet("topicthreads")]
		public async Task<IActionResult> GetTopicThreads(
			[FromQuery] Guid TopicId
			)
		{
			var result = _db.TopicThreads
				.Where(tt => tt.TopicId == TopicId)
				.AsEnumerable();

			return Ok(result);
		}

        [HttpGet("threads")]
        public async Task<IActionResult> GetTopicThread(
            [FromQuery] Guid ThreadId
            )
        {
			var result = _db.TopicThreads
				.Include(tt => tt.User)
                .Include(tt => tt.Messages)
					.ThenInclude(m => m.User)
				.Where(tt => tt.Id == ThreadId).FirstOrDefault();

            return Ok(result);
        }

		[HttpPut("topicthreads")]
		public async Task<IActionResult> UpdateTopicThread(
			[FromQuery] Guid Id,
			[FromBody] UpdateThreadRequest request
			)
		{
			var toChange = _db.TopicThreads.Where(tt => tt.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			toChange.ThreadName = request.ThreadName;
			toChange.ThreadDescription = request.ThreadDescription;

			_db.SaveChanges();

			return Ok();
		}

		[HttpDelete("topicthreads")]
		public async Task<IActionResult> DeleteTopicThread(
			[FromQuery] Guid Id
			)
		{
			var toChange = _db.TopicThreads.Where(t => t.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			_db.TopicThreads.Remove(toChange);

			_db.SaveChanges();

			return Ok();
		}




		[HttpGet("messages")]
		public async Task<IActionResult> GetMessages(
			[FromQuery] Guid TopicThreadId)
		{
			var result = _db.TopicThreadMessages
				.Include(ttm => ttm.User)
				.Where(ttm => ttm.TopicThreadId == TopicThreadId)
				.AsEnumerable();

			return Ok(result);
		}

		[HttpPost("messages")]
		[Authorize]
		public async Task<IActionResult> AddMessage(
			[FromBody] AddMessageRequest request
			)
		{
            var user = GetUserFromContext(HttpContext);

            if (user == null)
            {
                return BadRequest();
            }

            if (request.TopicThreadId == null)
			{
				if (request.TopicId == null)
				{
					return BadRequest("Either TopicThreadId or TopicId should be issued in request");
				}

				var newTopicThreadId = Guid.NewGuid();
				_db.TopicThreads.Add(
					new TopicThread
					{
						Id = newTopicThreadId,
						ThreadName = request.MessageName,
						ThreadDescription = request.MessageText,
						TopicId = (Guid) request.TopicId,
                        UserId = user.Id,
                        PostedAt = DateTime.UtcNow
					}
				);

				_db.SaveChanges();

				return Ok();
			}

            _db.TopicThreadMessages.Add(
				new TopicThreadMessage
				{
					MessageText = request.MessageName + "\n" + request.MessageText,
					TopicThreadId = (Guid) request.TopicThreadId,
					UserId = user.Id,
					PostedAt = DateTime.UtcNow
				}
			);

			_db.SaveChanges();

			return Ok();
		}

		[HttpPut("messages")]
		public async Task<IActionResult> UpdateMessage(
			[FromQuery] Guid Id,
			[FromBody] UpdateMessageRequest request
			)
		{
			var toChange = _db.TopicThreadMessages.Where(ttm => ttm.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			toChange.MessageText = request.MessageName + "\n" + request.MessageText;

			_db.SaveChanges();

			return Ok();
		}

		[HttpDelete("messages")]
		public async Task<IActionResult> DeleteMessage(
			[FromQuery] Guid Id
			)
		{

			var toChange = _db.TopicThreadMessages.Where(t => t.Id == Id).FirstOrDefault();

			if (toChange == null)
			{
				return NotFound("No such entity");
			}

			_db.TopicThreadMessages.Remove(toChange);

			_db.SaveChanges();

			return Ok();
		}
	}

	public record RegisterRequest
	{
		public required string Email { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
	}

    public record LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public record AddMessageRequest
    {
        public required string MessageName { get; set; }
        public required string MessageText { get; set; }
        public Guid? TopicThreadId { get; set; }
        public Guid? TopicId { get; set; }
    }
    public record UpdateMessageRequest
    {
        public required string MessageName { get; set; }
        public required string MessageText { get; set; }
    }

    public record UpdateThreadRequest
    {
        public required string ThreadName { get; set; }
        public required string ThreadDescription { get; set; }
    }

    public record AddLanguageRequest
    {
        public required string LanguageName { get; set; }
        public required string LanguageDescription { get; set; }
        public required IFormFile LanguageLogo { get; set; }
    }

    public record UpdateLanguageRequest
    {
        public required string LanguageName { get; set; }
        public required string LanguageDescription { get; set; }
        public required IFormFile? LanguageLogo { get; set; }
    }

    public record AddTopicRequest
    {
        public required string TopicName { get; set; }
        public required string TopicDescription { get; set; }
        public required Guid LanguageTopicId { get; set; }
    }

    public record UpdateTopicRequest
    {
        public required string TopicName { get; set; }
        public required string TopicDescription { get; set; }
    }

}
