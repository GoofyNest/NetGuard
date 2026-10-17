namespace Module.PacketHandler.Filtering
{
    /// <summary>
    /// Marker interface — every file under filters/Root/State/ that defines
    /// a class implementing this becomes a property on the generated SessionHost.
    /// The property name is the class name with the trailing "State" removed.
    /// </summary>
    public interface ISessionState { }
}