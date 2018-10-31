using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyManager : MonoBehaviour
{
    public GameObject child, parent;

    public Transform left, right;

    public float zOffset = -10;

    public void IsChild(bool isChild) {
        child.SetActive(isChild);
        parent.SetActive(!isChild);
    }

    public void SetLeftHandPosition(bool booleft, Vector3 pos) {
        if(booleft)
        {
            left.position = new Vector3(pos.x, pos.y, zOffset);
        }
        else
        {
            right.position = new Vector3(pos.x, pos.y, zOffset);
        }
    }
}
