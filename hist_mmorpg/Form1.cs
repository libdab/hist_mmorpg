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
			this.initGameObjects("Char_47", "fromCSV");

			//this.SynchGameObjectCollections ();

			//this.ImportFromCSV("gameObjects.csv", "fromCSV", true);
			//this.CreateMapArrayFromCSV ("map.csv", "fromCSV");

			// this.ArrayFromCSV ("/home/libdab/Dissertation_data/11-07-14/hacked-player.csv", true, "testGame", "skeletonPlayers1194");
        }

        /// <summary>
        /// Starts a new game using the parameters provided
        /// </summary>
        /// <param name="gameID">gameID of the new game</param>
        /// <param name="startYear">Starting year of the new game</param>
        /// <param name="duration">duration (years) of the new game</param>
        /// <param name="gameType">gameType of the new game (sets victory conditions)</param>        
        public void startNewGame(string gameID, uint startYear = 1337, uint duration = 100, uint gameType = 0)
        {
        }

		/// <summary>
        /// Initialises all game objects
		/// </summary>
		/// <param name="pc">ID of PlayerCharacter to set as Globals_Client.myPlayerCharacter</param>
		/// <param name="gameID">gameID of the game</param>
		public void initGameObjects(string pcID, string gameID)
        {

            if (Globals_Game.loadFromDatabase)
			{
				// load objects from database
				this.initialDBload (gameID);
			}
			else
			{
				// create from code
				this.initialLoad ();
			}

            // set myPlayerCharacter
            Globals_Client.myPlayerCharacter = Globals_Game.pcMasterList[pcID];

            // set inital fief to display
            Globals_Client.fiefToView = Globals_Client.myPlayerCharacter.location;

            // initialise list elements in UI
            this.setUpFiefsList();
            this.setUpMeetingPLaceCharsList();
            this.setUpHouseholdCharsList();
            this.setUpArmyList();
            this.setUpSiegeList();
            this.setUpJournalList();
            this.setUpRoyalGiftsLists();
            this.setUpProvinceLists();

            // set player's character to display
            Globals_Client.charToView = Globals_Client.myPlayerCharacter;

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

            // initialise player's character display in UI
            this.refreshCharacterContainer();

        }

        /// <summary>
        /// Creates new game
        /// </summary>
        public void startNewGame()
        {
        }

		/// <summary>
		/// Creates some game objects from code (temporary)
		/// </summary>
		public void initialLoad()
		{
            // create GameClock
            GameClock myGameClock = new GameClock("clock001", 1320);
            Globals_Game.clock = myGameClock;

			// create skills
			// Dictionary of skill effects
            Dictionary<string, double> effectsCommand = new Dictionary<string, double>();
			effectsCommand.Add("battle", 0.4);
            effectsCommand.Add("siege", 0.4);
			effectsCommand.Add("npcHire", 0.2);
			// create skill
			Skill command = new Skill("sk001", "Command", effectsCommand);
			// add to skillsCollection
			Globals_Game.skillMasterList.Add(command.skillID, command);

            Dictionary<string, double> effectsChivalry = new Dictionary<string, double>();
            effectsChivalry.Add("famExpense", 0.2);
			effectsChivalry.Add("fiefExpense", 0.1);
            effectsChivalry.Add("fiefLoy", 0.2);
            effectsChivalry.Add("npcHire", 0.1);
            effectsChivalry.Add("siege", 0.1);
			Skill chivalry = new Skill("sk002", "Chivalry", effectsChivalry);
            Globals_Game.skillMasterList.Add(chivalry.skillID, chivalry);

            Dictionary<string, double> effectsAbrasiveness = new Dictionary<string, double>();
			effectsAbrasiveness.Add("battle", 0.15);
			effectsAbrasiveness.Add("death", 0.05);
            effectsAbrasiveness.Add("fiefExpense", -0.05);
            effectsAbrasiveness.Add("famExpense", 0.05);
            effectsAbrasiveness.Add("time", 0.05);
            effectsAbrasiveness.Add("siege", -0.1);
			Skill abrasiveness = new Skill("sk003", "Abrasiveness", effectsAbrasiveness);
            Globals_Game.skillMasterList.Add(abrasiveness.skillID, abrasiveness);

            Dictionary<string, double> effectsAccountancy = new Dictionary<string, double>();
            effectsAccountancy.Add("time", 0.1);
            effectsAccountancy.Add("fiefExpense", -0.2);
            effectsAccountancy.Add("famExpense", -0.2);
            effectsAccountancy.Add("fiefLoy", -0.05);
			Skill accountancy = new Skill("sk004", "Accountancy", effectsAccountancy);
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
			Skill stupidity = new Skill("sk005", "Stupidity", effectsStupidity);
            Globals_Game.skillMasterList.Add(stupidity.skillID, stupidity);

            Dictionary<string, double> effectsRobust = new Dictionary<string, double>();
            effectsRobust.Add("virility", 0.2);
            effectsRobust.Add("npcHire", 0.05);
            effectsRobust.Add("fiefLoy", 0.05);
            effectsRobust.Add("death", -0.2);
			Skill robust = new Skill("sk006", "Robust", effectsRobust);
            Globals_Game.skillMasterList.Add(robust.skillID, robust);

            Dictionary<string, double> effectsPious = new Dictionary<string, double>();
            effectsPious.Add("virility", -0.2);
            effectsPious.Add("npcHire", 0.1);
            effectsPious.Add("fiefLoy", 0.1);
            effectsPious.Add("time", -0.1);
			Skill pious = new Skill("sk007", "Pious", effectsPious);
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
			Globals_Game.terrainMasterList.Add (plains.terrainCode, plains);
            Terrain hills = new Terrain("terr_H", "Hills", 1.5);
            Globals_Game.terrainMasterList.Add(hills.terrainCode, hills);
            Terrain forrest = new Terrain("terr_F", "Forrest", 1.5);
            Globals_Game.terrainMasterList.Add(forrest.terrainCode, forrest);
            Terrain mountains = new Terrain("terr_M", "Mountains", 15);
            Globals_Game.terrainMasterList.Add(mountains.terrainCode, mountains);
            Terrain impassable_mountains = new Terrain("terr_MX", "Impassable mountains", 91);
            Globals_Game.terrainMasterList.Add(impassable_mountains.terrainCode, impassable_mountains);

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

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'R', c1, plains, fief1Chars, keep1BarChars, false, false, 0, 2000000, armies001, false, transfers001, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief1.id, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 10, 1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'U',c1, hills, fief2Chars, keep2BarChars, false, false, 0, 4000, armies002, false, transfers002, false, r: myRank15);
            Globals_Game.fiefMasterList.Add(myFief2.id, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', c1, plains, fief3Chars, keep3BarChars, false, false, 0, 100000, armies003, false, transfers003, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief3.id, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', c1, plains, fief4Chars, keep4BarChars, false, false, 0, 100000, armies004, false, transfers004, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief4.id, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', f1, plains, fief5Chars, keep5BarChars, false, false, 0, 100000, armies005, false, transfers005, false, r: myRank15);
            Globals_Game.fiefMasterList.Add(myFief5.id, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', f1, plains, fief6Chars, keep6BarChars, false, false, 0, 100000, armies006, false, transfers006, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief6.id, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', f1, forrest, fief7Chars, keep7BarChars, false, false, 0, 100000, armies007, false, transfers007, false, r: myRank15);
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
            Tuple<uint, byte> myDob001 = new Tuple<uint, byte>(1287, 1);
            Tuple<uint, byte> myDob002 = new Tuple<uint, byte>(1260, 0);
            Tuple<uint, byte> myDob003 = new Tuple<uint, byte>(1278, 2);
            Tuple<uint, byte> myDob004 = new Tuple<uint, byte>(1295, 3);
            Tuple<uint, byte> myDob005 = new Tuple<uint, byte>(1293, 2);
            Tuple<uint, byte> myDob006 = new Tuple<uint, byte>(1285, 2);
            Tuple<uint, byte> myDob007 = new Tuple<uint, byte>(1285, 3);
            Tuple<uint, byte> myDob008 = new Tuple<uint, byte>(1307, 2);
            Tuple<uint, byte> myDob009 = new Tuple<uint, byte>(1305, 0);
            Tuple<uint, byte> myDob010 = new Tuple<uint, byte>(1305, 0);
            Tuple<uint, byte> myDob011 = new Tuple<uint, byte>(1302, 1);
            Tuple<uint, byte> myDob012 = new Tuple<uint, byte>(1303, 3);

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
            PlayerCharacter myChar1 = new PlayerCharacter("Char_101", "Dave", "Bond", myDob001, true, nationality02, true, 8.50, 9.0, myGoTo1, c1, 90, 0, 7.2, 6.1, generateSkillSet(), false, false, "Char_101", "Char_403", null, null, false, 13000, myEmployees1, myFiefsOwned1, myProvsOwned1, "ESX02", "ESX02", myTitles001, myArmies001, mySieges001, null, loc: myFief1, pID: "libdab");
            Globals_Game.pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("Char_102", "Bave", "Dond", myDob002, true, nationality01, true, 8.50, 6.0, myGoTo2, f1, 90, 0, 5.0, 4.5, generateSkillSet(), false, false, "Char_102", null, null, null, false, 13000, myEmployees2, myFiefsOwned2, myProvsOwned2, "ESR03", "ESR03", myTitles002, myArmies002, mySieges002, null, loc: myFief7, pID: "otherGuy");
            Globals_Game.pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("Char_401", "Jimmy", "Servant", myDob003, true, nationality02, true, 8.50, 6.0, myGoTo3, c1, 90, 0, 3.3, 6.7, generateSkillSet(), false, false, null, null, null, null, 0, false, false, myTitles003, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("Char_402", "Johnny", "Servant", myDob004, true, nationality02, true, 8.50, 6.0, myGoTo4, c1, 90, 0, 7.1, 5.2, generateSkillSet(), false, false, null, null, null, null, 10000, true, false, myTitles004, null, mb: myChar1.charID, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myNPC3 = new NonPlayerCharacter("Char_403", "Harry", "Bailiff", myDob005, true, nationality01, true, 8.50, 6.0, myGoTo5, c1, 90, 0, 7.1, 5.2, generateSkillSet(), true, false, null, null, null, null, 10000, false, false, myTitles005, null, mb: myChar2.charID, loc: myFief6);
            Globals_Game.npcMasterList.Add(myNPC3.charID, myNPC3);
            NonPlayerCharacter myChar1Wife = new NonPlayerCharacter("Char_404", "Bev", "Bond", myDob006, false, nationality02, true, 2.50, 9.0, myGoTo6, f1, 90, 0, 4.0, 6.0, generateSkillSet(), false, false, "Char_101", "Char_101", null, null, 30000, false, false, myTitles006, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            NonPlayerCharacter myChar2Son = new NonPlayerCharacter("Char_405", "Horatio", "Dond", myDob007, true, nationality01, true, 8.50, 6.0, myGoTo7, f1, 90, 0, 7.1, 5.2, generateSkillSet(), true, false, "Char_102", "Char_406", "Char_102", null, 10000, false, true, myTitles007, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            NonPlayerCharacter myChar2SonWife = new NonPlayerCharacter("Char_406", "Mave", "Dond", myDob008, false, nationality02, true, 2.50, 9.0, myGoTo8, f1, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "Char_102", "Char_405", null, null, 30000, false, false, myTitles008, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);
            NonPlayerCharacter myChar1Son = new NonPlayerCharacter("Char_407", "Rickie", "Bond", myDob009, true, nationality02, true, 2.50, 9.0, myGoTo9, c1, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "Char_101", null, "Char_101", "Char_404", 30000, false, true, myTitles009, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Son.charID, myChar1Son);
            NonPlayerCharacter myChar1Daughter = new NonPlayerCharacter("Char_408", "Elsie", "Bond", myDob010, false, nationality02, true, 2.50, 9.0, myGoTo10, c1, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "Char_101", null, "Char_101", "Char_404", 30000, false, false, myTitles010, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Daughter.charID, myChar1Daughter);
            NonPlayerCharacter myChar2Son2 = new NonPlayerCharacter("Char_409", "Wayne", "Dond", myDob011, true, nationality01, true, 2.50, 9.0, myGoTo11, f1, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "Char_102", null, "Char_102", null, 30000, false, false, myTitles011, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son2.charID, myChar2Son2);
            NonPlayerCharacter myChar2Daughter = new NonPlayerCharacter("Char_410", "Esmerelda", "Dond", myDob012, false, nationality01, true, 2.50, 9.0, myGoTo12, f1, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "Char_102", null, "Char_102", null, 30000, false, false, myTitles012, null, loc: myFief6);
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
            myKingdom1.owner = Globals_Game.kingOne;
            myKingdom2.owner = Globals_Game.kingTwo;

            // set heralds
            // Globals_Game.heraldOne = myChar1;
            Globals_Game.heraldTwo = myChar2;

            // set province owners
            myProv.owner = myChar1;
            myProv2.owner = myChar2;

            // Add provinces to list of provinces owned 
            myChar1.addToOwnedProvinces(myProv);
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
            myKingdom2.titleHolder = myChar1.charID;

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

            // create and add ailment
            Ailment myAilment1 = new Ailment(Globals_Game.getNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, 3, 1);
            myChar1.ailments.Add(myAilment1.ailmentID, myAilment1);

            // populate Globals_Server.gameTypes
            Globals_Server.gameTypes.Add(0, "Individual points");
            Globals_Server.gameTypes.Add(1, "Individual position");
            Globals_Server.gameTypes.Add(2, "Team historical");

            // populate Globals_Server.combatValues
            uint[] eCombatValues = new uint[] {9, 9, 1, 9, 3, 1};
            Globals_Server.combatValues.Add("E", eCombatValues);
            uint[] oCombatValues = new uint[] {7, 7, 3, 2, 2, 1};
            Globals_Server.combatValues.Add("O", oCombatValues);

            // populate Globals_Server.recruitRatios
            double[] eRecruitRatios = new double[] { 0.01, 0.02, 0, 0.15, 0.33, 0.49 };
            Globals_Server.recruitRatios.Add("E", eRecruitRatios);
            double[] oRecruitRatios = new double[] { 0.01, 0.02, 0.03, 0, 0.45, 0.49 };
            Globals_Server.recruitRatios.Add("O", oRecruitRatios);

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
            // next available key = thisPriorityKey039


            // create an army and add in appropriate places
            uint[] myArmyTroops = new uint[] {10, 10, 0, 100, 200, 400};
            Army myArmy = new Army(Globals_Game.getNextArmyID(), null, null, 90, null, trp: myArmyTroops);
            Globals_Game.armyMasterList.Add(myArmy.armyID, myArmy);
            myArmy.owner = myChar1.charID;
            myArmy.leader = myChar1.charID;
            myArmy.days = Globals_Game.pcMasterList[myArmy.leader].days;
            myChar1.myArmies.Add(myArmy);
            myChar1.armyID = myArmy.armyID;
            myArmy.location = Globals_Game.pcMasterList[myArmy.leader].location.id;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = new uint[] { 10, 10, 30, 0, 200, 0 };
            Army myArmy2 = new Army(Globals_Game.getNextArmyID(), null, null, 90, null, trp: myArmyTroops2, aggr: 1);
            Globals_Game.armyMasterList.Add(myArmy2.armyID, myArmy2);
            myArmy2.owner = myChar2.charID;
            myArmy2.leader = myChar2.charID;
            myArmy2.days = Globals_Game.pcMasterList[myArmy2.leader].days;
            myChar2.myArmies.Add(myArmy2);
            myChar2.armyID = myArmy2.armyID;
            myArmy2.location = Globals_Game.pcMasterList[myArmy2.leader].location.id;
            myChar2.location.armies.Add(myArmy2.armyID);

            // bar a character from the myFief1 keep
			myFief2.barCharacter(myNPC1.charID);
            myFief2.barCharacter(myChar2.charID);
            myFief2.barCharacter(myChar1Wife.charID);

            // create VictoryDatas for PCs
            VictoryData myVicData01 = new VictoryData(myChar1.playerID, myChar1.charID, myChar1.calculateStature(), myChar1.getPopulationPercentage(), myChar1.getFiefsPercentage());
            Globals_Game.victoryData.Add(myVicData01.playerID, myVicData01);
            VictoryData myVicData02 = new VictoryData(myChar2.playerID, myChar2.charID, myChar2.calculateStature(), myChar2.getPopulationPercentage(), myChar2.getFiefsPercentage());
            Globals_Game.victoryData.Add(myVicData02.playerID, myVicData02);

			// try retrieving fief from masterlist using fiefID
			// Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");

		}

		/// <summary>
		/// Generates a random skill set for a Character
		/// </summary>
        /// <returns>Tuple<Skill, int>[] for use with a Character object</returns>
        public Tuple<Skill, int>[] generateSkillSet()
        {

            // create array of skills between 2-3 in length
            Tuple<Skill, int>[] skillSet = new Tuple<Skill, int>[Globals_Game.myRand.Next(2, 4)];

            // populate array of skills with randomly chosen skills
            // 1) make temporary copy of skillKeys
            List<string> skillKeysCopy = new List<string>(Globals_Game.skillKeys);

            // 2) choose random skill, and assign random skill level
            for (int i = 0; i < skillSet.Length; i++)
            {
                // choose random skill
                int randSkill = Globals_Game.myRand.Next(0, skillKeysCopy.Count - 1);

                // assign random skill level
                int randSkillLevel = Globals_Game.myRand.Next(1, 10);

                // create Skill tuple
                skillSet[i] = new Tuple<Skill, int>(Globals_Game.skillMasterList[skillKeysCopy[randSkill]], randSkillLevel);

                // remove skill from skillKeysCopy to ensure isn't chosen again
                skillKeysCopy.RemoveAt(randSkill);
            }

            return skillSet;

        }

        /// <summary>
		/// Writes all objects for a particular game to database
		/// </summary>
		/// <param name="gameID">ID of game (used for Riak bucket)</param>
        public void writeToDB(string gameID)
		{
			// ========= write CLOCK
            this.writeClock(gameID, Globals_Game.clock);

			// ========= write GLOBALS_SERVER/GAME DICTIONARIES
            this.writeDictionary(gameID, "combatValues", Globals_Server.combatValues);
            this.writeDictionary(gameID, "recruitRatios", Globals_Server.recruitRatios);
            this.writeDictionary(gameID, "battleProbabilities", Globals_Server.battleProbabilities);
            this.writeDictionary(gameID, "victoryConditionTypes", Globals_Server.gameTypes);
            // convert jEntryPriorities prior to writing
			Dictionary<string, byte> jEntryPrioritiesRiak = this.jEntryPrioritiesToRiak (Globals_Game.jEntryPriorities);
			this.writeDictionary(gameID, "jEntryPriorities", jEntryPrioritiesRiak);

            // ========= write JOURNALS
            this.writeJournal(gameID, "serverScheduledEvents", Globals_Game.scheduledEvents);
            this.writeJournal(gameID, "serverPastEvents", Globals_Game.pastEvents);
            this.writeJournal(gameID, "clientPastEvents", Globals_Client.myPastEvents);

			// ========= write GLOBALS_GAME/CLIENT CHARACTER VARIABLES
			// Globals_Client.myPlayerCharacter
			this.writeCharacterVar (gameID, "myPlayerCharacter", Globals_Client.myPlayerCharacter);
            // Globals_Game.sysAdmin
			this.writeCharacterVar (gameID, "sysAdmin", Globals_Game.sysAdmin);
            // Globals_Game.kingOne
			this.writeCharacterVar (gameID, "kingOne", Globals_Game.kingOne);
            // Globals_Game.kingTwo
			this.writeCharacterVar (gameID, "kingTwo", Globals_Game.kingTwo);
            // Globals_Game.princeOne
			this.writeCharacterVar (gameID, "princeOne", Globals_Game.princeOne);
            // Globals_Game.princeTwo
			this.writeCharacterVar (gameID, "princeTwo", Globals_Game.princeTwo);
            // Globals_Game.heraldOne
			this.writeCharacterVar (gameID, "heraldOne", Globals_Game.heraldOne);
            // Globals_Game.heraldTwo
			this.writeCharacterVar (gameID, "heraldTwo", Globals_Game.heraldTwo);

			// ========= write GLOBALS_GAME/SERVER newID VARIABLES
			// newCharID
			this.writeNewIDvar (gameID, "newCharID", Globals_Game.newCharID);
			// newArmyID
			this.writeNewIDvar (gameID, "newArmyID", Globals_Game.newArmyID);
			// newDetachmentID
			this.writeNewIDvar (gameID, "newDetachmentID", Globals_Game.newDetachmentID);
			// newAilmentID
			this.writeNewIDvar (gameID, "newAilmentID", Globals_Game.newAilmentID);
			// newSiegeID
			this.writeNewIDvar (gameID, "newSiegeID", Globals_Game.newSiegeID);
			// newJournalEntryID
			this.writeNewIDvar (gameID, "newJournalEntryID", Globals_Game.newJournalEntryID);
            // gameType
            this.writeNewIDvar(gameID, "currentVictoryType", Globals_Game.gameType);
            // duration
            this.writeNewIDvar(gameID, "duration", Globals_Game.duration);
            // startYear
            this.writeNewIDvar(gameID, "startYear", Globals_Game.startYear);
            // newJournalEntryID
            this.writeNewIDvar(gameID, "newGameID", Globals_Server.newGameID);

			// ========= write SKILLS
            // clear existing key list
            if (Globals_Game.skillKeys.Count > 0)
			{
                Globals_Game.skillKeys.Clear();
			}

            // write each object in skillMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Skill> pair in Globals_Game.skillMasterList)
			{
				bool success = this.writeSkill (gameID, pair.Value);
				if (success)
				{
                    Globals_Game.skillKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "skillKeys", Globals_Game.skillKeys);

            // ========= write BASELANGUAGES
            // clear existing key list
            if (Globals_Game.baseLangKeys.Count > 0)
            {
                Globals_Game.baseLangKeys.Clear();
            }

            // write each object in baseLanguageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, BaseLanguage> pair in Globals_Game.baseLanguageMasterList)
            {
                bool success = this.writeBaseLanguage(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.baseLangKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "baseLangKeys", Globals_Game.baseLangKeys);

			// ========= write LANGUAGES
            // clear existing key list
            if (Globals_Game.langKeys.Count > 0)
            {
                Globals_Game.langKeys.Clear();
            }

            // write each object in languageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Language> pair in Globals_Game.languageMasterList)
            {
                bool success = this.writeLanguage(gameID, l: pair.Value);
                if (success)
                {
                    Globals_Game.langKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "langKeys", Globals_Game.langKeys);

            // ========= write NATIONALITY OBJECTS
            // clear existing key list
            if (Globals_Game.nationalityKeys.Count > 0)
            {
                Globals_Game.nationalityKeys.Clear();
            }

            // write each object in nationalityMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Nationality> pair in Globals_Game.nationalityMasterList)
            {
                bool success = this.writeNationality(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.nationalityKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "nationalityKeys", Globals_Game.nationalityKeys);

            // ========= write RANKS
            // clear existing key list
            if (Globals_Game.rankKeys.Count > 0)
            {
                Globals_Game.rankKeys.Clear();
            }

            // write each object in rankMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Rank> pair in Globals_Game.rankMasterList)
            {
                bool success = this.writeRank(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.rankKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "rankKeys", Globals_Game.rankKeys);

            // ========= write POSITIONS
            // clear existing key list
            if (Globals_Game.positionKeys.Count > 0)
            {
                Globals_Game.positionKeys.Clear();
            }

            // write each object in positionMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Position> pair in Globals_Game.positionMasterList)
            {
                bool success = this.writePosition(gameID, p: pair.Value);
                if (success)
                {
                    Globals_Game.positionKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "positionKeys", Globals_Game.positionKeys);

            // ========= write NPCs
            // clear existing key list
            if (Globals_Game.npcKeys.Count > 0)
			{
                Globals_Game.npcKeys.Clear();
			}

            // write each object in npcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, NonPlayerCharacter> pair in Globals_Game.npcMasterList)
			{
				bool success = this.writeNPC (gameID, npc: pair.Value);
				if (success)
				{
                    Globals_Game.npcKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "npcKeys", Globals_Game.npcKeys);

			// ========= write PCs
            // clear existing key list
            if (Globals_Game.pcKeys.Count > 0)
			{
                Globals_Game.pcKeys.Clear();
			}

            // write each object in pcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, PlayerCharacter> pair in Globals_Game.pcMasterList)
			{
				bool success = this.writePC (gameID, pc: pair.Value);
				if (success)
				{
                    Globals_Game.pcKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "pcKeys", Globals_Game.pcKeys);

			// ========= write KINGDOMS
            // clear existing key list
            if (Globals_Game.kingKeys.Count > 0)
            {
                Globals_Game.kingKeys.Clear();
            }

            // write each object in kingdomMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Kingdom> pair in Globals_Game.kingdomMasterList)
            {
                bool success = this.writeKingdom(gameID, k: pair.Value);
                if (success)
                {
                    Globals_Game.kingKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "kingKeys", Globals_Game.kingKeys);

			// ========= write PROVINCES
            // clear existing key list
            if (Globals_Game.provKeys.Count > 0)
			{
                Globals_Game.provKeys.Clear();
			}

            // write each object in provinceMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Province> pair in Globals_Game.provinceMasterList)
			{
				bool success = this.writeProvince (gameID, p: pair.Value);
				if (success)
				{
                    Globals_Game.provKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "provKeys", Globals_Game.provKeys);

			// ========= write TERRAINS
            // clear existing key list
            if (Globals_Game.terrKeys.Count > 0)
			{
                Globals_Game.terrKeys.Clear();
			}

            // write each object in terrainMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Terrain> pair in Globals_Game.terrainMasterList)
			{
				bool success = this.writeTerrain (gameID, pair.Value);
				if (success)
				{
                    Globals_Game.terrKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "terrKeys", Globals_Game.terrKeys);

            // ========= write VICTORYDATA OBJECTS
            // clear existing key list
            if (Globals_Game.victoryDataKeys.Count > 0)
            {
                Globals_Game.victoryDataKeys.Clear();
            }

            // write each object in victoryData, whilst also repopulating key list
            foreach (KeyValuePair<string, VictoryData> pair in Globals_Game.victoryData)
            {
                bool success = this.writeVictoryData(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.victoryDataKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "victoryDataKeys", Globals_Game.victoryDataKeys);

            // ========= write FIEFS
            // clear existing key list
            if (Globals_Game.fiefKeys.Count > 0)
			{
                Globals_Game.fiefKeys.Clear();
			}

            // write each object in fiefMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Fief> pair in Globals_Game.fiefMasterList)
			{
				bool success = this.writeFief (gameID, f: pair.Value);
				if (success)
				{
                    Globals_Game.fiefKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "fiefKeys", Globals_Game.fiefKeys);

			// ========= write ARMIES
            // clear existing key list
            if (Globals_Game.armyKeys.Count > 0)
            {
                Globals_Game.armyKeys.Clear();
            }

            // write each object in armyMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Army> pair in Globals_Game.armyMasterList)
            {
                bool success = this.writeArmy(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.armyKeys.Add(pair.Key);
                }
            }
				
			// write key list to database
			this.writeKeyList(gameID, "armyKeys", Globals_Game.armyKeys);

			// ========= write SIEGES
            // clear existing key list
            if (Globals_Game.siegeKeys.Count > 0)
            {
                Globals_Game.siegeKeys.Clear();
            }

            // write each object in siegeMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Siege> pair in Globals_Game.siegeMasterList)
            {
                bool success = this.writeSiege(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.siegeKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "siegeKeys", Globals_Game.siegeKeys);

			// ========= write MAP (edges collection)
            this.writeMapEdges(gameID, map: Globals_Game.gameMap);

		}

		/// <summary>
		/// Loads all objects for a particular game from database
		/// </summary>
        /// <param name="gameID">ID of game (Riak bucket)</param>
        public void initialDBload(string gameID)
		{

            // ========= load KEY LISTS (to ensure efficient retrieval of specific game objects)
			this.initialDBload_keyLists (gameID);

            // ========= load CLOCK
            Globals_Game.clock = this.initialDBload_clock(gameID, "gameClock");

            // ========= load GLOBAL_SERVER/GAME DICTIONARIES
            Globals_Server.combatValues = this.initialDBload_dictStringUint(gameID, "combatValues");
            Globals_Server.recruitRatios = this.initialDBload_dictStringDouble(gameID, "recruitRatios");
            Globals_Server.battleProbabilities = this.initialDBload_dictStringDouble(gameID, "battleProbabilities");
            Globals_Server.gameTypes = this.initialDBload_dictIntString(gameID, "victoryConditionTypes");
            Globals_Game.jEntryPriorities = this.initialDBload_dictStringByte(gameID, "jEntryPriorities");

			// ========= load GLOBAL_GAME/SERVER newID VARIABLES
			// newCharID
			Globals_Game.newCharID = this.initialDBload_newIDs (gameID, "newCharID");
			// newArmyID
			Globals_Game.newArmyID = this.initialDBload_newIDs (gameID, "newArmyID");
			// newDetachmentID
			Globals_Game.newDetachmentID = this.initialDBload_newIDs (gameID, "newDetachmentID");
			// newAilmentID
			Globals_Game.newAilmentID = this.initialDBload_newIDs (gameID, "newAilmentID");
			// newSiegeID
			Globals_Game.newSiegeID = this.initialDBload_newIDs (gameID, "newSiegeID");
			// newJournalEntryID
			Globals_Game.newJournalEntryID = this.initialDBload_newIDs (gameID, "newJournalEntryID");
            // gameType
            Globals_Game.gameType = this.initialDBload_newIDs(gameID, "currentVictoryType");
            // duration
            Globals_Game.duration = this.initialDBload_newIDs(gameID, "duration");
            // startYear
            Globals_Game.startYear = this.initialDBload_newIDs(gameID, "startYear");
            // newGameID
            Globals_Server.newGameID = this.initialDBload_newIDs(gameID, "newGameID");

			// ========= load JOURNALS
            Globals_Game.scheduledEvents = this.initialDBload_journal(gameID, "serverScheduledEvents");
            Globals_Game.pastEvents = this.initialDBload_journal(gameID, "serverPastEvents");
            Globals_Client.myPastEvents = this.initialDBload_journal(gameID, "clientPastEvents");

			// ========= load victoryData
            foreach (string element in Globals_Game.victoryDataKeys)
            {
                VictoryData vicData = this.initialDBload_victoryData(gameID, element);
                // add VictoryData to Globals_Game.victoryData
				Globals_Game.victoryData.Add(vicData.playerID , vicData);
            }

            // ========= load SKILLS
            foreach (string element in Globals_Game.skillKeys)
			{
				Skill skill = this.initialDBload_skill (gameID, element);
                // add Skill to skillMasterList
                Globals_Game.skillMasterList.Add(skill.skillID, skill);
			}

            // ========= load BASELANGUAGES
            foreach (string element in Globals_Game.baseLangKeys)
            {
                BaseLanguage bLang = this.initialDBload_baseLanguage(gameID, element);
                // add BaseLanguage to baseLanguageMasterList
                Globals_Game.baseLanguageMasterList.Add(bLang.id, bLang);
            }

            // ========= load LANGUAGES
            foreach (string element in Globals_Game.langKeys)
            {
                Language lang = this.initialDBload_language(gameID, element);
                // add Language to languageMasterList
                Globals_Game.languageMasterList.Add(lang.id, lang);
            }

            // ========= load NATIONALITY OBJECTS
            foreach (string element in Globals_Game.nationalityKeys)
            {
                Nationality nat = this.initialDBload_nationality(gameID, element);
                // add Nationality to nationalityMasterList
                Globals_Game.nationalityMasterList.Add(nat.natID, nat);
            }

            // ========= load RANKS
            foreach (byte element in Globals_Game.rankKeys)
            {
                Rank rank = this.initialDBload_rank(gameID, element.ToString());
                // add Rank to rankMasterList
                Globals_Game.rankMasterList.Add(rank.id, rank);
            }

            // ========= load POSITIONS
            foreach (byte element in Globals_Game.positionKeys)
            {
                Position pos = this.initialDBload_position(gameID, element.ToString());
                // add Position to positionMasterList
                Globals_Game.positionMasterList.Add(pos.id, pos);
            }

            // ========= load SIEGES
            foreach (string element in Globals_Game.siegeKeys)
            {
                Siege s = this.initialDBload_Siege(gameID, element);
                // add Siege to siegeMasterList
                Globals_Game.siegeMasterList.Add(s.siegeID, s);
            }

            // ========= load ARMIES
            foreach (string element in Globals_Game.armyKeys)
            {
                Army a = this.initialDBload_Army(gameID, element);
                // add Army to armyMasterList
                Globals_Game.armyMasterList.Add(a.armyID, a);
            }

            // ========= load NPCs
            foreach (string element in Globals_Game.npcKeys)
			{
				NonPlayerCharacter npc = this.initialDBload_NPC (gameID, element);
                // add NPC to npcMasterList
                Globals_Game.npcMasterList.Add(npc.charID, npc);
			}

            // ========= load PCs
            foreach (string element in Globals_Game.pcKeys)
			{
				PlayerCharacter pc = this.initialDBload_PC (gameID, element);
                // add PC to pcMasterList
                Globals_Game.pcMasterList.Add(pc.charID, pc);
			}

            // ========= load KINGDOMS
            foreach (string element in Globals_Game.kingKeys)
            {
                Kingdom king = this.initialDBload_Kingdom(gameID, element);
                // add Kingdom to kingdomMasterList
                Globals_Game.kingdomMasterList.Add(king.id, king);
            }

            // ========= load PROVINCES
            foreach (string element in Globals_Game.provKeys)
			{
				Province prov = this.initialDBload_Province (gameID, element);
                // add Province to provinceMasterList
                Globals_Game.provinceMasterList.Add(prov.id, prov);
			}

            // ========= load TERRAINS
            foreach (string element in Globals_Game.terrKeys)
			{
				Terrain terr = this.initialDBload_terrain (gameID, element);
                // add Terrain to terrainMasterList
                Globals_Game.terrainMasterList.Add(terr.terrainCode, terr);
			}

            // ========= load FIEFS
            foreach (string element in Globals_Game.fiefKeys)
			{
				Fief f = this.initialDBload_Fief (gameID, element);
                // add Fief to fiefMasterList
                Globals_Game.fiefMasterList.Add(f.id, f);
			}

            // ========= process any CHARACTER goTo QUEUES containing entries
            if (Globals_Game.goToList.Count > 0)
			{
                for (int i = 0; i < Globals_Game.goToList.Count; i++)
				{
                    this.populate_goTo(Globals_Game.goToList[i]);
				}
                Globals_Game.goToList.Clear();
			}

			// ========= write GLOBALS_GAME/CLIENT CHARACTER VARIABLES
			Globals_Client.myPlayerCharacter = this.initialDBload_charVariable (gameID, "myPlayerCharacter");
			Globals_Game.sysAdmin = this.initialDBload_charVariable (gameID, "sysAdmin");
			Globals_Game.kingOne = this.initialDBload_charVariable (gameID, "kingOne");
			Globals_Game.kingTwo = this.initialDBload_charVariable (gameID, "kingTwo");
			Globals_Game.princeOne = this.initialDBload_charVariable (gameID, "princeOne");
			Globals_Game.princeTwo = this.initialDBload_charVariable (gameID, "princeTwo");
			Globals_Game.heraldOne = this.initialDBload_charVariable (gameID, "heraldOne");
			Globals_Game.heraldTwo = this.initialDBload_charVariable (gameID, "heraldTwo");

            // ========= load MAP
            Globals_Game.gameMap = this.initialDBload_map(gameID, "mapEdges");
		}

		/// <summary>
		/// Loads all Riak key lists for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which key lists to be retrieved</param>
        public void initialDBload_keyLists(string gameID)
		{
            // populate skillKeys
			var skillKeyResult = rClient.Get(gameID, "skillKeys");
			if (skillKeyResult.IsSuccess)
			{
                Globals_Game.skillKeys = skillKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skillKeys from database.");
                }
			}

			// populate nationalityKeys
            var natKeyResult = rClient.Get(gameID, "nationalityKeys");
			if (natKeyResult.IsSuccess)
			{
                Globals_Game.nationalityKeys = natKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve nationalityKeys from database.");
                }
			}

			// populate langKeys
			var langKeyResult = rClient.Get(gameID, "langKeys");
			if (langKeyResult.IsSuccess)
			{
				Globals_Game.langKeys = langKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
				if (Globals_Client.showMessages)
				{
					System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve langKeys from database.");
				}
			}

			// populate baseLangKeys
			var bLangKeyResult = rClient.Get(gameID, "baseLangKeys");
			if (bLangKeyResult.IsSuccess)
            {
				Globals_Game.baseLangKeys = bLangKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
					System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve baseLangKeys from database.");
                }
            }

            // populate rankKeys
            var rankKeyResult = rClient.Get(gameID, "rankKeys");
            if (rankKeyResult.IsSuccess)
            {
                Globals_Game.rankKeys = rankKeyResult.Value.GetObject<List<byte>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve rankKeys from database.");
                }
            }

            // populate positionKeys
            var positionKeyResult = rClient.Get(gameID, "positionKeys");
            if (positionKeyResult.IsSuccess)
            {
                Globals_Game.positionKeys = positionKeyResult.Value.GetObject<List<byte>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve positionKeys from database.");
                }
            }

            // populate npcKeys
            var npcKeyResult = rClient.Get(gameID, "npcKeys");
			if (npcKeyResult.IsSuccess)
			{
                Globals_Game.npcKeys = npcKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve npcKeys from database.");
                }
			}

            // populate pcKeys
            var pcKeyResult = rClient.Get(gameID, "pcKeys");
			if (pcKeyResult.IsSuccess)
			{
                Globals_Game.pcKeys = pcKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve pcKeys from database.");
                }
			}

            // populate kingKeys
            var kingKeyResult = rClient.Get(gameID, "kingKeys");
            if (kingKeyResult.IsSuccess)
            {
                Globals_Game.kingKeys = kingKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve kingKeys from database.");
                }
            }

            // populate provKeys
            var provKeyResult = rClient.Get(gameID, "provKeys");
			if (provKeyResult.IsSuccess)
			{
                Globals_Game.provKeys = provKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve provKeys from database.");
                }
			}

            // populate terrKeys
            var terrKeyResult = rClient.Get(gameID, "terrKeys");
			if (terrKeyResult.IsSuccess)
			{
                Globals_Game.terrKeys = terrKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve terrKeys from database.");
                }
			}

            // populate victoryDataKeys
            var vicDataResult = rClient.Get(gameID, "victoryDataKeys");
            if (vicDataResult.IsSuccess)
            {
                Globals_Game.victoryDataKeys = vicDataResult.Value.GetObject<List<string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve victoryDataKeys from database.");
                }
            }

            // populate fiefKeys
            var fiefKeyResult = rClient.Get(gameID, "fiefKeys");
			if (fiefKeyResult.IsSuccess)
			{
                Globals_Game.fiefKeys = fiefKeyResult.Value.GetObject<List<string>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve fiefKeys from database.");
                }
			}

            // populate armyKeys
            var armyKeyResult = rClient.Get(gameID, "armyKeys");
            if (armyKeyResult.IsSuccess)
            {
                Globals_Game.armyKeys = armyKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve armyKeys from database.");
                }
            }

            // populate siegeKeys
            var siegeKeyResult = rClient.Get(gameID, "siegeKeys");
            if (siegeKeyResult.IsSuccess)
            {
                Globals_Game.siegeKeys = siegeKeyResult.Value.GetObject<List<string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve siegeKeys from database.");
                }
            }

        }

		/// <summary>
		/// Loads GameClock for a particular game from database
		/// </summary>
        /// <returns>GameClock object</returns>
        /// <param name="gameID">Game for which clock to be retrieved</param>
		/// <param name="clockID">ID of clock to be retrieved</param>
        public GameClock initialDBload_clock(string gameID, string clockID)
		{
			var clockResult = rClient.Get(gameID, clockID);
			var newClock = new GameClock();

			if (clockResult.IsSuccess)
			{
				newClock = clockResult.Value.GetObject<GameClock>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve GameClock " + clockID);
                }
			}

			return newClock;
		}

		/// <summary>
		/// Loads a particular newID variable for a particular game from database
		/// </summary>
		/// <returns>uint</returns>
		/// <param name="gameID">Game for which newID variable to be retrieved</param>
		/// <param name="clockID">newID variable to be retrieved</param>
		public uint initialDBload_newIDs(string gameID, string newID)
		{
			var newIDResult = rClient.Get(gameID, newID);
			uint newIDout = 0;

			if (newIDResult.IsSuccess)
			{
				newIDout = newIDResult.Value.GetObject<uint>();
			}
			else
			{
				if (Globals_Client.showMessages)
				{
					System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve newID variable " + newID);
				}
			}

			return newIDout;
		}

        /// <summary>
        /// Loads a Journal from the database
        /// </summary>
        /// <returns>Journal object</returns>
        /// <param name="gameID">Game for which Journal to be retrieved</param>
        /// <param name="journalID">ID of Journal to be retrieved</param>
        public Journal initialDBload_journal(string gameID, string journalID)
        {
            var journalResult = rClient.Get(gameID, journalID);
            var newJournal = new Journal();

            if (journalResult.IsSuccess)
            {
                newJournal = journalResult.Value.GetObject<Journal>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Journal " + journalID);
                }
            }

            return newJournal;
        }

		/// <summary>
		/// Loads a Character variable from the database
		/// </summary>
		/// <returns>PlayerCharacter object</returns>
		/// <param name="gameID">Game for which Character variable to be retrieved</param>
		/// <param name="charVarID">ID of Character variable to be retrieved</param>
		public PlayerCharacter initialDBload_charVariable(string gameID, string charVarID)
		{
			var charVarResult = rClient.Get(gameID, charVarID);
			String pcID = "";
			PlayerCharacter newPC = null;

			if (charVarResult.IsSuccess)
			{
				pcID = charVarResult.Value.GetObject<String>();
				if (pcID != "")
				{
					if (Globals_Game.pcMasterList.ContainsKey(pcID))
					{
						newPC = Globals_Game.pcMasterList[pcID];
					}
				}
			}
			else
			{
				if (Globals_Client.showMessages)
				{
					System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Character variable " + charVarID);
				}
			}

			return newPC;
		}

        /// <summary>
		/// Loads Dictionary<string, uint[]> from database
		/// </summary>
		/// <returns>Dictionary<string, uint[]> object</returns>
		/// <param name="gameID">Game for which Dictionary to be retrieved</param>
		/// <param name="dictID">ID of Dictionary to be retrieved</param>
        public Dictionary<string, uint[]> initialDBload_dictStringUint(string gameID, string dictID)
		{
			var dictResult = rClient.Get(gameID, dictID);
			var newDict = new Dictionary<string, uint[]>();

			if (dictResult.IsSuccess)
			{
				newDict = dictResult.Value.GetObject<Dictionary<string, uint[]>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
                }
			}

			return newDict;
		}

        /// <summary>
        /// Loads Dictionary<string, byte> from database
        /// </summary>
        /// <returns>Dictionary<string, byte> object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public Dictionary<string[], byte> initialDBload_dictStringByte(string gameID, string dictID)
        {
			Dictionary<string[], byte> dictOut = new Dictionary<string[], byte>();
            var dictResult = rClient.Get(gameID, dictID);
			var tempDict = new Dictionary<string, byte>();

            if (dictResult.IsSuccess)
            {
				tempDict = dictResult.Value.GetObject<Dictionary<string, byte>>();
				dictOut = this.jEntryPrioritiesFromRiak (tempDict);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
                }
            }

            return dictOut;
        }
			
		/// <summary>
		/// Converts jEntryPriorities Dictionary from Riak format into game format
		/// </summary>
		/// <returns>Dictionary<string[], byte> for game use</returns>
		/// <param name="dictToConvert">The Dictionary to convert</param>
		public Dictionary<string[], byte> jEntryPrioritiesFromRiak(Dictionary<string, byte> dictToConvert)
		{
			Dictionary<string[], byte> dictOut = new Dictionary<string[], byte> ();

			if (dictToConvert.Count > 0)
			{
				foreach (KeyValuePair<string, byte> thisEntry in dictToConvert)
				{
					string[] thisKey = thisEntry.Key.Split ('|');
					dictOut.Add (thisKey, thisEntry.Value);
				}
			}

			return dictOut;
		}

		/// <summary>
		/// Loads Dictionary<string, double[]> from database
		/// </summary>
		/// <returns>Dictionary<string, double[]> object</returns>
		/// <param name="gameID">Game for which Dictionary to be retrieved</param>
		/// <param name="dictID">ID of Dictionary to be retrieved</param>
        public Dictionary<string, double[]> initialDBload_dictStringDouble(string gameID, string dictID)
		{
			var dictResult = rClient.Get(gameID, dictID);
			var newDict = new Dictionary<string, double[]>();

			if (dictResult.IsSuccess)
			{
				newDict = dictResult.Value.GetObject<Dictionary<string, double[]>>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
                }
			}

			return newDict;
		}

        /// <summary>
        /// Loads Dictionary<uint, string> from database
        /// </summary>
        /// <returns>Dictionary<uint, string> object</returns>
        /// <param name="gameID">Game for which Dictionary to be retrieved</param>
        /// <param name="dictID">ID of Dictionary to be retrieved</param>
        public Dictionary<uint, string> initialDBload_dictIntString(string gameID, string dictID)
        {
            var dictResult = rClient.Get(gameID, dictID);
            var newDict = new Dictionary<uint, string>();

            if (dictResult.IsSuccess)
            {
                newDict = dictResult.Value.GetObject<Dictionary<uint, string>>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
                }
            }

            return newDict;
        }

        /// <summary>
		/// Loads a skill for a particular game from database
		/// </summary>
        /// <returns>Skill object</returns>
        /// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="skillID">ID of skill to be retrieved</param>
        public Skill initialDBload_skill(string gameID, string skillID)
		{
			var skillResult = rClient.Get(gameID, skillID);
			var newSkill = new Skill();

			if (skillResult.IsSuccess)
			{
				newSkill = skillResult.Value.GetObject<Skill>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skill " + skillID);
                }
			}

			return newSkill;
		}

		/// <summary>
		/// Loads an NPC for a particular game from database
		/// </summary>
        /// <returns>NonPlayerCharacter object</returns>
        /// <param name="gameID">Game for which NPC to be retrieved</param>
		/// <param name="npcID">ID of NPC to be retrieved</param>
        public NonPlayerCharacter initialDBload_NPC(string gameID, string npcID)
		{
			var npcResult = rClient.Get(gameID, npcID);
			var npcRiak = new NonPlayerCharacter_Riak();
			NonPlayerCharacter myNPC = new NonPlayerCharacter ();

			if (npcResult.IsSuccess)
			{
                // extract NonPlayerCharacter_Riak object
				npcRiak = npcResult.Value.GetObject<NonPlayerCharacter_Riak>();
                // if NonPlayerCharacter_Riak goTo queue contains entries, store for later processing
				if (npcRiak.goTo.Count > 0)
				{
                    Globals_Game.goToList.Add(npcRiak);
				}
                // create NonPlayerCharacter from NonPlayerCharacter_Riak
                myNPC = this.NPCfromRiakNPC(npcRiak);
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve NonPlayerCharacter " + npcID);
                }
			}

			return myNPC;
		}

		/// <summary>
		/// Loads a PC for a particular game from database
		/// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="gameID">Game for which PC to be retrieved</param>
		/// <param name="pcID">ID of PC to be retrieved</param>
        public PlayerCharacter initialDBload_PC(string gameID, string pcID)
		{
			var pcResult = rClient.Get(gameID, pcID);
			var pcRiak = new PlayerCharacter_Riak();
			PlayerCharacter myPC = new PlayerCharacter ();

			if (pcResult.IsSuccess)
			{
                // extract PlayerCharacter_Riak object
                pcRiak = pcResult.Value.GetObject<PlayerCharacter_Riak>();
                // if PlayerCharacter_Riak goTo queue contains entries, store for later processing
                if (pcRiak.goTo.Count > 0)
				{
                    Globals_Game.goToList.Add(pcRiak);
				}
                // create PlayerCharacter from PlayerCharacter_Riak
                myPC = this.PCfromRiakPC(pcRiak);
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve PlayerCharacter " + pcID);
                }
			}

			return myPC;
		}

        /// <summary>
        /// Loads a Kingdom for a particular game from database
        /// </summary>
        /// <returns>Kingdom object</returns>
        /// <param name="gameID">Game for which Kingdom to be retrieved</param>
        /// <param name="kingID">ID of Kingdom to be retrieved</param>
        public Kingdom initialDBload_Kingdom(string gameID, string kingID)
        {
            var kingResult = rClient.Get(gameID, kingID);
            var kingRiak = new Kingdom_Riak();
            Kingdom myKing = new Kingdom();

            if (kingResult.IsSuccess)
            {
                // extract Kingdom_Riak object
                kingRiak = kingResult.Value.GetObject<Kingdom_Riak>();
                // create Kingdom from Kingdom_Riak
                myKing = this.KingdomFromRiak(kingRiak);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Province " + kingID);
                }
            }

            return myKing;
        }

        /// <summary>
		/// Loads a Province for a particular game from database
		/// </summary>
        /// <returns>Province object</returns>
        /// <param name="gameID">Game for which Province to be retrieved</param>
		/// <param name="provID">ID of Province to be retrieved</param>
        public Province initialDBload_Province(string gameID, string provID)
		{
			var provResult = rClient.Get(gameID, provID);
			var provRiak = new Province_Riak();
			Province myProv = new Province ();

			if (provResult.IsSuccess)
			{
                // extract Province_Riak object
                provRiak = provResult.Value.GetObject<Province_Riak>();
                // create Province from Province_Riak
                myProv = this.ProvinceFromRiak(provRiak);
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Province " + provID);
                }
			}

			return myProv;
		}

		/// <summary>
		/// Loads a Terrain for a particular game from database
		/// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="gameID">Game for which Terrain to be retrieved</param>
		/// <param name="terrID">ID of Terrain to be retrieved</param>
        public Terrain initialDBload_terrain(string gameID, string terrID)
		{
			var terrainResult = rClient.Get(gameID, terrID);
			var newTerrain = new Terrain();

			if (terrainResult.IsSuccess)
			{
				newTerrain = terrainResult.Value.GetObject<Terrain>();
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Terrain " + terrID);
                }
			}

			return newTerrain;
		}

        /// <summary>
        /// Loads a VictoryData object for a particular game from database
        /// </summary>
        /// <returns>VictoryData object</returns>
        /// <param name="gameID">Game for which VictoryData to be retrieved</param>
        /// <param name="vicDataID">ID of VictoryData to be retrieved</param>
        public VictoryData initialDBload_victoryData(string gameID, string vicDataID)
        {
            var vicDataResult = rClient.Get(gameID, vicDataID);
            var newVictoryData = new VictoryData();

            if (vicDataResult.IsSuccess)
            {
                newVictoryData = vicDataResult.Value.GetObject<VictoryData>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve VictoryData " + vicDataID);
                }
            }

            return newVictoryData;
        }

        /// <summary>
        /// Loads a BaseLanguage for a particular game from database
        /// </summary>
        /// <returns>BaseLanguage object</returns>
        /// <param name="gameID">Game for which BaseLanguage to be retrieved</param>
        /// <param name="bLangID">ID of Language to be retrieved</param>
        public BaseLanguage initialDBload_baseLanguage(string gameID, string bLangID)
        {
            var bLangResult = rClient.Get(gameID, bLangID);
            var newBaseLang = new BaseLanguage();

            if (bLangResult.IsSuccess)
            {
                newBaseLang = bLangResult.Value.GetObject<BaseLanguage>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve BaseLanguage " + bLangID);
                }
            }

            return newBaseLang;
        }

        /// <summary>
        /// Loads a Language for a particular game from database
        /// </summary>
        /// <returns>Language object</returns>
        /// <param name="gameID">Game for which Language to be retrieved</param>
        /// <param name="langID">ID of Language to be retrieved</param>
        public Language initialDBload_language(string gameID, string langID)
        {
            var languageResult = rClient.Get(gameID, langID);
            var langRiak = new Language_Riak();
            var newLanguage = new Language();

            if (languageResult.IsSuccess)
            {
                // extract Language_Riak
                langRiak = languageResult.Value.GetObject<Language_Riak>();

                // create Language from Language_Riak
                newLanguage = this.LanguageFromRiak(langRiak);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Language " + langID);
                }
            }

            return newLanguage;
        }

        /// <summary>
        /// Loads a Nationality for a particular game from database
        /// </summary>
        /// <returns>Nationality object</returns>
        /// <param name="gameID">Game for which Nationality to be retrieved</param>
        /// <param name="natID">ID of Nationality to be retrieved</param>
        public Nationality initialDBload_nationality(string gameID, string natID)
        {
            var natResult = rClient.Get(gameID, natID);
            var newNat = new Nationality();

            if (natResult.IsSuccess)
            {
                newNat = natResult.Value.GetObject<Nationality>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Nationality " + natID);
                }
            }

            return newNat;
        }

        /// <summary>
        /// Loads a Rank for a particular game from database
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="gameID">Game for which Rank to be retrieved</param>
        /// <param name="rankID">ID of Rank to be retrieved</param>
        public Rank initialDBload_rank(string gameID, string rankID)
        {
            var rankResult = rClient.Get(gameID, rankID);
            var newRank = new Rank();

            if (rankResult.IsSuccess)
            {
                newRank = rankResult.Value.GetObject<Rank>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Rank " + rankID);
                }
            }

            return newRank;
        }

        /// <summary>
        /// Loads a Position for a particular game from database
        /// </summary>
        /// <returns>Position object</returns>
        /// <param name="gameID">Game for which Position to be retrieved</param>
        /// <param name="posID">ID of Position to be retrieved</param>
        public Position initialDBload_position(string gameID, string posID)
        {
            var posResult = rClient.Get(gameID, posID);
            var posRiak = new Position_Riak();
            var newPos = new Position();

            if (posResult.IsSuccess)
            {
                // extract Position_Riak object
                posRiak = posResult.Value.GetObject<Position_Riak>();

                // create Position from Position_Riak
                newPos = this.PositionfromPosRiak(posRiak);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Position " + posID);
                }
            }

            return newPos;
        }

        /// <summary>
		/// Loads a Fief for a particular game from database
		/// </summary>
        /// <returns>Fief object</returns>
        /// <param name="gameID">Game for which Fief to be retrieved</param>
		/// <param name="fiefID">ID of Fief to be retrieved</param>
        public Fief initialDBload_Fief(string gameID, string fiefID)
		{
			var fiefResult = rClient.Get(gameID, fiefID);
			var fiefRiak = new Fief_Riak();
			Fief myFief = new Fief ();

			if (fiefResult.IsSuccess)
			{
                // extract Fief_Riak object
                fiefRiak = fiefResult.Value.GetObject<Fief_Riak>();
                // create Fief from Fief_Riak
                myFief = this.FiefFromRiakFief(fiefRiak);
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Fief " + fiefID);
                }
			}

			return myFief;
		}

        /// <summary>
        /// Loads an Army for a particular game from database
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="gameID">Game for which Army to be retrieved</param>
        /// <param name="armyID">ID of Army to be retrieved</param>
        public Army initialDBload_Army(string gameID, string armyID)
        {
            var armyResult = rClient.Get(gameID, armyID);
            Army myArmy = new Army();

            if (armyResult.IsSuccess)
            {
                // extract Army object
                myArmy = armyResult.Value.GetObject<Army>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Army " + armyID);
                }
            }

            return myArmy;
        }

        /// <summary>
        /// Loads a Siege for a particular game from database
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="gameID">Game for which Siege to be retrieved</param>
        /// <param name="siegeID">ID of Siege to be retrieved</param>
        public Siege initialDBload_Siege(string gameID, string siegeID)
        {
            var siegeResult = rClient.Get(gameID, siegeID);
            Siege mySiege = new Siege();

            if (siegeResult.IsSuccess)
            {
                // extract Army object
                mySiege = siegeResult.Value.GetObject<Siege>();
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Siege " + siegeID);
                }
            }

            return mySiege;
        }

        /// <summary>
		/// Creates HexMapGraph for a particular game,
		/// using map edges collection retrieved from database
		/// </summary>
        /// <returns>HexMapGraph object</returns>
        /// <param name="gameID">Game for which map to be created</param>
		/// <param name="mapEdgesID">ID of map edges collection to be retrieved</param>
        public HexMapGraph initialDBload_map(string gameID, string mapEdgesID)
		{
			var mapResult = rClient.Get(gameID, mapEdgesID);
            List<TaggedEdge<string, string>> edgesList = new List<TaggedEdge<string, string>>();
			var newMap = new HexMapGraph();

			if (mapResult.IsSuccess)
			{
                edgesList = mapResult.Value.GetObject<List<TaggedEdge<string, string>>>();
				TaggedEdge<Fief, string>[] edgesArray = this.EdgeCollection_from_Riak (edgesList);
                // create map from edges collection
				newMap = new HexMapGraph ("map001", edgesArray);
			}
			else
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve map edges " + mapEdgesID);
                }
			}

			return newMap;
		}

		/// <summary>
		/// Converts Fief object (containing nested objects) into suitable format for JSON serialisation
		/// </summary>
        /// <returns>Fief_Riak object</returns>
        /// <param name="f">Fief to be converted</param>
		public Fief_Riak FieftoRiak(Fief f)
		{
			Fief_Riak fOut = null;
			fOut = new Fief_Riak (f);
			return fOut;
		}

		/// <summary>
		/// Converts Fief_Riak object into Fief object.
		/// Also inserts Fief into appropriate PlayerCharacter and NonPlayerCharacter objects.
		/// </summary>
        /// <returns>Fief object</returns>
        /// <param name="fr">Fief_Riak object to be converted</param>
		public Fief FiefFromRiakFief(Fief_Riak fr)
		{
			Fief fOut = null;
			// create Fief from Fief_Riak
			fOut = new Fief (fr);

			// insert province
            fOut.province = Globals_Game.provinceMasterList[fr.province];

            // insert language
            fOut.language = Globals_Game.languageMasterList[fr.language];

            // insert owner
            fOut.owner = Globals_Game.pcMasterList[fr.owner];
			// check if fief is in owner's list of fiefs owned
			bool fiefInList = fOut.owner.ownedFiefs.Any(item => item.id == fOut.id);
			// if not, add it
			if(! fiefInList)
			{
				fOut.owner.ownedFiefs.Add(fOut);
			}

			// insert ancestral owner
            fOut.ancestralOwner = Globals_Game.pcMasterList[fr.ancestralOwner];

			// insert bailiff (PC or NPC)
			if (fr.bailiff != null)
			{
                if (Globals_Game.npcMasterList.ContainsKey(fr.bailiff))
                {
                    fOut.bailiff = Globals_Game.npcMasterList[fr.bailiff];
                }
                else if (Globals_Game.pcMasterList.ContainsKey(fr.bailiff))
                {
                    fOut.bailiff = Globals_Game.pcMasterList[fr.bailiff];
				}
                else
                {
					fOut.bailiff = null;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Unable to identify bailiff (" + fr.bailiff + ") for Fief " + fOut.id);
                    }
				}
			}
				
			//insert terrain
            fOut.terrain = Globals_Game.terrainMasterList[fr.terrain];

			// insert characters
			if (fr.charactersInFief.Count > 0)
			{
				for (int i = 0; i < fr.charactersInFief.Count; i++)
				{
                    if (Globals_Game.npcMasterList.ContainsKey(fr.charactersInFief[i]))
					{
                        fOut.charactersInFief.Add(Globals_Game.npcMasterList[fr.charactersInFief[i]]);
                        Globals_Game.npcMasterList[fr.charactersInFief[i]].location = fOut;
					}
                    else if (Globals_Game.pcMasterList.ContainsKey(fr.charactersInFief[i]))
                    {
                        fOut.charactersInFief.Add(Globals_Game.pcMasterList[fr.charactersInFief[i]]);
                        Globals_Game.pcMasterList[fr.charactersInFief[i]].location = fOut;
                    }
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Unable to identify character (" + fr.charactersInFief[i] + ") for Fief " + fOut.id);
                        }
                    }

				}
			}

            // insert rank using rankID
            if (fr.rank != null)
            {
                if (Globals_Game.rankMasterList.ContainsKey(fr.rank))
                {
                    fOut.rank = Globals_Game.rankMasterList[fr.rank];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Fief " + fr.id + ": Rank not found (" + fr.rank + ")");
                    }
                }
            }

			return fOut;
		}

		/// <summary>
		/// Converts PlayerCharacter object (containing nested objects) into suitable format for JSON serialisation
		/// </summary>
        /// <returns>PlayerCharacter_Riak object</returns>
        /// <param name="pc">PlayerCharacter to be converted</param>
		public PlayerCharacter_Riak PCtoRiak(PlayerCharacter pc)
		{
			PlayerCharacter_Riak pcRiakOut = null;
			pcRiakOut = new PlayerCharacter_Riak (pc);
			return pcRiakOut;
		}

		/// <summary>
		/// Converts NonPlayerCharacter object (containing nested objects) into suitable format for JSON serialisation
		/// </summary>
        /// <returns>NonPlayerCharacter_Riak object</returns>
        /// <param name="npc">NonPlayerCharacter to be converted</param>
		public NonPlayerCharacter_Riak NPCtoRiak(NonPlayerCharacter npc)
		{
			NonPlayerCharacter_Riak npcRiakOut = null;
			npcRiakOut = new NonPlayerCharacter_Riak (npc);
			return npcRiakOut;
		}

        /// <summary>
        /// Converts Language object (containing nested objects) into suitable format for JSON serialisation
        /// </summary>
        /// <returns>Language_Riak object</returns>
        /// <param name="l">Language to be converted</param>
        public Language_Riak LangToRiak(Language l)
        {
            Language_Riak langRiakOut = null;
            langRiakOut = new Language_Riak(l);
            return langRiakOut;
        }

        /// <summary>
        /// Converts Position_Riak objects into Position objects
        /// </summary>
        /// <returns>Position object</returns>
        /// <param name="pr">Position_Riak object to be converted</param>
        public Position PositionfromPosRiak(Position_Riak pr)
        {
            Position posOut = null;
            // create Position from Position_Riak
            posOut = new Position(pr);

            // insert nationality
            if (Globals_Game.nationalityMasterList.ContainsKey(pr.nationality))
            {
                posOut.nationality = Globals_Game.nationalityMasterList[pr.nationality];
            }

            return posOut;
        }

        /// <summary>
		/// Converts PlayerCharacter_Riak objects into PlayerCharacter game objects
		/// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="pcr">PlayerCharacter_Riak object to be converted</param>
		public PlayerCharacter PCfromRiakPC(PlayerCharacter_Riak pcr)
		{
			PlayerCharacter pcOut = null;
			// create PlayerCharacter from PlayerCharacter_Riak
			pcOut = new PlayerCharacter (pcr);

            // insert language
            pcOut.language = Globals_Game.languageMasterList[pcr.language];

            // insert nationality
            pcOut.nationality = Globals_Game.nationalityMasterList[pcr.nationality];

            // insert skills
			if (pcr.skills.Length > 0)
			{
				for (int i = 0; i < pcr.skills.Length; i++)
				{
                    pcOut.skills[i] = new Tuple<Skill, int>(Globals_Game.skillMasterList[pcr.skills[i].Item1], pcr.skills[i].Item2);
				}
			}

			// insert employees
			if (pcr.myNPCs.Count > 0)
			{
				for (int i = 0; i < pcr.myNPCs.Count; i++)
				{
                    pcOut.myNPCs.Add(Globals_Game.npcMasterList[pcr.myNPCs[i]]);
				}
			}

            // insert armies
            if (pcr.myArmies.Count > 0)
            {
                for (int i = 0; i < pcr.myArmies.Count; i++)
                {
                    pcOut.myArmies.Add(Globals_Game.armyMasterList[pcr.myArmies[i]]);
                }
            }

            return pcOut;
		}

        /// <summary>
        /// Converts NonPlayerCharacter_Riak objects into NonPlayerCharacter game objects
        /// </summary>
        /// <returns>NonPlayerCharacter object</returns>
        /// <param name="npcr">NonPlayerCharacter_Riak object to be converted</param>
        public NonPlayerCharacter NPCfromRiakNPC(NonPlayerCharacter_Riak npcr)
        {
            NonPlayerCharacter npcOut = null;
            // create NonPlayerCharacter from NonPlayerCharacter_Riak
            npcOut = new NonPlayerCharacter(npcr);

            // insert language
            npcOut.language = Globals_Game.languageMasterList[npcr.language];

            // insert nationality
            npcOut.nationality = Globals_Game.nationalityMasterList[npcr.nationality];

            // insert skills
            if (npcr.skills.Length > 0)
            {
                for (int i = 0; i < npcr.skills.Length; i++)
                {
                    npcOut.skills[i] = new Tuple<Skill, int>(Globals_Game.skillMasterList[npcr.skills[i].Item1], npcr.skills[i].Item2);
                }
            }

            return npcOut;
        }

        /// <summary>
        /// Converts Language_Riak objects into Language game objects
		/// </summary>
        /// <returns>Language object</returns>
        /// <param name="lr">Language_Riak object to be converted</param>
		public Language LanguageFromRiak(Language_Riak lr)
		{
            Language langOut = null;
            // create NonPlayerCharacter from NonPlayerCharacter_Riak
            langOut = new Language(lr);

            // insert BaseLanguage
            langOut.baseLanguage = Globals_Game.baseLanguageMasterList[lr.baseLanguage];

			return langOut;
		}

		/// <summary>
		/// Converts HexMapGraph edges collection into suitable format for JSON serialisation
		/// </summary>
        /// <returns>'String-ified' edges collection</returns>
        /// <param name="edgesIn">Edges collection to be converted</param>
        public List<TaggedEdge<string, string>> EdgeCollection_to_Riak(List<TaggedEdge<Fief, string>> edgesIn)
		{
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();

			foreach (TaggedEdge<Fief, string> element in edgesIn)
			{
                // convert each Fief object to string ID
				edgesOut.Add (this.EdgeFief_to_EdgeString (element));
			}

			return edgesOut;
		}

		/// <summary>
        /// Converts 'String-ified' edges collection into HexMapGraph edges collection
		/// </summary>
        /// <returns>HexMapGraph edges collection</returns>
        /// <param name="edgesIn">'String-ified' edges collection to be converted</param>
        public TaggedEdge<Fief, string>[] EdgeCollection_from_Riak(List<TaggedEdge<string, string>> edgesIn)
		{
			TaggedEdge<Fief, string>[] edgesOut = new TaggedEdge<Fief, string>[edgesIn.Count];

			int i = 0;
            foreach (TaggedEdge<string, string> element in edgesIn)
			{
                // convert to HexMapGraph edge
                edgesOut[i] = this.EdgeString_to_EdgeFief(element);
				i++;
			}

			return edgesOut;
		}

		/// <summary>
        /// Converts HexMapGraph edge object into one suitable format for JSON serialisation
		/// (i.e. 'string-ifies' it)
		/// </summary>
        /// <returns>'String-ified' edge</returns>
        /// <param name="te">HexMapGraph edge to be converted</param>
        public TaggedEdge<string, string> EdgeFief_to_EdgeString(TaggedEdge<Fief, string> te)
		{
            TaggedEdge<string, string> edgeOut = new TaggedEdge<string, string>(te.Source.id, te.Target.id, te.Tag);
			return edgeOut;
		}

		/// <summary>
        /// Converts 'string-ified' edge into HexMapGraph edge
		/// </summary>
        /// <returns>HexMapGraph edge</returns>
        /// <param name="te">'String-ified' edge to be converted</param>
        public TaggedEdge<Fief, string> EdgeString_to_EdgeFief(TaggedEdge<string, string> te)
		{
            TaggedEdge<Fief, string> edgeOut = new TaggedEdge<Fief, string>(Globals_Game.fiefMasterList[te.Source], Globals_Game.fiefMasterList[te.Target], te.Tag);
			return edgeOut;
		}

		/// <summary>
        /// Inserts Fief objects into a Character's goTo Queue,
        /// based on fiefIDs in a Character_Riak's goTo Queue (used with load from database)
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="cr">Character_Riak containing goTo Queue</param>
		public bool populate_goTo(Character_Riak cr)
		{
			bool success = false;
			Character myCh = null;

			if (cr is PlayerCharacter_Riak)
			{
                if (Globals_Game.pcMasterList.ContainsKey(cr.charID))
				{
                    myCh = Globals_Game.pcMasterList[cr.charID];
					success = true;
				}
			}
			else if (cr is NonPlayerCharacter_Riak)
			{
                if (Globals_Game.npcMasterList.ContainsKey(cr.charID))
				{
                    myCh = Globals_Game.npcMasterList[cr.charID];
					success = true;
				}
			}
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("goTo queue processing: Character not found (" + cr.charID + ")");
                }
            }

            if (success)
            {
                foreach (string value in cr.goTo)
                {
                    myCh.goTo.Enqueue(Globals_Game.fiefMasterList[value]);
                }
            }

			return success;
		}

		/// <summary>
		/// Converts Kingdom object into suitable format for JSON serialisation
		/// </summary>
        /// <returns>Kingdom_Riak object</returns>
        /// <param name="k">Kingdom to be converted</param>
        public Kingdom_Riak KingdomToRiak(Kingdom k)
		{
            Kingdom_Riak kingOut = null;
            kingOut = new Kingdom_Riak(k);
			return kingOut;
		}

        /// <summary>
        /// Converts Position object into suitable format for JSON serialisation
        /// </summary>
        /// <returns>Position_Riak object</returns>
        /// <param name="p">Position to be converted</param>
        public Position_Riak PositionToRiak(Position p)
        {
            Position_Riak posOut = null;
            posOut = new Position_Riak(p);
            return posOut;
        }

        /// <summary>
        /// Converts Province object into suitable format for JSON serialisation
        /// </summary>
        /// <returns>Province_Riak object</returns>
        /// <param name="p">Province to be converted</param>
        public Province_Riak ProvinceToRiak(Province p)
        {
            Province_Riak oOut = null;
            oOut = new Province_Riak(p);
            return oOut;
        }

        /// <summary>
        /// Converts Kingdom_Riak objects into Kingdom game objects
        /// </summary>
        /// <returns>Kingdom object</returns>
        /// <param name="kr">Kingdom_Riak to be converted</param>
        public Kingdom KingdomFromRiak(Kingdom_Riak kr)
        {
            Kingdom kOut = null;
            kOut = new Kingdom(kr);

            // insert king
            if (kr.owner != null)
            {
                if (Globals_Game.pcMasterList.ContainsKey(kr.owner))
                {
                    kOut.owner = Globals_Game.pcMasterList[kr.owner];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Kingdom " + kr.id + ": King not found (" + kr.owner + ")");
                    }
                }
            }

            // insert rank
            if (kr.rank != null)
            {
                if (Globals_Game.rankMasterList.ContainsKey(kr.rank))
                {
                    kOut.rank = Globals_Game.rankMasterList[kr.rank];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Kingdom " + kr.id + ": Rank not found (" + kr.rank + ")");
                    }
                }
            }

            // insert nationality
            if (kr.nationality != null)
            {
                if (Globals_Game.nationalityMasterList.ContainsKey(kr.nationality))
                {
                    kOut.nationality = Globals_Game.nationalityMasterList[kr.nationality];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Kingdom " + kr.id + ": Nationality not found (" + kr.nationality + ")");
                    }
                }
            }

            return kOut;
        }

        /// <summary>
		/// Converts Province_Riak objects into Province game objects
		/// </summary>
        /// <returns>Province object</returns>
        /// <param name="pr">Province_Riak to be converted</param>
		public Province ProvinceFromRiak(Province_Riak pr)
		{
			Province oOut = null;
			oOut = new Province (pr);

            // insert overlord using overlordID
			if (pr.owner != null)
			{
                if (Globals_Game.pcMasterList.ContainsKey(pr.owner))
                {
                    oOut.owner = Globals_Game.pcMasterList[pr.owner];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Province " + pr.id + ": Overlord not found (" + pr.owner + ")");
                    }
                }
            }

			// check if province is in owner's list of provinces owned
			bool provInList = oOut.owner.ownedProvinces.Any(item => item.id == oOut.id);
			// if not, add it
			if(! provInList)
			{
				oOut.owner.ownedProvinces.Add(oOut);
			}
				
            // insert kingdom using kingdomID
            if (pr.kingdom != null)
            {
                if (Globals_Game.kingdomMasterList.ContainsKey(pr.kingdom))
                {
                    oOut.kingdom = Globals_Game.kingdomMasterList[pr.kingdom];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Province " + pr.id + ": Kingdom not found (" + pr.kingdom + ")");
                    }
                }
            }

            // insert rank using rankID
			if (pr.rank > 0)
            {
                if (Globals_Game.rankMasterList.ContainsKey(pr.rank))
                {
                    oOut.rank = Globals_Game.rankMasterList[pr.rank];
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Province " + pr.id + ": Rank not found (" + pr.rank + ")");
                    }
                }
            }

            return oOut;
		}

		/// <summary>
		/// Writes key list (List object) to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="k">key of key list</param>
		/// <param name="kl">key list to write</param>
        public bool writeKeyList<T>(string gameID, string k, List<T> kl)
		{

			var rList = new RiakObject(gameID, k, kl);
			var putListResult = rClient.Put(rList);

			if (! putListResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Key list " + rList.Key + " to bucket " + rList.Bucket);
                }
			}

			return putListResult.IsSuccess;
		}

		/// <summary>
		/// Writes GameClock object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="gc">GameClock to write</param>
        public bool writeClock(string gameID, GameClock gc)
		{
			var rClock = new RiakObject(gameID, "gameClock", gc);
			var putClockResult = rClient.Put(rClock);

			if (! putClockResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: GameClock to bucket " + rClock.Bucket);
                }
			}

			return putClockResult.IsSuccess;
		}

        /// <summary>
        /// Writes Journal object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Riak key to use</param>
        /// <param name="journal">Journal to write</param>
        public bool writeJournal(string gameID, string key, Journal journal)
        {
            var rJournal = new RiakObject(gameID, key, journal);
            var putJournalResult = rClient.Put(rJournal);

            if (!putJournalResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Journal " + key + " to bucket " + rJournal.Bucket);
                }
            }

            return putJournalResult.IsSuccess;
        }

		/// <summary>
		/// Writes Character variable to Riak
		/// </summary>
		/// <returns>bool indicating success</returns>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="key">Riak key to use</param>
		/// <param name="pc">PlayerCharacter to write</param>
		public bool writeCharacterVar(string gameID, string key, PlayerCharacter pc)
		{
			String pcID = "";
			if (pc != null)
			{
				pcID = pc.charID;
			}
			pcID = "\"" + pcID + "\"";

			var rCharVar = new RiakObject(gameID, key, pcID);
			var putCharVarResult = rClient.Put(rCharVar);

			if (!putCharVarResult.IsSuccess)
			{
				if (Globals_Client.showMessages)
				{
					System.Windows.Forms.MessageBox.Show("Write failed: Character variable " + key + " to bucket " + rCharVar.Bucket);
				}
			}

			return putCharVarResult.IsSuccess;
		}

		/// <summary>
		/// Writes newID variable to Riak
		/// </summary>
		/// <returns>bool indicating success</returns>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="key">Riak key to use</param>
		/// <param name="newID">newID to write</param>
		public bool writeNewIDvar(string gameID, string key, uint newID)
		{
			var rCharVar = new RiakObject(gameID, key, newID);
			var putCharVarResult = rClient.Put(rCharVar);

			if (!putCharVarResult.IsSuccess)
			{
				if (Globals_Client.showMessages)
				{
					System.Windows.Forms.MessageBox.Show("Write failed: newID variable " + key + " to bucket " + rCharVar.Bucket);
				}
			}

			return putCharVarResult.IsSuccess;
		}

        /// <summary>
		/// Writes Dictionary object to Riak
		/// </summary>
		/// <returns>bool indicating success</returns>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="key">Riak key to use</param>
		/// <param name="dictionary">Dictionary to write</param>
        public bool writeDictionary<T>(string gameID, string key, T dictionary)
		{
			var rDict = new RiakObject(gameID, key, dictionary);
			var putDictResult = rClient.Put(rDict);

			if (! putDictResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Dictionary " + key + " to bucket " + rDict.Bucket);
                }
			}

			return putDictResult.IsSuccess;
		}

		/// <summary>
		/// Converts jEntryPriorities Dictionary into a format suitable for Riak
		/// </summary>
		/// <returns>Dictionary<string, byte> for Riak storage</returns>
		/// <param name="dictToConvert">The Dictionary to convert</param>
		public Dictionary<string, byte> jEntryPrioritiesToRiak(Dictionary<string[], byte> dictToConvert)
		{
			Dictionary<string, byte> dictOut = new Dictionary<string, byte> ();

			if (dictToConvert.Count > 0)
			{
				foreach (KeyValuePair<string[], byte> thisEntry in dictToConvert)
				{
					dictOut.Add (thisEntry.Key[0] + "|" + thisEntry.Key[1], thisEntry.Value);
				}
			}

			return dictOut;
		}

		/// <summary>
		/// Writes Skill object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="s">Skill to write</param>
        public bool writeSkill(string gameID, Skill s)
		{
			var rSkill = new RiakObject(gameID, s.skillID, s);
			var putSkillResult = rClient.Put(rSkill);

			if (! putSkillResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Skill " + rSkill.Key + " to bucket " + rSkill.Bucket);
                }
			}

			return putSkillResult.IsSuccess;
		}

		/// <summary>
        /// Writes NonPlayerCharacter or NonPlayerCharacter_Riak object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="npc">NonPlayerCharacter to write</param>
        /// <param name="npcr">NonPlayerCharacter_Riak to write</param>
        public bool writeNPC(string gameID, NonPlayerCharacter npc = null, NonPlayerCharacter_Riak npcr = null)
		{
            if (npc != null)
            {
                // convert NonPlayerCharacter into NonPlayerCharacter_Riak
                npcr = this.NPCtoRiak(npc);
            }

            // write NonPlayerCharacter_Riak to database
            var rNPC = new RiakObject(gameID, npcr.charID, npcr);
			var putNPCresult = rClient.Put(rNPC);

			if (! putNPCresult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: NPC " + rNPC.Key + " to bucket " + rNPC.Bucket);
                }
			}

			return putNPCresult.IsSuccess;
		}

		/// <summary>
        /// Writes PlayerCharacter or PlayerCharacter_Riak object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="pc">PlayerCharacter to write</param>
        /// <param name="pcr">PlayerCharacter_Riak to write</param>
        public bool writePC(string gameID, PlayerCharacter pc = null, PlayerCharacter_Riak pcr = null)
		{
            if (pc != null)
            {
                // convert PlayerCharacter into PlayerCharacter_Riak
                pcr = this.PCtoRiak(pc);
            }

            // write PlayerCharacter_Riak to database
            var rPC = new RiakObject(gameID, pcr.charID, pcr);
			var putPCresult = rClient.Put(rPC);

			if (! putPCresult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: PC " + rPC.Key + " to bucket " + rPC.Bucket);
                }
			}

			return putPCresult.IsSuccess;
		}

		/// <summary>
        /// Writes Kingdom or Kingdom_Riak object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">Kingdom to write</param>
        /// <param name="kr">Kingdom_Riak to write</param>
        public bool writeKingdom(string gameID, Kingdom k = null, Kingdom_Riak kr = null)
		{
            if (k != null)
            {
                // convert Kingdom into Kingdom_Riak
                kr = this.KingdomToRiak(k);
            }

			var rKing = new RiakObject(gameID, kr.id, kr);
			var putKingResult = rClient.Put(rKing);

			if (! putKingResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Kingdom " + rKing.Key + " to bucket " + rKing.Bucket);
                }
			}

			return putKingResult.IsSuccess;
		}

        /// <summary>
        /// Writes Position or Position_Riak object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Position to write</param>
        /// <param name="pr">Position_Riak to write</param>
        public bool writePosition(string gameID, Position p = null, Position_Riak pr = null)
        {
            if (p != null)
            {
                // convert Position into Position_Riak
                pr = this.PositionToRiak(p);
            }

            var rPos = new RiakObject(gameID, pr.id.ToString(), pr);
            var putPosResult = rClient.Put(rPos);

            if (!putPosResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Position " + rPos.Key + " to bucket " + rPos.Bucket);
                }
            }

            return putPosResult.IsSuccess;
        }

        /// <summary>
        /// Writes Province or Province_Riak object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Province to write</param>
        /// <param name="pr">Province_Riak to write</param>
        public bool writeProvince(string gameID, Province p = null, Province_Riak pr = null)
        {
            if (p != null)
            {
                // convert Province into Province_Riak
                pr = this.ProvinceToRiak(p);
            }

            var rProv = new RiakObject(gameID, pr.id, pr);
            var putProvResult = rClient.Put(rProv);

            if (!putProvResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Province " + rProv.Key + " to bucket " + rProv.Bucket);
                }
            }

            return putProvResult.IsSuccess;
        }

        /// <summary>
        /// Writes BaseLanguage object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="bl">BaseLanguage to write</param>
        public bool writeBaseLanguage(string gameID, BaseLanguage bl)
        {

            var rBaseLanguage = new RiakObject(gameID, bl.id, bl);
            var putBaseLanguageResult = rClient.Put(rBaseLanguage);

            if (!putBaseLanguageResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: BaseLanguage " + rBaseLanguage.Key + " to bucket " + rBaseLanguage.Bucket);
                }
            }

            return putBaseLanguageResult.IsSuccess;
        }

        /// <summary>
        /// Writes Language or Language_Riak object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="l">Language to write</param>
        /// <param name="lr">Language_Riak to write</param>
        public bool writeLanguage(string gameID, Language l = null, Language_Riak lr = null)
		{
            if (l != null)
            {
                // convert Language into Language_Riak
                lr = this.LangToRiak(l);
            }

            // write Language_Riak to database
            var rLanguage = new RiakObject(gameID, lr.id, lr);
			var putLanguageResult = rClient.Put(rLanguage);

			if (! putLanguageResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Language " + rLanguage.Key + " to bucket " + rLanguage.Bucket);
                }
			}

			return putLanguageResult.IsSuccess;
		}

        /// <summary>
        /// Writes Nationality object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="n">Nationality to write</param>
        public bool writeNationality(string gameID, Nationality n)
        {

            var rNationality = new RiakObject(gameID, n.natID, n);
            var putNationalityResult = rClient.Put(rNationality);

            if (!putNationalityResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Nationality " + rNationality.Key + " to bucket " + rNationality.Bucket);
                }
            }

            return putNationalityResult.IsSuccess;
        }

        /// <summary>
        /// Writes Rank object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="r">Rank to write</param>
        public bool writeRank(string gameID, Rank r)
        {

            var rRank = new RiakObject(gameID, r.id.ToString(), r);
            var putRankResult = rClient.Put(rRank);

            if (!putRankResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Rank " + rRank.Key + " to bucket " + rRank.Bucket);
                }
            }

            return putRankResult.IsSuccess;
        }

        /// <summary>
        /// Writes VictoryData object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="vicDat">VictoryData to write</param>
        public bool writeVictoryData(string gameID, VictoryData vicDat)
        {

			var rVictoryData = new RiakObject(gameID, vicDat.playerID, vicDat);
            var putVictoryDataResult = rClient.Put(rVictoryData);

            if (!putVictoryDataResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: VictoryData " + rVictoryData.Key + " to bucket " + rVictoryData.Bucket);
                }
            }

            return putVictoryDataResult.IsSuccess;
        }

        /// <summary>
        /// Writes Terrain object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="t">Terrain to write</param>
        public bool writeTerrain(string gameID, Terrain t)
        {

            var rTerrain = new RiakObject(gameID, t.terrainCode, t);
            var putTerrainResult = rClient.Put(rTerrain);

            if (!putTerrainResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Terrain " + rTerrain.Key + " to bucket " + rTerrain.Bucket);
                }
            }

            return putTerrainResult.IsSuccess;
        }

        /// <summary>
        /// Writes Fief or Fief_Riak object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="f">Fief to write</param>
        /// <param name="fr">Fief_Riak to write</param>
        public bool writeFief(string gameID, Fief f = null, Fief_Riak fr = null)
		{
            if (f != null)
            {
                // convert Fief to Fief_Riak 
                fr = this.FieftoRiak(f);
            }

			var rFief = new RiakObject(gameID, fr.id, fr);
			var putFiefResult = rClient.Put(rFief);

			if (! putFiefResult.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Fief " + rFief.Key + " to bucket " + rFief.Bucket);
                }
			}

			return putFiefResult.IsSuccess;
		}

        /// <summary>
        /// Writes Army object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="a">Army to write</param>
        public bool writeArmy(string gameID, Army a)
        {
            var rArmy = new RiakObject(gameID, a.armyID, a);
            var putArmyResult = rClient.Put(rArmy);

            if (!putArmyResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Army " + rArmy.Key + " to bucket " + rArmy.Bucket);
                }
            }

            return putArmyResult.IsSuccess;
        }

        /// <summary>
        /// Writes Siege object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="s">Siege to write</param>
        public bool writeSiege(string gameID, Siege s)
        {
            var rSiege = new RiakObject(gameID, s.siegeID, s);
            var putSiegeResult = rClient.Put(rSiege);

            if (!putSiegeResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Siege " + rSiege.Key + " to bucket " + rSiege.Bucket);
                }
            }

            return putSiegeResult.IsSuccess;
        }

        /// <summary>
		/// Writes HexMapGraph edges collection to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="map">HexMapGraph containing edges collection to write</param>
        /// <param name="edges">Edges collection to write</param>
        public bool writeMapEdges(string gameID, HexMapGraph map = null, List<TaggedEdge<string, string>> edges = null)
		{
            if (map != null)
            {
                // convert Language into Language_Riak
                edges = this.EdgeCollection_to_Riak(map.myMap.Edges.ToList());
            }

            var rMapE = new RiakObject(gameID, "mapEdges", edges);
			var putMapResultE = rClient.Put(rMapE);

			if (! putMapResultE.IsSuccess)
			{
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Map edges collection " + rMapE.Key + " to bucket " + rMapE.Bucket);
                }
			}

			return putMapResultE.IsSuccess;
		}

        /// <summary>
		/// Updates game objects at end/start of season
		/// </summary>
        /// <param name="type">Specifies type of update to perform</param>
        public void seasonUpdate(string type = "full")
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
                        if ((npcEntry.Value.myBoss == null) && (npcEntry.Value.familyID == null))
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
                    this.siegeEnd(dissolvedSieges[i], siegeDescription);
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

            // CHECK SCHEDULED EVENTS
            if (!endDateReached)
            {
                List<JournalEntry> entriesForRemoval = this.processScheduledEvents();
                // remove processed events from Globals_Game.scheduledEvents
                for (int i = 0; i < entriesForRemoval.Count; i++)
                {
                    Globals_Game.scheduledEvents.entries.Remove(entriesForRemoval[i].jEntryID);
                }
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

            // iterate through clock's scheduled events
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

                        // do conditional checks
                        // death of mother
                        if (!mummy.isAlive)
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
                                if (((bride.location.siege != null) && (bride.inKeep))
                                    || ((groom.location.siege != null) && (groom.inKeep)))
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

        }

        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to move</param>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        public bool moveCharacter(Character ch, Fief target, double cost)
        {
            bool success = false;
            bool proceedWithMove = true;

            // check to see if character is leading a besieging army
            Army myArmy = ch.getArmy();
            if (myArmy != null)
            {
                string thisSiegeID = myArmy.checkIfBesieger();
                if (thisSiegeID != null)
                {
                    // give player fair warning of consequences to siege
                    DialogResult dialogResult = MessageBox.Show("Your army is currently besieging this fief.  Moving will end the siege.\r\nClick 'OK' to proceed.", "Proceed with move?", MessageBoxButtons.OKCancel);

                    // if choose to cancel
                    if (dialogResult == DialogResult.Cancel)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Move cancelled.");
                        }
                        proceedWithMove = false;
                    }

                    // if choose to proceed
                    else
                    {
                        // end the siege
                        Siege thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Siege (" + thisSiegeID + ") ended.");
                        }

                        // construct event description to be passed into siegeEnd
                        string siegeDescription = "On this day of Our Lord the forces of ";
                        siegeDescription += thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                        siegeDescription += " have chosen to abandon the siege of " + thisSiege.getFief().name;
                        siegeDescription += ". " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                        siegeDescription += " retains ownership of the fief.";

                        this.siegeEnd(thisSiege, siegeDescription);
                    }

                }
            }

            if (proceedWithMove)
            {
                // move character
                success = ch.moveCharacter(target, cost);
            }

            return success;
        }
        
        /// <summary>
        /// Moves an NPC without a boss one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="npc">NPC to move</param>
        public bool randomMoveNPC(NonPlayerCharacter npc)
        {
            bool success = false;

            // generate random int 0-6 to see if moves
            int randomInt = Globals_Game.myRand.Next(7);

            if (randomInt > 0)
            {
                // get a destination
                Fief target = Globals_Game.gameMap.chooseRandomHex(npc.location);

                // get travel cost
                double travelCost = this.getTravelCost(npc.location, target);

                // perform move
                success = this.moveCharacter(npc, target, travelCost);
            }

            return success;
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
            // fiefs
            this.provinceFiefListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Current GDP", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Last season tax income", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("", -2, HorizontalAlignment.Left);
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
        /// Creates UI display for list of armies owned by player
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
        /// Refreshes display of PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void refreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            ListViewItem[] fiefsOwned = new ListViewItem[Globals_Client.myPlayerCharacter.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < Globals_Client.myPlayerCharacter.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                // name
                fiefsOwned[i] = new ListViewItem(Globals_Client.myPlayerCharacter.ownedFiefs[i].name);

                // ID
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].id);

                // current location
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i] == Globals_Client.myPlayerCharacter.location)
                {
                    fiefsOwned[i].SubItems.Add("You are here");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // home fief
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i].id.Equals(Globals_Client.myPlayerCharacter.homeFief))
                {
                    fiefsOwned[i].SubItems.Add("Home");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // add item to fiefsListView
                this.fiefsListView.Items.Add(fiefsOwned[i]);
            }
        }

        /// <summary>
        /// Refreshes UI Court, Tavern, outside keep display
        /// </summary>
        /// <param name="place">string specifying whether court, tavern, outside keep</param>
        public void refreshMeetingPlaceDisplay(string place)
        {
            // refresh general information
            this.meetingPlaceDisplayText();

            // remove any previously displayed characters
            this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
            this.meetingPlaceCharDisplayTextBox.Text = "";

            // disable controls until character selected in list
            this.hireNPC_Btn.Enabled = false;
            this.hireNPC_TextBox.Text = "";
            this.hireNPC_TextBox.Enabled = false;
            this.meetingPlaceMoveToBtn.Enabled = false;
            this.meetingPlaceMoveToTextBox.Text = "";
            this.meetingPlaceMoveToTextBox.Enabled = false;
            this.meetingPlaceRouteBtn.Enabled = false;
            this.meetingPlaceRouteTextBox.Text = "";
            this.meetingPlaceRouteTextBox.Enabled = false;
            this.meetingPlaceEntourageBtn.Enabled = false;
            this.meetingPlaceProposeBtn.Enabled = false;
            this.meetingPlaceProposeTextBox.Text = "";
            this.meetingPlaceProposeTextBox.Enabled = false;

            // set label
            switch (place)
            {
                case "tavern":
                    this.meetingPlaceLabel.Text = "Ye Olde Tavern Of " + Globals_Client.fiefToView.name;
                    break;
                case "court":
                    this.meetingPlaceLabel.Text = "The Esteemed Court Of " + Globals_Client.fiefToView.name;
                    break;
                case "outsideKeep":
                    this.meetingPlaceLabel.Text = "Persons Present Outwith\r\nThe Keep Of " + Globals_Client.fiefToView.name;
                    break;
                default:
                    this.meetingPlaceLabel.Text = "A Generic Meeting Place";
                    break;
            }
            // refresh list of characters
            this.meetingPlaceDisplayList(place);
        }

        /// <summary>
        /// Refreshes general information displayed in Court, Tavern, outside keep
        /// </summary>
        public void meetingPlaceDisplayText()
        {
            string textToDisplay = "";
            // date/season and main character's days left
            textToDisplay += Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear + ".  Your days left: " + Globals_Client.myPlayerCharacter.days + "\r\n\r\n";
            // Fief name/ID and province name
            textToDisplay += "Fief: " + Globals_Client.myPlayerCharacter.location.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")  in " + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name + "\r\n\r\n";
            // Fief owner
            textToDisplay += "Owner: " + Globals_Client.myPlayerCharacter.location.owner.firstName + " " + Globals_Client.myPlayerCharacter.location.owner.familyName + "\r\n";
            // Fief overlord
            textToDisplay += "Overlord: " + Globals_Client.myPlayerCharacter.location.getOverlord().firstName + " " + Globals_Client.myPlayerCharacter.location.getOverlord().familyName + "\r\n";

            this.meetingPlaceTextBox.ReadOnly = true;
            this.meetingPlaceTextBox.Text = textToDisplay;
        }

        /// <summary>
        /// Refreshes display of Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        public void meetingPlaceDisplayList(string place)
        {
            // clear existing items in list
            this.meetingPlaceCharsListView.Items.Clear();

            // select which characters to display - i.e. in the keep (court) or not (tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            // iterates through characters
            for (int i = 0; i < Globals_Client.myPlayerCharacter.location.charactersInFief.Count; i++)
            {
                ListViewItem charsInCourt = null;

                // only display characters in relevant location (in keep, or not)
                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] != Globals_Client.myPlayerCharacter)
                    {

                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((Globals_Client.myPlayerCharacter.location.charactersInFief[i] as NonPlayerCharacter).wage == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                break;
                        }

                    }
                }

                // add item to fiefsListView
                if (charsInCourt != null)
                {
                    this.meetingPlaceCharsListView.Items.Add(charsInCourt);
                }
            }
        }

        /// <summary>
        /// Creates item for Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        /// <returns>ListViewItem containing character details</returns>
        public ListViewItem createMeetingPlaceListItem(Character ch)
        {
            // name
            ListViewItem myItem = new ListViewItem(ch.firstName + " " + ch.familyName);

            // charID
            myItem.SubItems.Add(ch.charID);

            // sex
            if (ch.isMale)
            {
                myItem.SubItems.Add("Male");
            }
            else
            {
                myItem.SubItems.Add("Female");
            }

            // to store household and type data
            string myHousehold = "";
            string myType = "";
            bool isEmployee = false;
            bool isFamily = false;

            // household
            if (ch.familyID != null)
            {
                myHousehold = ch.getHeadOfFamily().familyName + " (ID: " + ch.familyID + ")";

                if (ch.familyID.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isFamily = true;
                }
            }
            else if ((ch as NonPlayerCharacter).myBoss != null)
            {
                myHousehold = (ch as NonPlayerCharacter).getEmployer().familyName + " (ID: " + (ch as NonPlayerCharacter).myBoss + ")";
            }

            myItem.SubItems.Add(myHousehold);

            // type (e.g. family, NPC, player)

            // check for players and PCs
            if (ch is PlayerCharacter)
            {
                if ((ch as PlayerCharacter).playerID != null)
                {
                    myType = "PC (player)";
                }
                else
                {
                    myType = "PC (inactive)";
                }
            }
            else
            {
                // check for employees
                if (((ch as NonPlayerCharacter).myBoss != null) && (ch as NonPlayerCharacter).myBoss.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isEmployee = true;
                }

                // allocate NPC type
                if ((isFamily) || (isEmployee))
                {
                    myType = "My ";
                }
                if (isFamily)
                {
                    myType += "Family";
                }
                else if (isEmployee)
                {
                    myType += "Employee";
                }
                else
                {
                    myType = "NPC";
                }
            }

            myItem.SubItems.Add(myType);

            // show whether is in player's entourage
            bool isCompanion = false;

            if (ch is NonPlayerCharacter)
            {
                // iterate through employees checking for character
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == ch)
                    {
                        if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                        {
                            isCompanion = true;
                        }
                    }
                }
            }

            if (isCompanion)
            {
                myItem.SubItems.Add("Yes");
            }

            return myItem;
        }

        /// <summary>
        /// Refreshes Household display
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to display</param>
        public void refreshHouseholdDisplay(NonPlayerCharacter npc = null)
        {
            // set main character display as read only
            this.houseCharTextBox.ReadOnly = true;

            // disable controls until NPC selected in ListView
            this.houseCampBtn.Enabled = false;
            this.houseCampDaysTextBox.Enabled = false;
            this.familyNameChildButton.Enabled = false;
            this.familyNameChildTextBox.Enabled = false;
            this.familyNpcSpousePregBtn.Enabled = false;
            this.houseHeirBtn.Enabled = false;
            this.houseMoveToBtn.Enabled = false;
            this.houseMoveToTextBox.Enabled = false;
            this.houseRouteBtn.Enabled = false;
            this.houseEntourageBtn.Enabled = false;
            this.houseFireBtn.Enabled = false;
            this.houseExamineArmiesBtn.Enabled = false;

            // remove any previously displayed text
            this.houseCharTextBox.Text = "";
            this.houseCampDaysTextBox.Text = "";
            this.familyNameChildTextBox.Text = "";
            this.houseMoveToTextBox.Text = "";
            this.houseRouteTextBox.Text = "";
            this.houseProposeBrideTextBox.Text = "";
            
            // clear existing items in characters list
            this.houseCharListView.Items.Clear();

            // variables needed for name check (to see if NPC needs naming)
            string nameWarning = "The following offspring need to be named:\r\n\r\n";
            bool showNameWarning = false;

            // iterates through household characters adding information to ListView
            // and checking if naming is required
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                ListViewItem houseChar = null;

                // name
                houseChar = new ListViewItem(Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName);

                // charID
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].charID);

                // function (e.g. employee, son, wife, etc.)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].getFunction(Globals_Client.myPlayerCharacter));

                // responsibilities (i.e. jobs)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].getResponsibilities(Globals_Client.myPlayerCharacter));

                // location
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].location.id + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].location.name + ")");

                // show whether is in player's entourage
                if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                {
                    houseChar.SubItems.Add("Yes");
                }

                // check if needs to be named
                if (Globals_Client.myPlayerCharacter.myNPCs[i].familyID != null)
                {
                    bool nameRequired = Globals_Client.myPlayerCharacter.myNPCs[i].checkForName(0);

                    if (nameRequired)
                    {
                        showNameWarning = true;
                        nameWarning += " - " + Globals_Client.myPlayerCharacter.myNPCs[i].charID + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName + ")\r\n";
                    }
                }

                if (houseChar != null)
                {
                    // if NPC passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == npc)
                    {
                        houseChar.Selected = true;
                    }

                    // add item to fiefsListView
                    this.houseCharListView.Items.Add(houseChar);
                }

            }

            // always enable marriage proposal controls
            this.houseProposeBtn.Enabled = true;
            this.houseProposeBrideTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Text = Globals_Client.myPlayerCharacter.charID;

            this.houseCharListView.HideSelection = false;
            this.houseCharListView.Focus();

            if (showNameWarning)
            {
                nameWarning += "\r\nAny children who are not named by the age of one will,\r\nwhere possible, be named after their royal highnesses the king and queen.";
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(nameWarning);
                }
            }
        }

        /// <summary>
        /// Refreshes royal gifts display
        /// </summary>
        public void refreshRoyalGiftsContainer()
        {
            // get PlayerCharacter (to allow for herald viewing king's finances)
            PlayerCharacter thisKing = null;
            if (Globals_Client.myPlayerCharacter.checkIsKing())
            {
                thisKing = Globals_Client.myPlayerCharacter;
            }
            else if (Globals_Client.myPlayerCharacter.checkIsHerald())
            {
                thisKing = Globals_Client.myPlayerCharacter.getKing();
            }

            if (thisKing != null)
            {
                // to store financial data
                int totGDP = 0;
                int provTaxInc = 0;
                int totTaxInc = 0;
                int totTreas = 0;
                double taxRate = 0;

                // disable controls until place selected in ListView
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
                this.royalGiftsPositionRemoveBtn.Enabled = false;

                // remove any previously displayed text

                // clear existing items in places lists
                this.royalGiftsProvListView.Items.Clear();
                this.royalGiftsFiefListView.Items.Clear();
                this.royalGiftsPositionListView.Items.Clear();

                // iterates through owned provinces and fiefs, adding information to appropriate ListView
                // PROVINCES
                foreach (Province thisProvince in thisKing.ownedProvinces)
                {
                    ListViewItem provItem = null;

                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // title holder
                    // get character
                    PlayerCharacter thisHolder = null;
					//System.Windows.Forms.MessageBox.Show("Got here!");
                    if (Globals_Game.pcMasterList.ContainsKey(thisProvince.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisProvince.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        provItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate and total tax income
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            taxRate = fiefEntry.Value.keyStatsCurrent[12];
                            // update tax income total for province
                            provTaxInc += Convert.ToInt32(fiefEntry.Value.keyStatsCurrent[11]);
                        }
                    }

                    // add tax rate subitem
                    provItem.SubItems.Add(taxRate.ToString());

                    // add tax income subitem
                    provItem.SubItems.Add("£" + provTaxInc);

                    // update total tax income for all provinces
                    totTaxInc += provTaxInc;

                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsProvListView.Items.Add(provItem);
                    }

                }

                // add listviewitem with total tax income (all provinces)
                string[] provItemTotalSubs = new string[] { "", "", "", "", "£" + totTaxInc };
                ListViewItem provItemTotal = new ListViewItem(provItemTotalSubs);
                this.royalGiftsProvListView.Items.Add(provItemTotal);

                // FIEFS
                totTaxInc = 0;
                foreach (Fief thisFief in thisKing.ownedFiefs)
                {
                    ListViewItem fiefItem = null;

                    // id
                    fiefItem = new ListViewItem(thisFief.id);

                    // name
                    fiefItem.SubItems.Add(thisFief.name);

                    // province name
                    fiefItem.SubItems.Add(thisFief.province.name);

                    // title holder
                    // get character
                    Character thisHolder = null;
                    if (Globals_Game.pcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisFief.titleHolder];
                    }
                    else if (Globals_Game.npcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.npcMasterList[thisFief.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        fiefItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // gdp
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[1]);
                    // update GDP total
                    totGDP += Convert.ToInt32(thisFief.keyStatsCurrent[1]);

                    // last tax income
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[11]);
                    // update tax income total
                    totTaxInc += Convert.ToInt32(thisFief.keyStatsCurrent[11]);

                    // treasury
                    fiefItem.SubItems.Add("£" + thisFief.treasury);
                    // update treasury total
                    totTreas += thisFief.treasury;

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsFiefListView.Items.Add(fiefItem);
                    }

                }

                // add listviewitem with total GDP and tax income (all fiefs)
                string[] fiefItemTotalSubs = new string[] { "", "", "", "", "£" + totGDP, "£" + totTaxInc, "£" + totTreas };
                ListViewItem fiefItemTotal = new ListViewItem(fiefItemTotalSubs);
                this.royalGiftsFiefListView.Items.Add(fiefItemTotal);

                // POSITIONS
                foreach (KeyValuePair<byte, Position> thisPos in Globals_Game.positionMasterList)
                {
                    // only list posistions for this nationality
                    if (thisPos.Value.nationality == thisKing.nationality)
                    {
                        ListViewItem posItem = null;

                        // id
                        posItem = new ListViewItem(thisPos.Value.id.ToString());

                        // name
                        posItem.SubItems.Add(thisPos.Value.getName(thisKing.language));

                        // stature
                        posItem.SubItems.Add(thisPos.Value.stature.ToString());

                        // holder
                        // get character
                        Character thisHolder = null;
                        if (thisPos.Value.officeHolder != null)
                        {
                            if (Globals_Game.pcMasterList.ContainsKey(thisPos.Value.officeHolder))
                            {
                                thisHolder = Globals_Game.pcMasterList[thisPos.Value.officeHolder];
                            }
                        }

                        // title holder name & id
                        if (thisHolder != null)
                        {
                            posItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                        }
                        else
                        {
                            posItem.SubItems.Add(" - ");
                        }

                        if (posItem != null)
                        {
                            // add item to royalGiftsPositionListView
                            this.royalGiftsPositionListView.Items.Add(posItem);
                        }
                    }
                }

                Globals_Client.containerToView = this.royalGiftsContainer;
                Globals_Client.containerToView.BringToFront();
            }

        }

        /// <summary>
        /// Refreshes overlord province management display
        /// </summary>
        /// <param name="province">Province to display</param>
        public void refreshProvinceContainer(Province province = null)
        {
            // disable controls until place selected in ListView
            this.provinceTaxBtn.Enabled = false;
            this.provinceTaxTextBox.Enabled = false;

            // remove any previously displayed text
            this.provinceTaxTextBox.Text = "";

            // clear existing items in places lists
            this.provinceProvListView.Items.Clear();
            this.provinceFiefListView.Items.Clear();

            // iterates through provinces where the character holds the title, adding information to ListView
            foreach (string placeID in Globals_Client.myPlayerCharacter.myTitles)
            {
                ListViewItem provItem = null;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisProvince = Globals_Game.provinceMasterList[placeID];
                }

                if (thisProvince != null)
                {
                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // owner
                    // get character
                    PlayerCharacter thisOwner = null;
                    if (Globals_Game.pcMasterList.ContainsKey(thisProvince.titleHolder))
                    {
                        thisOwner = Globals_Game.pcMasterList[thisProvince.titleHolder];
                    }

                    // title holder name & id
                    if (thisOwner != null)
                    {
                        provItem.SubItems.Add(thisOwner.firstName + " " + thisOwner.familyName + "(" + thisOwner.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate
                    // get a fief
                    Fief thisFief = null;
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            thisFief = fiefEntry.Value;
                            break;
                        }
                    }

                    // get tax rate from fief
                    if (thisFief != null)
                    {
                        provItem.SubItems.Add(thisFief.keyStatsCurrent[12].ToString());
                    }

                    // see if province to view has been passed in
                    if (province != null)
                    {
                        if (province == thisProvince)
                        {
                            provItem.Selected = true;
                        }
                    }
                    
                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceProvListView.Items.Add(provItem);
                    }
                }

            }

            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Refreshes information the fief list in the overlord's province management display
        /// </summary>
        public void refreshProvinceFiefList(Province p)
        {
            bool underOccupation = false;

            // clear existing items in list
            this.provinceFiefListView.Items.Clear();

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                ListViewItem fiefItem = null;

                if (fiefEntry.Value.province == p)
                {
                    // check for enemy occupation
                    underOccupation = fiefEntry.Value.checkEnemyOccupation();

                    // id
                    fiefItem = new ListViewItem(fiefEntry.Value.id);

                    // name
                    fiefItem.SubItems.Add(fiefEntry.Value.name);

                    // owner
                    // get character
                    PlayerCharacter thisOwner = null;
                    if (Globals_Game.pcMasterList.ContainsKey(fiefEntry.Value.titleHolder))
                    {
                        thisOwner = Globals_Game.pcMasterList[fiefEntry.Value.titleHolder];
                    }

                    // owner name & id
                    if (thisOwner != null)
                    {
                        fiefItem.SubItems.Add(thisOwner.firstName + " " + thisOwner.familyName + "(" + thisOwner.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // GDP
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[1]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // last tax income
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[11]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // check if underOccupation message needed
                    if (underOccupation)
                    {
                        fiefItem.SubItems.Add("Under enemy occupation!");
                        fiefItem.ForeColor = Color.Red;
                    }

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceFiefListView.Items.Add(fiefItem);
                    }
                }

            }
        }

        /// <summary>
        /// Retrieves information for journal display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="indexPosition">The index position of the journal entry to be displayed</param>
        public string displayJournalEntry(int indexPosition)
        {
            string jentryText = "";

            // get journal entry
            JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(indexPosition).Value;

            // get text
            jentryText = thisJentry.getJournalEntryDetails();

            return jentryText;
        }

        /// <summary>
        /// Retrieves information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="ch">Character whose information is to be displayed</param>
        public string displayCharacter(Character ch)
        {
            string charText = "";
            bool isMyNPC = false;

            // check if is in player's myNPCs
            if (Globals_Client.myPlayerCharacter.myNPCs.Contains((ch as NonPlayerCharacter)) || (ch == Globals_Client.myPlayerCharacter))
            {
                isMyNPC = true;
            }

            // check to see if is army leader
            if (ch.armyID != null)
            {
                charText += "NOTE: This character is currently LEADING AN ARMY (" + ch.armyID + ")\r\n\r\n";
            }

            // check to see if is under siege
            if (ch.location.siege != null)
            {
                if (ch.inKeep)
                {
                    charText += "NOTE: This character is located in a KEEP UNDER SIEGE\r\n\r\n";
                }
            }

            // character ID
            charText += "Character ID: " + ch.charID + "\r\n";

            // player ID
            if (ch is PlayerCharacter)
            {
                if ((ch as PlayerCharacter).playerID != null)
                {
                    charText += "Player ID: " + (ch as PlayerCharacter).playerID + "\r\n";
                }
            }
            
            // name
            charText += "Name: " + ch.firstName + " " + ch.familyName + "\r\n";

            // age
            charText += "Age: " + ch.calcCharAge() + "\r\n";

            // sex
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

            // nationality
			charText += "Nationality: " + ch.nationality.name + "\r\n";

            if (ch is PlayerCharacter)
            {
                // home fief
                Fief homeFief = (ch as PlayerCharacter).getHomeFief();
                charText += "Home fief: " + homeFief.name + " (" + homeFief.id + ")\r\n";

                // ancestral home fief
                Fief ancHomeFief = (ch as PlayerCharacter).getAncestralHome();
                charText += "Ancestral Home fief: " + ancHomeFief.name + " (" + ancHomeFief.id + ")\r\n";
            }

            if (isMyNPC)
            {
                // health (& max. health)
                charText += "Health: ";
                if (!ch.isAlive)
                {
                    charText += "You're Dead!";
                }
                else
                {
                    charText += ch.calculateHealth() + " (max. health: " + ch.maxHealth + ")";
                }
                charText += "\r\n";

                // any death modifiers (from skills)
                charText += "  (Death modifier from skills: " + ch.calcSkillEffect("death") + ")\r\n";

                // virility
                charText += "Virility: " + ch.virility + "\r\n";
            }

            // location
            charText += "Current location: " + ch.location.name + " (" + ch.location.province.name + ")\r\n";
            
            // language
            charText += "Language: " + ch.language.getName() + "\r\n";

            if (isMyNPC)
            {
                // days left
                charText += "Days remaining: " + ch.days + "\r\n";
            }

            // stature
            charText += "Stature: " + ch.calculateStature() + "\r\n";
            charText += "  (base stature: " + ch.calculateStature(false) + " | modifier: " + ch.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + ch.management + "\r\n";

            // combat rating
            charText += "Combat: " + ch.combat + "\r\n";

            // skills list
            charText += "Skills:\r\n";
            for (int i = 0; i < ch.skills.Length; i++)
            {
                charText += "  - " + ch.skills[i].Item1.name + " (level " + ch.skills[i].Item2 + ")\r\n";
            }

            // whether inside/outside the keep
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

            // marital status
            charText += "You are ";
            if (ch.spouse != null)
            {
                charText += "happily married\r\n";
                // spouse ID
                charText += "Your spouse's ID is: " + ch.spouse;
            }
            else
            {
                charText += "not married.";
            }
            charText += "\r\n";

            // if pregnant
            if (!ch.isMale)
            {
                charText += "You are ";
                if (!ch.isPregnant)
                {
                    charText += "not ";
                }
                charText += "pregnant\r\n";
            }

            // if spouse pregnant
            else
            {
                if (ch.spouse != null)
                {
                    NonPlayerCharacter thisSpouse = Globals_Game.npcMasterList[ch.spouse];
                    if (thisSpouse.isPregnant)
                    {
                        charText += "Your spouse is pregnant (congratulations!)\r\n";
                    }
                    else
                    {
                        charText += "Your spouse is not pregnant\r\n";
                    }
                }
            }

            // engaged
            charText += "You are ";
            if (ch.fiancee != null)
            {
                charText += "engaged to be married to ID " + ch.fiancee;
            }
            else
            {
                charText += "not engaged to be married";
            }
            charText += "\r\n";

            // father
            charText += "Father's ID: ";
            if (ch.father != null)
            {
                charText += ch.father;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // mother
            charText += "Mother's ID: ";
            if (ch.mother != null)
            {
                charText += ch.mother;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // head of family
            charText += "Head of family's ID: ";
            if (ch.familyID != null)
            {
                charText += ch.familyID;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // gather additional information for PC/NPC
            bool isPC = ch is PlayerCharacter;
            if (isPC)
            {
                if (isMyNPC)
                {
                    charText += this.displayPlayerCharacter((PlayerCharacter)ch);
                }
            }
            else
            {
                charText += this.displayNonPlayerCharacter((NonPlayerCharacter)ch);
            }


            // if TITLES are to be shown
            if (this.characterTitlesCheckBox.Checked)
            {
                charText += "\r\n\r\n------------------ TITLES ------------------\r\n\r\n";

                // kingdoms
                foreach (string titleEntry in ch.myTitles)
                {
                    // get kingdom
                    Place thisPlace = null;

                    if (Globals_Game.kingdomMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.kingdomMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.getName(ch.language) + " (rank " + thisPlace.rank.id + ") of ";
                        // get kingdom details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }

                // provinces
                foreach (string titleEntry in ch.myTitles)
                {
                    // get province
                    Place thisPlace = null;

                    if (Globals_Game.provinceMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.provinceMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.getName(ch.language) + " (rank " + thisPlace.rank.id + ") of ";
                        // get province details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }

                // fiefs
                foreach (string titleEntry in ch.myTitles)
                {
                    // get fief
                    Place thisPlace = null;

                    if (Globals_Game.fiefMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.fiefMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.getName((thisPlace as Fief).language) + " (rank " + thisPlace.rank.id + ") of ";
                        // get fief details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }
            }

            return charText;
        }

        /// <summary>
        /// Retrieves PlayerCharacter-specific information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="pc">PlayerCharacter whose information is to be displayed</param>
        public string displayPlayerCharacter(PlayerCharacter pc)
        {
            string pcText = "";

            // whether outlawed
            pcText += "You are ";
            if (!pc.outlawed)
            {
                pcText += "not ";
            }
            pcText += "outlawed\r\n";

            // purse
            pcText += "Purse: " + pc.purse + "\r\n";

            // employees
            pcText += "Employees:\r\n";
            for (int i = 0; i < pc.myNPCs.Count; i++)
            {
                pcText += "  - " + pc.myNPCs[i].firstName + " " + pc.myNPCs[i].familyName;
                if (pc.myNPCs[i].inEntourage)
                {
                    pcText += " (travelling companion)";
                }
                pcText += "\r\n";
            }

            // owned fiefs
            pcText += "Fiefs owned:\r\n";
            for (int i = 0; i < pc.ownedFiefs.Count; i++)
            {
                pcText += "  - " + pc.ownedFiefs[i].name + "\r\n";
            }

			// owned provinces
			pcText += "Provinces owned:\r\n";
			for (int i = 0; i < pc.ownedProvinces.Count; i++)
			{
				pcText += "  - " + pc.ownedProvinces[i].name + "\r\n";
			}

            return pcText;
        }

        /// <summary>
        /// Retrieves NonPlayerCharacter-specific information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string displayNonPlayerCharacter(NonPlayerCharacter npc)
        {
            string npcText = "";

            // boss
            if (npc.myBoss != null)
            {
                npcText += "Hired by (ID): " + npc.myBoss + "\r\n";
            }

            // estimated salary level (if character is male)
            if (npc.isMale)
            {
                npcText += "Potential salary: " + npc.calcWage(Globals_Client.myPlayerCharacter) + "\r\n";

                // most recent salary offer from player (if any)
                npcText += "Last offer from this PC: ";
                if (npc.lastOffer.ContainsKey(Globals_Client.myPlayerCharacter.charID))
                {
                    npcText += npc.lastOffer[Globals_Client.myPlayerCharacter.charID];
                }
                else
                {
                    npcText += "N/A";
                }
                npcText += "\r\n";

                // current salary
                npcText += "Current salary: " + npc.wage + "\r\n";
            }

            /*
            // function
            if (npc.function != null)
            {
                npcText += "Function: " + npc.function;
            }
            else
            {
                npcText += "Function: N/A";
            }
            npcText += "\r\n";
            */

            return npcText;
        }

        /// <summary>
        /// Retrieves information for Army display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="a">Army for which information is to be displayed</param>
        public string displayArmyData(Army a)
        {
            string armyText = "";
            uint[] troopNumbers = a.troops;
            Fief armyLocation = Globals_Game.fiefMasterList[a.location];

            // check if is garrison in a siege
            string siegeID = a.checkIfSiegeDefenderGarrison();
            if (siegeID == null)
            {
                // check if is additional defender in a siege
                siegeID = a.checkIfSiegeDefenderAdditional();
            }

            // if is defender in a siege, indicate
            if (siegeID != null)
            {
                armyText += "NOTE: This army is currently UNDER SIEGE\r\n\r\n";
            }

            else
            {
                // check if is besieger in a siege
                siegeID = a.checkIfBesieger();

                // if is besieger in a siege, indicate
                if (siegeID != null)
                {
                    armyText += "NOTE: This army is currently BESIEGING THIS FIEF\r\n\r\n";
                }

                // check if is siege in fief (but army not involved)
                else
                {
                    if (armyLocation.siege != null)
                    {
                        armyText += "NOTE: This fief is currently UNDER SIEGE\r\n\r\n";
                    }
                }
            }

            // ID
            armyText += "ID: " + a.armyID + "\r\n\r\n";

            // nationality
            armyText += "Nationality: " + a.getOwner().nationality.name + "\r\n\r\n";

            // days left
            armyText += "Days left: " + a.days + "\r\n\r\n";

            // location
            armyText += "Location: " + armyLocation.name + " (Province: " + armyLocation.province.name + ".  Kingdom: " + armyLocation.province.kingdom.name + ")\r\n\r\n";

            // leader
            Character armyLeader = a.getLeader();

            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!  You should appoint one as soon as possible.";
            }
            else
            {
                armyText += armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")";
            }
            armyText += "\r\n\r\n";

            // labels for troop types
            string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Yeomen: ", " - Foot: ", " - Rabble: " };

            // display numbers for each troop type
            for (int i = 0; i < troopNumbers.Length; i++)
            {
                armyText += troopTypeLabels[i] + troopNumbers[i];
                armyText += "\r\n";
            }
            armyText += "   ==================\r\n";
            armyText += " - TOTAL: " + a.calcArmySize() + "\r\n\r\n";

            // whether is maintained (and at what cost)
            if (a.isMaintained)
            {
                uint armyCost = a.calcArmySize() * 500;

                armyText += "This army is currently being maintained (at a cost of £" + armyCost + ")\r\n\r\n";
            }
            else
            {
                armyText += "This army is NOT currently being maintained\r\n\r\n";
            }

            // aggression level
            armyText += "Aggression level: " + a.aggression + "\r\n\r\n";

            // sally value
            armyText += "Sally value: " + a.combatOdds + "\r\n\r\n";

            return armyText;
        }

        /// <summary>
        /// Retrieves information for Siege display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="s">Siege for which information is to be displayed</param>
        public string displaySiegeData(Siege s)
        {
            string siegeText = "";
            Fief siegeLocation = s.getFief();
            PlayerCharacter fiefOwner = siegeLocation.owner;
            bool isDefender = (fiefOwner == Globals_Client.myPlayerCharacter);
            Army besieger = s.getBesiegingArmy();
            PlayerCharacter besiegingPlayer = s.getBesiegingPlayer();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            Character besiegerLeader = besieger.getLeader();
            Character defGarrLeader = defenderGarrison.getLeader();
            Character defAddLeader = null;
            if (defenderAdditional != null)
            {
                defAddLeader = defenderAdditional.getLeader();
            }

            // ID
            siegeText += "ID: " + s.siegeID + "\r\n\r\n";

            // fief
            siegeText += "Fief: " + siegeLocation.name + " (Province: " + siegeLocation.province.name + ".  Kingdom: " + siegeLocation.province.kingdom.name + ")\r\n\r\n";

            // fief owner
            siegeText += "Fief owner: " + fiefOwner.firstName + " " + fiefOwner.familyName + " (ID: " + fiefOwner.charID + ")\r\n\r\n";

            // besieging player
            siegeText += "Besieging player: " + besiegingPlayer.firstName + " " + besiegingPlayer.familyName + " (ID: " + besiegingPlayer.charID + ")\r\n\r\n";

            // start date
            siegeText += "Start date: " + s.startYear + ", " + Globals_Game.clock.seasons[s.startSeason] + "\r\n\r\n";

            // duration so far
            siegeText += "Days used so far: " + s.totalDays + "\r\n\r\n";

            // days left in current season
            siegeText += "Days remaining in current season: " + s.days + "\r\n\r\n";

            // defending forces
            siegeText += "Defending forces: ";
            // only show details if player is defender
            if (isDefender)
            {
                // garrison details
                siegeText += "\r\nGarrison: " + defenderGarrison.armyID + "\r\n";
                siegeText += "- Leader: ";
                if (defGarrLeader != null)
                {
                    siegeText += defGarrLeader.firstName + " " + defGarrLeader.familyName + " (ID: " + defGarrLeader.charID + ")";
                }
                else
                {
                    siegeText += "None";
                }
                siegeText += "\r\n";
                siegeText += "- [Kn: " + defenderGarrison.troops[0] + ";  MAA: " + defenderGarrison.troops[1]
                    + ";  LCav: " + defenderGarrison.troops[2] + ";  Yeo: " + defenderGarrison.troops[3]
                    + ";  Ft: " + defenderGarrison.troops[4] + ";  Rbl: " + defenderGarrison.troops[5] + "]";

                // additional army details
                if (defenderAdditional != null)
                {
                    siegeText += "\r\n\r\nField army: " + defenderAdditional.armyID + "\r\n";
                    siegeText += "- Leader: ";
                    if (defAddLeader != null)
                    {
                        siegeText += defAddLeader.firstName + " " + defAddLeader.familyName + " (ID: " + defAddLeader.charID + ")";
                    }
                    else
                    {
                        siegeText += "None";
                    }
                    siegeText += "\r\n";
                    siegeText += "- [Kn: " + defenderAdditional.troops[0] + ";  MAA: " + defenderAdditional.troops[1]
                        + ";  LCav: " + defenderAdditional.troops[2] + ";  Yeo: " + defenderAdditional.troops[3]
                        + ";  Ft: " + defenderAdditional.troops[4] + ";  Rbl: " + defenderAdditional.troops[5] + "]";
                }

                siegeText += "\r\n\r\nTotal defender casualties so far (including attrition): " + s.totalCasualtiesDefender;
            }

            // if player not defending, hide defending forces details
            else
            {
                siegeText += "Unknown";
            }
            siegeText += "\r\n\r\n";

            // besieging forces
            siegeText += "Besieging forces: ";
            // only show details if player is besieger
            if (!isDefender)
            {
                // besieging forces details
                siegeText += "\r\nField army: " + besieger.armyID + "\r\n";
                siegeText += "- Leader: ";
                if (besiegerLeader != null)
                {
                    siegeText += besiegerLeader.firstName + " " + besiegerLeader.familyName + " (ID: " + besiegerLeader.charID + ")";
                }
                else
                {
                    siegeText += "None";
                }
                siegeText += "\r\n";
                siegeText += "- [Kn: " + besieger.troops[0] + ";  MAA: " + besieger.troops[1]
                    + ";  LCav: " + besieger.troops[2] + ";  Yeo: " + besieger.troops[3]
                    + ";  Ft: " + besieger.troops[4] + ";  Rbl: " + besieger.troops[5] + "]";

                siegeText += "\r\n\r\nTotal attacker casualties so far (including attrition): " + s.totalCasualtiesAttacker;
            }

            // if player not besieger, hide besieging forces details
            else
            {
                siegeText += "Unknown";
            }
            siegeText += "\r\n\r\n";

            // keep level
            siegeText += "Keep level:\r\n";
            // keep level at start
            siegeText += "- at start of siege: " + s.startKeepLevel + "\r\n";

            // current keep level
            siegeText += "- current: " + siegeLocation.keepLevel + "\r\n\r\n";

            if (!isDefender)
            {
                siegeText += "Chance of success in next round:\r\n";
                // chance of storm success
                /* double keepLvl = this.calcStormKeepLevel(s);
                double successChance = this.calcStormSuccess(keepLvl); */
                // get battle values for both armies
                uint[] battleValues = this.calculateBattleValue(besieger, defenderGarrison, Convert.ToInt32(siegeLocation.keepLevel));
                double successChance = this.calcVictoryChance(battleValues[0], battleValues[1]);
                siegeText += "- storm: " + successChance + "\r\n";

                // chance of negotiated success
                siegeText += "- negotiated: " + successChance / 2 + "\r\n\r\n";
            }

            return siegeText;
        }

        /// <summary>
        /// Retrieves general information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        /// <param name="isOwner">bool indicating if fief owned by player</param>
        public string displayGeneralFiefData(Fief f, bool isOwner)
        {
            string fiefText = "";

            // ID
            fiefText += "ID: " + f.id + "\r\n";

            // name (& province name)
            fiefText += "Name: " + f.name + " (Province: " + f.province.name + ".  Kingdom: " + f.province.kingdom.name + ")\r\n";

            // rank
            fiefText += "Title (rank): ";
            fiefText += f.rank.getName(f.language) + " (" + f.rank.id + ")\r\n";

            // population
            fiefText += "Population: " + f.population + "\r\n";

            // fields
            fiefText += "Fields level: " + f.fields + "\r\n";

            // industry
            fiefText += "Industry level: " + f.industry + "\r\n";

            // owner's ID
            fiefText += "Owner (ID): " + f.owner.charID + "\r\n";

            // ancestral owner's ID
            fiefText += "Ancestral owner (ID): " + f.ancestralOwner.charID + "\r\n";

            // bailiff's ID
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

            // no. of troops (only if owned)
            if (isOwner)
            {
                fiefText += "Troops: " + f.troops + "\r\n";
            }

            // fief status
            fiefText += "Status: ";
            // if under siege, replace status with siege
            if (f.siege != null)
            {
                fiefText += "UNDER SIEGE!";
            }
            else
            {
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
            }

            fiefText += "\r\n";

            // language
            fiefText += "Language: " + f.language.getName() + "\r\n";

            // terrain type
            fiefText += "Terrain: " + f.terrain.description + "\r\n";

            /*
            // characters present
            fiefText += "Characters present:";
            for (int i = 0; i < f.characters.Count; i++)
            {
                fiefText += " " + f.characters[i].firstName + " " + f.characters[i].familyName;
                if (i < (f.characters.Count - 1))
                {
                    fiefText += ",";
                }
                else
                {
                    fiefText += "\r\n";
                }
            }

            // characters barred
            fiefText += "Characters barred from keep (IDs):\r\n";
            for (int i = 0; i < f.barredCharacters.Count; i++)
            {
                fiefText += " " + f.barredCharacters[i] + "\r\n";
            }

            // if French barred
            fiefText += "The French are ";
            if (!f.frenchBarred)
            {
                fiefText += "not";
            }
            fiefText += " barred from the keep\r\n";

            // if English barred
            fiefText += "The English are ";
            if (!f.englishBarred)
            {
                fiefText += "not";
            }
            fiefText += " barred from the keep\r\n\r\n";
             * */

            return fiefText;
        }

        /// <summary>
        /// Retrieves previous season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsPrev(Fief f)
        {
            bool displayData = true;

            string fiefText = "PREVIOUS SEASON\r\n=================\r\n\r\n";

            // if under siege, check to see if display data (based on siege start date)
            if (f.siege != null)
            {
                Siege thisSiege = f.getSiege();
                displayData = this.checkToShowFinancialData(-1, thisSiege);
            }

            // if not OK to display data, show message
            if (!displayData)
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }
            // if is OK, display as normal
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.keyStatsPrevious[0] + "\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.keyStatsPrevious[1] + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.keyStatsPrevious[2] + "%\r\n\r\n";

                // officials spend
                fiefText += "Officials expenditure: " + f.keyStatsPrevious[3] + "\r\n\r\n";

                // garrison spend
                fiefText += "Garrison expenditure: " + f.keyStatsPrevious[4] + "\r\n\r\n";

                // infrastructure spend
                fiefText += "Infrastructure expenditure: " + f.keyStatsPrevious[5] + "\r\n\r\n";

                // keep spend
                fiefText += "Keep expenditure: " + f.keyStatsPrevious[6] + "\r\n";
                // keep level
                fiefText += "   (Keep level: " + f.keyStatsPrevious[7] + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.keyStatsPrevious[8] + "\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.keyStatsPrevious[9] + "\r\n\r\n";

                // total expenses
                fiefText += "Total fief expenses: " + f.keyStatsPrevious[10] + "\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.keyStatsPrevious[11] + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.keyStatsPrevious[12] + "%)\r\n\r\n";

                // surplus
                fiefText += "Bottom line: " + f.keyStatsPrevious[13];
            }

            return fiefText;
        }

        /// <summary>
        /// Retrieves current season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsCurr(Fief f)
        {
            bool displayData = true;

            string fiefText = "CURRENT SEASON\r\n=================\r\n\r\n";

            // if under siege, check to see if display data (based on siege start date)
            if (f.siege != null)
            {
                Siege thisSiege = f.getSiege();
                displayData = this.checkToShowFinancialData(0, thisSiege);
            }

            // if not OK to display data, show message
            if (!displayData)
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }
            // if is OK, display as normal
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.keyStatsCurrent[0] + "\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.keyStatsCurrent[1] + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.keyStatsCurrent[2] + "%\r\n\r\n";

                // officials spend
                fiefText += "Officials expenditure: " + f.keyStatsCurrent[3] + "\r\n\r\n";

                // garrison spend
                fiefText += "Garrison expenditure: " + f.keyStatsCurrent[4] + "\r\n\r\n";

                // infrastructure spend
                fiefText += "Infrastructure expenditure: " + f.keyStatsCurrent[5] + "\r\n\r\n";

                // keep spend
                fiefText += "Keep expenditure: " + f.keyStatsCurrent[6] + "\r\n";
                // keep level
                fiefText += "   (Keep level: " + f.keyStatsCurrent[7] + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.keyStatsCurrent[8] + "\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.keyStatsCurrent[9] + "\r\n\r\n";

                // total expenses
                fiefText += "Total fief expenses: " + f.keyStatsCurrent[10] + "\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.keyStatsCurrent[11] + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.keyStatsCurrent[12] + "%)\r\n\r\n";

                // surplus
                fiefText += "Bottom line: " + f.keyStatsCurrent[13];
            }
            
            return fiefText;
        }

        /// <summary>
        /// Retrieves next season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsNext(Fief f)
        {
            string fiefText = "NEXT SEASON (ESTIMATE)\r\n========================\r\n\r\n";

            // if under siege, don't display data
            if (f.siege != null)
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }

            // if NOT under siege
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.calcNewLoyalty() + "\r\n";
                // various loyalty modifiers
                fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod() + ")\r\n";
                fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod() + ")\r\n";
                fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted(f.bailiffDaysInFief >= 30) + ")\r\n";
                fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod(f.bailiffDaysInFief >= 30) + ")\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.calcNewGDP() + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.taxRateNext + "%\r\n\r\n";

                // officials expenditure
                fiefText += "Officials expenditure: " + f.officialsSpendNext + "\r\n\r\n";

                // Garrison expenditure
                fiefText += "Garrison expenditure: " + f.garrisonSpendNext + "\r\n\r\n";

                // Infrastructure expenditure
                fiefText += "Infrastructure expenditure: " + f.infrastructureSpendNext + "\r\n\r\n";

                // keep expenditure
                fiefText += "Keep expenditure: " + f.keepSpendNext + "\r\n";
                // keep level
                fiefText += "   (keep level: " + f.calcNewKeepLevel() + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.calcNewIncome() + "\r\n";
                // various income modifiers
                fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod(f.bailiffDaysInFief >= 30) + ")\r\n";
                fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod() + ")\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.calcFamilyExpenses() + "\r\n";
                // famExpenses modifier for player/spouse
                if ((f.owner.spouse != null) && (Globals_Game.npcMasterList[f.owner.spouse].management > f.owner.management))
                {
                    fiefText += "  (which may include a famExpense skills modifier: " + Globals_Game.npcMasterList[f.owner.spouse].calcSkillEffect("famExpense") + ")";
                }
                else
                {
                    fiefText += "  (which may include a famExpense skills modifier: " + f.owner.calcSkillEffect("famExpense") + ")";
                }
                fiefText += "\r\n\r\n";

                // total expenses (fief and family)
                fiefText += "Total fief expenses: " + (f.calcNewExpenses() + f.calcFamilyExpenses()) + "\r\n";
                // bailiff fief expenses modifier
                fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif(f.bailiffDaysInFief >= 30) + ")\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.calcNewOlordTaxes() + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.province.taxRate + "%)\r\n\r\n";

                // bottom line
                fiefText += "Bottom line: " + f.calcNewBottomLine();
            }

            return fiefText;
        }

        /// <summary>
        /// Refreshes main Character display screen
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        public void refreshCharacterContainer(Character ch = null)
        {
            // if character not specified, default to player
            if (ch == null)
            {
                ch = Globals_Client.myPlayerCharacter;
            }

            // refresh Character display TextBox
            this.characterTextBox.ReadOnly = true;
            this.characterTextBox.Text = this.displayCharacter(ch);

            // clear previous entries in Camp TextBox
            this.travelCampDaysTextBox.Text = "";

            // multimove button only enabled if is player or an employee
            if (ch != Globals_Client.myPlayerCharacter)
			{
                if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(ch))
				{
					this.travelMoveToBtn.Enabled = false;
				}
			}

            Globals_Client.containerToView = this.characterContainer;
            Globals_Client.containerToView.BringToFront();
        }

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
        /// Refreshes main Army display screen
        /// </summary>
        /// <param name="a">Army whose information is to be displayed</param>
        public void refreshArmyContainer(Army a = null)
        {
            
            // clear existing information
            this.armyTextBox.Text = "";
            this.armyRecruitTextBox.Text = "";
            this.armyTransDropWhoTextBox.Text = "";
            this.armyTransKnightTextBox.Text = "";
            this.armyTransMAAtextBox.Text = "";
            this.armyTransLCavTextBox.Text = "";
            this.armyTransYeomenTextBox.Text = "";
            this.armyTransFootTextBox.Text = "";
            this.armyTransRabbleTextBox.Text = "";
            this.armyAggroTextBox.Text = "";
            this.armyOddsTextBox.Text = "";
            this.armyCampTextBox.Text = "";

            // ensure textboxes aren't interactive
            this.armyTextBox.ReadOnly = true;

            // disable controls until army selected
            this.armyRecruitBtn.Enabled = false;
            this.armyRecruitTextBox.Enabled = false;
            this.armyMaintainBtn.Enabled = false;
            this.armyAppointLeaderBtn.Enabled = false;
            this.armyAppointSelfBtn.Enabled = false;
            this.armyTransDropBtn.Enabled = false;
            this.armyTransDropWhoTextBox.Enabled = false;
            this.armyTransKnightTextBox.Enabled = false;
            this.armyTransMAAtextBox.Enabled = false;
            this.armyTransLCavTextBox.Enabled = false;
            this.armyTransYeomenTextBox.Enabled = false;
            this.armyTransFootTextBox.Enabled = false;
            this.armyTransRabbleTextBox.Enabled = false;
            this.armyTransPickupBtn.Enabled = false;
            this.armyDisbandBtn.Enabled = false;
            this.armyAutoCombatBtn.Enabled = false;
            this.armyAggroTextBox.Enabled = false;
            this.armyOddsTextBox.Enabled = false;
            this.armyCampBtn.Enabled = false;
            this.armyCampTextBox.Enabled = false;
            this.armyExamineBtn.Enabled = false;
            this.armyPillageBtn.Enabled = false;
            this.armySiegeBtn.Enabled = false;
            this.armyQuellRebellionBtn.Enabled = false;
            
            // clear existing items in armies list
            this.armyListView.Items.Clear();

            // iterates through player's armies adding information to ListView
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myArmies.Count; i++)
            {
                ListViewItem thisArmy = null;

                // armyID
                thisArmy = new ListViewItem(Globals_Client.myPlayerCharacter.myArmies[i].armyID);

                // leader
                Character armyLeader = Globals_Client.myPlayerCharacter.myArmies[i].getLeader();
                if (armyLeader != null)
                {
                    thisArmy.SubItems.Add(armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")");
                }
                else
                {
                    thisArmy.SubItems.Add("No leader");
                }

                // location
                Fief armyLocation = Globals_Game.fiefMasterList[Globals_Client.myPlayerCharacter.myArmies[i].location];
                thisArmy.SubItems.Add(armyLocation.name + " (" + armyLocation.id + ")");

                // size
                thisArmy.SubItems.Add(Globals_Client.myPlayerCharacter.myArmies[i].calcArmySize().ToString());

                if (thisArmy != null)
                {
                    // if army passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myArmies[i] == a)
                    {
                        thisArmy.Selected = true;
                    }

                    // add item to armyListView
                    this.armyListView.Items.Add(thisArmy);
                }

            }

            if (a == null)
            {
                // if player is not leading any armies, set button text to 'recruit new' and enable
                if (Globals_Client.myPlayerCharacter.armyID == null)
                {
                    this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                    this.armyRecruitBtn.Tag = "new";
                    this.armyRecruitBtn.Enabled = true;
                    this.armyRecruitTextBox.Enabled = true;
                }
            }

            Globals_Client.containerToView = this.armyContainer;
            Globals_Client.containerToView.BringToFront();

            // check which panel to display
            string armyPanelTag = this.armyContainer.Panel1.Tag.ToString();
            if (armyPanelTag.Equals("combat"))
            {
                this.armyCombatPanel.BringToFront();
            }
            else
            {
                this.armyManagementPanel.BringToFront();
            }

            this.armyListView.Focus();
        }

        /// <summary>
        /// Refreshes main journal display screen
        /// </summary>
        /// <param name="a">JournalEntry to be displayed</param>
        public void refreshJournalContainer(int jEntryIndex = -1)
        {
            // get JournalEntry
            JournalEntry jEntryPassedIn = null;
            if ((jEntryIndex >= 0) && (!(jEntryIndex > Globals_Client.jEntryMax)))
            {
                jEntryPassedIn = Globals_Client.eventSetToView.ElementAt(jEntryIndex).Value;
            }

            // clear existing information
            this.journalTextBox.Text = "";

            // ensure textboxes aren't interactive
            this.journalTextBox.ReadOnly = true;

            // disable controls until JournalEntry selected

            // clear existing items in journal list
            this.journalListView.Items.Clear();

            // iterates through journal entries adding information to ListView
            foreach (KeyValuePair<uint, JournalEntry> thisJentry in Globals_Client.eventSetToView)
            {
                ListViewItem thisEntry = null;

                // jEntryID
                thisEntry = new ListViewItem(Convert.ToString(thisJentry.Value.jEntryID));

                // date
                string entrySeason = Globals_Game.clock.seasons[thisJentry.Value.season];
                thisEntry.SubItems.Add(entrySeason + ", " + thisJentry.Value.year);

                // type
                thisEntry.SubItems.Add(thisJentry.Value.type);

                if (thisEntry != null)
                {
                    // if journal entry passed in as parameter, show as selected
                    if (thisJentry.Value == jEntryPassedIn)
                    {
                        thisEntry.Selected = true;
                    }

                    // add item to journalListView
                    this.journalListView.Items.Add(thisEntry);
                }

            }

            // switch off 'unread entries' alert
            this.setJournalAlert(false);

            Globals_Client.containerToView = this.journalContainer;
            Globals_Client.containerToView.BringToFront();
            this.journalListView.Focus();
        }

        /// <summary>
        /// Refreshes main Siege display screen
        /// </summary>
        /// <param name="s">Siege whose information is to be displayed</param>
        public void refreshSiegeContainer(Siege s = null)
        {

            // clear existing information
            this.siegeTextBox.Text = "";

            // ensure textboxes aren't interactive
            this.siegeTextBox.ReadOnly = true;

            // disable controls until siege selected
            this.siegeNegotiateBtn.Enabled = false;
            this.siegeStormBtn.Enabled = false;
            this.siegeReduceBtn.Enabled = false;
            this.siegeEndBtn.Enabled = false;

            // clear existing items in siege list
            this.siegeListView.Items.Clear();

            // iterates through player's sieges adding information to ListView
            for (int i = 0; i < Globals_Client.myPlayerCharacter.mySieges.Count; i++)
            {
                ListViewItem thisSiegeItem = null;
                Siege thisSiege = Globals_Client.myPlayerCharacter.getSiege(Globals_Client.myPlayerCharacter.mySieges[i]);

                // armyID
                thisSiegeItem = new ListViewItem(thisSiege.siegeID);

                // fief
                Fief siegeLocation = thisSiege.getFief();
                thisSiegeItem.SubItems.Add(siegeLocation.name + " (" + siegeLocation.id + ")");

                // defender
                PlayerCharacter defendingPlayer = thisSiege.getDefendingPlayer();
                thisSiegeItem.SubItems.Add(defendingPlayer.firstName + " " + defendingPlayer.familyName + " (" + defendingPlayer.charID + ")");

                // besieger
                Army besiegingArmy = thisSiege.getBesiegingArmy();
                PlayerCharacter besieger = thisSiege.getBesiegingPlayer();
                thisSiegeItem.SubItems.Add(besieger.firstName + " " + besieger.familyName + " (" + besieger.charID + ")");

                if (thisSiegeItem != null)
                {
                    // if siege passed in as parameter, show as selected
                    if (thisSiege == s)
                    {
                        thisSiegeItem.Selected = true;
                    }

                    // add item to siegeListView
                    this.siegeListView.Items.Add(thisSiegeItem);
                }

            }

            Globals_Client.containerToView = this.siegeContainer;
            Globals_Client.containerToView.BringToFront();
            this.siegeListView.Focus();
        }

        /// <summary>
        /// Refreshes main Fief display screen
        /// </summary>
        /// <param name="f">Fief whose information is to be displayed</param>
        public void refreshFiefContainer(Fief f = null)
        {
            // if fief not specified, default to player's current location
            if (f == null)
            {
                f = Globals_Client.myPlayerCharacter.location;
            }

            Globals_Client.fiefToView = f;

            bool isOwner = Globals_Client.myPlayerCharacter.ownedFiefs.Contains(Globals_Client.fiefToView);
            bool displayWarning = false;
            string toDisplay = "";

            // set name label text
            this.fiefLabel.Text = Globals_Client.fiefToView.name + " (" + Globals_Client.fiefToView.id + ")";
            // set siege label text
            if (f.siege != null)
            {
                this.fiefSiegeLabel.Text = "Fief under siege";
            }
            else
            {
                this.fiefSiegeLabel.Text = "";
            }

            // refresh main fief TextBox with updated info
            this.fiefTextBox.Text = this.displayGeneralFiefData(Globals_Client.fiefToView, isOwner);

            // ensure textboxes aren't interactive
            this.fiefTextBox.ReadOnly = true;
            this.fiefPrevKeyStatsTextBox.ReadOnly = true;
            this.fiefCurrKeyStatsTextBox.ReadOnly = true;
            this.fiefNextKeyStatsTextBox.ReadOnly = true;
            this.fiefTransferAmountTextBox.Text = "";

            // if fief is NOT owned by player, disable fief management buttons and TextBoxes 
            if (! isOwner)
            {
                this.adjustSpendBtn.Enabled = false;
                this.taxRateLabel.Enabled = false;
                this.garrSpendLabel.Enabled = false;
                this.offSpendLabel.Enabled = false;
                this.infraSpendLabel.Enabled = false;
                this.keepSpendLabel.Enabled = false;
                this.adjGarrSpendTextBox.Enabled = false;
                this.adjInfrSpendTextBox.Enabled = false;
                this.adjOffSpendTextBox.Enabled = false;
                this.adjustKeepSpendTextBox.Enabled = false;
                this.adjustTaxTextBox.Enabled = false;
                this.fiefGarrExpMaxBtn.Enabled = false;
                this.fiefInfraExpMaxBtn.Enabled = false;
                this.fiefKeepExpMaxBtn.Enabled = false;
                this.fiefOffExpMaxBtn.Enabled = false;
                this.viewBailiffBtn.Enabled = false;
                this.lockoutBtn.Enabled = false;
                this.selfBailiffBtn.Enabled = false;
                this.setBailiffBtn.Enabled = false;
                this.removeBaliffBtn.Enabled = false;
                this.fiefTransferToFiefBtn.Enabled = false;
                this.fiefTransferToHomeBtn.Enabled = false;
                this.fiefHomeTreasTextBox.Enabled = false;
                this.fiefTransferAmountTextBox.Enabled = false;
                this.FiefTreasTextBox.Enabled = false;
                this.fiefGrantTitleBtn.Enabled = false;

                // set TextBoxes to nowt
                this.adjGarrSpendTextBox.Text = "";
                this.adjInfrSpendTextBox.Text = "";
                this.adjOffSpendTextBox.Text = "";
                this.adjustKeepSpendTextBox.Text = "";
                this.adjustTaxTextBox.Text = "";
                this.fiefHomeTreasTextBox.Text = "";
                this.FiefTreasTextBox.Text = "";
                this.fiefPrevKeyStatsTextBox.Text = "";
                this.fiefCurrKeyStatsTextBox.Text = "";
                this.fiefNextKeyStatsTextBox.Text = "";
            }

            // if fief IS owned by player, enable fief management buttons and TextBoxes 
            else
            {
                // get home fief
                Fief home = Globals_Client.myPlayerCharacter.getHomeFief();

                // get home treasury
                int homeTreasury = 0;
                if (f == home)
                {
                    homeTreasury = home.getAvailableTreasury();
                }
                else
                {
                    homeTreasury = home.getAvailableTreasury(true);
                }

                // get this fief's treasury
                int fiefTreasury = f.getAvailableTreasury(); ;

                // if fief UNDER SIEGE, leave most controls disabled
                if (f.siege != null)
                {
                    // allow view bailiff
                    this.viewBailiffBtn.Enabled = true;

                    // allow financial data TextBoxes to show appropriate data
                    this.fiefPrevKeyStatsTextBox.Text = this.displayFiefKeyStatsPrev(Globals_Client.fiefToView);
                    this.fiefCurrKeyStatsTextBox.Text = this.displayFiefKeyStatsCurr(Globals_Client.fiefToView);
                    this.fiefNextKeyStatsTextBox.Text = this.displayFiefKeyStatsNext(Globals_Client.fiefToView);
                }

                // if NOT under siege, enable usual controls
                else
                {
                    this.adjustSpendBtn.Enabled = true;
                    this.taxRateLabel.Enabled = true;
                    this.garrSpendLabel.Enabled = true;
                    this.offSpendLabel.Enabled = true;
                    this.infraSpendLabel.Enabled = true;
                    this.keepSpendLabel.Enabled = true;
                    this.adjGarrSpendTextBox.Enabled = true;
                    this.adjInfrSpendTextBox.Enabled = true;
                    this.adjOffSpendTextBox.Enabled = true;
                    this.adjustKeepSpendTextBox.Enabled = true;
                    this.adjustTaxTextBox.Enabled = true;
                    this.fiefGarrExpMaxBtn.Enabled = true;
                    this.fiefInfraExpMaxBtn.Enabled = true;
                    this.fiefKeepExpMaxBtn.Enabled = true;
                    this.fiefOffExpMaxBtn.Enabled = true;
                    this.viewBailiffBtn.Enabled = true;
                    this.lockoutBtn.Enabled = true;
                    this.setBailiffBtn.Enabled = true;
                    this.removeBaliffBtn.Enabled = true;
                    this.fiefHomeTreasTextBox.Enabled = true;
                    this.fiefHomeTreasTextBox.ReadOnly = true;
                    this.FiefTreasTextBox.ReadOnly = true;
                    this.fiefGrantTitleBtn.Enabled = true;

                    // don't enable 'appoint self' button if you're already the bailiff
                    if (f.bailiff == Globals_Client.myPlayerCharacter)
                    {
                        this.selfBailiffBtn.Enabled = false;
                    }
                    else
                    {
                        this.selfBailiffBtn.Enabled = true;
                    }

                    // don't enable treasury transfer controls if in Home Fief (can't transfer to self)
                    if (f == Globals_Client.myPlayerCharacter.getHomeFief())
                    {
                        this.fiefTransferToFiefBtn.Enabled = false;
                        this.fiefTransferToHomeBtn.Enabled = false;
                        this.fiefTransferAmountTextBox.Enabled = false;
                        this.FiefTreasTextBox.Enabled = false;
                    }
                    else
                    {
                        this.fiefTransferToFiefBtn.Enabled = true;
                        this.fiefTransferToHomeBtn.Enabled = true;
                        this.fiefTransferAmountTextBox.Enabled = true;
                        this.fiefHomeTreasTextBox.Enabled = true;
                        this.FiefTreasTextBox.Enabled = true;
                    }

                    // set TextBoxes to show appropriate data
                    this.adjGarrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.garrisonSpendNext);
                    this.adjInfrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.infrastructureSpendNext);
                    this.adjOffSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.officialsSpendNext);
                    this.adjustKeepSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.keepSpendNext);
                    this.adjustTaxTextBox.Text = Convert.ToString(Globals_Client.fiefToView.taxRateNext);
                    this.fiefPrevKeyStatsTextBox.Text = this.displayFiefKeyStatsPrev(Globals_Client.fiefToView);
                    this.fiefCurrKeyStatsTextBox.Text = this.displayFiefKeyStatsCurr(Globals_Client.fiefToView);
                    this.fiefNextKeyStatsTextBox.Text = this.displayFiefKeyStatsNext(Globals_Client.fiefToView);

                    // check if in home fief
                    if (f == home)
                    {
                        // don't show fief treasury
                        this.FiefTreasTextBox.Text = "";
                    }
                    else
                    {
                        // display fief treasury
                        this.FiefTreasTextBox.Text = fiefTreasury.ToString();
                    }

                    // display home treasury
                    this.fiefHomeTreasTextBox.Text = homeTreasury.ToString();

                    // check to see if proposed expenditure level doesn't exceed fief treasury
                    // get fief expenses (includes bailiff modifiers)
                    uint totalSpend = Convert.ToUInt32(Globals_Client.fiefToView.calcNewExpenses());

                    // make sure expenditure can be supported by the treasury
                    // if it can't, display a message and cancel the commit
                    if (!Globals_Client.fiefToView.checkExpenditureOK(totalSpend))
                    {
                        int difference = Convert.ToInt32(totalSpend - fiefTreasury);
                        toDisplay = "Your proposed expenditure exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                        toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                        toDisplay += "\r\n\r\nAny unsupportable expenditure levels will be automatically adjusted during the seasonal update.";
                        displayWarning = true;
                    }
                }

            }

            Globals_Client.containerToView = this.fiefContainer;
            Globals_Client.containerToView.BringToFront();

            if (displayWarning)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay, "WARNING: EXPENDITURE TOO HIGH");
                }
            }
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

                // get total spend
                uint totalSpend = newOff + newGarr + newInfra + newKeep;

                // factor in bailiff skills modifier for fief expenses
                double bailiffModif = 0;

                // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
                bailiffModif = Globals_Client.fiefToView.calcBailExpModif(Globals_Client.fiefToView.bailiffDaysInFief >= 30);

                if (bailiffModif != 0)
                {
                    totalSpend = totalSpend + Convert.ToUInt32(totalSpend * bailiffModif);
                }

                // check that expenditure can be supported by the treasury
                // if it can't, display a message and cancel the commit
                if (!Globals_Client.fiefToView.checkExpenditureOK(totalSpend))
                {
                    int difference = Convert.ToInt32(totalSpend - Globals_Client.fiefToView.getAvailableTreasury());
                    string toDisplay = "Your spending exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                    toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay, "TRANSACTION CANCELLED");
                    }
                }
                // if treasury funds are sufficient to cover expenditure, do the commit
                else
                {                    
                    // tax rate
                    // check if amount/rate changed
                    if (newTax != Globals_Client.fiefToView.taxRateNext)
                    {
                        // adjust tax rate
                        Globals_Client.fiefToView.adjustTaxRate(newTax);
                        spendChanged = true;
                    }

                    // officials spend
                    // check if amount/rate changed
                    if (newOff != Globals_Client.fiefToView.officialsSpendNext)
                    {
                        // adjust officials spend
                        Globals_Client.fiefToView.adjustOfficialsSpend(Convert.ToUInt32(this.adjOffSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // garrison spend
                    // check if amount/rate changed
                    if (newGarr != Globals_Client.fiefToView.garrisonSpendNext)
                    {
                        // adjust garrison spend
                        Globals_Client.fiefToView.adjustGarrisonSpend(Convert.ToUInt32(this.adjGarrSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // infrastructure spend
                    // check if amount/rate changed
                    if (newInfra != Globals_Client.fiefToView.infrastructureSpendNext)
                    {
                        // adjust infrastructure spend
                        Globals_Client.fiefToView.adjustInfraSpend(Convert.ToUInt32(this.adjInfrSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // adjust keep spend
                    // check if amount/rate changed
                    if (newKeep != Globals_Client.fiefToView.keepSpendNext)
                    {
                        // adjust keep spend
                        Globals_Client.fiefToView.adjustKeepSpend(Convert.ToUInt32(this.adjustKeepSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // display appropriate message
                    string toDisplay = "";
                    if (spendChanged)
                    {
                        toDisplay += "Expenditure adjusted";
                    }
                    else
                    {
                        toDisplay += "Expenditure unchanged";
                    }

                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
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
                if (spendChanged)
                {
                    // refresh display
                    this.refreshCurrentScreen();
                }
            }
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

        /// <summary>
        /// Gets travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="source">Source fief</param>
        /// <param name="target">Target fief</param>
        private double getTravelCost(Fief source, Fief target, string armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (source.terrain.travelCost + target.terrain.travelCost) / 2;

            // apply season modifier
            cost = cost * Globals_Game.clock.calcSeasonTravMod();

            // if necessary, apply army modifier
            if (armyID != null)
            {
                cost = cost * Globals_Game.armyMasterList[armyID].calcMovementModifier();
            }

            return cost;
        }

        /// <summary>
        /// Refreshes travel display screen
        /// </summary>
        private void refreshTravelContainer()
        {
            // get current fief
            Fief thisFief = Globals_Client.myPlayerCharacter.location;

            // string[] to hold direction text
            string[] directions = new string[] { "NE", "E", "SE", "SW", "W", "NW" };
            // Button[] to hold corresponding travel buttons
            Button[] travelBtns = new Button[] { travel_NE_btn, travel_E_btn, travel_SE_btn, travel_SW_btn, travel_W_btn, travel_NW_btn };

            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + thisFief.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")" + "\r\n" + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name;

            for (int i = 0; i < directions.Length; i++ )
            {
                // retrieve target fief for that direction
                Fief target = Globals_Game.gameMap.getFief(thisFief, directions[i]);
                // display fief details and travel cost
                if (target != null)
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\n";
                    travelBtns[i].Text += target.name + " (" + target.id + ")\r\n";
                    travelBtns[i].Text += target.province.name + ", " + target.province.kingdom.name + "\r\n\r\n";
                    travelBtns[i].Text += "Cost: " + this.getTravelCost(thisFief, target);
                }
                else
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\nNo fief present";
                }
            }

            // set text for informational labels
            this.travelLocationLabel.Text = "You are here: " + thisFief.name + " (" + thisFief.id + ")";
            this.travelDaysLabel.Text = "Your remaining days: " + Globals_Client.myPlayerCharacter.days;
            
            // set text for 'enter/exit keep' button, depending on whether player in/out of keep
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
            }

            // enable all controls
            this.travel_E_btn.Enabled = true;
            this.travel_Home_btn.Enabled = true;
            this.travel_NE_btn.Enabled = true;
            this.travel_NW_btn.Enabled = true;
            this.travel_SE_btn.Enabled = true;
            this.travel_SW_btn.Enabled = true;
            this.travel_W_btn.Enabled = true;
            this.travelCampBtn.Enabled = true;
            this.travelCampDaysTextBox.Enabled = true;
            this.travelExamineArmiesBtn.Enabled = true;
            this.travelMoveToBtn.Enabled = true;
            this.travelMoveToTextBox.Enabled = true;
            this.travelRouteBtn.Enabled = true;
            this.travelRouteTextBox.Enabled = true;
            this.enterKeepBtn.Enabled = true;
            this.listOutsideKeepBtn.Enabled = true;
            this.visitCourtBtn1.Enabled = true;
            this.visitTavernBtn.Enabled = true;

            // clear existing data in TextBoxes
            this.travelMoveToTextBox.Text = "";
            this.travelCampDaysTextBox.Text = "";
            this.travelRouteTextBox.Text = "";

            // check to see if fief is besieged and, if so, disable various controls
            if (thisFief.siege != null)
            {
                // check to see if are inside/outside keep
                if (Globals_Client.myPlayerCharacter.inKeep)
                {
                    // if inside keep, disable all controls except tavern and court
                    this.travel_E_btn.Enabled = false;
                    this.travel_Home_btn.Enabled = false;
                    this.travel_NE_btn.Enabled = false;
                    this.travel_NW_btn.Enabled = false;
                    this.travel_SE_btn.Enabled = false;
                    this.travel_SW_btn.Enabled = false;
                    this.travel_W_btn.Enabled = false;
                    this.travelCampBtn.Enabled = false;
                    this.travelCampDaysTextBox.Enabled = false;
                    this.travelExamineArmiesBtn.Enabled = false;
                    this.travelMoveToBtn.Enabled = false;
                    this.travelMoveToTextBox.Enabled = false;
                    this.travelRouteBtn.Enabled = false;
                    this.travelRouteTextBox.Enabled = false;
                    this.enterKeepBtn.Enabled = false;
                    this.listOutsideKeepBtn.Enabled = false;
                }

                else
                {
                    // if outside keep, disable tavern, court and 'enter keep' but leave all others enabled
                    this.enterKeepBtn.Enabled = false;
                    this.visitCourtBtn1.Enabled = false;
                    this.visitTavernBtn.Enabled = false;
                }
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
            Fief fiefToDisplay = null;

            // loop through player's owned fiefs until correct fief is found
            for (int i = 0; i < Globals_Client.myPlayerCharacter.ownedFiefs.Count; i++)
            {
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i].id.Equals(this.fiefsListView.SelectedItems[0].SubItems[1].Text))
                {
                    fiefToDisplay = Globals_Client.myPlayerCharacter.ownedFiefs[i];
                }

            }

            // display fief details
            if (fiefToDisplay != null)
            {
                this.refreshFiefContainer(fiefToDisplay);
                //this.fiefTextBox.Text = this.displayFief(fiefToView);
                Globals_Client.containerToView = this.fiefContainer;
                Globals_Client.containerToView.BringToFront();
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
            // if player in keep
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                // exit keep
                Globals_Client.myPlayerCharacter.exitKeep();
                // change button text
                this.enterKeepBtn.Text = "Enter Keep";
                // refresh display
                this.refreshTravelContainer();
            }
            // if player not in keep
            else
            {
                // attempt to enter keep
                bool entered = Globals_Client.myPlayerCharacter.enterKeep();
                // if successful
                if (entered)
                {
                    // change button text
                    this.enterKeepBtn.Text = "Exit Keep";
                    // refresh display
                    this.refreshTravelContainer();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the visitCourtBtn1 button
        /// which causes the player (and entourage) to enter the keep and
        /// refreshes and displays the court screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void visitCourtBtn1_Click(object sender, EventArgs e)
        {
            string place = "court";

            bool enteredKeep = false;

            // if player not in keep
            if (!Globals_Client.myPlayerCharacter.inKeep)
            {
                // attempt to enter keep
                enteredKeep = Globals_Client.myPlayerCharacter.enterKeep();
            }
            else
            {
                enteredKeep = true;
            }

            // if in keep
            if (enteredKeep)
            {
                // set button tags to reflect which meeting place
                this.hireNPC_Btn.Tag = place;
                this.meetingPlaceMoveToBtn.Tag = place;
                this.meetingPlaceRouteBtn.Tag = place;
                this.meetingPlaceEntourageBtn.Tag = place;
                // refresh court screen 
                this.refreshMeetingPlaceDisplay(place);
                // display court screen
                Globals_Client.containerToView = this.meetingPlaceContainer;
                Globals_Client.containerToView.BringToFront();
            }

        }

        /// <summary>
        /// Responds to the click event of the visitTavernBtn button
        /// which causes the player (and entourage) to exit the keep (if necessary)
        /// and refreshes and displays the tavern screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void visitTavernBtn_Click_1(object sender, EventArgs e)
        {
            string place = "tavern";

            // exit keep if required
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                Globals_Client.myPlayerCharacter.exitKeep();
            }
            // set button tags to reflect which meeting place
            this.hireNPC_Btn.Tag = place;
            this.meetingPlaceMoveToBtn.Tag = place;
            this.meetingPlaceRouteBtn.Tag = place;
            this.meetingPlaceEntourageBtn.Tag = place;
            // refresh tavern screen 
            this.refreshMeetingPlaceDisplay(place);
            // display tavern screen
            Globals_Client.containerToView = this.meetingPlaceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the meetingPlaceCharsListView object,
        /// invoking the displayCharacter method, passing a Character to display
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

                        // check whether is employee or family
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
                            if (((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).myBoss != null)
                                && ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).myBoss.Equals(Globals_Client.myPlayerCharacter.charID)))
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

                            // can only employ men
                            if (charToDisplay.isMale)
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
                            if (((charToDisplay.spouse != null) || (charToDisplay.isMale)) || (charToDisplay.fiancee != null))
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
        /// invoking either processEmployOffer or fireNPC
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
                    this.refreshMeetingPlaceDisplay(place);
                }
            }
            // if hiring an NPC
            else
            {
                // if in the tavern and NPC is hired, refresh whole screen (NPC removed from list)
                if (isHired)
                {
                    this.refreshMeetingPlaceDisplay(place);
                }
                else
                {
                    this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(Globals_Client.charToView);
                }
            }

        }

        /// <summary>
        /// Moves character to the target fief, through intervening fiefs (stored in goTo queue)
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to be moved</param>
        private bool characterMultiMove(Character ch)
        {
            bool success = false;
            double travelCost = 0;
            int steps = ch.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                // get travel cost
                travelCost = this.getTravelCost(ch.location, ch.goTo.Peek(), ch.armyID);
                // attempt to move character
                success = this.moveCharacter(ch, ch.goTo.Peek(), travelCost);
                // if move successfull, remove fief from goTo queue
                if (success)
                {
                    ch.goTo.Dequeue();
                }
                // if not successfull, exit loop
                else
                {
                    break;
                }
            }

            if (ch == Globals_Client.myPlayerCharacter)
            {
                // if player has moved, indicate success
                if (ch.goTo.Count < steps)
                {
                    success = true;
                }
            }

            return success;

        }

        /// <summary>
        /// Moves a character to a specified fief using the shortest path
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void moveTo(string whichScreen)
        {
            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }                
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelMoveToTextBox;
            }

            // check for existence of fief
            if (Globals_Game.fiefMasterList.ContainsKey(myTextBox.Text.ToUpper()))
            {
                // retrieves target fief
                Fief target = Globals_Game.fiefMasterList[myTextBox.Text.ToUpper()];

                // obtains goTo queue for shortest path to target
                Globals_Client.charToView.goTo = Globals_Game.gameMap.getShortestPath(Globals_Client.charToView.location, target);

                // if retrieve valid path
                if (Globals_Client.charToView.goTo.Count > 0)
                {
                    // if character is NPC, check entourage and remove if necessary
                    if (!whichScreen.Equals("travel"))
                    {
                        if (Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.charToView))
                        {
                            (Globals_Client.charToView as NonPlayerCharacter).inEntourage = false;
                        }
                    }

                    // perform move
                    this.characterMultiMove(Globals_Client.charToView);
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
                else if (whichScreen.Equals("travel"))
                {
                    this.refreshTravelContainer();
                }

            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Target fief ID not found.  Please ensure fiefID is valid.");
                }
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

        // temporary method to write object data to database
        public string[][] ArrayFromCSV(string csvFilename, bool writeToDB, string bucket = "", string key = "")
        {
            var linesIn = new List<string[]>();

            StreamReader sr = new StreamReader(csvFilename);

            int Row = 0;
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(',');
                if (line.Length != 67)
                {
                    System.Windows.Forms.MessageBox.Show("row " + Row + " = " + line.Length);
                }
                /* string toDisplay = "";
                for (int i = 0; i < line.Length; i++)
                {
                    toDisplay += line [i] + " ";
                }
                toDisplay += line.Length;
                System.Windows.Forms.MessageBox.Show(toDisplay); */
                linesIn.Add(line);
                Row++;
            }

            var outArray = linesIn.ToArray();

            if (writeToDB)
            {
                var arrayToDB = new RiakObject(bucket, key, outArray);
                var putArrayResult = rClient.Put(arrayToDB);

                if (!putArrayResult.IsSuccess)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: " + arrayToDB.Key + " to bucket " + arrayToDB.Bucket);
                }
            }

            /*
            // test read from Riak
            var fiefArrayResult = client.Get(bucket, key);

            if (fiefArrayResult.IsSuccess)
            {
                string toDisplay = "";
                var fiefArrayRiak = fiefArrayResult.Value.GetObject<string[,]>();
                System.Windows.Forms.MessageBox.Show(fiefArrayRiak.GetLength(0) + " ; " + fiefArrayRiak.GetLength(1) + "   .");
                for (int i = 0; i < fiefArrayRiak.GetLength(0); i++)
                {
                    for (int ii = 0; ii < fiefArrayRiak.GetLength(1); ii++)
                    {
                        toDisplay += fiefArrayRiak [i,ii] + " ";
                    }
                    System.Windows.Forms.MessageBox.Show(toDisplay);
                    toDisplay = "";
                }
            }
            else
            {
                // System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve PlayerCharacter " + pcID);
            } */

            return outArray;
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
        /// Responds to the click event of the selfBailiffBtn button,
        /// appointing the player as bailiff of the displayed fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void selfBailiffBtn_Click(object sender, EventArgs e)
        {
            // give player fair warning of bailiff commitments
            DialogResult dialogResult = MessageBox.Show("Being a bailiff will restrict your movement.  Click 'OK' to proceed.", "Proceed with appointment?", MessageBoxButtons.OKCancel);

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
        /// Responds to the CheckedChanged event of the characterTitlesCheckBox,
        /// displaying the player's titles/ranks
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void characterTitlesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.refreshCharacterContainer(Globals_Client.charToView);
        }

        /// <summary>
        /// Responds to the click event of the listOutsideKeepBtn button
        /// which causes the player (and entourage) to exit the keep (if necessary)
        /// and refreshes and displays the 'outside keep' screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void listOutsideKeepBtn_Click(object sender, EventArgs e)
        {
            string place = "outsideKeep";

            // exit keep if required
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                Globals_Client.myPlayerCharacter.exitKeep();
            }
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

                // FAMILY MATTERS CONTROLS
                // if family selected, enable 'choose heir' button, disbale 'fire' button
                if ((Globals_Client.charToView.familyID != null) && (Globals_Client.charToView.familyID.Equals(Globals_Client.myPlayerCharacter.charID)))
                {
                    this.houseHeirBtn.Enabled = true;
                    this.houseFireBtn.Enabled = false;

                    // if is male and married, enable NPC 'get wife with child' control
                    if ((Globals_Client.charToView.isMale) && (Globals_Client.charToView.spouse != null))
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
                if ((Globals_Client.charToView.inKeep) && (Globals_Client.charToView.location.siege != null))
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
        /// Allows the character to remain in their current location for the specified
        /// number of days, incrementing bailiffDaysInFief if appropriate
        /// </summary>
        /// <param name="ch">The Character who wishes to camp</param>
        /// <param name="campDays">Number of days to camp</param>
        public void campWaitHere(Character ch, byte campDays)
        {
            bool proceed = true;

            // check has enough days available
            if (ch.days < (Double)campDays)
            {
                campDays = Convert.ToByte(Math.Truncate(ch.days));
                DialogResult dialogResult = MessageBox.Show("You only have " + campDays + " available.  Click 'OK' to proceed.", "Proceed with camp?", MessageBoxButtons.OKCancel);

                // if choose to cancel
                if (dialogResult == DialogResult.Cancel)
                {
                    proceed = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You decide not to camp after all.");
                    }
                }
            }

            if (proceed)
            {
                // check if player's entourage needs to camp
                bool entourageCamp = false;

                // if character is player, camp entourage
                if (ch == Globals_Client.myPlayerCharacter)
                {
                    entourageCamp = true;
                }

                // if character NOT player
                else
                {
                    // if is in entourage, give player chance to remove prior to camping
                    if ((ch as NonPlayerCharacter).inEntourage)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " has been removed from your entourage.");
                        }
                        Globals_Client.myPlayerCharacter.removeFromEntourage((ch as NonPlayerCharacter));
                    }
                }

                // adjust character's days
                if (ch is PlayerCharacter)
                {
                    (ch as PlayerCharacter).adjustDays(campDays);
                }
                else
                {
                    ch.adjustDays(campDays);
                }

                // inform player
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " remains in " + ch.location.name + " for " + campDays + " days.");
                }

                // check if character is army leader, if so check for army attrition
                if (ch.armyID != null)
                {
                    // get army
                    Army thisArmy = ch.getArmy();

                    // number of attrition checks
                    byte attritionChecks = 0;
                    attritionChecks = Convert.ToByte(campDays / 7);
                    // total attrition
                    uint totalAttrition = 0;

                    for (int i = 0; i < attritionChecks; i++)
                    {
                        // calculate attrition
                        double attritionModifer = thisArmy.calcAttrition();
                        // apply attrition
                        totalAttrition += thisArmy.applyTroopLosses(attritionModifer);
                    }

                    // inform player
                    if (totalAttrition > 0)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Army (" + thisArmy.armyID + ") lost " + totalAttrition + " troops due to attrition.");
                        }
                    }
                }

                // keep track of bailiffDaysInFief before any possible increment
                Double bailiffDaysBefore = ch.location.bailiffDaysInFief;

                // keep track of identity of bailiff
                Character myBailiff = null;

                // check if character is bailiff of this fief
                if (ch.location.bailiff == ch)
                {
                    myBailiff = ch;
                }

                // if character not bailiff, if appropriate, check to see if anyone in entourage is
                else if (entourageCamp)
                {
                    // if player is bailiff
                    if (Globals_Client.myPlayerCharacter == ch.location.bailiff)
                    {
                        myBailiff = Globals_Client.myPlayerCharacter;
                    }
                    // if not, check for bailiff in entourage
                    else
                    {
                        for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                        {
                            if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                            {
                                if (Globals_Client.myPlayerCharacter.myNPCs[i] != ch)
                                {
                                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == ch.location.bailiff)
                                    {
                                        myBailiff = Globals_Client.myPlayerCharacter.myNPCs[i];
                                    }
                                }
                            }
                        }
                    }

                }

                // if bailiff identified as someone who camped
                if (myBailiff != null)
                {
                    // increment bailiffDaysInFief
                    ch.location.bailiffDaysInFief += campDays;
                    // if necessary, display message to player
                    if (ch.location.bailiffDaysInFief >= 30)
                    {
                        // don't display this message if min bailiffDaysInFief was already achieved
                        if (!(bailiffDaysBefore >= 30))
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show(myBailiff.firstName + " " + myBailiff.familyName + " has fulfilled his bailiff duties in " + ch.location.name + ".");
                            }
                        }
                    }
                }
            }

            // refresh display
            if (proceed)
            {
                if (ch == Globals_Client.myPlayerCharacter)
                {
                    this.refreshTravelContainer();
                }
                else
                {
                    this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                }
            }

        }

        /// <summary>
        /// Allows a character to be moved along a specific route by using direction codes
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void takeThisRoute(string whichScreen)
        {
            bool proceed;
            Fief source = null;
            Fief target = null;
            Queue<Fief> route = new Queue<Fief>();

            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceRouteTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseRouteTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelRouteTextBox;
            }

            // get list of directions
            string[] directions = myTextBox.Text.Split(',').ToArray<string>();

            // convert to Queue of fiefs
            for (int i = 0; i < directions.Length; i++)
            {
                // source for first move is character's current location
                if (i == 0)
                {
                    source = Globals_Client.charToView.location;
                }
                // source for all other moves is the previous target fief
                else
                {
                    source = target;
                }

                // get the target fief
                target = Globals_Game.gameMap.getFief(source, directions[i].ToUpper());

                // if target successfully acquired, add to queue
                if (target != null)
                {
                    route.Enqueue(target);
                }
                // if no target acquired, display message and break
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Invalid direction code encountered.  Route halted at " + source.name + " (" + source.id + ")");
                    }
                    break;
                }

                // if there are any fiefs in the queue, overwrite the character's goTo queue
                // then process by calling characterMultiMove
                if (route.Count > 0)
                {
                    Globals_Client.charToView.goTo = route;
                    proceed = this.characterMultiMove(Globals_Client.charToView);
                    if (!proceed)
                    {
                        break;
                    }
                }
            }

            // refresh appropriate screen
            this.refreshCurrentScreen();
            /*if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                this.refreshMeetingPlaceDisplay(whichScreen); ;
            }
            else if (whichScreen.Equals("house"))
            {
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
            else if (whichScreen.Equals("travel"))
            {
                this.refreshTravelContainer();
            }*/

        }

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

        /*
        /// <summary>
        /// Responds to the click event of the fiefTransferToFiefBtn button
        /// allowing players to transfer funds from the fief treasury to the home treasury
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefTransferToFiefBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Fief fiefFrom = Globals_Client.myChar.getHomeFief();
                Fief fiefTo = Globals_Client.fiefToView;
                int amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);

                // make sure are enough funds to cover transfer
                if (amount > fiefFrom.getAvailableTreasury(true))
                {
                    // if not, inform player and adjust amount downwards
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Too few funds available in Home Treasury; amount adjusted.");
                    }
                    amount = fiefFrom.getAvailableTreasury(true);
                }

                // make the transfer
                this.treasuryTransfer(fiefFrom, fiefTo, amount);
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
        /// Responds to the click event of the fiefTransferToHomeBtn button
        /// allowing players to transfer funds from the home treasury to the fief treasury
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefTransferToHomeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Fief fiefFrom = Globals_Client.fiefToView;
                Fief fiefTo = Globals_Game.fiefMasterList[Globals_Client.myChar.homeFief];
                int amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);

                // make sure are enough funds to cover transfer
                if (amount > fiefFrom.getAvailableTreasury())
                {
                    // if not, inform player and adjust amount downwards
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Too few funds available in " + fiefFrom.name + " Treasury; amount adjusted.");
                    }
                    amount = fiefFrom.getAvailableTreasury();
                }

                // make the transfer
                this.treasuryTransfer(fiefFrom, fiefTo, amount);
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
        } */

        /// <summary>
        /// Transfers funds between the home treasury and the fief treasury
        /// </summary>
        /// <param name="from">The Fief from which funds are to be transferred</param>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        public void treasuryTransfer(Fief from, Fief to, int amount)
        {
            // subtract from source treasury
            from.treasury = from.treasury - amount;
            // add to target treasury
            to.treasury = to.treasury + amount;
            // refresh fief display
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Responds to the click event of the familyGetSpousePregBt button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyGetSpousePregBtn_Click(object sender, EventArgs e)
        {
            // get spouse
            Character mySpouse = Globals_Client.myPlayerCharacter.getSpouse();

            // perform standard checks
            if (this.checkBeforePregnancyAttempt(Globals_Client.myPlayerCharacter))
            {
                // ensure are both in/out of keep
                mySpouse.inKeep = Globals_Client.myPlayerCharacter.inKeep;

                // attempt pregnancy
                bool pregnant = Globals_Client.myPlayerCharacter.getSpousePregnant(mySpouse);
            }

            // refresh screen
            this.refreshCurrentScreen();

            /*
            // test event scheduled in clock
            List<JournalEntry> myEvents = new List<JournalEntry>();
            myEvents = Globals_Client.clock.scheduledEvents.getEventsOnDate();
            if (myEvents.Count > 0)
            {
                foreach (JournalEntry jEvent in myEvents)
                {
                    System.Windows.Forms.MessageBox.Show("Year: " + jEvent.year + " | Season: " + jEvent.season + " | Who: " + jEvent.personae + " | What: " + jEvent.type);
                }
            } */
        }

        /// <summary>
        /// Generates a new NPC based on parents' statistics
        /// </summary>
        /// <returns>NonPlayerCharacter or null</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public NonPlayerCharacter generateNewNPC(NonPlayerCharacter mummy, Character daddy)
        {
            NonPlayerCharacter newNPC = new NonPlayerCharacter();

            // charID
            newNPC.charID = Globals_Game.getNextCharID();
            // first name
            newNPC.firstName = "Baby";
            // family name
            newNPC.familyName = daddy.familyName;
            // date of birth
            newNPC.birthDate = new Tuple<uint, byte>(Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
            // sex
            newNPC.isMale = this.generateSex();
            // nationality
            newNPC.nationality = daddy.nationality;
            // whether is alive
            newNPC.isAlive = true;
            // maxHealth
            newNPC.maxHealth = this.generateKeyCharacteristics(mummy.maxHealth, daddy.maxHealth);
            // virility
            newNPC.virility = this.generateKeyCharacteristics(mummy.virility, daddy.virility);
            // goTo queue
            newNPC.goTo = new Queue<Fief>();
            // language
            newNPC.language = daddy.language;
            // days left
            newNPC.days = 90;
            // stature modifier
            newNPC.statureModifier = 0;
            // management
            newNPC.management = this.generateKeyCharacteristics(mummy.management, daddy.management);
            // combat
            newNPC.combat = this.generateKeyCharacteristics(mummy.combat, daddy.combat);
            // skills
            newNPC.skills = this.generateSkillSetFromParents(mummy.skills, daddy.skills, newNPC.isMale);
            // if in keep
            newNPC.inKeep = mummy.inKeep;
            // if pregnant
            newNPC.isPregnant = false;
            // familyID
            newNPC.familyID = daddy.familyID;
            // spouse
            newNPC.spouse = null;
            // father
            newNPC.father = daddy.charID;
            // mother
            newNPC.mother = mummy.charID;
            // fiancee
            newNPC.fiancee = null;
            // location
            newNPC.location = null;
            // titles
            newNPC.myTitles = new List<string>();
            // armyID
            newNPC.armyID = null;
            // ailments
            newNPC.ailments = new Dictionary<string, Ailment>();
            // employer (myBoss)
            newNPC.myBoss = null;
            // salary/allowance
            newNPC.wage = 0;
            // lastOffer (will remain empty for family members)
            newNPC.lastOffer = new Dictionary<string, uint>();
            // inEntourage
            newNPC.inEntourage = false;
            // isHeir
            newNPC.isHeir = false;

            return newNPC;
        }

        /// <summary>
        /// Generates a random sex for a Character
        /// </summary>
        /// <returns>bool indicating whether is male</returns>
        public bool generateSex()
        {
            bool isMale = false;

            // generate random (0-1) to see if male or female
            if (Globals_Game.myRand.Next(0, 2) == 0)
            {
                isMale = true;
            }

            return isMale;
        }

        /// <summary>
        /// Generates a characteristic stat for a Character, based on parent stats
        /// </summary>
        /// <returns>Double containing characteristic stat</returns>
        /// <param name="mummyStat">The mother's characteristic stat</param>
        /// <param name="daddyStat">The father's characteristic stat</param>
        public Double generateKeyCharacteristics(Double mummyStat, Double daddyStat)
        {
            Double newStat = 0;

            // get average of parents' stats
            Double parentalAverage = (mummyStat + daddyStat) / 2;

            // generate random (0 - 100) to determine relationship of new stat to parentalAverage
            double randPercentage = Globals_Game.GetRandomDouble(100);

            // calculate new stat
            if (randPercentage <= 35)
            {
                newStat = parentalAverage;
            }
            else if (randPercentage <= 52.5)
            {
                newStat = parentalAverage - 1;
            }
            else if (randPercentage <= 70)
            {
                newStat = parentalAverage + 1;
            }
            else if (randPercentage <= 80)
            {
                newStat = parentalAverage - 2;
            }
            else if (randPercentage <= 90)
            {
                newStat = parentalAverage + 2;
            }
            else if (randPercentage <= 95)
            {
                newStat = parentalAverage - 3;
            }
            else
            {
                newStat = parentalAverage + 3;
            }

            // make sure new stat falls within acceptable range
            if (newStat < 1)
            {
                newStat = 1;
            }
            else if (newStat > 9)
            {
                newStat = 9;
            }

            return newStat;
        }

        /// <summary>
        /// Generates a skill set for a Character, based on parent skills
        /// </summary>
        /// <returns>Array containing skill set</returns>
        /// <param name="mummySkills">The mother's skills</param>
        /// <param name="daddySkills">The father's skills</param>
        /// <param name="isMale">Whether character is a male</param>
        public Tuple<Skill, int>[] generateSkillSetFromParents(Tuple<Skill, int>[] mummySkills, Tuple<Skill, int>[] daddySkills, bool isMale)
        {
            // create a List to temporarily hold skills
            // will convert to array at end of method
            List<Tuple<Skill, int>> newSkillsList = new List<Tuple<Skill, int>>();

            // number of skills to return
            int numSkills = 0;

            // need to compare parent's skills to see how many match (could effect no. of child skills)
            int matchingSkills = 0;
            int totalSkillsAvail = 0;

            // iterate through parents' skills identifying matches
            for (int i = 0; i < mummySkills.Length; i++ )
            {
                for (int ii = 0; ii < daddySkills.Length; ii++ )
                {
                    if (mummySkills[i].Item1.skillID.Equals(daddySkills[ii].Item1.skillID))
                    {
                        matchingSkills++;
                    }
                }
            }

            // get total skill pool available from both parents
            totalSkillsAvail = (mummySkills.Length + daddySkills.Length) - matchingSkills;

            // if are only 2 skills in total, can only be 2 child skills
            if (totalSkillsAvail == 2)
            {
                numSkills = 2;
            }
            else
            {
                // generate random (2-3) to see how many skills child will have
                numSkills = Globals_Game.myRand.Next(2, 4);
            }

            // if are only 2 skills in parents' skill pool (i.e. both parents have same skills)
            // then use highest level skills (enhanced)
            if (totalSkillsAvail == 2)
            {
                for (int i = 0; i < mummySkills.Length; i++)
                {
                    for (int j = 0; j < daddySkills.Length; j++ )
                    {
                        if (mummySkills[i].Item1.skillID.Equals(daddySkills[j].Item1.skillID))
                        {
                            // get highest of duplicate skills' level
                            int maxLevel = Math.Max(mummySkills[i].Item2, daddySkills[j].Item2);

                            // adjust the skill level upwards
                            int newSkillLevel = 0;
                            if (maxLevel > 6)
                            {
                                newSkillLevel = 9;
                            }
                            else
                            {
                                newSkillLevel = maxLevel + 2;
                            }

                            // creat new skill item
                            Tuple<Skill, int> mySkill = new Tuple<Skill, int>(mummySkills[i].Item1, newSkillLevel);
                            
                            // add to temporary list
                            newSkillsList.Add(mySkill);

                            break;
                        }
                    }
                }
            }

            // if are more than 2 skills in parents' skill pool
            else
            {
                Tuple<Skill, int> mySkill;

                // decide which parent to use first (in case have to choose 2 skills from one parent)
                Tuple<Skill, int>[] firstSkillSet = null;
                Tuple<Skill, int>[] lastSkillSet = null;

                // use same sex parent first
                if (isMale)
                {
                    firstSkillSet = daddySkills;
                    lastSkillSet = mummySkills;
                }
                else
                {
                    firstSkillSet = mummySkills;
                    lastSkillSet = daddySkills;
                }

                // to hold chosen skill
                int chosenSkill = 0;
                // to hold previous chosen skill, to allow comparison
                int PrevChosenSkill = 0;

                // get a skill from the first parent
                chosenSkill = Globals_Game.myRand.Next(0, firstSkillSet.Length);

                // creat new skill item
                mySkill = new Tuple<Skill, int>(firstSkillSet[chosenSkill].Item1, firstSkillSet[chosenSkill].Item2);
                // add to temporary list
                newSkillsList.Add(mySkill);
                // record which skill was chosen in case comparison needed
                PrevChosenSkill = chosenSkill;

                // if child is to have 3 skills
                if (numSkills == 3)
                {
                    do {
                        // get another skill from the first parent
                        chosenSkill = Globals_Game.myRand.Next(0, firstSkillSet.Length);

                        // creat new skill item
                        mySkill = new Tuple<Skill, int>(firstSkillSet[chosenSkill].Item1, firstSkillSet[chosenSkill].Item2);
                        // add to temporary list
                        newSkillsList.Add(mySkill);

                    // do chosen skill doesn't match the first
                    } while (chosenSkill == PrevChosenSkill);

                }

                // get a skill from the other parent
                chosenSkill = Globals_Game.myRand.Next(0, lastSkillSet.Length);

                // check to see if already have skill in newSkillsList
                bool duplicate = false;
                // to hold any duplicate skill items
                Tuple<Skill, int> duplicateItem = null;

                // iterate through existing skills list checking for duplicates
                foreach (Tuple<Skill, int> element in newSkillsList)
                {
                    if (lastSkillSet[chosenSkill].Item1.skillID.Equals(element.Item1.skillID))
                    {
                        duplicate = true;
                        // record duplicate skill item
                        duplicateItem = element;
                    }
                }

                // if the last chosen skill was a duplicate
                if (duplicate)
                {
                    // get highest of duplicate skills' level
                    int maxLevel = Math.Max(duplicateItem.Item2, lastSkillSet[chosenSkill].Item2);

                    // adjust the skill level upwards
                    int newSkillLevel = 0;
                    if (maxLevel > 6)
                    {
                        newSkillLevel = 9;
                    }
                    else
                    {
                        newSkillLevel = maxLevel + 2;
                    }

                    // remove the duplicate item from the list
                    newSkillsList.Remove(duplicateItem);

                    // create a new skill item with enhanced skill level
                    mySkill = new Tuple<Skill, int>(duplicateItem.Item1, newSkillLevel);
                }

                // if the last chosen skill was not a duplicate
                else
                {
                    // copy chosen skill into new skill item
                    mySkill = new Tuple<Skill, int>(lastSkillSet[chosenSkill].Item1, lastSkillSet[chosenSkill].Item2);
                }

                // add to temporary list
                newSkillsList.Add(mySkill);
            }

            // create new skills array from temporary list
            Tuple<Skill, int>[] newSkills = newSkillsList.ToArray();

            return newSkills;
        }

        /// <summary>
        /// Performs childbirth procedure
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public void giveBirth(NonPlayerCharacter mummy, Character daddy)
        {
            string description = "";

            // get head of family
            PlayerCharacter thisHeadOfFamily = daddy.getHeadOfFamily();

            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = this.generateNewNPC(mummy, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.checkDeath(true, false, false);

            if (!isStillborn)
            {
                // add baby to npcMasterList
                Globals_Game.npcMasterList.Add(weeBairn.charID, weeBairn);

                // set baby's location
                weeBairn.location = mummy.location;
                weeBairn.location.charactersInFief.Add(weeBairn);

                // add baby to family
                Globals_Client.myPlayerCharacter.myNPCs.Add(weeBairn);
            }
            else
            {
                weeBairn.isAlive = false;
            }

            // check for mother dying during childbirth
            bool mummyDied = mummy.checkDeath(true, true, isStillborn);

            // construct and send JOURNAL ENTRY

            // personae
            string[] childbirthPersonae = new string[] { thisHeadOfFamily.charID + "|headOfFamily", mummy.charID + "|mother", daddy.charID + "|father", weeBairn.charID + "|child" };

            // description
            description += "On this day of Our Lord " + mummy.firstName + " " + mummy.familyName;
            description += ", wife of " + daddy.firstName + " " + daddy.familyName + ", went into labour.";

            // mother and baby alive
            if ((!isStillborn) && (!mummyDied))
            {
                description += " Both the mother and her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " are doing well and " + thisHeadOfFamily.firstName + " " + thisHeadOfFamily.familyName;
                description += " is delighted to welcome a new member into his family.";
            }

            // baby OK, mother dead
            if ((!isStillborn) && (mummyDied))
            {
                description += " The baby ";
                if (weeBairn.isMale)
                {
                    description += "boy";
                }
                else
                {
                    description += "girl";
                }
                description += " is doing well but sadly the mother died during childbirth. ";
                description += thisHeadOfFamily.firstName + " " + thisHeadOfFamily.familyName;
                description += " welcomes the new member into his family.";
            }

            // mother OK, baby dead
            if ((isStillborn) && (!mummyDied))
            {
                description += " The mother is doing well but sadly her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " died during childbirth.";
            }

            // both mother and baby died
            if ((isStillborn) && (mummyDied))
            {
                description += " Tragically, both the mother and her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " died of complications during the childbirth.";
            }

            // put together new journal entry
            JournalEntry childbirth = new JournalEntry(Globals_Game.getNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, childbirthPersonae, "birth", descr: description);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(childbirth);

            // if appropriate, process mother's death
            if (mummyDied)
            {
                mummy.processDeath("childbirth");
            }

            
            // display message
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(description);
            }

            this.refreshHouseholdDisplay();
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
        /// Responds to the click event of the familyNameChildButton
        /// allowing the player to name the selected child
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNameChildButton_Click(object sender, EventArgs e)
        {
            NonPlayerCharacter child = null;

            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get NPC to name
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        child = Globals_Client.myPlayerCharacter.myNPCs[i];
                        break;
                    }
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
					this.writeToDB ("testGame");
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
            // necessary in order to be able to access button tag
            ToolStripItem menuItem = sender as ToolStripItem;

            // get type of update from button tag
            string updateType = menuItem.Tag.ToString();

            this.seasonUpdate(updateType);
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

            if ((playerID.Trim() == "") || (playerID == null))
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
                Globals_Client.myPlayerCharacter = Globals_Game.pcMasterList[playerID];
                Globals_Client.charToView = Globals_Client.myPlayerCharacter;
                this.refreshCharacterContainer(Globals_Client.charToView);
            }
        }

        private void houseHeirBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];

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
        /// Performs functions associated with creating a new army
        /// </summary>
        /// <param name="a">The army to be added to the game</param>
        public void addArmy(Army a)
        {
            // get leader
            Character armyLeader = a.getLeader();

            // get owner
            PlayerCharacter armyOwner = a.getOwner();

            // get location
            Fief armyLocation = Globals_Game.fiefMasterList[a.location];

            // add to armyMasterList
            Globals_Game.armyMasterList.Add(a.armyID, a);

            // add to owner's myArmies
            armyOwner.myArmies.Add(a);

            // add to leader
            if (armyLeader != null)
            {
                armyLeader.armyID = a.armyID;
            }

            // add to fief's armies
            armyLocation.armies.Add(a.armyID);

        }
        
        /// <summary>
        /// Responds to the click event of the armyRecruitBtn button, allowing the player 
        /// to create a new army and/or recruit additional troops in the current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armyRecruitBtn_Click(object sender, EventArgs e)
        {
            // get tag from button
            Button button = sender as Button;
            string operation = button.Tag.ToString();

            // get fief
            Fief thisFief = Globals_Client.myPlayerCharacter.location;

            // check for siege
            if (thisFief.siege != null)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You cannot recruit from a fief under siege.  Recruitment cancelled.");
                }
            }

            // if not under siege, proceed
            else
            {
                try
                {
                    // get number of troops specified
                    UInt32 numberWanted = Convert.ToUInt32(this.armyRecruitTextBox.Text);

                    // if no existing army, create one
                    if (operation.Equals("new"))
                    {
                        Army newArmy = new Army(Globals_Game.getNextArmyID(), Globals_Client.myPlayerCharacter.charID, Globals_Client.myPlayerCharacter.charID, Globals_Client.myPlayerCharacter.days, Globals_Client.myPlayerCharacter.location.id);
                        this.addArmy(newArmy);
                    }

                    // recruit troops
                    Globals_Client.myPlayerCharacter.recruitTroops(numberWanted);

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
                this.mantainArmy(Globals_Client.armyToView);

                // refresh display
                this.refreshArmyContainer(Globals_Client.armyToView);
            }
        }

        public void mantainArmy(Army a)
        {
            string toDisplay = "";

            // get cost
            uint maintCost = a.calcArmySize() * 500;

            // get available treasury
            Fief homeFief = Globals_Client.myPlayerCharacter.getHomeFief();
            int availTreas = homeFief.getAvailableTreasury();

            // check if army is already maintained
            if (!a.isMaintained)
            {
                // check if can afford maintenance
                if (maintCost > availTreas)
                {
                    // display 'no' message
                    toDisplay += "Sorry, milord, to maintain this army would cost £" + maintCost + "\r\n";
                    toDisplay += "and you only have £" + availTreas + " available in the home treasury.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
                else
                {
                    // set isMaintained
                    a.isMaintained = true;

                    // deduct funds from treasury
                    homeFief.treasury -= Convert.ToInt32(maintCost);

                    // display confirmation message
                    toDisplay += "Army maintained at a cost of £" + maintCost + ".";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any entourage button
        /// invoking either addToEntourage or removeFromEntourage
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
                if (Globals_Client.charToView.armyID == null)
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
                if (siegeID == null)
                {
                    siegeID = Globals_Client.armyToView.checkIfSiegeDefenderAdditional();
                }

                // if is defender in a siege, disable controls
                if (siegeID != null)
                {
                    this.armyRecruitBtn.Enabled = false;
                    this.armyRecruitTextBox.Enabled = false;
                    this.armyMaintainBtn.Enabled = false;
                    this.armyAppointLeaderBtn.Enabled = false;
                    this.armyAppointSelfBtn.Enabled = false;
                    this.armyTransDropBtn.Enabled = false;
                    this.armyTransDropWhoTextBox.Enabled = false;
                    this.armyTransKnightTextBox.Enabled = false;
                    this.armyTransMAAtextBox.Enabled = false;
                    this.armyTransLCavTextBox.Enabled = false;
                    this.armyTransYeomenTextBox.Enabled = false;
                    this.armyTransFootTextBox.Enabled = false;
                    this.armyTransRabbleTextBox.Enabled = false;
                    this.armyTransPickupBtn.Enabled = false;
                    this.armyDisbandBtn.Enabled = false;
                    this.armyAutoCombatBtn.Enabled = false;
                    this.armyAggroTextBox.Enabled = false;
                    this.armyOddsTextBox.Enabled = false;
                    this.armyCampBtn.Enabled = false;
                    this.armyCampTextBox.Enabled = false;
                    this.armyExamineBtn.Enabled = false;
                    this.armyPillageBtn.Enabled = false;
                    this.armySiegeBtn.Enabled = false;

                    // clear existing information
                    this.armyRecruitTextBox.Text = "";
                    this.armyTransDropWhoTextBox.Text = "";
                    this.armyTransKnightTextBox.Text = "";
                    this.armyTransMAAtextBox.Text = "";
                    this.armyTransLCavTextBox.Text = "";
                    this.armyTransYeomenTextBox.Text = "";
                    this.armyTransFootTextBox.Text = "";
                    this.armyTransRabbleTextBox.Text = "";
                    this.armyAggroTextBox.Text = "";
                    this.armyOddsTextBox.Text = "";
                    this.armyCampTextBox.Text = "";
                }

                // if isn't defender in a siege, enable controls as usual
                else
                {
                    // if player is leading an army but not the one on view, disable 'recruit' button
                    if ((!(Globals_Client.armyToView.leader == Globals_Client.myPlayerCharacter.charID))
                        && (!(Globals_Client.myPlayerCharacter.armyID == null)))
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
                        else if (Globals_Client.myPlayerCharacter.armyID == null)
                        {
                            this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                            this.armyRecruitBtn.Tag = "new";
                        }
                    }


                    // re-enable controls
                    this.armyMaintainBtn.Enabled = true;
                    this.armyAppointLeaderBtn.Enabled = true;
                    this.armyAppointSelfBtn.Enabled = true;
                    this.armyTransDropBtn.Enabled = true;
                    this.armyTransKnightTextBox.Enabled = true;
                    this.armyTransMAAtextBox.Enabled = true;
                    this.armyTransLCavTextBox.Enabled = true;
                    this.armyTransYeomenTextBox.Enabled = true;
                    this.armyTransFootTextBox.Enabled = true;
                    this.armyTransRabbleTextBox.Enabled = true;
                    this.armyTransDropWhoTextBox.Enabled = true;
                    this.armyTransPickupBtn.Enabled = true;
                    this.armyDisbandBtn.Enabled = true;
                    this.armyAutoCombatBtn.Enabled = true;
                    this.armyAggroTextBox.Enabled = true;
                    this.armyOddsTextBox.Enabled = true;
                    this.armyCampBtn.Enabled = true;
                    this.armyCampTextBox.Enabled = true;
                    this.armyExamineBtn.Enabled = true;
                    this.armyPillageBtn.Enabled = true;
                    this.armySiegeBtn.Enabled = true;

                    // check to see if current fief is in rebellion and enable control as appropriate
                    // get fief
                    Fief thisFief = Globals_Game.fiefMasterList[Globals_Client.armyToView.location];
                    if (thisFief.status.Equals('R'))
                    {
                        this.armyQuellRebellionBtn.Enabled = true;
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
                    this.armyTransYeomenTextBox.Text = "0";
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
            bool proceed = true;
            bool adjustDays = true;
            int daysTaken = 0;
            uint totalTroopsToTransfer = 0;

            if (Globals_Client.armyToView != null)
            {
                // run checks on data in fields
                try
                {
                    // labels for troop types
                    string[] troopTypeLabels = new string[] { "knights", "men-at-arms", "light cavalry", "yeomen", "foot", "rabble" };

                    // get number of troops to transfer
                    uint[] troopsToTransfer = new uint[] {0, 0, 0, 0, 0, 0};
                    troopsToTransfer[0] = Convert.ToUInt32(this.armyTransKnightTextBox.Text);
                    troopsToTransfer[1] = Convert.ToUInt32(this.armyTransMAAtextBox.Text);
                    troopsToTransfer[2] = Convert.ToUInt32(this.armyTransLCavTextBox.Text);
                    troopsToTransfer[3] = Convert.ToUInt32(this.armyTransYeomenTextBox.Text);
                    troopsToTransfer[4] = Convert.ToUInt32(this.armyTransFootTextBox.Text);
                    troopsToTransfer[5] = Convert.ToUInt32(this.armyTransRabbleTextBox.Text);

                    // check each troop type; if not enough in army, cancel
                    for (int i = 0; i < troopsToTransfer.Length; i++)
                    {
                        if (troopsToTransfer[i] > Globals_Client.armyToView.troops[i])
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("You don't have enough " + troopTypeLabels[i] + " in your army for that transfer.  Transfer cancelled.");
                            }
                            proceed = false;
                            adjustDays = false;
                        }
                        else
                        {
                            totalTroopsToTransfer += troopsToTransfer[i];
                        }
                    }

                    // if no troops selected for transfer, cancel
                    if ((totalTroopsToTransfer == 0) && (proceed))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You haven't selected any troops for transfer.  Transfer cancelled.");
                        }
                        proceed = false;
                        adjustDays = false;
                    }

                    // if reduces army to < 100 troops, warn
                    if (((Globals_Client.armyToView.calcArmySize() - totalTroopsToTransfer) < 100) && (proceed))
                    {
                        DialogResult dialogResult = MessageBox.Show("This transfer will reduce your army manpower to dangerous levels.  Click OK to proceed.", "Proceed with transfer?", MessageBoxButtons.OKCancel);

                        // if choose to cancel
                        if (dialogResult == DialogResult.Cancel)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Transfer cancelled.");
                            }
                            proceed = false;
                            adjustDays = false;
                        }
                    }

                    // check have minimum days necessary for transfer
                    if (Globals_Client.armyToView.days < 10)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You don't have enough days left for this transfer.  Transfer cancelled.");
                        }
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        // calculate time taken for transfer
                        daysTaken = Globals_Game.myRand.Next(10, 31);

                        // check if have enough days for transfer in this instance
                        if (daysTaken > Globals_Client.armyToView.days)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.");
                            }
                            proceed = false;
                        }
                    }

                    // check transfer recipient exists
                    if (!Globals_Game.pcMasterList.ContainsKey(this.armyTransDropWhoTextBox.Text))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify transfer recipient.  Transfer cancelled.");
                        }
                        proceed = false;
                    }

                    if (proceed)
                    {
                        // remove troops from army
                        for (int i = 0; i < Globals_Client.armyToView.troops.Length; i++)
                        {
                            Globals_Client.armyToView.troops[i] -= troopsToTransfer[i];
                        }

                        // get fief
                        Fief thisFief = Globals_Game.fiefMasterList[Globals_Client.armyToView.location];

                        // create transfer entry
                        string[] thisTransfer = new string[9] { Globals_Client.myPlayerCharacter.charID, this.armyTransDropWhoTextBox.Text,
                            troopsToTransfer[0].ToString(), troopsToTransfer[1].ToString(), troopsToTransfer[2].ToString(),
                            troopsToTransfer[3].ToString(), troopsToTransfer[4].ToString(), troopsToTransfer[5].ToString(),
                            (Globals_Client.armyToView.days - daysTaken).ToString() };

                        // add to fief's troopTransfers list
                        thisFief.troopTransfers.Add(Globals_Game.getNextDetachmentID(), thisTransfer);
                    }

                    if (adjustDays)
                    {
                        // get leader
                        Character myLeader = Globals_Client.armyToView.getLeader();

                        // adjust days
                        myLeader.adjustDays(daysTaken);

                        // calculate possible attrition for army
                        byte attritionChecks = Convert.ToByte(daysTaken / 7);
                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = Globals_Client.armyToView.calcAttrition();
                            // apply attrition
                            Globals_Client.armyToView.applyTroopLosses(attritionModifer);
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
        /// Disbands the specified army
        /// </summary>
        /// <param name="a">Army to be disbanded</param>
        public void disbandArmy(Army a)
        {
            // check for siege involvement
            string siegeID = a.checkForSiegeRole();
            Siege thisSiege = null;
            if (siegeID != null)
            {
                thisSiege = Globals_Game.siegeMasterList[siegeID];
            }

            // remove from siege
            if (thisSiege != null)
            {
                // check if are additional defending army
                string whichRole = a.checkIfSiegeDefenderAdditional();
                if (whichRole != null)
                {
                    thisSiege.defenderAdditional = null;
                }

                // check if are besieging army
                else
                {
                    whichRole = a.checkIfBesieger();
                    if (whichRole != null)
                    {
                        thisSiege.besiegerArmy = null;
                    }
                }
            }

            // remove from fief
            Fief thisFief = a.getLocation();
            thisFief.armies.Remove(a.armyID);

            // remove from owner
            PlayerCharacter thisOwner = a.getOwner();
            thisOwner.myArmies.Remove(a);

            // remove from leader
            Character thisLeader = a.getLeader();
            if (thisLeader != null)
            {
                thisLeader.armyID = null;
            }

            // remove from armyMasterList
            Globals_Game.armyMasterList.Remove(a.armyID);

            // set army to null
            a = null;
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

                    // check values and alter if appropriate
                    if (newAggroLevel < 0)
                    {
                        newAggroLevel = 0;
                    }
                    else if (newAggroLevel > 2)
                    {
                        newAggroLevel = 2;
                    }
                    if (newOddsValue < 0)
                    {
                        newOddsValue = 0;
                    }
                    else if (newOddsValue > 9)
                    {
                        newOddsValue = 9;
                    }

                    // update army's values
                    Globals_Client.armyToView.aggression = newAggroLevel;
                    Globals_Client.armyToView.combatOdds = newOddsValue;
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
                    this.campWaitHere(thisLeader, campDays);
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

            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            // if no army selected
            if (this.armyListView.SelectedItems.Count < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No army selected!");
                }
            }

            // if army selected
            else
            {
                // check if has minimum days
                if (Globals_Client.armyToView.days < 1)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                    }
                }

                // has minimum days
                else
                {
                    // see how long reconnaissance takes
                    int reconDays = Globals_Game.myRand.Next(1, 4);

                    // check if runs out of time
                    if (Globals_Client.armyToView.days < reconDays)
                    {
                        // set days to 0
                        thisLeader.adjustDays(Globals_Client.armyToView.days);
                        this.refreshArmyContainer(Globals_Client.armyToView);
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
                        }
                    }

                    // doesn't run out of time
                    else
                    {
                        // adjust days
                        thisLeader.adjustDays(reconDays);
                        this.refreshArmyContainer(Globals_Client.armyToView);

                        // display armies list
                        SelectionForm examineArmies = new SelectionForm(this, "armies", obs: thisLeader);
                        examineArmies.Show();
                    }

                }

            }

        }

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

            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            // if no NPC selected
            if (this.houseCharListView.SelectedItems.Count < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No NPC selected!");
                }
            }

            // if NPC selected
            else
            {
                // check if has minimum days
                if (Globals_Client.charToView.days < 1)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                    }
                }

                // has minimum days
                else
                {
                    // see how long reconnaissance takes
                    int reconDays = Globals_Game.myRand.Next(1, 4);

                    // check if runs out of time
                    if (Globals_Client.charToView.days < reconDays)
                    {
                        // set days to 0
                        Globals_Client.charToView.adjustDays(Globals_Client.armyToView.days);
                        this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
                        }
                    }

                    // doesn't run out of time
                    else
                    {
                        // adjust days
                        Globals_Client.charToView.adjustDays(reconDays);
                        this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));

                        // display armies list
                        SelectionForm examineArmies = new SelectionForm(this, "armies", obs: thisObserver);
                        examineArmies.Show();
                    }

                }

            }

        }

        /// <summary>
        /// Responds to the click event of the travelExamineArmiesBtn button
        /// displaying a list of all armies in the Player's current fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            // check if has minimum days
            if (Globals_Client.myPlayerCharacter.days < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                }
            }

            // has minimum days
            else
            {
                // see how long reconnaissance takes
                int reconDays = Globals_Game.myRand.Next(1, 4);

                // check if runs out of time
                if (Globals_Client.myPlayerCharacter.days < reconDays)
                {
                    // set days to 0
                    Globals_Client.myPlayerCharacter.adjustDays(Globals_Client.myPlayerCharacter.days);
                    this.refreshTravelContainer();
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
                    }
                }

                // doesn't run out of time
                else
                {
                    // adjust days
                    Globals_Client.myPlayerCharacter.adjustDays(reconDays);
                    this.refreshTravelContainer();

                    // display armies list
                    SelectionForm examineArmies = new SelectionForm(this, "armies", obs: Globals_Client.myPlayerCharacter);
                    examineArmies.Show();
                }

            }

        }

        /// <summary>
        /// Calculates battle values of both armies participating in a battle or siege
        /// </summary>
        /// <returns>uint[] containing battle values of attacking & defending armies</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        public uint[] calculateBattleValue(Army attacker, Army defender, int keepLvl = 0)
        {
            uint[] battleValues = new uint[2];
            double attackerLV = 0;
            double defenderLV = 0;

            // get leaders
            Character attackerLeader = attacker.getLeader();
            Character defenderLeader = defender.getLeader();

            // get leadership values for each army leader
            attackerLV = attackerLeader.getLeadershipValue();

            // defender may not have leader
            if (defenderLeader != null)
            {
                defenderLV = defenderLeader.getLeadershipValue();
            }
            else
            {
                defenderLV = 4;
            }

            // calculate battle modifier based on LVs
            // determine highest/lowest of 2 LVs
            double maxLV = Math.Max(attackerLV, defenderLV);
            double minLV = Math.Min(attackerLV, defenderLV);
            double battleModifier = maxLV / minLV;

            // get base combat value for each army
            uint attackerCV = Convert.ToUInt32(attacker.calculateCombatValue());
            uint defenderCV = Convert.ToUInt32(defender.calculateCombatValue(keepLvl));

            // apply battle modifer to the army CV corresponding to the highest LV
            if (attackerLV == maxLV)
            {
                attackerCV = Convert.ToUInt32(attackerCV * battleModifier);
            }
            else
            {
                defenderCV = Convert.ToUInt32(defenderCV * battleModifier);
            }

            battleValues[0] = attackerCV;
            battleValues[1] = defenderCV;

            return battleValues;
        }

        /// <summary>
        /// Calculates whether the attacking army is able to successfully bring
        /// the defending army to battle
        /// </summary>
        /// <returns>bool indicating whether battle has commenced</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        public bool bringToBattle(uint attackerValue, uint defenderValue, string circumstance = "battle")
        {
            bool battleHasCommenced = false;
            double[] combatOdds = Globals_Server.battleProbabilities["odds"];
            double[] battleChances = Globals_Server.battleProbabilities[circumstance];
            double thisChance = 0;

            for (int i = 0; i < combatOdds.Length; i++)
            {
                if (i < combatOdds.Length - 1)
                {
                    if (attackerValue / defenderValue < combatOdds[i])
                    {
                        thisChance = battleChances[i];
                        break;
                    }
                }
                else
                {
                    thisChance = battleChances[i];
                    break;
                }
            }

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to battleChance
            if (randomPercentage <= thisChance)
            {
                battleHasCommenced = true;
            }

            if (battleHasCommenced)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The attacker has successfully brought the defender to battle");
                }
            }

            return battleHasCommenced;
        }

        /// <summary>
        /// Determines whether the attacking army is victorious in a battle
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public bool decideBattleVictory(uint attackerValue, uint defenderValue)
        {
            bool attackerVictorious = false;

            // calculate chance of victory
            double attackerVictoryChance = this.calcVictoryChance(attackerValue, defenderValue);

            // generate random percentage
            int randomPercentage = Globals_Game.myRand.Next(101);

            // compare random percentage to attackerVictoryChance
            if (randomPercentage <= attackerVictoryChance)
            {
                attackerVictorious = true;
            }

            return attackerVictorious;
        }

        /// <summary>
        /// Calculates chance that the attacking army will be victorious in a battle
        /// </summary>
        /// <returns>double containing percentage chance of victory</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public double calcVictoryChance(uint attackerValue, uint defenderValue)
        {
            return (attackerValue / (Convert.ToDouble(attackerValue + defenderValue))) * 100;
        }

        /// <summary>
        /// Calculates casualties from a battle for both sides
        /// </summary>
        /// <returns>double[] containing percentage loss modifier for each side</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="attackerVictorious">bool indicating whether attacking army was victorious</param>
        public double[] calculateBattleCasualties(uint attackerValue, uint defenderValue, bool attackerVictorious)
        {
            double[] battleCasualties = new double[2];

            // generate casualty increments
            double winnerIncrement = Globals_Game.GetRandomDouble(min: 0.01, max: 0.02);
            double loserIncrement = Globals_Game.GetRandomDouble(min: 0.04, max: 0.08);

            // determine highest/lowest battle value
            double maxBV = Math.Max(attackerValue, defenderValue);
            double minBV = Math.Min(attackerValue, defenderValue);

            // derive increment multiplier
            double incrementMultiplier = maxBV / minBV;

            // calculate casualty increment
            if (attackerVictorious)
            {
                battleCasualties[0] = winnerIncrement * incrementMultiplier;
                battleCasualties[1] = loserIncrement * incrementMultiplier;
            }
            else
            {
                battleCasualties[0] = loserIncrement * incrementMultiplier;
                battleCasualties[1] = winnerIncrement * incrementMultiplier;
            }

            return battleCasualties;
        }

        /// <summary>
        /// Calculates whether either army has retreated due to the outcome of a battle
        /// </summary>
        /// <returns>int[] indicating the retreat distance (fiefs) of each army</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="aCasualties">The attacking army casualty modifier</param>
        /// <param name="dCasualties">The defending army casualty modifier</param>
        /// <param name="attackerVictorious">bool indicating if attacking army was victorious</param>
        public int[] checkForRetreat(Army attacker, Army defender, double aCasualties, double dCasualties, bool attackerVictorious)
        {
            bool[] hasRetreated = {false, false};
            int[] retreatDistance = {0, 0};

            // check if loser retreats due to battlefield casualties
            if (!attackerVictorious)
            {
                // if have >= 20% casualties
                if (aCasualties >= 0.2)
                {
                    // indicate attacker has retreated
                    hasRetreated[0] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[0] = Globals_Game.myRand.Next(1, 3);
                }
            }
            else
            {
                // if have >= 20% casualties
                if (dCasualties >= 0.2)
                {
                    // indicate defender has retreated
                    hasRetreated[1] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[1] = Globals_Game.myRand.Next(1, 3);
                }
            }

            // check to see if defender retreats due to aggression setting (i.e. was forced into battle)
            // NOTE: this will only happen if attacker and defender still present in fief
            if ((defender.aggression == 0) && (!hasRetreated[0] && !hasRetreated[1]))
            {
                // indicate defender has retreated
                hasRetreated[1] = true;

                // indicate retreat distance
                retreatDistance[1] = 1;
            }

            return retreatDistance;
        }

        /// <summary>
        /// Processes the retreat of an army
        /// </summary>
        /// <param name="a">The army to retreat</param>
        /// <param name="retreatDistance">The retreat distance</param>
        public void processRetreat(Army a, int retreatDistance)
        {
            // get fief to retreat from
            Fief retreatFrom = a.getLocation();

            // get army leader
            Character thisLeader = a.getLeader();

            // get army owner
            PlayerCharacter thisOwner = a.getOwner();

            // for each hex in retreatDistance, process retreat
            for (int i = 0; i < retreatDistance; i++ )
            {
                // get current location
                Fief from = a.getLocation();

                // get fief to retreat to
                Fief target = Globals_Game.gameMap.chooseRandomHex(from, true, thisOwner, retreatFrom);

                // get travel cost
                double travelCost = this.getTravelCost(from, target);

                // check for army leader (defender may not have had one)
                if (thisLeader != null)
                {
                    // ensure leader has enough days (retreats are immune to running out of days)
                    if (thisLeader.days < travelCost)
                    {
                        thisLeader.adjustDays(thisLeader.days - travelCost);
                    }

                    // perform retreat
                    bool success = this.moveCharacter(thisLeader, target, travelCost);
                }

                // if no leader
                else
                {
                    // ensure army has enough days (retreats are immune to running out of days)
                    if (a.days < travelCost)
                    {
                        a.days = travelCost;
                    }

                    // perform retreat
                    a.moveWithoutLeader(target, travelCost);
                }

            }

        }

        /// <summary>
        /// Elects a new leader from NPCs accompanying an army (upon death of PC leader)
        /// </summary>
        /// <returns>The new leader</returns>
        /// <param name="attacker">List<NonPlayerCharacter> containing candidates for the post</param>
        public NonPlayerCharacter electNewArmyLeader(List<NonPlayerCharacter> candidates)
        {
            NonPlayerCharacter newLeader = null;

            double highestRating = 0;

            foreach (NonPlayerCharacter candidate in candidates)
            {
                double armyLeaderRating = candidate.calcArmyLeadershipRating();

                if (armyLeaderRating > highestRating)
                {
                    highestRating = armyLeaderRating;
                    newLeader = candidate;
                }
            }

            return newLeader;
        }

        /// <summary>
        /// Implements the processes involved in a battle between two armies in the field
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="circumstance">string indicating circumstance of battle</param>
        public bool giveBattle(Army attacker, Army defender, string circumstance = "battle")
        {
            string toDisplay = "";
            bool attackerVictorious = false;
            bool battleHasCommenced = false;
            uint[] battleValues = new uint[2];
            double[] casualtyModifiers = new double[2];
            double statureChange = 0;

            // if applicable, get siege
            Siege thisSiege = null;
            string thisSiegeID = defender.checkIfBesieger();
            if (thisSiegeID != null)
            {
                // get siege
                thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
            }            
            
            // get starting troop numbers
            uint attackerStartTroops = attacker.calcArmySize();
            uint defenderStartTroops = defender.calcArmySize();
            uint attackerCasualties = 0;
            uint defenderCasualties = 0;

            // get leaders
            Character attackerLeader = attacker.getLeader();
            Character defenderLeader = defender.getLeader();

            // introductory text for message
            switch (circumstance)
            {
                case"pillage":
                    toDisplay += "The fief garrison and militia, led by " + attackerLeader.firstName
                        + " " + attackerLeader.familyName + ", sallied forth to bring the pillaging "
                        + defender.armyID + ", led by " + defenderLeader.firstName + " "
                        + defenderLeader.familyName + " and owned by " + defender.getOwner().firstName
                        + " " + defender.getOwner().familyName + " to battle."
                        + "\r\n\r\nOutcome: ";
                    break;
                case "siege":
                    toDisplay += "The fief garrison and militia, led by " + attackerLeader.firstName
                        + " " + attackerLeader.familyName + ", sallied forth to bring the besieging "
                        + defender.armyID + ", led by " + defenderLeader.firstName + " "
                        + defenderLeader.familyName + " and owned by " + defender.getOwner().firstName
                        + " " + defender.getOwner().familyName + " to battle."
                        + "\r\n\r\nOutcome: ";
                    break;
                default:
                    toDisplay += "On this day of our lord " + attacker.armyID + ",";
                    if (attackerLeader != null)
                    {
                        toDisplay += " led by " + attackerLeader.firstName + " " + attackerLeader.familyName + " and";
                    }
                    toDisplay += " owned by "
                        + attacker.getOwner().firstName + " " + attacker.getOwner().familyName
                        + ", moved to attack " + defender.armyID + ",";
                    if (defenderLeader != null)
                    {
                        toDisplay += " led by " + defenderLeader.firstName + " " + defenderLeader.familyName + " and";
                    }
                    toDisplay += " owned by " + defender.getOwner().firstName
                        + " " + defender.getOwner().familyName + ", in the fief of " + attacker.getLocation().name
                        + "\r\n\r\nOutcome: ";
                    break;
            }

            // get battle values for both armies
            battleValues = this.calculateBattleValue(attacker, defender);

            // check if attacker has managed to bring defender to battle
            // NOTE 1: defender aggression isn't a factor in pillage battles
            // NOTE 2: battle always occurs if defending army sallies during siege
            if (((defender.aggression != 0) && (!circumstance.Equals("pillage"))) || (circumstance.Equals("siege")))
            {
                battleHasCommenced = true;
            }
            else
            {
                battleHasCommenced = this.bringToBattle(battleValues[0], battleValues[1], circumstance);
            }

            if (battleHasCommenced)
            {
                // WHO HAS WON?
                // calculate if attacker has won
                attackerVictorious = this.decideBattleVictory(battleValues[0], battleValues[1]);

                // UPDATE STATURE
                // get winner and loser
                Army winner = null;
                Army loser = null;
                if (attackerVictorious)
                {
                    winner = attacker;
                    loser = defender;
                }
                else
                {
                    winner = defender;
                    loser = attacker;
                }

                // calculate and apply winner's stature increase
                statureChange = 0.8 * (loser.calcArmySize() / Convert.ToDouble(10000));
                winner.getOwner().statureModifier += statureChange;

                // calculate and apply loser's stature loss
                statureChange = -0.5 * (winner.calcArmySize() / Convert.ToDouble(10000));
                loser.getOwner().statureModifier += statureChange;

                // CASUALTIES
                // calculate troop casualties for both sides
                casualtyModifiers = this.calculateBattleCasualties(battleValues[0], battleValues[1], attackerVictorious);

                // check if losing army has disbanded
                bool attackerDisbanded = false;
                bool defenderDisbanded = false;
                uint totalAttackTroopsLost = 0;
                uint totalDefendTroopsLost = 0;

                // if losing side sustains >= 50% casualties, disbands
                if (attackerVictorious)
                {
                    // either indicate losing army to be disbanded
                    if (casualtyModifiers[1] >= 0.5)
                    {
                        defenderDisbanded = true;
                        totalDefendTroopsLost = defender.calcArmySize();
                    }
                    // OR apply troop casualties to losing army
                    else
                    {
                        totalDefendTroopsLost = defender.applyTroopLosses(casualtyModifiers[1]);
                    }

                    // apply troop casualties to winning army
                    totalAttackTroopsLost = attacker.applyTroopLosses(casualtyModifiers[0]);
                }
                else
                {
                    if (casualtyModifiers[0] >= 0.5)
                    {
                        attackerDisbanded = true;
                        totalAttackTroopsLost = attacker.calcArmySize();
                    }
                    else
                    {
                        totalAttackTroopsLost = attacker.applyTroopLosses(casualtyModifiers[0]);
                    }

                    totalDefendTroopsLost = defender.applyTroopLosses(casualtyModifiers[1]);
                }

                // UPDATE TOTAL SIEGE LOSSES, if appropriate
                // NOTE: the defender in this battle is the attacker in the siege and v.v.
                if (thisSiege != null)
                {
                    // update total defender siege losses
                    thisSiege.totalCasualtiesDefender += Convert.ToInt32(totalAttackTroopsLost);
                    // update total attacker siege losses
                    thisSiege.totalCasualtiesAttacker += Convert.ToInt32(totalDefendTroopsLost);
                }

                // get casualty figures (for message)
                if (!attackerDisbanded)
                {
                    // get attacker casualties
                    attackerCasualties = totalAttackTroopsLost;
                }
                if (!defenderDisbanded)
                {
                    // get defender casualties
                    defenderCasualties = totalDefendTroopsLost;
                }

                // DAYS
                // adjust days
                // NOTE: don't adjust days if is a siege (will be deducted elsewhere)
                if (!circumstance.Equals("siege"))
                {
                    attackerLeader.adjustDays(1);
                    // need to check for defender having no leader
                    if (defenderLeader != null)
                    {
                        defenderLeader.adjustDays(1);
                    }
                    else
                    {
                        attacker.days--;
                    }
                }

                // RETREATS
                // create array of armies (for easy processing)
                Army[] bothSides = { attacker, defender };

                // check if either army needs to retreat
                int[] retreatDistances = this.checkForRetreat(attacker, defender, casualtyModifiers[0], casualtyModifiers[1], attackerVictorious);

                // if is pillage or siege, attacking army (the fief's army) doesn't retreat
                // if is pillage, the defending army (the pillagers) always retreats if has lost
                if (circumstance.Equals("pillage") || circumstance.Equals("siege"))
                {
                    retreatDistances[0] = 0;
                }

                if (circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        retreatDistances[1] = 1;
                    }
                }

                // if have retreated, perform it
                for (int i = 0; i < retreatDistances.Length; i++ )
                {
                    if (retreatDistances[i] > 0)
                    {
                        this.processRetreat(bothSides[i], retreatDistances[i]);
                    }
                }

                // PC/NPC INJURIES/DEATHS
                // check if any PCs/NPCs have been wounded or killed
                bool characterDead = false;

                // 1. ATTACKER
                bool attackerLeaderDead = false;
                uint friendlyBV = battleValues[0];
                uint enemyBV = battleValues[1];

                // if army leader a PC, check entourage
                if (attackerLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (attackerLeader as PlayerCharacter).myNPCs.Count; i++)
                    {
                        if ((attackerLeader as PlayerCharacter).myNPCs[i].inEntourage)
                        {
                            characterDead = (attackerLeader as PlayerCharacter).myNPCs[i].calculateCombatInjury(casualtyModifiers[0]);
                        }

                        // process death, if applicable
                        if (characterDead)
                        {
                            (attackerLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                        }
                    }
                }

                // check army leader
                attackerLeaderDead = attackerLeader.calculateCombatInjury(casualtyModifiers[0]);

                // process death, if applicable
                if (attackerLeaderDead)
                {
                    Character newLeader = null;

                    // if is pillage, do NOT elect new leader for attacking army
                    if (!circumstance.Equals("pillage"))
                    {
                        // if possible, elect new leader from entourage
                        if (attackerLeader is PlayerCharacter)
                        {
                            if ((attackerLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = this.electNewArmyLeader((attackerLeader as PlayerCharacter).myNPCs);
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        attacker.assignNewLeader(newLeader);
                    }

                    // process death
                    attackerLeader.processDeath("injury");
                }
                else
                {
                    // if pillage, if fief's army loses, make sure bailiff always returns to keep
                    if (circumstance.Equals("pillage"))
                    {
                        if (!attackerVictorious)
                        {
                            attackerLeader.inKeep = true;
                        }
                    }
                }

                // 2. DEFENDER
                bool defenderLeaderDead = false;

                // need to check if defending army had a leader
                if (defenderLeader != null)
                {
                    // if army leader a PC, check entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs[i].inEntourage)
                            {
                                characterDead = (defenderLeader as PlayerCharacter).myNPCs[i].calculateCombatInjury(casualtyModifiers[1]);
                            }

                            // process death, if applicable
                            if (characterDead)
                            {
                                (defenderLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                            }
                        }
                    }

                    // check army leader
                    defenderLeaderDead = defenderLeader.calculateCombatInjury(casualtyModifiers[1]);

                    // process death, if applicable
                    if (defenderLeaderDead)
                    {
                        Character newLeader = null;

                        // if possible, elect new leader from entourage
                        if (defenderLeader is PlayerCharacter)
                        {
                            if ((defenderLeader as PlayerCharacter).myNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = this.electNewArmyLeader((defenderLeader as PlayerCharacter).myNPCs);
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        defender.assignNewLeader(newLeader);

                        // process death
                        defenderLeader.processDeath("injury");
                    }
                }

                // UPDATE MESSAGE
                // who won
                if (attackerVictorious)
                {
                    toDisplay += attacker.armyID;
                }
                else
                {
                    toDisplay += defender.armyID;
                }
                toDisplay += " was victorious.\r\n\r\n";

                // casualties & retreats - attacker
                if (attackerDisbanded)
                {
                    toDisplay += attacker.armyID + " disbanded due to heavy casualties";
                }
                else
                {
                    toDisplay += attacker.armyID + " suffered a total of " + attackerCasualties + " casualties";
                    if (retreatDistances[0] > 0)
                    {
                        toDisplay += " and retreated from the fief";
                    }
                }
                toDisplay += ".";
                if (attackerLeaderDead)
                {
                    toDisplay += " " + attackerLeader.firstName + " " + attackerLeader.familyName + " died due to injuries received.";
                }
                toDisplay += "\r\n\r\n";

                // casualties & retreats - defender
                if (defenderDisbanded)
                {
                    toDisplay += defender.armyID + " disbanded due to heavy casualties";
                }
                else
                {
                    toDisplay += defender.armyID + " suffered a total of " + defenderCasualties + " casualties";
                    if (retreatDistances[1] > 0)
                    {
                        toDisplay += " and retreated from the fief";
                    }
                }
                toDisplay += ".";
                if (defenderLeaderDead)
                {
                    toDisplay += " " + defenderLeader.firstName + " " + defenderLeader.familyName + " died due to injuries received.";
                }
                toDisplay += "\r\n\r\n";

                if (circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        toDisplay += "The pillage in " + attacker.getLocation().name + " has been prevented.";
                    }
                }

                // check for SIEGE RELIEF
                // attacker (relieving army) victory or defender (besieging army) retreat = relief
                if ((attackerVictorious) || (retreatDistances[1] > 0))
                {
                    // check to see if defender was besieging the fief keep
                    if (thisSiege != null)
                    {
                        // construct event description to be passed into siegeEnd
                        string siegeDescription = "On this day of Our Lord the forces of ";
                        siegeDescription += attacker.getOwner().firstName + " " + attacker.getOwner().familyName;
                        siegeDescription += " have defeated the forces of " + thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                        siegeDescription += ", relieving the siege of " + thisSiege.getFief().name + ".";
                        siegeDescription += " " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                        siegeDescription += " retains ownership of the fief.";

                        // end the siege
                        this.siegeEnd(thisSiege, siegeDescription);

                        // add to message
                        toDisplay += "The siege in " + thisSiege.getFief().name + " has been raised.";

                    }
                }

                // DISBANDMENT

                // if is pillage, attacking (temporary) army always disbands after battle
                if (circumstance.Equals("pillage"))
                {
                    attackerDisbanded = true;
                }

                // process army disbandings (after all other functions completed)
                if (attackerDisbanded)
                {
                    this.disbandArmy(attacker);
                }

                if (defenderDisbanded)
                {
                    this.disbandArmy(defender);
                }

            }
            else
            {
                if ((circumstance.Equals("pillage")) || circumstance.Equals("siege"))
                {
                    toDisplay += attacker.armyID + " was unsuccessfull in bringing " + defender.armyID + " to battle.";
                }
                else
                {
                    toDisplay += defender.armyID + " successfully refused battle and retreated from the fief.";
                }
            }

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            // personae tags vary depending on circumstance
            string attackOwnTag = "|attackerOwner";
            string attackLeadTag = "|attackerLeader";
            string defendOwnTag = "|defenderOwner";
            string defendLeadTag = "|defenderLeader";
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("siege")))
            {
                attackOwnTag = "|sallyOwner";
                attackLeadTag = "|sallyLeader";
                defendOwnTag = "|defenderAgainstSallyOwner";
                defendLeadTag = "|defenderAgainstSallyLeader";
            }
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(defender.getOwner().charID + defendOwnTag);
            tempPersonae.Add(attackerLeader.charID + attackLeadTag);
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + defendLeadTag);
            }
            tempPersonae.Add(attacker.getOwner().charID + attackOwnTag);
            tempPersonae.Add(attacker.getLocation().owner.charID + "|fiefOwner");
            string[] battlePersonae = tempPersonae.ToArray();
            
            // location
            string battleLocation = attacker.getLocation().id;
 
            // use popup text as description
            string battleDescription = toDisplay;

            // put together new journal entry
            JournalEntry battleResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, battlePersonae, "battle", loc: battleLocation, descr: battleDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(battleResult);

            // display pop-up informational message
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(toDisplay, "BATTLE RESULTS");
            }

            // refresh screen
            this.refreshCurrentScreen();

            return attackerVictorious;

        }

        /// <summary>
        /// Calculates the outcome of the pillage of a fief by an army
        /// </summary>
        /// <param name="f">The fief being pillaged</param>
        /// <param name="a">The pillaging army</param>
        /// <param name="circumstance">The circumstance under which the fief is being pillaged</param>
        public void processPillage(Fief f, Army a, string circumstance = "pillage")
        {
            string pillageResults = "";
            double thisLoss = 0;
            double moneyPillagedTotal = 0;
            double moneyPillagedOwner = 0;
            double pillageMultiplier = 0;

            // get army leader
            Character armyLeader = a.getLeader();

            // get pillaging army owner (receives a proportion of total spoils)
            PlayerCharacter armyOwner = a.getOwner();

            // get garrison leader (to add to journal entry)
            Character defenderLeader = f.bailiff;

            // calculate pillageMultiplier (based on no. pillagers per 1000 population)
            pillageMultiplier = a.calcArmySize() / (f.population / 1000);

            // calculate days taken for pillage
            double daysTaken = Globals_Game.myRand.Next(7, 16);
            if (daysTaken > a.days)
            {
                daysTaken = a.days;
            }

            // update army days
            armyLeader.adjustDays(daysTaken);

            pillageResults += "- Days taken: " + daysTaken + "\r\n";

            // % population loss
            thisLoss = (0.007 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply population loss
            pillageResults += "- Population loss: " + Convert.ToUInt32((f.population * (thisLoss / 100))) + "\r\n";
            f.population -= Convert.ToInt32((f.population * (thisLoss / 100)));

            // % treasury loss
            if (!circumstance.Equals("quellRebellion"))
            {
                thisLoss = (0.2 * pillageMultiplier);
                // ensure is at least 1%
                if (thisLoss < 1)
                {
                    thisLoss = 1;
                }
                // apply treasury loss
                pillageResults += "- Treasury loss: " + Convert.ToInt32((f.treasury * (thisLoss / 100))) + "\r\n";
                if (f.treasury > 0)
                {
                    f.treasury -= Convert.ToInt32((f.treasury * (thisLoss / 100)));
                }
            }

            // % loyalty loss
            thisLoss = (0.33 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply loyalty loss
            pillageResults += "- Loyalty loss: " + (f.loyalty * (thisLoss / 100)) + "\r\n";
            f.loyalty -= (f.loyalty * (thisLoss / 100));

            // % fields loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply fields loss
            pillageResults += "- Fields loss: " + (f.fields * (thisLoss / 100)) + "\r\n";
            f.fields -= (f.fields * (thisLoss / 100));

            // % industry loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply industry loss
            pillageResults += "- Industry loss: " + (f.industry * (thisLoss / 100)) + "\r\n";
            f.industry -= (f.industry * (thisLoss / 100));

            // money pillaged (based on GDP)
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // calculate base amount pillaged
            double baseMoneyPillaged = (f.keyStatsCurrent[1] * (thisLoss / 100));
            moneyPillagedTotal = baseMoneyPillaged;
            pillageResults += "- Base Money Pillaged: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";

            // factor in no. days spent pillaging (get extra 5% per day > 7)
            int daysOver7 = Convert.ToInt32(daysTaken) - 7;
            if (daysOver7 > 0)
            {
                for (int i = 0; i < daysOver7; i++)
                {
                    moneyPillagedTotal += (baseMoneyPillaged * 0.05);
                }
                pillageResults += "  - with bonus for extra " + daysOver7 + " days taken: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";
            }

            // check for jackpot
            // generate randomPercentage to see if hit the jackpot
            int myRandomPercent = Globals_Game.myRand.Next(101);
            if (myRandomPercent <= 30)
            {
                // generate random int to multiply amount pillaged
                int myRandomMultiplier = Globals_Game.myRand.Next(3, 11);
                moneyPillagedTotal = moneyPillagedTotal * myRandomMultiplier;
                pillageResults += "  - with bonus for jackpot: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";
            }

            // check proportion of money pillaged goes to army owner (based on stature)
            double proportionForOwner = 0.05 * armyOwner.calculateStature();
            moneyPillagedOwner = (moneyPillagedTotal * proportionForOwner);
            pillageResults += "- Money pillaged by attacking player: " + Convert.ToInt32(moneyPillagedOwner) + "\r\n";

            // apply to army owner's home fief treasury
            armyOwner.getHomeFief().treasury += Convert.ToInt32(moneyPillagedOwner);

            // apply loss of stature to army owner if fief has same language
            if (armyOwner.language.id == f.language.id)
            {
                armyOwner.statureModifier += -0.3;
            }
            else if (armyOwner.language.baseLanguage.id == f.language.baseLanguage.id)
            {
                armyOwner.statureModifier += -0.2;
            }

            // set isPillaged for fief
            f.isPillaged = true;

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(f.owner.charID + "|fiefOwner");
            tempPersonae.Add(armyOwner.charID + "|attackerOwner");
            if (armyLeader != null)
            {
                tempPersonae.Add(armyLeader.charID + "|attackerLeader");
            }
            if ((defenderLeader != null) && (!circumstance.Equals("quellRebellion")))
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderLeader");
            }
            string[] pillagePersonae = tempPersonae.ToArray();

            // location
            string pillageLocation = f.id;

            // type
            string type = "";
            if (circumstance.Equals("pillage"))
            {
                type += "pillage";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                type += "rebellionQuelled";
            }

            // use popup text as description
            string pillageDescription = "";

            if (circumstance.Equals("pillage"))
            {
                pillageDescription += "On this day of Our Lord the fief of ";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageDescription += "On this day of Our Lord the rebellion in the fief of ";
            }
            pillageDescription += f.name + " owned by " + f.owner.firstName + " " + f.owner.familyName;

            if ((circumstance.Equals("pillage")) && (defenderLeader != null))
            {
                if (f.owner != defenderLeader)
                {
                    pillageDescription += " and defended by " + defenderLeader.firstName + " " + defenderLeader.familyName + ",";
                }
            }

            if (circumstance.Equals("pillage"))
            {
                pillageDescription += " was pillaged by the forces of ";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageDescription += " was quelled by the forces of ";
            }
            pillageDescription += armyOwner.firstName + " " + armyOwner.familyName;
            if (armyLeader != null)
            {
                if (armyOwner != armyLeader)
                {
                    pillageDescription += ", led by " + armyLeader.firstName + " " + armyLeader.familyName;
                }
            }
            pillageDescription += ".\r\n\r\nResults:\r\n";
            pillageDescription += pillageResults;

            // put together new journal entry
            JournalEntry pillageEntry = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, pillagePersonae, type, loc: pillageLocation, descr: pillageDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(pillageEntry);

            // show message
            if (Globals_Client.showMessages)
            {
                // set label
                string messageLabel = "";

                if (circumstance.Equals("pillage"))
                {
                    messageLabel += "PILLAGE ";
                }
                else if (circumstance.Equals("quellRebellion"))
                {
                    messageLabel += "QUELL REBELLION ";
                }

                // show message
                System.Windows.Forms.MessageBox.Show(pillageDescription, messageLabel + "RESULTS");
            }
        }

        /// <summary>
        /// Creates a defending army for defence of a fief during pillage or siege
        /// </summary>
        /// <returns>The defending army</returns>
        /// <param name="f">The fief being pillaged</param>
        public Army createDefendingArmy(Fief f)
        {
            Army defender = null;
            Character armyLeader = null;
            string armyLeaderID = null;
            double armyLeaderDays = 90;

            // if present in fief, get bailiff and assign as army leader
            if (f.bailiff != null)
            {
                for (int i = 0; i < f.charactersInFief.Count; i++)
                {
                    if (f.charactersInFief[i] == f.bailiff)
                    {
                        armyLeader = f.bailiff;
                        armyLeaderID = armyLeader.charID;
                        armyLeaderDays = armyLeader.days;
                        break;
                    }
                }
            }

            // gather troops to creat army
            uint garrisonSize = 0;
            uint militiaSize = 0;
            uint[] troopsForArmy = new uint[] { 0, 0, 0, 0, 0, 0 };
            uint[] tempTroops = new uint[] { 0, 0, 0, 0, 0, 0 };
            uint totalSoFar = 0;

            // get army nationality
            string thisNationality = f.owner.nationality.natID.ToUpper();
            if (!thisNationality.Equals("E"))
            {
                thisNationality = "O";
            }

            // get size of fief garrison
            garrisonSize = Convert.ToUInt32(f.getGarrisonSize());

            // get size of fief 'militia' responding to emergency
            militiaSize = Convert.ToUInt32(f.callUpTroops(minProportion: 0.33, maxProportion: 0.66));

            // get defending troops based on following troop type proportions:
            // militia = Global_Server.recruitRatios for types 0-4, fill with rabble
            // garrison = Global_Server.recruitRatios * 2 for types 0-3, fill with foot

            // 1. militia (includes proportion of rabble)
            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = Convert.ToUInt32(militiaSize * Globals_Server.recruitRatios[thisNationality][i]);
                }
                // fill up with rabble
                else
                {
                    tempTroops[i] = militiaSize - totalSoFar;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // 2. garrison (all 'professional' troops)
            totalSoFar = 0;

            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 2)
                {
                    tempTroops[i] = Convert.ToUInt32(garrisonSize * (Globals_Server.recruitRatios[thisNationality][i] * 2));
                }
                // fill up with foot
                else if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = garrisonSize - totalSoFar;
                }
                // no rabble in garrison
                else
                {
                    tempTroops[i] = 0;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // create temporary army for battle/siege
            defender = new Army("Garrison" + Globals_Game.getNextArmyID(), armyLeaderID, f.owner.charID, armyLeaderDays, f.id, trp: troopsForArmy);
            this.addArmy(defender);

            return defender;
        }

        /// <summary>
        /// Implements the processes involved in the pillage of a fief by an army
        /// </summary>
        /// <param name="a">The pillaging army</param>
        /// <param name="f">The fief being pillaged</param>
        public void pillageFief(Army a, Fief f)
        {
            bool pillageCancelled = false;
            bool bailiffPresent = false;
            Army fiefArmy = null;

            // check if bailiff present in fief (he'll lead the army)
            if (f.bailiff != null)
            {
                for (int i = 0; i < f.charactersInFief.Count; i++ )
                {
                    if (f.charactersInFief[i] == f.bailiff)
                    {
                        bailiffPresent = true;
                        break;
                    }
                }
            }

            // if bailiff is present, create an army and attempt to give battle
            // no bailiff = no leader = pillage is unopposed by defending forces
            if (bailiffPresent)
            {
                // create temporary army for battle
                fiefArmy = this.createDefendingArmy(f);

                // give battle and get result
                pillageCancelled = this.giveBattle(fiefArmy, a, circumstance: "pillage");

                if (pillageCancelled)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("The pillaging force has been forced to retreat by the fief's defenders!");
                    }
                }

                else
                {
                    // check still have enough days left
                    if (a.days < 7)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("After giving battle, the pillaging army no longer has\r\nsufficient days for this operation.  Pillage cancelled.");
                        }
                        pillageCancelled = true;
                    }
                }

            }

            if (!pillageCancelled)
            {
                // process pillage
                this.processPillage(f, a);
            }

        }

        /// <summary>
        /// Implements conditional checks prior to the pillage or siege of a fief
        /// </summary>
        /// <returns>bool indicating whether pillage/siege can proceed</returns>
        /// <param name="f">The fief being pillaged/besieged</param>
        /// <param name="a">The pillaging/besieging army</param>
        /// <param name="circumstance">The circumstance - pillage or siege</param>
        public bool checksBeforePillageSiege(Army a, Fief f, string circumstance = "pillage")
        {
            bool proceed = true;
            string operation = "";

            // check if is your own fief
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if (f.owner == a.getOwner())
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot pillage your own fief!  Pillage cancelled.");
                        }
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot besiege your own fief!  Siege cancelled.");
                        }
                    }
                }
            }

            // check if fief is under siege
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if ((f.siege != null) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("You cannot pillage a fief that is under siege.  Pillage cancelled.");
                        }
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("This fief is already under siege.  Siege cancelled.");
                        }
                    }
                }
            }

            // check if fief already pillages
            // note: not necessary for quell rebellion (get a 'free' pillage)
            if (!circumstance.Equals("quellRebellion"))
            {
                if (circumstance.Equals("pillage"))
                {
                    // check isPillaged = false
                    if ((f.isPillaged) && (proceed))
                    {
                        proceed = false;
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("This fief has already been pillaged during\r\nthe current season.  Pillage cancelled.");
                        }
                    }
                }
            }

            // check if your army has a leader
            if (a.getLeader() == null)
            {
                proceed = false;

                if (circumstance.Equals("quellRebellion"))
                {
                    operation = "Operation";
                }
                if (circumstance.Equals("pillage"))
                {
                    operation = "Pillage";
                }
                else if (circumstance.Equals("siege"))
                {
                    operation = "Siege";
                }

                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This army has no leader.  " + operation + " cancelled.");
                }
            }

            // check has min days required
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("quellRebellion")))
            {
                // pillage = min 7
                if ((a.days < 7) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("quellRebellion"))
                    {
                        operation = "Quell rebellion";
                    }
                    else
                    {
                        operation = "Pillage";
                    }

                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na this operation.  " + operation + " cancelled.");
                    }
                }
            }
            else if (circumstance.Equals("siege"))
            {
                // siege = 1 (to set up siege)
                if ((a.days < 1) && (proceed))
                {
                    proceed = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na siege operation.  Siege cancelled.");
                    }
                }
            }

            // check for presence of armies belonging to fief owner
            if (proceed)
            {
                // iterate through armies in fief
                for (int i = 0; i < f.armies.Count; i++)
                {
                    // get army
                    Army armyInFief = Globals_Game.armyMasterList[f.armies[i]];

                    // check if owned by fief owner
                    if (armyInFief.owner.Equals(f.owner.charID))
                    {
                        // army must be outside keep
                        if (!armyInFief.getLeader().inKeep)
                        {
                            // army must have correct aggression settings
                            if (armyInFief.aggression > 1)
                            {
                                proceed = false;
                                if (circumstance.Equals("pillage"))
                                {
                                    operation = "Pillage";
                                }
                                else if (circumstance.Equals("siege"))
                                {
                                    operation = "Siege";
                                }
                                else if (circumstance.Equals("quellRebellion"))
                                {
                                    operation = "Quell rebellion";
                                }

                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("There is at least one defending army (" + armyInFief.armyID + ") that must be defeated\r\nbefore you can conduct this operation.  " + operation + " cancelled.");
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Implements conditional checks prior to a siege operation
        /// </summary>
        /// <returns>bool indicating whether siege operation can proceed</returns>
        /// <param name="s">The siege</param>
        /// <param name="operation">The operation - round or end</param>
        public bool checksBeforeSiegeOperation(Siege s, string operation = "round")
        {
            bool proceed = true;
            int daysRequired = 0;

            if (operation.Equals("round"))
            {
                daysRequired = 10;
            }
            else if (operation.Equals("end"))
            {
                daysRequired = 1;
            }

            // check has min days required
            if (s.days < daysRequired)
            {
                proceed = false;
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("There are not enough days remaining for this\r\na siege operation.  Operation cancelled.");
                }
            }

            return proceed;
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
        /// Calculates rough battle odds between two armies (i.e ratio of attacking army combat
        /// value to defending army combat value).  NOTE: does not involve leadership values
        /// </summary>
        /// <returns>int containing battle odds</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        public int getBattleOdds(Army attacker, Army defender)
        {
            double battleOdds = 0;

            battleOdds = Math.Floor(attacker.calculateCombatValue() / defender.calculateCombatValue());

            return Convert.ToInt32(battleOdds);
        }

        /// <summary>
        /// Ends the specified siege
        /// </summary>
        /// <param name="s">Siege to be ended</param>
        /// <param name="s">String containing circumstances under which the siege ended</param>
        public void siegeEnd(Siege s, string circumstance)
        {
            // get principle objects
            PlayerCharacter defendingPlayer = s.getDefendingPlayer();
            Army defenderGarrison = s.getDefenderGarrison();
            Character defenderLeader = defenderGarrison.getLeader();
            PlayerCharacter besiegingPlayer = s.getBesiegingPlayer();
            Army defenderAdditional = s.getDefenderAdditional();
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.getLeader();
            }
            Fief besiegedFief = s.getFief();
            Character besiegingArmyLeader = s.getBesiegingArmy().getLeader();

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(defendingPlayer.charID + "|fiefOwner");
            tempPersonae.Add(besiegingPlayer.charID + "|attackerOwner");
            tempPersonae.Add(besiegingArmyLeader.charID + "|attackerLeader");
            // get defenderLeader
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            if (addDefendLeader != null)
            {
                tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = besiegedFief.id;

            // description
            string siegeDescription = "";
            if (circumstance == null)
            {
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += besiegingPlayer.firstName + " " + besiegingPlayer.familyName;
                siegeDescription += " abandoned the siege of " + besiegedFief.name;
                siegeDescription += ". The ownership of this fief is retained by ";
                siegeDescription += defendingPlayer.firstName + " " + defendingPlayer.familyName + ".";
            }
            else
            {
                siegeDescription = circumstance;
            }

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeEnd", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // disband garrison
            this.disbandArmy(defenderGarrison);

            // disband additional defending army
            if (defenderAdditional != null)
            {
                this.disbandArmy(defenderAdditional);
            }

            // remove from PCs
            besiegingPlayer.mySieges.Remove(s.siegeID);
            defendingPlayer.mySieges.Remove(s.siegeID);

            // remove from fief
            besiegedFief.siege = null;

            // sync days of all effected objects (to remove influence of attacking leader's skills)
            // work out proportion of seasonal allowance remaining
            double daysProportion = 0;
            if (besiegingArmyLeader != null)
            {
                daysProportion = s.days / besiegingArmyLeader.getDaysAllowance();
            }
            else
            {
                daysProportion = s.days / 90;
            }

            // iterate through characters in fief keep
            foreach (Character thisChar in besiegedFief.charactersInFief)
            {
                if (thisChar.inKeep)
                {
                    // reset character's days to reflect days spent in siege
                    thisChar.days = thisChar.getDaysAllowance() * daysProportion;
                }
            }

            // remove from master list
            Globals_Game.siegeMasterList.Remove(s.siegeID);

            // set to null
            s = null;

            // refresh screen
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Calculates the precentage chance of successfully storming a keep, based on keep level
        /// </summary>
        /// <returns>double containing precentage chance of success</returns>
        /// <param name="keepLvl">The keep level</param>
        public double calcStormSuccess(double keepLvl)
        {
            double stormFailurePercent = 0;

            for (int i = 0; i <= keepLvl; i++ )
            {
                if (i == 0)
                {
                    stormFailurePercent = 5;
                }
                else if (i == 1)
                {
                    stormFailurePercent = 70;
                }
                else
                {
                    stormFailurePercent = stormFailurePercent + (stormFailurePercent * (0.08 * (1 / (keepLvl - 1))));
                }

                // ensure is always slight chance of success
                if (stormFailurePercent > 99)
                {
                    stormFailurePercent = 99;
                }
            }

            // return success % (inverse of stormFailurePercent)
            return 100 - stormFailurePercent;
        }

        /// <summary>
        /// Calculates the keep level prior to a storm by attacking forces in a siege
        /// </summary>
        /// <returns>double containing keep level</returns>
        /// <param name="s">The siege</param>
        public double calcStormKeepLevel(Siege s)
        {
            double keepLvl = 0;
            double keepLvlModifier = 0;
            Fief besiegedFief = s.getFief();
            Army attacker = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            uint[] battleValues = new uint[2];

            // get basic keep level
            keepLvl = besiegedFief.keepLevel;

            // get battle values for both armies
            battleValues = this.calculateBattleValue(attacker, defenderGarrison, Convert.ToInt32(keepLvl));

            // work out keepLvlModifier based on battle values
            uint maxBV = Math.Max(battleValues[0], battleValues[1]);
            uint minBV = Math.Min(battleValues[0], battleValues[1]);

            keepLvlModifier = (maxBV / minBV) - 1;

            // ensure keepLvlModifier is 10 max
            if (keepLvlModifier > 10)
            {
                keepLvlModifier = 10;
            }

            // apply keepLvlModifier depending on which side had highest BV
            if (maxBV == battleValues[0])
            {
                keepLvl -= keepLvlModifier;
            }
            else
            {
                keepLvl += keepLvlModifier;
            }

            return keepLvl;
        }
        
        /// <summary>
        /// Processes the storming of the keep by attacking forces in a siege
        /// </summary>
        /// <param name="s">The siege</param>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public void siegeStormRound(Siege s, uint defenderCasualties, double originalKeepLvl)
        {
            bool stormSuccess = false;
            Fief besiegedFief = s.getFief();
            Army besiegingArmy = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            PlayerCharacter attackingPlayer = s.getBesiegingPlayer();
            Character defenderLeader = defenderGarrison.getLeader();
            Character attackerLeader = besiegingArmy.getLeader();
            double statureChange = 0;

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.getLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = s.getFief().id;

            // description
            string siegeDescription = "";
                
            // get STORM RESULT
            uint[] battleValues = this.calculateBattleValue(besiegingArmy, defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel));
            stormSuccess = this.decideBattleVictory(battleValues[0], battleValues[1]);

            // KEEP DAMAGE
            // base damage to keep level (10%)
            double keepDamageModifier = 0.1;

            // calculate further damage, based on comparative battle values (up to extra 15%)
            // uint [] battleValues = this.calculateBattleValue(besiegingArmy, defenderGarrison, Convert.ToInt32(keepLvl));
            // divide attackerBV by defenderBV to get extraDamageMultiplier
            double extraDamageMultiplier = battleValues[0] / battleValues[1];

            // ensure extraDamageMultiplier is max 10
            if (extraDamageMultiplier > 10)
            {
                extraDamageMultiplier = 10;
            }

            // generate random double 0-1 to see what proportion of extraDamageMultiplier will apply
            double myRandomDouble = Globals_Game.GetRandomDouble(1);
            extraDamageMultiplier =  extraDamageMultiplier * myRandomDouble;

            keepDamageModifier += (0.015 * extraDamageMultiplier);
            keepDamageModifier = (1 - keepDamageModifier);

            // apply keep damage
            besiegedFief.keepLevel = besiegedFief.keepLevel * keepDamageModifier;

            // CASUALTIES, based on comparative battle values and keep level
            // 1. DEFENDER
            // defender base casualtyModifier
            double defenderCasualtyModifier = 0.01;
            defenderCasualtyModifier = defenderCasualtyModifier * (Convert.ToDouble(battleValues[0]) / battleValues[1]);

            // apply casualties
            defenderCasualties += defenderGarrison.applyTroopLosses(defenderCasualtyModifier);
            // update total defender siege losses
            s.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // 2. ATTACKER
            double attackerCasualtyModifier = 0.01;
            attackerCasualtyModifier = attackerCasualtyModifier * (Convert.ToDouble(battleValues[1]) / battleValues[0]);
            // for attacker, add effects of keep level, modified by on storm success
            if (stormSuccess)
            {
                attackerCasualtyModifier += (0.001 * besiegedFief.keepLevel);
            }
            else
            {
                attackerCasualtyModifier += (0.002 * besiegedFief.keepLevel);
            }
            // apply casualties
            uint attackerCasualties = besiegingArmy.applyTroopLosses(attackerCasualtyModifier);
            // update total attacker siege losses
            s.totalCasualtiesAttacker += Convert.ToInt32(attackerCasualties);

            // PC/NPC INJURIES
            // NOTE: defender only (attacker leaders assumed not to have climbed the walls)
            bool characterDead = false;
            if (defenderLeader != null)
            {
                // if defenderLeader is PC, check for casualties amongst entourage
                if (defenderLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++ )
                    {
                        NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                        characterDead = thisNPC.calculateCombatInjury(defenderCasualtyModifier);

                        if (characterDead)
                        {
                            // process death
                            (defenderLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                        }
                    }
                }

                // check defenderLeader
                characterDead = defenderLeader.calculateCombatInjury(defenderCasualtyModifier);

                if (characterDead)
                {
                    // remove as leader
                    defenderGarrison.leader = null;

                    // process death
                    defenderLeader.processDeath("injury");
                }
            }

            if (stormSuccess)
            {
                // pillage fief
                this.processPillage(besiegedFief, besiegingArmy);

                // CAPTIVES
                // identify captives - fief owner, his family, and any PCs of enemy nationality
                List<Character> captives = new List<Character>();
                foreach (Character thisCharacter in besiegedFief.charactersInFief)
                {
                    if (thisCharacter.inKeep)
                    {
                        // fief owner and his family
                        if (thisCharacter.familyID != null)
                        {
                            if (thisCharacter.familyID.Equals(s.getDefendingPlayer().charID))
                            {
                                captives.Add(thisCharacter);
                            }
                        }

                        // PCs of enemy nationality
                        else if (thisCharacter is PlayerCharacter)
                        {
                            if (!thisCharacter.nationality.Equals(attackingPlayer.nationality))
                            {
                                captives.Add(thisCharacter);
                            }
                        }
                    }
                }

                // collect ransom
                int thisRansom = 0;
                int totalRansom = 0;
                foreach (Character thisCharacter in captives)
                {
                    // PCs
                    if (thisCharacter is PlayerCharacter)
                    {
                        // calculate ransom (10% of total GDP)
                        thisRansom = Convert.ToInt32(((thisCharacter as PlayerCharacter).getTotalGDP() * 0.1));
                        // remove from captive's home treasury
                        (thisCharacter as PlayerCharacter).getHomeFief().treasury -= thisRansom;
                    }
                    // NPCs (family of fief's old owner)
                    else
                    {
                        // calculate ransom (family allowance)
                        string thisFunction = (thisCharacter as NonPlayerCharacter).getFunction(s.getDefendingPlayer());
                        thisRansom = Convert.ToInt32((thisCharacter as NonPlayerCharacter).calcFamilyAllowance(thisFunction));
                        // remove from head of family's home treasury
                        s.getDefendingPlayer().getHomeFief().treasury -= thisRansom;
                    }

                    // add to besieger's home treasury
                    attackingPlayer.getHomeFief().treasury += thisRansom;
                    totalRansom += thisRansom;
                }

                // calculate change to besieging player's stature
                statureChange = 0.1 * (s.getFief().population / Convert.ToDouble(10000));

                // construct event description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY stormed the keep of " + s.getFief().name + ".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescriptionFull += "and " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ".\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += s.getDefendingPlayer().firstName + " " + s.getDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureChange + " stature.";
                // details of ransoms
                if (totalRansom > 0)
                {
                    siegeDescriptionFull += "\r\n\r\nA number of persons (";
                    Character lastCaptive = captives.Last();
                    foreach (Character thisCharacter in captives)
                    {
                        siegeDescriptionFull += thisCharacter.firstName + " " + thisCharacter.familyName;
                        if (thisCharacter != lastCaptive)
                        {
                            siegeDescriptionFull += ", ";
                        }
                    }
                    siegeDescriptionFull += ") were ransomed for a total of £" + totalRansom + ".";
                }

                // end the siege
                this.siegeEnd(s, siegeDescriptionFull);

                // change fief ownership
                besiegedFief.changeOwnership(attackingPlayer);
            }

            // storm unsuccessful
            else
            {
                // calculate change to besieging player's stature
                statureChange = -0.2 * (Convert.ToDouble(s.getFief().population) / 10000);

                // description
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " were UNSUCCESSFULL in their attempt to storm the keep of " + s.getFief().name;
                siegeDescription += ".\r\n\r\nTotal casualties numbered " + attackerCasualties + " for the attacking forces ";
                siegeDescription += "and " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".\r\n\r\nThis failure has resulted in a loss of ";
                siegeDescription += statureChange + " for " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // inform player of result
            System.Windows.Forms.MessageBox.Show(siegeDescription);

            // apply change to besieging player's stature
            s.getBesiegingPlayer().statureModifier += statureChange;

            // refresh screen
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Processes a single negotiation round of a siege
        /// </summary>
        /// <returns>bool indicating whether negotiation was successful</returns>
        /// <param name="s">The siege</param>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public bool siegeNegotiationRound(Siege s, uint defenderCasualties, double originalKeepLvl)
        {
            bool negotiateSuccess = false;

            // get required objects
            Fief besiegedFief = s.getFief();
            Army besieger = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Character defenderLeader = defenderGarrison.getLeader();
            Character attackerLeader = besieger.getLeader();
            Army defenderAdditional = s.getDefenderAdditional();

            // =================== start construction of JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.getLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = s.getFief().id;

            // description
            string siegeDescription = "";

            // calculate success chance
            uint[] battleValues = this.calculateBattleValue(besieger, defenderGarrison, Convert.ToInt32(besiegedFief.keepLevel));
            double successChance = this.calcVictoryChance(battleValues[0], battleValues[1]) / 2;

            // generate random double 0-100 to see if storm a success
            double myRandomDouble = Globals_Game.GetRandomDouble(100);

            if (myRandomDouble <= successChance)
            {
                negotiateSuccess = true;
            }

            // negotiation successful
            if (negotiateSuccess)
            {
                // add to winning player's stature
                double statureIncrease = 0.2 * (s.getFief().population / Convert.ToDouble(10000));
                s.getBesiegingPlayer().statureModifier += statureIncrease;

                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " SUCCESSFULLY negotiated an end to the siege of " + s.getFief().name +".";

                // create more detailed description for siegeEnd
                string siegeDescriptionFull = siegeDescription;
                siegeDescriptionFull += "\r\n\r\nThe ownership of this fief has now passed from ";
                siegeDescriptionFull += s.getDefendingPlayer().firstName + " " + s.getDefendingPlayer().familyName;
                siegeDescriptionFull += " to " + s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescriptionFull += " who has also earned an increase of " + statureIncrease + " stature.";
                siegeDescriptionFull += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescriptionFull += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescriptionFull += besiegedFief.keepLevel + ".";

                // end the siege
                this.siegeEnd(s, siegeDescriptionFull);

                // change fief ownership
                s.getFief().changeOwnership(s.getBesiegingPlayer());

            }

            // negotiation unsuccessful
            else
            {
                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                siegeDescription += " FAILED to negotiate an end to the siege of " + s.getFief().name + ".";
                siegeDescription += "\r\n\r\nTotal casualties during this round numbered " + defenderCasualties + " for the defending forces";
                siegeDescription += ". In addition the keep level was reduced from " + originalKeepLvl + " to ";
                siegeDescription += besiegedFief.keepLevel + ".";
            }

            // create and send JOURNAL ENTRY
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeStorm", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // update total defender siege losses
            s.totalCasualtiesDefender += Convert.ToInt32(defenderCasualties);

            // inform player of success
            System.Windows.Forms.MessageBox.Show(siegeDescription);

            // refresh screen
            this.refreshCurrentScreen();

            return negotiateSuccess;
        }
        
        /// <summary>
        /// Processes a single reduction round of a siege
        /// </summary>
        /// <param name="s">The siege</param>
        /// <param name="type">The type of round - storm, negotiate, reduction (default)</param>
        public void siegeReductionRound(Siege s, string type = "reduction")
        {
            bool siegeRaised = false;
            Fief besiegedFief = s.getFief();
            Army besieger = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = null;
            Character defenderLeader = defenderGarrison.getLeader();
            Character attackerLeader = besieger.getLeader();

            // check for sallying army
            if (s.defenderAdditional != null)
            {
                defenderAdditional = s.getDefenderAdditional();

                if (defenderAdditional.aggression > 1)
                {
                    // get odds
                    int battleOdds = this.getBattleOdds(defenderAdditional, besieger);

                    // if odds OK, give battle
                    if (battleOdds >= defenderAdditional.combatOdds)
                    {
                        // process battle and apply results, if required
                        siegeRaised = this.giveBattle(defenderAdditional, besieger, circumstance: "siege");

                        // check for disbandment of defenderAdditional and remove from siege if necessary
                        if (!siegeRaised)
                        {
                            if (!besiegedFief.armies.Contains(s.defenderAdditional))
                            {
                                defenderAdditional = null;
                            }
                        }

                    }
                }
            }

            if (siegeRaised)
            {
                // NOTE: if sally was success, siege is ended in Form1.giveBattle
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("The defenders have successfully raised the siege!");
                }
            }

            else
            {
                // process results of siege round
                // reduce keep level by 5%
                double originalKeepLvl = besiegedFief.keepLevel;
                besiegedFief.keepLevel = (besiegedFief.keepLevel * 0.92);

                // apply combat losses to defenderGarrison
                // NOTE: attrition for both sides is calculated in siege.syncDays

                double combatLosses = 0.01;
                uint troopsLost = defenderGarrison.applyTroopLosses(combatLosses);

                // check for death of defending PCs/NPCs
                if (defenderLeader != null)
                {
                    bool characterDead = false;

                    // if defenderLeader is PC, check for casualties amongst entourage
                    if (defenderLeader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defenderLeader as PlayerCharacter).myNPCs.Count; i++)
                        {
                            NonPlayerCharacter thisNPC = (defenderLeader as PlayerCharacter).myNPCs[i];
                            characterDead = thisNPC.calculateCombatInjury(combatLosses);

                            if (characterDead)
                            {
                                // process death
                                (defenderLeader as PlayerCharacter).myNPCs[i].processDeath("injury");
                            }
                        }
                    }

                    // check defenderLeader
                    characterDead = defenderLeader.calculateCombatInjury(combatLosses);

                    if (characterDead)
                    {
                        // remove as leader
                        defenderGarrison.leader = null;

                        // process death
                        defenderLeader.processDeath("injury");
                    }
                }

                // update total days (NOTE: siege.days will be updated in syncDays)
                s.totalDays += 10;

                // synchronise days
                s.syncDays(s.days - 10);

                if (type.Equals("reduction"))
                {
                    // UPDATE SIEGE LOSSES
                    s.totalCasualtiesDefender += Convert.ToInt32(troopsLost);

                    // =================== construct and send JOURNAL ENTRY
                    // ID
                    uint entryID = Globals_Game.getNextJournalEntryID();

                    // personae
                    List<string> tempPersonae = new List<string>();
                    tempPersonae.Add(s.getDefendingPlayer().charID + "|fiefOwner");
                    tempPersonae.Add(s.getBesiegingPlayer().charID + "|attackerOwner");
                    if (attackerLeader != null)
                    {
                        tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
                    }
                    if (defenderLeader != null)
                    {
                        tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
                    }
                    // get additional defending leader
                    Character addDefendLeader = null;
                    if (defenderAdditional != null)
                    {
                        addDefendLeader = defenderAdditional.getLeader();
                        if (addDefendLeader != null)
                        {
                            tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                        }
                    }
                    string[] siegePersonae = tempPersonae.ToArray();

                    // location
                    string siegeLocation = s.getFief().id;

                    // use popup text as description
                    string siegeDescription = "On this day of Our Lord the siege of " + s.getFief().name + " by ";
                    siegeDescription += s.getBesiegingPlayer().firstName + " " + s.getBesiegingPlayer().familyName;
                    siegeDescription += " continued.  The besieged garrison lost a total of " + troopsLost + " troops, ";
                    siegeDescription += " and the keep level was reduced from " + originalKeepLvl + " to ";
                    siegeDescription += besiegedFief.keepLevel + ".";

                    // put together new journal entry
                    JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeReduction", loc: siegeLocation, descr: siegeDescription);

                    // add new journal entry to pastEvents
                    Globals_Game.addPastEvent(siegeResult);
                }

                if (type.Equals("storm"))
                {
                    this.siegeStormRound(s, troopsLost, originalKeepLvl);
                }
                else if (type.Equals("negotiation"))
                {
                    this.siegeNegotiationRound(s, troopsLost, originalKeepLvl);
                }
            }

            if (type.Equals("reduction"))
            {
                // refresh screen
                this.refreshCurrentScreen();
            }

        }

        /// <summary>
        /// Checks to see if display of financial data for the specified financial period
        /// is permitted due to ongoing siege
        /// </summary>
        /// <returns>bool indicating whether display is permitted</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        /// <param name="s">The siege</param>
        public bool checkToShowFinancialData(int relativeSeason, Siege s)
        {
            bool displayData = true;

            uint financialPeriodYear = this.getFinancialYear(relativeSeason);
            if (financialPeriodYear > s.startYear)
            {
                displayData = false;
            }
            else if (financialPeriodYear == s.startYear)
            {
                byte financialPeriodSeason = this.getFinancialSeason(relativeSeason);
                if (financialPeriodSeason > s.startSeason)
                {
                    displayData = false;
                }
            }

            return displayData;
        }

        /// <summary>
        /// Gets the year for the specified financial period
        /// </summary>
        /// <returns>The year</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public uint getFinancialYear(int relativeSeason)
        {
            uint financialYear = 0;
            uint thisYear = Globals_Game.clock.currentYear;

            switch (relativeSeason)
            {
                case (-1):
                    if (Globals_Game.clock.currentSeason == 0)
                    {
                        financialYear = thisYear - 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                case (1):
                    if (Globals_Game.clock.currentSeason == 4)
                    {
                        financialYear = thisYear + 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                default:
                    financialYear = thisYear;
                    break;
            }

            return financialYear;
        }

        /// <summary>
        /// Gets the season for the specified financial period
        /// </summary>
        /// <returns>The season</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public byte getFinancialSeason(int relativeSeason)
        {
            byte financialSeason = 0;
            byte thisSeason = Globals_Game.clock.currentSeason;

            switch (relativeSeason)
            {
                case (-1):
                    if (thisSeason == 0)
                    {
                        financialSeason = 4;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason--;
                    }
                    break;
                case (1):
                    if (thisSeason == 4)
                    {
                        financialSeason = 0;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason++;
                    }
                    break;
                default:
                    financialSeason = thisSeason;
                    break;
            }

            return financialSeason;
        }

        /// <summary>
        /// Allows an attacking army to lay siege to an enemy fief
        /// </summary>
        /// <param name="attacker">The attacking army</param>
        /// <param name="target">The fief to be besieged</param>
        public void siegeStart(Army attacker, Fief target)
        {
            Army defenderGarrison = null;
            Army defenderAdditional = null;

            // create defending force
            defenderGarrison = this.createDefendingArmy(target);

            // check for existence of army in keep
            for (int i = 0; i < target.armies.Count; i++)
            {
                // get army
                Army armyInFief = Globals_Game.armyMasterList[target.armies[i]];

                // check is in keep
                if (armyInFief.getOwner().inKeep)
                {
                    // check owner is same as that of fief (i.e. can help in siege)
                    if (armyInFief.owner.Equals(target.owner.charID))
                    {
                        defenderAdditional = armyInFief;
                        break;
                    }
                }
            }

            // get the minumum days of all army objects involved
            double minDays = Math.Min(attacker.days, defenderGarrison.days);
            if (defenderAdditional != null)
            {
                minDays = Math.Min(minDays, defenderAdditional.days);
            }

            // get defenderAdditional ID, or null if no defenderAdditional
            string defAddID = null;
            if (defenderAdditional != null)
            {
                defAddID = defenderAdditional.armyID;
            }
            
            // create siege object
            Siege mySiege = new Siege(Globals_Game.getNextSiegeID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, attacker.getOwner().charID, target.owner.charID, attacker.armyID, defenderGarrison.armyID, target.id, minDays, target.keepLevel, defAdd: defAddID);

            // add to master list
            Globals_Game.siegeMasterList.Add(mySiege.siegeID, mySiege);

            // add to siege owners
            mySiege.getBesiegingPlayer().mySieges.Add(mySiege.siegeID);
            mySiege.getDefendingPlayer().mySieges.Add(mySiege.siegeID);

            // add to fief
            target.siege = mySiege.siegeID;

            // reduce expenditures in fief, except for garrison
            target.infrastructureSpendNext = 0;
            target.keepSpendNext = 0;
            target.officialsSpendNext = 0;

            // update days (NOTE: siege.days will be updated in syncDays)
            mySiege.totalDays++;

            // sychronise days
            mySiege.syncDays(mySiege.days - 1);

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(mySiege.getDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(mySiege.getBesiegingPlayer().charID + "|attackerOwner");
            tempPersonae.Add(attacker.getLeader().charID + "|attackerLeader");
            // get defenderLeader
            Character defenderLeader = defenderGarrison.getLeader();
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.getLeader();
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = mySiege.getFief().id;

            // description
            string siegeDescription = "On this day of Our Lord the forces of ";
            siegeDescription += mySiege.getBesiegingPlayer().firstName + " " + mySiege.getBesiegingPlayer().familyName;
            siegeDescription += ", led by " + attacker.getLeader().firstName + " " + attacker.getLeader().familyName;
            siegeDescription += " laid siege to the keep of " + mySiege.getFief().name;
            siegeDescription += ", owned by " + mySiege.getDefendingPlayer().firstName + " " + mySiege.getDefendingPlayer().familyName;
            if (defenderLeader != null)
            {
                siegeDescription += ". The defending garrison is led by " + defenderLeader.firstName + " " + defenderLeader.familyName;
            }
            if (addDefendLeader != null)
            {
                siegeDescription += ". Additional defending forces are led by " + addDefendLeader.firstName + " " + addDefendLeader.familyName;
            }
            siegeDescription += ".";

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siege", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // display siege in siege screen
            this.refreshSiegeContainer(mySiege);
        }

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
                    if (besiegingArmy.leader != null)
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

        /*
        /// <summary>
        /// Responds to the click event of the siegeNegotiateBtn button
        /// processing a single siege negotaiation round
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeNegotiateBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege);

                if (proceed)
                {
                    // process siege negotiation round
                    this.siegeNegotiationRound(thisSiege);
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
        /// Responds to the click event of the siegeStormBtn button
        /// processing a single siege storm round
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeStormBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get siege
                Siege thisSiege = Globals_Game.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege);

                if (proceed)
                {
                    // process siege storm round
                    this.siegeStormRound(thisSiege);
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
        */

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
                    this.siegeEnd(thisSiege, siegeDescription);

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
        /// Performs standard conditional checks before a pregnancy attempt
        /// </summary>
        /// <returns>bool indicating whether or not to proceed with pregnancy attempt</returns>
        /// <param name="husband">The husband</param>
        public bool checkBeforePregnancyAttempt(Character husband)
        {
            bool proceed = true;

            // check is married
            if (husband.spouse != null)
            {
                // get spouse
                NonPlayerCharacter wife = Globals_Game.npcMasterList[husband.spouse];

                // check to make sure is in same fief
                if (!(wife.location == husband.location))
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You have to be in the same fief to do that!");
                    }
                    proceed = false;
                }

                else
                {
                    // make sure wife not already pregnant
                    if (wife.isPregnant)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!", "PREGNANCY ATTEMPT CANCELLED");
                        }
                        proceed = false;
                    }

                    // check if are kept apart by siege
                    else
                    {
                        if ((husband.location.siege != null) && (husband.inKeep != wife.inKeep))
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("I'm afraid the husband and wife are being separated by the ongoing siege.", "PREGNANCY ATTEMPT CANCELLED");
                            }
                            proceed = false;
                        }

                        else
                        {
                            // ensure player and spouse have at least 1 day remaining
                            double minDays = Math.Min(husband.days, wife.days);

                            if (minDays < 1)
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("Sorry, you don't have enough time left for this in the current season.", "PREGNANCY ATTEMPT CANCELLED");
                                }
                                proceed = false;
                            }
                            else
                            {
                                // ensure days are synchronised
                                if (husband.days != minDays)
                                {
                                    if (husband is PlayerCharacter)
                                    {
                                        (husband as PlayerCharacter).adjustDays(husband.days - minDays);
                                    }
                                    else
                                    {
                                        husband.adjustDays(husband.days - minDays);
                                    }
                                }
                                else
                                {
                                    wife.adjustDays(wife.days - minDays);
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This man is not married.", "PREGNANCY ATTEMPT CANCELLED");
                }
                proceed = false;
            }

            return proceed;
        }

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
                if (this.checkBeforePregnancyAttempt(Globals_Client.charToView))
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
        /// Adds a new JournalEntry to the myPastEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public bool addMyPastEvent(JournalEntry jEntry)
        {
            bool success = false;
            byte priority = 0;

            success = Globals_Client.myPastEvents.addNewEntry(jEntry);

            if (success)
            {
                // check for entry priority
                priority = jEntry.checkEventForPriority();

                // set alert
                this.setJournalAlert(true, priority);
            }

            return success;

        }

        /// <summary>
        /// Sets the myPastEvents Journal's areNewEntries setting to the appropriate value
        /// and sets the journal menu alert to the desired priority
        /// </summary>
        /// <param name="setAlert">The desired bool value</param>
        /// <param name="newPriority">The desired priority</param>
        public void setJournalAlert(bool setAlert, byte newPriority = 0)
        {
            // set Journal menu alert (BackColor) and priority as appropriate
            if (!setAlert)
            {
                // if no new entries, set to default colour
                this.journalToolStripMenuItem.BackColor = Control.DefaultBackColor;
                // set journal priority
                Globals_Client.myPastEvents.priority = 0;
            }
            else
            {
                // only change alert colour if new priority higher than current
                if (newPriority >= Globals_Client.myPastEvents.priority)
                {
                    // set journal priority
                    Globals_Client.myPastEvents.priority = newPriority;

                    // set to appropriate alert colour
                    switch (newPriority)
                    {
                        case 1:
                            this.journalToolStripMenuItem.BackColor = Color.Orange;
                            break;
                        case 2:
                            this.journalToolStripMenuItem.BackColor = Color.Red;
                            break;
                        default:
                            this.journalToolStripMenuItem.BackColor = Color.GreenYellow;
                            break;
                    }
                }
            }

            // set areNewEntries value
            if (Globals_Client.myPastEvents.areNewEntries != setAlert)
            {
                Globals_Client.myPastEvents.areNewEntries = setAlert;
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
            string[] myEventPersonae003 = new string[] { "405|father", "406|mother", "102|familyHead", "101|uncle"};
            JournalEntry myEntry003 = new JournalEntry(Globals_Game.getNextJournalEntryID(), 1320, 0, myEventPersonae003, "birth");
            Globals_Game.addPastEvent(myEntry003);
        }

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
                    if (infoSplit[1] != null)
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

        /// <summary>
        /// Responds to the click event of the viewEntriesToolStripMenuItem
        /// displaying the journal screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get entries for current season by default
            uint thisYear = Globals_Game.clock.currentYear;
            byte thisSeason = Globals_Game.clock.currentSeason;
            Globals_Client.eventSetToView = Globals_Client.myPastEvents.getEventsOnDate(yr: thisYear, seas: thisSeason);

            // get max index position
            Globals_Client.jEntryMax = Globals_Client.eventSetToView.Count - 1;
            // set default index position
            Globals_Client.jEntryToView = -1;

            // display journal screen
            this.refreshJournalContainer();
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
            }

            // retrieve and display character information
            this.journalTextBox.Text = this.displayJournalEntry(Globals_Client.jEntryToView);            

            // check if marriage proposal controls should be enabled
            JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;
            bool enableProposalControls = thisJentry.checkForProposalControlsEnabled();
            this.journalProposalAcceptBtn.Enabled = enableProposalControls;
            this.journalProposalRejectBtn.Enabled = enableProposalControls;
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
        /// Responds to the click event of the journalPrevBtn button
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

            if (this.houseProposeBrideTextBox.Text.Trim() == "")
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                }
            }
            else if (this.houseProposeGroomTextBox.Text.Trim() == "")
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

        /// <summary>
        /// Allows a character to propose marriage between himself, or a male family member,
        /// and a female family member of another player
        /// </summary>
        /// <returns>bool indicating whether proposal was processed successfully</returns>
        /// <param name="bride">The prospective bride</param>
        /// <param name="groom">The prospective groom</param>
        public bool proposeMarriage(Character bride, Character groom)
        {
            bool success = true;

            // get interested parties
            PlayerCharacter headOfFamilyGroom = Globals_Game.pcMasterList[groom.familyID];
            PlayerCharacter headOfFamilyBride = Globals_Game.pcMasterList[bride.familyID];

            // ID
            uint proposalID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string brideEntry = bride.charID + "|bride";
            string groomEntry = groom.charID + "|groom";
            string[] myProposalPersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, brideEntry, groomEntry };


            // description
            string description = "On this day of Our Lord a proposal has been made by ";
            description += headOfFamilyGroom.firstName + " " + headOfFamilyGroom.familyName + " to ";
            description += headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName + " that ";
            if (headOfFamilyGroomEntry.Equals(groomEntry))
            {
                description += "he";
            }
            else
            {
                description += groom.firstName + " " + groom.familyName;
            }
            description += " be betrothed to " + bride.firstName + " " + bride.familyName;

            // create and send a proposal (journal entry)
            JournalEntry myProposal = new JournalEntry(proposalID, year, season, myProposalPersonae, "proposalMade", descr: description);
            success = Globals_Game.addPastEvent(myProposal);

            return success;
        }

        /// <summary>
        /// Allows a character to reply to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether reply was processed successfully</returns>
        /// <param name="jEntry">The proposal</param>
        /// <param name="proposalAccepted">bool indicating whether proposal accepted</param>
        public bool replyToProposal(JournalEntry jEntry, bool proposalAccepted)
        {
            bool success = true;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < jEntry.personae.Length; i++ )
            {
                string thisPersonae = jEntry.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
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

            // ID
            uint replyID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string[] myReplyPersonae = new string[] { headOfFamilyBrideEntry, headOfFamilyGroomEntry, thisBrideEntry, thisGroomEntry };

            // type
            string type = "";
            if (proposalAccepted)
            {
                type = "proposalAccepted";
            }
            else
            {
                type = "proposalRejected";
            }

            // description
            string description = "On this day of Our Lord the proposed marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + bride.familyName + " has been ";
            if (proposalAccepted)
            {
                description += "ACCEPTED";
            }
            else
            {
                description += "REJECTED";
            }
            description += " by " + headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName + ".";
            if (proposalAccepted)
            {
                description += " Let the bells ring out in celebration!";
            }

            // create and send a proposal reply (journal entry)
            JournalEntry myProposalReply = new JournalEntry(replyID, year, season, myReplyPersonae, type, descr: description);
            success = Globals_Game.addPastEvent(myProposalReply);

            if (success)
            {
                // mark proposal as replied
                jEntry.description += "\r\n\r\n** You ";
                if (proposalAccepted)
                {
                    jEntry.description += "ACCEPTED ";
                }
                else
                {
                    jEntry.description += "REJECTED ";
                }
                jEntry.description += "this proposal in " + Globals_Game.clock.seasons[season] + ", " + year;

                // if accepted, process engagement
                if (proposalAccepted)
                {
                    this.processEngagement(myProposalReply);
                }
            }

            // refresh screen
            this.refreshCurrentScreen();

            return success;
        }

        /// <summary>
        /// Processes the actions involved with an engagement
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        /// <param name="jEntry">The marriage proposal acceptance</param>
        public bool processEngagement(JournalEntry jEntry)
        {
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
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
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

            // ID
            uint replyID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;
            if (season == 3)
            {
                season = 0;
                year++;
            }
            else
            {
                season++;
            }

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry };

            // type
            string type = "marriage";

            // create and add a marriage entry to the scheduledEvents journal
            JournalEntry marriageEntry = new JournalEntry(replyID, year, season, marriagePersonae, type);
            success = Globals_Game.addScheduledEvent(marriageEntry);

            // show bride and groom as engaged
            if (success)
            {
                bride.fiancee = groom.charID;
                groom.fiancee = bride.charID;
            }

            return success;
        }

        /// <summary>
        /// Processes the actions involved with a marriage
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        /// <param name="jEntry">The marriage proposal acceptance</param>
        public bool processMarriage(JournalEntry jEntry)
        {
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
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
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

            // ID
            uint marriageID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry };

            // type
            string type = "marriage";

            // description
            string description = "On this day of Our Lord there took place a marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + groom.familyName + " (nee " + bride.familyName + ").";
            description += " Let the bells ring out in celebration!";

            // create and add a marriage entry to the pastEvents journal
            JournalEntry marriageEntry = new JournalEntry(marriageID, year, season, marriagePersonae, type, descr: description);
            success = Globals_Game.addPastEvent(marriageEntry);

            if (success)
            {
                // remove fiancees
                bride.fiancee = null;
                groom.fiancee = null;

                // add spouses
                bride.spouse = groom.charID;
                groom.spouse = bride.charID;

                // change wife's family
                bride.familyID = groom.familyID;
                bride.familyName = groom.familyName;

                // switch myNPCs
                headOfFamilyBride.myNPCs.Remove(bride as NonPlayerCharacter);
                headOfFamilyGroom.myNPCs.Add(bride as NonPlayerCharacter);

                // move wife to groom's location
                bride.location = groom.location;

                // check to see if headOfFamilyBride should receive increase in stature
                // get highest rank for headOfFamilyBride and headOfFamilyGroom
                Rank brideHighestRank = headOfFamilyBride.getHighestRank();
                Rank groomHighestRank = headOfFamilyGroom.getHighestRank();

                // compare ranks
                if (groomHighestRank.id < brideHighestRank.id)
                {
                    headOfFamilyBride.statureModifier += ((brideHighestRank.id - groomHighestRank.id) * 0.4);
                }
            }

            return success;
        }

        /// <summary>
        /// Implements conditional checks prior to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether proposal can proceed</returns>
        /// <param name="bride">The prospective bride</param>
        /// <param name="groom">The prospective groom</param>
        public bool checksBeforeProposal(Character bride, Character groom)
        {
            bool proceed = true;
            string message = "";

            // ============= BRIDE
            // check is female
            if (bride.isMale)
            {
                message = "You cannot propose to a man!";
                proceed = false;
            }

            // check is of age
            else
            {
                if (bride.calcCharAge() < 14)
                {
                    message = "The prospective bride has yet to come of age.";
                    proceed = false;
                }

                else
                {
                    // check isn't engaged
                    if (bride.fiancee != null)
                    {
                        message = "The prospective bride is already engaged.";
                        proceed = false;
                    }

                    else
                    {
                        // check isn't married
                        if (bride.spouse != null)
                        {
                            message = "The prospective bride is already married.";
                            proceed = false;
                        }
                        else
                        {
                            // check is family member of player
                            if (bride.familyID == null)
                            {
                                message = "The prospective bride is not of a suitable family.";
                                proceed = false;
                            }
                            else
                            {
                                // ============= GROOM
                                // check is male
                                if (!groom.isMale)
                                {
                                    message = "The proposer must be a man.";
                                    proceed = false;
                                }
                                else
                                {
                                    // check is of age
                                    if (groom.calcCharAge() < 14)
                                    {
                                        message = "The prospective groom has yet to come of age.";
                                        proceed = false;
                                    }
                                    else
                                    {
                                        // check is unmarried
                                        if (groom.spouse != null)
                                        {
                                            message = "The prospective groom is already married.";
                                            proceed = false;
                                        }
                                        else
                                        {
                                            // check isn't engaged
                                            if (groom.fiancee != null)
                                            {
                                                message = "The prospective groom is already engaged.";
                                                proceed = false;
                                            }
                                            else
                                            {
                                                // check is family member of player OR is player themself
                                                if (groom.familyID == null)
                                                {
                                                    message = "The prospective groom is not of a suitable family.";
                                                    proceed = false;
                                                }
                                                else
                                                {
                                                    // check isn't in family same family as bride
                                                    if (groom.familyID == bride.familyID)
                                                    {
                                                        message = "The prospective bride and groom are in the same family!";
                                                        proceed = false;
                                                    }
                                                }

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                }
            }

            if (!proceed)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(message);
                }
            }

            return proceed;
        }

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

                if (this.meetingPlaceProposeTextBox.Text.Trim() == "")
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
        /// Responds to the click event of the royalGiftsToolStripMenuItem
        /// which displays royal gifts screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display royal gifts screen
            this.refreshRoyalGiftsContainer();
            // display household affairs screen
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
                            if (thisPos.officeHolder != null)
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

            if (giftType != null)
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
                    PlayerCharacter currentHolder = null;
                    if (Globals_Game.pcMasterList.ContainsKey(thisPos.officeHolder))
                    {
                        currentHolder = Globals_Game.pcMasterList[thisPos.officeHolder];
                    }

                    // remove from position
                    if (currentHolder != null)
                    {
                        thisPos.removePosition(currentHolder);

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
        /// Checks to see if an attempts to quell a rebellion has been successful
        /// </summary>
        /// <returns>bool indicating quell success or failure</returns>
        /// <param name="a">The army trying to quell the rebellion</param>
        /// <param name="f">The rebelling fief</param>
        public bool quell_checkSuccess(Army a, Fief f)
        {
            bool rebellionQuelled = false;

            // calculate base chance of success, based on army size and fief population
            double successChance = a.calcArmySize() / (f.population / Convert.ToDouble(1000));

            // get army leader
            Character aLeader = null;
            if (a.getLeader() != null)
            {
                aLeader = a.getLeader();
            }

            if (aLeader != null)
            {
                // apply any bonus for leadership skills
                successChance += aLeader.getLeadershipValue();

                // apply any bonus for ancestral ownership
                if ((f.owner != f.ancestralOwner) && (aLeader == f.ancestralOwner))
                {
                    successChance += (aLeader.calculateStature() * 2.22);
                }
            }

            // ensure successChance always between 1 > 99 (to allow for minimum chance of success/failure)
            if (successChance < 1)
            {
                successChance = 1;
            }
            else if (successChance > 99)
            {
                successChance = 99;
            }

            // generate random double 0-100 to check for success
            rebellionQuelled = (Globals_Game.GetRandomDouble(101) <= successChance);

            return rebellionQuelled;
        }

        /// <summary>
        /// Attempts to quell the rebellion in the specified fief using the specified army
        /// </summary>
        /// <param name="a">The army trying to quell the rebellion</param>
        /// <param name="f">The rebelling fief</param>
        public void quellRebellion(Army a, Fief f)
        {
            // check to see if quell attempt was successful
            bool quellSuccessful = this.quell_checkSuccess(a, f);

            // if quell successful
            if (quellSuccessful)
            {
                // pillage the fief
                this.processPillage(f, a, "quellRebellion");

                // process change of ownership, if appropriate
                if (f.owner != a.getOwner())
                {
                    f.changeOwnership(a.getOwner());
                }

                // set status
                f.status = 'C';
            }

            // if quell not successful
            else
            {
                // retreat army 1 hex
                this.processRetreat(a, 1);

                // CREATE JOURNAL ENTRY
                // get interested parties
                bool success = true;
                PlayerCharacter fiefOwner = f.owner;
                PlayerCharacter attackerOwner = a.getOwner();
                Character attackerLeader = null;
                if (a.getLeader() != null)
                {
                    attackerLeader = a.getLeader();
                }

                // ID
                uint entryID = Globals_Game.getNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                List<string> tempPersonae = new List<string>();
                tempPersonae.Add(fiefOwner.charID + "|fiefOwner");
                tempPersonae.Add(attackerOwner.charID + "|attackerOwner");
                if (attackerLeader != null)
                {
                    tempPersonae.Add(attackerLeader.charID + "|attackerLeader");
                }
                string[] quellPersonae = tempPersonae.ToArray();

                // type
                string type = "rebellionQuellFailed";

                // location
                string location = f.id;

                // description
                string description = "On this day of Our Lord the attempt by the forces of ";
                description += attackerOwner.firstName + " " + attackerOwner.familyName;
                if (attackerLeader != null)
                {
                    description += ", led by " + attackerLeader.firstName + " " + attackerLeader.familyName + ",";
                }
                description += " FAILED in their attempt to quell the rebellion in the fief of " + f.name;
                description += ", owned by " + fiefOwner.firstName + " " + fiefOwner.familyName + ".";
                description += "\r\n\r\nThe army was forced to retreat into an adjoining fief.";

                // create and add a journal entry to the pastEvents journal
                JournalEntry quellEntry = new JournalEntry(entryID, year, season, quellPersonae, type, loc: location, descr: description);
                success = Globals_Game.addPastEvent(quellEntry);
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
                            this.quellRebellion(thisArmy, thisFief);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Creates game objects using data imported from a CSV file and writes them to the database
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the Riak bucket in which to store the game objects</param>
		/// <param name="resynch">bool indicating whether or not to resynch the game objects</param>
		public bool ImportFromCSV(string filename, string bucketID, bool resynch = false)
        {
            bool inputFileError = false;
            string lineIn;
            string[] lineParts;
            StreamReader srObjects = null;

			// list for storing object keys
            List<string> fiefKeyList = new List<string>();
            List<string> provKeyList = new List<string>();
            List<string> kingKeyList = new List<string>();
            List<string> pcKeyList = new List<string>();
            List<string> npcKeyList = new List<string>();
            List<string> skillKeyList = new List<string>();
            List<string> armyKeyList = new List<string>();
            List<string> langKeyList = new List<string>();
            List<string> baseLangKeyList = new List<string>();
            List<string> natKeyList = new List<string>();
            List<byte> rankKeyList = new List<byte>();
            List<byte> posKeyList = new List<byte>();
            List<string> siegeKeyList = new List<string>();
            List<string> terrKeyList = new List<string>();

			// dictionaries for storing objects (if resynching)
			Dictionary<string, Fief_Riak> fiefMasterList = new Dictionary<string, Fief_Riak>();
			Dictionary<string, PlayerCharacter_Riak> pcMasterList = new Dictionary<string, PlayerCharacter_Riak>();
			Dictionary<string, NonPlayerCharacter_Riak> npcMasterList = new Dictionary<string, NonPlayerCharacter_Riak>();
			Dictionary<string, Province_Riak> provinceMasterList = new Dictionary<string, Province_Riak>();
			Dictionary<string, Kingdom_Riak> kingdomMasterList = new Dictionary<string, Kingdom_Riak>();
			Dictionary<string, Siege> siegeMasterList = new Dictionary<string, Siege>();
			Dictionary<string, Army> armyMasterList = new Dictionary<string, Army>();

            try
            {
                // opens StreamReader to read in  data from csv file
                srObjects = new StreamReader(filename);
            }
            // catch following IO exceptions that could be thrown by the StreamReader 
            catch (FileNotFoundException fnfe)
            {
                inputFileError = true;
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fnfe.Message);
                }
            }
            catch (IOException ioe)
            {
                inputFileError = true;
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ioe.Message);
                }
            }

            // while there is data in the line
            while ((lineIn = srObjects.ReadLine()) != null)
            {
                // put the contents of the line into lineParts array, splitting on (char)9 (TAB)
                lineParts = lineIn.Split(',');

                if (lineParts[0].Equals("fief"))
                {
                    Fief_Riak thisFiefRiak = null;

                    thisFiefRiak = this.importFromCSV_Fief(lineParts);

                    if (thisFiefRiak != null)
                    {
						// check for resynching
						if (resynch)
						{
							// add to masterList
							fiefMasterList.Add (thisFiefRiak.id, thisFiefRiak);
						}

						else
						{
							// save to Riak
							this.writeFief(bucketID, fr: thisFiefRiak);
						}

                        // add fief id to keylist
                        fiefKeyList.Add(thisFiefRiak.id);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create Fief object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("province"))
                {
                    Province_Riak thisProvRiak = null;

                    if (lineParts.Length != 8)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Province object.");
                        }
                    }
                    else
                    {
                        thisProvRiak = this.importFromCSV_Prov(lineParts);

                        if (thisProvRiak != null)
                        {
							// check for resynching
							if (resynch)
							{
								// add to masterList
								provinceMasterList.Add (thisProvRiak.id, thisProvRiak);
							}

							else
							{
								// save to Riak
								this.writeProvince(bucketID, pr: thisProvRiak);
							}

                            // add province id to keylist
                            provKeyList.Add(thisProvRiak.id);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Province object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("kingdom"))
                {
                    Kingdom_Riak thisKingRiak = null;

                    if (lineParts.Length != 7)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Kingdom object.");
                        }
                    }
                    else
                    {
                        thisKingRiak = this.importFromCSV_Kingdom(lineParts);

                        if (thisKingRiak != null)
                        {
							// check for resynching
							if (resynch)
							{
								// add to masterList
								kingdomMasterList.Add (thisKingRiak.id, thisKingRiak);
							}

							else
							{
								// save to Riak
								this.writeKingdom(bucketID, kr: thisKingRiak);
							}

                            // add kingdom id to keylist
                            kingKeyList.Add(thisKingRiak.id);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Kingdom object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("pc"))
                {
                    PlayerCharacter_Riak thisPcRiak = null;

                    thisPcRiak = this.importFromCSV_PC(lineParts);

                    if (thisPcRiak != null)
                    {
						// check for resynching
						if (resynch)
						{
							// add to masterList
							pcMasterList.Add (thisPcRiak.charID, thisPcRiak);
						}

						else
						{
							// save to Riak
							this.writePC(bucketID, pcr: thisPcRiak);
						}

                        // add id to keylist
                        pcKeyList.Add(thisPcRiak.charID);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create PlayerCharacter object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("npc"))
                {
                    NonPlayerCharacter_Riak thisNpcRiak = null;

                    thisNpcRiak = this.importFromCSV_NPC(lineParts);

                    if (thisNpcRiak != null)
                    {
						// check for resynching
						if (resynch)
						{
							// add to masterList
							npcMasterList.Add (thisNpcRiak.charID, thisNpcRiak);
						}

						else
						{
							// save to Riak
							this.writeNPC(bucketID, npcr: thisNpcRiak);
						}

                        // add id to keylist
                        npcKeyList.Add(thisNpcRiak.charID);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create NonPlayerCharacter object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("skill"))
                {
                    Skill thisSkill = null;

                    thisSkill = this.importFromCSV_Skill(lineParts);

                    if (thisSkill != null)
                    {
                        // save to Riak
                        this.writeSkill(bucketID, thisSkill);

                        // add id to keylist
                        skillKeyList.Add(thisSkill.skillID);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create Skill object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("army"))
                {
                    Army thisArmy = null;

                    if (lineParts.Length != 15)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Army object.");
                        }
                    }
                    else
                    {
                        thisArmy = this.importFromCSV_Army(lineParts);

                        if (thisArmy != null)
                        {
							// check for resynching
							if (resynch)
							{
								// add to masterList
								armyMasterList.Add (thisArmy.armyID, thisArmy);
							}

							else
							{
								// save to Riak
								this.writeArmy(bucketID, thisArmy);
							}

                            // add id to keylist
                            armyKeyList.Add(thisArmy.armyID);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Army object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("language"))
                {
                    Language_Riak thisLangRiak = null;

                    if (lineParts.Length != 4)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Language object.");
                        }
                    }
                    else
                    {
                        thisLangRiak = this.importFromCSV_Language(lineParts);

                        if (thisLangRiak != null)
                        {
                            // save to Riak
                            this.writeLanguage(bucketID, lr: thisLangRiak);

                            // add id to keylist
                            langKeyList.Add(thisLangRiak.id);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Language object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("baseLanguage"))
                {
                    BaseLanguage thisBaseLang = null;

                    if (lineParts.Length != 3)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for BaseLanguage object.");
                        }
                    }
                    else
                    {
                        thisBaseLang = this.importFromCSV_BaseLanguage(lineParts);

                        if (thisBaseLang != null)
                        {
                            // save to Riak
                            this.writeBaseLanguage(bucketID, thisBaseLang);

                            // add id to keylist
                            baseLangKeyList.Add(thisBaseLang.id);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create BaseLanguage object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("nationality"))
                {
                    Nationality thisNat = null;

                    if (lineParts.Length != 3)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Nationality object.");
                        }
                    }
                    else
                    {
                        thisNat = this.importFromCSV_Nationality(lineParts);

                        if (thisNat != null)
                        {
                            // save to Riak
                            this.writeNationality(bucketID, thisNat);

                            // add id to keylist
                            natKeyList.Add(thisNat.natID);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Nationality object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("rank"))
                {
                    Rank thisRank = null;

                    thisRank = this.importFromCSV_Rank(lineParts);

                    if (thisRank != null)
                    {
                        // save to Riak
                        this.writeRank(bucketID, thisRank);

                        // add id to keylist
                        rankKeyList.Add(thisRank.id);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create Rank object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("position"))
                {
                    Position_Riak thisPosRiak = null;

                    thisPosRiak = this.importFromCSV_Position(lineParts);

                    if (thisPosRiak != null)
                    {
                        // save to Riak
                        this.writePosition(bucketID, pr: thisPosRiak);

                        // add id to keylist
                        posKeyList.Add(thisPosRiak.id);
                    }
                    else
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Unable to create Position object: " + lineParts[1]);
                        }
                    }
                }

                else if (lineParts[0].Equals("siege"))
                {
                    Siege thisSiege = null;

                    if (lineParts.Length != 16)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Siege object.");
                        }
                    }
                    else
                    {
                        thisSiege = this.importFromCSV_Siege(lineParts);

                        if (thisSiege != null)
                        {
							// check for resynching
							if (resynch)
							{
								// add to masterList
								siegeMasterList.Add (thisSiege.siegeID, thisSiege);
							}

							else
							{
								// save to Riak
								this.writeSiege(bucketID, thisSiege);
							}

                            // add id to keylist
                            siegeKeyList.Add(thisSiege.siegeID);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Siege object: " + lineParts[1]);
                            }
                        }
                    }
                }

                else if (lineParts[0].Equals("terrain"))
                {
                    Terrain thisTerr = null;

                    if (lineParts.Length != 4)
                    {
                        inputFileError = true;
                        if (Globals_Client.showDebugMessages)
                        {
                            MessageBox.Show("Incorrect number of data parts for Terrain object.");
                        }
                    }
                    else
                    {
                        thisTerr = this.importFromCSV_Terrain(lineParts);

                        if (thisTerr != null)
                        {
                            // save to Riak
                            this.writeTerrain(bucketID, thisTerr);

                            // add id to keylist
                            terrKeyList.Add(thisTerr.terrainCode);
                        }
                        else
                        {
                            inputFileError = true;
                            if (Globals_Client.showDebugMessages)
                            {
                                MessageBox.Show("Unable to create Terrain object: " + lineParts[1]);
                            }
                        }
                    }
                }
            }

			// perform resynch if appropriate
			if(resynch)
			{
				// pass objects for resynching/writing
				this.SynchGameObjectCollections (fiefMasterList, pcMasterList, npcMasterList, provinceMasterList,
					kingdomMasterList, siegeMasterList, armyMasterList, bucketID);
			}

            // save keyLists to database
            // fiefs
            if (fiefKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "fiefKeys", fiefKeyList);
            }

            // provinces
            if (provKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "provKeys", provKeyList);
            }

            // kingdoms
            if (kingKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "kingKeys", kingKeyList);
            }

            // PCs
            if (pcKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "pcKeys", pcKeyList);
            }

            // NPCs
            if (npcKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "npcKeys", npcKeyList);
            }

            // skills
            if (skillKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "skillKeys", skillKeyList);
            }

            // armies
            if (armyKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "armyKeys", armyKeyList);
            }

            // languages
            if (langKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "langKeys", langKeyList);
            }

            // baseLanguages
            if (baseLangKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "baseLangKeys", baseLangKeyList);
            }

            // nationalities
            if (natKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "nationalityKeys", natKeyList);
            }

            // ranks
            if (rankKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "rankKeys", rankKeyList);
            }

            // positions
            if (posKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "positionKeys", posKeyList);
            }

            // sieges
            if (siegeKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "siegeKeys", siegeKeyList);
            }

            // terrains
            if (terrKeyList.Count > 0)
            {
                // TODO: save keylist to Riak
                this.writeKeyList(bucketID, "terrKeys", terrKeyList);
            }

            return inputFileError;
        }

        /// <summary>
        /// Creates a Fief_Riak object using data in a string array
        /// </summary>
        /// <returns>Fief_Riak object</returns>
        /// <param name="fiefData">string[] holding source data</param>
        public Fief_Riak importFromCSV_Fief(string[] fiefData)
        {
            Fief_Riak thisFiefRiak = null;

            try
            {
                // create empty lists for variable length collections
                // (characters, barredChars, armies)
                List<string> characters = new List<string>();
                List<string> barredChars = new List<string>();
                List<string> armies = new List<string>();

                // check to see if any data present for variable length collections
                if (fiefData.Length > 59)
                {
                    // create variables to hold start/end index positions
                    int chStart, chEnd, barChStart, barChEnd, arStart, arEnd;
                    chStart = chEnd = barChStart = barChEnd = arStart = arEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
					for (int i = 59; i < fiefData.Length; i++)
                    {
                        if (fiefData[i].Equals("chStart"))
                        {
                            chStart = i;
                        }
                        else if (fiefData[i].Equals("chEnd"))
                        {
                            chEnd = i;
                        }
                        else if (fiefData[i].Equals("barChStart"))
                        {
                            barChStart = i;
                        }
                        else if (fiefData[i].Equals("barChEnd"))
                        {
                            barChEnd = i;
                        }
                        else if (fiefData[i].Equals("arStart"))
                        {
                            arStart = i;
                        }
                        else if (fiefData[i].Equals("arEnd"))
                        {
                            arEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // characters
                    if ((chStart > -1) && (chEnd > -1))
                    {
                        for (int i = chStart + 1; i < chEnd; i++)
                        {
                            characters.Add(fiefData[i]);
                        }
                    }

                    // barredChars
                    if ((barChStart > -1) && (barChEnd > -1))
                    {
                        for (int i = barChStart + 1; i < barChEnd; i++)
                        {
                            barredChars.Add(fiefData[i]);
                        }
                    }

                    // armies
                    if ((arStart > -1) && (arEnd > -1))
                    {
                        for (int i = arStart + 1; i < arEnd; i++)
                        {
                            armies.Add(fiefData[i]);
                        }
                    }
                }

                // create financial data arrays
                // current
                double[] finCurr = new double[] { Convert.ToDouble(fiefData[14]), Convert.ToDouble(fiefData[15]),
                    Convert.ToDouble(fiefData[16]), Convert.ToDouble(fiefData[17]), Convert.ToDouble(fiefData[18]),
                    Convert.ToDouble(fiefData[19]), Convert.ToDouble(fiefData[20]), Convert.ToDouble(fiefData[21]),
               Convert.ToDouble(fiefData[22]), Convert.ToDouble(fiefData[23]), Convert.ToDouble(fiefData[24]),
                Convert.ToDouble(fiefData[25]), Convert.ToDouble(fiefData[26]), Convert.ToDouble(fiefData[27]) };

                // previous
                double[] finPrev = new double[] { Convert.ToDouble(fiefData[28]), Convert.ToDouble(fiefData[29]),
                    Convert.ToDouble(fiefData[30]), Convert.ToDouble(fiefData[31]), Convert.ToDouble(fiefData[32]),
                    Convert.ToDouble(fiefData[33]), Convert.ToDouble(fiefData[34]), Convert.ToDouble(fiefData[35]),
               Convert.ToDouble(fiefData[36]), Convert.ToDouble(fiefData[37]), Convert.ToDouble(fiefData[38]),
                Convert.ToDouble(fiefData[39]), Convert.ToDouble(fiefData[40]), Convert.ToDouble(fiefData[41]) };

                // check for presence of conditional values
                string tiHo, own, ancOwn, bail, sge;
                tiHo = own = ancOwn = bail = sge = null;

                if (!String.IsNullOrWhiteSpace(fiefData[54]))
                {
                    tiHo = fiefData[54];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[55]))
                {
                    own = fiefData[55];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[56]))
                {
                    ancOwn = fiefData[56];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[57]))
                {
                    bail = fiefData[57];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[58]))
                {
                    sge = fiefData[58];
                }

                // create Fife_Riak object
                thisFiefRiak = new Fief_Riak(fiefData[1], fiefData[2], fiefData[3], Convert.ToInt32(fiefData[4]),
                    Convert.ToDouble(fiefData[5]), Convert.ToDouble(fiefData[6]), Convert.ToUInt32(fiefData[7]),
                    Convert.ToDouble(fiefData[8]), Convert.ToDouble(fiefData[9]), Convert.ToUInt32(fiefData[10]),
                    Convert.ToUInt32(fiefData[11]), Convert.ToUInt32(fiefData[12]), Convert.ToUInt32(fiefData[13]),
                    finCurr, finPrev, Convert.ToDouble(fiefData[42]), Convert.ToDouble(fiefData[43]),
                    Convert.ToChar(fiefData[44]), fiefData[45], fiefData[46], characters, barredChars,
                    Convert.ToBoolean(fiefData[47]), Convert.ToBoolean(fiefData[48]), Convert.ToByte(fiefData[49]),
                    Convert.ToInt32(fiefData[50]), armies, Convert.ToBoolean(fiefData[51]),
                    new Dictionary<string, string[]>(), Convert.ToBoolean(fiefData[52]), Convert.ToByte(fiefData[53]),
                    tiHo: tiHo, own: own, ancOwn: ancOwn, bail: bail, sge: sge);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisFiefRiak;
        }

        /// <summary>
        /// Creates a Province_Riak object using data in a string array
        /// </summary>
        /// <returns>Province_Riak object</returns>
        /// <param name="provData">string[] holding source data</param>
        public Province_Riak importFromCSV_Prov(string[] provData)
        {
            Province_Riak thisProvRiak = null;

            try
            {
                // check for presence of conditional values
                string tiHo, own, kingdom;
                tiHo = own = kingdom = null;

                if (!String.IsNullOrWhiteSpace(provData[5]))
                {
                    tiHo = provData[5];
                }
                if (!String.IsNullOrWhiteSpace(provData[6]))
                {
                    own = provData[6];
                }
                if (!String.IsNullOrWhiteSpace(provData[7]))
                {
                    kingdom = provData[7];
                }

                // create Province_Riak object
                thisProvRiak = new Province_Riak(provData[1], provData[2], Convert.ToByte(provData[3]),
                    Convert.ToDouble(provData[4]), tiHo, own, kingdom);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisProvRiak;
        }

        /// <summary>
        /// Creates a Kingdom_Riak object using data in a string array
        /// </summary>
        /// <returns>Kingdom_Riak object</returns>
        /// <param name="kingData">string[] holding source data</param>
        public Kingdom_Riak importFromCSV_Kingdom(string[] kingData)
        {
            Kingdom_Riak thisKingRiak = null;

            try
            {
                // check for presence of conditional values
                string tiHo, own;
                tiHo = own = null;

                if (!String.IsNullOrWhiteSpace(kingData[5]))
                {
                    tiHo = kingData[5];
                }
                if (!String.IsNullOrWhiteSpace(kingData[6]))
                {
                    own = kingData[6];
                }

                // create Kingdom_Riak object
                thisKingRiak = new Kingdom_Riak(kingData[1], kingData[2], Convert.ToByte(kingData[3]), kingData[4],
                    tiHo, own);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisKingRiak;
        }

        /// <summary>
        /// Creates a PlayerCharacter_Riak object using data in a string array
        /// </summary>
        /// <returns>PlayerCharacter_Riak object</returns>
        /// <param name="pcData">string[] holding source data</param>
        public PlayerCharacter_Riak importFromCSV_PC(string[] pcData)
        {
            PlayerCharacter_Riak thisPcRiak = null;

            try
            {
                // create empty lists for variable length collections
                // (skills, myTitles, myNPCs, myOwnedFiefs, myOwnedProvinces, myArmies, mySieges)
                Tuple<string, int>[] skills = null;
                List<string> myTitles = new List<string>();
                List<string> myNPCs = new List<string>();
                List<string> myOwnedFiefs = new List<string>();
                List<string> myOwnedProvinces = new List<string>();
                List<string> myArmies = new List<string>();
                List<string> mySieges = new List<string>();

                // check to see if any data present for variable length collections
                if (pcData.Length > 30)
                {
                    // create variables to hold start/end index positions
                    int skStart, skEnd, tiStart, tiEnd, npcStart, npcEnd, fiStart, fiEnd, prStart, prEnd, arStart, arEnd,
                        siStart, siEnd;
                    skStart = skEnd = tiStart = tiEnd = npcStart = npcEnd = fiStart = fiEnd = prStart = prEnd = arStart
                        = arEnd = siStart = siEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 30; i < pcData.Length; i++)
                    {
                        if (pcData[i].Equals("skStart"))
                        {
                            skStart = i;
                        }
                        else if (pcData[i].Equals("skEnd"))
                        {
                            skEnd = i;
                        }
                        else if (pcData[i].Equals("tiStart"))
                        {
                            tiStart = i;
                        }
                        else if (pcData[i].Equals("tiEnd"))
                        {
                            tiEnd = i;
                        }
                        else if (pcData[i].Equals("npcStart"))
                        {
                            npcStart = i;
                        }
                        else if (pcData[i].Equals("npcEnd"))
                        {
                            npcEnd = i;
                        }
                        else if (pcData[i].Equals("fiStart"))
                        {
                            fiStart = i;
                        }
                        else if (pcData[i].Equals("fiEnd"))
                        {
                            fiEnd = i;
                        }
                        else if (pcData[i].Equals("prStart"))
                        {
                            prStart = i;
                        }
                        else if (pcData[i].Equals("prEnd"))
                        {
                            prEnd = i;
                        }
                        else if (pcData[i].Equals("arStart"))
                        {
                            arStart = i;
                        }
                        else if (pcData[i].Equals("arEnd"))
                        {
                            arEnd = i;
                        }
                        else if (pcData[i].Equals("siStart"))
                        {
                            siStart = i;
                        }
                        else if (pcData[i].Equals("siEnd"))
                        {
                            siEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // skills
                    List<Tuple<string, int>> tempSkills = new List<Tuple<string, int>>();

                    if ((skStart > -1) && (skEnd > -1))
                    {
                        // check to ensure all skills have accompanying skill level
                        if (Globals_Game.IsOdd(skStart + skEnd))
                        {
                            for (int i = skStart + 1; i < skEnd; i = i + 2)
                            {
                                Tuple<string, int> thisSkill = new Tuple<string, int>(pcData[i], Convert.ToInt32(pcData[i+1]));
                                tempSkills.Add(thisSkill);
                            }
                            // convert skills list to skills array
                            skills = tempSkills.ToArray();
                        }
                    }

                    // myTitles
                    if ((tiStart > -1) && (tiEnd > -1))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i++)
                        {
                            myTitles.Add(pcData[i]);
                        }
                    }

                    // myNPCs
                    if ((npcStart > -1) && (npcEnd > -1))
                    {
                        for (int i = npcStart + 1; i < npcEnd; i++)
                        {
                            myNPCs.Add(pcData[i]);
                        }
                    }

                    // myOwnedFiefs
                    if ((fiStart > -1) && (fiEnd > -1))
                    {
                        for (int i = fiStart + 1; i < fiEnd; i++)
                        {
                            myOwnedFiefs.Add(pcData[i]);
                        }
                    }

                    // myOwnedProvinces
                    if ((prStart > -1) && (prEnd > -1))
                    {
                        for (int i = prStart + 1; i < prEnd; i++)
                        {
                            myOwnedProvinces.Add(pcData[i]);
                        }
                    }

                    // myArmies
                    if ((arStart > -1) && (arEnd > -1))
                    {
                        for (int i = arStart + 1; i < arEnd; i++)
                        {
                            myArmies.Add(pcData[i]);
                        }
                    }

                    // mySieges
                    if ((siStart > -1) && (siEnd > -1))
                    {
                        for (int i = siStart + 1; i < siEnd; i++)
                        {
                            mySieges.Add(pcData[i]);
                        }
                    }
                }

                // if no skills, generate random set
                if (skills == null)
                {
                    Tuple<Skill, int>[] generatedSkills = this.generateSkillSet();

                    // convert to format for saving to database
                    skills = new Tuple<String, int>[generatedSkills.Length];
                    for (int i = 0; i < generatedSkills.Length; i++)
                    {
                        skills[i] = new Tuple<string, int>(generatedSkills[i].Item1.skillID, generatedSkills[i].Item2);
                    }
                }

                // create DOB tuple
                if (String.IsNullOrWhiteSpace(pcData[5]))
                {
                    pcData[5] = Globals_Game.myRand.Next(4).ToString();
                }
                Tuple<uint, byte> dob = new Tuple<uint,byte>(Convert.ToUInt32(pcData[4]), Convert.ToByte(pcData[5]));

                // check for presence of CONDITIONAL VARIABLES
                string loc, aID, pID;
                loc = aID = pID = null;

                if (!String.IsNullOrWhiteSpace(pcData[27]))
                {
                    loc = pcData[27];
                }
                if (!String.IsNullOrWhiteSpace(pcData[28]))
                {
                    aID = pcData[28];
                }
                if (!String.IsNullOrWhiteSpace(pcData[29]))
                {
                    pID = pcData[29];
                }

                // create PlayerCharacter_Riak object
                thisPcRiak = new PlayerCharacter_Riak(pcData[1], pcData[2], pcData[3], dob, Convert.ToBoolean(pcData[6]),
                    pcData[7], Convert.ToBoolean(pcData[8]), Convert.ToDouble(pcData[9]), Convert.ToDouble(pcData[10]),
                    new List<string>(), pcData[11], Convert.ToDouble(pcData[12]), Convert.ToDouble(pcData[13]),
                    Convert.ToDouble(pcData[14]), Convert.ToDouble(pcData[15]), skills, Convert.ToBoolean(pcData[16]),
                    Convert.ToBoolean(pcData[17]), pcData[18], pcData[19], pcData[20], pcData[21], myTitles,
                    pcData[22], Convert.ToBoolean(pcData[23]), Convert.ToUInt32(pcData[24]), myNPCs, myOwnedFiefs,
                    myOwnedProvinces, pcData[25], pcData[26], myArmies, mySieges, ails: new Dictionary<string, Ailment>(),
                    loc: loc, aID: aID, pID: pID);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisPcRiak;
        }

        /// <summary>
        /// Creates a NonPlayerCharacter_Riak object using data in a string array
        /// </summary>
        /// <returns>NonPlayerCharacter_Riak object</returns>
        /// <param name="npcData">string[] holding source data</param>
        public NonPlayerCharacter_Riak importFromCSV_NPC(string[] npcData)
        {
            NonPlayerCharacter_Riak thisNpcRiak = null;

            try
            {
                // create empty lists for variable length collections
                // (skills, myTitles)
                Tuple<string, int>[] skills = null;
                List<string> myTitles = new List<string>();

                // check to see if any data present for variable length collections
                if (npcData.Length > 29)
                {
                    // create variables to hold start/end index positions
                    int skStart, skEnd, tiStart, tiEnd;
                    skStart = skEnd = tiStart = tiEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 29; i < npcData.Length; i++)
                    {
                        if (npcData[i].Equals("skStart"))
                        {
                            skStart = i;
                        }
                        else if (npcData[i].Equals("skEnd"))
                        {
                            skEnd = i;
                        }
                        else if (npcData[i].Equals("tiStart"))
                        {
                            tiStart = i;
                        }
                        else if (npcData[i].Equals("tiEnd"))
                        {
                            tiEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // skills
                    List<Tuple<string, int>> tempSkills = new List<Tuple<string, int>>();

                    if ((skStart > -1) && (skEnd > -1))
                    {
                        // check to ensure all skills have accompanying skill level
                        if (Globals_Game.IsOdd(skStart + skEnd))
                        {
                            for (int i = skStart + 1; i < skEnd; i = i + 2)
                            {
                                Tuple<string, int> thisSkill = new Tuple<string, int>(npcData[i], Convert.ToInt32(npcData[i + 1]));
                                tempSkills.Add(thisSkill);
                            }
                            // convert skills list to skills array
                            skills = tempSkills.ToArray();
                        }
                    }

                    // myTitles
                    if ((tiStart > -1) && (tiEnd > -1))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i++)
                        {
                            myTitles.Add(npcData[i]);
                        }
                    }
                }

                // if no skills, generate random set
                if (skills == null)
                {
                    Tuple<Skill, int>[] generatedSkills = this.generateSkillSet();

                    // convert to format for saving to database
                    skills = new Tuple<String, int>[generatedSkills.Length];
                    for (int i = 0; i < generatedSkills.Length; i++)
                    {
                        skills[i] = new Tuple<string, int>(generatedSkills[i].Item1.skillID, generatedSkills[i].Item2);
                    }
                }

                // create DOB tuple
                if (String.IsNullOrWhiteSpace(npcData[5]))
                {
                    npcData[5] = Globals_Game.myRand.Next(4).ToString();
                }
                Tuple<uint, byte> dob = new Tuple<uint, byte>(Convert.ToUInt32(npcData[4]), Convert.ToByte(npcData[5]));

                // check for presence of CONDITIONAL VARIABLES
                string loc, aID, boss;
                loc = aID = boss = null;

                if (!String.IsNullOrWhiteSpace(npcData[26]))
                {
                    loc = npcData[26];
                }
                if (!String.IsNullOrWhiteSpace(npcData[27]))
                {
                    aID = npcData[27];
                }
                if (!String.IsNullOrWhiteSpace(npcData[28]))
                {
                    boss = npcData[28];
                }

                // create NonPlayerCharacter_Riak object
                thisNpcRiak = new NonPlayerCharacter_Riak(npcData[1], npcData[2], npcData[3], dob, Convert.ToBoolean(npcData[6]),
                    npcData[7], Convert.ToBoolean(npcData[8]), Convert.ToDouble(npcData[9]), Convert.ToDouble(npcData[10]),
                    new List<string>(), npcData[11], Convert.ToDouble(npcData[12]), Convert.ToDouble(npcData[13]),
                    Convert.ToDouble(npcData[14]), Convert.ToDouble(npcData[15]), skills, Convert.ToBoolean(npcData[16]),
                    Convert.ToBoolean(npcData[17]), npcData[18], npcData[19], npcData[20], npcData[21], myTitles, npcData[22],
                    Convert.ToUInt32(npcData[23]), Convert.ToBoolean(npcData[24]), Convert.ToBoolean(npcData[25]),
                    ails: new Dictionary<string, Ailment>(), loc: loc, aID: aID, mb: boss);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisNpcRiak;
        }

        /// <summary>
        /// Creates a Skill object using data in a string array
        /// </summary>
        /// <returns>Skill object</returns>
        /// <param name="skillData">string[] holding source data</param>
        public Skill importFromCSV_Skill(string[] skillData)
        {
            Skill thisSkill = null;

            try
            {
                // create empty lists for variable length collections
                // (effects)
                Dictionary<string, double> effects = new Dictionary<string,double>();

                // check to see if any data present for variable length collections
                if (skillData.Length > 3)
                {
                    // create variables to hold start/end index positions
                    int effStart, effEnd;
                    effStart = effEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 3; i < skillData.Length; i++)
                    {
                        if (skillData[i].Equals("effStart"))
                        {
                            effStart = i;
                        }
                        else if (skillData[i].Equals("effEnd"))
                        {
                            effEnd = i;
                        }
                    }

                    // ADD ITEMS to appropriate list
                    // effects
                    if ((effStart > -1) && (effEnd > -1))
                    {
                        // check to ensure all effects have accompanying effect level
                        if (!Globals_Game.IsOdd(effStart + effEnd))
                        {
                            for (int i = effStart + 1; i < effEnd; i = i + 2)
                            {
                                effects.Add(skillData[i], Convert.ToDouble(skillData[i+1]));
                            }
                        }
                    }
                }

                // create Skill object
                thisSkill = new Skill(skillData[1], skillData[2], effects);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisSkill;
        }

        /// <summary>
        /// Creates a Army object using data in a string array
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="armyData">string[] holding source data</param>
        public Army importFromCSV_Army(string[] armyData)
        {
            Army thisArmy = null;

            try
            {
                // create troops array
                uint[] troops = new uint[] { Convert.ToUInt32(armyData[9]), Convert.ToUInt32(armyData[10]),
                    Convert.ToUInt32(armyData[11]), Convert.ToUInt32(armyData[12]), Convert.ToUInt32(armyData[13]),
                    Convert.ToUInt32(armyData[14])};

                // check for presence of conditional values
                string maint, aggr, odds;
                maint = aggr = odds = null;

                if (!String.IsNullOrWhiteSpace(armyData[6]))
                {
                    maint = armyData[6];
                }
                if (!String.IsNullOrWhiteSpace(armyData[7]))
                {
                    aggr = armyData[7];
                }
                if (!String.IsNullOrWhiteSpace(armyData[8]))
                {
                    odds = armyData[8];
                }

                // create Army object
                thisArmy = new Army(armyData[1], armyData[2], armyData[3], Convert.ToDouble(armyData[4]), armyData[5],
                    maint: Convert.ToBoolean(maint), aggr: Convert.ToByte(aggr), odds: Convert.ToByte(odds), trp: troops);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisArmy;
        }

        /// <summary>
        /// Creates a Language_Riak object using data in a string array
        /// </summary>
        /// <returns>Language_Riak object</returns>
        /// <param name="langData">string[] holding source data</param>
        public Language_Riak importFromCSV_Language(string[] langData)
        {
            Language_Riak thisLangRiak = null;

            try
            {
                // create Language_Riak object
                thisLangRiak = new Language_Riak(langData[1], langData[2], Convert.ToInt32(langData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisLangRiak;
        }

        /// <summary>
        /// Creates a BaseLanguage object using data in a string array
        /// </summary>
        /// <returns>BaseLanguage object</returns>
        /// <param name="baseLangData">string[] holding source data</param>
        public BaseLanguage importFromCSV_BaseLanguage(string[] baseLangData)
        {
            BaseLanguage thisBaseLang = null;

            try
            {
                // create BaseLanguage object
                thisBaseLang = new BaseLanguage(baseLangData[1], baseLangData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisBaseLang;
        }

        /// <summary>
        /// Creates a Nationality object using data in a string array
        /// </summary>
        /// <returns>Nationality object</returns>
        /// <param name="natData">string[] holding source data</param>
        public Nationality importFromCSV_Nationality(string[] natData)
        {
            Nationality thisNat = null;

            try
            {
                // create Nationality object
                thisNat = new Nationality(natData[1], natData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisNat;
        }

        /// <summary>
        /// Creates a Rank object using data in a string array
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="rankData">string[] holding source data</param>
        public Rank importFromCSV_Rank(string[] rankData)
        {
            Rank thisRank = null;

            try
            {
                // create empty lists for variable length collections
                // (title)
                TitleName[] title = null;

                // create variables to hold start/end index positions
                int tiStart, tiEnd;
                tiStart = tiEnd = -1;

                // iterate through main list STORING START/END INDEX POSITIONS
                for (int i = 3; i < rankData.Length; i++)
                {
                    if (rankData[i].Equals("tiStart"))
                    {
                        tiStart = i;
                    }
                    else if (rankData[i].Equals("tiEnd"))
                    {
                        tiEnd = i;
                    }
                }

                // ADD ITEMS to appropriate list
                // title
                List<TitleName> tempTitle = new List<TitleName>();

                if ((tiStart > -1) && (tiEnd > -1))
                {
                    // check to ensure all effects have accompanying effect level
                    if (Globals_Game.IsOdd(tiStart + tiEnd))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i = i + 2)
                        {
                            TitleName thisTitle = new TitleName(rankData[i], rankData[i + 1]);
                            tempTitle.Add(thisTitle);
                        }
                        // create title array from list
                        title = tempTitle.ToArray();
                    }
                }

                if (title.Length > 0)
                {
                    // create Rank object
                    thisRank = new Rank(Convert.ToByte(rankData[1]), title, Convert.ToByte(rankData[2]));
                }

            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisRank;
        }

        /// <summary>
        /// Creates a Position_Riak object using data in a string array
        /// </summary>
        /// <returns>Position_Riak object</returns>
        /// <param name="posData">string[] holding source data</param>
        public Position_Riak importFromCSV_Position(string[] posData)
        {
            Position_Riak thisPosRiak = null;

            try
            {
                // create empty lists for variable length collections
                // (title)
                TitleName[] title = null;

                // create variables to hold start/end index positions
                int tiStart, tiEnd;
                tiStart = tiEnd = -1;

                // iterate through main list STORING START/END INDEX POSITIONS
                for (int i = 3; i < posData.Length; i++)
                {
                    if (posData[i].Equals("tiStart"))
                    {
                        tiStart = i;
                    }
                    else if (posData[i].Equals("tiEnd"))
                    {
                        tiEnd = i;
                    }
                }

                // ADD ITEMS to appropriate list
                // title
                List<TitleName> tempTitle = new List<TitleName>();

                if ((tiStart > -1) && (tiEnd > -1))
                {
                    // check to ensure all effects have accompanying effect level
                    if (Globals_Game.IsOdd(tiStart + tiEnd))
                    {
                        for (int i = tiStart + 1; i < tiEnd; i = i + 2)
                        {
                            TitleName thisTitle = new TitleName(posData[i], posData[i + 1]);
                            tempTitle.Add(thisTitle);
                        }
                        // create title array from list
                        title = tempTitle.ToArray();
                    }
                }

                if (title.Length > 0)
                {
                    // create Rank object
                    thisPosRiak = new Position_Riak(Convert.ToByte(posData[1]), title, Convert.ToByte(posData[2]), posData[3], posData[4]);
                }

            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisPosRiak;
        }

        /// <summary>
        /// Creates a Siege object using data in a string array
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="siegeData">string[] holding source data</param>
        public Siege importFromCSV_Siege(string[] siegeData)
        {
            Siege thisSiege = null;

            try
            {
                // check for presence of conditional values
                string totAttCas, totDefCas, totDays, defenderAdd, siegeEnd;
                totAttCas = totDefCas = totDays = defenderAdd = siegeEnd = null;

                if (!String.IsNullOrWhiteSpace(siegeData[11]))
                {
                    totAttCas = siegeData[11];
                }
                if (!String.IsNullOrWhiteSpace(siegeData[12]))
                {
                    totDefCas = siegeData[12];
                }
                if (!String.IsNullOrWhiteSpace(siegeData[13]))
                {
                    totDays = siegeData[13];
                }
                if (!String.IsNullOrWhiteSpace(siegeData[14]))
                {
                    defenderAdd = siegeData[14];
                }
                if (!String.IsNullOrWhiteSpace(siegeData[15]))
                {
                    siegeEnd = siegeData[15];
                }

                // create Siege object
                thisSiege = new Siege(siegeData[1], Convert.ToUInt32(siegeData[2]), Convert.ToByte(siegeData[3]), siegeData[4],
                    siegeData[5], siegeData[6], siegeData[7], siegeData[8], Convert.ToDouble(siegeData[9]),
                    Convert.ToDouble(siegeData[10]), Convert.ToInt32(totAttCas), Convert.ToInt32(totDefCas),
                    Convert.ToDouble(totDays), defenderAdd, siegeEnd);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisSiege;
        }

        /// <summary>
        /// Creates a Terrain object using data in a string array
        /// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="terrData">string[] holding source data</param>
        public Terrain importFromCSV_Terrain(string[] terrData)
        {
            Terrain thisTerr = null;

            try
            {
                // create Terrain object
                thisTerr = new Terrain(terrData[1], terrData[2], Convert.ToDouble(terrData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(oe.Message);
                }
            }

            return thisTerr;
        }

        /// <summary>
        /// Uses individual game objects to populate variable-length collections within other game objects
        /// </summary>
		/// <param name="fiefMasterList">Fief_Riak objects</param>
		/// <param name="pcMasterList">PlayerCharacter_Riak objects</param>
		/// <param name="npcMasterList">NonPlayerCharacter_Riak objects</param>
		/// <param name="provinceMasterList">Province_Riak objects</param>
		/// <param name="kingdomMasterList">Kingdom_Riak objects</param>
		/// <param name="siegeMasterList">Siege objects</param>
		/// <param name="armyMasterList">Army objects</param>
		/// <param name="bucketID">The name of the Riak bucket in which to store the game objects</param>
		public void SynchGameObjectCollections(Dictionary<string, Fief_Riak> fiefMasterList, Dictionary<string,
			PlayerCharacter_Riak> pcMasterList, Dictionary<string, NonPlayerCharacter_Riak> npcMasterList,
			Dictionary<string, Province_Riak> provinceMasterList, Dictionary<string, Kingdom_Riak> kingdomMasterList,
			Dictionary<string, Siege> siegeMasterList, Dictionary<string, Army> armyMasterList, string bucketID)
        {

			// iterate through FIEFS
			foreach (KeyValuePair<string, Fief_Riak> fiefEntry in fiefMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(fiefEntry.Value.titleHolder))
                {
					Character_Riak thisTiHo = null;
					if (pcMasterList.ContainsKey(fiefEntry.Value.titleHolder))
					{
						thisTiHo = pcMasterList[fiefEntry.Value.titleHolder];
					}
					else if (npcMasterList.ContainsKey(fiefEntry.Value.titleHolder))
					{
						thisTiHo = npcMasterList[fiefEntry.Value.titleHolder];
					}

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(fiefEntry.Key))
                        {
                            thisTiHo.myTitles.Add(fiefEntry.Key);
                        }
                    }
                }

                // get owner
                if (fiefEntry.Value.owner != null)
                {
					PlayerCharacter_Riak thisOwner = null;
					if (pcMasterList.ContainsKey(fiefEntry.Value.owner))
					{
						thisOwner = pcMasterList[fiefEntry.Value.owner];
					}

                    // put fief in owner's ownedFiefs
					if (!thisOwner.ownedFiefs.Contains(fiefEntry.Key))
                    {
						thisOwner.ownedFiefs.Add(fiefEntry.Key);
                    }
                }
            }

            // iterate through PROVINCES
			foreach (KeyValuePair<string, Province_Riak> provEntry in provinceMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(provEntry.Value.titleHolder))
                {
					Character_Riak thisTiHo = null;
					if (pcMasterList.ContainsKey(provEntry.Value.titleHolder))
					{
						thisTiHo = pcMasterList[provEntry.Value.titleHolder];
					}
					else if (npcMasterList.ContainsKey(provEntry.Value.titleHolder))
					{
						thisTiHo = npcMasterList[provEntry.Value.titleHolder];
					}

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(provEntry.Key))
                        {
                            thisTiHo.myTitles.Add(provEntry.Key);
                        }
                    }
                }

                // get owner
                if (provEntry.Value.owner != null)
                {
					PlayerCharacter_Riak thisOwner = null;
					if (pcMasterList.ContainsKey(provEntry.Value.owner))
					{
						thisOwner = pcMasterList[provEntry.Value.owner];
					}

                    // put province in owner's ownedProvinces
					if (!thisOwner.ownedProvinces.Contains(provEntry.Key))
                    {
						thisOwner.ownedProvinces.Add(provEntry.Key);
                    }
                }
            }

            // iterate through KINGDOMS
			foreach (KeyValuePair<string, Kingdom_Riak> kingEntry in kingdomMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(kingEntry.Value.titleHolder))
                {
					Character_Riak thisTiHo = null;
					if (pcMasterList.ContainsKey(kingEntry.Value.titleHolder))
					{
						thisTiHo = pcMasterList[kingEntry.Value.titleHolder];
					}
					else if (npcMasterList.ContainsKey(kingEntry.Value.titleHolder))
					{
						thisTiHo = npcMasterList[kingEntry.Value.titleHolder];
					}

                    // put fief id in holder's myTitles
                    if (thisTiHo != null)
                    {
                        if (!thisTiHo.myTitles.Contains(kingEntry.Key))
                        {
                            thisTiHo.myTitles.Add(kingEntry.Key);
                        }
                    }
                }
            }

            // iterate through PCs
			foreach (KeyValuePair<string, PlayerCharacter_Riak> pcEntry in pcMasterList)
            {
                if (pcEntry.Value.isAlive)
                {
                    // get location
					if (!String.IsNullOrWhiteSpace(pcEntry.Value.location))
                    {
						Fief_Riak thisFief = null;
						if (fiefMasterList.ContainsKey(pcEntry.Value.location))
						{
							thisFief = fiefMasterList[pcEntry.Value.location];
						}

                        // put PC in fief's characters
						if (!thisFief.charactersInFief.Contains(pcEntry.Key))
                        {
							thisFief.charactersInFief.Add(pcEntry.Key);
                        }
                    }
                }
            }

            // iterate through NPCs
			foreach (KeyValuePair<string, NonPlayerCharacter_Riak> npcEntry in npcMasterList)
            {
                if (npcEntry.Value.isAlive)
                {
                    // get location
					if (!String.IsNullOrWhiteSpace(npcEntry.Value.location))
                    {
						Fief_Riak thisFief = null;
						if (fiefMasterList.ContainsKey(npcEntry.Value.location))
						{
							thisFief = fiefMasterList[npcEntry.Value.location];
						}

                        // put NPC in fief's characters
						if (thisFief != null)
						{
							if (!thisFief.charactersInFief.Contains(npcEntry.Key))
							{
								thisFief.charactersInFief.Add(npcEntry.Key);
							}
						}
                    }

                    // get employer
					if (!String.IsNullOrWhiteSpace(npcEntry.Value.myBoss))
                    {
						PlayerCharacter_Riak thisBoss = null;
						if (pcMasterList.ContainsKey(npcEntry.Value.myBoss))
						{
							thisBoss = pcMasterList[npcEntry.Value.myBoss];
						}

                        if (thisBoss != null)
                        {
                            // put NPC in employer's myNPCs
							if (!thisBoss.myNPCs.Contains(npcEntry.Key))
                            {
								thisBoss.myNPCs.Add(npcEntry.Key);
                            }
                        }
                    }

                    // get familyID
					if (!String.IsNullOrWhiteSpace(npcEntry.Value.familyID))
                    {
						PlayerCharacter_Riak thisHeadOfFamily = null;
						if (pcMasterList.ContainsKey(npcEntry.Value.familyID))
						{
							thisHeadOfFamily = pcMasterList[npcEntry.Value.familyID];
						}

                        if (thisHeadOfFamily != null)
                        {
                            // put NPC in headOfFamily's myNPCs
							if (!thisHeadOfFamily.myNPCs.Contains(npcEntry.Key))
                            {
								thisHeadOfFamily.myNPCs.Add(npcEntry.Key);
                            }
                        }
                    }
                }
            }

            // iterate through SIEGES
            foreach (KeyValuePair<string, Siege> siegeEntry in siegeMasterList)
            {
                // ensure siege not ended
                if (String.IsNullOrWhiteSpace(siegeEntry.Value.endDate))
                {
                    // get attacking PC
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.besiegingPlayer))
                    {
						PlayerCharacter_Riak attacker = null;
						if (pcMasterList.ContainsKey(siegeEntry.Value.besiegingPlayer))
						{
							attacker = pcMasterList[siegeEntry.Value.besiegingPlayer];
						}

                        // put siege id in attacker's mySieges
                        if (attacker != null)
                        {
                            if (!attacker.mySieges.Contains(siegeEntry.Key))
                            {
                                attacker.mySieges.Add(siegeEntry.Key);
                            }
                        }
                    }

                    // get defending PC
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.defendingPlayer))
                    {
						PlayerCharacter_Riak defender = null;
						if (pcMasterList.ContainsKey(siegeEntry.Value.defendingPlayer))
						{
							defender = pcMasterList[siegeEntry.Value.defendingPlayer];
						}

                        // put siege id in defender's mySieges
                        if (defender != null)
                        {
                            if (!defender.mySieges.Contains(siegeEntry.Key))
                            {
                                defender.mySieges.Add(siegeEntry.Key);
                            }
                        }
                    }

                    // get defending Fief
                    if (!String.IsNullOrWhiteSpace(siegeEntry.Value.besiegedFief))
                    {
						Fief_Riak besiegedFief = null;
						if (fiefMasterList.ContainsKey(siegeEntry.Value.besiegedFief))
						{
							besiegedFief = fiefMasterList[siegeEntry.Value.besiegedFief];
						}

                        // put siege id in fief's siege
                        if (besiegedFief != null)
                        {
                            if (!besiegedFief.siege.Equals(siegeEntry.Key))
                            {
                                besiegedFief.siege = siegeEntry.Key;
                            }
                        }
                    }
                }
            }

            // iterate through ARMIES
            foreach (KeyValuePair<string, Army> armyEntry in armyMasterList)
            {
                // get army owner
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.owner))
                {
					PlayerCharacter_Riak owner = null;
					if (pcMasterList.ContainsKey(armyEntry.Value.owner))
					{
						owner = pcMasterList[armyEntry.Value.owner];
					}

                    // put army in owner's myArmies
                    if (owner != null)
                    {
						if (!owner.myArmies.Contains(armyEntry.Key))
                        {
							owner.myArmies.Add(armyEntry.Key);
                        }
                    }
                }

                // get army leader
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.leader))
                {
					Character_Riak leader = null;
					if (pcMasterList.ContainsKey(armyEntry.Value.leader))
					{
						leader = pcMasterList[armyEntry.Value.leader];
					}
					else if (npcMasterList.ContainsKey(armyEntry.Value.leader))
					{
						leader = npcMasterList[armyEntry.Value.leader];
					}

                    // put army id in leader's armyID
                    if (leader != null)
                    {
                        if (!leader.armyID.Equals(armyEntry.Key))
                        {
                            leader.armyID = armyEntry.Key;
                        }
                    }
                }

                // get army location
                if (!String.IsNullOrWhiteSpace(armyEntry.Value.location))
                {
					Fief_Riak thisFief = null;
					if (fiefMasterList.ContainsKey(armyEntry.Value.location))
					{
						thisFief = fiefMasterList[armyEntry.Value.location];
					}

                    // put army id in fief's armies
                    if (thisFief != null)
                    {
                        if (!thisFief.armies.Contains(armyEntry.Key))
                        {
                            thisFief.armies.Add(armyEntry.Key);
                        }
                    }
                }
            }

			// save all objects
			foreach (KeyValuePair<string, Fief_Riak> fiefEntry in fiefMasterList)
			{
				// save to Riak
				this.writeFief(bucketID, fr: fiefEntry.Value);
			}
			foreach (KeyValuePair<string, Province_Riak> provEntry in provinceMasterList)
			{
				// save to Riak
				this.writeProvince(bucketID, pr: provEntry.Value);
			}
			foreach (KeyValuePair<string, Kingdom_Riak> kingEntry in kingdomMasterList)
			{
				// save to Riak
				this.writeKingdom(bucketID, kr: kingEntry.Value);
			}
			foreach (KeyValuePair<string, PlayerCharacter_Riak> pcEntry in pcMasterList)
			{
				// save to Riak
				this.writePC(bucketID, pcr: pcEntry.Value);
			}
			foreach (KeyValuePair<string, NonPlayerCharacter_Riak> npcEntry in npcMasterList)
			{
				// save to Riak
				this.writeNPC(bucketID, npcr: npcEntry.Value);
			}
			foreach (KeyValuePair<string, Siege> siegeEntry in siegeMasterList)
			{
				// save to Riak
				this.writeSiege(bucketID, siegeEntry.Value);
			}
			foreach (KeyValuePair<string, Army> armyEntry in armyMasterList)
			{
				// save to Riak
				this.writeArmy(bucketID, armyEntry.Value);
			}

        }

        /// <summary>
        /// Creates game map using data imported from a CSV file and writes it to the database
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the Riak bucket in which to store the game objects</param>
        public bool CreateMapArrayFromCSV(string filename, string bucketID)
        {
            bool success = true;
            List<TaggedEdge<string, string>> mapEdges = new List<TaggedEdge<string, string>>();
            string lineIn;
            string[] lineParts;
            StreamReader srHexes = null;
            string[,] mapHexes = null;
            int row = 0;

            try
            {
                // opens StreamReader to read in  data from csv file
                srHexes = new StreamReader(filename);
            }
            // catch following IO exceptions that could be thrown by the StreamReader 
            catch (FileNotFoundException fnfe)
            {
                success = false;
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(fnfe.Message);
                }
            }
            catch (IOException ioe)
            {
                success = false;
                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show(ioe.Message);
                }
            }

            // CREATE HEXMAP ARRAY
            // while there is data in the line
            while ((lineIn = srHexes.ReadLine()) != null)
            {
                // put the contents of the line into lineParts array, splitting on (char)9 (TAB)
                lineParts = lineIn.Split(',');

                // first line should contain array dimensions
                if (lineParts[0].Equals("dimensions"))
                {
                    mapHexes = new string[Convert.ToInt32(lineParts[1]), Convert.ToInt32(lineParts[2])];
                }

                // the rest of the lines hold the values (fiefIDs)
                else
                {
                    for (int i = 0; i < mapHexes.GetLength(1); i++ )
                    {
                        mapHexes[row, i] = lineParts[i];
                    }

                    // increment row
                    row++;
                }
            }

            // create list of map edges from array
            mapEdges = this.CreateMapFromArray(mapHexes);

            // save to database
            this.writeMapEdges(bucketID, edges: mapEdges);

            return success;
        }

        /// <summary>
        /// Creates list of map edges using a 2D string array
        /// </summary>
        /// <returns>List containing map edges</returns>
        /// <param name="mapArray">string[,] containing map data</param>
        public List<TaggedEdge<string, string>> CreateMapFromArray(string[,] mapArray)
        {
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();
            TaggedEdge<string, string> thisEdge = null;

            // iterate row
            for (int i = 0; i < mapArray.GetLength(0); i++)
            {
                // iterate column
                for (int j = 0; j < mapArray.GetLength(1); j++)
                {
                    // don't process null entries
                    if (!String.IsNullOrWhiteSpace(mapArray[i, j]))
                    {
                        // if not first hex in row, ADD LINKS BETWEEN THIS HEX/FIEF AND PREVIOUS HEX/FIEF
                        if (j != 0)
                        {
                            if (!String.IsNullOrWhiteSpace(mapArray[i, j-1]))
                            {
                                // add link to previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i, j - 1], "W");
                                edgesOut.Add(thisEdge);

                                // add link from previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j - 1], mapArray[i, j], "E");
                                edgesOut.Add(thisEdge);
                            }
                        }

                        // if not first row, ADD LINKS BETWEEN THIS HEX/FIEF AND HEX/FIEFS ABOVE
                        if (i != 0)
                        {
                            // keep track of target columns
                            int col = 0;

                            // if not first column in even-numbered row, add link between this hex/fief and hex/fief above left
                            if (!((!Globals_Game.IsOdd(i)) && (j == 0)))
                            {
                                // target correct column (above left is different for odd/even numbered rows)
                                if (Globals_Game.IsOdd(i))
                                {
                                    col = j;
                                }
                                else
                                {
                                    col = j - 1;
                                }

                                if (!String.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NW");
                                    edgesOut.Add(thisEdge);

                                    // add link from above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SE");
                                    edgesOut.Add(thisEdge);
                                }
                            }

                            // if not last column in odd-numbered row, add link between this hex/fief and hex/fief above right
                            if (!((Globals_Game.IsOdd(i)) && (j == mapArray.GetLength(1) - 1)))
                            {
                                // target correct column (above right is different for odd/even numbered rows)
                                if (Globals_Game.IsOdd(i))
                                {
                                    col = j+1;
                                }
                                else
                                {
                                    col = j;
                                }

                                if (!String.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NE");
                                    edgesOut.Add(thisEdge);

                                    // add link from above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SW");
                                    edgesOut.Add(thisEdge);
                                }
                            }
                        }
                    }
                }
            }

            return edgesOut;
        }

    }

}
