using Module.Networking;

namespace NetGuard.Hot;

public static class SessionBridge
{
    public static void Attach(SessionData s)
    {
        s.Host = new SessionHost();
    }

    // No-op. All shell-owned fields stay on SessionData; nothing to mirror.
    public static void Sync(SessionData s) { }

    public static void Migrate(object? oldHost, object? newHost)
    {
        if (oldHost is null || newHost is null) return;
        if (newHost is not SessionHost @new) return;

        var oldT = oldHost.GetType();

        // --- Agent data ---
        var oldAg = oldT.GetProperty("Agent")?.GetValue(oldHost);
        if (oldAg is not null)
        {
            var agT = oldAg.GetType();

            var oldChars = agT.GetProperty("Chars")?.GetValue(oldAg) as System.Collections.IEnumerable;
            if (oldChars is not null)
            {
                foreach (var oldChar in oldChars)
                {
                    if (oldChar is null) continue;
                    var cs = new CharState();
                    CopyCharScalars(oldChar.GetType(), oldChar, cs);
                    @new.Agent.Chars.Add(cs);
                }
            }
        }
    }

    private static void CopyCharScalars(Type srcT, object src, CharState dst)
    {
        object? Get(string n) => srcT.GetProperty(n)?.GetValue(src);

        dst.Charname         = (string)(Get("Charname")         ?? "");
        dst.IsVisible        = (bool)  (Get("IsVisible")        ?? false);
        dst.ServerTime       = (long)  (Get("ServerTime")       ?? 0L);
        dst.RefObjID         = (long)  (Get("RefObjID")         ?? 0L);
        dst.Scale            = (int)   (Get("Scale")            ?? 0);
        dst.CurLevel         = (int)   (Get("CurLevel")         ?? 0);
        dst.MaxLevel         = (int)   (Get("MaxLevel")         ?? 0);
        dst.ExpOffset        = (ulong) (Get("ExpOffset")        ?? 0UL);
        dst.SExpOffset       = (long)  (Get("SExpOffset")       ?? 0L);
        dst.RemainGold       = (ulong) (Get("RemainGold")       ?? 0UL);
        dst.RemainSkillPoint = (long)  (Get("RemainSkillPoint") ?? 0L);
        dst.RemainStatPoint  = (int)   (Get("RemainStatPoint")  ?? 0);
        dst.RemainHwanCount  = (int)   (Get("RemainHwanCount")  ?? 0);
        dst.GatheredExpPoint = (long)  (Get("GatheredExpPoint") ?? 0L);
        dst.Hp               = (long)  (Get("Hp")               ?? 0L);
        dst.Mp               = (long)  (Get("Mp")               ?? 0L);
        dst.AutoInverstExp   = (int)   (Get("AutoInverstExp")   ?? 0);
        dst.DailyPk          = (int)   (Get("DailyPk")          ?? 0);
        dst.TotalPk          = (int)   (Get("TotalPk")          ?? 0);
        dst.PkPenaltyPoint   = (long)  (Get("PkPenaltyPoint")   ?? 0L);
        dst.HwanLevel        = (int)   (Get("HwanLevel")        ?? 0);
        dst.PvpCape          = (int)   (Get("PvpCape")          ?? 0);
        dst.Timestamp        = (DateTime)(Get("Timestamp")      ?? DateTime.UtcNow);

        var oldInv = srcT.GetProperty("Inventory")?.GetValue(src);
        if (oldInv is not null)
        {
            var invT = oldInv.GetType();
            dst.Inventory = new Inventory
            {
                Size      = (int)(invT.GetProperty("Size")?.GetValue(oldInv)      ?? 0),
                ItemCount = (int)(invT.GetProperty("ItemCount")?.GetValue(oldInv) ?? 0),
            };
        }
    }
}