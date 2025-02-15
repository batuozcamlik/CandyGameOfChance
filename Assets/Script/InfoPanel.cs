using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public CandyScribleObject[] allCandySO;
    public GameObject[] allInfo;

    public GameObject infoUIObject;
    public MoneyManager moneyMng;
    void Start()
    {
        closeUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            openUI();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            closeUI();
        }
    }

    public void openUI()
    {
        infoUIObject.SetActive(true);
        UpdateUI();
    }

    public void closeUI()
    {
        infoUIObject.SetActive(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < allInfo.Length; i++)
        {
            allInfo[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = allCandySO[i].candySprite;

            allInfo[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "12+ " + (allCandySO[i].threeMatch*moneyMng.currentBet).ToString();

            allInfo[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "10-11 "+ (allCandySO[i].twoMatch * moneyMng.currentBet).ToString();

            allInfo[i].transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "8-9 "+ (allCandySO[i].oneMatch * moneyMng.currentBet).ToString();

        }
    }
}
