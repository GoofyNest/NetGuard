namespace Module.PacketHandler.Filtering
{
    public static class BusRegistry
    {
        public static BusDispatcher Dispatcher { get; } = new();
    }
}