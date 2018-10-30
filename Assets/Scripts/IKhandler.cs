using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKhandler : MonoBehaviour
{
    Animator anim;

    public float ikWeight = 1;

    public Transform leftArmIKTarget, rightArmIKTarget, leftLegIKTarget, rightLegIKTarget;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {

		
	}

    private void OnAnimatorIK() {
        // Arm weights
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

        // Leg weights
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);

        // Arm goals
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftArmIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightArmIKTarget.position);

        // Leg goals
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftLegIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightLegIKTarget.position);
    }
}
