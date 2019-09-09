using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FruitUI : MonoBehaviour {

    private int id;
    private Fruit fruit;

    private Button selectButton;
    private Image fruitImage;

    [SerializeField] private Sprite apple;
    [SerializeField] private Sprite lemon;
    [SerializeField] private Sprite lime;
    [SerializeField] private Sprite orange;

    public delegate void EventHandler(int id);
    public static event EventHandler FruitSelected;

    private void Awake() {
        this.fruitImage = this.GetComponent<Image>();
        this.selectButton = this.GetComponent<Button>();
        this.selectButton.onClick.AddListener(OnFruitSelect);
        InitFruit();
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnDestroy() {
        this.selectButton.onClick.RemoveListener(OnFruitSelect);
    }

    private void HideFruit() {
        this.selectButton.enabled = false;
        this.fruitImage.enabled = false;
    }

    private void InitFruit() {
        // select randomly from the FruitTypes available
        Array fruitTypes = Enum.GetValues(typeof(FruitType));
        FruitType fruitType = (FruitType)fruitTypes.GetValue(Random.Range(0, fruitTypes.Length));
        this.fruit = new Fruit(fruitType);
        switch(fruitType) {
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
        }
    }

    /**** Events ****/
    private void OnFruitSelect() {
        // call method in delegate to notify FruitActionManager
        //Debug.Log(this.fruit.printNeighbours());
        FruitSelected(this.id);
    }

    /**** API ****/
    public void SetId(int id) {
        this.id = id;
        this.fruit.SetId(id);
    }

    public void SetNeighbour(Fruit neighbour, Direction d) {
        this.fruit.SetNeighbour(neighbour, d);
    }

    public Fruit GetFruit() {
        return this.fruit;
    }

    public bool CompareNeighbour(Fruit fruit) {
        // if neighbour is valid
        return this.fruit.isValidNeighbour(fruit);
    }

    public void UpdateNeighbours(Fruit fruit) {
        this.fruit.UpdateNeighbours(fruit);
    }
}
