using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Invector.ItemManager
{
    [CustomEditor(typeof(vItemManager))]
    [System.Serializable]
    public class vItemManagerEditor : Editor
    {
        vItemManager manager;
        SerializedProperty itemReferenceList;
        GUISkin skin, oldSkin;
        bool inAddItem;
        int selectedItem;
        Vector2 scroll;
        bool showManagerEvents;
        bool showItemAttributes;
        string[] ignoreProperties = new string[] { "equipPoints", "applyAttributeEvents", "items", "startItems", "onUseItem", "onOpenCloseInventory", "onEquipItem", "onUnequipItem" };
        bool[] inEdition;
        string[] newPointNames;
        Transform parentBone;
        Animator animator;

        [MenuItem("Invector/Melee Combat/Components/ItemManager (Player Only)")]
        static void MenuComponent()
        {
            if (Selection.activeGameObject)
            {
                var itemManager = Selection.activeGameObject.AddComponent<vItemManager>();
                itemManager.CreateDefaultEquipPoints();
            }

            else
                Debug.Log("Please select the Player to add the component.");
        }

        void OnEnable()
        {
            manager = (vItemManager)target;
            itemReferenceList = serializedObject.FindProperty("startItems");
            skin = Resources.Load("skin") as GUISkin;
            manager.CreateDefaultEquipPoints();
            animator = manager.GetComponent<Animator>();
            if (manager.equipPoints != null)
            {
                inEdition = new bool[manager.equipPoints.Count];
                newPointNames = new string[manager.equipPoints.Count];
            }

            else
                manager.equipPoints = new List<EquipPoint>();
        }

        public override void OnInspectorGUI()
        {
            oldSkin = GUI.skin;
            serializedObject.Update();
            if (skin) GUI.skin = skin;
            GUILayout.BeginVertical("Item Manager", "window");
            if (skin) GUILayout.Space(30);
            GUI.skin = oldSkin;
            DrawPropertiesExcluding(serializedObject, ignoreProperties);
            GUI.skin = skin;

            if (GUILayout.Button("Open Item List"))
            {
                vItemListWindow.CreateWindow(manager.itemListData);
            }

            if (manager.itemListData)
            {
                GUILayout.BeginVertical("box");
                if (itemReferenceList.arraySize > manager.itemListData.items.Count)
                {
                    manager.startItems.Resize(manager.itemListData.items.Count);
                }
                GUILayout.Box("Start Items " + manager.startItems.Count);
                if (!inAddItem && manager.itemListData.items.Count > 0 && GUILayout.Button("Add Item", EditorStyles.miniButton))
                {
                    inAddItem = true;
                }
                if (inAddItem && manager.itemListData.items.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    selectedItem = EditorGUILayout.Popup(new GUIContent("SelectItem"), selectedItem, GetItemContents(manager.itemListData.items));
                    bool isValid = true;

                    if (manager.startItems.Find(i => i.id == manager.itemListData.items[selectedItem].id) != null)
                    {
                        isValid = false;
                        EditorGUILayout.HelpBox("This item already exist", MessageType.Error);
                    }
                    GUILayout.BeginHorizontal();

                    if (isValid && GUILayout.Button("Add", EditorStyles.miniButton))
                    {
                        itemReferenceList.arraySize++;

                        itemReferenceList.GetArrayElementAtIndex(itemReferenceList.arraySize - 1).FindPropertyRelative("id").intValue = manager.itemListData.items[selectedItem].id;
                        itemReferenceList.GetArrayElementAtIndex(itemReferenceList.arraySize - 1).FindPropertyRelative("amount").intValue = 1;
                        EditorUtility.SetDirty(manager);
                        serializedObject.ApplyModifiedProperties();
                        inAddItem = false;
                    }
                    if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                    {
                        inAddItem = false;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                scroll = GUILayout.BeginScrollView(scroll, GUILayout.MinHeight(200), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
                for (int i = 0; i < manager.startItems.Count; i++)
                {
                    if (manager.itemListData.items.Find(t => t.id.Equals(manager.startItems[i].id)) != null)
                    {
                        GUILayout.BeginHorizontal();

                        var item = manager.itemListData.items.Find(t => t.id.Equals(manager.startItems[i].id));
                        if (item)
                        {
                            GUILayout.BeginHorizontal("box");
                            var rect = GUILayoutUtility.GetRect(20, 20);

                            if (item.icon != null)
                            {
                                DrawTextureGUI(rect, item.icon, new Vector2(30, 30));
                            }

                            var name = " ID " + item.id.ToString("00") + "\n - " + item.name + "\n - " + item.type.ToString();
                            var content = new GUIContent(name, null, "Click to Open");
                            GUILayout.Label(content, EditorStyles.miniLabel);
                            GUILayout.BeginVertical("box", GUILayout.Height(20), GUILayout.Width(40));
                            GUILayout.Label("Amount", EditorStyles.miniLabel);
                            manager.startItems[i].amount = EditorGUILayout.IntField(manager.startItems[i].amount, GUILayout.Width(40));
                            if (manager.startItems[i].amount < 1)
                            {
                                manager.startItems[i].amount = 1;
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(10);
                            GUILayout.EndHorizontal();
                            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(50)))
                            {
                                itemReferenceList.DeleteArrayElementAtIndex(i);
                                EditorUtility.SetDirty(target);
                                serializedObject.ApplyModifiedProperties();
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                        Color backgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.clear;
                        var _rec = GUILayoutUtility.GetLastRect();
                        _rec.width -= 25;

                        EditorGUIUtility.AddCursorRect(_rec, MouseCursor.Link);

                        if (GUI.Button(_rec, ""))
                        {
                            if (manager.itemListData.inEdition)
                            {
                                if (vItemListWindow.Instance != null)
                                    vItemListWindow.SetCurrentSelectedItem(manager.itemListData.items.IndexOf(item));
                                else
                                    vItemListWindow.CreateWindow(manager.itemListData, manager.itemListData.items.IndexOf(item));
                            }
                            else
                                vItemListWindow.CreateWindow(manager.itemListData, manager.itemListData.items.IndexOf(item));
                        }
                        GUI.backgroundColor = backgroundColor;
                    }
                    else
                    {
                        itemReferenceList.DeleteArrayElementAtIndex(i);
                        EditorUtility.SetDirty(manager);
                        serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }

                GUILayout.EndScrollView();
                GUI.skin.box = boxStyle;

                GUILayout.EndVertical();
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(manager);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            var equipPoints = serializedObject.FindProperty("equipPoints");
            var applyAttributeEvents = serializedObject.FindProperty("applyAttributeEvents");
            var onUseItem = serializedObject.FindProperty("onUseItem");
            var onOpenCloseInventoty = serializedObject.FindProperty("onOpenCloseInventory");
            var onEquipItem = serializedObject.FindProperty("onEquipItem");
            var onUnequipItem = serializedObject.FindProperty("onUnequipItem");
            if (equipPoints.arraySize != inEdition.Length)
            {
                inEdition = new bool[equipPoints.arraySize];
                newPointNames = new string[manager.equipPoints.Count];
            }
            if (equipPoints != null) DrawEquipPoints(equipPoints);
            if (applyAttributeEvents != null) DrawAttributeEvents(applyAttributeEvents);
            GUILayout.BeginVertical("box");
            showManagerEvents = GUILayout.Toggle(showManagerEvents, showManagerEvents ? "Close Events" : "Open Events", EditorStyles.miniButton);
            GUI.skin = oldSkin;
            if (showManagerEvents)
            {
                if (onOpenCloseInventoty != null) EditorGUILayout.PropertyField(onOpenCloseInventoty);
                if (onUseItem != null) EditorGUILayout.PropertyField(onUseItem);
                if (onEquipItem != null) EditorGUILayout.PropertyField(onEquipItem);
                if (onUnequipItem != null) EditorGUILayout.PropertyField(onUnequipItem);
            }
            GUI.skin = skin;
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(manager);
                serializedObject.ApplyModifiedProperties();
            }

            GUI.skin = oldSkin;
        }

        void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
        {
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                                       sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            Vector2 actualSize = size;
            actualSize.y *= (sprite.rect.height / sprite.rect.width);
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);

        }

        GUIContent GetItemContent(vItem item)
        {
            var texture = item.icon != null ? item.icon.texture : null;
            return new GUIContent(item.name, texture, item.description); ;
        }

        GUIContent[] GetItemContents(List<vItem> items)
        {
            GUIContent[] names = new GUIContent[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                var texture = items[i].icon != null ? items[i].icon.texture : null;
                names[i] = new GUIContent(items[i].name, texture, items[i].description);
            }
            return names;
        }

        void DrawEquipPoints(SerializedProperty prop)
        {
            GUILayout.BeginVertical("box");
            prop.isExpanded = GUILayout.Toggle(prop.isExpanded, prop.isExpanded ? "Close Equip Points" : "Open Equip Points", EditorStyles.miniButton);
            if (prop.isExpanded)
            {
                prop.arraySize = EditorGUILayout.IntField("Points", prop.arraySize);
                for (int i = 0; i < prop.arraySize; i++)
                {

                    var equiPointName = prop.GetArrayElementAtIndex(i).FindPropertyRelative("equiPointName");
                    var defaultPoint = prop.GetArrayElementAtIndex(i).FindPropertyRelative("defaultPoint");
                    var points = prop.GetArrayElementAtIndex(i).FindPropertyRelative("customPoints");
                    var onInstantiateEquiment = prop.GetArrayElementAtIndex(i).FindPropertyRelative("onInstantiateEquiment");

                    try
                    {
                        GUILayout.BeginVertical("box");
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(equiPointName);
                        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                        {
                            prop.DeleteArrayElementAtIndex(i);
                            GUILayout.EndHorizontal();
                            break;
                        }
                        GUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(defaultPoint);
                        GUILayout.BeginVertical("box");
                        points.isExpanded = GUILayout.Toggle(points.isExpanded, "Custom Points", EditorStyles.miniButton);
                        if (points.isExpanded)
                        {
                            GUILayout.Space(5);
                            if (!inEdition[i] && GUILayout.Button("New Point", EditorStyles.miniButton))
                            {
                                inEdition[i] = true;
                                if (equiPointName.stringValue.Contains("Left") || equiPointName.stringValue.Contains("left"))
                                {
                                    if (animator)
                                        parentBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                                }
                                else
                                {
                                    if (animator)
                                        parentBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                                }
                            }

                            if (inEdition[i])
                            {
                                GUILayout.Box("New Custom Point");

                                parentBone = (Transform)EditorGUILayout.ObjectField("Parent Bone", parentBone, typeof(Transform), true);
                                newPointNames[i] = EditorGUILayout.TextField("Custom Point Name", newPointNames[i]);
                                bool valid = true;
                                if (string.IsNullOrEmpty(newPointNames[i]))
                                {
                                    valid = false;
                                    EditorGUILayout.HelpBox("Custom Point Name is empty", MessageType.Error);
                                }
                                var array = ConvertToArray<Transform>(points);
                                if (Array.Exists<Transform>(array, point => point.gameObject.name.Equals(newPointNames[i])))
                                {
                                    valid = false;
                                    EditorGUILayout.HelpBox("Custom Point Name already exist", MessageType.Error);
                                }

                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                                {
                                    inEdition[i] = false;
                                }
                                GUI.enabled = parentBone && valid;

                                if (GUILayout.Button("Create", EditorStyles.miniButton))
                                {
                                    var customPoint = new GameObject(newPointNames[i]);

                                    customPoint.transform.parent = parentBone;
                                    customPoint.transform.localPosition = Vector3.zero;
                                    customPoint.transform.forward = manager.transform.forward;
                                    points.arraySize++;
                                    points.GetArrayElementAtIndex(points.arraySize - 1).objectReferenceValue = customPoint.transform;
                                    EditorUtility.SetDirty(manager);
                                    serializedObject.ApplyModifiedProperties();
                                    inEdition[i] = false;
                                }

                                GUI.enabled = true;
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.Space(5);
                            for (int a = 0; a < points.arraySize; a++)
                            {
                                var remove = false;
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(points.GetArrayElementAtIndex(a), true);
                                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                                    remove = true;
                                GUILayout.EndHorizontal();
                                if (remove)
                                {
                                    var obj = (Transform)points.GetArrayElementAtIndex(a).objectReferenceValue;
                                    points.DeleteArrayElementAtIndex(a);
                                    if (obj)
                                    {
                                        points.DeleteArrayElementAtIndex(a);
                                        DestroyImmediate(obj.gameObject);
                                    }
                                    EditorUtility.SetDirty(manager);
                                    serializedObject.ApplyModifiedProperties();
                                    break;
                                }
                            }
                        }
                        GUILayout.EndVertical();

                        GUI.skin = oldSkin;
                        if (onInstantiateEquiment != null) EditorGUILayout.PropertyField(onInstantiateEquiment);

                        GUI.skin = skin;
                        GUILayout.EndVertical();
                    }
                    catch { }

                }
            }
            GUILayout.EndVertical();
        }
        T[] ConvertToArray<T>(SerializedProperty prop)
        {
            T[] value = new T[prop.arraySize];
            for (int i = 0; i < prop.arraySize; i++)
            {
                object element = prop.GetArrayElementAtIndex(i).objectReferenceValue;
                value[i] = (T)element;
            }
            return value;
        }
        void DrawAttributeEvents(SerializedProperty prop)
        {
            GUILayout.BeginVertical("box");
            prop.isExpanded = GUILayout.Toggle(prop.isExpanded, prop.isExpanded ? "Close Attribute Events" : "Open Attribute Events", EditorStyles.miniButton);
            if (prop.isExpanded)
            {
                prop.arraySize = EditorGUILayout.IntField("Attributes", prop.arraySize);
                for (int i = 0; i < prop.arraySize; i++)
                {

                    var attributeName = prop.GetArrayElementAtIndex(i).FindPropertyRelative("attributeName");
                    var onApplyAttribute = prop.GetArrayElementAtIndex(i).FindPropertyRelative("onApplyAttribute");
                    try
                    {
                        GUILayout.BeginVertical("box");
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(attributeName);
                        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                        {
                            prop.DeleteArrayElementAtIndex(i);
                            GUILayout.EndHorizontal();
                            break;
                        }
                        GUILayout.EndHorizontal();
                        GUI.skin = oldSkin;
                        EditorGUILayout.PropertyField(onApplyAttribute);
                        GUI.skin = skin;
                        GUILayout.EndVertical();
                    }
                    catch { }

                }
            }
            GUILayout.EndVertical();
        }


    }
}
