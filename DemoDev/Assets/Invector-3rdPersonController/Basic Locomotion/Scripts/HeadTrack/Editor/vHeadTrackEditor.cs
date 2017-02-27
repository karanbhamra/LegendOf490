﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(vHeadTrack))]
public class vHeadTrackEditor : Editor
{
    Animator animator;
    GUISkin skin;

    void OnEnable()
    {
        vHeadTrack track = (vHeadTrack)target;
        animator = track.GetComponentInParent<Animator>();
        skin = Resources.Load("skin") as GUISkin;
    }  

    [MenuItem("Invector/Basic Locomotion/Components/HeadTrack")]
    static void MenuComponent()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<vHeadTrack>();
        else
            Debug.Log("Please select a GameObject to add the component.");
    }

    public override void OnInspectorGUI()
    {
        vHeadTrack track = (vHeadTrack)target;
        if (skin != null) GUI.skin = skin;
        if (animator)
        {
            GUILayout.BeginVertical("Head Track System", "window");
            GUILayout.Space(30);
            base.DrawDefaultInspector();

            if (track.head == null)
                track.head = animator.GetBoneTransform(HumanBodyBones.Head);

            if(track.useLimitAngle)
            {
                GUILayout.BeginVertical("box");
                //GUILayout.Box("Head Track Angle Limit", GUILayout.ExpandWidth(true));
                GUILayout.Label(new GUIContent("Angle Range X"), EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                track.minAngleX = EditorGUILayout.FloatField(track.minAngleX, GUILayout.MaxWidth(40));
                EditorGUILayout.MinMaxSlider(ref track.minAngleX, ref track.maxAngleX, -180, 180);
                track.maxAngleX = EditorGUILayout.FloatField(track.maxAngleX, GUILayout.MaxWidth(40));
                GUILayout.EndHorizontal();            

                GUILayout.Label(new GUIContent("Angle Range Y"), EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                track.minAngleY = EditorGUILayout.FloatField(track.minAngleY, GUILayout.MaxWidth(40));
                EditorGUILayout.MinMaxSlider(ref track.minAngleY, ref track.maxAngleY, -180, 180);
                track.maxAngleY = EditorGUILayout.FloatField(track.maxAngleY, GUILayout.MaxWidth(40));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }            

            GUILayout.EndVertical();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }

}
