using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitActionManager : MonoBehaviour {

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
    private const float SELECT_REMOVAL_TIME = 0.5f;
    private float removalTime = 0.0f;
    private bool startTimer = false;

	// store spawned UI by ID
	private GameObject[] UIElements;

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

        for (int i = 0; i < this.Rows; i++) {
            for(int j = 0; j < this.Columns; j++) {
                GameObject newUI;
                if (i > 0 && i < this.Rows-1 && j > 0 && j < this.Columns-1) {
                    newUI = (GameObject)Instantiate(this.fruitUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
                } else {
                    // spawn lineUI here
                    newUI = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
                    this.UIElements[this.Columns * i + j] = newUI;
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

    // replace a FruitUI with a LineUI as pairs are selected by user
    private void ReplaceWithLine(int id) {
        // bump old element to end of siblings to prevent sibling shuffling before destroy portion of gameLoop
        this.UIElements[id].transform.SetAsLastSibling();
        Destroy(this.UIElements[id]);
        this.UIElements[id] = (GameObject)Instantiate(this.lineUI, gameObject.transform, false);
        this.UIElements[id].transform.SetSiblingIndex(id);
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

    // get the direction of fruitB relative to fruitA
    private Direction FindNeighbourDir(int idFruitA, int idFruitB) {
        FruitUI fruitA = this.UIElements[idFruitA].GetComponent<FruitUI>();
        FruitUI fruitB = this.UIElements[idFruitB].GetComponent<FruitUI>();

        return (fruitA.FindNeighbourDir(fruitB.GetFruit())) ;
	}

    // retrives the Map edge UI ID that is in a straight line from the given point
    // in the given direction
    private int GetMapEdgePosition(Direction d, int point) {
        int i = point / this.Columns;
        int j = point % this.Columns;
        switch(d) {
            case Direction.N:
                return (j);
            case Direction.S:
                return (this.Columns * (this.Rows - 1) + j);
            case Direction.W:
                return (this.Columns * i);
            default:
                return (this.Columns * i + Columns - 1);
        }
    }

    // direct neighbours will share opposing directions as they are in opposing directions relative
    // to each other
    private bool AreDirectNeighbours(Direction dA, Direction dB) {
        return (dA == Direction.W && dB == Direction.E
            || dA == Direction.E && dB == Direction.W
            || dA == Direction.N && dB == Direction.S
            || dA == Direction.S && dB == Direction.N);
    }

    // wall neighbours will both return the relative direction of the map edge that makes them neighbours
    private bool AreMapEdgeNeighbours(Direction dA, Direction dB) {
        return (dA == dB && dA != Direction.NAD);
    }

    // tell the FruitUI elements to update who their neighbours are
    private void UpdateNeighbours(int idA, int idB) {
        FruitUI fruitA = this.UIElements[idA].GetComponent<FruitUI>();
        FruitUI fruitB = this.UIElements[idB].GetComponent<FruitUI>();
        fruitA.UpdateNeighbours(fruitB.GetFruit());
        fruitB.UpdateNeighbours(fruitA.GetFruit());
    }

    // draws a line between two points. line is always vertical or horizontal
    private List<int> BuildLineBetweenPoints(int start, int finish) {

        List<int> filledLine = new List<int>();
        // either i or j of points mush be the same. Iterate along the different one;
        int startI = start / this.Columns;
        int startJ = start % this.Columns;
        int finishI = finish / this.Columns;
        int finishJ = finish % this.Columns;

        // Complex while loop conditions. Basically, done to preserve order of start to finish as some
        // starts and finishes may be graphically above and blow each other
        if(startI == finishI) {
            int j = startJ;
            while ((startJ < finishJ && j < finishJ) || (startJ > finishJ && j > finishJ)) {
                filledLine.Add(this.Columns * startI + j);
                if (startJ < finishJ) j++;
                if (startJ > finishJ) j--;
            }
        } else {
            int i = startI;
            while ((startI < finishI && i < finishI) || (startI > finishI && i > finishI)) {
                filledLine.Add(this.Columns * i + startJ);
                if (startI < finishI) i++;
                if (startI > finishI) i--;
            } 
        }
        return filledLine;

    }

    // Draw a line to the common neighbour. Two types of neighbour, ones that are neighbour via map edge and
    // ones that are directly neighbours. This is only called when we know they are neighbours, so no risk of NAD
    private List<int> GetPathBetweenSelected(int start, int finish, Direction fromStart, Direction fromFinish) {
        // if they are wall neighbours
        List<int> path = new List<int>();

        // the order of the elements in the path are important for the line drawing function. that is why
        // the order of these path.addrange calls must stay in this order as we need the relative direction
        // of the path at each UI element to select the correct sprite
        if (!AreMapEdgeNeighbours(fromStart, fromFinish)) {
            // draw line from start to edge position
            path.AddRange(BuildLineBetweenPoints(start, finish));
        } else {
            // wall neighbour drawings will have two corners and therefor 3 linesegments (start to edge1,
            // edge1 to edge2, and edge2 to finish)
            int startEdge = GetMapEdgePosition(fromStart, start);
            int finishEdge = GetMapEdgePosition(fromFinish, finish);
            path.AddRange(BuildLineBetweenPoints(start, startEdge));
            path.AddRange(BuildLineBetweenPoints(startEdge, finishEdge));
            path.AddRange(BuildLineBetweenPoints(finishEdge, finish));
        }

        // stick the finish on to create a complete history of the path
        path.Add(finish);
        return path;
    }

    /**** Events ****/
    private void HandleFruitSelect(int id) {
        // line elements can't be selected
        if(fruitSelectedIDOne == -1 && fruitSelectedIDTwo == -1) {
			fruitSelectedIDOne = id;
        } else if(fruitSelectedIDOne != -1 && fruitSelectedIDTwo == -1) {
            // compare two from one's perspective and one from two's perspective
            Direction fromFruitOne = FindNeighbourDir(fruitSelectedIDOne, id);
            Direction fromFruitTwo = FindNeighbourDir(id, fruitSelectedIDOne);

            if (id != fruitSelectedIDOne &&
                (AreDirectNeighbours(fromFruitOne, fromFruitTwo) || AreMapEdgeNeighbours(fromFruitOne, fromFruitTwo)) ) {
                // neighbours are valid, build a path between them
				fruitSelectedIDTwo = id;
                this.connectingPath = GetPathBetweenSelected(fruitSelectedIDOne, fruitSelectedIDTwo, fromFruitOne, fromFruitTwo);
                UpdateNeighbours(fruitSelectedIDOne, fruitSelectedIDTwo);
                startTimer = true;
				
            } else {
                //was an invalid second selection (aka not a neighbour)
				fruitSelectedIDTwo = -1;
				fruitSelectedIDOne = -1;
			}
            // use event handler here to deselect button for good UI
            this.eventSystem.SetSelectedGameObject(null);
        }
    }
}
