using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEvaluator : MonoBehaviour
{
    public static BoardEvaluator instance;
    public Field[] fields;
    private Dictionary<Field[], WinState> FieldResultLookUp;
    
    public WinState Evaluate(Field[] board)
    {
        return FieldResultLookUp[board];
    }

    public bool HasPlayerWon(Player player, Field[] board)
    {
        if (player == Player.X) return Evaluate(board) == WinState.CrossWin;
        else return Evaluate(board) == WinState.NoughtWin;
    }
    
    private void Awake()
    {
        instance = this;
        
        /* We are using the method of pre calculating every board
         position and storing it in a hashtable to evaluate a winner*/
        FieldResultLookUp = new Dictionary<Field[], WinState>(new FieldComparer());
        CalculateResultForEveryPossibleField();
    }
    
    private void CalculateResultForEveryPossibleField()
    {
        for (int i = 0; i < 19683; ++i)
        {
            int c = i;
            fields = new Field[9];

            for (var i1 = 0; i1 < fields.Length; i1++)
            {
                fields[i1] = new Field();
            }

            for (int j = 0; j < 9; ++j)
            {
                switch (c % 3)
                {
                    case 0:
                        fields[j].currentState = FieldState.None;
                        break;
                    case 1:
                        fields[j].currentState = FieldState.Nought;
                        break;
                    case 2:
                        fields[j].currentState = FieldState.Cross;
                        break;
                }

                c /= 3;
            }

            var winState = EvaluateField();
            FieldResultLookUp.Add(fields, winState);
        }
    }

    private WinState EvaluateField()
    {
        WinState winState = WinState.Draw;

        for (int i = 0; i < 3; i++)
        {
            int horizontalValue = (int) fields[i * 3].currentState + (int) fields[i * 3 + 1].currentState +
                                  (int) fields[i * 3 + 2].currentState;
            int verticalValue = (int) fields[i].currentState + (int) fields[i + 3].currentState +
                                (int) fields[i + 6].currentState;

            if (DetermineWinner(horizontalValue, verticalValue, ref winState)) return winState;
        }

        int diagonalTopLeftValue =
            (int) fields[0].currentState + (int) fields[4].currentState + (int) fields[8].currentState;
        int diagonalTopRightValue =
            (int) fields[2].currentState + (int) fields[4].currentState + (int) fields[6].currentState;


        if (DetermineWinner(diagonalTopLeftValue, diagonalTopRightValue, ref winState)) return winState;
        return WinState.Draw;
    }
    
    private static bool DetermineWinner(int horizontalValue, int verticalValue, ref WinState winState)
    {
        if (horizontalValue == 3 || verticalValue == 3)
        {
            winState = WinState.CrossWin;
            return true;
        }

        if (horizontalValue == -3 || verticalValue == -3)
        {
            winState = WinState.NoughtWin;
            return true;
        }

        return false;
    }
}
