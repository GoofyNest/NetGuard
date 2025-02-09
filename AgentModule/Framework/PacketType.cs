namespace AgentModule.Framework
{
    class Opcodes
    {
        public class Client
        {
            public const ushort
                GLOBAL_PING = 0x2002,
                ACCEPT_HANDSHAKE = 0x9000,

                AGENT_AUTH_REQUEST = 0x6103, // Reponse SERVER_AGENT_AUTH_RESPONSE

                AGENT_CHARACTER_SELECTION_JOIN_REQUEST = 0x7001,
                AGENT_CHARACTER_SELECTION_ACTION_REQUEST = 0x7007, // Reponse SERVER_AGENT_CHARACTER_SELECTION_RESPONSE

                AGENT_LOGOUT_REQUEST = 0x7005;


            // Add other packet types as needed
        }

        public class Server
        {
            public const ushort
                GLOBAL_IDENTIFICATION = 0x2001,
                LOGIN_SERVER_HANDSHAKE = 0x5000,

                AGENT_GAME_READY = 0x3012,
                AGENT_GAME_READY2 = 0x3014, // Happens if locale is different in client

                AGENT_CHARDATA_BEGIN = 0x34A5,
                AGENT_CHARDATA = 0x3013,
                AGENT_CHARDATA_END = 0x34A6,

                AGENT_COS_INFO = 0x30C8,
                AGENT_COS_UPDATE = 0x30C9,
                AGENT_COS_UPDATE_RIDESTATE = 0xB0CB,

                GLOBAL_NODE_STATUS1 = 0x2005,
                GLOBAL_NODE_STATUS2 = 0x6005,

                AGENT_AUTH_RESPONSE = 0xA103, // IPLimit = 5, ServerIsFull = 4,

                AGENT_CHARACTER_SELECTION_RESPONSE = 0xB007,

                AGENT_ENVIROMMENT_CELESTIAL_POSITION = 0x3020;
            // Add other packet types as needed
        }
    }
}