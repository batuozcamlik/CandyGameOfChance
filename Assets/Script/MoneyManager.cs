using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public Board board;

    public float currentMoney;
    public float currentBet;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI betText;

    public GameObject moneyAddText;
    void Start()
    {
        board=FindAnyObjectByType<Board>();
        UpdateText();
    }

    public void Play()
    {
        if((currentMoney- currentBet) >=0)
        {
            board.Play();
            currentMoney -= currentBet;
            UpdateText();
        }

    }

    public void AddMoney(float money,string tag)
    {
        currentMoney += money*currentBet;
        Debug.LogWarning(money + "TL Eklendi");


        GameObject moneyText= Instantiate(moneyAddText, GetCenterPositionByTag(tag), Quaternion.identity);
        moneyText.transform.DOMoveY(moneyText.transform.position.y + 2, 2f).OnComplete(()=>Destroy(moneyText));

        moneyText.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "+"+(money * currentBet).ToString();
        UpdateText();


    }

    public void changeBet(int a)
    {
        if(a>0)
        {
            if (currentBet + a <= 100)
            {
                currentBet += a;
            }
        }
        else
        {
            if (currentBet - a > 0)
            {
                currentBet += a;
            }
        }

        UpdateText();
    }

    public Vector2 GetCenterPositionByTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        if (objectsWithTag.Length == 0)
        {
            Debug.LogWarning("Tag ile eþleþen obje bulunamadý.");
            return Vector2.zero;
        }

        Vector2 totalPosition = Vector2.zero;
        foreach (GameObject obj in objectsWithTag)
        {
            totalPosition += (Vector2)obj.transform.position;
        }

        Vector2 centerPosition = totalPosition / objectsWithTag.Length;
        return centerPosition;
    }

    public void UpdateText()
    {
        betText.text = currentBet.ToString()+"TL";
        moneyText.text = currentMoney.ToString() + "TL";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
