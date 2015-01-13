using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace hist_mmorpg 
{
    /// <summary>
    /// Class storing data on army
    /// </summary>
    public class Army 
    {
		/// <summary>
		/// Holds army ID
		/// </summary>
		public String armyID { get; set; }
        /// <summary>
        /// Holds troops in army
        /// 0 = knights
        /// 1 = menAtArms
        /// 2 = lightCav
        /// 3 = longbowmen
        /// 4 = crossbowmen
        /// 5 = foot
        /// 6 = rabble
        /// </summary>
        public uint[] troops = new uint[7] {0, 0, 0, 0, 0, 0, 0};
        /// <summary>
        /// Holds army leader (ID)
        /// </summary>
        public string leader { get; set; }
        /// <summary>
        /// Holds army owner (ID)
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// Holds army's remaining days in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds army location (fiefID)
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// Indicates whether army is being actively maintained by owner
        /// </summary>
        public bool isMaintained { get; set; }
        /// <summary>
        /// Indicates army's aggression level (automated response to combat)
        /// </summary>
        public byte aggression { get; set; }
        /// <summary>
        /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
        /// </summary>
        public byte combatOdds { get; set; }

        /// <summary>
        /// Constructor for Army
        /// </summary>
		/// <param name="id">String holding ID of army</param>
        /// <param name="ldr">string holding ID of army leader</param>
        /// <param name="own">string holding ID of army owner</param>
        /// <param name="day">double holding remaining days in season for army</param>
        /// <param name="loc">string holding army location (fiefID)</param>
        /// <param name="maint">bool indicating whether army is being actively maintained by owner</param>
        /// <param name="aggr">byte indicating army's aggression level</param>
        /// <param name="odds">byte indicating army's combat odds value</param>
        /// <param name="trp">uint[] holding troops in army</param>
        public Army(String id, string ldr, string own, double day, string loc, bool maint = false, byte aggr = 1, byte odds = 9, uint[] trp = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Utility_Methods.firstCharToUpper(id.Trim());

            if (!Utility_Methods.validateArmyID(id))
            {
                throw new InvalidDataException("Army ID must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // LDR
            if (!String.IsNullOrWhiteSpace(ldr))
            {
                // trim and ensure 1st is uppercase
                ldr = Utility_Methods.firstCharToUpper(ldr.Trim());

                if (!Utility_Methods.validateCharacterID(ldr))
                {
                    throw new InvalidDataException("Army leader ID must have the format 'Char_' followed by some numbers");
                }
            }

            // OWN
            // trim and ensure 1st is uppercase
            own = Utility_Methods.firstCharToUpper(own.Trim());

            if (!Utility_Methods.validateCharacterID(own))
            {
                throw new InvalidDataException("Army owner id must have the format 'Char_' followed by some numbers");
            }

            // DAY
            if (!Utility_Methods.validateDays(day))
            {
                throw new InvalidDataException("Army days must be a double between 0-109");
            }

            // LOC
            // trim and ensure is uppercase
            loc = loc.Trim().ToUpper();

            if (!Utility_Methods.validatePlaceID(loc))
            {
                throw new InvalidDataException("Army location id must be 5 characters long, start with a letter, and end in at least 2 numbers");
            }

            // AGGR
            // check is < 3
            if (aggr > 2)
            {
                throw new InvalidDataException("Army aggression level must be a byte less than 3");
            }

            this.armyID = id;
            this.leader = ldr;
            this.owner = own;
            this.days = day;
            this.location = loc;
            this.isMaintained = maint;
            this.aggression = aggr;
            this.combatOdds = odds;
            if (trp != null)
            {
                this.troops = trp;
            }
        }

        /// <summary>
        /// Constructor for Army taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Army()
		{
		}

        /// <summary>
        /// Performs functions associated with creating a new army
        /// </summary>
        public void addArmy()
        {
            // get leader
            Character armyLeader = this.getLeader();

            // get owner
            PlayerCharacter armyOwner = this.getOwner();

            // get location
            Fief armyLocation = this.getLocation();

            // add to armyMasterList
            Globals_Game.armyMasterList.Add(this.armyID, this);

            // add to owner's myArmies
            armyOwner.myArmies.Add(this);

            // add to leader
            if (armyLeader != null)
            {
                armyLeader.armyID = this.armyID;
            }

            // add to fief's armies
            armyLocation.armies.Add(this.armyID);

        }

        /// <summary>
        /// Maintains the specified field army
        /// </summary>
        public void mantainArmy()
        {
            string toDisplay = "";

            // get cost
            uint maintCost = this.calcArmySize() * 500;

            // get available treasury
            Fief homeFief = this.getOwner().getHomeFief();
            int availTreas = homeFief.getAvailableTreasury();

            // check if army is already maintained
            if (!this.isMaintained)
            {
                // check if can afford maintenance
                if (maintCost > availTreas)
                {
                    // display 'no' message
                    toDisplay += "Sorry, milord, to maintain this army would cost £" + maintCost + "\r\n";
                    toDisplay += "and you only have £" + availTreas + " available in the home treasury.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
                else
                {
                    // present alternative number and ask for confirmation
                    toDisplay = "It will cost £" + maintCost + " to maintain this army";
                    toDisplay += " and you have £" + homeFief.treasury + " in your home treasury.\r\n\r\n";
                    toDisplay += "Do you wish to proceed?";
                    DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with operation?", MessageBoxButtons.OKCancel);

                    // if choose to proceed
                    if (dialogResult != DialogResult.Cancel)
                    {
                        // set isMaintained
                        this.isMaintained = true;

                        // deduct funds from treasury
                        homeFief.treasury -= Convert.ToInt32(maintCost);

                        // display confirmation message
                        toDisplay = "Army maintained at a cost of £" + maintCost + ".";
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(toDisplay);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the army's aggression and combatOdds values
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newAggroLevel">The new aggression value</param>
        /// <param name="newOddsValue">The new combatOdds value</param>
        public bool adjustAutoLevels(byte newAggroLevel, byte newOddsValue)
        {
            bool success = true;

            // check values and alter if appropriate
            if (newAggroLevel < 0)
            {
                newAggroLevel = 0;
            }
            else if (newAggroLevel > 2)
            {
                newAggroLevel = 2;
            }
            if (newOddsValue < 0)
            {
                newOddsValue = 0;
            }
            else if (newOddsValue > 9)
            {
                newOddsValue = 9;
            }

            // update army's values
            this.aggression = newAggroLevel;
            this.combatOdds = newOddsValue;

            if ((this.aggression != newAggroLevel) || (this.combatOdds != newOddsValue))
            {
                success = false;
            }

            return success;
        }
		
        /// <summary>
        /// Assigns a new leader to the army
        /// NOTE: you CAN assign a null character as leader (i.e. the army becomes leaderless)
        /// </summary>
        /// <param name="newLeader">The new leader (can be null)</param>
        public void assignNewLeader(Character newLeader)
        {
            // check if army is involved in a siege
            Siege mySiege = this.getSiege();

            // Remove army from current leader
            Character oldLeader = this.getLeader();
            if (oldLeader != null)
            {
                oldLeader.armyID = null;
            }

            // if no new leader (i.e. if just removing old leader)
            if (newLeader == null)
            {
                // in army, set new leader
                this.leader = null;
            }

            // if is new leader
            else
            {
                // add army to new leader
                newLeader.armyID = this.armyID;

                // in army, set new leader
                this.leader = newLeader.charID;

                // if new leader is NPC, remove from player's entourage
                if (newLeader is NonPlayerCharacter)
                {
                    (newLeader as NonPlayerCharacter).inEntourage = false;
                }

                // calculate days synchronisation
                double minDays = Math.Min(newLeader.days, this.days);
                double maxDays = Math.Max(newLeader.days, this.days);
                double difference = maxDays - minDays;

                if (newLeader.days != minDays)
                {
                    // synchronise days
                    newLeader.adjustDays(difference);
                }
                else
                {
                    // if army not involved in siege, check for attrition in normal way
                    if (mySiege == null)
                    {
                        byte attritionChecks = 0;
                        attritionChecks = Convert.ToByte(difference / 7);

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = this.calcAttrition();
                            // apply attrition
                            this.applyTroopLosses(attritionModifer);
                        }
                    }

                    // if army is involved in siege, attrition applied at siege level
                    else
                    {
                        mySiege.syncDays(newLeader.days);
                    }

                }

            }
        }

        /// <summary>
        /// Calculates total army size
        /// </summary>
        /// <returns>uint containing army size</returns>
        public uint calcArmySize()
        {
            uint armySize = 0;

            foreach (uint troopType in this.troops)
            {
                armySize += troopType;
            }

            return armySize;
        }

        /// <summary>
        /// Moves army to another fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="showAttrition">bool indicating whether to display message containing attrition losses</param>
        public bool moveArmy(bool showAttrition = false)
        {
            bool success = false;

            // get leader
            Character myLeader = this.getLeader();

            // get old fief
            Fief myOldFief = Globals_Game.fiefMasterList[this.location];
            // get new fief
            Fief myNewFief = Globals_Game.fiefMasterList[myLeader.location.id];

            // remove from old fief
            myOldFief.removeArmy(this.armyID);

            // add to new fief
            myNewFief.addArmy(this.armyID);

            // change location
            this.location = myLeader.location.id;

            // update days
            this.days = myLeader.days;

            // calculate attrition
            double attritionModifer = this.calcAttrition();
            // apply attrition
            uint troopsLost = this.applyTroopLosses(attritionModifer);

            // inform player of losses
            if (showAttrition)
            {
                if (troopsLost > 0)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Your army (" + this.armyID + ") has lost " + troopsLost + " from attrition in " + myNewFief.name);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Calculates movement modifier for the army
        /// </summary>
        /// <returns>uint containing movement modifier</returns>
        public uint calcMovementModifier()
        {
            uint movementMod = 1;

            // generate random double (0-100)
            Double myRandomDouble = Globals_Game.myRand.NextDouble() * 100;

            // calculate chance of modifier based on army size
            Double modifierChance = (Math.Floor(this.calcArmySize() / (Double)1000) * 3);

            // check to see if modifier required
            if (myRandomDouble <= modifierChance)
            {
                movementMod = 3;
            }

            return movementMod;
        }

        /// <summary>
        /// Calculates attrition for the army
        /// </summary>
        /// <returns>double containing casualty modifier to be applied troops</returns>
        public double calcAttrition()
        {
            uint troopNumbers = this.calcArmySize();
            double casualtyModifier = 0;
            Double attritionChance = 0;
            String toDisplay = "";

            // ensure is no attrition if army maintained
            if (!this.isMaintained)
            {
                // get fief
                Fief currentFief = this.getLocation();

                // get leader
                Character myLeader = this.getLeader();

                // calculate base chance of attrition
                attritionChance = (troopNumbers / Convert.ToDouble(currentFief.population)) * 100;
                toDisplay += "Base chance: " + attritionChance + "\r\n";

                // factor in effect of leader (need to check if army has leader)
                if (myLeader != null)
                {
                    // apply effect of leader
                    attritionChance = attritionChance - ((myLeader.calculateStature() + myLeader.management) / 2);
                    toDisplay += "Leader effect: " + (myLeader.calculateStature() + myLeader.management) / 2 + "\r\n";
                }

                // factor in effect of season (add 20 if is winter or spring)
                if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                {
                    attritionChance = attritionChance + 20;
                    toDisplay += "Season effect: 20\r\n";
                }

                // normalise chance of attrition
                if (attritionChance < 10)
                {
                    attritionChance = 10;
                }
                else if (attritionChance > 100)
                {
                    attritionChance = 100;
                }

                // generate random number (0-100) to check if attrition occurs
                Double randomPercent = Globals_Game.myRand.NextDouble() * 100;

                // check if attrition occurs
                if (randomPercent <= attritionChance)
                {
                    // calculate base casualtyModifier
                    casualtyModifier = (troopNumbers / Convert.ToDouble(currentFief.population)) / 10;
                    toDisplay += "casualtyModifier: " + casualtyModifier + "\r\n";

                    // factor in effect of season on potential losses (* 3 if is winter or spring)
                    if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                    {
                        casualtyModifier = casualtyModifier * 3;
                        toDisplay += "casualtyModifier after seasonal effect: " + casualtyModifier + "\r\n";
                    }

                }
            }

            if (casualtyModifier > 0)
            {
                if (Globals_Client.showDebugMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay);
                }
            }

            return casualtyModifier;
        }

        /// <summary>
        /// Applies troop losses after attrition, battle, siege, etc.
        /// </summary>
        /// <returns>uint containing total number of troops lost</returns>
        /// <param name="lossModifier">modifier to be applied to each troop type</param>
        public uint applyTroopLosses(double lossModifier)
        {
            // keep track of total troops lost
            uint troopsLost = 0;

            for (int i = 0; i < this.troops.Length; i++ )
            {
                uint thisTypeLost = Convert.ToUInt32(this.troops[i] * lossModifier);
                troopsLost += thisTypeLost;
                this.troops[i] -= thisTypeLost;
            }

            return troopsLost;
        }

        /// <summary>
        /// Creates a detachment from the army's troops and leaves it in the fief
        /// </summary>
        /// <returns>bool indicating success of transfer</returns>
        /// <param name="details">string[] containing troop numbers and recipient (ID)</param>
        public bool createDetachment(string[] details)
        {
            bool proceed = true;
            bool adjustDays = true;
            int daysTaken = 0;
            uint totalTroopsToTransfer = 0;
            string toDisplay = "";
            string[] troopTypeLabels = new string[] { "knights", "men-at-arms", "light cavalry", "longbowmen", "crossbowmen", "foot", "rabble" };
            Character myLeader = null;

            // carry out CONDITIONAL CHECKS

            // 1. check arry length
            if (details.Length != 8)
            {
                proceed = false;
                adjustDays = false;
                toDisplay = "Not enough data parts in transfer details array.";
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                }
            }
            else
            {
                // 2. check each troop type; if not enough in army, cancel
                for (int i = 0; i < details.Length - 1; i++)
                {
                    if (Convert.ToUInt32(details[i]) > this.troops[i])
                    {
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "You don't have enough " + troopTypeLabels[i] + " in your army for that transfer.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                        }
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        totalTroopsToTransfer += Convert.ToUInt32(details[i]);
                    }
                }

                if (proceed)
                {
                    // 3. if no troops selected for transfer, cancel
                    if (totalTroopsToTransfer < 1)
                    {
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "You haven't selected any troops for transfer.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                        }
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        // 4. check have minimum days necessary for transfer
                        if (Globals_Client.armyToView.days < 10)
                        {
                            if (Globals_Client.showMessages)
                            {
                                toDisplay = "You don't have enough days left for this transfer.";
                                System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                            }
                            proceed = false;
                            adjustDays = false;
                        }
                        else
                        {
                            // 5. check if have enough days for transfer in this instance

                            // calculate time taken for transfer
                            daysTaken = Globals_Game.myRand.Next(10, 31);

                            if (daysTaken > Globals_Client.armyToView.days)
                            {
                                if (Globals_Client.showMessages)
                                {
                                    toDisplay = "Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.";
                                    System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                }
                                proceed = false;
                            }
                            else
                            {
                                // 6. check transfer recipient exists
                                if (!Globals_Game.pcMasterList.ContainsKey(details[7]))
                                {
                                    if (Globals_Client.showMessages)
                                    {
                                        toDisplay = "Cannot identify transfer recipient.";
                                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                    }
                                    proceed = false;
                                    adjustDays = false;
                                }
                                else
                                {
                                    // 7. check army has a leader

                                    // get leader
                                    myLeader = this.getLeader();

                                    if (myLeader == null)
                                    {
                                        if (Globals_Client.showMessages)
                                        {
                                            toDisplay = "An army without a leader cannot make a transfer.";
                                            System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                        }
                                        proceed = false;
                                        adjustDays = false;
                                    }
                                    else
                                    {
                                        // 8. if reduces army to < 100 troops, display warning
                                        if (((this.calcArmySize() - totalTroopsToTransfer) < 100) && (proceed))
                                        {
                                            toDisplay = "This transfer will reduce your army manpower to dangerous levels.  Click OK to proceed.";
                                            DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with transfer?", MessageBoxButtons.OKCancel);

                                            // if choose to cancel
                                            if (dialogResult == DialogResult.Cancel)
                                            {
                                                if (Globals_Client.showMessages)
                                                {
                                                    toDisplay = "Transfer cancelled.";
                                                    System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                                }
                                                proceed = false;
                                                adjustDays = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // remove troops from army
                for (int i = 0; i < this.troops.Length; i++)
                {
                    this.troops[i] -= Convert.ToUInt32(details[i]);
                }

                // get fief
                Fief thisFief = this.getLocation();

                // create transfer entry
                string[] thisTransfer = new string[10] { this.owner, details[7], details[0], details[1], details[2],
                            details[3], details[4], details[5], details[6], (this.days - daysTaken).ToString() };

                // add to fief's troopTransfers list
                string tranferID = Globals_Game.getNextDetachmentID();
                thisFief.troopTransfers.Add(tranferID, thisTransfer);

                // check detachment added to troopTransfers
                if (!thisFief.troopTransfers.ContainsKey(tranferID))
                {
                    proceed = false;
                }
            }

            if (adjustDays)
            {
                // adjust days
                myLeader.adjustDays(daysTaken);

                // calculate possible attrition for army
                byte attritionChecks = Convert.ToByte(daysTaken / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = this.calcAttrition();

                    // apply attrition
                    if (attritionModifer > 0)
                    {
                        this.applyTroopLosses(attritionModifer);
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Calculates the army's combat value for a combat engagement (NOTE: doesn't include leadership modifier)
        /// </summary>
        /// <returns>double containing combat value</returns>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        public double calculateCombatValue(int keepLvl = 0)
        {
            double cv = 0;

            // get leader and owner
            Character myLeader = this.getLeader();
            PlayerCharacter myOwner = this.getOwner();

            // get nationality (effects combat values)
            string troopNationality = myOwner.nationality.natID;

            // get combat values for that nationality
            uint[] thisCombatValues = Globals_Server.combatValues[troopNationality];

            // get CV for each troop type
            for (int i = 0; i < this.troops.Length; i++)
            {
                cv += this.troops[i] * thisCombatValues[i];
            }

            // if calculating defender during keep storm, account for keep level
            // (1000 foot per level)
            if (keepLvl > 0)
            {
                cv += (keepLvl * 1000) * thisCombatValues[5];
            }

            // get leader's combat value
            if (myLeader != null)
            {
                cv += myLeader.getCombatValue();

                // if leader is PC, get CV of entourage (male characters only)
                if (myLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (myLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        if ((myLeader as PlayerCharacter).myNPCs[i].inEntourage)
                        {
                            if ((myLeader as PlayerCharacter).myNPCs[i].isMale)
                            {
                                cv += (myLeader as PlayerCharacter).myNPCs[i].getCombatValue();
                            }
                        }
                    }
                }
            }

            return cv;
        }

        /// <summary>
        /// Calculates the estimated number of troops of all types in the army
        /// </summary>
        /// <returns>uint[] containing estimated troop numbers for all types</returns>
        /// <param name="observer">The character making the estimate</param>
        public uint[] getTroopsEstimate(Character observer)
        {
            uint[] troopNumbers = new uint[7] {0, 0, 0, 0, 0, 0, 0};

            // get random int (0-2) to decide whether to over- or under-estimate troop number
            int overUnder = Globals_Game.myRand.Next(3);

            // get observer's estimate variance (based on his leadership value)
            double estimateVariance = observer.getEstimateVariance();

            // perform estimate for each troop type
            for (int i = 0; i < troopNumbers.Length; i++)
            {
                // get troop number upon which to base estimate
                troopNumbers[i] = this.troops[i];

                // generate random double between 0 and estimate variance to decide variance in this case
                double thisVariance = Utility_Methods.GetRandomDouble(estimateVariance);

                // apply variance (negatively or positively) to troop number
                // 0 = under-estimate, 1-2 = over-estimate
                if (overUnder == 0)
                {
                    troopNumbers[i] = troopNumbers[i] - Convert.ToUInt32(troopNumbers[i] * thisVariance);
                }
                else
                {
                    troopNumbers[i] = troopNumbers[i] + Convert.ToUInt32(troopNumbers[i] * thisVariance);
                }
            }

            return troopNumbers;
        }

        /// <summary>
        /// Gets the army's location (fief)
        /// </summary>
        /// <returns>the fief</returns>
        public Fief getLocation()
        {
            Fief thisFief = null;

            if (!String.IsNullOrWhiteSpace(this.location))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.location))
                {
                    thisFief = Globals_Game.fiefMasterList[this.location];
                }
            }

            return thisFief;
        }

        /// <summary>
        /// Gets the army's owner
        /// </summary>
        /// <returns>the owner</returns>
        public PlayerCharacter getOwner()
        {
            PlayerCharacter myOwner = null;

            // get leader from PC master list
            if (!String.IsNullOrWhiteSpace(this.owner))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.owner))
                {
                    myOwner = Globals_Game.pcMasterList[this.owner];
                }
            }

            return myOwner;
        }

        /// <summary>
        /// Gets the army's leader
        /// </summary>
        /// <returns>the leader</returns>
        public Character getLeader()
        {
            Character myLeader = null;

            if (!String.IsNullOrWhiteSpace(this.leader))
            {
                // get leader from appropriate master list
                if (Globals_Game.npcMasterList.ContainsKey(this.leader))
                {
                    myLeader = Globals_Game.npcMasterList[this.leader];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(this.leader))
                {
                    myLeader = Globals_Game.pcMasterList[this.leader];
                }
            }

            return myLeader;
        }

        /// <summary>
        /// Performs functions associated with army move for an army unaccompanied by a leader 
        /// </summary>
        /// <param name="target">The fief to move to</param>
        /// <param name="travelCost">The cost of moving to target fief</param>
        public void moveWithoutLeader(Fief target, double travelCost)
        {
            // get current location
            Fief from = this.getLocation();

            // remove from current fief
            from.armies.Remove(this.armyID);

            // add to target fief
            target.armies.Add(this.armyID);

            // change location
            this.location = target.id;

            // change days
            this.days = this.days - travelCost;

            // calculate attrition
            double attritionModifer = this.calcAttrition();
            // apply attrition
            uint troopsLost = this.applyTroopLosses(attritionModifer);
        }

        /// <summary>
        /// Checks to see if army is besieging a fief/keep
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string checkIfBesieger()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.getLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.getSiege();

                // check if this army is besieging army
                if (thisSiege.getBesiegingArmy() == this)
                {
                    thisSiegeID = thisFief.siege;
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army is the defending garrison in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string checkIfSiegeDefenderGarrison()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.getLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.getSiege();
                if (thisSiege.getDefenderGarrison() == this)
                {
                    thisSiegeID = thisFief.siege;
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army is an additional defending army in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string checkIfSiegeDefenderAdditional()
        {
            string thisSiegeID = null;

            // get fief
            Fief thisFief = this.getLocation();

            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                Siege thisSiege = thisFief.getSiege();
                if (thisSiege.defenderAdditional != null)
                {
                    if (thisSiege.getDefenderAdditional() == this)
                    {
                        thisSiegeID = thisFief.siege;
                    }
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Checks to see if army has any role (defending or besieging) in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public string checkForSiegeRole()
        {
            string thisSiegeID = null;

            // check if army is a defending garrison in a siege
            thisSiegeID = this.checkIfSiegeDefenderGarrison();

            if (String.IsNullOrWhiteSpace(thisSiegeID))
            {
                // check if army is an additional defending army in a siege
                thisSiegeID = this.checkIfSiegeDefenderAdditional();

                if (String.IsNullOrWhiteSpace(thisSiegeID))
                {
                    // check if army is besieger in a siege
                    thisSiegeID = this.checkIfBesieger();
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Gets the siege object associated with the siegeID provided
        /// </summary>
        /// <returns>The siege</returns>
        public Siege getSiege()
        {
            Siege thisSiege = null;

            // check for siege ID associated with army
            string siegeID = this.checkForSiegeRole();

            // get siege
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                if (Globals_Game.siegeMasterList.ContainsKey(siegeID))
                {
                    thisSiege = Globals_Game.siegeMasterList[siegeID];
                }
            }

            return thisSiege;
        }
        
        /// <summary>
        /// Updates army data at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating if army has dissolved</returns>
        public bool updateArmy()
        {
            bool hasDissolved = false;
            bool attritionApplies = true;
            string siegeID = null;
            bool isSiegeDefGarr = false;
            bool isSiegeDefAdd = false;
            Siege thisSiege = null;

            // check for SIEGE INVOLVEMENT
            // check that army is a defending garrison in a siege
            siegeID = this.checkIfSiegeDefenderGarrison();
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                isSiegeDefGarr = true;
                thisSiege = this.getSiege();
            }
            else
            {
                siegeID = this.checkIfSiegeDefenderAdditional();
                // check that army is an additional defending army in a siege
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    isSiegeDefAdd = true;
                    thisSiege = this.getSiege();
                }
            }

            // check to see if attrition applies to defending forces in siege
            // (based on besieged fief bailiff management rating)
            if ((isSiegeDefGarr) || (isSiegeDefAdd))
            {
                attritionApplies = thisSiege.checkAttritionApplies();
            }

            // get leader
            Character myLeader = this.getLeader();

            if (attritionApplies)
            {
                // check for attrition due to days remaining
                byte attritionChecks = Convert.ToByte(this.days / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = this.calcAttrition();
                    // apply attrition
                    uint troopsLost = this.applyTroopLosses(attritionModifer);

                    // update siege losses, if applicable
                    if (thisSiege != null)
                    {
                        // siege defenders
                        if ((isSiegeDefGarr) || (isSiegeDefAdd))
                        {
                            thisSiege.totalCasualtiesDefender += Convert.ToInt32(troopsLost);
                        }

                        // siege attackers
                        else
                        {
                            thisSiege.totalCasualtiesAttacker += Convert.ToInt32(troopsLost);
                        }
                    }
                }
            }

            if (!isSiegeDefGarr)
            {
                // check if army dissolves (less than 100 men)
                // NOTE: defending garrisons do not dissolve in this way
                if (this.calcArmySize() < 100)
                {
                    hasDissolved = true;
                }
            }

            // update army days
            if (!hasDissolved)
            {
                if (myLeader != null)
                {
                    this.days = myLeader.days;
                }
                else
                {
                    this.days = 90;
                }
            }

            // reset isMaintained
            this.isMaintained = false;

            return hasDissolved;
        }

        /// <summary>
        /// Runs conditional checks prior to the army launching an attack on another army
        /// </summary>
        /// <returns>bool indicating whether attack can proceed</returns>
        /// <param name="targetArmy">The army to be attacked</param>
        public bool checksBeforeAttack(Army targetArmy)
        {
            bool proceed = true;
            string toDisplay = "";

            // check has enough days to give battle (1)
            if (this.days < 1)
            {
                if (Globals_Client.showMessages)
                {
                    toDisplay = "Your army doesn't have enough days left to give battle.";
                    System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                    proceed = false;
                }
            }
            else
            {
                // SIEGE INVOLVEMENT
                // check if defending army is the garrison in a siege
                string siegeID = targetArmy.checkIfSiegeDefenderGarrison();
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    if (Globals_Client.showMessages)
                    {
                        toDisplay = "The defending army is currently being besieged and cannot be attacked.";
                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                        proceed = false;
                    }
                }

                else
                {
                    // check if defending army is the additional defender in a siege
                    siegeID = targetArmy.checkIfSiegeDefenderAdditional();
                    if (!String.IsNullOrWhiteSpace(siegeID))
                    {
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "The defending army is currently being besieged and cannot be attacked.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                            proceed = false;
                        }
                    }

                    else
                    {
                        // check if are attacking your own army
                        if (this.getOwner() == targetArmy.getOwner())
                        {
                            if (Globals_Client.showMessages)
                            {
                                toDisplay = "You cannot attack one of your own armies.";
                                System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                proceed = false;
                            }
                        }

                        else
                        {
                            // check if attacking king's army
                            if (this.getOwner() == this.getOwner().getKing())
                            {
                                // display warning and get decision
                                DialogResult dialogResult = MessageBox.Show("You are about to attack an army owned by your king.\r\nClick 'OK' to proceed.", "Proceed with attack?", MessageBoxButtons.OKCancel);

                                // if choose to cancel
                                if (dialogResult == DialogResult.Cancel)
                                {
                                    if (Globals_Client.showMessages)
                                    {
                                        toDisplay = "Attack cancelled.";
                                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION CANCELLED");
                                        proceed = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Disbands the army
        /// </summary>
        public void disbandArmy()
        {
            // check for siege involvement
            string siegeID = this.checkForSiegeRole();
            Siege thisSiege = null;
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                thisSiege = Globals_Game.siegeMasterList[siegeID];
            }

            // remove from siege
            if (thisSiege != null)
            {
                // check if are additional defending army
                string whichRole = this.checkIfSiegeDefenderAdditional();
                if (!String.IsNullOrWhiteSpace(whichRole))
                {
                    thisSiege.defenderAdditional = null;
                }

                // check if are besieging army
                else
                {
                    whichRole = this.checkIfBesieger();
                    if (!String.IsNullOrWhiteSpace(whichRole))
                    {
                        thisSiege.besiegerArmy = null;
                    }
                }
            }

            // remove from fief
            Fief thisFief = this.getLocation();
            thisFief.armies.Remove(this.armyID);

            // remove from owner
            PlayerCharacter thisOwner = this.getOwner();
            thisOwner.myArmies.Remove(this);

            // remove from leader
            Character thisLeader = this.getLeader();
            if (thisLeader != null)
            {
                thisLeader.armyID = null;
            }

            // remove from armyMasterList
            Globals_Game.armyMasterList.Remove(this.armyID);
        }


    }
}
