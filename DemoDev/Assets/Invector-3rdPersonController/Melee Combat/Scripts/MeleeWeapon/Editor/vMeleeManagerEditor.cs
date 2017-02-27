﻿using UnityEngine;
using System.Collections;
using UnityEditorInternal;
using UnityEditor.Animations;
using UnityEditor;
using System.Collections.Generic;
using Invector;
using System;

[CustomEditor(typeof(vMeleeManager))]
public class vMeleeManagerEditor : Editor
{
    #region variables
     
    vMeleeManager manager;
    Animator animator;
    
    int selectedID;
    int seletedHitboxIndex;
    int toolBarBodyMembers;
    int damagePercentage;

    bool selLeftArm, selRightArm, selLeftLeg, selRightLeg, selHead, selTorso;
    bool showEvents;
    bool showDefaultInfo;
    bool inAddBodyMember;
    bool isHuman;
    bool inCreateHitBox;
    bool inChangeHitBoxCollider;

    Transform leftLowerArm, rightLowerArm, leftLowerLeg, rightLowerLeg;
    BodyMember currentBodyMember;
    BodyMember extraBodyMember;        
    Component hitCollider;
    GUISkin skin;

    vHitBoxType triggerType;

    string seletedBone;
    string[] ignoreProperties = new string[] { "m_Script", "Members", "defaultDamage", "hitProperties", "leftWeapon", "rightWeapon", "onDamageHit", "onRecoilHit" };

    #endregion

    [MenuItem("Invector/Melee Combat/Components/MeleeManager")]
    static void MenuComponent()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<vMeleeManager>();
        else
            Debug.Log("Please select a Character to add the component.");
    }

    void OnSceneGUI()
    {
        var renderers = manager.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
            EditorUtility.SetSelectedWireframeHidden(renderer, false);
        DrawRecoilRange();
    }

    void OnEnable()
    {
        manager = (vMeleeManager)target;
        if (AssetDatabase.Contains(manager.gameObject)) return;
        CreateDefaultBodyMembers();
        skin = Resources.Load("skin") as GUISkin;
    }

    void CreateDefaultBodyMembers()
    {
        animator = manager.GetComponent<Animator>();
        if (animator && animator.isHuman)
        {
            leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            CheckSingleHitBoxe(leftLowerArm, vHumanBones.LeftLowerArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            CheckSingleHitBoxe(rightLowerArm, vHumanBones.RightLowerArm);
            leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            CheckSingleHitBoxe(leftLowerLeg, vHumanBones.LeftLowerLeg);
            rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            CheckSingleHitBoxe(rightLowerLeg, vHumanBones.RightLowerLeg);
        }
    }

    void CheckSingleHitBoxe(Transform transform, vHumanBones bodyPart, bool debug = false)
    {
        if (transform)
        {
            vMeleeAttackObject attackObject = transform.GetComponent<vMeleeAttackObject>();
            var _hitBoxes = transform.GetComponentsInChildren<vHitBox>();
            var validHitBoxes = _hitBoxes.vToList().FindAll(hitBox => hitBox.transform.parent == attackObject.transform);
            if (attackObject == null)
            {
                attackObject = transform.gameObject.AddComponent<vMeleeAttackObject>();
            }
            attackObject.hitBoxes = validHitBoxes;
            if (manager && manager.Members!=null)
            {
                var bodyMembers = manager.Members.FindAll(member => member.bodyPart == bodyPart.ToString());
                if (bodyMembers.Count > 0)
                {
                    bodyMembers[0].isHuman = true;
                    bodyMembers[0].attackObject = attackObject;
                    bodyMembers[0].bodyPart = bodyPart.ToString();
                    bodyMembers[0].transform = transform;
                    if (bodyMembers.Count > 1)
                    {
                        for (int i = 1; i < bodyMembers.Count; i++)
                        {
                            manager.Members.Remove(bodyMembers[i]);
                        }
                    }
                    EditorUtility.SetDirty(manager);
                }
                else
                {
                    BodyMember bodyMember = new BodyMember();
                    bodyMember.isHuman = true;
                    bodyMember.attackObject = attackObject;
                    bodyMember.bodyPart = bodyPart.ToString();
                    bodyMember.transform = transform;
                    manager.Members.Add(bodyMember);
                    EditorUtility.SetDirty(manager);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        var oldSkin = GUI.skin;
        GUI.skin = skin;
        serializedObject.Update();
        var script = serializedObject.FindProperty("m_Script");

        GUILayout.BeginVertical("Melee Manager by Invector", "window");
        GUILayout.Space(30);

        if (script != null)
            EditorGUILayout.PropertyField(script);

        GUI.enabled = !AssetDatabase.Contains(manager.gameObject);
        if (manager.Members == null || manager.Members.Count == 0)
        {
            if (GUILayout.Button("Create Default Body Members", EditorStyles.miniButton, GUILayout.ExpandWidth(true))) CreateDefaultBodyMembers();
        }
        GUILayout.BeginVertical("box");

        OpenCloseDefaultInfo();
        OpenCloseEvents(oldSkin);
        AddExtraBodyPart();        
       
        GUI.enabled = true;
        
        GUILayout.EndVertical();

        var seletedBodyMember = manager.Members.Find(member => member.bodyPart == seletedBone);
        GUILayout.BeginVertical(seletedBodyMember != null ? "highlightBox" : "box");
        DrawBodyMemberToogles();

        if (seletedBodyMember != null)
        {
            bool canRemove = seletedBodyMember.bodyPart != vHumanBones.LeftLowerArm.ToString() && seletedBodyMember.bodyPart != vHumanBones.RightLowerArm.ToString() &&
                             seletedBodyMember.bodyPart != vHumanBones.LeftLowerLeg.ToString() && seletedBodyMember.bodyPart != vHumanBones.RightLowerLeg.ToString();
            DrawBodyMember(ref seletedBodyMember, seletedBodyMember.bodyPart.ToString(), canRemove);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Box("Who you can Hit?", GUILayout.ExpandWidth(true));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hitProperties"), true);
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box");
        GUILayout.Box("Weapons");
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box");
        GUILayout.Label("LeftWeapon", EditorStyles.miniLabel);
        manager.leftWeapon = EditorGUILayout.ObjectField(manager.leftWeapon, typeof(vMeleeWeapon), true) as vMeleeWeapon;
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("RightWeapon", EditorStyles.miniLabel);
        manager.rightWeapon = EditorGUILayout.ObjectField(manager.rightWeapon, typeof(vMeleeWeapon), true) as vMeleeWeapon;
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();      
              
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    void OpenCloseEvents(GUISkin oldSkin)
    {
        var onDamageHit = serializedObject.FindProperty("onDamageHit");
        var onRecoilHit = serializedObject.FindProperty("onRecoilHit");

        GUILayout.BeginVertical(showEvents ? "highlightBox" : "box");
        showEvents = GUILayout.Toggle(showEvents, showEvents ? "Close Events" : "Open Events", EditorStyles.miniButton);
        GUI.skin = oldSkin;
        if (showEvents)
        {
            if (onDamageHit != null) EditorGUILayout.PropertyField(onDamageHit);
            if (onRecoilHit != null) EditorGUILayout.PropertyField(onRecoilHit);
        }
        GUI.skin = skin;
        GUILayout.EndVertical();
    }

    void OpenCloseDefaultInfo()
    {
        GUILayout.BeginVertical(showDefaultInfo ? "highlightBox" : "box");

        showDefaultInfo = GUILayout.Toggle(showDefaultInfo, showDefaultInfo ? "Close Default Info" : "Open Default Info", EditorStyles.miniButton);
        var oldSkin = GUI.skin;
        GUI.skin = oldSkin;
        if (showDefaultInfo)
        {
            manager.defaultDamage.value = EditorGUILayout.IntField("DefaultDamage", manager.defaultDamage.value);
            DrawPropertiesExcluding(serializedObject, ignoreProperties);
        }
        GUI.skin = skin;
        GUILayout.EndVertical();
    }

    void AddExtraBodyPart()
    {
        GUILayout.BeginVertical(inAddBodyMember ? "highlightBox" : "box");
        if (!inAddBodyMember && GUILayout.Button("Add Extra Body Member", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
        {
            extraBodyMember = new BodyMember();
            inAddBodyMember = true;
            isHuman = true;
        }
        if (inAddBodyMember)
            DrawAddExtraBodyMember();
        GUILayout.EndVertical();
    }

    void DrawRecoilRange()
    {
        var coll = manager.gameObject.GetComponent<Collider>();
        if (coll != null && manager != null && manager.hitProperties != null && manager.hitProperties.useRecoil && manager.hitProperties.drawRecoilGizmos)
        {
            Handles.DrawWireDisc(coll.bounds.center, Vector3.up, 0.5f);
            Handles.color = new Color(1, 0, 0, 0.2f);
            Handles.DrawSolidArc((Vector3)coll.bounds.center, Vector3.up, manager.transform.forward, (float)manager.hitProperties.recoilRange, 0.5f);
            Handles.DrawSolidArc((Vector3)coll.bounds.center, Vector3.up, manager.transform.forward, (float)-manager.hitProperties.recoilRange, 0.5f);
        }
    }

    void DrawBodyMemberToogles()
    {
        var bmleftLowerArm = manager.Members.Find(member => member.bodyPart == vHumanBones.LeftLowerArm.ToString());
        var bmrightLowerArm = manager.Members.Find(member => member.bodyPart == vHumanBones.RightLowerArm.ToString());
        var bmleftLowerLeg = manager.Members.Find(member => member.bodyPart == vHumanBones.LeftLowerLeg.ToString());
        var bmrightLowerLeg = manager.Members.Find(member => member.bodyPart == vHumanBones.RightLowerLeg.ToString());

        GUILayout.BeginVertical();
        GUILayout.Box("Body Members", GUILayout.ExpandWidth(true));

        GUILayout.EndVertical();
        // GUILayout.Box("Default Human Body Members", GUILayout.ExpandWidth(true));
        GUILayout.BeginHorizontal();
        if (bmleftLowerArm != null)
            BodyMemberToogle(bmleftLowerArm.bodyPart, ref bmleftLowerArm, "LeftLowerArm");
        if (bmrightLowerArm != null)
            BodyMemberToogle(bmrightLowerArm.bodyPart, ref bmrightLowerArm, "RightLowerArm");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (bmleftLowerLeg != null)
            BodyMemberToogle(bmleftLowerLeg.bodyPart, ref bmleftLowerLeg, "LeftLowerLeg");
        if (bmrightLowerLeg != null)
            BodyMemberToogle(bmrightLowerLeg.bodyPart, ref bmrightLowerLeg, "RightLowerLeg");
        GUILayout.EndHorizontal();

        //GUILayout.Box("Extra Human BodyMembers", GUILayout.ExpandWidth(true));
        for (int i = 0; i < manager.Members.Count; i++)
        {
            if (manager.Members[i] != bmleftLowerArm && manager.Members[i] != bmrightLowerArm &&
                manager.Members[i] != bmleftLowerLeg && manager.Members[i] != bmrightLowerLeg)
            {
                var bodyMember = manager.Members[i];
                BodyMemberToogle(bodyMember.bodyPart, ref bodyMember, bodyMember.bodyPart.ToString());
                CheckHitBoxes(manager.Members[i]);
            }
            else
                CheckHitBoxes(manager.Members[i], true);
        }
    }

    void CheckHitBoxes(BodyMember bodyMember, bool isDefault = false)
    {
        if (AssetDatabase.Contains(manager.gameObject)) return;
        var hitBoxes = bodyMember.transform.GetComponentsInChildren<vHitBox>();
        var _result = hitBoxes.vToList().FindAll(hitBox => hitBox.transform.parent == bodyMember.transform);
        if (_result.Count > 0)
        {
            bodyMember.attackObject.hitBoxes = _result;
        }
        else
        {
            var hitBox = new GameObject("hitBox", typeof(vHitBox), typeof(BoxCollider));
            var scale = Vector3.one * 0.15f;
            if (isDefault)
            {
                var lookDir = bodyMember.transform.GetChild(0).position - bodyMember.transform.position;
                var rotation = Quaternion.LookRotation(lookDir);
                scale.z = Vector3.Distance(bodyMember.transform.position, bodyMember.transform.GetChild(0).position);
                var point = bodyMember.transform.position + (lookDir.normalized) * (scale.z * 0.7f);
                hitBox.transform.position = point;
                hitBox.transform.rotation = rotation;
                hitBox.transform.localScale = scale;
                hitBox.transform.parent = bodyMember.transform;
            }
            else
            {
                hitBox.transform.localScale = scale;
                hitBox.transform.parent = bodyMember.transform;
                hitBox.transform.localPosition = Vector3.zero;
                hitBox.transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    void DrawAddExtraBodyMember()
    {
        if (extraBodyMember != null)
        {
            isHuman = Convert.ToBoolean(EditorGUILayout.Popup("Member Type", Convert.ToInt32(isHuman), new string[] { "Generic", "Human" }));
            extraBodyMember.isHuman = isHuman;
            if (isHuman)
            {
                vHumanBones humanBone = 0;
                try
                {
                    humanBone = (vHumanBones)Enum.Parse(typeof(vHumanBones), extraBodyMember.bodyPart);
                }
                catch { }
                humanBone = (vHumanBones)EditorGUILayout.EnumPopup("Body Part", humanBone);
                extraBodyMember.bodyPart = humanBone.ToString();
                var humanBodyBone = (HumanBodyBones)Enum.Parse(typeof(HumanBodyBones), extraBodyMember.bodyPart);
                extraBodyMember.transform = manager.GetComponent<Animator>().GetBoneTransform(humanBodyBone);
            }
            else
            {
                extraBodyMember.bodyPart = EditorGUILayout.TextField("BodyPart Name", extraBodyMember.bodyPart);
            }

            extraBodyMember.transform = EditorGUILayout.ObjectField("Body Member", extraBodyMember.transform, typeof(Transform), true) as Transform;

            var valid = true;
            if (extraBodyMember.transform != null && manager.Members.Find(member => member.transform == extraBodyMember.transform) != null)
            {
                EditorGUILayout.HelpBox("This Body Member already exists, select another", MessageType.Error);
                valid = false;
            }

            if (manager.Members.Find(member => member.bodyPart == extraBodyMember.bodyPart) != null)
            {
                EditorGUILayout.HelpBox("This Body Part already exists, select another", MessageType.Error);
                valid = false;
            }
            GUILayout.BeginHorizontal();
            if (valid)
            {
                if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
                {
                    BodyMember member = new BodyMember();
                    member.attackObject = extraBodyMember.transform.gameObject.AddComponent<vMeleeAttackObject>();
                    member.transform = extraBodyMember.transform;
                    member.bodyPart = extraBodyMember.bodyPart;
                    var type = typeof(BoxCollider);
                    var hitObject = new GameObject("hitBox", typeof(vHitBox), type);
                    hitObject.transform.localScale = Vector3.one * 0.2f;
                    hitObject.transform.parent = member.transform;
                    hitObject.transform.localPosition = Vector3.zero;
                    hitObject.transform.localEulerAngles = Vector3.zero;
                    var hitBox = hitObject.GetComponent<vHitBox>();
                    hitBox.damagePercentage = 100;
                    hitBox.triggerType = vHitBoxType.Damage | vHitBoxType.Recoil;
                    member.attackObject.hitBoxes = new List<vHitBox>();
                    member.attackObject.hitBoxes.Add(hitBox);
                    inCreateHitBox = false;
                    manager.Members.Add(member);
                    extraBodyMember = null;
                    inAddBodyMember = false;
                }
            }
            if (GUILayout.Button("Cancel", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
            {
                extraBodyMember = null;
                inAddBodyMember = false;
            }
            GUILayout.EndHorizontal();
        }
    }

    void DrawBodyMember(ref BodyMember bodyMember, string name, bool canRemove = false)
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        //GUILayout.Box("Selected " + name, GUILayout.ExpandWidth(true));
        if (canRemove && GUILayout.Button("X"))
        {
            var hitColliders = bodyMember.attackObject.hitBoxes;
            for (int i = 0; i < hitColliders.Count; i++)
            {
                DestroyImmediate(hitColliders[i].gameObject);
            }
            DestroyImmediate(bodyMember.attackObject);
            manager.Members.Remove(bodyMember);
        }
        GUILayout.EndHorizontal();
        bodyMember.attackObject = EditorGUILayout.ObjectField("Attack Object", bodyMember.attackObject, typeof(vMeleeAttackObject), true) as vMeleeAttackObject;
        bodyMember.attackObject.damageModifier = EditorGUILayout.IntField(new GUIContent("Damage Modifier +", "Use This to Change the Default damage"), bodyMember.attackObject.damageModifier);
        GUILayout.Box("Hit Boxes", GUILayout.ExpandWidth(true));
        DrawHitBoxesList(bodyMember.attackObject);
        GUILayout.EndVertical();
    }

    void DrawHitBoxesList(vMeleeAttackObject attackObject)
    {
        if (attackObject != null && attackObject.hitBoxes != null)
            for (int i = 0; i < attackObject.hitBoxes.Count; i++)
            {
                try
                {
                    GUILayout.BeginHorizontal();
                    if (attackObject.hitBoxes[i] != null && attackObject.hitBoxes[i].transform == attackObject.transform ||
                    (attackObject.GetComponent<vHitBox>() != null))
                    {
                        DestroyImmediate(attackObject.GetComponent<vHitBox>());
                        attackObject.hitBoxes.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    Color color = GUI.color;
                    GUI.color = seletedHitboxIndex == i ? new Color(1, 1, 0, 0.6f) : color;

                    if (GUILayout.Button("Hit Box " + (i + 1), EditorStyles.miniButton))
                    {
                        if (seletedHitboxIndex == i)
                            seletedHitboxIndex = -1;
                        else
                            seletedHitboxIndex = i;
                    }
                    GUI.color = color;
                    if (attackObject.hitBoxes.Count > 1 && GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        if (attackObject.hitBoxes[i] != null && attackObject.hitBoxes[i].transform != attackObject.transform)
                        {
                            DestroyImmediate(attackObject.hitBoxes[i].gameObject);
                        }
                        attackObject.hitBoxes.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                catch { }
            }

        if (seletedHitboxIndex > -1 && seletedHitboxIndex < attackObject.hitBoxes.Count)
        {
            GUILayout.BeginVertical("box");
            var hitBox = attackObject.hitBoxes[seletedHitboxIndex];
            if (hitBox)
            {
                EditorGUILayout.ObjectField("Selected Hit Box " + (seletedHitboxIndex + 1), hitBox, typeof(vHitBox), true);
                //GUILayout.Box("Hit Settings", GUILayout.ExpandWidth(true));
                hitBox.damagePercentage = EditorGUILayout.IntSlider("Damage Percentage", hitBox.damagePercentage, 0, 100);
                hitBox.triggerType = (vHitBoxType)EditorGUILayout.EnumMaskField("Trigger Type", hitBox.triggerType);
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(hitBox);
                }
            }
            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        if (!inCreateHitBox && GUILayout.Button("Create New Hit Box", EditorStyles.miniButton))
        {
            inCreateHitBox = true;
            damagePercentage = 100;
            triggerType = vHitBoxType.Damage | vHitBoxType.Recoil;
        }
        if (inCreateHitBox)
        {
            GUILayout.Box("New Hit Box", GUILayout.ExpandWidth(true));
            damagePercentage = EditorGUILayout.IntSlider("Damage Percentage", damagePercentage, 0, 100);
            triggerType = (vHitBoxType)EditorGUILayout.EnumMaskField("Trigger Type", triggerType);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
            {
                var type = typeof(BoxCollider);
                var hitObject = new GameObject("hitBox", typeof(vHitBox), type);
                hitObject.transform.localScale = Vector3.one * 0.2f;
                hitObject.transform.parent = attackObject.transform;
                hitObject.transform.localPosition = Vector3.zero;
                hitObject.transform.localEulerAngles = Vector3.zero;
                var hitBox = hitObject.GetComponent<vHitBox>();
                hitBox.damagePercentage = damagePercentage;
                hitBox.triggerType = triggerType;
                attackObject.hitBoxes.Add(hitBox);
                inCreateHitBox = false;
            }

            if (GUILayout.Button("Cancel", EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
            {
                inCreateHitBox = false;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
    }

    void BodyMemberToogle(string bodyPart, ref BodyMember bodyMember, string name)
    {
        if (bodyMember != null)
        {
            Color color = GUI.color;
            GUI.color = seletedBone == bodyPart ? new Color(1, 1, 0, 0.6f) : color;
            if (GUILayout.Button(name, EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
            {
                if (seletedBone == bodyPart) seletedBone = "null";
                else
                    seletedBone = bodyPart;
                seletedHitboxIndex = -1;
                Repaint();
            }
            GUI.color = color;

            foreach (vHitBox hitBox in bodyMember.attackObject.hitBoxes)
            {
                if (hitBox != null)
                {
                    hitBox.gameObject.tag = "Ignore Ragdoll";
                    hitBox.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }
        }
    }

}
