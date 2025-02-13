using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    [Header("Tile")]
    public GameObject tilePrefab;
    private BackGroundTile[,] allTiles;

    [Header("Candy")]
    public GameObject[] candies;
    public GameObject[,] allCandies;
    [Header("Parent")]
    public Transform backGroundTileParent;
    public Transform candyParent;
    void Start()
    {
        allTiles = new BackGroundTile[width, height];
        SetUp();
    }

    public void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile= Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.SetParent(backGroundTileParent);
                backgroundTile.name="("+i+","+j+")";

                int candyToUse=Random.Range(0, candies.Length);
                GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                candy.transform.SetParent(candyParent);
                candy.name = "Candy " + "(" + i + "," + j + ")";
            }
        }
    }
}
