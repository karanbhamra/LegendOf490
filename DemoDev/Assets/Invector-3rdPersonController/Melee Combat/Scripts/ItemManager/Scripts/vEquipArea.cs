using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Invector.ItemManager
{
    public class vEquipArea : MonoBehaviour
    {
        public delegate void OnPickUpItem(vEquipArea area, vItemSlot slot);
        public OnPickUpItem onPickUpItemCallBack;

        public vInventory inventory;
        public InventoryWindow rootWindow;
        public vItemWindow itemPicker;
        public List<vEquipSlot> equiSlots;
        public string equipPointName;
        public OnChangeEquipmentEvent onEquipItem;
        public OnChangeEquipmentEvent onUnequipItem;
        [HideInInspector]
        public vEquipSlot currentSelectedSlot;
        private int indexOfEquipedItem;
        public vItem lastEquipedItem;

        void Start()
        {
            inventory = GetComponentInParent<vInventory>();
            rootWindow = GetComponentInParent<InventoryWindow>();
            foreach (vEquipSlot slot in equiSlots)
            {
                slot.onSubmitSlotCallBack = OnSubmitSlot;
                slot.onSelectSlotCallBack = OnSelectSlot;
                slot.onDeselectSlotCallBack = OnDeselect;
                slot.amountText.text = "";
            }
          
        }

        public void OnSubmitSlot(vItemSlot slot)
        {
            if (itemPicker != null)
            {
                currentSelectedSlot = slot as vEquipSlot;
                itemPicker.gameObject.SetActive(true);
                itemPicker.CreateEquipmentWindow(inventory.items, currentSelectedSlot.itemType, slot.item, OnPickItem);
            }
        }

        public void RemoveItem(vEquipSlot slot)
        {
            if (slot)
            {
                slot.RemoveItem();
            }
        }

        public void RemoveItem()
        {
            if (currentSelectedSlot)
            {
                
                {
                    var _item = currentSelectedSlot.item; 
                    onUnequipItem.Invoke(this, _item);
                    currentSelectedSlot.RemoveItem();
                }
            }
        }

        public void OnSelectSlot(vItemSlot slot)
        {
            currentSelectedSlot = slot as vEquipSlot;
        }

        public void OnDeselect(vItemSlot slot)
        {
            currentSelectedSlot = null;
        }

        public void OnPickItem(vItemSlot slot)
        { 
            if (currentSelectedSlot.item != null && slot.item != currentSelectedSlot.item)
            {
                onUnequipItem.Invoke(this, currentSelectedSlot.item);
            }
            if(slot.item!=currentSelectedSlot.item)
            {
                if (onPickUpItemCallBack != null && slot)
                    onPickUpItemCallBack(this, slot);
                if (currentSelectedSlot.item!=null && currentSelectedSlot.item != slot.item)
                    lastEquipedItem = slot.item;
                currentSelectedSlot.AddItem(slot.item);
                onEquipItem.Invoke(this, slot.item);
                if (currentSelectedSlot.item != null) currentSelectedSlot.item.isInEquipArea = false;
                
              
            }          
          
            itemPicker.gameObject.SetActive(false);
        }

        public vItem currentEquipedItem
        {
            get{
                var validEquipSlots = ValidSlots;
                return validEquipSlots[indexOfEquipedItem].item;
            }            
        }

        public void NextEquipSlot()
        {
            if (equiSlots == null || equiSlots.Count == 0) return;
            lastEquipedItem = currentEquipedItem;
            var validEquipSlots = ValidSlots;
            if (indexOfEquipedItem + 1 < validEquipSlots.Count)            
                indexOfEquipedItem++;            
            else            
                indexOfEquipedItem = 0;
         
            onEquipItem.Invoke(this, currentEquipedItem);
            onUnequipItem.Invoke(this, lastEquipedItem);
            
        }

        public void PreviousEquipSlot()
        {
            if (equiSlots == null || equiSlots.Count == 0) return;
            lastEquipedItem = currentEquipedItem;
            var validEquipSlots = ValidSlots;

            if (indexOfEquipedItem - 1 >= 0)
                indexOfEquipedItem--;
            else
                indexOfEquipedItem = validEquipSlots.Count - 1;
            onEquipItem.Invoke(this, currentEquipedItem);
            onUnequipItem.Invoke(this, lastEquipedItem);
           
        }

        public List<vEquipSlot> ValidSlots
        {
          get { return equiSlots.FindAll(slot => slot.isValid); }
        }

        public bool ContainsItem(vItem item)
        {
            return ValidSlots.Find(slot => slot.item == item) != null;
        }
    }   
}
