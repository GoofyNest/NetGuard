/*
0xA102 | Gateway | Server -> Client
Name: GatewayLoginResponse

// Variant: GatewayLoginResponse.Invoke
byte unknown_36
if (unknown_36 == 0x01)
{
    if (gameVersion == Japanese)
    {
        uint unknown_45 // Token
        ascii unknown_46
        ushort unknown_47 // Port
        ascii unknown_48
        byte unknown_49 // Channel
    }
    if (gameVersion == Global && selectedAccount.Channel == 0x02)
    {
        uint unknown_58 // Token
        ascii unknown_59
        ushort unknown_60 // Port
        byte unknown_61 // Channel
        ascii unknown_62
    }
    // Stop parsing this variant.
}
byte code
switch (code): case 26:
{
    ushort count
    int timestamp
}

// Variant: GatewayLoginResponseHook.ReplacePacket
byte result
if (result == 0x01)
{
    uint token
    if (gameVersion == RuSro)
    {
        ascii unknown_42
        ushort unknown_42
    }
    if (!(gameVersion == RuSro))
    {
        ascii unknown_45
        ushort unknown_45
    }
}
if (packet.Remaining > 0)
{
    byte unknown_62[packet.Remaining]
}

// Variant: GatewayLoginResponseHook.ReplacePacket (written payload)
byte result
uint token
ascii unknown_58
ushort port
if (packet.Remaining > 0)
{
    byte remainingPayload[remainingBytes]
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

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

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA102)] // SERVER_GATEWAY_LOGIN_RESPONSE
    public sealed class GatewayLoginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var response = new PacketHandlingResult();
                byte res = packet.ReadUInt8();
                Custom.WriteLine($"SERVER_GATEWAY_LOGIN_RESPONSE {res}");
                if (res == 1)
                {
                    uint id = packet.ReadUInt32();
                    string host = packet.ReadAscii();
                    int port = packet.ReadUInt16();
                    var index = Main.Config.ModuleBinding.FindIndex(m => m.ModuleIP == host && m.ModulePort == port);
                    if (index == -1)
                    {
                        Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                        return response;
                    }
        
                    client.Gateway.SentLoginRequest = true;
                    foreach (var agent in Main.Config.ModuleBinding.Where(m => m.ModuleType == ModuleType.AgentModule))
                    {
                        SessionBus.SendTo(agent.ApiHost, agent.ApiPort, agent.ApiPassword, new BusMessage("gateway.login_requested", new { Username = client.PlayerInfo.Username ?? "", Target = $"{host}:{port}", }));
                    }
        
                    var guardModule = Main.Config.ModuleBinding[index];
                    Custom.WriteLine($"Using {guardModule.GuardIP} {guardModule.GuardPort}", ConsoleColor.Cyan);
                    var modified = new Packet(0xA102, true); // SERVER_GATEWAY_LOGIN_RESPONSE
                    modified.WriteUInt8(res);
                    modified.WriteUInt32(id);
                    modified.WriteAscii(guardModule.GuardIP);
                    modified.WriteUInt16((ushort)guardModule.GuardPort);
                    modified.WriteUInt32(0);
                    response.ModifiedPackets.Add(new PacketList { Packet = modified });
                    return response;
                }
        
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA102 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}