using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Bogus;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using mockAPI.Config;
using mockAPI.DataContext;
using mockAPI.Middleware;
using mockAPI.Middleware.Setting;
using mockAPI.Models;
using mockAPI.Repositories;
using mockAPI.Services;
using mockAPI.Validator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


builder.Services.AddScoped<IEventRegistrationsRepository, EventRegistrationsRepository>();
builder.Services.AddScoped<IProductReadService, ProductReadService>();
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IEventRegistractionsService, EventRegistractionsService>();

builder.Services.AddCors(options => 
{
    options.AddPolicy("CorsPolicy", builder => 
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination");
    });
});

//var connection = new SqliteConnection("DataSource=:memory:");
//connection.Open();

// Register the AppDbContext
Console.WriteLine(Directory.GetCurrentDirectory());
string connectionString = "Data Source=E:SqliteDB.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
    
builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Setting Validate
// 一個個設定
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddValidatorsFromAssemblyContaining<EventRegistrationDTOValidator>();

// Setting all validates for package
var validator = typeof(mockAPI.Validator.EventRegistrationDTOValidator).Assembly;
builder.Services.AddValidatorsFromAssembly(validator);

var connectionStringBuilder = new SqliteConnectionStringBuilder();
connectionStringBuilder.DataSource = connectionString;
builder.Services.AddTransient<IDbConnection>(sp => new SqliteConnection(connectionString));


builder.Services.AddConfigureProblemDetails();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<EventRegistrationDTOValidator>();



///JWT Settings
builder.Services.AddJwtAuthentication(builder.Configuration);

// ForwardedHeaders Middleware
builder.Services.AddForwardedHeaders();


// builder.Services.AddHealthChecks()
//     .AddCheck<DatabasePerformanceHealthCheck>("sqliteDB", 
//         tags: ["database"]);

var app = builder.Build();


#region 建立memory資料庫
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     dbContext.Database.EnsureCreated();
//     if (!dbContext.Products.Any())
//     {
//         var ProductsFaker = new Faker<Product>()
//             .RuleFor(p => p.Name, f => f.Commerce.ProductName())
//             .RuleFor(p => p.Price, f => f.Finance.Amount(50, 2000))
//             .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 5));
//         var products = ProductsFaker.Generate(100);
//         dbContext.Products.AddRange(products);
//         dbContext.SaveChanges();

//     }




//     if (!dbContext.EventRegistrations.Any())
//     {
//         var faker = new Faker<EventRegistration>()
//             .RuleFor(e => e.Id, f => f.Random.Guid().ToString())
//             .RuleFor(e => e.FullName, f => f.Name.FullName())
//             .RuleFor(e => e.Email, f => f.Internet.Email())
//             .RuleFor(e => e.EventName, f => f.Lorem.Word())
//             .RuleFor(e => e.EventDate, f => f.Date.Future())
//             .RuleFor(e => e.DaysAttending, f => f.Random.Int(1, 7))
//             .RuleFor(e => e.Notes, f => f.Lorem.Sentence());

//         var registrations = faker.Generate(1000);
//         dbContext.EventRegistrations.AddRange(registrations);
//         dbContext.SaveChanges();

//     }

// }

#endregion

using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
{
    connection.Open();

    var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS EventRegistrations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                GUID TEXT,
                FullName TEXT,
                Email TEXT,
                EventName TEXT,
                EventDate TEXT,
                DaysAttending INTEGER,
                Notes TEXT
            )";
        createTableCommand.ExecuteNonQuery();



        var checkTableCommand = connection.CreateCommand();
        checkTableCommand.CommandText = "SELECT COUNT(*) FROM EventRegistrations";
        var count = Convert.ToInt64(checkTableCommand.ExecuteScalar());

        if (count == 0)
        {
            var faker = new Faker<EventRegistration>()
                .RuleFor(e => e.GUID, f => Guid.NewGuid())
                .RuleFor(e => e.FullName, f => f.Name.FullName())
                .RuleFor(e => e.Email, f => f.Internet.Email())
                .RuleFor(e => e.EventName, f => f.Lorem.Word())
                .RuleFor(e => e.EventDate, f => f.Date.Future())
                .RuleFor(e => e.DaysAttending, f => f.Random.Int(1, 7))
                .RuleFor(e => e.Notes, f => f.Lorem.Sentence());

            var registrations = faker.Generate(10000);

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO EventRegistrations (GUID, FullName, Email, EventName, EventDate, DaysAttending, Notes)
                VALUES (@GUID, @FullName, @Email, @EventName, @EventDate, @DaysAttending, @Notes)";

            foreach (var registration in registrations)
            {
                insertCommand.Parameters.Clear();
                insertCommand.Parameters.AddWithValue("@GUID", registration.GUID);
                insertCommand.Parameters.AddWithValue("@FullName", registration.FullName);
                insertCommand.Parameters.AddWithValue("@Email", registration.Email);
                insertCommand.Parameters.AddWithValue("@EventName", registration.EventName);
                insertCommand.Parameters.AddWithValue("@EventDate", registration.EventDate.ToString("yyyy-MM-dd"));
                insertCommand.Parameters.AddWithValue("@DaysAttending", registration.DaysAttending);
                insertCommand.Parameters.AddWithValue("@Notes", registration.Notes);
                insertCommand.ExecuteNonQuery();
            }
        }

    // createTableCommand.CommandText = @"
    //     CREATE TABLE IF NOT EXISTS Books (
    //         Id INT PRIMARY KEY IDENTITY(1,1),
    //         Title NVARCHAR(255) NOT NULL,
    //         Author NVARCHAR(255) NOT NULL,
    //         PublicationDate DATETIME NOT NULL,
    //         ISBN NVARCHAR(13) NOT NULL,
    //         Genre NVARCHAR(100) NOT NULL,
    //         Summary NVARCHAR(MAX)
    //     )";
    // createTableCommand.ExecuteNonQuery();
    // var checkTableCommand = connection.CreateCommand();
    // checkTableCommand.CommandText = "SELECT COUNT(*) FROM Books";
    // var count = Convert.ToInt64(checkTableCommand.ExecuteScalar());

    // if (count == 0)
    // {
    //     var faker = new Faker<Book>()
    //             .RuleFor(b => b.Title, f => f.Lorem.Sentence(3, 5))
    //             .RuleFor(b => b.Author, f => f.Name.FullName())
    //             .RuleFor(b => b.PublicationDate, f => f.Date.Past(50))
    //             .RuleFor(b => b.ISBN, f => f.Random.Replace("###-#-####-####-#"))
    //             .RuleFor(b => b.Genre, f => f.PickRandom(new[] 
    //             { 
    //                 "科幻", "奇幻", "推理", "恐怖", "愛情", 
    //                 "歷史", "商業", "科普", "自助", "傳記" 
    //             }))
    //             .RuleFor(b => b.Summary, f => f.Lorem.Paragraph(3));


    //     var books = faker.Generate(100);
    //     var insertCommand = connection.CreateCommand();
    //         insertCommand.CommandText = @"
    //             INSERT INTO Books (Title, Author, PublicationDate, ISBN, Genre, Summary)
    //             VALUES (@Title, @Author, @PublicationDate, @ISBN, @Genre, @Summary)";


    //     foreach (var book in books)
    //     {
    //         insertCommand.Parameters.Clear();
    //         insertCommand.Parameters.AddWithValue("@Title", book.Title);
    //         insertCommand.Parameters.AddWithValue("@Author", book.Author);
    //         insertCommand.Parameters.AddWithValue("@PublicationDate", book.PublicationDate);
    //         insertCommand.Parameters.AddWithValue("@ISBN", book.ISBN);
    //         insertCommand.Parameters.AddWithValue("@Genre", book.Genre);
    //         insertCommand.Parameters.AddWithValue("@Summary", book.Summary);
    //         insertCommand.ExecuteNonQuery();
    //     }
    //}

}


app.MapOpenApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
   {
       options.SwaggerEndpoint("/openapi/v1.json", "v1");
   });

}

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.MapHealthChecks("/api/health", new HealthCheckOptions()
// {
//     ResultStatusCodes =
//     {
//         [HealthStatus.Healthy] = StatusCodes.Status200OK,
//         [HealthStatus.Degraded] = StatusCodes.Status200OK,
//         [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable

//     },
//     ResponseWriter = async (context, report) =>
//     {
//         context.Response.ContentType = "application/json";
//         var response = new
//         {
//             status = report.Status.ToString(),
//             checks = report.Entries.Select(entry => new
//             {
//                 name = entry.Key,
//                 status = entry.Value.Status.ToString(),
//                 description = entry.Value.Description,
//                 duration = entry.Value.Duration
//             }),
//             totalDuration = report.TotalDuration

//         };
//         await context.Response.WriteAsJsonAsync(response);
//     }


// });

 




app.Run();

 