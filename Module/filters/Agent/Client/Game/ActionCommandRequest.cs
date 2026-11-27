/*
0x7074 | Agent | Client -> Server
Name: ActionCommandRequest

// Variant: SkillManager.CastSkill
byte execute // Execute
byte cast // Use Skill
uint id
byte entity
if (gameVersion < Thailand)
{
    byte unknown_296 // value: 1
}
uint targetId

// Variant: SkillManager.CastSkillOld
byte unknown_365 // value: 1
byte unknown_366 // value: 4
uint id
byte entity
uint targetId

// Variant: SkillManager.CastBuff
byte unknown_428 // value: 1
byte unknown_429 // value: 4
uint id
if (skill.Record.TargetGroup_Self || skill.Record.TargetGroup_Party)
{
    byte entity
    uint unknown_435 // value: target == 0 ? Game.Player.UniqueId : target
}
if (!(skill.Record.TargetGroup_Self || skill.Record.TargetGroup_Party))
{
    byte none
}

// Variant: SkillManager.CastSkillAt
byte execute // Execute
byte cast // Use Skill
uint id
byte area
ushort region
if (target.Region.IsDungeon)
{
    short xOffset
    short yOffset
    short zOffset
}
if (!(target.Region.IsDungeon))
{
    int xOffset
    int zOffset
    int yOffset
}

// Variant: SkillManager.CastAutoAttack
byte execute // Execute
byte attack // Use Skill
byte entity
if (gameVersion < Thailand)
{
    byte unknown_554 // value: 1
}
uint uniqueId

// Variant: SkillManager.CastSkillAt
byte execute // Execute
byte cast // Use Skill
uint skillId
byte area
ushort id
float xOffset
float zOffset
float yOffset

// Variant: SkillManager.CancelBuff
byte execute // Execute
byte dispel // Cancel Buff
uint skillId
byte none

// Variant: SkillManager.CancelAction
byte unknown_615 // value: 0x02

// Variant: SpawnedItem.Pickup
byte execute
byte pickup
byte entity
uint uniqueId

// Variant: CommandsBundle.SendTraceRequest
byte unknown_145 // value: 1
byte unknown_146 // value: 3
byte unknown_147 // value: 1
uint uniqueId

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/ActionCommandType.cs
enum ActionCommandType : byte
{
    Execute = 1,
    Cancel = 2,
}

// Library/RSBot.Core/Objects/ActionState.cs
enum ActionState : byte
{
    Begin = 1,
    End = 2,
    Error = 3,
}

// Library/RSBot.Core/Objects/ActionTarget.cs
enum ActionTarget : int
{
    None = 0,
    Entity = 1,
    Area = 2,
}

// Library/RSBot.Core/Objects/ActionType.cs
enum ActionType : byte
{
    Attack = 1,
    Pickup = 2,
    Trace = 3,
    Cast = 4,
    Dispel = 5,
}

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7074)]
    public sealed class ActionCommandRequest : IFilter
    {
        public enum ActionCommandType : byte
        {
            Execute = 1,
            Cancel = 2,
        }

        public enum ActionType : byte
        {
            Attack = 1,
            Pickup = 2,
            Trace = 3,
            Cast = 4,
            Dispel = 5,
        }

        public enum ActionTarget : byte
        {
            None = 0,
            Entity = 1,
            Area = 2,
        }

        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            //if (PacketDiagnostics.Enabled)
            //    Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);
            var response = new PacketHandlingResult();

            var h = client.Host()!;
            
            var _files = Main.Settings.ServerType.CurrentValue;

            if (_files != "jSRO")
                return response;

            Packet modified = new Packet(0x7074);

            ActionCommandType CommandType = (ActionCommandType)packet.ReadUInt8();
            switch (CommandType)
            {
                case ActionCommandType.Execute:
                    {
                        ActionType ActionType = (ActionType)packet.ReadUInt8();

                        int packetLength = packet.GetBytes().Length;

                        switch (ActionType)
                        {
                            /*
                                [C -> S][7074]
                                01                                                ................
                                01                                                ................
                                01                                                ................
                                01                                                ................
                                60 3C 00 00                                       `<..............
                            */
                            case ActionType.Attack: // 0x01
                                {
                                    if(packetLength != 8)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Attack blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;

                            case ActionType.Pickup: // 0x02
                                {
                                    ActionTarget TargetType = (ActionTarget)packet.ReadUInt8();

                                    byte malformedByte = packet.ReadUInt8();

                                    if (malformedByte != 0x01)
                                    {
                                        try
                                        {
                                            modified.WriteUInt8((byte)CommandType);
                                            modified.WriteUInt8((byte)ActionType);
                                            modified.WriteUInt8((byte)TargetType);
                                            modified.WriteUInt8(0x01);

                                            // 18 09 00 00
                                            modified.WriteUInt8(malformedByte);
                                            modified.WriteUInt8(packet.ReadUInt8());
                                            modified.WriteUInt8(packet.ReadUInt8());
                                            modified.WriteUInt8(packet.ReadUInt8());

                                            response.ModifiedPackets.Add(new PacketList
                                            {
                                                Packet = modified,
                                                SendImmediately = true,
                                                SecurityType = SecurityType.RemoteSecurity
                                            });

                                            Custom.WriteLine("Emulating bot malformed pickup packet");
                                            return response;
                                        }
                                        catch (Exception ex)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Pickup exception {ex.ToString()}");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }
                                    }
                                }
                                break;


                            /*
                                [C -> S][7074]
                                01                                                ................
                                03                                                ................
                                01                                                ................
                                01                                                ................
                                23 4A 01 00                                       #J..............
                            */
                            case ActionType.Trace: // 0x03
                                {
                                    if(packetLength != 8)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Trace blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;

                            case ActionType.Cast: // 0x04
                                {
                                    try 
                                    {
                                        var RefSkill = Main.Skills;

                                        var Skill_ID = packet.ReadUInt32();
                                        //if (PacketDiagnostics.Enabled) Custom.WriteLine($"ID: {Skill_ID}");

                                        RefSkill.TryGetValue((int)Skill_ID, out var _skill);

                                        if(_skill == null)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Cast sending invalid Skill_ID");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }

                                        
                                        
                                        /*
                                            Fix exploits here
                                        */

                                        if(packetLength > 12)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Cast packetLength > 12, should never happen");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }

                                        // RSBot crash emulation
                                        if(packetLength > 7 && packetLength <= 11)
                                        {
                                            modified.WriteUInt8((byte)CommandType);
                                            modified.WriteUInt8((byte)ActionType);
                                            modified.WriteUInt32(Skill_ID);
                                            
                                            packet.ReadUInt8();

                                            modified.WriteUInt8(0x01);
                                            modified.WriteUInt8(0x01);
                                            modified.WriteUInt32(packet.ReadUInt32());

                                            response.ModifiedPackets.Add(new PacketList
                                            {
                                                Packet = modified,
                                                SendImmediately = true,
                                                SecurityType = SecurityType.RemoteSecurity
                                            });

                                            Custom.WriteLine("Emulating bot malformed skill cast packet");
                                            //TrackScudCast(_skill, client);
                                            return response;
                                        }
                                        //TrackScudCast(_skill, client);
                                    }
                                    catch (Exception ex)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Cast exception {ex.ToString()}");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;


                            /*
                                [C -> S][7074]
                                01                                                ................
                                05                                                ................
                                29 0C 00 00                                       )...............
                                00                                                ................

                            */
                            case ActionType.Dispel: // 0x05
                                {
                                    var RefSkill = Main.Skills;

                                    if(packetLength != 7)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Dispel blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }

                                    var Skill_ID = packet.ReadUInt32();

                                    RefSkill.TryGetValue((int)Skill_ID, out var _skill);

                                    if(_skill == null)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Dispel sending invalid Skill_ID");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }


                                    if (_skill.GroupID == 630)
                                    {
                                        var character = h.Agent.Current(client.PlayerInfo.CurrentCharName);
                                        var remaining = (character?.ScudDispelBlockedUntil ?? DateTime.MinValue) - DateTime.UtcNow;
                                        if (remaining > TimeSpan.Zero)
                                        {
                                            Custom.WriteLine($"[0x7074] Dispel blocked character={client.PlayerInfo.CurrentCharName} skill={Skill_ID} group=630 remaining={remaining.TotalSeconds:F1}s");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }
                                    }

                                    h.UseItem.DispelTime = DateTime.UtcNow;
                                    Custom.WriteLine($"[0x7074] Dispel character={client.PlayerInfo.CurrentCharName} skill={Skill_ID} timestamp={h.UseItem.DispelTime:O}; teleport blocked for 5 seconds.");

                                    //Custom.WriteLine($"{_skill.ToString()}");
                                }
                                break;

                            default:
                                response.ResultType = PacketResultType.Block;
                                return response;
                        }
                        break;
                    }

                case ActionCommandType.Cancel:
                    {
                        modified.WriteUInt8(0x02);
                        response.ModifiedPackets.Add(new PacketList
                        {
                            Packet = modified,
                            SendImmediately = true,
                            SecurityType = SecurityType.RemoteSecurity
                        });
                        return response;
                    }

                default:
                    response.ResultType = PacketResultType.Block;
                    return response;
            }

            return response;
        }

        private static void TrackScudCast(Module.Helpers.ClientData.SkillData skill, SessionData client)
        {
            if (skill.GroupID != 630) return;

            // Group 630: Param2 is the 'dura' marker; Param3 is milliseconds.
            if (skill.Param2 != 1685418593 || skill.Param3 <= 0)
                throw new InvalidOperationException($"Invalid Scud duration for skill {skill.ID}.");

            var character = client.Host()!.Agent.EnsureCurrent(client.PlayerInfo.CurrentCharName);
            character.ScudDispelBlockedUntil = DateTime.UtcNow.AddMilliseconds(skill.Param3);
            Custom.WriteLine($"[0x7074] Scud cast character={character.Charname} skill={skill.ID} duration={skill.Param3}ms dispelBlockedUntil={character.ScudDispelBlockedUntil:O}");
        }
    }
}
