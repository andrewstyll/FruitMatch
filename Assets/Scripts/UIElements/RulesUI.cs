using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesUI : MonoBehaviour {

    private Button startGameButton;

    // subscribe to this delegate to know when to start the game
    public delegate void StartGameHandler();
    public static event StartGameHandler StartGame;

    private void Awake() {
        this.startGameButton = gameObject.GetComponent<Button>();
        this.startGameButton.onClick.AddListener(StartGameClick);
    }

    private void OnDestroy() {
        this.startGameButton.onClick.RemoveListener(StartGameClick);
    }
    // on button click, trigger start game event
    private void StartGameClick() {
        StartGame();
    }
}
