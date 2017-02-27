using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Invector.ItemManager
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(vItemCollection))]
    public class vItemCollectionEditor : Editor
    {
        vItemCollection collection;
        SerializedProperty itemReferenceList;
        GUISkin skin;
        bool inAddItem;
        int selectedItem;
        Vector2 scroll;

        void OnEnable()
        {
            collection = (vItemCollection)target;
            skin = Resources.Load("skin") as GUISkin;
            itemReferenceList = serializedObject.FindProperty("items");
        }

        public override void OnInspectorGUI()
        {
            var oldSkin = GUI.skin;

            serializedObject.Update();
            if (skin) GUI.skin = skin;
            GUILayout.BeginVertical("Item Collection", "window");
            GUILayout.Space(30);
            GUI.skin = oldSkin;
            base.OnInspectorGUI();
            if (skin) GUI.skin = skin;

            if (collection.itemListData)
            {
                GUILayout.BeginVertical("box");
                if (itemReferenceList.arraySize > collection.itemListData.items.Count)
                {
                    collection.items.Resize(collection.itemListData.items.Count);
                }
                GUILayout.Box("Item Collection " + collection.items.Count);
                if (!inAddItem && collection.itemListData.items.Count > 0 && GUILayout.Button("Add Item", EditorStyles.miniButton))
                {
                    inAddItem = true;
                }
                if (inAddItem && collection.itemListData.items.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    selectedItem = EditorGUILayout.Popup(new GUIContent("SelectItem"), selectedItem, GetItemContents(collection.itemListData.items));
                    bool isValid = true;

                    if (collection.items.Find(i => i.id == collection.itemListData.items[selectedItem].id) != null)
                    {
                        isValid = false;
                        EditorGUILayout.HelpBox("This item already exist", MessageType.Error);
                    }
                    GUILayout.BeginHorizontal();

                    if (isValid && GUILayout.Button("Add", EditorStyles.miniButton))
                    {
                        itemReferenceList.arraySize++;
                        itemReferenceList.GetArrayElementAtIndex(itemReferenceList.arraySize - 1).FindPropertyRelative("id").intValue = collection.itemListData.items[selectedItem].id;
                        itemReferenceList.GetArrayElementAtIndex(itemReferenceList.arraySize - 1).FindPropertyRelative("amount").intValue = 1;
                        EditorUtility.SetDirty(collection);
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
                for (int i = 0; i < collection.items.Count; i++)
                {
                    if (collection.itemListData.items.Find(t => t.id.Equals(collection.items[i].id)) != null)
                    {
                        GUILayout.BeginHorizontal();

                        var item = collection.itemListData.items.Find(t => t.id.Equals(collection.items[i].id));
                        if (item)
                        {
                            GUILayout.BeginHorizontal("box");
                            var rect = GUILayoutUtility.GetRect(20, 20);
                            if (item.icon != null)
                            {
                                DrawTextureGUI(rect, item.icon, new Vector2(30, 30));
                            }

                            var name = " ID " + item.id.ToString("00") + "\n - " + item.name + "\n - " + item.type.ToString();
                            var content = new GUIContent(name, null, item.description);
                            GUILayout.Label(content, EditorStyles.miniLabel);
                            GUILayout.BeginVertical("box", GUILayout.Height(20), GUILayout.Width(40));
                            GUILayout.Label("Amount", EditorStyles.miniLabel);
                            collection.items[i].amount = EditorGUILayout.IntField(collection.items[i].amount, GUILayout.Width(40));
                            if (collection.items[i].amount < 1)
                            {
                                collection.items[i].amount = 1;
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
                            if (collection.itemListData.inEdition)
                            {
                                if (vItemListWindow.Instance != null)
                                    vItemListWindow.SetCurrentSelectedItem(collection.itemListData.items.IndexOf(item));
                                else
                                    vItemListWindow.CreateWindow(collection.itemListData, collection.itemListData.items.IndexOf(item));
                            }
                            else
                                vItemListWindow.CreateWindow(collection.itemListData, collection.itemListData.items.IndexOf(item));
                        }
                        GUI.backgroundColor = backgroundColor;
                    }
                    else
                    {
                        itemReferenceList.DeleteArrayElementAtIndex(i);
                        EditorUtility.SetDirty(collection);
                        serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }

                GUILayout.EndScrollView();
                GUI.skin.box = boxStyle;

                GUILayout.EndVertical();
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(collection);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
            GUI.skin = oldSkin;
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

        void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
        {
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                                       sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            Vector2 actualSize = size;

            actualSize.y *= (sprite.rect.height / sprite.rect.width);
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
        }
    }

}

