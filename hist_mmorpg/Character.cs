using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on character
    /// </summary>
    public class Character
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
        /// Holds current location (Fief object)
        /// </summary>
        public Fief location { get; set; }
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
        /// Holds army's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }
        /// <summary>
        /// Holds spouse (Character)
        /// </summary>
        public Character spouse { get; set; }
        /// <summary>
        /// Holds father (Character)
        /// </summary>
        public Character father { get; set; }
        /// <summary>
        /// Holds head of family (Character)
        /// </summary>
        public Character familyHead { get; set; }

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
        /// <param name="loc">Fief holding character location (fief ID)</param>
        /// <param name="lang">String holding character language code</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">Double holding character status rating</param>
        /// <param name="mngmnt">Double holding character management rating</param>
        /// <param name="cbt">Double holding character combat rating</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="marr">char holding character marital status</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="cl">GameClock holding season</param>
        /// <param name="sp">Character holding spouse</param>
        /// <param name="fath">Character holding father</param>
        /// <param name="famHead">Character holding head of family</param>
        public Character(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Fief loc, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg,
            GameClock cl, Character sp = null, Character fath = null, Character famHead = null)
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

            // validate preg = only if character female
            if ((isM) && (preg))
            {
                throw new InvalidDataException("Male characters cannot be pregnant!");
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
            this.location = loc;
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
            this.spouse = sp;
            this.father = fath;
            this.familyHead = famHead;
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
            Fief loc, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg,
            GameClock cl, bool outl, uint pur, List<NonPlayerCharacter> emp, List<Fief> kps, Character sp = null, Character fath = null, Character famHead = null)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, loc, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, cl, sp, fath, famHead)
        {

            this.outlawed = outl;
            this.purse = pur;
            this.employees = emp;
            this.ownedFiefs = kps;
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
            npc.myBoss = this;
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
        /// <param name="ch">NPC to be added</param>
        public void addToEntourage(NonPlayerCharacter ch)
        {
            ch.inEntourage = true;
        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="ch">NPC to be removed</param>
        public void removeFromEntourage(NonPlayerCharacter ch)
        {
            ch.inEntourage = false;
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

    }

    /// <summary>
    /// Class storing data on non player character
    /// </summary>
    public class NonPlayerCharacter : Character
    {

        /// <summary>
        /// Holds NPC's boss
        /// </summary>
        public Character myBoss { get; set; }
        /// <summary>
        /// Holds fief ID for destination (specified by NPC's boss)
        /// </summary>
        public string goTo { get; set; }
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
        /// <param name="mb">uint holding NPC's boss (ID)</param>
        /// <param name="go">String holding fief ID for destination (specified by NPC's boss)</param>
        /// <param name="wa">string holding NPC's wages</param>
        /// <param name="inEnt">bool denoting if in/out of boss's entourage</param>
        public NonPlayerCharacter(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Fief loc, string lang, double day, Double stat, Double mngmnt, Double cbt, Skill[] skl, bool inK, bool marr, bool preg,
            GameClock cl, string go, uint wa, bool inEnt, Character mb = null, Character sp = null, Character fath = null, Character famHead = null)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, loc, lang, day, stat, mngmnt, cbt, skl, inK, marr, preg, cl, sp, fath, famHead)
        {
            // TODO: validate hb = 1-10000
            // TODO: validate go = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // TODO: validate wa = uint

            this.myBoss = mb;
            this.goTo = go;
            this.wage = wa;
            this.inEntourage = inEnt;
            this.lastOffer = new Dictionary<string, uint>();
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

}
