using DotNetAIChatbot.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<
    DotNetAIChatbot.Services.OllamaService>(client =>
    {
        client.BaseAddress =
            new Uri("http://localhost:11434/");

        client.Timeout = TimeSpan.FromMinutes(3);
    });

builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ChatbotConnection")
    )
);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
