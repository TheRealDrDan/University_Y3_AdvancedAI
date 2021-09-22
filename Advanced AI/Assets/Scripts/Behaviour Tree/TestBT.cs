using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBT : MonoBehaviour
{

    BehaviourTree bt = new BehaviourTree();

    public NavMeshAgent agent;
    public GameObject pickupObjectPrefab;
    public float throwPower = 20f;
    [SerializeField]
    private GameObject pobject;
    [SerializeField]
    private Transform throwPoint;
    private bool pickedUpObject = false;


    private bool waiting = false;
    private float currentTime = 0f;
    

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        bt.AddNode(Node.Typing.SEQUENCE, 0);

        bt.AddNode(Node.Typing.LEAF, 1, WalkToObject);
        bt.AddNode(Node.Typing.LEAF, 1, PickUpObject);
        bt.AddNode(Node.Typing.LEAF, 1, LookNewDir);
        bt.AddNode(Node.Typing.LEAF, 1, ThrowObject);
        bt.AddNode(Node.Typing.LEAF, 1, Wait);

        
    }
    
    void Update()
    {        
            bt.Process();
    }
    

    Node.State WalkToObject()
    {
        if (waiting)
            return Node.State.SUCCESS;
        if (pobject == null)
            return Node.State.FAILURE;
        if (pickedUpObject == true)
            return Node.State.FAILURE;
        agent.SetDestination(pobject.transform.position);
        if(Vector3.Distance(transform.position, pobject.transform.position) < 3f)
        {
            return Node.State.SUCCESS;
        }
        return Node.State.RUNNING;
    }

    Node.State PickUpObject()
    {
        if (waiting)
            return Node.State.SUCCESS;
        if (Vector3.Distance(transform.position, pobject.transform.position) < 2f)
        {
            pickedUpObject = true;
            Destroy(pobject);
            return Node.State.SUCCESS;
        }
        return Node.State.FAILURE;
    }


    Node.State LookNewDir()
    {
        if (waiting)
            return Node.State.SUCCESS;
        Vector3 dir = new Vector3(Random.Range(-100f, 100f), 1f, Random.Range(-100f, 100f));
        transform.LookAt(dir);
        return Node.State.SUCCESS;
    }

    Node.State ThrowObject()
    {
        if (waiting)
            return Node.State.SUCCESS; 
        if (pickedUpObject == false)
            return Node.State.FAILURE;

        pobject = Instantiate(pickupObjectPrefab, throwPoint);
        pobject.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * throwPower,ForceMode.Impulse);
        pobject.transform.parent = null;
        pickedUpObject = false;
        
        return Node.State.SUCCESS;
    }


    Node.State Wait()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= 1f)
        {
            waiting = false;
            currentTime = 0;
            return Node.State.SUCCESS;
        }
        else
        {
            waiting = true;
            return Node.State.RUNNING;
        }            
    }
}
