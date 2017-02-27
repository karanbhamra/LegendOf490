using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text;

namespace Invector.ItemManager
{
    public delegate void OnSubmitSlot(vItemSlot slot);
    public delegate void OnSelectSlot(vItemSlot slot);
    public delegate void OnCompleteSlotList(List<vItemSlot> slots);

    public class vItemWindow : MonoBehaviour
    {
        public vItemSlot slotPrefab;
        public RectTransform contentWindow;
        public Text itemtext;
        private OnSubmitSlot onSubmitSlot;
        private OnSelectSlot onSelectSlot;
        public OnCompleteSlotList onCompleteSlotListCallBack;
        public List<vItemSlot> slots;
        private vItem currentItem;
        private StringBuilder text;
        public void CreateEquipmentWindow(List<vItem> items, OnSubmitSlot onPickUpItemCallBack = null, OnSelectSlot onSelectSlotCallBack = null, bool destroyAdictionSlots = true)
        {
            if (items.Count == 0)
            {
                if (itemtext) itemtext.text = "";
                if (slots.Count > 0 && destroyAdictionSlots)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        Destroy(slots[i].gameObject);
                    }
                    slots.Clear();
                }
                return;
            }

            bool selecItem = false;
            onSubmitSlot = onPickUpItemCallBack;
            onSelectSlot = onSelectSlotCallBack;
            if (slots == null) slots = new List<vItemSlot>();
            var count = slots.Count;           
            if (count < items.Count)
            {
                for (int i = count; i < items.Count; i++)
                {
                    var slot = Instantiate(slotPrefab) as vItemSlot;
                    slots.Add(slot);
                }
            }
            else if(count > items.Count)
            {
                for (int i = count-1; i > items.Count-1; i--)
                {
                    Destroy(slots[slots.Count - 1].gameObject);
                    slots.RemoveAt(slots.Count-1);
                 
                }
            }
            count = slots.Count;          
            for (int i = 0; i < items.Count; i++)
            {
                vItemSlot slot = null;
                if (i < items.Count)
                {
                    slot = slots[i];
                    slot.AddItem(items[i]);
                    slot.CheckItem(items[i].isInEquipArea);
                    slot.onSubmitSlotCallBack = OnSubmit;
                    slot.onSelectSlotCallBack = OnSelect;
                    var rectTranform = slot.GetComponent<RectTransform>();
                    rectTranform.SetParent(contentWindow);
                    rectTranform.localPosition = Vector3.zero;

                    rectTranform.localScale = Vector3.one;
                    if (currentItem != null && currentItem == items[i])
                    {
                        selecItem = true;
                        SetSelectable(slot.gameObject);
                    }
                }
            }

            if (slots.Count > 0 && !selecItem)
            {
                StartCoroutine(SetSelectableHandle(slots[0].gameObject));
            }

            if (onCompleteSlotListCallBack != null)
            {
                onCompleteSlotListCallBack(slots);
            }
        }

        public void CreateEquipmentWindow(List<vItem> items, vItemType type, vItem currentItem = null, OnSubmitSlot onPickUpItemCallback = null, OnSelectSlot onSelectSlotCallBack = null)
        {
            this.currentItem = currentItem;
            var _items = items.FindAll(item => item.type == type);
        
            CreateEquipmentWindow(_items, onPickUpItemCallback);
        }

        IEnumerator SetSelectableHandle(GameObject target)
        {
            if (this.enabled)
            {
                yield return new WaitForEndOfFrame();
                SetSelectable(target);
            }
        }

        void SetSelectable(GameObject target)
        {
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.pointerExitHandler);
            EventSystem.current.SetSelectedGameObject(target, new BaseEventData(EventSystem.current));
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.selectHandler);
        }

        public void OnSubmit(vItemSlot slot)
        {
            if (onSubmitSlot != null)
                onSubmitSlot(slot);
        }

        public void OnSelect(vItemSlot slot)
        {
            if(itemtext!=null)
            {
                if (slot.item == null)
                {
                    itemtext.text = "";
                }
                else
                {
                    text = new StringBuilder();
                    text.Append(slot.item.name + "\n");
                    text.AppendLine(slot.item.description);
                    itemtext.text = text.ToString();
                }
            }
            
            if (onSelectSlot != null)
                onSelectSlot(slot);
        }

        public void OnCancel()
        {

        }
    }
}