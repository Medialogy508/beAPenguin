using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyContainer : MonoBehaviour {

	public ulong trackingId;
	public BodyPart handLeft, handRight, head, footLeft, footRight;

	public Dictionary<string, Transform> parts = new Dictionary<string, Transform>();
	//public List<BodyPart> parts = new List<BodyPart>();

	// Putting the parts into the dictionary
	private void Start() {
		parts["handLeft"] = handLeft.transform;
		parts["handRight"] = handRight.transform;
		parts["head"] = head.transform;
		parts["footLeft"] = footLeft.transform;
		parts["footRight"] = footRight.transform;
	}

}
