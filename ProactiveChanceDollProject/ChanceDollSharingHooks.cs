using System.Linq;
using BepInEx.Logging;
using RoR2;
using UnityEngine.Networking;

namespace ProactiveChanceDoll
{
    internal static class ChanceDollSharingHooks
    {
        private static bool _hooked = false;
        private static ItemIndex _chanceDollIndex = ItemIndex.None;
        // private static ManualLogSource log = Logger.CreateLogSource("ProactiveChanceDoll");

        internal static void Hook()
        {
            if (_hooked) {return;}
            On.RoR2.ShrineChanceBehavior.AddShrineStack += AddShrineStack; // hook to default shrine behaviour.
            _hooked = true;
        }

        internal static void UnHook()
        {
            if (!_hooked) {return;}
            On.RoR2.ShrineChanceBehavior.AddShrineStack -= AddShrineStack;
            _hooked = false;
        }

        private static void AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig,
                                            ShrineChanceBehavior shrine, Interactor interactor)
        {
            if (_chanceDollIndex == ItemIndex.None) {
                _chanceDollIndex = RoR2.ItemCatalog.FindItemIndex("ExtraShrineItem");
                // log.LogInfo($"Chance Doll ItemIndex: {_chanceDollIndex}");
            }
            
            if (!NetworkServer.active || !Run.instance.IsExpansionEnabled(ItemCatalog.GetItemDef(_chanceDollIndex).requiredExpansion)) {
                orig(shrine, interactor);
                // log.LogInfo("expansion disabled/network server inactive");
                return;
            }

            var inventory = interactor?.GetComponent<CharacterBody>()?.inventory;
            if (!inventory)
            {
                orig(shrine, interactor);
                // log.LogInfo("!inventory");
                return;
            }

            var lobbyDollCount = PlayerCharacterMasterController.instances
                .Where(player => player.master?.inventory)
                .Sum(player => player.master.inventory.GetItemCountEffective(_chanceDollIndex)); // worst case runtime exists here as O(n); unlikely to be an issue.

            var selfDollCount = inventory.GetItemCountEffective(_chanceDollIndex);
            
            var tempDollCount = lobbyDollCount - selfDollCount;

            if (tempDollCount <= 0)
            {
                orig(shrine, interactor);
                // log.LogInfo("tempDollCount <= 0");
                return;
            }

            inventory.GiveItemPermanent(_chanceDollIndex, tempDollCount);
            try
            {
                orig(shrine, interactor);
                // log.LogInfo($"gave {tempDollCount} Chance Dolls");
            }
            finally
            {
                inventory.RemoveItemPermanent(_chanceDollIndex, tempDollCount);
                // log.LogInfo($"removed {tempDollCount} Chance Dolls");
            }
        }
    }
}
