using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class JumpAtLedge : MonoBehaviour {

    Animator animator;
    NavMeshAgent agent;
    bool jumpOnce = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
  
        playAnime();
    }
    private void playAnime()
    {
        if (jumpOnce)
        {
            if (agent.isOnOffMeshLink)
            {
                Debug.Log("Succesfully Jumped");
                animator.SetTrigger("Jump");
                jumpOnce = false;
            }
        }
    }
}
