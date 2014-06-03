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
        /// Holds fief tax rate (next season)
        /// </summary>
        public Double taxRateNext { get; set; }
        /// <summary>
        /// Holds expenditure on officials (next season)
        /// </summary>
        public uint officialsSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on garrison (next season)
        /// </summary>
        public uint garrisonSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure (next season)
        /// </summary>
        public uint infrastructureSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on keep (next season)
        /// </summary>
        public uint keepSpendNext { get; set; }
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
        /// <param name="txNxt">Double holding fief tax rate (next season)</param>
        /// <param name="offNxt">uint holding officials expenditure (next season)</param>
        /// <param name="garrNxt">uint holding garrison expenditure (next season)</param>
        /// <param name="infraNxt">uint holding infrastructure expenditure (next season)</param>
        /// <param name="keepNxt">uint holding keep expenditure (next season)</param>
        /// <param name="kpLvl">Double holding fief keep level</param>
        /// <param name="loy">Double holding fief loyalty rating</param>
        /// <param name="stat">char holding fief status</param>
        /// <param name="terr">char holding terrain code for fief</param>
        /// <param name="chars">List<Character> holding characters present in fief</param>
        /// <param name="barChars">List<string> holding IDs of characters barred from keep</param>
        /// <param name="engBarr">bool indicating whether English nationality barred from keep</param>
        /// <param name="frBarr">bool indicating whether French nationality barred from keep</param>
        public Fief(String id, String nam, Province prov, uint pop, string own, string ancOwn, string bail, Double fld, Double ind, uint trp,
            Double tx, uint off, uint garr, uint infra, uint keep, Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, Double kpLvl, 
            Double loy, char stat, char terr, List<Character> chars, List<string> barChars, bool engBarr, bool frBarr)
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
            // validate tx = 0-100.00
            if ((txNxt < 0) || (txNxt > 100))
            {
                throw new InvalidDataException("Fief tax rate (next season) must be a double between 0 and 100");
            }

            // TODO: validate offNxt = (upper limit?)
            // TODO: validate garrNxt = (upper limit?)
            // TODO: validate infraNxt = (upper limit?)
            // TODO: validate keepNxt = (upper limit?)

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
            this.taxRateNext = txNxt;
            this.officialsSpendNext = offNxt;
            this.garrisonSpendNext = garrNxt;
            this.infrastructureSpendNext = infraNxt;
            this.keepSpendNext = keepNxt;
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
        /// <param name="season">string specifying whether to calculate for this or next season</returns>
        public uint calcGDP(string season)
        {
            uint gdp = 0;
            uint fldProd = 0;
            uint indProd = 0;
            switch (season)
            {
                case "next":
                    // calculate production from fields
                    fldProd = Convert.ToUInt32((this.calcFieldLevel() * 8997));
                    // calculate production from industry
                    indProd = Convert.ToUInt32(this.calcIndustryLevel() * (290 * Math.Pow(1.2, ((this.calcNewPop() / 1000) - 1))));
                    // calculate final gdp
                    gdp = (fldProd + indProd) / (this.calcNewPop() / 1000);
                    break;
                default:
                    // calculate production from fields
                    fldProd = Convert.ToUInt32((this.fields * 8997));
                    // calculate production from industry
                    indProd = Convert.ToUInt32(this.industry * (290 * Math.Pow(1.2, ((this.population / 1000) - 1))));
                    // calculate final gdp
                    gdp = (fldProd + indProd) / (this.population / 1000);
                    break;
            }

            return gdp;
        }

        /// <summary>
        /// Calculates fief population increase
        /// </summary>
        /// <returns>uint containing new fief population</returns>
        public uint calcNewPop()
        {
            uint newPop = Convert.ToUInt32(this.population + (this.population * 0.035));
            return newPop;
        }

        /// <summary>
        /// Calculates fief income
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</returns>
        public uint calcIncome(string season)
        {
            uint fiefIncome = 0;
            switch (season)
            {
                case "next":
                    fiefIncome = Convert.ToUInt32(this.calcGDP("next") * (this.taxRateNext / 100));
                    break;
                default:
                    fiefIncome = Convert.ToUInt32(this.calcGDP("this") * (this.taxRate / 100));
                    break;
            }
            return fiefIncome;
        }

        /// <summary>
        /// Calculates fief expenses
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</returns>
        public uint calcExpenses(string season)
        {
            uint fiefExpenses = 0;
            switch (season)
            {
                case "next":
                    fiefExpenses = this.officialsSpendNext + this.infrastructureSpendNext + this.garrisonSpendNext + this.keepSpendNext;
                    break;
                default:
                    fiefExpenses = this.officialsSpend + this.infrastructureSpend + this.garrisonSpend + this.keepSpend;
                    break;
            }
            return fiefExpenses;
        }

        /// <summary>
        /// Calculates fief bottom line
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</returns>
        public int calcBottomLine(string season)
        {
            int fiefBottomLine = 0;
            switch (season)
            {
                case "next":
                    fiefBottomLine = (int)this.calcIncome("next") - (int)this.calcExpenses("next");
                    break;
                default:
                    fiefBottomLine = (int)this.calcIncome("this") - (int)this.calcExpenses("this");
                    break;
            }
            return fiefBottomLine;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</returns>
        public void adjustTaxRate(double tx)
        {
            this.taxRateNext = tx;
        }

        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</returns>
        public void adjustOfficialsSpend(uint os)
        {
            this.officialsSpendNext = os;
        }

        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</returns>
        public void adjustInfraSpend(uint infs)
        {
            this.infrastructureSpendNext = infs;
        }

        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</returns>
        public void adjustGarrisonSpend(uint gs)
        {
            this.garrisonSpendNext = gs;
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</returns>
        public void adjustKeepSpend(uint ks)
        {
            this.keepSpendNext = ks;
        }

        /// <summary>
        /// Calculates new fief field level (from next season's spend)
        /// </summary>
        /// <returns>double containing new field level</returns>
        public double calcFieldLevel()
        {
            double fldLvl = this.fields = this.fields + ((this.infrastructureSpendNext / 1000.00) / 500.00);
            return fldLvl;
        }

        /// <summary>
        /// Calculates new fief industry level (from next season's spend)
        /// </summary>
        /// <returns>double containing new industry level</returns>
        public double calcIndustryLevel()
        {
            double indLvl = this.industry = this.industry + ((this.infrastructureSpendNext / 1000.00) / 400.00);
            return indLvl;
        }

        /// <summary>
        /// Calculates new fief keep level (from next season's spend)
        /// </summary>
        /// <returns>double containing new keep level</returns>
        public double calcKeepLevel()
        {
            double kpLvl = this.keepLevel +((this.keepSpendNext / 1000.00) / 400.00);
            return kpLvl;
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
