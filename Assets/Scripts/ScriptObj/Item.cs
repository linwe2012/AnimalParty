using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string canonicalName;
    public Sprite itemImage;
    public int itemheld;
    [TextArea]
    public string ItemInfo;
    public GameObject initialThing;
    public bool isEmptyHand;
}
