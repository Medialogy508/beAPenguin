using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyContainer : MonoBehaviour {

	public ulong trackingId;
	public BodyPart handLeft, handRight, head, footLeft, footRight, crotch, spineMid, shoulderLeft, shoulderRight;

	public Dictionary<string, Transform> parts = new Dictionary<string, Transform>();

	// Putting the parts into the dictionary
	private void Start() {
		parts["handLeft"] = handLeft.transform;
		parts["handRight"] = handRight.transform;
		parts["head"] = head.transform;
		parts["footLeft"] = footLeft.transform;
		parts["footRight"] = footRight.transform;
		parts["spineBase"] = crotch.transform;
		parts["shoulderLeft"] = shoulderLeft.transform;
		parts["shoulderRight"] = shoulderRight.transform;
	}

}
