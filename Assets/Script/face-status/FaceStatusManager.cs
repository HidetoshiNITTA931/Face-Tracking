using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.Face;

public class FaceStatusManager : MonoBehaviour
{
    private KinectSensor kinectSensor;
    
    private int bodyCount;
    private Body[] bodies;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private FaceFrameResult[] faceFrameResults;
    private BodySourceManager bodySourceManager;
    public GameObject bodyManager;
    public GameObject faceDot;
    
    private const float FaceRotationIncrementInDegrees = 5.0f;
    private GameObject faceStatus;
    private FaceStatusWriter statusWriter;


    void Start()
    {
        // one sensor is currently supported
        kinectSensor = KinectSensor.GetDefault();

        // set the maximum number of bodies that would be tracked by Kinect
        bodyCount = kinectSensor.BodyFrameSource.BodyCount;

        // allocate storage to store body objects
        bodies = new Body[bodyCount];

        // specify the required face frame results
        FaceFrameFeatures faceFrameFeatures =
            FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.BoundingBoxInInfraredSpace
                | FaceFrameFeatures.PointsInInfraredSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.MouthOpen;

        // create a face frame source + reader to track each face in the FOV
        faceFrameSources = new FaceFrameSource[bodyCount];
        faceFrameReaders = new FaceFrameReader[bodyCount];
        for (int i = 0; i < bodyCount; i++)
        {
            // create the face frame source with the required face frame features and an initial tracking Id of 0
            faceFrameSources[i] = FaceFrameSource.Create(kinectSensor, 0, faceFrameFeatures);

            // open the corresponding reader
            faceFrameReaders[i] = faceFrameSources[i].OpenReader();
        }

        bodySourceManager = bodyManager.GetComponent<BodySourceManager>();

        // FaceStatusオブジェクトを取り込む
        faceStatus = GameObject.Find("FaceStatus");
        // FaceStatusオブジェクトからFaceStatusWriterクラスを取り込む
        statusWriter = faceStatus.GetComponent<FaceStatusWriter>();
    }

    void LateUpdate()
    {   
        bodies = bodySourceManager.GetData();
        if (bodies == null)
        {
            Debug.Log("No Bodies");
            return;
        }

        // iterate through each body and update face source
        for (int i = 0; i < bodyCount; i++)
        {
            // check if a valid face is tracked in this face source
            if (faceFrameSources[i].IsTrackingIdValid)
            {
                using (FaceFrame frame = faceFrameReaders[i].AcquireLatestFrame())
                {
                    if (frame != null)
                    {
                        if (frame.TrackingId == 0)
                        {
                            continue;
                        }

                        // do something with result
                        var result = frame.FaceFrameResult;
                        var facePoints = result.FacePointsInColorSpace;
                        foreach (Point point in facePoints.Values)
                        {
                            // point = (0, 0)の場合があるのでそれは除く
                            if ((int)point.X != 0)
                            {
                                generatePoint(point);
                                var eyeLeftClosed = result.FaceProperties[FaceProperty.LeftEyeClosed];
                                var eyeRightClosed = result.FaceProperties[FaceProperty.RightEyeClosed];
                                var mouthOpen = result.FaceProperties[FaceProperty.MouthOpen];
                                var rotationOriant = result.FaceRotationQuaternion;
                                int pitch, yaw, roll;
                                ExtractFaceRotationInDegrees(rotationOriant, out pitch, out yaw, out roll);

                                // Write face status
                                statusWriter.EyeLeftClose = eyeLeftClosed.ToString();
                                statusWriter.EyeRightClose = eyeRightClosed.ToString();
                                statusWriter.MouthOpen = mouthOpen.ToString();
                                statusWriter.Pitch = pitch;
                                statusWriter.Yaw = yaw;
                                statusWriter.Roll = roll;
                                statusWriter.Write();

                            }
                            
                        }
                    }
                }
            }
            else
            {
                // check if the corresponding body is tracked 
                if (bodies[i].IsTracked)
                {
                    // update the face frame source to track this body
                    faceFrameSources[i].TrackingId = bodies[i].TrackingId;
                }

            }
        }

    }

    void generatePoint(Point point)
    {
        float xx = (point.X / 100) - 9.6f;
        float yy = 5.4f - (point.Y / 100);
        GameObject newFaceDot = Instantiate(faceDot, new Vector3(xx, yy, 0.5f), Quaternion.identity) as GameObject;

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