namespace Module.Networking
{
    public sealed record BusMessage(string Kind, object? Payload = null);
}