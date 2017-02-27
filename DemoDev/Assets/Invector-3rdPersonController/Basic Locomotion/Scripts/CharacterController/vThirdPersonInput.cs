﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Invector.CharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region variables       
        public GameplayInputStyle gameplayInputStyle = GameplayInputStyle.DirectionalInput;
        public LayerMask clickMoveLayer = 1 << 0;
        public bool inverseInputDirection;

        [Header("Default Inputs")]
        public GenericInput horizontalInput = new GenericInput("Horizontal", "LeftAnalogHorizontal", "Horizontal");
        public GenericInput verticallInput = new GenericInput("Vertical", "LeftAnalogVertical", "Vertical");
        public GenericInput jumpInput = new GenericInput("Space", "X", "X");
        public GenericInput rollInput = new GenericInput("Q", "B", "B");
        public GenericInput strafeInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
        public GenericInput sprintInput = new GenericInput("LeftShift", "LeftStickClick", "LeftStickClick");
        public GenericInput crouchInput = new GenericInput("C", "Y", "Y");
        public GenericInput actionInput = new GenericInput("E", "A", "A");
        public GenericInput cancelInput = new GenericInput("Backspace", "B", "B");

        [Header("Camera Settings")]
        public GenericInput rotateCameraXInput = new GenericInput("Mouse X", "RightAnalogHorizontal", "Mouse X");
        public GenericInput rotateCameraYInput = new GenericInput("Mouse Y", "RightAnalogVertical", "Mouse Y");
        public GenericInput cameraZoomInput = new GenericInput("Mouse ScrollWheel", "", "");

        protected v3rdPersonCamera tpCamera;                // acess camera info        
        [HideInInspector]
        public string customCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        [HideInInspector]
        public bool keepDirection;                          // keep the current direction in case you change the cameraState

        // isometric cursor
        public delegate void OnEnableCursor(Vector3 position);
        public delegate void OnDisableCursor();
        public OnEnableCursor onEnableCursor;
        public OnDisableCursor onDisableCursor;
        Vector3 cursorPoint;
        // isometric cursor

        protected vThirdPersonController cc;                // access the ThirdPersonController component
        protected vHUDController hud;                       // acess vHUDController component 
        protected Vector2 oldInput;
        protected bool canInput;

        public enum GameplayInputStyle
        {
            ClickAndMove,
            DirectionalInput
        }

        protected InputDevice inputDevice { get { return vInput.instance.inputDevice; } }

        #endregion        

        protected virtual void Start()
        {
            cc = GetComponent<vThirdPersonController>();
            if (vThirdPersonController.instance == cc || vThirdPersonController.instance == null)
            {
#if UNITY_5_4_OR_NEWER
                SceneManager.sceneLoaded += OnLevelFinishedLoading;
#endif
                cc.Init();
                tpCamera = FindObjectOfType<v3rdPersonCamera>();
                if (tpCamera) tpCamera.SetMainTarget(this.transform);
                cursorPoint = transform.position;
                canInput = true;
                Cursor.visible = false;
                hud = vHUDController.instance;
                if (hud != null)
                    hud.Init(cc);
            }
        }

        #region Initialize Character, Camera & HUD when LoadScene

#if UNITY_5_4_OR_NEWER
	    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	    {
    		try
    		{
	    		cc.Init();
	    		tpCamera = FindObjectOfType<v3rdPersonCamera>();
	    		if (tpCamera) tpCamera.SetMainTarget(this.transform);
	    		cursorPoint = transform.position;
	    		canInput = true;
	    		Cursor.visible = false;
	    		hud = vHUDController.instance;
	    		if (hud != null)
		    		hud.Init(cc);	
			}
	    	catch
	    	{
		    	SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	    	}
        }
#else
        public void OnLevelWasLoaded(int level)
        {
            try
            {
                cc = GetComponent<vThirdPersonController>();
                if (vThirdPersonController.instance == cc || vThirdPersonController.instance == null)
                {
                    cc.Init();
                    tpCamera = FindObjectOfType<v3rdPersonCamera>();
                    if (tpCamera) tpCamera.SetMainTarget(this.transform);
                    cursorPoint = transform.position;
                    canInput = true;
                    Cursor.visible = false;
                    hud = vHUDController.instance;
                    if (hud != null)
                        hud.Init(cc);
                }
            }
            catch
            {

            }
        }
#endif
        #endregion

        protected virtual void LateUpdate()
        {
            if (cc == null) return;
            if (!canInput)
            {
                cc.input = Vector3.zero;
                return;
            }

            if (Time.timeScale == 0) return;    // stop all input routine if pause the game
            InputHandle();                      // update input methods
            UpdateCameraStates();               // update camera states
        }

        protected virtual void FixedUpdate()
        {
            cc.AirControl();
        }

        protected virtual void Update()
        {
            cc.UpdateMotor();                   // call ThirdPersonMotor methods               
            cc.UpdateAnimator();            	// call ThirdPersonAnimator methods
            UpdateHUD();                        // update hud graphics
        }

        protected virtual void InputHandle()
        {
            ExitGameInput();
            CameraInput();

            if (!cc.lockMovement && !cc.ragdolled)
            {
                MoveCharacter();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
                ActionInput();
                EnterLadderInput();
                ExitLadderInput();
            }
        }

        #region Basic Locomotion Inputs

        public void LockInput(bool value)
        {
            canInput = !value;
        }

        protected virtual void MoveCharacter()
        {
            if (gameplayInputStyle == GameplayInputStyle.ClickAndMove)
            {
                cc.rotateByWorld = true;
                ClickAndMove();
            }
            else
                ControllerInput();
        }

        protected virtual void ControllerInput()
        {
            // gets input from mobile
            cc.input.x = horizontalInput.GetAxis();
            cc.input.y = verticallInput.GetAxis();
            // update oldInput to compare with current Input if  keepDirection
            if (!keepDirection)
                oldInput = cc.input;
        }

        protected virtual void StrafeInput()
        {
            if (strafeInput.GetButtonDown())
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (sprintInput.GetButtonDown())
                cc.Sprint(true);
            else
                cc.Sprint(false);
        }

        protected virtual void CrouchInput()
        {
            if (crouchInput.GetButtonDown())
                cc.Crouch();
        }

        protected virtual void JumpInput()
        {
            if (jumpInput.GetButtonDown())
                cc.Jump();
        }

        protected virtual void RollInput()
        {
            if (rollInput.GetButtonDown())
                cc.Roll();
        }

        protected virtual void ActionInput()
        {
            if (cc.triggerAction == null || cc.doingCustomAction || cc.animator.IsInTransition(0)) return;

            if (actionInput.GetButtonDown())
            {
                cc.TriggerAction(cc.triggerAction);
                hud.HideActionText();
            }
        }

        protected virtual void EnterLadderInput()
        {
            if (cc.ladderAction == null || cc.isUsingLadder) return;

            if (actionInput.GetButtonDown())
                cc.TriggerEnterLadder(cc.ladderAction);
        }

        protected virtual void ExitLadderInput()
        {
            if (!cc.isUsingLadder) return;

            if (cc.ladderAction == null)
            {
                // exit ladder at any moment by pressing the cancelInput
                if (cc.baseLayerInfo.IsName("ClimbLadder") && cancelInput.GetButtonDown())
                    cc.ExitLadder(0, true);
            }
            else
            {
                var animationClip = cc.ladderAction.exitAnimation;
                if (animationClip == "ExitLadderBottom")
                {
                    // exit ladder when reach the bottom by pressing the cancelInput or pressing down at
                    if ((cancelInput.GetButtonDown() || cc.speed <= -0.05f) && !cc.exitingLadder)
                        cc.ExitLadder(2);
                }
                else if (animationClip == "ExitLadderTop" && cc.baseLayerInfo.IsName("ClimbLadder"))  // exit the ladder from the top
                {
                    if ((cc.speed >= 0.05f) && !cc.exitingLadder)  // trigger the exit animation by pressing up
                        cc.ExitLadder(1);
                }
            }
        }

        protected virtual void ExitGameInput()
        {
            // just a example to quit the application 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Cursor.visible)
                    Cursor.visible = true;
                else
                    Application.Quit();
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            cc.CheckTriggers(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            cc.CheckTriggerExit(other);
        }

        #endregion

        #region TopDown Methods

        protected virtual void ClickAndMove()
        {
            var dir = (cursorPoint - transform.position).normalized;
            RaycastHit hit;
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(tpCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clickMoveLayer))
                {
                    if (onEnableCursor != null)
                    {
                        onEnableCursor(hit.point);
                    }
                    cursorPoint = hit.point;
                }
            }
            if (!NearPoint(cursorPoint, transform.position))
                cc.input = new Vector2(dir.x, dir.z);
            else
            {
                if (onDisableCursor != null)
                    onDisableCursor();
                cc.input = Vector2.Lerp(cc.input, Vector3.zero, 20 * Time.deltaTime);
            }
        }

        protected virtual bool NearPoint(Vector3 a, Vector3 b)
        {
            var _a = new Vector3(a.x, transform.position.y, a.z);
            var _b = new Vector3(b.x, transform.position.y, b.z);
            return Vector3.Distance(_a, _b) <= 0.5f;
        }

        #endregion

        #region Camera Methods

        protected virtual void CameraInput()
        {
            if (tpCamera == null || cc.lockCamera)
                return;
            var Y = rotateCameraYInput.GetAxis();
            var X = rotateCameraXInput.GetAxis();
            var zoom = cameraZoomInput.GetAxis();
            tpCamera.RotateCamera(X, Y);
            tpCamera.Zoom(zoom);
            // tranform Character direction from camera if not KeepDirection
            if (!keepDirection)
                cc.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);
            // rotate the character with the camera while strafing        
            RotateWithCamera(tpCamera != null ? tpCamera.transform : null);
            // change keedDirection from input diference
            if (keepDirection && Vector2.Distance(cc.input, oldInput) > 0.2f) keepDirection = false;
        }

        protected virtual void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<v3rdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }

            if (changeCameraState && !cc.isStrafing)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
            else if (cc.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (cc.isStrafing)
                tpCamera.ChangeState("Strafing", true);
            else
                tpCamera.ChangeState("Default", true);
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (cc.isStrafing && !cc.actions && !cc.lockMovement && !cc.lockMovement)
            {
                // smooth align character with aim position               
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    cc.RotateToTarget(tpCamera.lockTarget);
                }
                else //if(input != Vector2.zero)
                {
                    cc.RotateWithAnotherTransform(cameraTransform);
                }
            }
        }

        #endregion

        #region HUD       

        public virtual void UpdateHUD()
        {
            if (hud == null)
                return;

            hud.UpdateHUD(cc);
        }

        #endregion
    }
}