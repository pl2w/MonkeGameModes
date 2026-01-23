using Fusion;
using GorillaGameModes;
using Photon.Pun;

namespace GorillaTagPartyGames.GameModes;

public class TeamInfection : GorillaGameManager
{
    public override GameModeType GameType() => (GameModeType)4821;
    public override string GameModeName() => GameModeInfo.TeamTagGuid;
    public override string GameModeNameRoomLabel() => string.Empty;

    public override int MyMatIndex(NetPlayer player) => 3;

    public override void OnSerializeRead(object newData)
    {
    }

    public override object OnSerializeWrite() => null;

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
    }
    
    public override void AddFusionDataBehaviour(NetworkObject behaviour)
    {
        behaviour.AddBehaviour<CasualGameModeData>();
    }
}