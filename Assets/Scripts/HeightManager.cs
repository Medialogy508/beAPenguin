using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightManager : MonoBehaviour {

	Penguin penguin;

	BodyPartManager bodyPartManager;

	// Use this for initialization
	void Start () {
		penguin = GetComponent<Penguin>();
		bodyPartManager = GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(penguin.trackingId != null) {
			print(bodyPartManager.GetHeadHeight((ulong) penguin.trackingId));
		}
	}
}
