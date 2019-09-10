using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundUI : MonoBehaviour {

    // instructions image
    [SerializeField] private GameObject instructions;
    // end game image
    [SerializeField] private GameObject endGame;

    private void Awake() {
        RulesUI.StartGame += StartGame;
        StatusBarUI.EndGame += DisplayEndGame;
        EndGameUI.RestartGame += StartGame;

        endGame.SetActive(false);
        instructions.SetActive(true);
    }

    private void OnDestroy() {
        RulesUI.StartGame -= StartGame;
        StatusBarUI.EndGame -= DisplayEndGame;
        EndGameUI.RestartGame -= StartGame;
    }

    // deactivate the instructions window
    private void StartGame() {
        instructions.SetActive(false);
        endGame.SetActive(false);
    }

    private void DisplayEndGame(string scoreText) {
        // don't need score, just didn't want to
        endGame.SetActive(true);
    }
}
