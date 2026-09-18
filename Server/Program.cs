using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RevaliNowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure())); // De database-container kan bij opstarten nog niet klaar zijn

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost") // Voeg localhost (proxy) toe
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RevaliNowDbContext>();
    
    // Maakt de database aan en vult hem met de testdata uit HasData (RevaliNowDbContext)
    context.Database.Migrate();

    Console.WriteLine("--- Database gemigreerd en gevuld met testdata ---");
}

// app.UseHttpsRedirection(); // Optioneel: in Docker dev vaak uitgezet om SSL gedoe te voorkomen

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

app.Run();