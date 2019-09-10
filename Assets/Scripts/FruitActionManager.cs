using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitActionManager : MonoBehaviour {

    [SerializeField] private GameObject fruitUI;
    [SerializeField] private GameObject lineUI;
    [SerializeField] private int Rows;
    [SerializeField] private int Columns;

    // -1 means no fruit currently selected, use to limit selected fruit to 2
    private int fruitSelectedIDOne = -1;
	private int fruitSelectedIDTwo = -1;
    private List<int> connectingPath = null;

    private const float SELECT_REMOVAL_TIME = 0.5f;
    private float removalTime = 0.0f;
    private bool startTimer = false;

	// store spawned UI, may need to access later? we'll see.
	private GameObject[] UIElements;
    private bool[] hasFruit;

    [SerializeField] private EventSystem eventSystem;

    // Start is called before the first frame update
    private void Awake() {
        FruitUI.FruitSelected += HandleFruitSelect;
        SpawnUI();
    }

    private void Start() {
        InitFruitUI();
    }

    private void Update() {

        if(PairSelected()) {
            if(startTimer) {
                DrawLine();
                startTimer = false;
            }
            removalTime += Time.deltaTime;
            if (removalTime > SELECT_REMOVAL_TIME) {
                ReplaceWithLine(fruitSelectedIDOne);
                ReplaceWithLine(fruitSelectedIDTwo);
                HideLine();
                fruitSelectedIDTwo = -1;
                fruitSelectedIDOne = -1;
                connectingPath = null;
                removalTime = 0.0f;
            }
            
        }
	}

    private void OnDestroy() {
        FruitUI.FruitSelected -= HandleFruitSelect;
    }
    
    private void SpawnUI() {
        this.UIElements = new GameObject[this.Rows * this.Columns];
        this.hasFruit = new bool[this.Rows * this.Columns];

        for (int i = 0; i < this.Rows; i++) {
            for(int j = 0; j < this.Columns; j++) {
                GameObject newUI;
                if (i > 0 && i < this.Rows-1 && j > 0 && j < this.Columns-1) {
                    newUI = (GameObject)Instantiate(this.fruitUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
                    this.hasFruit[this.Columns * i + j] = true;
                } else {
                    // spawn lineUI here
                    newUI = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
                    this.hasFruit[this.Columns * i + j] = false;
                }
                newUI.transform.SetSiblingIndex(Columns * i + j);
            }
        }
    }

    private void InitFruitUI() {
        // only iterate through fruit, so avoid outside edge of grid
        for (int i = 1; i < this.Rows-1; i++) {
            for (int j = 1; j < this.Columns-1; j++) {
                FruitUI UIScript = this.UIElements[this.Columns * i + j].GetComponent<FruitUI>();
                UIScript.SetId(this.Columns * i + j);
                // set neighbours
                Fruit neighbour;
                if(i > 1) {
                    neighbour = this.UIElements[this.Columns * (i - 1) + j].GetComponent<FruitUI>().GetFruit();
                    UIScript.SetNeighbour(neighbour, Direction.N);
                }
                if(i < this.Rows - 2) {
                    neighbour = this.UIElements[this.Columns * (i + 1) + j].GetComponent<FruitUI>().GetFruit();
                    UIScript.SetNeighbour(neighbour, Direction.S);
                }
                if(j > 1) {
                    neighbour = this.UIElements[this.Columns * i + j - 1].GetComponent<FruitUI>().GetFruit();
                    UIScript.SetNeighbour(neighbour, Direction.W);
                }
                if(j < this.Columns - 2) {
                    neighbour = this.UIElements[this.Columns * i + j + 1].GetComponent<FruitUI>().GetFruit();
                    UIScript.SetNeighbour(neighbour, Direction.E);
                }
            }
        }
    }

    private void ReplaceWithLine(int id) {
        // bump old element to end of siblings to prevent sibling shuffling before destroy portion of gameLoop
        this.UIElements[id].transform.SetAsLastSibling();
        Destroy(this.UIElements[id]);
        this.UIElements[id] = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
        this.UIElements[id].transform.SetSiblingIndex(id);
        this.hasFruit[id] = false;
    }

    private void DrawLine() {

        // draw line for each inbetween line element. Need to provide previous and next element in path so
        // that each UI element can select correct sprite
        for(int i = 0; i < this.connectingPath.Count; i++) {
            GameObject lineElement = UIElements[this.connectingPath[i]];
            int iPrev = fruitSelectedIDOne / this.Columns;
            int jPrev = fruitSelectedIDOne % this.Columns;
            int iCurr = this.connectingPath[i] / this.Columns;
            int jCurr = this.connectingPath[i] % this.Columns;
            int iNext = fruitSelectedIDTwo / this.Columns;
            int jNext = fruitSelectedIDTwo % this.Columns;

            if (i > 0) {
                iPrev = this.connectingPath[i - 1] / this.Columns;
                jPrev = this.connectingPath[i - 1] % this.Columns;
            }
            if (i < this.connectingPath.Count - 1) {
                iNext = this.connectingPath[i + 1] / this.Columns;
                jNext = this.connectingPath[i + 1] % this.Columns;
            }

            lineElement.GetComponent<LineUI>().ChooseImage(iPrev, jPrev, iCurr, jCurr, iNext, jNext);
        }
    }

    private void HideLine() {
        for (int i = 0; i < this.connectingPath.Count; i++) {
            GameObject lineElement = UIElements[this.connectingPath[i]];
            lineElement.GetComponent<LineUI>().RemoveImage();
        }
    }

    private bool PairSelected() {
        return (this.fruitSelectedIDOne != -1 && this.fruitSelectedIDTwo != -1 && connectingPath != null);
    }

    private bool AreNeighbours(int idA, int idB) {
        FruitUI fruitA = this.UIElements[idA].GetComponent<FruitUI>();
        FruitUI fruitB = this.UIElements[idB].GetComponent<FruitUI>();
        
        return (fruitA.CompareNeighbour(fruitB.GetFruit())
                && fruitB.CompareNeighbour(fruitA.GetFruit()));
	}

    private void UpdateNeighbours(int idA, int idB) {
        FruitUI fruitA = this.UIElements[idA].GetComponent<FruitUI>();
        FruitUI fruitB = this.UIElements[idB].GetComponent<FruitUI>();
        fruitA.UpdateNeighbours(fruitB.GetFruit());
        fruitB.UpdateNeighbours(fruitA.GetFruit());
    }

    // Perform a BFS to find the shortes valid path between 2 pieces to draw the red connecting line
    // important as displays the rules of the game
    private List<int> GetPathBetweenSelected(int start, int finish) {
        List<int>[,] paths = new List<int>[this.Rows, this.Columns];
        Queue<int> BFS = new Queue<int>();
        paths[start / this.Columns, start % this.Columns] = new List<int>();

        // fill BFS with neighbours. Add current neighbour to list when moving to new neighbour as we
        // don't want to change color of fruit squares, just the spaces in between

        BFS.Enqueue(start);
        while (BFS.Count > 0) {
            int position = BFS.Dequeue();
            int i = position / this.Columns;
            int j = position % this.Columns;
            List<int> path = paths[i, j];
            if(position != start) {
                path.Add(position);
            }

            // find neighbours
            if(i > 0 && paths[i-1,j] == null &&
                (!hasFruit[this.Columns*(i - 1) + j] || this.Columns*(i - 1) + j == finish)) {
                paths[i-1,j] = new List<int>(path);
                if(this.Columns * (i - 1) + j == finish) {
                    return paths[i - 1, j];
                }
                BFS.Enqueue(this.Columns * (i - 1) + j);
            }
            if (i < this.Rows-1 && paths[i+1, j] == null &&
                (!hasFruit[this.Columns * (i + 1) + j] || this.Columns * (i + 1) + j == finish)) {
                paths[i+1,j] = new List<int>(path);

                if (this.Columns * (i + 1) + j == finish) {
                    return paths[i + 1, j];
                }
                BFS.Enqueue(this.Columns * (i + 1) + j);
            }
            if (j > 0 && paths[i, j-1] == null &&
                (!hasFruit[this.Columns * i + j-1] || this.Columns * i + j-1 == finish)) {
                paths[i,j-1] = new List<int>(path);

                if (this.Columns * i + j-1 == finish) {
                    return paths[i, j - 1];
                }
                BFS.Enqueue(this.Columns * i + j - 1);
            }
            if (j < this.Columns-1 && paths[i, j+1] == null &&
                (!hasFruit[this.Columns * i + j+1] || this.Columns * i + j+1 == finish)) {
                paths[i,j+1] = new List<int>(path);

                if (this.Columns * i + j + 1 == finish) {
                    return paths[i, j + 1];
                }
                BFS.Enqueue(this.Columns * i + j + 1);
            }
        }
        // don't return null, tricky returns for no reason are never fun
        return new List<int>();
    }

    /**** Events ****/
    private void HandleFruitSelect(int id) {
        // line elements can't be selected
        if(fruitSelectedIDOne == -1 && fruitSelectedIDTwo == -1) {
			fruitSelectedIDOne = id;
        } else if(fruitSelectedIDOne != -1 && fruitSelectedIDTwo == -1) {
			// call compare on fruit one and two
			if (id != fruitSelectedIDOne && AreNeighbours(fruitSelectedIDOne, id)) {
				fruitSelectedIDTwo = id;
                
                this.connectingPath = GetPathBetweenSelected(fruitSelectedIDOne, fruitSelectedIDTwo);
                UpdateNeighbours(fruitSelectedIDOne, fruitSelectedIDTwo);
                startTimer = true;
				//Debug.Log(fruitSelectedIDOne + " " + fruitSelectedIDTwo);
            } else {
                // use event handler here to deselect button for good UI
				fruitSelectedIDTwo = -1;
				fruitSelectedIDOne = -1;
			}
            this.eventSystem.SetSelectedGameObject(null);
        } else {
            // something went wrong
            Debug.Log("Invalid State: " + fruitSelectedIDOne + " " + id);
        }
    }
}
