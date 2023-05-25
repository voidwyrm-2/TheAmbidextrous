﻿using System;
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

namespace NuclearPasta.TheAmbidextrous
{
    [BepInPlugin(MOD_ID, "The Ambidextrous", "1.0.3")]
    class TheAmbidextrousMod : BaseUnityPlugin
    {
        private const string MOD_ID = "dv.theambidextrous";

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("ambidexterity/super_jump");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("ambidexterity/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("ambidexterity/mean_lizards");
        public static readonly PlayerFeature<bool> DualWielding = PlayerBool("ambidexterity/dual_wield");
        //public static readonly PlayerFeature<bool> ObjectSwallowEffects = PlayerBool("ambidexterity/swallow_object");
        public static readonly PlayerFeature<bool> DualEnergyCell = PlayerBool("ambidexterity/dual_energycell");
        //public static readonly PlayerFeature<bool> DoubleJump = PlayerBool("ambidexterity/double_jump");
        //public static readonly PlayerFeature<bool> Rebirth = PlayerBool("ambidexterity/rebirth");
        public static readonly PlayerFeature<bool> WallClimbing = PlayerBool("ambidexterity/wallclimb");
        public PlayerFeature<bool> playerFeature;
        public int climbDuration = 0;



        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.Jump += Player_Jump;
            On.Player.Die += Player_Die;
            On.Lizard.ctor += Lizard_ctor;
            On.Player.Grabability += DoubleEnergyCell;
            //On.Player.SwallowObject += new On.Player.hook_SwallowObject(Player_SwallowObject);
            On.Player.Grabability += DoubleSpear;
            //On.Player.Jump += Player_Double_Jump;
            //On.Player.Die += Phoenix;
            On.Player.Update += new On.Player.hook_Update(this.OnWall);
        }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        private void OnWall(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.slugcatStats.name.value == "The Ambidextrous" && WallClimbing.TryGet(self, out bool wallclimbbool) && wallclimbbool)
            {
                orig.Invoke(self, eu);
                bool flag2;
                bool flag = this.playerFeature.TryGet(self, out flag2) && flag2;
                bool flag3 = flag;
                if (flag3)
                {
                    bool flag4 = self.bodyMode == Player.BodyModeIndex.WallClimb;
                    if (flag4)
                    {
                        self.customPlayerGravity = 0.01f;
                        self.mainBodyChunk.lastPos = self.mainBodyChunk.pos;
                        self.mainBodyChunk.vel.y = 0f;
                        self.animation = Player.AnimationIndex.DownOnFours;
                        bool flag5 = this.climbDuration < 20;
                        if (flag5)
                        {
                            bool flag6 = self.input[0].y > 0;
                            if (flag6)
                            {
                                this.climbDuration++;
                                self.mainBodyChunk.vel.y = Mathf.Lerp((float)(10 * self.input[0].y), 0f, 0.075f * (float)this.climbDuration);
                            }
                            else
                            {
                                this.climbDuration = Mathf.Clamp(this.climbDuration - 1, 0, 10);
                            }
                        }
                        else
                        {
                            this.climbDuration = 0;
                            self.mainBodyChunk.vel.y = 0f;
                        }
                    }
                    else
                    {
                        self.customPlayerGravity = self.room.gravity;
                    }
                }
            }
        }

        //private void Phoenix(On.Player.orig_Die orig, Player self)
        //{
        //bool wasDead = self.dead;
        //if (!wasDead && self.dead && Rebirth.TryGet(self, out bool playerisdead) && playerisdead)
        //{ 

        //On.Player.

        //}
        //}

        //private void Player_Double_Jump(On.Player.orig_Jump orig, Player self)
        //{
        //orig(self);

        //if (DoubleJump.TryGet(self, out var power) && self.lowerBodyFramesOffGround > 0 && self.upperBodyFramesOffGround > 0 && self.input[0].jmp == true)
        //{

        //if ()
        //{

        //}
        //}
        //}

        private Player.ObjectGrabability DoubleEnergyCell(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            bool flag = obj is Weapon;
            if (flag)
            {
                bool flag2 = DualWielding.TryGet(self, out bool dualwieldbool) && dualwieldbool;
                if (flag2)
                {
                    return (Player.ObjectGrabability)1;
                }
            }
            return orig.Invoke(self, obj);

            //if this is not my scug, then default behavior.
            //if it is my scug and it is the object type of EnergyCell then change the grabability to only using one hand
            //bool flag = DualEnergyCell.TryGet(self, out bool dualenergycellbool) && dualenergycellbool;
            //if (self.slugcatStats.name.value == "The Ambidextrous" && obj is EnergyCell && flag == true)
            //{
            //    return (Player.ObjectGrabability)1; 
            //}
            //return orig(self, obj);
        }

        //credit to Deathpits for being awesome and helping an idiot like me,
        //Vigaro for a simplified version,
        //and Slime_Cubed for making me both more and less confused than I already was
        //start of the conversion: https://discord.com/channels/291184728944410624/305139167300550666/1110215808354889748
        //conversion with Slime_Cubed: https://discord.com/channels/291184728944410624/431534164932689921/1110263399885045810
        //(for those who need to reference something from there)
        private Player.ObjectGrabability DoubleSpear(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {

            bool flag = obj is Weapon;
            if (flag)
            {
                bool flag2 = DualWielding.TryGet(self, out bool dualwieldbool) && dualwieldbool;
                if (flag2)
                {
                    return (Player.ObjectGrabability)1;
                }
            }
            return orig.Invoke(self, obj);

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

        // Implement ExlodeOnDeath
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if(!wasDead && self.dead
                && ExplodeOnDeath.TryGet(self, out bool explode)
                && explode)
            {
                // Adapted from ScavengerBomb.Explode
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));

                room.ScreenMovement(pos, default, 1.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
            }
        }
    }
}