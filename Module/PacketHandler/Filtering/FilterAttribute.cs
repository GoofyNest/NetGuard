namespace Module.PacketHandler.Filtering
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FilterAttribute : Attribute
    {
        public ushort Opcode { get; }
        public FilterAttribute(ushort opcode) => Opcode = opcode;
    }
}