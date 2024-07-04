namespace Engine.Framework
{
    class Opcodes
    {
        public class Client
        {
            public const ushort
                CLIENT_GLOBAL_PING = 0x2002,
                CLIENT_ACCEPT_HANDSHAKE = 0x9000,

                CLIENT_AGENT_AUTH_REQUEST = 0x6103, // Reponse SERVER_AGENT_AUTH_RESPONSE

                CLIENT_AGENT_CHARACTER_SELECTION_JOIN_REQUEST = 0x7001,
                CLIENT_AGENT_CHARACTER_SELECTION_ACTION_REQUEST = 0x7007, // Reponse SERVER_AGENT_CHARACTER_SELECTION_RESPONSE

                CLIENT_AGENT_LOGOUT_REQUEST = 0x7005;


            // Add other packet types as needed
        }

        public class Server
        {
            public const ushort
                GLOBAL_IDENTIFICATION = 0x2001,
                LOGIN_SERVER_HANDSHAKE = 0x5000,

                SERVER_LOADING_END = 0x34A6,

                GLOBAL_UNKNOWN_2005 = 0x2005,
                GLOBAL_UNKNONW_6005 = 0x6005,

                SERVER_AGENT_AUTH_RESPONSE = 0xA103, // IPLimit = 5, ServerIsFull = 4,

                SERVER_AGENT_CHARACTER_SELECTION_RESPONSE = 0xB007,

                SERVER_AGENT_ENVIROMMENT_CELESTIAL_POSITION = 0x3020;
            // Add other packet types as needed
        }
    }
}