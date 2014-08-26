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
        public uint rabble { get; set; }
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
        /// Constructor for Army
        /// </summary>
		/// <param name="own">String holding ID of army</param>
        /// <param name="kni">uint holding no. of knights in army</param>
        /// <param name="maa">uint holding no. of men-at-arms in army</param>
        /// <param name="ltCav">uint holding no. of light cavalry in army</param>
        /// <param name="yeo">uint holding no. of yeomen in army</param>
        /// <param name="ft">uint holding no. of foot in army</param>
        /// <param name="rbl">uint holding no. of rabble in army</param>
        /// <param name="ldr">string holding ID of army leader</param>
        /// <param name="own">string holding ID of army owner</param>
        /// <param name="day">double holding remaining days in season for army</param>
        /// <param name="cl">GameClock holding season</param>
        /// <param name="loc">string holding army location (fiefID)</param>
        /// <param name="maint">bool indicating whether army is being actively maintained by owner</param>
        public Army(String id, string ldr, string own, double day, GameClock cl, string loc, uint kni = 0, uint maa = 0, uint ltCav = 0, uint yeo = 0, uint ft = 0, uint rbl = 0, bool maint = false)
        {

            // TODO: validate kni = (upper limit?)
            // TODO: validate maa = (upper limit?)
            // TODO: validate ltCav = (upper limit?)
            // TODO: validate yeo = (upper limit?)
            // TODO: validate ft = (upper limit?)
            // TODO: validate rbl = (upper limit?)
            // TODO: validate ldr ID = 1-10000?

            // validate day > 90
            if ((day > 90) || (day < 0))
            {
                throw new InvalidDataException("Army remaining days must be an integer between 0 and 90");
            }

			this.armyID = id;
            this.knights = kni;
            this.menAtArms = maa;
            this.lightCavalry = ltCav;
            this.yeomen = yeo;
            this.foot = ft;
            this.rabble = rbl;
            this.leader = ldr;
            this.owner = own;
            this.days = day;
            this.clock = cl;
            this.location = loc;
            this.isMaintained = maint;
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

            armySize = this.foot + this.knights + this.lightCavalry + this.menAtArms + this.rabble + this.yeomen;

            return armySize;
        }

        /// <summary>
        /// Moves army to another fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool moveArmy()
        {
            bool success = false;

            // get leader
            Character myLeader = null;
            if (Globals.npcMasterList.ContainsKey(this.leader))
            {
                myLeader = Globals.npcMasterList[this.leader];
            }
            else
            {
                myLeader = Globals.pcMasterList[this.leader];
            }

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

    }
}
