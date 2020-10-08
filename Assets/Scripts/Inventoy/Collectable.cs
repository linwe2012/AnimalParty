using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Item iteminfo;
    /*
    public void Set_Info(1Item iteminfo)
    {
        this.GetComponent<MeshFilter>().mesh = iteminfo.item;
        this.iteminfo = iteminfo;
    }

    public void Activate()
    {
        InventoryManager.Get_Another_Item(iteminfo);
        Debug.Log(iteminfo.itemheld);
        Destroy(this.gameObject);
    }*/
}
