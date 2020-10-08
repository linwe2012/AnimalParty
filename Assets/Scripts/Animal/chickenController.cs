using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class chickenController : MonoBehaviour
{
    private float vertical;
    private float horizontal;
    private Vector3 dir;
    private Vector3 tmpPos;
    private Animator ani;
    private List<Vector3> targetPos=new List<Vector3>();
    private NavMeshAgent chickenNav;
    private Transform targetNav;
    private enum chickenState
    {
        STAND, //站立
        WANDER, //随机走动
        WANDERING,
        RUN, //奔跑
        RUNNING,
        EAT //吃
    };
    private chickenState State;
    public GameObject cornKernels;
    public GameObject chicken;

    private int Satisfaction = 0;
    private int MaxSat = 3;
    private bool satisfied = false;
    void Start()
    {
        ani = chicken.GetComponent<Animator>();
        State = chickenState.STAND;
        chickenNav = gameObject.GetComponent<NavMeshAgent>();
        //tmpPos = chicken.transform.;
        //for (int i =0;i<4;i++)
        //{
        //    Vector3 newPos = new Vector3(tmpPos.x + Random.Range(-2.0f, 2.0f), 0, tmpPos.z + Random.Range(-2.0f, 2.0f));
        //    targetPos.Add(newPos);
        //}
        //State = chickenState.STAND;
        //ani.SetBool("isEat", true);
        //ani.SetInteger("randAct", Random.Range(0, 5));

        StartCoroutine(WaitAndShow(Random.Range(0.0f, 5.0f)));
    }

    void Update()
    {

        //vertical = Input.GetAxis("Vertical");
        //horizontal = Input.GetAxis("Horizontal");

        //Quaternion direction = Quaternion.LookRotation(cornKernels.transform.position- chicken.transform.position, Vector3.up);

        //chicken.transform.rotation = Quaternion.Slerp(chicken.transform.rotation, direction, 0.1f);
        
        //ani.SetBool("isRun", true);
        

        if(State==chickenState.RUN)
        {
            GameObject tmpDir = cornKernels.transform.GetChild(Random.Range(0, cornKernels.transform.childCount)).gameObject;
            dir = tmpDir.transform.position;
            //Debug.Log(tmpDir);
            State = chickenState.RUNNING;
        }
        if(State==chickenState.RUNNING)
        {
            //dir = cornKernels.transform.position;
            Quaternion direction = Quaternion.LookRotation(dir - chicken.transform.position, Vector3.up);
            chicken.transform.rotation = Quaternion.Slerp(chicken.transform.rotation, direction, 0.1f);
            //chicken.transform.Translate(Vector3.forward * 1.0f * Time.deltaTime);
            chickenNav.SetDestination(dir);
            ani.SetBool("isRun", true);
            if (calDistance(dir, chicken.transform.position) < 1.0f)
            {
                chickenNav.SetDestination(chicken.transform.position);
                State = chickenState.EAT;
                ani.SetBool("isEat", true);
                ani.SetBool("isRun", false);
            }
        }
        if (State == chickenState.WANDER)
        {
            //Debug.Log(dir);
            //Debug.Log(chicken.transform.position);
            State = chickenState.WANDERING;
            StartCoroutine(Wander(Random.Range(4.0f, 8.0f)));
            ani.SetBool("isWander", true);
            chickenNav.speed = 0.2f;


        }
        if(State==chickenState.WANDERING)
        {
            //dir = cornKernels.transform.GetChild(Random.Range(0, cornKernels.transform.childCount)).transform.position;

            //chicken.transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
            
        }

        //if (State==chickenState.STAND)
        //{
        //    dir = targetPos[Random.Range(0, 4)];
        //    State = chickenState.WANDER;

        //}

        //if (State == chickenState.WANDER)
        //{
        //    //产生动作
        //    chicken.transform.rotation = Quaternion.LookRotation(dir);
        //    chicken.transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
        //    ani.SetBool("isWander", true);
        //}
        //Debug.Log(calDistance(dir, chicken.transform.localPosition));
        //if (calDistance(dir, chicken.transform.localPosition) < 0.1f) 
        //{
        //    State = chickenState.STAND;
        //    ani.SetBool("isWander", false);
        //}

        //if(cornKernels.transform.childCount!=0)
        //{

        //}

    }
    private void OnCollisionEnter(Collision collision)
    {
        State = chickenState.STAND;
    }
    private float calDistance(Vector3 v1,Vector3 v2)
    {
        float dis;
        dis=Mathf.Sqrt(Mathf.Pow(v1.x-v2.x,2)+Mathf.Pow(v1.z-v2.z,2));
        return dis;
    }

    IEnumerator Wander(float waitTime)
    {
        while(true)
        {
            
            Vector3 tmp = chicken.transform.position;
            dir = new Vector3(tmp.x + Random.Range(-2.0f, 2.0f), tmp.y, tmp.z + Random.Range(-2.0f, 2.0f));
            Quaternion direction = Quaternion.LookRotation(dir - chicken.transform.position, Vector3.up);
            chicken.transform.rotation = Quaternion.Slerp(chicken.transform.rotation, direction, 0.1f);
            chickenNav.SetDestination(dir);
            yield return new WaitForSeconds(10.0f);
        }


    }
    IEnumerator WaitAndShow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //等待执行 
        int Status = Random.Range(0, 8);
        //Status = 0;
        if (Status <=3)
        {
            State = chickenState.RUN;
            //do nothing
        }
        else if (Status == 4)
        {
            //State = chickenState.RUN;
            State = chickenState.EAT;
            ani.SetBool("isEat", true);
        }
        else if (Status == 5)
        {
            //State = chickenState.RUN;
            State = chickenState.EAT;
            ani.SetBool("isTurn", true);
        }
        else
        {
            State = chickenState.WANDER;
        }
    } 
    public void addSatisfication()
    {
        if (Satisfaction >= MaxSat)
            return;
        else
            Satisfaction++;

        ani.SetBool("isEat", true);

        if (Satisfaction==MaxSat)
        {
            satisfied = true;
        }
    }
    public bool isSatisfied()
    {
        return satisfied;
    }
}
