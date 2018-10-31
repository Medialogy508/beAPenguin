using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartManager : MonoBehaviour {

	public SimpleBodySourceView bodyView;

	public float childHeight;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public Transform GetPart(string jointName, int index) {
		
		return null;
	}

	public float GetHeadHeight(int index) {
		return GetPart("Head", index).position.y;
	}

	public float GetChildHeight() {
		return childHeight;
	}

	public int GetBodyAmount() {
		return 0;
	}
}
