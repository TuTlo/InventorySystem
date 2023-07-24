using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public delegate void ButtonDelegate();

    public event ButtonDelegate ClickButton;

    public static Transform selectedSlot;
    public static Transform selectedItem;


    void Start()
    {
        GameObject btObj = GameObject.Find(name); //找到游戏对象的名字
        Button bt1 = btObj.GetComponent<Button>(); //找到该按钮
        bt1.onClick.AddListener(delegate() {this.ButtonClick(btObj); });
        //Debug.Log("bt1: " + bt1.GetInstanceID());
    }

    public void ButtonClick(GameObject go)
    {
        if (ClickButton != null)
        {
            ClickButton();
            //obtain clicked slot
            selectedSlot = go.GetComponent<Button>().transform.parent;
            // Debug.Log("listener: " + selectedSlot.GetInstanceID());
            selectedItem = go.GetComponent<Button>().transform;
        }
    }


}
