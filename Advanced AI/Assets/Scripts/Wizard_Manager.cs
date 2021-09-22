using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard_Manager : MonoBehaviour
{

    /// <summary>
    /// THE WIZARD MANAGER SCRIPT IS RESPONSIBLE FOR KEEPING REFERENCES OF ALL CURRENTLY ALIVE WIZARDS ON THE ARENA.
    /// THE WIZARD MANAGER ALSO HANDLES "SOUND" AS IT CAN SEND NOTIFICATIONS TO ALL WIZARDS.
    /// ALMOST LIKE THE EVEVLOPE SYSTEM IN WEEK3
    /// </summary>


    public static Wizard_Manager wm;

    public List<Wizard_Agent> Wizards = new List<Wizard_Agent>();

    private bool show = true;

    GUIStyle guiStyle = new GUIStyle();
    /// <summary>
    /// RESPONSIBLE FOR DRAWING THE ON SCREEN UI
    /// </summary>
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(10, 10, 100, 20), "Wizards Left: " + Wizards.Count, guiStyle);       
        GUI.Label(new Rect(10, 40, 100, 20), "Hide/UnHide Guide (H)", guiStyle);
        if (show)
        {
            GUI.Label(new Rect(10, 70, 100, 20), "Mouse Buttons - Switch Wizard", guiStyle);
            GUI.Label(new Rect(10, 130, 100, 20), "Orange Spell = Fire\nWhite Spell = Ice\nPurple Spell = Lightning\nBlue Spell = Water\nFire vs Ice\nLightning vs Water\nPurple dome cloaks any Wizard inside.\nGreen aura represents Healing spell.\n\nWizards Cast bigger spells\n when more enemies are visible.\n\nWizards hear the sounds of Large Spells,\n and any deaths of other Wizards.", guiStyle);
        }
    }


    private void Awake()
    {
        wm = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (show)
                show = false;
            else
                show = true;
        }
    }
    /// <summary>
    /// Notifies all Wizards that a sound has been made, and its location.
    /// </summary>
    /// Location of the sound as a transform
    /// <param name="_soundLocation"></param>
    public void AlertSound(Transform _soundLocation)
    {
        foreach (Wizard_Agent agent in Wizards)
        {
            agent.lastHeardSound = _soundLocation;
        }
    }
}
