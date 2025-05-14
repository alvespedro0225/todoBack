using API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                      });
});

var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();
app.MapTodosEndpoints();
app.Run();
