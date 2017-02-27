using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Invector;
using Invector.EventSystems;
using UnityEngine.EventSystems;

namespace Invector.CharacterController
{
    public abstract class vThirdPersonMotor : vCharacter
    {
        #region Variables        

        #region Stamina

        [Header("--- Stamina Consumption ---")]
        [SerializeField]
        protected float sprintStamina = 30f;
        [SerializeField]
        protected float jumpStamina = 30f;
        [SerializeField]
        protected float rollStamina = 25f;

        #endregion

        #region Layers
        [Header("---! Layers !---")]
        [Tooltip("Layers that the character can walk on")]
        public LayerMask groundLayer = 1 << 0;
        [Tooltip("Distance to became not grounded")]
        [SerializeField]
        protected float groundMinDistance = 0.2f;
        [SerializeField]
        protected float groundMaxDistance = 0.5f;

        [Tooltip("What objects can make the character auto crouch")]
        public LayerMask autoCrouchLayer = 1 << 0;
        [Tooltip("[SPHERECAST] ADJUST IN PLAY MODE - White Spherecast put just above the head, this will make the character Auto-Crouch if something hit the sphere.")]
        public float headDetect = 0.95f;

        [Tooltip("Select the layers the your character will stop moving when close to")]
        public LayerMask stopMoveLayer;
        [Tooltip("[RAYCAST] Stopmove Raycast Height")]
        public float stopMoveHeight = 0.65f;
        [Tooltip("[RAYCAST] Stopmove Raycast Distance")]
        public float stopMoveDistance = 0.5f;
        #endregion

        #region Character Variables

        public enum LocomotionType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree
        }

        [Header("--- Locomotion Setup ---")]

        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;
        [Tooltip("Lock all the character input")]
        public bool lockMovement;
        [HideInInspector]
        public bool doingCustomAction;
        [Tooltip("Lock the Camera input")]
        public bool lockCamera;
        [Tooltip("Use this to rotate the character using the World axis, or false to use the camera axis - CHECK for Isometric Camera")]
        [SerializeField]
        public bool rotateByWorld = false;
        [Tooltip("Check if you want to use QuickStop animation")]
        [SerializeField]
        protected bool quickStopAnim = true;
        protected bool canQuickStop;
        [Tooltip("Can control the roll direction")]
        [SerializeField]
        protected bool rollControl = false;
        [Tooltip("Speed of the rotation on free directional movement")]
        [SerializeField]
        public float freeRotationSpeed = 10f;
        [Tooltip("Speed of the rotation while strafe movement")]
        public float strafeRotationSpeed = 10f;
        [Tooltip("Put your Random Idle animations at the AnimatorController and select a value to randomize, 0 is disable.")]
        [SerializeField]
        protected float randomIdleTime = 0f;

        [Header("Jump Options")]

        [Tooltip("Check to control the character while jumping")]
        public bool jumpAirControl = true;
        [Tooltip("How much time the character will be jumping")]
        public float jumpTimer = 0.3f;
        [HideInInspector]
        public float jumpCounter;
        [Tooltip("Add Extra jump speed, based on your speed input the character will move forward")]
        public float jumpForward = 1.5f;
        [Tooltip("Add Extra jump height, if you want to jump only with Root Motion leave the value with 0.")]
        public float jumpHeight = 0.5f;

        [Header("--- Movement Speed ---")]

        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeWalkSpeed = 0.5f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeRunningSpeed = 1.1f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeSprintSpeed = 1.2f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeCrouchSpeed = 1f;
        [Tooltip("Add extra speed for the strafe movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeWalkSpeed = 0.5f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeRunningSpeed = 1f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeSprintSpeed = 1.5f;
        [Tooltip("Add extra speed for the strafe movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeCrouchSpeed = 1f;

        [Header("--- Grounded Setup ---")]

        [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
        public float stepOffsetEnd = 0.45f;
        [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
        public float stepOffsetStart = 0.05f;
        [Tooltip("Higher value will result jittering on ramps, lower values will have difficulty on steps")]
        public float stepSmooth = 2.5f;
        [Tooltip("Max angle to walk")]
        [SerializeField]
        protected float slopeLimit = 45f;
        [Tooltip("Apply extra gravity when the character is not grounded")]
        [SerializeField]
        protected float extraGravity = -0.35f;
        [Tooltip("Turn the Ragdoll On when falling at high speed (check VerticalVelocity) - leave the value with 0 if you don't want this feature")]
        [SerializeField]
        protected float ragdollVel = -16f;
        protected float groundDistance;
        public RaycastHit groundHit;

        [Header("--- Debug Info ---")]
        public bool debugWindow;

        #endregion

        #region Actions

        // movement bools
        [HideInInspector]
        public bool
            isGrounded,
            isCrouching,
            inCrouchArea,
            isStrafing,
            isSprinting,
            isSliding,
            stopMove,
            autoCrouch;

        // action bools
        [HideInInspector]
        public bool
            isRolling,
            isJumping,
            isUsingLadder,
            isGettingUp,
            quickStop,
            landHigh,
            jumpOver,
            stepUp,
            climbUp;

        // one bool to rule then all
        [HideInInspector]
        public bool actions
        {
            get
            {
                return jumpOver || stepUp || climbUp || isUsingLadder || isRolling || quickStop || landHigh;
            }
        }

        protected void RemoveComponents()
        {
            if (_capsuleCollider != null) Destroy(_capsuleCollider);
            if (_rigidbody != null) Destroy(_rigidbody);
            if (animator != null) Destroy(animator);
            var comps = GetComponents<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
        }

        #endregion

        #region Direction Variables
        [HideInInspector]
        public Vector3 targetDirection;
        protected Quaternion targetRotation;
        [HideInInspector]
        public Quaternion freeRotation;
        [HideInInspector]
        public bool keepDirection;
        [HideInInspector]
        public Vector2 oldInput;

        #endregion

        #region Components               
        [HideInInspector]
        public vTriggerAction triggerAction;                        // access vTriggerAction component
        [HideInInspector]
        public vTriggerLadderAction ladderAction;                   // access vTriggerLadderAction component      
        [HideInInspector]
        public Rigidbody _rigidbody;                                // access the Rigidbody component
        [HideInInspector]
        public PhysicMaterial frictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
        [HideInInspector]
        public CapsuleCollider _capsuleCollider;                    // access CapsuleCollider information

        #endregion

        #region Hide Variables

        [HideInInspector]
        public float colliderRadius, colliderHeight;        // storage capsule collider extra information        
        [HideInInspector]
        public Vector3 colliderCenter;                      // storage the center of the capsule collider info        
        [HideInInspector]
        public Vector2 input;                               // generate input for the controller        
        [HideInInspector]
        public float speed, direction, verticalVelocity;    // general variables to the locomotion

        // get Layers from the Animator Controller
        [HideInInspector]
        public AnimatorStateInfo baseLayerInfo, underBodyInfo, rightArmInfo, leftArmInfo, fullBodyInfo, upperBodyInfo;
        #endregion

        #endregion

        public void Init()
        {
            // this method is called on the Start of the ThirdPersonController

            // access components
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;

            // prevents the collider from slipping on ramps
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = 0.25f;
            frictionPhysics.dynamicFriction = 0.25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // default physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider info
            _capsuleCollider = GetComponent<CapsuleCollider>();

            // save your collider preferences 
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;

            // avoid collision detection with inside colliders 
            Collider[] AllColliders = this.GetComponentsInChildren<Collider>();
            Collider thisCollider = GetComponent<Collider>();
            for (int i = 0; i < AllColliders.Length; i++)
            {
                Physics.IgnoreCollision(thisCollider, AllColliders[i]);
            }

            // health info
            currentHealth = maxHealth;
            currentHealthRecoveryDelay = healthRecoveryDelay;
            currentStamina = maxStamina;

            canQuickStop = true;
        }

        public virtual void UpdateMotor()
        {
            CheckHealth();
            CheckStamina();
            CheckGround();
            CheckRagdoll();

            ControlCapsuleHeight();
            ControlJumpBehaviour();
            ControlLocomotion();

            StaminaRecovery();
            HealthRecovery();
        }

        #region Health & Stamina

        public override void TakeDamage(Damage damage, bool hitReaction = true)
        {
            // don't apply damage if the character is rolling, you can add more conditions here
            if (currentHealth <= 0 || !damage.ignoreDefense && isRolling)
                return;
            // reduce the current health by the damage amount.
            currentHealth -= damage.value;
            currentHealthRecoveryDelay = healthRecoveryDelay;

            // only trigger the hitReaction animation when the character is not doing an action, using ladder or jumping
            var hitReactionConditions = ((!actions || (actions)) || (isUsingLadder)) && !isJumping;
            if (hitReactionConditions && currentHealth > 0)
            {
                animator.SetInteger("HitDirection", (int)transform.HitAngle(damage.sender.position));
                // trigger hitReaction animation
                if (hitReaction)
                {
                    // set the ID of the reaction based on the attack animation state of the attacker - Check the MeleeAttackBehaviour script
                    animator.SetInteger("ReactionID", damage.reaction_id);
                    animator.SetTrigger("TriggerReaction");
                }
                else
                {
                    animator.SetInteger("RecoilID", damage.recoil_id);
                    animator.SetTrigger("TriggerRecoil");
                }
            }

            onReceiveDamage.Invoke(damage);
            // apply vibration on the gamepad   
            vInput.instance.GamepadVibration(0.25f);
            // turn the ragdoll on if the weapon is checked with 'activeRagdoll' 
            if (damage.activeRagdoll)
                onActiveRagdoll.Invoke();
        }

        public void ReduceStamina(float value, bool accumulative)
        {
            if (accumulative) currentStamina -= value * Time.deltaTime;
            else currentStamina -= value;
            if (currentStamina < 0)
                currentStamina = 0;
        }

        public void DeathBehaviour()
        {
            // lock the player input
            lockMovement = true;
            // change the culling mode to render the animation until finish
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            // trigger die animation            
            if (deathBy == DeathBy.Animation || deathBy == DeathBy.AnimationWithRagdoll)
                animator.SetBool("isDead", true);
        }

        void CheckHealth()
        {
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                if (onDead != null) onDead.Invoke(gameObject);
            }
        }

        void HealthRecovery()
        {
            if (currentHealth <= 0) return;
            if (currentHealthRecoveryDelay > 0)
            {
                currentHealthRecoveryDelay -= Time.deltaTime;
            }
            else
            {
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                if (currentHealth < maxHealth)
                    currentHealth = Mathf.Lerp(currentHealth, maxHealth, healthRecovery * Time.deltaTime);
            }
        }

        void CheckStamina()
        {
            // check how much stamina this action will consume
            if (isSprinting)
            {
                currentStaminaRecoveryDelay = 0.25f;
                ReduceStamina(sprintStamina, true);
            }
        }

        void StaminaRecovery()
        {
            if (currentStaminaRecoveryDelay > 0)
            {
                currentStaminaRecoveryDelay -= Time.deltaTime;
            }
            else
            {
                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
                if (currentStamina < maxStamina)
                    currentStamina += staminaRecovery;
            }
        }

        #endregion

        #region Locomotion 

        protected bool freeLocomotionConditions
        {
            get
            {
                if (locomotionType.Equals(LocomotionType.OnlyStrafe)) isStrafing = true;
                return !isStrafing && !isUsingLadder && !landHigh && !ragdolled && !locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.OnlyFree);
            }
        }

        void ControlLocomotion()
        {
            if (lockMovement || currentHealth <= 0) return;

            if (freeLocomotionConditions)
                FreeMovement();     // free directional movement
            else
                StrafeMovement();   // move forward, backwards, strafe left and right
        }

        void StrafeMovement()
        {
            var _speed = Mathf.Clamp(input.y, -1f, 1f);
            var _direction = Mathf.Clamp(input.x, -1f, 1f);
            speed = _speed;
            direction = _direction;
            if (isSprinting) speed += 0.5f;
            if (direction >= 0.7 || direction <= -0.7 || speed <= 0.1) isSprinting = false;
        }

        void FreeMovement()
        {
            if (animator.IsInTransition(0)) return;

            // set speed to both vertical and horizontal inputs
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);
            speed = Mathf.Clamp(speed, 0, 1);
            // add 0.5f on sprint to change the animation on animator
            if (isSprinting) speed += 0.5f;
            if (stopMove) speed = 0f;

            var conditions = (!actions || quickStop || isRolling && rollControl);
            if (input != Vector2.zero && targetDirection.magnitude > 0.1f && conditions)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;

                // apply free directional rotation while not turning180 animations
                if (isGrounded || (!isGrounded && jumpAirControl))
                {
                    if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                    var euler = new Vector3(0, eulerY, 0);
                    if (animator.IsInTransition(0)) return;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), freeRotationSpeed * Time.fixedDeltaTime);
                }
                if (!keepDirection)
                    oldInput = input;
                if (Vector2.Distance(oldInput, input) > 0.9f && keepDirection)
                    keepDirection = false;
            }
        }

        protected void ControlSpeed(float velocity)
        {
            if (Time.deltaTime == 0) return;

            if (!actions)
            {
                Vector3 v = (animator.deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
                v.y = _rigidbody.velocity.y;
                //_rigidbody.velocity = v;
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, v, 20f * Time.deltaTime);
            }
            else
            {
                Vector3 v = Vector3.zero;
                v.y = _rigidbody.velocity.y;
                _rigidbody.velocity = v;
                transform.position = animator.rootPosition;
            }
        }

        protected void StopMove()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded) return;

            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position + new Vector3(0, stopMoveHeight, 0), targetDirection.normalized);

            if (Physics.Raycast(ray, out hitinfo, _capsuleCollider.radius + stopMoveDistance, stopMoveLayer) && !isUsingLadder)
            {
                var hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                if (hitinfo.distance <= stopMoveDistance && hitAngle > 85)
                    stopMove = true;
                else if (hitAngle >= slopeLimit + 1f && hitAngle <= 85)
                    stopMove = true;
            }
            else if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer) && !isUsingLadder)
            {
                var hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);
                if (hitAngle >= slopeLimit + 1f && hitAngle <= 85)
                    stopMove = true;
            }
            else
                stopMove = false;
        }

        #endregion

        #region Jump Methods

        protected void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            if (isJumping)
            {
                jumpCounter -= Time.deltaTime;

                if (jumpCounter <= 0)
                {
                    jumpCounter = 0;
                    isJumping = false;
                }
            }

            // apply extra force to the jump height
            _rigidbody.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
        }

        public void AirControl()
        {
            if (isGrounded) return;
            if (!jumpFwdCondition) return;

            var vel = transform.forward * jumpForward * speed;
            vel.y = _rigidbody.velocity.y;

            EnableGravityAndCollision(0f);

            if (jumpAirControl)
            {
                if (isStrafing)
                {
                    _rigidbody.velocity = vel;
                    _rigidbody.AddForce(transform.forward * (jumpForward * speed), ForceMode.VelocityChange);
                    _rigidbody.AddForce(transform.right * (jumpForward * direction), ForceMode.VelocityChange);
                }
                else
                {
                    _rigidbody.velocity = vel;
                    _rigidbody.AddForce(transform.forward * (jumpForward * speed), ForceMode.VelocityChange);
                }
            }
            else
                _rigidbody.AddForce(transform.forward * jumpForward, ForceMode.VelocityChange);
        }

        protected bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }

        #endregion

        #region Ground Check

        void CheckGround()
        {
            CheckGroundDistance();

            if (isDead)
            {
                isGrounded = true;
                return;
            }

            // change the physics material to very slip when not grounded
            _capsuleCollider.material = (isGrounded && GroundAngle() < slopeLimit) ? frictionPhysics : slippyPhysics;
            // we don't want to stick the character grounded if one of these bools is true
            bool checkGroundConditions = !jumpOver && !stepUp && !climbUp && !isUsingLadder && !isRolling;

            var magVel = (float)System.Math.Round(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude, 2);
            magVel = Mathf.Clamp(magVel, 0, 1);

            var groundCheckDistance = groundMinDistance;
            if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

            if (checkGroundConditions)
            {
                // clear the checkground to free the character to attack on air                
                var onStep = StepOffset();

                if (groundDistance <= 0.05f)
                {
                    isGrounded = true;
                    //if (!onStep) _rigidbody.velocity = Vector3.ProjectOnPlane(_rigidbody.velocity, groundHit.normal);
                    Sliding();
                }
                else
                {
                    if (groundDistance >= groundCheckDistance)
                    {
                        isGrounded = false;
                        // check vertical velocity
                        verticalVelocity = _rigidbody.velocity.y;
                        // apply extra gravity when falling
                        if (!onStep && !isJumping)
                            _rigidbody.AddForce(transform.up * extraGravity, ForceMode.VelocityChange);
                    }
                    else if (!onStep && !isJumping)
                    {
                        _rigidbody.AddForce(transform.up * (extraGravity * 2), ForceMode.VelocityChange);
                    }
                }
            }
        }

        void CheckGroundDistance()
        {
            if (isDead) return;
            if (_capsuleCollider != null)
            {
                // radius of the SphereCast
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;
                // position of the SphereCast origin starting at the base of the capsule
                Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                // ray for RayCast
                Ray ray1 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
                // ray for SphereCast
                Ray ray2 = new Ray(pos, -Vector3.up);
                // raycast for check the ground distance
                if (Physics.Raycast(ray1, out groundHit, colliderHeight / 2 + 2f, groundLayer))
                    dist = transform.position.y - groundHit.point.y;
                // sphere cast around the base of the capsule to check the ground distance
                if (Physics.SphereCast(ray2, radius, out groundHit, 2f, groundLayer))
                {
                    // check if sphereCast distance is small than the ray cast distance
                    if (dist > (groundHit.distance - _capsuleCollider.radius * 0.1f))
                        dist = (groundHit.distance - _capsuleCollider.radius * 0.1f);
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        void Sliding()
        {
            var onStep = StepOffset();
            var groundAngleTwo = 0f;
            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
                groundAngleTwo = Vector3.Angle(Vector3.up, hitinfo.normal);

            if (GroundAngle() > slopeLimit + 1f && GroundAngle() <= 85 &&
                groundAngleTwo > slopeLimit + 1f && groundAngleTwo <= 85 &&
                groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;
                isGrounded = false;
                var slideVelocity = (GroundAngle() - slopeLimit) * 5f;
                slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -slideVelocity, _rigidbody.velocity.z);
            }
            else
            {
                isSliding = false;
                isGrounded = true;
            }
        }

        bool StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded) return false;

            var hit = new RaycastHit();
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + transform.forward * ((_capsuleCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out hit, stepOffsetEnd - stepOffsetStart, groundLayer))
            {
                if (hit.point.y >= (transform.position.y) && hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var heightPoint = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, heightPoint, (speed * stepSmooth) * Time.fixedDeltaTime);

                    //var heightPoint = new Vector3(_rigidbody.velocity.x, hit.point.y + 0.1f, _rigidbody.velocity.z);
                    //_rigidbody.velocity = heightPoint * 10f * Time.fixedDeltaTime;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Colliders Check

        void ControlCapsuleHeight()
        {
            if (isCrouching || isRolling || landHigh)
            {
                _capsuleCollider.center = colliderCenter / 1.4f;
                _capsuleCollider.height = colliderHeight / 1.4f;
            }
            else if (isUsingLadder)
            {
                _capsuleCollider.radius = colliderRadius / 1.25f;
            }
            else
            {
                // back to the original values
                _capsuleCollider.center = colliderCenter;
                _capsuleCollider.radius = colliderRadius;
                _capsuleCollider.height = colliderHeight;
            }
        }

        protected void DisableGravityAndCollision()
        {
            animator.SetFloat("InputHorizontal", 0f);
            animator.SetFloat("InputVertical", 0f);
            animator.SetFloat("VerticalVelocity", 0f);
            _rigidbody.useGravity = false;
            _capsuleCollider.isTrigger = true;
        }

        /// <summary>
        /// Turn rigidbody gravity on the uncheck the capsulle collider as Trigger when the animation has finish playing
        /// </summary>
        /// <param name="normalizedTime">Check the value of your animation Exit Time and insert here</param>
        protected void EnableGravityAndCollision(float normalizedTime)
        {
            // enable collider and gravity at the end of the animation
            if (baseLayerInfo.normalizedTime >= normalizedTime)
            {
                _capsuleCollider.isTrigger = false;
                _rigidbody.useGravity = true;
            }
        }
        #endregion

        #region Camera Methods

        public virtual void RotateToTarget(Transform target)
        {
            if (target)
            {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                targetRotation = Quaternion.Euler(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), strafeRotationSpeed * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void UpdateTargetDirection(Transform referenceTransform = null)
        {
            if (referenceTransform && !rotateByWorld)
            {
                var forward = keepDirection ? referenceTransform.forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                forward = keepDirection ? forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0; //set to 0 because of referenceTransform rotation on the X axis

                //get the right-facing direction of the referenceTransform
                var right = keepDirection ? referenceTransform.right : referenceTransform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
                targetDirection = keepDirection ? targetDirection : new Vector3(input.x, 0, input.y);
        }

        #endregion

        #region Ragdoll 

        void CheckRagdoll()
        {
            if (ragdollVel == 0) return;

            // check your verticalVelocity and assign a value on the variable RagdollVel at the Player Inspector
            if (verticalVelocity <= ragdollVel && groundDistance <= 0.1f)
            {
                transform.SendMessage("ActivateRagdoll", SendMessageOptions.DontRequireReceiver);
            }
        }

        public override void ResetRagdoll()
        {
            lockMovement = false;
            verticalVelocity = 0f;
            ragdolled = false;
            _rigidbody.WakeUp();

            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _capsuleCollider.isTrigger = false;
        }

        public override void EnableRagdoll()
        {
            animator.SetFloat("InputHorizontal", 0f);
            animator.SetFloat("InputVertical", 0f);
            animator.SetFloat("VerticalVelocity", 0f);
            isUsingLadder = false;
            ragdolled = true;
            _capsuleCollider.isTrigger = true;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            lockMovement = true;
        }

        #endregion

        public virtual string DebugInfo(string additionalText = "")
        {
            string debugInfo = string.Empty;
            if (debugWindow)
            {
                float delta = Time.smoothDeltaTime;
                float fps = 1 / delta;

                debugInfo =
                    "FPS " + fps.ToString("#,##0 fps") + "\n" +
                    "inputVertical = " + input.y.ToString("0.0") + "\n" +
                    "inputHorizontal = " + input.x.ToString("0.0") + "\n" +
                    "verticalVelocity = " + verticalVelocity.ToString("0.00") + "\n" +
                    "groundDistance = " + groundDistance.ToString("0.00") + "\n" +
                    "groundAngle = " + GroundAngle().ToString("0.00") + "\n" +
                    "useGravity = " + _rigidbody.useGravity.ToString() + "\n" +
                    "colliderIsTrigger = " + _capsuleCollider.isTrigger.ToString() + "\n" +

                    "\n" + "--- Movement Bools ---" + "\n" +
                    "onGround = " + isGrounded.ToString() + "\n" +
                    "lockPlayer = " + lockMovement.ToString() + "\n" +
                    "stopMove = " + stopMove.ToString() + "\n" +
                    "sliding = " + isSliding.ToString() + "\n" +
                    "sprinting = " + isSprinting.ToString() + "\n" +
                    "crouch = " + isCrouching.ToString() + "\n" +
                    "strafe = " + isStrafing.ToString() + "\n" +
                    "quickStop = " + quickStop.ToString() + "\n" +
                    "landHigh = " + landHigh.ToString() + "\n" +

                    "\n" + "--- Actions Bools ---" + "\n" +
                    "roll = " + isRolling.ToString() + "\n" +
                    "stepUp = " + stepUp.ToString() + "\n" +
                    "jumpOver = " + jumpOver.ToString() + "\n" +
                    "climbUp = " + climbUp.ToString() + "\n" +
                    "isJumping = " + isJumping.ToString() + "\n" +
                    "isUsingLadder = " + isUsingLadder.ToString() + "\n" +
                    "ragdoll = " + ragdolled.ToString() + "\n" +
                    "actions = " + actions.ToString() + "\n" + additionalText;
            }
            return debugInfo;
        }
    }
}