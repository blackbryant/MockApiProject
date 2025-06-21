using System;
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;
using mockAPI.Models;

namespace Test.Mockapi;

public class TestContext
{
    DbContextOptions<AppDbContext> options;

    public TestContext()
    {
        options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        if (options == null)
            throw new Exception("options could not null");

        AddFakeBooks();


    }



    public DbContextOptions<AppDbContext> GetAppDbContext()
    {

        return options;
    }

    public void AddFakeBooks()
    {
         using var context = new AppDbContext(options);
        // 加入 Fake Book 資料
        context.Books.Add(new Book
        {
            Id = 1,
            Title = "Unit Testing in .NET",
            Author = "Bryant Lin",
            PublicationDate = new DateTime(2024, 5, 1),
            ISBN = "1234567890",
            Genre = "Education",
            Summary = "A book about writing unit tests in .NET"
        });
        context.SaveChanges();
     }  



}
