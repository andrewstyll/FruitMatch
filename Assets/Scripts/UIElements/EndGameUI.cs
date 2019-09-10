using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour {

    private string score = "";

    // end game display data
    private Button restartGame;
    private Text scoreText;

    [SerializeField] private GameObject scoreDisplay;

    // subscribe to this delegate to know when to start the game
    public delegate void EndGameHandler();
    public static event EndGameHandler RestartGame;

    private void Awake() {
        restartGame = this.gameObject.GetComponent<Button>();
        this.scoreText = scoreDisplay.GetComponent<Text>();

        StatusBarUI.EndGame += UpdateScore;
        this.restartGame.onClick.AddListener(RestartGameClick);
    }

    private void Update() {
        this.scoreText.text = score;
    }

    private void OnDestroy() {
        StatusBarUI.EndGame -= UpdateScore;
        this.restartGame.onClick.RemoveListener(RestartGameClick);
    }

    /**** Events ****/
    private void RestartGameClick() {
        RestartGame();
    }

    // display the score
    private void UpdateScore(string gameScore) {
        this.score = gameScore;
    }
}
