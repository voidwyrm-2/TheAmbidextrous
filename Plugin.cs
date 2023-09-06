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
        public static readonly int AmbiMaxFood = 14;
        public static readonly int DoughMaxFood = 14;
        //int Customeeinput = 0; //do not delete

        //important storage bools
        public static bool DoesPlayerExist;
        public static bool IsPlayerAlive;
        public static bool IsPlayerAmbidextrous;
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

            //On.Player.Die += Player_BlackholeSun;
            //On.AbstractCreature.ctor += AbstractCreature_ctor;

            On.Player.Update += Player_GiveSofanthiel;

            On.Player.Destroy += PlayerDestroyHook;
            On.Player.Die += Player_Die;
            On.Player.ActivateAscension += Player_ActivateAscension;

        }

        private void Player_ActivateAscension(On.Player.orig_ActivateAscension orig, Player self)
        {
            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                self.ActivateAscension();
            }
        }

        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            if (Input.GetKey(KeyCode.LeftControl) && SecretScugHooks.IsOvenTimerDone)
            {
                //this.hurtLevel = 0f;
                self.playerState.permanentDamageTracking = 0.0;
            }
            orig(self);
        }

        private void PlayerDestroyHook(On.Player.orig_Destroy orig, Player self)
        {
            orig.Invoke(self);
            self.Die();
        }


        //Original: UW_S02 //Test: UW_A12
        private void Player_GiveSofanthiel(On.Player.orig_Update orig, Player self, bool eu)
        {
            //var DroneBool = new PlayerNPCState(AbstractCreature abstractCreature, int playerNumber);
            if (IsPlayerAmbidextrous)
            {
                //PlayerNPCState.Drone = true;
                MoreSlugcatsEnums.GateRequirement.RoboLock = null;
                RegionGate.GateRequirement roboLock = MoreSlugcatsEnums.GateRequirement.RoboLock;
                if (roboLock != null)
                {
                    roboLock.Unregister();
                }
            }
            orig(self, eu);
        }


        /*
        public FirecrackerPlant.ScareObject scareObj;

        private void AbstractCreature_ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            if (realizedCreature.scareObj == null)
            {
                var room = realizedCreature.room;
                var pos = realizedCreature.mainBodyChunk.pos;
                var color = realizedCreature.ShortCutColor();
                realizedCreature.scareObj = new FirecrackerPlant.ScareObject(pos);
                realizedCreature.scareObj.fearRange = 8000f;
                realizedCreature.scareObj.fearScavs = true;
                realizedCreature.room.AddObject(this.scareObj);
                realizedCreature.room.InGameNoise(new InGameNoise(base.firstChunk.pos, 8000f, this, 1f));
            }
        }

        private void Player_BlackholeSun(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if (!wasDead && self.dead)
            {
                // Adapted from ScavengerBomb.Explode
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                Vector2 vector = Vector2.Lerp(pos, lastPos, 0.35f);
                self.room.AddObject(new SingularityBomb.SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
                self.room.AddObject(new Explosion(self.room, self, vector, 7, 450f, 6.2f, 10f, 280f, 0.25f, self.thrownBy, 0.3f, 160f, 1f));
                self.room.AddObject(new Explosion(self.room, self, vector, 7, 2000f, 4f, 0f, 400f, 0.25f, self.thrownBy, 0.3f, 200f, 1f));
                self.room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, self.explodeColor));
                self.room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                self.room.AddObject(new Explosion.ExplosionLight(vector, 2000f, 2f, 60, self.explodeColor));
                self.room.AddObject(new ShockWave(vector, 350f, 0.485f, 300, true));
                self.room.AddObject(new ShockWave(vector, 2000f, 0.185f, 180, false));
                for (int i = 0; i < 25; i++)
                {
                    Vector2 a = Custom.RNV();
                    if (self.room.GetTile(vector + a * 20f).Solid)
                    {
                        if (!self.room.GetTile(vector - a * 20f).Solid)
                        {
                            a *= -1f;
                        }
                        else
                        {
                            a = Custom.RNV();
                        }
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        self.room.AddObject(new Spark(vector + a * Mathf.Lerp(30f, 60f, Random.value), a * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 20f * Random.value, Color.Lerp(self.explodeColor, new Color(1f, 1f, 1f), Random.value), null, 11, 28));
                    }
                    self.room.AddObject(new Explosion.FlashingSmoke(vector + a * 40f * Random.value, a * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + 0.05f * Random.value, new Color(1f, 1f, 1f), self.explodeColor, Random.Range(3, 11)));
                }
                for (int k = 0; k < 6; k++)
                {
                    self.room.AddObject(new SingularityBomb.BombFragment(vector, Custom.DegToVec(((float)k + Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, Random.value)));
                }
                self.room.ScreenMovement(new Vector2?(vector), default(Vector2), 0.9f);
                for (int l = 0; l < self.abstractPhysicalObject.stuckObjects.Count; l++)
                {
                    self.abstractPhysicalObject.stuckObjects[l].Deactivate();
                }
                self.room.PlaySound(SoundID.Bomb_Explode, vector);
                self.room.InGameNoise(new InGameNoise(vector, 9000f, self, 1f));
                for (int m = 0; m < self.room.physicalObjects.Length; m++)
                {
                    for (int n = 0; n < self.room.physicalObjects[m].Count; n++)
                    {
                        if (self.room.physicalObjects[m][n] is Creature && Custom.Dist(self.room.physicalObjects[m][n].firstChunk.pos, base.firstChunk.pos) < 350f)
                        {
                            if (self.thrownBy != null)
                            {
                                (self.room.physicalObjects[m][n] as Creature).killTag = self.thrownBy.abstractCreature;
                            }
                            (self.room.physicalObjects[m][n] as Creature).Die();
                        }
                        if (self.room.physicalObjects[m][n] is ElectricSpear)
                        {
                            if ((self.room.physicalObjects[m][n] as ElectricSpear).abstractSpear.electricCharge == 0)
                            {
                                (self.room.physicalObjects[m][n] as ElectricSpear).Recharge();
                            }
                            else
                            {
                                (self.room.physicalObjects[m][n] as ElectricSpear).ExplosiveShortCircuit();
                            }
                        }
                    }
                }
                self.CreateFear();
                self.scareObj.lifeTime = -600;
                self.scareObj.fearRange = 12000f;
                self.room.InGameNoise(new InGameNoise(pos, 12000f, self, 1f));
            }
        }
        */

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