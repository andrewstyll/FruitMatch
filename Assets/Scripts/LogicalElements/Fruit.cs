using System;
using System.Collections.Generic;
using System.IO;

public class Fruit {

    private int id;
    private FruitType type;

    // references neighbours of current fruit. id Direction doesn't exist, a map edge is on that direction
    private Dictionary<Direction, Fruit> neighbours;

    public Fruit(FruitType type) {
        this.type = type;
        this.neighbours = new Dictionary<Direction, Fruit>();
    }

    // neighbour update logic given a direction pair (vertical, horizontal)
    private void UpdateNeighbourPair(Direction A, Direction B, Fruit fruit) {
        Fruit neighbourA = null;
        Fruit neighbourB = null;
        this.neighbours.TryGetValue(A, out neighbourA);
        this.neighbours.TryGetValue(B, out neighbourB);

        // both neighbours aren't wall edges, so theres some work to do
        if (neighbourA != null || neighbourB != null) {

            // if either neighbour is a wall edge, the other neighbour removes that direction,
            // as it's neighbour in that same direction is now a wall edge
            if (neighbourA == null) {
                neighbourB.RemoveNeighbour(A);
            } else if (neighbourB == null) {
                neighbourA.RemoveNeighbour(B);
            } else {

                /* both neighbours valid, if one of them is the neighbour being removed, the other neighbour
                 * needs the neighbour of that to be removed neighbour
                 * ex. B is north, A is south and needs new north, but be is being removed (therefor not valid neighbour)
                 * A takes b's north neighbour as it's own north neighbour (it's kind of tricky...)
                 */
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
                    // neither neighbour being removed, simple neighbour swap
                    neighbourA.SetNeighbour(neighbourB, B);
                    neighbourB.SetNeighbour(neighbourA, A);
                }
            }
        }
    }

    // if both fruits don't have the same direction stored, a map edge is shared in that direction
    private Direction MapEdgeIsNeighbour(Fruit fruit) {
        Direction d = Direction.NAD;
        if(GetNeighbourByDirection(Direction.N) == null && fruit.GetNeighbourByDirection(Direction.N) == null) {
            d = Direction.N;
        } else if(GetNeighbourByDirection(Direction.S) == null && fruit.GetNeighbourByDirection(Direction.S) == null) {
            d = Direction.S;
        } else if(GetNeighbourByDirection(Direction.E) == null && fruit.GetNeighbourByDirection(Direction.E) == null) {
            d = Direction.E;
        } else if(GetNeighbourByDirection(Direction.W) == null && fruit.GetNeighbourByDirection(Direction.W) == null) {
            d = Direction.W;
        }
        return d;
    }

    // is A a direct neighbour of B???
    private bool IsDirectNeighbour(Fruit fruit, Direction A) {
        return (GetNeighbourByDirection(A) != null && GetNeighbourByDirection(A).GetId() == fruit.id);
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

    // returns the neighbour given the direction or null (not a fan of null returns...)
    public Fruit GetNeighbourByDirection(Direction d) {
        Fruit f = null;
        this.neighbours.TryGetValue(d, out f);
        
        return f;
    }

    // get the direction of the potential neighbour. Favour direct Neighbours (likely shortest path)
    public Direction FindNeighbourDir(Fruit fruit) {
        Direction d = Direction.NAD;
        if (fruit.GetFruitType() == this.type) {
            // either the edge of the grid is a common neighbour or the fruits are neighbours by being
            // in line with each other (fruitA is left of fruitB && fruitB is right of fruitA
            if(IsDirectNeighbour(fruit, Direction.N)) {
                d = Direction.N;
            } else if(IsDirectNeighbour(fruit, Direction.S)) {
                d = Direction.S;
            } else if(IsDirectNeighbour(fruit, Direction.E)) {
                d = Direction.E;
            } else if(IsDirectNeighbour(fruit, Direction.W)) {
                d = Direction.W;
            } else {
                d = MapEdgeIsNeighbour(fruit);
            }
        }
        return d;
    }

    // update assuming this fruit and argument fruit are being removed
    public void UpdateNeighbours(Fruit fruit) {
        // perform vertical swap, then horizontal
        UpdateNeighbourPair(Direction.N, Direction.S, fruit);
        UpdateNeighbourPair(Direction.E, Direction.W, fruit);
    }

    // debug method to verify neighbour set and update
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