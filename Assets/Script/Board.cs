using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            Play();
        }
    }

    public void Play()
    {
        if (allCandies[0, 0] != null)
        {
            StartCoroutine(ResetMap());
        }
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

           
                

            }
        }

        StartCoroutine(StartCandySpawn());


    }

    IEnumerator StartCandySpawn()
    {
        for (int i = 0; i < width; i++)
        {
            yield return new WaitForSeconds(0.1f);

            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);

                int candyToUse = Random.Range(0, candies.Length);
                float additionalHeight = tempPosition.y + 10;
                GameObject candy = Instantiate(candies[candyToUse], new Vector2(tempPosition.x, additionalHeight), Quaternion.identity);

                candy.GetComponent<Candy>().targetX = tempPosition.x;
                candy.GetComponent<Candy>().targetY = tempPosition.y;

                candy.transform.SetParent(candyParent);
                candy.name = "Candy " + "(" + i + "," + j + ")";
                allCandies[i, j] = candy;
            }
        }
    }

  

    public bool CheckAllMatches()
    {
        bool isMatched = false;

        for (int i = 0; i < allCandySO.Length; i++)
        {
            int value = FindMatches(allCandySO[i].tag);
            Debug.Log(allCandySO[i]+" Sekerden"+ value + "adet var");

            if (value >= 8 && value <= 9)
            {
                // 8 ile 9 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].oneMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));
                isMatched=true;

            }
            else if (value >= 10 && value <= 11)
            {
                // 10 ile 11 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].twoMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));

                isMatched = true;
            }
            else if (value >= 12)
            {
                // 12 ve yukarýsý için yapýlacak iþlem
                Debug.Log(allCandySO[i].threeMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));

                isMatched = true;
            }
        }

        return isMatched;
    }

    public int FindMatches(string tag)
    {
        int count = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allCandies[i, j] != null)
                {
                    if (allCandies[i, j].tag == tag)
                    {
                        count++;
                    }
                }
                
            }
        }

        Debug.Log(tag+" : "+count);
        return count;
    }
    /*
    public void destroyCandy(string tag)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].tag == tag)
                    {
                        destroyMathesAt(i, j);
                    }
                }
            }
        }

        StartCoroutine(DecreaseRowCo());
    }*/

    IEnumerator DestroyCandyCO(string tag)
    {
        List<GameObject> allMatchCandy = new List<GameObject>();



        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].tag == tag)
                    {
                        //allMatchCandy.Add(allCandies[i, j]);
                        allCandies[i, j].GetComponent<Candy>().isMatched = true;
                        //destroyMathesAt(i, j);
                    }
                }
            }
        }


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched == true)
                    {
                        allCandies[i, j].transform.DOScale(1.5f, 1);
                    }

                }
            }
        }


        yield return new WaitForSeconds(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched == true)
                    {
                        allCandies[i, j].transform.DOScale(1, 1);
                    }

                }
            }
        }

        yield return new WaitForSeconds(1);

        /*
        for (int a = 0; a < allMatchCandy.Count; a++)
        {
            destroyMathesAt((int)allMatchCandy[a].GetComponent<Candy>().targetX, (int)allMatchCandy[a].GetComponent<Candy>().targetY);

        }
        */

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if(allCandies[i, j].GetComponent<Candy>().isMatched==true)
                    {
                        destroyMathesAt(i, j);
                    }
                    
                }
            }
        }


        StartCoroutine(DecreaseRowCo());
    }

    public void destroyMathesAt(int i,int j)
    {
        Destroy(allCandies[i, j].gameObject);
        allCandies[i, j] = null;

    }

    IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i,j]==null)
                {
                    nullCount++;
                }
                else if(nullCount>0)
                {
                    allCandies[i,j].GetComponent<Candy>().targetY -= nullCount;
                    allCandies[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());

        
    }

    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i,j]==null)
                {
                    Vector2 tempPos = new Vector2(i, j+10);
                    int candyToUse = Random.Range(0, candies.Length);

                    GameObject candy = Instantiate(candies[candyToUse],tempPos,Quaternion.identity);
                    candy.GetComponent<Candy>().targetX = i;
                    candy.GetComponent<Candy>().targetY = j;
                    candy.gameObject.transform.SetParent(candyParent);
                    allCandies[i, j] = candy;
                }
            }
        }
    }

    IEnumerator FillBoardCo()
    {
        RefillBoard();
        //yield return new WaitForSeconds(0.5f);

        Debug.LogWarning(checkIsReachCandy());

        yield return new WaitUntil(() => checkIsReachCandy() == true);

        while(CheckAllMatches())
        {
            yield return new WaitForSeconds(3f);
        }

        yield return new WaitForSeconds(1);

        

    }

   

    IEnumerator ResetMap()
    {
        for (int i = 0; i < width; i++)
        {
            yield return new WaitForSeconds(0.2f);

            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    allCandies[i, j].GetComponent<Candy>().targetY = j - 10;
                }

            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j].gameObject!=null)
                {
                    destroyMathesAt(i, j);
                }

            }
        }

        StartCoroutine(StartCandySpawn());

        yield return new WaitForSeconds(2f);


        CheckAllMatches();
    }

    bool checkIsReachCandy()
    {
        bool a = true;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i,j]!=null)
                {
                    a = allCandies[i, j].GetComponent<Candy>().isReachFinish;
                }
                else
                {
                    a = false;
                }
            }
        }
        return a;
   }
}
