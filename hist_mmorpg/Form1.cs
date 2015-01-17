using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using CorrugatedIron;
using CorrugatedIron.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Main user interface component
    /// </summary>
    public partial class Form1 : Form, Game_Observer
    {
		/// <summary>
		/// Holds target RiakCluster 
		/// </summary>
		RiakCluster rCluster;
		/// <summary>
        /// Holds RiakClient to communicate with RiakCluster
		/// </summary>
		RiakClient rClient;

        /// <summary>
        /// Constructor for Form1
        /// </summary>
        public Form1()
        {
            // register as observer
            Globals_Game.registerObserver(this);

            // initialise form elements
            InitializeComponent();

			// initialise Riak elements
			rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
			rClient = (RiakClient)rCluster.CreateClient();

            // initialise game objects
            this.initGameObjects("Char_158", gameID: "fromCSV", objectDataFile: "gameObjects.csv", mapDataFile: "map.csv",
            start: 1194, king1: "Char_47", king2: "Char_40", herald1: "Char_1", sysAdmin: "Char_196");

			// this.ImportFromCSV("gameObjects.csv", bucketID: "fromCSV", synch: true, toDatabase: true);
			// this.CreateMapArrayFromCSV ("map.csv", bucketID: "fromCSV", toDatabase: true);
        }

        // ------------------- GAME START/INITIALISATION

		/// <summary>
        /// Initialises all game objects
		/// </summary>
		/// <param name="pcID">ID of PlayerCharacter to set as Globals_Client.myPlayerCharacter</param>
		/// <param name="gameID">gameID of the game</param>
        /// <param name="objectDataFile">Name of file containing game object CSV data</param>
        /// <param name="mapDataFile">Name of file containing map CSV data</param>
        /// <param name="type">Game type</param>
        /// <param name="duration">Game duration (years)</param>
        /// <param name="start">Start year</param>
        /// <param name="king1">ID of PlayerCharacter in role of kingOne</param>
        /// <param name="king2">ID of PlayerCharacter in role of kingTwo</param>
        /// <param name="herald1">ID of PlayerCharacter in role of heraldOne</param>
        /// <param name="herald2">ID of PlayerCharacter in role of heraldTwo</param>
        /// <param name="sysAdmin">ID of PlayerCharacter in role of sysAdmin</param>
        public void initGameObjects(string pcID, string gameID = null, string objectDataFile = null,
            string mapDataFile = null, uint type = 0, uint duration = 100, uint start = 1337, string king1 = null,
            string king2 = null, string herald1 = null, string herald2 = null, string sysAdmin = null)
        {
            bool dataLoaded = false;

            // LOAD DATA
            // database
            if (Globals_Game.loadFromDatabase)
			{
				// load objects
				this.databaseRead (gameID);

                dataLoaded = true;
			}
            // CSV file
            else if (Globals_Game.loadFromCSV)
            {
                // load objects (mainly) from CSV file
                if (!String.IsNullOrWhiteSpace(objectDataFile))
                {
                    // load objects
                    this.newGameFromCSV(objectDataFile, mapDataFile, start);

                    // initialise Globals_Game.victoryData
                    this.synchroniseVictoryData();

                    dataLoaded = true;
                }
            }
            // from code (for quick testing)
            if (!dataLoaded)
			{
                // load objects
				this.loadFromCode ();

                // initialise Globals_Game.victoryData
                this.synchroniseVictoryData();
            }

            if ((!Globals_Game.loadFromDatabase) || (!dataLoaded))
            {
                // INITIALISE ROLES
                // set kings
                if (!String.IsNullOrWhiteSpace(king1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king1))
                    {
                        Globals_Game.kingOne = Globals_Game.pcMasterList[king1];
                    }
                }
                if (!String.IsNullOrWhiteSpace(king2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(king2))
                    {
                        Globals_Game.kingTwo = Globals_Game.pcMasterList[king2];
                    }
                }

                // set heralds
                if (!String.IsNullOrWhiteSpace(herald1))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald1))
                    {
                        Globals_Game.heraldOne = Globals_Game.pcMasterList[herald1];
                    }
                }
                if (!String.IsNullOrWhiteSpace(herald2))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(herald2))
                    {
                        Globals_Game.heraldTwo = Globals_Game.pcMasterList[herald2];
                    }
                }

                // set sysAdmin
                if (!String.IsNullOrWhiteSpace(sysAdmin))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(sysAdmin))
                    {
                        Globals_Game.sysAdmin = Globals_Game.pcMasterList[sysAdmin];
                    }
                }

                // SET GAME PARAMETERS
                // game type
                if (type != 0)
                {
                    Globals_Game.gameType = type;
                }

                // start date
                if (start != 1337)
                {
                    Globals_Game.startYear = start;
                }

                // duration
                if (duration != 100)
                {
                    Globals_Game.duration = duration;
                }

            }

            // INITIALISE GLOBALS_CLIENT VARIABLES
            // set myPlayerCharacter
            Globals_Client.myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            // set inital fief to display
            Globals_Client.fiefToView = Globals_Client.myPlayerCharacter.location;

            // set player's character to display
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;

            // INITIALISE UI ELEMENTS
            this.setUpFiefsList();
            this.setUpMeetingPLaceCharsList();
            this.setUpHouseholdCharsList();
            this.setUpArmyList();
            this.setUpSiegeList();
            this.setUpJournalList();
            this.setUpRoyalGiftsLists();
            this.setUpProvinceLists();
            this.setUpEditSkillEffectList();

            // check if royal & overlord menus should be displayed
            this.initMenuPermissions();

            // =================== TESTING
            
            // create and add army
            uint[] myArmyTroops1 = new uint[] { 5, 10, 0, 30, 40, 4000, 6020 };
            Army myArmy1 = new Army(Globals_Game.getNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
            myArmy1.addArmy(); 
            
            // create and add army
            uint[] myArmyTroops2 = new uint[] { 5, 10, 0, 30, 40, 80, 220 };
            Army myArmy2 = new Army(Globals_Game.getNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
            myArmy2.addArmy();
            /*
            // create ailment
            Ailment myAilment = new Ailment(Globals_Game.getNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, 1, 0);
            Globals_Game.pcMasterList["Char_196"].ailments.Add(myAilment.ailmentID, myAilment); */
            // =================== END TESTING

            // INITIALISE UI DISPLAY
            this.refreshCharacterContainer();
        }

        /// <summary>
        /// Initialises permissions to view client menus (Royal Gifts, Overlord, Admin)
        /// </summary>
        public void initMenuPermissions()
        {
            // check if royal & overlord menus should be displayed
            if (((Globals_Client.myPlayerCharacter.checkIsKing())
                || (Globals_Client.myPlayerCharacter.checkIfOverlord()))
                || (Globals_Client.myPlayerCharacter.checkIsHerald()))
            {
                this.royalFunctionsToolStripMenuItem.Enabled = true;

                // check if royal gifts menu items should be enabled
                if ((Globals_Client.myPlayerCharacter.checkIsKing())
                || (Globals_Client.myPlayerCharacter.checkIsHerald()))
                {
                    this.royalGiftsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.royalGiftsToolStripMenuItem.Enabled = false;
                }

                // check if manage provinces menu items should be enabled
                if (Globals_Client.myPlayerCharacter.checkIfOverlord())
                {
                    this.manageProvincesToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.manageProvincesToolStripMenuItem.Enabled = false;
                }
            }

            // if not king/overlord, disable royal & overlord menus
            else
            {
                this.royalFunctionsToolStripMenuItem.Enabled = false;
            }

            // enable sysAdmin menu, if appropriate
            if (Globals_Client.myPlayerCharacter.checkIsSysAdmin())
            {
                this.adminToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.adminToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Ensures that the Globals_Game.victoryData is up-to-date
        /// </summary>
        public void synchroniseVictoryData()
        {
            List<string> toRemove = new List<string>();
            List<VictoryData> toAdd = new List<VictoryData>();

            // iterate through Globals_Game.victoryData
            foreach (KeyValuePair<string, VictoryData> vicDataEntry in Globals_Game.victoryData)
            {
                // check that player still active
                PlayerCharacter thisPC = null;
                if (Globals_Game.pcMasterList.ContainsKey(vicDataEntry.Value.playerCharacterID))
                {
                    thisPC = Globals_Game.pcMasterList[vicDataEntry.Value.playerCharacterID];
                }

                // if PC exists
                if (thisPC != null)
                {
                    // check is active player
                    if ((String.IsNullOrWhiteSpace(thisPC.playerID)) || (!thisPC.isAlive))
                    {
                        toRemove.Add(vicDataEntry.Key);
                    }
                }

                // if PC doesn't exist
                else
                {
                    toRemove.Add(vicDataEntry.Key);
                }
            }

            // remove Globals_Game.victoryData entries if necessary
            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++ )
                {
                    Globals_Game.victoryData.Remove(toRemove[i]);
                }
            }
            toRemove.Clear();

            // iterate through pcMasterList
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Game.pcMasterList)
            {
                // check for playerID (i.e. active player)
                if (!String.IsNullOrWhiteSpace(pcEntry.Value.playerID))
                {
                    // check if is in Globals_Game.victoryData
                    if (!Globals_Game.victoryData.ContainsKey(pcEntry.Value.playerID))
                    {
                        // create and add new VictoryData if necessary
                        VictoryData newVicData = new VictoryData(pcEntry.Value.playerID, pcEntry.Value.charID, pcEntry.Value.calculateStature(), pcEntry.Value.getPopulationPercentage(), pcEntry.Value.getFiefsPercentage(), pcEntry.Value.getMoneyPercentage());
                        toAdd.Add(newVicData);
                    }
                }
            }

            // add any new Globals_Game.victoryData entries
            if (toAdd.Count > 0)
            {
                for (int i = 0; i < toAdd.Count; i++)
                {
                    Globals_Game.victoryData.Add(toAdd[i].playerID, toAdd[i]);
                }
            }
            toAdd.Clear();

        }

        /// <summary>
        /// Creates UI display for list of skill effects in the edit skill screen
        /// </summary>
        public void setUpEditSkillEffectList()
        {
            // add necessary columns
            this.adminEditSkillEffsListView.Columns.Add("Effect Name", -2, HorizontalAlignment.Left);
            this.adminEditSkillEffsListView.Columns.Add("Level", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for king's lists of provinces and fiefs
        /// </summary>
        public void setUpRoyalGiftsLists()
        {
            // add necessary columns
            // provinces
            this.royalGiftsProvListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Title Holder", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Last Tax Rate", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Last Tax Income", -2, HorizontalAlignment.Left);
            // fiefs
            this.royalGiftsFiefListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Province", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Title Holder", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Last GDP", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Last Tax Income", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Current Treasury", -2, HorizontalAlignment.Left);
            // positions
            this.royalGiftsPositionListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Position", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Stature", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Holder", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for overlord's lists of provinces (and associated fiefs)
        /// </summary>
        public void setUpProvinceLists()
        {
            // add necessary columns
            // provinces
            this.provinceProvListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Last season tax rate", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Kingdom ID", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Kingdom Name", -2, HorizontalAlignment.Left);
            // fiefs
            this.provinceFiefListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Current GDP", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Last season tax income", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for list of journal entries
        /// </summary>
        public void setUpJournalList()
        {
            // add necessary columns
            this.journalListView.Columns.Add("Entry ID", -2, HorizontalAlignment.Left);
            this.journalListView.Columns.Add("Date", -2, HorizontalAlignment.Left);
            this.journalListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for list of sieges owned by player
        /// </summary>
        public void setUpSiegeList()
        {
            // add necessary columns
            this.siegeListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.siegeListView.Columns.Add("Fief", -2, HorizontalAlignment.Left);
            this.siegeListView.Columns.Add("Defender", -2, HorizontalAlignment.Left);
            this.siegeListView.Columns.Add("Besieger", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for list of armies owned by player
        /// </summary>
        public void setUpArmyList()
        {
            // add necessary columns
            this.armyListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.armyListView.Columns.Add("Leader", -2, HorizontalAlignment.Left);
            this.armyListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
            this.armyListView.Columns.Add("Size", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for list of characters in the Household screen
        /// </summary>
        public void setUpHouseholdCharsList()
        {
            // add necessary columns
            this.houseCharListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Function", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Responsibilities", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for list of characters present in Court, Tavern, outside keep
        /// </summary>
        public void setUpMeetingPLaceCharsList()
        {
            // add necessary columns
            this.meetingPlaceCharsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Sex", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Household", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void setUpFiefsList()
        {
            // add necessary columns
            this.fiefsListView.Columns.Add("Fief Name", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Where am I?", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Home Fief?", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Province Name", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
        }

        /// <summary>
		/// Creates some game objects from code (temporary)
		/// </summary>
        /// <param name="start">Start year</param>
        public void loadFromCode(uint start = 1337)
		{
            // create GameClock
            Globals_Game.clock = new GameClock("clock_1", start);

			// create skills
			// Dictionary of skill effects
            Dictionary<string, double> effectsCommand = new Dictionary<string, double>();
			effectsCommand.Add("battle", 0.4);
            effectsCommand.Add("siege", 0.4);
			effectsCommand.Add("npcHire", 0.2);
			// create skill
			Skill command = new Skill("skill_1", "Command", effectsCommand);
			// add to skillsCollection
			Globals_Game.skillMasterList.Add(command.skillID, command);

            Dictionary<string, double> effectsChivalry = new Dictionary<string, double>();
            effectsChivalry.Add("famExpense", 0.2);
			effectsChivalry.Add("fiefExpense", 0.1);
            effectsChivalry.Add("fiefLoy", 0.2);
            effectsChivalry.Add("npcHire", 0.1);
            effectsChivalry.Add("siege", 0.1);
            Skill chivalry = new Skill("skill_2", "Chivalry", effectsChivalry);
            Globals_Game.skillMasterList.Add(chivalry.skillID, chivalry);

            Dictionary<string, double> effectsAbrasiveness = new Dictionary<string, double>();
			effectsAbrasiveness.Add("battle", 0.15);
			effectsAbrasiveness.Add("death", 0.05);
            effectsAbrasiveness.Add("fiefExpense", -0.05);
            effectsAbrasiveness.Add("famExpense", 0.05);
            effectsAbrasiveness.Add("time", 0.05);
            effectsAbrasiveness.Add("siege", -0.1);
            Skill abrasiveness = new Skill("skill_3", "Abrasiveness", effectsAbrasiveness);
            Globals_Game.skillMasterList.Add(abrasiveness.skillID, abrasiveness);

            Dictionary<string, double> effectsAccountancy = new Dictionary<string, double>();
            effectsAccountancy.Add("time", 0.1);
            effectsAccountancy.Add("fiefExpense", -0.2);
            effectsAccountancy.Add("famExpense", -0.2);
            effectsAccountancy.Add("fiefLoy", -0.05);
            Skill accountancy = new Skill("skill_4", "Accountancy", effectsAccountancy);
            Globals_Game.skillMasterList.Add(accountancy.skillID, accountancy);

            Dictionary<string, double> effectsStupidity = new Dictionary<string, double>();
            effectsStupidity.Add("battle", -0.4);
            effectsStupidity.Add("death", 0.05);
            effectsStupidity.Add("fiefExpense", 0.2);
            effectsStupidity.Add("famExpense", 0.2);
            effectsStupidity.Add("fiefLoy", -0.1);
            effectsStupidity.Add("npcHire", -0.1);
            effectsStupidity.Add("time", -0.1);
            effectsStupidity.Add("siege", -0.4);
            Skill stupidity = new Skill("skill_5", "Stupidity", effectsStupidity);
            Globals_Game.skillMasterList.Add(stupidity.skillID, stupidity);

            Dictionary<string, double> effectsRobust = new Dictionary<string, double>();
            effectsRobust.Add("virility", 0.2);
            effectsRobust.Add("npcHire", 0.05);
            effectsRobust.Add("fiefLoy", 0.05);
            effectsRobust.Add("death", -0.2);
            Skill robust = new Skill("skill_6", "Robust", effectsRobust);
            Globals_Game.skillMasterList.Add(robust.skillID, robust);

            Dictionary<string, double> effectsPious = new Dictionary<string, double>();
            effectsPious.Add("virility", -0.2);
            effectsPious.Add("npcHire", 0.1);
            effectsPious.Add("fiefLoy", 0.1);
            effectsPious.Add("time", -0.1);
            Skill pious = new Skill("skill_7", "Pious", effectsPious);
            Globals_Game.skillMasterList.Add(pious.skillID, pious);

			// add each skillsCollection key to skillsKeys
            foreach (KeyValuePair<string, Skill> entry in Globals_Game.skillMasterList)
			{
                Globals_Game.skillKeys.Add(entry.Key);
			}

            // create BaseLanguage & Language objects
            BaseLanguage c = new BaseLanguage("lang_C", "Celtic");
            Globals_Game.baseLanguageMasterList.Add(c.id, c);
            Language c1 = new Language(c, 1);
            Globals_Game.languageMasterList.Add(c1.id, c1);
            Language c2 = new Language(c, 2);
            Globals_Game.languageMasterList.Add(c2.id, c2);
            BaseLanguage f = new BaseLanguage("lang_F", "French");
            Globals_Game.baseLanguageMasterList.Add(f.id, f);
            Language f1 = new Language(f, 1);
            Globals_Game.languageMasterList.Add(f1.id, f1);
            BaseLanguage e = new BaseLanguage("lang_E", "English");
            Globals_Game.baseLanguageMasterList.Add(e.id, e);
            Language e1 = new Language(e, 1);
            Globals_Game.languageMasterList.Add(e1.id, e1);

			// create terrain objects
			Terrain plains = new Terrain("terr_P", "Plains", 1);
			Globals_Game.terrainMasterList.Add (plains.id, plains);
            Terrain hills = new Terrain("terr_H", "Hills", 1.5);
            Globals_Game.terrainMasterList.Add(hills.id, hills);
            Terrain forrest = new Terrain("terr_F", "Forrest", 1.5);
            Globals_Game.terrainMasterList.Add(forrest.id, forrest);
            Terrain mountains = new Terrain("terr_M", "Mountains", 15);
            Globals_Game.terrainMasterList.Add(mountains.id, mountains);
            Terrain impassable_mountains = new Terrain("terr_MX", "Impassable mountains", 91);
            Globals_Game.terrainMasterList.Add(impassable_mountains.id, impassable_mountains);

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

            // create ranks for kingdoms, provinces, fiefs
            TitleName[] myTitleName03 = new TitleName[4];
            myTitleName03[0] = new TitleName("lang_C1", "King");
            myTitleName03[1] = new TitleName("lang_C2", "King");
            myTitleName03[2] = new TitleName("lang_E1", "King");
            myTitleName03[3] = new TitleName("lang_F1", "Roi");
            Rank myRank03 = new Rank(3, myTitleName03, 6);
            Globals_Game.rankMasterList.Add(myRank03.id, myRank03);

            TitleName[] myTitleName09 = new TitleName[4];
            myTitleName09[0] = new TitleName("lang_C1", "Prince");
            myTitleName09[1] = new TitleName("lang_C2", "Prince");
            myTitleName09[2] = new TitleName("lang_E1", "Prince");
            myTitleName09[3] = new TitleName("lang_F1", "Prince");
            Rank myRank09 = new Rank(9, myTitleName09, 4);
            Globals_Game.rankMasterList.Add(myRank09.id, myRank09);

            TitleName[] myTitleName11 = new TitleName[4];
            myTitleName11[0] = new TitleName("lang_C1", "Earl");
            myTitleName11[1] = new TitleName("lang_C2", "Earl");
            myTitleName11[2] = new TitleName("lang_E1", "Earl");
            myTitleName11[3] = new TitleName("lang_F1", "Comte");
            Rank myRank11 = new Rank(11, myTitleName11, 4);
            Globals_Game.rankMasterList.Add(myRank11.id, myRank11);

            TitleName[] myTitleName15 = new TitleName[4];
            myTitleName15[0] = new TitleName("lang_C1", "Baron");
            myTitleName15[1] = new TitleName("lang_C2", "Baron");
            myTitleName15[2] = new TitleName("lang_E1", "Baron");
            myTitleName15[3] = new TitleName("lang_F1", "Baron");
            Rank myRank15 = new Rank(15, myTitleName15, 2);
            Globals_Game.rankMasterList.Add(myRank15.id, myRank15);

            TitleName[] myTitleName17 = new TitleName[4];
            myTitleName17[0] = new TitleName("lang_C1", "Lord");
            myTitleName17[1] = new TitleName("lang_C2", "Lord");
            myTitleName17[2] = new TitleName("lang_E1", "Lord");
            myTitleName17[3] = new TitleName("lang_F1", "Sire");
            Rank myRank17 = new Rank(17, myTitleName17, 1);
            Globals_Game.rankMasterList.Add(myRank17.id, myRank17);

            // create Nationality objects for Kingdoms, Characters and positions
			Nationality nationality01 = new Nationality("Fr", "French");
            Globals_Game.nationalityMasterList.Add(nationality01.natID, nationality01);
			Nationality nationality02 = new Nationality("Eng", "English");
            Globals_Game.nationalityMasterList.Add(nationality02.natID, nationality02);

            // create Positions
            TitleName myTiName01 = new TitleName("lang_C1", "Keeper of the Privy Seal");
            TitleName myTiName011 = new TitleName("lang_C2", "Keeper of the Privy Seal");
            TitleName myTiName012 = new TitleName("lang_E1", "Keeper of the Privy Seal");
            TitleName[] myTitles01 = new TitleName[] { myTiName01, myTiName011, myTiName012 };
            Position myPos01 = new Position(100, myTitles01, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos01.id, myPos01);
            TitleName myTiName02 = new TitleName("lang_C1", "Lord High Steward");
            TitleName myTiName021 = new TitleName("lang_C2", "Lord High Steward");
            TitleName myTiName022 = new TitleName("lang_E1", "Lord High Steward");
            TitleName[] myTitles02 = new TitleName[] { myTiName02, myTiName021, myTiName022 };
            Position myPos02 = new Position(101, myTitles02, 2, null, nationality02);
            Globals_Game.positionMasterList.Add(myPos02.id, myPos02);

            // create kingdoms for provinces
            Kingdom myKingdom1 = new Kingdom("E0000", "England", nationality02, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom1.id, myKingdom1);
            Kingdom myKingdom2 = new Kingdom("F0000", "France", nationality01, r: myRank03);
            Globals_Game.kingdomMasterList.Add(myKingdom2.id, myKingdom2);

            // create provinces for fiefs
            Province myProv = new Province("ESX00", "Sussex", 6.2, king: myKingdom1, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv.id, myProv);
            Province myProv2 = new Province("ESR00", "Surrey", 6.2, king: myKingdom2, r: myRank11);
            Globals_Game.provinceMasterList.Add(myProv2.id, myProv2);

            // create financial arrays for fiefs
            double[] prevFin001 = new double[] { 6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2, 340894 };
            double[] currFin001 = new double[] { 5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2, 350274 };
            double[] prevFin002 = new double[14];
            double[] currFin002 = new double[14];
            double[] prevFin003 = new double[14];
            double[] currFin003 = new double[14];
            double[] prevFin004 = new double[14];
            double[] currFin004 = new double[14];
            double[] prevFin005 = new double[14];
            double[] currFin005 = new double[14];
            double[] prevFin006 = new double[] { 6.6, 4760000, 10, 12000, 42000, 2000, 2000, 5.3, 476000, 47594, 105594, 29512, 6.2, 340894 };
            double[] currFin006 = new double[] { 5.6, 4860000, 10, 12000, 42000, 2000, 2000, 5.5, 486000, 47594, 105594, 30132, 6.2, 350274 };
            double[] prevFin007 = new double[14];
            double[] currFin007 = new double[14];

            // create armies lists for fiefs
            List<string> armies001 = new List<string>();
            List<string> armies002 = new List<string>();
            List<string> armies003 = new List<string>();
            List<string> armies004 = new List<string>();
            List<string> armies005 = new List<string>();
            List<string> armies006 = new List<string>();
            List<string> armies007 = new List<string>();

            // create troop transfer lists for fiefs
            Dictionary<string, string[]> transfers001 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers002 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers003 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers004 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers005 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers006 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers007 = new Dictionary<string, string[]>();

            // create barredNationalities for fiefs
            List<string> barredNats01 = new List<string>();
            List<string> barredNats02 = new List<string>();
            List<string> barredNats03 = new List<string>();
            List<string> barredNats04 = new List<string>();
            List<string> barredNats05 = new List<string>();
            List<string> barredNats06 = new List<string>();
            List<string> barredNats07 = new List<string>();

            Fief myFief1 = new Fief("ESX02", "Cuckfield", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'C', c1, plains, fief1Chars, keep1BarChars, barredNats01, 0, 2000000, armies001, false, transfers001, false);
            Globals_Game.fiefMasterList.Add(myFief1.id, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", null, null, myRank15, myProv, 10000, 3.50, 0.20, 50, 10, 10, 1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'C', c1, hills, fief2Chars, keep2BarChars, barredNats02, 0, 4000, armies002, false, transfers002, false);
            Globals_Game.fiefMasterList.Add(myFief2.id, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', c1, plains, fief3Chars, keep3BarChars, barredNats03, 0, 100000, armies003, false, transfers003, false);
            Globals_Game.fiefMasterList.Add(myFief3.id, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", null, null, myRank17, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', c1, plains, fief4Chars, keep4BarChars, barredNats04, 0, 100000, armies004, false, transfers004, false);
            Globals_Game.fiefMasterList.Add(myFief4.id, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", null, null, myRank15, myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', f1, plains, fief5Chars, keep5BarChars, barredNats05, 0, 100000, armies005, false, transfers005, false);
            Globals_Game.fiefMasterList.Add(myFief5.id, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", null, null, myRank17, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', f1, plains, fief6Chars, keep6BarChars, barredNats06, 0, 100000, armies006, false, transfers006, false);
            Globals_Game.fiefMasterList.Add(myFief6.id, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", null, null, myRank15, myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', f1, forrest, fief7Chars, keep7BarChars, barredNats07, 0, 100000, armies007, false, transfers007, false);
            Globals_Game.fiefMasterList.Add(myFief7.id, myFief7);

			// create QuickGraph undirected graph
			// 1. create graph
			var myHexMap = new HexMapGraph("map001");
            Globals_Game.gameMap = myHexMap;
			// 2. Add edge and auto create vertices
			// from myFief1
			myHexMap.addHexesAndRoute(myFief1, myFief2, "W", (myFief1.terrain.travelCost + myFief2.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief1, myFief3, "E", (myFief1.terrain.travelCost + myFief3.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief1, myFief6, "NE", (myFief1.terrain.travelCost + myFief6.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief1, myFief4, "SE", (myFief1.terrain.travelCost + myFief4.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief1, myFief5, "SW", (myFief1.terrain.travelCost + myFief5.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief1, myFief7, "NW", (myFief1.terrain.travelCost + myFief7.terrain.travelCost) / 2);
			// from myFief2
			myHexMap.addHexesAndRoute(myFief2, myFief1, "E", (myFief2.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief2, myFief7, "NE", (myFief2.terrain.travelCost + myFief7.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief2, myFief5, "SE", (myFief2.terrain.travelCost + myFief5.terrain.travelCost) / 2);
			// from myFief3
			myHexMap.addHexesAndRoute(myFief3, myFief4, "SW", (myFief3.terrain.travelCost + myFief4.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief3, myFief6, "NW", (myFief3.terrain.travelCost + myFief6.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief3, myFief1, "W", (myFief3.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			// from myFief4
			myHexMap.addHexesAndRoute(myFief4, myFief3, "NE", (myFief4.terrain.travelCost + myFief3.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief4, myFief1, "NW", (myFief4.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief4, myFief5, "W", (myFief4.terrain.travelCost + myFief5.terrain.travelCost) / 2);
			// from myFief5
			myHexMap.addHexesAndRoute(myFief5, myFief1, "NE", (myFief5.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief5, myFief2, "NW", (myFief5.terrain.travelCost + myFief2.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief5, myFief4, "E", (myFief5.terrain.travelCost + myFief4.terrain.travelCost) / 2);
			// from myFief6
			myHexMap.addHexesAndRoute(myFief6, myFief3, "SE", (myFief6.terrain.travelCost + myFief3.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief6, myFief1, "SW", (myFief6.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief6, myFief7, "W", (myFief6.terrain.travelCost + myFief7.terrain.travelCost) / 2);
			// from myFief7
			myHexMap.addHexesAndRoute(myFief7, myFief6, "E", (myFief7.terrain.travelCost + myFief6.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief7, myFief1, "SE", (myFief7.terrain.travelCost + myFief1.terrain.travelCost) / 2);
			myHexMap.addHexesAndRoute(myFief7, myFief2, "SW", (myFief7.terrain.travelCost + myFief2.terrain.travelCost) / 2);

			// create goTo queues for characters
			Queue<Fief> myGoTo1 = new Queue<Fief>();
			Queue<Fief> myGoTo2 = new Queue<Fief>();
			Queue<Fief> myGoTo3 = new Queue<Fief>();
			Queue<Fief> myGoTo4 = new Queue<Fief>();
			Queue<Fief> myGoTo5 = new Queue<Fief>();
            Queue<Fief> myGoTo6 = new Queue<Fief>();
            Queue<Fief> myGoTo7 = new Queue<Fief>();
            Queue<Fief> myGoTo8 = new Queue<Fief>();
            Queue<Fief> myGoTo9 = new Queue<Fief>();
            Queue<Fief> myGoTo10 = new Queue<Fief>();
            Queue<Fief> myGoTo11 = new Queue<Fief>();
            Queue<Fief> myGoTo12 = new Queue<Fief>();

			// add some goTo entries for myChar1
			//myGoTo1.Enqueue (myFief2);
			//myGoTo1.Enqueue (myFief7);
            
			// create entourages for PCs
			List<NonPlayerCharacter> myEmployees1 = new List<NonPlayerCharacter>();
			List<NonPlayerCharacter> myEmployees2 = new List<NonPlayerCharacter>();

			// create lists of fiefs owned by PCs and add some fiefs
			List<Fief> myFiefsOwned1 = new List<Fief>();
			List<Fief> myFiefsOwned2 = new List<Fief>();

            // create lists of provinces owned by PCs and add some fiefs
            List<Province> myProvsOwned1 = new List<Province>();
            List<Province> myProvsOwned2 = new List<Province>();

            // create DOBs for characters
            Tuple<uint, byte> myDob001 = new Tuple<uint, byte>(1161, 1);
            Tuple<uint, byte> myDob002 = new Tuple<uint, byte>(1134, 0);
            Tuple<uint, byte> myDob003 = new Tuple<uint, byte>(1152, 2);
            Tuple<uint, byte> myDob004 = new Tuple<uint, byte>(1169, 3);
            Tuple<uint, byte> myDob005 = new Tuple<uint, byte>(1167, 2);
            Tuple<uint, byte> myDob006 = new Tuple<uint, byte>(1159, 2);
            Tuple<uint, byte> myDob007 = new Tuple<uint, byte>(1159, 3);
            Tuple<uint, byte> myDob008 = new Tuple<uint, byte>(1181, 2);
            Tuple<uint, byte> myDob009 = new Tuple<uint, byte>(1179, 0);
            Tuple<uint, byte> myDob010 = new Tuple<uint, byte>(1179, 0);
            Tuple<uint, byte> myDob011 = new Tuple<uint, byte>(1176, 1);
            Tuple<uint, byte> myDob012 = new Tuple<uint, byte>(1177, 3);

            // create titles list for characters
            List<string> myTitles001 = new List<string>();
            List<string> myTitles002 = new List<string>();
            List<string> myTitles003 = new List<string>();
            List<string> myTitles004 = new List<string>();
            List<string> myTitles005 = new List<string>();
            List<string> myTitles006 = new List<string>();
            List<string> myTitles007 = new List<string>();
            List<string> myTitles008 = new List<string>();
            List<string> myTitles009 = new List<string>();
            List<string> myTitles010 = new List<string>();
            List<string> myTitles011 = new List<string>();
            List<string> myTitles012 = new List<string>();

            // create armies list for PCs
            List<Army> myArmies001 = new List<Army>();
            List<Army> myArmies002 = new List<Army>();

            // create sieges list for PCs
            List<string> mySieges001 = new List<string>();
            List<string> mySieges002 = new List<string>();

            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("Char_47", "Dave", "Bond", myDob001, true, nationality02, true, 8.50, 9.0, myGoTo1, c1, 90, 0, 7.2, 6.1, Utility_Methods.generateSkillSet(), false, false, "Char_47", "Char_403", null, null, false, 13000, myEmployees1, myFiefsOwned1, myProvsOwned1, "ESX02", "ESX02", myTitles001, myArmies001, mySieges001, null, loc: myFief1, pID: "libdab");
            Globals_Game.pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("Char_40", "Bave", "Dond", myDob002, true, nationality01, true, 8.50, 6.0, myGoTo2, f1, 90, 0, 5.0, 4.5, Utility_Methods.generateSkillSet(), false, false, "Char_40", null, null, null, false, 13000, myEmployees2, myFiefsOwned2, myProvsOwned2, "ESR03", "ESR03", myTitles002, myArmies002, mySieges002, null, loc: myFief7, pID: "otherGuy");
            Globals_Game.pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("Char_401", "Jimmy", "Servant", myDob003, true, nationality02, true, 8.50, 6.0, myGoTo3, c1, 90, 0, 3.3, 6.7, Utility_Methods.generateSkillSet(), false, false, null, null, null, null, 0, false, false, myTitles003, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("Char_402", "Johnny", "Servant", myDob004, true, nationality02, true, 8.50, 6.0, myGoTo4, c1, 90, 0, 7.1, 5.2, Utility_Methods.generateSkillSet(), false, false, null, null, null, null, 10000, true, false, myTitles004, null, empl: myChar1.charID, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myNPC3 = new NonPlayerCharacter("Char_403", "Harry", "Bailiff", myDob005, true, nationality01, true, 8.50, 6.0, myGoTo5, c1, 90, 0, 7.1, 5.2, Utility_Methods.generateSkillSet(), true, false, null, null, null, null, 10000, false, false, myTitles005, null, empl: myChar2.charID, loc: myFief6);
            Globals_Game.npcMasterList.Add(myNPC3.charID, myNPC3);
            NonPlayerCharacter myChar1Wife = new NonPlayerCharacter("Char_404", "Bev", "Bond", myDob006, false, nationality02, true, 2.50, 9.0, myGoTo6, f1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), false, false, "Char_47", "Char_47", null, null, 30000, false, false, myTitles006, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            NonPlayerCharacter myChar2Son = new NonPlayerCharacter("Char_405", "Horatio", "Dond", myDob007, true, nationality01, true, 8.50, 6.0, myGoTo7, f1, 90, 0, 7.1, 5.2, Utility_Methods.generateSkillSet(), true, false, "Char_40", "Char_406", "Char_40", null, 10000, false, true, myTitles007, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            NonPlayerCharacter myChar2SonWife = new NonPlayerCharacter("Char_406", "Mave", "Dond", myDob008, false, nationality02, true, 2.50, 9.0, myGoTo8, f1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), true, false, "Char_40", "Char_405", null, null, 30000, false, false, myTitles008, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);
            NonPlayerCharacter myChar1Son = new NonPlayerCharacter("Char_407", "Rickie", "Bond", myDob009, true, nationality02, true, 2.50, 9.0, myGoTo9, c1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, true, myTitles009, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Son.charID, myChar1Son);
            NonPlayerCharacter myChar1Daughter = new NonPlayerCharacter("Char_408", "Elsie", "Bond", myDob010, false, nationality02, true, 2.50, 9.0, myGoTo10, c1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, false, myTitles010, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Daughter.charID, myChar1Daughter);
            NonPlayerCharacter myChar2Son2 = new NonPlayerCharacter("Char_409", "Wayne", "Dond", myDob011, true, nationality01, true, 2.50, 9.0, myGoTo11, f1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles011, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son2.charID, myChar2Son2);
            NonPlayerCharacter myChar2Daughter = new NonPlayerCharacter("Char_410", "Esmerelda", "Dond", myDob012, false, nationality01, true, 2.50, 9.0, myGoTo12, f1, 90, 0, 4.0, 6.0, Utility_Methods.generateSkillSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles012, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Daughter.charID, myChar2Daughter);

            /*
            // create and add a scheduled birth
            string[] birthPersonae = new string[] { myChar1Wife.familyID + "|headOfFamily", myChar1Wife.charID + "|mother", myChar1Wife.spouse + "|father" };
            JournalEntry myEntry = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 1, birthPersonae, "birth");
            Globals_Game.scheduledEvents.entries.Add(myEntry.jEntryID, myEntry); */

            // get character's correct days allowance
            myChar1.days = myChar1.getDaysAllowance();
            myChar2.days = myChar2.getDaysAllowance();
            myNPC1.days = myNPC1.getDaysAllowance();
            myNPC2.days = myNPC2.getDaysAllowance();
            myNPC3.days = myNPC3.getDaysAllowance();
            myChar1Wife.days = myChar1Wife.getDaysAllowance();
            myChar2Son.days = myChar2Son.getDaysAllowance();
            myChar2SonWife.days = myChar2SonWife.getDaysAllowance();
            myChar1Son.days = myChar1Son.getDaysAllowance();
            myChar1Daughter.days = myChar1Daughter.getDaysAllowance();
            myChar2Son2.days = myChar2Son2.getDaysAllowance();
            myChar2Daughter.days = myChar2Daughter.getDaysAllowance();

            // set kingdom owners
            Globals_Game.kingOne = myChar1;
            Globals_Game.kingTwo = myChar2;

            // set province owners
            myProv.owner = myChar2;
            myProv2.owner = myChar2;

            // Add provinces to list of provinces owned 
            myChar2.addToOwnedProvinces(myProv);
            myChar2.addToOwnedProvinces(myProv2);

            // set fief owners
			myFief1.owner = myChar1;
			myFief2.owner = myChar1;
			myFief3.owner = myChar1;
			myFief4.owner = myChar1;
			myFief5.owner = myChar2;
			myFief6.owner = myChar2;
			myFief7.owner = myChar2;

            // Add fiefs to list of fiefs owned 
            myChar1.addToOwnedFiefs(myFief1);
            myChar1.addToOwnedFiefs(myFief3);
            myChar1.addToOwnedFiefs(myFief4);
            myChar2.addToOwnedFiefs(myFief6);
            myChar1.addToOwnedFiefs(myFief2);
            myChar2.addToOwnedFiefs(myFief5);
            myChar2.addToOwnedFiefs(myFief7);

            // set kingdom title holders
            myKingdom1.titleHolder = myChar1.charID;
			myKingdom2.titleHolder = myChar2.charID;

            // set province title holders
            myProv.titleHolder = myChar1.charID;
            myProv2.titleHolder = myChar1.charID;

            // set fief title holders
            myFief1.titleHolder = myChar1.charID;
            myFief2.titleHolder = myChar1.charID;
            myFief3.titleHolder = myChar1.charID;
            myFief4.titleHolder = myChar1.charID;
            myFief5.titleHolder = myChar2.charID;
            myFief6.titleHolder = myChar2.charID;
            myFief7.titleHolder = myChar2.charID;

            // add titles (all types of places) to myTitles lists
            myChar1.myTitles.Add(myKingdom1.id);
            myChar1.myTitles.Add(myProv.id);
            myChar1.myTitles.Add(myFief1.id);
            myChar1.myTitles.Add(myFief2.id);
            myChar1.myTitles.Add(myFief3.id);
            myChar1.myTitles.Add(myFief4.id);
            myChar2.myTitles.Add(myKingdom2.id);
            myChar2.myTitles.Add(myProv2.id);
            myChar2.myTitles.Add(myFief5.id);
            myChar2.myTitles.Add(myFief6.id);
            myChar2.myTitles.Add(myFief7.id);

            // set fief ancestral owners
			myFief1.ancestralOwner = myChar1;
			myFief2.ancestralOwner = myChar1;
			myFief3.ancestralOwner = myChar1;
			myFief4.ancestralOwner = myChar1;
			myFief5.ancestralOwner = myChar2;
			myFief6.ancestralOwner = myChar2;
			myFief7.ancestralOwner = myChar2;

            // set fief bailiffs
            myFief1.bailiff = myChar1;
            myFief2.bailiff = myChar1;
            myFief6.bailiff = myNPC3;

			// add NPC to employees
            myChar1.hireNPC(myNPC2, 12000);
			// set employee as travelling companion
			myChar1.addToEntourage(myNPC2);
            // give player a wife
            myChar1.spouse = myChar1Wife.charID;
            // add NPC to employees/family
            myChar1.myNPCs.Add(myChar1Wife);
            myChar1.myNPCs.Add(myChar1Son);
            myChar1.myNPCs.Add(myChar1Daughter);
            myChar2.hireNPC(myNPC3, 10000);
            myChar2.myNPCs.Add(myChar2Son);
            myChar2.myNPCs.Add(myChar2SonWife);
            myChar2.myNPCs.Add(myChar2Son2);
            myChar2.myNPCs.Add(myChar2Daughter);

			// add some characters to myFief1
			myFief1.addCharacter(myChar1);
			myFief1.addCharacter(myChar2);
			myFief1.addCharacter(myNPC1);
            myFief1.addCharacter(myNPC2);
            myFief1.addCharacter(myChar1Son);
            myFief1.addCharacter(myChar1Daughter);
            myFief6.addCharacter(myNPC3);
            myFief1.addCharacter(myChar1Wife);
            myFief6.addCharacter(myChar2Son);
            myFief6.addCharacter(myChar2SonWife);
            myFief6.addCharacter(myChar2Son2);
            myFief6.addCharacter(myChar2Daughter);

            // populate Globals_Server.gameTypes
            Globals_Server.gameTypes.Add(0, "Individual points");
            Globals_Server.gameTypes.Add(1, "Individual position");
            Globals_Server.gameTypes.Add(2, "Team historical");

            // populate Globals_Server.combatValues
            uint[] eCombatValues = new uint[] {9, 9, 1, 9, 5, 3, 1};
            Globals_Server.combatValues.Add("Eng", eCombatValues);
            uint[] fCombatValues = new uint[] {7, 7, 3, 2, 4, 2, 1};
            Globals_Server.combatValues.Add("Fr", fCombatValues);
            uint[] sCombatValues = new uint[] { 8, 8, 1, 2, 4, 4, 1 };
            Globals_Server.combatValues.Add("Sco", sCombatValues);
            uint[] oCombatValues = new uint[] { 7, 7, 3, 2, 4, 2, 1 };
            Globals_Server.combatValues.Add("Oth", oCombatValues);

            // populate Globals_Server.recruitRatios
            double[] eRecruitRatios = new double[] { 0.01, 0.02, 0, 0.12, 0.03, 0.32, 0.49 };
            Globals_Server.recruitRatios.Add("Eng", eRecruitRatios);
            double[] fRecruitRatios = new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 };
            Globals_Server.recruitRatios.Add("Fr", fRecruitRatios);
            double[] sRecruitRatios = new double[] { 0.01, 0.02, 0, 0, 0.04, 0.43, 0.49 };
            Globals_Server.recruitRatios.Add("Sco", sRecruitRatios);
            double[] oRecruitRatios = new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 };
            Globals_Server.recruitRatios.Add("Oth", oRecruitRatios);

            // populate Globals_Server.battleProbabilities
            double[] odds = new double[] { 2, 3, 4, 5, 6, 99 };
            Globals_Server.battleProbabilities.Add("odds", odds);
            double[] bChance = new double[] { 10, 30, 50, 70, 80, 90 };
            Globals_Server.battleProbabilities.Add("battle", bChance);
            double[] pChance = new double[] { 10, 20, 30, 40, 50, 60 };
            Globals_Server.battleProbabilities.Add("pillage", pChance);

            // populate Globals_Game.jEntryPriorities
            // marriage
            string[] thisPriorityKey001 = {"proposalMade", "headOfFamilyBride"};
            Globals_Game.jEntryPriorities.Add(thisPriorityKey001, 2);
            string[] thisPriorityKey002 = { "proposalRejected", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey002, 2);
            string[] thisPriorityKey003 = { "proposalAccepted", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey003, 2);
            string[] thisPriorityKey004 = { "marriage", "headOfFamilyBride" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey004, 1);
            string[] thisPriorityKey005 = { "marriage", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey005, 1);
            string[] thisPriorityKey016 = { "marriageCancelled", "headOfFamilyGroom" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey016, 2);
            string[] thisPriorityKey017 = { "marriageCancelled", "headOfFamilyBride" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey017, 1);
            // birth
            string[] thisPriorityKey006 = { "birth", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey006, 2);
            string[] thisPriorityKey007 = { "birth", "father" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey007, 2);
            // battle
            string[] thisPriorityKey008 = { "battle", "defenderOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey008, 2);
            string[] thisPriorityKey009 = { "battle", "sallyOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey009, 2);
            string[] thisPriorityKey010 = { "battle", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey010, 1);
            // siege
            string[] thisPriorityKey011 = { "siege", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey011, 2);
            string[] thisPriorityKey012 = { "siegeReduction", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey012, 1);
            string[] thisPriorityKey013 = { "siegeStorm", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey013, 1);
            string[] thisPriorityKey014 = { "siegeNegotiation", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey014, 1);
            string[] thisPriorityKey015 = { "siegeEnd", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey015, 2);
            // death
            string[] thisPriorityKey018 = { "deathOfHeir", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey018, 2);
            string[] thisPriorityKey019 = { "deathOfFamilyMember", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey019, 1);
            string[] thisPriorityKey020 = { "deathOfEmployee", "employer" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey020, 1);
            string[] thisPriorityKey021 = { "deathOfPlayer", "newHeadOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey021, 2);
            string[] thisPriorityKey022 = { "deathOfPlayer", "deceasedHeadOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey022, 2);
            // injury
            string[] thisPriorityKey023 = { "injury", "employer" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey023, 1);
            string[] thisPriorityKey024 = { "injury", "headOfFamily" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey024, 1);
            string[] thisPriorityKey025 = { "injury", "injuredCharacter" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey025, 2);
            // pillage
            string[] thisPriorityKey026 = { "pillage", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey026, 2);
            // fief status change
            string[] thisPriorityKey027 = { "fiefStatusUnrest", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey027, 1);
            string[] thisPriorityKey028 = { "fiefStatusRebellion", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey028, 2);
            // quelling rebellion
            string[] thisPriorityKey029 = { "rebellionQuelled", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey029, 1);
            string[] thisPriorityKey030 = { "rebellionQuellFailed", "fiefOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey030, 1);
            // fief/province/position title holder change
            string[] thisPriorityKey031 = { "grantTitleFief", "newTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey031, 1);
            string[] thisPriorityKey032 = { "grantTitleFief", "oldTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey032, 1);
            string[] thisPriorityKey033 = { "grantTitleProvince", "newTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey033, 1);
            string[] thisPriorityKey034 = { "grantTitleProvince", "oldTitleHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey034, 1);
            string[] thisPriorityKey035 = { "grantPosition", "newPositionHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey035, 1);
            string[] thisPriorityKey036 = { "grantPosition", "oldPositionHolder" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey036, 1);
            // fief change of ownership
            string[] thisPriorityKey037 = { "fiefOwnership_Hostile", "oldOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey037, 2);
            string[] thisPriorityKey038 = { "fiefOwnership_Gift", "newOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey038, 2);
            // ownership/kingship challenges
            string[] thisPriorityKey039 = { "ownershipChallenge_new", "owner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey039, 2);
            string[] thisPriorityKey040 = { "ownershipChallenge_success", "newOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey040, 2);
            string[] thisPriorityKey041 = { "ownershipChallenge_success", "oldOwner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey041, 2);
            string[] thisPriorityKey042 = { "ownershipChallenge_failure", "owner" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey042, 2);
            string[] thisPriorityKey043 = { "ownershipChallenge_failure", "challenger" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey043, 2);
            string[] thisPriorityKey044 = { "depose_success", "newKing" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey044, 2);
            string[] thisPriorityKey045 = { "depose_success", "oldKing" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey045, 2);
            string[] thisPriorityKey049 = { "depose_success", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey049, 2);
            string[] thisPriorityKey046 = { "depose_failure", "king" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey046, 2);
            string[] thisPriorityKey047 = { "depose_failure", "pretender" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey047, 2);
            string[] thisPriorityKey050 = { "depose_failure", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey050, 1);
            string[] thisPriorityKey048 = { "depose_new", "king" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey048, 2);
            string[] thisPriorityKey051 = { "depose_new", "all" };
            Globals_Game.jEntryPriorities.Add(thisPriorityKey051, 2);
            // next available key = thisPriorityKey052

            // create an army and add in appropriate places
            uint[] myArmyTroops = new uint[] {10, 10, 0, 100, 100, 200, 400};
            Army myArmy = new Army(Globals_Game.getNextArmyID(), null, myChar1.charID, 90, myChar1.location.id, trp: myArmyTroops);
            Globals_Game.armyMasterList.Add(myArmy.armyID, myArmy);
            myChar1.myArmies.Add(myArmy);
            // myChar1.armyID = myArmy.armyID;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = new uint[] { 10, 10, 30, 0, 40, 200, 0 };
            Army myArmy2 = new Army(Globals_Game.getNextArmyID(), myChar2.charID, myChar2.charID, myChar2.days, myChar2.location.id, trp: myArmyTroops2, aggr: 1);
            Globals_Game.armyMasterList.Add(myArmy2.armyID, myArmy2);
            myChar2.myArmies.Add(myArmy2);
            myChar2.armyID = myArmy2.armyID;
            myChar2.location.armies.Add(myArmy2.armyID);

            // bar a character from the myFief1 keep
			myFief2.barCharacter(myNPC1.charID);
            myFief2.barCharacter(myChar2.charID);
            myFief2.barCharacter(myChar1Wife.charID);
            
            /*
            // create VictoryDatas for PCs
            VictoryData myVicData01 = new VictoryData(myChar1.playerID, myChar1.charID, myChar1.calculateStature(), myChar1.getPopulationPercentage(), myChar1.getFiefsPercentage(), myChar1.getMoneyPercentage());
            Globals_Game.victoryData.Add(myVicData01.playerID, myVicData01);
            VictoryData myVicData02 = new VictoryData(myChar2.playerID, myChar2.charID, myChar2.calculateStature(), myChar2.getPopulationPercentage(), myChar2.getFiefsPercentage(), myChar2.getMoneyPercentage());
            Globals_Game.victoryData.Add(myVicData02.playerID, myVicData02);*/

			// try retrieving fief from masterlist using fiefID
			// Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");
        }

        // ------------------- SEASONAL UPDATE

        /// <summary>
		/// Updates game objects at end/start of season
		/// </summary>
        public void seasonUpdate()
		{
            // used to check if character update is necessary
            bool performCharacterUpdate = true;

            // SWITCH OFF MESSAGES
            Globals_Client.showMessages = false;

            // FIEFS
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                fiefEntry.Value.updateFief();
            }

            // NONPLAYERCHARACTERS
            foreach (KeyValuePair<string, NonPlayerCharacter> npcEntry in Globals_Game.npcMasterList)
            {
                // check if NonPlayerCharacter is alive
                performCharacterUpdate = npcEntry.Value.isAlive;

                if (performCharacterUpdate)
                {
                    // updateCharacter includes checkDeath
                    npcEntry.Value.updateCharacter();

                    // check again if NonPlayerCharacter is alive (after checkDeath)
                    if (npcEntry.Value.isAlive)
                    {
                        // random move if has no boss and is not family member
                        if ((String.IsNullOrWhiteSpace(npcEntry.Value.employer)) && (String.IsNullOrWhiteSpace(npcEntry.Value.familyID)))
                        {
                            this.randomMoveNPC(npcEntry.Value);
                        }

                        // finish previously started multi-hex move if necessary
                        if (npcEntry.Value.goTo.Count > 0)
                        {
                            this.characterMultiMove(npcEntry.Value);
                        }
                    }
                }

            }

            // PLAYERCHARACTERS
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Game.pcMasterList)
            {
                // check if PlayerCharacter is alive
                performCharacterUpdate = pcEntry.Value.isAlive;

                if (performCharacterUpdate)
                {
                    // updateCharacter includes checkDeath
                    pcEntry.Value.updateCharacter();

                    // check again if PlayerCharacter is alive (after checkDeath)
                    if (pcEntry.Value.isAlive)
                    {
                        // finish previously started multi-hex move if necessary
                        if (pcEntry.Value.goTo.Count > 0)
                        {
                            this.characterMultiMove(pcEntry.Value);
                        }
                    }
                }
            }

            // add any newly promoted NPCs to Globals_Game.pcMasterList
            if (Globals_Game.promotedNPCs.Count > 0)
            {
                foreach (PlayerCharacter pc in Globals_Game.promotedNPCs)
                {
                    if (!Globals_Game.pcMasterList.ContainsKey(pc.charID))
                    {
                        Globals_Game.pcMasterList.Add(pc.charID, pc);
                    }
                }
            }

            // clear Globals_Game.promotedNPCs
            Globals_Game.promotedNPCs.Clear();

            // ARMIES

            // keep track of any armies requiring removal (if hav fallen below 100 men)
            List<Army> disbandedArmies = new List<Army>();
            bool hasDissolved = false;

            // iterate through armies
            foreach (KeyValuePair<string, Army> armyEntry in Globals_Game.armyMasterList)
            {
                hasDissolved = armyEntry.Value.updateArmy();

                // add to dissolvedArmies if appropriate
                if (hasDissolved)
                {
                    disbandedArmies.Add(armyEntry.Value);
                }
            }

            // remove any dissolved armies
            if (disbandedArmies.Count > 0)
            {
                for (int i = 0; i < disbandedArmies.Count; i++)
                {
                    // disband army
                    this.disbandArmy(disbandedArmies[i]);
                }

                // clear dissolvedArmies
                disbandedArmies.Clear();
            }

            // SIEGES

            // keep track of any sieges requiring removal
            List<Siege> dissolvedSieges = new List<Siege>();
            bool hasEnded = false;

            // iterate through sieges
            foreach (KeyValuePair<string, Siege> siegeEntry in Globals_Game.siegeMasterList)
            {
                hasEnded = siegeEntry.Value.updateSiege();

                // add to dissolvedSieges if appropriate
                if (hasEnded)
                {
                    dissolvedSieges.Add(siegeEntry.Value);
                }
            }

            // remove any dismantled sieges
            if (dissolvedSieges.Count > 0)
            {
                for (int i = 0; i < dissolvedSieges.Count; i++ )
                {
                    // construct event description to be passed into siegeEnd
                    string siegeDescription = "On this day of Our Lord the forces of ";
                    siegeDescription += dissolvedSieges[i].getBesiegingPlayer().firstName + " " + dissolvedSieges[i].getBesiegingPlayer().familyName;
                    siegeDescription += " have been forced to abandon the siege of " + dissolvedSieges[i].getFief().name;
                    siegeDescription += " due to the poor condition of their troops. The ownership of this fief is retained by ";
                    siegeDescription += dissolvedSieges[i].getDefendingPlayer().firstName + " " + dissolvedSieges[i].getDefendingPlayer().familyName + ".";

                    // dismantle siege
                    this.siegeEnd(dissolvedSieges[i], false, siegeDescription);
                }

                // clear dissolvedSieges
                dissolvedSieges.Clear();
            }

            // ADVANCE SEASON AND YEAR
            Globals_Game.clock.advanceSeason();

            //UPDATE AND GET SCORES for individual point game
            SortedList<double, string> currentScores = new SortedList<double,string>();
            if (Globals_Game.gameType == 0)
            {
                //update scores
                foreach (KeyValuePair<string, VictoryData> scoresEntry in Globals_Game.victoryData)
                {
                    scoresEntry.Value.updateData();
                }

                // get scores
                currentScores = Globals_Game.getCurrentScores();
            }

            // CHECK FOR END GAME
            string gameResults = "";
            bool endDateReached = false;
            bool absoluteVictory = false;
            Kingdom victor = null;

            // absolute victory (all fiefs owned by one kingdom)
            victor = this.checkTeamAbsoluteVictory();
            if (victor != null)
            {
                absoluteVictory = true;
                gameResults += "The kingdom of " + victor.name + " under the valiant leadership of ";
                gameResults += victor.owner.firstName + " " + victor.owner.familyName;
                gameResults += " is victorious, having taken all fiefs under its control.";
            }

            // if no absolute victory
            else
            {
                // check if game end date reached
                if (Globals_Game.getGameEndDate() == Globals_Game.clock.currentYear)
                {
                    endDateReached = true;
                }
            }

            // individual points game
            if (Globals_Game.gameType == 0)
            {
                if ((endDateReached) || (absoluteVictory))
                {
                    // get top scorer (ID)
                    string topScorer = currentScores.Last().Value;

                    foreach (KeyValuePair<double, string> scoresEntry in currentScores.Reverse())
                    {
                        // get PC
                        PlayerCharacter thisPC = Globals_Game.pcMasterList[Globals_Game.victoryData[scoresEntry.Value].playerCharacterID];

                        if (absoluteVictory)
                        {
                            gameResults += "\r\n\r\n";
                        }

                        // check for top scorer
                        if (thisPC.playerID.Equals(topScorer))
                        {
                            gameResults += "The individual winner is " + thisPC.firstName + " " + thisPC.familyName + " (player: " + thisPC.playerID + ")";
                            gameResults += " with a score of " + scoresEntry.Key + ".\r\n\r\nThe rest of the scores are:\r\n";
                        }

                        else
                        {
                            gameResults += thisPC.firstName + " " + thisPC.familyName + " (player: " + thisPC.playerID + ")";
                            gameResults += " with a score of " + scoresEntry.Key + ".\r\n";
                        }
                    }
                }
            }

            // individual position game
            else if (Globals_Game.gameType == 1)
            {
                if ((endDateReached) || (absoluteVictory))
                {
                    if (absoluteVictory)
                    {
                        gameResults += "\r\n\r\n";
                    }

                    gameResults += "The individual winners are ";
                    gameResults += Globals_Game.kingOne.firstName + " " + Globals_Game.kingOne.familyName + " (King of Kingdom One)";
                    gameResults += " and " + Globals_Game.kingTwo.firstName + " " + Globals_Game.kingTwo.familyName + " (King of Kingdom Two).";
                }
            }

            // team historical game
            else if (Globals_Game.gameType == 2)
            {
                if ((endDateReached) && (!absoluteVictory))
                {
                    victor = this.checkTeamHistoricalVictory();
                    gameResults += "The kingdom of " + victor.name + " under the valiant leadership of ";
                    gameResults += victor.owner.firstName + " " + victor.owner.familyName + " is victorious.";
                    if (victor.nationality.natID.Equals("Fr"))
                    {
                        gameResults += "  It has managed to eject the English from its sovereign territory.";
                    }
                    else if (victor.nationality.natID.Equals("Eng"))
                    {
                        gameResults += "  It has managed to retain control of at least one fief in French sovereign territory.";
                    }
                }
            }

            // announce winners
            if ((endDateReached) || (absoluteVictory))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(gameResults);
                }
            }

            if (!((endDateReached) || (absoluteVictory)))
            {
                // CHECK SCHEDULED EVENTS
                List<JournalEntry> entriesForRemoval = this.processScheduledEvents();
                // remove processed events from Globals_Game.scheduledEvents
                for (int i = 0; i < entriesForRemoval.Count; i++)
                {
                    Globals_Game.scheduledEvents.entries.Remove(entriesForRemoval[i].jEntryID);
                }

                // CHECK OWNERSHIP CHALLENGES
                Globals_Game.processOwnershipChallenges();
            }

            // SWITCH ON MESSAGES
            Globals_Client.showMessages = true;

            // REFRESH CURRENT SCREEN
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Checks for a historical team victory (victory depends on whether the English own any French fiefs)
        /// </summary>
        /// <returns>Kingdom object belonging to victor</returns>
        public Kingdom checkTeamHistoricalVictory()
        {
            Kingdom victor = null;

            // get France and England
            Kingdom france = Globals_Game.kingdomMasterList["Fr"];
            Kingdom england = Globals_Game.kingdomMasterList["Eng"];

            // set France as victor by default
            victor = france;

            // check each French fief for enemy occupation
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                if (fiefEntry.Value.getRightfulKingdom() == france)
                {
                    if (fiefEntry.Value.getCurrentKingdom() == england)
                    {
                        victor = england;
                    }
                }
            }

            return victor;
        }

        /// <summary>
        /// Checks for absolute victory (all fiefs owned by one kingdom)
        /// </summary>
        /// <returns>Kingdom object belonging to victor</returns>
        public Kingdom checkTeamAbsoluteVictory()
        {
            Kingdom victor = null;
            int fiefCount = 0;

            // iterate through kingdoms
            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                // reset fiefCount
                fiefCount = 0;

                // iterate through fiefs, checking if owned by this kingdom
                foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                {
                    if (fiefEntry.Value.getCurrentKingdom() == kingdomEntry.Value)
                    {
                        // if owned by this kingdom, increment count
                        fiefCount++;
                    }
                }

                // check if kingdom owns all fiefs
                if (fiefCount == Globals_Game.fiefMasterList.Count)
                {
                    victor = kingdomEntry.Value;
                    break;
                }
            }

            return victor;
        }

        /// <summary>
        /// Iterates through the scheduledEvents journal, implementing the appropriate actions
        /// </summary>
        /// <returns>List<JournalEntry> containing journal entries to be removed</returns>
        public List<JournalEntry> processScheduledEvents()
        {
            List<JournalEntry> forRemoval = new List<JournalEntry>();
            bool proceed = true;

            // iterate through scheduled events
            foreach (KeyValuePair<uint, JournalEntry> jEntry in Globals_Game.scheduledEvents.entries)
            {
                proceed = true;

                if ((jEntry.Value.year == Globals_Game.clock.currentYear) && (jEntry.Value.season == Globals_Game.clock.currentSeason))
                {
                    //BIRTH
                    if ((jEntry.Value.type).ToLower().Equals("birth"))
                    {
                        // get parents
                        NonPlayerCharacter mummy = null;
                        Character daddy = null;
                        for (int i = 0; i < jEntry.Value.personae.Length; i++)
                        {
                            string thisPersonae = jEntry.Value.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');

                            if (thisPersonaeSplit.Length > 1)
                            {
                                switch (thisPersonaeSplit[1])
                                {
                                    case "mother":
                                        mummy = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                        break;
                                    case "father":
                                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                        {
                                            daddy = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                                        }
                                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                        {
                                            daddy = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        // do conditional checks
                        // death of mother or father
                        if ((!mummy.isAlive) || (!daddy.isAlive))
                        {
                            proceed = false;
                        }


                        if (proceed)
                        {
                            // run childbirth procedure
                            this.giveBirth(mummy, daddy);
                        }

                        // add entry to list for removal
                        forRemoval.Add(jEntry.Value);
                    }

                    // MARRIAGE
                    else if ((jEntry.Value.type).ToLower().Equals("marriage"))
                    {
                        // get bride and groom
                        Character bride = null;
                        Character groom = null;

                        for (int i = 0; i < jEntry.Value.personae.Length; i++)
                        {
                            string thisPersonae = jEntry.Value.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');

                            switch (thisPersonaeSplit[1])
                            {
                                case "bride":
                                    bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                    break;
                                case "groom":
                                    if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                                    }
                                    else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                                    {
                                        groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        // CONDITIONAL CHECKS
                        // death of bride or groom
                        if ((!bride.isAlive) || (!groom.isAlive))
                        {
                            proceed = false;

                            // add entry to list for removal
                            forRemoval.Add(jEntry.Value);
                        }

                        // separated by siege
                        else
                        {
                            // if are in different fiefs OR in same fief but not both in keep
                            if ((bride.location != groom.location)
                                || ((bride.location == groom.location) && (bride.inKeep != groom.inKeep)))
                            {
                                // if there's a siege in the fief where the character is in the keep
                                if (((!String.IsNullOrWhiteSpace(bride.location.siege)) && (bride.inKeep))
                                    || ((!String.IsNullOrWhiteSpace(groom.location.siege)) && (groom.inKeep)))
                                {
                                    proceed = false;

                                    // postpone marriage until next season
                                    if (jEntry.Value.season == 3)
                                    {
                                        jEntry.Value.season = 0;
                                        jEntry.Value.year++;
                                    }
                                    else
                                    {
                                        jEntry.Value.season++;
                                    }
                                }
                            }
                        }

                        if (proceed)
                        {
                            // process marriage
                            this.processMarriage(jEntry.Value);

                            // add entry to list for removal
                            forRemoval.Add(jEntry.Value);
                        }

                    }
                }
            }

            return forRemoval;

        }

        // ------------------- EXIT/CLOSE

        /// <summary>
        /// Responds to the click event of the exitToolStripMenuItem
        /// closing the application
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // closes application
            Application.Exit();
        }

        /// <summary>
        /// Overrides System.Windows.Forms.OnFormClosing to allow for more controlled
        /// closing sequence whether exiting via File menu or X button.  Allows closing
        /// to proceed unhindered if Windows shutting down
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // allows immediate closing if Windows shutting down
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show("Really Quit?", "Exit", MessageBoxButtons.OKCancel))
            {
                case DialogResult.OK:
                    // write to database if necessary
                    if (Globals_Game.writeToDatabase)
                    {
                        this.databaseWrite("fromCSV");
                    }
                    break;

                // if cancel pressed, do nothing (don't close)
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                default:
                    break;
            }
        }

        // ------------------- UTILITY METHODS

        /// <summary>
        /// Refreshes whichever screen is currently being displayed in the UI
        /// </summary>
        public void refreshCurrentScreen()
        {
            // fief
            if (Globals_Client.containerToView == this.fiefContainer)
            {
                this.refreshFiefContainer(Globals_Client.fiefToView);
            }

            // character
            else if (Globals_Client.containerToView == this.characterContainer)
            {
                this.refreshCharacterContainer(Globals_Client.charToView);
            }

            // household affairs
            else if (Globals_Client.containerToView == this.houseContainer)
            {
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }

            // travel
            else if (Globals_Client.containerToView == this.travelContainer)
            {
                this.refreshTravelContainer();
            }

            // meeting place
            else if (Globals_Client.containerToView == this.meetingPlaceContainer)
            {
                if ((this.meetingPlaceLabel.Text).ToLower().Contains("tavern"))
                {
                    this.refreshMeetingPlaceDisplay("tavern");
                }
                else if ((this.meetingPlaceLabel.Text).ToLower().Contains("outwith"))
                {
                    this.refreshMeetingPlaceDisplay("outside");
                }
                else if ((this.meetingPlaceLabel.Text).ToLower().Contains("court"))
                {
                    this.refreshMeetingPlaceDisplay("court");
                }

            }

            // armies
            else if (Globals_Client.containerToView == this.armyContainer)
            {
                this.refreshArmyContainer(Globals_Client.armyToView);
            }

            // sieges
            else if (Globals_Client.containerToView == this.siegeContainer)
            {
                this.refreshSiegeContainer(Globals_Client.siegeToView);
            }

            // journal
            else if (Globals_Client.containerToView == this.journalContainer)
            {
                this.refreshJournalContainer(Globals_Client.jEntryToView);
            }

            // royal gifts
            else if (Globals_Client.containerToView == this.royalGiftsContainer)
            {
                this.refreshRoyalGiftsContainer();
            }

            // overlord provinces
            else if (Globals_Client.containerToView == this.provinceContainer)
            {
                this.refreshProvinceContainer(Globals_Client.provinceToView);
            }

            // adminEdit
            else if (Globals_Client.containerToView == this.adminEditContainer)
            {
                // get objectType
                string objectType = this.adminEditGetBtn.Tag.ToString();

                switch (objectType)
                {
                    case "PC":
                        this.refreshCharEdit();
                        break;
                    case "NPC":
                        this.refreshCharEdit();
                        break;
                    case "Fief":
                        this.refreshPlaceEdit();
                        break;
                    case "Province":
                        this.refreshPlaceEdit();
                        break;
                    case "Kingdom":
                        this.refreshPlaceEdit();
                        break;
                    case "Army":
                        this.refreshArmyEdit();
                        break;
                    case "Skill":
                        this.refreshSkillEdit();
                        break;
                }
            }

        }

        /// <summary>
        /// Disables all controls within the parent container
        /// </summary>
        /// <param name="parentContainer">The parent container</param>
        public void disableControls(Control parentContainer)
        {
            foreach (Control c in parentContainer.Controls)
            {
                // clear TextBoxes
                if (c is TextBox)
                {
                    (c as TextBox).Text = "";
                    c.Enabled = false;
                }

                // clear CheckBoxes
                if (c is CheckBox)
                {
                    (c as CheckBox).Checked = false;
                }

                // disable controls
                if ((c is CheckBox) || (c is Button))
                {
                    c.Enabled = false;
                }

                // clear ListViews
                if (c is ListView)
                {
                    (c as ListView).Items.Clear();
                }
            }
        }

        /// <summary>
        /// Enables all controls within the parent container
        /// </summary>
        /// <param name="parentContainer">The parent container</param>
        public void enableControls(Control parentContainer)
        {
            foreach (Control c in parentContainer.Controls)
            {
                // disable controls
                if (((c is CheckBox) || (c is Button)) || (c is TextBox))
                {
                    c.Enabled = true;
                }
            }
        }

        // ------------------- MVC

        /// <summary>
        /// Updates appropriate components when data received from observable
        /// </summary>
        /// <param name="info">String containing data about component to update</param>
        public void update(string info)
        {
            // get update info
            string[] infoSplit = info.Split('|');
            switch (infoSplit[0])
            {
                case "newEvent":
                    // get jEntry ID and retrieve from Globals_Game
                    if (!String.IsNullOrWhiteSpace(infoSplit[1]))
                    {
                        uint newJentryID = Convert.ToUInt32(infoSplit[1]);
                        JournalEntry newJentry = Globals_Game.pastEvents.entries[newJentryID];

                        // check to see if is of interest to player
                        if (newJentry.checkEventForInterest())
                        {
                            this.addMyPastEvent(newJentry);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        // ------------------- TEST METHODS

        /// <summary>
        /// Responds to the click event of the updateCharacter button
        /// which performs end/start of seasonal updates for character on display (testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void updateCharacter_Click(object sender, EventArgs e)
        {
            // something
        }

        /// <summary>
        /// Responds to the click event of the updateFiefBtn button
        /// which performs end/start of seasonal updates for Fief on display (testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void updateFiefBtn_Click(object sender, EventArgs e)
        {
            Globals_Client.fiefToView.validateFiefExpenditure();
            this.refreshFiefContainer();
        }

        /// <summary>
        /// Responds to the click event of button1
        /// calling any method I see fit
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.giveBirth(Globals_Game.npcMasterList[Globals_Client.myPlayerCharacter.spouse], Globals_Client.myPlayerCharacter);
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void testRefreshScreen(object sender, EventArgs e)
        {
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Responds to the click event of the testUpdateMenuItem
        /// performing a full seasonal update
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void testUpdateMenuItem_Click(object sender, EventArgs e)
        {
            this.seasonUpdate();
        }

        /// <summary>
        /// Responds to the click event of the switchPlayerMenuItem
        /// allowing the switch to another player (for testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void switchPlayerMenuItem_Click(object sender, EventArgs e)
        {
            // get new player ID
            string playerID = this.switchPlayerMenuTextBox.Text;

            if (String.IsNullOrWhiteSpace(playerID))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No PlayerCharacter ID entered.  Operation cancelled.");
                }
            }
            else if (!Globals_Game.pcMasterList.ContainsKey(playerID))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("PlayerCharacter could not be identified.  Operation cancelled.");
                }
            }
            else
            {
                // switch Globals_Client.myPlayerCharacter
                Globals_Client.myPlayerCharacter = Globals_Game.pcMasterList[playerID];
                // set new PC as Globals_Client.charToView
                Globals_Client.charToView = Globals_Client.myPlayerCharacter;
                // ensure new PC has correct permissions to view menus
                this.initMenuPermissions();
                // refresh and display character information
                this.refreshCharacterContainer(Globals_Client.charToView);
            }
        }

        /// <summary>
        /// Responds to the click event of the addTestJournalEntryToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void addTestJournalEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create and add a past event
            string[] myEventPersonae = new string[] { "101|father", "404|mother" };
            JournalEntry myEntry = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 0, myEventPersonae, "birth");
            Globals_Game.addPastEvent(myEntry);

            // and another
            string[] myEventPersonae002 = new string[] { "101|attackerOwner", "102|defenderOwner", "402|attackerLeader", "403|defenderLeader", "101|fiefOwner" };
            JournalEntry myEntry002 = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 0, myEventPersonae002, "battle", "ESX02", "On this day there was a battle between the forces of blah and blah.");
            Globals_Game.addPastEvent(myEntry002);

            // and another
            string[] myEventPersonae003 = new string[] { "405|father", "406|mother", "102|familyHead", "101|uncle" };
            JournalEntry myEntry003 = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 0, myEventPersonae003, "birth");
            Globals_Game.addPastEvent(myEntry003);
        }

        // ------------------- CONTROLS
        // ------------------- SYSADMIN

        /// <summary>
        /// Responds to the SelectedIndexChanged event of any of the adminEditSkillEffsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSkillEffsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.adminEditSkillEffsListView.SelectedItems.Count > 0)
            {
                // display selected skill for editing
                this.adminEditSkillEffTextBox.Text = this.adminEditSkillEffsListView.SelectedItems[0].SubItems[0].Text;
                this.adminEditSkillEfflvlTextBox.Text = this.adminEditSkillEffsListView.SelectedItems[0].SubItems[1].Text;
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'edit skill effects' buttons
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSkillEffBtn_Click(object sender, EventArgs e)
        {
            string effName = null;
            double effLvl = 0;
            bool effectsChanged = false;

            // get button and tag
            Button thisButton = (sender as Button);
            string operation = thisButton.Tag.ToString();

            try
            {
                // get effects collection
                Dictionary<string, double> effects = new Dictionary<string, double>();
                for (int i = 0; i < this.adminEditSkillEffsListView.Items.Count; i++)
                {
                    effects.Add(this.adminEditSkillEffsListView.Items[i].SubItems[0].Text,
                        Convert.ToDouble(this.adminEditSkillEffsListView.Items[i].SubItems[1].Text));
                }

                // get selected effect
                effName = this.adminEditSkillEffTextBox.Text;
                if (!String.IsNullOrWhiteSpace(effName))
                {
                    effLvl = Convert.ToDouble(this.adminEditSkillEfflvlTextBox.Text);
                }

                if (effLvl > 0)
                {
                    // perform operation
                    switch (operation)
                    {
                        // change selected effect
                        case "chaEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects[effName] = effLvl;
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        case "addEffect":
                            // check effect present in collection
                            if (!effects.ContainsKey(effName))
                            {
                                effects.Add(effName, effLvl);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " already exists.  Operation cancelled.");
                                }
                            }
                            break;
                        case "delEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects.Remove(effName);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (effectsChanged)
                    {
                        this.refreshSkillEffectsList(effects);
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Effects updated.");
                        }
                    }
                }

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the adminEditSaveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSaveBtn_Click(object sender, EventArgs e)
        {
            bool success = false;

            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    success = this.saveCharEdit(objectType);
                    break;

                case "NPC":
                    success = this.saveCharEdit(objectType);
                    break;

                case "Fief":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Province":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Kingdom":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Skill":
                    success = this.saveSkillEdit();
                    break;

                case "Army":
                    success = this.saveArmyEdit();
                    break;

                default:
                    break;
            }

            if (success)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Object saved.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'edit object' MenuItems
        /// displaying the appropriate screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditObjectMenuItem_Click(object sender, EventArgs e)
        {
            // get MenuItem
            ToolStripMenuItem thisItem = (sender as ToolStripMenuItem);
            string objectType = thisItem.Tag.ToString();

            // display edit object screen
            Globals_Client.containerToView = this.adminEditContainer;
            Globals_Client.containerToView.BringToFront();

            // set get/save button tag to identify object type (for retrieving and saving object)
            this.adminEditGetBtn.Tag = objectType;
            this.adminEditSaveBtn.Tag = objectType;
            this.adminEditCancelBtn.Tag = objectType;

            // clear previous object ID from TextBox
            this.adminEditTextBox.Text = "";

            // change admin edit control properties to match object type
            this.adminEditGetBtn.Text = "Get " + objectType;
            this.adminEditSaveBtn.Text = "Save " + objectType;
            this.adminEditLabel.Text = objectType + " ID";

            // display appropriate panel
            switch (objectType)
            {
                case "PC":
                    // clear previous data
                    this.refreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit pc panel
                    this.adminEditCharPcPanel.BringToFront();
                    break;
                case "NPC":
                    // clear previous data
                    this.refreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit npc panel
                    this.adminEditCharNpcPanel.BringToFront();
                    break;
                case "Fief":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit fief panel
                    this.adminEditFiefPanel.BringToFront();
                    break;
                case "Province":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit province panel
                    this.adminEditProvPanel.BringToFront();
                    break;
                case "Kingdom":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit kingdom panel
                    this.adminEditKingPanel.BringToFront();
                    break;
                case "Skill":
                    // clear previous data
                    this.refreshSkillEdit();
                    this.adminEditSkillIdTextBox.ReadOnly = true;
                    // display edit skill panel
                    this.adminEditSkillPanel.BringToFront();
                    break;
                case "Army":
                    // clear previous data
                    this.refreshArmyEdit();
                    this.adminEditArmyIdTextBox.ReadOnly = true;
                    // display edit army panel
                    this.adminEditArmyPanel.BringToFront();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to the click event of any of the adminEditGetBtn button
        /// retrieving the specified object
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditGetBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // get specified object
            switch (objectType)
            {
                case "PC":
                    // get PC
                    PlayerCharacter thisPC = null;
                    if (Globals_Game.pcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisPC = Globals_Game.pcMasterList[this.adminEditTextBox.Text];
                    }

                    // display PC details
                    if (thisPC != null)
                    {
                        this.refreshCharEdit(thisPC);
                    }
                    break;
                case "NPC":
                    // get NPC
                    NonPlayerCharacter thisNPC = null;
                    if (Globals_Game.npcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisNPC = Globals_Game.npcMasterList[this.adminEditTextBox.Text];
                    }

                    // display NPC details
                    if (thisNPC != null)
                    {
                        this.refreshCharEdit(thisNPC);
                    }
                    break;

                case "Fief":
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.adminEditTextBox.Text];
                    }

                    // display fief details
                    if (thisFief != null)
                    {
                        this.refreshPlaceEdit(thisFief);
                    }
                    break;

                case "Province":
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.adminEditTextBox.Text];
                    }

                    // display province details
                    if (thisProv != null)
                    {
                        this.refreshPlaceEdit(thisProv);
                    }
                    break;

                case "Kingdom":
                    // get kingdom
                    Kingdom thiskingdom = null;
                    if (Globals_Game.kingdomMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thiskingdom = Globals_Game.kingdomMasterList[this.adminEditTextBox.Text];
                    }

                    // display kingdom details
                    if (thiskingdom != null)
                    {
                        this.refreshPlaceEdit(thiskingdom);
                    }
                    break;

                case "Skill":
                    // get skill
                    Skill thisSkill = null;
                    if (Globals_Game.skillMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisSkill = Globals_Game.skillMasterList[this.adminEditTextBox.Text];
                    }

                    // display skill details
                    if (thisSkill != null)
                    {
                        this.refreshSkillEdit(thisSkill);
                    }
                    break;

                case "Army":
                    // get army
                    Army thisArmy = null;
                    if (Globals_Game.armyMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisArmy = Globals_Game.armyMasterList[this.adminEditTextBox.Text];
                    }

                    // display army details
                    if (thisArmy != null)
                    {
                        this.refreshArmyEdit(thisArmy);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to the click event of the adminEditCancelBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditCancelBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    this.refreshCharEdit();
                    break;

                case "NPC":
                    this.refreshCharEdit();
                    break;

                case "Fief":
                    this.refreshPlaceEdit();
                    break;

                case "Province":
                    this.refreshPlaceEdit();
                    break;

                case "Kingdom":
                    this.refreshPlaceEdit();
                    break;

                case "Skill":
                    this.refreshSkillEdit();
                    break;

                case "Army":
                    this.refreshArmyEdit();
                    break;

                default:
                    break;
            }

            // clear ID box
            this.adminEditTextBox.Clear();
        }

        // ------------------- SIEGE/PILLAGE/REBELLION

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the siegeListView object,
        /// allowing details of the selected siege to be displayed
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get siege to view
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                Globals_Client.siegeToView = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];
            }

            if (Globals_Client.siegeToView != null)
            {
                Army besiegingArmy = Globals_Client.siegeToView.getBesiegingArmy();
                PlayerCharacter besiegingPlayer = Globals_Client.siegeToView.getBesiegingPlayer();
                bool playerIsBesieger = (Globals_Client.myPlayerCharacter == besiegingPlayer);

                // display data for selected siege
                this.siegeTextBox.Text = this.displaySiegeData(Globals_Client.siegeToView);

                // if player is besieger
                if (playerIsBesieger)
                {
                    // enable various controls
                    this.siegeReduceBtn.Enabled = true;
                    this.siegeEndBtn.Enabled = true;

                    // if besieging army has a leader
                    if (!String.IsNullOrWhiteSpace(besiegingArmy.leader))
                    {
                        // enable proactive controls (storm, negotiate)
                        this.siegeNegotiateBtn.Enabled = true;
                        this.siegeStormBtn.Enabled = true;
                    }

                    // if besieging army has no leader
                    else
                    {
                        // disable proactive controls (storm, negotiate)
                        this.siegeNegotiateBtn.Enabled = false;
                        this.siegeStormBtn.Enabled = false;
                    }
                }

                // if player is defender
                else
                {
                    // disable various controls
                    this.siegeNegotiateBtn.Enabled = false;
                    this.siegeStormBtn.Enabled = false;
                    this.siegeReduceBtn.Enabled = false;
                    this.siegeEndBtn.Enabled = false;
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armySiegeBtn button
        /// instigating the siege of a fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armySiegeBtn_Click(object sender, EventArgs e)
        {
            // check army selected
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

                // get fief
                Fief thisFief = thisArmy.getLocation();

                // do various checks
                proceed = this.checksBeforePillageSiege(thisArmy, thisFief, "siege");

                // process siege
                if (proceed)
                {
                    this.siegeStart(thisArmy, thisFief);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the viewMySiegesToolStripMenuItem
        /// displaying the siege management screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewMySiegesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.siegeToView = null;
            this.refreshSiegeContainer();
        }

        /// <summary>
        /// Responds to any of the click events of the siegeRound buttons
        /// processing a single siege round of specified type
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeRoundBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get tag from button
                Button button = sender as Button;
                string roundType = button.Tag.ToString();

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege);

                if (proceed)
                {
                    // process siege round of specified type
                    this.siegeReductionRound(thisSiege, roundType);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No siege selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the siegeEndBtn button
        /// dismantling the selected siege
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeEndBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege, "end");

                if (proceed)
                {
                    // construct event description to be passed into siegeEnd
                    string siegeDescription = "On this day of Our Lord the forces of ";
                    siegeDescription += thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                    siegeDescription += " have chosen to abandon the siege of " + thisSiege.getFief().name;
                    siegeDescription += ". " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                    siegeDescription += " retains ownership of the fief.";

                    // process siege reduction round
                    this.siegeEnd(thisSiege, false, siegeDescription);

                    //refresh screen
                    this.refreshCurrentScreen();
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No siege selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the armyPillageBtn button
        /// instigating the pillage of a fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyPillageBtn_Click(object sender, EventArgs e)
        {
            // check army selected
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

                // get fief
                Fief thisFief = thisArmy.getLocation();

                // do various checks
                proceed = this.checksBeforePillageSiege(thisArmy, thisFief);

                // process pillage
                if (proceed)
                {
                    this.pillageFief(thisArmy, thisFief);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the Click event of the armyQuellRebellionBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyQuellRebellionBtn_Click(object sender, EventArgs e)
        {
            if (this.armyListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get army
                Army thisArmy = null;
                if (Globals_Game.armyMasterList.ContainsKey(this.armyListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisArmy != null)
                {
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(thisArmy.location))
                    {
                        thisFief = Globals_Game.fiefMasterList[thisArmy.location];
                    }

                    if (thisFief != null)
                    {
                        // do various checks
                        proceed = this.checksBeforePillageSiege(thisArmy, thisFief, circumstance: "quellRebellion");

                        if (proceed)
                        {
                            bool quellSuccess = thisFief.quellRebellion(thisArmy);

                            // quell successful, pillage fief
                            if (quellSuccess)
                            {
                                // pillage the fief
                                this.processPillage(thisFief, thisArmy, "quellRebellion");
                            }

                            // if not successful, retreat army
                            else
                            {
                                // retreat army 1 hex
                                this.processRetreat(thisArmy, 1);
                            }
                        }
                    }
                }

            }
        }

        // ------------------- ROYAL/OVERLORD FUNCTIONS

        /// <summary>
        /// Responds to the click event of the royalGiftsToolStripMenuItem
        /// which displays royal gifts screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display royal gifts screen
            this.refreshRoyalGiftsContainer();
            Globals_Client.containerToView = this.royalGiftsContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of either of the royal gifts ListViews
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Province thisProv = null;
            Fief thisFief = null;
            Position thisPos = null;

            // get ListView tag
            ListView listview = sender as ListView;
            string whichView = listview.Tag.ToString();

            // check for and correct 'loop backs' due to listview item deselection
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                }
            }

            // get selected place or position
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    if (Globals_Game.provinceMasterList.ContainsKey(this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    if (Globals_Game.fiefMasterList.ContainsKey(this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    // get position
                    if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                    {
                        thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                    }
                }
            }

            // deselect any selected items in other listView
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
            }

            // set button text and tag
            if (whichView.Equals("province"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Province Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Province Title";
                if (thisProv != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "province|" + thisProv.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "province|" + thisProv.id;
                }
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Fief Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Fief Title";
                if (thisFief != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsGiftFiefBtn.Tag = "fief|" + thisFief.id;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (thisPos != null)
                {
                    this.royalGiftsPositionBtn.Tag = thisPos.id;
                }
            }

            // enable/disable controls as appropriate

            // check to see if viewer is king or herald
            if (!Globals_Client.myPlayerCharacter.checkIsHerald())
            {
                // provinces
                if (whichView.Equals("province"))
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        if (thisProv != null)
                        {
                            // revoke title button
                            if (thisProv.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }
                        }
                    }

                    // 'grant title' button
                    this.royalGiftsGrantTitleBtn.Enabled = true;

                    // gift fief button
                    this.royalGiftsGiftFiefBtn.Enabled = false;

                    // position buttons
                    this.royalGiftsPositionBtn.Enabled = false;
                    this.royalGiftsPositionRemoveBtn.Enabled = false;
                }

                // fiefs
                else if (whichView.Equals("fief"))
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        if (thisFief != null)
                        {
                            // revoke title button
                            if (thisFief.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = true;

                            // 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = true;

                            // position buttons
                            this.royalGiftsPositionBtn.Enabled = false;
                            this.royalGiftsPositionRemoveBtn.Enabled = false;
                        }
                    }
                }

                // positions
                else if (whichView.Equals("position"))
                {
                    if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        if (thisPos != null)
                        {
                            // bestow position button
                            this.royalGiftsPositionBtn.Enabled = true;

                            // remove position button, enabled if there is a current holder
                            if (!String.IsNullOrWhiteSpace(thisPos.officeHolder))
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = true;
                            }
                            else
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = false;
                            }

                            // revoke title button
                            this.royalGiftsRevokeTitleBtn.Enabled = false;

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = false;

                            // always enable 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = false;
                        }
                    }
                }
            }

            // don't enable controls if herald
            else
            {
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
            }

            // give focus back to appropriate listview
            if (whichView.Equals("province"))
            {
                this.royalGiftsProvListView.Focus();
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsFiefListView.Focus();
            }
            else if (whichView.Equals("position"))
            {
                this.royalGiftsPositionListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the click event of either the royalGiftsGrantTitleBtn button
        /// or the royalGiftsGiftFiefBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsBtn_Click(object sender, EventArgs e)
        {
            // get gift type and place id from button tag and name
            Button button = sender as Button;
            string giftID = button.Tag.ToString();
            string giftType = null;

            if (button.Name.ToString().Equals("royalGiftsGrantTitleBtn"))
            {
                giftType = "royalGiftTitle";
            }
            else if (button.Name.ToString().Equals("royalGiftsGiftFiefBtn"))
            {
                giftType = "royalGiftFief";
            }
            else if (button.Name.ToString().Equals("royalGiftsPositionBtn"))
            {
                giftType = "royalGiftPosition";
            }

            if (!String.IsNullOrWhiteSpace(giftType))
            {
                // check for previously opened SelectionForm and close if necessary
                if (Application.OpenForms.OfType<SelectionForm>().Any())
                {
                    Application.OpenForms.OfType<SelectionForm>().First().Close();
                }

                // open new SelectionForm
                SelectionForm royalGiftSelection = null;
                // if gifting place or place title
                if (!giftType.Equals("royalGiftPosition"))
                {
                    royalGiftSelection = new SelectionForm(this, giftType, place: giftID);
                }

                // if bestowing position
                else
                {
                    royalGiftSelection = new SelectionForm(this, giftType, posID: Convert.ToByte(giftID));
                }
                royalGiftSelection.Show();
            }

        }

        /// <summary>
        /// Responds to the Click event of the manageProvincesToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void manageProvincesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear existing provinceToView
            Globals_Client.provinceToView = null;

            // display royal gifts screen
            this.refreshProvinceContainer();

            // display household affairs screen
            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the provinceProvListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceProvListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // refresh fief list
                    this.refreshProvinceFiefList(thisProvince);

                    // populate provinceTaxTextBox
                    this.provinceTaxTextBox.Text = thisProvince.taxRate.ToString();

                    // enable controls
                    this.provinceTaxBtn.Enabled = true;
                    this.provinceTaxTextBox.Enabled = true;
                    this.provinceChallengeBtn.Enabled = true;

                    // set provinceToView
                    Globals_Client.provinceToView = thisProvince;
                }
            }
        }

        /// <summary>
        /// Responds to the Click event of the provinceTaxBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceTaxBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                bool rateChanged = false;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // keep track of whether tax has changed
                    double originalRate = thisProvince.taxRate;

                    try
                    {
                        // get new rate
                        Double newTax = Convert.ToDouble(this.provinceTaxTextBox.Text);

                        // if rate changed, commit new rate
                        if (newTax != originalRate)
                        {
                            // adjust tax rate
                            thisProvince.adjustTaxRate(newTax);
                            rateChanged = true;

                            // display confirmation message
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Province tax rate changed.");
                            }
                        }
                    }
                    catch (System.FormatException fe)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                        }
                    }
                    catch (System.OverflowException ofe)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                        }
                    }
                    finally
                    {
                        // refresh screen if expenditure changed
                        if (rateChanged)
                        {
                            // refresh display
                            this.refreshCurrentScreen();
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsPositionRemoveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsPositionRemoveBtn_Click(object sender, EventArgs e)
        {
            if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
            {
                // get position
                Position thisPos = null;
                if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                {
                    thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                }

                if (thisPos != null)
                {
                    // get current holder
                    PlayerCharacter currentHolder = thisPos.getOfficeHolder();

                    // remove from position
                    if (currentHolder != null)
                    {
                        thisPos.removeFromOffice(currentHolder);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }

            }
        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsRevokeTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsRevokeTitleBtn_Click(object sender, EventArgs e)
        {
            // get place type and id from button tag
            Button button = sender as Button;
            string[] placeDetails = button.Tag.ToString().Split('|');

            // fiefs
            if (placeDetails[0].Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisFief = Globals_Game.fiefMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisFief != null)
                    {
                        Globals_Client.myPlayerCharacter.grantTitle(Globals_Client.myPlayerCharacter, thisFief);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }
            }

            // provinces
            else if (placeDetails[0].Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisProv = Globals_Game.provinceMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisProv != null)
                    {
                        Globals_Client.myPlayerCharacter.grantTitle(Globals_Client.myPlayerCharacter, thisProv);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the Click event of the provinceChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get kingdom
                Kingdom targetKingdom = null;
                if (Globals_Game.kingdomMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[4].Text))
                {
                    targetKingdom = Globals_Game.kingdomMasterList[this.provinceProvListView.SelectedItems[0].SubItems[4].Text];
                }

                if (targetKingdom != null)
                {
                    targetKingdom.lodgeOwnershipChallenge();
                }

                this.provinceProvListView.Focus();
            }
        }

        // ------------------- TRAVEL SCREEN

        /// <summary>
        /// Responds to the click event of any routeBtn buttons invoking the takeThisRoute method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void routeBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the movement command occurred
            string whichScreen = button.Tag.ToString();

            // perform move
            this.takeThisRoute(whichScreen);
        }

        /// <summary>
        /// Responds to the click event of the travelExamineArmiesBtn button
        /// displaying a list of all armies in the Player's current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // examine armies
            this.examineArmiesInFief(Globals_Client.myPlayerCharacter);
        }

        /// <summary>
        /// Responds to the click event of the travelCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelCampBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get days to camp
                byte campDays = Convert.ToByte(this.travelCampDaysTextBox.Text);

                // camp
                this.campWaitHere(Globals_Client.myPlayerCharacter, campDays);
            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
            finally
            {
                // refresh display
                this.refreshTravelContainer();
            }

        }

        /// <summary>
        /// Responds to the click event of any of the travel buttons
        /// which attempts to move the player to the target fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelBtnClick(object sender, EventArgs e)
        {
            bool success = false;
            // necessary in order to be able to access button tag
            Button button = sender as Button;
            // get target fief using travel button tag (contains direction string)
            Fief targetFief = Globals_Game.gameMap.getFief(Globals_Client.myPlayerCharacter.location, button.Tag.ToString());

            if (targetFief != null)
            {
                // get travel cost
                double travelCost = this.getTravelCost(Globals_Client.myPlayerCharacter.location, targetFief, Globals_Client.myPlayerCharacter.armyID);
                // attempt to move player to target fief
                success = this.moveCharacter(Globals_Client.myPlayerCharacter, targetFief, travelCost);
                // if move successfull, refresh travel display
                if (success)
                {
                    Globals_Client.fiefToView = targetFief;
                    this.refreshTravelContainer();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the enterKeepBtn button
        /// which causes the player (and entourage) to enter/exit the keep and
        /// refreshes the travel screen, setting appropriate text for the enterKeepBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void enterKeepBtn_Click(object sender, EventArgs e)
        {
            // attempt to enter/exit keep
            bool success = Globals_Client.myPlayerCharacter.exitEnterKeep();

            // if successful
            if (success)
            {
                // refresh display
                this.refreshTravelContainer();
            }

        }

        /// <summary>
        /// Responds to the click event of any moveTo buttons invoking the moveTo method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void moveToBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the movement command occurred
            string whichScreen = button.Tag.ToString();

            // perform move
            this.moveTo(whichScreen);
        }

        /// <summary>
        /// Responds to the click event of any of the 'visit meeting place' buttons
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void visitMeetingPlace(object sender, EventArgs e)
        {
            bool success = true;

            // get button
            Button thisButton = (sender as Button);
            string place = thisButton.Tag.ToString();

            // enter/exit keep if required
            switch (place)
            {
                case "court":
                    if (!Globals_Client.myPlayerCharacter.inKeep)
                    {
                        success = Globals_Client.myPlayerCharacter.enterKeep();
                    }
                    break;
                default:
                    if (Globals_Client.myPlayerCharacter.inKeep)
                    {
                        success = Globals_Client.myPlayerCharacter.exitKeep();
                    }
                    break;
            }

            if (success)
            {
                // set button tags to reflect which meeting place
                this.hireNPC_Btn.Tag = place;
                this.meetingPlaceMoveToBtn.Tag = place;
                this.meetingPlaceRouteBtn.Tag = place;
                this.meetingPlaceEntourageBtn.Tag = place;

                // refresh outside keep screen 
                this.refreshMeetingPlaceDisplay(place);

                // display tavern screen
                Globals_Client.containerToView = this.meetingPlaceContainer;
                Globals_Client.containerToView.BringToFront();
            }
        }

        /// <summary>
        /// Responds to the click event of the navigateToolStripMenuItem
        /// which refreshes and displays the navigation screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ensure reflects player's location
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;

            // refresh navigation data
            Globals_Client.fiefToView = Globals_Client.myPlayerCharacter.location;
            this.refreshTravelContainer();

            // show navigation screen
            Globals_Client.containerToView = this.travelContainer;
            Globals_Client.containerToView.BringToFront();
        }

        // ------------------- MEETING PLACE SCREEN

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the meetingPlaceCharsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceCharsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in the fief
            for (int i = 0; i < Globals_Client.fiefToView.charactersInFief.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.fiefToView.charactersInFief[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.fiefToView.charactersInFief[i];

                        // check whether is this PC's employee or family
                        if (Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.fiefToView.charactersInFief[i]))
                        {
                            // see if is in entourage to set text of entourage button
                            if ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).inEntourage)
                            {
                                this.meetingPlaceEntourageBtn.Text = "Remove From Entourage";
                            }
                            else
                            {
                                this.meetingPlaceEntourageBtn.Text = "Add To Entourage";
                            }

                            // enable 'move to' controls
                            this.meetingPlaceMoveToBtn.Enabled = true;
                            this.meetingPlaceMoveToTextBox.Enabled = true;
                            this.meetingPlaceRouteBtn.Enabled = true;
                            this.meetingPlaceRouteTextBox.Enabled = true;
                            this.meetingPlaceEntourageBtn.Enabled = true;

                            // disable marriage proposals
                            this.meetingPlaceProposeBtn.Enabled = false;
                            this.meetingPlaceProposeTextBox.Text = "";
                            this.meetingPlaceProposeTextBox.Enabled = false;

                            // if is employee
                            if ((!String.IsNullOrWhiteSpace((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer))
                                && ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer.Equals(Globals_Client.myPlayerCharacter.charID)))
                            {
                                // set appropriate text for hire/fire button, and enable it
                                this.hireNPC_Btn.Text = "Fire NPC";
                                this.hireNPC_Btn.Enabled = true;
                                // disable 'salary offer' text box
                                this.hireNPC_TextBox.Visible = false;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }
                        }

                        // if is not employee or family
                        else
                        {
                            // set appropriate text for hire/fire controls, and enable them
                            this.hireNPC_Btn.Text = "Hire NPC";
                            this.hireNPC_TextBox.Visible = true;

                            // can only employ men (non-PCs)
                            if (charToDisplay.checkCanHire(Globals_Client.myPlayerCharacter))
                            {
                                this.hireNPC_Btn.Enabled = true;
                                this.hireNPC_TextBox.Enabled = true;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }

                            // disable 'move to' and entourage controls
                            this.meetingPlaceMoveToBtn.Enabled = false;
                            this.meetingPlaceMoveToTextBox.Enabled = false;
                            this.meetingPlaceRouteBtn.Enabled = false;
                            this.meetingPlaceRouteTextBox.Enabled = false;
                            this.meetingPlaceEntourageBtn.Enabled = false;

                            // checks for enabling marriage proposals
                            if (((!String.IsNullOrWhiteSpace(charToDisplay.spouse)) || (charToDisplay.isMale)) || (!String.IsNullOrWhiteSpace(charToDisplay.fiancee)))
                            {
                                // disable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = false;
                                this.meetingPlaceProposeTextBox.Text = "";
                                this.meetingPlaceProposeTextBox.Enabled = false;
                            }
                            else
                            {
                                // enable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = true;
                                this.meetingPlaceProposeTextBox.Text = Globals_Client.myPlayerCharacter.charID;
                                this.meetingPlaceProposeTextBox.Enabled = true;
                            }
                        }
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(charToDisplay);
                this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
                this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Responds to the click event of any hireNPC button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void hireNPC_Btn_Click(object sender, EventArgs e)
        {
            bool amHiring = false;
            bool isHired = false;

            // get hireNPC_Btn tag (shows which meeting place are in)
            string place = Convert.ToString(((Button)sender).Tag);

            // if selected NPC is not a current employee
            if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.charToView))
            {
                amHiring = true;

                try
                {
                    // get offer amount
                    UInt32 newOffer = Convert.ToUInt32(this.hireNPC_TextBox.Text);
                    // submit offer
                    isHired = Globals_Client.myPlayerCharacter.processEmployOffer((Globals_Client.charToView as NonPlayerCharacter), newOffer);

                }
                catch (System.FormatException fe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                catch (System.OverflowException ofe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                    }
                }

            }

            // if selected NPC is already an employee
            else
            {
                // fire NPC
                Globals_Client.myPlayerCharacter.fireNPC(Globals_Client.charToView as NonPlayerCharacter);
            }

            // refresh appropriate screen
            // if firing an NPC
            if (!amHiring)
            {
                if (place.Equals("house"))
                {
                    this.refreshHouseholdDisplay();
                }
                else
                {
                    this.refreshMeetingPlaceDisplay(place, Globals_Client.charToView);
                }
            }
            // if hiring an NPC
            else
            {
                // if in the tavern and NPC is hired, refresh whole screen (NPC removed from list)
                if ((isHired) && (place.Equals("tavern")))
                {
                    this.refreshMeetingPlaceDisplay(place);
                }
                else
                {
                    this.refreshMeetingPlaceDisplay(place, Globals_Client.charToView);
                    //this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(Globals_Client.charToView);
                }
            }

        }

        /// <summary>
        /// Responds to the click event of any entourage button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void entourageBtn_Click(object sender, EventArgs e)
        {
            // for messages
            string toDisplay = "";

            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the command occurred
            string whichScreen = button.Tag.ToString();

            // check which action to perform
            // if is in entourage, remove
            if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
            {
                Globals_Client.myPlayerCharacter.removeFromEntourage((Globals_Client.charToView as NonPlayerCharacter));
            }

            // if is not in entourage, add
            else
            {
                // check to see if NPC is army leader
                // if not leader, proceed
                if (String.IsNullOrWhiteSpace(Globals_Client.charToView.armyID))
                {
                    // add to entourage
                    Globals_Client.myPlayerCharacter.addToEntourage((Globals_Client.charToView as NonPlayerCharacter));

                }

                // if is army leader, can't add to entourage
                else
                {
                    toDisplay += "Sorry, milord, this person is an army leader\r\n";
                    toDisplay += "and, therefore, cannot be added to your entourage.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
            }

            // refresh appropriate screen
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                this.refreshMeetingPlaceDisplay(whichScreen); ;
            }
            else if (whichScreen.Equals("house"))
            {
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
        }

        // ------------------- MARRIAGE

        /// <summary>
        /// Responds to the click event of the meetingPlaceProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceProposeBtn_Click(object sender, EventArgs e)
        {
            // get entry
            if (this.meetingPlaceCharsListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                Character bride = null;
                Character groom = null;
                string brideID = "";
                string groomID = "";

                if (String.IsNullOrWhiteSpace(this.meetingPlaceProposeTextBox.Text))
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                    }
                }
                else
                {
                    // get bride and groom IDs
                    brideID = this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text;
                    groomID = this.houseProposeGroomTextBox.Text;

                    // get bride
                    if (Globals_Game.npcMasterList.ContainsKey(brideID))
                    {
                        bride = Globals_Game.npcMasterList[brideID];
                    }

                    if (bride == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                        }
                    }
                    else
                    {
                        // get groom
                        if (Globals_Game.npcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.npcMasterList[groomID];
                        }
                        else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.pcMasterList[groomID];
                        }

                        if (groom == null)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                            }
                        }
                        else
                        {
                            // carry out conditional checks
                            proceed = this.checksBeforeProposal(bride, groom);

                            // if checks OK, process proposal
                            if (proceed)
                            {
                                this.proposeMarriage(bride, groom);
                            }
                        }

                    }
                }
            }

            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a prospective bride.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of either of the proposal reply buttons,
        /// sending the appropriate reply
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalProposalReplyButton_Click(object sender, EventArgs e)
        {
            if (this.journalListView.SelectedItems.Count > 0)
            {
                bool proposalAccepted = false;

                // get tag from button
                Button button = sender as Button;
                string reply = button.Tag.ToString();

                // set appropriate response
                if (reply.Equals("accept"))
                {
                    proposalAccepted = true;
                }

                // get JournalEntry
                JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;

                // send reply
                this.replyToProposal(thisJentry, proposalAccepted);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No journal entry selected.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the houseProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseProposeBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;

            Character bride = null;
            Character groom = null;
            string brideID = "";
            string groomID = "";

            if (String.IsNullOrWhiteSpace(this.houseProposeBrideTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                }
            }
            else if (String.IsNullOrWhiteSpace(this.houseProposeGroomTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                }
            }
            else
            {
                // get bride and groom IDs
                brideID = this.houseProposeBrideTextBox.Text;
                groomID = this.houseProposeGroomTextBox.Text;

                // get bride
                if (Globals_Game.npcMasterList.ContainsKey(brideID))
                {
                    bride = Globals_Game.npcMasterList[brideID];
                }

                if (bride == null)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                    }
                }
                else
                {
                    // get groom
                    if (Globals_Game.npcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.npcMasterList[groomID];
                    }
                    else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.pcMasterList[groomID];
                    }

                    if (groom == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                        }
                    }
                    else
                    {
                        // carry out conditional checks
                        proceed = this.checksBeforeProposal(bride, groom);

                        // if checks OK, process proposal
                        if (proceed)
                        {
                            this.proposeMarriage(bride, groom);

                            // refresh screen
                            this.refreshCurrentScreen();
                        }
                    }

                }
            }

        }

        // ------------------- HOUSEHOLD MANAGEMENT

        /// <summary>
        /// Responds to the click event of the houseExamineArmiesBtn button
        /// displaying a list of all armies in the current NPC's fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // NPC
            Character thisObserver = Globals_Client.charToView;

            // examine armies
            this.examineArmiesInFief(thisObserver);
        }

        /// <summary>
        /// Responds to the click event of the familyNameChildButton
        /// allowing the player to name the selected child
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNameChildButton_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get NPC to name
                NonPlayerCharacter child = null;
                if (Globals_Game.npcMasterList.ContainsKey(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                {
                    child = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];
                }

                if (child != null)
                {
                    if (Regex.IsMatch(this.familyNameChildTextBox.Text.Trim(), @"^[a-zA-Z- ]+$"))
                    {
                        child.firstName = this.familyNameChildTextBox.Text;
                        this.refreshHouseholdDisplay(child);
                    }
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("'" + this.familyNameChildTextBox.Text + "' is an unsuitable name, milord.");
                        }
                    }
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not retrieve details of NonPlayerCharacter.");
                    }
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a character from the list.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the houseHeirBtn button
        /// allowing the switch to another player (for testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseHeirBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];

                // check for suitability
                bool isSuitable = selectedNPC.checksForHeir(Globals_Client.myPlayerCharacter);

                if (isSuitable)
                {
                    // check for an existing heir and remove
                    foreach (NonPlayerCharacter npc in Globals_Client.myPlayerCharacter.myNPCs)
                    {
                        if (npc.isHeir)
                        {
                            npc.isHeir = false;
                        }
                    }

                    // appoint NPC as heir
                    selectedNPC.isHeir = true;

                    // refresh the household screen (in the main form)
                    this.refreshHouseholdDisplay(selectedNPC);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the dealWithHouseholdAffairsToolStripMenuItem
        /// which causes the Household screen to display, listing the player's
        /// family and employed NPCs
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void dealWithHouseholdAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.charToView = null;

            // refresh household affairs screen 
            this.refreshHouseholdDisplay();

            // display household affairs screen
            Globals_Client.containerToView = this.houseContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the houseCharListView object,
        /// invoking the displayCharacter method, passing a Character to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCharListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in employees
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                if (this.houseCharListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.myPlayerCharacter.myNPCs[i];
                        break;
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                this.houseCharTextBox.Text = this.displayCharacter(charToDisplay);
                this.houseCharTextBox.ReadOnly = true;

                // see if is in entourage to set text of entourage button
                if ((charToDisplay as NonPlayerCharacter).inEntourage)
                {
                    this.houseEntourageBtn.Text = "Remove From Entourage";
                }
                else
                {
                    this.houseEntourageBtn.Text = "Add To Entourage";
                }

                // set text for 'enter/exit keep' button, depending on whether player in/out of keep
                if (Globals_Client.charToView.inKeep)
                {
                    this.houseEnterKeepBtn.Text = "Exit Keep";
                }
                else
                {
                    this.houseEnterKeepBtn.Text = "Enter Keep";
                }

                // FAMILY MATTERS CONTROLS
                // if family selected, enable 'choose heir' button, disbale 'fire' button
                if ((!String.IsNullOrWhiteSpace(Globals_Client.charToView.familyID)) && (Globals_Client.charToView.familyID.Equals(Globals_Client.myPlayerCharacter.charID)))
                {
                    this.houseHeirBtn.Enabled = true;
                    this.houseFireBtn.Enabled = false;

                    // if is male and married, enable NPC 'get wife with child' control
                    if ((Globals_Client.charToView.isMale) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.spouse)))
                    {
                        this.familyNpcSpousePregBtn.Enabled = true;
                    }
                    else
                    {
                        this.familyNpcSpousePregBtn.Enabled = false;
                    }
                }
                else
                {
                    this.houseHeirBtn.Enabled = false;
                    this.houseFireBtn.Enabled = true;
                    this.familyNpcSpousePregBtn.Enabled = false;
                }

                // if character firstname = "Baby", enable 'name child' controls
                if ((charToDisplay as NonPlayerCharacter).firstName.Equals("Baby"))
                {
                    this.familyNameChildButton.Enabled = true;
                    this.familyNameChildTextBox.Enabled = true;
                }
                // if not, ensure are disabled
                else
                {
                    this.familyNameChildButton.Enabled = false;
                    this.familyNameChildTextBox.Enabled = false;
                }

                // 'get wife with child' button always enabled
                this.familyGetSpousePregBtn.Enabled = true;

                // SIEGE CHECKS
                // check to see if is inside besieged keep
                if ((Globals_Client.charToView.inKeep) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.location.siege)))
                {
                    // if is inside besieged keep, disable most of controls
                    this.houseCampBtn.Enabled = false;
                    this.houseCampDaysTextBox.Enabled = false;
                    this.houseMoveToBtn.Enabled = false;
                    this.houseMoveToTextBox.Enabled = false;
                    this.houseRouteBtn.Enabled = false;
                    this.houseEntourageBtn.Enabled = false;
                    this.houseFireBtn.Enabled = false;
                    this.houseExamineArmiesBtn.Enabled = false;
                }

                // is NOT inside besieged keep
                else
                {
                    // re-enable controls
                    this.houseCampBtn.Enabled = true;
                    this.houseCampDaysTextBox.Enabled = true;
                    this.houseMoveToBtn.Enabled = true;
                    this.houseMoveToTextBox.Enabled = true;
                    this.houseRouteBtn.Enabled = true;
                    this.houseRouteTextBox.Enabled = true;
                    this.houseEntourageBtn.Enabled = true;
                    this.houseExamineArmiesBtn.Enabled = true;

                }

            }
        }

        /// <summary>
        /// Responds to the click event of the houseCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCampBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get days to camp
                byte campDays = Convert.ToByte(this.houseCampDaysTextBox.Text);

                // camp
                this.campWaitHere(Globals_Client.charToView, campDays);
            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
            finally
            {
                // refresh display
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }

        }

        // ------------------- FIEF MANAGEMENT

        /// <summary>
        /// Responds to the click event of the viewMyHomeFiefToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewMyHomeFiefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get home fief
            Fief homeFief = Globals_Client.myPlayerCharacter.getHomeFief();

            if (homeFief != null)
            {
                // display home fief
                this.refreshFiefContainer(homeFief);
            }

            // if have no home fief
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You have no home fief!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'Max' buttons inn the fief management screen,
        /// filling in the maximum expenditure for the selected field
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void maxSpendButton_Click(object sender, EventArgs e)
        {
            uint maxSpend = 0;

            // get tag from button
            Button button = sender as Button;
            string expType = button.Tag.ToString();

            // get max spend of specified type
            maxSpend = Globals_Client.fiefToView.getMaxSpend(expType);

            if (maxSpend != 0)
            {
                switch (expType)
                {
                    case "garrison":
                        this.adjGarrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "infrastructure":
                        this.adjInfrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "keep":
                        this.adjustKeepSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "officials":
                        this.adjOffSpendTextBox.Text = maxSpend.ToString();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the fiefGrantTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefGrantTitleBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseTitleHolder = new SelectionForm(this, "titleHolder");
            chooseTitleHolder.Show();
        }

        /// <summary>
        /// Responds to the click event of the fiefTransferFundsPlayerBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefTransferFundsPlayerBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm transferFunds = new SelectionForm(this, "transferFunds");
            transferFunds.Show();
        }

        /// <summary>
        /// Responds to the click event of the selfBailiffBtn button,
        /// appointing the player as bailiff of the displayed fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void selfBailiffBtn_Click(object sender, EventArgs e)
        {
            // give player fair warning of bailiff commitments
            DialogResult dialogResult = MessageBox.Show("Being a bailiff will restrict your movement (as you need to remain in the fief for 30 days to have any effect).  Click 'OK' to proceed.", "Proceed with appointment?", MessageBoxButtons.OKCancel);

            // if choose to cancel
            if (dialogResult == DialogResult.Cancel)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Appointment cancelled.");
                }
            }

            // if choose to proceed
            else
            {
                // if the fief has an existing bailiff
                if (Globals_Client.fiefToView.bailiff != null)
                {
                    // relieve him of his duties
                    Globals_Client.fiefToView.bailiff = null;
                }

                // set player as bailiff
                Globals_Client.fiefToView.bailiff = Globals_Client.myPlayerCharacter;
            }

            // refresh fief display
            this.refreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the click event of any of the 'transfer funds' buttons
        /// allowing players to transfer funds between treasuries
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get button
                Button button = sender as Button;
                // get transfer parameters from tag
                string transferType = button.Tag.ToString();

                Fief fiefFrom = null;
                Fief fiefTo = null;
                int amount = 0;

                switch (transferType)
                {
                    case "toFief":
                        fiefFrom = Globals_Client.myPlayerCharacter.getHomeFief();
                        fiefTo = Globals_Client.fiefToView;
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    case "toHome":
                        fiefFrom = Globals_Client.fiefToView;
                        fiefTo = Globals_Client.myPlayerCharacter.getHomeFief();
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    default:
                        break;
                }

                if (((fiefFrom != null) && (fiefTo != null)) && (amount > 0))
                {
                    // make sure are enough funds to cover transfer
                    if (amount > fiefFrom.getAvailableTreasury(true))
                    {
                        // if not, inform player and adjust amount downwards
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Too few funds available for this transfer.");
                        }
                    }

                    else
                    {
                        // make the transfer
                        this.treasuryTransfer(fiefFrom, fiefTo, amount);
                    }

                }

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the viewBailiffBtn button
        /// which refreshes and displays the character screen, showing details of the
        /// bailiff for the selected fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewBailiffBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // if player is bailiff, show in personal characteristics screen
                if (Globals_Client.fiefToView.bailiff == Globals_Client.myPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.myPlayerCharacter;
                    this.refreshCharacterContainer(Globals_Client.charToView);
                }

                // if NPC is bailiff, show in household affairs screen
                else if (Globals_Client.fiefToView.bailiff is NonPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.fiefToView.bailiff;
                    // refresh household affairs screen 
                    this.refreshHouseholdDisplay(Globals_Client.charToView as NonPlayerCharacter);
                    // display household affairs screen
                    Globals_Client.containerToView = this.houseContainer;
                    Globals_Client.containerToView.BringToFront();
                }
            }

            // display message that is no bailiff
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This fief currently has no bailiff.");
                }
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the fiefsListView object,
        /// invoking the displayFief method, passing a Fief to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // enable controls
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                this.fiefsChallengeBtn.Enabled = true;
                this.fiefsViewBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the setBailiffBtn button
        ///invoking and displaying the character selection screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void setBailiffBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseBailiff = new SelectionForm(this, "bailiff");
            chooseBailiff.Show();
        }

        /// <summary>
        /// Responds to the click event of the lockoutBtn button
        /// invoking and displaying the lockout screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void lockoutBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm lockOutOptions = new SelectionForm(this, "lockout");
            lockOutOptions.Show();
        }

        /// <summary>
        /// Responds to the click event of the removeBaliffBtn button,
        /// relieving the current bailiff of his duties
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void removeBaliffBtn_Click(object sender, EventArgs e)
        {
            // if the fief has an existing bailiff
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // relieve him of his duties
                Globals_Client.fiefToView.bailiff = null;

                // refresh fief display
                this.refreshFiefContainer(Globals_Client.fiefToView);
            }
        }

        /// <summary>
        /// Responds to the click event of the adjustSpendBtn button
        /// which commits the expenditures and tax rate for the coming year
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adjustSpendBtn_Click(object sender, EventArgs e)
        {
            // keep track of whether any spends ahve changed
            bool spendChanged = false;

            try
            {
                // get new amounts
                Double newTax = Convert.ToDouble(this.adjustTaxTextBox.Text);
                UInt32 newOff = Convert.ToUInt32(this.adjOffSpendTextBox.Text);
                UInt32 newGarr = Convert.ToUInt32(this.adjGarrSpendTextBox.Text);
                UInt32 newInfra = Convert.ToUInt32(this.adjInfrSpendTextBox.Text);
                UInt32 newKeep = Convert.ToUInt32(this.adjustKeepSpendTextBox.Text);

                // process adjustments
                Globals_Client.fiefToView.adjustExpenditures(newTax, newOff, newGarr, newInfra, newKeep);
                spendChanged = true;
            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
            finally
            {
                // refresh screen if expenditure changed
                if (spendChanged)
                {
                    // refresh display
                    this.refreshCurrentScreen();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the fiefAutoAdjustBtn button,
        /// displaying the armyManagementPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefAutoAdjustBtn_Click(object sender, EventArgs e)
        {
            int overspend = 0;

            // calculate overspend
            overspend = (Globals_Client.fiefToView.calcNewExpenses() - Globals_Client.fiefToView.getAvailableTreasury());

            // auto-adjust expenditure
            Globals_Client.fiefToView.autoAdjustExpenditure(Convert.ToUInt32(overspend));

            // refresh screen
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Responds to the click event of myFiefsToolStripMenuItem
        /// which refreshes and displays the owned fiefs screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void myFiefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.fiefToView = null;
            this.refreshMyFiefs();
            Globals_Client.containerToView = this.fiefsOwnedContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the click event of the fiefManagementToolStripMenuItem
        /// which displays main Fief information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshFiefContainer(Globals_Client.myPlayerCharacter.location);
        }

        /// <summary>
        /// Responds to the Click event of the fiefsChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get province
                Province targetProv = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.fiefsListView.SelectedItems[0].SubItems[5].Text))
                {
                    targetProv = Globals_Game.provinceMasterList[this.fiefsListView.SelectedItems[0].SubItems[5].Text];
                }

                if (targetProv != null)
                {
                    targetProv.lodgeOwnershipChallenge();
                }

                this.fiefsListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the Click event of the fiefsViewBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsViewBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get fief to view
                Globals_Client.fiefToView = Globals_Game.fiefMasterList[this.fiefsListView.SelectedItems[0].SubItems[1].Text];

                // go to fief display screen
                this.refreshFiefContainer(Globals_Client.fiefToView);
                Globals_Client.containerToView = this.fiefContainer;
                Globals_Client.containerToView.BringToFront();
            }
        }

        // ------------------- CHARACTER DISPLAY

        /// <summary>
        /// Responds to the click event of the personalCharacteristicsAndAffairsToolStripMenuItem
        /// which displays main Character information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void personalCharacteristicsAndAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;
            this.refreshCharacterContainer(Globals_Client.charToView);
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the characterTitlesCheckBox,
        /// displaying the player's titles/ranks
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void characterTitlesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.refreshCharacterContainer(Globals_Client.charToView);
        }
        
        // ------------------- CHILDBIRTH

        /// <summary>
        /// Responds to the click event of the familyNpcSpousePregBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNpcSpousePregBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get spouse
                Character mySpouse = Globals_Client.charToView.getSpouse();

                // perform standard checks
                if (this.checksBeforePregnancyAttempt(Globals_Client.charToView))
                {
                    // ensure are both in/out of keep
                    mySpouse.inKeep = Globals_Client.charToView.inKeep;

                    // attempt pregnancy
                    bool pregnant = Globals_Client.charToView.getSpousePregnant(mySpouse);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No character selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the familyGetSpousePregBt button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyGetSpousePregBtn_Click(object sender, EventArgs e)
        {
            // perform standard checks
            if (this.checksBeforePregnancyAttempt(Globals_Client.myPlayerCharacter))
            {
                // get spouse
                Character mySpouse = Globals_Client.myPlayerCharacter.getSpouse();

                // ensure are both in/out of keep
                mySpouse.inKeep = Globals_Client.myPlayerCharacter.inKeep;

                // attempt pregnancy
                bool pregnant = Globals_Client.myPlayerCharacter.getSpousePregnant(mySpouse);
            }

            // refresh screen
            this.refreshCurrentScreen();
        }

        // ------------------- JOURNAL

        /// <summary>
        /// Responds to the click event of the viewEntriesToolStripMenuItem
        /// displaying the journal screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get ToolStripMenuItem
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            // get entry scope from tag
            string entryScope = menuItem.Tag.ToString();

            // get and display entries
            this.viewJournalEntries(entryScope);
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the journalListView
        /// displaying the selected journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get entry
            if (this.journalListView.SelectedItems.Count > 0)
            {
                uint jEntryID = Convert.ToUInt32(this.journalListView.SelectedItems[0].SubItems[0].Text);
                Globals_Client.jEntryToView = Globals_Client.eventSetToView.IndexOfKey(jEntryID);

                // retrieve and display character information
                this.journalTextBox.Text = this.displayJournalEntry(Globals_Client.jEntryToView);

                // check if marriage proposal controls should be enabled
                JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;
                bool enableProposalControls = thisJentry.checkForProposalControlsEnabled();
                this.journalProposalAcceptBtn.Enabled = enableProposalControls;
                this.journalProposalRejectBtn.Enabled = enableProposalControls;

                // mark entry as viewed
                Globals_Client.myPastEvents.entries[jEntryID].viewed = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the journalPrevBtn button
        /// selecting and displaying the previous journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalPrevBtn_Click(object sender, EventArgs e)
        {
            // check if at beginning of index
            if (Globals_Client.jEntryToView == 0)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are no entries prior to this one.");
                }
            }

            else
            {
                // amend index position
                Globals_Client.jEntryToView--;
            }

            // refresh journal screen
            this.refreshJournalContainer(Globals_Client.jEntryToView);
        }

        /// <summary>
        /// Responds to the click event of the journalNextBtn button
        /// selecting and displaying the next journal entry
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalNextBtn_Click(object sender, EventArgs e)
        {
            // check if at beginning of index
            if (Globals_Client.jEntryToView == Globals_Client.jEntryMax)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are no entries after this one.");
                }
            }

            else
            {
                // amend index position
                Globals_Client.jEntryToView++;
            }

            // refresh journal screen
            this.refreshJournalContainer(Globals_Client.jEntryToView);
        }

        /// <summary>
        /// Responds to the Click event of the journalToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get and tally unread entries
            Globals_Client.eventSetToView = Globals_Client.myPastEvents.getUnviewedEntries();
            int unreadEntries = Globals_Client.eventSetToView.Count;

            // indicate no. of unread entries in menu item text
            this.viewMyEntriesunreadToolStripMenuItem.Text = "View UNREAD entries (" + unreadEntries + ")";
        }

        // ------------------- ARMY MANAGEMENT

        /// <summary>
        /// Responds to the click event of the armyRecruitBtn button, allowing the player 
        /// to create a new army and/or recruit additional troops in the current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyRecruitBtn_Click(object sender, EventArgs e)
        {
            bool armyExists = true;

            // get tag from button
            Button button = sender as Button;
            string operation = button.Tag.ToString();

            if (operation.Equals("new"))
            {
                armyExists = false;
            }

            try
            {
                // get number of troops specified
                UInt32 numberWanted = Convert.ToUInt32(this.armyRecruitTextBox.Text);

                // recruit troops
                Globals_Client.myPlayerCharacter.recruitTroops(numberWanted, armyExists);

                // get army
                Army myArmy = Globals_Client.myPlayerCharacter.getArmy();

                // refresh display
                this.refreshArmyContainer(myArmy);

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the armyMaintainBtn button
        /// allowing the player to maintain the army in the field
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyMaintainBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                // maintain army
                Globals_Client.armyToView.mantainArmy();

                // refresh display
                this.refreshArmyContainer(Globals_Client.armyToView);
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the armyListView object,
        /// invoking the displayArmyData method and passing an Army to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // get army to view
            if (this.armyListView.SelectedItems.Count > 0)
            {
                Globals_Client.armyToView = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];
            }

            if (Globals_Client.armyToView != null)
            {
                // display data for selected army
                this.armyTextBox.Text = this.displayArmyData(Globals_Client.armyToView);

                // check if is defender in a siege
                string siegeID = Globals_Client.armyToView.checkIfSiegeDefenderGarrison();
                if (String.IsNullOrWhiteSpace(siegeID))
                {
                    siegeID = Globals_Client.armyToView.checkIfSiegeDefenderAdditional();
                }

                // if is defender in a siege, disable controls
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    this.disableControls(this.armyManagementPanel);
                    this.disableControls(this.armyCombatPanel);

                    // always enable switch between management and combat panels
                    this.armyDisplayCmbtBtn.Enabled = true;
                    this.armyDisplayMgtBtn.Enabled = true;
                }

                // if isn't defender in a siege, enable controls
                else
                {
                    // recruit controls
                    // if player is leading an army but not the one on view, disable 'recruit' button
                    if ((!String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                        && (!(Globals_Client.armyToView.leader.Equals(Globals_Client.myPlayerCharacter.charID))))
                    {
                        this.armyRecruitBtn.Enabled = false;
                        this.armyRecruitTextBox.Enabled = false;
                    }
                    // otherwise, enable 'recruit' button
                    else
                    {
                        this.armyRecruitBtn.Enabled = true;
                        this.armyRecruitTextBox.Enabled = true;

                        // if army on view is led by player, set button text to 'recruit additional'
                        if (Globals_Client.armyToView.leader == Globals_Client.myPlayerCharacter.charID)
                        {
                            this.armyRecruitBtn.Text = "Recruit Additional Troops From Current Fief";
                            this.armyRecruitBtn.Tag = "add";
                        }
                        // if player is not leading any armies, set button text to 'recruit new'
                        else if (String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.armyID))
                        {
                            this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                            this.armyRecruitBtn.Tag = "new";
                        }
                    }

                    // if has no leader
                    if (String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                    {
                        // set army aggression to 0
                        if (Globals_Client.armyToView.aggression > 0)
                        {
                            Globals_Client.armyToView.aggression = 0;
                        }

                        // disable 'proactive' army functions
                        this.armyExamineBtn.Enabled = false;
                        this.armyPillageBtn.Enabled = false;
                        this.armySiegeBtn.Enabled = false;
                        this.armyAutoCombatBtn.Enabled = false;
                        this.armyAggroTextBox.Enabled = false;
                        this.armyOddsTextBox.Enabled = false;
                        this.armyTransDropBtn.Enabled = false;
                        this.armyTransKnightTextBox.Enabled = false;
                        this.armyTransMAAtextBox.Enabled = false;
                        this.armyTransLCavTextBox.Enabled = false;
                        this.armyTransLongbowTextBox.Enabled = false;
                        this.armyTransCrossbowTextBox.Enabled = false;
                        this.armyTransFootTextBox.Enabled = false;
                        this.armyTransRabbleTextBox.Enabled = false;
                        this.armyTransDropWhoTextBox.Enabled = false;
                        this.armyTransPickupBtn.Enabled = false;
                    }

                    // has leader
                    else
                    {
                        this.armyExamineBtn.Enabled = true;
                        this.armyPillageBtn.Enabled = true;
                        this.armySiegeBtn.Enabled = true;
                        this.armyAutoCombatBtn.Enabled = true;
                        this.armyAggroTextBox.Enabled = true;
                        this.armyOddsTextBox.Enabled = true;
                        this.armyTransDropBtn.Enabled = true;
                        this.armyTransKnightTextBox.Enabled = true;
                        this.armyTransMAAtextBox.Enabled = true;
                        this.armyTransLCavTextBox.Enabled = true;
                        this.armyTransLongbowTextBox.Enabled = true;
                        this.armyTransCrossbowTextBox.Enabled = true;
                        this.armyTransFootTextBox.Enabled = true;
                        this.armyTransRabbleTextBox.Enabled = true;
                        this.armyTransDropWhoTextBox.Enabled = true;
                        this.armyTransPickupBtn.Enabled = true;
                    }

                    // other controls
                    this.armyMaintainBtn.Enabled = true;
                    this.armyAppointLeaderBtn.Enabled = true;
                    this.armyAppointSelfBtn.Enabled = true;
                    this.armyDisbandBtn.Enabled = true;
                    this.armyCampBtn.Enabled = true;
                    this.armyCampTextBox.Enabled = true;

                    // check to see if current fief is in rebellion and enable control as appropriate
                    // get fief
                    Fief thisFief = Globals_Client.armyToView.getLocation();
                    if (thisFief.status.Equals('R'))
                    {
                        // only enable if army has leader
                        if (!String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                        {
                            this.armyQuellRebellionBtn.Enabled = true;
                        }
                    }
                    else
                    {
                        this.armyQuellRebellionBtn.Enabled = false;
                    }

                    // set auto combat values
                    this.armyAggroTextBox.Text = Globals_Client.armyToView.aggression.ToString();
                    this.armyOddsTextBox.Text = Globals_Client.armyToView.combatOdds.ToString();

                    // preload own ID in 'drop off to' textbox (assumes transferring between own armies)
                    this.armyTransDropWhoTextBox.Text = Globals_Client.myPlayerCharacter.charID;
                    // and set all troop transfer numbers to 0
                    this.armyTransKnightTextBox.Text = "0";
                    this.armyTransMAAtextBox.Text = "0";
                    this.armyTransLCavTextBox.Text = "0";
                    this.armyTransLongbowTextBox.Text = "0";
                    this.armyTransCrossbowTextBox.Text = "0";
                    this.armyTransFootTextBox.Text = "0";
                    this.armyTransRabbleTextBox.Text = "0";
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the listMToolStripMenuItem
        /// displaying the army management screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void listMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "management";
            Globals_Client.armyToView = null;
            this.refreshArmyContainer();
        }

        /// <summary>
        /// Responds to the click event of the armyAppointLeaderBtn button
        /// invoking and displaying the character selection screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAppointLeaderBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            string thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseLeader = new SelectionForm(this, "leader", armID: thisArmyID);
                chooseLeader.Show();
            }

            // if no army selected
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the armyAppointSelfBtn button
        /// allowing the player to appoint themselves as army leader
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAppointSelfBtn_Click(object sender, EventArgs e)
        {
            // get army
            Army thisArmy = Globals_Game.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

            thisArmy.assignNewLeader(Globals_Client.myPlayerCharacter);

            // refresh the army information (in the main form)
            this.refreshArmyContainer(thisArmy);
        }

        /// <summary>
        /// Responds to the click event of the armyTransDropBtn button
        /// allowing the player to leave troops in the fief for transfer to other armies
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyTransDropBtn_Click(object sender, EventArgs e)
        {
            bool success = true;
            string toDisplay = "";

            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get number of troops to transfer
                    uint[] troopsToTransfer = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
                    troopsToTransfer[0] = Convert.ToUInt32(this.armyTransKnightTextBox.Text);
                    troopsToTransfer[1] = Convert.ToUInt32(this.armyTransMAAtextBox.Text);
                    troopsToTransfer[2] = Convert.ToUInt32(this.armyTransLCavTextBox.Text);
                    troopsToTransfer[3] = Convert.ToUInt32(this.armyTransLongbowTextBox.Text);
                    troopsToTransfer[4] = Convert.ToUInt32(this.armyTransCrossbowTextBox.Text);
                    troopsToTransfer[5] = Convert.ToUInt32(this.armyTransFootTextBox.Text);
                    troopsToTransfer[6] = Convert.ToUInt32(this.armyTransRabbleTextBox.Text);

                    // create detachment details
                    string[] detachmentDetails = new string[] {troopsToTransfer[0].ToString(), troopsToTransfer[1].ToString(),
                        troopsToTransfer[2].ToString(), troopsToTransfer[3].ToString(), troopsToTransfer[4].ToString(),
                        troopsToTransfer[5].ToString(), troopsToTransfer[6].ToString(), this.armyTransDropWhoTextBox.Text };

                    // create detachment and leave in fief
                    success = Globals_Client.armyToView.createDetachment(detachmentDetails);

                    // inform player
                    if (!success)
                    {
                        if (Globals_Client.showMessages)
                        {
                            toDisplay = "An error occurred that prevented the transfer.";
                            System.Windows.Forms.MessageBox.Show(toDisplay, "ERROR DETECTED");
                        }
                    }
                    else
                    {
                        toDisplay = "Transfer successful.";
                        System.Windows.Forms.MessageBox.Show(toDisplay, "OPERATION SUCCESSFUL");
                    }

                }

                catch (System.FormatException fe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                catch (System.OverflowException ofe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                finally
                {
                    this.refreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyTransPickupBtn button, invoking and displaying the
        /// transfer selection screen, allowing detachments in the current fief to be added to the army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyTransPickupBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            string thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseTroops = new SelectionForm(this, "transferTroops", armID: thisArmyID);
                chooseTroops.Show();
            }

            // if no army selected
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the armyManagementToolStripMenuItem
        /// which displays main Army information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get player's army
            Army thisArmy = Globals_Client.myPlayerCharacter.getArmy();

            // display army mangement screen
            this.armyContainer.Panel1.Tag = "management";
            this.refreshArmyContainer(thisArmy);
        }

        /// <summary>
        /// Responds to the click event of the armyDisbandBtn button, disbanding the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisbandBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                // disband army
                this.disbandArmy(Globals_Client.armyToView);

                // refresh display
                this.refreshArmyContainer();
            }
        }

        /// <summary>
        /// Responds to the click event of the armyAutoCombatBtn button, setting the army's
        /// aggression and combat odds values to those in the text fields
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyAutoCombatBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get new aggression level
                    byte newAggroLevel = Convert.ToByte(this.armyAggroTextBox.Text);

                    // get new combat odds value
                    byte newOddsValue = Convert.ToByte(this.armyOddsTextBox.Text);

                    // check and adjust values
                    bool success = Globals_Client.armyToView.adjustAutoLevels(newAggroLevel, newOddsValue);

                    // inform player
                    string toDisplay = "";
                    string msgTitle = "";
                    if (success)
                    {
                        toDisplay = "Auto-combat values successfully adjusted.";
                        msgTitle = "OPERATION SUCCESSFUL";
                    }
                    else
                    {
                        toDisplay = "I'm afraid there has been a problem adjusting the auto-combat values, my lord.";
                        msgTitle = "ERROR OCCURRED";
                    }
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay, msgTitle);
                    }
                }

                catch (System.FormatException fe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                catch (System.OverflowException ofe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                finally
                {
                    // refresh display
                    this.refreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyCampBtn button
        /// invoking the campWaitHere method for the army leader (and army)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyCampBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.armyToView != null)
            {
                try
                {
                    // get days to camp
                    byte campDays = Convert.ToByte(this.armyCampTextBox.Text);

                    // get leader
                    Character thisLeader = Globals_Client.armyToView.getLeader();

                    // camp
                    if (thisLeader != null)
                    {
                        this.campWaitHere(thisLeader, campDays);
                    }

                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot conduct this operation without a leader.", "OPERATION CANCELLED");
                        }
                    }
                }

                catch (System.FormatException fe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                catch (System.OverflowException ofe)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                    }
                }
                finally
                {
                    // refresh display
                    this.refreshArmyContainer(Globals_Client.armyToView);
                }

            }

        }

        /// <summary>
        /// Responds to the click event of the armyExamineBtn button
        /// displaying a list of all armies in the current army's fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyExamineBtn_Click(object sender, EventArgs e)
        {
            // army leader
            Character thisLeader = Globals_Client.armyToView.getLeader();

            // examine armies
            this.examineArmiesInFief(thisLeader);
        }

        /// <summary>
        /// Responds to the click event of the armyDisplayMgtBtn button,
        /// displaying the armyCombatPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisplayMgtBtn_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "management";
            this.armyManagementPanel.BringToFront();
            this.armyListView.Focus();
        }

        /// <summary>
        /// Responds to the click event of the armyDisplayCmbtBtn button,
        /// displaying the armyManagementPanel
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyDisplayCmbtBtn_Click(object sender, EventArgs e)
        {
            this.armyContainer.Panel1.Tag = "combat";
            this.armyCombatPanel.BringToFront();
            this.armyListView.Focus();
        }

        /// <summary>
        /// Responds to the click event of the murderThisCharacterToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void murderThisCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get character
            string charID = this.muderCharacterMenuTextBox1.Text;

            if (String.IsNullOrWhiteSpace(charID))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No Character ID entered.  Operation cancelled.");
                }
            }

            Character charToMurder = null;
             if (Globals_Game.pcMasterList.ContainsKey(charID))
            {
                charToMurder = Globals_Game.pcMasterList[charID];
            }
             else if (Globals_Game.npcMasterList.ContainsKey(charID))
            {
                charToMurder = Globals_Game.npcMasterList[charID];
            }

             if (charToMurder != null)
             {
                 charToMurder.processDeath();
             }

             else
             {
                 if (Globals_Client.showMessages)
                 {
                     System.Windows.Forms.MessageBox.Show("Character could not be identified.  Operation cancelled.");
                 }
             }
        }

        /// <summary>
        /// Responds to the click event of the houseEnterKeepBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseEnterKeepBtn_Click(object sender, EventArgs e)
        {            
            // attempt to enter/exit keep
            bool success = Globals_Client.charToView.exitEnterKeep();

            // if successful
            if (success)
            {
                // refresh display
                this.refreshHouseholdDisplay(Globals_Client.charToView as NonPlayerCharacter);
            }
        }

    }

}
