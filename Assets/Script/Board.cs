using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


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

    [Header("CheckSystem")]
    public CandyScribleObject[] allCandySO;
    
    void Start()
    {
        allTiles = new BackGroundTile[width, height];
        allCandies = new GameObject[width, height];
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
                float additionalHeight = tempPosition.y + 10;
                GameObject candy = Instantiate(candies[candyToUse], new Vector2(tempPosition.x,additionalHeight), Quaternion.identity);

                candy.GetComponent<Candy>().targetX = tempPosition.x;
                candy.GetComponent<Candy>().targetY = tempPosition.y;

                candy.transform.SetParent(candyParent);
                candy.name = "Candy " + "(" + i + "," + j + ")";
                allCandies[i,j] = candy;
            }
        }

        CheckAllMatches();
    }

  

    public void CheckAllMatches()
    {
        for (int i = 0; i < allCandySO.Length; i++)
        {
            int value = FindMatches(allCandySO[i].tag);
            //Debug.Log(allCandySO[i]+" Sekerden"+ value + "adet var");

            if (value >= 8 && value <= 9)
            {
                // 8 ile 9 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].oneMatch * value);

                destroyCandy(allCandySO[i].tag);

            }
            else if (value >= 10 && value <= 11)
            {
                // 10 ile 11 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].twoMatch * value);
                destroyCandy(allCandySO[i].tag);
            }
            else if (value >= 12)
            {
                // 12 ve yukarýsý için yapýlacak iþlem
                Debug.Log(allCandySO[i].threeMatch * value);
                destroyCandy(allCandySO[i].tag);
            }
        }
    }

    public int FindMatches(string tag)
    {
        int count = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i,j].tag == tag)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void destroyCandy(string tag)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j].tag == tag)
                {
                    Destroy(allCandies[i, j]);
                }
            }
        }
    }
}
