using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on kingdom
    /// </summary>
    public class Kingdom : Place
    {
        /*
        /// <summary>
        /// Holds kingdom ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds kingdom name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds kingdom king (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public String titleHolder { get; set; }
        /// <summary>
        /// Holds kingdom rank (Rank object)
        /// </summary>
        public Rank rank { get; set; }

        /// <summary>
        /// Constructor for Kingdom
        /// </summary>
        /// <param name="id">String holding kingdom ID</param>
        /// <param name="nam">String holding kingdom name</param>
        /// <param name="own">Kingdom king (PlayerCharacter)</param>
        /// <param name="r">Kingdom rank (Rank object)</param> */
        public Kingdom(String id, String nam, String tiHo = null, PlayerCharacter own = null, Rank r = null)
        : base(id, nam, tiHo, own, r)
        {

            /*
            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Kingdom name must be between 1 and 40 characters in length");
            }


            this.id = id;
            this.name = nam;
            this.owner = own;
            this.rank = r; */

        }

        /// <summary>
        /// Constructor for Kingdom taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Kingdom()
		{
		}

		/// <summary>
        /// Constructor for Kingdom using Kingdom_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="kr">Kingdom_Riak object to use as source</param>
        public Kingdom(Kingdom_Riak kr)
		{
			this.id = kr.id;
			this.name = kr.name;
            // overlord to be inserted later
			this.owner = null;
            // rank to be inserted later
            this.rank = null;
        }
    }

    /// <summary>
    /// Class converting kingdom data into format suitable for Riak (JSON) storage
    /// </summary>
    public class Kingdom_Riak : Place_Riak
    {
        /*
		/// <summary>
        /// Holds kingdom ID
		/// </summary>
        public String id { get; set; }
		/// <summary>
        /// Holds kingdom name
		/// </summary>
		public String name { get; set; }
		/// <summary>
        /// Holds kingdom overlord (ID)
		/// </summary>
		public String owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public String titleHolder { get; set; }
        /// <summary>
        /// Holds kingdom rank (ID)
        /// </summary>
        public String rank { get; set; }

		/// <summary>
        /// Constructor for Kingdom_Riak.
        /// For use when serialising to Riak
        /// </summary>
        /// <param name="k">Kingdom object to be used as source</param> */
        public Kingdom_Riak(Kingdom king)
            : base(k: king)
		{
            /*
            this.id = k.id;
            this.name = k.name;
            this.owner = k.owner.charID;
            this.titleHolder = k.titleHolder;
            this.rank = k.rank.rankID; */
		}

        /// <summary>
        /// Constructor for Kingdom_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Kingdom_Riak()
		{
		}
    }

}
