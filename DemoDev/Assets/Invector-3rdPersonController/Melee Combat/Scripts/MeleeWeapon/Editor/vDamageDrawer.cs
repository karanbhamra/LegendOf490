﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Invector.CharacterController
{
    [CustomPropertyDrawer(typeof(Damage))]
    public class vDamageDrawer : PropertyDrawer
    {
        bool isOpen;
        GUISkin skin;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldSkin = GUI.skin;
            if (!skin) skin = Resources.Load("skin") as GUISkin;
            if (skin) GUI.skin = skin;

            GUI.Box(position, "");
            position.width -= 10;
            position.height = 15;
            position.y += 5f;
            position.x += 5;
            isOpen = GUI.Toggle(position, isOpen, property.displayName + " Options", EditorStyles.miniButton);

            if (isOpen)
            {
                var value = property.FindPropertyRelative("value");
                var staminaBlockCost = property.FindPropertyRelative("staminaBlockCost");
                var staminaRecoveryDelay = property.FindPropertyRelative("staminaRecoveryDelay");
                var ignoreDefense = property.FindPropertyRelative("ignoreDefense");
                var activeRagdoll = property.FindPropertyRelative("activeRagdoll");
                var hitreactionID = property.FindPropertyRelative("reaction_id");
                var obj = (property.serializedObject.targetObject as MonoBehaviour);

                bool valid = true;
                if (obj != null)
                {
                    var parent = obj.transform.parent;
                    if (parent != null)
                    {
                        var manager = parent.GetComponentInParent<vMeleeManager>();
                        valid = obj.GetType() == typeof(vMeleeWeapon) || manager == null;
                    }
                }

                if (value != null && valid)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, value);
                }
                if (staminaBlockCost != null)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, staminaBlockCost);
                }
                if (staminaRecoveryDelay != null)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, staminaRecoveryDelay);
                }
                if (ignoreDefense != null)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, ignoreDefense);
                }
                if (activeRagdoll != null)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, activeRagdoll);
                }
                if (hitreactionID != null)
                {
                    position.y += 20;
                    EditorGUI.PropertyField(position, hitreactionID);
                }
            }

            GUI.skin = oldSkin;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return !isOpen ? 25 : 145;
        }
    }
}