using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item", order = 0)]
[Serializable]
public class Item : ScriptableObject 
{
    public string itemName;
    public Sprite itemImage;
    //public int itemHeld;
    [TextArea]
    public string itemInfo;

    //礼物 战斗用道具分类
    public Boolean gift;
    public Boolean currency;

    //
    public int price;
}
