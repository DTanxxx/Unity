﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberWizard : MonoBehaviour
{
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] TextMeshProUGUI guessText;

    int guess;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        NextGuess();
    }

    public void onPressHigher()
    {
        min = guess + 1;
        NextGuess();
    }

    public void onPressLower()
    {
        max = guess;
        NextGuess();
    }

    void NextGuess()
    {
        guess = Random.Range(min, max);
        guessText.text = guess.ToString();
    }
}