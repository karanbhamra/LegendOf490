using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Invector.ItemManager
{
    [CustomEditor(typeof(vItemListData))]
    public class vItemListEditor : Editor
    {
        GUISkin skin;
	    vItemListData itemList;
	    
        void OnEnable()
        {
            itemList = (vItemListData)target;
            skin = Resources.Load("skin") as GUISkin;
        }

        [MenuItem("Invector/Resources/New ItemListData")]
        static void CreateNewListData()
        {
            vItemListData listData = ScriptableObject.CreateInstance<vItemListData>();
            AssetDatabase.CreateAsset(listData, "Assets/ItemListData.asset");
        }

        public override void OnInspectorGUI()
        {
            if (skin) GUI.skin = skin;

            serializedObject.Update();

            GUI.enabled = !Application.isPlaying;

            GUILayout.BeginVertical("Item List", "window");
	        GUILayout.Space(30);
	        
            if (!itemList.inEdition && GUILayout.Button("Edit"))
            {
                vItemListWindow.CreateWindow(itemList);
            }
            else if (itemList.inEdition)
            {
                if (vItemListWindow.Instance != null)
                {
                    if (vItemListWindow.Instance.itemList == null)
                    {
                        vItemListWindow.Instance.Close();
                    }
                    else
                        EditorGUILayout.HelpBox("The Item List Window is open", MessageType.Info);
                }
                else
                {
                    itemList.inEdition = false;
                }
            }
            GUILayout.EndVertical();
            if (GUI.changed || serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);

            }
        }
    }

    public class vItemListWindow : EditorWindow
    {
        public vItemListData itemList;
        public GUISkin skin;
        public SerializedObject serializedObject;
        vItem addItem;
        vItemDrawer addItemDrawer;
        vItemDrawer currentItemDrawer;
        bool inAddItem;
        bool openAttributeList;
        bool inCreateAttribute;
        string attributeName;
        int indexSelected;
        Vector2 scroolView;
        Vector2 attributesScroll;
        public static vItemListWindow Instance;

        public static void CreateWindow(vItemListData itemList)
        {
            vItemListWindow window = (vItemListWindow)EditorWindow.GetWindow(typeof(vItemListWindow), false, "ItemList Editor");
            Instance = window;
            window.itemList = itemList;
            window.skin = Resources.Load("skin") as GUISkin;
            Instance.Init();
        }

        public static void CreateWindow(vItemListData itemList, int firtItemSelected)
        {
            vItemListWindow window = (vItemListWindow)EditorWindow.GetWindow(typeof(vItemListWindow), false, "ItemList Editor");
            Instance = window;
            window.itemList = itemList;
            window.skin = Resources.Load("skin") as GUISkin;
            Instance.Init(firtItemSelected);
        }

        void Init()
        {
            serializedObject = new SerializedObject(itemList);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(itemList));
            skin = Resources.Load("skin") as GUISkin;
            if (subAssets.Length > 1)
            {
                for (int i = subAssets.Length - 1; i >= 0; i--)
                {
                    var item = subAssets[i] as vItem;

                    if (item && !itemList.items.Contains(item))
                    {
                        item.id = GetUniqueID();
                        itemList.items.Add(item);
                    }
                }
                EditorUtility.SetDirty(itemList);
                OrderByID();
            }
            itemList.inEdition = true;
            this.Show();
        }

        void Init(int firtItemSelected)
        {
            Init();
            SetCurrentSelectedItem(firtItemSelected);
        }

        public void OnGUI()
        {
            if (skin) GUI.skin = skin;

            GUILayout.BeginVertical("Item List", "window");
            GUILayout.Space(30);
            GUILayout.BeginVertical("box");

            GUI.enabled = !Application.isPlaying;
            itemList = EditorGUILayout.ObjectField("ItemListData", itemList, typeof(vItemListData), false) as vItemListData;

            if (serializedObject == null && itemList != null)
            {
                serializedObject = new SerializedObject(itemList);
            }
            else if (itemList == null)
            {
                GUILayout.EndVertical();
                return;
            }

            serializedObject.Update();

            if (!inAddItem && GUILayout.Button("Add Item"))
            {
                addItem = ScriptableObject.CreateInstance<vItem>();
                addItem.name = "New Item";

                currentItemDrawer = null;
                inAddItem = true;
            }
            if (inAddItem)
            {
                DrawAddItem();
            }
            GUILayout.Space(10);
            openAttributeList = GUILayout.Toggle(openAttributeList, !openAttributeList ? "Open Item Attributes" : "Close Item Attributes", "button");

            if (openAttributeList)
            {
                GUILayout.BeginVertical("box");

                attributesScroll = GUILayout.BeginScrollView(attributesScroll);

                for (int i = 0; i < itemList.itemAttributes.Count; i++)
                {
                    GUILayout.BeginHorizontal("box");

                    GUILayout.Label(itemList.itemAttributes[i]);
                    if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        itemList.itemAttributes.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                if (!inCreateAttribute && GUILayout.Button("Create Attribute", EditorStyles.miniButton))
                {
                    inCreateAttribute = true;
                    attributeName = "New Attribute";
                }
                if (inCreateAttribute)
                {
                    GUILayout.BeginVertical("box");
                    attributeName = EditorGUILayout.TextField("Attribute Name", attributeName);
                    if (attributeName.Contains(" "))
                        attributeName = attributeName.Replace(" ", string.Empty);
                    var isValid = true;
                    if (itemList.itemAttributes.Contains(attributeName))
                    {
                        isValid = false;
                        EditorGUILayout.HelpBox("This Attribute already exist", MessageType.Error);
                    }
                    if (string.IsNullOrEmpty(attributeName))
                    {
                        isValid = false;
                        EditorGUILayout.HelpBox("Attribute name can't be empty", MessageType.Error);
                    }
                    GUILayout.BeginHorizontal();
                    if (isValid && GUILayout.Button("Create", EditorStyles.miniButton))
                    {
                        itemList.itemAttributes.Add(attributeName);
                        inCreateAttribute = false;
                    }
                    if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                    {
                        inCreateAttribute = false;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            GUILayout.Box(itemList.items.Count.ToString("00") + " Items");
            scroolView = GUILayout.BeginScrollView(scroolView, GUILayout.ExpandWidth(true));
            for (int i = 0; i < itemList.items.Count; i++)
            {
                if (itemList.items[i] != null)
                {
                    Color color = GUI.color;
                    GUI.color = currentItemDrawer != null && currentItemDrawer.item == itemList.items[i] ? Color.green : color;
                    GUILayout.BeginVertical("box");
                    GUI.color = color;
                    GUILayout.BeginHorizontal();
                    var texture = itemList.items[i].icon != null ? ConvertSpriteToTexture(itemList.items[i].icon) : null;
                    var name = " ID " + itemList.items[i].id.ToString("00") + "\n - " + itemList.items[i].name + "\n - " + itemList.items[i].type.ToString();
                    var content = new GUIContent(name, texture, currentItemDrawer != null && currentItemDrawer.item == itemList.items[i] ? "Click to Close" : "Click to Open");
                    GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                    GUI.skin.box.alignment = TextAnchor.UpperLeft;
                    GUI.skin.box.fontStyle = FontStyle.Italic;

                    GUI.skin.box.fontSize = 11;

                    if (GUILayout.Button(content, "box", GUILayout.Height(50), GUILayout.MinWidth(50)))
                    {
                        GUI.FocusControl("clearFocus");
                        scroolView.y = 1 + i * 60;
                        currentItemDrawer = currentItemDrawer != null ? currentItemDrawer.item == itemList.items[i] ? null : new vItemDrawer(itemList.items[i]) : new vItemDrawer(itemList.items[i]);
                    }
                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

                    GUI.skin.box = boxStyle;

                    if (GUILayout.Button("x", GUILayout.MaxWidth(20), GUILayout.Height(45)))
                    {
                        var item = itemList.items[i];
                        itemList.items.RemoveAt(i);
                        DestroyImmediate(item, true);
                        OrderByID();
                        AssetDatabase.SaveAssets();
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(itemList);
                        GUILayout.EndHorizontal();
                        Repaint();
                        break;
                    }
                    GUILayout.EndHorizontal();

                    GUI.color = color;
                    if (currentItemDrawer != null && currentItemDrawer.item == itemList.items[i] && itemList.items.Contains(currentItemDrawer.item))
                    {
                        currentItemDrawer.DrawItem(ref itemList.itemAttributes, ref itemList.items, false);
                    }

                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            if (GUI.changed || serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(itemList);
            }
        }

        public static void SetCurrentSelectedItem(int index)
        {
            if (Instance != null && Instance.itemList != null && Instance.itemList.items != null && Instance.itemList.items.Count > 0 && index < Instance.itemList.items.Count)
            {
                Instance.currentItemDrawer = Instance.currentItemDrawer != null ? Instance.currentItemDrawer.item == Instance.itemList.items[index] ? null : new vItemDrawer(Instance.itemList.items[index]) : new vItemDrawer(Instance.itemList.items[index]);
                Instance.scroolView.y = 1 + index * 60;
                Instance.Repaint();
            }

        }

        void OnDestroy()
        {
            if (itemList)
            {
                itemList.inEdition = false;
            }
        }

        Texture2D ConvertSpriteToTexture(Sprite sprite)
        {
            try
            {
                if (sprite.rect.width != sprite.texture.width)
                {
                    Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    Color[] colors = newText.GetPixels();
                    Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.y),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.width),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.height));
                    Debug.Log(colors.Length + "_" + newColors.Length);
                    newText.SetPixels(newColors);
                    newText.Apply();
                    return newText;
                }
                else
                    return sprite.texture;
            }
            catch
            {
                return sprite.texture;
            }
        }

        private void DrawAddItem()
        {
            GUILayout.BeginVertical("box");
            if (addItem != null)
            {
                if (addItemDrawer == null || addItemDrawer.item == null || addItemDrawer.item != addItem)
                    addItemDrawer = new vItemDrawer(addItem);
                bool isValid = true;
                if (addItemDrawer != null)
                {
                    GUILayout.Box("Create Item Window");
                    addItemDrawer.DrawItem(ref itemList.itemAttributes, ref itemList.items, false, true);
                }

                if (string.IsNullOrEmpty(addItem.name))
                {
                    isValid = false;
                    EditorGUILayout.HelpBox("This item name cant be null or empty,please type a name", MessageType.Error);
                }

                if (itemList.items.FindAll(item => item.name.Equals(addItemDrawer.item.name)).Count > 0)
                {
                    isValid = false;
                    EditorGUILayout.HelpBox("This item name already exists", MessageType.Error);
                }
                GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(false));

                if (isValid && GUILayout.Button("Add"))
                {
                    AssetDatabase.AddObjectToAsset(addItem, AssetDatabase.GetAssetPath(itemList));
                    addItem.hideFlags = HideFlags.HideInHierarchy;
                    addItem.id = GetUniqueID();
                    itemList.items.Add(addItem);
                    OrderByID();
                    addItem = null;
                    inAddItem = false;
                    addItemDrawer = null;
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(itemList);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Cancel"))
                {
                    addItem = null;
                    inAddItem = false;
                    addItemDrawer = null;
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(itemList);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Error", MessageType.Error);
            }
            GUILayout.EndVertical();
        }

        int GetUniqueID(int value = 0)
        {
            var result = value;


            for (int i = 0; i < itemList.items.Count + 1; i++)
            {
                var item = itemList.items.Find(t => t.id == i);
                if (item == null)
                {
                    result = i;
                    break;
                }

            }

            return result;
        }

        void OrderByID()
        {
            if (itemList && itemList.items != null && itemList.items.Count > 0)
            {
                var list = itemList.items.OrderBy(i => i.id).ToList();
                itemList.items = list;
            }

        }
    }
}
