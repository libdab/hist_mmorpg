using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on province
    /// </summary>
    public class Province : Place
    {
        /// <summary>
        /// Holds province tax rate
        /// </summary>
        public Double taxRate { get; set; }
        /// <summary>
        /// Holds province kingdom object
        /// </summary>
        public Kingdom kingdom { get; set; }

        /// <summary>
        /// Constructor for Province
        /// </summary>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">Province's Kingdom object</param>
        public Province(String id, String nam, Double otax, String tiHo = null, PlayerCharacter own = null, Kingdom king = null, Rank r = null)
            : base(id, nam, tiHo, own, r)
        {
            this.taxRate = otax;
            this.kingdom = king;
        }

        /// <summary>
        /// Constructor for Province taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Province()
		{
		}

		/// <summary>
        /// Constructor for Province using Province_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ps">Province_Serialised object to use as source</param>
		public Province(Province_Serialised ps)
			: base(ps: ps)
		{
			this.taxRate = ps.taxRate;
            // kingdom to be inserted later
            this.kingdom = null;
        }

        /// <summary>
        /// Adjusts province tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void adjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The maximum tax rate is 100%.  Rate adjusted.");
                }
            }
            else if (tx < 0)
            {
                tx = 0;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The minimum tax rate is 0%.  Rate adjusted.");
                }
            }

            this.taxRate = tx;
        }

        /// <summary>
        /// Gets the province's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom getRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (this.kingdom != null)
            {
                thisKingdom = this.kingdom;
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the province's current kingdom (i.e. the kingdom of the current owner)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom getCurrentKingdom()
        {
            Kingdom thisKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.owner.nationality)
                {
                    thisKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return thisKingdom;
        }

        /// <summary>
        /// Transfers ownership of the province to the specified PlayerCharacter
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        public void transferOwnership(PlayerCharacter newOwner)
        {
            // get current title holder
            Character titleHolder = this.getTitleHolder();

            // remove from current title holder's titles
            titleHolder.myTitles.Remove(this.id);

            // add to newOwner's titles
            newOwner.myTitles.Add(this.id);

            // update province titleHolder property
            this.titleHolder = newOwner.charID;

            // remove from current owner's ownedProvinces
            this.owner.ownedProvinces.Remove(this);

            // add to newOwner's ownedProvinces
            newOwner.ownedProvinces.Add(this);

            // update province owner property
            this.owner = newOwner;
        }

    }

	/// <summary>
	/// Class converting province data into serialised format suitable (JSON)
	/// </summary>
	public class Province_Serialised : Place_Serialised
	{
        /// <summary>
		/// Holds province tax rate
		/// </summary>
		public Double taxRate { get; set; }
        /// <summary>
        /// Holds province kingdom (ID)
        /// </summary>
        public String kingdom { get; set; }

		/// <summary>
        /// Constructor for Province_Serialised.
        /// For use when serialising.
        /// </summary>
		/// <param name="prov">Province object to be used as source</param>
		public Province_Serialised(Province prov)
            : base(p: prov)
		{
            this.taxRate = prov.taxRate;
            this.kingdom = prov.kingdom.id;
		}

        /// <summary>
        /// Constructor for Province_Serialised taking seperate values.
        /// For creating Province_Serialised from CSV file.
        /// </summary>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">string holding Province's Kingdom (id)</param>
        public Province_Serialised(String id, String nam, byte r, Double otax, String tiHo = null, string own = null, string king = null)
            : base(id, nam, r, own: own, tiHo: tiHo)
        {
            this.taxRate = otax;
            this.kingdom = king;
        }

        /// <summary>
        /// Constructor for Province_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Province_Serialised()
		{
		}
	}
}
