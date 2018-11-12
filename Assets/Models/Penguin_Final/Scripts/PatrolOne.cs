using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolOne : MonoBehaviour {

    public List<Vector3> Positions;
    public Transform posHolder;
    int posIteration = 0;
    bool canMoveAgain = true;
    public Vector2 idlingTime;

    NavMeshAgent agent;

    void Start () {
        agent = GetComponent<NavMeshAgent>();
        Positions = new List<Vector3>();
        for (int i = 0; i < posHolder.childCount; i++){
            Positions.Add(posHolder.GetChild(i).position);
        }
	}
	
	void Update () {
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
            agent.destination = new Vector3(Positions[index].x, transform.position.y, Positions[index].z);
    }

    void CanMoveAgain() {
        canMoveAgain = true;
    }
}