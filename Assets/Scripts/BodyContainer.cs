using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyContainer : MonoBehaviour {

	public ulong trackingId;
	public BodyPart handLeft, handRight, head, footLeft, footRight;

	private Dictionary<ulong, Transform> parts = new Dictionary<ulong, Transform>();
	//public List<BodyPart> parts = new List<BodyPart>();

	private void Start() {
		//parts.Add(handLeft);
		//parts.Add(handRight);
		//parts.Add(head);
		//parts.Add(footLeft);
		//parts.Add(footRight);
	}

}
