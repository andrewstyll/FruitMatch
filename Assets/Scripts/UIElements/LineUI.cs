﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LineUI : MonoBehaviour {
    // Use this for initialization
    private Image lineImage;
    private bool makeImageVisible = false;

    [SerializeField] private Sprite SE;
    [SerializeField] private Sprite NE;
    [SerializeField] private Sprite SW;
    [SerializeField] private Sprite NW;
    [SerializeField] private Sprite Horizontal;
    [SerializeField] private Sprite Vertical;


    private void Awake() {
        this.lineImage = this.gameObject.GetComponent<Image>();
        HideImage();
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(makeImageVisible && this.lineImage.enabled == false) {
            ShowImage();
        } else if(!makeImageVisible && this.lineImage.enabled == true) {
            HideImage();
        }
    }

    private void HideImage() {
        this.lineImage.enabled = false;
    }

    private void ShowImage() {
        this.lineImage.enabled = true;
    }

    private Direction GetCardinalDirection(int i, int j, int myI, int myJ) {
        if(i > myI) {
            return Direction.S;
        } else if(i < myI) {
            return Direction.N;
        } else {
            if(j < myJ) {
                return Direction.W;
            } else {
                return Direction.E;
            }
        }
    }

    /**** Public API ****/
    public void ChooseImage(int iPrev, int jPrev, int iCurr, int jCurr, int iNext, int jNext) {
        Sprite lineSprite;

        Direction prev = GetCardinalDirection(iPrev, jPrev, iCurr, jCurr);
        Direction next = GetCardinalDirection(iNext, jNext, iCurr, jCurr);

        if(prev == Direction.N && next == Direction.S ||
            prev == Direction.S && next == Direction.N) {
            lineSprite = Vertical;
        } else if (prev == Direction.E && next == Direction.W ||
            prev == Direction.W && next == Direction.E) {
            lineSprite = Horizontal;
        } else if (prev == Direction.N && next == Direction.E ||
            prev == Direction.E && next == Direction.N) {
            lineSprite = NE;
        } else if (prev == Direction.N && next == Direction.W ||
            prev == Direction.W && next == Direction.N) {
            lineSprite = NW;
        } else if (prev == Direction.S && next == Direction.W ||
            prev == Direction.W && next == Direction.S) {
            lineSprite = SW;
        } else {
            lineSprite = SE;
        }

        lineImage.sprite = lineSprite; 
        makeImageVisible = true;
    }

    public void RemoveImage() {
        makeImageVisible = false;
    }
}
