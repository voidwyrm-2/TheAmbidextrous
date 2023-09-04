using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using System.Runtime.CompilerServices;
using MoreSlugcats;
using Noise;
using On;
using PorcupineCat;
using RWCustom;
using Pkuyo.Wanderer;
using BepInEx.Logging;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Pkuyo.Wanderer.Feature;
using Pkuyo.Wanderer.Cosmetic;
using IL;
using MonoMod.RuntimeDetour;
using static Menu.Remix.MixedUI.OpTextBox;
using UnityEngine.PlayerLoop;
using DevInterface;

namespace NuclearPasta.TheAmbidextrous.Player_Hooks
{
    public class ItemConversion
    {
        
        public static void OnEnable()
        {

            //Ambi
            On.Player.SwallowObject += Player_BlackholebombToGrenade;
            On.Player.SwallowObject += Player_GrenadeToRock;
            On.Player.SwallowObject += Player_CherrybombToBatnip;
            On.Player.SwallowObject += Player_LanternToGooieduck;
            On.Player.SwallowObject += Player_IggyeyeToPearl;
            On.Player.SwallowObject += Player_BubblegrassToBatnip;

            //Dough
            On.Player.SwallowObject += Player_eeg;

        }


        private static void Player_eeg(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && SecretScugHooks.IsOvenTimerDone && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Rock && self.FoodInStomach == TheAmbidextrousMod.DoughMaxFood)
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(3);
                //base.Logger.LogDebug("Chef Mung Daal: noice1");
            }
        }


        //Ambi
        private static void Player_BlackholebombToGrenade(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(3);
                //base.Logger.LogDebug("Chef Mung Daal: noice1");
            }
        }

        private static void Player_GrenadeToRock(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(2);
                //base.Logger.LogDebug("Chef Mung Daal: noice2");
            }
        }

        private static void Player_CherrybombToBatnip(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(1);
                //base.Logger.LogDebug("Chef Mung Daal: noice3");
            }
        }

        private static void Player_LanternToGooieduck(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Lantern && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                //base.Logger.LogDebug("Chef Mung Daal: noice4");
            }
        }

        private static void Player_IggyeyeToPearl(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(7);
                //base.Logger.LogDebug("Chef Mung Daal: noice5");
            }
        }

        private static void Player_BubblegrassToBatnip(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (TheAmbidextrousMod.IsPlayerAmbidextrous == true && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.BubbleGrass && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(2);
                //base.Logger.LogDebug("Chef Mung Daal: noice6");
            }
        }









    }
}
