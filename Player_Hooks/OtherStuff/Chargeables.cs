﻿using MoreSlugcats;
using UnityEngine;

namespace NuclearPasta.TheAmbidextrous.Player_Hooks.OtherStuff
{
    public class ChargeablesState
    {
        /*
        AmbidexState state;
        public ChargeablesState(AmbidexState state)
        {
            this.state = state;
        }

        Creature.Grasp[] grasps = new Creature.Grasp[2];
        int chargeHeldItem = -1;
        PhysicalObject chargeTarget;
        int chargeGrasp;
        public int tryInteractHold = 0;
        public void GrabHook(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            if (grasps[0] != self.grasps[0] || grasps[1] != self.grasps[1])
                tryInteractHold = -1;
            grasps[0] = self.grasps[0];
            grasps[1] = self.grasps[1];
            if (tryInteractHold == 0 && self.input[0].pckp && !self.input[1].pckp)
                tryInteractHold = 10;
            if (tryInteractHold >= 0 && !self.input[0].pckp || state.zipCooldown > 0)
                tryInteractHold = 0;

            if (tryInteractHold > 0)
            {
                tryInteractHold--;
                if (tryInteractHold == 0 && state.zipCooldown == 0 && chargeHeldItem < 0)
                {
                    for(int i = 0; i < self.grasps.Length; i++)
                    {
                        if (self.grasps[i] != null && (self.grasps[i].grabbed is ElectricSpear || self.grasps[i].grabbed is Rock))
                        {
                            chargeTarget = self.grasps[i].grabbed;
                            if (i == 0 && chargeTarget is ElectricSpear es && es.abstractSpear.electricCharge > 0 && self.grasps[1] != null && self.grasps[1].grabbed is Rock)
                            {
                                chargeTarget = self.grasps[1].grabbed;
                                chargeGrasp = 1;
                            }
                            self.eatExternalFoodSourceCounter = 4;
                            self.handOnExternalFoodSource = chargeTarget.bodyChunks[0].pos;
                            chargeHeldItem = 4;
                            chargeGrasp = i;
                            break;
                        }
                    }
                }
            }
            else if (tryInteractHold < 0)
            {
                tryInteractHold++;
            }
        }
        public const int foodvalue = 6;
        public const int rubbishChargeValue = 6;
        public const int spearChargeValue = 12;

        public static bool HasEnoughCharge(AmbidexState state, int chargevalue)
        {
            if (state.player.room.game.IsArenaSession)
                return state.zipChargesReady >= chargevalue;
            chargevalue -= state.zipChargesReady;
            chargevalue -= state.zipChargesStored;
            if (state.player != null)
                chargevalue -= state.player.FoodInStomach * foodvalue;
            if(chargevalue > 0)
                state.player.room.game.cameras[0].hud.foodMeter.refuseCounter = 50;
            return chargevalue <= 0;
        }
        //assumes hasenoughfood
        public static void SpendCharge(AmbidexState state, int chargevalue)
        {
            if (state.player.room.game.IsArenaSession)
            {
                state.zipChargesReady -= chargevalue;
                return;
            }
            int diff = Mathf.Min(chargevalue, state.zipChargesReady);
            chargevalue -= diff;
            state.zipChargesReady -= diff;
            int max_iter = 2;
            int iter = 0;
            while (chargevalue > state.zipChargesStored && iter < max_iter)
            {
                iter++;
                chargevalue -= foodvalue;
                if(state.player != null)
                    state.player.SubtractFood(1);
                if (chargevalue < 0)
                {
                    state.zipChargesStored -= chargevalue;
                    chargevalue = 0;
                }
            }
            state.zipChargesStored -= chargevalue;
        }
        public void Update()
        {
            chargeHeldItem--;
            if (chargeHeldItem == 0 && chargeTarget != null)
            {
                int cost = chargeTarget is ElectricSpear ? spearChargeValue : rubbishChargeValue;
                if (state.player.room.game.IsArenaSession)
                    cost = 1;
                if(chargeTarget is ElectricSpear && ChargeOf(chargeTarget) > 0)
                {
                    if (state.rechargeZipStorage(2) > 0)
                        if (Random.value < 0.20)
                        {
                            state.rechargeZipStorage(12);
                            (chargeTarget as ElectricSpear).ExplosiveShortCircuit();
                        }
                    else
                        state.DoFailureEffect();
                }
                else if (ChargeOf(chargeTarget) == 0)
                {
                    if (HasEnoughCharge(state, cost))
                    {
                        SpendCharge(state, cost);
                        SetCharge(chargeTarget, 1);

                        if (state.player == null)
                            return;
                        state.player.room.AddObject(new ZapCoil.ZapFlash(chargeTarget.firstChunk.pos, 0.5f));
                        state.player.room.PlaySound(SoundID.Zapper_Zap, chargeTarget.firstChunk.pos, .3f, 1.5f + UnityEngine.Random.value * 1.5f);
                        state.player.room.PlaySound(SoundID.Rock_Hit_Creature, chargeTarget.firstChunk, false, 0.6f, 1);
                        if (chargeTarget.Submersion > 0.5f)
                            chargeTarget.room.AddObject(new UnderwaterShock(chargeTarget.room, null, chargeTarget.firstChunk.pos, 10, 800f, 2f, state.player, new Color(0.8f, 0.8f, 1f)));

                    }
                    else
                        state.DoFailureEffect();
                }
                //the eatExternalFoodSourceCounter animation ends with a food increase. counteract this.
                state.player.eatExternalFoodSourceCounter -= 1;
            }
        }

        int ChargeOf(PhysicalObject w)
        {
            if (w is ElectricSpear es)
                return es.abstractSpear.electricCharge;
            if (w is Rock)
                return 0;
            return 0;
        }
        void SetCharge(PhysicalObject w, int charge)
        {
            if (w is ElectricSpear es)
                es.abstractSpear.electricCharge = charge == 1 ? 3 : 0;
            else if (w is Rock && charge == 1)
            {
                var abst = w.abstractPhysicalObject;
                state.player.ReleaseGrasp(chargeGrasp);
                w.RemoveFromRoom();
                state.player.room.abstractRoom.RemoveEntity(abst);
            }
        }
        */
    }
}
