using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using Invector;

namespace Invector.ItemManager
{
    public class vItemDrawer
    {
        public vItem item;
        bool inAddAttribute;
        string attribute = "";
        int attributeValue;
        int index;
        Editor defaultInspector;
        bool inEditName;
        string currentName;
        public vItemDrawer(vItem item)
        {
            this.item = item;
            defaultInspector = Editor.CreateEditor(this.item);
        }

        public void DrawItem(ref List<string> attributes,ref List<vItem> items, bool showObject = true, bool editName = false)
        {
            if (!item) return;
            SerializedObject _item = new SerializedObject(item);
            _item.Update();
            
            GUILayout.BeginVertical("box");

            if (showObject)
                EditorGUILayout.ObjectField(item, typeof(vItem), false);

          
            if (editName)
                item.name = EditorGUILayout.TextField("Item name", item.name);
            else
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(item.name, GUILayout.ExpandWidth(true));
                if(!inEditName && GUILayout.Button("EditName", EditorStyles.miniButton))
                {
                    currentName = item.name;
                    inEditName = true;
                }
                GUILayout.EndHorizontal();
            }
            if(inEditName)
            {
                var sameItemName = items.Find(i => i.name == currentName && i != item);
                currentName = EditorGUILayout.TextField("New Name", currentName);

                GUILayout.BeginHorizontal("box");
                if (sameItemName==null && !string.IsNullOrEmpty(currentName)&& GUILayout.Button("OK", EditorStyles.miniButton, GUILayout.MinWidth(60)))
                {
                    item.name = currentName;
                    inEditName = false;

                }
                if (GUILayout.Button("Cancel", EditorStyles.miniButton, GUILayout.MinWidth(60)))
                {
                    inEditName = false;
                }
                GUILayout.EndHorizontal();
                if (sameItemName != null)
                    EditorGUILayout.HelpBox("This name already exist", MessageType.Error);
                if(string.IsNullOrEmpty(currentName))
                    EditorGUILayout.HelpBox("This name can not be empty", MessageType.Error);
            }  

            EditorGUILayout.LabelField("Description");

            item.description = EditorGUILayout.TextArea(item.description);
            item.type = (vItemType)EditorGUILayout.EnumPopup("Item Type", item.type); 
            item.stackable = EditorGUILayout.Toggle("Stackable", item.stackable);

            if (item.stackable)
            {
                if (item.maxStack <= 0) item.maxStack = 1;
                item.maxStack = EditorGUILayout.IntField("Max Stack", item.maxStack);
            }
            else item.maxStack = 1;

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon");
            item.icon = (Sprite)EditorGUILayout.ObjectField(item.icon, typeof(Sprite), false);
            var rect = GUILayoutUtility.GetRect(40, 40);

            if (item.icon != null)
            {
                DrawTextureGUI(rect, item.icon, new Vector2(40, 40));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box");
            GUILayout.Label("Original Object");
            item.originalObject = (GameObject)EditorGUILayout.ObjectField(item.originalObject, typeof(GameObject), false);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            GUILayout.Label("Drop Object");
            item.dropObject = (GameObject)EditorGUILayout.ObjectField(item.dropObject, typeof(GameObject), false);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
           
            GUILayout.EndVertical();

            DrawAttributes(ref attributes);
            GUILayout.BeginVertical("box");
            GUILayout.Box(new GUIContent("Custom Settings", "This area is used for additional properties\n in vItem Properties in defaultInspector region"));
            defaultInspector.DrawDefaultInspector();
            GUILayout.EndVertical();
            if (GUI.changed || _item.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(item);
            }
        }

        void DrawAttributes(ref List<string> attributes)
        {
            try
            {
                var _attributes = attributes.vToArray();
                GUILayout.BeginVertical("box");
                GUILayout.Box("Attributes", GUILayout.ExpandWidth(true));

                if (!inAddAttribute && GUILayout.Button("Add Attribute", EditorStyles.miniButton))                
                    inAddAttribute = true;
                
                if (inAddAttribute)
                {
                    if (attributes.Count > 0)
                    {
                        index = Array.IndexOf(_attributes, attribute);
                        if (index != -1)
                        {
                            GUILayout.BeginHorizontal("box");
                            index = EditorGUILayout.Popup(index, _attributes);
                            attribute = attributes[index];
                            EditorGUILayout.LabelField("Value", GUILayout.MinWidth(60));
                            attributeValue = EditorGUILayout.IntField(attributeValue);
                            GUILayout.EndHorizontal();
                            if (item.attributes != null && item.attributes.Contains(attribute))
                            {
                                EditorGUILayout.HelpBox("This attribute already exist ", MessageType.Error);
                                if (GUILayout.Button("Cancel", EditorStyles.miniButton, GUILayout.MinWidth(60)))
                                {
                                    inAddAttribute = false;
                                }
                            }
                            else
                            {
                                GUILayout.BeginHorizontal("box");
                                if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.MinWidth(60)))
                                {
                                    item.attributes.Add(new vItemAttribute(attribute, attributeValue));
                                    attribute = attributes[0];
                                    attributeValue = 0;
                                    inAddAttribute = false;

                                }
                                if (GUILayout.Button("Cancel", EditorStyles.miniButton, GUILayout.MinWidth(60)))
                                {
                                    attribute = attributes[0];
                                    attributeValue = 0;
                                    inAddAttribute = false;
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            attribute = attributes[0];
                        }
                    }
                }

                EditorGUILayout.Space();
                for (int i = 0; i < item.attributes.Count; i++)
                {
                    GUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField(item.attributes[i].name.ToString(), GUILayout.MinWidth(60));
                    item.attributes[i].value = EditorGUILayout.IntField(item.attributes[i].value);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("x", GUILayout.MaxWidth(30)))
                    {
                        item.attributes.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            catch { }
        }

        void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
        {
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                                       sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            Vector2 actualSize = size;

            actualSize.y *= (sprite.rect.height / sprite.rect.width);
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
        }

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}

