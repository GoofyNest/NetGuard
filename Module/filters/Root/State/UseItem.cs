using Module.PacketHandler.Filtering;

namespace NetGuard.Hot;

public sealed class UseItem : ISessionState
{
    public DateTime AttackPetTime { get; set; } = DateTime.MinValue;
    public DateTime GrabPetTime { get; set; } = DateTime.MinValue;
    public DateTime HorseTime { get; set; } = DateTime.MinValue;
    public DateTime DispelTime { get; set; } = DateTime.MinValue;

    public static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan DispelCooldown = TimeSpan.FromSeconds(5);

    public bool AnyOnCooldown(out string which, out TimeSpan remaining)
    {
        var now = DateTime.UtcNow;
        foreach (var (name, t, cooldown) in new[] {
            (nameof(AttackPetTime), AttackPetTime, Cooldown),
            (nameof(GrabPetTime),   GrabPetTime, Cooldown),
            (nameof(HorseTime),     HorseTime, Cooldown),
            ("Dispel",             DispelTime, DispelCooldown) })
        {
            var elapsed = now - t;
            if (elapsed < cooldown)
            {
                which = name;
                remaining = cooldown - elapsed;
                return true;
            }
        }
        which = "";
        remaining = TimeSpan.Zero;
        return false;
    }
}
