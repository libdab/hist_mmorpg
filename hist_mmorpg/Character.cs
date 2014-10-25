using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on character (PC and NPC)
    /// </summary>
    public abstract class Character
    {

        /// <summary>
        /// Holds character ID
        /// </summary>
        public string charID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
        public String firstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public String familyName { get; set; }
        /// <summary>
        /// Tuple holding character's year and season of birth
        /// </summary>
        public Tuple<uint, byte> birthDate { get; set; }
        /// <summary>
        /// Holds if character male
        /// </summary>
        public bool isMale { get; set; }
        /// <summary>
        /// Holds character nationality
        /// </summary>
        public String nationality { get; set; }
        /// <summary>
        /// bool indicating whether character is alive
        /// </summary>
        public bool isAlive { get; set; }
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
        /// Holds character's language and dialect
        /// </summary>
        public Tuple<Language, int> language { get; set; }
        /// <summary>
        /// Holds character's remaining days in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds modifier to character's base stature
        /// </summary>
        public Double statureModifier { get; set; }
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
        public Tuple<Skill, int>[] skills { get; set; }
        /// <summary>
        /// bool indicating if character is in the keep
        /// </summary>
        public bool inKeep { get; set; }
        /// <summary>
        /// Holds character pregnancy status
        /// </summary>
        public bool isPregnant { get; set; }
        /// <summary>
        /// Holds charID of head of family with which character associated
        /// </summary>
        public String familyID { get; set; }
        /// <summary>
        /// Holds spouse (charID)
        /// </summary>
        public String spouse { get; set; }
        /// <summary>
        /// Holds father (CharID)
        /// </summary>
        public String father { get; set; }
        /// <summary>
        /// Holds mother (CharID)
        /// </summary>
        public String mother { get; set; }
        /// <summary>
        /// Hold fiancee (charID)
        /// </summary>
        public string fiancee { get; set; }
        /// <summary>
        /// Holds current location (Fief object)
        /// </summary>
        public Fief location { get; set; }
        /// <summary>
        /// Holds character's titles (fiefIDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }
        /// <summary>
        /// Holds ailments effecting character's health
        /// </summary>
        public Dictionary<string, Ailment> ailments = new Dictionary<string, Ailment>();

        /// <summary>
        /// Constructor for Character
        /// </summary>
        /// <param name="id">string holding character ID</param>
        /// <param name="firstNam">String holding character's first name</param>
        /// <param name="famNam">String holding character's family name</param>
        /// <param name="dob">Tuple<uint, byte> holding character's year and season of birth</param>
        /// <param name="isM">bool holding if character male</param>
        /// <param name="nat">String holding character nationality</param>
        /// <param name="alive">bool indicating whether character is alive</param>
        /// <param name="mxHea">Double holding character maximum health</param>
        /// <param name="vir">Double holding character virility rating</param>
        /// <param name="go">Queue<Fief> of Fiefs to auto-travel to</param>
        /// <param name="lang">Tuple<Language, int> holding character language and dialect</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">Double holding character stature rating</param>
        /// <param name="mngmnt">Double holding character management rating</param>
        /// <param name="cbt">Double holding character combat rating</param>
        /// <param name="skl">Array containing character's skills</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famID">String holding charID of head of family with which character associated</param>
        /// <param name="sp">String holding spouse (charID)</param>
        /// <param name="fath">String holding father (charID)</param>
        /// <param name="moth">String holding mother (charID)</param>
        /// <param name="fia">Holds fiancee (charID)</param>
        /// <param name="loc">Fief holding current location</param>
        /// <param name="myTi">List holding character's titles (fiefIDs)</param>
        /// <param name="aID">String holding armyID of army character is leading</param>
        /// <param name="ails">Dictionary<string, Ailment> holding ailments effecting character's health</param>
        public Character(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, String nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool preg,
            String famID, String sp, String fath, String moth, List<String> myTi, string fia, Dictionary<string, Ailment> ails = null, Fief loc = null, String aID = null)
        {

            // validation
            // TODO validate id = 1-10000?

            // validate firstNam length = 2-40
            if ((firstNam.Length < 2) || (firstNam.Length > 40))
            {
                throw new InvalidDataException("Character first name must be between 2 and 40 characters in length");
            }

            // validate famNam length = 2-40
            if ((famNam.Length < 2) || (famNam.Length > 40))
            {
                throw new InvalidDataException("Character family name must be between 2 and 40 characters in length");
            }

            // TODO: validate dob

            // validate preg = not if male
            if (isM)
            {
				this.isPregnant = false;
            }

            //  TODO: validate nat

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
            this.firstName = firstNam;
            this.familyName = famNam;
            this.birthDate = dob;
            this.isMale = isM;
            this.nationality = nat;
            this.isAlive = alive;
            this.maxHealth = mxHea;
            this.virility = vir;
            this.goTo = go;
            this.language = lang;
            this.days = day;
            this.statureModifier = stat;
            this.management = mngmnt;
            this.combat = cbt;
            this.skills = skl;
            this.inKeep = inK;
            this.isPregnant = preg;
			this.location = loc;
            this.spouse = sp;
            this.father = fath;
            this.mother = moth;
            this.familyID = famID;
            this.myTitles = myTi;
            this.armyID = aID;
            if (ails != null)
            {
                this.ailments = ails;
            }
            this.fiancee = fia;
        }

		/// <summary>
		/// Constructor for Character using PlayerCharacter_Riak or NonPlayerCharacter_Riak object.
        /// For use when de-serialising from Riak
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
				this.firstName = charToUse.firstName;
                this.familyName = charToUse.familyName;
                this.birthDate = charToUse.birthDate;
				this.isMale = charToUse.isMale;
				this.nationality = charToUse.nationality;
                this.isAlive = charToUse.isAlive;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
                // create empty Queue, to be populated later
                this.goTo = new Queue<Fief>();
				this.language = null;
				this.days = charToUse.days;
				this.statureModifier = charToUse.statureModifier;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
                // create empty array, to be populated later
                this.skills = new Tuple<Skill, int>[charToUse.skills.Length];
				this.inKeep = charToUse.inKeep;
				this.isPregnant = charToUse.pregnant;
                this.spouse = charToUse.spouse;
                this.father = charToUse.father;
                this.mother = charToUse.mother;
                this.familyID = charToUse.familyID;
                this.location = null;
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
                this.ailments = charToUse.ailments;
                this.fiancee = charToUse.fiancee;
			}
		}

        /// <summary>
        /// Constructor for Character using NonPlayerCharacter object,
        /// for use when respawning deceased NPCs or promoting NPC to PC (after PC death)
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        /// <param name="circumstance">The circumstance - respawn or promotion</param>
        public Character(NonPlayerCharacter npc, string circumstance, List<string> pcTitles = null)
        {
            switch (circumstance)
            {
                case "respawn":
                    this.charID = Convert.ToString(Globals_Server.getNextCharID());
                    this.birthDate = new Tuple<uint, byte>(Globals_Server.clock.currentYear - 20, Globals_Server.clock.currentSeason);
                    this.maxHealth = Globals_Server.myRand.Next(1, 10);
                    this.goTo = new Queue<Fief>();
                    this.days = 90;
                    this.statureModifier = 0;
                    this.inKeep = false;
                    this.isPregnant = false;
                    this.spouse = null;
                    this.father = null;
                    this.mother = null;
                    this.familyID = null;
                    this.myTitles = new List<string>();
                    this.armyID = null;
                    this.ailments = new Dictionary<string, Ailment>();
                    this.fiancee = null;
                    break;
                case "promote":
                    this.charID = npc.charID;
                    this.birthDate = npc.birthDate;
                    this.maxHealth = npc.maxHealth;
                    this.goTo = npc.goTo;
                    this.days = npc.days;
                    this.statureModifier = npc.statureModifier;
                    this.inKeep = npc.inKeep;
                    this.isPregnant = npc.isPregnant;
                    this.spouse = npc.spouse;
                    this.father = npc.father;
                    this.mother = npc.mother;
                    this.familyID = npc.charID;
                    this.myTitles = npc.myTitles;
                    if (pcTitles != null)
                    {
                        foreach (string thisTitle in pcTitles)
                        {
                            this.myTitles.Add(thisTitle);
                        }
                    }
                    this.armyID = npc.armyID;
                    this.ailments = npc.ailments;
                    this.fiancee = npc.fiancee;
                    break;
                default:
                    break;
            }

            this.firstName = npc.firstName;
            this.familyName = npc.familyName;
            this.isMale = npc.isMale;
            this.nationality = npc.nationality;
            this.isAlive = true;
            this.virility = npc.virility;
            this.language = npc.language;
            this.management = npc.management;
            this.combat = npc.combat;
            this.skills = new Tuple<Skill, int>[npc.skills.Length];
            for (int i = 0; i < npc.skills.Length; i++)
            {
                this.skills[i] = npc.skills[i];
            }
            this.location = npc.location;
        }

        /// <summary>
        /// Calculates character's age
        /// </summary>
        /// <returns>int containing character's age</returns>
        public int calcCharAge()
        {
            int myAge = 0;

            // subtract year of birth from current year
            myAge = Convert.ToByte(Globals_Server.clock.currentYear - this.birthDate.Item1);
            // if current season < season of birth, subtract 1 from age (not reached birthday yet)
            if (Globals_Server.clock.currentSeason < this.birthDate.Item2)
            {
                myAge--;
            }

            return myAge;
        }

        /// <summary>
        /// Retrieves stature associated with character's highest rank
        /// </summary>
        /// <returns>byte containing stature associated with character's highest rank</returns>
        public byte getHighRankStature()
        {
            byte highRankStature = 0;

            foreach (String fiefID in this.myTitles)
            {
                if (Globals_Server.fiefMasterList[fiefID].rank.stature > highRankStature)
                {
                    highRankStature = Globals_Server.fiefMasterList[fiefID].rank.stature;
                }
            }

            return highRankStature;
        }
       
        /// <summary>
        /// Calculates character's base or current stature
        /// </summary>
        /// <returns>Double containing character's base stature</returns>
        /// <param name="type">bool indicating whether to return current stature (or just base)</param>
        public Double calculateStature(bool currentStature = true)
        {
            Double stature = 0;

            // get stature for character's highest rank
            stature = this.getHighRankStature();

            // factor in age
            if (this.calcCharAge() <= 10)
            {
                stature += 0;
            }
            else if ((this.calcCharAge() > 10) && (this.calcCharAge() < 21))
            {
                stature += 0.5;
            }
            else if (this.calcCharAge() < 31)
            {
                stature += 1;
            }
            else if (this.calcCharAge() < 41)
            {
                stature += 2;
            }
            else if (this.calcCharAge() < 51)
            {
                stature += 3;
            }
            else if (this.calcCharAge() < 61)
            {
                stature += 4;
            }
            else
            {
                stature += 5;
            }

            // factor in sex (it's a man's world)
            if (!this.isMale)
            {
                stature -= 6;
            }

            // ensure doesn't exceed boundaries
            if (stature < 0)
            {
                stature = 0;
            }
            else if (stature > 9)
            {
                stature = 9;
            }

            // factor in character's current statureModifier if required
            if (currentStature)
            {
                stature += this.statureModifier;
            }

            return stature;
        }

        /// <summary>
        /// Calculates character's base or current health
        /// </summary>
        /// <returns>Double containing character's health</returns>
        /// <param name="currentHealth">bool indicating whether to return current health</param>
        public double calculateHealth(bool currentHealth = true)
        {

            double charHealth = 0;
            double ageModifier = 0;

            // calculate health age modifier, based on age
            if (this.calcCharAge() < 1)
            {
                ageModifier = 0.25;
            }
            else if (this.calcCharAge() < 5)
            {
                ageModifier = 0.5;
            }
            else if (this.calcCharAge() < 10)
            {
                ageModifier = 0.8;
            }
            else if (this.calcCharAge() < 20)
            {
                ageModifier = 0.9;
            }
            else if (this.calcCharAge() < 35)
            {
                ageModifier = 1;
            }
            else if (this.calcCharAge() < 40)
            {
                ageModifier = 0.95;
            }
            else if (this.calcCharAge() < 45)
            {
                ageModifier = 0.9;
            }
            else if (this.calcCharAge() < 50)
            {
                ageModifier = 0.85;
            }
            else if (this.calcCharAge() < 55)
            {
                ageModifier = 0.75;
            }
            else if (this.calcCharAge() < 60)
            {
                ageModifier = 0.65;
            }
            else if (this.calcCharAge() < 70)
            {
                ageModifier = 0.55;
            }
            else
            {
                ageModifier = 0.35;
            }

            // calculate health based on maxHealth and health age modifier
            charHealth = (this.maxHealth * ageModifier);

            // factor in current health modifers if appropriate
            if (currentHealth)
            {
                foreach (KeyValuePair<string, Ailment> ailment in this.ailments)
                {
                    charHealth -= ailment.Value.effect;
                }
            }

            return charHealth;
        }

        /// <summary>
        /// Checks for character death
        /// </summary>
        /// <returns>Boolean indicating whether character dead</returns>
        /// <param name="isBirth">bool indicating whether check is due to birth</param>
        /// <param name="isMother">bool indicating whether (if check is due to birth) character is mother</param>
        /// <param name="isStillborn">bool indicating whether (if check is due to birth) baby was stillborn</param>
        public Boolean checkDeath(bool isBirth = false, bool isMother = false, bool isStillborn = false)
        {
            // Check if chance of death effected by character skills
            double deathSkillsModifier = this.calcSkillEffect("death");

            // calculate base chance of death
            // chance = 2.8% (2.5% for women) per health level below 10
            Double deathChanceIncrement = 0;
            if (this.isMale)
            {
                deathChanceIncrement = 2.8;
            }
            else
            {
                deathChanceIncrement = 2.5;
            }

            Double deathChance = (10 - this.calculateHealth()) * deathChanceIncrement;

            // apply skills modifier (if exists)
            if (deathSkillsModifier != 0)
            {
                deathChance = deathChance + (deathChance * deathSkillsModifier);
            }

            // factor in birth event if appropriate
            if (isBirth)
            {
                // if check is on mother and baby was stillborn
                // (indicates unspecified complications with pregnancy)
                if ((isMother) && (isStillborn))
                {
                    deathChance = deathChance * 2;
                }
                // if is baby, or mother of healthy baby
                else
                {
                    deathChance = deathChance * 1.5;
                }
            }

            // generate a rndom double between 0-100 and compare to deathChance
            if ((Globals_Server.GetRandomDouble(100)) <= deathChance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Performs necessary actions upon the death of a character
        /// </summary>
        /// <param name="circumstance">string containing the circumstance of the death</param>
        public void processDeath(string circumstance = "natural")
        {
            NonPlayerCharacter thisHeir = null;

            // get role of character
            string role = "";
            // PCs
            if (this is PlayerCharacter)
            {
                if ((this as PlayerCharacter).playerID != null)
                {
                    role = "player";
                }
                else
                {
                    role = "PC";
                }
            }

            // NPCs
            else
            {
                if ((this as NonPlayerCharacter).familyID != null)
                {
                    if ((this as NonPlayerCharacter).isHeir)
                    {
                        role = "familyHeir";
                    }
                    else
                    {
                        role = "family";
                    }
                }
                else if ((this as NonPlayerCharacter).myBoss != null)
                {
                    role = "employee";
                }
                else
                {
                    role = "NPC";
                }
            }

            // ============== 1. set isAlive = false
            this.isAlive = false;

            // ============== 2. remove from FIEF
            this.location.charactersInFief.Remove(this);

            // ============== 3. remove from ARMY LEADERSHIP
            if (this.armyID != null)
            {
                // get army
                Army thisArmy = null;
                if (Globals_Server.armyMasterList.ContainsKey(this.armyID))
                {
                    thisArmy = Globals_Server.armyMasterList[this.armyID];

                    // set army leader to null
                    if (thisArmy != null)
                    {
                        thisArmy.leader = null;
                    }
                }

            }

            // ============== 4. if married, remove from SPOUSE
            if (this.spouse != null)
            {
                Character mySpouse = this.getSpouse();

                if (mySpouse != null)
                {
                    mySpouse.spouse = null;
                }
                
            }

            // ============== 5. if engaged, remove from FIANCEE and CANCEL MARRIAGE
            if (this.fiancee != null)
            {
                Character myFiancee = this.getFiancee();

                if (myFiancee != null)
                {
                    // remove from fiancee
                    myFiancee.fiancee = null;

                    // get marriage entry in Globals_Server.scheduledEvents
                    // get role
                    if (this.isMale)
                    {
                        role = "groom";
                    }
                    else
                    {
                        role = "bride";
                    }

                    List<JournalEntry> marriages = Globals_Server.scheduledEvents.getSpecificEntries(this.charID, role, "marriage");

                    foreach (JournalEntry jEntry in marriages)
                    {
                        // generate marriageCancelled entry
                        bool success = false;

                        // get interested parties
                        PlayerCharacter headOfFamilyBride = null;
                        PlayerCharacter headOfFamilyGroom = null;
                        Character bride = null;
                        Character groom = null;

                        for (int i = 0; i < jEntry.personae.Length; i++)
                        {
                            string thisPersonae = jEntry.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');

                            switch (thisPersonaeSplit[1])
                            {
                                case "headOfFamilyBride":
                                    headOfFamilyBride = Globals_Server.pcMasterList[thisPersonaeSplit[0]];
                                    break;
                                case "headOfFamilyGroom":
                                    headOfFamilyGroom = Globals_Server.pcMasterList[thisPersonaeSplit[0]];
                                    break;
                                case "bride":
                                    bride = Globals_Server.npcMasterList[thisPersonaeSplit[0]];
                                    break;
                                case "groom":
                                    if (Globals_Server.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Server.pcMasterList[thisPersonaeSplit[0]];
                                    }
                                    else if (Globals_Server.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Server.npcMasterList[thisPersonaeSplit[0]];
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        // ID
                        uint newEntryID = Globals_Server.getNextJournalEntryID();

                        // date
                        uint year = Globals_Server.clock.currentYear;
                        byte season = Globals_Server.clock.currentSeason;

                        // personae
                        string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
                        string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
                        string thisBrideEntry = bride.charID + "|bride";
                        string thisGroomEntry = groom.charID + "|groom";
                        string[] newEntryPersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry };

                        // type
                        string type = "marriageCancelled";

                        // description
                        string description = "On this day of Our Lord the imminent marriage between ";
                        description += groom.firstName + " " + groom.familyName + " and ";
                        description += bride.firstName + " " + bride.familyName;
                        description += " has been CANCELLED due to the sad and untimely death of ";
                        description += this.firstName + " " + this.familyName;
                        description += ". Let the bells fall silent.";

                        // create and add a marriageCancelled entry to Globals_Server.pastEvents
                        JournalEntry newEntry = new JournalEntry(newEntryID, year, season, newEntryPersonae, type, descr: description);
                        success = Globals_Server.addPastEvent(newEntry);

                        // delete marriage entry in Globals_Server.scheduledEvents
                        Globals_Server.scheduledEvents.entries.Remove(jEntry.jEntryID);
                    }
                }

            }

            // ============== 6. check if PREGNANT, if so remove birth events from Globals_Server.scheduledEvents
            if (this.isPregnant)
            {
                // get birth entry in Globals_Server.scheduledEvents

                List<JournalEntry> births = Globals_Server.scheduledEvents.getSpecificEntries(this.charID, "mother", "birth");

                foreach (JournalEntry jEntry in births)
                {
                    Globals_Server.scheduledEvents.entries.Remove(jEntry.jEntryID);
                }
            }

            // ============== 7. check and remove from BAILIFF positions
            PlayerCharacter employer = null;
            if (this is PlayerCharacter)
            {
                employer = (this as PlayerCharacter);
            }
            else
            {
                // if is an employee
                if ((this as NonPlayerCharacter).myBoss != null)
                {
                    // get boss
                    employer = (this as NonPlayerCharacter).getEmployer();
                }

            }

            // check to see if is a bailiff.  If so, remove
            if (employer != null)
            {
                foreach (Fief thisFief in employer.ownedFiefs)
                {
                    if (thisFief.bailiff == this)
                    {
                        thisFief.bailiff = null;
                    }
                }
            }

            // ============== 8. (NPC) check and remove from PC MYNPCS list
            PlayerCharacter headOfFamily = null;
            if (this is NonPlayerCharacter)
            {
                // 8.1 employees
                if (role.Equals("employee"))
                {
                    // remove from boss's myNPCs
                    employer.myNPCs.Remove((this as NonPlayerCharacter));
                }

                // 8.2 family members
                else if (role.Contains("family"))
                {
                    // get head of family
                    headOfFamily = this.getHeadOfFamily();

                    if (headOfFamily != null)
                    {
                        // remove from head of family's myNPCs
                        headOfFamily.myNPCs.Remove((this as NonPlayerCharacter));
                    }
                }

            }

            // ============== 9. (NPC) re-assign TITLES to fief owner
            if (this is NonPlayerCharacter)
            {
                foreach (string fiefID in (this as NonPlayerCharacter).myTitles)
                {
                    // get fief
                    Fief thisFief = Globals_Server.fiefMasterList[fiefID];

                    // re-assign title to fief owner
                    thisFief.owner.myTitles.Add(fiefID);
                    thisFief.titleHolder = thisFief.owner.charID;
                }
            }


            // ============== 10. RESPAWN dead non-family NPCs
            if ((role.Equals("employee")) || (role.Equals("character")))
            {
                this.respawnNPC(this as NonPlayerCharacter);
            }

            // ============== 11. (Player) PROMOTE HEIR
            else if (role.Equals("player"))
            {
                // get heir
                foreach (NonPlayerCharacter npc in (this as PlayerCharacter).myNPCs)
                {
                    if (npc.isHeir)
                    {
                        thisHeir = npc;
                    }
                }

                if (thisHeir != null)
                {
                    this.promoteNPC(thisHeir, (this as PlayerCharacter));
                }
            }

            // ============== 12. create and send DEATH JOURNAL ENTRY (unless is a player or a nobody)
            if (!role.Equals("character"))
            {
                bool success = false;

                // ID
                uint deathEntryID = Globals_Server.getNextJournalEntryID();

                // date
                uint year = Globals_Server.clock.currentYear;
                byte season = Globals_Server.clock.currentSeason;

                // personae, type, description
                string interestedPlayerEntry = "";
                string deceasedCharacterEntry = "";
                string type = "";
                string description = "On this day of Our Lord ";
                description += this.firstName + " " + this.familyName;

                // family member/heir
                if (role.Contains("family"))
                {
                    // personae
                    interestedPlayerEntry = headOfFamily.charID + "|headOfFamily";
                    deceasedCharacterEntry = this.charID;

                    if (role.Equals("heir"))
                    {
                        deceasedCharacterEntry += "|deceasedHeir";

                        // type
                        type = "deathOfHeir";
                    }
                    else
                    {
                        deceasedCharacterEntry += "|deceasedFamilyMember";

                        // type
                        type = "deathOfFamilyMember";
                    }

                    // description
                    description += ", " + (this as NonPlayerCharacter).getFunction(headOfFamily) + " of ";
                    description += headOfFamily.firstName + " " + headOfFamily.familyName;
                }

                // employee
                else if (role.Equals("employee"))
                {
                    // personae
                    interestedPlayerEntry = employer.charID + "|employer";
                    deceasedCharacterEntry += "|deceasedEmployee";

                    // type
                    type = "deathOfEmployee";

                    // description
                    description += ", employee of ";
                    description += employer.firstName + " " + employer.familyName;
                }

                // player
                else if (role.Equals("player"))
                {
                    // personae
                    interestedPlayerEntry = thisHeir.charID + "|newHeadOfFamily";
                    deceasedCharacterEntry += this.charID + "|deceasedHeadOfFamily";

                    // type
                    type = "deathOfPlayer";

                    // description
                    description += ", head of the " + this.familyName + " family";
                }

                // personae
                string[] deathPersonae = new string[] { interestedPlayerEntry, deceasedCharacterEntry };

                // description
                description += ", passed away due to ";
                switch (circumstance)
                {
                    case "injury":
                        description += "injuries sustained on the field of battle.";
                        break;
                    case "childbirth":
                        description += "complications arising from childbirth.";
                        break;
                    default:
                        description += "natural causes.";
                        break;
                }

                // player death additional description
                if ((role.Equals("player")) && (thisHeir != null))
                {
                    description += " He is succeeded by his " + thisHeir.getFunction(this as PlayerCharacter);
                    description += " " + thisHeir.firstName + " " + thisHeir.familyName + ".";
                }
                // no heir
                else
                {
                    description += " As he left no heirs, this once great family is no more.";
                }
                description += " Sympathies are extended to family and friends of the deceased.";


                // create and add a death entry to Globals_Server.pastEvents
                JournalEntry deathEntry = new JournalEntry(deathEntryID, year, season, deathPersonae, type, descr: description);
                success = Globals_Server.addPastEvent(deathEntry);
            }

        }

        /// <summary>
        /// Promotes a NonPlayerCharacter to a PlayerCharacter
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to be promoted</param>
        /// <param name="pc">Deceased PlayerCharacter</param>
        public void promoteNPC(NonPlayerCharacter npc, PlayerCharacter pc)
        {
            // ============== 1. CREATE NEW PC from NPC (heir)
            PlayerCharacter promotedNPC = new PlayerCharacter(npc, pc);

            // remove from npcMasterList and add to pcMasterList
            Globals_Server.npcMasterList.Remove(npc.charID);
            Globals_Server.pcMasterList.Add(promotedNPC.charID, promotedNPC);

            // ============== 2. change all FAMILYID & MYBOSS of MYNPCS to promotedNPC's
            for (int i = 0; i < promotedNPC.myNPCs.Count; i++ )
            {
                if (promotedNPC.myNPCs[i].familyID.Equals(pc.charID))
                {
                    promotedNPC.myNPCs[i].familyID = promotedNPC.charID;
                }

                if (promotedNPC.myNPCs[i].myBoss.Equals(pc.charID))
                {
                    promotedNPC.myNPCs[i].myBoss = promotedNPC.charID;
                }
            }

            // ============== 3. change OWNER & ANCESTRALOWNER for FIEFS
            List<string> ancestOwnerChanges = new List<string>();
            List<string> ownerChanges = new List<string>();
            foreach (KeyValuePair<string, Fief> thisFiefEntry in Globals_Server.fiefMasterList)
            {
                // get fiefs requiring change to ancestralOwner
                if (thisFiefEntry.Value.ancestralOwner == pc)
                {
                    ancestOwnerChanges.Add(thisFiefEntry.Key);
                }

                // get fiefs requiring change to owner
                if (thisFiefEntry.Value.owner == pc)
                {
                    ownerChanges.Add(thisFiefEntry.Key);
                }
            }

            // make necessary changes
            if (ancestOwnerChanges.Count > 0)
            {
                foreach (string thisFiefID in ancestOwnerChanges)
                {
                    Globals_Server.fiefMasterList[thisFiefID].ancestralOwner = promotedNPC;
                }

                foreach (string thisFiefID in ownerChanges)
                {
                    Globals_Server.fiefMasterList[thisFiefID].owner = promotedNPC;
                }
            }

            // ============== 4. change OWNER for ARMIES
            for (int i = 0; i < promotedNPC.myArmies.Count; i++ )
            {
                promotedNPC.myArmies[i].owner = promotedNPC.charID;
            }

            // ============== 5. change BESIEGINGPLAYER for SIEGES
            for (int i = 0; i < promotedNPC.mySieges.Count; i++)
            {
                // get siege
                Siege thisSiege = Globals_Server.siegeMasterList[promotedNPC.mySieges[i]];
                thisSiege.besiegingPlayer = promotedNPC.charID;
            }

            // ============== 6. change GLOBALS_CLIENT.MYCHAR
            if (Globals_Client.myChar == pc)
            {
                Globals_Client.myChar = promotedNPC;
            }

        }

        /// <summary>
        /// Creates new NonPlayerCharacter, based on supplied NonPlayerCharacter
        /// </summary>
        /// <param name="oldNPC">Old NonPlayerCharacter</param>
        public void respawnNPC(NonPlayerCharacter oldNPC)
        {
            // create basic NPC
            NonPlayerCharacter newNPC = new NonPlayerCharacter(oldNPC);

            // LOCATION
            List<string> fiefIDs = new List<string>();

            // get all fief where language same as newNPC's
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Server.fiefMasterList)
            {
                if (fiefEntry.Value.language == newNPC.language)
                {
                    fiefIDs.Add(fiefEntry.Key);
                }
            }

            // generate random int to choose new location
            string newLocationID = fiefIDs[Globals_Server.myRand.Next(0, fiefIDs.Count)];

            // assign location
            newNPC.location = Globals_Server.fiefMasterList[newLocationID];
            newNPC.location.charactersInFief.Add(newNPC);

            // TODO: FIRSTNAME

        }
        
        /// <summary>
        /// Enables character to enter keep (if not barred)
        /// </summary>
        /// <returns>bool indicating success</returns>
        public virtual bool enterKeep()
        {
            bool success = true;

            // if character is English and English barred, don't allow entry
            if (location.englishBarred)                
            {                    
                if (this.nationality.Equals("E"))                    
                {                       
                    success = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Bailiff: The duplicitous English are barred from entering this keep, Good Sir!");
                    }
                }               
            }

            // if character is French and French barred, don't allow entry
            else if (location.frenchBarred)
            {
                if (this.nationality.Equals("F"))
                {
                    success = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Bailiff: The perfidious French are barred from entering this keep, Mon Seigneur!");
                    }
                }
            }

            // if character is specifically barred, don't allow entry
            else 
            {
                if (location.barredCharacters.Contains(this.charID))
                {
                    success = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Bailiff: Your person is barred from entering this keep, Good Sir!");
                    }
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
            // exit keep
            this.inKeep = false;
        }

        /// <summary>
        /// Calculates effect of character's management rating on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double calcFiefIncMod()
        {
            double incomeModif = 0;
            // 2.5% increase in income per management level above 1
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
            // 1.25% increase in loyalty per stature/management average above 1
            loyModif = (((this.calculateStature() + this.management) / 2) - 1) * 1.25;
            loyModif = loyModif / 100;
            return loyModif;
        }

        /// <summary>
        /// Calculates effect of a particular skill effect
        /// </summary>
        /// <returns>double containing skill effect modifier</returns>
        /// <param name="effect">string specifying which skill effect to calculate</param>
        public double calcSkillEffect(String effect)
        {
            double skillEffectModifier = 0;

            // iterate through skills
            for (int i = 0; i < this.skills.Length; i++)
            {
                // iterate through skill effects, looking for effect
                foreach (KeyValuePair<string, double> entry in this.skills[i].Item1.effects)
                {
                    // if present, update total modifier
                    if (entry.Key.Equals(effect))
                    {
                        // get this particular modifer (based on character's skill level)
                        // and round up if necessary (i.e. to get the full effect)
                        double thisModifier = (this.skills[i].Item2 * 0.111);
                        if (this.skills[i].Item2 == 9)
                        {
                            thisModifier = 1;
                        }
                        // add to exisiting total modifier
                        skillEffectModifier += (entry.Value * thisModifier);
                    }
                }
            }

            return skillEffectModifier;
        }

        /// <summary>
        /// Gets the army being led by the character
        /// </summary>
        /// <returns>The army</returns>
        public Army getArmy()
        {
            Army thisArmy = null;

            if (this.armyID != null)
            {
                if (Globals_Server.armyMasterList.ContainsKey(this.armyID))
                {
                    thisArmy = Globals_Server.armyMasterList[this.armyID];
                }
            }

            return thisArmy;
        }

        /// <summary>
        /// Gets character's father
        /// </summary>
        /// <returns>The father</returns>
        public Character getFather()
        {
            Character father = null;

            if (this.father != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.father))
                {
                    father = Globals_Server.pcMasterList[this.father];
                }
                else if (Globals_Server.npcMasterList.ContainsKey(this.father))
                {
                    father = Globals_Server.npcMasterList[this.father];
                }
            }

            return father;
        }

        /// <summary>
        /// Gets character's mother
        /// </summary>
        /// <returns>The mother</returns>
        public Character getMother()
        {
            Character mother = null;

            if (this.mother != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.mother))
                {
                    mother = Globals_Server.pcMasterList[this.mother];
                }
                else if (Globals_Server.npcMasterList.ContainsKey(this.mother))
                {
                    mother = Globals_Server.npcMasterList[this.mother];
                }
            }

            return mother;
        }

        /// <summary>
        /// Gets character's king
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter getKing()
        {
            PlayerCharacter myKing = null;

            if (this.familyID != null)
            {
                myKing = this.getHeadOfFamily().getHomeFief().province.kingdom.owner;
            }

            return myKing;
        }

        /// <summary>
        /// Gets character's queen
        /// </summary>
        /// <returns>The queen</returns>
        public NonPlayerCharacter getQueen()
        {
            PlayerCharacter myKing = null;
            NonPlayerCharacter myQueen = null;

            if (this.familyID != null)
            {
                // get king
                myKing = this.getHeadOfFamily().getHomeFief().province.kingdom.owner;
                
                // get queen
                if (myKing.spouse != null)
                {
                    if (Globals_Server.npcMasterList.ContainsKey(myKing.spouse))
                    {
                        myQueen = Globals_Server.npcMasterList[myKing.spouse];
                    }
                }
            }

            return myQueen;
        }

        /// <summary>
        /// Gets character's overlord
        /// </summary>
        /// <returns>The overlord</returns>
        public PlayerCharacter getOverlord()
        {
            PlayerCharacter myOverlord = null;

            if (this.familyID != null)
            {
                myOverlord = this.getHeadOfFamily().getHomeFief().province.owner;
            }

            return myOverlord;
        }

        /// <summary>
        /// Gets character's head of family
        /// </summary>
        /// <returns>The head of the family</returns>
        public PlayerCharacter getHeadOfFamily()
        {
            PlayerCharacter headFamily = null;

            if (this.familyID != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.familyID))
                {
                    headFamily = Globals_Server.pcMasterList[this.familyID];
                }
            }

            return headFamily;
        }

        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        public virtual bool moveCharacter(Fief target, double cost)
        {
            bool success = false;

            if (Globals_Client.showMessages)
            {
                // check to see if character has fiefs in their goTo queue
                // (i.e. they have a pre-planned move)
                if (this.goTo.Count > 0)
                {
                    // check to see if this fief is the next planned destination
                    if (target != this.goTo.Peek())
                    {
                        // if not the next planned destination, give choice to continue or cancel
                        DialogResult dialogResult = MessageBox.Show("This move will clear your stored destination list.  Click 'OK' to proceed.", "Proceed with move?", MessageBoxButtons.OKCancel);

                        // if choose to cancel, return
                        if (dialogResult == DialogResult.Cancel)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Move cancelled.");
                            }
                            return success;
                        }
                        // if choose to proceed, clear entries from goTo
                        else
                        {
                            this.goTo.Clear();
                        }
                    }
                }
            }

            // ensure have enough days left to allow for move
            if (this.days >= cost)
            {
                // remove character from current fief's character list
                this.location.removeCharacter(this);
                // set location to target fief
                this.location = target;
                // add character to target fief's character list
                this.location.addCharacter(this);
                // arrives outside keep
                this.inKeep = false;
                // deduct move cost from days left
                if (this is PlayerCharacter)
                {
                    (this as PlayerCharacter).adjustDays(cost);
                }
                else
                {
                    this.adjustDays(cost);
                }
                // check if has accompanying army, if so move it
                if (this.armyID != null)
                {
                    this.getArmy().moveArmy();
                }
                success = true;
            }

            else
            {
                // if target fief not in character's goTo queue, add it
                if (this.goTo.Count == 0)
                {
                    this.goTo.Enqueue(target);
                }

                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("I'm afraid you've run out of days.\r\nYour journey will continue next season.");
                }
            }

            return success;
        }

        /// <summary>
        /// Gets the character's full days allowance, including adjustment for skills
        /// </summary>
        /// <returns>Full days allowance</returns>
        public double getDaysAllowance()
        {
            // base allowance
            double myDays = 90;

            // check for time efficiency in skills
            double timeSkillsMOd = this.calcSkillEffect("time");
            if (timeSkillsMOd != 0)
            {
                // apply skill effects
                myDays = myDays + (myDays * timeSkillsMOd);
            }

            return myDays;
        }

        /// <summary>
        /// Adjusts the character's remaining days by subtracting the specified number of days
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public virtual void adjustDays(Double daysToSubtract)
        {
            // adjust character's days
            this.days -= daysToSubtract;

            // ensure days not < 0
            if (this.days < 0)
            {
                this.days = 0;
            }

            // if army leader, synchronise army days
            if (this.armyID != null)
            {
                // get army
                Army thisArmy = this.getArmy();

                if (thisArmy != null)
                {
                    // synchronise days
                    thisArmy.days = this.days;
                }
            }
        }

        /// <summary>
        /// Uses up the character's remaining days, which will be added to bailiffDaysInFief if appropriate
        /// </summary>
        public void useUpDays()
        {
            Double remainingDays = this.days;

            // if character is bailiff of this fief, increment bailiffDaysInFief
            if (this.location.bailiff == this)
            {
                this.location.bailiffDaysInFief += remainingDays;
            }
        }

        /// <summary>
        /// Calculates whether character manages to get spouse pregnant
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="wife">Character's spouse</param>
        public bool getSpousePregnant(Character wife)
        {
            bool success = false;

            // make sure not already pregnant
            if (wife.isPregnant)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!", "PREGNANCY ATTEMPT CANCELLED");
                }
            }
            else
            {
                // ensure player and spouse have at least 1 day remaining
                double minDays = Math.Min(this.days, wife.days);

                if (minDays < 1)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Sorry, you don't have enough time left for this in the current season.", "PREGNANCY ATTEMPT CANCELLED");
                    }
                }
                else
                {
                    // ensure days are synchronised
                    if (this.days != minDays)
                    {
                        (this as PlayerCharacter).adjustDays(this.days - minDays);
                    }
                    else
                    {
                        wife.adjustDays(wife.days - minDays);
                    }

                    // generate random (0 - 100) to see if pregnancy successful
                    double randPercentage = Globals_Server.GetRandomDouble(100);

                    // holds chance of pregnancy based on age and virility
                    int chanceOfPregnancy = 0;

                    // holds pregnancy modifier based on virility
                    double pregModifier = 0;

                    // spouse's age
                    int spouseAge = wife.calcCharAge();

                    // calculate base chance of pregnancy, based on age of spouse
                    if ((!(spouseAge < 14)) && (!(spouseAge > 55)))
                    {
                        if (spouseAge < 18)
                        {
                            chanceOfPregnancy = 8;
                        }
                        else if (spouseAge < 25)
                        {
                            chanceOfPregnancy = 10;
                        }
                        else if (spouseAge < 30)
                        {
                            chanceOfPregnancy = 8;
                        }
                        else if (spouseAge < 35)
                        {
                            chanceOfPregnancy = 6;
                        }
                        else if (spouseAge < 40)
                        {
                            chanceOfPregnancy = 5;
                        }
                        else if (spouseAge < 45)
                        {
                            chanceOfPregnancy = 4;
                        }
                        else if (spouseAge < 50)
                        {
                            chanceOfPregnancy = 2;
                        }
                        else if (spouseAge < 55)
                        {
                            chanceOfPregnancy = 1;
                        }
                    }

                    // factor in effect of parent's virility
                    // but only if within child-bearing age bracket (14 - 55)
                    if ((!(spouseAge < 14)) && (!(spouseAge > 55)))
                    {
                        // modifier will be in range 0.4 - -0.4 depending on parent's virility
                        // 1. get average parent virility
                        pregModifier = (this.virility + wife.virility) / 2;
                        // 2. subtract 5 and divide by 10 to give final modifier
                        pregModifier = (pregModifier - 5) / 10;

                        // apply modifier to chanceOfPregnancy
                        chanceOfPregnancy = chanceOfPregnancy + Convert.ToInt32(chanceOfPregnancy * pregModifier);
                        if (chanceOfPregnancy < 0)
                        {
                            chanceOfPregnancy = 0;
                        }
                    }

                    // compare chanceOfPregnancy with randPercentage to see if pregnancy successful
                    if (chanceOfPregnancy > 0)
                    {
                        // if attempt successful
                        if (randPercentage <= chanceOfPregnancy)
                        {
                            // set spouse as pregnant
                            wife.isPregnant = true;

                            // schedule birth in clock sheduledEvents
                            uint birthYear = Globals_Server.clock.currentYear;
                            byte birthSeason = Globals_Server.clock.currentSeason;
                            if (Globals_Server.clock.currentSeason == 0)
                            {
                                birthSeason = (byte)(birthSeason + 3);
                            }
                            else
                            {
                                birthSeason = (byte)(birthSeason - 1);
                                birthYear = birthYear + 1;
                            }
                            string[] birthPersonae = new string[] { wife.familyID + "|headOfFamily", wife.charID + "|mother", wife.spouse + "|father" };
                            JournalEntry birth = new JournalEntry(Globals_Server.getNextJournalEntryID(), birthYear, birthSeason, birthPersonae, "birth");
                            Globals_Server.scheduledEvents.entries.Add(birth.jEntryID, birth);

                             // display message of celebration
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Let the bells ring out, milord.  " + wife.firstName + " " + wife.familyName + " is pregnant!", "PREGNANCY SUCCESSFUL");
                            }
                            success = true;
                        }
                        // if attempt not successful
                        else
                        {
                            // display encouraging message
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("I'm afraid " + wife.firstName + " " + wife.familyName + " is not pregnant.  Better luck next time, milord!", "PREGNANCY UNSUCCESSFUL");
                            }
                        }

                        // succeed or fail, deduct a day
                        if (this is PlayerCharacter)
                        {
                            (this as PlayerCharacter).adjustDays(1);
                        }

                        wife.adjustDays(1);

                    }
                    // if pregnancy impossible
                    else
                    {
                        // give the player the bad news
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Ahem ...\r\n\r\nUnfortunately, the fief physician advises that " + wife.firstName + " " + wife.familyName + " will never get pregnant with her current partner", "PREGNANCY UNSUCCESSFUL");
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Calculates the character's leadership value (for army leaders)
        /// </summary>
        /// <returns>double containg leadership value</returns>
        public double getLeadershipValue()
        {
            double lv = 0;

            // get base LV
            lv = (this.combat + this.management + this.calculateStature()) / 3;

            // factor in skills effect
            double combatSkillsMOd = this.calcSkillEffect("battle");
            if (combatSkillsMOd != 0)
            {
                lv = lv + (lv * combatSkillsMOd);
            }

            return lv;
        }

        /// <summary>
        /// Calculates the character's combat value for a combat engagement
        /// </summary>
        /// <returns>double containg combat value</returns>
        public double getCombatValue()
        {
            double cv = 0;

            // get base CV
            cv += (this.combat + this.calculateHealth()) / 2;

            // factor in armour
            cv += 5;

            // factor in nationality
            if (this.nationality.Equals("E"))
            {
                cv += 5;
            }

            return cv;
        }

        /// <summary>
        /// Calculates the character's estimate variance when estimating the size of an enemy army
        /// </summary>
        /// <returns>double containg estimate variance</returns>
        public double getEstimateVariance()
        {
            // base estimate variance
            double ev = 0.05;

            // apply effects of leadership value
            ev = ev + ((10 - this.getLeadershipValue()) * 0.05);

            // factor in skills effect
            ev = ev - (ev * this.calcSkillEffect("battle"));

            return ev;
        }

        /// <summary>
        /// gets the character's spouse
        /// </summary>
        /// <returns>The spouse or null</returns>
        public Character getSpouse()
        {
            Character mySpouse = null;

            if (this.spouse != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.spouse))
                {
                    mySpouse = Globals_Server.pcMasterList[this.spouse];
                }
                else if (Globals_Server.npcMasterList.ContainsKey(this.spouse))
                {
                    mySpouse = Globals_Server.npcMasterList[this.spouse];
                }
            }

            return mySpouse;
        }

        /// <summary>
        /// gets the character's fiancee
        /// </summary>
        /// <returns>The spouse or null</returns>
        public Character getFiancee()
        {
            Character myFiancee = null;

            if (this.fiancee != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.fiancee))
                {
                    myFiancee = Globals_Server.pcMasterList[this.fiancee];
                }
                else if (Globals_Server.npcMasterList.ContainsKey(this.fiancee))
                {
                    myFiancee = Globals_Server.npcMasterList[this.fiancee];
                }
            }

            return myFiancee;
        }

        /// <summary>
        /// Updates character data at the end/beginning of the season
        /// </summary>
        public void updateCharacter()
        {
            // check for character DEATH
            bool characterDead = this.checkDeath();

            // if character dead, process death
            if (characterDead)
            {
                this.processDeath();
            }

            else
            {
                // update AILMENTS (decrement effects, remove)
                // keep track of any ailments that have healed
                List<Ailment> healedAilments = new List<Ailment>();
                bool isHealed = false;

                // iterate through ailments
                foreach (KeyValuePair<string, Ailment> ailmentEntry in this.ailments)
                {
                    isHealed = ailmentEntry.Value.updateAilment();

                    // add to healedAilments if appropriate
                    if (isHealed)
                    {
                        healedAilments.Add(ailmentEntry.Value);
                    }
                }

                // remove any healed ailments
                if (healedAilments.Count > 0)
                {
                    for (int i = 0; i < healedAilments.Count; i++)
                    {
                        // remove ailment
                        this.ailments.Remove(healedAilments[i].ailmentID);
                    }

                    // clear healedAilments
                    healedAilments.Clear();
                }

                // automatic BABY NAMING
                if (this is NonPlayerCharacter)
                {
                    // if (age >= 1) && (firstName.Equals("Baby")), character firstname = king's
                    if (this.familyID != null)
                    {
                        if ((this as NonPlayerCharacter).checkForName(1))
                        {
                            // boys = try to get king's firstName 
                            if (this.isMale)
                            {
                                if (this.getKing() != null)
                                {
                                    this.firstName = this.getKing().firstName;
                                }
                            }
                            else
                            {
                                // girls = try to get queen's firstName 
                                if (this.getQueen() != null)
                                {
                                    this.firstName = this.getQueen().firstName;
                                }
                            }
                        }
                    }
                }

                // reset DAYS
                this.days = this.getDaysAllowance();
                this.adjustDays(0);

            }

        }

        /// <summary>
        /// Calculates the character's fief management rating (i.e. how good they are at managing a fief)
        /// </summary>
        /// <returns>double containing fief management rating</returns>
        public double calcFiefManagementRating()
        {
            // baseline rating
            double fiefMgtRating = (this.management + this.calculateStature()) / 2;

            // check for skills effecting fief loyalty
            double fiefLoySkill = this.calcSkillEffect("fiefLoy");

            // check for skills effecting fief expenses
            double fiefExpSkill = this.calcSkillEffect("fiefExpense");

            // combine skills into single modifier. Note: fiefExpSkill is * by -1 because 
            // a negative effect on expenses is good, so needs to be normalised
            double mgtSkills = (fiefLoySkill + (-1 * fiefExpSkill));

            // calculate final fief management rating
            fiefMgtRating += (fiefMgtRating * mgtSkills);

            return fiefMgtRating;
        }

        /// <summary>
        /// Calculates the character's army leadership rating (i.e. how good they are at leading an army)
        /// </summary>
        /// <returns>double containing army leadership rating</returns>
        public double calcArmyLeadershipRating()
        {
            // baseline rating
            double armyLeaderRating = (this.management + this.calculateStature() + this.combat) / 3;

            // check for skills effecting battle
            double battleSkills = this.calcSkillEffect("battle");

            // check for skills effecting siege
            double siegeSkills = this.calcSkillEffect("siege");

            // combine skills into single modifier 
            double combatSkills = battleSkills + siegeSkills;

            // calculate final combat rating
            armyLeaderRating += (armyLeaderRating * combatSkills);

            return armyLeaderRating;
        }

        /// <summary>
        /// Calculates chance and effect of character injuries resulting from a battle
        /// </summary>
        /// <returns>bool indicating whether character has died of injuries</returns>
        /// <param name="armyCasualtyLevel">double indicating friendly army casualty level</param>
        public bool calculateCombatInjury(double armyCasualtyLevel)
        {
            bool isDead = false;
            uint healthLoss = 0;

            // calculate base chance of injury (based on armyCasualtyLevel)
            double injuryPercentChance = (armyCasualtyLevel * 100);

            // factor in combat skill of character
            injuryPercentChance += 5 - this.combat;

            // ensure is at least 1% chance of injury
            if (injuryPercentChance < 1)
            {
                injuryPercentChance = 1;
            }

            // generate random percentage
            int randomPercent = Globals_Server.myRand.Next(101);

            // compare randomPercent with injuryChance to see if injury occurred
            if (randomPercent <= injuryPercentChance)
            {
                // generate random int 1-5 specifying health loss
                healthLoss = Convert.ToUInt32(Globals_Server.myRand.Next(1, 6));
            }

            // check if should create and add an ailment
            if (healthLoss > 0)
            {
                uint minEffect = 0;

                // check if character has died of injuries
                if (this.calculateHealth() < healthLoss)
                {
                    isDead = true;
                }

                // check if results in permanent damage
                if (healthLoss > 4)
                {
                    minEffect = 1;
                }

                // create ailment
                Ailment myAilment = new Ailment(Globals_Server.getNextAilmentID(), "Battlefield injury", Globals_Server.clock.seasons[Globals_Server.clock.currentSeason] + ", " + Globals_Server.clock.currentYear, healthLoss, minEffect);

                // add to character
                this.ailments.Add(myAilment.ailmentID, myAilment);
            }

            // =================== if is injured but not dead, create and send JOURNAL ENTRY
            if ((!isDead) && (healthLoss > 0))
            {
                // ID
                uint entryID = Globals_Server.getNextJournalEntryID();

                // personae
                PlayerCharacter concernedPlayer = null;
                List<string> tempPersonae = new List<string>();

                // add injured character
                tempPersonae.Add(this.charID + "|injuredCharacter");
                if (this is NonPlayerCharacter)
                {
                    if (this.familyID != null)
                    {
                        concernedPlayer = (this as NonPlayerCharacter).getHeadOfFamily();
                        if (concernedPlayer != null)
                        {
                            tempPersonae.Add(concernedPlayer.charID + "|headOfFamily");
                        }
                    }

                    else if ((this as NonPlayerCharacter).myBoss != null)
                    {
                        concernedPlayer = (this as NonPlayerCharacter).getEmployer();
                        if (concernedPlayer != null)
                        {
                            tempPersonae.Add(concernedPlayer.charID + "|employer");
                        }
                    }
                }
                string[] injuryPersonae = tempPersonae.ToArray();

                // location
                string injuryLocation = this.location.id;

                // description
                string injuryDescription = "On this day of our lord ";
                injuryDescription += this.firstName + " " + this.familyName;
                if (concernedPlayer != null)
                {
                    injuryDescription += ", " + (this as NonPlayerCharacter).getFunction(concernedPlayer) + " of ";
                    injuryDescription += concernedPlayer.firstName + " " + concernedPlayer.familyName + ",";
                }
                injuryDescription += " was injured on the field of battle in the fief of ";
                injuryDescription += this.location.name + ".";

                // create and send JOURNAL ENTRY
                JournalEntry injuryEntry = new JournalEntry(entryID, Globals_Server.clock.currentYear, Globals_Server.clock.currentSeason, injuryPersonae, "injury", loc: injuryLocation, descr: injuryDescription);

                // add new journal entry to pastEvents
                Globals_Server.addPastEvent(injuryEntry);
            }

            return isDead;
        }

        /// <summary>
        /// Gets the fiefs in which the character is the bailiff
        /// </summary>
        /// <returns>List<Fief> containing the fiefs</returns>
        public List<Fief> getFiefsBailiff()
        {
            List<Fief> myFiefs = new List<Fief>();

            // iterate through owned fiefs, searching for character as bailiff
            foreach (Fief thisFief in Globals_Client.myChar.ownedFiefs)
            {
                if (thisFief.bailiff == this)
                {
                    myFiefs.Add(thisFief);
                }
            }

            return myFiefs;
        }

        /// <summary>
        /// Gets the armies of which the character is the leader
        /// </summary>
        /// <returns>List<Army> containing the armies</returns>
        public List<Army> getArmiesLeader()
        {
            List<Army> myArmies = new List<Army>();

            // iterate through armies, searching for character as leader
            foreach (Army thisArmy in Globals_Client.myChar.myArmies)
            {
                if (thisArmy.getLeader() == this)
                {
                    myArmies.Add(thisArmy);
                }
            }

            return myArmies;
        }

    }

    /// <summary>
    /// Class storing data on PlayerCharacter
    /// </summary>
    public class PlayerCharacter : Character
    {

        /// <summary>
        /// Holds character outlawed status
        /// </summary>
        public bool outlawed { get; set; }
        /// <summary>
        /// Holds character's treasury
        /// </summary>
        public uint purse { get; set; }
        /// <summary>
        /// Holds character's employees and family (NonPlayerCharacter objects)
        /// </summary>
        public List<NonPlayerCharacter> myNPCs = new List<NonPlayerCharacter>();
        /// <summary>
        /// Holds character's owned fiefs
        /// </summary>
        public List<Fief> ownedFiefs = new List<Fief>();
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public String homeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public String ancestralHomeFief { get; set; }
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// </summary>
        public String playerID { get; set; }
        /// <summary>
        /// Holds character's armies (Army objects)
        /// </summary>
        public List<Army> myArmies = new List<Army>();
        /// <summary>
        /// Holds character's sieges (siegeIDs)
        /// </summary>
        public List<string> mySieges = new List<string>();

        /// <summary>
        /// Constructor for PlayerCharacter
        /// </summary>
        /// <param name="outl">bool holding character outlawed status</param>
        /// <param name="pur">uint holding character purse</param>
        /// <param name="npcs">List<NonPlayerCharacter> holding employees and family of character</param>
        /// <param name="owned">List<Fief> holding fiefs owned by character</param>
        /// <param name="home">String holding character's home fief (fiefID)</param>
        /// <param name="anchome">String holding character's ancestral home fief (fiefID)</param>
        /// <param name="pID">String holding ID of player who is currently playing this PlayerCharacter</param>
        /// <param name="myA">List<Army> holding character's armies</param>
        /// <param name="myS">List<string> holding character's sieges (siegeIDs)</param>
        public PlayerCharacter(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, String nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool preg, String famID,
            String sp, String fath, String moth, bool outl, uint pur, List<NonPlayerCharacter> npcs, List<Fief> owned, String home, String ancHome, List<String> myTi, List<Army> myA,
            List<string> myS, string fia, Dictionary<string, Ailment> ails = null, Fief loc = null, String pID = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc)
        {
            this.outlawed = outl;
            this.purse = pur;
            this.myNPCs = npcs;
            this.ownedFiefs = owned;
            this.homeFief = home;
            this.ancestralHomeFief = ancHome;
            this.playerID = pID;
            this.myArmies = myA;
            this.mySieges = myS;
        }

        /// <summary>
        /// Constructor for PlayerCharacter taking no parameters
        /// For use when de-serialising from Riak
        /// </summary>
        public PlayerCharacter()
		{
		}

		/// <summary>
		/// Constructor for PlayerCharacter using PlayerCharacter_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
		/// <param name="pcr">PlayerCharacter_Riak object to use as source</param>
		public PlayerCharacter(PlayerCharacter_Riak pcr)
			: base(pcr: pcr)
		{

			this.outlawed = pcr.outlawed;
			this.purse = pcr.purse;
            // create empty NPC List, to be populated later
			this.myNPCs = new List<NonPlayerCharacter> ();
            // create empty Fief List, to be populated later
            this.ownedFiefs = new List<Fief>();
            this.homeFief = pcr.homeFief;
            this.ancestralHomeFief = pcr.ancestralHomeFief;
            this.playerID = pcr.playerID;
            // create empty Army List, to be populated later
            this.myArmies = new List<Army>();
            this.mySieges = pcr.mySieges;
		}

        /// <summary>
        /// Constructor for PlayerCharacter using NonPlayerCharacter object and a PlayerCharacter object,
        /// for use when promoting a deceased PC's heir
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        public PlayerCharacter(NonPlayerCharacter npc, PlayerCharacter pc)
            : base(npc, "promote", pc.myTitles)
        {
            this.outlawed = false;
            this.purse = 0;
            this.myNPCs = pc.myNPCs;
            this.ownedFiefs = pc.ownedFiefs;
            this.homeFief = pc.homeFief;
            this.ancestralHomeFief = pc.ancestralHomeFief;
            this.playerID = pc.playerID;
            this.myArmies = pc.myArmies;
            this.mySieges = pc.mySieges;
        }

        /// <summary>
        /// Returns the siege object associated with the specified siegeID
        /// </summary>
        /// <returns>The siege object</returns>
        /// <param name="id">The siegeID of the siege</param>
        public Siege getSiege(string id)
        {
            return Globals_Server.siegeMasterList[id];
        }

        /// <summary>
        /// Returns the current total GDP for all fiefs owned by the PlayerCharacter
        /// </summary>
        /// <returns>The current total GDP</returns>
        public int getTotalGDP()
        {
            int totalGDP = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                totalGDP += Convert.ToInt32(thisFief.keyStatsCurrent[1]);
            }

            return totalGDP;
        }

        /// <summary>
        /// Finds the highest ranking fief in the PlayerCharacter's owned fiefs
        /// </summary>
        /// <returns>The fiefID of the highest ranking fief</returns>
        public string getHighestRankingFief()
        {
            string homeFief = null;
            byte highestStature = 0;

            foreach (Fief thisFief in this.ownedFiefs)
            {
                if (thisFief.rank.stature > highestStature)
                {
                    highestStature = thisFief.rank.stature;
                    homeFief = thisFief.id;
                }
            }

            return homeFief;
        }

        /// <summary>
        /// Processes an offer for employment
        /// </summary>
        /// <returns>bool indicating acceptance of offer</returns>
        /// <param name="npc">NPC receiving offer</param>
        /// <param name="offer">Proposed wage</param>
        public bool processEmployOffer(NonPlayerCharacter npc, uint offer)
        {
            bool accepted = false;

            // get NPC's potential salary
            double potentialSalary = npc.calcWage(this);

            // generate random (0 - 100) to see if accepts offer
            double chance = Globals_Server.GetRandomDouble(100);

            // get 'npcHire' skill effect modifier (increase/decrease chance of offer being accepted)
            double hireSkills = this.calcSkillEffect("npcHire");
            // convert to % to allow easy modification of chance
            hireSkills = (hireSkills * 100);
            // apply to chance
            chance = (chance + (hireSkills * -1));
            // ensure chance is a valid %
            if (chance < 0)
            {
                chance = 0;
            }
            else if (chance > 100)
            {
                chance = 100;
            }


            // get range of negotiable offers
            // minimum = 90% of potential salary, below which all offers rejected
            double minAcceptable = potentialSalary - (potentialSalary / 10);
            // maximum = 110% of potential salary, above which all offers accepted
            double maxAcceptable = potentialSalary + (potentialSalary / 10);
            // get range
            double rangeNegotiable = (maxAcceptable - minAcceptable);

            // ensure this offer is more than the last from this PC
            bool offerLess = false;
            if (npc.lastOffer.ContainsKey(this.charID))
            {
                if (!(offer > npc.lastOffer[this.charID]))
                {
                    offerLess = true;
                }
                // if new offer is greater, over-write previous offer
                else
                {
                    npc.lastOffer[this.charID] = offer;
                }
            }
            // if no previous offer, add new entry
            else
            {
                npc.lastOffer.Add(this.charID, offer);
            }

            // automatically accept if offer > 10% above potential salary
            if (offer > maxAcceptable)
            {
                accepted = true;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": You've made me an offer I can't refuse, Milord!");
                }
            }

            // automatically reject if offer < 10% below potential salary
            else if (offer < minAcceptable)
            {
                accepted = false;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": Don't insult me, Sirrah!");
                }
            }

            // automatically reject if offer !> previous offer
            else if (offerLess)
            {
                accepted = false;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You must improve on your previous offer (£" + npc.lastOffer[this.charID] + ")");
                }
            }

            else
            {
                // see where offer lies (as %) within rangeNegotiable
                double offerPercentage = ((offer - minAcceptable) / rangeNegotiable) * 100;
                // compare randomly generated % (chance) with offerPercentage
                if (chance <= offerPercentage)
                {
                    accepted = true;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": It's a deal, Milord!");
                    }
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": You'll have to do better than that, Good Sir!");
                    }
                }
            }

            if (accepted)
            {
                // hire this NPC
                this.hireNPC(npc, offer);
            }

            return accepted;
        }

        /// <summary>
        /// Hire an NPC
        /// </summary>
        /// <param name="npc">NPC to hire</param>
        /// <param name="wage">NPC's wage</param>
        public void hireNPC(NonPlayerCharacter npc, uint wage)
        {
            // add to employee list
            this.myNPCs.Add(npc);
            // set NPC wage
            npc.wage = wage;
            // set this PC as NPC's boss
            npc.myBoss = this.charID;
            // remove any offers by this PC from NPCs lastOffer list
            npc.lastOffer.Clear();
        }

        /// <summary>
        /// Fire an NPC
        /// </summary>
        /// <param name="npc">NPC to fire</param>
        public void fireNPC(NonPlayerCharacter npc)
        {
            // remove from bailiff duties
            List<Fief> fiefsBailiff = npc.getFiefsBailiff();
            if (fiefsBailiff.Count > 0)
            {
                for (int i = 0; i < fiefsBailiff.Count; i++ )
                {
                    fiefsBailiff[i].bailiff = null;
                }
            }

            // remove from army duties
            List<Army> armiesLeader = npc.getArmiesLeader();
            if (armiesLeader.Count > 0)
            {
                for (int i = 0; i < armiesLeader.Count; i++ )
                {
                    armiesLeader[i].leader = null;
                }
            }

            // remove from employee list
            this.myNPCs.Remove(npc);

            // set NPC wage to 0
            npc.wage = 0;

            // remove this PC as NPC's boss
            npc.myBoss = null;

            // remove NPC from entourage
            npc.inEntourage = false;

            // eject from keep
            npc.inKeep = false;

            // if NPC has entries in goTo, clear
            if (npc.goTo.Count > 0)
            {
                npc.goTo.Clear();
            }
        }

        /// <summary>
        /// Adds an NPC to the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be added</param>
        public void addToEntourage(NonPlayerCharacter npc)
        {
            // if NPC has entries in goTo, clear
            if (npc.goTo.Count > 0)
            {
                npc.goTo.Clear();
            }

            // keep track of original days value for PC
            double myDays = this.days;

            // ensure days are synchronised
            double minDays = Math.Min(this.days, npc.days);
            this.days = minDays;
            npc.days = minDays;

            // add to entourage
            npc.inEntourage = true;

            // ensure days of entourage are synched with PC
            if (this.days != myDays)
            {
                this.adjustDays(0);
            }
        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be removed</param>
        public void removeFromEntourage(NonPlayerCharacter npc)
        {
            //remove from entourage
            npc.inEntourage = false;
        }

        /// <summary>
        /// Adds a Fief to the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be added</param>
        public void addToOwnedFiefs(Fief f)
        {
            // add fief
            this.ownedFiefs.Add(f);
        }

        /// <summary>
        /// Removes a Fief from the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be removed</param>
        public void removeFromOwnedFiefs(Fief f)
        {
            // remove fief
            this.ownedFiefs.Remove(f);
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to enter keep (if not barred).
        /// Then moves entourage (if not individually barred). Ignores nationality bar
        /// for entourage if PlayerCharacter allowed to enter
        /// </summary>
        /// <returns>bool indicating success</returns>
        public override bool enterKeep()
        {
            // invoke base method for PlayerCharacter
            bool success = base.enterKeep();

            // if PlayerCharacter enters keep
            if (success)
            {
                // iterate through employees
                for (int i = 0; i < this.myNPCs.Count; i++)
                {
                    // if employee in entourage, allow to enter keep unless individually barred
                    if (this.myNPCs[i].inEntourage)
                    {
                        if (location.barredCharacters.Contains(this.myNPCs[i].charID))
                        {
                            this.myNPCs[i].inKeep = false;
                            String toDisplay = "";
                            toDisplay += "Bailiff: One or more of your entourage is barred from entering this keep!";
                            toDisplay += "\r\nThey will rejoin you after your visit.";
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show(toDisplay);
                            }
                        }
                        else
                        {
                            this.myNPCs[i].inKeep = true;
                        }
                    }

                }
                
            }

            return success;

        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to exit keep. Then exits entourage.
        /// </summary>
        public override void exitKeep()
        {
            // invoke base method for PlayerCharacter
            base.exitKeep();

            // iterate through employees
            for (int i = 0; i < this.myNPCs.Count; i++)
            {
                // if employee in entourage, exit keep
                if (this.myNPCs[i].inEntourage)
                {
                    this.myNPCs[i].inKeep = false;
                }
            }
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to synchronise the days of their entourage
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public override void adjustDays(Double daysToSubtract)
        {
            // use base method to subtract days from PlayerCharacter
            base.adjustDays(daysToSubtract);

            // iterate through employees
            for (int i = 0; i < this.myNPCs.Count; i++)
            {
                // if employee in entourage, set NPC days to same as player
                if (this.myNPCs[i].inEntourage)
                {
                    this.myNPCs[i].days = this.days;
                }
            }

        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to target fief. Then moves entourage.
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        public override bool moveCharacter(Fief target, double cost)
        {

            // use base method to move PlayerCharacter
            bool success = base.moveCharacter(target, cost);

            // if PlayerCharacter move successfull
            if (success)
            {
                // iterate through employees
                for (int i = 0; i < this.myNPCs.Count; i++)
                {
                    // if employee in entourage, move employee
                    if (this.myNPCs[i].inEntourage)
                    {
                        this.moveEntourageNPC(target, this.myNPCs[i]);
                    }
                }
            }

            return success;

        }

        /// <summary>
        /// Moves an NPC in a player's entourage (i.e. sets new location)
        /// </summary>
        /// <param name="target">Target fief</param>
        /// <param name="npc">NonPlayerCharacter to move</param>
        public void moveEntourageNPC(Fief target, NonPlayerCharacter npc)
        {
            // remove character from current fief's character list
            npc.location.removeCharacter(npc);
            // set location to target fief
            npc.location = target;
            // add character to target fief's character list
            npc.location.addCharacter(npc);
            // arrives outside keep
            npc.inKeep = false;
        }
        
        /// <summary>
        /// Recruits troops from the current fief
        /// </summary>
        /// <returns>uint containing number of troops recruited</returns>
        /// <param name="number">How many troops to recruit</param>
        public int recruitTroops(uint number)
        {
            // used to record outcome of various checks
            bool proceed = true;

            // used to confirm final purchase of troops
            bool confirmPurchase = false;

            int troopsRecruited = 0;
            int revisedRecruited = 0;
            int indivTroopCost = 0;
            int troopCost = 0;
            int daysUsed = 0;

            // get home fief
            Fief homeFief = this.getHomeFief();

            // get army
            Army thisArmy = this.getArmy();

            // calculate cost of individual soldier
            if (this.location.ancestralOwner == this)
            {
                indivTroopCost = 500;
            }
            else
            {
                indivTroopCost = 2000;
            }

            // string to hold any messages
            string toDisplay = "";

            // various checks to see whether to proceed
            // 1. see if fief owned by player
            if (!this.ownedFiefs.Contains(this.location))
            {
                proceed = false;
                toDisplay = "You cannot recruit in this fief, my lord, as you don't actually own it.";
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay);
                }
            }
            else
            {
                // 2. see if recruitment already occurred for this season
                if (this.location.hasRecruited)
                {
                    proceed = false;
                    toDisplay = "I'm afraid you have already recruited here in this season, my lord.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
                else
                {
                    // 3. Check language and loyalty permit recruitment
                    if ((!this.language.Item1.languageID.Equals(this.location.language.Item1.languageID))
                        && (this.location.loyalty < 7))
                    {
                        proceed = false;
                        toDisplay = "I'm sorry, my lord, you do not speak the same language as the people in this fief,\r\n";
                        toDisplay += "and thier loyalty is not sufficiently high to allow recruitment.";
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(toDisplay);
                        }
                    }
                    else
                    {
                        // 4. check sufficient funds for at least 1 troop
                        if (!(homeFief.getAvailableTreasury() > indivTroopCost))
                        {
                            proceed = false;
                            toDisplay = "I'm sorry, my Lord; you have insufficient funds for recruitment.";
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show(toDisplay);
                            }
                        }
                        else
                        {
                            // 5. check sufficient days remaining
                            // see how long recuitment attempt will take: generate random int (1-5)
                            daysUsed = Globals_Server.myRand.Next(6);

                            if (this.days < daysUsed)
                            {
                                proceed = false;
                                toDisplay = "I'm afraid you have run out of days, my lord.";
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show(toDisplay);
                                }
                            }
                        }
                    }
                }
            }

            // if have not passed any of checks above, return
            if (!proceed)
            {
                return troopsRecruited;
            }

            // calculate potential cost
            troopCost = Convert.ToInt32(number) * indivTroopCost;

            // check to see if can afford the specified number of troops
            // if can't afford specified number
            if (!(homeFief.getAvailableTreasury() >= troopCost))
            {
                // work out how many troops can afford
                double roughNumber = homeFief.getAvailableTreasury() / indivTroopCost;
                revisedRecruited = Convert.ToInt32(Math.Floor(roughNumber));

                // present alternative number and ask for confirmation
                toDisplay = "Sorry, milord, you do not have the funds to recruit " + number + " troops.";
                toDisplay += "  However, you can afford to recruit " + revisedRecruited + ".\r\n";
                toDisplay += "Do you wish to proceed with the recruitment of the revised number?";
                DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with revised recruitment?", MessageBoxButtons.OKCancel);

                // if choose to cancel
                if (dialogResult == DialogResult.Cancel)
                {
                    proceed = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
                    }
                }
                // chooses to proceed
                else
                {
                    // revise number to recruit
                    number = Convert.ToUInt32(revisedRecruited);
                }
            }

            if (proceed)
            {
                // calculate number of troops responding to call (based on fief population)
                troopsRecruited = this.location.callUpTroops(minProportion: 0.4);

                // adjust if necessary
                if (troopsRecruited >= number)
                {
                    troopsRecruited = Convert.ToInt32(number);

                    // calculate total cost
                    troopCost = troopsRecruited * indivTroopCost;

                    // confirm recruitment
                    toDisplay = troopsRecruited + " men have responded to your call, milord, and they would cost " + troopCost + " to recruit.\r\n";
                    toDisplay += "There is " + homeFief.getAvailableTreasury() + "in the home treasury.  Do you wish to proceed with recruitment?";
                    DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with recruitment?", MessageBoxButtons.OKCancel);

                    // if choose to cancel
                    if (dialogResult == DialogResult.Cancel)
                    {
                        confirmPurchase = false;
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
                        }
                    }
                    // chooses to proceed
                    else
                    {
                        confirmPurchase = true;
                    }
                }
                // if less than specified number respond to call
                else
                {
                    // calculate total cost
                    troopCost = troopsRecruited * indivTroopCost;

                    // confirm recruitment
                    toDisplay = "Only " + troopsRecruited + " men have responded to your call, milord, and they would cost " + troopCost + " to recruit.\r\n";
                    toDisplay += "There is " + homeFief.getAvailableTreasury() + "in the home treasury.  Do you wish to proceed with recruitment?";
                    DialogResult dialogResult = MessageBox.Show(toDisplay, "Proceed with recruitment?", MessageBoxButtons.OKCancel);

                    // if choose to cancel
                    if (dialogResult == DialogResult.Cancel)
                    {
                        confirmPurchase = false;
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
                        }
                    }
                    // chooses to proceed
                    else
                    {
                        confirmPurchase = true;
                    }
                }

                if (confirmPurchase)
                {
                    // deduct cost of troops from treasury
                    homeFief.treasury = homeFief.treasury - troopCost;

                    // work out how many of each type recruited
                    uint[] typesRecruited = new uint[] {0, 0, 0, 0, 0, 0};
                    uint totalSoFar = 0;
                    for (int i = 0; i < typesRecruited.Length; i++ )
                    {
                        // work out 'trained' troops numbers
                        if (i < typesRecruited.Length - 1)
                        {
                            // get army nationality
                            string thisNationality = this.nationality.ToUpper();
                            if (!thisNationality.Equals("E"))
                            {
                                thisNationality = "O";
                            }
                            typesRecruited[i] = Convert.ToUInt32(troopsRecruited * Globals_Server.recruitRatios[thisNationality][i]);
                            totalSoFar += typesRecruited[i];
                        }
                        // fill up with rabble
                        else
                        {
                            typesRecruited[i] = Convert.ToUInt32(troopsRecruited) - totalSoFar;
                        }
                    }

                    // add new troops to army
                    for (int i = 0; i < thisArmy.troops.Length; i++ )
                    {
                        thisArmy.troops[i] += typesRecruited[i];
                    }

                    // indicate recruitment has occurred in this fief
                    this.location.hasRecruited = true;
                }

                // update character's days
                this.adjustDays(daysUsed);

                // update army's days
                thisArmy.days = this.days;
            }

            return troopsRecruited;
        }

        /// <summary>
        /// Returns the PlayerCharacter's home fief
        /// </summary>
        /// <returns>The home fief</returns>
        public Fief getHomeFief()
        {
            Fief thisHomeFief = null;

            if (this.homeFief != null)
            {
                if (Globals_Server.fiefMasterList.ContainsKey(this.homeFief))
                {
                    thisHomeFief = Globals_Server.fiefMasterList[this.homeFief];
                }
            }

            return thisHomeFief;
        }

        /// <summary>
        /// Returns the PlayerCharacter's ancestral home fief
        /// </summary>
        /// <returns>The ancestral home fief</returns>
        public Fief getAncestralHome()
        {
            Fief ancestralHome = null;

            if (this.ancestralHomeFief != null)
            {
                if (Globals_Server.fiefMasterList.ContainsKey(this.ancestralHomeFief))
                {
                    ancestralHome = Globals_Server.fiefMasterList[this.ancestralHomeFief];
                }
            }

            return ancestralHome;
        }

        /*
        /// <summary>
        /// Transfers the fief title to the specified character
        /// </summary>
        /// <param name="newTitleHolder">The new title holder</param>
        public void transferTitle(Character newTitleHolder)
        {
            // remove title from existing holder
            Character oldTitleHolder = this.getTitleHolder();
            oldTitleHolder.myTitles.Remove(this.fiefID);

            // add title to new owner
            newTitleHolder.myTitles.Add(this.fiefID);
            this.titleHolder = newTitleHolder.charID;
        } */

        /// <summary>
        /// Transfers the title of a fief or province to another character
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newHolder">The character receiving the title</param>
        /// <param name="placeID">The ID of the fief or province</param>
        /// <param name="titleType">The type of title - fief or province</param>
        public bool grantTitle(Character newHolder, string placeID, string titleType)
        {
            bool success = true;
            Fief thisFief = null;
            Province thisProvince = null;

            // checks
            switch (titleType)
            {
                case "fief":
                    if (Globals_Server.fiefMasterList.ContainsKey(placeID))
                    {
                        thisFief = Globals_Server.fiefMasterList[placeID];
                    }

                    // ownership
                    if (!(this == thisFief.owner))
                    {
                        success = false;
                    }
                    break;
                case "province":
                    break;
                default:
                    break;
            }

            return success;
        }

    }

    /// <summary>
    /// Class storing data on NonPlayerCharacter
    /// </summary>
    public class NonPlayerCharacter : Character
    {

        /// <summary>
        /// Holds NPC's employer (charID)
        /// </summary>
        public String myBoss { get; set; }
        /// <summary>
        /// Holds NPC's wage
        /// </summary>
        public uint wage { get; set; }
        /// <summary>
        /// Holds last wage offer from individual PCs
        /// </summary>
        public Dictionary<string, uint> lastOffer { get; set; }
        /// <summary>
        /// Denotes if in employer's entourage
        /// </summary>
        public bool inEntourage { get; set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool isHeir { get; set; }

        /// <summary>
        /// Constructor for NonPlayerCharacter
        /// </summary>
        /// <param name="mb">String holding NPC's employer (charID)</param>
        /// <param name="wa">string holding NPC's wage</param>
        /// <param name="inEnt">bool denoting if in employer's entourage</param>
        /// <param name="isH">bool denoting if is player's heir</param>
        public NonPlayerCharacter(String id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, String nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool preg, String famID,
            String sp, String fath, String moth, uint wa, bool inEnt, bool isH, List<String> myTi, string fia, Dictionary<string, Ailment> ails = null, String mb = null, Fief loc = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, preg, famID, sp, fath, moth, myTi, fia, ails, loc)
        {
            // TODO: validate hb = 1-10000
            // TODO: validate go = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // TODO: validate wa = uint

            this.myBoss = mb;
            this.wage = wa;
            this.inEntourage = inEnt;
            this.lastOffer = new Dictionary<string, uint>();
            this.isHeir = isH;
        }

        /// <summary>
        /// Constructor for NonPlayerCharacter taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public NonPlayerCharacter()
		{
		}

		/// <summary>
		/// Constructor for NonPlayerCharacter using NonPlayerCharacter_Riak object
        /// For use when de-serialising from Riak
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
            this.isHeir = npcr.isHeir;
		}

        /// <summary>
        /// Constructor for NonPlayerCharacter using NonPlayerCharacter object,
        /// for use when respawning deceased NPCs
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        public NonPlayerCharacter(NonPlayerCharacter npc)
            : base(npc, "respawn")
        {
            this.myBoss =null;
            this.wage = 0;
            this.inEntourage = false;
            this.lastOffer = new Dictionary<string,uint>();
            this.isHeir = false;
        }

        /// <summary>
        /// Calculates the family allowance of a family NPC, based on age and function
        /// </summary>
        /// <returns>uint containing family allowance</returns>
        /// <param name="func">NPC's function</param>
        public uint calcFamilyAllowance(String func)
        {
            uint famAllowance = 0;
            double ageModifier = 1;

            // factor in family function
            if (func.ToLower().Equals("wife"))
            {
                famAllowance = 30000;
            }
            else
            {
                if (func.ToLower().Contains("heir"))
                {
                    famAllowance = 40000;
                }
                else if (func.ToLower().Equals("son"))
                {
                    famAllowance = 20000;
                }
                else if (func.ToLower().Equals("daughter"))
                {
                    famAllowance = 15000;
                }
                else
                {
                    famAllowance = 10000;
                }

                // calculate age modifier
                if ((this.calcCharAge() <= 7))
                {
                    ageModifier = 0.25;
                }
                else if ((this.calcCharAge() > 7) && (this.calcCharAge() <= 14))
                {
                    ageModifier = 0.5;
                }
                else if ((this.calcCharAge() > 14) && (this.calcCharAge() <= 21))
                {
                    ageModifier = 0.75;
                }

                // apply age modifier
                famAllowance = Convert.ToUInt32(famAllowance * ageModifier);
            }

            return famAllowance;
        }

        /// <summary>
        /// Derives NPC function
        /// </summary>
        /// <returns>String containing NPC function</returns>
        /// <param name="pc">PlayerCharacter with whom NPC has relationship</param>
        public String getFunction(PlayerCharacter pc)
        {
            String myFunction = "";

            // check for employees
            if ((this.myBoss != null) && (this.myBoss.Equals(pc.charID)))
            {
                myFunction = "Employee";
            }

            // check for family function
            else if ((this.familyID != null) && (this.familyID.Equals(pc.familyID)))
            {
                // default value
                myFunction = "Family Member";

                // get character's father
                Character thisFather = this.getFather();
                // get PC's father
                Character pcFather = pc.getFather();

                if (this.father != null)
                {
                    // sons & daughters
                    if (this.father == pc.charID)
                    {
                        if (this.isMale)
                        {
                            myFunction = "Son";
                        }
                        else
                        {
                            myFunction = "Daughter";
                        }
                    }

                    // brothers and sisters
                    if (this.father == pc.father)
                    {
                        if (this.isMale)
                        {
                            myFunction = "Brother";
                        }
                        else
                        {
                            myFunction = "Sister";
                        }
                    }

                    // uncles and aunts
                    if ((pcFather != null) && (pcFather.father != null))
                    {
                        if (this.father == pcFather.father)
                        {
                            if (this.isMale)
                            {
                                myFunction = "Uncle";
                            }
                            else
                            {
                                myFunction = "Aunt";
                            }
                        }
                    }

                    if (thisFather.father != null)
                    {
                        // grandsons & granddaughters
                        if (thisFather.father == pc.charID)
                        {
                            if (this.isMale)
                            {
                                myFunction = "Grandson";
                            }
                            else
                            {
                                myFunction = "Granddaughter";
                            }
                        }
                    }
                }

                // grandmother
                if (pcFather != null)
                {
                    if ((pcFather.mother != null) && (pcFather.mother.Equals(this.charID)))
                    {
                        myFunction = "Grandmother";
                    }
                }

                if ((pc.mother != null) && (pc.mother == this.charID))
                {
                    // mother
                    myFunction = "Mother";
                }

                // wife
                if ((this.spouse != null) && (this.spouse == pc.charID))
                {
                    if (this.isMale)
                    {
                        myFunction = "Husband";
                    }
                    else
                    {
                        myFunction = "Wife";
                    }
                }

                // check for heir
                if (this.isHeir)
                {
                    myFunction += " & Heir";
                }
            }

            return myFunction;
        }

        /// <summary>
        /// Gets an NPC's employment responsibilities
        /// </summary>
        /// <returns>String containing NPC responsibilities</returns>
        /// <param name="pc">PlayerCharacter by whom NPC is employed</param>
        public String getResponsibilities(PlayerCharacter pc)
        {
            String myResponsibilities = "";
            List<Fief> bailiffDuties = new List<Fief>();

            // check for employment function
            if (((this.myBoss != null) && (this.myBoss.Equals(pc.charID)))
                || ((this.familyID != null) && (this.familyID.Equals(pc.charID))))
            {
                // check PC's fiefs for bailiff
                foreach (Fief thisFief in pc.ownedFiefs)
                {
                    if (thisFief.bailiff == this)
                    {
                        bailiffDuties.Add(thisFief);
                    }
                }

                // create entry for bailiff duties
                if (bailiffDuties.Count > 0)
                {
                    myResponsibilities += "Bailiff (";
                    for (int i = 0; i < bailiffDuties.Count; i++ )
                    {
                        myResponsibilities += bailiffDuties[i].id;
                        if (i < (bailiffDuties.Count - 1))
                        {
                            myResponsibilities += ", ";
                        }
                    }
                    myResponsibilities += ")";
                }

                // check for army leadership
                if (this.armyID != null)
                {
                    if (!myResponsibilities.Equals(""))
                    {
                        myResponsibilities += ". ";
                    }
                    myResponsibilities += "Army leader (" + this.armyID + ").";
                }

                // if employee who isn't bailiff or army leader = 'Unspecified'
                if (myResponsibilities.Equals(""))
                {
                    if ((this.myBoss != null) && (this.myBoss.Equals(pc.charID)))
                    {
                        myResponsibilities = "Unspecified";
                    }
                }
            }

            return myResponsibilities;
        }

        /// <summary>
        /// Checks if recently born NPC still needs to be named
        /// </summary>
        /// <returns>bool indicating whether NPC needs naming</returns>
        /// <param name="age">NPC age to check for</param>
        public bool checkForName(byte age)
        {
            bool needsName = false;

            // look for NPC with age < 1 who has firstname of 'baby'
            if ((this.calcCharAge() == age) && ((this.firstName).ToLower().Equals("baby")))
            {
                needsName = true;
            }

            return needsName;
        }

        /// <summary>
        /// Calculates the potential salary (per season) for the NonPlayerCharacter,
        /// taking into account the stature of the hiring PlayerCharacter
        /// </summary>
        /// <returns>uint containing salary</returns>
        /// <param name="hiringPlayer">Hiring player</param>
        public uint calcWage(PlayerCharacter hiringPlayer)
        {
            double salary = 0;
            double basicSalary = 1500;

            // get fief management rating
            double fiefMgtRating = this.calcFiefManagementRating();

            // get army leadership rating
            double armyLeaderRating = this.calcArmyLeadershipRating();

            // determine lowest of 2 ratings
            double minRating = Math.Min(armyLeaderRating, fiefMgtRating);
            // determine highest of 2 ratings
            double maxRating = Math.Max(armyLeaderRating, fiefMgtRating);
            if (maxRating < 0)
            {
                maxRating = 0;
            }

            // calculate potential salary, based on highest rating
            salary = basicSalary * maxRating;
            // if appropriate, also including 'flexibility bonus' for lowest rating
            if (minRating > 0)
            {
                salary += (basicSalary * (minRating / 2));
            }

            // factor in hiring player's stature
            // (4% reduction in NPC's salary for each stature rank above 4)
            if (hiringPlayer.calculateStature() > 4)
            {
                double statMod = 1 - ((hiringPlayer.calculateStature() - 4) * 0.04);
                salary = salary * statMod;
            }

            return Convert.ToUInt32(salary);
        }

        /// <summary>
        /// Gets the character's head of family
        /// </summary>
        /// <returns>The head of family or null</returns>
        public PlayerCharacter getHeadOfFamily()
        {
            PlayerCharacter myHeadOfFamily = null;

            if (this.familyID != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.familyID))
                {
                    myHeadOfFamily = Globals_Server.pcMasterList[this.familyID];
                }
            }

            return myHeadOfFamily;
        }

        /// <summary>
        /// Gets the character's employer
        /// </summary>
        /// <returns>The employer or null</returns>
        public PlayerCharacter getEmployer()
        {
            PlayerCharacter myEmployer = null;

            if (this.myBoss != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(this.myBoss))
                {
                    myEmployer = Globals_Server.pcMasterList[this.myBoss];
                }
            }

            return myEmployer;
        }

    }

	/// <summary>
	/// Class used to convert Character to/from format suitable for Riak (JSON)
	/// </summary>
	public abstract class Character_Riak
	{

		/// <summary>
		/// Holds character ID
		/// </summary>
		public string charID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
        public String firstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public String familyName { get; set; }
        /// <summary>
        /// Tuple holding character's year and season of birth
        /// </summary>
        public Tuple<uint, byte> birthDate { get; set; }
        /// <summary>
		/// Holds if character male
		/// </summary>
		public bool isMale { get; set; }
		/// <summary>
		/// Holds character nationality
		/// </summary>
		public String nationality { get; set; }
        /// <summary>
        /// bool indicating whether character is alive
        /// </summary>
        public bool isAlive { get; set; }
        /// <summary>
		/// Holds character maximum health
		/// </summary>
		public Double maxHealth { get; set; }
		/// <summary>
		/// Holds character virility
		/// </summary>
		public Double virility { get; set; }
		/// <summary>
        /// Queue of Fiefs (fiefID) to auto-travel to
		/// </summary>
		public List<String> goTo { get; set; }
		/// <summary>
		/// Holds character language and dialect
		/// </summary>
        public Tuple<String, int> language { get; set; }
		/// <summary>
		/// Holds character's remaining days in season
		/// </summary>
		public double days { get; set; }
		/// <summary>
		/// Holds character's stature
		/// </summary>
		public Double statureModifier { get; set; }
		/// <summary>
		/// Holds character's management rating
		/// </summary>
		public Double management { get; set; }
		/// <summary>
		/// Holds character's combat rating
		/// </summary>
		public Double combat { get; set; }
		/// <summary>
		/// Array holding character's skills (skillID)
		/// </summary>
		public Tuple<String, int>[] skills { get; set; }
		/// <summary>
		/// bool indicating if character is in the keep
		/// </summary>
		public bool inKeep { get; set; }
		/// <summary>
		/// Holds character pregnancy status
		/// </summary>
		public bool pregnant { get; set; }
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
        /// Holds mother (Character ID)
        /// </summary>
        public String mother { get; set; }
        /// <summary>
        /// Holds charID of head of family with which character associated
		/// </summary>
		public String familyID { get; set; }
        /// <summary>
        /// Holds chaacter's fiancee (charID)
        /// </summary>
        public string fiancee { get; set; }
        /// <summary>
        /// Holds character's titles (fiefIDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }
        /// <summary>
        /// Holds ailments effecting character's health
        /// </summary>
        public Dictionary<string, Ailment> ailments = new Dictionary<string, Ailment>();

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
				this.familyName = charToUse.familyName;
                this.firstName = charToUse.firstName;
				this.birthDate = charToUse.birthDate;
				this.isMale = charToUse.isMale;
				this.nationality = charToUse.nationality;
                this.isAlive = charToUse.isAlive;
				this.maxHealth = charToUse.maxHealth;
				this.virility = charToUse.virility;
				this.goTo = new List<string> ();
				if (charToUse.goTo.Count > 0)
				{
					foreach (Fief value in charToUse.goTo)
					{
						this.goTo.Add (value.id);
					}
				}
				this.language = new Tuple<string,int>(charToUse.language.Item1.languageID, charToUse.language.Item2);
				this.days = charToUse.days;
				this.statureModifier = charToUse.statureModifier;
				this.management = charToUse.management;
				this.combat = charToUse.combat;
				this.skills = new Tuple<String, int>[charToUse.skills.Length];
				for (int i = 0; i < charToUse.skills.Length; i++)
				{
					this.skills [i] = new Tuple<string,int>(charToUse.skills [i].Item1.skillID, charToUse.skills [i].Item2);
				}
				this.inKeep = charToUse.inKeep;
				this.pregnant = charToUse.isPregnant;
				this.location = charToUse.location.id;
                this.spouse = charToUse.spouse;
                this.father = charToUse.father;
                this.mother = charToUse.mother;
                this.familyID = charToUse.familyID;
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
                this.ailments = charToUse.ailments;
                this.fiancee = charToUse.fiancee;
            }
		}

	}

	/// <summary>
	/// Class used to convert PlayerCharacter to/from format suitable for Riak (JSON)
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
		/// Holds character's employees and family (charID)
		/// </summary>
		public List<String> myNPCs = new List<String>();
		/// <summary>
		/// Holds character's owned fiefs (fiefID)
		/// </summary>
		public List<String> ownedFiefs = new List<String>();
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public String homeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public String ancestralHomeFief { get; set; }
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// </summary>
        public String playerID { get; set; }
        /// <summary>
        /// Holds character's armies (Army objects)
        /// </summary>
        public List<String> myArmies = new List<String>();
        /// <summary>
        /// Holds character's sieges (Siege objects)
        /// </summary>
        public List<string> mySieges = new List<string>();

		/// <summary>
		/// Constructor for PlayerCharacter_Riak
		/// </summary>
		/// <param name="pc">PlayerCharacter object to use as source</param>
		public PlayerCharacter_Riak(PlayerCharacter pc)
			: base(pc: pc)
		{

			this.outlawed = pc.outlawed;
			this.purse = pc.purse;
			if (pc.myNPCs.Count > 0)
			{
				for (int i = 0; i < pc.myNPCs.Count; i++)
				{
					this.myNPCs.Add (pc.myNPCs[i].charID);
				}
			}
			if (pc.ownedFiefs.Count > 0)
			{
				for (int i = 0; i < pc.ownedFiefs.Count; i++)
				{
					this.ownedFiefs.Add (pc.ownedFiefs[i].id);
				}
			}
            this.homeFief = pc.homeFief;
            this.ancestralHomeFief = pc.ancestralHomeFief;
            this.playerID = pc.playerID;
            if (pc.myArmies.Count > 0)
            {
                for (int i = 0; i < pc.myArmies.Count; i++ )
                {
                    this.myArmies.Add(pc.myArmies[i].armyID);
                }
            }
            this.mySieges = pc.mySieges;
		}

        /// <summary>
        /// Constructor for PlayerCharacter_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public PlayerCharacter_Riak()
		{
		}
	}

	/// <summary>
	/// Class used to convert NonPlayerCharacter to/from format suitable for Riak (JSON)
	/// </summary>
	public class NonPlayerCharacter_Riak : Character_Riak
	{

		/// <summary>
		/// Holds NPC's employer (charID)
		/// </summary>
		public String myBoss { get; set; }
		/// <summary>
		/// Holds NPC's wage
		/// </summary>
		public uint wage { get; set; }
		/// <summary>
		/// Holds last wage offer from individual PCs
		/// </summary>
		public Dictionary<string, uint> lastOffer { get; set; }
		/// <summary>
        /// Denotes if in employer's entourage
		/// </summary>
		public bool inEntourage { get; set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool isHeir { get; set; }


		/// <summary>
		/// Constructor for NonPlayerCharacter_Riak
		/// </summary>
		/// <param name="npc">NonPlayerCharacter object to use as source</param>
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
            this.isHeir = npc.isHeir;
		}

        /// <summary>
        /// Constructor for NonPlayerCharacter_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public NonPlayerCharacter_Riak()
		{
		}

	}

}
