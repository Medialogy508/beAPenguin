using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class AV_cameraScript : MonoBehaviour {

	public List<Camera> cameras = new List<Camera>();
	VideoPlayer videoPlayer;
	const int QSAMPLES = 128;
		const float REFVAL = 0.1f;  // RMS for 0 dB

	// Use this for initialization
	void Start () {
		videoPlayer = gameObject.GetComponent<VideoPlayer>();
		videoPlayer.Play();

		
 
		
	}

	float GetAudioDb() {
		float[] samples = new float[QSAMPLES];
 
		videoPlayer.GetComponent<AudioSource>().GetOutputData(samples, 0);
		
		float sqrSum = 0.0f;
		
		int i = QSAMPLES; while (i --> 0) {
		
			sqrSum += samples[i] * samples[i];
		}
		
		float rms = Mathf.Sqrt(sqrSum/QSAMPLES); // rms value 0-1
		float dbv = 20.0f*Mathf.Log10(rms/REFVAL);
		

		return ((dbv + 40)/60) * 100;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(GetAudioDb() > 37f) {
			foreach (var penguin in GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>().GetAllPenguins()) {
				penguin.GetComponent<HeightManager>().parent.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, GetAudioDb());
          		
			}
		} else {
			foreach (var penguin in GameObject.FindGameObjectWithTag("BodyPartManager").GetComponent<BodyPartManager>().GetAllPenguins()) {
				penguin.GetComponent<HeightManager>().parent.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
          		
			}
		}
		
		//print(samples[videoPlayer.GetComponent<AudioSource>().clip.samples]);


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
