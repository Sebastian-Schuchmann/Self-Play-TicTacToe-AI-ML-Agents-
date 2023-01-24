using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public enum WinState
{
    Draw,
    NoughtWin,
    CrossWin
}

public class Board : MonoBehaviour
{
    public bool Log;
    
    private AgentManager _agentManager;
    public Field[] Fields => SetFieldFromComponent();
    [SerializeField] private FieldComponent[] fieldComponents;
    
    public int _fieldsOccupiedX { get; private set; }
    public int _fieldsOccupiedO { get; private set; }
    private int _fieldsOccupied = 0;
    
    private void Start()
    {
        _agentManager = GetComponent<AgentManager>();
        foreach (var field in fieldComponents) field.OnFieldChanged += OnBoardChanged;
    }
    
    private void OnBoardChanged(Field fieldChanged)
    {
        var winState = BoardEvaluator.instance.Evaluate(Fields);
        
        _fieldsOccupied++;

        if (_fieldsOccupied == 9 || winState != WinState.Draw)
        {
            TrackWinInTensorboard(winState);
            _agentManager.AssignRewardsOnWin(winState);
            
            if(Log) Debug.Log("<color=green>" + "Winner is: " + winState + "</color>");
            _agentManager.EndEpisodes();

            if (!_agentManager.NiceToWatchMode)
            {
                ResetBoard();
            }
            else
            {
                if(_agentManager.humanIsPlaying) WinnerScreen.instance.PlayerWon(winState);
                StartCoroutine(DelayReset());
            }
        }
        else
        {
            _agentManager.ChangePlayer();
        }
    }

    IEnumerator DelayReset()
    {
        yield return new WaitForSecondsRealtime(2f);
        ResetBoard();
        yield return true;
    }
    

    private static void TrackWinInTensorboard(WinState winState)
    {
        switch (winState)
        {
            case WinState.Draw:
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/Draw", 1, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/X-Won", 0, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/O-Won", 0, StatAggregationMethod.Average);
                break;
            case WinState.CrossWin:
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/Draw", 0, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/X-Won", 1, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/O-Won", 0, StatAggregationMethod.Average);
                break;
            case WinState.NoughtWin:
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/Draw", 0, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/X-Won", 0, StatAggregationMethod.Average);
                Academy.Instance.StatsRecorder.Add("TicTacToeWinner/O-Won", 1, StatAggregationMethod.Average);
                break;
        }
    }

    private Field[] SetFieldFromComponent()
    {
        var fieldFromComponents = new Field[9];

        for (int i = 0; i < fieldComponents.Length; i++)
        {
            fieldFromComponents[i] = fieldComponents[i].field;
        }

        return fieldFromComponents;
    }
    
    

    private void ResetBoard()
    {
        _fieldsOccupied = 0;
        _fieldsOccupiedO = 0;
        _fieldsOccupiedX = 0;
        
        foreach (var field in fieldComponents) field.ResetField();
        
        _agentManager.ChangePlayer();
    }
    
    public void SelectField(int fieldId, Player type)
    {
        if (type == Player.O) _fieldsOccupiedO++;
        if (type == Player.X) _fieldsOccupiedX++;
        
        if (_agentManager.NiceToWatchMode) StartCoroutine(WaitAndSelect(fieldId, type));
        else fieldComponents[fieldId].Select(type);
    }

    IEnumerator WaitAndSelect(int fieldId, Player type)
    {
        fieldComponents[fieldId].ChangeAppearance(type == Player.O ? FieldState.Nought : FieldState.Cross);
        yield return new WaitForSecondsRealtime(0.3f);
        fieldComponents[fieldId].Select(type);
        yield return true;
    }
    
    public IEnumerable<int> GetAvailableFields()
    {
        List<int> availableFields = new List<int>(9);

        for (int i = 0; i < Fields.Length; i++)
        {
            if (Fields[i].currentState == FieldState.None)
                availableFields.Add(i);
        }

        return availableFields.ToArray();
    }

    public List<int> GetOccupiedFields()
    {
        List<int> impossibleFields = new List<int>(9);

        for (int i = 0; i < Fields.Length; i++)
        {
            if (Fields[i].currentState != FieldState.None)
                impossibleFields.Add(i);
        }

        return impossibleFields;
    }
}