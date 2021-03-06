﻿using System.Collections;
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
	
	[SerializeField]
	public List<float> oldHeights = new List<float>();
	bool canJump = true;
	bool jumping = false;

	bool highEnough;

	public GameObject child, parent;

	public int jumpCount = 0;


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
				navManager.SetBaseOffset(1.5f);
			} else {
				highEnough = false;
				navManager.SetBaseOffset(0.5f);
			}
		}
		if(penguin.trackingId != null) {
			IsChild(highEnough);
			GetHeightDifference();
			// Jump trigger
			if(jumping) {
				jumping = false;
				
				//parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, (bodyPartManager.GetHeadHeight((ulong) penguin.trackingId) * 0.55f) * 1f, parent.transform.localPosition.z);
				child.GetComponent<Animator>().SetTrigger("Jump");
				parent.GetComponent<Animator>().SetTrigger("Jump");
			}
		}
	}

	public void GetHeightDifference() {
		float newHeight = bodyPartManager.GetHeadHeight((ulong) penguin.trackingId);

		oldHeights.Add(newHeight);

		if(oldHeights.Capacity > 3) {
			oldHeights.RemoveAt(0);
		}
		float heightSum = 0;
		foreach(float height in oldHeights) {
			heightSum += height;
		}

		newHeight = heightSum/oldHeights.Capacity-1;

        print(newHeight);

		if(oldHeight - newHeight < heightDiffThresh && canJump) {
			canJump = false;
			jumping = true;
			print("Jump");
            jumpCount++;
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
