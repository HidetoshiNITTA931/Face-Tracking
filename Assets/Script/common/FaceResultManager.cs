using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.Face;


public class FaceResultManager : MonoBehaviour
{
    private KinectSensor kinectSensor;

    private int bodyCount;
    private Body[] bodies;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private FaceFrameResult[] faceFrameResults;
    private BodySourceManager bodySourceManager;
    public GameObject bodyManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // one sensor is currently supported
        kinectSensor = KinectSensor.GetDefault();

        // set the maximum number of bodies that would be tracked by Kinect
        bodyCount = kinectSensor.BodyFrameSource.BodyCount;
        Debug.LogFormat("On FaceResult Manager body count = {0}", bodyCount);

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
        faceFrameResults = new FaceFrameResult[bodyCount];

        for (int i = 0; i < bodyCount; i++)
        {
            // create the face frame source with the required face frame features and an initial tracking Id of 0
            faceFrameSources[i] = FaceFrameSource.Create(kinectSensor, 0, faceFrameFeatures);

            // open the corresponding reader
            faceFrameReaders[i] = faceFrameSources[i].OpenReader();
        }

        bodySourceManager = bodyManager.GetComponent<BodySourceManager>();
    }

    public int GetBodyCount()
    {
        return bodyCount;
    }

    public FaceFrameResult [] GetFaceFrameResults()
    {
        return faceFrameResults;

    }

    // Update is called once per frame
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
                        faceFrameResults[i] = frame.FaceFrameResult;
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
}
