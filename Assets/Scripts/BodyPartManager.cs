using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartManager : MonoBehaviour {

	public SimpleBodySourceView bodyView;

	List<GameObject> penguins = new List<GameObject>();

	public float childHeight;

	// Use this for initialization
	void Start () {
		GameObject[] penguinObjs = GameObject.FindGameObjectsWithTag("Penguin");
		foreach(GameObject obj in penguinObjs) {
			penguins.Add(obj); 
		}
	}
	

	//Assign a tracking id, from a newly found body
	public void AssignPenguinIndex(ulong id) {
		foreach(GameObject obj in penguins) {
			Penguin tempPenguin = obj.GetComponent<Penguin>();
			if(tempPenguin.trackingId == null) {
				tempPenguin.trackingId = id;
				obj.transform.name = "Penguin : " + id;
				obj.GetComponent<HeightManager>().Invoke("GetABody", 0.1f);
				//print(obj.transform.name);
				return;
			}
		}
	}

	//Remove a penguin's tracking id, due to lost body
	public void RemovePenguinIndex(ulong id) {
		// Iterate over the penguin gameobjects
		foreach(GameObject obj in penguins) {
			//the penguin object from the penguin gameobject
			Penguin tempPenguin = obj.GetComponent<Penguin>();
			if(tempPenguin.trackingId == id) {
				tempPenguin.trackingId = null;
				//obj.GetComponent<HeightManager>().NullHeight();
				obj.transform.name = "Penguin : " + "Nobody";
				return;
			}
		}
	}

	//Get a part from the a specfic penguin, with the name and tracking id
	public Transform GetPart(string jointName, ulong id) {
		//The body transforms
		Dictionary<ulong, Transform> tempTransforms = bodyView.GetTransforms();

		// Transform dictionary keys
		List<ulong> transformIds = new List<ulong>(tempTransforms.Keys);

		//Returning the part from the dictionary
        foreach(ulong trackingId in transformIds) {
			if(transformIds.Contains(id)) {
				try {
					return tempTransforms[id].GetComponent<BodyContainer>().parts[jointName];
				} catch(KeyNotFoundException) {
					print("Can't find key, did you spell that shit correct? " + jointName);
				}
				
			}
		}
		return null;
	}

	public float GetHeadHeight(ulong id) {
		try {
			print((Mathf.Abs(GetPart("spineBase", id).position.z + 34.98f) / 6));
			return (GetPart("head", id).position.y+2) + (Mathf.Abs(GetPart("spineBase", id).position.z + 34.98f) / 6);
		} catch (NullReferenceException e) {
			Debug.LogError(e.Message);
			return 0;
		}
	}

	public float GetChildHeight() {
		return childHeight;
	}

	public int GetBodyAmount() {
		return bodyView.GetTransforms().Count;
	}
}
