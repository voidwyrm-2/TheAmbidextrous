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
        private static int Customeeinput = 0;
        public static bool SecretScugUnlocked = false;
        public static bool SecretScugReset = false;

        public static void Awake()
        {
            On.MoreSlugcats.SingularityBomb.ctor += new On.MoreSlugcats.SingularityBomb.hook_ctor(SecretScugHooks.Player_SecretScugZeroMode_ctor);
        }


        private static readonly bool DebugSecretScugReenable = false;
        public static void OnEnable()
        {

            On.Player.Update += Player_OvenTimerCheck;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += PlayerSlugcatStats_HideSecretScug;
            On.Menu.MainMenu.Update += MainMenu_SecretCode;
            On.RainWorldGame.RestartGame += RainWorldGame_RestartGame;

        }

        private static void RainWorldGame_RestartGame(On.RainWorldGame.orig_RestartGame orig, RainWorldGame self)
        {
            //if(self.)
            orig(self);
        }

        private static void MainMenu_SecretCode(On.Menu.MainMenu.orig_Update orig, Menu.MainMenu self)
        {
            if (Input.anyKey)
            {
                if (Customeeinput == 0 && Input.GetKey(KeyCode.B))
                {
                    //Customeeinput = 1;
                    Customeeinput += 1;
                    Console.WriteLine("input: " + Customeeinput);

                }
                else if (Customeeinput == 1 && Input.GetKey(KeyCode.R))
                {
                    //Customeeinput = 2;
                    Customeeinput += 1;
                    Console.WriteLine("input: " + Customeeinput);

                }
                else if (Customeeinput == 2 && Input.GetKey(KeyCode.E))
                {
                    //Customeeinput = 3;
                    Customeeinput += 1;
                    Console.WriteLine("input: " + Customeeinput);

                }
                else if (Customeeinput == 3 && Input.GetKey(KeyCode.A))
                {
                    //Customeeinput = 4;
                    Customeeinput += 1;
                    Console.WriteLine("input: " + Customeeinput);

                }
                else if (Customeeinput == 4 && Input.GetKey(KeyCode.D))
                {
                    //Customeeinput = 5;
                    Customeeinput += 1;
                    Console.WriteLine("input: " + Customeeinput);

                }
                else if (Customeeinput == 5 /*&& Input.GetKey(KeyCode.Enter)*/)
                {
                    self.manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = SecretScug;
                    self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.IntroRoll);
                    SecretScugUnlocked = true;
                    Customeeinput = 0;

                }
                else if (SecretScugReset)
                {
                    //self.manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = null;
                    SecretScugUnlocked = false;
                    Customeeinput = 0;
                    SecretScugReset = false;

                }
            }
            orig(self);
        }

        private static bool PlayerSlugcatStats_HideSecretScug(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (!SecretScugUnlocked && IsOvenTimerDone && !DebugSecretScugReenable)
            {
                return true;
            }
            else if (SecretScugUnlocked && IsOvenTimerDone && !DebugSecretScugReenable)
            {
                return false;
            }
            else if ((SecretScugUnlocked || !SecretScugUnlocked) && IsOvenTimerDone && DebugSecretScugReenable)
            {
                return false;
            }
            return orig(i);
        }

        private static void Player_SecretScugZeroMode_ctor(On.MoreSlugcats.SingularityBomb.orig_ctor orig, SingularityBomb self, AbstractPhysicalObject abstractPhysicalObject, World world)
        {   
            if (world.game.IsStorySession == true && IsOvenTimerDone)
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
