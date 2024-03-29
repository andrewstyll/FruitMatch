﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour {

    // constant time per game and string
    private const float TIME_PER_GAME = 60.0f;
    private const string SCORE_BASE_TEXT = "Score: ";

    // status bar score and timer variables
    private int score;
    private float timeRemaining;
    private Slider timer;
    private Text scoreText;

    // mark that the game has begun
    private bool gameIsStarted = false;

    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject scoreObject;

    // subscribe to this delegate to know when the game is over
    public delegate void StatusBarHandler(string scoreText);
    public static event StatusBarHandler EndGame;

    private void Awake() {
        RulesUI.StartGame += StartGame;
        FruitActionManager.ValidPairCreated += PairCreated;
        EndGameUI.RestartGame += StartGame;

        this.timer = timerObject.GetComponent<Slider>();
        this.scoreText = scoreObject.GetComponent<Text>();
    }

    private void Update() {
        if(this.gameIsStarted) {
            if (this.timeRemaining > 0.0f) {
                this.timeRemaining -= Time.deltaTime;
                this.timer.value = (TIME_PER_GAME - this.timeRemaining) / TIME_PER_GAME;
                this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
            } else if(this.timeRemaining <= 0.0f) {
                this.gameIsStarted = false;
                EndGame(SCORE_BASE_TEXT + score.ToString());
            }
        }
    }

    // unsub from events to prevent potential future null reference
    private void OnDestroy() {
        RulesUI.StartGame -= StartGame;
        FruitActionManager.ValidPairCreated -= PairCreated;
        EndGameUI.RestartGame -= StartGame;
    }

    // init the UI data. Seperate as may want called w/o event
    private void InitUI() {
        this.score = 0;
        this.timeRemaining = TIME_PER_GAME;

        this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
    }

    /**** Events ****/
    private void PairCreated() {
        this.score++;
        this.timeRemaining = Mathf.Min(TIME_PER_GAME, this.timeRemaining + 5.0f);
    }

    // on game start, set variable to allow clock to being ticking
    private void StartGame() {
        InitUI();
        this.gameIsStarted = true;
    }
}
