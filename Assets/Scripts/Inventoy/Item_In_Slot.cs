using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_In_Slot : MonoBehaviour
{
    public Image SpriteImage;
    public Text num;
    public Item item_here;

    public void Set_Item(Item prop)
    {
        item_here = prop;

        if (prop == null)
        {
            SpriteImage.overrideSprite = null;
            num.text = "";
            this.gameObject.SetActive(false);
        }
        else
        {
            SpriteImage.overrideSprite = prop.itemImage;
            num.text = prop.itemheld.ToString();
            this.gameObject.SetActive(true);
        }
    }
}
