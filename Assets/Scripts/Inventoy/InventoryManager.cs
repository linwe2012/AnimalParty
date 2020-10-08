using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager _instance;

    public Inventory my_inventory; // 背包
    public GameObject Grid; // 之前背包的格子部分
    public Slot Slot_Prefab;// 格子的预设
    public Text Description_Text;// 描述内容的文字
    //public static float timer = 0.1f;
    public static int pointer_index = 0;// 代表背包此时指定的
    public Sprite Chosen_Grid_Sprite;
    public Sprite Normal_Grid_Sprite;

    public List<Slot> slots = new List<Slot>();

    public const int Bag_Capacity = 20;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        _instance = this;
    }

    private void OnEnable()
    {
        Refresh_Bag();
    }

    /**
     * 返回当前选择位置的道具，如果没有则返回null
     * */
    public Item Get_Item_From_Inventory()
    {
        return slots[pointer_index].Get_Item_inside();
    }


    public void HandleInput(KeyCode code)
    {
        if (code == KeyCode.A)
        {
            pointer_index -= 1;
            if (pointer_index < 0)
                pointer_index = Bag_Capacity - 1;
            Refresh_Bag();
        }
        if (code == KeyCode.D)
        {
            pointer_index += 1;
            if (pointer_index >= Bag_Capacity)
                pointer_index = 0;
            Refresh_Bag();
        }
        if (code == KeyCode.W)
        {
            pointer_index -= 5;
            if (pointer_index < 0)
                pointer_index += Bag_Capacity;
            Refresh_Bag();
        }
        if (code == KeyCode.S)
        {
            pointer_index += 5;
            if (pointer_index >= Bag_Capacity)
                pointer_index -= Bag_Capacity;
            Refresh_Bag();
        }
    }


    /**
     * 往背包中添加一样新的道具 
     * */
    public void Get_Another_Item(Item item)
    {
        if (_instance.my_inventory.itemlist.Contains(item))
        {
            item.itemheld += 1;
        }
        else
        {
            int i;
            for (i = 0; i < _instance.my_inventory.itemlist.Count; i++)
            {
                if (_instance.my_inventory.itemlist[i] == null)
                {
                    _instance.my_inventory.itemlist[i] = item;
                    item.itemheld += 1;
                    break;
                }
            }
            if (i >= _instance.my_inventory.itemlist.Count)
            {
                Debug.Log("包裹装不下了");
            }
            /*Slot newItem = Instantiate(_instance.Slot_Prefab, _instance.Grid.transform);
            _instance.my_inventory.itemlist.Add(item);
            newItem.initial(item);*/
        }

        // Refresh_Bag();
    }

    /**
     * 从背包中移去某样道具（这里只有从背包中删除的功能不会创造实体）
     * item表示要移去的道具的内容
     * Whole表示是否要全部丢弃
     * */
     public void Drop_Item(Item item, bool Whole)
     {
        if (Whole)
        {
            item.itemheld = 0;
        }
        else
        {
            item.itemheld -= 1;
            if (item.itemheld < 0)
                item.itemheld = 0;
        }
        Refresh_Bag();
    }

    /**
     * 清除背包所有内容(仅仅是逻辑上,不会穿凿实体)
     * */
     public void Clear_Item()
    {
        for(int i = 0; i < my_inventory.itemlist.Count; i++)
        {
            if(my_inventory.itemlist[i] != null)
            {
                my_inventory.itemlist[i].itemheld = 0;
            }
        }
        Refresh_Bag();
    }

    /** 
     * 刷新背包的内容
     * */
    public void Refresh_Bag()
    {
        return;
        if (_instance.Grid.transform.childCount < _instance.my_inventory.itemlist.Count)
        {
            _instance.slots.Clear();
            for (int i = 0; i < _instance.Grid.transform.childCount; i++)
            {
                if (_instance.Grid.transform.childCount == 0) break;
                Destroy(_instance.Grid.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < _instance.my_inventory.itemlist.Count; i++)
            {
                Item one_item = _instance.my_inventory.itemlist[i];

                Slot newItem = Instantiate(_instance.Slot_Prefab, _instance.Grid.transform);
                if (one_item == null) newItem.initial(i, null);
                else if (one_item.itemheld == 0)
                {
                    _instance.my_inventory.itemlist.Remove(one_item);
                    newItem.initial(i, null);
                }
                else
                    newItem.initial(i, one_item);
                _instance.slots.Add(newItem);
            }
        }
        else
        {
            for (int i = 0; i < _instance.my_inventory.itemlist.Count; i++)
            {
                Item one_item = _instance.my_inventory.itemlist[i];

                if (one_item == null) _instance.slots[i].initial(i, null);
                else if (one_item.itemheld == 0)// 如果物品已经没有了就直接从列表中移除
                {
                    int index = _instance.my_inventory.itemlist.IndexOf(one_item);
                    _instance.my_inventory.itemlist[index] = null;
                    _instance.slots[i].initial(i, null);
                }
                else
                    _instance.slots[i].initial(i, one_item);

                if (i == pointer_index)
                {
                    _instance.slots[i].GetComponent<Image>().overrideSprite = Chosen_Grid_Sprite;
                    if (_instance.slots[i].Get_Item_inside() != null)
                    {
                        Description_Text.text = _instance.slots[i].Get_Item_inside().ItemInfo;
                    }
                    else
                    {
                        Description_Text.text = "";
                    }
                }
                else
                {
                    _instance.slots[i].GetComponent<Image>().overrideSprite = Normal_Grid_Sprite;
                }
            }
        }

    }
}
