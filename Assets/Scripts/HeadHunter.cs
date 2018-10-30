using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunter : MonoBehaviour
{
    public GameObject[] heads, handsL, handsR, legsL, legsR;

	// Update is called once per frame
	void Update () {
        heads = GameObject.FindGameObjectsWithTag("Head");
        handsL = GameObject.FindGameObjectsWithTag("HandRight");
        handsR = GameObject.FindGameObjectsWithTag("HandLeft");

        legsL = GameObject.FindGameObjectsWithTag("FootRight");
        legsR = GameObject.FindGameObjectsWithTag("FootLeft");

    }

    public Vector3[] GetPositions()
    {
        if (heads == null)
            throw new System.Exception("We have no heads in scene");

        Vector3[] positions = new Vector3[heads.Length];

        for(int i = 0; i < positions.Length; i++)
        {
            if(heads[i] == null)
            {
                positions[i] = new Vector3(-100, -100, 0);
            }
            else
                positions[i] = heads[i].transform.position;
        }

        return positions;
    }

    public Vector3[] GetLeftHandPositions()
    { 
        if (handsL == null)
            throw new System.Exception("We have no left hands in scene");

        Vector3[] positions = new Vector3[handsL.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            if (handsL[i] == null)
            {
                positions[i] = new Vector3(-100, -100, 0);
            }
            else
                positions[i] = handsL[i].transform.position;
        }

        return positions;
    }


    public Vector3[] GetRightHandPositions()
    {
        if (handsR == null)
            throw new System.Exception("We have no left hands in scene");

        Vector3[] positions = new Vector3[handsR.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            if (handsR[i] == null)
            {
                positions[i] = new Vector3(-100, -100, 0);
            }
            else
                positions[i] = handsR[i].transform.position;
        }

        return positions;
    }

    public Vector3 GetPosition(int num)
    {
        if (heads == null)
            throw new System.Exception("We have no heads in scene");
        return heads[num].transform.position;
    }
}
