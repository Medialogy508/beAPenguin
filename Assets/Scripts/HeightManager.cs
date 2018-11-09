using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
public class HeightManager : MonoBehaviour {

	Penguin penguin;

	BodyPartManager bodyPartManager;

	NavManager navManager;

	public float? height = null;

	public float heightDiffThresh;

	float oldHeight = 100;
	bool canJump = true;
	bool jumping = false;

	bool highEnough;

	public GameObject child, parent;


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
			GetHeightDifference();
			if(jumping) {
				//TODO SET BONE POSITION INSTEAD
				child.transform.position = new Vector3(child.transform.position.x, (bodyPartManager.GetHeadHeight((ulong) penguin.trackingId) * 1f)-7f, child.transform.position.z);
				parent.transform.position = new Vector3(parent.transform.position.x, (bodyPartManager.GetHeadHeight((ulong) penguin.trackingId) * 1f)-7f, parent.transform.position.z);
			} else {
				child.transform.localPosition = new Vector3(child.transform.localPosition.x, -0.12f, child.transform.localPosition.z);
				parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, -0.12f, parent.transform.localPosition.z);
			}
		}
	}

	public void GetHeightDifference() {
		float newHeight = bodyPartManager.GetHeadHeight((ulong) penguin.trackingId);
		if(oldHeight - newHeight < heightDiffThresh && canJump) {
			canJump = false;
			jumping = true;
			print("Jump");
			Invoke("CanJumpAgain", .5f);
		}
		
		oldHeight = newHeight;
	}

	public void CanJumpAgain() {
		jumping = false;
		canJump = true;
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
        child.SetActive(!isChild);
        parent.SetActive(isChild);
    }
}
