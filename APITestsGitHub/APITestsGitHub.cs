using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace APITestsGitHub
{
    public class APITestsGitHub
    {
        private RestClient client;
        [SetUp]
        public void Setup()
        {
            this.client = new RestClient("https://api.github.com");
            string[] lines = Array.Empty<string>();
            string path = Directory.GetCurrentDirectory();
            path = path.Substring(0, path.IndexOf("APITestsGitHub"));
            foreach (string file in Directory.EnumerateFiles(path, "setup.txt"))
            {
                lines = File.ReadAllLines(file);
            }
            this.client.Authenticator = new HttpBasicAuthenticator(lines[0], lines[1]);
        }

        #region Helper Methods

        public async Task<Issue> CreateIssue(string title, string body)
        {
            //Arrange
            var request = new RestRequest("/repos/TeamAlabala/APISolutionAndPostman/issues");

            //Act
            request.AddBody(new { body, title });
            var responce = await this.client.ExecuteAsync(request, Method.Post);
            var issue = JsonSerializer.Deserialize<Issue>(responce.Content);
            return issue;

        }

        #endregion

        [Test]
        public async Task Test_GitHub_APIRequest()
        {
            //Arrange
            var request = new RestRequest("/repos/TeamAlabala/APISolutionAndPostman/issues");

            //Act
            var responce = await this.client.ExecuteAsync(request,Method.Get);
            var issues = JsonSerializer.Deserialize<List<Issue>>(responce.Content);

            //Assert
            Assert.That(issues.Count > 1);
            foreach (var item in issues)
            {
                Assert.Greater(item.id, 0);
                Assert.Greater(item.number, 0);
                Assert.IsNotEmpty(item.title);
            }

            Assert.AreEqual(HttpStatusCode.OK, responce.StatusCode);

        }      

        [Test]
        public async Task Test_Create_GitHubIssueAsync()
        {
            //Arrange
            string title = "New issue from RestSharp";
            string body = "Something";

            //Act
            var issue = await CreateIssue(title, body);

            //Assert
            Assert.Greater(issue.id, 0);
            Assert.Greater(issue.number, 0);
            Assert.IsNotEmpty(issue.title);
        }

        [TestCase("BG", "1000", "Sofija")]
        [TestCase("BG", "8600", "Jambol")]
        [TestCase("CA", "M5S", "Toronto")]
        [TestCase("DE", "01067", "Dresden")]
        [TestCase("GB", "B1", "Birmingham")]
        public async Task TestZipopotamus(string countryCode,string zipCode,string expextedPlace)
        {
            //Arrange
            client = new RestClient("https://api.zippopotam.us/");
            var request = new RestRequest(countryCode+"/"+zipCode);

            //Act
            var responce = await this.client.ExecuteAsync(request,Method.Get);
            var location = new SystemTextJsonSerializer().Deserialize<Location>(responce);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, responce.StatusCode);
            Assert.AreEqual(countryCode,location.Abbreviation);
            Assert.AreEqual(zipCode,location.PostCode);
            StringAssert.Contains(expextedPlace, location.Places[0].PlaceName);
        }
    }
}