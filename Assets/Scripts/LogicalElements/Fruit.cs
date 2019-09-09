using System;
using System.Collections.Generic;
using System.IO;

public class Fruit {

    private FruitType type;
    private int id;
    private Dictionary<Direction, Fruit> neighbours;

    public Fruit(FruitType type) {
        this.type = type;
        this.neighbours = new Dictionary<Direction, Fruit>();
    }

    private void UpdateNeighbourPair(Direction A, Direction B, Fruit fruit) {
        Fruit neighbourA = null;
        Fruit neighbourB = null;
        this.neighbours.TryGetValue(A, out neighbourA);
        this.neighbours.TryGetValue(B, out neighbourB);
        if (neighbourA != null || neighbourB != null) {
            if (neighbourA == null) {
                neighbourB.RemoveNeighbour(A);
            } else if (neighbourB == null) {
                neighbourA.RemoveNeighbour(B);
            } else {
                if (neighbourA.id == fruit.id) {
                    neighbourA = fruit.GetNeighbourByDirection(A);
                    if (neighbourA == null) {
                        neighbourB.RemoveNeighbour(A);
                    } else {
                        neighbourB.SetNeighbour(neighbourA, A);
                    }
                } else if (neighbourB.id == fruit.id) {
                    neighbourB = fruit.GetNeighbourByDirection(B);
                    if (neighbourB == null) {
                        neighbourA.RemoveNeighbour(B);
                    } else {
                        neighbourA.SetNeighbour(neighbourB, B);
                    }
                } else {
                    neighbourA.SetNeighbour(neighbourB, B);
                    neighbourB.SetNeighbour(neighbourA, A);
                }
            }
        }
    }

    private bool WallIsCommonNeighbour(Fruit fruit) {
        return ((GetNeighbourByDirection(Direction.N) == null && fruit.GetNeighbourByDirection(Direction.N) == null)
            || (GetNeighbourByDirection(Direction.S) == null && fruit.GetNeighbourByDirection(Direction.S) == null)
            || (GetNeighbourByDirection(Direction.E) == null && fruit.GetNeighbourByDirection(Direction.E) == null)
            || (GetNeighbourByDirection(Direction.W) == null && fruit.GetNeighbourByDirection(Direction.W) == null));
    }

    private bool isCommonNeighbour(Fruit fruit, Direction A, Direction B) {
        return (GetNeighbourByDirection(A) != null && fruit.GetNeighbourByDirection(B) != null
                && GetNeighbourByDirection(A).GetId() == fruit.id &&
                fruit.GetNeighbourByDirection(B).GetId() == this.id);
    }

    /**** Public API ****/
    public void SetId(int id) {
        this.id = id;
    }

    public void SetNeighbour(Fruit neighbour, Direction d) {
        if(!this.neighbours.ContainsKey(d)) {
            this.neighbours.Add(d, neighbour);
        } else {
            this.neighbours[d] = neighbour;
        }
    }

    public void RemoveNeighbour(Direction d) {
        this.neighbours.Remove(d);
    }

    public FruitType GetFruitType() {
        return this.type;
    }

    public int GetId() {
        return this.id;
    }

    public Fruit GetNeighbourByDirection(Direction d) {
        Fruit f = null;
        this.neighbours.TryGetValue(d, out f);
        
        return f;
    }

    public bool isValidNeighbour(Fruit fruit) {
        bool isNeighbour = false;
        if (fruit.GetFruitType() == this.type) {
            // either the edge of the grid is a common neighbour or the fruits are neighbours by being
            // in line with each other (fruitA is left of fruitB && fruitB is right of fruitA
            if (WallIsCommonNeighbour(fruit)
                || isCommonNeighbour(fruit, Direction.N, Direction.S)
                || isCommonNeighbour(fruit, Direction.S, Direction.N)
                || isCommonNeighbour(fruit, Direction.E, Direction.W)
                || isCommonNeighbour(fruit, Direction.W, Direction.E) ) { 
    
                isNeighbour = true;
            }
        }
        return isNeighbour;
    }

    public void UpdateNeighbours(Fruit fruit) {
        // perform vertical swap
        UpdateNeighbourPair(Direction.N, Direction.S, fruit);
        UpdateNeighbourPair(Direction.E, Direction.W, fruit);
    }

    public String printNeighbours() {
        String printMe = "";
        foreach (KeyValuePair<Direction, Fruit> entry in this.neighbours) {
            // do something with entry.Value or entry.Key
            int id = entry.Value.GetId();
            string dir = "";
            switch(entry.Key) {
                case Direction.E:
                    dir = "E: ";
                    break;
                case Direction.W:
                    dir = "W: ";
                    break;
                case Direction.S:
                    dir = "S: ";
                    break;
                case Direction.N:
                    dir = "N: ";
                    break;
            }
            printMe += dir + id + " ";
        }
        return printMe;
    }
}