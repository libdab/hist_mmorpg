using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on character
    /// </summary>
    public abstract class Character
    {

        /// <summary>
        /// Holds character ID
        /// </summary>
        public string charID { get; set; }
        /// <summary>
        /// Holds character name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds character age
        /// </summary>
        public uint age { get; set; }
        /// <summary>
        /// Holds if character male
        /// </summary>
        public bool isMale { get; set; }
        /// <summary>
        /// Holds character nationality
        /// </summary>
        public String nationality { get; set; }
        /// <summary>
        /// Holds character current health
        /// </summary>
        public Double health { get; set; }
        /// <summary>
        /// Holds character maximum health
        /// </summary>
        public Double maxHealth { get; set; }
        /// <summary>
        /// Holds character virility
        /// </summary>
        public Double virility { get; set; }
        /// <summary>
        /// Queue of Fiefs to auto-travel to
        /// </summary>
		public Queue<Fief> goTo = new Queue<Fief> ();
        /// <summary>
        /// Holds character language
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// Holds character's remaining days in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds character's stature
        /// </summary>
        public Double stature { get; set; }
        /// <summary>
        /// Holds character's management rating
        /// </summary>
        public Double management { get; set; }
        /// <summary>
        /// Holds character's combat rating
        /// </summary>
        public Double combat { get; set; }
        /// <summary>
        /// Array holding character's skills
        /// </summary>
        public Skill[] skills { get; set; }
        /// <summary>
        /// bool indicating if character is in the keep
        /// </summary>
        public bool inKeep { get; set; }
        /// <summary>
        /// Holds character marital status
        /// </summary>
        public bool married { get; set; }
        /// <summary>
        /// Holds character pregnancy status
        /// </summary>
        public bool pregnant { get; set; }
        /// <summary>
        /// Holds head of family (charID)
        /// </summary>
        public String familyHead { get; set; }
        /// <summary>
        /// Holds spouse (charID)
        /// </summary>
        public String spouse { get; set; }
        /// <summary>
        /// Holds father (CharID)
        /// </summary>
        public String father { get; set; }
        /// <summary>
        /// Holds current location (Fief object)
        /// </summary>
        public Fief location { get; set; }
        /// <summary>
        /// Holds army's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }

        /// <summary>
        /// Constructor for Character
        /// </summary>
        /// <param name="id">string holding character ID</param>
        /// <param name="nam">String holding character name</param>
        /// <param name="ag">uint holding character age</param>
        /// <param name="isM">bool holding if character male</param>
        /// <param name="nat">String holding character nationality</param>
        /// <param name="hea">Double holding character current health</param>
        /// <param name="mxHea">Double holding character maximum health</param>
        /// <param name="vir">Double holding character virility rating</param>
        /// <param name="go">Queue<Fief> of Fiefs to auto-travel to</param>
        /// <param name="lang">String holding character language code</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">Double holding character status rating</param>
        /// <param name="mngmnt">Double holding character management rating</param>
        /// <param name="cbt">Double holding character combat rating</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="marr">char holding character marital status</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famHead">String holding head of family (ID)</param>
        /// <param name="sp">String holding spouse (ID)</param>
        /// <param name="fath">String holding father</param>
        /// <param name="cl">GameClock holding season</param>
		/// <param name="loc">Fief holding current location</param>
        public Character(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Queue<Fief> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg,
            String famHead, String sp, String fath, GameClock cl = null, Fief loc = null)
        {

            // validation
            // TODO validate id = 1-10000?

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Character name must be between 1 and 40 characters in length");
            }

            // validate ag < 100
            if (ag > 99)
            {
                throw new InvalidDataException("Character age must be an integer between 0 and 100");
            }

			// validate preg = not if male
            if (isM)
            {
				this.pregnant = false;
            }

            // validate nat = Eng/Fr
            if ((!nat.Equals("Eng")) && (!nat.Equals("Fr")))
            {
                throw new InvalidDataException("Character nationality must be either 'Eng' or 'Fr'");
            }

            // validate hea = 1-9.00
            if ((hea < 1) || (hea > 9))
            {
                throw new InvalidDataException("Character health must be a double between 1 and 9");
            }

            // validate maxHea = 1-9.00
            if ((mxHea < 1) || (mxHea > 9))
            {
                throw new InvalidDataException("Character maximum health must be a double between 1 and 9");
            }

            // validate vir = 1-9.00
            if ((vir < 1) || (vir > 9))
            {
                throw new InvalidDataException("Character virility must be a double between 1 and 9");
            }


            // TODO: validate lang = string B,C,D,E,F,G,H,I,L/1-3

            // validate day = 0-90
            if ((day > 90) || (day < 0))
            {
                throw new InvalidDataException("Character remaining days must be a double between 0 and 90");
            }

            // validate stat = 0-9.00
            if (stat > 9)
            {
                throw new InvalidDataException("Character stature must be a double between 0 and 9");
            }

            // validate mngmnt = 0-9.00
            if (mngmnt > 9)
            {
                throw new InvalidDataException("Character stature must be a double between 0 and 9");
            }

            // validate cbt = 0-9.00
            if (cbt > 9)
            {
                throw new InvalidDataException("Character stature must be a double between 0 and 9");
            }

            // TODO: validate spID = 1-10000?
            // TODO: ensure married characters have a sp and unmarried ones don't? (but may initially be null)
            // TODO: validate fath ID = 1-10000?

            // TODO: validate famHead ID = 1-10000?

            this.charID = id;
            this.name = nam;
            this.age = ag;
            this.isMale = isM;
            this.nationality = nat;
            this.health = hea;
            this.maxHealth = mxHea;
            this.virility = vir;
            this.goTo = go;
            this.language = lang;
            this.days = day;
            this.stature = stat;
            this.management = mngmnt;
            this.combat = cbt;
            this.skills = skl;
            this.inKeep = inK;
            this.married = marr;
            this.pregnant = preg;
            this.clock = cl;
			this.location = loc;
            this.spouse = sp;
            this.father = fath;
            this.familyHead = famHead;
        }

		/// <summary>
		/// Constructor for Character using PlayerCharacter_Riak or NonPlayerCharacter_Riak object
		/// </summary>
		/// <param name="pcr">PlayerCharacter_Riak object to use as source</param>
		/// <param name="npcr">NonPlayerCharacter_Riak object to use as source</param>
		public Character(PlayerCharacter_Riak pcr = null, NonPlayerCharacter_Riak npcr = null)
		{
			Character_Riak charToUse = null;

			if (pcr != null)
			{
				charToUse = pcr;
			}
			else if (npcr != null)
			{
				charToUse = npcr;
			}

			if (charToUse != null)
			{
				this.charID = charToUse.charID;
				this.name = charToUse.name;
				this.age = charToUse.age;
				this.isMale = charToUse.isMale;
				this.nationality = charToUse.nationality;
				this.health = charToUse.health;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
				this.goTo = new Queue<Fief> ();
				this.language = charToUse.language;
				this.days = charToUse.days;
				this.stature = charToUse.stature;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
				this.skills = new Skill[charToUse.skills.Length];
				this.inKeep = charToUse.inKeep;
				this.married = charToUse.married;
				this.pregnant = charToUse.pregnant;
				if ((charToUse.spouse != null) && (charToUse.spouse.Length > 0))
				{
					this.spouse = charToUse.spouse;
				}
				if ((charToUse.father != null) && (charToUse.father.Length > 0))
				{
					this.father = charToUse.father;
				}
				if ((charToUse.familyHead != null) && (charToUse.familyHead.Length > 0))
				{
					this.familyHead = charToUse.familyHead;
				}
				this.clock = null;
				this.location = null;
			}
		}

        /// <summary>
        /// Checks for character death
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        public Boolean checkDeath()
        {

            // Check if chance of death effected by character skills
            double deathSkillsModifier = this.getDeathSkillsMod();

            // calculate base chance of death
            Double deathChance = (10 - this.health) * 2.8;

            // apply skills modifier (if exists)
            if (deathSkillsModifier != 0)
            {
                deathChance = deathChance + (deathChance * deathSkillsModifier);
            }

            // generate a rndom double between 0-100 and compare to deathChance
            Random rand = new Random();
            if ((rand.NextDouble() * 100) <= deathChance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks for skills death modifier
        /// </summary>
        /// <returns>double containing death modifier</returns>
        public double getDeathSkillsMod()
        {
            double deathModifier = 0;

            for (int i = 0; i < skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in skills[i].effects)
                {
                    if (entry.Key.Equals("death"))
                    {
                        deathModifier += entry.Value;
                    }
                }
            }

            if (deathModifier != 0)
            {
                deathModifier = (deathModifier / 100);
            }

            return deathModifier;
        }

        /* 
        /// <summary>
        /// Calculates fief manager rating based on characteristics/skills
        /// </summary>
        /// <returns>double containing death modifier</returns>
        public double getFiefMngRating()
        {
            double mngRating = 0;

            for (int i = 0; i < skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in skills[i].effects)
                {
                    if (entry.Key.Equals("death"))
                    {
                        deathModifier += entry.Value;
                    }
                }
            }

            if (deathModifier != 0)
            {
                deathModifier = (deathModifier / 100);
            }

            return deathModifier;
        } */

        /// <summary>
        /// Enables character to enter keep (if not barred)
        /// </summary>
        /// <returns>bool indicating success</returns>
        public virtual bool enterKeep()
        {
            bool success = true;

                
            if (location.englishBarred)                
            {                    
                if (this.nationality.Equals("Eng"))                    
                {                       
                    success = false;                       
                }               
            }

            else if (location.frenchBarred)
            {
                if (this.nationality.Equals("Fr"))
                {
                    success = false;
                }
            }

            else 
            {
                if (location.barredCharacters.Contains(this.charID))
                {
                    success = false;
                }

            }

            this.inKeep = success;
            return success;
        }

        /// <summary>
        /// Enables character to exit keep
        /// </summary>
        public virtual void exitKeep()
        {

            this.inKeep = false;
        }

        /// <summary>
        /// Calculates effect of character's stats on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcFiefIncMod()
        {
            double incomeModif = 0;
            incomeModif = (this.management - 1) * 2.5;
            incomeModif = incomeModif / 100;
            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of character's stats on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcFiefLoyMod()
        {
            double loyModif = 0;
            loyModif = (((this.stature + this.management) / 2) - 1) * 1.25;
            loyModif = loyModif / 100;
            return loyModif;
        }

        /// <summary>
        /// Calculates effect of character skills on fief expenses
        /// </summary>
        /// <returns>double containing fief expenses modifier</returns>
        public double calcFiefExpModif()
        {
            double fiefExpModifier = 0;

            for (int i = 0; i < this.skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in this.skills[i].effects)
                {
                    if (entry.Key.Equals("fiefExpense"))
                    {
                        fiefExpModifier += entry.Value;
                    }
                }
            }

            if (fiefExpModifier != 0)
            {
                fiefExpModifier = fiefExpModifier / 100;
            }

            return fiefExpModifier;
        }

        /// <summary>
        /// Calculates effect of character skills on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double calcFiefLoySkillMod()
        {
            double loySkillsModifier = 0;

            for (int i = 0; i < this.skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in this.skills[i].effects)
                {
                    if (entry.Key.Equals("fiefLoy"))
                    {
                        loySkillsModifier += entry.Value;
                    }
                }
            }

            if (loySkillsModifier != 0)
            {
                loySkillsModifier = (loySkillsModifier / 100);
            }

            return loySkillsModifier;
        }

        /// <summary>
        /// Calculates effect of character skills on army leadership value in battle
        /// </summary>
        /// <returns>double containing battle modifier</returns>
        public double calcBattleSkillMod()
        {
            double battleSkillsModifier = 0;

            for (int i = 0; i < this.skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in this.skills[i].effects)
                {
                    if (entry.Key.Equals("battle"))
                    {
                        battleSkillsModifier += entry.Value;
                    }
                }
            }

            if (battleSkillsModifier != 0)
            {
                battleSkillsModifier = (battleSkillsModifier / 100);
            }

            return battleSkillsModifier;
        }

        /// <summary>
        /// Calculates effect of character skills on army leadership value in siege
        /// </summary>
        /// <returns>double containing siege modifier</returns>
        public double calcSiegeSkillMod()
        {
            double siegeSkillsModifier = 0;

            for (int i = 0; i < this.skills.Length; i++)
            {
                foreach (KeyValuePair<string, int> entry in this.skills[i].effects)
                {
                    if (entry.Key.Equals("siege")) 
                    {
                        siegeSkillsModifier += entry.Value;
                    }
                }
            }

            if (siegeSkillsModifier != 0)
            {
                siegeSkillsModifier = (siegeSkillsModifier / 100);
            }

            return siegeSkillsModifier;
        }

        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        public virtual bool moveCharacter(Fief target, double cost)
        {
            bool success = false;

            if (this.days >= cost)
            {
                this.location.removeCharacter(this);
                this.location = target;
                this.location.addCharacter(this);
                this.inKeep = false;
                this.days = this.days - cost;
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Updates character data at the end/beginning of the season
        /// </summary>
        public void updateCharacter()
        {
            // check for character death
            if (this.checkDeath())
            {
                this.health = 0;
            }
            // advance character age
            // adjust stature (due to age)
            // childbirth (and associated chance of death for mother/baby)
            // Movement of NPCs (maybe this goes in NPC class)
        }


    }

    /// <summary>
    /// Class storing data on player character
    /// </summary>
    public class PlayerCharacter : Character
    {

        /// <summary>
        /// Holds character outlawed status
        /// </summary>
        public bool outlawed { get; set; }
        /// <summary>
        /// Holds character's finances
        /// </summary>
        public uint purse { get; set; }
        /// <summary>
        /// Holds character's entourage
        /// </summary>
        public List<NonPlayerCharacter> employees = new List<NonPlayerCharacter>();
        /// <summary>
        /// Holds character's entourage
        /// </summary>
        public List<Fief> ownedFiefs = new List<Fief>();

        /// <summary>
        /// Constructor for PlayerCharacter
        /// </summary>
        /// <param name="outl">bool holding character outlawed status</param>
        /// <param name="pur">uint holding character purse</param>
        /// <param name="emp">List<NonPlayerCharacter> holding employees of character</param>
        /// <param name="kps">List<Fief> holding fiefs owned by character</param>
        public PlayerCharacter(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Queue<Fief> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg, String famHead,
            String sp, String fath, bool outl, uint pur, List<NonPlayerCharacter> emp, List<Fief> kps, GameClock cl = null, Fief loc = null)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, famHead, sp, fath, cl, loc)
        {

            this.outlawed = outl;
            this.purse = pur;
            this.employees = emp;
            this.ownedFiefs = kps;
        }

		public PlayerCharacter()
		{
		}

		/// <summary>
		/// Constructor for PlayerCharacter using PlayerCharacter_Riak object
		/// </summary>
		/// <param name="pcr">PlayerCharacter_Riak object to use as source</param>
		public PlayerCharacter(PlayerCharacter_Riak pcr)
			: base(pcr: pcr)
		{

			this.outlawed = pcr.outlawed;
			this.purse = pcr.purse;
			this.employees = new List<NonPlayerCharacter> ();
			this.ownedFiefs = new List<Fief> ();
		}

        /// <summary>
        /// Processes an offer for employment
        /// </summary>
        /// <returns>bool containing wage</returns>
        public bool processEmployOffer(NonPlayerCharacter npc, uint offer)
        {
            bool accepted = false;
            // get NPC's potential salary
            double potentialSalary = npc.calcNPCwage();
            // generate random (0 - 100) to see if accepts offer
            Random rand = new Random();
            double chance = rand.NextDouble() * 100;

            // get range of acceptable offers
            // minimum = 90% of potential salary
            double minAcceptable = potentialSalary - (potentialSalary / 10);
            // maximum = 110% of potential salary
            double maxAcceptable = potentialSalary + (potentialSalary / 10);
            // get increments
            double increment = (maxAcceptable - minAcceptable) / 10;

            // ensure this offer is more than the last from this PC
            bool offerLess = false;
            if (npc.lastOffer.ContainsKey(this.charID))
            {
                if (!(offer > npc.lastOffer[this.charID]))
                {
                    offerLess = true;
                }
                else
                {
                    npc.lastOffer[this.charID] = offer;
                }
            }
            else
            {
                npc.lastOffer.Add(this.charID, offer);
            }

            if (offerLess)
            {
                accepted = false;
            }
            else if (offer > maxAcceptable)
            {
                accepted = true;
            }
            else if (offer < minAcceptable)
            {
                accepted = false;
            }

            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if ((offer >= minAcceptable + (increment * (i))) && (offer < minAcceptable + (increment * (i + 1))))
                    {
                        if (chance <= 10 * (i + 1))
                        {
                            accepted = true;
                        }

                        break;
                    }
                }
            }

            if (accepted)
            {
                this.hireNPC(npc, offer);
                /* npc.myBoss = this;
                npc.wage = offer;
                npc.lastOffer.Clear(); */
            }

            return accepted;
        }

        /// <summary>
        /// Adds an NPC to PC's employees list
        /// </summary>
        /// <param name="npc">NPC to hire</param>
        /// <param name="wage">NPC's wage</param>
        public void hireNPC(NonPlayerCharacter npc, uint wage)
        {
            this.employees.Add(npc);
            npc.wage = wage;
            npc.myBoss = this.charID;
            npc.lastOffer.Clear();
        }

        /// <summary>
        /// Removes an NPC from PC's employees list
        /// </summary>
        /// <param name="npc">NPC to fire</param>
        public void fireNPC(NonPlayerCharacter npc)
        {
            this.employees.Remove(npc);
            npc.wage = 0;
            npc.myBoss = null;
            npc.inEntourage = false;
        }

        /// <summary>
        /// Adds an NPC to the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be added</param>
        public void addToEntourage(NonPlayerCharacter npc)
        {
            // ensure days is set to lesser of PC/NPC days
            double minDays = Math.Min(this.days, npc.days);
            this.days = minDays;
            npc.days = minDays;
            npc.inEntourage = true;
        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be removed</param>
        public void removeFromEntourage(NonPlayerCharacter npc)
        {
            npc.inEntourage = false;
        }

        /// <summary>
        /// Adds a Fief to the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be added</param>
        public void addToOwnedFiefs(Fief f)
        {
            this.ownedFiefs.Add(f);
        }

        /// <summary>
        /// Removes a Fief from the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be removed</param>
        public void removeFromOwnedFiefs(Fief f)
        {
            this.ownedFiefs.Remove(f);
        }

        /// <summary>
        /// Calls base method to enable boss character to enter keep (if not barred)
        /// Also moves entourage (if not individually barred) - ignores nationality bar
        /// if boss character allowed to enter
        /// </summary>
        /// <returns>bool indicating success</returns>
        public override bool enterKeep()
        {
            bool success = base.enterKeep();

            if (success)
            {
                for (int i = 0; i < this.employees.Count; i++)
                {
                    if (this.employees[i].inEntourage)
                    {
                        if (location.barredCharacters.Contains(this.employees[i].charID))
                        {
                            this.employees[i].inKeep = false;
                        }
                        else
                        {
                            this.employees[i].inKeep = true;
                        }
                    }

                }
                
            }

            return success;

        }

        /// <summary>
        /// Calls base method to enable character to exit keep, then moves entourage
        /// </summary>
        public override void exitKeep()
        {
            base.exitKeep();

            for (int i = 0; i < this.employees.Count; i++)
            {
                if (this.employees[i].inEntourage)
                {
                    this.employees[i].inKeep = false;
                }
            }
        }

        /// <summary>
        /// Moves character's entourage to target fief
        /// </summary>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        public override bool moveCharacter(Fief target, double cost)
        {

            // use base method to move character
            bool success = base.moveCharacter(target, cost);

            if (success)
            {
                for (int i = 0; i < this.employees.Count; i++)
                {
                    if (this.employees[i].inEntourage)
                    {
                        this.employees[i].moveCharacter(target, cost);
                    }
                }
            }

            return success;

        }

    }

    /// <summary>
    /// Class storing data on non player character
    /// </summary>
    public class NonPlayerCharacter : Character
    {

        /// <summary>
        /// Holds NPC's boss (charID)
        /// </summary>
        public String myBoss { get; set; }
        /// <summary>
        /// Holds NPC's wages
        /// </summary>
        public uint wage { get; set; }
        /// <summary>
        /// Holds last wage offer
        /// </summary>
        public Dictionary<string, uint> lastOffer { get; set; }
        /// <summary>
        /// Denotes if in/out of boss's entourage
        /// </summary>
        public bool inEntourage { get; set; }

        /// <summary>
        /// Constructor for NonPlayerCharacter
        /// </summary>
        /// <param name="mb">String holding NPC's boss (ID)</param>
        /// <param name="go">String holding fief ID for destination (specified by NPC's boss)</param>
        /// <param name="wa">string holding NPC's wages</param>
        /// <param name="inEnt">bool denoting if in/out of boss's entourage</param>
        public NonPlayerCharacter(String id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Queue<Fief> go, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg, String famHead,
            String sp, String fath, uint wa, bool inEnt, String mb = null, GameClock cl = null, Fief loc = null)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, famHead, sp, fath, cl, loc)
        {
            // TODO: validate hb = 1-10000
            // TODO: validate go = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // TODO: validate wa = uint

            this.myBoss = mb;
            this.wage = wa;
            this.inEntourage = inEnt;
            this.lastOffer = new Dictionary<string, uint>();
        }

		public NonPlayerCharacter()
		{
		}

		/// <summary>
		/// Constructor for NonPlayerCharacter using NonPlayerCharacter_Riak object
		/// </summary>
		/// <param name="npcr">NonPlayerCharacter_Riak object to use as source</param>
		public NonPlayerCharacter(NonPlayerCharacter_Riak npcr)
			: base(npcr: npcr)
		{
			if ((npcr.myBoss != null) && (npcr.myBoss.Length > 0))
			{
				this.myBoss = npcr.myBoss;
			}
			this.wage = npcr.wage;
			this.inEntourage = npcr.inEntourage;
			this.lastOffer = npcr.lastOffer;
		}

        /// <summary>
        /// Calculates potential salary (per season) for NPC, taking into account hiring player's stature
        /// </summary>
        /// <returns>uint containing salary</returns>
        public uint calcNPCwage()
        {
            double salary = 0;
            double basicSalary = 1500;

            // calculate management rating
            double fiefMgt = (this.management + this.stature) / 2;
            double fiefLoySkills = this.calcFiefLoySkillMod();
            double fiefExpSkill = this.calcFiefExpModif();
            double mgtSkills = (fiefLoySkills + (-1 * fiefExpSkill));
            fiefMgt = fiefMgt + (fiefMgt * mgtSkills);

            // calculate combat rating
            double combat = (this.management + this.stature + this.combat) / 3;
            double battleSkills = this.calcBattleSkillMod();
            double siegeSkills = this.calcSiegeSkillMod();
            double combatSkills = battleSkills + siegeSkills;
            combat = combat + (combat * combatSkills);

            if (fiefMgt > combat)
            {
                salary = (basicSalary * fiefMgt) + (basicSalary * (combat / 2));
            }
            else
            {
                salary = (basicSalary * combat) + (basicSalary * (fiefMgt / 2));
            }

            // factor in hiring player's stature
            if (this.stature > 4)
            {
                double statMod = 1 - ((this.stature - 4) * 0.04);
                salary = salary * statMod;
            }

            return Convert.ToUInt32(salary);
        }

    }

	/// <summary>
	/// Class used to convert Character to/from format suitable for Riak
	/// </summary>
	public class Character_Riak
	{

		/// <summary>
		/// Holds character ID
		/// </summary>
		public string charID { get; set; }
		/// <summary>
		/// Holds character name
		/// </summary>
		public String name { get; set; }
		/// <summary>
		/// Holds character age
		/// </summary>
		public uint age { get; set; }
		/// <summary>
		/// Holds if character male
		/// </summary>
		public bool isMale { get; set; }
		/// <summary>
		/// Holds character nationality
		/// </summary>
		public String nationality { get; set; }
		/// <summary>
		/// Holds character current health
		/// </summary>
		public Double health { get; set; }
		/// <summary>
		/// Holds character maximum health
		/// </summary>
		public Double maxHealth { get; set; }
		/// <summary>
		/// Holds character virility
		/// </summary>
		public Double virility { get; set; }
		/// <summary>
		/// Queue of Fiefs to auto-travel to
		/// </summary>
		public List<String> goTo { get; set; }
		/// <summary>
		/// Holds character language
		/// </summary>
		public string language { get; set; }
		/// <summary>
		/// Holds character's remaining days in season
		/// </summary>
		public double days { get; set; }
		/// <summary>
		/// Holds character's stature
		/// </summary>
		public Double stature { get; set; }
		/// <summary>
		/// Holds character's management rating
		/// </summary>
		public Double management { get; set; }
		/// <summary>
		/// Holds character's combat rating
		/// </summary>
		public Double combat { get; set; }
		/// <summary>
		/// Array holding character's skills
		/// </summary>
		public String[] skills { get; set; }
		/// <summary>
		/// bool indicating if character is in the keep
		/// </summary>
		public bool inKeep { get; set; }
		/// <summary>
		/// Holds character marital status
		/// </summary>
		public bool married { get; set; }
		/// <summary>
		/// Holds character pregnancy status
		/// </summary>
		public bool pregnant { get; set; }
		/// <summary>
		/// Holds army's GameClock (season)
		/// </summary>
		public GameClock clock { get; set; }
		/// <summary>
		/// Holds current location (Fief ID)
		/// </summary>
		public String location { get; set; }
		/// <summary>
		/// Holds spouse (Character ID)
		/// </summary>
		public String spouse { get; set; }
		/// <summary>
		/// Holds father (Character ID)
		/// </summary>
		public String father { get; set; }
		/// <summary>
		/// Holds head of family (Character ID)
		/// </summary>
		public String familyHead { get; set; }

		/// <summary>
		/// Constructor for Character_Riak
		/// </summary>
		/// <param name="pc">PlayerCharacter object to use as source</param>
		/// <param name="npc">NonPlayerCharacter object to use as source</param>
		public Character_Riak(PlayerCharacter pc = null, NonPlayerCharacter npc = null)
		{
			Character charToUse = null;

			if (pc != null)
			{
				charToUse = pc;
			}
			else if (npc != null)
			{
				charToUse = npc;
			}

			if (charToUse != null)
			{
				this.charID = charToUse.charID;
				this.name = charToUse.name;
				this.age = charToUse.age;
				this.isMale = charToUse.isMale;
				this.nationality = charToUse.nationality;
				this.health = charToUse.health;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
				this.goTo = new List<string> ();
				if (charToUse.goTo.Count > 0)
				{
					foreach (Fief value in charToUse.goTo)
					{
						this.goTo.Add (value.fiefID);
					}
				}
				this.language = charToUse.language;
				this.days = charToUse.days;
				this.stature = charToUse.stature;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
				this.skills = new String[charToUse.skills.Length];
				for (int i = 0; i < charToUse.skills.Length; i++)
				{
					this.skills [i] = charToUse.skills [i].skillID;
				}
				this.inKeep = charToUse.inKeep;
				this.married = charToUse.married;
				this.pregnant = charToUse.pregnant;
				this.clock = null;
				this.location = charToUse.location.fiefID;
				if (charToUse.spouse != null) {
					this.spouse = charToUse.spouse;
				} else {
					this.spouse = null;
				}
				if (charToUse.father != null) {
					this.father = charToUse.father;
				} else {
					this.father = null;
				}
				if (charToUse.familyHead != null) {
					this.familyHead = charToUse.familyHead;
				} else {
					this.familyHead = null;
				}
			}
		}

	}

	/// <summary>
	/// Class used to convert PlayerCharacter to/from format suitable for Riak
	/// </summary>
	public class PlayerCharacter_Riak : Character_Riak
	{

		/// <summary>
		/// Holds character outlawed status
		/// </summary>
		public bool outlawed { get; set; }
		/// <summary>
		/// Holds character's finances
		/// </summary>
		public uint purse { get; set; }
		/// <summary>
		/// Holds character's entourage
		/// </summary>
		public List<String> employees = new List<String>();
		/// <summary>
		/// Holds character's entourage
		/// </summary>
		public List<String> ownedFiefs = new List<String>();

		/// <summary>
		/// Constructor for PlayerCharacter_Riak
		/// </summary>
		/// <param name="pc">PlayerCharacter object</param>
		public PlayerCharacter_Riak(PlayerCharacter pc)
			: base(pc: pc)
		{

			this.outlawed = pc.outlawed;
			this.purse = pc.purse;
			if (pc.employees.Count > 0)
			{
				for (int i = 0; i < pc.employees.Count; i++)
				{
					this.employees.Add (pc.employees[i].charID);
				}
			}
			if (pc.ownedFiefs.Count > 0)
			{
				for (int i = 0; i < pc.ownedFiefs.Count; i++)
				{
					this.ownedFiefs.Add (pc.ownedFiefs[i].fiefID);
				}
			}
		}

		public PlayerCharacter_Riak()
		{
		}
	}

	/// <summary>
	/// Class used to convert NonPlayerCharacter to/from format suitable for Riak
	/// </summary>
	public class NonPlayerCharacter_Riak : Character_Riak
	{

		/// <summary>
		/// Holds NPC's boss
		/// </summary>
		public String myBoss { get; set; }
		/// <summary>
		/// Holds NPC's wages
		/// </summary>
		public uint wage { get; set; }
		/// <summary>
		/// Holds last wage offer
		/// </summary>
		public Dictionary<string, uint> lastOffer { get; set; }
		/// <summary>
		/// Denotes if in/out of boss's entourage
		/// </summary>
		public bool inEntourage { get; set; }


		/// <summary>
		/// Constructor for NonPlayerCharacter_Riak
		/// </summary>
		/// <param name="npc">NonPlayerCharacter object</param>
		public NonPlayerCharacter_Riak(NonPlayerCharacter npc)
			: base(npc: npc)
		{

			if (npc.myBoss != null)
			{
				this.myBoss = npc.myBoss;
			}
			this.wage = npc.wage;
			this.inEntourage = npc.inEntourage;
			this.lastOffer = npc.lastOffer;
		}

		public NonPlayerCharacter_Riak()
		{
		}
	}



}
