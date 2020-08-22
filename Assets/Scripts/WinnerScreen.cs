using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerScreen : MonoBehaviour
{
    public static WinnerScreen instance;
    
    public GameObject X_Won;
    public GameObject O_Won;
    public GameObject Draw;
    
    private Animator _animator;
    private static readonly int Winner = Animator.StringToHash("Winner");

    private void Start()
    {
        instance = this;
        _animator = GetComponent<Animator>();
    }

    public void PlayerWon(WinState state)
    {
        X_Won.SetActive(false);
        O_Won.SetActive(false);
        Draw.SetActive(false);
        
        if (state == WinState.CrossWin) X_Won.SetActive(true);
        else if (state == WinState.NoughtWin) O_Won.SetActive(true);
        else Draw.SetActive(true);

        TriggerAnimation();
    }
    
    void TriggerAnimation()
    {
        _animator.SetTrigger(Winner);
    }
}


