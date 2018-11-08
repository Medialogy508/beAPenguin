using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightManager : MonoBehaviour {

	Penguin penguin;

	BodyPartManager bodyPartManager;

	NavManager navManager;

	public float? height = null;

	bool highEnough;

	public GameObject childTrans, parentTrans;

	// Use this for initialization
	void Start () {
		penguin = GetComponent<Penguin>();
		bodyPartManager = GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>();
		navManager = GetComponent<NavManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(height != null) {
			if (height > bodyPartManager.GetChildHeight()) {
				highEnough = true;
				navManager.SetBaseOffset(2f);
			} else {
				highEnough = false;
				navManager.SetBaseOffset(1.5f);
			}
		}
		if(penguin.trackingId != null) {
			IsChild(highEnough);
		}
	}

	public void GetABody() {
		SetHeight((float) bodyPartManager.GetHeadHeight((ulong) penguin.trackingId));
	}

	public void SetHeight(float newHeight) {
		print(height + " -> " + newHeight);
		height = newHeight;
	}

	public void NullHeight() {
		height = null;
	}

	public void IsChild(bool isChild) {
        childTrans.SetActive(!isChild);
        parentTrans.SetActive(isChild);
    }
}
