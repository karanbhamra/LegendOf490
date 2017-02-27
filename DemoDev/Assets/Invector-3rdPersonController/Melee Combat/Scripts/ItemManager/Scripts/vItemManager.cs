using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;
using System;
using Invector.CharacterController;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace Invector.ItemManager
{
    public class vItemManager : MonoBehaviour
    {
        public vInventory inventoryPrefab;
        [HideInInspector]
        public vInventory inventory;
        public vItemListData itemListData;
        public bool dropItemsWhenDead;

        #region SerializedProperties in Custom Editor

        [SerializeField]
        public List<ItemReference> startItems = new List<ItemReference>();

        public List<vItem> items;
        public OnUseItemEvent onUseItem;
        public OnOpenCloseInventory onOpenCloseInventory;
        public OnChangeEquipmentEvent onEquipItem, onUnequipItem;
        [SerializeField]
        public List<EquipPoint> equipPoints;
        [SerializeField]
        public List<ApplyAttributeEvent> applyAttributeEvents;
        #endregion
        private bool inEquip;
        private float equipTimer;
        private Animator animator;
	    private static vItemManager instance;
	    
        void Start()
	    {
		    if(instance!=null)return;
            inventory = FindObjectOfType<vInventory>();
	       
		    instance = this;
            if (!inventory && inventoryPrefab)
            {
                inventory = Instantiate(inventoryPrefab);
            }

            if (inventory)
            {
                inventory.GetItemsHandler = GetItems;
                inventory.onEquipItem.AddListener(EquipItem);
                inventory.onUnequipItem.AddListener(UnequipItem);
                inventory.onDropItem.AddListener(DropItem);
                inventory.onLeaveItem.AddListener(LeaveItem);
                inventory.onUseItem.AddListener(UseItem);
                inventory.onOpenCloseInventory.AddListener(OnOpenCloseInventory);
            }
		    
            if (dropItemsWhenDead)
            {
                var character = GetComponent<vCharacter>();
                if (character)
                    character.onDead.AddListener(DropAllItens);
            }

            items = new List<vItem>();
            if (itemListData)
            {
                for (int i = 0; i < startItems.Count; i++)
                {
                    AddItem(startItems[i].id, startItems[i].amount);
                }
            }
		    
            animator = GetComponent<Animator>();
        }

        public void CreateDefaultEquipPoints()
        {
            var manager = GetComponent<vMeleeManager>();
            if (manager == null)
            {
                Debug.LogWarning("ItemManager work need a vMeleeManager Component\nPlease add vMeleeManager Component to " + gameObject.name, gameObject);
            }
	        
            animator = GetComponent<Animator>();
            if (equipPoints == null)
                equipPoints = new List<EquipPoint>();

            #region LeftEquipPoint
            var equipPointL = equipPoints.Find(p => p.equiPointName == "LeftArm");
            if (equipPointL == null)
            {
                EquipPoint pointL = new EquipPoint();
                pointL.equiPointName = "LeftArm";
                if (manager)
                {
#if UNITY_EDITOR
                    UnityEventTools.AddPersistentListener<GameObject>(pointL.onInstantiateEquiment, manager.SetLeftWeapon);
#else
                    pointL.onInstantiateEquiment.AddListener(manager.SetLeftWeapon);
#endif
                }

                if (animator)
                {
                    var defaultEquipPointL = new GameObject("defaultEquipPoint");
                    var parent = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    defaultEquipPointL.transform.SetParent(parent);
                    defaultEquipPointL.transform.localPosition = Vector3.zero;
                    defaultEquipPointL.transform.forward = transform.forward;
                    pointL.defaultPoint = defaultEquipPointL.transform;
                }
                equipPoints.Add(pointL);
            }
            else
            {
                if (equipPointL.defaultPoint == null)
                {
                    if (animator)
                    {
                        var parent = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                        var defaultPoint = parent.FindChild("defaultEquipPoint");
	                    
	                    if (defaultPoint) 
		                    equipPointL.defaultPoint = defaultPoint;
                        else
                        {
                            var _defaultPoint = new GameObject("defaultEquipPoint");
                            _defaultPoint.transform.SetParent(parent);
                            _defaultPoint.transform.localPosition = Vector3.zero;
                            _defaultPoint.transform.forward = transform.forward;
                            equipPointL.defaultPoint = _defaultPoint.transform;
                        }
                    }
                }

                bool containsListener = false;
                for (int i = 0; i < equipPointL.onInstantiateEquiment.GetPersistentEventCount(); i++)
                {
                    if (equipPointL.onInstantiateEquiment.GetPersistentMethodName(i).Equals("SetLeftWeapon"))
                    {
                        containsListener = true;
                        break;
                    }
                }

                if (!containsListener && manager)
                {
#if UNITY_EDITOR
                    UnityEventTools.AddPersistentListener<GameObject>(equipPointL.onInstantiateEquiment, manager.SetLeftWeapon);
#else                    
                    equipPointL.onInstantiateEquiment.AddListener(manager.SetLeftWeapon);
#endif
                }
            }
            #endregion

            #region RightEquipPoint
            var equipPointR = equipPoints.Find(p => p.equiPointName == "RightArm");
            if (equipPointR == null)
            {
                EquipPoint pointR = new EquipPoint();
                pointR.equiPointName = "RightArm";
                if (manager)
                {
#if UNITY_EDITOR
                    UnityEventTools.AddPersistentListener<GameObject>(pointR.onInstantiateEquiment, manager.SetRightWeapon);
#else
                    pointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
#endif
                }

                if (animator)
                {
                    var defaultEquipPointR = new GameObject("defaultEquipPoint");
                    var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    defaultEquipPointR.transform.SetParent(parent);
                    defaultEquipPointR.transform.localPosition = Vector3.zero;
                    defaultEquipPointR.transform.forward = transform.forward;
                    pointR.defaultPoint = defaultEquipPointR.transform;
                }
                equipPoints.Add(pointR);
            }
            else
            {
                if (equipPointR.defaultPoint == null)
                {
                    if (animator)
                    {
                        var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                        var defaultPoint = parent.FindChild("defaultEquipPoint");
                        if (defaultPoint) equipPointR.defaultPoint = defaultPoint;
                        else
                        {
                            var _defaultPoint = new GameObject("defaultEquipPoint");
                            _defaultPoint.transform.SetParent(parent);
                            _defaultPoint.transform.localPosition = Vector3.zero;
                            _defaultPoint.transform.forward = transform.forward;
                            equipPointR.defaultPoint = _defaultPoint.transform;
                        }
                    }
                }

                bool containsListener = false;
                for (int i = 0; i < equipPointR.onInstantiateEquiment.GetPersistentEventCount(); i++)
                {
                    if (equipPointR.onInstantiateEquiment.GetPersistentMethodName(i).Equals("SetRightWeapon"))
                    {
                        containsListener = true;
                        break;
                    }
                }

                if (!containsListener && manager)
                {
#if UNITY_EDITOR
                    UnityEventTools.AddPersistentListener<GameObject>(equipPointR.onInstantiateEquiment, manager.SetRightWeapon);
#else
                    equipPointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
#endif
                }
            }
            #endregion
        }

        public List<vItem> GetItems()
        {
            return items;
        }

        /// <summary>
        /// Add new Instance of Item to itemList
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(int itemID, int amount)
        {
            if (itemListData != null && itemListData.items.Count > 0)
            {
                var item = itemListData.items.Find(t => t.id.Equals(itemID));
                if (item)
                {
                    var sameItems = items.FindAll(i => i.stackable && i.id == item.id && i.amount < i.maxStack);
                    if (sameItems.Count == 0)
                    {
                        var _item = Instantiate(item);
                        _item.name = _item.name.Replace("(Clone)", string.Empty);
                        _item.amount = 0;
                        for (int i = 0; i < item.maxStack && _item.amount < _item.maxStack && amount > 0; i++)
                        {
                            _item.amount++;
                            amount--;
                        }

                        items.Add(_item);
                        if (amount > 0)
                        {
                            AddItem(item.id, amount);
                        }
                    }
                    else
                    {
                        var indexOffItem = items.IndexOf(sameItems[0]);

                        for (int i = 0; i < items[indexOffItem].maxStack && items[indexOffItem].amount < items[indexOffItem].maxStack && amount > 0; i++)
                        {
                            items[indexOffItem].amount++;
                            amount--;
                        }
                        if (amount > 0)
                        {
                            AddItem(item.id, amount);
                        }
                    }
                }
            }
        }

        public void UseItem(vItem item)
        {
            if (item)
            {
                onUseItem.Invoke(item);
                if (item.attributes != null && item.attributes.Count > 0 && applyAttributeEvents.Count > 0)
                {
                    foreach (ApplyAttributeEvent attributeEvent in applyAttributeEvents)
                    {
                        var attributes = item.attributes.FindAll(a => a.name.Equals(attributeEvent.attributeName));
                        foreach (vItemAttribute attribute in attributes)
                            attributeEvent.onApplyAttribute.Invoke(attribute.value);
                    }
                }
                if (item.amount <= 0 && items.Contains(item)) items.Remove(item);
            }
        }

        public void LeaveItem(vItem item, int amount)
        {
            item.amount -= amount;
            if (item.amount <= 0 && items.Contains(item))
            {
                if (item.type != vItemType.Consumable)
                {
                    var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == item);
                    if (equipPoint != null && equipPoint.equipmentReference.equipedObject != null)
                        UnequipItem(equipPoint.area, equipPoint.equipmentReference.item);
                }
                items.Remove(item);
                Destroy(item);
            }
        }

        public void DropItem(vItem item, int amount)
        {
            item.amount -= amount;

            if (item.amount <= 0 && items.Contains(item))
            {
                if (item.type != vItemType.Consumable)
                {
                    var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == item);
                    if (equipPoint != null && equipPoint.equipmentReference.equipedObject != null)
                        UnequipItem(equipPoint.area, equipPoint.equipmentReference.item);
                }
                items.Remove(item);
                Destroy(item);
            }
            if (item.dropObject != null)
            {
                var dropObject = Instantiate(item.dropObject, transform.position, transform.rotation) as GameObject;
                vItemCollection collection = dropObject.GetComponent<vItemCollection>();
                if (collection != null)
                {
                    collection.items.Clear();
                    var itemReference = new ItemReference(item.id);
                    itemReference.amount = amount;
                    collection.items.Add(itemReference);
                }
            }
        }

        public void DropAllItens(GameObject target = null)
        {
            if (target != null && target != gameObject) return;
            List<ItemReference> itemReferences = new List<ItemReference>();
            for (int i = 0; i < items.Count; i++)
            {
                if (itemReferences.Find(_item => _item.id == items[i].id) == null)
                {
                    var sameItens = items.FindAll(_item => _item.id == items[i].id);
                    ItemReference itemReference = new ItemReference(items[i].id);
                    for (int a = 0; a < sameItens.Count; a++)
                    {
                        if (sameItens[a].type != vItemType.Consumable)
                        {
                            var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == sameItens[a]);
                            if (equipPoint != null && equipPoint.equipmentReference.equipedObject != null)
                                UnequipItem(equipPoint.area, equipPoint.equipmentReference.item);
                        }

                        itemReference.amount += sameItens[a].amount;
                        Destroy(sameItens[a]);
                    }
                    itemReferences.Add(itemReference);
                    if (equipPoints != null)
                    {
                        var equipPoint = equipPoints.Find(e => e.equipmentReference != null && e.equipmentReference.item != null && e.equipmentReference.item.id == itemReference.id && e.equipmentReference.equipedObject != null);
                        if (equipPoint != null)
                        {
                            Destroy(equipPoint.equipmentReference.equipedObject);
                            equipPoint.equipmentReference = null;
                        }
                    }
                    if (items[i].dropObject)
                    {
                        var dropObject = Instantiate(items[i].dropObject, transform.position, transform.rotation) as GameObject;
                        vItemCollection collection = dropObject.GetComponent<vItemCollection>();
                        if (collection != null)
                        {
                            collection.items.Clear();
                            collection.items.Add(itemReference);
                        }
                    }
                }
            }
            items.Clear();
        }

        public void EquipItem(vEquipArea equipArea, vItem item)
        {           
            onEquipItem.Invoke(equipArea, item);
            if (item != equipArea.currentEquipedItem) return;
            var equipPoint = equipPoints.Find(ep => ep.equiPointName == equipArea.equipPointName);
            if (equipPoint != null && item != null && equipPoint.equipmentReference.item != item)
            {
                equipTimer = item.equipDelayTime;

                var type = item.type;
                if (type != vItemType.Consumable)
                {

                    if (!inventory.isOpen)
                    {
                        animator.SetInteger("EquipItemID", equipArea.equipPointName.Contains("Right") ? item.EquipID : -item.EquipID);
                        animator.SetTrigger("EquipItem");
                    }
                    equipPoint.area = equipArea;
                    StartCoroutine(EquipItemRoutine(equipPoint, item));
                }
            }
        }

        public void UnequipItem(vEquipArea equipArea, vItem item)
        {

            onUnequipItem.Invoke(equipArea, item);
            //if (item != equipArea.lastEquipedItem) return;
            var equipPoint = equipPoints.Find(ep => ep.equiPointName == equipArea.equipPointName && ep.equipmentReference.item != null && ep.equipmentReference.item == item);
            if (equipPoint != null && item != null)
            {
                equipTimer = item.equipDelayTime;
                var type = item.type;
                if (type != vItemType.Consumable)
                {

                    if (!inventory.isOpen && !inEquip)
                    {
                        animator.SetInteger("EquipItemID", equipArea.equipPointName.Contains("Right") ? item.EquipID : -item.EquipID);

                        animator.SetTrigger("EquipItem");
                    }

                    StartCoroutine(UnequipItemRoutine(equipPoint, item));
                }
            }

        }

        IEnumerator EquipItemRoutine(EquipPoint equipPoint, vItem item)
        {           
            if (!inEquip)
            {
                inventory.canEquip = false;
                inEquip = true;
                if (!inventory.isOpen)
                {
                    while (equipTimer > 0)
                    {
                        equipTimer -= Time.deltaTime;
                        if (item == null) break;
                        yield return new WaitForEndOfFrame();
                    }
                }
                if (equipPoint != null)
                {
                    if (item.originalObject)
                    {
                        if (equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                            Destroy(equipPoint.equipmentReference.equipedObject);

                        var point = equipPoint.customPoints.Find(p => p.name == item.customEquipPoint);
                        var equipTransform = point != null ? point : equipPoint.defaultPoint;
                        var equipedObject = Instantiate(item.originalObject, equipTransform.position, equipTransform.rotation) as GameObject;
                        equipedObject.transform.parent = equipTransform;
                        if(equipPoint.equiPointName.Contains("Left"))
                        {
                            var scale = equipedObject.transform.localScale;
                            scale.x *= -1;
                            equipedObject.transform.localScale = scale;
                        }
                        equipPoint.equipmentReference.item = item;
                        equipPoint.equipmentReference.equipedObject = equipedObject;
                        var equipment = equipedObject.GetComponent<vIEquipment>();
                        if (equipment != null) equipment.OnEquip(item);
                        equipPoint.onInstantiateEquiment.Invoke(equipedObject);
                    }
                    else if (equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                    {
                        Destroy(equipPoint.equipmentReference.equipedObject);
                        equipPoint.equipmentReference.item = null;
                    }

                }
                inEquip = false;
                inventory.canEquip = true;
            }           
        }

        IEnumerator UnequipItemRoutine(EquipPoint equipPoint, vItem item)
        {
            if (!inEquip)
            {
                inventory.canEquip = false;
                inEquip = true;
                if (equipPoint != null && equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                {
                    var equipment = equipPoint.equipmentReference.equipedObject.GetComponent<vIEquipment>();
                    if (equipment != null) equipment.OnUnequip(item);
                    if (!inventory.isOpen)
                    {
                        while (equipTimer > 0)
                        {
                            equipTimer -= Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    Destroy(equipPoint.equipmentReference.equipedObject);
                    equipPoint.equipmentReference.item = null;

                }
                inEquip = false;
                inventory.canEquip = true;
            }
        }

        void OnOpenCloseInventory(bool value)
        {
            onOpenCloseInventory.Invoke(value);
        }
    }


    [System.Serializable]
    public class ItemReference
    {
        public int id;
        public int amount;
        public ItemReference(int id)
        {
            this.id = id;
        }
    }

    [System.Serializable]
    public class EquipPoint
    {
        #region SeralizedProperties in CustomEditor

        [SerializeField]
        public string equiPointName;
        public EquipmentReference equipmentReference = new EquipmentReference();
        [HideInInspector]
        public vEquipArea area;
        public Transform defaultPoint;
        public Transform[] customPoints2;
        public List<Transform> customPoints = new List<Transform>();
        public OnInstantiateItemObjectEvent onInstantiateEquiment = new OnInstantiateItemObjectEvent();

        #endregion
    }

    public class EquipmentReference
    {
        public GameObject equipedObject;
        public vItem item;
    }

    [System.Serializable]
    public class ApplyAttributeEvent
    {
        [SerializeField]
        public string attributeName;
        [SerializeField]
        public OnApplyAttribute onApplyAttribute;
    }

}

