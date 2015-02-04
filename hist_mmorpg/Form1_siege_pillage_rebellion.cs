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
    /// Partial class for Form1, containing functionality specific to siege, pillage and rebellion
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Retrieves information for Siege display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="s">Siege for which information is to be displayed</param>
        public string DisplaySiegeData(Siege s)
        {
            string siegeText = "";
            Fief siegeLocation = s.getFief();
            PlayerCharacter fiefOwner = siegeLocation.owner;
            bool isDefender = (fiefOwner == Globals_Client.myPlayerCharacter);
            Army besieger = s.getBesiegingArmy();
            PlayerCharacter besiegingPlayer = s.getBesiegingPlayer();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            Character besiegerLeader = besieger.GetLeader();
            Character defGarrLeader = defenderGarrison.GetLeader();
            Character defAddLeader = null;
            if (defenderAdditional != null)
            {
                defAddLeader = defenderAdditional.GetLeader();
            }

            // ID
            siegeText += "ID: " + s.siegeID + "\r\n\r\n";

            // fief
            siegeText += "Fief: " + siegeLocation.name + " (Province: " + siegeLocation.province.name + ".  Kingdom: " + siegeLocation.province.kingdom.name + ")\r\n\r\n";

            // fief owner
            siegeText += "Fief owner: " + fiefOwner.firstName + " " + fiefOwner.familyName + " (ID: " + fiefOwner.charID + ")\r\n\r\n";

            // besieging player
            siegeText += "Besieging player: " + besiegingPlayer.firstName + " " + besiegingPlayer.familyName + " (ID: " + besiegingPlayer.charID + ")\r\n\r\n";

            // start date
            siegeText += "Start date: " + s.startYear + ", " + Globals_Game.clock.seasons[s.startSeason] + "\r\n\r\n";

            // duration so far
            siegeText += "Days used so far: " + s.totalDays + "\r\n\r\n";

            // days left in current season
            siegeText += "Days remaining in current season: " + s.days + "\r\n\r\n";

            // defending forces
            siegeText += "Defending forces: ";
            // only show details if player is defender
            if (isDefender)
            {
                // garrison details
                siegeText += "\r\nGarrison: " + defenderGarrison.armyID + "\r\n";
                siegeText += "- Leader: ";
                if (defGarrLeader != null)
                {
                    siegeText += defGarrLeader.firstName + " " + defGarrLeader.familyName + " (ID: " + defGarrLeader.charID + ")";
                }
                else
                {
                    siegeText += "None";
                }
                siegeText += "\r\n";
                siegeText += "- [Kn: " + defenderGarrison.troops[0] + ";  MAA: " + defenderGarrison.troops[1]
                    + ";  LCav: " + defenderGarrison.troops[2] + ";  Lng: " + defenderGarrison.troops[3]
                    + ";  Crss: " + defenderGarrison.troops[4] + ";  Ft: " + defenderGarrison.troops[5]
                    + ";  Rbl: " + defenderGarrison.troops[6] + "]";

                // additional army details
                if (defenderAdditional != null)
                {
                    siegeText += "\r\n\r\nField army: " + defenderAdditional.armyID + "\r\n";
                    siegeText += "- Leader: ";
                    if (defAddLeader != null)
                    {
                        siegeText += defAddLeader.firstName + " " + defAddLeader.familyName + " (ID: " + defAddLeader.charID + ")";
                    }
                    else
                    {
                        siegeText += "None";
                    }
                    siegeText += "\r\n";
                    siegeText += "- [Kn: " + defenderAdditional.troops[0] + ";  MAA: " + defenderAdditional.troops[1]
                        + ";  LCav: " + defenderAdditional.troops[2] + ";  Lng: " + defenderAdditional.troops[3]
                        + ";  Crss: " + defenderAdditional.troops[4] + ";  Ft: " + defenderAdditional.troops[5]
                        + ";  Rbl: " + defenderAdditional.troops[6] + "]";
                }

                siegeText += "\r\n\r\nTotal defender casualties so far (including attrition): " + s.totalCasualtiesDefender;
            }

            // if player not defending, hide defending forces details
            else
            {
                siegeText += "Unknown";
            }
            siegeText += "\r\n\r\n";

            // besieging forces
            siegeText += "Besieging forces: ";
            // only show details if player is besieger
            if (!isDefender)
            {
                // besieging forces details
                siegeText += "\r\nField army: " + besieger.armyID + "\r\n";
                siegeText += "- Leader: ";
                if (besiegerLeader != null)
                {
                    siegeText += besiegerLeader.firstName + " " + besiegerLeader.familyName + " (ID: " + besiegerLeader.charID + ")";
                }
                else
                {
                    siegeText += "None";
                }
                siegeText += "\r\n";
                siegeText += "- [Kn: " + besieger.troops[0] + ";  MAA: " + besieger.troops[1]
                    + ";  LCav: " + besieger.troops[2] + ";  Lng: " + besieger.troops[3] + ";  Crss: " + besieger.troops[4]
                    + ";  Ft: " + besieger.troops[5] + ";  Rbl: " + besieger.troops[6] + "]";

                siegeText += "\r\n\r\nTotal attacker casualties so far (including attrition): " + s.totalCasualtiesAttacker;
            }

            // if player not besieger, hide besieging forces details
            else
            {
                siegeText += "Unknown";
            }
            siegeText += "\r\n\r\n";

            // keep level
            siegeText += "Keep level:\r\n";
            // keep level at start
            siegeText += "- at start of siege: " + s.startKeepLevel + "\r\n";

            // current keep level
            siegeText += "- current: " + siegeLocation.keepLevel + "\r\n\r\n";

            if (!isDefender)
            {
                siegeText += "Chance of success in next round:\r\n";
                // chance of storm success
                /* double keepLvl = this.calcStormKeepLevel(s);
                double successChance = this.calcStormSuccess(keepLvl); */
                // get battle values for both armies
                uint[] battleValues = this.CalculateBattleValue(besieger, defenderGarrison, Convert.ToInt32(siegeLocation.keepLevel));
                double successChance = this.CalcVictoryChance(battleValues[0], battleValues[1]);
                siegeText += "- storm: " + successChance + "\r\n";

                // chance of negotiated success
                siegeText += "- negotiated: " + successChance / 2 + "\r\n\r\n";
            }

            return siegeText;
        }

        /// <summary>
        /// Refreshes main Siege display screen
        /// </summary>
        /// <param name="s">Siege whose information is to be displayed</param>
        public void refreshSiegeContainer(Siege s = null)
        {

            // clear existing information
            this.siegeTextBox.Text = "";

            // ensure textboxes aren't interactive
            this.siegeTextBox.ReadOnly = true;

            // disable controls until siege selected
            this.siegeNegotiateBtn.Enabled = false;
            this.siegeStormBtn.Enabled = false;
            this.siegeReduceBtn.Enabled = false;
            this.siegeEndBtn.Enabled = false;

            // clear existing items in siege list
            this.siegeListView.Items.Clear();

            // iterates through player's sieges adding information to ListView
            for (int i = 0; i < Globals_Client.myPlayerCharacter.mySieges.Count; i++)
            {
                ListViewItem thisSiegeItem = null;
                Siege thisSiege = Globals_Client.myPlayerCharacter.getSiege(Globals_Client.myPlayerCharacter.mySieges[i]);

                if (thisSiege != null)
                {
                    // siegeID
                    thisSiegeItem = new ListViewItem(thisSiege.siegeID);

                    // fief
                    Fief siegeLocation = thisSiege.getFief();
                    thisSiegeItem.SubItems.Add(siegeLocation.name + " (" + siegeLocation.id + ")");

                    // defender
                    PlayerCharacter defendingPlayer = thisSiege.getDefendingPlayer();
                    thisSiegeItem.SubItems.Add(defendingPlayer.firstName + " " + defendingPlayer.familyName + " (" + defendingPlayer.charID + ")");

                    // besieger
                    Army besiegingArmy = thisSiege.getBesiegingArmy();
                    PlayerCharacter besieger = thisSiege.getBesiegingPlayer();
                    thisSiegeItem.SubItems.Add(besieger.firstName + " " + besieger.familyName + " (" + besieger.charID + ")");

                    if (thisSiegeItem != null)
                    {
                        // if siege passed in as parameter, show as selected
                        if (thisSiege == s)
                        {
                            thisSiegeItem.Selected = true;
                        }

                        // add item to siegeListView
                        this.siegeListView.Items.Add(thisSiegeItem);
                    }
                }

            }

            Globals_Client.containerToView = this.siegeContainer;
            Globals_Client.containerToView.BringToFront();
            this.siegeListView.Focus();
        }

        /// <summary>
        /// Calculates the outcome of the pillage of a fief by an army
        /// </summary>
        /// <param name="f">The fief being pillaged</param>
        /// <param name="a">The pillaging army</param>
        /// <param name="circumstance">The circumstance under which the fief is being pillaged</param>
        public void processPillage(Fief f, Army a, string circumstance = "pillage")
        {
            string pillageResults = "";
            double thisLoss = 0;
            double moneyPillagedTotal = 0;
            double moneyPillagedOwner = 0;
            double pillageMultiplier = 0;

            // get army leader
            Character armyLeader = a.GetLeader();

            // get pillaging army owner (receives a proportion of total spoils)
            PlayerCharacter armyOwner = a.GetOwner();

            // get garrison leader (to add to journal entry)
            Character defenderLeader = null;
            if (f.bailiff != null)
            {
                defenderLeader = f.bailiff;
            }

            // calculate pillageMultiplier (based on no. pillagers per 1000 population)
            pillageMultiplier = a.CalcArmySize() / (f.population / 1000);

            // calculate days taken for pillage
            double daysTaken = Globals_Game.myRand.Next(7, 16);
            if (daysTaken > a.days)
            {
                daysTaken = a.days;
            }

            // update army days
            armyLeader.AdjustDays(daysTaken);

            pillageResults += "- Days taken: " + daysTaken + "\r\n";

            // % population loss
            thisLoss = (0.007 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply population loss
            pillageResults += "- Population loss: " + Convert.ToUInt32((f.population * (thisLoss / 100))) + "\r\n";
            f.population -= Convert.ToInt32((f.population * (thisLoss / 100)));

            // % treasury loss
            if (!circumstance.Equals("quellRebellion"))
            {
                thisLoss = (0.2 * pillageMultiplier);
                // ensure is between 1%-80%
                if (thisLoss < 1)
                {
                    thisLoss = 1;
                }
                else if (thisLoss > 80)
                {
                    thisLoss = 80;
                }
                // apply treasury loss
                pillageResults += "- Treasury loss: " + Convert.ToInt32((f.treasury * (thisLoss / 100))) + "\r\n";
                if (f.treasury > 0)
                {
                    f.treasury -= Convert.ToInt32((f.treasury * (thisLoss / 100)));
                }
            }

            // % loyalty loss
            thisLoss = (0.33 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply loyalty loss
            pillageResults += "- Loyalty loss: " + (f.loyalty * (thisLoss / 100)) + "\r\n";
            f.loyalty -= (f.loyalty * (thisLoss / 100));

            // % fields loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply fields loss
            pillageResults += "- Fields loss: " + (f.fields * (thisLoss / 100)) + "\r\n";
            f.fields -= (f.fields * (thisLoss / 100));

            // % industry loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply industry loss
            pillageResults += "- Industry loss: " + (f.industry * (thisLoss / 100)) + "\r\n";
            f.industry -= (f.industry * (thisLoss / 100));

            // money pillaged (based on GDP)
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-50%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 50)
            {
                thisLoss = 50;
            }
            // calculate base amount pillaged based on fief GDP
            double baseMoneyPillaged = (f.keyStatsCurrent[1] * (thisLoss / 100));
            moneyPillagedTotal = baseMoneyPillaged;
            pillageResults += "- Base Money Pillaged: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";

            // factor in no. days spent pillaging (get extra 5% per day > 7)
            int daysOver7 = Convert.ToInt32(daysTaken) - 7;
            if (daysOver7 > 0)
            {
                for (int i = 0; i < daysOver7; i++)
                {
                    moneyPillagedTotal += (baseMoneyPillaged * 0.05);
                }
                pillageResults += "  - with bonus for extra " + daysOver7 + " days taken: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";
            }

            // check for jackpot
            // generate randomPercentage to see if hit the jackpot
            int myRandomPercent = Globals_Game.myRand.Next(101);
            if (myRandomPercent <= 30)
            {
                // generate random int to multiply amount pillaged
                int myRandomMultiplier = Globals_Game.myRand.Next(3, 11);
                moneyPillagedTotal = moneyPillagedTotal * myRandomMultiplier;
                pillageResults += "  - with bonus for jackpot: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";
            }

            // check proportion of money pillaged goes to army owner (based on stature)
            double proportionForOwner = 0.05 * armyOwner.CalculateStature();
            moneyPillagedOwner = (moneyPillagedTotal * proportionForOwner);
            pillageResults += "- Money pillaged by attacking player: " + Convert.ToInt32(moneyPillagedOwner) + "\r\n";

            // apply to army owner's home fief treasury
            armyOwner.getHomeFief().treasury += Convert.ToInt32(moneyPillagedOwner);

            // apply loss of stature to army owner if fief has same language
            if (armyOwner.language.id == f.language.id)
            {
                armyOwner.AdjustStatureModifier(-0.3);
            }
            else if (armyOwner.language.baseLanguage.id == f.language.baseLanguage.id)
            {
                armyOwner.AdjustStatureModifier(-0.2);
            }

            // set isPillaged for fief
            f.isPillaged = true;

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(f.owner.charID + "|fiefOwner");
            tempPersonae.Add(armyOwner.charID + "|attackerOwner");
            if (armyLeader != null)
            {
                tempPersonae.Add(armyLeader.charID + "|attackerLeader");
            }
            if ((defenderLeader != null) && (!circumstance.Equals("quellRebellion")))
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderLeader");
            }
            if (circumstance.Equals("quellRebellion"))
            {
                tempPersonae.Add("all|all");
            }
            string[] pillagePersonae = tempPersonae.ToArray();

            // location
            string pillageLocation = f.id;

            // type
            string type = "";
            if (circumstance.Equals("pillage"))
            {
                type += "pillage";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                type += "rebellionQuelled";
            }

            // use popup text as description
            string pillageDescription = "";

            if (circumstance.Equals("pillage"))
            {
                pillageDescription += "On this day of Our Lord the fief of ";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageDescription += "On this day of Our Lord the rebellion in the fief of ";
            }
            pillageDescription += f.name + " owned by " + f.owner.firstName + " " + f.owner.familyName;

            if ((circumstance.Equals("pillage")) && (defenderLeader != null))
            {
                if (f.owner != defenderLeader)
                {
                    pillageDescription += " and defended by " + defenderLeader.firstName + " " + defenderLeader.familyName + ",";
                }
            }

            if (circumstance.Equals("pillage"))
            {
                pillageDescription += " was pillaged by the forces of ";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageDescription += " was quelled by the forces of ";
            }
            pillageDescription += armyOwner.firstName + " " + armyOwner.familyName;
            if (armyLeader != null)
            {
                if (armyOwner != armyLeader)
                {
                    pillageDescription += ", led by " + armyLeader.firstName + " " + armyLeader.familyName;
                }
            }
            pillageDescription += ".\r\n\r\nResults:\r\n";
            pillageDescription += pillageResults;

            // put together new journal entry
            JournalEntry pillageEntry = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, pillagePersonae, type, loc: pillageLocation, descr: pillageDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(pillageEntry);

            // show message
            if (Globals_Client.showMessages)
            {
                // set label
                string messageLabel = "";

                if (circumstance.Equals("pillage"))
                {
                    messageLabel += "PILLAGE ";
                }
                else if (circumstance.Equals("quellRebellion"))
                {
                    messageLabel += "QUELL REBELLION ";
                }

                // show message
                System.Windows.Forms.MessageBox.Show(pillageDescription, messageLabel + "RESULTS");
            }
        }

        /// <summary>
        /// Creates a defending army for defence of a fief during pillage or siege
        /// </summary>
        /// <returns>The defending army</returns>
        /// <param name="f">The fief being pillaged</param>
        public Army CreateDefendingArmy(Fief f)
        {
            Army defender = null;
            Character armyLeader = null;
            string armyLeaderID = null;
            double armyLeaderDays = 90;

            // if present in fief, get bailiff and assign as army leader
            if (f.bailiff != null)
            {
                for (int i = 0; i < f.charactersInFief.Count; i++)
                {
                    if (f.charactersInFief[i] == f.bailiff)
                    {
                        armyLeader = f.bailiff;
                        armyLeaderID = armyLeader.charID;
                        armyLeaderDays = armyLeader.days;
                        break;
                    }
                }
            }

            // gather troops to create army
            uint garrisonSize = 0;
            uint militiaSize = 0;
            uint[] troopsForArmy = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint[] tempTroops = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint totalSoFar = 0;

            // get army nationality
            string thisNationality = f.owner.nationality.natID;

            // get size of fief garrison
            garrisonSize = Convert.ToUInt32(f.GetGarrisonSize());

            // get size of fief 'militia' responding to emergency
            militiaSize = Convert.ToUInt32(f.CallUpTroops(minProportion: 0.33, maxProportion: 0.66));

            // get defending troops based on following troop type proportions:
            // militia = Global_Server.recruitRatios for types 0-4, fill with rabble
            // garrison = Global_Server.recruitRatios * 2 for types 0-3, fill with foot

            // 1. militia (includes proportion of rabble)
            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = Convert.ToUInt32(militiaSize * Globals_Server.recruitRatios[thisNationality][i]);
                }
                // fill up with rabble
                else
                {
                    tempTroops[i] = militiaSize - totalSoFar;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // 2. garrison (all 'professional' troops)
            totalSoFar = 0;

            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 2)
                {
                    tempTroops[i] = Convert.ToUInt32(garrisonSize * (Globals_Server.recruitRatios[thisNationality][i] * 2));
                }
                // fill up with foot
                else if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = garrisonSize - totalSoFar;
                }
                // no rabble in garrison
                else
                {
                    tempTroops[i] = 0;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // create temporary army for battle/siege
            defender = new Army("Garrison" + Globals_Game.getNextArmyID(), armyLeaderID, f.owner.charID, armyLeaderDays, f.id, trp: troopsForArmy);
            defender.AddArmy();

            return defender;
        }

        /// <summary>
        /// Implements the processes involved in the pillage of a fief by an army
        /// </summary>
        /// <param name="a">The pillaging army</param>
        /// <param name="f">The fief being pillaged</param>
        public void pillageFief(Army a, Fief f)
        {
            bool pillageCancelled = false;
            bool bailiffPresent = false;
            Army fiefArmy = null;

            // check if bailiff present in fief (he'll lead the army)
            if (f.bailiff != null)
            {
                for (int i = 0; i < f.charactersInFief.Count; i++)
                {
                    if (f.charactersInFief[i] == f.bailiff)
                    {
                        bailiffPresent = true;
                        break;
                    }
                }
            }

            // if bailiff is present, create an army and attempt to give battle
            // no bailiff = no leader = pillage is unopposed by defending forces
            if (bailiffPresent)
            {
                // create temporary army for battle
                fiefArmy = this.CreateDefendingArmy(f);

                // give battle and get result
                pillageCancelled = this.GiveBattle(fiefArmy, a, circumstance: "pillage");

                if (pillageCancelled)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("The pillaging force has been forced to retreat by the fief's defenders!");
                    }
                }

                else
                {
                    // check still have enough days left
                    if (a.days < 7)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("After giving battle, the pillaging army no longer has\r\nsufficient days for this operation.  Pillage cancelled.");
                        }
                        pillageCancelled = true;
                    }
                }

            }

            if (!pillageCancelled)
            {
                // process pillage
                this.processPillage(f, a);
            }

        }

        /// <summary>
        /// Implements conditional checks prior to the pillage or siege of a fief
        /// </summary>
        /// <returns>bool indicating whether pillage/siege can proceed</returns>
        /// <param name="f">The fief being pillaged/besieged</param>
        /// <param name="a">The pillaging/besieging army</param>
        /// <param name="circumstance">The circumstance - pillage or siege</param>
        public bool ChecksBeforePillageSiege(Army a, Fief f, string circumstance = "pillage")
        {
            bool proceed = true;
            string operation = "";

            // check if is your own fief
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if (f.owner == a.GetOwner())
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot pillage your own fief!  Pillage cancelled.");
                        }
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot besiege your own fief!  Siege cancelled.");
                        }
                    }
                }
            }

            // check if fief is under siege
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if ((!String.IsNullOrWhiteSpace(f.siege)) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot pillage a fief that is under siege.  Pillage cancelled.");
                        }
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("This fief is already under siege.  Siege cancelled.");
                        }
                    }
                }
            }

            // check if fief already pillaged
            // note: not necessary for quell rebellion (get a 'free' pillage)
            if (!circumstance.Equals("quellRebellion"))
            {
                if (circumstance.Equals("pillage"))
                {
                    // check isPillaged = false
                    if ((f.isPillaged) && (proceed))
                    {
                        proceed = false;
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("This fief has already been pillaged during\r\nthe current season.  Pillage cancelled.");
                        }
                    }
                }
            }

            // check if your army has a leader
            if (a.GetLeader() == null)
            {
                proceed = false;

                if (circumstance.Equals("quellRebellion"))
                {
                    operation = "Operation";
                }
                if (circumstance.Equals("pillage"))
                {
                    operation = "Pillage";
                }
                else if (circumstance.Equals("siege"))
                {
                    operation = "Siege";
                }

                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This army has no leader.  " + operation + " cancelled.");
                }
            }

            // check has min days required
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("quellRebellion")))
            {
                // pillage = min 7
                if ((a.days < 7) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("quellRebellion"))
                    {
                        operation = "Quell rebellion";
                    }
                    else
                    {
                        operation = "Pillage";
                    }

                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na this operation.  " + operation + " cancelled.");
                    }
                }
            }
            else if (circumstance.Equals("siege"))
            {
                // siege = 1 (to set up siege)
                if ((a.days < 1) && (proceed))
                {
                    proceed = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na siege operation.  Siege cancelled.");
                    }
                }
            }

            // check for presence of armies belonging to fief owner
            if (proceed)
            {
                // iterate through armies in fief
                for (int i = 0; i < f.armies.Count; i++)
                {
                    // get army
                    Army armyInFief = Globals_Game.armyMasterList[f.armies[i]];

                    // check if owned by fief owner
                    if (armyInFief.owner.Equals(f.owner.charID))
                    {
                        // army must be outside keep
                        if (!armyInFief.GetLeader().inKeep)
                        {
                            // army must have correct aggression settings
                            if (armyInFief.aggression > 1)
                            {
                                proceed = false;
                                if (circumstance.Equals("pillage"))
                                {
                                    operation = "Pillage";
                                }
                                else if (circumstance.Equals("siege"))
                                {
                                    operation = "Siege";
                                }
                                else if (circumstance.Equals("quellRebellion"))
                                {
                                    operation = "Quell rebellion";
                                }

                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("There is at least one defending army (" + armyInFief.armyID + ") that must be defeated\r\nbefore you can conduct this operation.  " + operation + " cancelled.");
                                }

                                break;
                            }
                        }
                    }
                }

                // check if fief in rebellion
                if ((circumstance.Equals("siege")) && (proceed))
                {
                    if (f.status.Equals('R'))
                    {
                        proceed = false;

                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot lay siege to a keep if the fief is in rebellion.", "OPERATION CANCELLED");
                        }
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Implements conditional checks prior to a siege operation
        /// </summary>
        /// <returns>bool indicating whether siege operation can proceed</returns>
        /// <param name="s">The siege</param>
        /// <param name="operation">The operation - round or end</param>
        public bool ChecksBeforeSiegeOperation(Siege s, string operation = "round")
        {
            bool proceed = true;
            int daysRequired = 0;

            if (operation.Equals("round"))
            {
                daysRequired = 10;
            }
            else if (operation.Equals("end"))
            {
                daysRequired = 1;
            }

            // check has min days required
            if (s.days < daysRequired)
            {
                proceed = false;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are not enough days remaining for this\r\na siege operation.  Operation cancelled.");
                }
            }

            return proceed;
        }

        /// <summary>
        /// Ends the specified siege
        /// </summary>
        /// <param name="s">Siege to be ended</param>
        /// <param name="siegeSuccessful">bool indicating whether the siege was successful</param>
        /// <param name="s">String containing circumstances under which the siege ended</param>
        public void siegeEnd(Siege s, bool siegeSuccessful, string circumstance)
        {
            // carry out functions associated with siege end
            s.siegeEnd(siegeSuccessful, circumstance);

            // set to null
            s = null;

            // refresh screen
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Calculates the percentage chance of successfully storming a keep, based on keep level
        /// </summary>
        /// <returns>double containing precentage chance of success</returns>
        /// <param name="keepLvl">The keep level</param>
        public double CalcStormSuccess(double keepLvl)
        {
            double stormFailurePercent = 0;

            for (int i = 0; i <= keepLvl; i++)
            {
                if (i == 0)
                {
                    stormFailurePercent = 5;
                }
                else if (i == 1)
                {
                    stormFailurePercent = 70;
                }
                else
                {
                    stormFailurePercent = stormFailurePercent + (stormFailurePercent * (0.08 * (1 / (keepLvl - 1))));
                }

                // ensure is always slight chance of success
                if (stormFailurePercent > 99)
                {
                    stormFailurePercent = 99;
                }
            }

            // return success % (inverse of stormFailurePercent)
            return 100 - stormFailurePercent;
        }

        /// <summary>
        /// Processes the storming of the keep by attacking forces in a siege
        /// </summary>
        /// <param name="s">The siege</param>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public void siegeStormRound(Siege s, uint defenderCasualties, double originalKeepLvl)
        {
            bool stormSuccess = false;
            Fief besiegedFief = s.getFief();
            Army besiegingArmy = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            PlayerCharacter attackingPlayer = s.getBesiegingPlayer();
            Character defenderLeader = defenderGarrison.GetLeader();
            Character attackerLeader = besiegingArmy.GetLeader();
            double statureChange = 0;

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = s.getFief().id;

            // description
            string siegeDescription = "";

            // get STORM RESULT
            uint[] battleValues = this.CalculateBattleValue(besiegingArmy, defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel), true);
            stormSuccess = this.DecideBattleVictory(battleValues[0], battleValues[1]);

            // KEEP DAMAGE
            // base damage to keep level (10%)
            double keepDamageModifier = 0.1;

            // calculate further damage, based on comparative battle values (up to extra 15%)
            // uint [] battleValues = this.calculateBattleValue(besiegingArmy, defenderGarrison, Convert.ToInt32(keepLvl));
            // divide attackerBV by defenderBV to get extraDamageMultiplier
            double extraDamageMultiplier = battleValues[0] / battleValues[1];

            // ensure extraDamageMultiplier is max 10
            if (extraDamageMultiplier > 10)
            {
                extraDamageMultiplier = 10;
            }

            // generate random double 0-1 to see what proportion of extraDamageMultiplier will apply
            double myRandomDouble = Utility_Methods.GetRandomDouble(1);
            extraDamageMultiplier = extraDamageMultiplier * myRandomDouble;

            keepDamageModifier += (0.015 * extraDamageMultiplier);
            keepDamageModifier = (1 - keepDamageModifier);

            // apply keep damage
            besiegedFief.keepLevel = besiegedFief.keepLevel * keepDamageModifier;

            // CASUALTIES, based on comparative battle values and keep level
            // 1. DEFENDER
            // defender base casualtyModifier
            double defenderCasualtyModifier = 0.01;
            defenderCasualtyModifier = defenderCasualtyModifier * (Convert.ToDouble(battleValues[0]) / battleValues[1]);

            // apply casualties
            defenderCasualties += defenderGarrison.ApplyTroopLosses(defenderCasualtyModifier);
            // update total defender siege losses
            s.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // 2. ATTACKER
            double attackerCasualtyModifier = 0.01;
            attackerCasualtyModifier = attackerCasualtyModifier * (Convert.ToDouble(battleValues[1]) / battleValues[0]);
            // for attacker, add effects of keep level, modified by on storm success
            if (stormSuccess)
            {
                attackerCasualtyModifier += (0.005 * besiegedFief.keepLevel);
            }
            else
            {
                attackerCasualtyModifier += (0.01 * besiegedFief.keepLevel);
            }
            // apply casualties
            uint attackerCasualties = besiegingArmy.ApplyTroopLosses(attackerCasualtyModifier);
            // update total attacker siege losses
            s.totalCasualtiesAttacker += Convert.ToInt32(attackerCasualties);

            // PC/NPC INJURIES
            // NOTE: defender only (attacker leaders assumed not to have climbed the walls)
            bool characterDead = false;
            if (defenderLeader != null)
            {
                // if defenderLeader is PC, check for casualties amongst entourage
                if (defenderLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                        characterDead = thisNPC.CalculateCombatInjury(defenderCasualtyModifier);

                        if (characterDead)
                        {
                            // process death
                            (defenderLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");
                        }
                    }
                }

                // check defenderLeader
                characterDead = defenderLeader.CalculateCombatInjury(defenderCasualtyModifier);

                if (characterDead)
                {
                    // remove as leader
                    defenderGarrison.leader = null;

                    // process death
                    defenderLeader.ProcessDeath("injury");
                }
            }

            if (stormSuccess)
            {
                // pillage fief
                this.processPillage(besiegedFief, besiegingArmy);

                // CAPTIVES
                // identify captives - fief owner, his family, and any PCs of enemy nationality
                List<Character> captives = new List<Character>();
                foreach (Character thisCharacter in besiegedFief.charactersInFief)
                {
                    if (thisCharacter.inKeep)
                    {
                        // fief owner and his family
                        if (!String.IsNullOrWhiteSpace(thisCharacter.familyID))
                        {
                            if (thisCharacter.familyID.Equals(s.getDefendingPlayer().charID))
                            {
                                captives.Add(thisCharacter);
                            }
                        }

                        // PCs of enemy nationality
                        else if (thisCharacter is PlayerCharacter)
                        {
                            if (!thisCharacter.nationality.Equals(attackingPlayer.nationality))
                            {
                                captives.Add(thisCharacter);
                            }
                        }
                    }
                }

                // collect ransom
                int thisRansom = 0;
                int totalRansom = 0;
                foreach (Character thisCharacter in captives)
                {
                    // PCs
                    if (thisCharacter is PlayerCharacter)
                    {
                        // calculate ransom (10% of total GDP)
                        thisRansom = Convert.ToInt32(((thisCharacter as PlayerCharacter).getTotalGDP() * 0.1));
                        // remove from captive's home treasury
                        (thisCharacter as PlayerCharacter).getHomeFief().treasury -= thisRansom;
                    }
                    // NPCs (family of fief's old owner)
                    else
                    {
                        // calculate ransom (family allowance)
                        string thisFunction = (thisCharacter as NonPlayerCharacter).getFunction(s.getDefendingPlayer());
                        thisRansom = Convert.ToInt32((thisCharacter as NonPlayerCharacter).calcFamilyAllowance(thisFunction));
                        // remove from head of family's home treasury
                        s.getDefendingPlayer().getHomeFief().treasury -= thisRansom;
                    }

                    // add to besieger's home treasury
                    attackingPlayer.getHomeFief().treasury += thisRansom;
                    totalRansom += thisRansom;
                }

                // calculate change to besieging player's stature
                statureChange = 0.1 * (s.getFief().population / Convert.ToDouble(10000));

                // construct event description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY stormed the keep of " + s.getFief().name + ".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescriptionFull += "and " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ".\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += s.getDefendingPlayer().firstName + " " + s.getDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureChange + " stature.";
                // details of ransoms
                if (totalRansom > 0)
                {
                    siegeDescriptionFull += "\r\n\r\nA number of persons (";
                    Character lastCaptive = captives.Last();
                    foreach (Character thisCharacter in captives)
                    {
                        siegeDescriptionFull += thisCharacter.firstName + " " + thisCharacter.familyName;
                        if (thisCharacter != lastCaptive)
                        {
                            siegeDescriptionFull += ", ";
                        }
                    }
                    siegeDescriptionFull += ") were ransomed for a total of £" + totalRansom + ".";
                }

                // end the siege
                this.siegeEnd(s, true, siegeDescriptionFull);

                // change fief ownership
                besiegedFief.ChangeOwnership(attackingPlayer);
            }

            // storm unsuccessful
            else
            {
                // calculate change to besieging player's stature
                statureChange = -0.2 * (Convert.ToDouble(s.getFief().population) / 10000);

                // description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " were UNSUCCESSFULL in their attempt to storm the keep of " + s.getFief().name;
                siegeDescription += ".\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescription += "and " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".\r\n\r\nThis failure has resulted in a loss of ";
                siegeDescription += statureChange + " for " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // inform player of result
            System.Windows.Forms.MessageBox.Show(siegeDescription);

            // apply change to besieging player's stature
            s.getBesiegingPlayer().AdjustStatureModifier(statureChange);

            // refresh screen
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Processes a single negotiation round of a siege
        /// </summary>
        /// <returns>bool indicating whether negotiation was successful</returns>
        /// <param name="s">The siege</param>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public bool siegeNegotiationRound(Siege s, uint defenderCasualties, double originalKeepLvl)
        {
            bool negotiateSuccess = false;

            // get required objects
            Fief besiegedFief = s.getFief();
            Army besieger = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Character defenderLeader = defenderGarrison.GetLeader();
            Character attackerLeader = besieger.GetLeader();
            Army defenderAdditional = s.getDefenderAdditional();

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = s.getFief().id;

            // description
            string siegeDescription = "";

            // calculate success chance
            uint[] battleValues = this.CalculateBattleValue(besieger, defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel));
            double successChance = this.CalcVictoryChance(battleValues[0], battleValues[1]) / 2;

            // generate random double 0-100 to see if negotiation a success
            double myRandomDouble = Utility_Methods.GetRandomDouble(100);

            if (myRandomDouble <= successChance)
            {
                negotiateSuccess = true;
            }

            // negotiation successful
            if (negotiateSuccess)
            {
                // add to winning player's stature
                double statureIncrease = 0.2 * (s.getFief().population / Convert.ToDouble(10000));
                s.getBesiegingPlayer().AdjustStatureModifier(statureIncrease);

                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY negotiated an end to the siege of " + s.getFief().name + ".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += s.getDefendingPlayer().firstName + " " + s.getDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureIncrease + " stature.";
                siegeDescriptionFull += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescriptionFull += besiegedFief.keepLevel + ".";

                // end the siege
                this.siegeEnd(s, true, siegeDescriptionFull);

                // change fief ownership
                s.getFief().ChangeOwnership(s.getBesiegingPlayer());

            }

            // negotiation unsuccessful
            else
            {
                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " FAILED to negotiate an end to the siege of " + s.getFief().name + ".";
                siegeDescription += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".";
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // update total defender siege losses
            s.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // inform player of success
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(siegeDescription);
            }

            // refresh screen
            this.refreshCurrentScreen();

            return negotiateSuccess;
        }

        /// <summary>
        /// Processes a single reduction round of a siege
        /// </summary>
        /// <param name="s">The siege</param>
        /// <param name="type">The type of round - storm, negotiate, reduction (default)</param>
        public void siegeReductionRound(Siege s, string type = "reduction")
        {
            bool siegeRaised = false;
            Fief besiegedFief = s.getFief();
            Army besieger = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = null;

            // check for sallying army
            if (!String.IsNullOrWhiteSpace(s.defenderAdditional))
            {
                defenderAdditional = s.getDefenderAdditional();

                if (defenderAdditional.aggression > 0)
                {
                    // get odds
                    int battleOdds = this.GetBattleOdds(defenderAdditional, besieger);

                    // if odds OK, give battle
                    if (battleOdds >= defenderAdditional.combatOdds)
                    {
                        // process battle and apply results, if required
                        siegeRaised = this.GiveBattle(defenderAdditional, besieger, circumstance: "siege");

                        // check for disbandment of defenderAdditional and remove from siege if necessary
                        if (!siegeRaised)
                        {
                            if (!besiegedFief.armies.Contains(s.defenderAdditional))
                            {
                                defenderAdditional = null;
                            }
                        }

                    }
                }
            }

            if (siegeRaised)
            {
                // NOTE: if sally was success, siege is ended in Form1.giveBattle
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The defenders have successfully raised the siege!");
                }
            }

            else
            {
                Character defenderLeader = defenderGarrison.GetLeader();
                Character attackerLeader = besieger.GetLeader();

                // process results of siege round
                // reduce keep level by 8%
                double originalKeepLvl = besiegedFief.keepLevel;
                besiegedFief.keepLevel = (besiegedFief.keepLevel * 0.92);

                // apply combat losses to defenderGarrison
                // NOTE: attrition for both sides is calculated in siege.syncDays

                double combatLosses = 0.01;
                uint troopsLost = defenderGarrison.ApplyTroopLosses(combatLosses);

                // check for death of defending PCs/NPCs
                if (defenderLeader != null)
                {
                    bool characterDead = false;

                    // if defenderLeader is PC, check for casualties amongst entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                            characterDead = thisNPC.CalculateCombatInjury(combatLosses);

                            if (characterDead)
                            {
                                // process death
                                (defenderLeader as PlayerCharacter).myNPCs[i].ProcessDeath("injury");
                            }
                        }
                    }

                    // check defenderLeader
                    characterDead = defenderLeader.CalculateCombatInjury(combatLosses);

                    if (characterDead)
                    {
                        // remove as leader
                        defenderGarrison.leader = null;

                        // process death
                        defenderLeader.ProcessDeath("injury");
                    }
                }

                // update total days (NOTE: siege.days will be updated in syncDays)
                s.totalDays += 10;

                // synchronise days
                s.syncDays(s.days - 10);

                if (type.Equals("reduction"))
                {
                    // UPDATE SIEGE LOSSES
                    s.totalCasualtiesDefender += Convert.ToInt32(troopsLost);

                    // =================== construct and send JOURNAL ENTRY
                    // ID
                    uint entryID = Globals_Game.getNextJournalEntryID();

                    // personae
                    List<string> tempPersonae = new List<string>();
                    tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
                    tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
                    if (attackerLeader != null)
                    {
                        tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
                    }
                    if (defenderLeader != null)
                    {
                        tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
                    }
                    // get additional defending leader
                    Character addDefendLeader = null;
                    if (defenderAdditional != null)
                    {
                        addDefendLeader = defenderAdditional.GetLeader();
                        if (addDefendLeader != null)
                        {
                            tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                        }
                    }
                    string[] siegePersonae = tempPersonae.ToArray();

                    // location
                    string siegeLocation = s.getFief().id;

                    // use popup text as description
                    string siegeDescription = "On this day of Our Lord the siege of " + s.getFief().name + " by ";
                    siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                    siegeDescription += " continued.  The besieged garrison lost a total of " + troopsLost + " troops, ";
                    siegeDescription += " and the keep level was reduced from " + originalKeepLvl + " to ";
                    siegeDescription += besiegedFief.keepLevel + ".";

                    // put together new journal entry
                    JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeReduction", loc: siegeLocation, descr: siegeDescription);

                    // add new journal entry to pastEvents
                    Globals_Game.addPastEvent(siegeResult);
                }

                if (type.Equals("storm"))
                {
                    this.siegeStormRound(s, troopsLost, originalKeepLvl);
                }
                else if (type.Equals("negotiation"))
                {
                    this.siegeNegotiationRound(s, troopsLost, originalKeepLvl);
                }
            }

            if (type.Equals("reduction"))
            {
                // refresh screen
                this.refreshCurrentScreen();
            }

        }
        /// <summary>
        /// Allows an attacking army to lay siege to an enemy fief
        /// </summary>
        /// <param name="attacker">The attacking army</param>
        /// <param name="target">The fief to be besieged</param>
        public void siegeStart(Army attacker, Fief target)
        {
            Army defenderGarrison = null;
            Army defenderAdditional = null;

            // check for existence of army in keep
            for (int i = 0; i < target.armies.Count; i++)
            {
                // get army
                Army armyInFief = Globals_Game.armyMasterList[target.armies[i]];

                // check is in keep
                Character armyLeader = armyInFief.GetLeader();
                if (armyLeader != null)
                {
                    if (armyLeader.inKeep)
                    {
                        // check owner is same as that of fief (i.e. can help in siege)
                        if (armyInFief.GetOwner() == target.owner)
                        {
                            defenderAdditional = armyInFief;
                            break;
                        }
                    }
                }
            }

            // create defending force
            defenderGarrison = this.CreateDefendingArmy(target);

            // get the minumum days of all army objects involved
            double minDays = Math.Min(attacker.days, defenderGarrison.days);
            if (defenderAdditional != null)
            {
                minDays = Math.Min(minDays, defenderAdditional.days);
            }

            // get defenderAdditional ID, or null if no defenderAdditional
            string defAddID = null;
            if (defenderAdditional != null)
            {
                defAddID = defenderAdditional.armyID;
            }

            // create siege object
            Siege mySiege = new Siege(Globals_Game.getNextSiegeID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, attacker.GetOwner().charID, target.owner.charID, attacker.armyID, defenderGarrison.armyID, target.id, minDays, target.keepLevel, defAdd: defAddID);

            // add to master list
            Globals_Game.siegeMasterList.Add(mySiege.siegeID, mySiege);

            // add to siege owners
            mySiege.getBesiegingPlayer().mySieges.Add(mySiege.siegeID);
            mySiege.getDefendingPlayer().mySieges.Add(mySiege.siegeID);

            // add to fief
            target.siege = mySiege.siegeID;

            // reduce expenditures in fief, except for garrison
            target.infrastructureSpendNext = 0;
            target.keepSpendNext = 0;
            target.officialsSpendNext = 0;

            // update days (NOTE: siege.days will be updated in syncDays)
            mySiege.totalDays++;

            // sychronise days
            mySiege.syncDays(mySiege.days - 1);

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(mySiege.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(mySiege.getBesiegingPlayer().charID + "|attackerOwner");
            tempPersonae.Add(attacker.GetLeader().charID + "|attackerLeader");
            // get defenderLeader
            Character defenderLeader = defenderGarrison.GetLeader();
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.GetLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = mySiege.getFief().id;

            // description
            string siegeDescription = "On this day of Our Lord the forces of ";
            siegeDescription += mySiege.getBesiegingPlayer().firstName + " " + mySiege.getBesiegingPlayer().familyName;
            siegeDescription += ", led by " + attacker.GetLeader().firstName + " " + attacker.GetLeader().familyName;
            siegeDescription += " laid siege to the keep of " + mySiege.getFief().name;
            siegeDescription += ", owned by " + mySiege.getDefendingPlayer().firstName + " " + mySiege.getDefendingPlayer().familyName;
            if (defenderLeader != null)
            {
                siegeDescription += ". The defending garrison is led by " + defenderLeader.firstName + " " + defenderLeader.familyName;
            }
            if (addDefendLeader != null)
            {
                siegeDescription += ". Additional defending forces are led by " + addDefendLeader.firstName + " " + addDefendLeader.familyName;
            }
            siegeDescription += ".";

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siege", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // display siege in siege screen
            this.refreshSiegeContainer(mySiege);
        }
    }
}
