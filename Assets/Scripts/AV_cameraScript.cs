using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AV_cameraScript : MonoBehaviour {

	public List<Camera> cameras = new List<Camera>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1") || Input.GetKeyDown("2") || Input.GetKeyDown("3")) {
			print(int.Parse(Input.inputString) - 1);


			foreach (Camera cam in cameras) {
				if(cam != null)
					cam.enabled = false;
			}

			cameras[int.Parse(Input.inputString) - 1].enabled = true;
		}

		if(Input.GetKeyDown("w")) {
			GameObject[] penguins = GameObject.FindGameObjectsWithTag("Penguin");

			foreach (var penguin in penguins) {
				print(penguin.GetComponent<NavMeshAgent>().areaMask);
				if(penguin.GetComponent<NavMeshAgent>().areaMask == -1) {
					penguin.GetComponent<NavMeshAgent>().areaMask = 8;
				} else {
					penguin.GetComponent<NavMeshAgent>().areaMask = -1;
				}
				
			}
		}
	}
}
