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
        /// Holds character marital status
        /// </summary>
        public bool isMarried { get; set; }
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
        /// Holds current location (Fief object)
        /// </summary>
        public Fief location { get; set; }
        /// <summary>
        /// Holds character's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }
        /// <summary>
        /// Holds character's titles (fiefIDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }

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
        /// <param name="marr">char holding character marital status</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famID">String holding charID of head of family with which character associated</param>
        /// <param name="sp">String holding spouse (ID)</param>
        /// <param name="fath">String holding father</param>
        /// <param name="cl">GameClock holding season</param>
		/// <param name="loc">Fief holding current location</param>
        /// <param name="myTi">List holding character's titles (fiefIDs)</param>
        /// <param name="aID">String holding armyID of army character is leading</param>
        public Character(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, String nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool marr, bool preg,
            String famID, String sp, String fath, List<String> myTi, GameClock cl = null, Fief loc = null, String aID = null)
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

            // validate nat = Eng/Fr
            if ((!nat.Equals("Eng")) && (!nat.Equals("Fr")))
            {
                throw new InvalidDataException("Character nationality must be either 'Eng' or 'Fr'");
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
            this.isMarried = marr;
            this.isPregnant = preg;
            this.clock = cl;
			this.location = loc;
            this.spouse = sp;
            this.father = fath;
            this.familyID = famID;
            this.myTitles = myTi;
            this.armyID = aID;
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
				this.isMarried = charToUse.married;
				this.isPregnant = charToUse.pregnant;
				if ((charToUse.spouse != null) && (charToUse.spouse.Length > 0))
				{
					this.spouse = charToUse.spouse;
				}
				if ((charToUse.father != null) && (charToUse.father.Length > 0))
				{
					this.father = charToUse.father;
				}
				if ((charToUse.familyID != null) && (charToUse.familyID.Length > 0))
				{
					this.familyID = charToUse.familyID;
				}
				this.clock = null;
				this.location = null;
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
			}
		}

        /// <summary>
        /// Calculates character's age
        /// </summary>
        /// <returns>byte containing character's age</returns>
        public byte calcCharAge()
        {
            byte myAge = 0;

            // subtract year of birth from current year
            myAge = Convert.ToByte(clock.currentYear - this.birthDate.Item1);
            // if current season < season of birth, subtract 1 from age (not reached birthday yet)
            if (clock.currentSeason < this.birthDate.Item2)
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
                if (Globals.fiefMasterList[fiefID].rank.stature > highRankStature)
                {
                    highRankStature = Globals.fiefMasterList[fiefID].rank.stature;
                }
            }

            return highRankStature;
        }
       
        /// <summary>
        /// Calculates character's base or current stature
        /// </summary>
        /// <returns>Double containing character's base stature</returns>
        /// <param name="type">bool indicating whether to return current stature (or just base)</param>
        public Double calculateStature(bool currentStature)
        {
            Double stature = 0;

            // get stature for character's highest rank
            stature = this.getHighRankStature();

            // factor in age
            if ((this.calcCharAge() > 10) && (this.calcCharAge() < 21))
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
            /* if (currentHealth)
            {
                charHealth = charHealth + this.ailments;
            } */

            return charHealth;
        }

        /// <summary>
        /// Checks for character death
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
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
            if ((Globals.GetRandomDouble(100)) <= deathChance)
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
        public void processDeath()
        {
            // 1. set isAlive = false
            this.isAlive = false;

            // 2. remove from fief
            this.location.characters.Remove(this);

            // 3. check if involved with any scheduled events and remove
            // to store events to remove
            List<JournalEvent> eventsToRemove = new List<JournalEvent>();
            // iterate through clock's scheduled events
            foreach (JournalEvent jEvent in this.clock.scheduledEvents.events)
            {
                // if event concerned with this character
                if (jEvent.personae.Equals(this.charID))
                {
                    // store event
                    eventsToRemove.Add(jEvent);
                }
            }

            // if events found, remove them
            if (eventsToRemove.Count > 0)
            {
                foreach (JournalEvent thisEvent in eventsToRemove)
                {
                    this.clock.scheduledEvents.events.Remove(thisEvent);
                }

            }
            // tidy up
            eventsToRemove.Clear();

            // 4. remove from fief barred lists
            if (this.location.barredCharacters.Contains(this.charID))
            {
                this.location.barredCharacters.Remove(this.charID);
            }

            // 5. if married, remove from spouse
            if (this.isMarried)
            {
                if (Globals.npcMasterList.ContainsKey(this.spouse))
                {
                    Globals.npcMasterList[this.spouse].spouse = null;
                    Globals.npcMasterList[this.spouse].isMarried = false;
                }
                else if (Globals.pcMasterList.ContainsKey(this.spouse))
                {
                    Globals.pcMasterList[this.spouse].spouse = null;
                    Globals.pcMasterList[this.spouse].isMarried = false;
                }
                
            }

            // 6. (NPC) check and remove from PC myNPCs list
            // 7. (NPC) check and remove from bailiff positions
            if (this is NonPlayerCharacter)
            {
                // if is an employee
                if ((this as NonPlayerCharacter).myBoss != null)
                {
                    // get boss
                    PlayerCharacter boss = Globals.pcMasterList[(this as NonPlayerCharacter).myBoss];

                    // check to see if is a bailiff.  If so, remove
                    foreach (Fief thisFief in boss.ownedFiefs)
                    {
                        if (thisFief.bailiff == this)
                        {
                            thisFief.bailiff = null;
                        }
                    }

                    // remove from boss's myNPCs
                    boss.myNPCs.Remove((this as NonPlayerCharacter));
                }

                // if is a family member
                if ((this as NonPlayerCharacter).familyID != null)
                {
                    // get boss
                    PlayerCharacter familyHead = Globals.pcMasterList[(this as NonPlayerCharacter).familyID];

                    // check to see if is a bailiff.  If so, remove
                    foreach (Fief thisFief in familyHead.ownedFiefs)
                    {
                        if (thisFief.bailiff == this)
                        {
                            thisFief.bailiff = null;
                        }
                    }

                    // remove from head of family's myNPCs
                    familyHead.myNPCs.Remove((this as NonPlayerCharacter));
                }

                // TODO: inform PC
            }

            // TODO: (PC) check and remove from bailiff positions

            // TODO: check and remove from army leadership

            // TODO: clear titles and assign to heir (for PC) or owner (for NPC)

            // TODO: (Player) transfer dead player PC to chosen heir = create new PC from NPC
            // transfer fiefs, titles, ancestral ownership, employees, change family functions
            // change familyID of all family members
            // IF NO HEIR?

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
                if (this.nationality.Equals("Eng"))                    
                {                       
                    success = false;
                    System.Windows.Forms.MessageBox.Show("Bailiff: The duplicitous English are barred from entering this keep, Good Sir!");
                }               
            }

            // if character is French and French barred, don't allow entry
            else if (location.frenchBarred)
            {
                if (this.nationality.Equals("Fr"))
                {
                    success = false;
                    System.Windows.Forms.MessageBox.Show("Bailiff: The perfidious French are barred from entering this keep, Mon Seigneur!");
                }
            }

            // if character is specifically barred, don't allow entry
            else 
            {
                if (location.barredCharacters.Contains(this.charID))
                {
                    success = false;
                    System.Windows.Forms.MessageBox.Show("Bailiff: Your person is barred from entering this keep, Good Sir!");
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
            loyModif = (((this.calculateStature(true) + this.management) / 2) - 1) * 1.25;
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
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="isUpdate">Indicates if is update mode</param>
        public virtual bool moveCharacter(Fief target, double cost, bool isUpdate = false)
        {
            bool success = false;

            if (!isUpdate)
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
                            System.Windows.Forms.MessageBox.Show("Move cancelled.");
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
                    Globals.armyMasterList[armyID].moveArmy();
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

                if (!isUpdate)
                {
                    System.Windows.Forms.MessageBox.Show("I'm afraid you've run out of days.\r\nYour journey will continue next season.");
                }
            }

            return success;
        }

        /// <summary>
        /// Appoints the character as leader of the specified army
        /// </summary>
        /// <param name="thisArmy">The Army the character will lead</param>
        public void appointAsLeader(Army thisArmy)
        {
            // Remove army from current leader
            Character oldLeader = thisArmy.getLeader();
            oldLeader.armyID = null;            

            // add army to new leader
            this.armyID = thisArmy.armyID;

            // in army, set new leader
            thisArmy.leader = this.charID;

            // if new leader is NPC, remove from player's entourage
            if (this is NonPlayerCharacter)
            {
                (this as NonPlayerCharacter).inEntourage = false;
            }

            // calculate days synchronisation
            double minDays = Math.Min(thisArmy.days, this.days);
            double daysToAdjust = 0;

            if (this.days != minDays)
            {
                daysToAdjust = (this.days - minDays);
            }
            else
            {
                // check for attrition (i.e. army had to wait for leader)
                byte attritionChecks = 0;
                attritionChecks = Convert.ToByte((thisArmy.days - this.days) / 7);

                for (int i = 0; i < attritionChecks; i++)
                {
                    uint attrition = thisArmy.calcAttrition();
                    thisArmy.foot = thisArmy.foot - attrition;
                }

            }

            // synchronise days
            this.adjustDays(daysToAdjust);
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
                Army thisArmy = Globals.armyMasterList[this.armyID];

                // synchronise days
                thisArmy.days = this.days;
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
        public bool getSpousePregnant(NonPlayerCharacter wife)
        {
            bool success = false;

            // make sure not already pregnant
            if (wife.isPregnant)
            {
                System.Windows.Forms.MessageBox.Show(wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!", "PREGNANCY ATTEMPT CANCELLED");
            }
            else
            {
                // ensure player and spouse have at least 1 day remaining
                double minDays = Math.Min(this.days, wife.days);

                if (minDays < 1)
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, you don't have enough time left for this in the current season.", "PREGNANCY ATTEMPT CANCELLED");
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
                    double randPercentage = Globals.GetRandomDouble(100);

                    // holds chance of pregnancy based on age and virility
                    int chanceOfPregnancy = 0;

                    // holds pregnancy modifier based on virility
                    double pregModifier = 0;

                    // spouse's age
                    byte spouseAge = wife.calcCharAge();

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
                            uint birthYear = this.clock.currentYear;
                            byte birthSeason = this.clock.currentSeason;
                            if (this.clock.currentSeason == 0)
                            {
                                birthSeason = (byte)(birthSeason + 3);
                            }
                            else
                            {
                                birthSeason = (byte)(birthSeason - 1);
                                birthYear = birthYear + 1;
                            }
                            JournalEvent birth = new JournalEvent(birthYear, birthSeason, wife.charID, "birth");
                            this.clock.scheduledEvents.events.Add(birth);

                             // display message of celebration
                            System.Windows.Forms.MessageBox.Show("Let the bells ring out, milord.  " + wife.firstName + " " + wife.familyName + " is pregnant!", "PREGNANCY SUCCESSFUL");
                            success = true;
                        }
                        // if attempt not successful
                        else
                        {
                            // display encouraging message
                            System.Windows.Forms.MessageBox.Show("I'm afraid " + wife.firstName + " " + wife.familyName + " is not pregnant.  Better luck next time, milord!", "PREGNANCY UNSUCCESSFUL");
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
                        System.Windows.Forms.MessageBox.Show("Ahem ...\r\n\r\nUnfortunately, the fief physician advises that " + wife.firstName + " " + wife.familyName + " will never get pregnant with her current partner", "PREGNANCY UNSUCCESSFUL");
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
            lv = (this.combat + this.management + this.calculateStature(true)) / 3;

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
            cv = (this.combat + this.calculateHealth()) / 2;

            // factor in armour
            cv = cv + 5;

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
        /// Updates character data at the end/beginning of the season
        /// </summary>
        public void updateCharacter()
        {
            // check for character death
            if (this.checkDeath())
            {
                this.isAlive = false;
            }

            if (this.isAlive)
            {
                if (this is NonPlayerCharacter)
                {
                    // if (age >= 1) && (firstName.Equals("Baby")), character firstname = king's
                    if (this.familyID != null)
                    {
                        if ((this as NonPlayerCharacter).checkForName(1))
                        {
                            // get king's firstName
                            String newFirstName = "";
                            newFirstName = Globals.fiefMasterList[Globals.pcMasterList[this.familyID].homeFief].province.kingdom.king.firstName;
                            this.firstName = newFirstName;
                        }
                    }
                }

                // reset days
                this.days = 90;
            }

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
        public PlayerCharacter(string id, String firstNam, String famNam, Tuple<uint, byte> dob, bool isM, String nat, bool alive, Double mxHea, Double vir,
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool marr, bool preg, String famHead,
            String sp, String fath, bool outl, uint pur, List<NonPlayerCharacter> npcs, List<Fief> owned, String home, String ancHome, List<String> myTi, List<Army> myA, GameClock cl = null, Fief loc = null, String pID = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, famHead, sp, fath, myTi, cl, loc)
        {
            this.outlawed = outl;
            this.purse = pur;
            this.myNPCs = npcs;
            this.ownedFiefs = owned;
            this.homeFief = home;
            this.ancestralHomeFief = ancHome;
            this.playerID = pID;
            this.myArmies = myA;
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
            double chance = Globals.GetRandomDouble(100);

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
                System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": You've made me an offer I can't refuse, Milord!");
            }

            // automatically reject if offer < 10% below potential salary
            else if (offer < minAcceptable)
            {
                accepted = false;
                System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": Don't insult me, Sirrah!");
            }

            // automatically reject if offer !> previous offer
            else if (offerLess)
            {
                accepted = false;
                System.Windows.Forms.MessageBox.Show("You must improve on your previous offer (£" + npc.lastOffer[this.charID] + ")");
            }

            else
            {
                // see where offer lies (as %) within rangeNegotiable
                double offerPercentage = ((offer - minAcceptable) / rangeNegotiable) * 100;
                // compare randomly generated % (chance) with offerPercentage
                if (chance <= offerPercentage)
                {
                    accepted = true;
                    System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": It's a deal, Milord!");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(npc.firstName + " " + npc.familyName + ": You'll have to do better than that, Good Sir!");
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
                            System.Windows.Forms.MessageBox.Show(toDisplay);
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
        /// <param name="isUpdate">Indicates if is update mode</param>
        public override bool moveCharacter(Fief target, double cost, bool isUpdate = false)
        {

            // use base method to move PlayerCharacter
            bool success = base.moveCharacter(target, cost, isUpdate);

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
            Fief homeFief = Globals.fiefMasterList[this.homeFief];

            // get army
            Army thisArmy = Globals.armyMasterList[this.armyID];

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
                System.Windows.Forms.MessageBox.Show(toDisplay);
            }
            else
            {
                // 2. see if recruitment already occurred for this season
                if (this.location.hasRecruited)
                {
                    proceed = false;
                    toDisplay = "I'm afraid you have already recruited here in this season, my lord.";
                    System.Windows.Forms.MessageBox.Show(toDisplay);
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
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                    else
                    {
                        // 4. check sufficient funds for at least 1 troop
                        if (!(homeFief.getAvailableTreasury() > indivTroopCost))
                        {
                            proceed = false;
                            toDisplay = "I'm sorry, my Lord; you have insufficient funds for recruitment.";
                            System.Windows.Forms.MessageBox.Show(toDisplay);
                        }
                        else
                        {
                            // 5. check sufficient days remaining
                            // see how long recuitment attempt will take: generate random int (1-5)
                            daysUsed = Globals.myRand.Next(6);

                            if (this.days < daysUsed)
                            {
                                proceed = false;
                                toDisplay = "I'm afraid you have run out of days, my lord.";
                                System.Windows.Forms.MessageBox.Show(toDisplay);
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
                    System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
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
                // generate random double (2-5) to see the % of population responding to call
                double popPercent = (Globals.GetRandomDouble(5, min: 2) / 100);

                // calculate number of troops responding to call and adjust if necessary
                troopsRecruited = Convert.ToInt32(this.location.population * popPercent);
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
                        System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
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
                        System.Windows.Forms.MessageBox.Show("Recruitment cancelled");
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
                    // add new troops to army
                    thisArmy.foot += Convert.ToUInt32(troopsRecruited);
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
            Queue<Fief> go, Tuple<Language, int> lang, double day, Double stat, Double mngmnt, Double cbt, Tuple<Skill, int>[] skl, bool inK, bool marr, bool preg, String famHead,
            String sp, String fath, uint wa, bool inEnt, bool isH, List<String> myTi, String mb = null, GameClock cl = null, Fief loc = null)
            : base(id, firstNam, famNam, dob, isM, nat, alive, mxHea, vir, go, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, famHead, sp, fath, myTi, cl, loc)
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
        /// Calculates the family allowance of a family NPC, based on age and function
        /// </summary>
        /// <returns>uint containing family allowance</returns>
        /// <param name="func">NPC's function</param>
        public uint calcFamilyAllowance(String func)
        {
            uint famAllowance = 0;

            // factor in family function
            if (func.ToLower().Equals("wife"))
            {
                famAllowance = 30000;
            }
            else
            {
                if (func.ToLower().Equals("heir"))
                {
                    famAllowance = 20000;
                }
                else if ((func.ToLower().Equals("son")) || (func.ToLower().Equals("son-in-law")))
                {
                    famAllowance = 15000;
                }
                else if ((func.ToLower().Equals("daughter")) || (func.ToLower().Equals("daughter-in-law")))
                {
                    famAllowance = 10000;
                }
                else if (func.ToLower().Contains("grand"))
                {
                    famAllowance = 10000;
                }

                // factor in age
                if ((this.calcCharAge() > 14) && (this.calcCharAge() < 21))
                {
                    famAllowance = Convert.ToUInt32(famAllowance * 1.5);
                }
                else if (this.calcCharAge() > 20)
                {
                    famAllowance = famAllowance * 2;
                }
            }

            return famAllowance;
        }

        /// <summary>
        /// Derives NPC function
        /// </summary>
        /// <returns>String containing NPC function</returns>
        /// <param name="bigCheese">PlayerCharacter with whom NPC has relationship</param>
        public String getFunction(PlayerCharacter bigCheese)
        {
            String myFunction = "";
            bool isFirstEntry = true;

            // check for family function
            if (this.familyID != null)
            {
                if (this.familyID.Equals(bigCheese.charID))
                {
                    if (!isFirstEntry)
                    {
                        myFunction += " & ";
                    }
                    else
                    {
                        isFirstEntry = false;
                    }

                    // check for wife
                    if (this.spouse != null)
                    {
                        if (this.spouse.Equals(bigCheese.charID))
                        {
                            myFunction += "Wife";
                        }
                    }

                    else if (this.father != null)
                    {
                        // check for son/daughter
                        if (this.father.Equals(bigCheese.charID))
                        {
                            if (this.isMale)
                            {
                                myFunction += "Son";
                            }
                            else
                            {
                                myFunction += "Daughter";
                            }
                        }

                        else
                        {
                            // check for grandkids
                            if (Globals.npcMasterList[this.father].father != null)
                            {
                                if (Globals.npcMasterList[this.father].father.Equals(bigCheese.charID))
                                {
                                    if (this.isMale)
                                    {
                                        myFunction += "Grandson";
                                    }
                                    else
                                    {
                                        myFunction += "Granddaughter";
                                    }
                                }
                            }
                        }
                    }

                    // if none of above, must be daughter-in-law
                    else
                    {
                        if (!this.isMale)
                        {
                            myFunction += "Daughter-in-Law";
                        }
                    }

                    // check for heir
                    if (this.isHeir)
                    {
                        myFunction += " & Heir";
                    }

                }
            }

            // check for employment function
            if (this.myBoss != null)
            {
                if (this.myBoss.Equals(bigCheese.charID))
                {
                    // check if is bailiff of any fiefs
                    for (int i = 0; i < bigCheese.ownedFiefs.Count; i++ )
                    {
                        if (bigCheese.ownedFiefs[i].bailiff == this)
                        {
                            if (! isFirstEntry)
                            {
                                myFunction += " & ";
                            }
                            else
                            {
                                isFirstEntry = false;
                            }

                            myFunction += "Bailiff of " + bigCheese.ownedFiefs[i].name + " (" + bigCheese.ownedFiefs[i].fiefID + ")";
                        }
                    }

                    // check if is army leader
                    if (this.armyID != null)
                    {
                        if (!isFirstEntry)
                        {
                            myFunction += " & ";
                        }
                        else
                        {
                            isFirstEntry = false;
                        }

                        myFunction += "Leader of " + this.armyID;
                    }

                    if (myFunction.Equals(""))
                    {
                        myFunction += "Unspecified";
                    }
                }
            }

            return myFunction;
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
        public uint calcWage(PlayerCharacter pc)
        {
            double salary = 0;
            double basicSalary = 1500;

            // calculate fief management rating
            // baseline rating
            double fiefMgtRating = (this.management + this.calculateStature(true)) / 2;
            // check for skills effecting fief loyalty
            double fiefLoySkill = this.calcSkillEffect("fiefLoy");
            // check for skills effecting fief expenses
            double fiefExpSkill = this.calcSkillEffect("fiefExpense");
            // combine skills into single modifier. Note: fiefExpSkill is * by -1 because 
            // a negative effect on expenses is good, so needs to be normalised
            double mgtSkills = (fiefLoySkill + (-1 * fiefExpSkill));
            // calculate final fief management rating
            fiefMgtRating = fiefMgtRating + (fiefMgtRating * mgtSkills);

            // calculate combat rating
            // baseline rating
            double combatRating = (this.management + this.calculateStature(true) + this.combat) / 3;
            // check for skills effecting battle
            double battleSkills = this.calcSkillEffect("battle");
            // check for skills effecting siege
            double siegeSkills = this.calcSkillEffect("siege");
            // combine skills into single modifier 
            double combatSkills = battleSkills + siegeSkills;
            // calculate final combat rating
            combatRating = combatRating + (combatRating * combatSkills);

            // determine lowest of 2 ratings
            double minRating = Math.Min(combatRating, fiefMgtRating);
            // determine highest of 2 ratings
            double maxRating = Math.Max(combatRating, fiefMgtRating);

            // calculate potential salary, mainly based on highest rating
            // but also including 'flexibility bonus' for lowest rating
            salary = (basicSalary * maxRating) + (basicSalary * (minRating / 2));

            // factor in hiring player's stature
            // (4% reduction in NPC's salary for each stature rank above 4)
            if (this.calculateStature(true) > 4)
            {
                double statMod = 1 - ((this.calculateStature(true) - 4) * 0.04);
                salary = salary * statMod;
            }

            return Convert.ToUInt32(salary);
        }

    }

	/// <summary>
	/// Class used to convert Character to/from format suitable for Riak (JSON)
	/// </summary>
	public class Character_Riak
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
        /// Holds charID of head of family with which character associated
		/// </summary>
		public String familyID { get; set; }
        /// <summary>
        /// Holds character's titles (fiefIDs)
        /// </summary>
        public List<String> myTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public String armyID { get; set; }

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
						this.goTo.Add (value.fiefID);
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
				this.married = charToUse.isMarried;
				this.pregnant = charToUse.isPregnant;
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
				if (charToUse.familyID != null) {
					this.familyID = charToUse.familyID;
				} else {
					this.familyID = null;
				}
                this.myTitles = charToUse.myTitles;
                this.armyID = charToUse.armyID;
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
					this.ownedFiefs.Add (pc.ownedFiefs[i].fiefID);
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
