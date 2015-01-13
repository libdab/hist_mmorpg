using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using CorrugatedIron;
using CorrugatedIron.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Partial class for Form1, containing functionality specific to battle
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Calculates battle values of both armies participating in a battle or siege
        /// </summary>
        /// <returns>uint[] containing battle values of attacking & defending armies</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        public uint[] calculateBattleValue(Army attacker, Army defender, int keepLvl = 0)
        {
            uint[] battleValues = new uint[2];
            double attackerLV = 0;
            double defenderLV = 0;

            // get leaders
            Character attackerLeader = attacker.getLeader();
            Character defenderLeader = defender.getLeader();

            // get leadership values for each army leader
            attackerLV = attackerLeader.getLeadershipValue();

            // defender may not have leader
            if (defenderLeader != null)
            {
                defenderLV = defenderLeader.getLeadershipValue();
            }
            else
            {
                defenderLV = 4;
            }

            // calculate battle modifier based on LVs
            // determine highest/lowest of 2 LVs
            double maxLV = Math.Max(attackerLV, defenderLV);
            double minLV = Math.Min(attackerLV, defenderLV);
            double battleModifier = maxLV / minLV;

            // get base combat value for each army
            uint attackerCV = Convert.ToUInt32(attacker.calculateCombatValue());
            uint defenderCV = Convert.ToUInt32(defender.calculateCombatValue(keepLvl));

            // apply battle modifer to the army CV corresponding to the highest LV
            if (attackerLV == maxLV)
            {
                attackerCV = Convert.ToUInt32(attackerCV * battleModifier);
            }
            else
            {
                defenderCV = Convert.ToUInt32(defenderCV * battleModifier);
            }

            battleValues[0] = attackerCV;
            battleValues[1] = defenderCV;

            return battleValues;
        }

        /// <summary>
        /// Calculates whether the attacking army is able to successfully bring
        /// the defending army to battle
        /// </summary>
        /// <returns>bool indicating whether battle has commenced</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        public bool bringToBattle(uint attackerValue, uint defenderValue, string circumstance = "battle")
        {
            bool battleHasCommenced = false;
            double[] combatOdds = Globals_Server.battleProbabilities["odds"];
            double[] battleChances = Globals_Server.battleProbabilities[circumstance];
            double thisChance = 0;

            for (int i = 0; i < combatOdds.Length; i++)
            {
                if (i < combatOdds.Length - 1)
                {
                    if (attackerValue / defenderValue < combatOdds[i])
                    {
                        thisChance = battleChances[i];
                        break;
                    }
                }
                else
                {
                    thisChance = battleChances[i];
                    break;
                }
            }

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to battleChance
            if (randomPercentage <= thisChance)
            {
                battleHasCommenced = true;
            }

            if (battleHasCommenced)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The attacker has successfully brought the defender to battle");
                }
            }

            return battleHasCommenced;
        }

        /// <summary>
        /// Determines whether the attacking army is victorious in a battle
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public bool decideBattleVictory(uint attackerValue, uint defenderValue)
        {
            bool attackerVictorious = false;

            // calculate chance of victory
            double attackerVictoryChance = this.calcVictoryChance(attackerValue, defenderValue);

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to attackerVictoryChance
            if (randomPercentage <= attackerVictoryChance)
            {
                attackerVictorious = true;
            }

            return attackerVictorious;
        }

        /// <summary>
        /// Calculates chance that the attacking army will be victorious in a battle
        /// </summary>
        /// <returns>double containing percentage chance of victory</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public double calcVictoryChance(uint attackerValue, uint defenderValue)
        {
            return (attackerValue / (Convert.ToDouble(attackerValue + defenderValue))) * 100;
        }

        /// <summary>
        /// Calculates casualties from a battle for both sides
        /// </summary>
        /// <returns>double[] containing percentage loss modifier for each side</returns>
        /// <param name="attackerTroops">uint containing attacking army troop numbers</param>
        /// <param name="defenderTroops">uint containing defending army troop numbers</param>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="attackerVictorious">bool indicating whether attacking army was victorious</param>
        public double[] calculateBattleCasualties(uint attackerTroops, uint defenderTroops, uint attackerValue, uint defenderValue, bool attackerVictorious)
        {
            double[] battleCasualties = new double[2];
            double largeArmyModifier = 0;
            bool largestWon = true;

            // determine highest/lowest battle value
            double maxBV = Math.Max(attackerValue, defenderValue);
            double minBV = Math.Min(attackerValue, defenderValue);

            // use BVs to determine high mark for base casualty rate of army with smallest battle value (see below)
            double highCasualtyRate = (maxBV / (maxBV + minBV)) * 100;

            // determine base casualty rate for army with smallest battle value
            double smallestModifier = Globals_Game.myRand.Next(10, Convert.ToInt32(highCasualtyRate)) / (double)100;

            // determine if army with largest battle value won
            if (maxBV == attackerValue)
            {
                if (!attackerVictorious)
                {
                    largestWon = false;
                }
            }
            else
            {
                if (attackerVictorious)
                {
                    largestWon = false;
                }
            }

            // if army with largest battle value won
            if (largestWon)
            {
                // calculate casualty modifier for army with largest battle value
                // (based on adapted version of Lanchester's Square Law - i.e. largest army loses less troops than smallest)
                largeArmyModifier = (1 + ((minBV * minBV) / (maxBV * maxBV))) / 2;

                // attacker is large army
                if (attackerVictorious)
                {
                    battleCasualties[1] = smallestModifier;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
                // defender is large army
                else
                {
                    battleCasualties[0] = smallestModifier;
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
            }

            // if army with smallest battle value won
            else
            {
                // calculate casualty modifier for army with largest battle value
                // this ensures its losses will be roughly the same as the smallest army (because it lost)
                largeArmyModifier = Globals_Game.myRand.Next(80, 121) / (double)100;

                // defender is large army
                if (attackerVictorious)
                {
                    // smallest army losses reduced because they won
                    battleCasualties[0] = smallestModifier / 2;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
                // attacker is large army
                else
                {
                    battleCasualties[1] = smallestModifier / 2;
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
            }


            return battleCasualties;
        }

        /// <summary>
        /// Calculates whether either army has retreated due to the outcome of a battle
        /// </summary>
        /// <returns>int[] indicating the retreat distance (fiefs) of each army</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="aCasualties">The attacking army casualty modifier</param>
        /// <param name="dCasualties">The defending army casualty modifier</param>
        /// <param name="attackerVictorious">bool indicating if attacking army was victorious</param>
        public int[] checkForRetreat(Army attacker, Army defender, double aCasualties, double dCasualties, bool attackerVictorious)
        {
            bool[] hasRetreated = { false, false };
            int[] retreatDistance = { 0, 0 };

            // check if loser retreats due to battlefield casualties
            if (!attackerVictorious)
            {
                // if have >= 20% casualties
                if (aCasualties >= 0.2)
                {
                    // indicate attacker has retreated
                    hasRetreated[0] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[0] = Globals_Game.myRand.Next(1, 3);
                }
            }
            else
            {
                // if have >= 20% casualties
                if (dCasualties >= 0.2)
                {
                    // indicate defender has retreated
                    hasRetreated[1] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[1] = Globals_Game.myRand.Next(1, 3);
                }
            }

            // check to see if defender retreats due to aggression setting (i.e. was forced into battle)
            // NOTE: this will only happen if attacker and defender still present in fief
            if ((defender.aggression == 0) && (!hasRetreated[0] && !hasRetreated[1]))
            {
                // indicate defender has retreated
                hasRetreated[1] = true;

                // indicate retreat distance
                retreatDistance[1] = 1;
            }

            return retreatDistance;
        }

        /// <summary>
        /// Processes the retreat of an army
        /// </summary>
        /// <param name="a">The army to retreat</param>
        /// <param name="retreatDistance">The retreat distance</param>
        public void processRetreat(Army a, int retreatDistance)
        {
            // get starting fief
            Fief startingFief = a.getLocation();

            // get army leader
            Character thisLeader = a.getLeader();

            // get army owner
            PlayerCharacter thisOwner = a.getOwner();

            // for each hex in retreatDistance, process retreat
            for (int i = 0; i < retreatDistance; i++)
            {
                // get current location
                Fief from = a.getLocation();

                // get fief to retreat to
                Fief target = Globals_Game.gameMap.chooseRandomHex(from, true, thisOwner, startingFief);

                if (target != null)
                {
                    // get travel cost
                    double travelCost = this.getTravelCost(from, target);

                    // check for army leader (defender may not have had one)
                    if (thisLeader != null)
                    {
                        // ensure leader has enough days (retreats are immune to running out of days)
                        if (thisLeader.days < travelCost)
                        {
                            thisLeader.adjustDays(thisLeader.days - travelCost);
                        }

                        // perform retreat
                        bool success = this.moveCharacter(thisLeader, target, travelCost, false);
                    }

                    // if no leader
                    else
                    {
                        // ensure army has enough days (retreats are immune to running out of days)
                        if (a.days < travelCost)
                        {
                            a.days = travelCost;
                        }

                        // perform retreat
                        a.moveWithoutLeader(target, travelCost);
                    }
                }

            }

        }

        /// <summary>
        /// Elects a new leader from NPCs accompanying an army (upon death of PC leader)
        /// </summary>
        /// <returns>The new leader</returns>
        /// <param name="attacker">List<NonPlayerCharacter> containing candidates for the post</param>
        public NonPlayerCharacter electNewArmyLeader(List<NonPlayerCharacter> candidates)
        {
            NonPlayerCharacter newLeader = null;

            double highestRating = 0;

            foreach (NonPlayerCharacter candidate in candidates)
            {
                double armyLeaderRating = candidate.calcArmyLeadershipRating();

                if (armyLeaderRating > highestRating)
                {
                    highestRating = armyLeaderRating;
                    newLeader = candidate;
                }
            }

            return newLeader;
        }

        /// <summary>
        /// Implements the processes involved in a battle between two armies in the field
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        public bool giveBattle(Army attacker, Army defender, string circumstance = "battle")
        {
            string toDisplay = "";
            bool attackerVictorious = false;
            bool battleHasCommenced = false;
            uint[] battleValues = new uint[2];
            double[] casualtyModifiers = new double[2];
            double statureChange = 0;

            // if applicable, get siege
            Siege thisSiege = null;
            string thisSiegeID = defender.checkIfBesieger();
            if (!String.IsNullOrWhiteSpace(thisSiegeID))
            {
                // get siege
                thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
            }

            // get starting troop numbers
            uint attackerStartTroops = attacker.calcArmySize();
            uint defenderStartTroops = defender.calcArmySize();
            uint attackerCasualties = 0;
            uint defenderCasualties = 0;

            // get leaders
            Character attackerLeader = attacker.getLeader();
            Character defenderLeader = defender.getLeader();

            // introductory text for message
            switch (circumstance)
            {
                case "pillage":
                    toDisplay += "The fief garrison and militia, led by " + attackerLeader.firstName
                        + " " + attackerLeader.familyName + ", sallied forth to bring the pillaging "
                        + defender.armyID + ",";
                    if (defenderLeader != null)
                    {
                        toDisplay += " led by " + defenderLeader.firstName + " " + defenderLeader.familyName + " and";
                    }
                    toDisplay += " owned by " + defender.getOwner().firstName + " " + defender.getOwner().familyName
                        + ", to battle."
                        + "\r\n\r\nOutcome: ";
                    break;
                case "siege":
                    toDisplay += "The fief garrison and militia, led by " + attackerLeader.firstName
                        + " " + attackerLeader.familyName + ", sallied forth to bring the besieging "
                        + defender.armyID + ",";
                    if (defenderLeader != null)
                    {
                        toDisplay += " led by " + defenderLeader.firstName + " " + defenderLeader.familyName + " and";
                    }
                    toDisplay += " owned by " + defender.getOwner().firstName + " " + defender.getOwner().familyName
                        + ", to battle."
                        + "\r\n\r\nOutcome: ";
                    break;
                default:
                    toDisplay += "On this day of our lord " + attacker.armyID + ",";
                    if (attackerLeader != null)
                    {
                        toDisplay += " led by " + attackerLeader.firstName + " " + attackerLeader.familyName + " and";
                    }
                    toDisplay += " owned by "
                        + attacker.getOwner().firstName + " " + attacker.getOwner().familyName
                        + ", moved to attack " + defender.armyID + ",";
                    if (defenderLeader != null)
                    {
                        toDisplay += " led by " + defenderLeader.firstName + " " + defenderLeader.familyName + " and";
                    }
                    toDisplay += " owned by " + defender.getOwner().firstName
                        + " " + defender.getOwner().familyName + ", in the fief of " + attacker.getLocation().name
                        + "\r\n\r\nOutcome: ";
                    break;
            }

            // get battle values for both armies
            battleValues = this.calculateBattleValue(attacker, defender);

            // check if attacker has managed to bring defender to battle
            // case 1: defending army sallies during siege to attack besieger = battle always occurs
            if (circumstance.Equals("siege"))
            {
                battleHasCommenced = true;
            }
            // case 2: defending militia attacks pillaging army during pollage = battle always occurs
            else if (circumstance.Equals("pillage"))
            {
                battleHasCommenced = true;
            }
            // case 3: defender aggression and combatOdds allows battle
            else if (defender.aggression != 0)
            {
                if (defender.aggression == 1)
                {
                    // get odds
                    int battleOdds = this.getBattleOdds(attacker, defender);

                    // if odds OK, give battle
                    if (battleOdds <= defender.combatOdds)
                    {
                        battleHasCommenced = true;
                    }

                    // if not, check for battle
                    else
                    {
                        battleHasCommenced = this.bringToBattle(battleValues[0], battleValues[1], circumstance);

                        if (!battleHasCommenced)
                        {
                            this.processRetreat(defender, 1);
                        }
                    }
                }

                else
                {
                    battleHasCommenced = true;
                }
            }

            // otherwise, check to see if the attacker can bring the defender to battle
            else
            {
                battleHasCommenced = this.bringToBattle(battleValues[0], battleValues[1], circumstance);

                if (!battleHasCommenced)
                {
                    this.processRetreat(defender, 1);
                }
            }

            if (battleHasCommenced)
            {
                // WHO HAS WON?
                // calculate if attacker has won
                attackerVictorious = this.decideBattleVictory(battleValues[0], battleValues[1]);

                // UPDATE STATURE
                // get winner and loser
                Army winner = null;
                Army loser = null;
                if (attackerVictorious)
                {
                    winner = attacker;
                    loser = defender;
                }
                else
                {
                    winner = defender;
                    loser = attacker;
                }

                // calculate and apply winner's stature increase
                statureChange = 0.8 * (loser.calcArmySize() / Convert.ToDouble(10000));
                winner.getOwner().adjustStatureModifier(statureChange);

                // calculate and apply loser's stature loss
                statureChange = -0.5 * (winner.calcArmySize() / Convert.ToDouble(10000));
                loser.getOwner().adjustStatureModifier(statureChange);

                // CASUALTIES
                // calculate troop casualties for both sides
                casualtyModifiers = this.calculateBattleCasualties(attackerStartTroops, defenderStartTroops, battleValues[0], battleValues[1], attackerVictorious);

                // check if losing army has disbanded
                bool attackerDisbanded = false;
                bool defenderDisbanded = false;
                uint totalAttackTroopsLost = 0;
                uint totalDefendTroopsLost = 0;

                // if losing side sustains >= 50% casualties, disbands
                if (attackerVictorious)
                {
                    // either indicate losing army to be disbanded
                    if (casualtyModifiers[1] >= 0.5)
                    {
                        defenderDisbanded = true;
                        totalDefendTroopsLost = defender.calcArmySize();
                    }
                    // OR apply troop casualties to losing army
                    else
                    {
                        totalDefendTroopsLost = defender.applyTroopLosses(casualtyModifiers[1]);
                    }

                    // apply troop casualties to winning army
                    totalAttackTroopsLost = attacker.applyTroopLosses(casualtyModifiers[0]);
                }
                else
                {
                    if (casualtyModifiers[0] >= 0.5)
                    {
                        attackerDisbanded = true;
                        totalAttackTroopsLost = attacker.calcArmySize();
                    }
                    else
                    {
                        totalAttackTroopsLost = attacker.applyTroopLosses(casualtyModifiers[0]);
                    }

                    totalDefendTroopsLost = defender.applyTroopLosses(casualtyModifiers[1]);
                }

                // UPDATE TOTAL SIEGE LOSSES, if appropriate
                // NOTE: the defender in this battle is the attacker in the siege and v.v.
                if (thisSiege != null)
                {
                    // update total defender siege losses
                    thisSiege.totalCasualtiesDefender += Convert.ToInt32(totalAttackTroopsLost);
                    // update total attacker siege losses
                    thisSiege.totalCasualtiesAttacker += Convert.ToInt32(totalDefendTroopsLost);
                }

                // get casualty figures (for message)
                if (!attackerDisbanded)
                {
                    // get attacker casualties
                    attackerCasualties = totalAttackTroopsLost;
                }
                if (!defenderDisbanded)
                {
                    // get defender casualties
                    defenderCasualties = totalDefendTroopsLost;
                }

                // DAYS
                // adjust days
                // NOTE: don't adjust days if is a siege (will be deducted elsewhere)
                if (!circumstance.Equals("siege"))
                {
                    attackerLeader.adjustDays(1);
                    // need to check for defender having no leader
                    if (defenderLeader != null)
                    {
                        defenderLeader.adjustDays(1);
                    }
                    else
                    {
                        defender.days -= 1;
                    }
                }

                // RETREATS
                // create array of armies (for easy processing)
                Army[] bothSides = { attacker, defender };

                // check if either army needs to retreat
                int[] retreatDistances = this.checkForRetreat(attacker, defender, casualtyModifiers[0], casualtyModifiers[1], attackerVictorious);

                // if is pillage or siege, attacking army (the fief's army) doesn't retreat
                // if is pillage, the defending army (the pillagers) always retreats if has lost
                if (circumstance.Equals("pillage") || circumstance.Equals("siege"))
                {
                    retreatDistances[0] = 0;
                }

                if (circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        retreatDistances[1] = 1;
                    }
                }

                // if have retreated, perform it
                for (int i = 0; i < retreatDistances.Length; i++)
                {
                    if (retreatDistances[i] > 0)
                    {
                        this.processRetreat(bothSides[i], retreatDistances[i]);
                    }
                }

                // PC/NPC INJURIES/DEATHS
                // check if any PCs/NPCs have been wounded or killed
                bool characterDead = false;

                // 1. ATTACKER
                bool attackerLeaderDead = false;
                uint friendlyBV = battleValues[0];
                uint enemyBV = battleValues[1];

                // if army leader a PC, check entourage
                if (attackerLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (attackerLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        if ((attackerLeader as PlayerCharacter).myNPCs[i].inEntourage)
                        {
                            characterDead = (attackerLeader as PlayerCharacter).myNPCs[i].calculateCombatInjury(casualtyModifiers[0]);
                        }

                        // process death, if applicable
                        if (characterDead)
                        {
                            (attackerLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                        }
                    }
                }

                // check army leader
                attackerLeaderDead = attackerLeader.calculateCombatInjury(casualtyModifiers[0]);

                // process death, if applicable
                if (attackerLeaderDead)
                {
                    Character newLeader = null;

                    // if is pillage, do NOT elect new leader for attacking army
                    if (!circumstance.Equals("pillage"))
                    {
                        // if possible, elect new leader from entourage
                        if (attackerLeader is PlayerCharacter)
                        {
                            if ((attackerLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = this.electNewArmyLeader((attackerLeader as PlayerCharacter).myNPCs);
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        attacker.assignNewLeader(newLeader);
                    }

                    // process death
                    attackerLeader.processDeath("injury");
                }
                else
                {
                    // if pillage, if fief's army loses, make sure bailiff always returns to keep
                    if (circumstance.Equals("pillage"))
                    {
                        if (!attackerVictorious)
                        {
                            attackerLeader.inKeep = true;
                        }
                    }
                }

                // 2. DEFENDER
                bool defenderLeaderDead = false;

                // need to check if defending army had a leader
                if (defenderLeader != null)
                {
                    // if army leader a PC, check entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs[i].inEntourage)
                            {
                                characterDead = (defenderLeader as PlayerCharacter).myNPCs[i].calculateCombatInjury(casualtyModifiers[1]);
                            }

                            // process death, if applicable
                            if (characterDead)
                            {
                                (defenderLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                            }
                        }
                    }

                    // check army leader
                    defenderLeaderDead = defenderLeader.calculateCombatInjury(casualtyModifiers[1]);

                    // process death, if applicable
                    if (defenderLeaderDead)
                    {
                        Character newLeader = null;

                        // if possible, elect new leader from entourage
                        if (defenderLeader is PlayerCharacter)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = this.electNewArmyLeader((defenderLeader as PlayerCharacter).myNPCs);
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        defender.assignNewLeader(newLeader);

                        // process death
                        defenderLeader.processDeath("injury");
                    }
                }

                // UPDATE MESSAGE
                // who won
                if (attackerVictorious)
                {
                    toDisplay += attacker.armyID;
                }
                else
                {
                    toDisplay += defender.armyID;
                }
                toDisplay += " was victorious.\r\n\r\n";

                // casualties & retreats - attacker
                if (attackerDisbanded)
                {
                    toDisplay += attacker.armyID + " disbanded due to heavy casualties";
                }
                else
                {
                    toDisplay += attacker.armyID + " suffered a total of " + attackerCasualties + " casualties";
                    if (retreatDistances[0] > 0)
                    {
                        toDisplay += " and retreated from the fief";
                    }
                }
                toDisplay += ".";
                if (attackerLeaderDead)
                {
                    toDisplay += " " + attackerLeader.firstName + " " + attackerLeader.familyName + " died due to injuries received.";
                }
                toDisplay += "\r\n\r\n";

                // casualties & retreats - defender
                if (defenderDisbanded)
                {
                    toDisplay += defender.armyID + " disbanded due to heavy casualties";
                }
                else
                {
                    toDisplay += defender.armyID + " suffered a total of " + defenderCasualties + " casualties";
                    if (retreatDistances[1] > 0)
                    {
                        toDisplay += " and retreated from the fief";
                    }
                }
                toDisplay += ".";
                if ((defenderLeader != null) && (defenderLeaderDead))
                {
                    toDisplay += " " + defenderLeader.firstName + " " + defenderLeader.familyName + " died due to injuries received.";
                }
                toDisplay += "\r\n\r\n";

                if (circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        toDisplay += "The pillage in " + attacker.getLocation().name + " has been prevented.";
                    }
                }

                // check for SIEGE RELIEF
                // attacker (relieving army) victory or defender (besieging army) retreat = relief
                if ((attackerVictorious) || (retreatDistances[1] > 0))
                {
                    // check to see if defender was besieging the fief keep
                    if (thisSiege != null)
                    {
                        // construct event description to be passed into siegeEnd
                        string siegeDescription = "On this day of Our Lord the forces of ";
                        siegeDescription += attacker.getOwner().firstName + " " + attacker.getOwner().familyName;
                        siegeDescription += " have defeated the forces of " + thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                        siegeDescription += ", relieving the siege of " + thisSiege.getFief().name + ".";
                        siegeDescription += " " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                        siegeDescription += " retains ownership of the fief.";

                        // end the siege
                        this.siegeEnd(thisSiege, siegeDescription);

                        // add to message
                        toDisplay += "The siege in " + thisSiege.getFief().name + " has been raised.";

                    }
                }

                // DISBANDMENT

                // if is pillage, attacking (temporary) army always disbands after battle
                if (circumstance.Equals("pillage"))
                {
                    attackerDisbanded = true;
                }

                // process army disbandings (after all other functions completed)
                if (attackerDisbanded)
                {
                    this.disbandArmy(attacker);
                }

                if (defenderDisbanded)
                {
                    this.disbandArmy(defender);
                }

            }
            else
            {
                if ((circumstance.Equals("pillage")) || circumstance.Equals("siege"))
                {
                    toDisplay += attacker.armyID + " was unsuccessfull in bringing " + defender.armyID + " to battle.";
                }
                else
                {
                    toDisplay += defender.armyID + " successfully refused battle and retreated from the fief.";
                }
            }

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            // personae tags vary depending on circumstance
            string attackOwnTag = "|attackerOwner";
            string attackLeadTag = "|attackerLeader";
            string defendOwnTag = "|defenderOwner";
            string defendLeadTag = "|defenderLeader";
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("siege")))
            {
                attackOwnTag = "|sallyOwner";
                attackLeadTag = "|sallyLeader";
                defendOwnTag = "|defenderAgainstSallyOwner";
                defendLeadTag = "|defenderAgainstSallyLeader";
            }
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(defender.getOwner().charID + defendOwnTag);
            tempPersonae.Add(attackerLeader.charID + attackLeadTag);
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + defendLeadTag);
            }
            tempPersonae.Add(attacker.getOwner().charID + attackOwnTag);
            tempPersonae.Add(attacker.getLocation().owner.charID + "|fiefOwner");
            if ((!circumstance.Equals("pillage")) && (!circumstance.Equals("siege")))
            {
                tempPersonae.Add("all|all");
            }
            string[] battlePersonae = tempPersonae.ToArray();

            // location
            string battleLocation = attacker.getLocation().id;

            // use popup text as description
            string battleDescription = toDisplay;

            // put together new journal entry
            JournalEntry battleResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, battlePersonae, "battle", loc: battleLocation, descr: battleDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(battleResult);

            // display pop-up informational message
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(toDisplay, "BATTLE RESULTS");
            }

            // refresh screen
            this.refreshCurrentScreen();

            return attackerVictorious;

        }

        /// <summary>
        /// Calculates rough battle odds between two armies (i.e ratio of attacking army combat
        /// value to defending army combat value).  NOTE: does not involve leadership values
        /// </summary>
        /// <returns>int containing battle odds</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        public int getBattleOdds(Army attacker, Army defender)
        {
            double battleOdds = 0;

            battleOdds = Math.Floor(attacker.calculateCombatValue() / defender.calculateCombatValue());

            return Convert.ToInt32(battleOdds);
        }
    }
}
