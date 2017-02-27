using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;

namespace Invector.ItemManager
{
    public class vEquipAreaControl : MonoBehaviour
    {
        public List<vEquipArea> equipAreas;

        void Start()
        {
            equipAreas = GetComponentsInChildren<vEquipArea>().vToList();
            foreach (vEquipArea area in equipAreas)            
                area.onPickUpItemCallBack = OnPickUpItemCallBack;
            
            vInventory inventory = GetComponentInParent<vInventory>();
            if (inventory)
                inventory.onOpenCloseInventory.AddListener(OnOpen);
        }

        public void OnOpen(bool value)
        {
        //    if (value)
        //    {
        //        for (int i = 0; i < equipAreas.Count; i++)
        //        {
        //            for (int a = 0; a < equipAreas[i].equiSlots.Count; a++)
        //            {
        //                if (equipAreas[i].equiSlots[a].item == null)
        //                {
        //                   // equipAreas[i].onUnequipItem.Invoke(equipAreas[i], equipAreas[i].equiSlots[a].item);
        //                    equipAreas[i].equiSlots[a].RemoveItem();                            
        //                }
                         
        //            }
        //        }
        //    }
        }

        public void OnPickUpItemCallBack(vEquipArea area, vItemSlot slot)
        {
            for (int i = 0; i < equipAreas.Count; i++)
            {
                var sameSlots = equipAreas[i].equiSlots.FindAll(_slot => _slot.item != null && _slot.item == slot.item);
                for (int a = 0; a < sameSlots.Count; a++)
                {
                    equipAreas[i].onUnequipItem.Invoke(equipAreas[i], sameSlots[a].item);
                    equipAreas[i].RemoveItem(sameSlots[a]);
                }
                   
            }
        }
    }

}
