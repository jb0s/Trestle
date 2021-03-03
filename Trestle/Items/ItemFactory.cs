using System;
using System.Collections.Generic;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Items
{
    public class ItemFactory
    {
        private static Dictionary<Material, Item> _items = new()
        {
            // TODO: Add this
        };

        public static Item GetItemById(short id, byte metadata = 0)
        {
            Material idToMaterial = (Material) id;
            if (_items.ContainsKey(idToMaterial))
                return _items[idToMaterial];

            return new Item((ushort)id, metadata);
        }
    }
}