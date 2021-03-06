﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolOne : MonoBehaviour {

    public List<Vector3> Positions;
    public Transform posHolder;
    int posIteration = 0;
    bool canMoveAgain = true;

    public bool shouldMove = true;
    public Vector2 idlingTime;
    float velo;
    Animator animator;

    NavMeshAgent agent;

    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Random.Range(0.5f, 4);

        animator = GetComponent<Animator>();
        Positions = new List<Vector3>();
        for (int i = 0; i < posHolder.childCount; i++){
            Positions.Add(posHolder.GetChild(i).position);
        }
	}
	
	void Update () {
        velo = agent.velocity.magnitude;
        idleOrNot(velo); 
        if (posIteration > Positions.Count)
        {
            posIteration = 0;
        }

        if (canMoveAgain)
        {
            if (Vector3.Distance(transform.position, Positions[posIteration]) > 7f)
            {
                patrolling(posIteration);
            } else {
                posIteration++;
                posIteration = Random.Range(0, Positions.Count);
                canMoveAgain = false;
                Invoke("CanMoveAgain", Random.Range(idlingTime.x, idlingTime.y));
            }
        }
    }

    void patrolling(int index){
        if(shouldMove)
            agent.destination = new Vector3(Positions[index].x, transform.position.y, Positions[index].z);
    }

    void CanMoveAgain() {
        canMoveAgain = true;
    }

    void idleOrNot(float veloTemp){
        if(veloTemp < 0.1f || !shouldMove)  {
            animator.SetBool("idle",true);
        } else {
            animator.SetBool("idle", false);
        }
    }
}