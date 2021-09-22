using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool useMouse = false;

    private void Start()
    {
        if (agent == null)
            agent = gameObject.GetComponent<NavMeshAgent>();
       if(!useMouse)
        StartCoroutine("newPosition");
    }

    void Update()
    {
        if (!useMouse)
            return;
        
        //THIS CODE CONVERTS THE MOUSE CLICK POSITION TO A WORLD POSITION AND SETS THE AGENT NAVMESH SYSTEM'S DESTINATION TO IT.

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
        
    }

    IEnumerator newPosition()
    {
        
        //THIS CODE GENERATES A RANDOM VECTOR AND PASSES IT TO THE NAVMESH SYSTEM AS A DESTINATION.

        while (true)
        {
            Vector3 newpos = new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-75f, 75f));
            agent.SetDestination(newpos);
            yield return new WaitForSeconds(Random.Range(1.5f,3.5f));
        }   
    }

}
