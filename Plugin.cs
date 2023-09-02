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

namespace NuclearPasta.TheAmbidextrous
{
    [BepInPlugin("dv.theambidextrous", "The Ambidextrous", "1.0.3")]
    class TheAmbidextrousMod : BaseUnityPlugin
    {

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("ambidexterity/super_jump");
        public static readonly GameFeature<float> MeanLizards = GameFloat("ambidexterity/mean_lizards");

        static SlugcatStats.Name MySlugcat = new SlugcatStats.Name("Ambidextrous");
        static SlugcatStats.Name SecretScug = new SlugcatStats.Name("Breadlord");
        //int Customeeinput = 0; //do not delete

        //important storage bools
        public static bool DoesPlayerExist;
        public static bool IsPlayerAlive;
        public void OnEnable()
        {

            On.Player.Jump += Player_Jump;
            On.Lizard.ctor += Lizard_ctor;
            On.Player.Grabability += DoubleEnergyCell;
            On.Player.Grabability += DoubleSpear;
            On.Player.SwallowObject += Player_SwallowObject1;
            On.Player.SwallowObject += Player_SwallowObject2;
            On.Player.SwallowObject += Player_SwallowObject3;
            On.Player.SwallowObject += Player_SwallowObject4;
            On.Player.SwallowObject += Player_SwallowObject5;
            On.Player.SwallowObject += Player_SwallowObject6;
            
            On.Player.Grabability += DoubleCada;
            
            On.SlugcatStats.HiddenOrUnplayableSlugcat += PlayerSlugcatStats_HideSecretScug;
            
            //On.Player.Update += Player_OnMushrooms; //currently unused, do not delete
            //On.Player.Update += Player_AbsoluteSaint;
            On.Player.Grabability += Player_SpearOars;
            On.Player.Update += Player_DoesPlayerExist;
            //On.Player.Update += StartWithSpearOnBack;
        }

        private void StartWithSpearOnBack(On.Player.orig_Update orig, Player self, bool eu)
        {   
            /*
            if (self.spearOnBack != null)
            {
                
                self.Spear = new AbstractSpear(room.world, null, new WorldCoordinate(room.abstractRoom.index, 350, 13, 0), room.game.GetNewID(), false);
                room.abstractRoom.AddEntity(this.spear);
                self.spear.realizedObject.firstChunk.HardSetPosition(self.mainBodyChunk.pos + new Vector2(-30f, 0f));
                self.spearOnBack.SpearToBack(self.spear.realizedObject as Spear);
                
            }
            */
        }

        private void Player_DoesPlayerExist(On.Player.orig_Update orig, Player self, bool eu)
        {
            if(self != null)
            {
                DoesPlayerExist = true;

                if(DoesPlayerExist == true && self.Stunned == false && self.Consious == true)
                {
                    IsPlayerAlive = true;
                }
            }
            
            
        }


        private Player.ObjectGrabability Player_SpearOars(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (IsPlayerAlive == true && self.SlugCatClass == MySlugcat && self.submerged == true)
            {
                if(obj is Spear)
                {
                    
                }
            }
            return orig(self, obj);
        }



        private void Player_AbsoluteSaint(On.Player.orig_Update orig, Player self, bool eu)
        {
            if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) /*self.input[0].jmp && self.input[0].pckp && self.input[0].thrw && self.input[0].x == 0 && self.input[0].y != 0*/ && self.KarmaIsReinforced == false && self.SlugCatClass == MySlugcat)
            {
                if (self.FoodInStomach >= 9)
                {
                    //self.KarmaIsReinforced = true;
                    (self.abstractCreature.world.game.session as StoryGameSession).saveState.deathPersistentSaveData.reinforcedKarma = true;
                    self.SubtractFood(9);
                }
            }
            orig(self, eu);
        }


        //currently unused, do not delete
        //private void Player_OnMushrooms(On.Player.orig_Update orig, Player self, bool eu)
        //{
        //    if (/*(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) &&*/ (self.input[0].jmp && self.input[0].pckp && self.input[0].thrw && self.input[0].x == 0 && (self.input[0].y != 0 && self.input[0].y > -1)) && self.SlugCatClass == MySlugcat)
        //    {
        //        if (self.FoodInStomach > 0)
        //        {
        //            self.mushroomEffect = 1.0f;
        //        }
        //    }
        //    orig(self, eu);
        //}


        private bool PlayerSlugcatStats_HideSecretScug(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (/*!(SecretScugUnlocked = false) && */SecretScug == i)
            {
                return true;
            }
            return orig(i);
        }


        private Player.ObjectGrabability DoubleCada(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Cicada && self.SlugCatClass == MySlugcat)
            {
                    return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);
        }


        private void Player_SwallowObject1(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && self.SlugCatClass == MySlugcat && self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(3);
                base.Logger.LogDebug("Chef Mung Daal: noice1");
            }
        }

        private void Player_SwallowObject2(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (self.SlugCatClass == MySlugcat && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(2);
                base.Logger.LogDebug("Chef Mung Daal: noice2");
            }
        }

        private void Player_SwallowObject3(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (self.SlugCatClass == MySlugcat && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(1);
                base.Logger.LogDebug("Chef Mung Daal: noice3");
            }
        }

        private void Player_SwallowObject4(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && self.SlugCatClass == MySlugcat && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Lantern && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                base.Logger.LogDebug("Chef Mung Daal: noice4");
            }
        }

        private void Player_SwallowObject5(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (ModManager.MSC && self.SlugCatClass == MySlugcat && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(7);
                base.Logger.LogDebug("Chef Mung Daal: noice5");
            }
        }

        private void Player_SwallowObject6(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig.Invoke(self, grasp);
            if (self.SlugCatClass == MySlugcat && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.BubbleGrass && !(self.FoodInStomach == 14))
            {
                self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), -1, -1, null);
                self.AddFood(2);
                base.Logger.LogDebug("Chef Mung Daal: noice6");
            }
        }


        private Player.ObjectGrabability DoubleEnergyCell(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (obj is EnergyCell && self.SlugCatClass == MySlugcat)
            {
                    return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);
            //if this is not my scug, then default behavior.
            //if it is my scug and it is the object type of EnergyCell then change the grabability to only using one hand
            //bool flag = DualEnergyCell.TryGet(self, out bool dualenergycellbool) && dualenergycellbool;
            //if (self.slugcatStats.name.value == "The Ambidextrous" && obj is EnergyCell && flag == true)
            //{
            //    return (Player.ObjectGrabability)1; 
            //}
            //return orig(self, obj);
        }

        private Player.ObjectGrabability DoubleSpear(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (obj is Spear && self.SlugCatClass == MySlugcat)
            {
                    return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);

            //bool flag2 = DualWielding.TryGet(self, out bool dualwieldbool) && dualwieldbool;
            //if (self.slugcatStats.name.value == "The Ambidextrous" && obj is Weapon && flag2 == true)//if this is not my scug, then default behavior. if it is my scug and it is the object type of Weapon then change the grabability to only using one hand
            //{  
            //    return (Player.ObjectGrabability)1;
            //}
            //return orig(self, obj);
            //if (self.slugcatStats.name.value != "The Ambidextrous")
            //{
            // If this isn't your cat, do default behaviour.
            //return orig(self, obj);
            //}
            //if (obj is Weapon)
            //{
            // Any weapon is dual-wieldable, including spears
            //return Player.ObjectGrabability.OneHand;
            //}
            //else
            //{
            // Do default behaviour otherwise
            //return orig(self, obj);
            //}

        }


        // Implement MeanLizards
        private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if(MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }


        // Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }

    }
}