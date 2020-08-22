using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FieldState
{
    None = 0,
    Nought = -1,
    Cross = 1
}
public class Field : IEquatable<Field>
{
    public FieldState currentState = FieldState.None;
    
    public Field()
    {
    }

    public int ObserveField(Player playerType)
    {
        //Flip state depending on player to align observations
        if (playerType == Player.O)
        {
            if (currentState == FieldState.Cross) return 2;
            if (currentState == FieldState.Nought) return 1;
        }
        
        if (playerType == Player.X)
        {
            if (currentState == FieldState.Cross) return 1;
            if (currentState == FieldState.Nought) return 2;
        }

        return 0;
    }
    
    
    
    public bool Equals(Field other)
    {
        return currentState == other.currentState;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Field) obj);
    }

    public override int GetHashCode()
    {
        return (int) currentState;
    }
}
