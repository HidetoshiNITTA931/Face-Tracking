using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetController : MonoBehaviour
{
    public float zz;
    public float scaleConst = 0.1f;
    public float xconst;
    public float yconst;
    public int average_num;

    private List<int> HightList = new List<int>();
    private List<int> CenterXList = new List<int>();
    private List<int> CenterYList = new List<int>();

    private List<int> PitchList = new List<int>();
    private List<int> YawList = new List<int>();
    private List<int> RollList = new List<int>();


    // Start is called before the first frame update
    void Start()
    {

    }

    public void UpdateScale(int Height)
    {
        HightList.Add(Height);
        if (HightList.Count >= average_num)
        {
            float val = AverageList(HightList);
            float sc = scaleConst * val;
            transform.localScale = new Vector3(sc, sc, sc);
            HightList.RemoveAt(0);
        }
        Debug.Log(HightList.Count);
        
    }

    public void UpdateRotation(int pitch, int yaw, int roll)
    {
        PitchList.Add(pitch);
        YawList.Add(yaw);
        RollList.Add(roll);

        if(PitchList.Count >= average_num)
        {
            Quaternion _quatermion = Quaternion.Euler(-1 * AverageList(PitchList),
                AverageList(YawList) + 180, -AverageList(RollList));
            transform.rotation = _quatermion;
            PitchList.RemoveAt(0);
            YawList.RemoveAt(0);
            RollList.RemoveAt(0);
        }
    }

    public void UpdatePosition(int CenterX, int CenterY)
    {
        CenterXList.Add(CenterX);
        CenterYList.Add(CenterY);

        if(CenterXList.Count >= average_num)
        {
            float xx = (AverageList(CenterXList) / 100) - 9.6f;
            float yy = 5.4f - (AverageList(CenterYList) / 100);
            transform.position = new Vector3(xx + xconst, yy + yconst, zz);
            CenterXList.RemoveAt(0);
            CenterYList.RemoveAt(0);
        }
        
    }


    private float AverageList(List<int> array)
    {
        float all = 0;
        for (int i = 0; i < array.Count; i++)
        {
            all += array[i];
        }
        float ave = all / array.Count;
        return ave;
    }
}
