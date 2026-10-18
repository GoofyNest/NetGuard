using Module.PacketHandler.Filtering;

namespace NetGuard.Hot;

public sealed class UseItem : ISessionState
{
    public DateTime AttackPetTime { get; set; } = DateTime.MinValue;
    public DateTime GrabPetTime { get; set; } = DateTime.MinValue;
    public DateTime HorseTime { get; set; } = DateTime.MinValue;

    public static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(10);

    public bool AnyOnCooldown(out string which, out TimeSpan remaining)
    {
        var now = DateTime.UtcNow;
        foreach (var (name, t) in new[] {
            (nameof(AttackPetTime), AttackPetTime),
            (nameof(GrabPetTime),   GrabPetTime),
            (nameof(HorseTime),     HorseTime) })
        {
            var elapsed = now - t;
            if (elapsed < Cooldown)
            {
                which = name;
                remaining = Cooldown - elapsed;
                return true;
            }
        }
        which = "";
        remaining = TimeSpan.Zero;
        return false;
    }
}