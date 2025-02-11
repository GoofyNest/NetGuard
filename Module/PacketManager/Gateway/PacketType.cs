namespace Module.PacketManager.Gateway
{
    class Opcodes
    {
        public class Client
        {
            public const ushort
                GLOBAL_PING = 0x2002,
                ACCEPT_HANDSHAKE = 0x9000,

                GATEWAY_PATCH_REQUEST = 0x6100,
                GATEWAY_SHARD_LIST_REQUEST = 0x6101,
                GATEWAY_LOGIN_REQUEST = 0x6102,
                GATEWAY_NOTICE_REQUEST = 0x6104,
                GATEWAY_SHARD_LIST_PING_REQUEST = 0x6106,
                GATEWAY_LOGIN_IBUV_ANSWER = 0x6323;


            // Add other packet types as needed
        }

        public class Server
        {
            public const ushort
                GLOBAL_IDENTIFICATION = 0x2001,
                LOGIN_SERVER_HANDSHAKE = 0x5000,

                SERVER_GATEWAY_PATCH_RESPONSE = 0xA100,
                SERVER_GATEWAY_SHARD_LIST_RESPONSE = 0xA101,
                SERVER_GATEWAY_LOGIN_RESPONSE = 0xA102,
                SERVER_GATEWAY_NOTICE_RESPONSE = 0xA104,
                SERVER_GATEWAY_SHARD_LIST_PING_RESPONSE = 0xA106,
                SERVER_GATEWAY_LOGIN_IBUV_RESULT = 0xA323,
                SERVER_GATEWAY_LOGIN_IBUV_CHALLENGE = 0x2322;
            // Add other packet types as needed
        }
    }
}