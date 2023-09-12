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
using NuclearPasta.TheAmbidextrous.Player_Hooks;

namespace NuclearPasta.TheAmbidextrous
{
    [BepInPlugin("dv.theambidextrous", "The Ambidextrous", "1.0.3")]
    class TheAmbidextrousMod : BaseUnityPlugin
    {

        public static readonly SlugcatStats.Name AmbidextrousSlugcat = new SlugcatStats.Name("Ambidextrous");
        public static readonly SlugcatStats.Name SecretScug = new SlugcatStats.Name("Breadlord");
        public static readonly int AmbiMaxFood = 7;
        public static readonly int DoughMaxFood = 14;
        //int Customeeinput = 0; //do not delete

        //important storage bools
        public static bool DoesPlayerExist;
        public static bool IsPlayerAlive;
        public static bool IsPlayerAmbidextrous;
        public static bool TestAscention;
        public void OnEnable()
        {
            //ExternalClassTemplate.OnEnable(); //use this one as a template, please do not delete

            MiscHooks.OnEnable();
            ScugGrabability.OnEnable();
            ItemConversion.OnEnable();

            //secret scug crap
            SecretScugHooks.OnEnable();
            SecretScugHooks.Awake();


            //On.Player.Update += Player_OnMushrooms; //currently unused, do not delete
            //On.Player.Update += Player_AbsoluteSaint;
            //On.Player.Grabability += Player_SpearOars;
            On.Player.Update += Player_DoesPlayerExist;
            On.Player.Update += StartWithSpearOnBack;
            On.Player.Update += Player_CheckforAmbidexterity;

            On.Player.Destroy += PlayerDestroyHook;
            On.Player.Update += Player_Die;

        }

        //self.owner.room.game.cameras[0].hud.textPrompt.AddMessage(self.owner.room.game.rainWorld.inGameTranslator.Translate("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH"), 0, 200, false, false);

        private void Player_Die(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (SecretScugHooks.IsOvenTimerDone && Input.GetKeyDown(KeyCode.LeftControl))
            {
                //this.hurtLevel = 0f;
                self.playerState.permanentDamageTracking = 0.0;
            }
            orig(self, eu);
        }

        private void PlayerDestroyHook(On.Player.orig_Destroy orig, Player self)
        {
            orig.Invoke(self);
            self.Die();
            orig(self);
        }


        private void Player_CheckforAmbidexterity(On.Player.orig_Update orig, Player self, bool eu)
        {
            if(self.SlugCatClass == AmbidextrousSlugcat)
            {
                IsPlayerAmbidextrous = true;
            }
            orig(self, eu);
        }

        private void StartWithSpearOnBack(On.Player.orig_Update orig, Player self, bool eu)
        {
            /*
            if (self.spearOnBack != null)
            {
                
                self.Spear = new AbstractSpear(room.world, null, new WorldCoordinate(room.abstractRoom.index, 350, 13, 0), room.game.GetNewID(), false);
                room.abstractRoom.AddEntity(self.spear);
                self.spear.realizedObject.firstChunk.HardSetPosition(self.mainBodyChunk.pos + new Vector2(-30f, 0f));
                self.spearOnBack.SpearToBack(self.spear.realizedObject as Spear);
                
            }
            */
            orig(self, eu);
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
            orig(self, eu);
        }


        /*
        private Player.ObjectGrabability Player_SpearOars(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (IsPlayerAlive == true && self.SlugCatClass == AmbidextrousSlugcat && self.submerged == true)
            {
                if(obj is Spear)
                {
                    if(obj.grabbedBy[0].grabber is Player && obj.grabbedBy[1].grabber is Player)
                    {

                    }
                }
            }
            return orig(self, obj);
        }
        */


        private void Player_AbsoluteSaint(On.Player.orig_Update orig, Player self, bool eu)
        {
            if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) /*self.input[0].jmp && self.input[0].pckp && self.input[0].thrw && self.input[0].x == 0 && self.input[0].y != 0*/ && self.KarmaIsReinforced == false && self.SlugCatClass == AmbidextrousSlugcat)
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

    }
}