var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hi Sam!!! Recipe Vault API is running!");

app.Run();
