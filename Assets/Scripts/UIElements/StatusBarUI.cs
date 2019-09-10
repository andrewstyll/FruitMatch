using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour {

    private const float TIME_PER_GAME = 60.0f;
    private const string SCORE_BASE_TEXT = "Score: ";

    private int score;
    private float timeRemaining;
    private Slider timer;
    private Text scoreText;

    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject scoreObject;

    private void Awake() {

        FruitActionManager.ValidPairCreated += PairCreated;
        this.score = 0;
        this.timeRemaining = TIME_PER_GAME;

        this.timer = timerObject.GetComponent<Slider>();
        this.scoreText = scoreObject.GetComponent<Text>();

        this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
    }

    private void Update() {
        if(this.timeRemaining > 0.0f) {
            this.timeRemaining -= Time.deltaTime;
            this.timer.value = (TIME_PER_GAME - this.timeRemaining) / TIME_PER_GAME;
            this.scoreText.text = SCORE_BASE_TEXT + score.ToString();
        }
    }

    private void OnDestroy() {
        FruitActionManager.ValidPairCreated -= PairCreated;
    }

    /**** Events ****/
    private void PairCreated() {
        this.score++;
        this.timeRemaining = Mathf.Min(TIME_PER_GAME, this.timeRemaining + 5.0f);
    }
}
