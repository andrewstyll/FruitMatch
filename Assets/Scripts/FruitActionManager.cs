using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitActionManager : MonoBehaviour {

    private const int MAX_TURNS = 2;

    // UI prefabs
    [SerializeField] private GameObject fruitUI;
    [SerializeField] private GameObject lineUI;

    // adjustable columns and rows.
    [SerializeField] private int Rows;
    [SerializeField] private int Columns;

    // -1 = fruit currently selected, connectingPath is path between fruit
    private int fruitSelectedIDOne = -1;
	private int fruitSelectedIDTwo = -1;
    private List<int> connectingPath = null;

    // timer to limit display of connecting path
    private const float SELECT_REMOVAL_TIME = 0.25f;
    private float removalTime = 0.0f;
    private bool startTimer = false;

	// store spawned UI by ID
	private GameObject[] UIElements;
    private bool[] isFruit;

    // manage button selecting with EventSystem
    [SerializeField] private EventSystem eventSystem;

    // subscribe to this delegate to know when a pair has been made
    public delegate void FruitPairHandler();
    public static event FruitPairHandler ValidPairCreated;

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
            // time to remove the line and the paired fruits
            if (removalTime > SELECT_REMOVAL_TIME) {
                ReplaceWithLine(fruitSelectedIDOne);
                ReplaceWithLine(fruitSelectedIDTwo);
                HideLine();
                fruitSelectedIDTwo = -1;
                fruitSelectedIDOne = -1;
                connectingPath = null;
                removalTime = 0.0f;
                ValidPairCreated();
            }
            
        }
	}

    // unsub from events to prevent potential future null reference
    private void OnDestroy() {
        FruitUI.FruitSelected -= HandleFruitSelect;
    }

    // draw the LineUI and fruitUI elements
    private void SpawnUI() {
        this.UIElements = new GameObject[this.Rows * this.Columns];
        this.isFruit = new bool[this.Rows * this.Columns];

        for (int i = 0; i < this.Rows; i++) {
            for(int j = 0; j < this.Columns; j++) {
                GameObject newUI;
                if (i > 0 && i < this.Rows-1 && j > 0 && j < this.Columns-1) {
                    newUI = (GameObject)Instantiate(this.fruitUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
                    isFruit[this.Columns * i + j] = true;
                } else {
                    // spawn lineUI here
                    newUI = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
                    isFruit[this.Columns * i + j] = false;
                }
                newUI.transform.SetSiblingIndex(Columns * i + j);
            }
        }
    }

    // set the ID's and neighbours of each fruit element
    private void InitFruitUI() {
        // only iterate through fruit, so avoid outside edge of grid
        for (int i = 1; i < this.Rows-1; i++) {
            for (int j = 1; j < this.Columns-1; j++) {
                FruitUI UIScript = this.UIElements[this.Columns * i + j].GetComponent<FruitUI>();
                UIScript.SetId(this.Columns * i + j);
            }
        }
    }

    // replace a FruitUI with a LineUI as pairs are selected by user
    private void ReplaceWithLine(int id) {
        // bump old element to end of siblings to prevent sibling shuffling before destroy portion of gameLoop
        this.UIElements[id].transform.SetAsLastSibling();
        Destroy(this.UIElements[id]);
        this.UIElements[id] = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
        this.UIElements[id].transform.SetSiblingIndex(id);
        isFruit[id] = false;
    }

    // draw the line connecting two neighbours
    private void DrawLine() {

        /* draw line for each inbetween line element. Need to provide previous and next element in path so
         * that each UI element can select correct sprite. The starting and finishing nodes exist as the first
         * and last element of this list to provide context for the line
         */
        for(int i = 0; i < this.connectingPath.Count; i++) {
            if (i > 0 && i < this.connectingPath.Count - 1) {
                GameObject lineElement = UIElements[this.connectingPath[i]];
                int iPrev = this.connectingPath[i - 1] / this.Columns;
                int jPrev = this.connectingPath[i - 1] % this.Columns;
                int iCurr = this.connectingPath[i] / this.Columns;
                int jCurr = this.connectingPath[i] % this.Columns;
                int iNext = this.connectingPath[i + 1] / this.Columns;
                int jNext = this.connectingPath[i + 1] % this.Columns;

                lineElement.GetComponent<LineUI>().ChooseImage(iPrev, jPrev, iCurr, jCurr, iNext, jNext);
            }
        }
    }

    // hide the line connecting two neighbours
    private void HideLine() {
        for (int i = 0; i < this.connectingPath.Count; i++) {
            GameObject lineElement = UIElements[this.connectingPath[i]];
            lineElement.GetComponent<LineUI>().RemoveImage();
        }
    }

    // have two fruits been selected?
    private bool PairSelected() {
        return (this.fruitSelectedIDOne != -1 && this.fruitSelectedIDTwo != -1 && connectingPath != null);
    }

    private bool AreSameType(int IdFruitA, int IdFruitB) {
        return (UIElements[IdFruitA].GetComponent<FruitUI>().GetFruitType()
                == UIElements[IdFruitB].GetComponent<FruitUI>().GetFruitType());
    }

    private bool IsTurn(int idA, int idB) {
        int iA = idA / this.Columns;
        int jA = idA % this.Columns;
        int iB = idB / this.Columns;
        int jB = idB % this.Columns;

        return (iA != iB && jA != jB);
    }

    private List<int> GetNeighbours(int id) {
        List<int> neighbours = new List<int>();
        int i = id / this.Columns;
        int j = id % this.Columns;

        if (i > 0) {
            neighbours.Add(this.Columns * (i - 1) + j);
        }
        if (i < this.Rows-1) {
            neighbours.Add(this.Columns * (i + 1) + j);
        }
        if (j > 0) {
            neighbours.Add(this.Columns * i + j - 1);
        }
        if (j < this.Columns-1) {
            neighbours.Add(this.Columns * i + j + 1);
        }
        return neighbours;
    }

    private bool ValidPathExists(int IdStart, int IdFinish) {
        // perform BFS by layer
        bool[,] visited = new bool[MAX_TURNS + 1, this.Rows * this.Columns];
        int numTurns = 0; // denotes current # of turns we are on
        bool ret = false;
        Queue<List<int>> agenda = new Queue<List<int>>();
        Queue<List<int>> nextAgenda = new Queue<List<int>>();

        List<int> list = new List<int>();
        list.Add(IdStart);
        agenda.Enqueue(list);
        visited[0, IdStart] = true;

        while (numTurns <= MAX_TURNS) {
            while(agenda.Count > 0) {
                list = agenda.Dequeue();

                // has to be valid space or finish, can't have 3 or more turns
                List<int> neighbours = GetNeighbours(list[list.Count - 1]);
                foreach(int neighbour in neighbours) {
                    int currTurns = numTurns;
                    
                    if (list.Count > 1 && IsTurn(list[list.Count - 2], neighbour)) {
                        
                        currTurns++;
                    }
                    if (currTurns <= MAX_TURNS && !visited[currTurns, neighbour]
                        && (neighbour == IdFinish || !this.isFruit[neighbour])) {
                        
                        List<int> newList = new List<int>(list);
                        newList.Add(neighbour);
                        visited[currTurns, neighbour] = true;
                        if (neighbour == IdFinish) {
                            this.connectingPath = newList;
                            return true;
                        } else if (!this.isFruit[neighbour]) {
                            if (currTurns == numTurns) {
                                agenda.Enqueue(newList);
                            } else {
                                nextAgenda.Enqueue(newList);
                            }
                        }
                    }
                }
            }
            agenda = nextAgenda;
            nextAgenda = new Queue<List<int>>();
            numTurns++;
        }

        return ret;
    }

    /**** Events ****/
    private void HandleFruitSelect(int id) {
        // line elements can't be selected
        if(fruitSelectedIDOne == -1 && fruitSelectedIDTwo == -1) {
			fruitSelectedIDOne = id;
        } else if(fruitSelectedIDOne != -1 && fruitSelectedIDTwo == -1) {

            fruitSelectedIDTwo = id;

            if (fruitSelectedIDTwo != fruitSelectedIDOne && AreSameType(fruitSelectedIDOne, fruitSelectedIDTwo)
                && ValidPathExists(fruitSelectedIDOne, fruitSelectedIDTwo)) {
                
                startTimer = true;
				
            } else {
                // was an invalid second selection (aka not a neighbour)
				fruitSelectedIDTwo = -1;
				fruitSelectedIDOne = -1;
                connectingPath = null;
			}
            // use event handler here to deselect button for good UI
            this.eventSystem.SetSelectedGameObject(null);
        }
    }
}
