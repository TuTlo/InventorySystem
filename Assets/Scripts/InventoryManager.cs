using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour
{
    /// <summary>
    ///  show and display Inventory, shop UI
    /// </summary>
    public static InventoryManager instance;

    /// <summary>
    /// SerializableMap for Inventory Map
    /// </summary>
    [Serializable]
    public class StringIntMap : SerializableMap<string, int>
    {
        public StringIntMap() : base() { }
        public StringIntMap(StringIntMap map) : base(map) { }
    }

    /// <summary>
    /// local GameState save to each slot
    /// </summary>
    [Serializable]
    public class GameState
    {
        public StringIntMap PlayerBag = new StringIntMap();
        public StringIntMap PlayerWallet = new StringIntMap();
        public StringIntMap FightBag = new StringIntMap();
    }



    public static StringIntMap playerBag;
    public static StringIntMap playerWallet;
    public static StringIntMap fightBag;

    public Inventory AllItemInventory;
    public GiftManager GiftPanel;


    private static List<Item> allItemList;
    private static Dictionary<string, Item> allItemDict = new Dictionary<string, Item>();
    public GameObject slotGrid;

    // UI 
    // Pagination
    public Text currentPage;
    public Text totalPage;
    private static int page = 1; // current page integer always starts from first page
    private static int lastPage;

    public Text itemInfo;

    public Text CoinNum;
    public Text CrystalNum;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("singleton destroy...");
        }

        instance = this;

        playerBag = new StringIntMap();
        playerWallet = new StringIntMap();
        fightBag = new StringIntMap();
        playerWallet.Add("灵币", 0);
        playerWallet.Add("魂晶", 0);


        lastPage = 4; // lastPage = (int) Math.Ceiling((double)itemDict.Count / 9);
        totalPage.text = lastPage.ToString();

        //Register page button
        GameObject.Find("previous").GetComponent<ButtonListener>().ClickButton += OnPrePageButton;
        GameObject.Find("next").GetComponent<ButtonListener>().ClickButton += OnNextPageButton;

        // Get ALl Item

        allItemList = Instantiate(instance.AllItemInventory).itemList;
        foreach (Item item in allItemList)
        {
            allItemDict.Add(item.itemName, item);

        }

        // Register slot button        
        for (int i = 0; i < 10; i++)
        {
            Transform clickSlot = slotGrid.transform.GetChild(i);
            clickSlot.GetComponent<ButtonListener>().ClickButton += onSlotClick;
        }


    }

    /// <summary>
    /// 添加道具到背包
    /// </summary>
    /// <param name="addItemName"></param>
    /// <param name="num"></param>
    public static void addNewItem(string addItemName, int num)
    {
        if (allItemDict.ContainsKey(addItemName))
        {
            if (allItemDict[addItemName].gift)
            {
                if (playerBag.ContainsKey(addItemName))
                {
                    playerBag[addItemName] += num;
                    Debug.Log(addItemName + " + " + num);
                }
                else
                {
                    playerBag.Add(addItemName, num);
                    Debug.Log("new item: " + addItemName + " + " + num);
                }
            }
            else if (allItemDict[addItemName].currency)
            {
                if (playerWallet.ContainsKey(addItemName))
                {
                    playerWallet[addItemName] += num;
                    Debug.Log(addItemName + " + " + num);
                }
                else
                {
                    playerWallet.Add(addItemName, num);
                    Debug.Log("new item: " + addItemName + " + " + num);
                }
            }
            else
            {
                if (fightBag.ContainsKey(addItemName))
                {
                    fightBag[addItemName] += num;
                    Debug.Log(addItemName + " + " + num);
                }
                else
                {
                    fightBag.Add(addItemName, num);
                    Debug.Log("new item: " + addItemName + " + " + num);

                }
            }
            instance.Display(playerBag);
        }
        else
        {
            Debug.Log(addItemName + "不存在该物品");
        }

    }

    /// <summary>
    /// 使用道具，数量减少/从背包中移除
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="num"></param>
    public static void removeItem(string removeItemName, int num)
    {
        try
        {
            //普通背包
            if (playerBag.ContainsKey(removeItemName))
            {
                playerBag[removeItemName] -= num;

                if (playerBag[removeItemName] <= 0)
                {
                    playerBag.Remove(removeItemName);
                }
                instance.Display(playerBag);
                return;
            }

            //战斗背包 全局
            if (fightBag.ContainsKey(removeItemName))
            {
                fightBag[removeItemName] -= num;

                if (fightBag[removeItemName] <= 0)
                {
                    fightBag.Remove(removeItemName);
                }
                instance.Display(playerBag);
                return;
            }

            if (playerWallet.ContainsKey(removeItemName))
            {
                //钱包不需要移除，一直都显示为0
                playerWallet[removeItemName] -= num;
                instance.Display(playerBag);
                return;
            }
        }
        catch
        {
            Debug.Log("不存在： " + removeItemName);
        }
    }


    public void Display(StringIntMap showBag)
    {
        ShowWallet();

        for (int i = 0; i < 10; i++)
        {
            int itemIndex = i + 9 * (page - 1);

            if (page > 1)
            {
                itemIndex += page - 1;
            }

            if (itemIndex < showBag.Count)
            {
                Slot thisSlot = this.slotGrid.transform.GetChild(i).GetComponent<Slot>();

                Item showItem = allItemDict[showBag.ElementAt(itemIndex).Key];

                thisSlot.slotName.text = showItem.itemName;
                thisSlot.slotImage.sprite = showItem.itemImage;
                thisSlot.slotImage.color = new Color(255, 255, 255, 255);
                thisSlot.slotNum.text = showBag.ElementAt(itemIndex).Value.ToString();
                thisSlot.slotInfo.text = showItem.itemInfo;

            }
            else
            {
                Slot thisSlot = this.slotGrid.transform.GetChild(i).GetComponent<Slot>();
                thisSlot.slotName.text = "";
                thisSlot.slotImage.sprite = null;
                thisSlot.slotImage.color = new Color(255, 255, 255, 0);
                thisSlot.slotNum.text = "";
                thisSlot.slotInfo.text = "";
            }
        }
    }


    public void Display(StringIntMap showBag, GameObject slotGrid, int gridCount)
    {
        ShowWallet();

        for (int i = 0; i < gridCount; i++)
        {
            int itemIndex = i + 9 * (page - 1);

            if (page > 1)
            {
                itemIndex += page - 1;
            }

            if (itemIndex < showBag.Count)
            {
                Slot thisSlot = slotGrid.transform.GetChild(i).GetComponent<Slot>();

                Item showItem = allItemDict[showBag.ElementAt(itemIndex).Key];

                thisSlot.slotName.text = showItem.itemName;
                thisSlot.slotImage.sprite = showItem.itemImage;
                thisSlot.slotImage.color = new Color(255, 255, 255, 255);
                thisSlot.slotNum.text = showBag.ElementAt(itemIndex).Value.ToString();
                thisSlot.slotInfo.text = showItem.itemInfo;

            }
            else
            {
                Slot thisSlot = slotGrid.transform.GetChild(i).GetComponent<Slot>();
                thisSlot.slotName.text = "";
                thisSlot.slotImage.sprite = null;
                thisSlot.slotImage.color = new Color(255, 255, 255, 0);
                thisSlot.slotNum.text = "";
            }
        }
    }

    public void ShowWallet()
    {
        try
        {
            if (playerWallet.ContainsKey("灵币") || playerWallet.ContainsKey("魂晶"))
            {
                CoinNum.text = playerWallet["灵币"].ToString();
                CrystalNum.text = playerWallet["魂晶"].ToString();
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to load: {e.Message}");
        }


    }

    public static int GetEarn()
    {
        return playerWallet["灵币"];
    }


    private void onSlotClick()
    {
        Invoke("showItem", 0.02f);
    }

    private void showItem()
    {
        GiftPanel.Display();
    }

    /// <summary>
    /// Pagination function: could be refactor to a seperate script later
    /// </summary>
    private void Refresh(int p)
    {
        // 控制页数 
        // 最后一页下一页到第一页 第一页上一页就到最后一夜
        page += p;

        if (page < 1)
        {
            page = 4;
        }
        else if (page > 4)
        {
            page = 1;
        }
        // 更新UI
        currentPage.text = page.ToString();
    }
    private void OnNextPageButton()
    {
        Refresh(1);
        Display(playerBag);
    }

    private void OnPrePageButton()
    {
        Refresh(-1);
        Display(playerBag);
    }



}
