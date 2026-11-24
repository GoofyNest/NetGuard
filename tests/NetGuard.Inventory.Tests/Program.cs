using NetGuard.Hot;
using Module.Helpers.ClientData;
using App = Module.Main;
using Inventory = NetGuard.Hot.Inventory;
int passed = 0;
void Check(bool value, string message) { if (!value) throw new Exception(message); passed++; }
App.Settings.ServerType.CurrentValue = "jSRO";
App.Items[100] = new ItemData { ID = 100, CodeName128 = "STACK_A", TypeID2 = 3, MaxStack = 100 };
App.Items[101] = new ItemData { ID = 101, CodeName128 = "STACK_B", TypeID2 = 3, MaxStack = 100 };
App.Items[200] = new ItemData { ID = 200, CodeName128 = "SWORD", TypeID2 = 1, MaxStack = 1 };
var session = new SessionData(); SessionBridge.Attach(session); session.PlayerInfo.CurrentCharName = "test";
var character = session.Host()!.Agent.EnsureCurrent("test");
Inventory Reset(params (int slot,int id,int amount)[] items)
{
    character.Inventory = new Inventory { Size = 40 };
    foreach(var i in items) character.Inventory.Put(new ItemState { Slot=i.slot, Id=i.id, CodeName128=App.Items[i.id].CodeName128, Amount=i.amount, Durability=50, OptLevel=3 });
    return character.Inventory;
}
Packet Packet(ushort opcode, Action<Packet> write) { var p=new Packet(opcode);write(p);p.Lock();return p; }
void Apply(ushort opcode, Action<Packet> write) => InventoryTracking.Apply(Packet(opcode,write), session);
void Move(int from,int to,int amount) => Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(0);p.WriteUInt8((byte)from);p.WriteUInt8((byte)to);p.WriteUInt16((ushort)amount);});
void Stack(Packet p,int slot,int id,int amount,bool reason=false)
{
    p.WriteUInt8((byte)slot); if(reason)p.WriteUInt8(0);
    if(InventoryItemReader.Vsro)p.WriteUInt32(0);
    p.WriteUInt32((uint)id);p.WriteUInt16((ushort)amount);
}
var inv=Reset((13,100,20)); Move(13,14,20);
Check(inv.GetItem(13)==null&&inv.GetItem(14)?.Amount==20&&inv.ItemCount==1,"whole move");
Move(14,15,6);Check(inv.GetItem(14)?.Amount==14&&inv.GetItem(15)?.Amount==6&&inv.ItemCount==2,"split");
Move(15,14,6);Check(inv.GetItem(15)==null&&inv.GetItem(14)?.Amount==20&&inv.ItemCount==1,"merge removes empty source");
inv=Reset((13,100,20),(14,101,30));Move(13,14,20);Check(inv.GetItem(13)?.Id==101&&inv.GetItem(14)?.Id==100,"swap different items");
inv=Reset((13,200,1),(14,200,1));inv.GetItem(14)!.OptLevel=9;Move(13,14,1);Check(inv.GetItem(13)?.OptLevel==9&&inv.GetItem(14)?.OptLevel==3,"identical equipment swaps");
inv=Reset((13,100,90),(14,100,80));Move(13,14,90);Check(inv.GetItem(13)?.Amount==80&&inv.GetItem(14)?.Amount==90,"full stacks swap");
inv=Reset((13,100,20));Apply(0xB034,p=>{p.WriteUInt8(2);p.WriteUInt8(9);});Check(inv.GetItem(13)?.Amount==20&&inv.IsSynchronized,"failed operation unchanged");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(0);p.WriteUInt8(13);p.WriteUInt8(14);p.WriteUInt16(20);p.WriteUInt8(1);});
Check(inv.GetItem(13)?.Amount==20&&inv.GetItem(14)==null&&!inv.IsSynchronized,"truncated secondary move rolls back first move");
inv=Reset((13,100,20));Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(9);p.WriteUInt8(13);p.WriteUInt16(7);p.WriteUInt32(999);p.WriteUInt8(1);});
Check(inv.GetItem(13)?.Amount==13&&inv.Container("buyback").GetItem(0)?.Amount==7,"partial sale");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(34);p.WriteUInt8(14);p.WriteUInt8(0);p.WriteUInt16(7);});
Check(inv.GetItem(14)?.Id==100&&inv.GetItem(14)?.Amount==7&&inv.Container("buyback").ItemCount==0,"buyback identity");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(7);p.WriteUInt8(14);});Check(inv.GetItem(14)==null&&inv.ItemCount==1,"drop");
Apply(0xB04C,p=>{p.WriteUInt8(1);p.WriteUInt8(13);p.WriteUInt16(0);});Check(inv.ItemCount==0,"use last stack item");
inv=Reset();Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(6);Stack(p,13,100,12);});Check(inv.GetItem(13)?.Id==100&&inv.GetItem(13)?.Amount==12,"pickup parses identity and amount");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(6);Stack(p,13,100,18);});Check(inv.ItemCount==1&&inv.GetItem(13)?.Amount==18,"pickup stack replaces amount without duplicate slot");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(14);Stack(p,14,101,8,true);});Check(inv.GetItem(14)?.Id==101,"server add");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(28);p.WriteUInt32(8);Stack(p,15,100,9);});Check(inv.GetItem(15)?.Amount==9,"party pickup");
Apply(0x3040,p=>{p.WriteUInt8(13);p.WriteUInt8(1|2|4|8|16|32|64);p.WriteUInt32(101);p.WriteUInt8(6);p.WriteUInt64(777);p.WriteUInt16(4);p.WriteUInt32(99);p.WriteUInt8(3);p.WriteUInt8(1);p.WriteUInt32(11);p.WriteUInt32(22);});
Check(inv.GetItem(13) is { Id:101, OptLevel:6, Variance:777, Amount:4, Durability:99, State:3, MagParamNum:1, CodeName128:"STACK_B" },"all update flags and wire order");
Apply(0x3052,p=>{p.WriteUInt8(13);p.WriteUInt32(44);});Check(inv.GetItem(13)?.Durability==44,"durability");
Apply(0x3092,p=>{p.WriteUInt8(1);p.WriteUInt8(60);});Check(inv.Size==60,"inventory expansion");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(2);p.WriteUInt8(13);p.WriteUInt8(0);});Check(inv.GetItem(13)==null&&inv.Container("storage").GetItem(0)?.Id==101,"deposit");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(3);p.WriteUInt8(0);p.WriteUInt8(16);});Check(inv.GetItem(16)?.Id==101&&inv.Container("storage").ItemCount==0,"withdraw");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(27);p.WriteUInt32(555);p.WriteUInt8(16);p.WriteUInt8(3);});
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(26);p.WriteUInt32(555);p.WriteUInt8(3);p.WriteUInt8(17);});Check(inv.GetItem(17)?.Id==101&&inv.GetItem(16)==null,"pet transfer round trip");
Apply(0x3047,p=>p.WriteUInt64(0));Apply(0x3049,p=>{p.WriteUInt8(150);p.WriteUInt8(1);Stack(p,3,100,40);});Apply(0x3048,p=>{});
Check(inv.Container("storage").Size==150&&inv.Container("storage").GetItem(3)?.Amount==40,"storage snapshot");
var before=inv.GetItem(17)!.Amount;Apply(0x3040,p=>{p.WriteUInt8(17);p.WriteUInt8(8|16);p.WriteUInt16(88);});Check(inv.GetItem(17)?.Amount==before&&!inv.IsSynchronized,"truncated flag update rollback");
App.Settings.ServerType.CurrentValue="vSRO";inv=Reset();Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(6);Stack(p,13,100,33);});Check(inv.GetItem(13)?.Amount==33,"vSRO rental prefix");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(6);p.WriteUInt8(14);p.WriteUInt32(0);p.WriteUInt32(200);p.WriteUInt8(7);p.WriteUInt64(9);p.WriteUInt32(80);p.WriteUInt8(0);p.WriteUInt8(0);p.WriteUInt8(1);p.WriteUInt8(0);p.WriteUInt8(2);});Check(inv.GetItem(14)?.OptLevel==7&&inv.GetItem(14)?.Durability==80,"vSRO equipment bindings");
App.Settings.ServerType.CurrentValue="cSRO";Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(6);Stack(p,15,100,9);});Check(inv.GetItem(15)==null&&!inv.IsSynchronized,"unsupported layout cannot corrupt slots");
App.Settings.ServerType.CurrentValue="jSRO";
var newHost=new SessionHost();SessionBridge.Migrate(session.Host,newHost);var migrated=newHost.Agent.Current("test")!.Inventory;Check(migrated.GetItem(13)?.Amount==33&&migrated.GetItem(14)?.OptLevel==7,"hot reload preserves inventory quantities");
// A packet copy must leave the forwarded wire data and cursor untouched.
var raw=Packet(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(15);p.WriteUInt8(13);});var bytes=raw.GetBytes().ToArray();InventoryTracking.Apply(raw,session);Check(raw.ReadUInt8()==1&&raw.GetBytes().SequenceEqual(bytes),"server packet untouched");
// The new server exception guard must absorb malformed packets at the filter boundary.
new NetGuard.Filters.Agent.Server.InventoryOperationResponse().Handle(Packet(0xB034,p=>{}),session,null!);passed++;
Console.WriteLine($"PASS: {passed} inventory regression checks.");
App.Settings.Data.Path = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(Directory.GetCurrentDirectory(), "build", "Debug", "client_data");
foreach(var line in File.ReadLines(Path.Combine(App.Settings.Data.Path,"itemdata.txt")))
{
    var f=line.Split('\t');
    if(f.Length<13||f[0]!="1"||!int.TryParse(f[1],out var id))continue;
    App.Items.TryAdd(id,new ItemData { ID=id,CodeName128=f[2],TypeID2=byte.Parse(f[10]),TypeID3=byte.Parse(f[11]),TypeID4=byte.Parse(f[12]),MaxStack=100 });
}
var groups=File.ReadLines(Path.Combine(App.Settings.Data.Path,"refshopgroup.txt")).Select(s=>s.Split('\t')).Where(f=>f.Length>4&&f[0]=="1").ToArray();
ItemState? shopTemplate=null;string[]? selectedGroup=null;
foreach(var group in groups.Where(g=>g[4]!="xxx"))
{
    try { shopTemplate=InventoryShops.ResolveNpc(group[4],0,0);selectedGroup=group;break; }
    catch(InvalidDataException) { }
}
Check(shopTemplate!=null&&selectedGroup!=null,"real NPC shop files resolve item identity");
inv=Reset();character.SelectedNpcCode=selectedGroup![4];
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(8);p.WriteUInt8(0);p.WriteUInt8(0);p.WriteUInt8(1);p.WriteUInt8(13);p.WriteUInt16(2);});
Check(inv.GetItem(13)?.Id==shopTemplate!.Id&&inv.IsSynchronized,"NPC purchase from real references");
ItemState? mallTemplate=null;string[]? mallGroup=null;
foreach(var group in groups.Where(g=>g[3].Contains("MALL")))
{
    try { mallTemplate=InventoryShops.ResolveMall(int.Parse(group[2]),0,0,0);mallGroup=group;break; }
    catch(InvalidDataException) { }
}
Check(mallTemplate!=null&&mallGroup!=null,"real item mall references resolve");
Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(24);p.WriteUInt16(ushort.Parse(mallGroup![2]));p.WriteUInt8(0);p.WriteUInt8(0);p.WriteUInt8(0);p.WriteUInt8(1);p.WriteUInt8(14);p.WriteUInt16(1);});
Check(inv.GetItem(14)?.Id==mallTemplate!.Id&&inv.IsSynchronized,"mall purchase from real references");
inv=Reset((7,100,20));Apply(0x3201,p=>p.WriteUInt16(0));Check(inv.ItemCount==0,"ammo depletion");
inv=Reset((13,100,10));Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(9);p.WriteUInt8(13);p.WriteUInt16(10);p.WriteUInt32(999);p.WriteUInt8(255);});Check(inv.ItemCount==0,"full sale removes slot");
inv=Reset((13,200,1),(7,100,10));Apply(0xB034,p=>{p.WriteUInt8(1);p.WriteUInt8(0);p.WriteUInt8(13);p.WriteUInt8(6);p.WriteUInt16(0);p.WriteUInt8(1);p.WriteUInt8(0);p.WriteUInt8(7);p.WriteUInt8(14);p.WriteUInt16(10);});Check(inv.GetItem(6)?.Id==200&&inv.GetItem(14)?.Amount==10&&inv.GetItem(7)==null,"paired equipment and ammo move");
Console.WriteLine($"PASS: {passed} checks including actual shop reference files.");
App.Chars[300]=new CharData {ID=300,CodeName128=selectedGroup![4],TypeID1=1,TypeID2=2,TypeID3=2};
InventoryNpcTracking.Apply(Packet(0x3015,p=>{p.WriteUInt32(300);p.WriteUInt32(900);}),session);
InventoryNpcTracking.Apply(Packet(0xB045,p=>{p.WriteUInt8(1);p.WriteUInt32(900);}),session);
Check(character.SelectedNpcUniqueID==900&&character.SelectedNpcCode==selectedGroup[4],"NPC identity and confirmed selection");
var legacy = new {Agent=new {Chars=new[]{new {Charname="legacy",Inventory=new {Size=40,ItemCount=1,Items=new[]{new {Slot=13,Id=100L,CodeName128="STACK_A"}}}}}}};
var legacyTarget=new SessionHost();SessionBridge.Migrate(legacy,legacyTarget);
Check(legacyTarget.Agent.Current("legacy")!.Inventory.GetItem(13) is {AmountKnown:false} && !legacyTarget.Agent.Current("legacy")!.Inventory.IsSynchronized,"legacy quantities explicitly invalidated");
Console.WriteLine($"PASS: {passed} total inventory checks.");
var diagnosticInventory = new Inventory();
diagnosticInventory.Invalidate("Item details unknown at slot 17.");
var diagnostic = InventoryTracking.TakeSyncFailureMessage(diagnosticInventory, "test-character", 0xB034, 0x1A);
Check(diagnostic != null && diagnostic.Contains("OUT OF SYNC") && diagnostic.Contains("character=test-character")
    && diagnostic.Contains("opcode=0xB034") && diagnostic.Contains("operation=0x1A")
    && diagnostic.Contains("version=jSRO") && diagnostic.Contains("slot 17"), "sync log identifies character, opcode, operation, version and reason");
Check(InventoryTracking.TakeSyncFailureMessage(diagnosticInventory, "test-character", 0xB034, 0x1A) == null,
    "identical sync failure is not repeated");
diagnosticInventory.Invalidate("Unknown item reference 123.");
Check(InventoryTracking.TakeSyncFailureMessage(diagnosticInventory, "test-character", 0x3040)!.Contains("Unknown item reference 123"),
    "new failure reason is logged while already unsynchronized");
diagnosticInventory.IsSynchronized = true;
Check(InventoryTracking.TakeSyncFailureMessage(diagnosticInventory, "test-character", 0x3040) == null, "synced inventory emits no failure");
diagnosticInventory.Invalidate("Unknown item reference 123.");
Check(InventoryTracking.TakeSyncFailureMessage(diagnosticInventory, "test-character", 0x3040) != null, "new desync episode logs again");
inv=Reset();Apply(0x3040,p=>{p.WriteUInt8(17);p.WriteUInt8(8);p.WriteUInt16(3);});
Check(!inv.IsSynchronized && inv.LastSyncLogKey.Contains("opcode=0x3040") && inv.LastSyncLogKey.Contains("slot 17"),
    "non-exception invalidation reaches logging path");
InventoryTracking.SnapshotFailed(inv,"test","Snapshot item missing");
Check(inv.LastSyncLogKey.Contains("opcode=0x34A6") && inv.LastError=="Snapshot item missing", "snapshot failure logs marker opcode and reason");
App.Settings.ServerType.CurrentValue = "jSRO";
App.Skills[900] = new SkillData { ID = 900 };
var actionFilter = new NetGuard.Filters.Agent.Client.ActionCommandRequest();
var teleportFilter = new NetGuard.Filters.Agent.Client.TeleportRequest();
var cooldowns = session.Host()!.UseItem;
var beforeDispel = DateTime.UtcNow;
var dispelResult = actionFilter.Handle(Packet(0x7074, p => { p.WriteUInt8(1); p.WriteUInt8(5); p.WriteUInt32(900); p.WriteUInt8(0); }), session, null!);
Check(dispelResult.ResultType == PacketResultType.None && cooldowns.DispelTime >= beforeDispel,
    "valid Dispel records timestamp and forwards action");
var teleportResult = teleportFilter.Handle(Packet(0x705A, p => { }), session, null!);
Check(teleportResult.ModifiedPackets.Count == 1 && teleportResult.ModifiedPackets[0].SecurityType == SecurityType.LocalSecurity,
    "teleport during Dispel cooldown is replaced with client notice");
cooldowns.DispelTime = DateTime.UtcNow.AddSeconds(-6);
Check(teleportFilter.Handle(Packet(0x705A, p => { }), session, null!).ModifiedPackets.Count == 0,
    "teleport forwards after five-second Dispel cooldown");
var oldDispelTime = cooldowns.DispelTime;
var invalidDispel = actionFilter.Handle(Packet(0x7074, p => { p.WriteUInt8(1); p.WriteUInt8(5); p.WriteUInt32(uint.MaxValue); p.WriteUInt8(0); }), session, null!);
Check(invalidDispel.ResultType == PacketResultType.Block && cooldowns.DispelTime == oldDispelTime,
    "invalid Dispel does not refresh cooldown");
cooldowns.HorseTime = DateTime.UtcNow.AddSeconds(-6);
Check(teleportFilter.Handle(Packet(0x705A, p => { }), session, null!).ModifiedPackets.Count == 1,
    "existing ten-second horse cooldown still blocks teleport");
var scudRow = File.ReadLines(Path.Combine(args[0], "skilldata.txt"))
    .Select(line => line.Split('\t')).First(fields => fields.Length > 72 && fields[1] == "9882");
App.Skills[9882] = new SkillData { ID = 9882, GroupID = int.Parse(scudRow[(int)SkillField.GroupID]),
    Param2 = int.Parse(scudRow[(int)SkillField.Param2]), Param3 = int.Parse(scudRow[(int)SkillField.Param3]) };
PacketHandlingResult SkillAction(byte action, uint skill) => actionFilter.Handle(
    Packet(0x7074, p => { p.WriteUInt8(1); p.WriteUInt8(action); p.WriteUInt32(skill); p.WriteUInt8(0); }), session, null!);
var castStart = DateTime.UtcNow;
Check(SkillAction(4, 9882).ResultType == PacketResultType.None
    && character.ScudDispelBlockedUntil >= castStart.AddSeconds(15)
    && character.ScudDispelBlockedUntil <= DateTime.UtcNow.AddSeconds(15), "Scud cast uses live skilldata duration");
oldDispelTime = cooldowns.DispelTime;
Check(SkillAction(5, 9882).ResultType == PacketResultType.Block && cooldowns.DispelTime == oldDispelTime,
    "Scud Dispel blocked during duration without starting teleport cooldown");
Check(SkillAction(5, 900).ResultType == PacketResultType.None, "other skills can still be dispelled during Scud");
character.ScudDispelBlockedUntil = DateTime.UtcNow.AddMilliseconds(-1);
Check(SkillAction(5, 9882).ResultType == PacketResultType.None, "Scud Dispel allowed after duration expires");
App.Skills[9882].Param3 = 21000;
castStart = DateTime.UtcNow;
SkillAction(4, 9882);
Check(character.ScudDispelBlockedUntil >= castStart.AddSeconds(21), "duration follows skilldata rather than hardcoded seconds");
var until = character.ScudDispelBlockedUntil;
SkillAction(4, 900);
Check(character.ScudDispelBlockedUntil == until, "other casts do not change Scud duration");
var migratedScudHost = new SessionHost();
SessionBridge.Migrate(session.Host, migratedScudHost);
Check(migratedScudHost.Agent.Current("test")!.ScudDispelBlockedUntil == until, "Scud restriction survives hot reload migration");
Console.WriteLine($"PASS: {passed} inventory, diagnostic and cooldown checks.");
