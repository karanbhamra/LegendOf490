using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;
using UnityEngine.Events;
using System;

namespace Invector.ItemManager
{
    public class vItemCollection : vTriggerAction
    {
        [Header("Use ItemListData for get item reference")]
        public vItemListData itemListData;
        [HideInInspector]
        public List<ItemReference> items = new List<ItemReference>();
        public bool destroyOnCollect = true;
        [SerializeField]
        private OnCollectItems onCollectItems;
        public float onCollectDelay;

        public void OnCollectItems(GameObject target)
        {
            if (items.Count > 0)
            {
                items.Clear();
                StartCoroutine(OnCollect(target));
            }
        }

        IEnumerator OnCollect(GameObject target)
        {
            yield return new WaitForSeconds(onCollectDelay);

            onCollectItems.Invoke(target);
            if (destroyOnCollect) Destroy(this.gameObject);
        }      
    }   
}

