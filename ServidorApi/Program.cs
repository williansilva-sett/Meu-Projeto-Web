using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
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
 
// ── SERVIÇOS ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEntradaService, EntradaService>();
builder.Services.AddScoped<ISaidaService, SaidaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddSingleton<ISenhaHasher, SenhaHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IMetaService, MetaService>();
 
// ── BANCO DE DADOS ────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
 
// ── CONTROLLERS, SWAGGER ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VivaFinanças API", Version = "v1" });
 
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
 
// ── AUTOMAPPER, FLUENTVALIDATION ──────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
 
// ── JWT ───────────────────────────────────────────────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));
 
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração Jwt não encontrada.");
 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidAudience            = jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
 
builder.Services.AddAuthorization();
 
// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5079",
                "http://127.0.0.1:5500",
                "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
 
// ── PIPELINE ──────────────────────────────────────────────────────────────────
var app = builder.Build();
 
await SeedAdminAsync(app);
 
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("Frontend");
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "Cliente")),
    RequestPath = "/Cliente"
});
 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
 
// ── SEED DO ADMIN ─────────────────────────────────────────────────────────────
static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context      = scope.ServiceProvider.GetRequiredService<DataContext>();
    var senhaHasher  = scope.ServiceProvider.GetRequiredService<ISenhaHasher>();
    var config       = scope.ServiceProvider.GetRequiredService<IConfiguration>();
 
    var emailAdmin = config["AdminSeed:Email"];
    if (string.IsNullOrWhiteSpace(emailAdmin)) return;
 
    // Verifica se já existe uma Conta com esse email — não mais no Usuario
    var jaExiste = await context.Contas.AnyAsync(c => c.Email == emailAdmin);
    if (jaExiste) return;
 
    var senhaPlana = config["AdminSeed:Senha"];
    if (string.IsNullOrWhiteSpace(senhaPlana)) return;
 
    // Cria o Usuario com dados pessoais do admin
    var usuarioAdmin = new Usuario
    {
        Nome      = config["AdminSeed:Nome"]      ?? "Admin",
        Sobrenome = config["AdminSeed:Sobrenome"] ?? "Sistema",
        Idade     = int.TryParse(config["AdminSeed:Idade"], out var idade) ? idade : 30,
        Telefone  = config["AdminSeed:Telefone"]  ?? "00000000000"
    };
 
    context.Usuarios.Add(usuarioAdmin);
    await context.SaveChangesAsync(); // Salva para gerar o ID
 
    // Cria a Conta de sistema do admin vinculada ao Usuario
    var contaAdmin = new Conta
    {
        Email       = emailAdmin.ToLower().Trim(),
        Senha       = senhaHasher.Hash(senhaPlana),
        Tipo        = TipoUsuario.Admin,
        Ativo       = true,
        DataCriacao = DateTime.Now,
        UsuarioID   = usuarioAdmin.ID
    };
 
    context.Contas.Add(contaAdmin);
    await context.SaveChangesAsync();
}
 
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}