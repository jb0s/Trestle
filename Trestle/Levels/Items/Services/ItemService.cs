using System;
using Trestle.Enums;
using System.Reflection;
using Trestle.Exceptions.Items;
using System.Collections.Generic;
using Trestle.Levels.Items.Attributes;

namespace Trestle.Levels.Items.Services
{
    public interface IItemService
    {
        public IItem GetItem(Material material);

        public bool ItemExists(Material material);
        
        public void RegisterItems();
    }
    
    public class ItemService : IItemService
    {
        private readonly Dictionary<Material, Type> _items = new();

        public ItemService()
        {
            RegisterItems();
        }
        
        public IItem GetItem(Material material)
        {
            if (!ItemExists(material))
                throw new ItemDoesntExistException();

            return (IItem)Activator.CreateInstance(_items[material]);
        }

        public bool ItemExists(Material material)
            => _items.ContainsKey(material);
        
        public void RegisterItems()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<ItemAttribute>();
                if (attribute == null) 
                    continue;
                
                _items.Add(attribute.Material, type);
            }
        }
    }
}