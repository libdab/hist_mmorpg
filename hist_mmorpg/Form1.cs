using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickGraph;

namespace hist_mmorpg
{
    public partial class Form1 : Form, Mmorpg_View
    {

        /// <summary>
        /// Holds all Character objects
        /// </summary>
        private List<Character> charMasterList = new List<Character>();
        /// <summary>
        /// Holds all Fief objects
        /// </summary>
        private List<Fief> fiefMasterList = new List<Fief>();
        /// <summary>
        /// Holds CharacterModel
        /// </summary>
        private CharacterModel charModel;
        /// <summary>
        /// Holds FiefModel
        /// </summary>
        private FiefModel fModel;
        /// <summary>
        /// Holds Character to view
        /// </summary>
        private Character charToView;
        /// <summary>
        /// Holds Fief to view
        /// </summary>
        private Fief fiefToView;
        /// <summary>
        /// Holds HexMapGraph
        /// </summary>
        private HexMapGraph gameMap;
        /// <summary>
        /// Holds army's GameClock (season)
        /// </summary>
        public GameClock clock { get; set; }

        /// <summary>
        /// Constructor for Form1
        /// </summary>
        /// <param name="cm">CharacterModel holding model</param>
        /// <param name="nam">FiefModel holding model</param>
        public Form1(CharacterModel cm, FiefModel fm)
        {
            this.charModel = cm;
            this.fModel = fm;
            // registers as view of models
            cm.registerObserver(this);
            fm.registerObserver(this);
            // initialise display
            InitializeComponent();
            // create game objects
            PlayerCharacter thisPC = this.initGameObjects();
            // inform models of initial game objects
            cm.changeCurrent(thisPC);
            fm.changeCurrent(thisPC.location);
            this.setUpFiefsList();
            this.setUpCourtCharsList();
            this.characterContainer.BringToFront();
        }

        public PlayerCharacter initGameObjects()
        {
            // creat GameClock
            GameClock myGameClock = new GameClock(1320);
            this.clock = myGameClock;

            // create skills
            // Dictionary to hold collection of skills
            Dictionary<string, Skill> skillsCollection = new Dictionary<string, Skill>();

            // List to holds skill keys (for random selection)
            List<string> skillsKeys = new List<string>();

            // Dictionary of skill effects
            Dictionary<string, int> effectsCommand = new Dictionary<string, int>();
            effectsCommand.Add("battle", 40);
            effectsCommand.Add("siege", 40);
            effectsCommand.Add("npcHire", 20);
            // create skill
            Skill command = new Skill("Command", effectsCommand);
            // add to skillsCollection
            skillsCollection.Add(command.name, command);

            Dictionary<string, int> effectsChivalry = new Dictionary<string, int>();
            effectsChivalry.Add("famExpense", 20);
            effectsChivalry.Add("fiefExpense", 10);
            effectsChivalry.Add("fiefLoy", 20);
            effectsChivalry.Add("npcHire", 10);
            effectsChivalry.Add("siege", 10);
            Skill chivalry = new Skill("Chivalry", effectsChivalry);
            skillsCollection.Add(chivalry.name, chivalry);

            Dictionary<string, int> effectsAbrasiveness = new Dictionary<string, int>();
            effectsAbrasiveness.Add("battle", 15);
            effectsAbrasiveness.Add("death", 5);
            effectsAbrasiveness.Add("fiefExpense", -5);
            effectsAbrasiveness.Add("famExpense", 5);
            effectsAbrasiveness.Add("time", 5);
            effectsAbrasiveness.Add("siege", -10);
            Skill abrasiveness = new Skill("Abrasiveness", effectsAbrasiveness);
            skillsCollection.Add(abrasiveness.name, abrasiveness);

            Dictionary<string, int> effectsAccountancy = new Dictionary<string, int>();
            effectsAccountancy.Add("time", 10);
            effectsAccountancy.Add("fiefExpense", -20);
            effectsAccountancy.Add("famExpense", -20);
            effectsAccountancy.Add("fiefLoy", -5);
            Skill accountancy = new Skill("Accountancy", effectsAccountancy);
            skillsCollection.Add(accountancy.name, accountancy);

            Dictionary<string, int> effectsStupidity = new Dictionary<string, int>();
            effectsStupidity.Add("battle", -40);
            effectsStupidity.Add("death", 5);
            effectsStupidity.Add("fiefExpense", 20);
            effectsStupidity.Add("famExpense", 20);
            effectsStupidity.Add("fiefLoy", -10);
            effectsStupidity.Add("npcHire", -10);
            effectsStupidity.Add("time", -10);
            effectsStupidity.Add("siege", -40);
            Skill stupidity = new Skill("Stupidity", effectsStupidity);
            skillsCollection.Add(stupidity.name, stupidity);

            Dictionary<string, int> effectsRobust = new Dictionary<string, int>();
            effectsRobust.Add("virility", 20);
            effectsRobust.Add("npcHire", 5);
            effectsRobust.Add("fiefLoy", 5);
            effectsRobust.Add("death", -20);
            Skill robust = new Skill("Robust", effectsRobust);
            skillsCollection.Add(robust.name, robust);

            Dictionary<string, int> effectsPious = new Dictionary<string, int>();
            effectsPious.Add("virility", -20);
            effectsPious.Add("npcHire", 10);
            effectsPious.Add("fiefLoy", 10);
            effectsPious.Add("time", -10);
            Skill pious = new Skill("Pious", effectsPious);
            skillsCollection.Add(pious.name, pious);

            // add each skillsCollection key to skillsKeys
            foreach (KeyValuePair<string, Skill> entry in skillsCollection)
            {
                skillsKeys.Add(entry.Key);
            }

            // create new random for generating skills for Character
            Random rndSkills = new Random();

            // create array of skills between 2-3 in length
            Skill[] skillsArray1 = new Skill[rndSkills.Next(2, 4)];

            // populate array of skills with randomly chosen skills
            // make temporary copy of skillsKeys
            List<string> skillsKeysCopy = new List<string>(skillsKeys);
            for (int i = 0; i < skillsArray1.Length; i++)
            {
                int randChoice = rndSkills.Next(0, skillsKeysCopy.Count - 1);
                skillsArray1[i] = skillsCollection[skillsKeysCopy[randChoice]];
                skillsKeysCopy.RemoveAt(randChoice);
            }


            // create keep barred lists for fiefs
            List<string> keep1BarChars = new List<string>();
            List<string> keep2BarChars = new List<string>();
            List<string> keep3BarChars = new List<string>();
            List<string> keep4BarChars = new List<string>();
            List<string> keep5BarChars = new List<string>();
            List<string> keep6BarChars = new List<string>();
            List<string> keep7BarChars = new List<string>();

            // create chars lists for fiefs
            List<Character> fief1Chars = new List<Character>();
            List<Character> fief2Chars = new List<Character>();
            List<Character> fief3Chars = new List<Character>();
            List<Character> fief4Chars = new List<Character>();
            List<Character> fief5Chars = new List<Character>();
            List<Character> fief6Chars = new List<Character>();
            List<Character> fief7Chars = new List<Character>();

            // create province for fiefs
            Province myProv = new Province("ESX00", "Sussex, England", 6.2, "E1");
            Province myProv2 = new Province("ESR00", "Surrey, England", 6.2, "E1");

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief1Chars, keep1BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 1000, 1000, 2000, 2000, 10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', 'F', fief2Chars, keep2BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief3Chars, keep3BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief4Chars, keep4BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief5Chars, keep5BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief6Chars, keep6BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', 'P', fief7Chars, keep7BarChars, false, false, this.clock);
            fiefMasterList.Add(myFief7);
            Army myArmy = new Army(0, 0, 0, 0, 100, 0, "101", "401", 90, this.clock);

            // create QuickGraph undirected graph
            // 1. create graph
            var myHexMap = new HexMapGraph();
            this.gameMap = myHexMap;
            // 2. Add edge and auto create vertices
            // from myFief1
            myHexMap.addHexesAndRoute(myFief1, myFief2, "W");
            myHexMap.addHexesAndRoute(myFief1, myFief3, "E");
            myHexMap.addHexesAndRoute(myFief1, myFief6, "NE");
            myHexMap.addHexesAndRoute(myFief1, myFief4, "SE");
            myHexMap.addHexesAndRoute(myFief1, myFief5, "SW");
            myHexMap.addHexesAndRoute(myFief1, myFief7, "NW");
            // from myFief2
            myHexMap.addHexesAndRoute(myFief2, myFief1, "E");
            myHexMap.addHexesAndRoute(myFief2, myFief7, "NE");
            myHexMap.addHexesAndRoute(myFief2, myFief5, "SE");
            // from myFief3
            myHexMap.addHexesAndRoute(myFief3, myFief4, "SW");
            myHexMap.addHexesAndRoute(myFief3, myFief6, "NW");
            myHexMap.addHexesAndRoute(myFief3, myFief1, "W");
            // from myFief4
            myHexMap.addHexesAndRoute(myFief4, myFief3, "NE");
            myHexMap.addHexesAndRoute(myFief4, myFief1, "NW");
            myHexMap.addHexesAndRoute(myFief4, myFief5, "W");
            // from myFief5
            myHexMap.addHexesAndRoute(myFief5, myFief1, "NE");
            myHexMap.addHexesAndRoute(myFief5, myFief2, "NW");
            myHexMap.addHexesAndRoute(myFief5, myFief4, "E");
            // from myFief6
            myHexMap.addHexesAndRoute(myFief6, myFief3, "SE");
            myHexMap.addHexesAndRoute(myFief6, myFief1, "SW");
            myHexMap.addHexesAndRoute(myFief6, myFief7, "W");
            // from myFief7
            myHexMap.addHexesAndRoute(myFief7, myFief6, "E");
            myHexMap.addHexesAndRoute(myFief7, myFief1, "SE");
            myHexMap.addHexesAndRoute(myFief7, myFief2, "SW");

            // create entourages for PCs
            List<Character> myEnt1 = new List<Character>();
            List<Character> myEnt2 = new List<Character>();

            // create lists of fiefs owned by PCs and add some fiefs
            List<Fief> myFiefsOwned1 = new List<Fief>();
            List<Fief> myFiefsOwned2 = new List<Fief>();

            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("101", "Dave Bond", 50, true, "Fr", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, 7.2, 6.1, skillsArray1, false, true, false, this.clock, false, 13000, myEnt1, myFiefsOwned1);
            charMasterList.Add(myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("102", "Bave Dond", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, 5.0, 4.5, skillsArray1, true, false, false, this.clock, false, 13000, myEnt2, myFiefsOwned2);
            charMasterList.Add(myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("401", "Jimmy Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, 3.3, 6.7, skillsArray1, false, false, false, this.clock, "ESX05", 0);
            charMasterList.Add(myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("402", "Johnny Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, 7.1, 5.2, skillsArray1, false, false, false, this.clock, "ESX05", 10000, mb: myChar1);
            charMasterList.Add(myNPC2);
            NonPlayerCharacter myWife = new NonPlayerCharacter("403", "Molly Maguire", 50, false, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, 4.0, 6.0, skillsArray1, false, true, true, this.clock, "ESX05", 0);
            charMasterList.Add(myWife);

            // Add me a wife
            myChar1.spouse = myWife;
            // And my wife a husband
            myWife.spouse = myChar1;
            
            // set fief owners
            myFief1.owner = myChar1;
            myFief2.owner = myChar2;
            myFief3.owner = myChar1;
            myFief4.owner = myChar1;
            myFief5.owner = myChar2;
            myFief6.owner = myChar1;
            myFief7.owner = myChar2;

            // set fief ancestral owners
            myFief1.ancestralOwner = myChar2;
            myFief2.ancestralOwner = myChar1;
            myFief3.ancestralOwner = myChar1;
            myFief4.ancestralOwner = myChar1;
            myFief5.ancestralOwner = myChar2;
            myFief6.ancestralOwner = myChar1;
            myFief7.ancestralOwner = myChar2;

            // set province overlords
            myProv.overlord = myChar1;
            myProv2.overlord = myChar2;

            // set fief bailiffs
            myFief1.bailiff = myNPC1;
            myFief2.bailiff = myNPC2;

            // add NPC to entourage
            myChar1.addToEntourage(myNPC2);

            // Add fiefs to list of fiefs owned 
            myChar1.addToOwnedFiefs(myFief1);
            myChar1.addToOwnedFiefs(myFief3);
            myChar1.addToOwnedFiefs(myFief4);
            myChar1.addToOwnedFiefs(myFief6);
            myChar2.addToOwnedFiefs(myFief2);
            myChar2.addToOwnedFiefs(myFief5);
            myChar2.addToOwnedFiefs(myFief7);

            // add some characters to myFief1
            myFief1.addCharacter(myChar1);
            myFief1.addCharacter(myChar2);
            myFief1.addCharacter(myNPC1);
            myFief1.addCharacter(myNPC2);

            // bar a character from the myFief1 keep
            // myFief1.barCharacter(myNPC2.charID);

            // set inital character to display
            // this.charToView = myChar1;

            // set inital fief to display
            this.fiefToView = myChar1.location;

            return myChar1;

        }

        /// <summary>
        /// Updates game objects at end/start of season
        /// </summary>
        public void seasonUpdate()
        {
            // fiefs
            for (int i = 0; i < fiefMasterList.Count; i++ )
            {
                fiefMasterList[i].updateFief();
            }

            // characters
            for (int i = 0; i < charMasterList.Count; i++)
            {
                charMasterList[i].updateCharacter();
                if ((charMasterList[i].health != 0) && (charMasterList[i] is NonPlayerCharacter))
                {
                    this.seasonUpdateNPC((NonPlayerCharacter)charMasterList[i]);
                }
            }

        }

        /// <summary>
        /// End/start of season updates specific to NPC objects
        /// </summary>
        public void seasonUpdateNPC(NonPlayerCharacter npc)
        {
            if (npc.myBoss == null)
            {
                gameMap.randomNPCmove(npc, npc.location);
            }
        }

        // TODO
        public void setUpFiefsList()
        {
            // set up fiefs list
            this.fiefsListView.Columns.Add("Fief Name", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Where am I?", -2, HorizontalAlignment.Left);

            this.refreshMyFiefs();
        }
        
        // TODO
        public void setUpCourtCharsList()
        {
            // set up court characters list
            this.meetingPlaceCharsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Household", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Sex", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        // TODO
        public void refreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            ListViewItem[] fiefsOwned = new ListViewItem[this.charModel.currentCharacter.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < this.charModel.currentCharacter.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                fiefsOwned[i] = new ListViewItem(this.charModel.currentCharacter.ownedFiefs[i].name);
                fiefsOwned[i].SubItems.Add(this.charModel.currentCharacter.ownedFiefs[i].fiefID);
                if (this.charModel.currentCharacter.ownedFiefs[i] == this.charModel.currentCharacter.location)
                {
                    fiefsOwned[i].SubItems.Add("You are here");
                }
                // add item to fiefsListView
                this.fiefsListView.Items.Add(fiefsOwned[i]);
            }
        }

        // TODO
        public void refreshMeetingPlaceDisplay(string place)
        {
            this.meetingPlaceDisplayText(place);
            this.meetingPlaceDisplayList(place);
        }

        // TODO
        public void meetingPlaceDisplayText(string place)
        {
            string textToDisplay = "";
            textToDisplay += this.clock.seasons[this.clock.currentSeason] + ", " + this.clock.currentYear + ".  Your days left: " + this.charModel.currentCharacter.days + "\r\n\r\n";
            textToDisplay += "Fief: " + this.fiefToView.name + " (" + this.fiefToView.fiefID + ")  in " + this.fiefToView.province.name + "\r\n\r\n";
            textToDisplay += "Owner: " + this.fiefToView.owner.name + "\r\n";
            textToDisplay += "Overlord: " + this.fiefToView.province.overlord.name + "\r\n";

            this.meetingPlaceTextBox.Text = textToDisplay;
        }

        // TODO
        public void meetingPlaceDisplayList(string place)
        {
            // clear existing items in list
            this.meetingPlaceCharsListView.Items.Clear();

            // select which characters to display (court or tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            ListViewItem[] charsInCourt = new ListViewItem[this.fiefToView.characters.Count];
            // iterates through characters
            for (int i = 0; i < this.fiefToView.characters.Count; i++)
            {
                if (this.fiefToView.characters[i].inKeep == ifInKeep)
                {
                    // don't show this PlayerCharacter
                    if (this.fiefToView.characters[i] != this.charModel.currentCharacter)
                    {
                        // Create an item and subitems for each character
                        charsInCourt[i] = new ListViewItem(this.fiefToView.characters[i].name);
                        charsInCourt[i].SubItems.Add(this.fiefToView.characters[i].charID);
                        charsInCourt[i].SubItems.Add("A household");
                        if (this.fiefToView.characters[i].isMale)
                        {
                            charsInCourt[i].SubItems.Add("Male");
                        }
                        else
                        {
                            charsInCourt[i].SubItems.Add("Female");
                        }
                        charsInCourt[i].SubItems.Add("A type");

                        bool isCompanion = false;
                        for (int ii = 0; ii < this.charModel.currentCharacter.entourage.Count; ii++)
                        {
                            if (this.charModel.currentCharacter.entourage[ii] == this.fiefToView.characters[i])
                            {
                                isCompanion = true;
                            }
                        }

                        if (isCompanion)
                        {
                            charsInCourt[i].SubItems.Add("Yes");
                        }

                        // add item to fiefsListView
                        this.meetingPlaceCharsListView.Items.Add(charsInCourt[i]);
                    }
                }
            }
        }

        // TODO
        public string displayCharacter(Character ch)
        {
            string charText = "";

            charText += "ID: " + ch.charID + "\r\n";
            charText += "Name: " + ch.name + "\r\n";
            charText += "Age: " + ch.age + "\r\n";
            charText += "Sex: ";
            if (ch.isMale)
            {
                charText += "Male";
            }
            else
            {
                charText += "Female";
            }
            charText += "\r\n";
            charText += "Nationality: " + ch.nationality + "\r\n";
            charText += "Health: ";
            if (ch.health == 0)
            {
                charText += "You're Dead!";
            }
            else
            {
                charText += ch.health + " (max. health: " + ch.maxHealth + ")";
            }
            charText += "\r\n";
            charText += "  (Death modifier from skills: " + ch.getDeathSkillsMod() + ")\r\n";
            charText += "Virility: " + ch.virility + "\r\n";
            charText += "Current location: " + ch.location.name + " (" + ch.location.province.name + ")\r\n";
            charText += "Language: " + ch.language + "\r\n";
            charText += "Days remaining: " + ch.days + "\r\n";
            charText += "Stature: " + ch.stature + "\r\n";
            charText += "Management: " + ch.management + "\r\n";
            charText += "Combat: " + ch.combat + "\r\n";
            charText += "Skills:\r\n";
            for (int i = 0; i < ch.skills.Length; i++)
            {
                charText += "  - " + ch.skills[i].name + "\r\n";
            }
            charText += "You are ";
            if (ch.inKeep)
            {
                charText += "inside";
            }
            else
            {
                charText += "outside";
            }
            charText += " the keep\r\n";
            charText += "You are ";
            if (ch.married)
            {
                charText += "happily married";
            }
            else
            {
                charText += "single and lonely";
            }
            charText += "\r\n";
            if (ch.married)
            {
                charText += "Your spouse's ID is: " + ch.spouse.charID + "\r\n";
            }
            if (!ch.isMale)
            {
                charText += "You are ";
                if (!ch.pregnant)
                {
                    charText += "not ";
                }
                charText += "pregnant\r\n";
            }
            else
            {
                if (ch.married)
                {
                    if (ch.spouse.pregnant)
                    {
                        charText += "Your wife is pregnant (congratulations!)\r\n";
                    }
                    else
                    {
                        charText += "Your wife is not pregnant (try harder!)\r\n";
                    }
                }
            }
            charText += "Father's ID: ";
            if (ch.father != null)
            {
                charText += ch.father.charID;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";
            charText += "Head of family's ID: ";
            if (ch.familyHead != null)
            {
                charText += ch.familyHead.charID;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            this.characterTextBox.Text = charText;

            bool isPC = ch is PlayerCharacter;
            if (isPC)
            {
                charText += this.displayPlayerCharacter((PlayerCharacter)ch);
            }
            else
            {
                charText += this.displayNonPlayerCharacter((NonPlayerCharacter)ch);
            }

            return charText;
        }

        // TODO
        public string displayPlayerCharacter(PlayerCharacter ch)
        {
            string pcText = "";

            pcText += "You are ";
            if (!ch.outlawed)
            {
                pcText += "not ";
            }
            pcText += "outlawed\r\n";
            pcText += "Purse: " + ch.purse + "\r\n";
            pcText += "Entourage:";
            for (int i = 0; i < ch.entourage.Count; i++)
            {
                pcText += " " + ch.entourage[i].name;
                if (i < (ch.entourage.Count - 1))
                {
                    pcText += ",";
                }
                else
                {
                    pcText += "\r\n";
                }
            }
            pcText += "Fiefs owned:\r\n";
            for (int i = 0; i < ch.ownedFiefs.Count; i++)
            {
                pcText += "  - " + ch.ownedFiefs[i].name + "\r\n";
            }

            return pcText;
        }

        // TODO
        public string displayNonPlayerCharacter(NonPlayerCharacter ch)
        {
            string npcText = "";

            if (ch.myBoss != null)
            {
                npcText += "Hired by (ID): " + ch.myBoss.charID + "\r\n";
            }
            npcText += "Go to (Fief ID): " + ch.goTo + "\r\n";
            npcText += "Potential salary: " + ch.calcNPCwage(ch) + "\r\n";
            npcText += "Last offer from this PC: ";
            if (ch.lastOffer.ContainsKey(this.charModel.currentCharacter.charID))
            {
                npcText += ch.lastOffer[this.charModel.currentCharacter.charID];
            }
            else
            {
                npcText += "N/A";
            }
            npcText += "\r\n";
            npcText += "Current salary: " + ch.wage + "\r\n";

            return npcText;
        }

        // TODO
        public void displayFief(Fief f)
        {
            string fiefText = "";

            fiefText += "ID: " + f.fiefID + "\r\n";
            fiefText += "Name: " + f.name + " (Province: " + f.province.name + ")\r\n";
            fiefText += "Population: " + f.population + "\r\n";
            fiefText += "Owner (ID): " + f.owner.charID + "\r\n";
            fiefText += "Ancestral owner (ID): " + f.ancestralOwner.charID + "\r\n";
            fiefText += "Bailiff (ID): ";
            if (f.bailiff != null)
            {
                fiefText += f.bailiff.charID;
            }
            else
            {
                fiefText += "auto-bailiff";
            }
            fiefText += "\r\n";

            fiefText += "Troops: " + f.troops + "\r\n";
            fiefText += "Status: ";
            switch (f.status)
            {
                case 'U':
                    fiefText += "Unrest";
                    break;
                case 'R':
                    fiefText += "Rebellion!";
                    break;
                default:
                    fiefText += "Calm";
                     break;
            }
            fiefText += "\r\n";

            fiefText += "Terrain: ";
            switch (f.terrain)
            {
                case 'P':
                    fiefText += "Plains";
                    break;
                case 'H':
                    fiefText += "Hills";
                    break;
                case 'F':
                    fiefText += "Forrest";
                    break;
                case 'M':
                    fiefText += "Mountains";
                    break;
                default:
                    fiefText += "Plains";
                    break;
            }
            fiefText += "\r\n";

            fiefText += "Characters present:";
            for (int i = 0; i < f.characters.Count; i++)
            {
                fiefText += " " + f.characters[i].name;
                if (i < (f.characters.Count - 1))
                {
                    fiefText += ",";
                }
                else
                {
                    fiefText += "\r\n";
                }
            }
            fiefText += "Characters barred from keep (IDs):";
            for (int i = 0; i < f.barredCharacters.Count; i++)
            {
                fiefText += " " + f.barredCharacters[i];
                if (i < (f.barredCharacters.Count - 1))
                {
                    fiefText += ",";
                }
                else
                {
                    fiefText += "\r\n";
                }
            }
            fiefText += "The French are ";
            if (!f.frenchBarred)
            {
                fiefText += "not";
            }
            fiefText += " barred from the keep\r\n";
            fiefText += "The English are ";
            if (!f.englishBarred)
            {
                fiefText += "not";
            }
            fiefText += " barred from the keep\r\n\r\n";

            fiefText += "========= Management ==========\r\n\r\n";

            fiefText += "Loyalty: " + (f.loyalty + (f.loyalty * f.calcBlfLoyAdjusted())) + "\r\n";
            fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod("this") + ")\r\n";
            fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod("this") + ")\r\n";
            fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted() + ")\r\n";
            fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod() + ")\r\n";
            fiefText += "Fields level: " + f.fields + "\r\n";
            fiefText += "Industry level: " + f.industry + "\r\n";
            fiefText += "GDP: " + f.calcGDP("this") + "\r\n";
            fiefText += "Tax rate: " + f.taxRate + "\r\n";
            fiefText += "Officials expenditure: " + f.officialsSpend + " (modifier: " + f.calcOffIncMod("this") + ")\r\n";
            fiefText += "Garrison expenditure: " + f.garrisonSpend + "\r\n";
            fiefText += "Infrastructure expenditure: " + f.infrastructureSpend + "\r\n";
            fiefText += "Keep expenditure: " + f.keepSpend + "\r\n";
            fiefText += "Keep level: " + f.keepLevel + "\r\n";
            fiefText += "Income: " + (f.calcIncome("this") * f.calcStatusIncmMod()) + "\r\n";
            fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod() + ")\r\n";
            fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod("this") + ")\r\n";
            fiefText += "  (including fief status income modifier: " + f.calcStatusIncmMod() + ")\r\n";
            fiefText += "Family expenses: 0 (not yet implemented)\r\n";
            fiefText += "Total expenses: " + f.calcExpenses("this") + "\r\n";
            fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif() + ")\r\n";
            fiefText += "Overlord taxes: " + f.calcOlordTaxes("this") + "\r\n";
            fiefText += "Bottom line: " + f.calcBottomLine("this") + "\r\n\r\n";

            fiefText += "========= Next season =========\r\n";
            fiefText += "(with current bailiff & oLord tax)\r\n";
            fiefText += " (NOT including effects of status)\r\n\r\n";

            fiefText += "Loyalty: " + (f.calcNewLoyalty() + (f.calcNewLoyalty() * f.calcBlfLoyAdjusted())) + "\r\n";
            fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod("next") + ")\r\n";
            fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod("next") + ")\r\n";
            fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted() + ")\r\n";
            fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod() + ")\r\n";
            fiefText += "Fields level: " + f.calcNewFieldLevel() + "\r\n";
            fiefText += "Industry level: " + f.calcNewIndustryLevel() + "\r\n";
            fiefText += "GDP: " + f.calcGDP("next") + "\r\n";
            fiefText += "Tax rate: " + f.taxRateNext + "\r\n";
            fiefText += "Officials expenditure: " + f.officialsSpendNext + "\r\n";
            fiefText += "Garrison expenditure: " + f.garrisonSpendNext + "\r\n";
            fiefText += "Infrastructure expenditure: " + f.infrastructureSpendNext + "\r\n";
            fiefText += "Keep expenditure: " + f.keepSpendNext + "\r\n";
            fiefText += "Keep level: " + f.calcNewKeepLevel() + "\r\n";
            fiefText += "Income: " + f.calcIncome("next") + "\r\n";
            fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod() + ")\r\n";
            fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod("next") + ")\r\n";
            fiefText += "Family expenses: 0 (not yet implemented)\r\n";
            fiefText += "Total expenses: " + f.calcExpenses("next") + "\r\n";
            fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif() + ")\r\n";
            fiefText += "Overlord taxes: " + f.calcOlordTaxes("next") + "\r\n";
            fiefText += "Bottom line: " + f.calcBottomLine("next") + "\r\n\r\n";

            this.fiefTextBox.Text = fiefText;
        }

        /// <summary>
        /// Updates fief and character models and GameClock
        /// </summary>
        /// <param name="info">String containing data about display element to update</param>
        public void nextTurn()
        {
            this.fModel.updateFief();
            this.charModel.updateCharacter();
            this.clock.advanceSeason();
        }

        /// <summary>
        /// Updates appropriate display elements when data received from model
        /// </summary>
        /// <param name="info">String containing data about display element to update</param>
        public void update(String info)
        {
            switch (info)
            {
                case "refreshChar":
                    string textToDisplay = "";
                    textToDisplay += this.displayCharacter(this.charModel.currentCharacter);
                    this.characterTextBox.Text = textToDisplay;                    
                    break;
                case "refreshFief":
                    this.displayFief(this.fModel.currentFief);
                    break;
                default:
                    break;
            }
        }

        private void personalCharacteristicsAndAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* if (this.fiefContainer.Visible)
            {
                this.fiefContainer.Visible = false;
            }

            if (this.travelContainer.Visible)
            {
                this.travelContainer.Visible = false;
            }
            
            if (!this.characterContainer.Visible)
            {
                this.displayCharacter(this.charModel.currentCharacter);
                this.characterContainer.Visible = true;
            } */

            string textToDisplay = "";
            textToDisplay += this.displayCharacter(this.charModel.currentCharacter);
            this.characterTextBox.Text = textToDisplay;            
            this.characterContainer.BringToFront();
        }

        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*
            if (this.characterContainer.Visible)
            {
                this.characterContainer.Visible = false;
            }

            if (this.travelContainer.Visible)
            {
                this.travelContainer.Visible = false;
            }

            if (!this.fiefContainer.Visible)
            {
                this.fiefContainer.Visible = true;
            } */

            this.displayFief(this.charModel.currentCharacter.location);
            this.fiefContainer.BringToFront();
        }

        private void updateCharacter_Click(object sender, EventArgs e)
        {
            this.charModel.updateCharacter();
        }

        private void calcPop_Click(object sender, EventArgs e)
        {
            this.fModel.calcNewPop();
        }

        private void adjustTaxButton_Click(object sender, EventArgs e)
        {
            this.fModel.adjustTx(Convert.ToDouble(this.adjustTaxTextBox.Text));
        }

        private void adjOffSpend_Click(object sender, EventArgs e)
        {
            this.fModel.adjustOffSpend(Convert.ToUInt32(this.adjOffSpendTextBox.Text));
        }

        private void adjGarrSpendBtn_Click(object sender, EventArgs e)
        {
            this.fModel.adjustGarrSpend(Convert.ToUInt32(this.adjGarrSpendTextBox.Text));
        }

        private void adjInfrSpendBtn_Click(object sender, EventArgs e)
        {
            this.fModel.adjustInfSpend(Convert.ToUInt32(this.adjInfrSpendTextBox.Text));
        }

        private void adjustKeepSpendBtn_Click(object sender, EventArgs e)
        {
            this.fModel.adjustKpSpend(Convert.ToUInt32(this.adjustKeepSpendTextBox.Text));
        }

        private void updateFiefBtn_Click(object sender, EventArgs e)
        {
            this.fModel.updateFief();
        }

        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (this.fiefContainer.Visible)
            {
                this.fiefContainer.Visible = false;
            }

            if (!this.characterContainer.Visible)
            {
                this.characterContainer.Visible = false;
            }

            if (!this.travelContainer.Visible)
            {
                this.refreshTravelContainer();
                this.travelContainer.Visible = true;
            } */

            this.refreshTravelContainer();
            this.travelContainer.BringToFront();
        }

        /// <summary>
        /// Gtes travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="f">Target fief</param>
        private double getTravelCost(Fief f)
        {
            double cost = 0;
            cost = (1 + f.calcTerrainTravMod()) * this.clock.calcSeasonTravMod();
            return cost;
        }

        private void refreshTravelContainer()
        {
            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + this.charModel.currentCharacter.location.name + "\r\n(" + this.charModel.currentCharacter.location.province.name + ")";

            // get text for directional buttons
            // NE
            Fief targetNE = this.gameMap.getFief(this.charModel.currentCharacter.location, "NE");
            if (targetNE != null)
            {
                this.travel_NE_btn.Text = "NE FIEF:\r\n\r\n";
                this.travel_NE_btn.Text += targetNE.name + " (" + targetNE.fiefID + ")\r\n";
                this.travel_NE_btn.Text += "(" + targetNE.province.name + ")\r\n\r\n";
                this.travel_NE_btn.Text += "Cost: " + this.getTravelCost(targetNE);
            }
            else
            {
                this.travel_NE_btn.Text = "NE FIEF:\r\n\r\nNo fief present";
            }

            // E
            Fief targetE = this.gameMap.getFief(this.charModel.currentCharacter.location, "E");
            if (targetE != null)
            {
                this.travel_E_btn.Text = "E FIEF:\r\n\r\n";
                this.travel_E_btn.Text += targetE.name + " (" + targetE.fiefID + ")\r\n";
                this.travel_E_btn.Text += "(" + targetE.province.name + ")\r\n\r\n";
                this.travel_E_btn.Text += "Cost: " + this.getTravelCost(targetE);
            }
            else
            {
                this.travel_E_btn.Text = "E FIEF:\r\n\r\nNo fief present";
            }

            // SE
            Fief targetSE = this.gameMap.getFief(this.charModel.currentCharacter.location, "SE");
            if (targetSE != null)
            {
                this.travel_SE_btn.Text = "SE FIEF:\r\n\r\n";
                this.travel_SE_btn.Text += targetSE.name + " (" + targetSE.fiefID + ")\r\n";
                this.travel_SE_btn.Text += "(" + targetSE.province.name + ")\r\n\r\n";
                this.travel_SE_btn.Text += "Cost: " + this.getTravelCost(targetSE);
            }
            else
            {
                this.travel_SE_btn.Text = "SE FIEF:\r\n\r\nNo fief present";
            }

            // SW
            Fief targetSW = this.gameMap.getFief(this.charModel.currentCharacter.location, "SW");
            if (targetSW != null)
            {
                this.travel_SW_btn.Text = "SW FIEF:\r\n\r\n";
                this.travel_SW_btn.Text += targetSW.name + " (" + targetSW.fiefID + ")\r\n";
                this.travel_SW_btn.Text += "(" + targetSW.province.name + ")\r\n\r\n";
                this.travel_SW_btn.Text += "Cost: " + this.getTravelCost(targetSW);
            }
            else
            {
                this.travel_SW_btn.Text = "SW FIEF:\r\n\r\nNo fief present";
            }

            // W
            Fief targetW = this.gameMap.getFief(this.charModel.currentCharacter.location, "W");
            if (targetW != null)
            {
                this.travel_W_btn.Text = "W FIEF:\r\n\r\n";
                this.travel_W_btn.Text += targetW.name + " (" + targetW.fiefID + ")\r\n";
                this.travel_W_btn.Text += "(" + targetW.province.name + ")\r\n\r\n";
                this.travel_W_btn.Text += "Cost: " + this.getTravelCost(targetW);
            }
            else
            {
                this.travel_W_btn.Text = "W FIEF:\r\n\r\nNo fief present";
            }

            // NW
            Fief targetNW = this.gameMap.getFief(this.charModel.currentCharacter.location, "NW");
            if (targetNW != null)
            {
                this.travel_NW_btn.Text = "NW FIEF:\r\n\r\n";
                this.travel_NW_btn.Text += targetNW.name + " (" + targetNW.fiefID + ")\r\n";
                this.travel_NW_btn.Text += "(" + targetNW.province.name + ")\r\n\r\n";
                this.travel_NW_btn.Text += "Cost: " + this.getTravelCost(targetNW);
            }
            else
            {
                this.travel_NW_btn.Text = "NW FIEF:\r\n\r\nNo fief present";
            }

            // set text for 'enter/exit keep' button
            if (this.charModel.currentCharacter.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
            }

        }

        private void travel_W_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "W");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        private void myFiefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshMyFiefs();
            this.fiefsOwnedContainer.BringToFront();
        }

        private void travel_E_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "E");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        private void viewBailiffBtn_Click(object sender, EventArgs e)
        {
            if (this.fModel.currentFief.bailiff != null)
            {
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(this.fModel.currentFief.bailiff);
                this.characterTextBox.Text = textToDisplay;
                this.characterContainer.BringToFront();
            }
        }

        private void travel_NW_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "NW");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        private void travel_NE_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "NE");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        private void travel_SW_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "SW");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        private void travel_SE_btn_Click(object sender, EventArgs e)
        {
            Fief newFief = this.gameMap.moveCharacter(this.charModel.currentCharacter, this.charModel.currentCharacter.location, "SE");
            if (newFief != null)
            {
                this.fModel.currentFief = newFief;
                this.refreshTravelContainer();
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the fiefsListView ListView object,
        /// invoking the displayFief method, passing a Fief to indicate the fief to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Fief fiefToDisplay = null;

            for (int i = 0; i < this.charModel.currentCharacter.ownedFiefs.Count; i++ )
            {
                if (this.charModel.currentCharacter.ownedFiefs[i].fiefID.Equals(this.fiefsListView.SelectedItems[0].SubItems[1].Text))
                {
                    fiefToDisplay = this.charModel.currentCharacter.ownedFiefs[i];
                }

            }

            if (fiefToDisplay != null)
            {
                this.fiefToView = fiefToDisplay;
                this.displayFief(fiefToView);
                this.fiefContainer.BringToFront();
            }
        }

        private void visitTavernBtn_Click(object sender, EventArgs e)
        {

        }

        private void visitCourtBtn_Click(object sender, EventArgs e)
        {
            this.refreshMeetingPlaceDisplay("court");
            this.meetingPlaceContainer.BringToFront();
        }

        private void enterKeepBtn_Click(object sender, EventArgs e)
        {
            if (this.charModel.currentCharacter.inKeep)
            {
                this.charModel.currentCharacter.exitKeep();
                this.enterKeepBtn.Text = "Enter Keep";
                this.refreshTravelContainer();
            }
            else
            {
                this.charModel.currentCharacter.enterKeep();
                this.enterKeepBtn.Text = "Exit Keep";
                this.refreshTravelContainer();
            }
        }

        private void visitCourtBtn1_Click(object sender, EventArgs e)
        {
            if (! this.charModel.currentCharacter.inKeep)
            {
                this.charModel.currentCharacter.enterKeep();
            }
            this.refreshMeetingPlaceDisplay("court");
            this.meetingPlaceContainer.BringToFront();
        }

        private void visitTavernBtn_Click_1(object sender, EventArgs e)
        {
            if (this.charModel.currentCharacter.inKeep)
            {
                this.charModel.currentCharacter.exitKeep();
            }
            this.refreshMeetingPlaceDisplay("tavern");
            this.meetingPlaceContainer.BringToFront();
        }

        private void meetingPlaceCharsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            for (int i = 0; i < this.fiefToView.characters.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    if (this.fiefToView.characters[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = this.fiefToView.characters[i];
                    }
                }

            }

            if (charToDisplay != null)
            {
                this.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(charToDisplay);
                this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
            }
        }

        private void hireNPC_Btn_Click(object sender, EventArgs e)
        {
            bool offerAccepted = this.charModel.currentCharacter.processEmployOffer((NonPlayerCharacter)charToView, Convert.ToUInt32(this.hireNPC_TextBox.Text));
            string textToDisplay = "";
            textToDisplay += this.displayCharacter(charToView);
            this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
        }

    }
}
