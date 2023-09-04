using UnityEngine;
using RWCustom;
using On;
using PorcupineCat;
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
using System;
using BepInEx;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using System.Runtime.CompilerServices;
using MoreSlugcats;
using Noise;
using DevInterface;

namespace NuclearPasta.TheAmbidextrous.Player_Hooks
{
    public class Leeching
    {
        /*
        public void OnEnable()
        {
            //On.Player.ThrownSpear += Player_ThrownSpear;
        }

        
        private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            throw new System.NotImplementedException();
        }


        public override void OnKill(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (!victim.Template.smallCreature) 
            {
                int num = Mathf.Clamp(Mathf.CeilToInt(victim.TotalMass), 1, 8);
                for (int i = 0; i < num; i++)
                {
                    //player.AddQuarterFood();

                    player.room.AddObject(new LeechParticle(self, victim));
                }
            }
        }
        */
    }
}
