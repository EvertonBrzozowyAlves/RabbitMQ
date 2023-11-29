using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/Order", () =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };

    var queueName = "queueName";

    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.QueueDeclare( //creating queue programmatically
        queue: queueName,
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );

    var user = new User(name: "Everton", email: "everton@email.com");
    var order = new Order(user: user);

    var message = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "", //not passing will get the rabbit default
        routingKey: queueName, //if not defined, will assume the queueName, as in QueueDeclare
        basicProperties: null,
        body: body
    );

    return Results.Created($"/Order/{order.Id}", order);
})
.WithName("Create Order")
.WithOpenApi()
.Produces<Order>();

app.Run();
