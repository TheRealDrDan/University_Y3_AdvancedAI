using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot_AOE : MonoBehaviour
{
    public float damageRadius = 20f;
    public int element;

    public void Initiate(int _element)
    {
        element = _element;
    }

    private void Start()
    {
        //CHECK FOR ALL COLLIDERS WITH THE DAMAGE RADIUS
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider item in hitColliders)
        {
            //IF THE COLLIDER'S GAMEOBJECT HAS TAG "WIZARD" THEN INFLICT ELEMENT-BASED DAMAGE
            if(item.gameObject.tag == "Wizard")
            {
                Wizard_Agent targetScript = item.gameObject.GetComponent<Wizard_Agent>();
                if (targetScript != null)
                {                    
                    targetScript.InflictDamage(element, 200f);
                }
            }
        }
        //ALERT THE WIZARD_MANAGER SCRIPT OF THE SOUND
        Wizard_Manager.wm.AlertSound(transform);
    }
}
