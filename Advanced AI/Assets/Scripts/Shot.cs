using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public GameObject explosion_Prefab;

    private float speed = 20f;
    private int element;
   
    void Update()
    {
        //IF THE TARGET DIES BEFORE THE SHOT REACHES IT, THEN SELF-DESTRUCT
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }                    
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            //ONCE THE PROJECTILE "HITS" THE TARGET, CHECK FOR THE WIZARD SCRIPT AS VALIDATION
            Wizard_Agent targetScript = target.gameObject.GetComponent<Wizard_Agent>();
            if(targetScript != null)
            {
                //IF THE SCRIPT IS PRESENT THEN PASS THE DAMAGE VALUE OF THE PROJECTILE AS WELL AS ITS ELEMENT.
                //THE ELEMENT IS REQUIRED SO THAT THE WIZARD SCRIPT CAN DECIDE IF THE ELEMENT CAN BYBASS ANY PROTECTION BUBBLES.
                Debug.LogWarning("No 'Wizard_Agent' Script attached to Target");
                targetScript.InflictDamage(element, 150f);
            }                             
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        //WHEN THE PROJECTILE IS DESTROYED, CREATE THE ELEMENT EXPLOSION EFFECT.
        if (explosion_Prefab != null)
        {
            GameObject explosion = Instantiate(explosion_Prefab, transform.position, Quaternion.identity);
            Destroy(explosion, 0.4f);
        }
    }

    /// <summary>
    /// Called once an object with Shot.cs is created. Sets the projectiles target and element.
    /// </summary>
    /// <param name="_target"></param>
    /// Intended target transform.
    /// <param name="_element"></param>
    /// Element of the projectile
    public void Initiate(Transform _target, int _element)
    {
        target = _target;
        element = _element;
    }
}
