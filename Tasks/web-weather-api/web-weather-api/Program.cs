using web_weather.Services;
using web_weather.Utils;
using webweather.Services;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IWeatherService, OpenWeatherMapWeatherService>();
    builder.Services.AddScoped<IWeatherService, StormGlassWeatherService>();
    builder.Services.AddScoped<ISettings, Settings>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    app.Run();
}
