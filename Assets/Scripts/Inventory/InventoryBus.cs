using System;

public static class InventoryBus
{
    public static event Action<Item> OnChanged;

    public static void RaiseChanged(Item item) => OnChanged?.Invoke(item);
}