using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;

public enum Player
{
    X,
    O
}

public class AgentManager : MonoBehaviour
{
    public Player currentPlayer;
    private Board _board;
    
    [FormerlySerializedAs("Player1")] [FormerlySerializedAs("PlayerX")] [Header("AI Settings")] public TicTacToeAgent Agent1;
    [FormerlySerializedAs("Player2")] [FormerlySerializedAs("PlayerO")] public TicTacToeAgent Agent2;

    [Header("Human Settings")] public bool humanIsPlaying;
    public Player humanPlayer;
    
    [Header("General Settings")]
    public bool NiceToWatchMode;

    [HideInInspector]
    public bool PlayerChanged = true;

    private void Start()
    {
        PlayerChanged = true;
        _board = GetComponent<Board>();
    }

    public void ChangePlayer()
    {
        currentPlayer = currentPlayer == Player.O ? Player.X : Player.O;
        PlayerChanged = true;
    }

    private void FixedUpdate()
    {
        if (humanIsPlaying && currentPlayer == humanPlayer) return;
        if (PlayerChanged)
        {
            PlayerChanged = false;

            if (NiceToWatchMode) StartCoroutine(DelayedRequestDecision());
            else GetCurrentAgent().RequestDecision();
        }
    }

    private IEnumerator DelayedRequestDecision()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetCurrentAgent().RequestDecision();
        yield return true;
    }

 

    private TicTacToeAgent GetCurrentAgent()
    {
        if (currentPlayer == Agent1.type) return Agent1;
        else return Agent2;
    }

    private TicTacToeAgent GetAgentOfType(Player type)
    {
        if (Agent1.type == type) return Agent1;
        else return Agent2;
    }

    public void AssignRewardsOnWin(WinState winState)
    {
        if (winState == WinState.CrossWin)
        {
            GetAgentOfType(Player.X).SetReward(1f);
            GetAgentOfType(Player.O).SetReward(-1f);
        }
        else if (winState == WinState.NoughtWin)
        {
            GetAgentOfType(Player.O).SetReward(1f);
            GetAgentOfType(Player.X).SetReward(-1f);
        }
        else if (winState == WinState.Draw)
        {
            if (_board._fieldsOccupiedO > _board._fieldsOccupiedX)
            {
                GetAgentOfType(Player.O).SetReward(-0.25f);
                GetAgentOfType(Player.X).SetReward(0.75f);
            }
            else
            {
                GetAgentOfType(Player.O).SetReward(0.75f);
                GetAgentOfType(Player.X).SetReward(-0.25f);
            }
        }
    }

    public void EndEpisodes()
    {
        Agent2.EndEpisode();
        Agent1.EndEpisode();
    }
}