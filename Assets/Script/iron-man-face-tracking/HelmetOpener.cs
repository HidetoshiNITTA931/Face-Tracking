using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetOpener : MonoBehaviour
{
    private Quaternion fromQ;
    private Quaternion toQ;

    private Vector3 fromP;
    private Vector3 toP;

    private float Proportion;

    // 0:close 1:opend
    public int status = 0;
    private int starter;

    // Start is called before the first frame update
    void Start()
    {
        starter = 0;
    }

    public int GetStatus()
    {
        return status;
    }

    // Update is called once per frame
    void Update()
    {
        if (starter != 0)
        {
            if (Proportion < 1)
                Proportion += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(fromQ, toQ, Proportion);
            transform.localPosition = Vector3.Slerp(fromP, toP, Proportion);
        }
        
    }

    public void Open()
    {
        Proportion = 0;
        fromQ = this.transform.localRotation;
        toQ = Quaternion.Euler(-180, 0, 0);
        fromP = this.transform.localPosition;
        toP = new Vector3(0, 0.15f, 0);
        status = 1;
        starter = 1;
    }

    public void Close()
    {
        Proportion = 0;
        fromQ = this.transform.localRotation;
        toQ = Quaternion.Euler(-90, 0, 0);
        fromP = this.transform.localPosition;
        toP = new Vector3(0, 0, 0);
        status = 0;
        starter = 1;
    }

}
