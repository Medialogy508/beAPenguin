using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

//THIS IS FROM: https://www.youtube.com/watch?v=B7T0XTNk-Vg&index=2&list=PLmc6GPFDyfw-gF4aGw4Etgo0hJSWQcrYQ

public class SimpleBodySourceView : MonoBehaviour
{

    public float jointPositionScale = 10;
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    public List<Vector3> positions = new List<Vector3>();

    //MODIFICATION public Vector3 offset = new Vector3(0, 0, 0);

    private Dictionary<ulong, Transform> transforms = new Dictionary<ulong, Transform>();

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();

    private List<JointType> _joints = new List<JointType> {
       JointType.HandLeft,
       JointType.HandRight,
       JointType.Head,
       JointType.FootLeft,
       JointType.FootRight,
       JointType.SpineBase,
       JointType.ShoulderLeft,
       JointType.ShoulderRight
    };

    private void Update() {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
            return;

        List<ulong> trackedIds = new List<ulong>();

        foreach(var body in data)
        {
            if (body == null)
                continue;
            if (body.IsTracked) {
                trackedIds.Add(body.TrackingId);
                
            }
        }
        #endregion

        #region Delete Kinect Bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach(ulong trackingId in knownIds) {
            if(!trackedIds.Contains(trackingId)) {
                //Destroy body objet
                Destroy(mBodies[trackingId]);
                DeleteBody(trackingId);
                transforms.Remove(trackingId);
                //Remove from list
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        foreach(var body in data) {
            //if no body, skip
            if(body == null) {
                continue;
            }

            if(body.IsTracked) {
                //if body isn't tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId)) {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    transforms[body.TrackingId] = mBodies[body.TrackingId].transform;
                    //Assign new body to a penguin gameobject
                    NewBody(body.TrackingId);
                }

                //Update positions
                UpdateBodyObject(body, mBodies[body.TrackingId]);
                //UpdateChildBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }

    public Dictionary<ulong, Transform> GetTransforms() {
        return transforms;
    }

    void NewBody(ulong id) {
        GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>().AssignPenguinIndex(id);
    }

    void DeleteBody(ulong id) {
        GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>().RemovePenguinIndex(id);
    }

    private GameObject CreateBodyObject(ulong id) {
        //Create body parent
        GameObject body = new GameObject("Body:" + id);

        //Create joints
        foreach (JointType joint in _joints) {
            //Create object
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.tag = joint.ToString();
            newJoint.name = joint.ToString();

            //Parent to body
            newJoint.transform.parent = body.transform;

            
            positions.Add(new Vector3(0,1,0));

        }

        BodyContainer bodyContainer = body.AddComponent<BodyContainer>() as BodyContainer;

        bodyContainer.trackingId = id;

        // I'm sorry Søren, but this works
        bodyContainer.handLeft = new BodyPart("handLeft", body.transform.GetChild(0));
        bodyContainer.handRight = new BodyPart("handRight", body.transform.GetChild(1));
        bodyContainer.head = new BodyPart("head", body.transform.GetChild(2));
        bodyContainer.footLeft = new BodyPart("footRight", body.transform.GetChild(3));
        bodyContainer.footRight = new BodyPart("footRight", body.transform.GetChild(4));
        bodyContainer.crotch = new BodyPart("spineBase", body.transform.GetChild(5));
        bodyContainer.shoulderLeft = new BodyPart("shoulderLeft", body.transform.GetChild(6));
        bodyContainer.shoulderRight = new BodyPart("shoulderRight", body.transform.GetChild(7));

        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject) {
        int counter = 0;
        //Update joints
        foreach(JointType _joint in _joints) {
            //Get new target position
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            //targetPosition.z = 0;

            //Get joint, set new position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;

            positions[counter] = targetPosition;
            //transforms[counter].position = targetPosition;

            counter++;
        }
    }

    public Vector3 GetHeadPosition(int index) {
       
        return positions[index];
    }

    private Vector3 GetVector3FromJoint(Joint joint) {
        return new Vector3((joint.Position.X * jointPositionScale * 2f), (joint.Position.Y * jointPositionScale)+2, (joint.Position.Z * jointPositionScale) -45);
    }
}
