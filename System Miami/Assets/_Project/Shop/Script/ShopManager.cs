using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinUI;
    public ShopItemSO[] Items;
    public GameObject[] shopPanelGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;

    void Start()
    {
        for (int i = 0; i < Items.Length; i++)
            shopPanelGO[i].SetActive(true);
        coinUI.text = "Coins: " + coins.ToString();
        LoadPanels();
        CheckPurchaseable();
    }

    void Update()
    {
        
    }

    public void AddCoins()
    {
        coins++;
        coinUI.text = "Coins: " + coins.ToString();
        CheckPurchaseable();
    }

    public void CheckPurchaseable()
    {
        for(int i = 0; i < Items.Length;i++)
        {
            if (coins >= Items[i].baseCost)
                myPurchaseBtns[i].interactable = true;
            else
                myPurchaseBtns[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= Items[btnNo].baseCost)
        {
            coins = coins - Items[btnNo].baseCost;
            coinUI.text = "Coins: " + coins.ToString();
            CheckPurchaseable();
        }
    }

    public void LoadPanels()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            shopPanels[i].titleTxt.text = Items[i].title;
            shopPanels[i].descriptionTxt.text = Items[i].description;
            shopPanels[i].costTxt.text = "Coins: " + Items[i].baseCost.ToString();
        }
    }

}
