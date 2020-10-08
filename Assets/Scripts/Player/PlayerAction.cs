using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public abstract class HandleBlah
//{
//    public abstract void Handle();
//}

public class PlayerAction : MonoBehaviour
{
    public enum State
    {
        PLAY = 1,
        UI = 2
    }

    public enum Action : int
    {
        IDLE = 0,
        WALK = 1,
        RUN = 2,
        JUMP = 3,
        PICKINGUP = 4,
        FEEDING = 5,
        THROW = 6,
        PETTING = 7,
        WAVE = 8,
        FISHING = 9,
        FISHINGIDLE = 10,
        FISHREELING = 11,

        NumberOfActions
    }

    public State state;

    public static BitArray judgeState = new BitArray((int)Action.NumberOfActions);

    //public UIManager ui;

    private Rigidbody rigidbody;
    private Camera mainCamera;
    private Animator animator;
    private bool grounded;

    private List<Vector3> leftAccelData;
    private List<Vector3> leftGyroData;
    private List<Vector3> leftGyroData4;
    private List<Vector3> rightAccelData;
    private List<Vector3> rightGyroData;
    private List<Vector3> rightGyroData4;
    private List<Vector3> deltaRightGyroData;
    private bool isTiming;

    private bool throwFlag = false;
    private bool pickFlag = false;
    private bool jumpFlag = false;
    private bool fishingFlag = false;

    private float actionTime;

    public GameObject rightHandRefer;
    public static Action nowAction;
    public static float timer;
    public float blurTimer;
    public float jumpSpeed = 50;
    //public GameObject aimPointCanvas;
    private GameObject tmpHeldObject;
    public GameObject heldObject;
    private float actionIntensity;
    public float beta = 0.1f;
    public float gamma = 0.01f;

    // for fishing
    public bool fishingTrigger = false;
    private bool fishingMoveFlag = false;
    //public GameObject fishingTest;
    public Rigidbody fishRigidBody;
    public GameObject fishInstance;
    public FishCurve fishCurve;
    public GameObject fishParticleSystem;
    public GameObject fishScene;
    private List<float> intensityList = new List<float>();
    private float powerBarTimer = 0.00f;
    private float powerBarValue = 0.0f;

    public static bool fishOnHook = false;

    private void Start()
    {
        leftAccelData = new List<Vector3>();
        leftGyroData4 = new List<Vector3>();
        leftGyroData = new List<Vector3>();
        rightAccelData = new List<Vector3>();
        rightGyroData = new List<Vector3>();
        rightGyroData4 = new List<Vector3>();
        deltaRightGyroData = new List<Vector3>();
        judgeState.SetAll(false);
        judgeState.Set((int)Action.IDLE, true);
        judgeState.Set((int)Action.WALK, true);
        judgeState.Set((int)Action.RUN, true);
        judgeState.Set((int)Action.JUMP, true);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        state = State.PLAY;
        timer = 0;
        //aimPointCanvas.SetActive(false);
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        grounded = true;
        heldObject = null;
    }

    /*
    [System.Flags]
    public enum ActionJudge : int
    {
        IDLE = 0x1,
        WALK = 0x2,
        RUN = 0x4,
        JUMP = 0x8,
        THROW = 0x10,
        WAVE = 0x20,
        FISHING = 0x40,
        FISHINGIDLE = 0x80,
        FISHREELING = 0x100,
    }*/

    private void LateUpdate()
    {
        //state = State.PLAY;

        if (state == State.PLAY)
        {
            UpdateData();
            UpdateJudgeAction();
            if (heldObject != null && heldObject.GetComponent<ItemLock>().type == ItemLock.ItemType.FISHINGROD)
            {
                heldObject.transform.forward = transform.forward;
                heldObject.transform.Rotate(new Vector3(0, 1, 0), -90);
            }
            if (!isTiming)
            {
                UpdateAciton();
            }
            else
            {
                PerformAction();
                //UpdateTimer();
            }

        }

        //Debug.Log(string.Format("right accel:{0:N2},{1:N2},{2:N2}", JoyconInfo.rightAccel[0], JoyconInfo.rightAccel[1], JoyconInfo.rightAccel[2]));
        //Debug.Log(string.Format("left accel:{0:N2},{1:N2},{2:N2}", JoyconInfo.leftAccel[0], JoyconInfo.leftAccel[1], JoyconInfo.leftAccel[2]));
        //Debug.Log(string.Format("right gryo:{0:N2},{1:N2},{2:N2}", JoyconInfo.rightGyro[0], JoyconInfo.rightGyro[1], JoyconInfo.rightGyro[2]));
        //Debug.Log(string.Format("left gryo:{0:N2},{1:N2},{2:N2}", JoyconInfo.leftGyro[0], JoyconInfo.leftGyro[1], JoyconInfo.leftGyro[2]));
        //Debug.Log(string.Format("delta left gryo:{0:N2},{1:N2},{2:N2}", deltaLeftGyroData[deltaLeftGyroData.Count-1][0], deltaLeftGyroData[deltaLeftGyroData.Count - 1][1], deltaLeftGyroData[deltaLeftGyroData.Count - 1][2]));

    }

    private void UpdateJudgeAction()
    {
        judgeState.SetAll(false);
        // Jump
        if (grounded)
            judgeState.Set((int)Action.JUMP, true);
        // Throwing
        if (heldObject != null && JoyconInfo.joyconR.GetButtonDown(Joycon.Button.SHOULDER_2))
        {
            UIManager.HUD.ShowAim();
            UIManager.HUD.ShowAimFocus();
        }
        if (heldObject != null && JoyconInfo.joyconR.GetButton(Joycon.Button.SHOULDER_2))
        {
            transform.forward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
            judgeState.Set((int)Action.THROW, true);
        }
        if (heldObject != null && JoyconInfo.joyconR.GetButtonUp(Joycon.Button.SHOULDER_2))
        {
            //UIManager.HUD.ShowAimBlur();
            UIManager.HUD.ShowAimHide();
        }
        if (heldObject == null && timer == 0)
        {
            if(UIManager.HUD.PanelAim.active)
                UIManager.HUD.ShowAimHide();
        }
        // Waving
        if (heldObject == null && JoyconInfo.joyconR.GetButton(Joycon.Button.SHOULDER_2))
        {
            judgeState.Set((int)Action.WAVE, true);
        }
        // Petting
        if (nowAction == Action.IDLE && JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_DOWN))
        {
            judgeState.Set((int)Action.PETTING, true);
        }
        // PickingUp 
        //if (heldObject == null && nowAction == Action.IDLE && JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_RIGHT))
        if (nowAction == Action.IDLE && JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            judgeState.Set((int)Action.PICKINGUP, true);
        }
        // Feeding
        if(heldObject != null)
        {
            judgeState.Set((int)Action.FEEDING, true);
        }
        else
        {
            judgeState.Set((int)Action.FEEDING, false);
        }
        // Fishing
        if (fishingTrigger && heldObject != null && heldObject.GetComponent<ItemLock>().type == ItemLock.ItemType.FISHINGROD)
        {
            judgeState.Set((int)Action.FISHING, true);
        }
        else
        {
            judgeState.Set((int)Action.FISHING, false);
        }
        // FishReeling
        if (nowAction==Action.FISHINGIDLE)
        {
            judgeState.Set((int)Action.FISHREELING, true);
        }
        else
        {
            judgeState.Set((int)Action.FISHREELING, false);
        }

    }

    private void UpdateData()
    {
        if (leftAccelData.Count > 5)
            leftAccelData.RemoveAt(0);
        if (leftGyroData.Count > 5)
            leftGyroData.RemoveAt(0);

        if (leftGyroData4.Count > 3)
            leftGyroData4.RemoveAt(0);

        if (rightAccelData.Count > 5)
            rightAccelData.RemoveAt(0);
        if (rightGyroData.Count > 5)
            rightGyroData.RemoveAt(0);

        if (rightGyroData4.Count > 3)
            rightGyroData4.RemoveAt(0);

        leftAccelData.Add(JoyconInfo.leftAccel);
        leftGyroData.Add(JoyconInfo.leftGyro);

        leftGyroData4.Add(JoyconInfo.leftGyro);

        rightAccelData.Add(JoyconInfo.rightAccel);
        rightGyroData.Add(JoyconInfo.rightGyro);

        rightGyroData4.Add(JoyconInfo.rightGyro);

        if (rightGyroData.Count > 1)
        {
            if (deltaRightGyroData.Count > 18)
                deltaRightGyroData.RemoveAt(0);
            deltaRightGyroData.Add(rightGyroData[rightGyroData.Count - 1] - leftGyroData[rightGyroData.Count - 2]);
        }
    }

    private bool CheckAction(Action action)
    {
        switch(action)
        {
            case Action.PICKINGUP:
                if ((tmpHeldObject = isPickingItem()) != null)
                {
                    pickFlag = false;
                    return true;
                }
                else
                    return false;
            case Action.FEEDING:
                if (isFeeding())
                    return true;
                else
                    return false;
            case Action.JUMP:
                if (grounded && isJump())
                {
                    jumpFlag = false;
                    return true;
                }
                else
                    return false;
            case Action.THROW:
                if (isThrow())
                {
                    throwFlag = false;
                    return true;
                }
                else
                    return false;
            case Action.PETTING:
                if (isPetting())
                    return true;
                else
                    return false;
            case Action.WAVE:
                if (isWave())
                    return true;
                else
                    return false;
            case Action.FISHING:
                if (isFishing())
                    return true;
                else
                    return false;
            case Action.FISHREELING:
                if (isFishReeling())
                    return true;
                else
                    return false;
            default:
                return false;
        }
    }

    private void UpdateAciton()
    {
        
        if (fishingFlag)
            nowAction = Action.FISHINGIDLE;
        else
        {
            //if (powerBarTimer <= 0)
            //{
                if (fishingMoveFlag)
                {
                    //powerBarTimer = 0.00f;
                    UIManager.HUD.ShowPowerBarHide();
                    fishScene.SendMessage("FishEnd");
                    Vector3 direction = transform.position - fishInstance.transform.position;
                    if (powerBarValue > 0.2 && powerBarValue < 0.8)
                    {    //fishRigidBody.AddForce(direction * powerBarValue * 50.0f);
                        fishRigidBody.AddForce(new Vector3(0.0f, (1.0f - Mathf.Abs(powerBarValue - 0.5f)) * 80.0f, 0.0f));
                        fishScene.SendMessage("getOne");
                        //fishRigidBody.useGravity = true;
                        Instantiate(fishParticleSystem, fishInstance.transform);
                        fishOnHook = true;
                    }
                    else
                    {
                        fishRigidBody.useGravity = true;
                        fishOnHook = false;
                    }

                    //fishParticleSystem.SetActive(true);
                    fishingMoveFlag = false;
                }
            //}
            //else
            //{
            //    powerBarTimer -= Time.deltaTime;
            //}
            nowAction = Action.IDLE;
        }
        for(int i = 3;i < judgeState.Count;i++)
        {
            if(judgeState.Get(i))
            {
                if(CheckAction((Action)i))
                {
                    nowAction = (Action)i;
                    isTiming = true;
                    break;
                }       
            }
        }
        
        switch (nowAction)
        {
            case Action.IDLE:
                isTiming = false;
                animator.SetBool("Pick", false);
                animator.SetBool("Throw", false);
                animator.SetBool("Wave", false);
                animator.SetBool("Petting", false);
                animator.SetBool("Jump", false);
                animator.SetBool("FishReeling", false);
                animator.SetBool("Fishing", false);
                break;

            case Action.PICKINGUP:
                Debug.Log(string.Format("PICKING"));
                animator.SetBool("Pick", true);
                actionTime = 1.2f;
                break;
            case Action.THROW:
                Debug.Log(string.Format("Throwing"));
                //aimPointCanvas.SetActive(false);
                animator.SetBool("Throw", true);
                actionTime = 0.65f;
                break;
            case Action.PETTING:
                Debug.Log(string.Format("Petting"));
                animator.SetBool("Petting", true);
                actionTime = 2.6f;
                break;
            case Action.WAVE:
                Debug.Log(string.Format("Wave"));
                animator.SetBool("Wave", true);
                actionTime = 0.70f;
                break;
            case Action.JUMP:
                Debug.Log(string.Format("Jump"));
                animator.SetBool("Jump", true);
                actionTime = 1.5f;
                grounded = false;
                break;
            case Action.FISHING:
                fishingFlag = true;
                Debug.Log(string.Format("Fishing"));
                animator.SetBool("Fishing", true);
                actionTime = 1.0f;
                break;
            case Action.FISHINGIDLE:
                Debug.Log(string.Format("FishingIdle"));
                actionTime = 10.0f;
                break;
            case Action.FISHREELING:
                //fishingFlag = true;
                //fishingTest.GetComponent<FishCurve>().timer = 0.0f;
                fishingFlag = false;
                fishingMoveFlag = true;
                UIManager.HUD.ShowPowerBar();
                fishScene.SendMessage("FishStart");
                powerBarTimer = 10.0f;

                //fishCurve = heldObject.GetComponent<FishCurve>();
                Debug.Log(string.Format("FishReeling"));
                animator.SetBool("FishReeling", true);
                actionTime = 10.0f;
                //actionTime = 4.0f;
                break;
            default:
                isTiming = false;
                animator.SetBool("Pick", false);
                animator.SetBool("Throw", false);
                animator.SetBool("Wave", false);
                animator.SetBool("Petting", false);
                animator.SetBool("Jump", false);
                animator.SetBool("FishReeling", false);
                animator.SetBool("Fishing", false);
                break;
        }
        
    }

    private void PerformAction()
    {
        switch (nowAction)
        {
            case Action.PICKINGUP:
                PickingUp();
                break;
            case Action.THROW:
                Throwing();
                break;
            case Action.WAVE:
                Waving();
                break;
            case Action.PETTING:
                AnimationTimeStep();
                break;
            case Action.JUMP:
                Jumping();
                break;
            case Action.FISHING:
                Fishing();
                break;
            case Action.FISHINGIDLE:
                AnimationTimeStep();
                break;
            case Action.FISHREELING:
                FishReeling();
                break;
            default:
                isTiming = false;
                animator.SetBool("Pick", false);
                animator.SetBool("Throw", false);
                animator.SetBool("Wave", false);
                animator.SetBool("Petting", false);
                animator.SetBool("Jump", false);
                animator.SetBool("FishReeling", false);
                animator.SetBool("Fishing", false);
                break;
        }
    }

    private void Rumble()
    {
        if(JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_LEFT))
            JoyconInfo.joyconR.SetRumble(160, 320, 0.1f, 200);
        if(JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_RIGHT))
            JoyconInfo.joyconR.SetRumble(160, 320, 1.2f, 200);
    }

    void OnCollisionEnter(Collision other)
    {
        grounded = true;
    }

    // Action Check
    private bool isJump()
    {
        //if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.STICK))
        if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.STICK)||JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_UP))
        {
            return true;
        }
        else
            return false;
    }

    private bool isThrow()
    {
        bool flag = true;
        foreach(Vector3 rightGyro in rightGyroData)
        {
            if (rightGyro.y < 3.0)
            {
                flag = false;
                break;
            }
        }
        if(flag)
        {
            actionIntensity = 0.0f;
            foreach (Vector3 rightGyro in rightGyroData)
            {
                actionIntensity += Mathf.Abs(rightGyro.y);
            }
            actionIntensity /= rightGyroData.Count;
        }
        return flag;
    }

    private bool isPetting()
    {
        bool flag = true;
        
        int negativeLength = 0;
        int positiveLength = 0;
        bool lastPositive = deltaRightGyroData[0].z > 0? true : false;
        bool changeFlag = false;
        if (lastPositive)
            positiveLength++;
        else
            negativeLength++;
        for(int i = 0;i < deltaRightGyroData.Count;i++)
        {
            if (Mathf.Abs(deltaRightGyroData[i].z) > 0.1)
            {
                if(lastPositive && deltaRightGyroData[i].z > 0)
                {
                    positiveLength++;
                }
                else if(!lastPositive && deltaRightGyroData[i].z < 0)
                {
                    negativeLength++;
                }
                else if(!changeFlag && lastPositive && deltaRightGyroData[i].z < 0)
                {
                    lastPositive = false;
                    changeFlag = true;
                    negativeLength++;
                }
                else if(!changeFlag && !lastPositive && deltaRightGyroData[i].z > 0)
                {
                    lastPositive = true;
                    changeFlag = true;
                    positiveLength++;
                }
            }
            else
            {
                flag = false;
                break;
            }
        }
        if(flag)
        {
            flag = Mathf.Abs(positiveLength - negativeLength) < 2 ? true : false;
        }
        return flag;
    }
    
    private bool isWave()
    {
        bool flag = true;
        foreach (Vector3 rightGyro in rightGyroData)
        {
            if (rightGyro.z > -5.0)
            {
                flag = false;
                break;
            }
        }
        if(flag)
        {
            int SearchRadius = 5;
            GameObject[] creatrues = GameObject.FindGameObjectsWithTag("Creatures");
            foreach (var creature in creatrues)
            {
                if (Vector3.Distance(creature.transform.position, transform.position) < SearchRadius)
                {
                    //JoyconInfo.joyconR.SetRumble(160, 320, 1.0f, 200);
                    creature.SendMessage("hited");
                }

            }
        }
        return flag;
        //bool flag = true;
        //foreach(Vector3 rightGyro in rightGyroData)
        //{
        //    if (rightGyro.y > -10.0)
        //    {
        //        flag = false;
        //        break;
        //    }
        //}
        //return flag;
    }

    private bool isFishing()
    {
        bool flag = true;
        foreach (Vector3 rightGyro in rightGyroData)
        {
            if (rightGyro.z > -3.0)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            foreach (Vector3 leftGyro in leftGyroData)
            {
                if (leftGyro.z < 3.0)
                {
                    flag = false;
                    break;
                }
            }
        }
        return flag;

    }

    private bool isFishReeling()
    {
        if(nowAction == Action.FISHINGIDLE && JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
        {
            return true;
        }
        return false;
    }

    private GameObject isPickingItem()
    {
        int SearchRadius = 3;
        Collider[] cols = Physics.OverlapSphere(transform.position, SearchRadius);
        for (int i = 0; i < cols.Length; i++)
            if (cols[i].gameObject.GetComponent<ItemLock>() != null)
            {
                if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                {
                    Destroy(cols[i].GetComponent<Rigidbody>());
                    //Destroy(cols[i].GetComponent<Collider>());
                    return cols[i].gameObject;
                }
            }
        
        return null;
    }


    private void dontEat()
    {
        heldObject.GetComponent<ItemLock>().natual = false;
    }
    private bool isFeeding()
    {
        if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
        {
            int SearchRadius = 3;
            GameObject[] creatrues = GameObject.FindGameObjectsWithTag("Creatures");
            foreach(var creature in creatrues)
            {
                if (Vector3.Distance(creature.transform.position, transform.position) < SearchRadius)
                {
                    JoyconInfo.joyconR.SetRumble(160, 320, 1.0f, 200);
                    heldObject.GetComponent<ItemLock>().natual = true;
                    creature.SendMessage("noticed");

                    //JoyconInfo.joyconR.SetRumble(160, 320, 1.0f, 200);
                    //if(creature.SendMessage("noticed"))

                    //if (heldObject.GetComponent<ItemLock>().IsEditbale(AI_common_settings))
                    //heldObject.GetComponent<ItemLock>().natual = true;
                    return true;
                } 
                
            }
        }
        return false;
    }

    private GameObject feedItem()
    {
        if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
        {
            int SearchRadius = 3;
            Collider[] cols = Physics.OverlapSphere(transform.position, SearchRadius);
            for (int i = 0; i < cols.Length; i++)
                if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                {
                    cols[i].gameObject.GetComponent<ItemLock>().locked();
                    cols[i].gameObject.AddComponent<ReferObject>();
                    cols[i].gameObject.GetComponent<ReferObject>().referredObject = rightHandRefer;
                    cols[i].gameObject.GetComponent<ReferObject>().isReferring = true;
                    return cols[i].gameObject;
                }
        }
        return null;
    }

    // Action Peform
    private void AnimationTimeStep()
    {
        timer += Time.deltaTime;
        if (timer > actionTime)
        {
            timer = 0;
            actionTime = 0;
            isTiming = false;
        }
    }

    private void Waving()
    {
        AnimationTimeStep();
    }

    private void Throwing()
    {
        AnimationTimeStep();
        if (!throwFlag && timer > 0.30f)
        {
            throwFlag = true;
            if (heldObject != null)
            {
                Rigidbody rigidbody = heldObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                //SphereCollider collider = heldObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
                //collider.radius = 2.0f;
                Vector3 v3 = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f);
                Ray ray = mainCamera.ScreenPointToRay(v3);
                rigidbody.velocity = ray.direction * beta * actionIntensity;
                heldObject.GetComponent<Rigidbody>().useGravity = true;
                heldObject.GetComponent<Collider>().isTrigger = false;
                heldObject.GetComponent<ItemLock>().unlocked();
                Destroy(heldObject.GetComponent<ReferObject>());
                heldObject = null;
                tmpHeldObject = null;
                UIManager.HUD.ShowAimBlur();
            }
            //Debug.Log(string.Format("transform.forward:{0:N2},{1:N2},{2:N2}", transform.forward[0], transform.forward[1], transform.forward[2]));
            //Debug.Log(string.Format("transform.up:{0:N2},{1:N2},{2:N2}", transform.up[0], transform.up[1], transform.up[2]));
       }
    }

    private void PickingUp()
    {
        AnimationTimeStep();


        if (!pickFlag && timer > 0.40f)
        {
            // 如果是空手的话, 自动放到手里
            if(heldObject == null)
            {
                heldObject = tmpHeldObject;
                pickFlag = true;
                tmpHeldObject = null;

                heldObject.GetComponent<ItemLock>().locked();
                heldObject.AddComponent<ReferObject>();
                heldObject.GetComponent<Collider>().isTrigger = true;
                heldObject.GetComponent<ReferObject>().referredObject = rightHandRefer;
                heldObject.GetComponent<ReferObject>().isReferring = true;
            }

            // 否则将捡到的放进背包
            else
            {
                var newItem = tmpHeldObject.GetComponent<ItemLock>().item;

                UIManager.Inventory.AddItem(newItem);
                UIManager.HUD.ShowNewItemInInventoryHint(newItem);

                Destroy(tmpHeldObject);
                pickFlag = true;
                // heldObject = null;
                tmpHeldObject = null;
            }


            
            //if(heldObject != null)
            //{
            //    UIManager.Inventory.AddItem(heldObject.GetComponent<ItemLock>().item);
            //    Destroy(heldObject);
            //}
            //heldObject = tmpHeldObject;
            //pickFlag = true;
            //heldObject.GetComponent<ItemLock>().locked();
            //heldObject.AddComponent<ReferObject>();
            //heldObject.GetComponent<Collider>().isTrigger = true;
            //heldObject.GetComponent<ReferObject>().referredObject = rightHandRefer;
            //heldObject.GetComponent<ReferObject>().isReferring = true;
        }
}
    
    private void Jumping()
    {
        AnimationTimeStep();
        if (!jumpFlag && timer > 0.6f)
        {
            jumpFlag = true;
            rigidbody.AddForce(Vector3.up * jumpSpeed);
        }
    }

    private void Fishing()
    {
        AnimationTimeStep();
        nowAction = Action.FISHINGIDLE;
    }

    private void FishReeling()
    {
        
        float finalIntensity = 0.0f;
        finalIntensity -= fishCurve.powerCurve.Evaluate(timer / actionTime);
        actionIntensity = JoyconInfo.rightAccel.sqrMagnitude;
        if (actionIntensity > 3.0f)
        {
            finalIntensity += actionIntensity * gamma;
            //Debug.Log(actionIntensity);
            //Debug.Log(actionIntensity * gamma);
        }
        if(finalIntensity > 7.0f || finalIntensity < -7.0f)
        {
            timer = actionTime;
            UIManager.HUD.ShowPowerBarValue(finalIntensity > -7 ? 1.0f : 0.0f);
            powerBarValue = finalIntensity > -7 ? 1.0f : 0.0f;
            //powerBarTimer = -1.0f;
        }
        else
        {
            float finalIntensity01 = (Mathf.Clamp(finalIntensity, -7.0f, 7.0f) + 7.0f) / 14.0f;
            intensityList.Add(finalIntensity01);
            if (intensityList.Count > 2)
            {
                //powerBarValue = finalIntensity01;
                //UIManager.HUD.ShowPowerBarValue(finalIntensity01);
                intensityList.Sort();
                UIManager.HUD.ShowPowerBarValue(intensityList[1]);
                powerBarValue = intensityList[1];
                JoyconInfo.joyconR.SetRumble(160, 320, Mathf.Lerp(0.3f, 1.5f, powerBarValue), 200);
                intensityList.Clear();
            }
        }
        //fishingTest.transform.position += new Vector3(0, 0.01f, 0) * finalIntensity;
        AnimationTimeStep();
    }


    public bool isHolding()
    {
        return heldObject != null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Destination")
            fishingTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        fishingTrigger = false;
    }

}


