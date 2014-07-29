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
        public Province province { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        public uint population { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        public Double fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        public Double industry { get; set; }
        /// <summary>
        /// Holds number of troops in fief
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
        /// Holds fief language and dialect
        /// </summary>
        public Tuple<Language, int> language { get; set; }
        /// <summary>
        /// Holds terrain object
        /// </summary>
        public Terrain terrain { get; set; }
        /// <summary>
        /// Holds characters present in fief
        /// </summary>
        public List<Character> characters = new List<Character>();
        /// <summary>
		/// Holds characters banned from keep (charIDs)
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
        /// Holds fief's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }
        /// <summary>
        /// Holds fief owner (PlayerCharacter object)
        /// </summary>
		public PlayerCharacter owner { get; set; }
        /// <summary>
        /// Holds fief ancestral owner (PlayerCharacter object)
        /// </summary>
		public PlayerCharacter ancestralOwner { get; set; }
        /// <summary>
        /// Holds fief bailiff (Character object)
        /// </summary>
        public Character bailiff { get; set; }

        /// <summary>
        /// Constructor for Fief
        /// </summary>
        /// <param name="id">String holding fief ID</param>
        /// <param name="nam">String holding fief name</param>
        /// <param name="prov">Fief's Province object</param>
        /// <param name="pop">uint holding fief population</param>
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
        /// <param name="lang">Tuple<Language, int> holding language and dialect</param>
        /// <param name="terr">Terrain object for fief</param>
        /// <param name="chars">List holding characters present in fief</param>
        /// <param name="barChars">List holding IDs of characters barred from keep</param>
        /// <param name="engBarr">bool indicating whether English nationality barred from keep</param>
        /// <param name="frBarr">bool indicating whether French nationality barred from keep</param>
        /// <param name="cl">GameClock holding season</param>
		/// <param name="own">PlayerCharacter holding fief owner</param>
		/// <param name="ancOwn">PlayerCharacter holding fief ancestral owner</param>
        /// <param name="bail">Character holding fief bailiff</param>
        public Fief(String id, String nam, Province prov, uint pop, Double fld, Double ind, uint trp,
            Double tx, uint off, uint garr, uint infra, uint keep, Double txNxt, uint offNxt, uint garrNxt, uint infraNxt, uint keepNxt, Double kpLvl,
            Double loy, char stat, Tuple<Language, int> lang, Terrain terr, List<Character> chars, List<string> barChars, bool engBarr, bool frBarr, GameClock cl, PlayerCharacter own = null, PlayerCharacter ancOwn = null, Character bail = null)
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
            this.language = lang;
            this.terrain = terr;
            this.characters = chars;
            this.barredCharacters = barChars;
            this.englishBarred = engBarr;
            this.frenchBarred = frBarr;
            this.clock = cl;
        }

		/// <summary>
		/// Constructor for Fief using Fief_Riak object.
        /// For use when de-serialising from Riak
        /// </summary>
		/// <param name="fr">Fief_Riak object to use as source</param>
		public Fief(Fief_Riak fr)
		{
		
			this.fiefID = fr.fiefID;
			this.name = fr.name;
			this.province = null;
			this.population = fr.population;
			this.owner = null;
			this.ancestralOwner = null;
			this.bailiff = null;
			this.fields = fr.fields;
			this.industry = fr.industry;
			this.troops = fr.troops;
			this.taxRate = fr.taxRate;
			this.officialsSpend = fr.officialsSpend;
			this.garrisonSpend = fr.garrisonSpend;
			this.infrastructureSpend = fr.infrastructureSpend;
			this.keepSpend = fr.keepSpend;
			this.taxRateNext = fr.taxRateNext;
			this.officialsSpendNext = fr.officialsSpendNext;
			this.garrisonSpendNext = fr.garrisonSpendNext;
			this.infrastructureSpendNext = fr.infrastructureSpendNext;
			this.keepSpendNext = fr.keepSpendNext;
			this.keepLevel = fr.keepLevel;
			this.loyalty = fr.loyalty;
			this.status = fr.status;
            this.language = null;
			this.terrain = null;
            // create empty list to be populated later
			this.characters = new List<Character>();
			this.barredCharacters = fr.barredCharacters;
			this.englishBarred = fr.englishBarred;
			this.frenchBarred = fr.frenchBarred;
			this.clock = null;
		}

        /// <summary>
        /// Constructor for Fief taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Fief()
		{
		}

        /// <summary>
        /// Calculates fief GDP
        /// </summary>
        /// <returns>uint containing fief GDP</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public uint calcGDP(string season)
        {
            uint gdp = 0;
            uint fldProd = 0;
            uint indProd = 0;
            switch (season)
            {
                case "next":
                    // calculate production from fields using next season's expenditure
                    fldProd = Convert.ToUInt32((this.calcNewFieldLevel() * 8997));
                    // calculate production from industry using next season's expenditure
                    indProd = Convert.ToUInt32(this.calcNewIndustryLevel() * (290 * Math.Pow(1.2, ((this.calcNewPop() / 1000) - 1))));
                    // calculate final gdp
                    gdp = (fldProd + indProd) / (this.calcNewPop() / 1000);
                    break;
                default:
                    // calculate production from fields using current expenditure
                    fldProd = Convert.ToUInt32((this.fields * 8997));
                    // calculate production from industry using current expenditure
                    indProd = Convert.ToUInt32(this.industry * (290 * Math.Pow(1.2, ((this.population / 1000) - 1))));
                    // calculate final gdp
                    gdp = (fldProd + indProd) / (this.population / 1000);
                    break;
            }
            gdp = (gdp * 1000);
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
        /// Calculates fief income (NOT including income loss due to unrest/rebellion)
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int calcIncome(string season)
        {
            int fiefBaseIncome = 0;
            int fiefIncome = 0;
            switch (season)
            {
                case "next":
                    // using next season's expenditure and tax rate
                    fiefBaseIncome = Convert.ToInt32(this.calcGDP(season) * (this.taxRateNext / 100));
                    break;
                default:
                    // using current expenditure and tax rate
                    fiefBaseIncome = Convert.ToInt32(this.calcGDP(season) * (this.taxRate / 100));
                    break;
            }

            fiefIncome = fiefBaseIncome;

            // factor in bailiff modifier
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.calcBlfIncMod());

            // factor in officials spend modifier
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * this.calcOffIncMod(season));

            return fiefIncome;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcBlfIncMod()
        {
            double incomeModif = 0;

            // check if auto-bailiff
            if (this.bailiff == null)
            {
                // modifer = 0.025 per management level above 1
                // if auto-baliff set modifier at equivalent of management rating of 3
                incomeModif = 0.05;
            }
            else
            {
                incomeModif = this.bailiff.calcFiefIncMod();
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public double calcOffIncMod(string season)
        {
            double incomeModif = 0;

            switch (season)
            {
                case "next":
                    // using next season's expenditure and population
                    incomeModif = ((this.officialsSpendNext - ((double)this.calcNewPop() * 2)) / (this.calcNewPop() * 2)) / 10;
                    break;
                default:
                    // using current expenditure and population
                    incomeModif = ((this.officialsSpend - ((double)this.population * 2)) / (this.population * 2)) / 10;
                    break;
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of unrest/rebellion on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcStatusIncmMod()
        {
            double incomeModif = 0;

            switch (this.status)
            {
                // unrest = income reduced by 50%
                case 'U':
                    incomeModif = 0.5;
                    break;
                // unrest = income reduced by 100%
                case 'R':
                    incomeModif = 0;
                    break;
                // anything else = no reduction
                default:
                    incomeModif = 1;
                    break;
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates overlord taxes
        /// </summary>
        /// <returns>uint containing overlord taxes</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public uint calcOlordTaxes(string season)
        {
            // assumes overlord tax rate remains unchanged
            uint oTaxes = Convert.ToUInt32(this.calcIncome(season) * (this.province.overlordTaxRate / 100));
            return oTaxes;
        }

        /// <summary>
        /// Calculates fief expenses
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int calcExpenses(string season)
        {
            int fiefExpenses = 0;
            switch (season)
            {
                // using next season's expenditure
                case "next":
                    fiefExpenses = (int)this.officialsSpendNext + (int)this.infrastructureSpendNext + (int)this.garrisonSpendNext + (int)this.keepSpendNext;
                    break;
                // using current expenditure
                default:
                    fiefExpenses = (int)this.officialsSpend + (int)this.infrastructureSpend + (int)this.garrisonSpend + (int)this.keepSpend;
                    break;
            }

            // factor in bailiff skills modifier for fief expenses
            // assumes bailiff remains unchanged
            double bailiffModif = 0;
            bailiffModif = this.calcBailExpModif();
            if (bailiffModif != 0 )
            {
                fiefExpenses = fiefExpenses + Convert.ToInt32(fiefExpenses * bailiffModif);
            }

            return fiefExpenses;
        }

        /// <summary>
        /// Calculates effect of bailiff skills on fief expenses
        /// </summary>
        /// <returns>double containing fief expenses modifier</returns>
        public double calcBailExpModif()
        {
            double expSkillsModifier = 0;

            if (this.bailiff != null)
            {
                expSkillsModifier = this.bailiff.calcSkillEffect("fiefExpense");
            }

            return expSkillsModifier;
        }
        
        /// <summary>
        /// Calculates fief bottom line
        /// </summary>
        /// <returns>uint containing fief income</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int calcBottomLine(string season)
        {
            int fiefBottomLine = 0;
            switch (season)
            {
                // using next season's income, expenses and overlord taxes
                case "next":
                    fiefBottomLine = ((int)this.calcIncome("next") - (int)this.calcExpenses("next")) - (int)this.calcOlordTaxes("next");
                    break;
                // using current income, expenses and overlord taxes
                default:
                    // factor in effect of current fief status
                    int fiefIncome = Convert.ToInt32(this.calcIncome("this") * this.calcStatusIncmMod());
                    fiefBottomLine = (fiefIncome - (int)this.calcExpenses("this")) - (int)this.calcOlordTaxes("this");
                    break;
            }
            return fiefBottomLine;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void adjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
                System.Windows.Forms.MessageBox.Show("The maximum tax rate is 100%.  Rate adjusted.");
            }
            else if (tx < 0)
            {
                tx = 0;
                System.Windows.Forms.MessageBox.Show("The minimum tax rate is 0%.  Rate adjusted.");
            }

            this.taxRateNext = tx;
        }

        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</param>
        public void adjustOfficialsSpend(uint os)
        {
            // ensure min 0
            if (os < 0)
            {
                os = 0;
                System.Windows.Forms.MessageBox.Show("The minimum officials expenditure is 0.  Amount adjusted.");
            }

            // ensure doesn't exceed max permitted (4 per head of population)
            uint maxSpend = this.population * 4;
            if (os > maxSpend)
            {
                os = maxSpend;
                System.Windows.Forms.MessageBox.Show("The maximum officials expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
            }

            this.officialsSpendNext = os;
        }

        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</param>
        public void adjustInfraSpend(uint infs)
        {
            // ensure min 0
            if (infs < 0)
            {
                infs = 0;
                System.Windows.Forms.MessageBox.Show("The minimum infrastructure expenditure is 0.  Amount adjusted.");
            }

            // ensure doesn't exceed max permitted (6 per head of population)
            uint maxSpend = this.population * 6;
            if (infs > maxSpend)
            {
                infs = maxSpend;
                System.Windows.Forms.MessageBox.Show("The maximum infrastructure expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
            }

            this.infrastructureSpendNext = infs;
        }

        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</param>
        public void adjustGarrisonSpend(uint gs)
        {
            // ensure min 0
            if (gs < 0)
            {
                gs = 0;
                System.Windows.Forms.MessageBox.Show("The minimum garrison expenditure is 0.  Amount adjusted.");
            }

            // ensure doesn't exceed max permitted (14 per head of population)
            uint maxSpend = this.population * 14;
            if (gs > maxSpend)
            {
                gs = maxSpend;
                System.Windows.Forms.MessageBox.Show("The maximum garrison expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
            }

            this.garrisonSpendNext = gs;
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</param>
        public void adjustKeepSpend(uint ks)
        {
            // ensure min 0
            if (ks < 0)
            {
                ks = 0;
                System.Windows.Forms.MessageBox.Show("The minimum keep expenditure is 0.  Amount adjusted.");
            }

            // ensure doesn't exceed max permitted (13 per head of population)
            uint maxSpend = this.population * 13;
            if (ks > maxSpend)
            {
                ks = maxSpend;
                System.Windows.Forms.MessageBox.Show("The maximum keep expenditure for this fief is " + maxSpend + ".\r\nAmount adjusted.");
            }

            this.keepSpendNext = ks;
        }

        /// <summary>
        /// Calculates new fief field level (from next season's spend)
        /// </summary>
        /// <returns>double containing new field level</returns>
        public double calcNewFieldLevel()
        {
            double fldLvl = 0;

            // if no expenditure, field level reduced by 1%
            if (this.infrastructureSpendNext == 0)
            {
                fldLvl = this.fields - (this.fields / 100);
            }
            // field level increases by 0.2 per 100k spent
            else
            {
                fldLvl = this.fields + (this.infrastructureSpendNext / 500000.00);
            }

            return fldLvl;
        }

        /// <summary>
        /// Calculates new fief industry level (from next season's spend)
        /// </summary>
        /// <returns>double containing new industry level</returns>
        public double calcNewIndustryLevel()
        {
            double indLvl = 0;

            // if no expenditure, industry level reduced by 1%
            if (this.infrastructureSpendNext == 0)
            {
                indLvl = this.industry - (this.industry / 100);
            }
            // industry level increases by 0.1 per 100k spent
            else
            {
                indLvl = this.industry + (this.infrastructureSpendNext / 1000000.00);
            }
            return indLvl;
        }

        /// <summary>
        /// Calculates new fief keep level (from next season's spend)
        /// </summary>
        /// <returns>double containing new keep level</returns>
        public double calcNewKeepLevel()
        {
            double kpLvl = 0;

            // if no expenditure, keep level reduced by 0.15
            if (this.keepSpendNext == 0)
            {
                kpLvl = this.keepLevel - 0.15;
            }
            // keep level increases by 0.25 per 100k spent
            else
            {
                kpLvl = this.keepLevel + (this.keepSpendNext / 400000.00);
            }

            return kpLvl;
        }

        /// <summary>
        /// Calculates new fief loyalty level (i.e. for next season)
        /// </summary>
        /// <returns>double containing new fief loyalty</returns>
        public double calcNewLoyalty()
        {
            double newBaseLoy = 0;
            double newLoy = 0;

            // calculate effect of tax rate change = loyalty % change is direct inverse of tax % change
            newBaseLoy = this.loyalty + (this.loyalty * (((this.taxRateNext - this.taxRate) / 100) * -1));
            
            if (this.calcBottomLine("next") > 0)
            {
                // calculate effect of surplus 
                // = loyalty reduced in proportion to surplus divided by income
                newLoy = newBaseLoy - (this.calcBottomLine("next") / Convert.ToDouble(this.calcIncome("next")));
            }

            // calculate effect of officials spend
            newLoy = newLoy + (newBaseLoy * this.calcOffLoyMod("next"));

            // calculate effect of garrison spend
            newLoy = newLoy + (newBaseLoy * this.calcGarrLoyMod("next"));

            return newLoy;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief loyalty level
        /// Also includes effect of skills
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcBlfLoyAdjusted()
        {
            double loyModif = 0;
            double loySkillModif = 0;

            loyModif = this.calcBlfLoyMod();
            loySkillModif = this.calcBailLoySkillMod();

            // Check if loyalty effected by character skills
            if (loySkillModif != 0)
            {
                loyModif = loyModif + (loyModif * loySkillModif);
            }

            return loyModif;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief loyalty level
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcBlfLoyMod()
        {
            double loyModif = 0;

            if (this.bailiff == null)
            {
                // modifer = 0.0125 per stature/management average above 1
                // if auto-baliff, set modifier at equivalent of stature/management average of 3
                loyModif = 0.025;
            }
            else
            {
                loyModif = this.bailiff.calcFiefLoyMod();
            }

            return loyModif;
        }

        /// <summary>
        /// Calculates bailiff's skill modifier for fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcBailLoySkillMod()
        {
            double loySkillsModifier = 0;

            if (this.bailiff != null)
            {
                loySkillsModifier = this.bailiff.calcSkillEffect("fiefLoy");
            }

            return loySkillsModifier;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public double calcOffLoyMod(string season)
        {
            double loyaltyModif = 0;

            switch (season)
            {
                // using next season's officials spend and population
                case "next":
                    loyaltyModif = ((this.officialsSpendNext - ((double)this.calcNewPop() * 2)) / (this.calcNewPop() * 2)) / 10;
                    break;
                // using current officials spend and population
                default:
                    loyaltyModif = ((this.officialsSpend - ((double)this.population * 2)) / (this.population * 2)) / 10;
                    break;
            }

            return loyaltyModif;
        }

        /// <summary>
        /// Calculates effect of garrison spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public double calcGarrLoyMod(string season)
        {
            double loyaltyModif = 0;

            switch (season)
            {
                // using next season's garrison spend and population
                case "next":
                    loyaltyModif = ((this.garrisonSpendNext - ((double)this.calcNewPop() * 7)) / (this.calcNewPop() * 7)) / 10;
                    break;
                // using current garrison spend and population
                default:
                    loyaltyModif = ((this.garrisonSpend - ((double)this.population * 7)) / (this.population * 7)) / 10;
                    break;
            }

            return loyaltyModif;
        }

        /// <summary>
        /// Updates fief data at the end/beginning of the season
        /// </summary>
        public void updateFief()
        {
            // update loyalty level
            this.loyalty = this.calcNewLoyalty();
            // update fields level (based on next season infrastructure spend)
            this.fields = this.calcNewFieldLevel();
            // update industry level (based on next season infrastructure spend)
            this.industry = this.calcNewIndustryLevel();
            // update keep level (based on next season keep spend)
            this.keepLevel = this.calcNewKeepLevel();
            // update tax rate
            this.taxRate = this.taxRateNext;
            // update officials spend
            this.officialsSpend = this.officialsSpendNext;
            // update garrison spend
            this.garrisonSpend = this.garrisonSpendNext;
            // update infrastructure spend
            this.infrastructureSpend = this.infrastructureSpendNext;
            // update keep spend
            this.keepSpend = this.keepSpendNext;
            // check for unrest/rebellion
            this.status = this.checkFiefStatus();
        }

        /// <summary>
        /// Checks for transition from calm to unrest/rebellion;
        /// Or from unrest to calm
        /// </summary>
        /// <returns>char indicating fief status</returns>
        public char checkFiefStatus()
        {
            char stat = this.status;
            Random rand = new Random();

            // if fief in rebellion it can only be recovered by combat or bribe,
            // so don't run check
            if (! stat.Equals('R'))
            {
                // method 1 (depends on tax rate and surplus)
                if ((this.taxRate > 20) && (this.calcBottomLine("this") > (this.calcIncome("this") * 0.1)))
                {
                    if ((rand.NextDouble() * 100) <= (this.taxRate - 20))
                    {
                        stat = 'R';
                    }
                }

                // method 2 (depends on fief loyalty level)
                if (!stat.Equals('R'))
                {
                    double chance = (rand.NextDouble() * 100);
                    
                    // loyalty 3-4
                    if ((this.loyalty > 3) && (this.loyalty <= 4))
                    {
                        if (chance <= 2)
                        {
                            stat = 'R';
                        }
                        else if (chance <= 10)
                        {
                            stat = 'U';
                        }
                        else
                        {
                            stat = 'C';
                        }
                    }

                    // loyalty 2-3
                    else if ((this.loyalty > 2) && (this.loyalty <= 3))
                    {
                        if (chance <= 14)
                        {
                            stat = 'R';
                        }
                        else if (chance <= 30)
                        {
                            stat = 'U';
                        }
                        else
                        {
                            stat = 'C';
                        }
                    }

                    // loyalty 1-2
                    else if ((this.loyalty > 1) && (this.loyalty <= 2))
                    {
                        if (chance <= 26)
                        {
                            stat = 'R';
                        }
                        else if (chance <= 50)
                        {
                            stat = 'U';
                        }
                        else
                        {
                            stat = 'C';
                        }
                    }

                    // loyalty 0-1
                    else if ((this.loyalty > 0) && (this.loyalty <= 1))
                    {
                        if (chance <= 38)
                        {
                            stat = 'R';
                        }
                        else if (chance <= 70)
                        {
                            stat = 'U';
                        }
                        else
                        {
                            stat = 'C';
                        }
                    }

                    // loyalty 0
                    else if (this.loyalty == 0)
                    {
                        if (chance <= 50)
                        {
                            stat = 'R';
                        }
                        else if (chance <= 90)
                        {
                            stat = 'U';
                        }
                        else
                        {
                            stat = 'C';
                        }
                    }
                }
            }

            return stat;
        }

        /// <summary>
        /// Adds character to characters list
        /// </summary>
        /// <param name="ch">Character to be inserted into characters list</param>
        internal void addCharacter(Character ch)
        {
            // add character
            this.characters.Add(ch);
        }

        /// <summary>
        /// removes character from characters list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character to be removed from characters list</param>
        internal bool removeCharacter(Character ch)
        {
            // remove character
            bool success = this.characters.Remove(ch);

            return success;
        }

        /// <summary>
        /// Bar a specific character from the fief's keep
        /// </summary>
        /// <param name="ch">Character to be barred</param>
        internal void barCharacter(string ch)
        {
            // bar character
            this.barredCharacters.Add(ch);
        }

        /// <summary>
        /// Removes a fief keep bar from a specific character
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character for whom bar to be removed</param>
        internal bool removeBarCharacter(string ch)
        {
            // remove character bar
            bool success = this.barredCharacters.Remove(ch);

            return success;
        }

    }

	/// <summary>
	/// Class used to convert Fief to/from format suitable for Riak (JSON)
	/// </summary>
	public class Fief_Riak
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
		/// Holds fief's Province object (provinceID)
		/// </summary>
		public String province { get; set; }
		/// <summary>
		/// Holds fief population
		/// </summary>
		public uint population { get; set; }
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
		/// Holds fief status (code)
		/// </summary>
		public char status { get; set; }
        /// <summary>
        /// Holds fief language and dialect
        /// </summary>
        public Tuple<String, int> language { get; set; }
        /// <summary>
        /// Holds terrain object (terrainCode)
		/// </summary>
		public String terrain { get; set; }
		/// <summary>
		/// Holds list of characters present in fief (charIDs)
		/// </summary>
		public List<String> characters = new List<String>();
		/// <summary>
		/// Holds list of characters banned from keep (charIDs)
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
        /// Holds fief's GameClock (clockID)
		/// </summary>
		public String clock { get; set; }
		/// <summary>
		/// Holds fief owner (charID)
		/// </summary>
		public String owner { get; set; }
		/// <summary>
		/// Holds fief ancestral owner (charID)
		/// </summary>
		public String ancestralOwner { get; set; }
		/// <summary>
		/// Holds fief bailiff (charID)
		/// </summary>
		public String bailiff { get; set; }

		/// <summary>
		/// Constructor for Fief_Riak
		/// </summary>
		/// <param name="f">Fief object to use as source</param>
		public Fief_Riak(Fief f)
		{
			this.fiefID = f.fiefID;
			this.name = f.name;
			this.province = f.province.provinceID;
			this.population = f.population;
			this.owner = f.owner.charID;
			this.ancestralOwner = f.ancestralOwner.charID;
			if (f.bailiff != null) {
				this.bailiff = f.bailiff.charID;
			} else {
				this.bailiff = null;
			}
			this.fields = f.fields;
			this.industry = f.industry;
			this.troops = f.troops;
			this.taxRate = f.taxRate;
			this.officialsSpend = f.officialsSpend;
			this.garrisonSpend = f.garrisonSpend;
			this.infrastructureSpend = f.infrastructureSpend;
			this.keepSpend = f.keepSpend;
			this.taxRateNext = f.taxRateNext;
			this.officialsSpendNext = f.officialsSpendNext;
			this.garrisonSpendNext = f.garrisonSpendNext;
			this.infrastructureSpendNext = f.infrastructureSpendNext;
			this.keepSpendNext = f.keepSpendNext;
			this.keepLevel = f.keepLevel;
			this.loyalty = f.loyalty;
			this.status = f.status;
            this.language = new Tuple<string,int>(f.language.Item1.languageID, f.language.Item2);
			this.terrain = f.terrain.terrainCode;
			if (f.characters.Count > 0)
			{
				for (int i = 0; i < f.characters.Count; i++)
				{
					this.characters.Add (f.characters[i].charID);
				}
			}
			this.barredCharacters = f.barredCharacters;
			this.englishBarred = f.englishBarred;
			this.frenchBarred = f.frenchBarred;
			this.clock = f.clock.clockID;
		}

        /// <summary>
        /// Constructor for Fief_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Fief_Riak()
		{
		}
	}
}
