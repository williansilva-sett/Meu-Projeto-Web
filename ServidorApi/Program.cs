using Microsoft.EntityFrameworkCore;
using ServidorApi.Data; // Namespace onde está seu DataContext.cs
using ServidorApi.Services.Interfaces;
using ServidorApi.Services;
using ServidorApi.Mappings;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.FileProviders;
using FluentValidation;
using ServidorApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServidorApi.Configuration;


var builder = WebApplication.CreateBuilder(args);
// Adicione o registro da Injeção de Dependência
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAdminService, AdminService>();


// Configuração da conexão com o Banco de Dados (MySQL)
//O C# precisa saber que esse DataContext existe e que ele deve usar o MySQL.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VivaFinanças API", Version = "v1" });

    // Adiciona o campo "Bearer token" na UI do Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Informe o token JWT retornado pelo /api/auth/login"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
builder.Services.AddScoped<IEntradaService, EntradaService>();
builder.Services.AddScoped<ISaidaService, SaidaService>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
/*builder.Services.AddScoped<ISaldoContaService, SaldoContaService>();*/
builder.Services.AddSingleton<ISenhaHasher, SenhaHasher>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração Jwt não encontrada.");

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS — permite que o frontend acesse a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            // Origens permitidas: API servindo os arquivos + Live Server do VS Code
            .WithOrigins(
                "http://localhost:5079",   // Sua API (arquivos estáticos)
                "http://127.0.0.1:5500",  // Live Server padrão do VS Code
                "http://localhost:5500"    // Live Server alternativo
            )
            .AllowAnyHeader()  // Permite qualquer header (incluindo Authorization: Bearer)
            .AllowAnyMethod(); // Permite GET, POST, PUT, PATCH, DELETE
    });
});

    
var app = builder.Build();

await SeedAdminAsync(app);

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("Frontend"); // ← adicionar aqui
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

// Serve arquivos da pasta ../Cliente relativa ao projeto
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "Cliente")
    ),
    RequestPath = "/Cliente" // URL de acesso: http://localhost:5079/Cliente/...
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var senhaHasher = scope.ServiceProvider.GetRequiredService<ISenhaHasher>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var emailAdmin = config["AdminSeed:Email"];
    if (string.IsNullOrWhiteSpace(emailAdmin))
        return;

    var jaExiste = await context.Usuarios.AnyAsync(u => u.Email == emailAdmin);
    if (jaExiste)
        return;

    var senhaPlana = config["AdminSeed:Senha"];
    if (string.IsNullOrWhiteSpace(senhaPlana))
        return;

    var admin = new Usuario
    {
        Nome = config["AdminSeed:Nome"] ?? "Admin",
        Sobrenome = config["AdminSeed:Sobrenome"] ?? "Sistema",
        Idade = int.TryParse(config["AdminSeed:Idade"], out var idade) ? idade : 30,
        Telefone = config["AdminSeed:Telefone"] ?? "00000000000",
        Email = emailAdmin,
        Senha = senhaHasher.Hash(senhaPlana),
        Tipo = TipoUsuario.Admin,
        DataCriacao = DateTime.Now
    };

    context.Usuarios.Add(admin);
    await context.SaveChangesAsync();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
