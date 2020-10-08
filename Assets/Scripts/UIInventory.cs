using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UIInternal;
using UnityEngine;
using UnityEngine.UI;
/*
public class UIInventory : MonoBehaviour
{
// 当用户选择物品并且关闭 UI 的时候, 调用 Callback
public void ShowInventory(System.Action<Item> callback)
{

}
}
*/


public class UIInventory : MonoBehaviour
{
    InventoryManager inventoryManager;
    public SelectMission selectMission;
    public SystemManager systemManager;

    public GameObject PanelInventory;
    public GameObject BoxGrid;

    public GameObject ActiveLowerPanel;
    public GameObject ActiveUpperPanel;

    public GameObject TextTitle;
    public GameObject TextDesc;
    public bool AccepKeyboard = false;

    ExtendedGameObject2D extActiveLowerPanel;
    ExtendedGameObject2D extActiveUpperPanel;

    public AnimationCurve BounceCurve;
    public float BounceScale = 2.2f;
    public float BounceTime = 1.2f;
    float currenTime = 0.0f;

    public Item theItemInHand = null;

    System.Action doUpdate = ()=> { };

    public InventoryMiniPanel miniInventory;

    System.Action<Item> onSelectCallback;

    // int lastActivateSlot = 0;

    public int DeltaX = 139;
    public int DeltaY = 139;
    public int NumWidth = 5;
    public int NumHeight = 4;
    public Vector2Int Current = new Vector2Int(0, 0);
    SelectBox[] selectBox = new SelectBox[4];
    float stickTime = 0.0f;
    bool allowStick = true;

    [System.Serializable]
    public class TitleSelection
    {
        public GameObject TextTitle;
        public GameObject IconKey;
        TextMeshProUGUI tmpTitle;
        Image imIcon;
        Color color;
        Color textColor;


        public void Init()
        {
            imIcon = IconKey.GetComponent<Image>();
            tmpTitle = TextTitle.GetComponent<TextMeshProUGUI>();
            color = imIcon.color;
            color.a = 0.5f;
            imIcon.color = color;
            textColor = tmpTitle.color;
        }

        public void Activate()
        {
            IconKey.SetActive(false);
            textColor.a = 1;
            tmpTitle.color = textColor;
        }

        public void Deactivate()
        {
            IconKey.SetActive(true);
            textColor.a = 0.5f;
            tmpTitle.color = textColor;
        }
    }

    public TitleSelection TitleSystem;
    public TitleSelection TitleBag;

    public GameObject SubsectionBag;
    public GameObject SubsectionSystem;


    Vector3 GetDelatForPanel(Vector2 dir)
    {
        return new Vector3(
            DeltaX * dir.x,
            DeltaY * dir.y,
            0
            );
    }

    int LinearAddr(Vector2Int v)
    {
        return v.y * NumWidth + v.x;
    }

    void ClampAddr()
    {
        Current.x = Mathf.Clamp(Current.x, 0, NumWidth - 1);
        Current.y = Mathf.Clamp(Current.y, 0, NumHeight - 1);
    }


    public class Slot
    {
        public Item item { set { SetItem(value); } get { return theItem; } }
        public GameObject target;
        GameObject SpriteImage;
        Image image;
        GameObject glowing;
        // TODO 显示数字
        GameObject number;
        GameObject Title;
        GameObject Desc;
        Text textLegacyNumber;

        Image DefaultBackground;

        Item theItem;

        TextMeshProUGUI textTitle;
        TextMeshProUGUI textDesc;

        public void Init(GameObject _target, GameObject _Title, GameObject _Desc)
        {
            target = _target;
            SpriteImage = target.transform.GetChild(1).gameObject;
            image = SpriteImage.GetComponent<Image>();
            glowing = target.transform.GetChild(0).gameObject;
            number = target.transform.GetChild(2).gameObject;
            DefaultBackground = target.GetComponent<Image>();
            textLegacyNumber = SpriteImage.transform.GetChild(0).gameObject.GetComponent<Text>();

            Title = _Title;
            Desc = _Desc;

            textTitle = Title.GetComponent<TextMeshProUGUI>();
            textDesc = Desc.GetComponent<TextMeshProUGUI>();
        }

        void SetItem(Item _item)
        {
            theItem = _item;
            var color = image.color;
            if (_item && _item.itemheld != 0)
            {
                image.sprite = _item.itemImage;
                color.a = 0.95f;
                SpriteImage.SetActive(true);
                textLegacyNumber.text = theItem.itemheld.ToString();
            }
            else
            {
                image.sprite = null;
                color.a = 0.5f;
                SpriteImage.SetActive(false);
            }
            DefaultBackground.color = color;
        }

        public void Activate()
        {
            glowing.SetActive(true);
            if (theItem)
            {
                textTitle.text = theItem.itemName;
                textDesc.text = theItem.ItemInfo;

            }
            else
            {
                textTitle.text = "";
                textDesc.text = "";
            }
        }

        public void Deactivate()
        {
            glowing.SetActive(false);

        }
    }

    List<Slot> slots = new List<Slot>();
    public Inventory inventory;


    bool isShow = false;



    void ReRender()
    {
        int i = 0;
        for (int k = 0; k < inventory.itemlist.Count; ++k)
        {

            Item item = inventory.itemlist[k];
            if (item == null)
            {
                continue;
            }
            if (item.itemheld == 0)
            {
                continue;
            }
            if (item.isEmptyHand)
            {
                continue;
            }
            slots[i].item = item;
            ++i;
        }

        for (; i < NumWidth * NumHeight; ++i)
        {
            slots[i].item = null;
        }
    }

    bool hasBeenInit = false;
    public void Init()
    {
        if (hasBeenInit)
        {
            return;
        }
        hasBeenInit = true;

        extActiveLowerPanel = new ExtendedGameObject2D(ActiveLowerPanel);
        extActiveUpperPanel = new ExtendedGameObject2D(ActiveUpperPanel);

        for (int i = 0; i < BoxGrid.transform.childCount; ++i)
        {
            var slot = new Slot();
            slot.Init(BoxGrid.transform.GetChild(i).gameObject, TextTitle, TextDesc);
            slots.Add(slot);
        }

        System.Action<int, Vector3> setUp =
        (id, dir) =>
        {
            selectBox[id] = new SelectBox(
                ActiveUpperPanel.transform.GetChild(id).gameObject,
                ActiveLowerPanel.transform.GetChild(id).gameObject,
                dir
                );
        };

        SelectBox.scale = BounceScale;
        SelectBox.curve = BounceCurve;

        setUp(0, new Vector3(-1, -1, 0));
        setUp(1, new Vector3(1, -1, 0));
        setUp(2, new Vector3(1, 1, 0));
        setUp(3, new Vector3(-1, 1, 0));

        currenTime = 0;

        miniInventory.Init(inventory, this);
        TitleBag.Init();
        TitleSystem.Init();


        PanelInventory.SetActive(false);

        selectMission.Init();
        systemManager.Init(this);
    }

    private void Start()
    {
        Init();
    }

    void SwitchToLeft()
    {
        TitleBag.Activate();
        TitleSystem.Deactivate();
        SubsectionBag.SetActive(true);
        SubsectionSystem.SetActive(false);
    }

    void SwitchToRight()
    {
        TitleBag.Deactivate();
        TitleSystem.Activate();
        SubsectionBag.SetActive(false);
        SubsectionSystem.SetActive(true);
    }

    // 显示 Inventory UI 界面,
    // 注意 这时候 Inventory 负责输入, 调用者应该不处理任何按键
    // 直到 _onSelectCallback 回调的时候才能恢复处理按键
    // _onSelectCallback 回调的参数是用户选中的 Item, Item 可能为 null
    public void ShowInventory(Item itemInHand, System.Action<Item> _onSelectCallback, bool mini = false)
    {
        if(mini == true)
        {
            ShowMiniInventory(itemInHand, _onSelectCallback);
            return;
        }

        Time.timeScale = 0f;
        SwitchToLeft();

        theItemInHand = itemInHand;

        onSelectCallback = (item) =>
        {
            Time.timeScale = 1;

            doUpdate = () => { };
            JoyconInfo.Blind(ManagedInput.Viewer.Default, false);
            if (item != null)
            {
                item.itemheld -= 1;
            }
            _onSelectCallback(item);
        };

        if (itemInHand != null)
        {
            itemInHand.itemheld += 1;
        }

        currenTime = 0;
        stickTime = 0;
        isShow = true;
        allowStick = true;
        PanelInventory.SetActive(true);
        ReRender();
        slots[LinearAddr(Current)].Activate();

        JoyconInfo.Blind(ManagedInput.Viewer.Default, true);
        doUpdate = UpdateLargeInventory;
    }

    public void ShowMiniInventory(Item itemInHand, System.Action<Item> _onSelectCallback)
    {
        

        miniInventory.Show(itemInHand, (item)=>
        {
            doUpdate = () => { };
            _onSelectCallback(item);
        });

        doUpdate = () =>
        {
            miniInventory.Update();
        };
    }

    public void RemoveItem(Item item)
    {
        if (item.itemheld > 0)
        {
            item.itemheld -= 1;
        }
        // inventoryManager.Drop_Item(item, false);
        UpdateItemChange(item);
    }

    void UpdateItemChange(Item item)
    {
        foreach (var i in slots)
        {
            if (i.item == item)
            {
                i.item = item;
                break;
            }
        }
    }

    public void AddItem(Item item)
    {
        if (item == null)
        {
            return;
        }
        if (inventory.itemlist.Contains(item))
        {
            item.itemheld += 1;
        }
        else
        {
            inventory.itemlist.Add(item);
        }
        // InventoryManager._instance.Get_Another_Item(item);
        UpdateItemChange(item);
    }

    bool CheckDirectionOne(Vector2 stick, out KeyCode key)
    {
        key = KeyCode.A;

        var ldotY = Vector2.Dot(stick, new Vector2(0, 1));
        var ldotX = Vector2.Dot(stick, new Vector2(1, 0));

        var absldotY = Mathf.Abs(ldotY);
        var absldotX = Mathf.Abs(ldotX);

        if (absldotX < UIManager.Settings.inventoryStickSensitivity
            && absldotY < UIManager.Settings.inventoryStickSensitivity)
        {
            return false;
        }

        if (Math.Abs(absldotX - absldotY) < UIManager.Settings.inventoryStickSensitivity * 0.3f)
        {
            return false;
        }

        if (absldotY > absldotX)
        {
            if (ldotY > 0)
            {
                key = KeyCode.W;
            }
            else
            {
                key = KeyCode.S;
            }
        }
        else
        {
            if (ldotX > 0)
            {
                key = KeyCode.D;
            }
            else
            {
                key = KeyCode.A;
            }
        }
        return true;
    }

    bool GetCurrentDirection(out KeyCode key)
    {
        if (CheckDirectionOne(JoyconInfo.rightStick, out key))
        {
            return true;
        }

        if (CheckDirectionOne(JoyconInfo.leftStick, out key))
        {
            return true;
        }

        return false;
    }

    public void ShowSelectMission(System.Action<int> callback)
    {
        selectMission.Show((index) =>
        {
            selectMission.Hide();
            doUpdate = () => { };
            callback(index);
        });

        doUpdate = selectMission.Update;
    }

    public void TestShowSelectMission()
    {
        selectMission.Show((x)=> { });
        doUpdate = () =>
        {
            selectMission.Update();
        };
    }

    public void TestShowSelectMissionHide()
    {
        selectMission.Hide();
        doUpdate = () =>
        {
            
        };
    }


    public void HandleInput(KeyCode code)
    {
        Vector2Int dir = new Vector2Int(0, 0);
        switch (code)
        {
            case KeyCode.A:
                dir.x = -1;
                break;
            case KeyCode.W:
                dir.y = -1;
                break;
            case KeyCode.S:
                dir.y = 1;
                break;
            case KeyCode.D:
                dir.x = 1;
                break;
        }
        var lastCurrent = Current;

        Current += dir;
        ClampAddr();

        if (lastCurrent == Current)
        {
            return;
        }

        slots[LinearAddr(lastCurrent)].Deactivate();
        slots[LinearAddr(Current)].Activate();

        var dpanel = Current;
        dpanel.y *= -1;
        extActiveLowerPanel
            .LocalTranslateFromStartTransition(
                GetDelatForPanel(dpanel), 0.1f
            );
        extActiveUpperPanel
            .LocalTranslateFromStartTransition(
                GetDelatForPanel(dpanel), 0.1f
            );
    }

    void UpdateSystemSection()
    {
        systemManager.Update();
        if (systemManager.InputBubbleUp && JoyconInfo.joyconL.GetButtonDown(Joycon.Button.SHOULDER_2, ManagedInput.Viewer.UI))
        {
            SwitchToLeft();

            doUpdate = UpdateLargeInventory;
            doUpdate();
            return;
        }
        
    }

    void UpdateLargeInventory()
    {
        if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.SHOULDER_2, ManagedInput.Viewer.UI))
        {
            SwitchToRight();

            systemManager.Show(() =>
            {
                //Time.timeScale = 1;
                //doUpdate = () => { };
                PanelInventory.SetActive(false);
                onSelectCallback(null);
            });

            currenTime = 0;
            allowStick = true;

            doUpdate = UpdateSystemSection;
            doUpdate();
            return;
        }

        currenTime += Time.unscaledDeltaTime;
        if (currenTime > BounceTime)
        {
            currenTime -= BounceTime;
        }
        foreach (var select in selectBox)
        {
            select.Update(currenTime / BounceTime);
        }

        bool doSelectItem = false;

        using (var state = UIManager.EnterUIState())
        {
            //if (!JoyconInfo.rightConnected)
            //{
            //    if (AccepKeyboard == true)
            //    {
            //        bool handled = false;
            //        if (Input.GetKeyDown(KeyCode.W))
            //        {
            //            HandleInput(KeyCode.W);
            //        }
            //        if (Input.GetKeyDown(KeyCode.A))
            //        {
            //            HandleInput(KeyCode.A);
            //        }
            //        if (Input.GetKeyDown(KeyCode.S))
            //        {
            //            HandleInput(KeyCode.S);
            //        }
            //        if (Input.GetKeyDown(KeyCode.D))
            //        {
            //            HandleInput(KeyCode.D);
            //        }
            //
            //        if (Input.GetKeyDown(KeyCode.Return))
            //        {
            //            doSelectItem = true;
            //        }
            //
            //        if (handled)
            //        {
            //            stickTime = 0;
            //            allowStick = false;
            //        }
            //    }
            //}

            if (doSelectItem || JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
            {
                Item aitem = slots[LinearAddr(Current)].item;
                onSelectCallback(aitem);
                PanelInventory.SetActive(false);
                isShow = false;

                return;
            }

            if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI)
                // || JoyconInfo.joyconR.GetButtonDown(Joycon.Button.PLUS, ManagedInput.Viewer.UI)
                )
            {
                onSelectCallback(theItemInHand);
                PanelInventory.SetActive(false);
                isShow = false;
            }

            stickTime += Time.unscaledDeltaTime;
            if (stickTime > UIManager.Settings.startMenuSelectInterval)
            {
                allowStick = true;
                stickTime = 0;
            }

            if (allowStick && GetCurrentDirection(out KeyCode key))
            {
                HandleInput(key);
                stickTime = 0;
                allowStick = false;
                // Debug.Log("Handling Stick");
            }
        }
    }

    private void LateUpdate()
    {
        inventoryManager = InventoryManager._instance;

        doUpdate();
        if (!isShow)
        {
            return;
        }

        

        
    }
}