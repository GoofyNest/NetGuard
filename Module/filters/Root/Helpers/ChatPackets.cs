using SilkroadSecurityAPI;

namespace NetGuard.Hot.Helpers;

public static class ChatPackets
{
    public enum ChatType : byte
    {
        /// <summary>
        ///     General chat visible to nearby entities
        /// </summary>
        All = 1,

        /// <summary>
        ///     $
        /// </summary>
        PM = 2,

        /// <summary>
        ///     General (GM) chat visible to nearby entities
        /// </summary>
        AllGM = 3,

        /// <summary>
        ///     #
        /// </summary>
        Party = 4,

        /// <summary>
        ///     @
        /// </summary>
        Guild = 5,

        /// <summary>
        ///     Global chatting item
        /// </summary>
        Global = 6,

        /// <summary>
        ///     ~
        /// </summary>
        Notice = 7,

        /// <summary>
        ///     While in stall window
        /// </summary>
        Stall = 9,

        /// <summary>
        ///     %
        /// </summary>
        Union = 11,

        /// <summary>
        ///     SN_TALK_QNO_EU_EASTEU_21_14
        /// </summary>
        NPC = 13,

        /// <summary>
        ///     &
        /// </summary>
        Accademy = 16
    }

    public static Packet CreateNotice(string message)
    {
        var p = new Packet(0x3026, false, false);
        p.WriteUInt8((byte)ChatType.Notice);
        p.WriteAscii(message);
        return p;
    }

    public static Packet CreateChat(ChatType chatType, string senderName, string message)
    {
        var p = new Packet(0x3026, false, false);
        p.WriteUInt8((byte)chatType);
        p.WriteAscii(senderName);
        p.WriteAscii(message);
        return p;
    }

    public static Packet CreateChat(ChatType chatType, uint senderUniqueId, string message)
    {
        var p = new Packet(0x3026, false, false);
        p.WriteUInt8((byte)chatType);
        p.WriteUInt32(senderUniqueId);
        p.WriteAscii(message);
        return p;
    }
}