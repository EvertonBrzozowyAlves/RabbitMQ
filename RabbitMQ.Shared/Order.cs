using System.Text.Json;

namespace RabbitMQ.Shared;

public class Order
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }

    public Order(User user)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        User = user;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}
