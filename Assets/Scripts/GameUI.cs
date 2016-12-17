﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public Text txtScore;

    private int totScore = 0;
	// Use this for initialization
	void Start () {
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);
        DispScore(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
