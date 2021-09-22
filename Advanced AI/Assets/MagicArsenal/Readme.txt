----------------------------------------
MAGIC ARSENAL
----------------------------------------

1. Introduction
2. Spawning effects
3. Scaling effects
4. FAQ / Problemsolving
5. Asset Extras
6. Contact
7. Credits

----------------------------------------
1. INTRODUCTION
----------------------------------------

a) Effects can be found in the 'MagicArsenal/Effects/Prefabs'. Note that explosions/impacts are categorized next to the missiles in the 'Prefabs/Missiles'.

b) To browse the effects in Unity, locate the demo scenes in 'MagicArsenal/Demo/Scenes'.

If you want to play the demo in your project, you will first have to load the demo scenes in your Build Settings. Locate the scenes in 'MagicArsenal/Demo/Scenes', then drag them to your "Scenes in Build" in the Build Settings window. This will let you use the in-game GUI to change scenes.

----------------------------------------
2. SPAWNING EFFECTS
----------------------------------------

In some cases you can just drag&drop the effect into the scene to make them play, otherwise you can spawn them via scripting.

Small example on spawning an explosion via script:

public Vector3 effectNormal;

spawnEffect = Instantiate(spawnEffect, transform.position, Quaternion.FromToRotation(Vector3.up, effectNormal)) as GameObject;

----------------------------------------
3. SCALING
----------------------------------------

To scale an effect in the scene, you can simply use the default Scaling tool (Hotkey 'R'). You can also select the effect and set the Scale in the Hierarchy.

Please remember that some parts of the effects such as Point Lights, Trail Renderers and Audio Sources may have to be manually adjusted afterwards.

----------------------------------------
4. FAQ / Problemsolving
----------------------------------------

Q: Particles appear stretched or too thin after scaling
 
A: This means that one of the effects are using a Stretched Billboard render type. Select the prefab and locate the Renderer tab at the bottom of the Particle System. If you scaled the effect up to be twice as big, you'll also need to multiply the current Length Scale by two.

--------------------

Q: The effects look grey or darker than they're supposed to

A: https://forum.unity.com/threads/epic-toon-fx.390693/#post-3279824

--------------------

Q: Annoying "Invalid AABB aabb" errors

A: This seems to be an error that comes and goes in between some versions, possible fix: https://forum.unity.com/threads/epic-toon-fx.390693/#post-3542039

--------------------

Q: I can't find what I'm looking for

A: There are a lot of effects in this pack, I suggest searching the Project folder or send an e-mail if you need a hand.

--------------------

Q: Can you add X effect to this asset?

A: Maybe! Add sufficient details to your request, and I will consider including it for the next update. Please note that it can take weeks or months in between updates.

----------------------------------------
5. ASSET EXTRAS
----------------------------------------

In the 'MagicArsenal/Effects/Scripts' folder you can find some neat scripts that may further help you customize the effects.

MagicBeamStatic - A simple script for beam effects. Use the prefabs from 'Effects/Beam/Setup' and experiment with the Beam Options!

MagicLightFade - This lets you fade out lights which are useful for explosions

MagicLightFlicker - A script for making pulsating and flickering lights

MagicRotation - A simple script that applies constant rotation to an object

----------------------------------------
6. CONTACT
----------------------------------------

Need help with anything? 

E-Mail : archanor.work@gmail.com
Website: archanor.com

Follow me on Twitter for regular updates and news

Twitter: @Archanor

----------------------------------------
7. CREDITS
----------------------------------------

Special thanks to:

Jan Jørgensen
Daniel Kole
Julius Lyngby Forsberg
Thanks to mactinite for the sword model - http://opengameart.org/content/fantasy-sword-hand-painted-2
Thanke to HellGate for the cobblestone texture - https://opengameart.org/content/seamless-cobblestone-texture