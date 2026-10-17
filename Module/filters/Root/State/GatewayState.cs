using Module.PacketHandler.Filtering;

namespace NetGuard.Hot;

public sealed class GatewayState : ISessionState
{
    public ushort ServerID { get; set; }
    public bool SentPatchResponse { get; set; }
    public bool SentShardListResponse { get; set; }
}