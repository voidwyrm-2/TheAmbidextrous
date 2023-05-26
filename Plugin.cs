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
        //public static readonly PlayerFeature<bool> WallClimbing = PlayerBool("ambidexterity/wallclimb");
        //public static readonly PlayerFeature<bool> BackSpear = PlayerBool("ambidexterity/BackSpear");



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
            //On.Player.Update += new On.Player.hook_Update(this.OnWall);
            //On.Player.ctor += Player_BackSpear_ctor;
        }
        
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        //private static void Player_BackSpear_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        //{
        //    orig(self, abstractCreature, world);
        //
         //   if (BackSpear.TryGet(self, out var hasBackSpear) && hasBackSpear)
        //        self.spearOnBack ??= new Player.SpearOnBack(self);
        //}

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

            bool flag = obj is EnergyCell;
            if (flag)
            {
                bool flag2 = DualEnergyCell.TryGet(self, out bool dualenergycellbool) && dualenergycellbool;
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

            bool flag = obj is Spear;
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

        public class FoodOnBack
        {
            public FoodOnBack(Player owner)
            {
                if (owner.slugcatStats.name.value == "The Ambidextrous")
                {
                    this.owner = owner;
                    this.inFrontOfObjects = -1;
                }
            }

            public bool HasAFood
            {
                get
                {
                    return this.spear != null;
                }
            }

            public void Update() //bool eu
            {
                if (this.spear == null && this.counter > 20)
                {
                    //ON SECOND THOUGHT, WE SHOULD ONLY SWAP FOOD WITH OUR OFF HAND
                    //for (int i = 0; i < 2; i++)
                    int i = 1;

                    //SMALL TWEAKS. ALLOW BACK FOOD STORAGE IF OUR OTHER HAND IS EMPTY
                    if (this.owner.grasps[0] != null && this.owner.grasps[1] == null)
                        i = 0;

                    if (this.owner.grasps[i] != null && this.owner.grasps[i].grabbed is PhysicalObject && this.owner.grasps[i].grabbed is IPlayerEdible && this.owner.Grabability(this.owner.grasps[i].grabbed as PhysicalObject) == Player.ObjectGrabability.OneHand)
                    {
                        //WHY DON'T WE DO MOST OF THIS DOWN THERE?
                        //this.owner.bodyChunks[0].pos += Custom.DirVec(this.owner.grasps[i].grabbed.firstChunk.pos, this.owner.bodyChunks[0].pos) * 2f;
                        this.FoodToBack(this.owner.grasps[i].grabbed as PhysicalObject);
                        //this.counter = 0;
                        //break;
                    }
                }
            }


            public void GraphicsModuleUpdated(bool actuallyViewed, bool eu)
            {
                if (this.spear == null)
                    return;

                if (this.spear.slatedForDeletetion || this.spear.grabbedBy.Count > 0)
                {
                    if (this.abstractStick != null)
                        this.abstractStick.Deactivate();
                    this.spear = null;
                    return;
                }
                Vector2 vector = this.owner.mainBodyChunk.pos;
                Vector2 vector2 = this.owner.bodyChunks[1].pos;
                if (this.owner.graphicsModule != null)
                {
                    vector = Vector2.Lerp((this.owner.graphicsModule as PlayerGraphics).drawPositions[0, 0], (this.owner.graphicsModule as PlayerGraphics).head.pos, 0.2f);
                    vector2 = (this.owner.graphicsModule as PlayerGraphics).drawPositions[1, 0];
                }
                Vector2 vector3 = Custom.DirVec(vector2, vector);
                if (this.owner.Consious && this.owner.bodyMode != Player.BodyModeIndex.ZeroG && this.owner.EffectiveRoomGravity > 0f)
                {
                    //NO FLIP
                    //OKAY MAYBE SOME FLIP...
                    if (this.owner.bodyMode == Player.BodyModeIndex.Default && this.owner.animation == Player.AnimationIndex.None && this.owner.standing && this.owner.bodyChunks[1].pos.y < this.owner.bodyChunks[0].pos.y - 6f)
                        this.flip = Custom.LerpAndTick(this.flip, (float)this.owner.input[0].x * 0.3f, 0.05f, 0.02f);
                    else if (this.owner.bodyMode == Player.BodyModeIndex.Stand && this.owner.input[0].x != 0)
                        this.flip = Custom.LerpAndTick(this.flip, (float)this.owner.input[0].x, 0.02f, 0.1f);
                    else
                        this.flip = Custom.LerpAndTick(this.flip, (float)this.owner.flipDirection * Mathf.Abs(vector3.x), 0.15f, 0.16666667f);
                    //OVERLAP??? --I DON'T THINK NORMAL OBJECTS HAVE THIS
                    //this.spear.ChangeOverlap(vector3.y < -0.1f && this.owner.bodyMode != Player.BodyModeIndex.ClimbingOnBeam);
                    //THEY DO NOW :)
                    //this.ChangeOverlap(vector3.y < -0.1f && this.owner.bodyMode != Player.BodyModeIndex.ClimbingOnBeam);
                    //Debug.Log("----FOOD OVERLAP!: ");
                    //this.ChangeOverlap(true);
                    this.ChangeOverlap(false); //OKAY... SEEMS LIKE THIS SHOULD JUST BE FALSE ALL THE TIME
                }
                else
                {
                    this.flip = Custom.LerpAndTick(this.flip, 0f, 0.15f, 0.14285715f);
                    // this.spear.setRotation = new Vector2?(vector3 - Custom.PerpendicularVector(vector3) * 0.9f);
                    //this.spear.ChangeOverlap(false);
                    this.ChangeOverlap(false);

                }
                this.spear.firstChunk.MoveFromOutsideMyUpdate(eu, Vector2.Lerp(vector2, vector, 0.6f) - Custom.PerpendicularVector(vector2, vector) * 7.5f * this.flip);
                this.spear.firstChunk.vel = this.owner.mainBodyChunk.vel;
                //this.spear.rotationSpeed = 0f;
            }


            public void FoodToHand(bool eu)
            {
                if (this.spear == null)
                    return;
                // for (int i = 0; i < 2; i++)
                // {
                // if (this.owner.grasps[i] != null && this.owner.Grabability(this.owner.grasps[i].grabbed) >= Player.ObjectGrabability.BigOneHand)
                // return;
                // }

                if (this.owner.grasps[1] != null)
                    return;

                int num = -1;
                int num2 = 0;
                while (num2 < 2 && num == -1)
                {
                    if (this.owner.grasps[num2] == null)
                        num = num2;
                    num2++;
                }
                if (num == -1)
                    return;
                if (this.owner.graphicsModule != null)
                    this.spear.firstChunk.MoveFromOutsideMyUpdate(eu, (this.owner.graphicsModule as PlayerGraphics).hands[num].pos);
                //RETURN OUR ORIGINAL COLLISION
                //this.spear.collisionLayer = this.origCollisionLayer;
                this.spear.ChangeCollisionLayer(this.origCollisionLayer);
                this.spear.bodyChunks[0].collideWithTerrain = true;
                this.ChangeOverlap(true);
                this.owner.SlugcatGrab(this.spear, num);
                this.spear = null;
                this.interactionLocked = true;
                this.owner.noPickUpOnRelease = 20;
                this.owner.room.PlaySound(SoundID.Slugcat_Pick_Up_Spear, this.owner.mainBodyChunk);
                this.owner.room.PlaySound(SoundID.Scavenger_Knuckle_Hit_Ground, this.owner.mainBodyChunk);
                if (this.abstractStick != null)
                {
                    this.abstractStick.Deactivate();
                    this.abstractStick = null;
                }
            }


            //MADE IT A BOOLEAN SO WE CAN TELL IF IT RAN TO COMPLETION OR WAS CANCELED
            public bool FoodToBack(PhysicalObject spr)
            {
                if (this.spear != null)
                    return false;

                //IF IT'S A BATFLY, KILL IT. IF IT'S A DIFFERENT LIVING CREATURE, DON'T STOW IT
                if (spr is Creature && (spr as Creature).dead == false)
                {
                    if (spr is Fly)
                        (spr as Fly).Die();
                    else
                        return false;
                }

                if (spr is Mushroom)
                    (spr as Mushroom).growPos = null;
                if (spr is KarmaFlower)
                    (spr as KarmaFlower).growPos = null;
                if (spr is SlimeMold)
                    (spr as SlimeMold).stuckPos = null;

                //THIS USED TO HAPPEN IN UPDATE() BUT I MOVED IT DOWN HERE
                this.owner.bodyChunks[0].pos += Custom.DirVec(spr.firstChunk.pos, this.owner.bodyChunks[0].pos) * 2f;
                this.counter = 0;

                for (int i = 0; i < 2; i++)
                {
                    if (this.owner.grasps[i] != null && this.owner.grasps[i].grabbed == spr)
                    {
                        this.owner.ReleaseGrasp(i);
                        break;
                    }
                }
                this.spear = spr;
                // this.spear.ChangeMode(Weapon.Mode.OnBack); --A FRUIT IS NOT A WEAPON
                //MAKE THE FOOD NOT COLLIDE WITH US
                this.origCollisionLayer = this.spear.collisionLayer;
                //this.spear.collisionLayer = 2; //LIKE KARMA FLOWERS
                this.spear.ChangeCollisionLayer(0); //0 - OKAY BUT THIS LAYER STILL COLLIDES WITH TERRAIN AND THATS WEIRD
                this.spear.bodyChunks[0].collideWithTerrain = false;
                this.ChangeOverlap(false);
                this.interactionLocked = true;
                this.owner.noPickUpOnRelease = 20;
                this.owner.room.PlaySound(SoundID.Slugcat_Stash_Spear_On_Back, this.owner.mainBodyChunk);
                this.owner.room.PlaySound(SoundID.Scavenger_Knuckle_Hit_Ground, this.owner.mainBodyChunk);
                if (this.spear is PlayerCarryableItem) //FLASHY
                    (this.spear as PlayerCarryableItem).Blink();
                if (this.abstractStick != null)
                    this.abstractStick.Deactivate();
                this.abstractStick = new Player.AbstractOnBackStick(this.owner.abstractPhysicalObject, spr.abstractPhysicalObject);


                return true; //EVERYTHINGS A-OKAY HERE, BOSS!
                             //this.owner.graphicsModule.BringSpritesToFront();
            }


            public void DropFood()
            {
                if (this.spear == null)
                    return;

                this.spear.firstChunk.vel = this.owner.mainBodyChunk.vel + Custom.RNV() * 3f * UnityEngine.Random.value;
                // this.spear.ChangeMode(Weapon.Mode.Free);
                //this.spear.collisionLayer = this.origCollisionLayer;
                this.spear.ChangeCollisionLayer(this.origCollisionLayer);
                this.spear.bodyChunks[0].collideWithTerrain = true;
                this.spear = null;
                if (this.abstractStick != null)
                {
                    this.abstractStick.Deactivate();
                    this.abstractStick = null;
                }
            }

            //BARROWING THIS FROM WEAPON.CS
            public virtual void NewRoom(Room newRoom)
            {
                //SHRUG
                this.inFrontOfObjects = -1;

            }

            public void ChangeOverlap(bool newOverlap)
            {
                /*
                if (this.inFrontOfObjects == ((!newOverlap) ? 0 : 1) || this.owner.room == null)
                {
                    return;
                }
                for (int i = 0; i < this.owner.room.game.cameras.Length; i++)
                {
                    this.owner.room.game.cameras[i].MoveObjectToContainer(this as IDrawable, this.owner.room.game.cameras[i].ReturnFContainer((!newOverlap) ? "Background" : "Items"));
                }
                this.inFrontOfObjects = ((!newOverlap) ? 0 : 1);
                */
                //MAYBE WE JUST GO AS SIMPLE AS POSSIBLE?
                for (int i = 0; i < this.owner.room.game.cameras.Length; i++)
                {
                    //DIFFERENT OBJECT TYPES NEED TO PASS IN DIFFERENT THINGS TO SWAP LAYERS CORRECTLY
                    IDrawable objTarget; // = this.spear;
                    if (this.spear is IDrawable)
                        objTarget = this.spear as IDrawable;
                    else
                        objTarget = this.spear.graphicsModule;
                    this.owner.room.game.cameras[i].MoveObjectToContainer(objTarget, this.owner.room.game.cameras[i].ReturnFContainer((!newOverlap) ? "Background" : "Items"));
                }
                //IF IT'S LIKE A CREATURE OR SOMETHING, RESET IT'S GRAPHICS MODULE
                //if (newOverlap == false && this.spear.graphicsModule != null)
                //	this.spear.graphicsModule.Reset();
            }

            public Player owner;
            // public Spear spear;
            public PhysicalObject spear;
            public bool increment;
            public int counter;
            public float flip;
            public bool interactionLocked;
            public Player.AbstractOnBackStick abstractStick;
            //A NEW ONE
            public int origCollisionLayer;
            public int inFrontOfObjects;
        }
    }
}