using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinManager : MonoBehaviour
{
    public GameObject[] penguins;

    public GameObject manager;
    HeadHunter body;
    public float divide = 2;
    public float zOffset = 5f, xOffset = 0f, yOffset = -5f;
    public float yHandOffset = 5f;

    public float childHeight = 5f;

    public Vector3[] positions, handsL, handsR;

    void Start()
    {
        body = manager.GetComponent<HeadHunter>();
    }

    void Update()
    {
        positions = body.GetPositions();
        handsL = body.GetLeftHandPositions();
        handsR = body.GetRightHandPositions();

        for(int i = 0; i < penguins.Length; i++)
        {
            penguins[i].transform.position = new Vector3(-100, -100, 0);
        }

        if (positions.Length > 0)
        {
            Debug.Log(positions.Length);
            for (int i = 0; i < positions.Length; i++)
            {
                penguins[i].transform.position = new Vector3(xOffset + positions[i].x / divide, yOffset, positions[i].z + zOffset);

                //Check for child height
                penguins[i].GetComponent<FamilyManager>().IsChild(positions[i].y < childHeight);

                //Set left hand position
                penguins[i].GetComponent<FamilyManager>().SetLeftHandPosition(true, new Vector3(handsL[i].x, handsL[i].y + yHandOffset - positions[i].y, handsL[i].z));


                //Set right hand position
                penguins[i].GetComponent<FamilyManager>().SetLeftHandPosition(false, new Vector3(handsR[i].x, handsR[i].y + yHandOffset - positions[i].y, handsR[i].z));

            }
        }


        //(REMOVE UNNEEDED MASKS)



    }


}
