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
		/// Holds province overlord (Character object)
        /// </summary>
        public Character overlord { get; set; }
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
        /// <param name="olord">Character holding province overlord</param>
        /// <param name="otax">Double holding province overlord tax rate</param>
        /// <param name="lang">String holding province language code</param>
        public Province(String id, String nam, Double otax, string lang, Character olord = null)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Province name must be between 1 and 40 characters in length");
            }

            // validate otax = 0-100.00
            if ((otax < 0) || (otax > 100))
            {
                throw new InvalidDataException("Province overlord tax rate must be a double between 0 and 100");
            }

            // TODO: validate lang = string B,C,D,E,F,G,H,I,L/1-3

            this.provinceID = id;
            this.name = nam;
            this.overlord = olord;
            this.overlordTaxRate = otax;
            this.language = lang;

        }

		/// <summary>
		/// Constructor for Province using Province_Riak object
		/// </summary>
		/// <param name="pr">Province_Riak object</param>
		public Province(Province_Riak pr)
		{
			this.provinceID = pr.provinceID;
			this.name = pr.name;
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
		/// Constructor for Province_Riak
		/// </summary>
		/// <param name="prov">Province object</param>
		public Province_Riak(Province prov)
		{

			this.provinceID = prov.provinceID;
			this.name = prov.name;
			this.overlordID = prov.overlord.charID;
			this.overlordTaxRate = prov.overlordTaxRate;
			this.language = prov.language;

		}

		public Province_Riak()
		{
		}
	}
}
