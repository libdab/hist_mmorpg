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

			//this.ImportFromCSV("gameObjects.csv", "fromCSV", true);
			//this.CreateMapArrayFromCSV ("map.csv", "fromCSV");
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

        // ------------------- GAME START/INITIALISATION

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
				this.databaseRead (gameID);
			}
			else
			{
				// create from code
				this.loadFromCode ();
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
            this.setUpEditSkillEffectList();

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

            // enable sysAdmin menu, if appropriate
            if (Globals_Client.myPlayerCharacter.checkIsSysAdmin())
            {
                this.adminToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.adminToolStripMenuItem.Enabled = false;
            }

            // initialise player's character display in UI
            this.refreshCharacterContainer();

        }

		/// <summary>
		/// Creates some game objects from code (temporary)
		/// </summary>
		public void loadFromCode()
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

            // create barredNationalities for fiefs
            List<string> barredNats01 = new List<string>();
            List<string> barredNats02 = new List<string>();
            List<string> barredNats03 = new List<string>();
            List<string> barredNats04 = new List<string>();
            List<string> barredNats05 = new List<string>();
            List<string> barredNats06 = new List<string>();
            List<string> barredNats07 = new List<string>();

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'R', c1, plains, fief1Chars, keep1BarChars, barredNats01, 0, 2000000, armies001, false, transfers001, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief1.id, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 10, 1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'C', c1, hills, fief2Chars, keep2BarChars, barredNats02, 0, 4000, armies002, false, transfers002, false, r: myRank15);
            Globals_Game.fiefMasterList.Add(myFief2.id, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', c1, plains, fief3Chars, keep3BarChars, barredNats03, 0, 100000, armies003, false, transfers003, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief3.id, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', c1, plains, fief4Chars, keep4BarChars, barredNats04, 0, 100000, armies004, false, transfers004, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief4.id, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', f1, plains, fief5Chars, keep5BarChars, barredNats05, 0, 100000, armies005, false, transfers005, false, r: myRank15);
            Globals_Game.fiefMasterList.Add(myFief5.id, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', f1, plains, fief6Chars, keep6BarChars, barredNats06, 0, 100000, armies006, false, transfers006, false, r: myRank17);
            Globals_Game.fiefMasterList.Add(myFief6.id, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', f1, forrest, fief7Chars, keep7BarChars, barredNats07, 0, 100000, armies007, false, transfers007, false, r: myRank15);
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
            PlayerCharacter myChar1 = new PlayerCharacter("Char_47", "Dave", "Bond", myDob001, true, nationality02, true, 8.50, 9.0, myGoTo1, c1, 90, 0, 7.2, 6.1, Globals_Game.generateSkillSet(), false, false, "Char_47", "Char_403", null, null, false, 13000, myEmployees1, myFiefsOwned1, myProvsOwned1, "ESX02", "ESX02", myTitles001, myArmies001, mySieges001, null, loc: myFief1, pID: "libdab");
            Globals_Game.pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("Char_40", "Bave", "Dond", myDob002, true, nationality01, true, 8.50, 6.0, myGoTo2, f1, 90, 0, 5.0, 4.5, Globals_Game.generateSkillSet(), false, false, "Char_40", null, null, null, false, 13000, myEmployees2, myFiefsOwned2, myProvsOwned2, "ESR03", "ESR03", myTitles002, myArmies002, mySieges002, null, loc: myFief7, pID: "otherGuy");
            Globals_Game.pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("Char_401", "Jimmy", "Servant", myDob003, true, nationality02, true, 8.50, 6.0, myGoTo3, c1, 90, 0, 3.3, 6.7, Globals_Game.generateSkillSet(), false, false, null, null, null, null, 0, false, false, myTitles003, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("Char_402", "Johnny", "Servant", myDob004, true, nationality02, true, 8.50, 6.0, myGoTo4, c1, 90, 0, 7.1, 5.2, Globals_Game.generateSkillSet(), false, false, null, null, null, null, 10000, true, false, myTitles004, null, mb: myChar1.charID, loc: myFief1);
            Globals_Game.npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myNPC3 = new NonPlayerCharacter("Char_403", "Harry", "Bailiff", myDob005, true, nationality01, true, 8.50, 6.0, myGoTo5, c1, 90, 0, 7.1, 5.2, Globals_Game.generateSkillSet(), true, false, null, null, null, null, 10000, false, false, myTitles005, null, mb: myChar2.charID, loc: myFief6);
            Globals_Game.npcMasterList.Add(myNPC3.charID, myNPC3);
            NonPlayerCharacter myChar1Wife = new NonPlayerCharacter("Char_404", "Bev", "Bond", myDob006, false, nationality02, true, 2.50, 9.0, myGoTo6, f1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), false, false, "Char_47", "Char_47", null, null, 30000, false, false, myTitles006, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            NonPlayerCharacter myChar2Son = new NonPlayerCharacter("Char_405", "Horatio", "Dond", myDob007, true, nationality01, true, 8.50, 6.0, myGoTo7, f1, 90, 0, 7.1, 5.2, Globals_Game.generateSkillSet(), true, false, "Char_40", "Char_406", "Char_40", null, 10000, false, true, myTitles007, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            NonPlayerCharacter myChar2SonWife = new NonPlayerCharacter("Char_406", "Mave", "Dond", myDob008, false, nationality02, true, 2.50, 9.0, myGoTo8, f1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), true, false, "Char_40", "Char_405", null, null, 30000, false, false, myTitles008, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);
            NonPlayerCharacter myChar1Son = new NonPlayerCharacter("Char_407", "Rickie", "Bond", myDob009, true, nationality02, true, 2.50, 9.0, myGoTo9, c1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, true, myTitles009, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Son.charID, myChar1Son);
            NonPlayerCharacter myChar1Daughter = new NonPlayerCharacter("Char_408", "Elsie", "Bond", myDob010, false, nationality02, true, 2.50, 9.0, myGoTo10, c1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), true, false, "Char_47", null, "Char_47", "Char_404", 30000, false, false, myTitles010, null, loc: myFief1);
            Globals_Game.npcMasterList.Add(myChar1Daughter.charID, myChar1Daughter);
            NonPlayerCharacter myChar2Son2 = new NonPlayerCharacter("Char_409", "Wayne", "Dond", myDob011, true, nationality01, true, 2.50, 9.0, myGoTo11, f1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles011, null, loc: myFief6);
            Globals_Game.npcMasterList.Add(myChar2Son2.charID, myChar2Son2);
            NonPlayerCharacter myChar2Daughter = new NonPlayerCharacter("Char_410", "Esmerelda", "Dond", myDob012, false, nationality01, true, 2.50, 9.0, myGoTo12, f1, 90, 0, 4.0, 6.0, Globals_Game.generateSkillSet(), true, false, "Char_40", null, "Char_40", null, 30000, false, false, myTitles012, null, loc: myFief6);
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

            // set sysAdmin
            Globals_Game.sysAdmin = myChar1;

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

            // create and add ailment
            Ailment myAilment1 = new Ailment(Globals_Game.getNextAilmentID(), "Battlefield injury", Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear, 3, 1);
            myChar1.ailments.Add(myAilment1.ailmentID, myAilment1);

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
            Army myArmy = new Army(Globals_Game.getNextArmyID(), null, null, 90, null, trp: myArmyTroops);
            Globals_Game.armyMasterList.Add(myArmy.armyID, myArmy);
            myArmy.owner = myChar1.charID;
            // myArmy.leader = myChar1.charID;
            myArmy.days = Globals_Game.pcMasterList[myArmy.owner].days;
            myChar1.myArmies.Add(myArmy);
            // myChar1.armyID = myArmy.armyID;
            myArmy.location = Globals_Game.pcMasterList[myArmy.owner].location.id;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = new uint[] { 10, 10, 30, 0, 40, 200, 0 };
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

        // ------------------- SEASONAL UPDATE

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
                    Globals_Game.pcMasterList.Add(pc.charID, pc);
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

            // CHECK OWNERSHIP CHALLENGES
            Globals_Game.processOwnershipChallenges();

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
                Globals_Client.myPlayerCharacter = Globals_Game.pcMasterList[playerID];
                Globals_Client.charToView = Globals_Client.myPlayerCharacter;
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
            // get tag from button
            Button button = sender as Button;
            string operation = button.Tag.ToString();

            // get fief
            Fief thisFief = Globals_Client.myPlayerCharacter.location;

            // check for siege
            if (!String.IsNullOrWhiteSpace(thisFief.siege))
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
                        // if necessary, exit keep (new armies are created outside keep)
                        if (Globals_Client.myPlayerCharacter.inKeep)
                        {
                            Globals_Client.myPlayerCharacter.exitKeep();
                        }

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
                    if (((!String.IsNullOrWhiteSpace(Globals_Client.armyToView.leader))
                        && (!(Globals_Client.armyToView.leader.Equals(Globals_Client.myPlayerCharacter.charID))))
                        && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.armyID)))
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
                    string[] troopTypeLabels = new string[] { "knights", "men-at-arms", "light cavalry", "longbowmen", "crossbowmen", "foot", "rabble" };

                    // get number of troops to transfer
                    uint[] troopsToTransfer = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
                    troopsToTransfer[0] = Convert.ToUInt32(this.armyTransKnightTextBox.Text);
                    troopsToTransfer[1] = Convert.ToUInt32(this.armyTransMAAtextBox.Text);
                    troopsToTransfer[2] = Convert.ToUInt32(this.armyTransLCavTextBox.Text);
                    troopsToTransfer[3] = Convert.ToUInt32(this.armyTransLongbowTextBox.Text);
                    troopsToTransfer[4] = Convert.ToUInt32(this.armyTransCrossbowTextBox.Text);
                    troopsToTransfer[5] = Convert.ToUInt32(this.armyTransFootTextBox.Text);
                    troopsToTransfer[6] = Convert.ToUInt32(this.armyTransRabbleTextBox.Text);

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
                        Fief thisFief = Globals_Client.armyToView.getLocation();

                        // create transfer entry
                        string[] thisTransfer = new string[10] { Globals_Client.myPlayerCharacter.charID, this.armyTransDropWhoTextBox.Text,
                            troopsToTransfer[0].ToString(), troopsToTransfer[1].ToString(), troopsToTransfer[2].ToString(),
                            troopsToTransfer[3].ToString(), troopsToTransfer[4].ToString(), troopsToTransfer[5].ToString(),
                            troopsToTransfer[6].ToString(), (Globals_Client.armyToView.days - daysTaken).ToString() };

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

    }

}
