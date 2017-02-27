using UnityEngine;
using System.Collections;

namespace Invector.ItemManager
{
    public class vEquipSlot : vItemSlot
    {
        public vItemType itemType;

        public override void AddItem(vItem item)
        {
            if (item) item.isInEquipArea = true;
            base.AddItem(item);
        }

        public override void RemoveItem()
        {            
            if (item != null) item.isInEquipArea = false;
            base.RemoveItem();
        }
    }
}

