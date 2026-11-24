namespace NetGuard.Hot;

public static class InventoryShops
{
    private static readonly object Gate = new();
    private static string _stamp = "";
    private static Dictionary<string, string[]> _groups = new();
    private static Dictionary<string, string[]> _scraps = new();
    private static List<string[]> _groupMap = new(), _tabMap = new(), _tabs = new(), _goods = new();
    private static readonly string[] Files = { "refshopgroup.txt", "refmappingshopgroup.txt", "refmappingshopwithtab.txt", "refshoptab.txt", "refshopgoods.txt", "refscrapofpackageitem.txt" };
    private static int Number(string value) => int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
    private static void Load()
    {
        string path = Path.GetFullPath(Main.Settings.Data.Path, AppContext.BaseDirectory);
        string stamp = path + string.Join("|", Files.Select(f => File.GetLastWriteTimeUtc(Path.Combine(path, f)).Ticks));
        if (stamp == _stamp) return;
        List<string[]> Rows(string name, int minimum)
        {
            var result = new List<string[]>();
            using var reader = new StreamReader(Path.Combine(path, name), System.Text.Encoding.UTF8, true);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
                var columns = line.Split('\t');
                if (columns[0].Trim() != "1") continue;
                if (columns.Length < minimum) throw new InvalidDataException($"Incomplete shop record in {name}.");
                result.Add(columns);
            }
            return result;
        }
        var groups = Rows(Files[0], 5).ToDictionary(r => r[3], StringComparer.Ordinal);
        var groupMap = Rows(Files[1], 4); var tabMap = Rows(Files[2], 4);
        var tabs = Rows(Files[3], 5); var goods = Rows(Files[4], 5);
        var scraps = Rows(Files[5], 8).ToDictionary(r => r[2], StringComparer.Ordinal);
        _groups = groups; _groupMap = groupMap; _tabMap = tabMap; _tabs = tabs; _goods = goods; _scraps = scraps;
        _stamp = stamp;
    }
    public static bool IsStackable(long id) => Main.Items.TryGetValue((int)id, out var record) && record.TypeID2 == 3;
    private static ItemState Resolve(string group, int shopIndex, int tabIndex, int goodsSlot)
    {
        var shops = _groupMap.Where(r => r[2] == group).Select(r => r[3]).ToList();
        if (shopIndex < 0 || shopIndex >= shops.Count) throw new InvalidDataException("Unknown shop index.");
        var tabs = _tabMap.Where(r => r[2] == shops[shopIndex]).SelectMany(m => _tabs.Where(t => t[4] == m[3])).ToList();
        if (tabIndex < 0 || tabIndex >= tabs.Count) throw new InvalidDataException("Unknown shop tab.");
        var good = _goods.SingleOrDefault(r => r[2] == tabs[tabIndex][3] && Number(r[4]) == goodsSlot)
            ?? throw new InvalidDataException("Unknown shop goods slot.");
        if (!_scraps.TryGetValue(good[3], out var scrap)) throw new InvalidDataException("Missing package item mapping.");
        var record = Main.Items.Values.FirstOrDefault(i => i.CodeName128 == scrap[3])
            ?? throw new InvalidDataException($"Missing itemdata for {scrap[3]}.");
        return new ItemState { Id = record.ID, CodeName128 = record.CodeName128, OptLevel = Number(scrap[4]),
            Variance = unchecked((ulong)long.Parse(scrap[5], System.Globalization.CultureInfo.InvariantCulture)),
            Durability = Number(scrap[6]), MagParamNum = Number(scrap[7]), Amount = IsStackable(record.ID) ? Math.Max(1, Number(scrap[6])) : 1 };
    }
    public static ItemState ResolveNpc(string npcCode, int tab, int slot)
    {
        lock (Gate)
        {
            Load();
            var group = _groups.Values.SingleOrDefault(r => r[4] == npcCode && npcCode.Length > 0)
                ?? throw new InvalidDataException("Selected NPC has not been identified from its spawn packet.");
            return Resolve(group[3], 0, tab, slot);
        }
    }
    public static ItemState ResolveMall(int groupId, int shop, int tab, int slot)
    {
        lock (Gate)
        {
            Load();
            var group = _groups.Values.SingleOrDefault(r => Number(r[2]) == groupId)
                ?? throw new InvalidDataException("Unknown mall group.");
            return Resolve(group[3], shop, tab, slot);
        }
    }
}
