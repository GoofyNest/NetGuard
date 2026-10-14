namespace Module.PacketHandler.Filtering
{
    public static class FilterRegistry
    {
        public static FilterLoader? Loader { get; set; }

        public static IFilter? Resolve(FilterContext ctx, ushort opcode)
            => Loader?.Resolve(ctx, opcode);
    }
}