using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{    
    private GameObject followingObject;

    public int index = 0;
    public Vector3 offset = Vector3.zero;

    private void Start()
    {
        followingObject = Wizard_Manager.wm.Wizards[index].gameObject;
    }

    private void Update()
    {
        if (Wizard_Manager.wm.Wizards.Count == 0)
            return;       

        //WHEN THE USER CLICKS EITHER LEFT OR RIGHT MOUSE BUTTON< SWITCH CAMERA POSITION TO LOOK AT ANOTHER WIZARD

        if (Input.GetMouseButtonDown(1))
        {
            index--;
            if (index < 0)
                index = Wizard_Manager.wm.Wizards.Count - 1;
        }
        if (Input.GetMouseButtonDown(0))
        {
            index++;
            if (index >= Wizard_Manager.wm.Wizards.Count)
                index = 0;
        }
        if(index >= Wizard_Manager.wm.Wizards.Count)
            index = Wizard_Manager.wm.Wizards.Count - 1;
        followingObject = Wizard_Manager.wm.Wizards[index].gameObject;
    }

    void FixedUpdate()
    {
        if (Wizard_Manager.wm.Wizards.Count == 0)
            return;
        if (followingObject == null)
        {
            followingObject = Wizard_Manager.wm.Wizards[Random.Range(0, Wizard_Manager.wm.Wizards.Count-1)].gameObject;
        }
        this.transform.position = Vector3.Lerp(transform.position, new Vector3(followingObject.transform.position.x + offset.x, transform.position.y + offset.y, followingObject.transform.position.z + offset.z), 0.5f);
    }
}
