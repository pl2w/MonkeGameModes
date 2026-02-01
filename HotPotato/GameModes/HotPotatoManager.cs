using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using MonkeLib.Helpers;
using MonkeLib.Wrappers;
using Photon.Pun;

namespace HotPotato.GameModes;

public class HotPotatoManager : GorillaGameManager
{
    public List<int> _currentPotatoHolders = [];
    
    public GameState _gameState = GameState.WaitingForPlayers;
    public float _stateStartTime;
    
    public GameModeMaterials hotPotatoMaterial = GameModeMaterials.PaintBrawlNoTeamStunned;
    public GameModeMaterials defaultMaterial =  GameModeMaterials.Default;
    
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
        
        _currentPotatoHolders.Clear();
        _gameState = GameState.WaitingForPlayers;
        _stateStartTime = 0f;
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void StopPlaying()
    {
        base.StopPlaying();
    }

    public override int MyMatIndex(NetPlayer forPlayer)
    {
        if (_currentPotatoHolders.Contains(forPlayer.ActorNumber))
            return (int)hotPotatoMaterial;
        
        return (int)defaultMaterial;
    }
    
    public override float[] LocalPlayerSpeed()
    {
        if (_currentPotatoHolders.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber))
        {
            playerSpeed[0] = fastJumpLimit;
            playerSpeed[1] = fastJumpMultiplier;
            return playerSpeed;
        }
        
        playerSpeed[0] = slowJumpLimit;
        playerSpeed[1] = slowJumpMultiplier;
        return playerSpeed;
    }

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
    {
        if(NetworkSystem.Instance.IsMasterClient)
            return;
        
        _gameState = (GameState)(byte)stream.ReceiveNext();
        _currentPotatoHolders = ((int[])stream.ReceiveNext()).ToList();
    }
    
    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        if(!NetworkSystem.Instance.IsMasterClient)
            return;
        
        stream.SendNext((byte)_gameState);
        stream.SendNext(_currentPotatoHolders.ToArray());
    }
    
    public override void AddFusionDataBehaviour(NetworkObject behaviour) { }
    public override void OnSerializeRead(object newData) { }
    public override object OnSerializeWrite() => null;
}

public enum GameState
{
    WaitingForPlayers,
}