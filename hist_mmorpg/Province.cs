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
        /*
        /// <summary>
        /// Holds province ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds province name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds province overlord (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public String titleHolder { get; set; }
        /// <summary>
        /// Holds province Rank object
        /// </summary>
        public Rank rank { get; set; } */
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
        /// <param name="id">String holding province ID</param>
        /// <param name="nam">String holding province name</param>
        /// <param name="olord">Province overlord (PlayerCharacter)</param>
        /// <param name="ra">Province's Rank object</param>
        /// <param name="otax">Double holding province tax rate</param>
        /// <param name="king">Province's Kingdom object</param>
        public Province(String id, String nam, Double otax, String tiHo = null, PlayerCharacter own = null, Kingdom king = null, Rank r = null)
            : base(id, nam, tiHo, own, r)
        {
            /*
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

            this.id = id;
            this.name = nam;
            this.rank = ra;
            this.owner = olord; */
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
		{
			this.id = pr.id;
			this.name = pr.name;
            // overlord to be inserted later
			this.owner = null;
			this.taxRate = pr.taxRate;
            // kingdom to be inserted later
            this.kingdom = null;
            // rank to be inserted later
            this.rank = null;
        }
    }

	/// <summary>
	/// Class converting province data into format suitable for Riak (JSON) storage
	/// </summary>
	public class Province_Riak : Place_Riak
	{
        /*
		/// <summary>
		/// Holds province ID
		/// </summary>
		public String id { get; set; }
		/// <summary>
		/// Holds province name
		/// </summary>
		public String name { get; set; }
        /// <summary>
        /// Holds province Rank (ID)
        /// </summary>
        public String rank { get; set; }
		/// <summary>
		/// Holds province overlord (ID)
		/// </summary>
		public String owner { get; set; } */
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
            /*
			this.id = prov.id;
			this.name = prov.name;
            this.rank = prov.rank.rankID;
			this.owner = prov.owner.charID; */
            this.taxRate = prov.taxRate;
            this.kingdom = prov.kingdom.id;
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
