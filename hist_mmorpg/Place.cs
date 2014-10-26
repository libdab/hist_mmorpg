﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    public abstract class Place
    {
        /// <summary>
        /// Holds place ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds place name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds place owner (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public String titleHolder { get; set; }
        /// <summary>
        /// Holds place rank (Rank object)
        /// </summary>
        public Rank rank { get; set; }

        /// <summary>
        /// Constructor for Place
        /// </summary>
        /// <param name="id">String holding place ID</param>
        /// <param name="nam">String holding place name</param>
        /// <param name="own">Place owner (PlayerCharacter)</param>
        /// <param name="tiHo">String holding place title holder (charID)</param>
        /// <param name="rnk">Place rank (Rank object)</param>
        public Place(String id, String nam, String tiHo = null, PlayerCharacter own = null, Rank r = null)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Place name must be between 1 and 40 characters in length");
            }

            this.id = id;
            this.name = nam;
            this.owner = own;
            this.titleHolder = tiHo;
            this.rank = r;

        }

        /// <summary>
        /// Constructor for Place taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Place()
        {
        }

        /// <summary>
        /// Gets the place's title holder
        /// </summary>
        /// <returns>The title holder</returns>
        public Character getTitleHolder()
        {
            Character myTitleHolder = null;

            if (this.titleHolder != null)
            {
                // get title holder from appropriate master list
                if (Globals_Server.npcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = Globals_Server.npcMasterList[this.titleHolder];
                }
                else if (Globals_Server.pcMasterList.ContainsKey(this.titleHolder))
                {
                    myTitleHolder = Globals_Server.pcMasterList[this.titleHolder];
                }
            }

            return myTitleHolder;
        }

    }

    /// <summary>
    /// Class converting Place data into format suitable for Riak (JSON) storage
    /// </summary>
    public abstract class Place_Riak
    {
        /// <summary>
        /// Holds place ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Holds place name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Holds place owner (PlayerCharacter object)
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public string titleHolder { get; set; }
        /// <summary>
        /// Holds place rank (Rank object)
        /// </summary>
        public string rank { get; set; }

        /// <summary>
        /// Constructor for Place_Riak.
        /// For use when serialising to Riak
        /// </summary>
        /// <param name="k">Kingdom object to be used as source</param>
        public Place_Riak(Kingdom k = null, Province p = null, Fief f = null)
        {
            Place placeToUse = null;

            if (k != null)
            {
                placeToUse = k;
            }
            else if (p != null)
            {
                placeToUse = p;
            }
            else if (f != null)
            {
                placeToUse = f;
            }

            if (placeToUse != null)
            {
                this.id = placeToUse.id;
                this.name = placeToUse.name;
                this.owner = placeToUse.owner.charID;
                this.titleHolder = placeToUse.titleHolder;
                this.rank = placeToUse.rank.rankID;
            }
        }
    }

}