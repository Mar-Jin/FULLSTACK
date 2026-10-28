using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RevaliNow.Data;
using RevaliNow.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuratie komt uit appsettings.json, appsettings.{Environment}.json en omgevingsvariabelen (in die volgorde).
// Zonder deze waarden kan de API niet veilig draaien, dus dan stoppen we direct met een duidelijke melding.
string Verplicht(string sleutel) =>
    builder.Configuration[sleutel] is { Length: > 0 } waarde
        ? waarde
        : throw new InvalidOperationException($"Configuratiewaarde '{sleutel}' ontbreekt. Zie de README.");

var connectionString = Verplicht("ConnectionStrings:DefaultConnection");
var jwtKey = Verplicht("Jwt:Key");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key moet minstens 32 tekens lang zijn.");

// Add services to the container.
builder.Services.AddDbContext<RevaliNowDbContext>(options =>
    options.UseSqlServer(connectionString,
        sql => sql.EnableRetryOnFailure())); // De database-container kan bij opstarten nog niet klaar zijn

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

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
builder.Services.AddSwaggerGen(options =>
{
    // Maakt een 'Authorize'-knop in Swagger, zodat je met een token de beveiligde endpoints kunt testen
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Plak hier het token uit POST /auth/login"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RevaliNowDbContext>();

    // Maakt de database aan (migraties) en vult hem bij de eerste start met de dataset
    context.Database.Migrate();
    DbSeeder.Seed(context, Verplicht("Seed:PatientWachtwoord"));

    Console.WriteLine("--- Database gemigreerd en gevuld met testdata ---");
}

// app.UseHttpsRedirection(); // Optioneel: in Docker dev vaak uitgezet om SSL gedoe te voorkomen

app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
