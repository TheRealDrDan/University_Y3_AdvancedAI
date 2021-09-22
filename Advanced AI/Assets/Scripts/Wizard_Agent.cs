using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wizard_Agent : MonoBehaviour
{

    public float maxHealth = 1000f;
    public GameObject sight;
    public float sightRadius = 20f;
    [Range(10f, 360f)]
    public float viewingAngle = 80f;
    public Slider healthBar;

    public GameObject target;
    public int lastHitElement = 0;

    [HideInInspector]
    public Transform lastHeardSound;

    [SerializeField]
    private BehaviourTree bt;
    private Wizard_Manager wm;
    private NavMeshAgent agent;
    private Vector3 currentDestination = Vector3.zero;
    [SerializeField]
    private float health;
    private GameObject defenseEffect;
    private int currentDefenseElement;
    private int currentCastingElement;
    private bool waiting = false;
    private float currentTime = 0f;
    private float lastKnownHealthValue;
    private GameObject wall;
    private GameObject victory;

    //private List<GameObject> visibleEnemies = new List<GameObject>();
    private Dictionary<Collider, float> visibleEnemies = new Dictionary<Collider, float>();

    [SerializeField]
    private GameObject fireshotSmall_prefab;
    [SerializeField]
    private GameObject iceshotSmall_prefab;
    [SerializeField]
    private GameObject lightningshotSmall_prefab;
    [SerializeField]
    private GameObject watershotSmall_prefab;
    [SerializeField]
    private GameObject fireshotLarge_prefab;
    [SerializeField]
    private GameObject iceshotLarge_prefab;
    [SerializeField]
    private GameObject lightningshotLarge_prefab;
    [SerializeField]
    private GameObject watershotLarge_prefab;
    [SerializeField]
    private GameObject firedefense_prefab;
    [SerializeField]
    private GameObject icedefense_prefab;
    [SerializeField]
    private GameObject lightningdefense_prefab;
    [SerializeField]
    private GameObject waterdefense_prefab;
    [SerializeField]
    private GameObject wall_prefab;
    [SerializeField]
    private GameObject healEffect_prefab;
    [SerializeField]
    private GameObject deathEffect_prefab;
    [SerializeField]
    private GameObject largeSpellCastEffect_prefab;
    [SerializeField]
    private GameObject victorySpellEffect_prefab;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wm = Wizard_Manager.wm;
        health = maxHealth;
        lastKnownHealthValue = maxHealth;


        ///THIS SECTION CREATES THE WIZARDS BEHAVIOUR TREE UTILISING THE BehaviourTree.cs FRAMEWORK.
        ///ALL NODES ARE ADDED LEFT-RIGHT
        ///THE NUMBERS COMMENTED ABOVE REPRESENT THE INDEX OF THE MAJOR NODE.
        ///ONLY THE INDEXES OF SEQUENCE AND SELECT NODES ARE STORED SINCE LEAF NODE INDEXES ARE NOT REQUIRED.
        ///PLEASE LOOK AT THE PUBLISHER ILLUSTRATION OF THE BEHAVIOUR TREE FOR A VISUAL DEPICTION

        bt = new BehaviourTree();

        //1
        bt.AddNode(Node.Typing.SELECT, 0);
        bt.AddNode(Node.Typing.LEAF, 1, CELEBRATE);

        //2
        bt.AddNode(Node.Typing.SELECT, 1);


        //3 
        bt.AddNode(Node.Typing.SELECT, 2);

        //4
        bt.AddNode(Node.Typing.SEQUENCE, 3);
        bt.AddNode(Node.Typing.LEAF, 4, NOTENGAGED);
        bt.AddNode(Node.Typing.LEAF, 4, AMINOTVISIBLE);
        bt.AddNode(Node.Typing.LEAF, 4, NOTMAXHEALTH);
        bt.AddNode(Node.Typing.LEAF, 4, HEAL);
        bt.AddNode(Node.Typing.LEAF, 4, WAIT);

        //5
        bt.AddNode(Node.Typing.SEQUENCE, 3);
        bt.AddNode(Node.Typing.LEAF, 5, ISLOWHEALTH);

        //6
        bt.AddNode(Node.Typing.SELECT, 5);

        //7
        bt.AddNode(Node.Typing.SEQUENCE, 6);
        bt.AddNode(Node.Typing.LEAF, 7, AMIVISIBLE);
        bt.AddNode(Node.Typing.LEAF, 7, WALL);
        bt.AddNode(Node.Typing.LEAF, 7, HEAL);
        bt.AddNode(Node.Typing.LEAF, 7, WAIT);

        //8
        bt.AddNode(Node.Typing.SEQUENCE, 6);
        bt.AddNode(Node.Typing.LEAF, 8, HEAL);
        bt.AddNode(Node.Typing.LEAF, 8, WAIT);

        //9
        bt.AddNode(Node.Typing.SEQUENCE, 2);
        bt.AddNode(Node.Typing.LEAF, 9, WASIATTACKED);
        bt.AddNode(Node.Typing.LEAF, 9, APPLYPROTECTION);

        //10
        bt.AddNode(Node.Typing.SELECT, 1);

        //11
        bt.AddNode(Node.Typing.SEQUENCE, 10);
        bt.AddNode(Node.Typing.LEAF, 11, ENEMYNOTVISIBLE);
        bt.AddNode(Node.Typing.LEAF, 11, NOTENGAGED);
        bt.AddNode(Node.Typing.LEAF, 11, EXPLOSIONHEARD);
        bt.AddNode(Node.Typing.LEAF, 11, MOVETOEXPLOSION);

        //12
        bt.AddNode(Node.Typing.SEQUENCE, 10);
        bt.AddNode(Node.Typing.LEAF, 12, ENEMYNOTVISIBLE);
        bt.AddNode(Node.Typing.LEAF, 12, WONDER);

        //13
        bt.AddNode(Node.Typing.SELECT, 10);

        //14
        bt.AddNode(Node.Typing.SEQUENCE, 13);
        bt.AddNode(Node.Typing.LEAF, 14, MULTIPLEENEMIESINRANGE);
        bt.AddNode(Node.Typing.LEAF, 14, AIMCENTRE);
        bt.AddNode(Node.Typing.LEAF, 14, SELECTELEMENT);
        bt.AddNode(Node.Typing.LEAF, 14, CASTLARGESPELL);
        bt.AddNode(Node.Typing.LEAF, 14, WAIT);

        //15
        bt.AddNode(Node.Typing.SEQUENCE, 13);
        bt.AddNode(Node.Typing.LEAF, 15, PICKTARGET);
        bt.AddNode(Node.Typing.LEAF, 15, FACETARGET);
        bt.AddNode(Node.Typing.LEAF, 15, SELECTELEMENT);
        bt.AddNode(Node.Typing.LEAF, 15, CASTSMALLSPELL);
        bt.AddNode(Node.Typing.LEAF, 15, WAIT);
        
      
    }

    void Update()
    {
        bt.Process();
        healthBar.value = health;
    }


    /// <summary>
    /// Generates a random vector position around a radius of the agent
    /// </summary>
    /// <param name="radius"></param>
    /// Returns the newly generated vector position.
    /// <returns></returns>
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
    /// <summary>
    /// Causes damage to this Wizard. Damage is based on current elemental strengths/weaknesses.
    /// </summary>
    /// <param name="_element"></param>
    /// The element of the damage.
    /// <param name="_damage"></param>
    /// The base damage value.
    public void InflictDamage(int _element, float _damage)
    {
        float multiplier = 1f;
        if (currentDefenseElement == _element)
            multiplier = -0.5f; 
         else if (currentDefenseElement == 1 && _element == 2)
            multiplier = 2f;
        else if (currentDefenseElement == 2 && _element == 1)
            multiplier = 2f;
        else if (currentDefenseElement == 3 && _element == 4)
            multiplier = 2f;
        else if (currentDefenseElement == 4 && _element == 3)
            multiplier = 2f;

        health -= (_damage * multiplier);
        lastHitElement = _element;
        if(health <= 0)
        {
            wm.Wizards.Remove(this);
            GameObject effect = Instantiate(deathEffect_prefab, transform.position,Quaternion.identity);
            wm.AlertSound(transform);
            Destroy(effect, 1f);
            Destroy(gameObject);
        }
    }    
    /// <summary>
    /// Checks to see if this Wizard Agent is currently the target of other Wizard Agents.
    /// </summary>
    /// <returns></returns>
    /// Returns true if this Wizard Agent is being targetted.
    bool IsVisible()
    {
        bool isVisible = false;
        foreach (Wizard_Agent wAgent in wm.Wizards)
        {
            if (wAgent != this)
            {
                if (wAgent.target == gameObject)
                    isVisible = true;
            }
        }
        return isVisible;
    }

    //GIZMOS TO VISUALISE SIGHTLINES AND DESTINATIONS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, sightRadius);       
        Gizmos.DrawLine(transform.position, (transform.position + (transform.forward + ((transform.right / 45) * viewingAngle)).normalized * sightRadius));
        Gizmos.DrawLine(transform.position, (transform.position + (transform.forward - ((transform.right / 45) * viewingAngle)).normalized * sightRadius));
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentDestination, 0.5f);
        Gizmos.DrawLine(transform.position, currentDestination);
    }

    ///ACTIONS!
    ///ALL FUNCTIONS BELOW THIS POINT ARE ACTION FUNCTIONS (LEAF NODES OT THE BEHAVIOUR TREE)



    /// <summary>
    /// Performs the CELEBRATE action.
    /// </summary>
    /// <returns></returns>
    Node.State CELEBRATE()
    {
        if(wm.Wizards.Count > 1)
            return Node.State.FAILURE;
        agent.isStopped = true;
        currentDestination = Vector3.zero;
        if (victory != null)
            return Node.State.SUCCESS;
        victory = Instantiate(victorySpellEffect_prefab, transform.position, Quaternion.identity);
        Destroy(victory, 0.2f);
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Checks to see if this Wizards Health is at max.
    /// </summary>
    /// <returns></returns>
    /// Returns Success if the health value is NOT max.
     Node.State NOTMAXHEALTH()
    {      
        if (health != maxHealth)
            return Node.State.SUCCESS;
        return Node.State.FAILURE;
    }
    /// <summary>
    /// Checks to see if this Wizards Health is at critical.
    /// </summary>
    /// <returns></returns>
    /// Returns Success if health is below 410 points.
    Node.State ISLOWHEALTH()
    {
        if (health <= 410f)
            return Node.State.SUCCESS;
        return Node.State.FAILURE;
    }


    ///AS A SIDE NOTE; THE NEXT TWO FUNCTIONS WOULDN'T BE NEEDED IF THE BEHAVIOUR TREE USED A DECORATOR NODE!!!!!!!!!!!!!!!!!!!!!!!!!!
    ///FURTHER WORK MARK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    /// <summary>
    /// Checks to see if this Wizard is being targetted by another Wizard.
    /// </summary>
    /// <returns></returns>
    Node.State AMIVISIBLE()
    {
        if (IsVisible())
            return Node.State.SUCCESS;
        return Node.State.FAILURE;
    }
    /// <summary>
    /// Checks to see if this Wizard is NOT being targetted by another Wizard.
    /// </summary>
    /// <returns></returns>
    Node.State AMINOTVISIBLE()
    {
        if (IsVisible())
            return Node.State.FAILURE;
        return Node.State.SUCCESS;
    }



    /// <summary>
    /// Performs the WALL action, which creates an Arcane Dome around the Wizard.
    /// </summary>
    /// <returns></returns>
    Node.State WALL()
    {      
        //CHECK TO SEE IF AN ARCANE DOME HAS ALREADY BEEN PLACED AS TO AVOID DUPLICATIONS.
        if(wall == null)
        {
            wall = Instantiate(wall_prefab, transform.position, Quaternion.identity);
            wall.transform.parent = null;
            Destroy(wall, 6f);
        }
        return Node.State.SUCCESS;
    }
   /// <summary>
   /// Perfroms the HEAL action, which heals the Wizard for 50points.
   /// </summary>
   /// <returns></returns>
    Node.State HEAL()
    {
        if (waiting)
            return Node.State.SUCCESS;
        health += 50;
        GameObject effect = Instantiate(healEffect_prefab, transform);
        Destroy(effect, 0.5f);
        if (health >= maxHealth)
        {
            health = maxHealth;
            return Node.State.FAILURE;
        }       
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Checks to see if the Wizard lost health between a Behaviour Tree Tick.
    /// </summary>
    /// <returns></returns>
    /// Returns Success if its health value before the tick is more than the current health value.
    Node.State WASIATTACKED()
    {
        if(health < lastKnownHealthValue)
        {
            lastKnownHealthValue = health;
            return Node.State.SUCCESS;
        }
        return Node.State.FAILURE;
    }
    /// <summary>
    /// Performs the action APPLY_PROTECTION, which creates an elemental protection bubble around the Wizard based on the element of the last hit.
    /// </summary>
    /// <returns></returns>
    Node.State APPLYPROTECTION()
    {
        switch (lastHitElement)
        {
            case 1:
                if (currentDefenseElement == 1)
                    break;
                Destroy(defenseEffect);
                defenseEffect = Instantiate(firedefense_prefab,transform);
                currentDefenseElement = 1;
                break;
            case 2:
                if (currentDefenseElement == 2)
                    break;
                Destroy(defenseEffect);
                defenseEffect = Instantiate(icedefense_prefab, transform);
                currentDefenseElement = 2;
                break;
            case 3:
                if (currentDefenseElement == 3)
                    break;
                Destroy(defenseEffect);
                defenseEffect = Instantiate(lightningdefense_prefab, transform);
                currentDefenseElement = 3;
                break;
            case 4:
                if (currentDefenseElement == 4)
                    break;
                Destroy(defenseEffect);
                defenseEffect = Instantiate(waterdefense_prefab, transform);
                currentDefenseElement = 4;
                break;
        }
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Checks to see if this Wizard DOESN'T have a current Target Wizard.
    /// </summary>
    /// <returns></returns>
    /// Returns Sucess if this Wizard has no target.
    Node.State NOTENGAGED()
    {       
        if (target == null)
            return Node.State.SUCCESS;
        return Node.State.FAILURE;
    }
    /// <summary>
    /// Checks to see if this Wizard heard any sounds/explosions.
    /// </summary>
    /// <returns></returns>
    Node.State EXPLOSIONHEARD()
    {
        if(lastHeardSound == null)
            return Node.State.FAILURE;
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs the MOVE_TO_EXPLOSION action, which sets the Wizards destination to the source of the sound.
    /// </summary>
    /// <returns></returns>
    /// Returns Success upon reaching its destination.
    /// Returns Running while on route to its destination.
    /// Returns Failure if the sound origin is no longer valid.
    Node.State MOVETOEXPLOSION()
    {
        if (target != null)
            return Node.State.FAILURE;
        if (lastHeardSound == null)
            return Node.State.FAILURE;
        agent.isStopped = false;
        agent.updateRotation = true;
        currentDestination = lastHeardSound.position;
        agent.SetDestination(lastHeardSound.position);
        if (Vector3.Distance(transform.position, lastHeardSound.position) < 1f)
        {
            lastHeardSound = null;
            return Node.State.SUCCESS;
        }
        return Node.State.RUNNING;
    }
    /// <summary>
    /// Checks to see if there are NOT any Wizards in this Wizards line of sight.
    /// </summary>
    /// <returns></returns>
    /// Returns Success if this Wizard CANNOT see other Wizards.
    /// Returns Failed if this Wizard CAN see other Wizards.
    Node.State ENEMYNOTVISIBLE()
    {
        //RETURN ALL OBJECTS IN SIGHT RADIUS
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRadius);

        //REMOVE THOSE THAT ARE NOT IN THE VIEWING ANGLE
        Dictionary<Collider, float> collidersInView = new Dictionary<Collider, float>();            //FURTHER WORK!!!!!!!!!!!       THE REASON I USED A DICTIONARY HERE WAS TO ASSOCIATE EACH WIZARD SEEN WITH A VIEW ANGLE AS TO IMPLEMENT THE VISUAL FIELD MECHANIC
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Wizard")
            {
                Vector3 targetDir = hitColliders[i].transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);
                if (Mathf.Abs(angle) < viewingAngle)
                {
                    collidersInView.Add(hitColliders[i], angle);    //STORE THE VIEWING ANGLE. THIS IS TO IMPLEMENT A VIEWING FIELD MECHANIC                    FURTHER WORK!!!!!!!!!!
                }
            }
        }

        //Debug.Log("Wizards in view : " + collidersInView.Count);
        visibleEnemies.Clear();
        foreach (KeyValuePair<Collider, float> item in collidersInView)
        {
            RaycastHit hit;
            Vector3 targetDir = item.Key.transform.position - transform.position;   
            //CHECK TO SEE IF THE WIZARD IN THE SIGHT RADIUS IS NOT OBSCURED IN VIEW (AS IN BEHIND AN OBJECT)
            if (Physics.Raycast(transform.position, targetDir, out hit))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Wizard")
                {
                    visibleEnemies.Add(item.Key, item.Value);
                    Debug.DrawLine(transform.position, item.Key.transform.position);
                }                
            }
        }

        if (visibleEnemies.Count == 0)
        {
            target = null;
            return Node.State.SUCCESS;
        }     
        return Node.State.FAILURE;
    }
    /// <summary>
    /// Performs the WONDER action, which makes the Wizard walk to random destinations in the Arena.
    /// </summary>
    /// <returns></returns>
    /// Returns Success upon reaching destination.
    /// Returns Running while travelling to destination.
    Node.State WONDER()
    {
        if (target != null)
            return Node.State.FAILURE;
        agent.isStopped = false;
        agent.updateRotation = true;
        if (currentDestination == Vector3.zero)
            currentDestination = RandomNavmeshLocation(15f);
        agent.SetDestination(currentDestination);
        if (Vector3.Distance(transform.position, currentDestination) < 1f)
        {
            currentDestination = Vector3.zero;
            return Node.State.SUCCESS;
        }
        return Node.State.RUNNING;
    }
    /// <summary>
    /// Checks to see if there are multiple Wizards (>1) in the viewing field.
    /// </summary>
    /// <returns></returns>
    Node.State MULTIPLEENEMIESINRANGE()
    {
        if (visibleEnemies.Count > 1)
            return Node.State.SUCCESS;
        return Node.State.FAILURE;
    } 
    /// <summary>
    /// Performs the AIM_CENTRE action, which creates a target at the centre of the group of Wizard enemies.
    /// </summary>
    /// <returns></returns>
    Node.State AIMCENTRE()
    {       
        Vector3 middlePos = Vector3.zero;
        //for (int i = 0; i < visibleEnemies.Count; i++)
        //{
        //    middlePos += visibleEnemies[i].transform.position;
        //}
        foreach (KeyValuePair<Collider, float> item in visibleEnemies)
        {
            middlePos += item.Key.gameObject.transform.position;
        }
        middlePos /= 2f;
        GameObject empty = new GameObject();
        empty.name = "TargetASSIST";
        empty.transform.position = middlePos;
        Destroy(empty, 0.2f);
        target = empty;
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs SELECT_ELEMENT action, which selects the element to attack with based on the Targets Protective Bubble.
    /// </summary>
    /// <returns></returns>
    Node.State SELECTELEMENT()
    {
        Wizard_Agent wTarget = target.GetComponent<Wizard_Agent>();
        if(wTarget == null)
        {
            currentCastingElement = Random.Range(1, 4);
        }
        else
        {
            switch (wTarget.currentDefenseElement)
            {
                case 1:
                    currentCastingElement = 2;
                    break;
                case 2:
                    currentCastingElement = 1;
                    break;
                case 3:
                    currentCastingElement = 4;
                    break;
                case 4:
                    currentCastingElement = 3;
                    break;
                default:
                    currentCastingElement = Random.Range(1, 4);
                    break;
            }
        }
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs CAST_LARGE_SPELL action, which allows the Wizard to cast a Large elemental spell.
    /// </summary>
    /// <returns></returns>
    Node.State CASTLARGESPELL()
    {
        if (waiting)
            return Node.State.SUCCESS;
        agent.updateRotation = false;
        transform.LookAt(target.transform, transform.up);      
        GameObject shot = null;
        switch (currentCastingElement)
        {
            case 1:
                shot = Instantiate(fireshotLarge_prefab, target.transform.position, Quaternion.Euler(new Vector3(-90,0,0)));
                break;
            case 2:
                shot = Instantiate(iceshotLarge_prefab, target.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                break;
            case 3:
                shot = Instantiate(lightningshotLarge_prefab, target.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                break;
            case 4:
                shot = Instantiate(watershotLarge_prefab, target.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                break;
        }
        shot.GetComponent<Shot_AOE>().Initiate(currentCastingElement);
        Destroy(shot, 2f);
        GameObject castEffect = Instantiate(largeSpellCastEffect_prefab, transform);
        Destroy(castEffect, 1f);
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs the WAIT actiom, which forces the Wizard to wait for certain actions for 1 second.
    /// Typically used as the cooldown for spellcasting.
    /// </summary>
    /// <returns></returns>
    /// Returns Success once a second has passed.
    /// Returns Running which counting to the second.
    Node.State WAIT()
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
    /// <summary>
    /// Picks a target based on the Viewing angle.
    /// </summary>
    /// <returns></returns>
    Node.State PICKTARGET()
    {
        if (visibleEnemies.Count == 0)
            return Node.State.FAILURE;

        float smallestAngle = Mathf.Infinity;
        foreach (KeyValuePair<Collider, float> item in visibleEnemies)
        {
            if(item.Value < smallestAngle)
            {
                target = item.Key.gameObject;
                smallestAngle = item.Value;
            }
        }

        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs the FACE_TARGET action, which causes the Wizard to face its target.
    /// </summary>
    /// <returns></returns>
    Node.State FACETARGET()
    {        
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        currentDestination = Vector3.zero;
        agent.updateRotation = false;
        transform.LookAt(target.transform, transform.up);
        return Node.State.SUCCESS;
    }
    /// <summary>
    /// Performs the CAST_SMALL_SPELL action, which causes the Wizard to fire a small homing projectile at its target.
    /// </summary>
    /// <returns></returns>
    Node.State CASTSMALLSPELL()
    {
        if (waiting)
            return Node.State.SUCCESS;      
        GameObject shot = null;
        switch (currentCastingElement)
        {
            case 1:
                shot = Instantiate(fireshotSmall_prefab, transform.position, Quaternion.identity);
                break;
            case 2:
                shot = Instantiate(iceshotSmall_prefab, transform.position, Quaternion.identity);
                break;
            case 3:
                shot = Instantiate(lightningshotSmall_prefab, transform.position, Quaternion.identity);
                break;
            case 4:
                shot = Instantiate(watershotSmall_prefab, transform.position, Quaternion.identity);
                break;
        }
        shot.GetComponent<Shot>().Initiate(target.transform.transform, currentCastingElement);
        Destroy(shot, 2.8f);
        return Node.State.SUCCESS;
    }







    ///FAILED ATTEMPTS AND TESTING!!!! 
    ///PLEASE IGNORE!!!!


    // //Returns all objects in sightRadius range
    //Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRadius);

    // //Filter out those not in the viewing angle
    // Dictionary<Collider, float> collidersInView = new Dictionary<Collider, float>();
    // for (int i = 0; i < hitColliders.Length; i++)
    // {
    //     if (hitColliders[i].tag == "Wizard")
    //     {
    //         Vector3 targetDir = hitColliders[i].transform.position - transform.position;
    //         float angle = Vector3.Angle(targetDir, transform.forward);
    //         if (Mathf.Abs(angle) < viewingAngle)
    //         {
    //             collidersInView.Add(hitColliders[i], angle);    //STORE THE VIEWING ANGLE. This is to assign priority of things in view so that things in the corner of its vision are treated differently
    //         }
    //     }
    // }

    // Debug.Log("Wizards in view : " + collidersInView.Count);
    // targets.Clear();
    // foreach (KeyValuePair<Collider, float> item in collidersInView)
    // {
    //     RaycastHit hit;
    //     Vector3 targetDir = item.Key.transform.position - transform.position;
    //     if (Physics.Raycast(transform.position, targetDir, out hit))
    //     {
    //         if (hit.transform.gameObject.tag == "Wizard")
    //         {
    //             targets.Add(item.Key.transform);
    //         }
    //         Debug.DrawLine(transform.position, item.Key.transform.position);
    //     }
    // } 



    //int LowHealth()
    //{
    //    if (health <= 200)
    //        return 1;
    //    return 0;
    //}

    //void Heal()
    //{
    //    health += 50f;
    //}

    //int EnemyVisible()
    //{
    //    //Returns all objects in sightRadius range
    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRadius);

    //    //Filter out those not in the viewing angle
    //    Dictionary<Collider, float> collidersInView = new Dictionary<Collider, float>();
    //    for (int i = 0; i < hitColliders.Length; i++)
    //    {
    //        if (hitColliders[i].tag == "Wizard")
    //        {
    //            Vector3 targetDir = hitColliders[i].transform.position - transform.position;
    //            float angle = Vector3.Angle(targetDir, transform.forward);
    //            if (Mathf.Abs(angle) < viewingAngle)
    //            {
    //                collidersInView.Add(hitColliders[i], angle);    //STORE THE VIEWING ANGLE. This is to assign priority of things in view so that things in the corner of its vision are treated differently
    //            }
    //        }
    //    }

    //    Debug.Log("Wizards in view : " + collidersInView.Count);
    //    targets.Clear();
    //    foreach (KeyValuePair<Collider, float> item in collidersInView)
    //    {
    //        RaycastHit hit;
    //        Vector3 targetDir = item.Key.transform.position - transform.position;
    //        if (Physics.Raycast(transform.position, targetDir, out hit))
    //        {
    //            if (hit.transform.gameObject.tag == "Wizard")
    //            {
    //                targets.Add(item.Key.transform);
    //            }
    //            Debug.DrawLine(transform.position, item.Key.transform.position);
    //        }
    //    }
    //    if (targets.Count == 0)
    //        return 0;
    //    return 1;
    //}

    //    int MultipleEnemiesVisible()
    //{
    //    //ENEMY DETECTION CODE HERE!
    //    return 0;
    //}

    //void SetDefense()
    //{
    //    Destroy(equippedDefense);
    //    switch(lastHitElementAttack){
    //        case 1:
    //            equippedDefense = Instantiate(firedefense_prefab, transform);
    //            break;
    //        case 2:
    //            equippedDefense = Instantiate(icedefense_prefab, transform);
    //            break;
    //        case 3:
    //            equippedDefense = Instantiate(lightningdefense_prefab, transform);
    //            break;
    //        case 4:
    //            equippedDefense = Instantiate(waterdefense_prefab, transform);
    //            break;
    //    }
    //}
    //void SetElement()
    //{
    //    targetElement = targets[0].GetComponent<Enemy>().element + 1;
    //    switch (targetElement)
    //    {
    //        case 1:
    //            castingElement = 2;
    //            break;
    //        case 2:
    //            castingElement = 1;
    //            break;
    //        case 3:
    //            castingElement = 4;
    //            break;
    //        case 4:
    //            castingElement = 3;
    //            break;
    //    }
    //}

    //void Shoot()
    //{
    //    agent.updateRotation = false;
    //    transform.LookAt(targets[0],transform.up);
    //    //Vector3 direction = targets[0].position - transform.position;
    //    //Quaternion toRotation = Quaternion.LookRotation(transform.forward, direction);
    //    //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.1f * Time.time);
    //    GameObject shot = null;
    //    switch (castingElement)
    //    {
    //        case 1:
    //            shot = Instantiate(fireshot_prefab, transform.position, Quaternion.identity);
    //            break;
    //        case 2:
    //            shot = Instantiate(iceshot_prefab, transform.position, Quaternion.identity);
    //            break;
    //        case 3:
    //            shot = Instantiate(lightningshot_prefab, transform.position, Quaternion.identity);
    //            break;
    //        case 4:
    //            shot = Instantiate(watershot_prefab, transform.position, Quaternion.identity);
    //            break;
    //    }
    //    shot.GetComponent<Shot>().Initiate(targets[0].transform,castingElement);
    //    Destroy(shot, 2f);
    //}


    //void FindEnemy()
    //{
    //    agent.updateRotation = true;
    //    Vector3 newpos = RandomNavmeshLocation(10f); //new Vector3(Random.Range(-25, 25), 0f, Random.Range(-50, 50));
    //    if (Vector3.Distance(transform.position, currentDestination) < 1f || currentDestination == Vector3.zero)
    //    {
    //        currentDestination = newpos;
    //        agent.SetDestination(newpos);
    //    }
    //}
    //private Vector3 RandomNavmeshLocation(float radius)
    //{
    //    Vector3 randomDirection = Random.insideUnitSphere * radius;
    //    randomDirection += transform.position;
    //    NavMeshHit hit;
    //    Vector3 finalPosition = Vector3.zero;
    //    if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
    //    {
    //        finalPosition = hit.position;
    //    }
    //    return finalPosition;
    //}

    //int TargetClosest()
    //{
    //    //TARGET CLOSEST HERE!
    //    return 1;
    //}


}
