var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/venue/swagger/v1/swagger.json", "🎫 Venue Service API");
        c.SwaggerEndpoint("/event/swagger/v1/swagger.json", "📅 Event Service API");
        c.SwaggerEndpoint("/ticket/swagger/v1/swagger.json", "🎟️ Ticket Service API");

        c.DocumentTitle = "🎯 Concert Ticket Management - API Gateway";
        c.RoutePrefix = "swagger";
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapReverseProxy();
app.Run();
