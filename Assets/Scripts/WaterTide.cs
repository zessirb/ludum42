﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTide : MonoBehaviour {
    List<TileInfo> seaTiles = new List<TileInfo>();
    public enum TideState
    {
        RISING,
        FALLING,
        STILL
    }

    [SerializeField] private AnimatedTile waterTile;
    [SerializeField] private TideState state;
    [SerializeField] private int actualLayer;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject erosion;
    [SerializeField] private float risingTimer;
    [SerializeField] private GameObject mapManager;

    private int startingLayer = 2;
    public int maxLayer = 6;
    public int minLayer = 0;
    public float period = 20.0f;
    private bool gonnaRise = false;
    private float timeLeft;
    private TileInfo[,] terrainTilesInfo;

    private readonly string waterTag = "WaterTilemap";
    private readonly string managerTag = "Manager";
    // Use this for initialization
    void Start () {
        TileInfo.SetTileMapWater( water.GetComponent<Tilemap>(), 40);
        TileInfo.SetWaterTile(waterTile);
        terrainTilesInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo();
        for(int i = 0; i<terrainTilesInfo.GetLength(0);i++ ){
           for(int j = 0; j<terrainTilesInfo.GetLength(1);j++ ){
                if(terrainTilesInfo[i,j].GetIsSea()){
                    seaTiles.Add(terrainTilesInfo[i,j]);
                }
           }
        }

        water = GameObject.FindGameObjectWithTag(waterTag);
        erosion = GameObject.FindGameObjectWithTag(managerTag);

        if (water != null)
        {
            startingLayer = water.GetComponent<TilemapRenderer>().sortingOrder;
            state = TideState.STILL;
            risingTimer = 4.0f;
            timeLeft = risingTimer;
            actualLayer = startingLayer;
            GameObject.FindGameObjectsWithTag("Grid").First().GetComponent<IslandGenerator>().SetWater(actualLayer);
            // Change the state of the tide with the current state selected (after 'period' seconds)
            Invoke("ChangeTide", period);
        } else
        {
            Debug.LogError("Could not get Water with Tag " + waterTag);
        }
    }
	
	// Update is called once per frame
	void Update () {
        CheatyStateSettings();
        Tide();
		Flood();
    }   

    private void ChangeTide()
    {
        switch(state)
        {
            case TideState.STILL:
                break;
            case TideState.FALLING:
                if (actualLayer > minLayer)
                {
                    actualLayer--;
                }
                break;
            case TideState.RISING:
                if (actualLayer < maxLayer)
                {
                    actualLayer++;
                }
                break;
        }
        UpdateTideLayer();
        erosion.GetComponent<ErosionManager>().Erode();

        // Act as a clock, re invoke this method after 'period' seconds
        Invoke("ChangeTide", period);
    }

    private void UpdateTideLayer()
    {
        GameObject.FindGameObjectsWithTag("Grid").First().GetComponent<IslandGenerator>().SetWater(actualLayer);
        water.GetComponent<TilemapRenderer>().sortingOrder = actualLayer;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public bool WillRise()
    {
        return gonnaRise;
    }

    public bool IsStill()
    {
        return state == TideState.STILL;
    }
    
    public void SetTideState(TideState state)
    {
        this.state = state;
    }

    public float GetRisingTimer()
    {
        return this.risingTimer;
    }

    private void CheatyStateSettings()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            state = TideState.FALLING;
            Debug.Log("Change to FALLING");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            state = TideState.STILL;
            Debug.Log("Change to STILL");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            state = TideState.RISING;
            Debug.Log("Change to RISING");
        }
    }

    private void Tide()
    {
        timeLeft -= Time.deltaTime;
        if ( timeLeft <= 0 )
        {
            timeLeft = risingTimer;
            switch (state)
            {
                case TideState.STILL:
                    if (gonnaRise)
                    {
                        state = TideState.RISING;
                    } else
                    {
                        state = TideState.FALLING;
                    }
                    gonnaRise = !gonnaRise;
                    break;
                case TideState.FALLING:
                    state = TideState.STILL;
                    break;
                case TideState.RISING:
                    state = TideState.STILL;
                    break;
            }
        } 
    }

    public void Flood(){
        List<TileInfo> submergedTiles = new List<TileInfo>();
        submergedTiles.AddRange(seaTiles);
        int[,] DataMap = mapManager.GetComponent<MapManager>().GetMapDataCoordinates();
        terrainTilesInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo(); 
        List<TileInfo> tileDone = new List<TileInfo>();
        while(submergedTiles.Count() != 0){
            TileInfo tile = submergedTiles[0];
            submergedTiles.RemoveAt(0);
            if (tile.GetCoordinates()[0] == 0 ||
                 tile.GetCoordinates()[0] == DataMap.GetLength(0) - 1 ||
                 tile.GetCoordinates()[1] == 0 ||
                 tile.GetCoordinates()[1] == DataMap.GetLength(1) - 1)
            {

            }else{
                Vector3Int[] neighbours = tile.GetNeighboursCoordinates();
                foreach(Vector3Int neighbour in neighbours){
                    if(terrainTilesInfo[neighbour.x,neighbour.y].GetIsFlooded()){
                        if(!tileDone.Contains(terrainTilesInfo[neighbour.x,neighbour.y])){
                            submergedTiles.Add(terrainTilesInfo[neighbour.x,neighbour.y]);
                            tileDone.Add(terrainTilesInfo[neighbour.x,neighbour.y]);
                        }
                   }else{
                       if(DataMap[neighbour.x,neighbour.y]<3){
                           terrainTilesInfo[neighbour.x,neighbour.y].SetIsFlooded(true);
                           tileDone.Add(terrainTilesInfo[neighbour.x,neighbour.y]);
                       }
                   }
                 }
            }
        }
       


    }
}
