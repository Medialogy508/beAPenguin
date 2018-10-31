using System.Collections;
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
	
	// Update is called once per frame
	void Update () {

	}

	public void AssignPenguinIndex(ulong id) {
		foreach(GameObject obj in penguins) {
			Penguin tempPenguin = obj.GetComponent<Penguin>();
			if(tempPenguin.trackingId == null) {
				tempPenguin.trackingId = id;
				obj.transform.name = "Penguin : " + id;
				print(obj.transform.name);
				return;
			}
		}
	}

	public void RemovePenguinIndex(ulong id) {
		foreach(GameObject obj in penguins) {
			Penguin tempPenguin = obj.GetComponent<Penguin>();
			if(tempPenguin.trackingId == id) {
				tempPenguin.trackingId = null;
				obj.transform.name = "Penguin : " + "Nobody";
				return;
			}
		}
	}

	public Transform GetPart(string jointName, ulong id) {
		Dictionary<ulong, Transform> tempTransforms = bodyView.GetTransforms();

		List<ulong> transformIds = new List<ulong>(tempTransforms.Keys);

        foreach(ulong trackingId in transformIds) {
			if(transformIds.Contains(id)) {
				print("yay " + id);
			}
			//print(tempTransforms[trackingId].GetComponent<BodyContainer>().trackingId);
		}
		return null;
	}

	public float GetHeadHeight(ulong id) {
		return GetPart("Head", id).position.y;
	}

	public float GetChildHeight() {
		return childHeight;
	}

	public int GetBodyAmount() {
		return bodyView.GetTransforms().Count;
	}
}
