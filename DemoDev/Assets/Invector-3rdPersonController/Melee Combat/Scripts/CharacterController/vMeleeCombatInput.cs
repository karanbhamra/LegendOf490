using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Invector.ItemManager;
using Invector.EventSystems;
using System;

namespace Invector.CharacterController
{
    // here you can modify the Melee Combat inputs
    // if you want to modify the Basic Locomotion inputs, go to the vThirdPersonInput

    public class vMeleeCombatInput : vThirdPersonInput, vIMeleeFighter
    {
        #region Variables
        protected vMeleeManager meleeManager;
        protected vItemManager itemManager;
        protected bool isAttacking;
        protected bool isBlocking;

        [Header("MeleeCombat Inputs")]
        public GenericInput weakAttackInput = new GenericInput("Mouse0", "RB", "RB");
        public GenericInput strongAttackInput = new GenericInput("Alpha1", false, "RT", true, "RT", false);
        public GenericInput blockInput = new GenericInput("Mouse1", "LB", "LB");
        public GenericInput lockOnInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");

        #endregion

        protected override void Start()
        {
            base.Start();
            itemManager = GetComponent<vItemManager>();
        }

        protected override void InputHandle()
        {            
            if (cc == null) return;
                       
            if (MeleeAttackConditions)
            {
                MeleeWeakAttackInput();
                MeleeStrongAttackInput();
                BlockingInput();
            }

            if (!isAttacking)
            {
                base.InputHandle();
                UpdateCombatAnimations();
            }
            //else
                //cc.input = Vector2.zero;

            LockOnInput();
        }

        #region MeleeCombat Input Methods

        /// <summary>
        /// WEAK ATK INPUT
        /// </summary>
        protected virtual void MeleeWeakAttackInput()
        {
            if (cc.animator == null) return;

            if(weakAttackInput.GetButtonDown() && MeleeAttackStaminaConditions())
            {
                cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
                cc.animator.SetTrigger("WeakAttack");
            }         
        }

        /// <summary>
        /// STRONG ATK INPUT
        /// </summary>
        protected virtual void MeleeStrongAttackInput()
        {
            if (cc.animator == null) return;          

            if (strongAttackInput.GetButtonDown() && MeleeAttackStaminaConditions())
            {
                cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
                cc.animator.SetTrigger("StrongAttack");
            }
        }       

        /// <summary>
        /// BLOCK INPUT
        /// </summary>
        protected virtual void BlockingInput()
        {            
            if (cc.animator == null) return;

            isBlocking = blockInput.GetButton() && cc.currentStamina > 0;
        }

        /// <summary>
        /// ACTION INPUT
        /// </summary>
        protected override void ActionInput()
        {
            if (cc.triggerAction == null) return;

            vItemCollection collection = cc.triggerAction.GetComponent<vItemCollection>();

            if(actionInput.GetButtonDown() && !cc.doingCustomAction)
            {
                cc.TriggerAction(cc.triggerAction);
                if (collection && itemManager) CollectItem(collection);
            }          
            else if(cc.triggerAction.autoAction)
                if (collection && itemManager) CollectItem(collection);
        }

        /// <summary>
        /// LOCK ON INPUT
        /// </summary>
        void LockOnInput()
        {
            // check the input you select on the Action Input Painel 
            // only do the lockon if the character is on Free Movement
            if (!cc.locomotionType.Equals(vThirdPersonController.LocomotionType.OnlyFree))
            {
                if (lockOnInput.GetButtonDown() && !cc.actions)
                {                    
                    tpCamera.UpdateLockOn(cc.isStrafing);
                }
                else if(!cc.isStrafing && tpCamera.lockTarget != null)
                {
                    tpCamera.UpdateLockOn(false);
                }
            }

            SwitchTargetsInput();
        }

        /// <summary>
        /// SWITCH TARGETS INPUT
        /// </summary>
        void SwitchTargetsInput()
        {
            if (tpCamera == null) return;

            if (tpCamera.lockTarget)
            {
                // switch between targets using Keyboard
                if (inputDevice == InputDevice.MouseKeyboard)
                {
                    if (Input.GetKey(KeyCode.X))
                        tpCamera.gameObject.SendMessage("ChangeTarget", 1, SendMessageOptions.DontRequireReceiver);
                    else if (Input.GetKey(KeyCode.Z))
                        tpCamera.gameObject.SendMessage("ChangeTarget", -1, SendMessageOptions.DontRequireReceiver);
                }
                // switch between targets using GamePad
                else if (inputDevice == InputDevice.Joystick)
                {
                    var value = Input.GetAxisRaw("RightAnalogHorizontal");
                    if (value == 1)
                        tpCamera.gameObject.SendMessage("ChangeTarget", 1, SendMessageOptions.DontRequireReceiver);
                    else if (value == -1f)
                        tpCamera.gameObject.SendMessage("ChangeTarget", -1, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        #endregion

        #region Conditions

        protected virtual bool MeleeAttackStaminaConditions()
        {
            var result = cc.currentStamina - meleeManager.GetAttackStaminaCost();
            return result >= 0;
        }

        protected virtual bool MeleeAttackConditions
        {
            get
            {
                if (meleeManager == null) meleeManager = GetComponent<vMeleeManager>();
                return meleeManager != null && !cc.doingCustomAction && !cc.lockMovement;
            }
        }

        #endregion

        #region Update Animations

        protected virtual void UpdateCombatAnimations()
        {
            if (cc.animator == null || meleeManager == null) return;
            cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
            cc.animator.SetInteger("DefenseID", meleeManager.GetDefenseID());
            cc.animator.SetBool("IsBlocking", isBlocking);
            cc.animator.SetFloat("MoveSet_ID", meleeManager.GetMoveSetID(), .1f, Time.deltaTime);
        }                

        public virtual void SetRecoil(int id)
        {
            if (cc.animator == null) return;
            cc.animator.SetTrigger("TriggerRecoil");
            cc.animator.SetInteger("RecoilID", id);
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        #endregion

        #region Trigger Verifications

        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
        }

        protected virtual void CollectItem(vItemCollection collection)
        {
            foreach (ItemReference reference in collection.items)
            {
                itemManager.AddItem(reference.id, reference.amount);
            }
            collection.OnCollectItems(gameObject);
        }

        #endregion

        #region Melee Methods

        public void OnEnableAttack()
        {
            cc.currentStaminaRecoveryDelay = meleeManager.GetAttackStaminaRecoveryDelay();
            cc.currentStamina -= meleeManager.GetAttackStaminaCost();
            isAttacking = true;
        }

        public void OnDisableAttack()
        {
            isAttacking = false;
        }

        public void ResetAttackTriggers()
        {
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        public void BreakAttack(int breakAtkID)
        {
            ResetAttackTriggers();
            OnRecoil(breakAtkID);
        }

        public void OnRecoil(int recoilID)
        {
            cc.animator.SetInteger("RecoilID", recoilID);
            cc.animator.SetTrigger("TriggerRecoil");
        }

        public void OnReceiveAttack(Damage damage, vIMeleeFighter attacker)
        {
            // character is blocking
            if (!damage.ignoreDefense && isBlocking && meleeManager != null && meleeManager.CanBlockAttack(attacker.Character().transform.position))
            {
                var damageReduction = meleeManager.GetDefenseRate();
                if (damageReduction > 0)
                    damage.ReduceDamage(damageReduction);
                if (attacker != null && meleeManager != null && meleeManager.CanBreakAttack())
                    attacker.OnRecoil(meleeManager.GetDefenseRecoilID());
                meleeManager.OnDefense();
                cc.currentStaminaRecoveryDelay = damage.staminaRecoveryDelay;
                cc.currentStamina -= damage.staminaBlockCost;
            }         
            // apply damage
            cc.TakeDamage(damage, !isBlocking);
        }

        public vCharacter Character()
        {
            return cc;
        }

        #endregion

    }
}

