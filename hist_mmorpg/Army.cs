using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
        /// 3 = yeomen
        /// 4 = foot
        /// 5 = rabble
        /// </summary>
        public uint[] troops = new uint[6] {0, 0, 0, 0, 0, 0};
        /* /// <summary>
        /// Holds no. knights in army
        /// </summary>
        public uint knights { get; set; }
        /// <summary>
        /// Holds no. men at arms in army
        /// </summary>
        public uint menAtArms { get; set; }
        /// <summary>
        /// Holds no. light cavalry in army
        /// </summary>
        public uint lightCavalry { get; set; }
        /// <summary>
        /// Holds no. yeomen in army
        /// </summary>
        public uint yeomen { get; set; }
        /// <summary>
        /// Holds no. foot soldiers in army
        /// </summary>
        public uint foot { get; set; }
        /// <summary>
        /// Holds no. rabble in army
        /// </summary>
        public uint rabble { get; set; } */
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
        /// Holds army's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }
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
        /// <param name="cl">GameClock holding season</param>
        /// <param name="loc">string holding army location (fiefID)</param>
        /// <param name="maint">bool indicating whether army is being actively maintained by owner</param>
        /// <param name="aggr">byte indicating army's aggression level</param>
        /// <param name="odds">byte indicating army's combat odds value</param>
        /// <param name="trp">uint[] holding troops in army</param>
        public Army(String id, string ldr, string own, double day, GameClock cl, string loc, bool maint = false, byte aggr = 1, byte odds = 9, uint[] trp = null)
        {

            // TODO: validate trp = (upper limit?)
            // TODO: validate ldr ID = 1-10000?

            // validate day > 90
            if ((day > 90) || (day < 0))
            {
                throw new InvalidDataException("Army remaining days must be an integer between 0 and 90");
            }

			this.armyID = id;
            this.leader = ldr;
            this.owner = own;
            this.days = day;
            this.clock = cl;
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
        /// For use when de-serialising from Riak
        /// </summary>
        public Army()
		{
		}
		
        /// <summary>
        /// Calculates travel modifier for army size
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public double calcArmyTravMod()
        {
            double travelModifier = 0;

            travelModifier = (this.calcArmySize() / 1000) * 0.25;

            return travelModifier;
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
            Fief myOldFief = Globals.fiefMasterList[this.location];
            // get new fief
            Fief myNewFief = Globals.fiefMasterList[myLeader.location.fiefID];

            // remove from old fief
            myOldFief.removeArmy(this.armyID);

            // add to new fief
            myNewFief.addArmy(this.armyID);

            // change location
            this.location = myLeader.location.fiefID;

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
                    System.Windows.Forms.MessageBox.Show("Your army (" + this.armyID + ") has lost " + troopsLost + " from attrition in " + myNewFief.name);
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
            Double myRandomDouble = Globals.myRand.NextDouble() * 100;

            // calculate chance of modifier based on army size
            Double modifierChance = Math.Floor(this.calcArmySize() / (Double)1000);

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
        public double calcAttrition(uint troopNumbers = 0)
        {
            double casualtyModifier = 0;
            Double attritionChance = 0;
            String toDisplay = "";

            // if no specific troop number passed in, use army size
            if (troopNumbers == 0)
            {
                troopNumbers = this.calcArmySize();
            }

            // get fief
            Fief currentFief = Globals.fiefMasterList[this.location];

            // get leader
            Character myLeader = this.getLeader();

            // calculate base chance of attrition
            attritionChance = (troopNumbers / Convert.ToDouble(currentFief.population)) * 100;
            toDisplay += "Base chance: " + attritionChance + "\r\n";

            // factor in effect of leader
            attritionChance = attritionChance - ((myLeader.calculateStature(true) + myLeader.management) / 2);
            toDisplay += "Leader effect: " + (myLeader.calculateStature(true) + myLeader.management) / 2 + "\r\n";

            // factor in effect of season (add 20 if is winter or spring)
            if ((this.clock.currentSeason == 0) || (this.clock.currentSeason == 3))
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
            Double randomPercent = Globals.myRand.NextDouble() * 100;

            // check if attrition occurs
            if (randomPercent <= attritionChance)
            {
                // calculate base casualtyModifier
                casualtyModifier = (troopNumbers / Convert.ToDouble(currentFief.population)) / 10;
                toDisplay += "casualtyModifier: " + casualtyModifier + "\r\n";

                // factor in effect of season on potential losses (* 3 if is winter or spring)
                if ((this.clock.currentSeason == 0) || (this.clock.currentSeason == 3))
                {
                    casualtyModifier = casualtyModifier * 3;
                    toDisplay += "casualtyModifier after seasonal effect: " + casualtyModifier + "\r\n";
                }

            }

            if (casualtyModifier > 0)
            {
                System.Windows.Forms.MessageBox.Show(toDisplay);
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
        /// Calculates the army's combat value for a combat engagement
        /// </summary>
        /// <returns>double containing combat value</returns>
        public double calculateCombatValue()
        {
            double cv = 0;

            // get leader
            Character myLeader = this.getLeader();

            // get nationality (effects combat values)
            string troopNationality = "";
            if (myLeader.nationality.ToUpper().Equals("E"))
            {
                troopNationality = "E";
            }
            else
            {
                troopNationality = "O";
            }

            // get combat values for that nationality
            uint[] thisCombatValues = Globals.combatValues[troopNationality];

            // get CV for each troop type
            for (int i = 0; i < this.troops.Length; i++)
            {
                cv += this.troops[i] * thisCombatValues[i];
            }

            // get leader's CV
            cv += myLeader.getCombatValue();

            // if leader is PC, get CV of entourage
            if (myLeader is PlayerCharacter)
            {
                for (int i = 0; i < (myLeader as PlayerCharacter).myNPCs.Count; i++ )
                {
                    cv += (myLeader as PlayerCharacter).myNPCs[i].getCombatValue();
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
            uint[] troopNumbers = new uint[6] {0, 0, 0, 0, 0, 0};

            // get random int (0-2) to decide whether to over- or under-estimate troop number
            int overUnder = Globals.myRand.Next(3);

            // get observer's estimate variance (based on his leadership value)
            double estimateVariance = observer.getEstimateVariance();

            // perform estimate for each troop type
            for (int i = 0; i < troopNumbers.Length; i++)
            {
                // get troop number upon which to base estimate
                troopNumbers[i] = this.troops[i];

                // generate random double between 0 and estimate variance to decide variance in this case
                double thisVariance = Globals.GetRandomDouble(estimateVariance);

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
        /// Gets the army's owner
        /// </summary>
        /// <returns>the owner</returns>
        public PlayerCharacter getOwner()
        {
            PlayerCharacter myOwner = null;

            // get leader from PC master list
            myOwner = Globals.pcMasterList[this.owner];

            return myOwner;
        }

        /// <summary>
        /// Gets the army's leader
        /// </summary>
        /// <returns>the leader</returns>
        public Character getLeader()
        {
            Character myLeader = null;

            // get leader from appropriate master list
            if (Globals.npcMasterList.ContainsKey(this.leader))
            {
                myLeader = Globals.npcMasterList[this.leader];
            }
            else if (Globals.pcMasterList.ContainsKey(this.leader))
            {
                myLeader = Globals.pcMasterList[this.leader];
            }

            return myLeader;
        }
        
        /// <summary>
        /// Updates army data at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating if army has dissolved</returns>
        public bool updateArmy()
        {
            bool hasDissolved = false;

            // get leader
            Character myLeader = this.getLeader();

            // check for additional attrition
            byte attritionChecks = Convert.ToByte(this.days / 7);
            for (int i = 0; i < attritionChecks; i++ )
            {
                // calculate attrition
                double attritionModifer = this.calcAttrition();
                // apply attrition
                this.applyTroopLosses(attritionModifer);
            }

            // check if army dissolves (less than 100 men)
            if (this.calcArmySize() < 100)
            {
                hasDissolved = true;
            }

            // update army days
            if (!hasDissolved)
            {
                this.days = myLeader.days;
            }

            return hasDissolved;
        }

    }
}
