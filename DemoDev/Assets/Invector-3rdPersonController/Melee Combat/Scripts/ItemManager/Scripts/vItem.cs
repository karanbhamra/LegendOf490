using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.ItemManager
{
    [System.Serializable]
    public class vItem : ScriptableObject
    {
        #region SerializedProperties in customEditor
        [HideInInspector]
        public int id;
        [HideInInspector]
        public string description = "Item Description";
        [HideInInspector]
        public vItemType type;
       
        [HideInInspector]
        public Sprite icon;
        [HideInInspector]
        public bool stackable = true;
        [HideInInspector]
        public int maxStack;
        [HideInInspector]
        public int amount;
        [HideInInspector]
        public GameObject originalObject;
        [HideInInspector]
        public GameObject dropObject;       
        [HideInInspector]
        public List<vItemAttribute> attributes = new List<vItemAttribute>();
        [HideInInspector]
        public bool isInEquipArea;
        #endregion

        #region Properties in defaultInspector
        //[Header("Usable Settings")]
        //public int UseID;
        //public float useDelayTime = 0.5f;
        [Header("Equipable Settings")]
        public int EquipID;
        public string customEquipPoint = "defaultPoint";
        public float equipDelayTime = 0.5f;
        #endregion
    }
}

