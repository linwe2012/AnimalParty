using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTime : MonoBehaviour
{
    public UIManager ui;

    enum HelpSteps
    {
        None,

        HintItem1,
        HintItem2,
        HintItem3,
        HintItem4,
        HintItem5,
        HintItem6,

        Petting,
        PettingHide,
        ShowAim,
        ShowAimFoucus,
        ShowAimBlur,
        ShowAimHide,


        ThrowBelow,
        ThrowBelowHide,
        ShowZombieAttack,
        ShowZombieAttackHide,
        ClickBothSLSR,
        ClickSLSR,
        ClickR,
        ClickLeftR,
        ClickLeftSLSR,

        TestSelectMission,
        TestSelectMissionHide,

        Boating,
        Hunt,
        Eat,
        Defend,
        Fly,

        // Inventory 相关 API:
        Invent,

        InventAdd1,
        InventDel1,

        InventAdd2,
        InventAdd3,
        InventAdd4,


        Congrate1,
        Congrate1Hide,
        Congrate2,
        Congrate2Hide,
        Congrate3,
        Congrate3Hide,

        Loading,
        Loading10,
        Loading50,
        Loading100,
        Loading20,
        LoadingHide,

        Powerbar,
        Powerbar10,
        Powerbar20,
        Powerbar90,
        Powerbar5,
        Powerbar50,
        PowerbarHide,


        Minus,
        MinusHide,

        Dialog,
        DialogHide,

        Minimap,

        Stick,
        StickHide,

        
        
        
        
        
    }
    public Item[] itemsForTest = new Item[6];
    HelpSteps CurrentStep = HelpSteps.None;

    // Start is called before the first frame update
    void Start()
    {
        handleUpdate = false;
        UIManager.HUD.ShowJoyconConnectTutorial(() =>
        {
            handleUpdate = true;
        });
    }

    bool handleUpdate = true;

    // Update is called once per frame
    void Update()
    {
        if(!handleUpdate)
        {
            return;
        }

        bool bigPanel = JoyconInfo.joyconR.GetButtonDown(Joycon.Button.PLUS);
        bool smallPanel = JoyconInfo.joyconL.GetButtonDown(Joycon.Button.DPAD_LEFT)
            || JoyconInfo.joyconL.GetButtonDown(Joycon.Button.DPAD_RIGHT);


        if (bigPanel || smallPanel)
        {
            handleUpdate = false;
            UIManager.Inventory.ShowInventory(null, (Item item) =>
            {
                handleUpdate = true;
            }, smallPanel);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            CurrentStep = CurrentStep + 1;
            UIManager.Inventory.AccepKeyboard = true;

            switch (CurrentStep)
            {
                case HelpSteps.HintItem1:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[0]);
                    break;
                case HelpSteps.HintItem2:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[1]);
                    break;
                case HelpSteps.HintItem3:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[2]);
                    break;
                case HelpSteps.HintItem4:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[3]);
                    break;
                case HelpSteps.HintItem5:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[4]);
                    break;
                case HelpSteps.HintItem6:
                    UIManager.HUD.ShowNewItemInInventoryHint(itemsForTest[5]);
                    break;
                case HelpSteps.TestSelectMission:
                    UIManager.Inventory.TestShowSelectMission();
                    break;
                case HelpSteps.TestSelectMissionHide:
                    UIManager.Inventory.TestShowSelectMissionHide();
                    break;
                case HelpSteps.Hunt:
                    UIManager.HUD.ShowHelp(UIManager.Button.A, "Attack animal");
                    break;
                case HelpSteps.Eat:
                    UIManager.HUD.ShowHelp(UIManager.Button.B, "Eat Food");
                    break;
                case HelpSteps.Defend:
                    UIManager.HUD.ShowHelp(UIManager.Button.X, "Defend Pose");
                    break;
                case HelpSteps.Fly:
                    UIManager.HUD.ShowHelp(UIManager.Button.Y, "Fly!!! Fly hig Lorem");
                    break;
                case HelpSteps.Boating:
                    UIManager.Joycon.ShowBoat().MakeBigger(3);
                    break;
                case HelpSteps.ClickSLSR:
                    UIManager.Joycon.ShowClickSideButtons(new UIManager.Button[]{
                        UIManager.Button.SL,
                        UIManager.Button.SR
                    }).MakeBigger(3);
                    break;
                case HelpSteps.ClickR:
                    UIManager.Joycon.ShowClickSideButtons(new UIManager.Button[]{
                        UIManager.Button.Top
                    }).MakeBigger(3);
                    break;
                case HelpSteps.ClickLeftSLSR:
                    UIManager.Joycon.ShowClickSideButtons(new UIManager.Button[]{
                        UIManager.Button.SL | UIManager.Button.LeftJoycon,
                        UIManager.Button.SR | UIManager.Button.LeftJoycon,
                    }).MakeBigger(3);
                    break;
                case HelpSteps.ClickLeftR:
                    UIManager.Joycon.ShowClickSideButtons(new UIManager.Button[]{
                        UIManager.Button.Top | UIManager.Button.LeftJoycon,
                    }).MakeBigger(3);
                    break;
                case HelpSteps.ClickBothSLSR:
                    UIManager.Joycon.ShowClickSideButtons(new UIManager.Button[]{
                        UIManager.Button.SL,
                        UIManager.Button.SR,
                        UIManager.Button.SL | UIManager.Button.LeftJoycon,
                        UIManager.Button.SR | UIManager.Button.LeftJoycon,
                    }).MakeBigger(3);
                    break;
                case HelpSteps.ThrowBelow:
                    UIManager.Joycon.ShowThrowFromBelow("给企鹅喂食");
                    break;
                case HelpSteps.ShowAim:
                    UIManager.HUD.ShowAim();
                    break;
                case HelpSteps.ShowAimFoucus:
                    UIManager.HUD.ShowAimFocus();
                    break;
                case HelpSteps.ShowAimHide:
                    UIManager.HUD.ShowAimHide();
                    break;
                case HelpSteps.ShowAimBlur:
                    UIManager.HUD.ShowAimBlur();
                    break;
                case HelpSteps.ThrowBelowHide:
                    UIManager.Joycon.ShowThrowFromBelowHide();
                    break;
                case HelpSteps.ShowZombieAttack:
                    UIManager.Joycon.ShowZombieAttack("攻击蛇");
                    break;
                case HelpSteps.ShowZombieAttackHide:
                    UIManager.Joycon.ShowZombieAttackHide();
                    break;
                case HelpSteps.Stick:
                    UIManager.HUD.ShowJoyconStick("操作摇杆");
                    break;
                case HelpSteps.StickHide:
                    UIManager.HUD.ShowJoyconStickHide();
                    break;
                case HelpSteps.Petting:
                    UIManager.Joycon.ShowPettingAnimal("Pet the animal");
                    break;
                case HelpSteps.PettingHide:
                    UIManager.Joycon.ShowPettingAnimalHide();
                    break;
                case HelpSteps.Dialog:
                    UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "Hello, Welcome to Animal Party! 欢迎来到动物派对", "ZZP:");
                    break;
                case HelpSteps.DialogHide:
                    UIManager.HUD.ShowDialogHide();
                    break;
                case HelpSteps.Minimap:
                    UIManager.HUD.ShowMinimap(Camera.main);
                    break;
                case HelpSteps.Minus:
                    UIManager.Joycon.ShowMinus("使用 '-' 键切换视角", 20);
                    break;
                case HelpSteps.MinusHide:
                    UIManager.Joycon.ShowMinusHide();
                    break;
                case HelpSteps.Powerbar:
                    UIManager.HUD.ShowPowerBar();
                    break;
                case HelpSteps.PowerbarHide:
                    UIManager.HUD.ShowPowerBarHide();
                    break;
                case HelpSteps.Powerbar10:
                    UIManager.HUD.ShowPowerBarValue(0.1f);
                    break;
                case HelpSteps.Powerbar20:
                    UIManager.HUD.ShowPowerBarValue(0.2f);
                    break;
                case HelpSteps.Powerbar5:
                    UIManager.HUD.ShowPowerBarValue(0.05f);
                    break;
                case HelpSteps.Powerbar90:
                    UIManager.HUD.ShowPowerBarValue(0.9f);
                    break;
                case HelpSteps.Powerbar50:
                    UIManager.HUD.ShowPowerBarValue(0.5f);
                    break;
                case HelpSteps.Loading:
                    UIManager.HUD.ShowLoading("企鹅们来自遥远的南极, 动物园的企鹅们已经忘记了如何捕食");
                    break;
                case HelpSteps.Loading10:
                    UIManager.HUD.ShowLoadingValue(0.1f);
                    break;
                case HelpSteps.Loading20:
                    UIManager.HUD.ShowLoadingValue(0.2f);
                    break;
                case HelpSteps.Loading50:
                    UIManager.HUD.ShowLoadingValue(0.5f);
                    break;
                case HelpSteps.Loading100:
                    UIManager.HUD.ShowLoadingValue(1.0f);
                    break;
                case HelpSteps.LoadingHide:
                    UIManager.HUD.ShowLoadingHide();
                    break;
                case HelpSteps.Invent:
                    
                    UIManager.Inventory.ShowInventory(null, (Item item)=>
                    {

                    }, false);
                    break;
                case HelpSteps.InventAdd1:
                    UIManager.Inventory.AddItem(itemsForTest[0]);
                    
                    break;
                case HelpSteps.InventDel1:
                    UIManager.Inventory.RemoveItem(itemsForTest[0]);

                    break;
                case HelpSteps.InventAdd2:
                    UIManager.Inventory.AddItem(itemsForTest[1]);
                    
                    break;
                case HelpSteps.InventAdd3:
                    UIManager.Inventory.AddItem(itemsForTest[2]);
                    
                    break;
                case HelpSteps.InventAdd4:
                    UIManager.Inventory.AddItem(itemsForTest[3]);
                    
                    break;
                case HelpSteps.Congrate1:
                    UIManager.HUD.ShowCongraluate(1, "哦哦哦, 企鹅不大高兴, 他们抗议着回到了家中");
                    break;
                case HelpSteps.Congrate2:
                    UIManager.HUD.ShowCongraluate(2, "企鹅们比较满足, 回家向家人赞赏了你的善行");
                    break;
                case HelpSteps.Congrate3:
                    UIManager.HUD.ShowCongraluate(3, "企鹅非常快乐, 他们欢快的回家, 歌颂着你的名字");
                    break;
                case HelpSteps.Congrate1Hide:
                    UIManager.HUD.ShowCongraluateHide();
                    break;
                case HelpSteps.Congrate2Hide:
                    UIManager.HUD.ShowCongraluateHide();
                    break;
                case HelpSteps.Congrate3Hide:
                    UIManager.HUD.ShowCongraluateHide();
                    break;

            }
        }
    }
}
