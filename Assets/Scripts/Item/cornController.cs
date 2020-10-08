using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cornController : MonoBehaviour
{
    Quaternion r;
    // Start is called before the first frame update

    Vector3 tmpAngle;
    
    int bound = 10;
    int tmpCount = 0;
    enum rotationStatus
    {
        Default,
        Quater,
        Half,
        TQuater,
        Full,
        Finished
    };
    rotationStatus tmp = rotationStatus.Default;


    public GameObject cornKernel;
    public GameObject cornKernelList;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        r = JoyconInfo.rightOrientation;
        //Debug.Log(r);

        //transform.rotation = r;
        transform.rotation = new Quaternion(r.y, r.z,r.x , r.w);
        tmpAngle = transform.eulerAngles;
        if (tmp==rotationStatus.Default)
        {
            if (tmpAngle.x >= 0.0f && tmpAngle.x < 45.0f)
                tmp = rotationStatus.Quater;
        }
        if (tmp == rotationStatus.Quater)
        {
            if (tmpAngle.x >= 45.0f && tmpAngle.x < 90.0f)
                tmp = rotationStatus.Half;
        }
        else if(tmp==rotationStatus.Half)
        {
            if (tmpAngle.x >= 90.0f && tmpAngle.x < 315.0f)
                tmp = rotationStatus.TQuater;
        }
        else if (tmp == rotationStatus.TQuater)
        {
            if (tmpAngle.x >= 315.0f && tmpAngle.x < 360.0f)
                tmp = rotationStatus.Full;
        }
        else if(tmp ==rotationStatus.Full)
        {
            tmpCount++;
            if(tmpCount==bound)
            {
                tmp = rotationStatus.Finished;
                tmpCount = 0;
            }
        }
        else if(tmp==rotationStatus.Finished)
        {
            Debug.Log("ADD Corn");
            addCornKernel();
            tmp = rotationStatus.Default;
        }
    }
    private void addCornKernel()
    {
        GameObject newKernel = Instantiate(cornKernel, transform.position, transform.rotation);
        newKernel.transform.SetParent(cornKernelList.transform);
        newKernel.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-5,5), Random.Range(-5, 5), Random.Range(-5, 5));
    }
}
