using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSmoother : MonoBehaviour
{
    public Transform mHandMesh;

    private void Update()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 1.0f);
    }

}
