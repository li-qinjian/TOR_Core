using HarmonyLib;
using System.Collections.Generic;
using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TOR_Core.CharacterDevelopment;
using TOR_Core.Extensions;
using TaleWorlds.Library;

namespace TOR_Core.HarmonyPatches
{
    [HarmonyPatch]
    public static class ItemPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WeaponComponentData), "GetRelevantSkillFromWeaponClass")]
        public static bool AddGunpowderRelevantSkill(ref SkillObject __result, WeaponClass weaponClass)
        {
            if (weaponClass == WeaponClass.Cartridge || weaponClass == WeaponClass.Musket || weaponClass == WeaponClass.Pistol)
            {
                __result = TORSkills.GunPowder;
                return false;
            }
            else return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "IsItemPreferredForTown")]
        public static void OnlyProduceTorItems(ref bool __result, ItemObject item, Town townComponent)
        {
            if (__result && item.Culture == townComponent.Culture) __result = item.IsTorItem();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "GetRandomItemAux")]
        public static bool OnlyProduceCultureMatchingItems(ref EquipmentElement __result, ItemCategory itemGroupBase, Town townComponent, Dictionary<ItemCategory, List<ItemObject>> ____itemsInCategory)
        {
            var possibleItems = new List<ItemObject>();
            if (____itemsInCategory.TryGetValue(itemGroupBase, out var allItemsInGroup))
            {
                foreach (ItemObject obj in allItemsInGroup)
                {
                    if (townComponent != null && obj.Culture == townComponent.Culture && obj.ItemCategory == itemGroupBase)
                    {
                        possibleItems.Add(obj);
                    }
                }
                if (possibleItems.Count < 1) return true;
                ItemObject itemObject = possibleItems.GetRandomElementInefficiently();

                ItemModifierGroup itemModifierGroup = null;
                if (itemObject != null)
                {
                    ItemComponent itemComponent = itemObject.ItemComponent;
                    itemModifierGroup = itemComponent?.ItemModifierGroup;
                }

                ItemModifier itemModifier = null;
                if (itemModifierGroup != null)
                {
                    itemModifier = itemModifierGroup.GetRandomItemModifierProductionScoreBased();
                }

                __result = new EquipmentElement(itemObject, itemModifier, null, false);
                return false;
            }
            return true;
        }
    }
}
