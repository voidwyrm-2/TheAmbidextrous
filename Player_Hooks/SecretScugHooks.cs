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
    public class SecretScugHooks
    {

        public static readonly SlugcatStats.Name SecretScug = new SlugcatStats.Name("Breadlord");
        public static bool IsOvenTimerDone;
        //private static bool SecretScugUnlocked = false;

        private static readonly bool DebugSecretScugReenable = true;
        public static void OnEnable()
        {

            On.Player.Update += Player_OvenTimerCheck;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += PlayerSlugcatStats_HideSecretScug;

        }

        public static void Awake()
        {
            On.MoreSlugcats.SingularityBomb.ctor += new On.MoreSlugcats.SingularityBomb.hook_ctor(SecretScugHooks.Player_SecretScugZeroMode_ctor);
        }


        private static bool PlayerSlugcatStats_HideSecretScug(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (/*SecretScugUnlocked && */IsOvenTimerDone && !DebugSecretScugReenable)
            {
                return true;
            }
            return orig(i);
        }

        private static void Player_SecretScugZeroMode_ctor(On.MoreSlugcats.SingularityBomb.orig_ctor orig, SingularityBomb self, AbstractPhysicalObject abstractPhysicalObject, World world)
        {   
            if (IsOvenTimerDone)
            {
                self.zeroMode = true;
            }
            
            orig.Invoke(self, abstractPhysicalObject, world);
        }
        

        private static void Player_OvenTimerCheck(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.SlugCatClass == SecretScug)
            {
                IsOvenTimerDone = true;
            }
            orig(self, eu);
        }

    }
}
