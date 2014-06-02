using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on fief
    /// </summary>
    public class Fief
    {

        /// <summary>
        /// Holds fief ID
        /// </summary>
        public String fiefID { get; set; }
        /// <summary>
        /// Holds fief name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds fief's Province object
        /// </summary>
        // public String province { get; set; }
        public Province province { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        public uint population { get; set; }
        /// <summary>
        /// Holds fief owner (ID)
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// Holds fief ancestral owner (ID)
        /// </summary>
        public string ancestralOwner { get; set; }
        /// <summary>
        /// Holds fief bailiff (ID)
        /// </summary>
        public string bailiff { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        public Double fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        public Double industry { get; set; }
        /// <summary>
        /// Holds no. trrops in fief
        /// </summary>
        public uint troops { get; set; }
        /// <summary>
        /// Holds fief tax rate
        /// </summary>
        public Double taxRate { get; set; }
        /// <summary>
        /// Holds expenditure on officials
        /// </summary>
        public uint officialsSpend { get; set; }
        /// <summary>
        /// Holds expenditure on garrison
        /// </summary>
        public uint garrisonSpend { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure
        /// </summary>
        public uint infrastructureSpend { get; set; }
        /// <summary>
        /// Holds expenditure on keep
        /// </summary>
        public uint keepSpend { get; set; }
        /// <summary>
        /// Holds fief keep level
        /// </summary>
        public Double keepLevel { get; set; }
        /// <summary>
        /// Holds fief loyalty
        /// </summary>
        public Double loyalty { get; set; }
        /// <summary>
        /// Holds fief status (calm, unrest, rebellion)
        /// </summary>
        public char status { get; set; }
        /// <summary>
        /// Holds terrain code
        /// </summary>
        public char terrain { get; set; }
        /// <summary>
        /// Holds list of characters present in fief
        /// </summary>
        public List<Character> characters = new List<Character>();
        /// <summary>
        /// Holds fief's FiefKeep object
        /// </summary>
        public List<string> barredCharacters = new List<string>();
        /// <summary>
        /// Indicates whether English nationality barred from keep
        /// </summary>
        public bool englishBarred { get; set; }
        /// <summary>
        /// Indicates whether French nationality barred from keep
        /// </summary>
        public bool frenchBarred { get; set; }

        /// <summary>
        /// Constructor for Fief
        /// </summary>
        /// <param name="id">String holding fief ID</param>
        /// <param name="nam">String holding fief name</param>
        /// <param name="prov">Fief's Province object</param>
        /// <param name="pop">uint holding fief population</param>
        /// <param name="own">string holding fief owner ID</param>
        /// <param name="ancOwn">string holding fief ancestral owner ID</param>
        /// <param name="bail">string holding fief bailiff ID</param>
        /// <param name="fld">Double holding fief field level</param>
        /// <param name="fld">Double holding fief industry level</param>
        /// <param name="trp">uint holding no. of troops in fief</param>
        /// <param name="tx">Double holding fief tax rate</param>
        /// <param name="off">uint holding officials expenditure</param>
        /// <param name="garr">uint holding garrison expenditure</param>
        /// <param name="infra">uint holding infrastructure expenditure</param>
        /// <param name="keep">uint holding keep expenditure</param>
        /// <param name="kpLvl">Double holding fief keep level</param>
        /// <param name="loy">Double holding fief loyalty rating</param>
        /// <param name="stat">char holding fief status</param>
        /// <param name="terr">char holding terrain code for fief</param>
        /// <param name="chars">List<Character> holding characters present in fief</param>
        /// <param name="barChars">List<string> holding IDs of characters barred from keep</param>
        /// <param name="engBarr">bool indicating whether English nationality barred from keep</param>
        /// <param name="frBarr">bool indicating whether French nationality barred from keep</param>
        public Fief(String id, String nam, Province prov, uint pop, string own, string ancOwn, string bail, Double fld, Double ind, uint trp,
            Double tx, uint off, uint garr, uint infra, uint keep, Double kpLvl, Double loy, char stat, char terr, List<Character> chars, 
            List<string> barChars, bool engBarr, bool frBarr)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/01-19

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Fief name must be between 1 and 40 characters in length");
            }

            // TODO: validate prov ID = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate pop = 1-2000000
            if ((pop < 1) || (pop > 2000000))
            {
                throw new InvalidDataException("Fief population must be an integer between 1 and 2000000");
            }

            // TODO: validate own ID = 1-10000?
            // TODO: validate ancOwn ID = 1-10000?
            // TODO: validate bail ID = 1-10000?

            // validate fld >= 0
            if (fld < 0)
            {
                throw new InvalidDataException("Fief field level must be a double >= 0");
            }

            // validate ind >= 0
            if (ind < 0)
            {
                throw new InvalidDataException("Fief industry level must be a double >= 0");
            }

            // TODO: validate trp = (upper limit?)
            // validate tx = 0-100.00
            if ((tx < 0) || (tx > 100))
            {
                throw new InvalidDataException("Fief tax rate must be a double between 0 and 100");
            }

            // TODO: validate off = (upper limit?)
            // TODO: validate garr = (upper limit?)
            // TODO: validate infra = (upper limit?)
            // TODO: validate keep = (upper limit?)

            // validate keepLvl >= 0
            if (kpLvl < 0)
            {
                throw new InvalidDataException("Fief keep level must be a double >= 0");
            }

            // validate loy = 0-9.00
            if ((loy < 0) || (loy > 9))
            {
                throw new InvalidDataException("Fief loyalty must be a double between 0 and 9");
            }

            // validate stat = C/U/R
            if (((!stat.Equals('C')) && (!stat.Equals('U'))) && (!stat.Equals('R')))
            {
                throw new InvalidDataException("Fief status must be 'C', 'U' or 'R'");
            }

            // TODO: validate terr = P,H,F,M,S

            this.fiefID = id;
            this.name = nam;
            this.province = prov;
            this.population = pop;
            this.owner = own;
            this.ancestralOwner = ancOwn;
            this.bailiff = bail;
            this.fields = fld;
            this.industry = ind;
            this.troops = trp;
            this.taxRate = tx;
            this.officialsSpend = off;
            this.garrisonSpend = garr;
            this.infrastructureSpend = infra;
            this.keepSpend = keep;
            this.keepLevel = kpLvl;
            this.loyalty = loy;
            this.status = stat;
            this.terrain = terr;
            this.characters = chars;
            this.barredCharacters = barChars;
            this.englishBarred = engBarr;
            this.frenchBarred = frBarr;
        }

        /// <summary>
        /// Calculates fief GDP
        /// </summary>
        /// <returns>uint containing fief GDP</returns>
        public uint calcGDP()
        {
            uint gdp = 0;
            // calculate production from fields
            uint fldProd = Convert.ToUInt32((this.fields * 8997));
            // calculate production from industry
            uint indProd = Convert.ToUInt32(this.industry * (290 * Math.Pow(1.2, ((this.population / 1000) - 1))));
            // calculate final gdp
            gdp = (fldProd + indProd) / (this.population / 1000);
            return gdp;
        }

        /// <summary>
        /// Adds character to characters list
        /// </summary>
        /// <param name="ch">Character to be inserted into characters list</param>
        internal void addCharacter(Character ch)
        {
            this.characters.Add(ch);
        }

        /// <summary>
        /// removes character from characters list
        /// </summary>
        /// <param name="ch">Character to be removed from characters list</param>
        /// <returns>bool indicating success/failure</returns>
        internal bool removeCharacter(Character ch)
        {
            bool success = false;
            if (this.characters.Remove(ch))
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Adds character to barredCharacters list
        /// </summary>
        /// <param name="ch">Character to be inserted into barredCharacters list</param>
        internal void barCharacter(string ch)
        {
            this.barredCharacters.Add(ch);
        }

        /// <summary>
        /// removes character from barredCharacters list
        /// </summary>
        /// <param name="ch">Character to be removed from barredCharacters list</param>
        /// <returns>bool indicating success/failure</returns>
        internal bool removeBarCharacter(string ch)
        {
            bool success = false;
            if (this.barredCharacters.Remove(ch))
            {
                success = true;
            }

            return success;
        }

    }
}
