using Fusion;
using GorillaGameModes;
using Photon.Pun;

namespace GoldenBanana.GameModes;

public class GoldenBananaManager : GorillaGameManager
{
    public override GameModeType GameType() => (GameModeType)GameModeInfo.Id;
    public override string GameModeName() => GameModeInfo.Guid;
    public override string GameModeNameRoomLabel() => string.Empty;

    public override void StartPlaying()
    {
        base.StartPlaying();
        
        slowJumpLimit = 6.5f;
        slowJumpMultiplier = 1.1f;
        fastJumpLimit = 8.5f;
        fastJumpMultiplier = 1.3f;
    }
    
    public override float[] LocalPlayerSpeed()
    {
        playerSpeed[0] = slowJumpLimit;
        playerSpeed[1] = slowJumpMultiplier;
        return playerSpeed;
    }

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info) { }
    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info) { }
    public override void AddFusionDataBehaviour(NetworkObject behaviour) { }
    public override void OnSerializeRead(object newData) { }
    public override object OnSerializeWrite() => null;
}

public enum GameState
{
    WaitingForPlayers,
}