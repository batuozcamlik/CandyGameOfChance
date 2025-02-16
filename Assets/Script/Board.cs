using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;


public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public bool canPlay = true;

    [Header("Game Speed")]
    public float gameSpeed=1;
    public Toggle gameSpeedToggle;

    [Header("Tile")]
    public GameObject tilePrefab;
    private BackGroundTile[,] allTiles;

    [Header("Candy")]
    public GameObject[] candies;
    public GameObject[,] allCandies;
    [Header("Parent")]
    public Transform backGroundTileParent;
    public Transform candyParent;

    [Header("Check System")]
    public CandyScribleObject[] allCandySO;

    [Header("MoneySystem")]
    public MoneyManager moneyMng;

    [Header("Free Spin System")]
    [Range(0f, 100f)]
    public int freeSpinPosiblty;
    public GameObject freeSpinGameObj;

    [Header("Info")]
    private string testInfoString;


    void Start()
    {
        moneyMng=FindAnyObjectByType<MoneyManager>();

        allTiles = new BackGroundTile[width, height];
        allCandies = new GameObject[width, height];
        SetUp();
    }

    private void Update()
    {
        Time.timeScale = gameSpeed;

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
        if(!canPlay)
        {
            return;
        }
        if (allCandies[0, 0] != null)
        {
            StartCoroutine(ResetMap());
            canPlay = false;
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
        bool checkFreeSpin = FreeSpinCalculateProb();
        List<Vector2> freeSpinSpawnPos = new List<Vector2>();

        if(checkFreeSpin)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 newPos;
                do
                {
                    newPos = new Vector2(Random.Range(0, width), Random.Range(0, height));
                }
                while (freeSpinSpawnPos.Contains(newPos)); // Eðer liste zaten bu pozisyonu içeriyorsa, tekrar üret

                freeSpinSpawnPos.Add(newPos);
            }
        }
        else
        {
            checkFreeSpin = true;
            for (int i = 0; i < Random.Range(0,4); i++)
            {
                Vector2 newPos;
                do
                {
                    newPos = new Vector2(Random.Range(0, width), Random.Range(0, height));
                }
                while (freeSpinSpawnPos.Contains(newPos)); // Eðer liste zaten bu pozisyonu içeriyorsa, tekrar üret

                freeSpinSpawnPos.Add(newPos);
            }
        }
        
        

        for (int i = 0; i < width; i++)
        {
            yield return new WaitForSeconds(0.1f);

            for (int j = 0; j < height; j++)
            {

                if(checkFreeSpin)
                {
                    bool isSpawn = false;

                    for (int a = 0; a < freeSpinSpawnPos.Count; a++)
                    {
                        if(new Vector2(i, j) == freeSpinSpawnPos[a] &&isSpawn==false)
                        {
                            isSpawn = true;
                            yield return new WaitForSeconds(0.1f);
                          

                            GameObject freeSpinObj = Instantiate(freeSpinGameObj, new Vector2(i, j + 10), Quaternion.identity);

                            freeSpinObj.GetComponent<Candy>().targetX = i;
                            freeSpinObj.GetComponent<Candy>().targetY = j;

                            allCandies[i, j] = freeSpinObj;
                        }
                    }
                }

                if(allCandies[i, j]==null)
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
    }

    public bool FreeSpinCalculateProb()
    {
        int a = Random.Range(0, 101); 

        if (a < freeSpinPosiblty) 
        {
            return true;
        }
        else
        {
            return false;
        }

    }
  

    public bool CheckAllMatches()
    {
        bool isMatched = false;
        testInfoString = "";

        for (int i = 0; i < allCandySO.Length; i++)
        {
            int value = FindMatches(allCandySO[i].tag);
            //Debug.Log(allCandySO[i]+" Sekerden"+ value + "adet var");

            testInfoString += allCandySO[i].tag + " Sekerden " + value + " adet var " + "\n";

            if (value >= 8 && value <= 9)
            {
                // 8 ile 9 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].oneMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));
                isMatched=true;
                moneyMng.AddMoney(allCandySO[i].oneMatch * value, allCandySO[i].tag);

            }
            else if (value >= 10 && value <= 11)
            {
                // 10 ile 11 arasýnda yapýlacak iþlem
                Debug.Log(allCandySO[i].twoMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));

                isMatched = true;

                moneyMng.AddMoney(allCandySO[i].twoMatch * value, allCandySO[i].tag);
            }
            else if (value >= 12)
            {
                // 12 ve yukarýsý için yapýlacak iþlem
                Debug.Log(allCandySO[i].threeMatch * value);

                //destroyCandy(allCandySO[i].tag);
                StartCoroutine(DestroyCandyCO(allCandySO[i].tag));

                isMatched = true;

                moneyMng.AddMoney(allCandySO[i].threeMatch * value, allCandySO[i].tag);
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
                        allCandies[i, j].transform.DORotate(new Vector3(0, 0, 270), 2);
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

        //canPlay = true;
        Debug.Log("Refill!");



    }



    IEnumerator ResetMap()
    {
        for (int j = 0; j < height; j++)
        {
            yield return new WaitForSeconds(0.01f);

            for (int i = 0; i < width; i++)
            {
                yield return new WaitForSeconds(0.01f);
                if (allCandies[i, j] != null)
                {
                    allCandies[i, j].GetComponent<Candy>().targetY = j - 10;
                }

            }
        }

        yield return new WaitForSeconds(0.5f);

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
        canPlay = true;
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

    public void changeGameSpeed()
    {
        if(gameSpeedToggle.isOn)
        {
            gameSpeed = 2;
        }
        else
        {
            gameSpeed = 1;
        }
    }



    void OnGUI()
    {
        // GUI stilini belirle
        GUIStyle stil = new GUIStyle();
        stil.fontSize = 20; // Yazý boyutu
        stil.normal.textColor = Color.black; // Yazý rengi

        // Ekranýn sol üst köþesine metni yazdýr
        GUI.Label(new Rect(10, 10, 500, 30), testInfoString, stil);
    }
}
