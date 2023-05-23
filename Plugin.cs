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

namespace NuclearPasta.TheAmbidextrous
{
    [BepInPlugin(MOD_ID, "The Ambidextrous", "1.0.0")]
    class TheAmbidextrousMod : BaseUnityPlugin
    {
        private const string MOD_ID = "dv.theambidextrous";

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("ambidexterity/super_jump");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("ambidexterity/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("ambidexterity/mean_lizards");
        public static readonly PlayerFeature<bool> DualWielding = PlayerBool("ambidexterity/dual_wield");
        //public static readonly PlayerFeature<bool> ObjectSwallowEffects = PlayerBool("ambidexterity/swallow_object");
        public static readonly PlayerFeature<bool> DualEnergyCell = PlayerBool("ambidexterity/dual_energycell");



        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.Jump += Player_Jump;
            On.Player.Die += Player_Die;
            On.Lizard.ctor += Lizard_ctor;
            On.Player.Grabability += new On.Player.hook_Grabability(DoubleSpear);
            On.Player.SwallowObject += new On.Player.hook_SwallowObject(Player_SwallowObject);
            On.Player.Grabability += new On.Player.hook_Grabability(DoubleEnergyCell);
        }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        private Player.ObjectGrabability DoubleEnergyCell(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            //if this is not my scug, then default behavior.
            //if it is my scug and it is the object type of EnergyCell then change the grabability to only using one hand
            if (self.slugcatStats.name.value == "The Ambidextrous" && obj is EnergyCell)
            {
                return Player.ObjectGrabability.OneHand;
            }
            return orig(self, obj);
        }

            private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            Room room = self.room;
            Vector2 pos = self.mainBodyChunk.pos;
            Color color = self.ShortCutColor();
            AbstractPhysicalObject abstractPhysicalObject = self.grasps[grasp].grabbed.abstractPhysicalObject;
            orig.Invoke(self, grasp);
            bool flag = abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb && self.slugcatStats.name.value == "The Ambidextrous";
            if (flag)
            {
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                room.ScreenMovement(new Vector2?(pos), default(Vector2), 1.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new InGameNoise(pos, 9000f, self, 1f));
                self.objectInStomach = null;
                self.Die();
                Debug.Log("Tinkerer Triggered Bomb And Died!");
            }
            bool flag2 = abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && self.slugcatStats.name.value == "The Ambidextrous";
            if (flag2)
            {
                room.AddObject(new Explosion(room, self, pos, 7, 380f, 6.2f, 0.2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 240f, 0.045f, 30, false));
                room.ScreenMovement(new Vector2?(pos), default(Vector2), 1.3f);
                room.PlaySound(SoundID.Firecracker_Bang, pos);
                room.InGameNoise(new InGameNoise(pos, 9000f, self, 1f));
                self.objectInStomach = null;
                self.Stun(250);
                Debug.Log("Tinkerer Triggered: FirecrackerPlant");
            }
            bool flag3 = abstractPhysicalObject.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb && self.slugcatStats.name.value == "The Ambidextrous";
            if (flag3)
            {
                room.AddObject(new SingularityBomb.SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 20f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 0f, 400f, 0.25f, self, 0.3f, 200f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
                room.AddObject(new ShockWave(pos, 350f, 0.485f, 300, true));
                room.AddObject(new ShockWave(pos, 2000f, 0.185f, 180, false));
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new InGameNoise(pos, 9000f, self, 1f));
                self.objectInStomach = null;
                self.Die();
                Debug.Log("Tinkerer Triggered SingularityBomb And Died!");
            }
        }

        //credit to ⇐ Deathpits (Ping Me!) for being awesome and helping an idiot like me
        // and Vigaro for a simplified version
        private Player.ObjectGrabability DoubleSpear(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self.slugcatStats.name.value == "The Ambidextrous" && obj is Weapon)//if this is not my scug, then default behavior. if it is my scug and it is the object type of Weapon then change the grabability to only using one hand
            {
                return Player.ObjectGrabability.OneHand;
            }
            return orig(self, obj);
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