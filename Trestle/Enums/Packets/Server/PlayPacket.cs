﻿namespace Trestle.Enums.Packets.Server
{
    public enum PlayPacket : byte
    {
        TeleportConfirm = 0x00,
        TabComplete = 0x01,
        KeepAlive = 0x0B,
        ChatMessage = 0x02,
        Player = 0x0C,
        PlayerPosition = 0x0D,
        PlayerLook = 0x0F,
        PlayerPositionAndLook = 0x0E,
        PlayerDigging = 0x14,
        PlayerBlockPlacement = 0x1F,
        ClientSettings = 0x04,
        Animation = 0x1D,
        EntityAction = 0x15,
        CreativeInventoryAction = 0x1B,
        PluginMessage = 0x09,
        HeldItemChange = 0x1A,
        ClickWindow = 0x07,
        CloseWindow = 0x08,
        UseItem = 0x20,
        ClientStatus = 0x03,
        UseEntity = 0x0A
    }
}