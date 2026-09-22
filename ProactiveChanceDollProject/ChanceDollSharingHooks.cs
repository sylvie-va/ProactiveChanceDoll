using System.Linq;
using RoR2;
using UnityEngine.Networking;

namespace ProactiveChanceDoll
{
    internal static class ChanceDollSharingHooks
    {
        internal static void Hook()
        {
            On.RoR2.ShrineChanceBehavior.AddShrineStack += AddShrineStack; // hook to default shrine behaviour.
        }

        internal static void UnHook()
        {
            On.RoR2.ShrineChanceBehavior.AddShrineStack -= AddShrineStack;
        }

        private static void AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig,
                                            ShrineChanceBehavior shrine, Interactor interactor)
        {
            if (!NetworkServer.active) {
                orig(shrine, interactor);
                return;
            }

            var body = interactor?.GetComponent<CharacterBody>();
            var inventory = body?.inventory;
            var chanceDollIndex = ItemCatalog.FindItemIndex("ExtraShrineItem");
            if (!inventory || chanceDollIndex == ItemIndex.None) // Early return if SoTS is disabled or some other issue arises.
            {
                orig(shrine, interactor);
                return;
            }

            var lobbyDollCount = PlayerCharacterMasterController.instances
                .Where(player => player.master?.inventory)
                .Sum(player => player.master.inventory.GetItemCountEffective(chanceDollIndex)); // worst case runtime exists here as O(n); unlikely to be an issue.

            var selfDollCount = inventory.GetItemCountEffective(chanceDollIndex);
            
            var tempDollCount = lobbyDollCount - selfDollCount;

            if (tempDollCount <= 0)
            {
                orig(shrine, interactor);
                return;
            }

            inventory.GiveItemPermanent(chanceDollIndex, tempDollCount);
            try
            {
                orig(shrine, interactor);
            }
            finally
            {
                inventory.RemoveItemPermanent(chanceDollIndex, tempDollCount);
            }
        }
    }
}
