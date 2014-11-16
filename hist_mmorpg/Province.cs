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
        /// For use when de-serialising from Riak
        /// </summary>
        public Province()
		{
		}

		/// <summary>
		/// Constructor for Province using Province_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
		/// <param name="pr">Province_Riak object to use as source</param>
		public Province(Province_Riak pr)
			: base(pr: pr)
		{
			this.taxRate = pr.taxRate;
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

    }

	/// <summary>
	/// Class converting province data into format suitable for Riak (JSON) storage
	/// </summary>
	public class Province_Riak : Place_Riak
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
		/// Constructor for Province_Riak.
        /// For use when serialising to Riak
        /// </summary>
		/// <param name="prov">Province object to be used as source</param>
		public Province_Riak(Province prov)
            : base(p: prov)
		{
            this.taxRate = prov.taxRate;
            this.kingdom = prov.kingdom.id;
		}

        /// <summary>
        /// Constructor for Province_Riak taking seperate values.
        /// For creating Province_Riak from CSV file.
        /// </summary>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">string holding Province's Kingdom (id)</param>
        public Province_Riak(String id, String nam, Double otax, byte r, String tiHo = null, string own = null, string king = null)
            : base(id, nam, own: own, r: r, tiHo: tiHo)
        {
            this.taxRate = otax;
            this.kingdom = king;
        }

        /// <summary>
        /// Constructor for Province_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Province_Riak()
		{
		}
	}
}
