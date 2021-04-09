using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Items;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.ClickWindow)]
    public class ClickWindow : Packet
    {
        [Field]
        public byte WindowId { get; set; }
        
        [Field]
        public short Slot { get; set; }
        
        [Field]
        public byte Button { get; set; }
        
        [Field]
        public short ActionNumber { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public InventoryOperation Mode { get; set; }
        
        [Field]
        public ItemStack ClickedItem { get; set; }

        public override void HandlePacket()
        {
            var item = Inventory.ClickedItem;
            
            switch (Mode)
            {
                case InventoryOperation.MouseClick:
                    if (Inventory[Slot].ItemId == -1)
                    {
                        if (item == null)
                            return;

                        if (Button == 0)
                        {
                            Inventory.SetSlot(Slot, item);
                            Inventory.ClearClickedItem();
                        }
                        else
                        {
                            Inventory.SetSlot(Slot, item.ItemId, 1, item.Metadata);
                            Inventory.ClickedItem.ItemCount--;
                        }
                    }
                    else
                    {
                        if (Inventory.ClickedItem?.ItemId == Inventory[Slot].ItemId)
                        {
                            if (Button == 0)
                            {
                                Inventory.SetSlot(Slot, Inventory.ClickedItem.ItemId, Inventory[Slot].ItemCount + Inventory.ClickedItem.ItemCount);
                                Inventory.ClearClickedItem();
                            }
                            else
                            {
                                Inventory.SetSlot(Slot, Inventory.ClickedItem.ItemId, Inventory[Slot].ItemCount + 1);
                                Inventory.ClickedItem.ItemCount--;
                            }
                        }
                        else
                        {
                            Inventory.SetSlot(Slot, Inventory.ClickedItem?.ItemId ?? -1, Inventory.ClickedItem?.ItemCount ?? 0);
                            Inventory.ClickedItem = ClickedItem;
                        }
                    }
                    break;
                case InventoryOperation.ShiftMouseClick:
                    break;
                case InventoryOperation.NumberKeys:
                    break;
                case InventoryOperation.MiddleClick:
                    break;  
                case InventoryOperation.Drop:
                    break;
                case InventoryOperation.MouseDrag:
                    if (Slot == -999)
                    {
                        if (Button == 0 || Button == 4 || Button == 8)
                            Inventory.IsDragging = true;
                        else if (Button == 2 || Button == 6 || Button == 10)
                            Inventory.IsDragging = false;
                        return;
                    }
                    
                    // Left Mouse
                    if (Button == 1)
                    {
                        // TODO: handle this properly probably
                        Inventory.SetSlot(Slot, item);
                        Inventory.ClearClickedItem();
                        return;
                    }
                    
                    // Right Mouse
                    if (Button == 5)
                    {
                        Inventory.SetSlot(Slot, item.ItemId, 1, item.Metadata);
                        Inventory.ClickedItem.ItemCount--;
                        return;
                    }

                    // Middle Mouse
                    if (Button == 9)
                    {
                        // Vanilla client sends middle mouse packet even if it isn't creative, so this is needed.
                        if (Client.Player.GameMode != GameMode.Creative)
                            return;
                    }
                    break;
                case InventoryOperation.DoubleClick:
                    break;
            }
        }
    }
}