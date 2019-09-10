using System.Collections;
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

    private bool gameIsStarted = false;

    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject scoreObject;

    private void Awake() {
        RulesUI.StartGame += StartGame;
        FruitActionManager.ValidPairCreated += PairCreated;

        this.score = 0;
        this.timeRemaining = TIME_PER_GAME;

        this.timer = timerObject.GetComponent<Slider>();
        this.scoreText = scoreObject.GetComponent<Text>();

        this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
    }

    private void Update() {
        if(this.gameIsStarted) {
            if (this.timeRemaining > 0.0f) {
                this.timeRemaining -= Time.deltaTime;
                this.timer.value = (TIME_PER_GAME - this.timeRemaining) / TIME_PER_GAME;
                this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
            } else if(this.timeRemaining <= 0.0f) {
                this.gameIsStarted = false;
            }
        }
    }

    // unsub from events to prevent potential future null reference
    private void OnDestroy() {
        RulesUI.StartGame -= StartGame;
        FruitActionManager.ValidPairCreated -= PairCreated;
    }

    /**** Events ****/
    private void PairCreated() {
        this.score++;
        this.timeRemaining = Mathf.Min(TIME_PER_GAME, this.timeRemaining + 5.0f);
    }

    // on game start, set variable to allow clock to being ticking
    private void StartGame() {
        this.gameIsStarted = true;
    }
}
