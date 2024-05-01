using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using TeamService.Services;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        TeamApiService teamApiService = new TeamApiService(new Logger<TeamApiService>(new LoggerFactory()));
        [Test]
        public void Test1()
        {
            Assert.Pass();



        }
    }
}