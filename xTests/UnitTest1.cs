using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using PForum.Domain.Entities;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace xTests
{
	public class UnitTestGetEndpoints
	{
		private readonly HttpClient _httpClient;
		private readonly HttpClientHandler _handler;
		private readonly ITestOutputHelper _output;
		public UnitTestGetEndpoints(ITestOutputHelper output) 
		{
			_output = output;
			_handler = new HttpClientHandler();
			_httpClient = new HttpClient(_handler)
			{
				BaseAddress = new Uri("https://localhost:7162")
			};
		}

		[Fact]
		public async void TestGetLanguageTopics()
		{
			var response = await _httpClient
				.GetAsync("/api/application/langtopics");

			response.EnsureSuccessStatusCode();

			var casted = JsonSerializer
				.Deserialize <LanguageTopic[]>(await response.Content.ReadAsStringAsync());

			_output.WriteLine(await response.Content.ReadAsStringAsync());

			Assert.True(casted!.Length > 0);
		}

		[Fact]
		public async void TestGetTopics()
		{
			var response = await _httpClient
				.GetAsync("/api/application/topics?LanguageTopicId=a95c0212-1b98-4eaf-aa82-60ae7e6d50dc");

			response.EnsureSuccessStatusCode();

			var casted = JsonSerializer
				.Deserialize<Topic[]>(await response.Content.ReadAsStringAsync());

			Assert.True(casted!.Length > 0);
		}

		[Fact]
		public async void TestGetTopics_DoesNotExist()
		{
			var response = await _httpClient
				.GetAsync("/api/application/topics?LanguageTopicId=aboba");

			Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
		}

		[Fact]
		public async void TestGetThreads()
		{
			var response = await _httpClient
				.GetAsync("/api/application/topicthreads?TopicId=c4857631-e30a-439e-b6ce-99c94063a1aa");

			response.EnsureSuccessStatusCode();

			var casted = JsonSerializer
				.Deserialize<TopicThread[]>(await response.Content.ReadAsStringAsync());

			Assert.True(casted!.Length > 0);
		}

		[Fact]
		public async void TestGetThreads_DoesNotExist()
		{
			var response = await _httpClient
				.GetAsync("/api/application/topicthreads?TopicId=aboba");

			Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
		}

		[Fact]
		public async void TestGetMessages()
		{
			var response = await _httpClient
				.GetAsync("/api/application/messages?TopicThreadId=6089ac0e-35d5-4a83-a0eb-7cf04e68fccb");

			response.EnsureSuccessStatusCode();

			var casted = JsonSerializer
				.Deserialize<TopicThread[]>(await response.Content.ReadAsStringAsync());

			Assert.True(casted!.Length > 0);
		}

        [Fact]
        public async void TestGetMessages_DoesNotExist()
        {
            var response = await _httpClient
                .GetAsync("/api/application/messages?TopicThreadId=aboba");

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }
    }

	public class UnitTestPostEndpoints
	{
		private readonly HttpClient _httpClient;
		private readonly ITestOutputHelper _output;
		private readonly HttpClientHandler _handler;
		public UnitTestPostEndpoints(ITestOutputHelper output)
		{
			_output = output;
			_handler = new HttpClientHandler()
			{
				CookieContainer = new CookieContainer()
			};
			_httpClient = new HttpClient(_handler)
			{
				BaseAddress = new Uri("https://localhost:7162"),
			};
		}

		private async Task<IEnumerable<Cookie>> Login()
		{
			var pairs = new Dictionary<string, string>
			{
				["Email"] = "admin@pforum.com",
				["Password"] = "admin",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/login", content);

			_output.WriteLine(response.StatusCode.ToString());

			var cookies = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

			return cookies;
		}

		[Fact]
		public async void TestPostLanguageTopics()
		{
			var cookies = await Login();

			using var source = File.OpenRead("C:\\Users\\USER\\Desktop\\tornado-videohosting\\PForum\\PForum.API\\wwwroot\\img\\test.png");

			var datastream = new MemoryStream();

			await source.CopyToAsync(datastream);

			var content = new MultipartFormDataContent
			{
				{ new StreamContent(datastream), "LanguageLogo", "test.png" },
				{ new StringContent("testlangname1"), "LanguageName" },
				{ new StringContent("testlangname1"), "LanguageDescription" }
			};

			var response = await _httpClient
				.PostAsync("/api/application/langtopics", content);

			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async void TestPostTopics()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["TopicName"] = "testtopicname1",
				["TopicDescription"] = "testtopicname1",
				["LanguageTopicId"] = "a95c0212-1b98-4eaf-aa82-60ae7e6d50dc"
			};

			var content = new FormUrlEncodedContent(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/topics", content);

			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async void TestPostThreads()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["MessageName"] = "testthreadname1",
				["MessageText"] = "testthreadname1",
				["TopicId"] = "c4857631-e30a-439e-b6ce-99c94063a1aa"
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/messages", content);

			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async void TestPostMessages()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["MessageName"] = "testthreadname1",
				["MessageText"] = "testthreadname1",
				["TopicThreadId"] = "6089ac0e-35d5-4a83-a0eb-7cf04e68fccb"
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/messages", content);

			response.EnsureSuccessStatusCode();
		}
	}

	public class UnitTestPutEndpoints
	{
		private readonly HttpClient _httpClient;
		private readonly ITestOutputHelper _output;
		private readonly HttpClientHandler _handler;
		public UnitTestPutEndpoints(ITestOutputHelper output)
		{
			_output = output;
			_handler = new HttpClientHandler()
			{
				CookieContainer = new CookieContainer()
			};
			_httpClient = new HttpClient(_handler)
			{
				BaseAddress = new Uri("https://localhost:7162"),
			};
		}

		private async Task<IEnumerable<Cookie>> Login()
		{
			var pairs = new Dictionary<string, string>
			{
				["Email"] = "admin@pforum.com",
				["Password"] = "admin",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/login", content);

			_output.WriteLine(response.StatusCode.ToString());

			var cookies = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

			return cookies;
		}

		[Fact]
		public async void TestPutLanguageTopics()
		{
			var cookies = await Login();

			using var source = File.OpenRead("C:\\Users\\USER\\Desktop\\tornado-videohosting\\PForum\\PForum.API\\wwwroot\\img\\test.png");

			var datastream = new MemoryStream();

			await source.CopyToAsync(datastream);

			var content = new MultipartFormDataContent
			{
				{ new StreamContent(datastream), "LanguageLogo", "test.png" },
				{ new StringContent("testlangname1"), "LanguageName" },
				{ new StringContent("testlangname1"), "LanguageDescription" }
			};

			var response = await _httpClient
				.PutAsync("/api/application/langtopics?Id=0a1641df-bbb3-4fa4-a07f-97df90762558", content);

			response.EnsureSuccessStatusCode();
		}

        [Fact]
        public async void TestPutLanguageTopics_DoesNotExist()
        {
            var cookies = await Login();

            using var source = File.OpenRead("C:\\Users\\USER\\Desktop\\tornado-videohosting\\PForum\\PForum.API\\wwwroot\\img\\test.png");

            var datastream = new MemoryStream();

            await source.CopyToAsync(datastream);

            var content = new MultipartFormDataContent
            {
                { new StreamContent(datastream), "LanguageLogo", "test.png" },
                { new StringContent("testlangname1"), "LanguageName" },
                { new StringContent("testlangname1"), "LanguageDescription" }
            };

            var response = await _httpClient
                .PutAsync("/api/application/langtopics?Id=aboba", content);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
		public async void TestPutTopics()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["TopicName"] = "testtopicname1",
				["TopicDescription"] = "testtopicname1",
			};

			var content = new FormUrlEncodedContent(pairs);

			var response = await _httpClient
				.PutAsync("/api/application/topics?Id=eeae3ff0-21a1-40e9-bd24-1d60824d96b1", content);

			response.EnsureSuccessStatusCode();
		}

        [Fact]
        public async void TestPutTopics_DoesNotExist()
        {
            var cookies = await Login();

            var pairs = new Dictionary<string, string>
            {
                ["TopicName"] = "testtopicname1",
                ["TopicDescription"] = "testtopicname1",
            };

            var content = new FormUrlEncodedContent(pairs);

            var response = await _httpClient
                .PutAsync("/api/application/topics?Id=aboba", content);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
		public async void TestPutThreads()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["ThreadName"] = "testthreadname1",
				["ThreadDescription"] = "testthreadname1",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PutAsync("/api/application/topicthreads?Id=7dc8b582-aece-4348-952f-e9a2c4ee6048", content);

			response.EnsureSuccessStatusCode();
		}

        [Fact]
        public async void TestPutThreads_DoesNotExist()
        {
            var cookies = await Login();

            var pairs = new Dictionary<string, string>
            {
                ["ThreadName"] = "testthreadname1",
                ["ThreadDescription"] = "testthreadname1",
            };

            var content = JsonContent.Create(pairs);

            var response = await _httpClient
                .PutAsync("/api/application/topicthreads?Id=aboba", content);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
		public async void TestPutMessages()
		{
			var cookies = await Login();

			var pairs = new Dictionary<string, string>
			{
				["MessageName"] = "testthreadname1",
				["MessageText"] = "testthreadname1",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PutAsync("/api/application/messages?Id=29c2615d-8c84-4888-9f87-2f52ea4a0554", content);

			response.EnsureSuccessStatusCode();
		}

        [Fact]
        public async void TestPutMessages_DoesNotExist()
        {
            var cookies = await Login();

            var pairs = new Dictionary<string, string>
            {
                ["MessageName"] = "testthreadname1",
                ["MessageText"] = "testthreadname1",
            };

            var content = JsonContent.Create(pairs);

            var response = await _httpClient
                .PutAsync("/api/application/messages?Id=aboba", content);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }
    }

	public class UnitTestAuthEndpoints
	{
		private readonly HttpClient _httpClient;
		private readonly ITestOutputHelper _output;
		private readonly HttpClientHandler _handler;
		public UnitTestAuthEndpoints(ITestOutputHelper output)
		{
			_output = output;
			_handler = new HttpClientHandler()
			{
				CookieContainer = new CookieContainer()
			};
			_httpClient = new HttpClient(_handler)
			{
				BaseAddress = new Uri("https://localhost:7162"),
			};
		}

		private async Task<IEnumerable<Cookie>> Login()
		{
			var pairs = new Dictionary<string, string>
			{
				["Email"] = "admin@pforum.com",
				["Password"] = "admin",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/login", content);

			_output.WriteLine(response.StatusCode.ToString());

			var cookies = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

			return cookies;
		}

		[Fact]
		public async void TestRegister()
		{
			var usersOldJson = await _httpClient
				.GetAsync("/api/application/auth/users");
			var usersOld = JsonSerializer
				.Deserialize<User[]>(await usersOldJson.Content.ReadAsStringAsync());

			var pairs = new Dictionary<string, string>
			{
				["Email"] = "testuser" + Guid.NewGuid().ToString().Split("-")[0],
				["Username"] = "uname",
				["Password"] = "pwrd",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/register", content);

			var usersNewJson = await _httpClient
				.GetAsync("/api/application/auth/users");
			var usersNew = JsonSerializer
				.Deserialize<User[]>(await usersNewJson.Content.ReadAsStringAsync());

			response.EnsureSuccessStatusCode();
			Assert.True(usersNew!.Length == usersOld!.Length + 1);
		}

		[Fact]
		public async void TestRegister_AlreadyExists()
		{
			var usersOldJson = await _httpClient
				.GetAsync("/api/application/auth/users");
			var usersOld = JsonSerializer
				.Deserialize<User[]>(await usersOldJson.Content.ReadAsStringAsync());

			var pairs = new Dictionary<string, string>
			{
				["Email"] = "1",
				["Username"] = "1",
				["Password"] = "1",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/register", content);

			var usersNewJson = await _httpClient
				.GetAsync("/api/application/auth/users");
			var usersNew = JsonSerializer
				.Deserialize<User[]>(await usersNewJson.Content.ReadAsStringAsync());

			Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
			Assert.True(usersNew!.Length == usersOld!.Length);
		}

		[Fact]
		public async Task TestLogin()
		{
			var pairs = new Dictionary<string, string>
			{
				["Email"] = "admin@pforum.com",
				["Password"] = "admin",
			};

			var content = JsonContent.Create(pairs);

			var response = await _httpClient
				.PostAsync("/api/application/auth/login", content);

			var cookies = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

            response.EnsureSuccessStatusCode();
            Assert.True(cookies.Count() == 1);
			Assert.True(cookies.First().Name == "accessToken");

		}

        [Fact]
        public async Task TestLogin_DoesNotExist()
        {
            var cookiesOld = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

            var pairs = new Dictionary<string, string>
            {
                ["Email"] = "dbfbw21bdfbq389r2b3juwbv",
                ["Password"] = "dfwnegonweveerver",
            };

            var content = JsonContent.Create(pairs);

            var response = await _httpClient
                .PostAsync("/api/application/auth/login", content);

            var cookiesNew = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

			Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            Assert.True(
				!cookiesNew.Any() ||
                (cookiesNew.First().Name == "accessToken" && cookiesNew.First().Value == cookiesOld.First().Value)
			);
            //Assert.True(cookies.First().Name == "accessToken");

        }

        [Fact]
        public async Task TestLogin_WrongPassword()
        {
            var cookiesOld = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

            var pairs = new Dictionary<string, string>
            {
                ["Email"] = "admin@pforum.com",
                ["Password"] = "camcoadadca3e33e4fg",
            };

            var content = JsonContent.Create(pairs);

            var response = await _httpClient
                .PostAsync("/api/application/auth/login", content);

            var cookiesNew = _handler.CookieContainer.GetCookies(_httpClient.BaseAddress!).Cast<Cookie>();

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(
                !cookiesNew.Any() ||
                (cookiesNew.First().Name == "accessToken" && cookiesNew.First().Value == cookiesOld.First().Value)
            );
            //Assert.True(cookies.First().Name == "accessToken");

        }
    }
}