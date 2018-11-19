using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour {
	
	BodyPartManager bodyPartManager;
	Penguin penguin;

	public Animator anim;
	
	NavManager navManager;

    public float ikWeight = 1;

	HeightManager heightManager;

    // Use this for initialization
    void Start () {
		bodyPartManager = GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>();
        anim = GetComponent<Animator>();
		penguin = GetComponentInParent<Penguin>();
		navManager = GetComponentInParent<NavManager>();
		heightManager = GetComponentInParent<HeightManager>();
	}
	
	// Update is called once per frame
	void Update () {
		//print(navManager.agent.velocity.magnitude);
		anim.SetFloat("velocity", navManager.agent.velocity.magnitude);
		
	}

    private void OnAnimatorIK() {
		if(penguin.trackingId != null) {
			// Arm weights
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
			anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

			// Rotate body based on shoulder location
			// Mutliplier to scale rotation based on distance from center on x axis
			float rotationDistanceMultiplier = (Mathf.Abs(bodyPartManager.GetPart("spineBase", (ulong) penguin.trackingId).position.x * 1f) + 1);
			 
			float absShoulderX = -Mathf.Abs(bodyPartManager.GetPart("shoulderLeft", (ulong) penguin.trackingId).position.x) - Mathf.Abs(bodyPartManager.GetPart("shoulderRight", (ulong) penguin.trackingId).position.x);
			float absShoulderZ = Mathf.Abs(bodyPartManager.GetPart("shoulderLeft", (ulong) penguin.trackingId).position.z  - bodyPartManager.GetPart("shoulderRight", (ulong) penguin.trackingId).position.z) * rotationDistanceMultiplier;

			float spineAngleRadians = Mathf.Atan(absShoulderX/absShoulderZ);

			float spineAngle;

		

			if(bodyPartManager.GetPart("shoulderLeft", (ulong) penguin.trackingId).position.z > bodyPartManager.GetPart("shoulderRight", (ulong) penguin.trackingId).position.z) {
				spineAngle = (90 + ((spineAngleRadians) * (180.0f / Mathf.PI)));
			} else {
				spineAngle = (270 - ((spineAngleRadians) * (180.0f / Mathf.PI)));
			}

			anim.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.AngleAxis(spineAngle, Vector3.up));
			
			Vector3 spineRotationDir = bodyPartManager.GetPart("head", (ulong) penguin.trackingId).position - bodyPartManager.GetPart("spineBase", (ulong) penguin.trackingId).position;


			if(heightManager.height != null) {
				spineRotationDir.x *= -1;
				spineRotationDir.z *= -1;
			}
			
			Quaternion chestRot = anim.GetBoneTransform(HumanBodyBones.Chest).localRotation;

			anim.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.FromToRotation(Quaternion.AngleAxis(spineAngle, Vector3.up).normalized.eulerAngles, (spineRotationDir)));
			

			// Leg weights
			anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight/8);
			anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight/8);


			// TODO scale arm joints relative to average of feet.
			// Arm goals
			anim.SetIKPosition(AvatarIKGoal.LeftHand, bodyPartManager.GetPart("handRight", (ulong) penguin.trackingId).position);
			anim.SetIKPosition(AvatarIKGoal.RightHand, bodyPartManager.GetPart("handLeft", (ulong) penguin.trackingId).position);

			// Leg goals
			anim.SetIKPosition(AvatarIKGoal.LeftFoot, bodyPartManager.GetPart("footRight", (ulong) penguin.trackingId).position);
			anim.SetIKPosition(AvatarIKGoal.RightFoot, bodyPartManager.GetPart("footLeft", (ulong) penguin.trackingId).position);
		}
    }
}
