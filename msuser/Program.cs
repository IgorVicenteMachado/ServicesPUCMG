using msuser.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.LoadAppSettingsConfig();
builder.ConfigureDependencyInjection();
builder.ConfigureIdentityCore();
builder.ConfigureAuuthenticationScheme();
builder.ConfigureMvc();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwaggerGen();

var app = builder.Build();


//if (app.Environment.IsDevelopment())

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
