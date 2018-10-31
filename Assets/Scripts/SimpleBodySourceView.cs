using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

//THIS IS FROM: https://www.youtube.com/watch?v=B7T0XTNk-Vg&index=2&list=PLmc6GPFDyfw-gF4aGw4Etgo0hJSWQcrYQ

public class SimpleBodySourceView : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    public List<Transform> transforms = new List<Transform>();
    public List<Vector3> positions = new List<Vector3>();

    //MODIFICATION public Vector3 offset = new Vector3(0, 0, 0);

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();

    private List<JointType> _joints = new List<JointType>
    {
       JointType.HandLeft,
       JointType.HandRight,
       JointType.Head,
       JointType.FootLeft,
       JointType.FootRight
    };

    private void Update()
    {
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
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                //Destroy body objet
                Destroy(mBodies[trackingId]);
                
                transforms.Remove(mBodies[trackingId].transform);
                //Remove from list
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        foreach(var body in data)
        {
            //if no body, skip
            if(body == null)
            {
                continue;
            }

            if(body.IsTracked)
            {
                //if body isn't tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId)) {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    transforms.Add(mBodies[body.TrackingId].transform);
                }

                //Update positions
                UpdateBodyObject(body, mBodies[body.TrackingId]);
                //UpdateChildBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }

    public List<Transform> GetTransforms() {
        return transforms;
    }

    private GameObject CreateBodyObject(ulong id)
    {
        //Create body parent
        GameObject body = new GameObject("Body:" + id);
        //body.tag = "Target";

        int counter = 0;

        //Create joints
        foreach (JointType joint in _joints)
        {
            //Create object
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.tag = joint.ToString();
            newJoint.name = joint.ToString();

            //Parent to body
            newJoint.transform.parent = body.transform;

            
            positions.Add(new Vector3(0,1,0));
            //MODIFICATION //positions.Add(offset);

            counter++;
        }

        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        int counter = 0;
        //Update joints
        foreach(JointType _joint in _joints)
        {
            //Get new target position
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            //targetPosition.z = 0;

            //Get joint, set new position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;

            positions[counter] = targetPosition;

            counter++;
        }
    }

    public Vector3 GetHeadPosition(int index)
    {
        //foreach (ulong trackingId in knownIds)
        {
            //return mBodies[trackingId].gameObject.transform.position;
        }
        return positions[index];
    }

    /* This was an attempt to make it more smooth
    private void UpdateChildBodyObject(Body body, GameObject bodyObject)
    {
        //Update joints
        foreach (JointType _joint in _joints)
        {
            //Get new target position
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0;

            //Get joint, set new position
            Transform jointObject = bodyObject.transform.GetChild(0).Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }
    */

    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
