using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public enum State
    {
        PLAY = 1,
        UI = 2
    }

    public enum CameraState
    {
        First = 0,
        Third = 1,
    }

    public State state;
    public CameraState cameraState = CameraState.First;
    private GameObject mainCamera;
    private Vector3 cameraOffset;
    private Vector3 m_Rotation;
    private bool UIflag = false;
    private PlayerAction playerAction;

    public float rotateSpeed = 120.0f;
    public float moveSpeed = 4.0f;
       
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerAction = GetComponent<PlayerAction>();
        m_Rotation = new Vector3(0, 0, 0);
        if(mainCamera == null)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        state = State.PLAY;
        m_Rotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (state)
        {
            case State.PLAY:
                if ((int)PlayerAction.nowAction < 4)
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    if (cameraState == CameraState.First)
                    {
                        if (!mainCamera.GetComponent<FirstCamera>().enabled)
                        {
                            mainCamera.GetComponent<FirstCamera>().enabled = true;
                        }
                        if (mainCamera.GetComponent<ThirdPersonCamera>().enabled)
                        {
                            mainCamera.GetComponent<ThirdPersonCamera>().enabled = false;
                        }

                        MovePlayer(JoyconInfo.leftStick);
                        Look(JoyconInfo.leftStick);
                        Look(JoyconInfo.rightStick);
                    }
                    else if(cameraState == CameraState.Third)
                    {
                        if (!mainCamera.GetComponent<ThirdPersonCamera>().enabled)
                        {
                            mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
                        }
                        if (mainCamera.GetComponent<FirstCamera>().enabled)
                        {
                            mainCamera.GetComponent<FirstCamera>().enabled = false;
                        }
                        MovePlayerThird(JoyconInfo.leftStick);
                    }
                }
                if(JoyconInfo.joyconL.GetButtonDown(Joycon.Button.MINUS))
                {
                    if (cameraState == CameraState.First)
                        cameraState = CameraState.Third;
                    else if (cameraState == CameraState.Third)
                        cameraState = CameraState.First;
                }

                var bigInventory = JoyconInfo.joyconR.GetButtonDown(Joycon.Button.PLUS);
                var smallInventory = JoyconInfo.joyconL.GetButtonDown(Joycon.Button.DPAD_LEFT)
                    || JoyconInfo.joyconL.GetButtonDown(Joycon.Button.DPAD_RIGHT);

                if (bigInventory || smallInventory)
                {
                    state = State.UI;
                    playerAction.state = PlayerAction.State.UI;
                    UIflag = true;

                    Item helding = null;
                    if (playerAction.heldObject != null)
                    {
                        helding = playerAction.heldObject.GetComponent<ItemLock>().item;
                    }
                     
                    UIManager.Inventory.ShowInventory(helding, (Item item) =>
                    {
                        if(item != null)
                        {
                            if (playerAction.heldObject != null)
                            {
                                //UIManager.Inventory.AddItem(playerAction.heldObject.GetComponent<ItemLock>().item);
                                Destroy(playerAction.heldObject);
                                playerAction.heldObject = null;
                            }
                            //UIManager.Inventory.RemoveItem(item);
                            playerAction.heldObject = (GameObject)Instantiate(item.initialThing,new Vector3(0,0,0),new Quaternion());
                            playerAction.heldObject.GetComponent<ItemLock>().isInstanciated();
                            playerAction.heldObject.GetComponent<ItemLock>().locked();
                            
                            playerAction.heldObject.AddComponent<ReferObject>();
                            if(playerAction.heldObject.GetComponent<Rigidbody>() != null)
                                Destroy(playerAction.heldObject.GetComponent<Rigidbody>());
                            playerAction.heldObject.GetComponent<Rigidbody>().useGravity = false;
                            playerAction.heldObject.GetComponent<Collider>().isTrigger = true;
                            playerAction.heldObject.GetComponent<ReferObject>().referredObject = playerAction.rightHandRefer;
                            playerAction.heldObject.GetComponent<ReferObject>().isReferring = true;


                        }
                        state = State.PLAY;
                        playerAction.state = PlayerAction.State.PLAY;
                        UIflag = false;
                    }, smallInventory);

                }
                //OnGetItem();
                break;
            case State.UI:
                if (UIflag)
                {
                    //UIflag = false;
                    //UIManager.Inventory.ShowInventory((Item item) =>
                    //{
                    //    if (item != null)
                    //    {
                    //        if (playerAction.heldObject != null)
                    //            UIManager.Inventory.AddItem(playerAction.heldObject.GetComponent<ItemLock>().item);
                    //        playerAction.heldObject = Instantiate(item.initialThing);  
                            
                    //    }
                    //    state = State.PLAY;
                    //    playerAction.state = PlayerAction.State.PLAY;

                    //  });
                }
                break;
            default:
                break;
        }
   }

    private void MovePlayerThird(Vector2 direction)
    {

        if (direction.sqrMagnitude < 0.01)
        {
            animator.SetBool("Walk", false);
            return;
        }
        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        //if (JoyconInfo.joyconL.GetButton(Joycon.Button.STICK))
        if (JoyconInfo.joyconL.GetButton(Joycon.Button.STICK) || JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_DOWN))
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", true);
            scaledMoveSpeed *= 2;
        }
        else
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        var direct = new Vector3(direction.x, 0, direction.y);
        var move = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * direct;
        transform.localPosition += move * scaledMoveSpeed;
        var cameraForward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
        var angle = Vector3.Angle(new Vector3(0,0,1), direct.normalized);
        angle = direct.x < 0 ? -angle : angle;
        // rotate!!!
        transform.forward = cameraForward;
        transform.Rotate(new Vector3(0, 1, 0), angle);
        //transform.localEulerAngles = new Vector3(0, angle, 0);
        //Debug.Log(string.Format("Angle {0:N}", angle));
    }

    private void MovePlayer(Vector2 direction)
    {

        if (direction.sqrMagnitude < 0.01)
        {
            animator.SetBool("Walk", false);
            return;
        }
        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        //if (JoyconInfo.joyconL.GetButton(Joycon.Button.STICK))
        if (JoyconInfo.joyconL.GetButton(Joycon.Button.STICK) || JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_DOWN))
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", true);
            scaledMoveSpeed *= 2;
        }
        else
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        transform.localPosition += move * scaledMoveSpeed;
    }

    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        m_Rotation.y += rotate.x * scaledRotateSpeed;
        m_Rotation.x = 0;
        transform.localEulerAngles = m_Rotation;
        mainCamera.transform.localEulerAngles = new Vector3(mainCamera.transform.localEulerAngles.x, m_Rotation.y, mainCamera.transform.localEulerAngles.z);
    }

    private void MoveUI(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1)
            return;
        
    }






    private void CheckUI()
    {
        if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.PLUS))
        {
            // call ui
        }


    }



    
    public void gameOver()
    {

    }

}
