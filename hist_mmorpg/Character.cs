using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on character
    /// </summary>
    public class Character : Mmorpg_Model
    {

        /// <summary>
        /// List of observers registered to observe this subject
        /// </summary>
        private List<Form1> registeredObservers = new List<Form1>();
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
        public uint days { get; set; }
        /// <summary>
        /// Holds character's stature
        /// </summary>
        public Double stature { get; set; }
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
        /// Holds ID of spouse
        /// </summary>
        public string spouseID { get; set; }
        /// <summary>
        /// Holds character pregnancy status
        /// </summary>
        public bool pregnant { get; set; }
        /// <summary>
        /// Holds ID of father
        /// </summary>
        public string father { get; set; }
        /// <summary>
        /// Holds ID of head of family
        /// </summary>
        public string familyHead { get; set; }

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
        /// <param name="day">uint holding character remaining days in season</param>
        /// <param name="stat">Double holding character status rating</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="marr">char holding character marital status</param>
        /// <param name="spID">string holding spouse ID</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="fath">string holding father ID</param>
        /// <param name="famHead">string holding head of family ID</param>
        public Character(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Fief loc, string lang, uint day, Double stat, Skill[] skl, bool inK, bool marr, string spID, bool preg,
            string fath, string famHead)
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
            if (day > 90)
            {
                throw new InvalidDataException("Character remaining days must be an integer between 0 and 90");
            }

            // validate stat = 0-9.00
            if (stat > 9)
            {
                throw new InvalidDataException("Character stature must be a double between 0 and 9");
            }

            // TODO: validate spID = 1-10000?
            // ensure married characters have a spID and unmarried ones don't
            if ((! marr) && (! spID.Equals("NA")))
            {
                throw new InvalidDataException("For unmarried characters, spouseID must be 'NA'");
            }
            if ((marr) && (spID.Equals("NA")))
            {
                throw new InvalidDataException("Married characters should have a spouseID");
            }

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
            this.skills = skl;
            this.inKeep = inK;
            this.married = marr;
            this.spouseID = spID;
            this.pregnant = preg;
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
            int deathModifier = 0;
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

            // calculate base chance of death
            Double deathChance = (10 - this.health) * 2.8;

            // apply modifier (if exists)
            if (deathModifier != 0)
            {
                deathChance = deathChance + (deathChance * (deathModifier / 100));
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

                /* for (int i = 0; i < location.data.barredCharacters.Count; i++)
                {

                    if (this.charID.Equals(location.data.barredCharacters[i]))
                    {
                        success = false;
                        break;
                    }
                } */
            }

            this.inKeep = success;
            return success;
        }

        /// <summary>
        /// Adds a view (Form1 object) to the list of registered views
        /// </summary>
        /// <param name="obs">Form1 object to be added</param>
        public void registerObserver(Form1 obs)
        {
            // add new view to list
            registeredObservers.Add(obs);
        }

        /// <summary>
        /// Removes a view (Form1 object) from the list of registered views
        /// </summary>
        /// <param name="obs">Form1 object to be removed</param>
        public void removeObserver(Form1 obs)
        {
            // remove view from list
            registeredObservers.Remove(obs);
        }

        /// <summary>
        /// Notifies all views (Form1 objects) in the list of registered views
        /// that a change has occured to data in the model
        /// </summary>
        /// <param name="info">String indicating the type of data that has been changed</param>
        public void notifyObservers(String info)
        {
            // iterate through list of views
            foreach (Form1 obs in registeredObservers)
            {
                // call view's update method to perform the required actions
                // based on the string passed
                obs.update(info);
            }
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
        public List<Character> entourage = new List<Character>();

        /// <summary>
        /// Constructor for PlayerCharacter
        /// </summary>
        /// <param name="outl">bool holding character outlawed status</param>
        /// <param name="pur">uint holding character purse</param>
        public PlayerCharacter(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Fief loc, string lang, uint day, Double stat, Skill[] skl, bool inK, bool marr, string spID, bool preg, string fath,
            string famHead, bool outl, uint pur, List<Character> ent)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, loc, lang, day, stat, skl, inK, marr, spID, preg, fath, famHead)
        {

            this.outlawed = outl;
            this.purse = pur;
            this.entourage = ent;
        }

        /// <summary>
        /// Adds an NPC to the character's entourage
        /// </summary>
        /// <param name="ch">NPC to be added</param>
        public void addToEntourage(Character ch)
        {
            this.entourage.Add(ch);
        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="ch">NPC to be added</param>
        public void removeFromEntourage(Character ch)
        {
            this.entourage.Remove(ch);
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
                for (int i = 0; i < this.entourage.Count; i++)
                {
                    if (location.barredCharacters.Contains(this.entourage[i].charID))
                    {
                        this.entourage[i].inKeep = false;
                    }
                    else
                    {
                        this.entourage[i].inKeep = true;
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
        /// Holds NPC's boss (ID)
        /// </summary>
        public string hiredBy { get; set; }
        /// <summary>
        /// Holds fief ID for destination (specified by NPC's boss)
        /// </summary>
        public string goTo { get; set; }
        /// <summary>
        /// Holds NPC's wages
        /// </summary>
        public uint wage { get; set; }

        /// <summary>
        /// Constructor for NonPlayerCharacter
        /// </summary>
        /// <param name="hb">uint holding NPC's boss (ID)</param>
        /// <param name="go">String holding fief ID for destination (specified by NPC's boss)</param>
        /// <param name="wa">string holding NPC's wages</param>
        public NonPlayerCharacter(string id, String nam, uint ag, bool isM, String nat, Double hea, Double mxHea, Double vir,
            Fief loc, string lang, uint day, Double stat, Skill[] skl, bool inK, bool marr, string spID, bool preg, string fath,
            string famHead, string hb, string go, uint wa)
            : base(id, nam, ag, isM, nat, hea, mxHea, vir, loc, lang, day, stat, skl, inK, marr, spID, preg, fath, famHead)
        {
            // TODO: validate hb = 1-10000
            // TODO: validate go = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,LN,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // TODO: validate wa = uint

            this.hiredBy = hb;
            this.goTo = go;
            this.wage = wa;
        }

    }

}
