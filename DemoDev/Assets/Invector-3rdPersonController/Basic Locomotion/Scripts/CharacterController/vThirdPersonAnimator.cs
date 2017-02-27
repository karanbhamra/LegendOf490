using UnityEngine;
using System.Collections;

namespace Invector.CharacterController
{
    public abstract class vThirdPersonAnimator : vThirdPersonMotor
    {
        #region Variables
        // generate a random idle animation
        private float randomIdleCount, randomIdle;
        // used to lerp the head track
        private Vector3 lookPosition;
        // match cursorObject to help animation to reach their cursorObject
        [HideInInspector]
        public Transform matchTarget;
        // head track control, if you want to turn off at some point, make it 0
        [HideInInspector]
        public float lookAtWeight;
        [HideInInspector]
        public float oldSpeed;
        public float speedTime
        {
            get
            {
                var _speed = animator.GetFloat("InputVertical");
                var acceleration = (_speed - oldSpeed) / Time.fixedDeltaTime;
                oldSpeed = _speed;
                return Mathf.Round(acceleration);
            }
        }

        private bool triggerDieBehaviour;
        [HideInInspector]
        public bool exitingLadder;

        int baseLayer { get { return animator.GetLayerIndex("Base Layer"); } }
        int underBodyLayer { get { return animator.GetLayerIndex("UnderBody"); } }
        int rightArmLayer { get { return animator.GetLayerIndex("RightArm"); } }
        int leftArmLayer { get { return animator.GetLayerIndex("LeftArm"); } }
        int upperBodyLayer { get { return animator.GetLayerIndex("UpperBody"); } }
        int fullbodyLayer { get { return animator.GetLayerIndex("FullBody"); } }

        #endregion

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            LayerControl();
            ActionsControl();

            RandomIdle();

            // use vTriggerAction to trigger
            JumpOverAnimation();
            ClimbUpAnimation();
            StepUpAnimation();

            // trigger by input
            RollAnimation();
            LadderAnimation();

            // trigger at any time using conditions
            TriggerQuickStopAnimation();
            TriggerLandHighAnimation();

            LocomotionAnimation();
            DeadAnimation();
        }

        void LayerControl()
        {
            baseLayerInfo = animator.GetCurrentAnimatorStateInfo(baseLayer);
            underBodyInfo = animator.GetCurrentAnimatorStateInfo(underBodyLayer);
            rightArmInfo = animator.GetCurrentAnimatorStateInfo(rightArmLayer);
            leftArmInfo = animator.GetCurrentAnimatorStateInfo(leftArmLayer);
            upperBodyInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            fullBodyInfo = animator.GetCurrentAnimatorStateInfo(fullbodyLayer);
        }

        void ActionsControl()
        {
            // to have better control of your actions, you can filter the animations state using bools 
            // this way you can know exactly what animation state the character is playing

            stepUp = baseLayerInfo.IsName("StepUp");
            jumpOver = baseLayerInfo.IsName("JumpOver");
            climbUp = baseLayerInfo.IsName("ClimbUp");

            landHigh = baseLayerInfo.IsName("LandHigh");
            quickStop = baseLayerInfo.IsName("QuickStop");

            isRolling = baseLayerInfo.IsName("Roll");

            // resume the states of the ladder in one bool 
            isUsingLadder =
                baseLayerInfo.IsName("EnterLadderBottom") ||
                baseLayerInfo.IsName("ExitLadderBottom") ||
                baseLayerInfo.IsName("ExitLadderTop") ||
                baseLayerInfo.IsName("EnterLadderTop") ||
                baseLayerInfo.IsName("ClimbLadder");

            // locks player movement while a animation with tag 'LockMovement' is playing
            lockMovement = baseLayerInfo.IsTag("LockMovement") || upperBodyInfo.IsTag("LockMovement") || fullBodyInfo.IsTag("LockMovement");
            // ! -- you can add the Tag "CustomAction" into a AnimatonState and the character will not perform any Melee action -- !            
            doingCustomAction = baseLayerInfo.IsTag("CustomAction") || upperBodyInfo.IsTag("CustomAction") || fullBodyInfo.IsTag("CustomAction");
        }


        #region Locomotion Animations

        void RandomIdle()
        {
            if (input != Vector2.zero || actions) return;

            if (randomIdleTime > 0)
            {
                if (input.sqrMagnitude == 0 && !isCrouching && _capsuleCollider.enabled && isGrounded)
                {
                    randomIdleCount += Time.fixedDeltaTime;
                    if (randomIdleCount > 6)
                    {
                        randomIdleCount = 0;
                        animator.SetTrigger("IdleRandomTrigger");
                        animator.SetInteger("IdleRandom", Random.Range(1, 4));
                    }
                }
                else
                {
                    randomIdleCount = 0;
                    animator.SetInteger("IdleRandom", 0);
                }
            }
        }

        void LocomotionAnimation()
        {
            animator.SetBool("IsStrafing", isStrafing);
            animator.SetBool("IsCrouching", isCrouching);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("GroundDistance", groundDistance);
            animator.SetFloat("InputMagnitude", Mathf.Clamp(input.sqrMagnitude, 0f, 1f), .2f, Time.deltaTime);

            if (!isGrounded)
                animator.SetFloat("VerticalVelocity", verticalVelocity);

            if (isStrafing)
            {
                // strafe movement get the input 1 or -1
                animator.SetFloat("InputHorizontal", lockMovement ? 0f : direction, 0.25f, Time.deltaTime);
                StrafeTurningAnimation();
            }

            animator.SetFloat("InputVertical", !stopMove || lockMovement ? speed : 0f, 0.25f, Time.deltaTime);
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (isGrounded)
            {
                transform.rotation = animator.rootRotation;

                var speedDir = new Vector2(direction, speed);
                var strafeSpeed = (isSprinting ? 1.5f : 1f) * Mathf.Clamp(speedDir.magnitude, 0f, 1f);
                // strafe extra speed
                if (isStrafing)
                {
                    if (strafeSpeed <= 0.5f)
                        ControlSpeed(strafeWalkSpeed);
                    else if (strafeSpeed > 0.5f && strafeSpeed <= 1f)
                        ControlSpeed(strafeRunningSpeed);
                    else
                        ControlSpeed(strafeSprintSpeed);

                    if (isCrouching)
                        ControlSpeed(strafeCrouchSpeed);
                }
                else if (!isStrafing)
                {
                    // free extra speed                
                    if (speed <= 0.5f)
                        ControlSpeed(freeWalkSpeed);
                    else if (speed > 0.5 && speed <= 1f)
                        ControlSpeed(freeRunningSpeed);
                    else
                        ControlSpeed(freeSprintSpeed);

                    if (isCrouching)
                        ControlSpeed(freeCrouchSpeed);
                }
            }
        }

        void StrafeTurningAnimation()
        {
            if (!isStrafing)
            {
                animator.SetFloat("StrafeAngle", 0);
                return;
            }
            else
            {
                if (Vector3.Distance(targetRotation.eulerAngles, transform.rotation.eulerAngles) > 0)
                {
                    var newAngle = targetRotation.eulerAngles - transform.rotation.eulerAngles;
                    var _ang = newAngle.NormalizeAngle().y;
                    if (!doingCustomAction && isGrounded && !lockMovement)
                        animator.SetFloat("StrafeAngle", _ang);
                    else
                        animator.SetFloat("StrafeAngle", 0f);
                }
            }
        }

        #endregion


        #region Action Animations       

        void StepUpAnimation()
        {
            if (!stepUp) return;

            DisableGravityAndCollision();
            MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0f, 0.3f);
            EnableGravityAndCollision(0.8f);
        }

        void JumpOverAnimation()
        {
            if (!jumpOver) return;

            DisableGravityAndCollision();

            MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0f, 0.6f);

            EnableGravityAndCollision(0.8f);
        }

        void ClimbUpAnimation()
        {
            if (!climbUp) return;

            DisableGravityAndCollision();
            MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0f, 0.3f);
            EnableGravityAndCollision(0.9f);
        }

        void LadderAnimation()
        {
            if (!isUsingLadder) exitingLadder = false;

            LadderBottom();
            LadderTop();
        }

        void LadderBottom()
        {
            // enter ladder from bottom
            if (baseLayerInfo.IsName("EnterLadderBottom"))
            {
                DisableGravityAndCollision();

                if (baseLayerInfo.normalizedTime < 0.25f && !animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 0, 0.1f), 0), 0f, 0.25f);
                else if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), 0.25f, 0.7f);
            }

            // exit ladder bottom
            if (baseLayerInfo.IsName("ExitLadderBottom"))
            {
                Invoke("ResetActionState", 0.1f);
                EnableGravityAndCollision(0.1f);
            }
        }

        void LadderTop()
        {
            // enter ladder from top            
            if (baseLayerInfo.IsName("EnterLadderTop"))
            {
                DisableGravityAndCollision();

                if (baseLayerInfo.normalizedTime < 0.25f && !animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 0, 0.1f), 1f), 0f, 0.25f);
                else if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 1f), 0.25f, 0.7f);
            }

            // exit ladder top
            if (baseLayerInfo.IsName("ExitLadderTop"))
            {
                Invoke("ResetActionState", 0.1f);
                EnableGravityAndCollision(0.8f);
            }
        }

        void RollAnimation()
        {
            if (isRolling)
            {
                autoCrouch = true;

                if (isStrafing && (input != Vector2.zero || speed > 0.25f))
                {
                    // check the right direction for rolling if you are strafing
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, 25f * Time.fixedDeltaTime, 0.0f);
                    var rot = Quaternion.LookRotation(newDir);
                    var eulerAngles = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                    transform.eulerAngles = eulerAngles;
                }

                if (baseLayerInfo.normalizedTime > 0.1f && baseLayerInfo.normalizedTime < 0.3f)
                    _rigidbody.useGravity = false;

                // prevent the character to rolling up 
                if (verticalVelocity >= 1)
                    _rigidbody.velocity = Vector3.ProjectOnPlane(_rigidbody.velocity, groundHit.normal);

                // reset the rigidbody a little ealier to the character fall while on air
                if (baseLayerInfo.normalizedTime > 0.3f)
                    _rigidbody.useGravity = true;
            }
        }

        void DeadAnimation()
        {
            if (!isDead) return;

            if (!triggerDieBehaviour)
            {
                triggerDieBehaviour = true;
                DeathBehaviour();
            }

            // death by animation
            if (deathBy == DeathBy.Animation)
            {
                if (fullBodyInfo.IsName("Dead"))
                {
                    if (fullBodyInfo.normalizedTime >= 0.99f && groundDistance <= 0.15f)
                        RemoveComponents();
                }
            }
            // death by animation & ragdoll after a time
            else if (deathBy == DeathBy.AnimationWithRagdoll)
            {
                if (fullBodyInfo.IsName("Dead"))
                {
                    // activate the ragdoll after the animation finish played
                    if (fullBodyInfo.normalizedTime >= 0.8f)
                        SendMessage("ActivateRagdoll", SendMessageOptions.DontRequireReceiver);
                }
            }
            // death by ragdoll
            else if (deathBy == DeathBy.Ragdoll)
                SendMessage("ActivateRagdoll", SendMessageOptions.DontRequireReceiver);
        }

        void ResetActionState()
        {
            animator.SetInteger("ActionState", 0);
        }

        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
        {
            if (animator.isMatchingTarget || animator.IsInTransition(0))
                return;

            float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

            if (normalizeTime > normalisedEndTime)
                return;

            animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
        }

        #endregion


        #region Trigger Animations       

        public void TriggerAnimationState(string animationClip, float transition)
        {
            animator.CrossFadeInFixedTime(animationClip, transition);
        }

        public bool IsAnimatorTag(string tag)
        {
            if (animator == null) return false;
            if (baseLayerInfo.IsTag(tag)) return true;
            if (underBodyInfo.IsTag(tag)) return true;
            if (rightArmInfo.IsTag(tag)) return true;
            if (leftArmInfo.IsTag(tag)) return true;
            if (upperBodyInfo.IsTag(tag)) return true;
            if (fullBodyInfo.IsTag(tag)) return true;
            return false;
        }

        void TriggerQuickStopAnimation()
        {
            if (!quickStopAnim) return;
            var _speedTime = speedTime;
            bool quickStopConditions = !actions && isGrounded && canQuickStop && !isCrouching;

            // make a quickStop when release the button while running            
            if (_speedTime < -5f && quickStopConditions)
                animator.CrossFadeInFixedTime("QuickStop", 0.1f);
        }

        protected IEnumerator ResetQuickStop()
        {
            canQuickStop = false;
            yield return new WaitForSeconds(1f);
            canQuickStop = true;
        }

        void TriggerLandHighAnimation()
        {
            if (landHigh)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                vInput.instance.GamepadVibration(0.25f);
#endif
            }
        }

        #endregion


    }
}