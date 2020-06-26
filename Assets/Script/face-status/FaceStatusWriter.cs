using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using UnityEngine.UI;
using TMPro;

public class FaceStatusWriter : MonoBehaviour
{
    public string EyeLeftClose { get; set; }
    public string EyeRightClose { get; set; }
    public string MouthOpen { get; set; }
    public int Pitch { get; set; }
    public int Yaw { get; set; }
    public int Roll { get; set; }

    private TextMeshProUGUI uiText;

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
    }

    public void Write()
    {
        string status = "eyeLeftClose:" + EyeLeftClose + "\neyeRightClose:" + EyeRightClose
            + "\nmouthOpen:" + MouthOpen + "\npitch:" + Pitch.ToString() + "\nyaw:" + Yaw.ToString() 
            + "\nroll:" + Roll.ToString();
        Debug.Log(status);

        uiText.text = status;

    }
}
