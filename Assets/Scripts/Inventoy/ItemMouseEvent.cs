using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMouseEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver;

    public Inventory my_inventory;

    private int Origin_SlotID;// 当前的格子ID
    public Transform originParent;

    private void Awake()
    {
        isOver = false;
    }
    /*
    private void Update()
    {
        if(isOver)
        {
            Item_In_Slot Item_Sprite = this.gameObject.GetComponent<Item_In_Slot>();
            InventoryManager.Show_Description(Item_Sprite.item_here);
            if (Input.GetMouseButtonDown(1))// 如果这个时候右键被戳
            {
                Debug.Log("右键被戳！");
                Item_In_Slot Item_sprite = this.gameObject.GetComponent<Item_In_Slot>();
                Item used_Item = Item_sprite.item_here;
                InventoryManager._instance.Take_On_Item(used_Item);
            }
        }
    }*/

    public void OnBeginDrag(PointerEventData eventData)
    {
        originParent = this.transform.parent;
        Origin_SlotID = originParent.GetComponent<Slot>().Slot_index;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.position = eventData.position;
        transform.SetParent(this.transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if(target != null)
        {
            if (target.CompareTag(Tags.Inventory_Sprite))
            {
                Item tempItem = my_inventory.itemlist[Origin_SlotID];
                int Target_ID = target.transform.parent.GetComponent<Slot>().Slot_index;
                my_inventory.itemlist[Origin_SlotID] = my_inventory.itemlist[Target_ID];
                my_inventory.itemlist[Target_ID] = tempItem;


                transform.SetParent(target.transform.parent);
                target.transform.SetParent(originParent);
                target.transform.localPosition = Vector3.zero;
            }
            else if (target.CompareTag(Tags.Inventory_Grid))
            {
                if(target.GetComponent<Slot>().Slot_index != Origin_SlotID)
                {
                    int Target_ID = target.GetComponent<Slot>().Slot_index;
                    my_inventory.itemlist[Target_ID] = my_inventory.itemlist[Origin_SlotID];
                    my_inventory.itemlist[Origin_SlotID] = null;

                    Transform emptySprite = target.transform.GetChild(0);
                    transform.SetParent(target.transform);
                    emptySprite.SetParent(originParent.transform);
                }
                else
                {
                    transform.SetParent(originParent.transform);
                }
            }
            else
            {
                transform.SetParent(originParent.transform);
            }
        }
        else
        {
            transform.SetParent(originParent.transform);
        }

        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Reset_Position();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

    public void Reset_Position()
    {
        this.transform.localPosition = Vector3.zero;
    }
}
