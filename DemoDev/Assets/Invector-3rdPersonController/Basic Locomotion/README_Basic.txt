Thank you for support our asset!

*IMPORTANT* This asset requires Unity 5.2.0 or higher.

If you have any question about how it works or if you are experiencing any trouble, 
feel free to email us at: inv3ctor@gmail.com
Please do not Upload or share this asset as a Package without permission.

If you downloaded this asset illegally for studies or prototype purposes, 
please reconsider purchase if you want to publish your work, you can buy on the AssetStore 
or send us a email and we can figure something out, you can even post your work on our Forum, 
will be happy to help and advertise your game.

ASSETSTORE: https://www.assetstore.unity3d.com/en/#!/content/44227
FORUM: http://invector.proboards.com/
YOUTUBE: https://www.youtube.com/channel/UCSEoY03WFn7D0m1uMi6DxZQ
WEBSITE: http://www.invector.xyz/

Invector Team - 2016

Changelog v2.0c smallHOTFIX 11/12/2016

Changes:
- Fix Basic Change Scene buttons
- Minor changes on the 2.5D Scene controller
- Improved JumpMove transition
- Improved TurnOnSpot (add option for moveset)
- Fix Action bug when repeatedly smash the button during an action
- Fix occasionally bug when jumping during an action and falling though the ground
- DamageObjects now has the option to trigger a reactionID 

-----------------------------------------------------------------------------------------------------

Changelog v2.0b HOTFIX 22/11/2016

Changes:
- Improved rigibody movement using OnAnimatorMove 
- Improved animation transition from CrossFade to CrossFadeInFixedTime 
- Fix bug when hit the button twice or more when starting the action
- Fix MovementSpeed values not working on v2.0a
- Fix vMoveSetSpeed script to handle different moveset ID movement speed separatly
- Fix pendulum prefab missing mesh
- Fix ragdoll reseting at the y = 0 of the world location
- Fix ragdoll creator creating small collider for the head
- Fix sign prefabs examples

-----------------------------------------------------------------------------------------------------

Changelog v2.0a HOTFIX 08/10/2016

Changes:
- Fix character locomotion jittering at low frame rates 
- Fix links for youtube tutorials 
- Improved Jump Physics

-----------------------------------------------------------------------------------------------------

Changelog v2.0 BASIC LOCOMOTION  XX/10/2016

Changes:

- HeadTrack improved
- ThirdPerson & Topdown now share the same scripts, just change the input
- ThirdPersonInput & MeleeCombatInput scripts add to handle Input directly
- Changes in the Core of the Controller
- Animator Controller improved
- HUD improved
- New Action verification (+Performance)
- Extra Move Speed improved for the Controller, non-root motion or root motion with extra speed
- Capsule Collider will not be 'stuck' on walls anymore

-----------------------------------------------------------------------------------------------------

Changelog v1.3 BASIC LOCOMOTION BIG UPDATE 03/08/2016

Changes:
- Major changes on the main scripts of the Controller
- Improved Jump Physics
- Improved Ragdoll Transitions
- OnTriggerStay improved if using Ragdoll
- TriggerActions method changed, now we check the angle of the trigger instead of using Raycast (+performance)
- DraggableBox removed from the template (we'll remake in the future)

Add:
- New Animator Controller with less transitions, layers & parameters (easier to customize & add new animations)
- New V-bot 2.0 with custom textures and 3 LOD level
- New Headtrack system with LookAt to objects
- Area damage example (damage by frequency when staying inside the area) 
- New Input script that handle all the input separated from the controller (easy to integrate with input assets)
- New Speed by MoveSet script that allow adding different speed values to the locomotion for each moveset 
- Moving platform example
- New Main DemoScene

-----------------------------------------------------------------------------------------------------

Changelog v1.2a HOTFIX 15/06/2016

Fixed:
- GameController Spawning method getting errors or not spawning the Player prefab
- Layer 'Player' not auto-assigning on the Character childrens, making the character auto-crouch when add a Ragdoll
- Wrong stamina consuption at higher FPS 

---------------------------------------------------------

Changelog v1.2 BASIC LOCOMOTION 31/05/2016

Fixed:
- Error when trying to manually add a TPCamera into a Camera
- Wrong stamina consuption in some cases when rolling, jumping and sprint

Changes:
- Controller, Animator & Motor scripts revised with new commends and regions for both Player, easier to use and modify
- Overhall performance improvements 
- Sprint unlock for strafe locomotion, just add the animations into the blendtree

Add:
- Draggable Boxes 
- Dynamic Trigger Action to know if there is a obstacle above or ahead
- Puzzle Boxes Scene showing the new feature
- Add new Beat'n Up Demo Scene
- Tags & Layers are automatically add into the project 
- Health Item example to recover health

---------------------------------------------------------


Changelog v1.1d HOTFIX 17/01/2016
Fixed:
- Character Template missing Animator
 
-----------------------------------------------------------------------------------------------------
 
Changelog v1.1c 12/01/2016
 
Fixed:
- 2 Actions been activate at the same time, causing the Player to glitch between states
 
Changes:
- QuickStop and QuickTurn now are actions and are located in the Action State on the Animator
- Grounded State remade to handle Strafe animations
- Minor changes at the TPMotor, TPAnimator and TPController to prepare the AI on the next update
- Better transition from Strafe to Free locomotion, and the a smoother Quickturn
- ICharacter Interface created to handle the Ragdoll behaviour for both Player and AI
- Improve the verification for quickStop and quickTurn Add:
- Locomotion Type, now you can choose between Strafe Only, Free Only or Free with Strafe
- Crouch Strafe locomotion
- New Layer for Melee 3 Attack Combo, this layers comes without animations at this version,
but we will add some example animations on the next version,
along with the AI
 
-----------------------------------------------------------------------------------------------------

Changelog v1.1b 30/12/2015

Fixed:
- Message displaying change of input between mobile/pc when clicking 
- Fixed the error about bounds on the FootStep when walking between Terrain and Mesh 

Changes:
- TPCamera & HUD isoleted from the controller, now this components are modular and work individually
- Controller now works as a Prefab, can be Instantiated and will automatically find a Camera or the HUD component on the scene
- Culling fade now goes into the character instead of the Camera, also modular just like the Ragdoll of Footstep component.
- Strafe animations re-configured with better transitions
- Several TPCamera improvements of CameraState transitions and Culling 
- Improvements and changes into the TPAnimator and TPMotor, now we separated on methods that controls animations and locomotion behaviour from each other

Add: 
- GameController with SpawnPoint system 
- Non Root-motion movement add, with separated Directional Movement Speed and Strafing Speed
- Rotate by World bool add into the Controller for better locomotion when playing on Isometric Mode
- List of Ignore Tags for the Ragdoll Component, keep Weapons or Acessories that are children of the Player with the correctly rotation
- New Camera Feature - Fixed Point or Multiple Points (Resident Evil oldschool camera's style)
- Trigger System to change CameraPoints
- New Demo Scene with the V Mansion showing the new CameraMode Fixed Point
- Simple Door example

-----------------------------------------------------------------------------------------------------

Changelog HOTFIX v1.1a 10/11/2015

Fix Bugs:
- On the MobileScene the gameObject MobileControls need to be below the HUD gameObject on the Hierarchy (otherwise it will stop respond after the ragdoll turn on)
- Fix the Jump behaviour if you jump right on the edge, the character falls and jump again (mecanim issue)
- Fix the trajectory of the jump in case of high value of height and force forward 
- v1.1 shipped with a aditional XInput plugin, we removed and now you can export Mobile builds normally

Add: 
- The character now can jump while Aiming (just a condition add in the Animator)
- Add 'quick change scene' buttons on the demo scenes
- Add a Camera Fade effect to hide the character when is too close
- Add Clip Plane Margin for better clipping planes to the Camera
- Add InvectorJoystick.cs is just a quickfix to the Square touch area from the CrossPlataformInput to a Circular touch area
- Add the original Standard Assets CrossPlataformInput to improve compatibility with other assets and avoid errors of scripts duplicity
- Add back the AntiAliasing in the Camera

-----------------------------------------------------------------------------------------------------

Changelog v1.1 25/10/2015

Fix Bugs:
- Fix the vibrating upperbody animation when in Aiming mode after the ragdoll was activated (bug found by Chrisb3D, thanks!)
- Fix lockPlayer after roll through the Crouch Area and rarely lock the player on the exit (bug found by tmmandk, thank you sir!)
- Fix minor bug holding shift and enter on Aim mode still drains stamina (reported by Steel Grin, thanks!)

Changes:
- Script "FindTarget" changed to "TriggerAction" and add AutoActions bool
- Major changes on TPCamera script
- Update XInput plugins to the last version with x86 and x86_x64 support
- change CheckForwardAction() and create a CheckActionObject on ThirdPersonMotor
- remove CheckAutoCrouch from the ThirdPersonController and put on ThirdPersonMotor
- improved Culling of the camera 
- improved Ragdoll transition when back to player 
- improved footstep system syncronization
- improved Ground Detection
- now users need to set up layers manually to improve compatibility with others projects

Add:
- Add float spine curvature to set up how much the spine will curve while on Aiming mode
- Add support to realtime change input to Mobile (thanks to Xander Davis)
- Add support to MFi iOS gamepad (thanks to Xander Davis)
- Add Aiming button on Mobile Touch Controls
- Add AutoActions for TriggerActions automatically do an Action without the need of input
- Add Tag for AutoCrouch (use on a simple trigger object)
- Add FootStep support to Play a specific material on objects with multiple materials
- Add feature Jump 
- Add feature Camera Scroll Zoom (Mouse only)
- Add feature FixedAngle on CameraState
- Add DrawGizmos for Debug to verify the Raycasts on Motor (head, actions, stepup, stopmove)
- Add 2.5D Scene Demo
- Add Topdown Scene Demo
- Add Isometric Point&Click Scene Demo


