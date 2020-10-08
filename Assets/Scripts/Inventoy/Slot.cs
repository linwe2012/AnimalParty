using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public int Slot_index;// 格子的编号
    //public Item item_in_this_slot;
    //public Image Slot_Image;
    //public Text num;

    public void initial(int index, Item item)
    {
        Slot_index = index;
        Item_In_Slot an_Item = transform.GetChild(0).GetComponent<Item_In_Slot>();
        //Item_In_Slot an_Item = gameObject.GetComponentInChildren<Item_In_Slot>();
        an_Item.Set_Item(item);
    }

    public Item Get_Item_inside()
    {
        Item_In_Slot an_Item = transform.GetChild(0).GetComponent<Item_In_Slot>();
        if (an_Item.gameObject.activeInHierarchy == false)
            return null;
        else return an_Item.item_here;
    }


}
