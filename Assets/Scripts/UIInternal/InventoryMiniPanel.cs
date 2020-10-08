using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace UIInternal
{

    class MiniSlot
    {
        public GameObject target;
        public Item item
        {
            set
            {
                UpdateItem(value);
            }
            get { return myItem; }
        }

        Item myItem;

        RectTransform rect;

        GameObject itemImageObject;
        Image itemImage;
        GameObject glowing;
        GameObject textObject;
        Text textNumber;

        float startLocalPositionX = 0;

        public bool itemValid
        {
            get { return myItem != null && myItem.itemheld != 0; }
        }

        void UpdateItem(Item item)
        {
            myItem = item;

            if(itemValid)
            {
                itemImage.sprite = item.itemImage;

                if(item.itemheld > 1)
                {
                    textObject.SetActive(true);
                    textNumber.text = item.itemheld.ToString();
                }
                else
                {
                    textObject.SetActive(false);
                }
                itemImageObject.SetActive(true);
            }
            else
            {
                textObject.SetActive(false);
                itemImageObject.SetActive(false);
            }
        }

        public float localPositionX
        {
            set
            {
                var pos = rect.localPosition;
                pos.x = value;
                rect.localPosition = pos;
                startLocalPositionX = value;
            }
        }

        public void MoveDx(float dx)
        {
            var pos = rect.localPosition;
            pos.x = startLocalPositionX + dx;
            rect.localPosition = pos;
            
        }

        public void Init(GameObject _target)
        {
            target = _target;

            glowing = target.transform.GetChild(0).gameObject;
            itemImageObject = target.transform.GetChild(1).gameObject;
            itemImage = itemImageObject.GetComponent<Image>();
            rect = target.GetComponent<RectTransform>();
            textObject = itemImageObject.transform.GetChild(0).gameObject;
            textNumber = textObject.GetComponent<Text>();
            Hide();
        }

        public void Show()
        {
            target.SetActive(true);
        }

        public void Hide()
        {
            target.SetActive(false);
        }

        public void Activate()
        {
            glowing.SetActive(true);
        }

        public void Deactivate()
        {
            glowing.SetActive(false);
        }
        
    }

    /* 
     * 这个 slot train 本意是想要用 8 个 slot 动态模拟无限个 slots
     * But 代码复杂度超出我的现象, 而且一般性超过100个 slots 的可能性不大
    class SlotTrain
    {
        
        LinkedList<MiniSlot> slots = new LinkedList<MiniSlot>();
        List<Item> items = new List<Item>();
        float targetPosX;
        float currentPosX;

        float startPosX;

        float currentTime;
        public float totalTime = 0.5f;

        int firstDislpayItem;
        int lastDisplayItem;
        int cursor = 0;

        int firstDisplaySlot;
        int lastDisplaySlot;
        // int cursorSlot;
        int slotsCenter;
        LinkedListNode<MiniSlot> lastValidSlotNode;

        public void Add(int id, GameObject parent)
        {
            var slotObject = parent.transform.GetChild(id).gameObject;
            var slot = new MiniSlot();
            slot.Init(slotObject);
            
            slots.AddLast(slot);
            slotsCenter = slots.Count / 2;
        }

        public void PrepareSlots(List<Item> items_, int selected)
        {
            items = items_;

            var center = slotsCenter;

            firstDislpayItem = Mathf.Max(selected - center, 0);
            lastDisplayItem = Mathf.Min(selected + center, items_.Count);
            var start = center - (selected - firstDislpayItem);

            int cnt = 0;
            var slotNode = slots.First;
            for (int i = firstDislpayItem; i < lastDisplayItem; ++i)
            {
                var slot = slotNode.Value;
                slot.item = items_[i];
                slot.Show();
                slot.localPositionX = (start - center + cnt) * 15;
                ++cnt;
            }

            firstDisplaySlot = start;
            lastDisplaySlot = start + cnt;
            lastValidSlotNode = slotNode;

            for (; cnt < slots.Count; ++cnt)
            {
                var slot = slotNode.Value;
                slot.Hide();
            }

            targetPosX = 0;
            currentPosX = 0;
            currentTime = totalTime;
            cursor = selected;
        }


        public void MoveLeft()
        {
            if(cursor == items.Count - 1)
            {
                return;
            }

            ++cursor;
            

            if (firstDisplaySlot == 0)
            {
                lastValidSlotNode = slots.AddAfter(lastValidSlotNode, slots.First.Value);
                slots.Remove(slots.First);
            }
            else
            {
                --firstDisplaySlot;
            }

            if(lastDisplaySlot == slots.Count - 1)
            {

            }

            currentTime = 0;
            targetPosX -= 15;
            startPosX = currentPosX;
        }

        public void MoveRight()
        {
            currentTime = 0;
            targetPosX += 15;
            startPosX = currentPosX;
        }

        public void Update(AnimationCurve curve)
        {
            if(currentTime >= totalTime)
            {
                return;
            }

            currentTime += Time.deltaTime;
            if (currentTime >= totalTime)
            {
                currentTime = totalTime;
            }
            // Debug.Log("Dt" + currentTime.ToString());
            var portion = curve.Evaluate(currentTime / totalTime);
            var pos = portion * (targetPosX - startPosX);
            currentPosX = startPosX + pos;
            foreach (var slot in slots)
            {
                slot.MoveDx(currentPosX);
            }
        }


    }
    */

    class SlotTrain
    {
        List<MiniSlot> slots = new List<MiniSlot>();
        List<GameObject> dotsInstance = new List<GameObject>();
        GameObject dotsGrid;
        GameObject dotsPrefab;
        int slotsCenter;
        GameObject slotObject;
        GameObject slotGrid;
        GameObject activeDot;
        List<Item> items;

        float currentTime;
        public float totalTime = 0.5f;

        float targetPosX;
        float currentPosX;
        float startPosX;
        public int currentSelected;
        int dotNeedsActivated = 0;
        public Item currentSelectedItem
        {
            get { return slots[currentSelected].item; }
        }

        void PopulateOneDot()
        {
            dotsInstance.Add(GameObject.Instantiate(dotsPrefab, dotsGrid.transform));
        }

        public void Init(GameObject panelInv_, GameObject dotsGrid_, GameObject dotsPrefab_)
        {
            dotsGrid = dotsGrid_;
            dotsPrefab = dotsPrefab_;

            var currentDots = dotsGrid.transform.childCount;
            for(int i = 0; i < currentDots; ++i)
            {
                dotsInstance.Add(dotsGrid.transform.GetChild(i).gameObject);
            }

            while (dotsInstance.Count < slots.Count)
            {
                PopulateOneDot();
            }

            activeDot = GameObject.Instantiate(dotsPrefab, panelInv_.transform);
            var im = activeDot.GetComponent<Image>();
            var col = im.color;
            col.a = 1;
            im.color = col;
            activeDot.transform.localScale = activeDot.transform.localScale * 0.6f;
            activeDot.SetActive(false);
        }

        public void Add(int id, GameObject parent)
        {
            slotGrid = parent;
            slotObject = parent.transform.GetChild(id).gameObject;
            var slot = new MiniSlot();
            slot.Init(slotObject);

            slots.Add(slot);
            slotsCenter = slots.Count / 2;
        }

        public void PrepareSlots(List<Item> items_, int selected)
        {
            var center = slotsCenter;
            items = items_;
            currentSelected = selected;

            var cnt = 0;
            for(int i = 0; i < slots.Count && i < items_.Count; ++i, ++cnt)
            {
                slots[cnt].item = items_[i];
                slots[cnt].Show();
                slots[cnt].Deactivate();
                dotsInstance[cnt].SetActive(true);
            }

            for(int i = cnt; i < slots.Count; ++i, ++cnt)
            {
                slots[cnt].Hide();
                dotsInstance[cnt].SetActive(false);
            }

            for(int i = cnt; i < items_.Count; ++i)
            {
                var obj = GameObject.Instantiate(slotObject, slotGrid.transform);
                
                var slot = new MiniSlot();
                slot.Init(slotObject);

                slot.item = items_[i];
                slot.Show();
                slot.Deactivate();

                slots.Add(slot);
                PopulateOneDot();
                slotsCenter = slots.Count / 2;
            }

            var b = -15 * selected;
            for(int i = 0; i < items_.Count; ++i)
            {
                slots[i].localPositionX = b + i * 15;
            }
            slots[selected].Activate();
            dotNeedsActivated = 2;

            currentTime = totalTime;
            targetPosX = 0;
            startPosX = 0;
            currentPosX = 0;
        }

        public void MoveLeft()
        {
            if (currentSelected == items.Count - 1)
            {
                return;
            }

            currentTime = 0;
            targetPosX -= 15;
            startPosX = currentPosX;

            slots[currentSelected].Deactivate();
            ++currentSelected;
            slots[currentSelected].Activate();
            BindDotToActivatePosition();
        }

        public void MoveRight()
        {
            if (currentSelected == 0)
            {
                return;
            }
            
            currentTime = 0;
            targetPosX += 15;
            startPosX = currentPosX;

            slots[currentSelected].Deactivate();
            --currentSelected;
            slots[currentSelected].Activate();
            BindDotToActivatePosition();
        }

        void BindDotToActivatePosition()
        {
            activeDot.transform.position = dotsInstance[currentSelected].transform.position;
        }

        public void Update(AnimationCurve curve)
        {
            if (dotNeedsActivated > 0)
            {
                if(dotNeedsActivated == 1)
                {
                    activeDot.SetActive(true);
                    BindDotToActivatePosition();
                    dotNeedsActivated = 0;
                }
                else
                {
                    --dotNeedsActivated;
                }
                
            }

            if (currentTime >= totalTime)
            {
                return;
            }

            currentTime += Time.unscaledDeltaTime;
            if (currentTime >= totalTime)
            {
                currentTime = totalTime;
            }
            // Debug.Log("Dt" + currentTime.ToString());
            var portion = curve.Evaluate(currentTime / totalTime);
            var pos = portion * (targetPosX - startPosX);
            currentPosX = startPosX + pos;
            foreach (var slot in slots)
            {
                slot.MoveDx(currentPosX);
            }
        }

    }



        [System.Serializable]
    public class InventoryMiniPanel
    {
        public GameObject PanelMiniInventory;
        public GameObject DotPrefab;
        public GameObject GridForDot;
        public GameObject PanelSlots;

        public GameObject ActiveLowerPanel;
        public GameObject ActiveUpperPanel;
        public Item EmptyHandItem;

        public float MoveTime = 0.2f;

        SelectBox[] selectBox = new SelectBox[4];

        ExtendedGameObject2D extActiveLowerPanel;
        ExtendedGameObject2D extActiveUpperPanel;

        UIInventory config;
        Inventory inventory;
        List<GameObject> dotInstances;

        SlotTrain train;

        int numDotsActive;
        float currenTime;
        System.Action<Item> onSelectCallback;

        Item theItemInHand;


        public void Init(Inventory inventory_, UIInventory config_)
        {
            inventory = inventory_;
            config = config_;
            dotInstances = new List<GameObject>();
            train = new SlotTrain();

            numDotsActive = 0;
            //var dot = GameObject.Instantiate(DotPrefab, GridForDot.transform);
            //dotInstances.Add(dot);
            

            extActiveLowerPanel = new ExtendedGameObject2D(ActiveLowerPanel);
            extActiveUpperPanel = new ExtendedGameObject2D(ActiveUpperPanel);


            System.Action<int> setSlot = (id) =>
            {
                train.Add(id, PanelSlots);
            };

            setSlot(0);
            setSlot(1);
            setSlot(2);
            setSlot(3);
            setSlot(4);
            setSlot(5);
            setSlot(6);
            setSlot(7);
            setSlot(8);
            setSlot(9);
            setSlot(10);


            System.Action<int, Vector3> setUp =
            (id, dir) =>
            {
                selectBox[id] = new SelectBox(
                    ActiveUpperPanel.transform.GetChild(id).gameObject,
                    ActiveLowerPanel.transform.GetChild(id).gameObject,
                    dir
                    );
            };

            SelectBox.scale = config.BounceScale;
            SelectBox.curve = config.BounceCurve;

            setUp(0, new Vector3(-1, -1, 0));
            setUp(1, new Vector3(1, -1, 0));
            setUp(2, new Vector3(1, 1, 0));
            setUp(3, new Vector3(-1, 1, 0));

            train.totalTime = MoveTime;
            train.Init(PanelMiniInventory, GridForDot, DotPrefab);

            PanelMiniInventory.SetActive(false);

            skipUpdate = true;
        }

        bool skipUpdate;

        public void Show(Item itemInHand, System.Action<Item> onSelect)
        {
            theItemInHand = itemInHand;

            currenTime = 0;

            List<Item> items = new List<Item>();

            items.Add(EmptyHandItem);

            if(itemInHand != null)
                itemInHand.itemheld += 1;

            int index = 0;
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

                if(itemInHand && item.canonicalName == itemInHand.canonicalName)
                {
                    index = items.Count;
                }
                items.Add(item);
            }
            

            onSelectCallback = (item) =>
            {
                Time.timeScale = 1;
                JoyconInfo.Blind(ManagedInput.Viewer.Default, false);
                if (item)
                {
                    if(item.isEmptyHand)
                    {
                        item.itemheld = 0;
                        item = null;
                    }
                    else
                    {
                        if(item.itemheld > 0)
                        {
                            item.itemheld -= 1;
                        }
                    }
                }
                
                onSelect(item);
                PanelMiniInventory.SetActive(false);
            };

            JoyconInfo.Blind(ManagedInput.Viewer.Default, true);
            //if(items.Count == 0)
            //{
                //onSelect(itemInHand);
                //return;
            //}
            train.PrepareSlots(items, index);
            PanelMiniInventory.SetActive(true);

            Time.timeScale = 0.01f;
            skipUpdate = true;
        }

        enum StickMoveTo
        {
            None,
            Left,
            Right
        }
        StickMoveTo CheckDirectionOne(Vector2 stick)
        {
            var ldotX = Vector2.Dot(stick, new Vector2(1, 0));
            var absldotX = Mathf.Abs(ldotX);

            if (absldotX < UIManager.Settings.inventoryStickSensitivity)
            {
                return StickMoveTo.None;
            }

            if (ldotX > 0)
            {
                return StickMoveTo.Right;
            }
            else
            {
                return StickMoveTo.Left;
            }
        }

        StickMoveTo CheckDirection()
        {
            StickMoveTo res = CheckDirectionOne(JoyconInfo.rightStick);
            if(res == StickMoveTo.None)
            {
                res = CheckDirectionOne(JoyconInfo.leftStick);
            }
            if(res == StickMoveTo.None)
            {
                if(JoyconInfo.joyconL.GetButton(Joycon.Button.DPAD_LEFT, ManagedInput.Viewer.UI))
                {
                    res = StickMoveTo.Left;
                }
                else if(JoyconInfo.joyconL.GetButton(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
                {
                    res = StickMoveTo.Right;
                }
            }
            return res;
        }

        StickMoveTo lastMove = StickMoveTo.None;
        float selectInterval = 0;
        bool allowStick = true;

        public void Update()
        {
            if(skipUpdate)
            {
                skipUpdate = false;
                return;
            }
            currenTime += Time.unscaledDeltaTime;
            selectInterval += Time.unscaledDeltaTime;

            if (currenTime > config.BounceTime)
            {
                currenTime -= config.BounceTime;
            }
            foreach (var select in selectBox)
            {
                select.Update(currenTime / config.BounceTime);
            }

            using (var priviledge = UIManager.EnterUIState())
            {
                if (JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
                {
                    onSelectCallback(train.currentSelectedItem);
                    return;
                }
                else if(JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI))
                {
                    onSelectCallback(theItemInHand);
                    return;
                }

                var dir = CheckDirection();

                if(selectInterval > UIManager.Settings.startMenuSelectInterval)
                {
                    allowStick = true;
                    selectInterval = 0;
                }
                if(allowStick)
                {
                    if (dir == StickMoveTo.Right)
                    {
                        allowStick = false;
                        selectInterval = 0;
                        train.MoveRight();
                    }
                    else if (dir == StickMoveTo.Left)
                    {
                        allowStick = false;
                        selectInterval = 0;
                        train.MoveLeft();
                    }
                    
                }
                if(dir == StickMoveTo.None)
                {
                    allowStick = true;
                    selectInterval = 0;
                }
                lastMove = dir;
                train.Update(config.BounceCurve);
            }
        }

    }
}
