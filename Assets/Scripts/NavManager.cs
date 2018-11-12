using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text;
using System.IO;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class NavManager : MonoBehaviour {

	BodyPartManager bodyPartManager;

	public NavMeshAgent agent;

	Penguin penguin;

	public Transform goal;

	public Animator anim;

	GameObject moveGoal;
	HeightManager heightManager;

	public float timeEngaged = 0f;
	// Use this for initialization
	void Start () {
		penguin = GetComponent<Penguin>();
		bodyPartManager = GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>();
		agent = GetComponent<NavMeshAgent>();
		heightManager = GetComponent<HeightManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(penguin.trackingId != null && moveGoal != null) {
			//Resetting rotation
			this.transform.rotation = Quaternion.Euler(-4.075f, 0, 0);
			//Getting average x of feet and moving penguin to that
			float averageX = (bodyPartManager.GetPart("footLeft",(ulong) penguin.trackingId).position.x + bodyPartManager.GetPart("footRight",(ulong) penguin.trackingId).position.x)/2;
			Vector3 newMoveGoalPos = new Vector3(averageX, bodyPartManager.GetPart("spineBase",(ulong) penguin.trackingId).position.y, bodyPartManager.GetPart("spineBase",(ulong) penguin.trackingId).position.z);
			moveGoal.transform.position = newMoveGoalPos;
			agent.destination = moveGoal.transform.position;
			timeEngaged += Time.deltaTime;
		} else if(penguin.trackingId != null && moveGoal == null) {
			moveGoal = new GameObject("Move Goal : " + penguin.trackingId);			
			return;
		} else if(penguin.trackingId == null && moveGoal != null) {
			moveGoal.transform.position = new Vector3(-49.38345f, -2.180126f, -27.5f);
			heightManager.child.transform.localPosition = new Vector3(heightManager.child.transform.localPosition.x, -1.12f, heightManager.child.transform.localPosition.z);
			heightManager.parent.transform.localPosition = new Vector3(heightManager.parent.transform.localPosition.x, -1.12f, heightManager.parent.transform.localPosition.z);
			agent.destination = moveGoal.transform.position;
			this.transform.LookAt(new Vector3(-agent.destination.x, agent.destination.y, agent.destination.z), Vector3.up);
			Destroy(moveGoal);
			moveGoal = null;
			if (timeEngaged > 10) {
				WriteToFile(timeEngaged);
			}
		}		
	}

	public void WriteToFile(float time) {
		//"Date","Time","Height (A: 0, C: 1)","Time used"
		
		File.AppendAllText("Statistic.txt", '"' +  DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + '"' + ",");
		File.AppendAllText("Statistic.txt", '"' + "" +  DateTime.Now.TimeOfDay + '"' + ",");
		if (heightManager.height > bodyPartManager.GetChildHeight()) {
			//Adult is a zero
			File.AppendAllText("Statistic.txt",'"' +  "0" + '"' + ",");
		} else {
			// Children is a one
			File.AppendAllText("Statistic.txt",'"' + "1" + '"' + ",");
		}

		File.AppendAllText("Statistic.txt", '"' + timeEngaged.ToString() + '"' + Environment.NewLine);
		timeEngaged = 0f;
	}

	public void SetBaseOffset(float value) {
		agent.baseOffset = value;
	}
}
