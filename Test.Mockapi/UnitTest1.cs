using System.Net.Cache;
using mockAPI.DataContext;
using mockAPI.Repositories;
using mockAPI.Services;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Test.Mockapi;

public class UnitTest1
{
   private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    private TestContext testContext = new TestContext();


    [Fact]
    public async Task Test1()
    {
            var dbContext = new  AppDbContext(testContext.GetAppDbContext());
            var mockRepo = new Mock<BooksRepository>(dbContext);
            
            var bookService = new BookService(mockRepo.Object);


            // Act
            var result = await bookService.GetByIdAsync(1);

             _output.WriteLine("AAAAAAAAAAA");
             _output.WriteLine(result?.ToString() ?? "null");
            // Assert
            Assert.NotNull(result);
            //Assert.Equal(3, result.Count);
    }
}
