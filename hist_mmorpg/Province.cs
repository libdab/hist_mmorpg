using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on province
    /// </summary>
    public class Province
    {
        /// <summary>
        /// Holds province ID
        /// </summary>
        public String provinceID { get; set; }
        /// <summary>
        /// Holds province name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds province overlord (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter overlord { get; set; }
        /// <summary>
        /// Holds province overlord tax rate
        /// </summary>
        public Double overlordTaxRate { get; set; }
        /// <summary>
        /// Holds province language
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// Constructor for Province
        /// </summary>
        /// <param name="id">String holding province ID</param>
        /// <param name="nam">String holding province name</param>
        /// <param name="olord">Province overlord (PlayerCharacter)</param>
        /// <param name="otax">Double holding province overlord tax rate</param>
        /// <param name="lang">String holding province language code</param>
        public Province(String id, String nam, Double otax, string lang, PlayerCharacter olord = null)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Province name must be between 1 and 40 characters in length");
            }

            // validate otax = 0-100.00
            if (otax > 100)
            {
                otax = 100;
            }
            else if (otax < 0)
            {
                otax = 0;
            }

            // TODO: validate lang = string B,C,D,E,F,G,H,I,L/1-3

            this.provinceID = id;
            this.name = nam;
            this.overlord = olord;
            this.overlordTaxRate = otax;
            this.language = lang;

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
		{
			this.provinceID = pr.provinceID;
			this.name = pr.name;
            // overlord to be inserted later
			this.overlord = null;
			this.overlordTaxRate = pr.overlordTaxRate;
			this.language = pr.language;
		}
    }

	/// <summary>
	/// Class converting province data into format suitable for Riak (JSON) storage
	/// </summary>
	public class Province_Riak
	{
		/// <summary>
		/// Holds province ID
		/// </summary>
		public String provinceID { get; set; }
		/// <summary>
		/// Holds province name
		/// </summary>
		public String name { get; set; }
		/// <summary>
		/// Holds province overlord (ID)
		/// </summary>
		public String overlordID { get; set; }
		/// <summary>
		/// Holds province overlord tax rate
		/// </summary>
		public Double overlordTaxRate { get; set; }
		/// <summary>
		/// Holds province language
		/// </summary>
		public string language { get; set; }

		/// <summary>
		/// Constructor for Province_Riak.
        /// For use when serialising to Riak
        /// </summary>
		/// <param name="prov">Province object to be used as source</param>
		public Province_Riak(Province prov)
		{
			this.provinceID = prov.provinceID;
			this.name = prov.name;
			this.overlordID = prov.overlord.charID;
			this.overlordTaxRate = prov.overlordTaxRate;
			this.language = prov.language;
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
