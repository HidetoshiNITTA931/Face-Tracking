using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.Face;

public class PositionManager : MonoBehaviour
{
    private FaceFrameResult[] faceFrameResults;
    private FaceResultManager faceResultManager;
    private int bodyCount;
    public GameObject faceManager;
    public GameObject Ironman;
    private HelmetController helmetController;

    private const float FaceRotationIncrementInDegrees = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        faceResultManager = faceManager.GetComponent<FaceResultManager>();
        bodyCount = faceResultManager.GetBodyCount();
        helmetController = Ironman.GetComponent<HelmetController>();
    }

    // Update is called once per frame
    void Update()
    {
        faceFrameResults = faceResultManager.GetFaceFrameResults();
        bodyCount = faceResultManager.GetBodyCount();
        for (int i = 0; i < bodyCount; i++)
        {
            var result = faceFrameResults[i];
            if (result != null)
            {
                var rotationOriant = result.FaceRotationQuaternion;
                int pitch, yaw, roll;
                ExtractFaceRotationInDegrees(rotationOriant, out pitch, out yaw, out roll);
                RectI box = result.FaceBoundingBoxInColorSpace;
                int width = box.Right - box.Left;
                int height = box.Bottom - box.Top;
                int centerX = (int)((box.Right + box.Left) / 2);
                int centerY = (int)((box.Bottom + box.Top) / 2);

                if (centerX != 0)
                {
                    helmetController.UpdatePosition(centerX, centerY);
                    helmetController.UpdateScale(height);
                    helmetController.UpdateRotation(pitch, yaw, roll);
                }

            }
        }
    }

    private static void ExtractFaceRotationInDegrees(Windows.Kinect.Vector4 rotQuaternion,
        out int pitch, out int yaw, out int roll)
    {
        float x = rotQuaternion.X;
        float y = rotQuaternion.Y;
        float z = rotQuaternion.Z;
        float w = rotQuaternion.W;

        // convert face rotation quaternion to Euler angles in degrees
        double yawD, pitchD, rollD;
        pitchD = Mathf.Atan2(2 * ((y * z) + (w * x)), (w * w) - (x * x) - (y * y) + (z * z)) / Mathf.PI * 180.0;
        yawD = Mathf.Asin(2 * ((w * y) - (x * z))) / Mathf.PI * 180.0;
        rollD = Mathf.Atan2(2 * ((x * y) + (w * z)), (w * w) + (x * x) - (y * y) - (z * z)) / Mathf.PI * 180.0;

        // clamp the values to a multiple of the specified increment to control the refresh rate
        float increment = FaceRotationIncrementInDegrees;
        pitch = (int)(Mathf.Floor((float)(pitchD + ((increment / 2.0) * (pitchD > 0 ? 1.0 : -1.0))) / increment) * increment);
        yaw = (int)(Mathf.Floor((float)(yawD + ((increment / 2.0) * (yawD > 0 ? 1.0 : -1.0))) / increment) * increment);
        roll = (int)(Mathf.Floor((float)(rollD + ((increment / 2.0) * (rollD > 0 ? 1.0 : -1.0))) / increment) * increment);

    }
}
