using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundUI : MonoBehaviour {

    // instructions image
    [SerializeField] private GameObject instructions;

    private void Awake() {
        RulesUI.StartGame += StartGame;
    }

    private void OnDestroy() {
        RulesUI.StartGame -= StartGame;
    }

    // deactivate the instructions window
    private void StartGame() {
        instructions.SetActive(false);
    }
}
