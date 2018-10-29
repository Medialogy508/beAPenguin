using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject[] masks;

    public GameObject manager;
    //public GameObject mask;
    HeadHunter body;
    public float plus = 50;
    public float divide = 2;
    public float HeadPos;
    public float Min2Plus2;
    Vector3[] positions;

	void Start ()
    {
        body = manager.GetComponent<HeadHunter>();
	}
	
	void Update ()
    {
        positions = body.GetPositions();

        //CREATE MASKS

        for (int i = 0; i < masks.Length; i++)
        {
            masks[i].transform.position = new Vector3(-100, -100, 0);
        }


        if (positions.Length > 0)
        {
            for(int i = 0; i < positions.Length; i++)
            {
                masks[i].transform.position = new Vector3(positions[i].x / divide + plus, 0, -7);

                //SET POSITIONS
                //transform.position = new Vector3(body.GetPosition(0).x / divide + plus, transform.position.y, transform.position.z);
            }
        }


        //(REMOVE UNNEEDED MASKS)
                

        
    }


}
