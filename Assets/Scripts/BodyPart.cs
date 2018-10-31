using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart {

	[SerializeField]
	public Transform transform;

	[SerializeField]
	public string name;

	public BodyPart(string newName, Transform newTransform) {
		name = newName;
		transform = newTransform;
	}
}
