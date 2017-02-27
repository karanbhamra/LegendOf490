using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.ItemManager
{
    [System.Serializable]
    public class vItemAttribute
    {
        public string name = "";
        public int value;
        public vItemAttribute(string name, int value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public static class vItemAttributeHelper
    {
        public static bool Contains(this List<vItemAttribute> attributes, string name)
        {
            var attribute = attributes.Find(at => at.name == name);
            return attribute != null;
        }
        public static vItemAttribute GetAttributeByType(this List<vItemAttribute> attributes, string name)
        {
            var attribute = attributes.Find(at => at.name == name);
            return attribute;
        }
        public static bool Equals(this vItemAttribute attributeA, vItemAttribute attributeB)
        {
            return attributeA.name == attributeB.name;
        }
    }

}
