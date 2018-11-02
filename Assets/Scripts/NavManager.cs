using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavManager : MonoBehaviour {

	BodyPartManager bodyPartManager;

	public NavMeshAgent agent;

	Penguin penguin;

	public Transform goal;

	Animator anim;

	GameObject moveGoal;

	// Use this for initialization
	void Start () {
		penguin = GetComponent<Penguin>();
		bodyPartManager = GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>();
		agent = GetComponent<NavMeshAgent>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if(penguin.trackingId != null && moveGoal != null) {
			//this.transform.LookAt(new Vector3(-Camera.main.transform.position.x, Camera.main.transform.position.y, -Camera.main.transform.position.z), Vector3.up);
			this.transform.rotation = Quaternion.identity;
			moveGoal.transform.position = bodyPartManager.GetPart("spineBase",(ulong) penguin.trackingId).position;
			agent.destination = moveGoal.transform.position;
		} else if(penguin.trackingId != null && moveGoal == null) {
			moveGoal = new GameObject("Move Goal : " + penguin.trackingId);
			return;
		} else if(penguin.trackingId == null && moveGoal != null) {
			moveGoal.transform.position = new Vector3(-49.38345f, -2.180126f, -27.5f);
			agent.destination = moveGoal.transform.position;
			this.transform.LookAt(new Vector3(-agent.destination.x, agent.destination.y, agent.destination.z), Vector3.up);
			Destroy(moveGoal);
			moveGoal = null;
		}		
	}

	public void SetBaseOffset(float value) {
		agent.baseOffset = value;
	}

	public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
           
            randomDirection += origin;
           
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
           
            return navHit.position;
        }
}
