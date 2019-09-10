using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FruitUI : MonoBehaviour {

    // fruit identifiers
    private int id;
    private FruitType fruitType;

    // button and image attached to component
    private Button selectButton;
    private Image fruitImage;

    [SerializeField] private Sprite apple;
    [SerializeField] private Sprite lemon;
    [SerializeField] private Sprite lime;
    [SerializeField] private Sprite orange;
    [SerializeField] private Sprite grape;
    [SerializeField] private Sprite cherry;

    // subscribe to this delegate to know when a fruit has been selected
    public delegate void FruitEventHandler(int id);
    public static event FruitEventHandler FruitSelected;

    private void Awake() {
        this.fruitImage = this.GetComponent<Image>();
        this.selectButton = this.GetComponent<Button>();
        this.selectButton.onClick.AddListener(OnFruitSelect);
        InitFruit();
    }

    private void OnDestroy() {
        this.selectButton.onClick.RemoveListener(OnFruitSelect);
    }

    private void HideFruit() {
        this.selectButton.enabled = false;
        this.fruitImage.enabled = false;
    }

    private void InitFruit() {
        // select randomly from the FruitTypes available and init fruit logic
        Array fruitTypes = Enum.GetValues(typeof(FruitType));
        this.fruitType = (FruitType)fruitTypes.GetValue(Random.Range(0, fruitTypes.Length));
        switch(this.fruitType) {
            case FruitType.Apple:
                this.fruitImage.sprite = this.apple;
                break;
            case FruitType.Lemon:
                this.fruitImage.sprite = this.lemon;
                break;
            case FruitType.Lime:
                this.fruitImage.sprite = this.lime;
                break;
            case FruitType.Orange:
                this.fruitImage.sprite = this.orange;
                break;
            case FruitType.Grape:
                this.fruitImage.sprite = this.grape;
                break;
            case FruitType.Cherry:
                this.fruitImage.sprite = this.cherry;
                break;
        }
    }

    /**** Events ****/
    private void OnFruitSelect() {
        // call method in delegate to notify FruitActionManager
        //Debug.Log(this.fruit.printNeighbours());
        FruitSelected(this.id);
    }

    /**** Public API ****/
    public void SetId(int id) {
        this.id = id;
    }

    public FruitType GetFruitType() {
        return this.fruitType;
    }
}
