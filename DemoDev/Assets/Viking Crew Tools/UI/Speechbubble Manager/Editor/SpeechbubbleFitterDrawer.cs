using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VikingCrewTools {
    [CustomEditor(typeof(RatioLayoutFitter))]
    public class SpeechbubbleFitterDrawer : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            base.DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}