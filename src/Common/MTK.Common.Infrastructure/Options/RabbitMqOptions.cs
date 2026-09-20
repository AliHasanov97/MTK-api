namespace MTK.Common.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public string Host { get; init; } = "localhost";
    public ushort Port { get; init; } = 5672;
    public ushort ManagementPort { get; init; } = 15672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
}
