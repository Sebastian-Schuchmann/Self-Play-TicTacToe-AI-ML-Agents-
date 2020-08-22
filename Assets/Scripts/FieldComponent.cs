using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldComponent : MonoBehaviour
{
    public Field field;
    public AgentManager _agentManager;

    public GameObject highlighterO;
    public GameObject highlighterX;

    public GameObject X;
    public GameObject O;
    private static readonly int Place = Animator.StringToHash("Place");

    public void ResetField()
    {
        field.currentState = FieldState.None;
        ChangeAppearance();
    }

    private void ChangeAppearance()
    {
        ShowHideField(X, field.currentState == FieldState.Cross);
        ShowHideField(O, field.currentState == FieldState.Nought);
    }

    public void ChangeAppearance(FieldState state)
    {
        ShowHideField(X, state == FieldState.Cross);
        ShowHideField(O, state == FieldState.Nought);
    }

    private void ShowHideField(GameObject field, bool state)
    {
        if (state)
        {
            field.SetActive(true);

            if (_agentManager.NiceToWatchMode)
            {
                field.GetComponent<Animator>().SetTrigger(Place);
            }

            return;
        }

        field.SetActive(false);
    }

    public event Action<Field> OnFieldChanged;

    public void Select(Player type)
    {
        if (field.currentState == FieldState.None)
        {
            SetField(type == Player.O ? FieldState.Nought : FieldState.Cross);
        }
    }
    
    private void Start()
    {
        field = new Field();
    }

    private void SetField(FieldState newState)
    {
        field.currentState = newState;
        ChangeAppearance();

       
        OnFieldChanged?.Invoke(field);
    }
    
    private void OnMouseDown()
    {
        if (!_agentManager.humanIsPlaying) return;

        if (_agentManager.PlayerChanged)
        {
            if (field.currentState == FieldState.None)
                Select(_agentManager.currentPlayer);
        }
    }
    
    private void OnMouseOver()
    {
        if (!_agentManager.humanIsPlaying) return;
        
        if (_agentManager.PlayerChanged && _agentManager.humanPlayer == _agentManager.currentPlayer)
        {
            if(_agentManager.currentPlayer == Player.O) highlighterO.SetActive(true);
            if(_agentManager.currentPlayer == Player.X) highlighterX.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (!_agentManager.humanIsPlaying) return;
        
        highlighterO.SetActive(false);
        highlighterX.SetActive(false);
    }
}