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
    public class ScugGrabability
    {
        public static void OnEnable()
        {

            On.Player.Grabability += DoubleEnergyCell;
            On.Player.Grabability += DoubleSpear;
            On.Player.Grabability += DoubleCada;

        }

        private static Player.ObjectGrabability DoubleCada(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Cicada && TheAmbidextrousMod.IsPlayerAmbidextrous == true)
            {
                return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);
        }

        private static Player.ObjectGrabability DoubleEnergyCell(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (obj is EnergyCell && TheAmbidextrousMod.IsPlayerAmbidextrous == true)
            {
                return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);

        }

        private static Player.ObjectGrabability DoubleSpear(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            if (obj is Spear && TheAmbidextrousMod.IsPlayerAmbidextrous == true)
            {
                return (Player.ObjectGrabability)1;
            }
            return orig(self, obj);

        }

    }
}
