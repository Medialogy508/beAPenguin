using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavManager : MonoBehaviour {

	BodyPartManager bodyPartManager;

	NavMeshAgent agent;

	Penguin penguin;

	public Transform goal;

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
			agent.destination = goal.position;
		} else if(moveGoal == null) {
			moveGoal = new GameObject("Move Goal : " + penguin.trackingId);
		} else if(penguin.trackingId == null) {
			Destroy(moveGoal);
			moveGoal = null;
		}
	}

	public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
           
            randomDirection += origin;
           
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
           
            return navHit.position;
        }
}
