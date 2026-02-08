using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using MonkeLib.Helpers;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GoldenMonkey.GameModes;

public class GoldenMonkeyManager : GorillaGameManager
{
    public GameState _gameState = GameState.WaitingForPlayers;
    public float _stateStartTime = 0f;

    public Dictionary<int, int> _goldenMonkeyHoldTimes = new();
    public int _currentGoldenMonkey = -1;
    
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
        
        if (!NetworkSystem.Instance.IsMasterClient)
            return;
        
        ResetGame();
    }

    public override void ResetGame()
    {
        base.ResetGame();
        
        _goldenMonkeyHoldTimes.Clear();
        _gameState = GameState.WaitingForPlayers;
        _stateStartTime = 0f;
    }

    public override void Tick()
    {
        base.Tick();

        switch (_gameState)
        {
            case GameState.WaitingForPlayers:
                break;
        }
    }

    public void MakeGoldenMonkey()
    {
        if (!PhotonNetwork.IsMasterClient) 
            return;

        _currentGoldenMonkey = currentNetPlayerArray[Random.Range(0, currentNetPlayerArray.Length)].ActorNumber; 
    }

    public override float[] LocalPlayerSpeed()
    {
        playerSpeed[0] = slowJumpLimit;
        playerSpeed[1] = slowJumpMultiplier;
        return playerSpeed;
    }

    public override int MyMatIndex(NetPlayer forPlayer)
    {
        if (forPlayer.ActorNumber == _currentGoldenMonkey)
            return (int)GameModeMaterials.PaintBrawlNoTeamStunned;
        
        return (int)GameModeMaterials.Default;
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
    PlayingRound,
    RoundComplete
}