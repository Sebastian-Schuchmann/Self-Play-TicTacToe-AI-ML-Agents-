using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldComparer : IEqualityComparer<Field[]>
{
    public bool Equals(Field[] x, Field[] y)
    {
        bool isEqual = true;

        for (int i = 0; i < x.Length; i++)
        {
            isEqual &= x[i].Equals(y[i]);
        }

        return isEqual;
    }

    public int GetHashCode(Field[] field)
    {
        int HashCode = 0;

        for (int i = 0; i < field.Length; i++)
        {
            HashCode += (int) field[i].currentState;
        }

        return HashCode;
    }
}
