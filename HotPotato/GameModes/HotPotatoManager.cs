using Fusion;
using GorillaGameModes;
using MonkeLib.Helpers;
using MonkeLib.Wrappers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotPotato.GameModes;

public class HotPotatoManager : GorillaGameManager
{
    public List<int> _currentPotatoHolders = [];

    public GameState _gameState = GameState.WaitingForPlayers;

    public float _stateStartTime;
    private const float CountdownTime = 5f;
    private float _hotPotatoExplodeTime = 30f;

    public float tagCoolDown = 3f;
    public double lastTag;

    private bool isPlayingSound;

    public GameModeMaterials burntPotatoMaterial = GameModeMaterials.PaintBrawlNoTeamStunned;
    public GameModeMaterials hotPotatoMaterial = GameModeMaterials.PaintBrawlNoTeamEliminated;
    public GameModeMaterials defaultMaterial = GameModeMaterials.Default;

    private Texture paintBrawlNoTeamStunned, paintBrawlNoTeamEliminated;

    public override GameModeType GameType() => (GameModeType)GameModeInfo.Id;
    public override string GameModeName() => GameModeInfo.Guid;
    public override string GameModeNameRoomLabel() => string.Empty;

    private void Start()
    {
        paintBrawlNoTeamEliminated = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamEliminated].mainTexture;
        paintBrawlNoTeamStunned = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamStunned].mainTexture;
    }

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

        lastTag = 0.0;

        GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamEliminated].mainTexture = Plugin.potatoTexture;
        GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamStunned].mainTexture = Plugin.burntPotatoTexture;
    }

    public override void StopPlaying()
    {
        base.StopPlaying();

        _hotPotatoExplodeTime = 30f;
        _currentPotatoHolders.Clear();
        _gameState = GameState.WaitingForPlayers;
        _stateStartTime = 0f;
        lastTag = 0.0;

        GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamEliminated].mainTexture = paintBrawlNoTeamEliminated;
        GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[(int)GameModeMaterials.PaintBrawlNoTeamStunned].mainTexture = paintBrawlNoTeamStunned;
    }

    public override void Tick()
    {
        base.Tick();

        if (!NetworkSystem.Instance.IsMasterClient)
            return;

        switch (_gameState)
        {
            case GameState.WaitingForPlayers:
                if (EnoughPlayersToStart())
                    SetState(GameState.StartingRound);
                break;
            case GameState.StartingRound:
                if (Time.time - _stateStartTime >= CountdownTime)
                {
                    SetState(GameState.PlayingRound);
                }
                break;
            case GameState.PlayingRound:
                CheckGameEnded();
                break;
            case GameState.RoundComplete:
                SetState(GameState.StartingRound);
                break;
        }
    }

    private void CheckGameEnded()
    {
        if (currentNetPlayerArray.Length < 2)
        {
            SetState(GameState.WaitingForPlayers);
            return;
        }

        _hotPotatoExplodeTime -= Time.deltaTime;
        if (_hotPotatoExplodeTime <= 0f)
        {
            EndRound();
            Plugin.Log.LogInfo("Round ended");
        }

        if (_hotPotatoExplodeTime <= 10f && !isPlayingSound)
        {
            isPlayingSound = true;
            StartCoroutine(TickingSound());
        }
    }

    private void ResetRound()
    {
        _hotPotatoExplodeTime = 30f;
        _stateStartTime = 0f;
        lastTag = 0.0;
        _currentPotatoHolders.Clear();

        List<NetPlayer> selected = currentNetPlayerArray.OrderBy(x => UnityEngine.Random.value)
            .Take(GetPotatoCount())
            .ToList();

        selected.ForEach(p => _currentPotatoHolders.Add(p.ActorNumber));
        Plugin.Log.LogInfo($"Selected players: {string.Join(", ", selected.Select(p => p.NickName))}");
        selected.ForEach(p => RoomSystemWrapper.SendSoundEffectToPlayer(0, 0.25f, p));
    }

    private void EndRound()
    {
        foreach (NetPlayer participatingPlayer in GorillaGameModes.GameMode.ParticipatingPlayers)
            RoomSystemWrapper.SendSoundEffectToPlayer(2, 0.25f, participatingPlayer, true);

        SetState(GameState.RoundComplete);
    }

    private void SetState(GameState state)
    {
        _stateStartTime = Time.time;
        _gameState = state;

        switch (state)
        {
            case GameState.WaitingForPlayers:
                _currentPotatoHolders.Clear();
                _stateStartTime = 0f;
                break;
            case GameState.PlayingRound:
                ResetRound();
                break;
        }
    }

    public override int MyMatIndex(NetPlayer forPlayer)
    {
        if (_currentPotatoHolders.Contains(forPlayer.ActorNumber))
        {
            if (_gameState == GameState.StartingRound)
                return (int)burntPotatoMaterial;

            return (int)hotPotatoMaterial;
        }

        return (int)defaultMaterial;
    }

    public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
    {
        if (_currentPotatoHolders.Contains(myPlayer.ActorNumber) && !_currentPotatoHolders.Contains(otherPlayer.ActorNumber))
            return true;

        return false;
    }

    public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
    {
        if (_gameState != GameState.PlayingRound)
            return;

        if (Time.time < lastTag + tagCoolDown)
            return;

        if (!LocalCanTag(taggingPlayer, taggedPlayer))
            return;

        _currentPotatoHolders.Remove(taggingPlayer.ActorNumber);
        _currentPotatoHolders.Add(taggedPlayer.ActorNumber);

        lastTag = Time.time;

        RoomSystemWrapper.SendStatusEffectToPlayer(StatusEffects.TaggedTime, taggedPlayer);
        RoomSystemWrapper.SendSoundEffectOnOther(0, 0.25f, taggedPlayer);
    }

    public override void OnPlayerLeftRoom(NetPlayer leavingPlayer)
    {
        base.OnPlayerLeftRoom(leavingPlayer);

        if (!NetworkSystem.Instance.IsMasterClient)
            return;

        if (_currentPotatoHolders.Contains(leavingPlayer.ActorNumber))
            _currentPotatoHolders.Remove(leavingPlayer.ActorNumber);
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
        if (NetworkSystem.Instance.IsMasterClient)
            return;

        _gameState = (GameState)(byte)stream.ReceiveNext();
        _currentPotatoHolders = ((int[])stream.ReceiveNext()).ToList();
        _hotPotatoExplodeTime = (float)stream.ReceiveNext();
    }

    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!NetworkSystem.Instance.IsMasterClient)
            return;

        stream.SendNext((byte)_gameState);
        stream.SendNext(_currentPotatoHolders.ToArray());
        stream.SendNext(_hotPotatoExplodeTime);
    }

    private bool EnoughPlayersToStart()
    {
        return currentNetPlayerArray.Length >= 2;
    }

    private int GetPotatoCount()
    {
        if (currentNetPlayerArray.Length < 2) return 0;

        return Math.Clamp((currentNetPlayerArray.Length - 2) / 3 + 1, 1, 3);
    }

    public override void AddFusionDataBehaviour(NetworkObject behaviour) { }
    public override void OnSerializeRead(object newData) { }
    public override object OnSerializeWrite() => null;

    private IEnumerator TickingSound()
    {
        for (int i = 1; i <= 5; i++)
        {
            currentNetPlayerArray.ForEach(p => RoomSystemWrapper.SendSoundEffectOnOther(6, 0.25f, p));
            yield return new WaitForSeconds(2f);
        }
        isPlayingSound = false;
    }
}

public enum GameState : byte
{
    WaitingForPlayers,
    StartingRound,
    PlayingRound,
    RoundComplete
}