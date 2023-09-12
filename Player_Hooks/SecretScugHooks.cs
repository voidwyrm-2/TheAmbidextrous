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
        public static int BootlegAscendTimer = 0;
        public static bool BootlegMonkAscension = false;

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
            //On.RainWorldGame.RestartGame += RainWorldGame_RestartGame;
            On.Player.Update += Player_BootlegAscend;
            On.Player.Update += Player_ClassMechanicsSaint;

        }



        public static void Player_ClassMechanicsSaint(On.Player.orig_Update orig, Player self, bool eu)
        {
            bool flag = self.abstractCreature.world.game.IsStorySession && self.abstractCreature.world.game.StoryCharacter == SecretScug;
            if (self.SlugCatClass == SecretScug)
            {
                if (!MMF.cfgOldTongue.Value && self.input[0].jmp && !self.input[1].jmp && !self.input[0].pckp && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.HangFromBeam && self.SaintTongueCheck())
                {
                    Vector2 vector = new Vector2((float)self.flipDirection, 0.7f);
                    Vector2 normalized = vector.normalized;
                    if (self.input[0].y > 0)
                    {
                        normalized = new Vector2(0f, 1f);
                    }
                    normalized = (normalized + self.mainBodyChunk.vel.normalized * 0.2f).normalized;
                    self.tongue.Shoot(normalized);
                }
            }
            if (self.SlugCatClass == SecretScug && (self.KarmaCap >= 9 || (self.room.game.session is ArenaGameSession && self.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType == MoreSlugcatsEnums.GameTypeID.Challenge && self.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.challengeMeta.ascended)))
            {
                if (self.voidSceneTimer > 0 && flag)
                {
                    self.voidSceneTimer++;
                    if (!self.monkAscension)
                    {
                        self.ActivateAscension();
                    }
                    self.godTimer = self.maxGodTime;
                    if (self.voidSceneTimer > 60)
                    {
                        if (!self.forceBurst)
                        {
                            self.burstX = 0f;
                            self.burstY = 0f;
                        }
                        self.forceBurst = true;
                        self.killWait = Mathf.Min(self.killWait + 0.035f, 1f);
                    }
                }
                if (self.room.world.name == "HR")
                {
                    self.maxGodTime = 560f;
                }
                if (flag && self.AI == null && self.room.game.session is StoryGameSession && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.SaintEnlightMessage)
                {
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.SaintEnlightMessage = true;
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.rainWorld.inGameTranslator.Translate("While in the air, tap jump and pick-up together to take flight."), 240, 480, true, true);
                }
                if (self.wantToJump > 0 && self.monkAscension)
                {
                    self.DeactivateAscension();
                    self.wantToJump = 0;
                }
                else if (self.wantToJump > 0 && self.input[0].pckp && self.canJump <= 0 && !self.monkAscension && !self.tongue.Attached && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.Consious && !self.Stunned && self.godTimer > 0f && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                {
                    self.ActivateAscension();
                }
            }
            self.lastKillFac = self.killFac;
            self.lastKillWait = self.killWait;
            if (self.karmaCharging > 0)
            {
                self.godTimer = Mathf.Min(self.godTimer + 1f, self.maxGodTime);
                self.karmaCharging--;
            }
            if (self.monkAscension)
            {
                self.buoyancy = 0f;
                self.godDeactiveTimer = 0f;
                self.animation = Player.AnimationIndex.None;
                self.bodyMode = Player.BodyModeIndex.Default;
                if (self.tongue != null && self.tongue.Attached)
                {
                    self.tongue.Release();
                }
                if (self.godWarmup > -20f)
                {
                    self.godWarmup -= 1f;
                }
                if ((self.room == null || !self.room.game.setupValues.saintInfinitePower) && self.karmaCharging == 0 && self.godWarmup <= 0f)
                {
                    self.godTimer -= 1f;
                }
                if (self.dead || self.stun >= 20)
                {
                    self.DeactivateAscension();
                }
                if (self.godTimer <= 0f)
                {
                    self.godRecharging = true;
                    self.godTimer = -15f;
                    self.DeactivateAscension();
                }
                else
                {
                    self.godRecharging = false;
                }
                if (flag && self.AI == null && self.godTimer <= self.maxGodTime * 0.9f && self.room.game.session is StoryGameSession && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.KarmicBurstMessage)
                {
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.KarmicBurstMessage = true;
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.rainWorld.inGameTranslator.Translate("Hold throw and directional inputs while flying to perform an ascension."), 80, 240, true, true);
                }
                self.gravity = 0f;
                self.airFriction = 0.7f;
                float num = 2.75f;
                if (self.killWait >= 0.2f && !self.forceBurst)
                {
                    self.airFriction = 0.1f;
                    self.bodyChunks[0].vel = Custom.RNV() * Mathf.Lerp(0f, 20f, self.killWait);
                    num = 0f;
                }
                if (self.input[0].y > 0)
                {
                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + num;
                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + (num - 1f);
                }
                else if (self.input[0].y < 0)
                {
                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - num;
                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - (num - 1f);
                }
                if (self.input[0].x > 0)
                {
                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x + num;
                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x + (num - 1f);
                }
                else if (self.input[0].x < 0)
                {
                    self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x - num;
                    self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x - (num - 1f);
                }
                float num2 = 10f;
                float num3 = 400f;
                float num4 = 1f;
                float num5 = 2f;
                float num6 = 0.7f;
                if (!self.input[0].thrw && !self.forceBurst)
                {
                    if (self.voidSceneTimer == 0)
                    {
                        self.burstX *= num6;
                        self.burstY *= num6;
                    }
                    self.burstVelX *= num6;
                    self.burstVelY *= num6;
                    self.killPressed = false;
                    self.killFac *= 0.8f;
                    self.killWait *= 0.95f;
                    return;
                }
                if (!self.killPressed)
                {
                    if (!self.forceBurst)
                    {
                        self.killWait = Mathf.Min(self.killWait + 0.035f, 1f);
                        if (self.killWait == 1f)
                        {
                            self.killFac += 0.025f;
                        }
                    }
                    if (self.input[0].x != 0)
                    {
                        self.burstVelX = Mathf.Clamp(self.burstVelX + (float)self.input[0].x * num4, -num2, num2);
                    }
                    else if (self.burstVelX < -num5)
                    {
                        self.burstVelX += num5;
                    }
                    else if (self.burstVelX > num5)
                    {
                        self.burstVelX -= num5;
                    }
                    else
                    {
                        self.burstVelX = 0f;
                    }
                    if (self.input[0].y != 0)
                    {
                        self.burstVelY = Mathf.Clamp(self.burstVelY + (float)self.input[0].y * num4, -num2, num2);
                    }
                    else if (self.burstVelY < -num5)
                    {
                        self.burstVelY += num5;
                    }
                    else if (self.burstVelY > num5)
                    {
                        self.burstVelY -= num5;
                    }
                    else
                    {
                        self.burstVelY = 0f;
                    }
                    if (!self.forceBurst)
                    {
                        self.burstX = Mathf.Clamp(self.burstX + self.burstVelX, -num3, num3);
                        self.burstY = Mathf.Clamp(self.burstY + self.burstVelY, -num3, num3);
                    }
                    else if (flag)
                    {
                        float num7 = self.wormCutsceneTarget.x - (self.mainBodyChunk.pos.x + self.burstX);
                        float num8 = self.wormCutsceneTarget.y - (self.mainBodyChunk.pos.y + self.burstY + 60f);
                        if (Custom.DistLess(Vector2.zero, new Vector2(num7, num8), 450f))
                        {
                            float num9 = 0.02f;
                            if (self.wormCutsceneLockon)
                            {
                                num9 = 0.25f;
                            }
                            if (num7 > 0f)
                            {
                                self.burstX += Mathf.Clamp(num7 * num9, 2.5f, self.wormCutsceneLockon ? 100f : 10f);
                            }
                            else
                            {
                                self.burstX += Mathf.Clamp(num7 * num9, self.wormCutsceneLockon ? -100f : -10f, -2.5f);
                            }
                            if (num8 > 0f)
                            {
                                self.burstY += Mathf.Clamp(num8 * num9, 2.5f, self.wormCutsceneLockon ? 100f : 10f);
                            }
                            else
                            {
                                self.burstY += Mathf.Clamp(num8 * num9, self.wormCutsceneLockon ? -100f : -10f, -2.5f);
                            }
                            if (Custom.DistLess(Vector2.zero, new Vector2(num7, num8), 40f) && self.killWait == 1f)
                            {
                                self.killFac += 0.025f;
                                self.wormCutsceneLockon = true;
                            }
                        }
                    }
                }
                if (self.killFac >= 1f)
                {
                    num = 60f;
                    Vector2 vector2 = new Vector2(self.mainBodyChunk.pos.x + self.burstX, self.mainBodyChunk.pos.y + self.burstY + 60f);
                    bool flag2 = false;
                    for (int i = 0; i < self.room.physicalObjects.Length; i++)
                    {
                        for (int j = self.room.physicalObjects[i].Count - 1; j >= 0; j--)
                        {
                            if (j >= self.room.physicalObjects[i].Count)
                            {
                                j = self.room.physicalObjects[i].Count - 1;
                            }
                            PhysicalObject physicalObject = self.room.physicalObjects[i][j];
                            if (physicalObject != self)
                            {
                                foreach (BodyChunk bodyChunk in physicalObject.bodyChunks)
                                {
                                    if (Custom.DistLess(bodyChunk.pos, vector2, num + bodyChunk.rad) && self.room.VisualContact(bodyChunk.pos, vector2))
                                    {
                                        bodyChunk.vel += Custom.RNV() * 36f;
                                        if (physicalObject is Creature)
                                        {
                                            if (!(physicalObject as Creature).dead)
                                            {
                                                flag2 = true;
                                            }
                                            (physicalObject as Creature).Die();
                                        }
                                        if (physicalObject is SeedCob && !(physicalObject as SeedCob).AbstractCob.opened && !(physicalObject as SeedCob).AbstractCob.dead)
                                        {
                                            (physicalObject as SeedCob).spawnUtilityFoods();
                                        }
                                        if (self.room.game.session is StoryGameSession && physicalObject is Oracle && flag)
                                        {
                                            if ((physicalObject as Oracle).ID == MoreSlugcatsEnums.OracleID.CL && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripPebbles)
                                            {
                                                (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripPebbles = true;
                                                self.room.PlaySound(SoundID.SS_AI_Talk_1, self.mainBodyChunk, false, 1f, 0.4f);
                                                Vector2 pos = (physicalObject as Oracle).bodyChunks[0].pos;
                                                self.room.AddObject(new ShockWave(pos, 500f, 0.75f, 18, false));
                                                self.room.AddObject(new Explosion.ExplosionLight(pos, 320f, 1f, 5, Color.white));
                                                Debug.Log("Ascend saint pebbles");
                                                ((physicalObject as Oracle).oracleBehavior as CLOracleBehavior).dialogBox.Interrupt("...", 1);
                                                if (((physicalObject as Oracle).oracleBehavior as CLOracleBehavior).currentConversation != null)
                                                {
                                                    ((physicalObject as Oracle).oracleBehavior as CLOracleBehavior).currentConversation.Destroy();
                                                }
                                                (physicalObject as Oracle).health = 0f;
                                                flag2 = true;
                                            }
                                            if ((physicalObject as Oracle).ID == Oracle.OracleID.SL && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripMoon && (physicalObject as Oracle).glowers > 0 && (physicalObject as Oracle).mySwarmers.Count > 0)
                                            {
                                                for (int l = 0; l < (physicalObject as Oracle).mySwarmers.Count; l++)
                                                {
                                                    (physicalObject as Oracle).mySwarmers[l].ExplodeSwarmer();
                                                }
                                                (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripMoon = true;
                                                Debug.Log("Ascend saint moon");
                                                ((physicalObject as Oracle).oracleBehavior as SLOracleBehaviorHasMark).dialogBox.Interrupt("...", 1);
                                                if (((physicalObject as Oracle).oracleBehavior as SLOracleBehaviorHasMark).currentConversation != null)
                                                {
                                                    ((physicalObject as Oracle).oracleBehavior as SLOracleBehaviorHasMark).currentConversation.Destroy();
                                                }
                                                Vector2 pos2 = (physicalObject as Oracle).bodyChunks[0].pos;
                                                self.room.AddObject(new ShockWave(pos2, 500f, 0.75f, 18, false));
                                                self.room.AddObject(new Explosion.ExplosionLight(pos2, 320f, 1f, 5, Color.white));
                                                flag2 = true;
                                            }
                                        }
                                        if (physicalObject is Oracle && (physicalObject as Oracle).ID == MoreSlugcatsEnums.OracleID.ST && (physicalObject as Oracle).Consious)
                                        {
                                            Vector2 pos3 = (physicalObject as Oracle).bodyChunks[0].pos;
                                            self.room.AddObject(new ShockWave(pos3, 500f, 0.75f, 18, false));
                                            ((physicalObject as Oracle).oracleBehavior as STOracleBehavior).AdvancePhase();
                                            self.bodyChunks[0].vel = Vector2.zero;
                                            flag2 = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    for (int m = 0; m < self.room.updateList.Count; m++)
                    {
                        if (self.room.updateList[m] is Love)
                        {
                            Love love = self.room.updateList[m] as Love;
                            if (love.animator != null && love.timeUntilReboot == 0 && Custom.DistLess(love.pos, vector2, 100f))
                            {
                                love.InitiateReboot();
                                flag2 = true;
                            }
                        }
                    }
                    if (flag2 || self.voidSceneTimer > 0)
                    {
                        self.room.PlaySound(SoundID.Firecracker_Bang, self.mainBodyChunk, false, 1f, 0.75f + UnityEngine.Random.value);
                        self.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, self.mainBodyChunk, false, 1f, 0.5f + UnityEngine.Random.value * 0.5f);
                    }
                    else
                    {
                        self.room.PlaySound(SoundID.Snail_Pop, self.mainBodyChunk, false, 1f, 1.5f + UnityEngine.Random.value);
                    }
                    for (int n = 0; n < 20; n++)
                    {
                        self.room.AddObject(new Spark(vector2, Custom.RNV() * UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                    }
                    self.killFac = 0f;
                    self.killWait = 0f;
                    self.killPressed = true;
                    if (self.voidSceneTimer > 0)
                    {
                        self.voidSceneTimer = 0;
                        self.DeactivateAscension();
                        self.controller = null;
                        self.forceBurst = false;
                        return;
                    }
                }
            }
            else
            {
                if (self.godWarmup < 60f && self.godDeactiveTimer > 200f)
                {
                    self.godWarmup += 1f;
                }
                self.godDeactiveTimer += 1f;
                self.killPressed = false;
                self.killFac *= 0.8f;
                self.killWait *= 0.5f;
                float num10 = 0.15f * (self.maxGodTime / 400f);
                if (self.godRecharging)
                {
                    num10 = 0.15f * (self.maxGodTime / 400f);
                }
                self.godTimer = Mathf.Min(self.godTimer + num10, self.maxGodTime);
            }
            orig(self, eu);
        }



        //public static Color GetColor(PlayerGraphics graphics) {return }
        private static void Player_BootlegAscend(On.Player.orig_Update orig, Player self, bool eu)
        {
            if(IsOvenTimerDone)
            {
                if(Input.GetKeyDown(KeyCode.LeftControl))
                {

                }
            }
            orig(self, eu);
        }

        /*
        private static void RainWorldGame_RestartGame(On.RainWorldGame.orig_RestartGame orig, RainWorldGame self)
        {
            //if(self.)
            orig(self);
        }
        */

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
            
            orig(self, abstractPhysicalObject, world);
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
