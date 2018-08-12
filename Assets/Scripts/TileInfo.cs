using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo{

    private int x, y;
    private float durability;


    public TileInfo(Vector3Int vector){
        this.durability = 1.0f;
        this.x = vector.x;
        this.y = vector.y;
    }

    public Vector3Int GetCoordinates()
    {
        return new Vector3Int(x, y, 0);
    }

    public Vector3Int[] GetNeighboursCoordinates()
    {
        Vector3Int[] neighbours = new Vector3Int[6];
        neighbours[0] = new Vector3Int(this.x + 1, this.y - 1, 0);
        neighbours[1] = new Vector3Int(this.x + 1, this.y, 0);
        neighbours[2] = new Vector3Int(this.x, this.y + 1, 0);
        neighbours[3] = new Vector3Int(this.x - 1, this.y + 1, 0);
        neighbours[4] = new Vector3Int(this.x - 1, this.y, 0);
        neighbours[5] = new Vector3Int(this.x, this.y - 1, 0);
        return neighbours;
    }

    public void DecreaseDurability(float value)
    {
        this.durability -= value;
    }

    public float GetDurability()
    {
        return this.durability;
    }

    public void ResetDurability()
    {
        this.durability = 1.0f;
    }

}