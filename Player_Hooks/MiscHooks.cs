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
    public class MiscHooks
    {
        public static int meanness = 1;
        public static float power = 0.7f;
        
        public static void OnEnable()
        {

            On.Player.Jump += Player_Jump;
            On.Lizard.ctor += Lizard_ctor;

        }


        private static void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            if (TheAmbidextrousMod.IsPlayerAmbidextrous || SecretScugHooks.IsOvenTimerDone)
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
            orig(self, abstractCreature, world);
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            if (/*TheAmbidextrousMod.IsPlayerAmbidextrous*/ self.SlugCatClass == TheAmbidextrousMod.AmbidextrousSlugcat)
            {
                self.jumpBoost *= 1f + power;
            }
            else if (SecretScugHooks.IsOvenTimerDone)
            {
                self.jumpBoost *= 0f + power;
            }
            orig(self);
        }

    }
}
