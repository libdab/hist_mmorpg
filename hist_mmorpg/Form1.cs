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
            Globals_Server.registerObserver(this);

            // initialise form elements
            InitializeComponent();

			// initialise Riak elements
			rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
			rClient = (RiakClient)rCluster.CreateClient();

            // initialise game objects
			this.initGameObjects("NOTdb", "101");

			// this.ArrayFromCSV ("/home/libdab/Dissertation_data/11-07-14/hacked-player.csv", true, "testGame", "skeletonPlayers1194");

			// write game objects to Riak
			// this.writeToDB ("testGame");
        }

		/// <summary>
        /// Initialises all game objects
		/// </summary>
		/// <param name="source">Where to get object data (database or hard-coded)</param>
        /// <param name="pc">ID of PlayerCharacter to set as myChar</param>
        public void initGameObjects(String source, string pc)
        {

			if (source == "db")
			{
				// load objects from database
				this.initialDBload ("testGame");
			}
			else
			{
				// create from code
				this.initialLoad ();
			}

            // set myChar
            Globals_Client.myChar = Globals_Server.pcMasterList[pc];
                
            // set inital fief to display
            Globals_Client.fiefToView = Globals_Client.myChar.location;

            // initialise list elements in UI
            this.setUpFiefsList();
            this.setUpMeetingPLaceCharsList();
            this.setUpHouseholdCharsList();
            this.setUpArmyList();
            this.setUpSiegeList();

            // initialise character display in UI
            Globals_Client.charToView = Globals_Client.myChar;
            this.refreshCharacterContainer();

        }

		/// <summary>
		/// Creates some game objects from code (temporary)
		/// </summary>
		public void initialLoad()
		{
            // create GameClock
            GameClock myGameClock = new GameClock("clock001", 1320);
            Globals_Server.clock = myGameClock;

			// create skills
			// Dictionary of skill effects
            Dictionary<string, double> effectsCommand = new Dictionary<string, double>();
			effectsCommand.Add("battle", 0.4);
            effectsCommand.Add("siege", 0.4);
			effectsCommand.Add("npcHire", 0.2);
			// create skill
			Skill command = new Skill("sk001", "Command", effectsCommand);
			// add to skillsCollection
			Globals_Server.skillMasterList.Add(command.skillID, command);

            Dictionary<string, double> effectsChivalry = new Dictionary<string, double>();
            effectsChivalry.Add("famExpense", 0.2);
			effectsChivalry.Add("fiefExpense", 0.1);
            effectsChivalry.Add("fiefLoy", 0.2);
            effectsChivalry.Add("npcHire", 0.1);
            effectsChivalry.Add("siege", 0.1);
			Skill chivalry = new Skill("sk002", "Chivalry", effectsChivalry);
            Globals_Server.skillMasterList.Add(chivalry.skillID, chivalry);

            Dictionary<string, double> effectsAbrasiveness = new Dictionary<string, double>();
			effectsAbrasiveness.Add("battle", 0.15);
			effectsAbrasiveness.Add("death", 0.05);
            effectsAbrasiveness.Add("fiefExpense", -0.05);
            effectsAbrasiveness.Add("famExpense", 0.05);
            effectsAbrasiveness.Add("time", 0.05);
            effectsAbrasiveness.Add("siege", -0.1);
			Skill abrasiveness = new Skill("sk003", "Abrasiveness", effectsAbrasiveness);
            Globals_Server.skillMasterList.Add(abrasiveness.skillID, abrasiveness);

            Dictionary<string, double> effectsAccountancy = new Dictionary<string, double>();
            effectsAccountancy.Add("time", 0.1);
            effectsAccountancy.Add("fiefExpense", -0.2);
            effectsAccountancy.Add("famExpense", -0.2);
            effectsAccountancy.Add("fiefLoy", -0.05);
			Skill accountancy = new Skill("sk004", "Accountancy", effectsAccountancy);
            Globals_Server.skillMasterList.Add(accountancy.skillID, accountancy);

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
            Globals_Server.skillMasterList.Add(stupidity.skillID, stupidity);

            Dictionary<string, double> effectsRobust = new Dictionary<string, double>();
            effectsRobust.Add("virility", 0.2);
            effectsRobust.Add("npcHire", 0.05);
            effectsRobust.Add("fiefLoy", 0.05);
            effectsRobust.Add("death", -0.2);
			Skill robust = new Skill("sk006", "Robust", effectsRobust);
            Globals_Server.skillMasterList.Add(robust.skillID, robust);

            Dictionary<string, double> effectsPious = new Dictionary<string, double>();
            effectsPious.Add("virility", -0.2);
            effectsPious.Add("npcHire", 0.1);
            effectsPious.Add("fiefLoy", 0.1);
            effectsPious.Add("time", -0.1);
			Skill pious = new Skill("sk007", "Pious", effectsPious);
            Globals_Server.skillMasterList.Add(pious.skillID, pious);

			// add each skillsCollection key to skillsKeys
            foreach (KeyValuePair<string, Skill> entry in Globals_Server.skillMasterList)
			{
                Globals_Server.skillKeys.Add(entry.Key);
			}

            // create Language objects
            Language c = new Language("langC", "Celtic");
            Globals_Server.languageMasterList.Add(c.languageID, c);
            Language f = new Language("langF", "French");
            Globals_Server.languageMasterList.Add(f.languageID, f);
            // create languages for Fiefs
            Tuple<Language, int> myLang1 = new Tuple<Language, int>(c, 1);
            Tuple<Language, int> myLang2 = new Tuple<Language, int>(c, 2);
            Tuple<Language, int> myLang3 = new Tuple<Language, int>(f, 1);

			// create terrain objects
			Terrain plains = new Terrain("P", "Plains", 1);
			Globals_Server.terrainMasterList.Add (plains.terrainCode, plains);
			Terrain hills = new Terrain("H", "Hills", 1.5);
            Globals_Server.terrainMasterList.Add(hills.terrainCode, hills);
			Terrain forrest = new Terrain("F", "Forrest", 1.5);
            Globals_Server.terrainMasterList.Add(forrest.terrainCode, forrest);
			Terrain mountains = new Terrain("M", "Mountains", 90);
            Globals_Server.terrainMasterList.Add(mountains.terrainCode, mountains);

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
            Tuple<String, String>[] myTitle03 = new Tuple<string, string>[3];
            myTitle03[0] = new Tuple<string, string>("langC", "King");
            myTitle03[1] = new Tuple<string, string>("E", "King");
            myTitle03[2] = new Tuple<string, string>("langF", "Roi");
            Rank myRank03 = new Rank("03", myTitle03, 6);
            Globals_Server.rankMasterList.Add(myRank03.rankID, myRank03);

            Tuple<String, String>[] myTitle09 = new Tuple<string, string>[3];
            myTitle09[0] = new Tuple<string, string>("langC", "Prince");
            myTitle09[1] = new Tuple<string, string>("E", "Prince");
            myTitle09[2] = new Tuple<string, string>("langF", "Prince");
            Rank myRank09 = new Rank("09", myTitle09, 4);
            Globals_Server.rankMasterList.Add(myRank09.rankID, myRank09);

            Tuple<String, String>[] myTitle11 = new Tuple<string, string>[3];
            myTitle11[0] = new Tuple<string, string>("langC", "Earl");
            myTitle11[1] = new Tuple<string, string>("E", "Earl");
            myTitle11[2] = new Tuple<string, string>("langF", "Comte");
            Rank myRank11 = new Rank("11", myTitle11, 4);
            Globals_Server.rankMasterList.Add(myRank11.rankID, myRank11);

            Tuple<String, String>[] myTitle15 = new Tuple<string, string>[3];
            myTitle15[0] = new Tuple<string, string>("langC", "Baron");
            myTitle15[1] = new Tuple<string, string>("E", "Baron");
            myTitle15[2] = new Tuple<string, string>("langF", "Baron");
            Rank myRank15 = new Rank("15", myTitle15, 2);
            Globals_Server.rankMasterList.Add(myRank15.rankID, myRank15);

            Tuple<String, String>[] myTitle17 = new Tuple<string, string>[3];
            myTitle17[0] = new Tuple<string, string>("langC", "Lord");
            myTitle17[1] = new Tuple<string, string>("E", "Lord");
            myTitle17[2] = new Tuple<string, string>("langF", "Sire");
            Rank myRank17 = new Rank("17", myTitle17, 1);
            Globals_Server.rankMasterList.Add(myRank17.rankID, myRank17);

            // create kingdoms for provinces
            Kingdom myKingdom1 = new Kingdom("E0000", "England", r: myRank03);
            Globals_Server.kingdomMasterList.Add(myKingdom1.kingdomID, myKingdom1);
            Kingdom myKingdom2 = new Kingdom("B0000", "Boogiboogiland", r: myRank03);
            Globals_Server.kingdomMasterList.Add(myKingdom2.kingdomID, myKingdom2);

            // create provinces for fiefs
            Province myProv = new Province("ESX00", "Sussex", 6.2, king: myKingdom1, ra: myRank11);
            Globals_Server.provinceMasterList.Add(myProv.provinceID, myProv);
            Province myProv2 = new Province("ESR00", "Surrey", 6.2, king: myKingdom2, ra: myRank11);
            Globals_Server.provinceMasterList.Add(myProv2.provinceID, myProv2);

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
            List<String> armies001 = new List<string>();
            List<String> armies002 = new List<string>();
            List<String> armies003 = new List<string>();
            List<String> armies004 = new List<string>();
            List<String> armies005 = new List<string>();
            List<String> armies006 = new List<string>();
            List<String> armies007 = new List<string>();

            // create troop transfer lists for fiefs
            Dictionary<string, string[]> transfers001 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers002 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers003 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers004 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers005 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers006 = new Dictionary<string, string[]>();
            Dictionary<string, string[]> transfers007 = new Dictionary<string, string[]>();

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin001, prevFin001, 5.63, 5.5, 'C', myLang1, plains, fief1Chars, keep1BarChars, false, false, 0, 2000000, armies001, false, transfers001, false, ra: myRank17);
            Globals_Server.fiefMasterList.Add(myFief1.fiefID, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 10, 1000, 1000, 2000, 2000, currFin002, prevFin002, 5.63, 5.20, 'U', myLang1, hills, fief2Chars, keep2BarChars, false, false, 0, 4000, armies002, false, transfers002, false, ra: myRank15);
            Globals_Server.fiefMasterList.Add(myFief2.fiefID, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin003, prevFin003, 5.63, 5.5, 'C', myLang1, plains, fief3Chars, keep3BarChars, false, false, 0, 100000, armies003, false, transfers003, false, ra: myRank17);
            Globals_Server.fiefMasterList.Add(myFief3.fiefID, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin004, prevFin004, 5.63, 5.5, 'C', myLang1, plains, fief4Chars, keep4BarChars, false, false, 0, 100000, armies004, false, transfers004, false, ra: myRank17);
            Globals_Server.fiefMasterList.Add(myFief4.fiefID, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin005, prevFin005, 5.63, 5.5, 'C', myLang3, plains, fief5Chars, keep5BarChars, false, false, 0, 100000, armies005, false, transfers005, false, ra: myRank15);
            Globals_Server.fiefMasterList.Add(myFief5.fiefID, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin006, prevFin006, 5.63, 5.5, 'C', myLang3, plains, fief6Chars, keep6BarChars, false, false, 0, 100000, armies006, false, transfers006, false, ra: myRank17);
            Globals_Server.fiefMasterList.Add(myFief6.fiefID, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 10, 12000, 42000, 2000, 2000, currFin007, prevFin007, 5.63, 5.5, 'C', myLang3, forrest, fief7Chars, keep7BarChars, false, false, 0, 100000, armies007, false, transfers007, false, ra: myRank15);
            Globals_Server.fiefMasterList.Add(myFief7.fiefID, myFief7);

			// create QuickGraph undirected graph
			// 1. create graph
			var myHexMap = new HexMapGraph("map001");
            Globals_Server.gameMap = myHexMap;
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

			// add some goTo entries for myChar1
			//myGoTo1.Enqueue (myFief2);
			//myGoTo1.Enqueue (myFief7);
            
			// create entourages for PCs
			List<NonPlayerCharacter> myEmployees1 = new List<NonPlayerCharacter>();
			List<NonPlayerCharacter> myEmployees2 = new List<NonPlayerCharacter>();

			// create lists of fiefs owned by PCs and add some fiefs
			List<Fief> myFiefsOwned1 = new List<Fief>();
			List<Fief> myFiefsOwned2 = new List<Fief>();

            // create DOBs for characters
            Tuple<uint, byte> myDob001 = new Tuple<uint, byte>(1290, 1);
            Tuple<uint, byte> myDob002 = new Tuple<uint, byte>(1260, 0);
            Tuple<uint, byte> myDob003 = new Tuple<uint, byte>(1278, 2);
            Tuple<uint, byte> myDob004 = new Tuple<uint, byte>(1295, 3);
            Tuple<uint, byte> myDob005 = new Tuple<uint, byte>(1288, 2);
            Tuple<uint, byte> myDob006 = new Tuple<uint, byte>(1285, 3);
            Tuple<uint, byte> myDob007 = new Tuple<uint, byte>(1287, 2);

            // create titles list for characters
            List<String> myTitles001 = new List<string>();
            List<String> myTitles002 = new List<string>();
            List<String> myTitles003 = new List<string>();
            List<String> myTitles004 = new List<string>();
            List<String> myTitles005 = new List<string>();
            List<String> myTitles006 = new List<string>();
            List<String> myTitles007 = new List<string>();

            // create armies list for PCs
            List<Army> myArmies001 = new List<Army>();
            List<Army> myArmies002 = new List<Army>();

            // create sieges list for PCs
            List<string> mySieges001 = new List<string>();
            List<string> mySieges002 = new List<string>();

            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("101", "Dave", "Bond", myDob001, true, "E", true, 8.50, 9.0, myGoTo1, myLang1, 90, 0, 7.2, 6.1, generateSkillSet(), false, false, "101", "403", null, false, 13000, myEmployees1, myFiefsOwned1, "ESX02", "ESX02", myTitles001, myArmies001, mySieges001, loc: myFief1);
            Globals_Server.pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("102", "Bave", "Dond", myDob002, true, "F", true, 8.50, 6.0, myGoTo2, myLang1, 90, 0, 5.0, 4.5, generateSkillSet(), false, false, "102", null, null, false, 13000, myEmployees2, myFiefsOwned2, "ESR03", "ESR03", myTitles002, myArmies002, mySieges002, loc: myFief7);
            Globals_Server.pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("401", "Jimmy", "Servant", myDob003, true, "E", true, 8.50, 6.0, myGoTo3, myLang1, 90, 0, 3.3, 6.7, generateSkillSet(), false, false, null, null, null, 0, false, false, myTitles003, loc: myFief1);
            Globals_Server.npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("402", "Johnny", "Servant", myDob004, true, "E", true, 8.50, 6.0, myGoTo4, myLang1, 90, 0, 7.1, 5.2, generateSkillSet(), false, false, null, null, null, 10000, true, false, myTitles004, mb: myChar1.charID, loc: myFief1);
            Globals_Server.npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myNPC3 = new NonPlayerCharacter("403", "Harry", "Bailiff", myDob004, true, "F", true, 8.50, 6.0, myGoTo4, myLang1, 90, 0, 7.1, 5.2, generateSkillSet(), true, false, null, null, null, 10000, false, false, myTitles004, mb: myChar2.charID, loc: myFief6);
            Globals_Server.npcMasterList.Add(myNPC3.charID, myNPC3);
            NonPlayerCharacter myChar1Wife = new NonPlayerCharacter("404", "Bev", "Bond", myDob005, false, "E", true, 2.50, 9.0, myGoTo5, myLang3, 90, 0, 4.0, 6.0, generateSkillSet(), false, false, "101", "101", null, 30000, false, false, myTitles005, loc: myFief1);
            Globals_Server.npcMasterList.Add(myChar1Wife.charID, myChar1Wife);
            NonPlayerCharacter myChar2Son = new NonPlayerCharacter("405", "Horatio", "Dond", myDob006, true, "F", true, 8.50, 6.0, myGoTo6, myLang3, 90, 0, 7.1, 5.2, generateSkillSet(), true, false, "102", "406", "102", 10000, false, false, myTitles006, loc: myFief6);
            Globals_Server.npcMasterList.Add(myChar2Son.charID, myChar2Son);
            NonPlayerCharacter myChar2SonWife = new NonPlayerCharacter("406", "Mave", "Dond", myDob007, false, "E", true, 2.50, 9.0, myGoTo7, myLang3, 90, 0, 4.0, 6.0, generateSkillSet(), true, false, "102", "405", null, 30000, false, false, myTitles007, loc: myFief6);
            Globals_Server.npcMasterList.Add(myChar2SonWife.charID, myChar2SonWife);

            // create and add a scheduled birth
            string[] birthPersonae = new string[] { myChar1Wife.charID + "|mother", myChar1Wife.spouse + "|father" };
            JournalEvent myEvent = new JournalEvent(Globals_Server.getNextJournalEventID(), 1320, 1, birthPersonae, "birth");
            Globals_Server.scheduledEvents.events.Add(myEvent.jEventID, myEvent);

            // get character's correct days allowance
            myChar1.days = myChar1.getDaysAllowance();
            myChar2.days = myChar2.getDaysAllowance();
            myNPC1.days = myNPC1.getDaysAllowance();
            myNPC2.days = myNPC2.getDaysAllowance();
            myNPC3.days = myNPC3.getDaysAllowance();
            myChar1Wife.days = myChar1Wife.getDaysAllowance();
            myChar2Son.days = myChar2Son.getDaysAllowance();
            myChar2SonWife.days = myChar2SonWife.getDaysAllowance();

            // set fief owners
			myFief1.owner = myChar1;
			myFief2.owner = myChar1;
			myFief3.owner = myChar1;
			myFief4.owner = myChar1;
			myFief5.owner = myChar2;
			myFief6.owner = myChar2;
			myFief7.owner = myChar2;

            // set fief title holders
            myFief1.titleHolder = myChar1.charID;
            myFief2.titleHolder = myChar1.charID;
            myFief3.titleHolder = myChar1.charID;
            myFief4.titleHolder = myChar1.charID;
            myFief5.titleHolder = myChar2.charID;
            myFief6.titleHolder = myChar2.charID;
            myFief7.titleHolder = myChar2.charID;

            // add to myTitles lists
            myChar1.myTitles.Add(myFief1.fiefID);
            myChar1.myTitles.Add(myFief2.fiefID);
            myChar1.myTitles.Add(myFief3.fiefID);
            myChar1.myTitles.Add(myFief4.fiefID);
            myChar2.myTitles.Add(myFief5.fiefID);
            myChar2.myTitles.Add(myFief6.fiefID);
            myChar2.myTitles.Add(myFief7.fiefID);

            // set fief ancestral owners
			myFief1.ancestralOwner = myChar1;
			myFief2.ancestralOwner = myChar1;
			myFief3.ancestralOwner = myChar1;
			myFief4.ancestralOwner = myChar1;
			myFief5.ancestralOwner = myChar2;
			myFief6.ancestralOwner = myChar2;
			myFief7.ancestralOwner = myChar2;

			// set province overlords
			myProv.overlord = myChar1;
			myProv2.overlord = myChar2;

            // set kings
            myKingdom1.king = myChar1;
            myKingdom2.king = myChar2;

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
            // add wife to myNPCs
            myChar1.myNPCs.Add(myChar1Wife);
            // add NPC to employees/family
            myChar2.hireNPC(myNPC3, 10000);
            myChar2.myNPCs.Add(myChar2Son);
            myChar2.myNPCs.Add(myChar2SonWife);

			// Add fiefs to list of fiefs owned 
			myChar1.addToOwnedFiefs(myFief1);
			myChar1.addToOwnedFiefs(myFief3);
			myChar1.addToOwnedFiefs(myFief4);
			myChar2.addToOwnedFiefs(myFief6);
			myChar1.addToOwnedFiefs(myFief2);
			myChar2.addToOwnedFiefs(myFief5);
			myChar2.addToOwnedFiefs(myFief7);

			// add some characters to myFief1
			myFief1.addCharacter(myChar1);
			myFief1.addCharacter(myChar2);
			myFief1.addCharacter(myNPC1);
			myFief1.addCharacter(myNPC2);
            myFief6.addCharacter(myNPC3);
            myFief1.addCharacter(myChar1Wife);
            myFief6.addCharacter(myChar2Son);
            myFief6.addCharacter(myChar2SonWife);

            // create and add ailment
            Ailment myAilment1 = new Ailment(Globals_Server.getNextAilmentID(), "Battlefield injury", Globals_Server.clock.seasons[Globals_Server.clock.currentSeason] + ", " + Globals_Server.clock.currentYear, 3, 1);
            myChar1.ailments.Add(myAilment1.ailmentID, myAilment1);

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

            // create an army and add in appropriate places
            uint[] myArmyTroops = new uint[] {10, 10, 0, 100, 200, 400};
            Army myArmy = new Army(Globals_Server.getNextArmyID(), null, null, 90, null, trp: myArmyTroops);
            Globals_Server.armyMasterList.Add(myArmy.armyID, myArmy);
            myArmy.owner = myChar1.charID;
            myArmy.leader = myChar1.charID;
            myArmy.days = Globals_Server.pcMasterList[myArmy.leader].days;
            myChar1.myArmies.Add(myArmy);
            myChar1.armyID = myArmy.armyID;
            myArmy.location = Globals_Server.pcMasterList[myArmy.leader].location.fiefID;
            myChar1.location.armies.Add(myArmy.armyID);

            // create another (enemy) army and add in appropriate places
            uint[] myArmyTroops2 = new uint[] { 10, 10, 30, 0, 200, 400 };
            Army myArmy2 = new Army(Globals_Server.getNextArmyID(), null, null, 90, null, trp: myArmyTroops2, aggr: 1);
            Globals_Server.armyMasterList.Add(myArmy2.armyID, myArmy2);
            myArmy2.owner = myChar2.charID;
            myArmy2.leader = myChar2.charID;
            myArmy2.days = Globals_Server.pcMasterList[myArmy2.leader].days;
            myChar2.myArmies.Add(myArmy2);
            myChar2.armyID = myArmy2.armyID;
            myArmy2.location = Globals_Server.pcMasterList[myArmy2.leader].location.fiefID;
            myChar2.location.armies.Add(myArmy2.armyID);

            // bar a character from the myFief1 keep
			myFief2.barCharacter(myNPC1.charID);
            myFief2.barCharacter(myChar2.charID);
            myFief2.barCharacter(myChar1Wife.charID);

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
            Tuple<Skill, int>[] skillSet = new Tuple<Skill, int>[Globals_Server.myRand.Next(2, 4)];

            // populate array of skills with randomly chosen skills
            // 1) make temporary copy of skillKeys
            List<string> skillKeysCopy = new List<string>(Globals_Server.skillKeys);
            // 2) choose random skill, removing entry from keys list to ensure no duplication
            // Also assign random skill level
            for (int i = 0; i < skillSet.Length; i++)
            {
                // choose random skill
                int randSkill = Globals_Server.myRand.Next(0, skillKeysCopy.Count - 1);
                // assign random skill level
                int randSkillLevel = Globals_Server.myRand.Next(1, 10);
                // create Skill tuple
                skillSet[i] = new Tuple<Skill, int>(Globals_Server.skillMasterList[skillKeysCopy[randSkill]], randSkillLevel);
                skillKeysCopy.RemoveAt(randSkill);
            }

            return skillSet;

        }

        /// <summary>
		/// Writes all objects for a particular game to database
		/// </summary>
		/// <param name="gameID">ID of game (used for Riak bucket)</param>
		public void writeToDB(String gameID)
		{
			// ========= write CLOCK
            this.writeClock(gameID, Globals_Server.clock);

			// ========= write GLOBALS_SERVER DICTIONARIES
			this.writeDictionary(gameID, "combatValues", Globals_Server.combatValues);
			this.writeDictionary(gameID, "recruitRatios", Globals_Server.recruitRatios);
			this.writeDictionary(gameID, "battleProbabilities", Globals_Server.battleProbabilities);

            // ========= write JOURNALS
            this.writeJournal(gameID, "serverScheduledEvents", Globals_Server.scheduledEvents);
            this.writeJournal(gameID, "serverPastEvents", Globals_Server.pastEvents);
            this.writeJournal(gameID, "clientPastEvents", Globals_Client.myPastEvents);

            // ========= write SKILLS
            // clear existing key list
            if (Globals_Server.skillKeys.Count > 0)
			{
                Globals_Server.skillKeys.Clear();
			}

            // write each object in skillMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Skill> pair in Globals_Server.skillMasterList)
			{
				bool success = this.writeSkill (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.skillKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "skillKeys", Globals_Server.skillKeys);

			// ========= write LANGUAGES
            // clear existing key list
            if (Globals_Server.langKeys.Count > 0)
            {
                Globals_Server.langKeys.Clear();
            }

            // write each object in languageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Language> pair in Globals_Server.languageMasterList)
            {
                bool success = this.writeLanguage(gameID, pair.Value);
                if (success)
                {
                    Globals_Server.langKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "langKeys", Globals_Server.langKeys);

			// ========= write RANKS
            // clear existing key list
            if (Globals_Server.rankKeys.Count > 0)
            {
                Globals_Server.rankKeys.Clear();
            }

            // write each object in rankMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Rank> pair in Globals_Server.rankMasterList)
            {
                bool success = this.writeRank(gameID, pair.Value);
                if (success)
                {
                    Globals_Server.rankKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "rankKeys", Globals_Server.rankKeys);

			// ========= write NPCs
            // clear existing key list
            if (Globals_Server.npcKeys.Count > 0)
			{
                Globals_Server.npcKeys.Clear();
			}

            // write each object in npcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, NonPlayerCharacter> pair in Globals_Server.npcMasterList)
			{
				bool success = this.writeNPC (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.npcKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "npcKeys", Globals_Server.npcKeys);

			// ========= write PCs
            // clear existing key list
            if (Globals_Server.pcKeys.Count > 0)
			{
                Globals_Server.pcKeys.Clear();
			}

            // write each object in pcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, PlayerCharacter> pair in Globals_Server.pcMasterList)
			{
				bool success = this.writePC (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.pcKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "pcKeys", Globals_Server.pcKeys);

			// ========= write KINGDOMS
            // clear existing key list
            if (Globals_Server.kingKeys.Count > 0)
            {
                Globals_Server.kingKeys.Clear();
            }

            // write each object in kingdomMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Kingdom> pair in Globals_Server.kingdomMasterList)
            {
                bool success = this.writeKingdom(gameID, pair.Value);
                if (success)
                {
                    Globals_Server.kingKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "kingKeys", Globals_Server.kingKeys);

			// ========= write PROVINCES
            // clear existing key list
            if (Globals_Server.provKeys.Count > 0)
			{
                Globals_Server.provKeys.Clear();
			}

            // write each object in provinceMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Province> pair in Globals_Server.provinceMasterList)
			{
				bool success = this.writeProvince (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.provKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "provKeys", Globals_Server.provKeys);

			// ========= write TERRAINS
            // clear existing key list
            if (Globals_Server.terrKeys.Count > 0)
			{
                Globals_Server.terrKeys.Clear();
			}

            // write each object in terrainMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Terrain> pair in Globals_Server.terrainMasterList)
			{
				bool success = this.writeTerrain (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.terrKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "terrKeys", Globals_Server.terrKeys);

			// ========= write FIEFS
            // clear existing key list
            if (Globals_Server.fiefKeys.Count > 0)
			{
                Globals_Server.fiefKeys.Clear();
			}

            // write each object in fiefMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Fief> pair in Globals_Server.fiefMasterList)
			{
				bool success = this.writeFief (gameID, pair.Value);
				if (success)
				{
                    Globals_Server.fiefKeys.Add(pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "fiefKeys", Globals_Server.fiefKeys);

			// ========= write ARMIES
            // clear existing key list
            if (Globals_Server.armyKeys.Count > 0)
            {
                Globals_Server.armyKeys.Clear();
            }

            // write each object in armyMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Army> pair in Globals_Server.armyMasterList)
            {
                bool success = this.writeArmy(gameID, pair.Value);
                if (success)
                {
                    Globals_Server.armyKeys.Add(pair.Key);
                }
            }
				
			// write key list to database
			this.writeKeyList(gameID, "armyKeys", Globals_Server.armyKeys);

			// ========= write SIEGES
            // clear existing key list
            if (Globals_Server.siegeKeys.Count > 0)
            {
                Globals_Server.siegeKeys.Clear();
            }

            // write each object in siegeMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Siege> pair in Globals_Server.siegeMasterList)
            {
                bool success = this.writeSiege(gameID, pair.Value);
                if (success)
                {
                    Globals_Server.siegeKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "siegeKeys", Globals_Server.siegeKeys);

			// ========= write MAP (edges collection)
            this.writeMapEdges(gameID, Globals_Server.gameMap);

		}

		/// <summary>
		/// Loads all objects for a particular game from database
		/// </summary>
        /// <param name="gameID">ID of game (Riak bucket)</param>
		public void initialDBload(String gameID)
		{

            // ========= load KEY LISTS (to ensure efficient retrieval of specific game objects)
			this.initialDBload_keyLists (gameID);

            // ========= load CLOCK
            Globals_Server.clock = this.initialDBload_clock(gameID, "gameClock");

            // ========= load GLOBAL_SERVER DICTIONARIES
			Globals_Server.combatValues = this.initialDBload_dictUint(gameID, "combatValues");
			Globals_Server.recruitRatios = this.initialDBload_dictDouble(gameID, "recruitRatios");
			Globals_Server.battleProbabilities = this.initialDBload_dictDouble(gameID, "battleProbabilities");

            // ========= load JOURNALS
            Globals_Server.scheduledEvents = this.initialDBload_journal(gameID, "serverScheduledEvents");
            Globals_Server.pastEvents = this.initialDBload_journal(gameID, "serverPastEvents");
            Globals_Client.myPastEvents = this.initialDBload_journal(gameID, "clientPastEvents");

            // ========= load SKILLS
            foreach (String element in Globals_Server.skillKeys)
			{
				Skill skill = this.initialDBload_skill (gameID, element);
                // add Skill to skillMasterList
                Globals_Server.skillMasterList.Add(skill.skillID, skill);
			}

            // ========= load LANGUAGES
            foreach (String element in Globals_Server.langKeys)
            {
                Language lang = this.initialDBload_language(gameID, element);
                // add Language to languageMasterList
                Globals_Server.languageMasterList.Add(lang.languageID, lang);
            }

            // ========= load RANKS
            foreach (String element in Globals_Server.rankKeys)
            {
                Rank rank = this.initialDBload_rank(gameID, element);
                // add Rank to rankMasterList
                Globals_Server.rankMasterList.Add(rank.rankID, rank);
            }

            // ========= load SIEGES
            foreach (String element in Globals_Server.siegeKeys)
            {
                Siege s = this.initialDBload_Siege(gameID, element);
                // add Siege to siegeMasterList
                Globals_Server.siegeMasterList.Add(s.siegeID, s);
            }

            // ========= load ARMIES
            foreach (String element in Globals_Server.armyKeys)
            {
                Army a = this.initialDBload_Army(gameID, element);
                // add Army to armyMasterList
                Globals_Server.armyMasterList.Add(a.armyID, a);
            }

            // ========= load NPCs
            foreach (String element in Globals_Server.npcKeys)
			{
				NonPlayerCharacter npc = this.initialDBload_NPC (gameID, element);
                // add NPC to npcMasterList
                Globals_Server.npcMasterList.Add(npc.charID, npc);
			}

            // ========= load PCs
            foreach (String element in Globals_Server.pcKeys)
			{
				PlayerCharacter pc = this.initialDBload_PC (gameID, element);
                // add PC to pcMasterList
                Globals_Server.pcMasterList.Add(pc.charID, pc);
			}

            // ========= load KINGDOMS
            foreach (String element in Globals_Server.kingKeys)
            {
                Kingdom king = this.initialDBload_Kingdom(gameID, element);
                // add Kingdom to kingdomMasterList
                Globals_Server.kingdomMasterList.Add(king.kingdomID, king);
            }

            // ========= load PROVINCES
            foreach (String element in Globals_Server.provKeys)
			{
				Province prov = this.initialDBload_Province (gameID, element);
                // add Province to provinceMasterList
                Globals_Server.provinceMasterList.Add(prov.provinceID, prov);
			}

            // ========= load TERRAINS
            foreach (String element in Globals_Server.terrKeys)
			{
				Terrain terr = this.initialDBload_terrain (gameID, element);
                // add Terrain to terrainMasterList
                Globals_Server.terrainMasterList.Add(terr.terrainCode, terr);
			}

            // ========= load FIEFS
            foreach (String element in Globals_Server.fiefKeys)
			{
				Fief f = this.initialDBload_Fief (gameID, element);
                // add Fief to fiefMasterList
                Globals_Server.fiefMasterList.Add(f.fiefID, f);
			}

            // ========= process any CHARACTER goTo QUEUES containing entries
            if (Globals_Server.goToList.Count > 0)
			{
                for (int i = 0; i < Globals_Server.goToList.Count; i++)
				{
                    this.populate_goTo(Globals_Server.goToList[i]);
				}
                Globals_Server.goToList.Clear();
			}

            // ========= load MAP
            Globals_Server.gameMap = this.initialDBload_map(gameID, "mapEdges");
		}

		/// <summary>
		/// Loads all Riak key lists for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which key lists to be retrieved</param>
		public void initialDBload_keyLists(String gameID)
		{
            // populate skillKeys
			var skillKeyResult = rClient.Get(gameID, "skillKeys");
			if (skillKeyResult.IsSuccess)
			{
                Globals_Server.skillKeys = skillKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skillKeys from database.");
			}

			// populate langKeys
			var langKeyResult = rClient.Get(gameID, "langKeys");
			if (langKeyResult.IsSuccess)
			{
                Globals_Server.langKeys = langKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve langKeys from database.");
			}

            // populate rankKeys
            var rankKeyResult = rClient.Get(gameID, "rankKeys");
            if (rankKeyResult.IsSuccess)
            {
                Globals_Server.rankKeys = rankKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve rankKeys from database.");
            }

            // populate npcKeys
            var npcKeyResult = rClient.Get(gameID, "npcKeys");
			if (npcKeyResult.IsSuccess)
			{
                Globals_Server.npcKeys = npcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve npcKeys from database.");
			}

            // populate pcKeys
            var pcKeyResult = rClient.Get(gameID, "pcKeys");
			if (pcKeyResult.IsSuccess)
			{
                Globals_Server.pcKeys = pcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve pcKeys from database.");
			}

            // populate kingKeys
            var kingKeyResult = rClient.Get(gameID, "kingKeys");
            if (kingKeyResult.IsSuccess)
            {
                Globals_Server.kingKeys = kingKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve kingKeys from database.");
            }

            // populate provKeys
            var provKeyResult = rClient.Get(gameID, "provKeys");
			if (provKeyResult.IsSuccess)
			{
                Globals_Server.provKeys = provKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve provKeys from database.");
			}

            // populate terrKeys
            var terrKeyResult = rClient.Get(gameID, "terrKeys");
			if (terrKeyResult.IsSuccess)
			{
                Globals_Server.terrKeys = terrKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve terrKeys from database.");
			}

            // populate fiefKeys
            var fiefKeyResult = rClient.Get(gameID, "fiefKeys");
			if (fiefKeyResult.IsSuccess)
			{
                Globals_Server.fiefKeys = fiefKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve fiefKeys from database.");
			}

            // populate armyKeys
            var armyKeyResult = rClient.Get(gameID, "armyKeys");
            if (armyKeyResult.IsSuccess)
            {
                Globals_Server.armyKeys = armyKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve armyKeys from database.");
            }

            // populate siegeKeys
            var siegeKeyResult = rClient.Get(gameID, "siegeKeys");
            if (siegeKeyResult.IsSuccess)
            {
                Globals_Server.siegeKeys = siegeKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve siegeKeys from database.");
            }

        }

		/// <summary>
		/// Loads GameClock for a particular game from database
		/// </summary>
        /// <returns>GameClock object</returns>
        /// <param name="gameID">Game for which clock to be retrieved</param>
		/// <param name="clockID">ID of clock to be retrieved</param>
		public GameClock initialDBload_clock(String gameID, String clockID)
		{
			var clockResult = rClient.Get(gameID, clockID);
			var newClock = new GameClock();

			if (clockResult.IsSuccess)
			{
				newClock = clockResult.Value.GetObject<GameClock>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve GameClock " + clockID);
			}

			return newClock;
		}

        /// <summary>
        /// Loads a Journal from the database
        /// </summary>
        /// <returns>Journal object</returns>
        /// <param name="gameID">Game for which Journal to be retrieved</param>
        /// <param name="journalID">ID of Journal to be retrieved</param>
        public Journal initialDBload_journal(String gameID, String journalID)
        {
            var journalResult = rClient.Get(gameID, journalID);
            var newJournal = new Journal();

            if (journalResult.IsSuccess)
            {
                newJournal = journalResult.Value.GetObject<Journal>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Journal " + journalID);
            }

            return newJournal;
        }

        /// <summary>
		/// Loads Dictionary<string, uint[]> from database
		/// </summary>
		/// <returns>Dictionary<string, uint[]> object</returns>
		/// <param name="gameID">Game for which Dictionary to be retrieved</param>
		/// <param name="dictID">ID of Dictionary to be retrieved</param>
		public Dictionary<string, uint[]> initialDBload_dictUint(String gameID, String dictID)
		{
			var dictResult = rClient.Get(gameID, dictID);
			var newDict = new Dictionary<string, uint[]>();

			if (dictResult.IsSuccess)
			{
				newDict = dictResult.Value.GetObject<Dictionary<string, uint[]>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
			}

			return newDict;
		}

		/// <summary>
		/// Loads Dictionary<string, double[]> from database
		/// </summary>
		/// <returns>Dictionary<string, double[]> object</returns>
		/// <param name="gameID">Game for which Dictionary to be retrieved</param>
		/// <param name="dictID">ID of Dictionary to be retrieved</param>
		public Dictionary<string, double[]> initialDBload_dictDouble(String gameID, String dictID)
		{
			var dictResult = rClient.Get(gameID, dictID);
			var newDict = new Dictionary<string, double[]>();

			if (dictResult.IsSuccess)
			{
				newDict = dictResult.Value.GetObject<Dictionary<string, double[]>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Dictionary " + dictID);
			}

			return newDict;
		}

		/// <summary>
		/// Loads a skill for a particular game from database
		/// </summary>
        /// <returns>Skill object</returns>
        /// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="skillID">ID of skill to be retrieved</param>
		public Skill initialDBload_skill(String gameID, String skillID)
		{
			var skillResult = rClient.Get(gameID, skillID);
			var newSkill = new Skill();

			if (skillResult.IsSuccess)
			{
				newSkill = skillResult.Value.GetObject<Skill>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skill " + skillID);
			}

			return newSkill;
		}

		/// <summary>
		/// Loads an NPC for a particular game from database
		/// </summary>
        /// <returns>NonPlayerCharacter object</returns>
        /// <param name="gameID">Game for which NPC to be retrieved</param>
		/// <param name="npcID">ID of NPC to be retrieved</param>
		public NonPlayerCharacter initialDBload_NPC(String gameID, String npcID)
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
                    Globals_Server.goToList.Add(npcRiak);
				}
                // create NonPlayerCharacter from NonPlayerCharacter_Riak
                myNPC = this.NPCfromRiakNPC(npcRiak);
			}
			else
			{
				System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve NonPlayerCharacter " + npcID);
			}

			return myNPC;
		}

		/// <summary>
		/// Loads a PC for a particular game from database
		/// </summary>
        /// <returns>PlayerCharacter object</returns>
        /// <param name="gameID">Game for which PC to be retrieved</param>
		/// <param name="pcID">ID of PC to be retrieved</param>
		public PlayerCharacter initialDBload_PC(String gameID, String pcID)
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
                    Globals_Server.goToList.Add(pcRiak);
				}
                // create PlayerCharacter from PlayerCharacter_Riak
                myPC = this.PCfromRiakPC(pcRiak);
			}
			else
			{
				System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve PlayerCharacter " + pcID);
			}

			return myPC;
		}

        /// <summary>
        /// Loads a Kingdom for a particular game from database
        /// </summary>
        /// <returns>Kingdom object</returns>
        /// <param name="gameID">Game for which Kingdom to be retrieved</param>
        /// <param name="kingID">ID of Kingdom to be retrieved</param>
        public Kingdom initialDBload_Kingdom(String gameID, String kingID)
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
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Province " + kingID);
            }

            return myKing;
        }

        /// <summary>
		/// Loads a Province for a particular game from database
		/// </summary>
        /// <returns>Province object</returns>
        /// <param name="gameID">Game for which Province to be retrieved</param>
		/// <param name="provID">ID of Province to be retrieved</param>
		public Province initialDBload_Province(String gameID, String provID)
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
				System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve Province " + provID);
			}

			return myProv;
		}

		/// <summary>
		/// Loads a Terrain for a particular game from database
		/// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="gameID">Game for which Terrain to be retrieved</param>
		/// <param name="terrID">ID of Terrain to be retrieved</param>
		public Terrain initialDBload_terrain(String gameID, String terrID)
		{
			var terrainResult = rClient.Get(gameID, terrID);
			var newTerrain = new Terrain();

			if (terrainResult.IsSuccess)
			{
				newTerrain = terrainResult.Value.GetObject<Terrain>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Terrain " + terrID);
			}

			return newTerrain;
		}

        /// <summary>
        /// Loads a Language for a particular game from database
        /// </summary>
        /// <returns>Language object</returns>
        /// <param name="gameID">Game for which Language to be retrieved</param>
        /// <param name="rankID">ID of Language to be retrieved</param>
        public Language initialDBload_language(String gameID, String langID)
        {
            var languageResult = rClient.Get(gameID, langID);
            var newLanguage = new Language();

            if (languageResult.IsSuccess)
            {
                newLanguage = languageResult.Value.GetObject<Language>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Language " + langID);
            }

            return newLanguage;
        }

        /// <summary>
        /// Loads a Rank for a particular game from database
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="gameID">Game for which Rank to be retrieved</param>
        /// <param name="rankID">ID of Rank to be retrieved</param>
        public Rank initialDBload_rank(String gameID, String rankID)
        {
            var rankResult = rClient.Get(gameID, rankID);
            var newRank = new Rank();

            if (rankResult.IsSuccess)
            {
                newRank = rankResult.Value.GetObject<Rank>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Rank " + rankID);
            }

            return newRank;
        }

        /// <summary>
		/// Loads a Fief for a particular game from database
		/// </summary>
        /// <returns>Fief object</returns>
        /// <param name="gameID">Game for which Fief to be retrieved</param>
		/// <param name="fiefID">ID of Fief to be retrieved</param>
		public Fief initialDBload_Fief(String gameID, String fiefID)
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
				System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve Fief " + fiefID);
			}

			return myFief;
		}

        /// <summary>
        /// Loads an Army for a particular game from database
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="gameID">Game for which Army to be retrieved</param>
        /// <param name="armyID">ID of Army to be retrieved</param>
        public Army initialDBload_Army(String gameID, String armyID)
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
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Army " + armyID);
            }

            return myArmy;
        }

        /// <summary>
        /// Loads a Siege for a particular game from database
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="gameID">Game for which Siege to be retrieved</param>
        /// <param name="siegeID">ID of Siege to be retrieved</param>
        public Siege initialDBload_Siege(String gameID, String siegeID)
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
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve Siege " + siegeID);
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
		public HexMapGraph initialDBload_map(String gameID, String mapEdgesID)
		{
			var mapResult = rClient.Get(gameID, mapEdgesID);
			List<TaggedEdge<String, string>> edgesList = new List<TaggedEdge<string, string>>();
			var newMap = new HexMapGraph();

			if (mapResult.IsSuccess)
			{
				edgesList = mapResult.Value.GetObject<List<TaggedEdge<String, string>>>();
				TaggedEdge<Fief, string>[] edgesArray = this.EdgeCollection_from_Riak (edgesList);
                // create map from edges collection
				newMap = new HexMapGraph ("map001", edgesArray);
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve map edges " + mapEdgesID);
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
            fOut.province = Globals_Server.provinceMasterList[fr.province];

            // insert language
            fOut.language = new Tuple<Language, int>(Globals_Server.languageMasterList[fr.language.Item1], fr.language.Item2);

            // insert owner
            fOut.owner = Globals_Server.pcMasterList[fr.owner];
			// check if fief is in owner's list of fiefs owned
			bool fiefInList = fOut.owner.ownedFiefs.Any(item => item.fiefID == fOut.fiefID);
			// if not, add it
			if(! fiefInList)
			{
				fOut.owner.ownedFiefs.Add(fOut);
			}

			// insert ancestral owner
            fOut.ancestralOwner = Globals_Server.pcMasterList[fr.ancestralOwner];

			// insert bailiff (PC or NPC)
			if (fr.bailiff != null)
			{
                if (Globals_Server.npcMasterList.ContainsKey(fr.bailiff))
                {
                    fOut.bailiff = Globals_Server.npcMasterList[fr.bailiff];
                }
                else if (Globals_Server.pcMasterList.ContainsKey(fr.bailiff))
                {
                    fOut.bailiff = Globals_Server.pcMasterList[fr.bailiff];
				} else {
					fOut.bailiff = null;
					System.Windows.Forms.MessageBox.Show ("Unable to identify bailiff (" + fr.bailiff + ") for Fief " + fOut.fiefID);
				}
			}
				
			//insert terrain
            fOut.terrain = Globals_Server.terrainMasterList[fr.terrain];

			// insert characters
			if (fr.characters.Count > 0)
			{
				for (int i = 0; i < fr.characters.Count; i++)
				{
                    if (Globals_Server.npcMasterList.ContainsKey(fr.characters[i]))
					{
                        fOut.characters.Add(Globals_Server.npcMasterList[fr.characters[i]]);
                        Globals_Server.npcMasterList[fr.characters[i]].location = fOut;
					}
                    else if (Globals_Server.pcMasterList.ContainsKey(fr.characters[i]))
                    {
                        fOut.characters.Add(Globals_Server.pcMasterList[fr.characters[i]]);
                        Globals_Server.pcMasterList[fr.characters[i]].location = fOut;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Unable to identify character (" + fr.characters[i] + ") for Fief " + fOut.fiefID);
                    }

				}
			}

            // insert rank using rankID
            if (fr.rankID != null)
            {
                if (Globals_Server.rankMasterList.ContainsKey(fr.rankID))
                {
                    fOut.rank = Globals_Server.rankMasterList[fr.rankID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Fief " + fr.fiefID + ": Rank not found (" + fr.rankID + ")");
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
			PlayerCharacter_Riak pcOut = null;
			pcOut = new PlayerCharacter_Riak (pc);
			return pcOut;
		}

		/// <summary>
		/// Converts NonPlayerCharacter object (containing nested objects) into suitable format for JSON serialisation
		/// </summary>
        /// <returns>NonPlayerCharacter_Riak object</returns>
        /// <param name="npc">NonPlayerCharacter to be converted</param>
		public NonPlayerCharacter_Riak NPCtoRiak(NonPlayerCharacter npc)
		{
			NonPlayerCharacter_Riak npcOut = null;
			npcOut = new NonPlayerCharacter_Riak (npc);
			return npcOut;
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
            pcOut.language = new Tuple<Language, int>(Globals_Server.languageMasterList[pcr.language.Item1], pcr.language.Item2);

            // insert skills
			if (pcr.skills.Length > 0)
			{
				for (int i = 0; i < pcr.skills.Length; i++)
				{
                    pcOut.skills[i] = new Tuple<Skill, int>(Globals_Server.skillMasterList[pcr.skills[i].Item1], pcr.skills[i].Item2);
				}
			}

			// insert employees
			if (pcr.myNPCs.Count > 0)
			{
				for (int i = 0; i < pcr.myNPCs.Count; i++)
				{
                    pcOut.myNPCs.Add(Globals_Server.npcMasterList[pcr.myNPCs[i]]);
				}
			}

            // insert armies
            if (pcr.myArmies.Count > 0)
            {
                for (int i = 0; i < pcr.myArmies.Count; i++)
                {
                    pcOut.myArmies.Add(Globals_Server.armyMasterList[pcr.myArmies[i]]);
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
			npcOut = new NonPlayerCharacter (npcr);

            // insert language
            npcOut.language = new Tuple<Language, int>(Globals_Server.languageMasterList[npcr.language.Item1], npcr.language.Item2);
            
            // insert skills
			if (npcr.skills.Length > 0)
			{
				for (int i = 0; i < npcr.skills.Length; i++)
				{
                    npcOut.skills[i] = new Tuple<Skill, int>(Globals_Server.skillMasterList[npcr.skills[i].Item1], npcr.skills[i].Item2);
				}
			}

			return npcOut;
		}

		/// <summary>
		/// Converts HexMapGraph edges collection into suitable format for JSON serialisation
		/// </summary>
        /// <returns>'String-ified' edges collection</returns>
        /// <param name="edgesIn">Edges collection to be converted</param>
		public List<TaggedEdge<String, string>> EdgeCollection_to_Riak(List<TaggedEdge<Fief, string>> edgesIn)
		{
			List<TaggedEdge<String, string>> edgesOut = new List<TaggedEdge<string, string>> ();

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
		public TaggedEdge<Fief, string>[] EdgeCollection_from_Riak(List<TaggedEdge<String, string>> edgesIn)
		{
			TaggedEdge<Fief, string>[] edgesOut = new TaggedEdge<Fief, string>[edgesIn.Count];

			int i = 0;
			foreach (TaggedEdge<String, string> element in edgesIn)
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
		public TaggedEdge<String, string> EdgeFief_to_EdgeString(TaggedEdge<Fief, string> te)
		{
			TaggedEdge<String, string> edgeOut = new TaggedEdge<String, string>(te.Source.fiefID, te.Target.fiefID, te.Tag);
			return edgeOut;
		}

		/// <summary>
        /// Converts 'string-ified' edge into HexMapGraph edge
		/// </summary>
        /// <returns>HexMapGraph edge</returns>
        /// <param name="te">'String-ified' edge to be converted</param>
		public TaggedEdge<Fief, string> EdgeString_to_EdgeFief(TaggedEdge<String, string> te)
		{
            TaggedEdge<Fief, string> edgeOut = new TaggedEdge<Fief, string>(Globals_Server.fiefMasterList[te.Source], Globals_Server.fiefMasterList[te.Target], te.Tag);
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
                if (Globals_Server.pcMasterList.ContainsKey(cr.charID))
				{
                    myCh = Globals_Server.pcMasterList[cr.charID];
					success = true;
				}
			}
			else if (cr is NonPlayerCharacter_Riak)
			{
                if (Globals_Server.npcMasterList.ContainsKey(cr.charID))
				{
                    myCh = Globals_Server.npcMasterList[cr.charID];
					success = true;
				}
			}
            else
            {
                System.Windows.Forms.MessageBox.Show("goTo queue processing: Character not found (" + cr.charID + ")");
            }

            if (success)
            {
                foreach (String value in cr.goTo)
                {
                    myCh.goTo.Enqueue(Globals_Server.fiefMasterList[value]);
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
            Kingdom_Riak oOut = null;
            oOut = new Kingdom_Riak(k);
			return oOut;
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
            Kingdom oOut = null;
            oOut = new Kingdom(kr);

            // insert king
            if (kr.kingID != null)
            {
                if (Globals_Server.pcMasterList.ContainsKey(kr.kingID))
                {
                    oOut.king = Globals_Server.pcMasterList[kr.kingID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Kingdom " + kr.kingdomID + ": King not found (" + kr.kingID + ")");
                }
            }

            // insert rank
            if (kr.rankID != null)
            {
                if (Globals_Server.rankMasterList.ContainsKey(kr.rankID))
                {
                    oOut.rank = Globals_Server.rankMasterList[kr.rankID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Kingdom " + kr.kingdomID + ": Rank not found (" + kr.rankID + ")");
                }
            }

            return oOut;
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
			if (pr.overlordID != null)
			{
                if (Globals_Server.pcMasterList.ContainsKey(pr.overlordID))
                {
                    oOut.overlord = Globals_Server.pcMasterList[pr.overlordID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Province " + pr.provinceID + ": Overlord not found (" + pr.overlordID + ")");
                }
            }

            // insert kingdom using kingdomID
            if (pr.kingdomID != null)
            {
                if (Globals_Server.kingdomMasterList.ContainsKey(pr.kingdomID))
                {
                    oOut.kingdom = Globals_Server.kingdomMasterList[pr.kingdomID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Province " + pr.provinceID + ": Kingdom not found (" + pr.kingdomID + ")");
                }
            }

            // insert rank using rankID
            if (pr.rankID != null)
            {
                if (Globals_Server.rankMasterList.ContainsKey(pr.rankID))
                {
                    oOut.rank = Globals_Server.rankMasterList[pr.rankID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Province " + pr.provinceID + ": Rank not found (" + pr.rankID + ")");
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
		public bool writeKeyList(String gameID, String k, List<String> kl)
		{

			var rList = new RiakObject(gameID, k, kl);
			var putListResult = rClient.Put(rList);

			if (! putListResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Key list " + rList.Key + " to bucket " + rList.Bucket);
			}

			return putListResult.IsSuccess;
		}

		/// <summary>
		/// Writes GameClock object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="gc">GameClock to write</param>
		public bool writeClock(String gameID, GameClock gc)
		{
			var rClock = new RiakObject(gameID, "gameClock", gc);
			var putClockResult = rClient.Put(rClock);

			if (! putClockResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: GameClock to bucket " + rClock.Bucket);
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
        public bool writeJournal(String gameID, String key, Journal journal)
        {
            var rJournal = new RiakObject(gameID, key, journal);
            var putJournalResult = rClient.Put(rJournal);

            if (!putJournalResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Journal " + key + " to bucket " + rJournal.Bucket);
            }

            return putJournalResult.IsSuccess;
        }

        /// <summary>
		/// Writes Dictionary object to Riak
		/// </summary>
		/// <returns>bool indicating success</returns>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="key">Riak key to use</param>
		/// <param name="dictionary">Dictionary to write</param>
		public bool writeDictionary<T>(String gameID, String key, T dictionary)
		{
			var rDict = new RiakObject(gameID, key, dictionary);
			var putDictResult = rClient.Put(rDict);

			if (! putDictResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Dictionary " + key + " to bucket " + rDict.Bucket);
			}

			return putDictResult.IsSuccess;
		}

		/// <summary>
		/// Writes Skill object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="s">Skill to write</param>
		public bool writeSkill(String gameID, Skill s)
		{
			var rSkill = new RiakObject(gameID, s.skillID, s);
			var putSkillResult = rClient.Put(rSkill);

			if (! putSkillResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Skill " + rSkill.Key + " to bucket " + rSkill.Bucket);
			}

			return putSkillResult.IsSuccess;
		}

		/// <summary>
		/// Writes NonPlayerCharacter object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="npc">NonPlayerCharacter to write</param>
		public bool writeNPC(String gameID, NonPlayerCharacter npc)
		{
            // convert NonPlayerCharacter into NonPlayerCharacter_Riak
			NonPlayerCharacter_Riak riakNPC = this.NPCtoRiak (npc);

			var rNPC = new RiakObject(gameID, riakNPC.charID, riakNPC);
			var putNPCresult = rClient.Put(rNPC);

			if (! putNPCresult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: NPC " + rNPC.Key + " to bucket " + rNPC.Bucket);
			}

			return putNPCresult.IsSuccess;
		}

		/// <summary>
		/// Writes PlayerCharacter object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="pc">PlayerCharacter to write</param>
		public bool writePC(String gameID, PlayerCharacter pc)
		{
            // convert PlayerCharacter into PlayerCharacter_Riak
            PlayerCharacter_Riak riakPC = this.PCtoRiak(pc);

			var rPC = new RiakObject(gameID, riakPC.charID, riakPC);
			var putPCresult = rClient.Put(rPC);

			if (! putPCresult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: PC " + rPC.Key + " to bucket " + rPC.Bucket);
			}

			return putPCresult.IsSuccess;
		}

		/// <summary>
		/// Writes Kingdom object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">Kingdom to write</param>
        public bool writeKingdom(String gameID, Kingdom k)
		{
            // convert Kingdom into Kingdom_Riak
            Kingdom_Riak riakKing = this.KingdomToRiak(k);

			var rKing = new RiakObject(gameID, riakKing.kingdomID, riakKing);
			var putKingResult = rClient.Put(rKing);

			if (! putKingResult.IsSuccess)
			{
                System.Windows.Forms.MessageBox.Show("Write failed: Kingdom " + rKing.Key + " to bucket " + rKing.Bucket);
			}

			return putKingResult.IsSuccess;
		}

        /// <summary>
        /// Writes Province object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Province to write</param>
        public bool writeProvince(String gameID, Province p)
        {
            // convert Province into Province_Riak
            Province_Riak riakProv = this.ProvinceToRiak(p);

            var rProv = new RiakObject(gameID, riakProv.provinceID, riakProv);
            var putProvResult = rClient.Put(rProv);

            if (!putProvResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Province " + rProv.Key + " to bucket " + rProv.Bucket);
            }

            return putProvResult.IsSuccess;
        }

        /// <summary>
		/// Writes Language object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="l">Language to write</param>
        public bool writeLanguage(String gameID, Language l)
		{

			var rLanguage = new RiakObject(gameID, l.languageID, l);
			var putLanguageResult = rClient.Put(rLanguage);

			if (! putLanguageResult.IsSuccess)
			{
                System.Windows.Forms.MessageBox.Show("Write failed: Language " + rLanguage.Key + " to bucket " + rLanguage.Bucket);
			}

			return putLanguageResult.IsSuccess;
		}

        /// <summary>
        /// Writes Rank object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="r">Rank to write</param>
        public bool writeRank(String gameID, Rank r)
        {

            var rRank = new RiakObject(gameID, r.rankID, r);
            var putRankResult = rClient.Put(rRank);

            if (!putRankResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Rank " + rRank.Key + " to bucket " + rRank.Bucket);
            }

            return putRankResult.IsSuccess;
        }

        /// <summary>
        /// Writes Terrain object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="t">Terrain to write</param>
        public bool writeTerrain(String gameID, Terrain t)
        {

            var rTerrain = new RiakObject(gameID, t.terrainCode, t);
            var putTerrainResult = rClient.Put(rTerrain);

            if (!putTerrainResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Terrain " + rTerrain.Key + " to bucket " + rTerrain.Bucket);
            }

            return putTerrainResult.IsSuccess;
        }

        /// <summary>
		/// Writes Fief object to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="f">Fief to write</param>
		public bool writeFief(String gameID, Fief f)
		{
            // convert Fief into Fief_Riak
            Fief_Riak riakFief = this.FieftoRiak(f);

			var rFief = new RiakObject(gameID, riakFief.fiefID, riakFief);
			var putFiefResult = rClient.Put(rFief);

			if (! putFiefResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Fief " + rFief.Key + " to bucket " + rFief.Bucket);
			}

			return putFiefResult.IsSuccess;
		}

        /// <summary>
        /// Writes Army object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="a">Army to write</param>
        public bool writeArmy(String gameID, Army a)
        {
            var rArmy = new RiakObject(gameID, a.armyID, a);
            var putArmyResult = rClient.Put(rArmy);

            if (!putArmyResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Army " + rArmy.Key + " to bucket " + rArmy.Bucket);
            }

            return putArmyResult.IsSuccess;
        }

        /// <summary>
        /// Writes Siege object to Riak
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="s">Siege to write</param>
        public bool writeSiege(String gameID, Siege s)
        {
            var rSiege = new RiakObject(gameID, s.siegeID, s);
            var putSiegeResult = rClient.Put(rSiege);

            if (!putSiegeResult.IsSuccess)
            {
                System.Windows.Forms.MessageBox.Show("Write failed: Siege " + rSiege.Key + " to bucket " + rSiege.Bucket);
            }

            return putSiegeResult.IsSuccess;
        }

        /// <summary>
		/// Writes HexMapGraph edges collection to Riak
		/// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="map">HexMapGraph containing edges collection</param>
		public bool writeMapEdges(String gameID, HexMapGraph map)
		{
            // extract edges collection from HexMapGraph
			List<TaggedEdge<String, string>> riakMapEdges = this.EdgeCollection_to_Riak (map.myMap.Edges.ToList());

			var rMapE = new RiakObject(gameID, "mapEdges", riakMapEdges);
			var putMapResultE = rClient.Put(rMapE);

			if (! putMapResultE.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Map edges collection " + rMapE.Key + " to bucket " + rMapE.Bucket);
			}

			return putMapResultE.IsSuccess;
		}

        /// <summary>
		/// Updates game objects at end/start of season
		/// </summary>
        /// <param name="type">Specifies type of update to perform</param>
        public void seasonUpdate(String type = "full")
		{
            // used to check if character update is necessary
            bool performCharacterUpdate = true;

            // FIEFS
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Server.fiefMasterList)
            {
                fiefEntry.Value.updateFief();
            }

            // NONPLAYERCHARACTERS
            foreach (KeyValuePair<string, NonPlayerCharacter> npcEntry in Globals_Server.npcMasterList)
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
                            this.randomMoveNPC(npcEntry.Value, true);
                        }

                        // finish previously started multi-hex move if necessary
                        if (npcEntry.Value.goTo.Count > 0)
                        {
                            this.characterMultiMove(npcEntry.Value, true);
                        }
                    }
                }

            }

            // iterate through clock's scheduled events
            // check for births
            foreach (KeyValuePair<string, JournalEvent> jEvent in Globals_Server.scheduledEvents.events)
            {
                if ((jEvent.Value.year == Globals_Server.clock.currentYear) && (jEvent.Value.season == Globals_Server.clock.currentSeason))
                {
                    if ((jEvent.Value.type).ToLower().Equals("birth"))
                    {
                        // get parents
                        NonPlayerCharacter mummy = null;
                        Character daddy = Globals_Server.pcMasterList[mummy.spouse];
                        for (int i = 0; i < jEvent.Value.personae.Length; i++ )
                        {
                            string thisPersonae = jEvent.Value.personae[i];
                            string[] thisPersonaeSplit = thisPersonae.Split('|');
                            if (thisPersonaeSplit[1].Equals("mother"))
                            {
                                mummy = Globals_Server.npcMasterList[thisPersonaeSplit[0]];
                                daddy = Globals_Server.pcMasterList[mummy.spouse];
                                break;
                            }
                        }

                        // run childbirth procedure
                        this.giveBirth(mummy, daddy);
                    }
                }
            }

            // PLAYERCHARACTERS
            foreach (KeyValuePair<string, PlayerCharacter> pcEntry in Globals_Server.pcMasterList)
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
                            this.characterMultiMove(pcEntry.Value, true);
                        }
                    }
                }
            }

            // ARMIES

            // keep track of any armies requiring removal (if hav fallen below 100 men)
            List<Army> disbandedArmies = new List<Army>();
            bool hasDissolved = false;

            // iterate through armies
            foreach (KeyValuePair<string, Army> armyEntry in Globals_Server.armyMasterList)
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
            foreach (KeyValuePair<string, Siege> siegeEntry in Globals_Server.siegeMasterList)
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
                    // dismantle siege
                    this.siegeEnd(dissolvedSieges[i]);
                }

                // clear dissolvedSieges
                dissolvedSieges.Clear();
            }

            // ADVANCE SEASON AND YEAR
            Globals_Server.clock.advanceSeason();

            // REFRESH CURRENT SCREEN
            this.refreshCurrentScreen();
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

            // meeting place
            else if (Globals_Client.containerToView == this.meetingPlaceContainer)
            {
                if ((this.meetingPlaceLabel.Text).ToLower().Contains("tavern"))
                {
                    this.refreshMeetingPlaceDisplay("tavern");
                }
                else if ((this.meetingPlaceLabel.Text).ToLower().Contains("outside"))
                {
                    this.refreshMeetingPlaceDisplay("outsidekeep");
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
        }

        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to move</param>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="isUpdate">Indicates if is update mode</param>
        public bool moveCharacter(Character ch, Fief target, double cost, bool isUpdate = false)
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
                        System.Windows.Forms.MessageBox.Show("Move cancelled.");
                        proceedWithMove = false;
                    }

                    // if choose to proceed
                    else
                    {
                        // end the siege
                        Siege thisSiege = Globals_Server.siegeMasterList[thisSiegeID];
                        System.Windows.Forms.MessageBox.Show("Siege (" + thisSiegeID + ") ended.");
                        this.siegeEnd(thisSiege);
                    }

                }
            }

            if (proceedWithMove)
            {
                // move character
                success = ch.moveCharacter(target, cost, isUpdate);
            }

            return success;
        }
        
        /// <summary>
        /// Moves an NPC without a boss one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="npc">NPC to move</param>
        /// <param name="isUpdate">Indicates if is update mode</param>
        public bool randomMoveNPC(NonPlayerCharacter npc, bool isUpdate = false)
        {
            bool success = false;

            // generate random int 0-6 to see if moves
            int randomInt = Globals_Server.myRand.Next(7);

            if (randomInt > 0)
            {
                // get a destination
                Fief target = Globals_Server.gameMap.chooseRandomHex(npc.location);

                // get travel cost
                double travelCost = this.getTravelCost(npc.location, target);

                // perform move
                success = this.moveCharacter(npc, target, travelCost, isUpdate);
            }

            return success;
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
        /// Refreshes display of PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void refreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            ListViewItem[] fiefsOwned = new ListViewItem[Globals_Client.myChar.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < Globals_Client.myChar.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                fiefsOwned[i] = new ListViewItem(Globals_Client.myChar.ownedFiefs[i].name);
                fiefsOwned[i].SubItems.Add(Globals_Client.myChar.ownedFiefs[i].fiefID);
                // indicate if fief is current location
                if (Globals_Client.myChar.ownedFiefs[i] == Globals_Client.myChar.location)
                {
                    fiefsOwned[i].SubItems.Add("You are here");
                }
                // add item to fiefsListView
                this.fiefsListView.Items.Add(fiefsOwned[i]);
            }
        }

        /// <summary>
        /// Refreshes UI Court, Tavern, outside keep display
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
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
            this.meetingPlaceMoveToTextBox.Enabled = false;
            this.meetingPlaceMoveToTextBox.Text = "";
            this.meetingPlaceRouteBtn.Enabled = false;
            this.meetingPlaceRouteTextBox.Enabled = false;
            this.meetingPlaceRouteTextBox.Text = "";
            this.meetingPlaceEntourageBtn.Enabled = false;

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
            textToDisplay += Globals_Server.clock.seasons[Globals_Server.clock.currentSeason] + ", " + Globals_Server.clock.currentYear + ".  Your days left: " + Globals_Client.myChar.days + "\r\n\r\n";
            // Fief name/ID and province name
            textToDisplay += "Fief: " + Globals_Client.myChar.location.name + " (" + Globals_Client.myChar.location.fiefID + ")  in " + Globals_Client.myChar.location.province.name + ", " + Globals_Client.myChar.location.province.kingdom.name + "\r\n\r\n";
            // Fief owner
            textToDisplay += "Owner: " + Globals_Client.myChar.location.owner.firstName + " " + Globals_Client.myChar.location.owner.familyName + "\r\n";
            // Fief overlord
            textToDisplay += "Overlord: " + Globals_Client.myChar.location.getOverlord().firstName + " " + Globals_Client.myChar.location.getOverlord().familyName + "\r\n";

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
            for (int i = 0; i < Globals_Client.myChar.location.characters.Count; i++)
            {
                ListViewItem charsInCourt = null;

                // only display characters in relevant location (in keep, or not)
                if (Globals_Client.myChar.location.characters[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (Globals_Client.myChar.location.characters[i] != Globals_Client.myChar)
                    {

                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (Globals_Client.myChar.location.characters[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((Globals_Client.myChar.location.characters[i] as NonPlayerCharacter).wage == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myChar.location.characters[i]);
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myChar.location.characters[i]);
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

                if (ch.familyID.Equals(Globals_Client.myChar.charID))
                {
                    isFamily = true;
                }
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
                if (((ch as NonPlayerCharacter).myBoss != null) && (ch as NonPlayerCharacter).myBoss.Equals(Globals_Client.myChar.charID))
                {
                    isEmployee = true;
                }

                // allocate NPC type
                if ((isFamily) && (isEmployee))
                {
                    myType = "Family & Employee";
                }
                else if (isFamily)
                {
                    myType = "Family";
                }
                else if (isEmployee)
                {
                    myType = "Employee";
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
                for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
                {
                    if (Globals_Client.myChar.myNPCs[i] == ch)
                    {
                        if (Globals_Client.myChar.myNPCs[i].inEntourage)
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

            // ensure unable to use controls whilst no NPC selected in ListView
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
            
            // clear existing items in characters list
            this.houseCharListView.Items.Clear();

            // variables needed for name check (to see if NPC needs naming)
            String nameWarning = "The following offspring need to be named:\r\n\r\n";
            bool showNameWarning = false;

            // iterates through household characters adding information to ListView
            // and checking if naming is required
            for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
            {
                ListViewItem houseChar = null;

                // name
                houseChar = new ListViewItem(Globals_Client.myChar.myNPCs[i].firstName + " " + Globals_Client.myChar.myNPCs[i].familyName);

                // charID
                houseChar.SubItems.Add(Globals_Client.myChar.myNPCs[i].charID);

                // Function (i.e. employee's job, family member's role)
                houseChar.SubItems.Add(Globals_Client.myChar.myNPCs[i].getFunction(Globals_Client.myChar));

                // location
                houseChar.SubItems.Add(Globals_Client.myChar.myNPCs[i].location.fiefID + " (" + Globals_Client.myChar.myNPCs[i].location.name + ")");

                // show whether is in player's entourage
                if (Globals_Client.myChar.myNPCs[i].inEntourage)
                {
                    houseChar.SubItems.Add("Yes");
                }

                // check if needs to be named
                if (Globals_Client.myChar.myNPCs[i].familyID != null)
                {
                    bool nameRequired = Globals_Client.myChar.myNPCs[i].checkForName(0);

                    if (nameRequired)
                    {
                        showNameWarning = true;
                        nameWarning += " - " + Globals_Client.myChar.myNPCs[i].charID + " (" + Globals_Client.myChar.myNPCs[i].firstName + " " + Globals_Client.myChar.myNPCs[i].familyName + ")\r\n";
                    }
                }

                if (houseChar != null)
                {
                    // if NPC passed in as parameter, show as selected
                    if (Globals_Client.myChar.myNPCs[i] == npc)
                    {
                        houseChar.Selected = true;
                    }

                    // add item to fiefsListView
                    this.houseCharListView.Items.Add(houseChar);
                }

            }

            this.houseCharListView.HideSelection = false;
            this.houseCharListView.Focus();

            if (showNameWarning)
            {
                nameWarning += "\r\nAny children who are not named by the age of one will be named after his highness the king.";
                System.Windows.Forms.MessageBox.Show(nameWarning);
            }
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
            if (Globals_Client.myChar.myNPCs.Contains((ch as NonPlayerCharacter)) || (ch == Globals_Client.myChar))
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

            // ID
            charText += "ID: " + ch.charID + "\r\n";

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
            charText += "Nationality: " + ch.nationality + "\r\n";

            if (ch is PlayerCharacter)
            {
                // home fief
                Fief homeFief = (ch as PlayerCharacter).getHomeFief();
                charText += "Home fief: " + homeFief.name + " (" + homeFief.fiefID + ")\r\n";

                // ancestral home fief
                Fief ancHomeFief = (ch as PlayerCharacter).getAncestralHome();
                charText += "Ancestral Home fief: " + ancHomeFief.name + " (" + ancHomeFief.fiefID + ")\r\n";
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
            charText += "Language: " + ch.language.Item1.name + " (dialect " + ch.language.Item2 + ")\r\n";

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
                charText += "single and lonely";
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
                    NonPlayerCharacter thisSpouse = Globals_Server.npcMasterList[ch.spouse];
                    if (thisSpouse.isPregnant)
                    {
                        charText += "Your spouse is pregnant (congratulations!)\r\n";
                    }
                    else
                    {
                        charText += "Your spouse is not pregnant (try harder!)\r\n";
                    }
                }
            }

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

            // if titles are to be shown
            if (this.characterTitlesCheckBox.Checked)
            {
                pcText += "\r\n\r\n------------------ TITLES ------------------\r\n\r\n";

                // check kingdoms
                foreach (KeyValuePair<string, Kingdom> entry in Globals_Server.kingdomMasterList)
                {
                    // if PC is king
                    if (entry.Value.king.charID.Equals(pc.charID))
                    {
                        // get correct title
                        for (int i = 0; i < entry.Value.rank.title.Length; i++)
                        {
                            if (entry.Value.rank.title[i].Item1 == pc.language.Item1.languageID)
                            {
                                pcText += entry.Value.rank.title[i].Item2 + " (rank " + entry.Value.rank.rankID + ") of ";
                                break;
                            }
                        }
                        // get kingdom details
                        pcText += entry.Value.name + " (" + entry.Value.kingdomID + ")\r\n";
                    }
                }

                // check provinces
                foreach (KeyValuePair<string, Province> entry in Globals_Server.provinceMasterList)
                {
                    // if PC is overlord
                    if (entry.Value.overlord.charID.Equals(pc.charID))
                    {
                        // get correct title
                        for (int i = 0; i < entry.Value.rank.title.Length; i++)
                        {
                            if (entry.Value.rank.title[i].Item1 == pc.language.Item1.languageID)
                            {
                                pcText += entry.Value.rank.title[i].Item2 + " (rank " + entry.Value.rank.rankID + ") of ";
                                break;
                            }                        
                        }
                        // get province details
                        pcText += entry.Value.name + " (" + entry.Value.provinceID + ")\r\n";
                    }
                }

                // fiefs
                for (int i = 0; i < pc.myTitles.Count; i++ )
                {
                    // get correct title
                    for (int ii = 0; ii < Globals_Server.fiefMasterList[pc.myTitles[i]].rank.title.Length; ii++)
                    {
                        if (Globals_Server.fiefMasterList[pc.myTitles[i]].rank.title[ii].Item1 == pc.language.Item1.languageID)
                        {
                            pcText += Globals_Server.fiefMasterList[pc.myTitles[i]].rank.title[ii].Item2 + " (rank " + Globals_Server.fiefMasterList[pc.myTitles[i]].rank.rankID + ") of ";
                            break;
                        }
                    }
                    // get fief details
                    pcText += Globals_Server.fiefMasterList[pc.myTitles[i]].name + " (" + pc.myTitles[i] + ")\r\n";
                }
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

            // estimated salary level
            npcText += "Potential salary: " + npc.calcWage(Globals_Client.myChar) + "\r\n";

            // most recent salary offer from player (if any)
            npcText += "Last offer from this PC: ";
            if (npc.lastOffer.ContainsKey(Globals_Client.myChar.charID))
            {
                npcText += npc.lastOffer[Globals_Client.myChar.charID];
            }
            else
            {
                npcText += "N/A";
            }
            npcText += "\r\n";

            // current salary
            npcText += "Current salary: " + npc.wage + "\r\n";

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
            Fief armyLocation = Globals_Server.fiefMasterList[a.location];

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

            // days left
            armyText += "Days left: " + a.days + "\r\n\r\n";

            // location
            armyText += "Location: " + armyLocation.name + " (Province: " + armyLocation.province.name + ".  Kingdom: " + armyLocation.province.kingdom.name + ")\r\n\r\n";

            // leader
            Character armyLeader = a.getLeader();

            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!  You should appoint one as soon as possible.\r\n\r\n";
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
            bool isDefender = (fiefOwner == Globals_Client.myChar);
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
            siegeText += "Start date: " + s.startYear + ", " + Globals_Server.clock.seasons[s.startSeason] + "\r\n\r\n";

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

                siegeText += "\r\n\r\nTotal casualties so far: " + s.totalCasualtiesDefender;
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

                siegeText += "\r\n\r\nTotal casualties so far: " + s.totalCasualtiesAttacker;
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
                double keepLvl = this.calcStormKeepLevel(s);
                double successChance = this.calcStormSuccess(keepLvl) / 2;
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
            fiefText += "ID: " + f.fiefID + "\r\n";

            // name (& province name)
            fiefText += "Name: " + f.name + " (Province: " + f.province.name + ".  Kingdom: " + f.province.kingdom.name + ")\r\n";

            // rank
            fiefText += "Title (rank): ";
            for (int i = 0; i < f.rank.title.Length; i++ )
            {
                if (f.rank.title[i].Item1 == f.language.Item1.languageID)
                {
                    fiefText += f.rank.title[i].Item2 + " (" + f.rank.rankID + ")\r\n";
                    break;
                }
            }

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
            fiefText += "Language: " + f.language.Item1.name + " (dialect " + f.language.Item2 + ")\r\n";

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
                if ((f.owner.spouse != null) && (Globals_Server.npcMasterList[f.owner.spouse].management > f.owner.management))
                {
                    fiefText += "  (which may include a famExpense skills modifier: " + Globals_Server.npcMasterList[f.owner.spouse].calcSkillEffect("famExpense") + ")";
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
                fiefText += "   (tax rate: " + f.province.overlordTaxRate + "%)\r\n\r\n";

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
                ch = Globals_Client.myChar;
            }

            // refresh Character display TextBox
            this.characterTextBox.ReadOnly = true;
            this.characterTextBox.Text = this.displayCharacter(ch);

            // clear previous entries in Camp TextBox
            this.travelCampDaysTextBox.Text = "";

            // multimove button only enabled if is player or an employee
            if (ch != Globals_Client.myChar)
			{
                if (!Globals_Client.myChar.myNPCs.Contains(ch))
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
            Globals_Client.charToView = Globals_Client.myChar;
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
            
            // clear existing items in armies list
            this.armyListView.Items.Clear();

            // iterates through player's armies adding information to ListView
            for (int i = 0; i < Globals_Client.myChar.myArmies.Count; i++)
            {
                ListViewItem thisArmy = null;

                // armyID
                thisArmy = new ListViewItem(Globals_Client.myChar.myArmies[i].armyID);

                // leader
                Character armyLeader = Globals_Client.myChar.myArmies[i].getLeader();
                thisArmy.SubItems.Add(armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")");

                // location
                Fief armyLocation = Globals_Server.fiefMasterList[Globals_Client.myChar.myArmies[i].location];
                thisArmy.SubItems.Add(armyLocation.name + " (" + armyLocation.fiefID + ")");

                // size
                thisArmy.SubItems.Add(Globals_Client.myChar.myArmies[i].calcArmySize().ToString());

                if (thisArmy != null)
                {
                    // if army passed in as parameter, show as selected
                    if (Globals_Client.myChar.myArmies[i] == a)
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
                if (Globals_Client.myChar.armyID == null)
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
            for (int i = 0; i < Globals_Client.myChar.mySieges.Count; i++)
            {
                ListViewItem thisSiegeItem = null;
                Siege thisSiege = Globals_Client.myChar.getSiege(Globals_Client.myChar.mySieges[i]);

                // armyID
                thisSiegeItem = new ListViewItem(thisSiege.siegeID);

                // fief
                Fief siegeLocation = thisSiege.getFief();
                thisSiegeItem.SubItems.Add(siegeLocation.name + " (" + siegeLocation.fiefID + ")");

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
                f = Globals_Client.myChar.location;
            }

            Globals_Client.fiefToView = f;

            bool isOwner = Globals_Client.myChar.ownedFiefs.Contains(Globals_Client.fiefToView);
            bool displayWarning = false;
            String toDisplay = "";

            // set name label text
            this.fiefLabel.Text = Globals_Client.fiefToView.name + " (" + Globals_Client.fiefToView.fiefID + ")";
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

            // disable controls by default (will be enabled further down if appropriate)
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
                int homeTreasury = 0;
                int fiefTreasury = 0;

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
                    this.viewBailiffBtn.Enabled = true;
                    this.lockoutBtn.Enabled = true;
                    this.selfBailiffBtn.Enabled = true;
                    this.setBailiffBtn.Enabled = true;
                    this.removeBaliffBtn.Enabled = true;
                    this.fiefHomeTreasTextBox.Enabled = true;
                    this.fiefHomeTreasTextBox.ReadOnly = true;
                    this.FiefTreasTextBox.ReadOnly = true;

                    // don't enable treasury transfer controls if in Home Fief (can't transfer to self)
                    if (f == Globals_Client.myChar.getHomeFief())
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

                    // get home fief
                    Fief home = Globals_Client.myChar.getHomeFief();

                    // check if in home fief
                    if (f == home)
                    {
                        // don't show fief treasury
                        this.FiefTreasTextBox.Text = "";

                        // display home treasury
                        this.fiefHomeTreasTextBox.Text = home.getAvailableTreasury().ToString();
                    }
                    else
                    {
                        // display fief treasury
                        this.FiefTreasTextBox.Text = f.getAvailableTreasury().ToString();

                        // display home treasury
                        this.fiefHomeTreasTextBox.Text = home.getAvailableTreasury(true).ToString();
                    }

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
                System.Windows.Forms.MessageBox.Show(toDisplay, "WARNING: EXPENDITURE TOO HIGH");
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
            this.refreshFiefContainer(Globals_Client.myChar.location);
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
                    String toDisplay = "Your spending exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                    toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                    System.Windows.Forms.MessageBox.Show(toDisplay, "TRANSACTION CANCELLED");
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
                    String toDisplay = "";
                    if (spendChanged)
                    {
                        toDisplay += "Expenditure adjusted";
                    }
                    else
                    {
                        toDisplay += "Expenditure unchanged";
                    }

                    System.Windows.Forms.MessageBox.Show(toDisplay);
                }
            }
            catch (System.FormatException fe)
            {
                System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
            }
            catch (System.OverflowException ofe)
            {
                System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
            }
            finally
            {
                // refresh screen if expenditure changed
                if (spendChanged)
                {
                    // refresh display
                    this.refreshFiefContainer(Globals_Client.fiefToView);
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
            Globals_Client.charToView = Globals_Client.myChar;

            // refresh navigation data
            Globals_Client.fiefToView = Globals_Client.myChar.location;
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
        private double getTravelCost(Fief source, Fief target, String armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (source.terrain.travelCost + target.terrain.travelCost) / 2;

            // apply season modifier
            cost = cost * Globals_Server.clock.calcSeasonTravMod();

            // if necessary, apply army modifier
            if (armyID != null)
            {
                cost = cost * Globals_Server.armyMasterList[armyID].calcMovementModifier();
            }

            return cost;
        }

        /// <summary>
        /// Refreshes travel display screen
        /// </summary>
        private void refreshTravelContainer()
        {
            // get current fief
            Fief thisFief = Globals_Client.myChar.location;

            // string[] to hold direction text
            string[] directions = new string[] { "NE", "E", "SE", "SW", "W", "NW" };
            // Button[] to hold corresponding travel buttons
            Button[] travelBtns = new Button[] { travel_NE_btn, travel_E_btn, travel_SE_btn, travel_SW_btn, travel_W_btn, travel_NW_btn };

            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + thisFief.name + " (" + Globals_Client.myChar.location.fiefID + ")" + "\r\n" + Globals_Client.myChar.location.province.name + ", " + Globals_Client.myChar.location.province.kingdom.name;

            for (int i = 0; i < directions.Length; i++ )
            {
                // retrieve target fief for that direction
                Fief target = Globals_Server.gameMap.getFief(thisFief, directions[i]);
                // display fief details and travel cost
                if (target != null)
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\n";
                    travelBtns[i].Text += target.name + " (" + target.fiefID + ")\r\n";
                    travelBtns[i].Text += target.province.name + ", " + target.province.kingdom.name + "\r\n\r\n";
                    travelBtns[i].Text += "Cost: " + this.getTravelCost(thisFief, target);
                }
                else
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\nNo fief present";
                }
            }

            // set text for informational labels
            this.travelLocationLabel.Text = "You are here: " + thisFief.name + " (" + thisFief.fiefID + ")";
            this.travelDaysLabel.Text = "Your remaining days: " + Globals_Client.myChar.days;
            
            // set text for 'enter/exit keep' button, depending on whether player in/out of keep
            if (Globals_Client.myChar.inKeep)
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
                if (Globals_Client.myChar.inKeep)
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
            Fief targetFief = Globals_Server.gameMap.getFief(Globals_Client.myChar.location, button.Tag.ToString());

            if (targetFief != null)
            {
                // get travel cost
                double travelCost = this.getTravelCost(Globals_Client.myChar.location, targetFief, Globals_Client.myChar.armyID);
                // attempt to move player to target fief
                success = this.moveCharacter(Globals_Client.myChar, targetFief, travelCost);
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
                if (Globals_Client.fiefToView.bailiff == Globals_Client.myChar)
                {
                    Globals_Client.charToView = Globals_Client.myChar;
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
                System.Windows.Forms.MessageBox.Show("This fief currently has no bailiff.");
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
            for (int i = 0; i < Globals_Client.myChar.ownedFiefs.Count; i++)
            {
                if (Globals_Client.myChar.ownedFiefs[i].fiefID.Equals(this.fiefsListView.SelectedItems[0].SubItems[1].Text))
                {
                    fiefToDisplay = Globals_Client.myChar.ownedFiefs[i];
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
            if (Globals_Client.myChar.inKeep)
            {
                // exit keep
                Globals_Client.myChar.exitKeep();
                // change button text
                this.enterKeepBtn.Text = "Enter Keep";
                // refresh display
                this.refreshTravelContainer();
            }
            // if player not in keep
            else
            {
                // attempt to enter keep
                bool entered = Globals_Client.myChar.enterKeep();
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
            String place = "court";

            bool enteredKeep = false;

            // if player not in keep
            if (!Globals_Client.myChar.inKeep)
            {
                // attempt to enter keep
                enteredKeep = Globals_Client.myChar.enterKeep();
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
            String place = "tavern";

            // exit keep if required
            if (Globals_Client.myChar.inKeep)
            {
                Globals_Client.myChar.exitKeep();
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
            for (int i = 0; i < Globals_Client.fiefToView.characters.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.fiefToView.characters[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.fiefToView.characters[i];

                        // check whether is employee or family
                        if (Globals_Client.myChar.myNPCs.Contains(Globals_Client.fiefToView.characters[i]))
                        {
                            // see if is in entourage to set text of entourage button
                            if ((Globals_Client.fiefToView.characters[i] as NonPlayerCharacter).inEntourage)
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

                            // if is employee
                            if (((Globals_Client.fiefToView.characters[i] as NonPlayerCharacter).myBoss != null)
                                && ((Globals_Client.fiefToView.characters[i] as NonPlayerCharacter).myBoss.Equals(Globals_Client.myChar.charID)))
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
                            this.hireNPC_Btn.Enabled = true;
                            this.hireNPC_TextBox.Visible = true;
                            this.hireNPC_TextBox.Enabled = true;

                            // disable 'move to' and entourage controls
                            this.meetingPlaceMoveToBtn.Enabled = false;
                            this.meetingPlaceMoveToTextBox.Enabled = false;
                            this.meetingPlaceRouteBtn.Enabled = false;
                            this.meetingPlaceRouteTextBox.Enabled = false;
                            this.meetingPlaceEntourageBtn.Enabled = false;
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
            String place = Convert.ToString(((Button)sender).Tag);

            // if selected NPC is not a current employee
            if (!Globals_Client.myChar.myNPCs.Contains(Globals_Client.charToView))
            {
                amHiring = true;

                try
                {
                    // get offer amount
                    UInt32 newOffer = Convert.ToUInt32(this.hireNPC_TextBox.Text);
                    // submit offer
                    isHired = Globals_Client.myChar.processEmployOffer((Globals_Client.charToView as NonPlayerCharacter), newOffer);

                }
                catch (System.FormatException fe)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
                catch (System.OverflowException ofe)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }

            }

            // if selected NPC is already an employee
            else
            {
                // fire NPC
                Globals_Client.myChar.fireNPC(Globals_Client.charToView as NonPlayerCharacter);
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
        /// <param name="isUpdate">Indicates if is update mode</param>
        private bool characterMultiMove(Character ch, bool isUpdate = false)
        {
            bool success = false;
            double travelCost = 0;
            int steps = ch.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                // get travel cost
                travelCost = this.getTravelCost(ch.location, ch.goTo.Peek(), ch.armyID);
                // attempt to move character
                success = this.moveCharacter(ch, ch.goTo.Peek(), travelCost, isUpdate);
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

            if (ch == Globals_Client.myChar)
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
        public void moveTo(String whichScreen)
        {
            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myChar.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }                
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myChar.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelMoveToTextBox;
            }

            // check for existence of fief
            if (Globals_Server.fiefMasterList.ContainsKey(myTextBox.Text))
            {
                // retrieves target fief
                Fief target = Globals_Server.fiefMasterList[myTextBox.Text];

                // obtains goTo queue for shortest path to target
                Globals_Client.charToView.goTo = Globals_Server.gameMap.getShortestPath(Globals_Client.charToView.location, target);

                // if retrieve valid path
                if (Globals_Client.charToView.goTo.Count > 0)
                {
                    // if character is NPC, check entourage and remove if necessary
                    if (!whichScreen.Equals("travel"))
                    {
                        if (Globals_Client.myChar.myNPCs.Contains(Globals_Client.charToView))
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
                System.Windows.Forms.MessageBox.Show("Target fief ID not found.  Please ensure fiefID is valid.");
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
            String whichScreen = button.Tag.ToString();

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

        /// <summary>
        /// Checks whether the supplied integer is odd or even
        /// </summary>
        /// <returns>bool indicating whether odd</returns>
        /// <param name="value">Integer to be checked</param>
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        // temporary method to write object data to database
        public String[][] ArrayFromCSV(String csvFilename, bool writeToDB, String bucket = "", String key = "")
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
                System.Windows.Forms.MessageBox.Show("Appointment cancelled.");
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
                Globals_Client.fiefToView.bailiff = Globals_Client.myChar;
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
            String place = "outsideKeep";

            // exit keep if required
            if (Globals_Client.myChar.inKeep)
            {
                Globals_Client.myChar.exitKeep();
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
            for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
            {
                if (this.houseCharListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.myChar.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.myChar.myNPCs[i];
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
                if ((Globals_Client.charToView.familyID != null) && (Globals_Client.charToView.familyID.Equals(Globals_Client.myChar.charID)))
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

                // if character aged 0 and firstname = "Baby", enable 'name child' controls
                if ((charToDisplay as NonPlayerCharacter).checkForName(0))
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
                System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
            }
            catch (System.OverflowException ofe)
            {
                System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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
                this.campWaitHere(Globals_Client.myChar, campDays);
            }
            catch (System.FormatException fe)
            {
                System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
            }
            catch (System.OverflowException ofe)
            {
                System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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
                    System.Windows.Forms.MessageBox.Show("You decide not to camp after all.");
                }
            }

            if (proceed)
            {
                // check if player's entourage needs to camp
                bool entourageCamp = false;

                // if character is player, camp entourage
                if (ch == Globals_Client.myChar)
                {
                    entourageCamp = true;
                }

                // if character NOT player
                else
                {
                    // if is in entourage, give player chance to remove prior to camping
                    if ((ch as NonPlayerCharacter).inEntourage)
                    {
                        System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " has been removed from your entourage.");
                        Globals_Client.myChar.removeFromEntourage((ch as NonPlayerCharacter));
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
                System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " remains in " + ch.location.name + " for " + campDays + " days.");

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
                        System.Windows.Forms.MessageBox.Show("Army (" + thisArmy.armyID + ") lost " + totalAttrition + " troops due to attrition.");
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
                    if (Globals_Client.myChar == ch.location.bailiff)
                    {
                        myBailiff = Globals_Client.myChar;
                    }
                    // if not, check for bailiff in entourage
                    else
                    {
                        for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
                        {
                            if (Globals_Client.myChar.myNPCs[i].inEntourage)
                            {
                                if (Globals_Client.myChar.myNPCs[i] != ch)
                                {
                                    if (Globals_Client.myChar.myNPCs[i] == ch.location.bailiff)
                                    {
                                        myBailiff = Globals_Client.myChar.myNPCs[i];
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
                            System.Windows.Forms.MessageBox.Show(myBailiff.firstName + " " + myBailiff.familyName + " has fulfilled his bailiff duties in " + ch.location.name + ".");
                        }
                    }
                }
            }

            // refresh display
            if (proceed)
            {
                if (ch == Globals_Client.myChar)
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
        public void takeThisRoute(String whichScreen)
        {
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
                    Globals_Client.myChar.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseRouteTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myChar.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelRouteTextBox;
            }

            // get list of directions
            String[] directions = myTextBox.Text.Split(',').ToArray<String>();

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
                target = Globals_Server.gameMap.getFief(source, directions[i].ToUpper());

                // if target successfully acquired, add to queue
                if (target != null)
                {
                    route.Enqueue(target);
                }
                // if no target acquired, display message and break
                else
                {
                    System.Windows.Forms.MessageBox.Show("Invalid direction code encountered.  Route halted at " + source.name + " (" + source.fiefID + ")");
                    break;
                }

                // if there are any fiefs in the queue, overwrite the character's goTo queue
                // then process by calling characterMultiMove
                if (route.Count > 0)
                {
                    Globals_Client.charToView.goTo = route;
                    this.characterMultiMove(Globals_Client.charToView);
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
            else if (whichScreen.Equals("travel"))
            {
                this.refreshTravelContainer();
            }

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
            String whichScreen = button.Tag.ToString();

            // perform move
            this.takeThisRoute(whichScreen);
        }

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
                    System.Windows.Forms.MessageBox.Show("Too few funds available in Home Treasury; amount adjusted.");
                    amount = fiefFrom.getAvailableTreasury(true);
                }

                // make the transfer
                this.treasuryTransfer(fiefFrom, fiefTo, amount);
            }
            catch (System.FormatException fe)
            {
                System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
            }
            catch (System.OverflowException ofe)
            {
                System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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
                Fief fiefTo = Globals_Server.fiefMasterList[Globals_Client.myChar.homeFief];
                int amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);

                // make sure are enough funds to cover transfer
                if (amount > fiefFrom.getAvailableTreasury())
                {
                    // if not, inform player and adjust amount downwards
                    System.Windows.Forms.MessageBox.Show("Too few funds available in " + fiefFrom.name + " Treasury; amount adjusted.");
                    amount = fiefFrom.getAvailableTreasury();
                }

                // make the transfer
                this.treasuryTransfer(fiefFrom, fiefTo, amount);
            }
            catch (System.FormatException fe)
            {
                System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
            }
            catch (System.OverflowException ofe)
            {
                System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
            }
        }

        /// <summary>
        /// Transfers funds between the home treasury and the fief treasury
        /// </summary>
        /// <param name="from">The Fief from which funds are to be transferred</param>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        private void treasuryTransfer(Fief from, Fief to, int amount)
        {
            // subtract from source treasury
            from.treasury = from.treasury - amount;
            // add to target treasury
            to.treasury = to.treasury + amount;
            // refresh fief display
            this.refreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the click event of the familyGetSpousePregBt button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyGetSpousePregBtn_Click(object sender, EventArgs e)
        {
            // get spouse
            Character mySpouse = Globals_Client.myChar.getSpouse();

            // perform standard checks
            if (this.checkBeforePregnancyAttempt(Globals_Client.myChar))
            {
                // ensure are both in/out of keep
                mySpouse.inKeep = Globals_Client.myChar.inKeep;

                // attempt pregnancy
                bool pregnant = Globals_Client.myChar.getSpousePregnant(mySpouse);
            }

            /*
            // test event scheduled in clock
            List<JournalEvent> myEvents = new List<JournalEvent>();
            myEvents = Globals_Client.clock.scheduledEvents.getEventsOnDate();
            if (myEvents.Count > 0)
            {
                foreach (JournalEvent jEvent in myEvents)
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

            // first name
            newNPC.firstName = "Baby";
            // family name
            newNPC.familyName = daddy.familyName;
            // date of birth
            newNPC.birthDate = new Tuple<uint, byte>(Globals_Server.clock.currentYear, Globals_Server.clock.currentSeason);
            // nationality
            newNPC.nationality = daddy.nationality;
            // whether is alive
            newNPC.isAlive = true;
            // goTo queue
            newNPC.goTo = new Queue<Fief>();
            // language
            newNPC.language = daddy.language;
            // days left
            newNPC.days = 90;
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
            // location
            newNPC.location = null;
            // titles
            newNPC.myTitles = new List<string>();
            // sex
            newNPC.isMale = this.generateSex();
            // maxHealth
            newNPC.maxHealth = this.generateKeyCharacteristics(mummy.maxHealth, daddy.maxHealth);
            // virility
            newNPC.virility = this.generateKeyCharacteristics(mummy.virility, daddy.virility);
            // management
            newNPC.management = this.generateKeyCharacteristics(mummy.management, daddy.management);
            // combat
            newNPC.combat = this.generateKeyCharacteristics(mummy.combat, daddy.combat);
            // skills
            newNPC.skills = this.generateSkillSetFromParents(mummy.skills, daddy.skills, newNPC.isMale);
            // charID
            newNPC.charID = Convert.ToString(Globals_Server.getNextCharID());
            // stature modifier
            newNPC.statureModifier = 0;
            // employer (myBoss)
            newNPC.myBoss = null;
            // salary/allowance
            newNPC.wage = 0;
            // inEntourage
            newNPC.inEntourage = false;
            // lastOffer (will remain empty for family members)
            newNPC.lastOffer = new Dictionary<string, uint>();

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
            if (Globals_Server.myRand.Next(0, 2) == 0)
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
            double randPercentage = Globals_Server.GetRandomDouble(100);

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
                numSkills = Globals_Server.myRand.Next(2, 4);
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
                chosenSkill = Globals_Server.myRand.Next(0, firstSkillSet.Length);

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
                        chosenSkill = Globals_Server.myRand.Next(0, firstSkillSet.Length);

                        // creat new skill item
                        mySkill = new Tuple<Skill, int>(firstSkillSet[chosenSkill].Item1, firstSkillSet[chosenSkill].Item2);
                        // add to temporary list
                        newSkillsList.Add(mySkill);

                    // do chosen skill doesn't match the first
                    } while (chosenSkill == PrevChosenSkill);

                }

                // get a skill from the other parent
                chosenSkill = Globals_Server.myRand.Next(0, lastSkillSet.Length);

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
            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = this.generateNewNPC(mummy, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.checkDeath(true, false, false);

            if (!isStillborn)
            {
                Globals_Server.npcMasterList.Add(weeBairn.charID, weeBairn);
                string[] childbirthPersonae = new string[] { mummy.charID + "|mother", daddy.charID + "|father", weeBairn.charID + "|child" };
                JournalEvent childbirth = new JournalEvent(Globals_Server.getNextJournalEventID(), Globals_Server.clock.currentYear, Globals_Server.clock.currentSeason, childbirthPersonae, "Birth", descr: Globals_Client.myChar.firstName + " " + Globals_Client.myChar.familyName + " welcomes a new " + weeBairn.getFunction(Globals_Client.myChar) + " into his family");
                Globals_Server.pastEvents.events.Add(childbirth.jEventID, childbirth);
                weeBairn.location = mummy.location;
                weeBairn.location.characters.Add(weeBairn);
                Globals_Client.myChar.myNPCs.Add(weeBairn);
            }
            else
            {
                weeBairn.isAlive = false;
            }

            // check for mother dying during childbirth
            bool mummyDied = mummy.checkDeath(true, true, isStillborn);

            // inform father of outcome
            String toDisplay = "";
            // both mother and baby died
            if ((isStillborn) && (mummyDied))
            {
                toDisplay += "Brace yourself, milord.\r\n\r\nYour wife went into labour";
                toDisplay += " but I'm afraid the child was stillborn and your wife died of complications.";
                toDisplay += "\r\n\r\nMy condolences, milord.";
            }
            // baby died but mother OK
            else if (isStillborn)
            {
                toDisplay += "I have news, milord.\r\n\r\nYour wife went into labour";
                toDisplay += " but I'm afraid the child was stillborn.  I'm glad to say that your wife is recovering well.";
                toDisplay += "\r\n\r\nMy condolences, milord.";
            }
            // baby OK but mother died
            else if (mummyDied)
            {
                toDisplay += "I have news, milord.\r\n\r\nYour wife went into labour";
                toDisplay += " and has given birth to a healthy baby ";
                if (weeBairn.isMale)
                {
                    toDisplay += "boy";
                }
                else
                {
                    toDisplay += "girl";
                }
                toDisplay += ".  I'm sorry to report that your wife died of complications.";
                toDisplay += "\r\n\r\nMy condolences and congratulations, milord.";
            }
            // both mother and baby doing well
            else
            {
                toDisplay += "Wonderful news, milord.\r\n\r\nYour wife went into labour";
                toDisplay += " and has given birth to a healthy baby ";
                if (weeBairn.isMale)
                {
                    toDisplay += "boy";
                }
                else
                {
                    toDisplay += "girl";
                }
                toDisplay += ".  I'm glad to say that your wife is recovering well.";
                toDisplay += "\r\n\r\nMy congratulations, milord.";
            }
            System.Windows.Forms.MessageBox.Show(toDisplay);

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
            this.giveBirth(Globals_Server.npcMasterList[Globals_Client.myChar.spouse], Globals_Client.myChar);
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
                for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
                {
                    if (Globals_Client.myChar.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        child = Globals_Client.myChar.myNPCs[i];
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
                        System.Windows.Forms.MessageBox.Show("'" + this.familyNameChildTextBox.Text + "' is an unsuitable name, milord.");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Could not retrieve details of NonPlayerCharacter.");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select a character from the list.");
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
                    // TODO: do whatever necessary to ensure safe closedown
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
            String updateType = menuItem.Tag.ToString();

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
                System.Windows.Forms.MessageBox.Show("No PlayerCharacter ID entered.  Operation cancelled.");
            }
            else if (!Globals_Server.pcMasterList.ContainsKey(playerID))
            {
                System.Windows.Forms.MessageBox.Show("PlayerCharacter could not be identified.  Operation cancelled.");
            }
            else
            {
                Globals_Client.myChar = Globals_Server.pcMasterList[playerID];
                Globals_Client.charToView = Globals_Client.myChar;
                this.refreshCharacterContainer(Globals_Client.charToView);
            }
        }

        private void houseHeirBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Server.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];

                // check for an existing heir and remove
                foreach (NonPlayerCharacter npc in Globals_Client.myChar.myNPCs)
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
            Army thisArmy = Globals_Client.myChar.getArmy();

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
            Fief armyLocation = Globals_Server.fiefMasterList[a.location];

            // add to armyMasterList
            Globals_Server.armyMasterList.Add(a.armyID, a);

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
            String operation = button.Tag.ToString();

            // get fief
            Fief thisFief = Globals_Client.myChar.location;

            // check for siege
            if (thisFief.siege != null)
            {
                System.Windows.Forms.MessageBox.Show("You cannot recruit from a fief under siege.  Recruitment cancelled.");
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
                        Army newArmy = new Army(Globals_Server.getNextArmyID(), Globals_Client.myChar.charID, Globals_Client.myChar.charID, Globals_Client.myChar.days, Globals_Client.myChar.location.fiefID);
                        this.addArmy(newArmy);
                    }

                    // recruit troops
                    Globals_Client.myChar.recruitTroops(numberWanted);

                    // get army
                    Army myArmy = Globals_Client.myChar.getArmy();

                    // refresh display
                    this.refreshArmyContainer(myArmy);
                }
                catch (System.FormatException fe)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
                catch (System.OverflowException ofe)
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
                this.mantainArmy(Globals_Client.armyToView);

                // refresh display
                this.refreshArmyContainer(Globals_Client.armyToView);
            }
        }

        public void mantainArmy(Army a)
        {
            String toDisplay = "";

            // get cost
            uint maintCost = a.calcArmySize() * 500;

            // get available treasury
            Fief homeFief = Globals_Client.myChar.getHomeFief();
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
                    System.Windows.Forms.MessageBox.Show(toDisplay);
                }
                else
                {
                    // set isMaintained
                    a.isMaintained = true;

                    // deduct funds from treasury
                    homeFief.treasury -= Convert.ToInt32(maintCost);

                    // display confirmation message
                    toDisplay += "Army maintained at a cost of £" + maintCost + ".";
                    System.Windows.Forms.MessageBox.Show(toDisplay);
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
            String toDisplay = "";

            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the command occurred
            String whichScreen = button.Tag.ToString();

            // check which action to perform
            // if is in entourage, remove
            if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
            {
                Globals_Client.myChar.removeFromEntourage((Globals_Client.charToView as NonPlayerCharacter));
            }

            // if is not in entourage, add
            else
            {
                // check to see if NPC is army leader
                // if not leader, proceed
                if (Globals_Client.charToView.armyID == null)
                {
                    // add to entourage
                    Globals_Client.myChar.addToEntourage((Globals_Client.charToView as NonPlayerCharacter));

                }

                // if is army leader, can't add to entourage
                else
                {
                    toDisplay += "Sorry, milord, this person is an army leader\r\n";
                    toDisplay += "and, therefore, cannot be added to your entourage.";
                    System.Windows.Forms.MessageBox.Show(toDisplay);
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
                Globals_Client.armyToView = Globals_Server.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];
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
                    if ((!(Globals_Client.armyToView.leader == Globals_Client.myChar.charID))
                        && (!(Globals_Client.myChar.armyID == null)))
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
                        if (Globals_Client.armyToView.leader == Globals_Client.myChar.charID)
                        {
                            this.armyRecruitBtn.Text = "Recruit Additional Troops From Current Fief";
                            this.armyRecruitBtn.Tag = "add";
                        }
                        // if player is not leading any armies, set button text to 'recruit new'
                        else if (Globals_Client.myChar.armyID == null)
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

                    // set auto combat values
                    this.armyAggroTextBox.Text = Globals_Client.armyToView.aggression.ToString();
                    this.armyOddsTextBox.Text = Globals_Client.armyToView.combatOdds.ToString();

                    // preload own ID in 'drop off to' textbox (assumes transferring between own armies)
                    this.armyTransDropWhoTextBox.Text = Globals_Client.myChar.charID;
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

            String thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseLeader = new SelectionForm(this, "leader", armyID: thisArmyID);
                chooseLeader.Show();
            }

            // if no army selected
            else
            {
                System.Windows.Forms.MessageBox.Show("No army selected!");
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
            Army thisArmy = Globals_Server.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

            thisArmy.assignNewLeader(Globals_Client.myChar);

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
                            System.Windows.Forms.MessageBox.Show("You don't have enough "+ troopTypeLabels[i] + " in your army for that transfer.  Transfer cancelled.");
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
                        System.Windows.Forms.MessageBox.Show("You haven't selected any troops for transfer.  Transfer cancelled.");
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
                            System.Windows.Forms.MessageBox.Show("Transfer cancelled.");
                            proceed = false;
                            adjustDays = false;
                        }
                    }

                    // check have minimum days necessary for transfer
                    if (Globals_Client.armyToView.days < 10)
                    {
                        System.Windows.Forms.MessageBox.Show("You don't have enough days left for this transfer.  Transfer cancelled.");
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        // calculate time taken for transfer
                        daysTaken = Globals_Server.myRand.Next(10, 31);

                        // check if have enough days for transfer in this instance
                        if (daysTaken > Globals_Client.armyToView.days)
                        {
                            System.Windows.Forms.MessageBox.Show("Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.");
                            proceed = false;
                        }
                    }

                    // check transfer recipient exists
                    if (!Globals_Server.pcMasterList.ContainsKey(this.armyTransDropWhoTextBox.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify transfer recipient.  Transfer cancelled.");
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
                        Fief thisFief = Globals_Server.fiefMasterList[Globals_Client.armyToView.location];

                        // create transfer entry
                        string[] thisTransfer = new string[9] { Globals_Client.myChar.charID, this.armyTransDropWhoTextBox.Text,
                            troopsToTransfer[0].ToString(), troopsToTransfer[1].ToString(), troopsToTransfer[2].ToString(),
                            troopsToTransfer[3].ToString(), troopsToTransfer[4].ToString(), troopsToTransfer[5].ToString(),
                            (Globals_Client.armyToView.days - daysTaken).ToString() };

                        // add to fief's troopTransfers list
                        thisFief.troopTransfers.Add(Globals_Server.getNextDetachmentID(), thisTransfer);
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
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
                catch (System.OverflowException ofe)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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

            String thisArmyID = null;
            if (this.armyListView.SelectedItems.Count > 0)
            {
                // get armyID and army
                thisArmyID = this.armyListView.SelectedItems[0].SubItems[0].Text;

                // display selection form
                SelectionForm chooseTroops = new SelectionForm(this, "transfer", armyID: thisArmyID);
                chooseTroops.Show();
            }

            // if no army selected
            else
            {
                System.Windows.Forms.MessageBox.Show("No army selected!");
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
                thisSiege = Globals_Server.siegeMasterList[siegeID];
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
            Globals_Server.armyMasterList.Remove(a.armyID);

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
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
                catch (System.OverflowException ofe)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
                catch (System.OverflowException ofe)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
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
                System.Windows.Forms.MessageBox.Show("No army selected!");
            }

            // if army selected
            else
            {
                // check if has minimum days
                if (Globals_Client.armyToView.days < 1)
                {
                    System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                }

                // has minimum days
                else
                {
                    // see how long reconnaissance takes
                    int reconDays = Globals_Server.myRand.Next(1, 4);

                    // check if runs out of time
                    if (Globals_Client.armyToView.days < reconDays)
                    {
                        // set days to 0
                        thisLeader.adjustDays(Globals_Client.armyToView.days);
                        this.refreshArmyContainer(Globals_Client.armyToView);
                        System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
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
                System.Windows.Forms.MessageBox.Show("No NPC selected!");
            }

            // if NPC selected
            else
            {
                // check if has minimum days
                if (Globals_Client.charToView.days < 1)
                {
                    System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                }

                // has minimum days
                else
                {
                    // see how long reconnaissance takes
                    int reconDays = Globals_Server.myRand.Next(1, 4);

                    // check if runs out of time
                    if (Globals_Client.charToView.days < reconDays)
                    {
                        // set days to 0
                        Globals_Client.charToView.adjustDays(Globals_Client.armyToView.days);
                        this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                        System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
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
            if (Globals_Client.myChar.days < 1)
            {
                System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
            }

            // has minimum days
            else
            {
                // see how long reconnaissance takes
                int reconDays = Globals_Server.myRand.Next(1, 4);

                // check if runs out of time
                if (Globals_Client.myChar.days < reconDays)
                {
                    // set days to 0
                    Globals_Client.myChar.adjustDays(Globals_Client.myChar.days);
                    this.refreshTravelContainer();
                    System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
                }

                // doesn't run out of time
                else
                {
                    // adjust days
                    Globals_Client.myChar.adjustDays(reconDays);
                    this.refreshTravelContainer();

                    // display armies list
                    SelectionForm examineArmies = new SelectionForm(this, "armies", obs: Globals_Client.myChar);
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
            int randomPercentage = Globals_Server.myRand.Next(101);

            // compare random percentage to battleChance
            if (randomPercentage <= thisChance)
            {
                battleHasCommenced = true;
            }

            if (battleHasCommenced)
            {
                System.Windows.Forms.MessageBox.Show("The attacker has successfully brought the defender to battle");
            }

            return battleHasCommenced;
        }

        /// <summary>
        /// Calculates whether the attacking army is victorious in a battle
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public bool decideBattleVictory(uint attackerValue, uint defenderValue)
        {
            bool attackerVictorious = false;

            // calculate chance of victory
            double attackerVictoryChance = (attackerValue / (attackerValue + defenderValue)) * 100;

            // generate random percentage
            int randomPercentage = Globals_Server.myRand.Next(101);

            // compare random percentage to attackerVictoryChance
            if (randomPercentage <= attackerVictoryChance)
            {
                attackerVictorious = true;
            }

            return attackerVictorious;
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
            double winnerIncrement = Globals_Server.GetRandomDouble(min: 0.02, max: 0.04);
            double loserIncrement = Globals_Server.GetRandomDouble(min: 0.05, max: 0.1);

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
                    retreatDistance[0] = Globals_Server.myRand.Next(1, 3);
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
                    retreatDistance[1] = Globals_Server.myRand.Next(1, 3);
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
                Fief target = Globals_Server.gameMap.chooseRandomHex(from, true, thisOwner, retreatFrom);

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
            bool attackerVictorious = false;
            bool battleHasCommenced = false;
            uint[] battleValues = new uint[2];
            double[] casualtyModifiers = new double[2];

            // get leaders
            Character attackerLeader = attacker.getLeader();
            Character defenderLeader = defender.getLeader();

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

                // CASUALTIES
                // calculate troop casualties for both sides
                casualtyModifiers = this.calculateBattleCasualties(battleValues[0], battleValues[1], attackerVictorious);

                // check if losing army has disbanded
                bool attackerDisbanded = false;
                bool defenderDisbanded = false;

                // if losing side sustains >= 50% casualties, disbands
                if (attackerVictorious)
                {
                    // either indicate losing army to be disbanded
                    if (casualtyModifiers[1] >= 0.5)
                    {
                        defenderDisbanded = true;
                    }
                    // OR apply troop casualties to losing army
                    else
                    {
                        defender.applyTroopLosses(casualtyModifiers[1]);
                    }

                    // apply troop casualties to winning army
                    attacker.applyTroopLosses(casualtyModifiers[0]);
                }
                else
                {
                    if (casualtyModifiers[0] >= 0.5)
                    {
                        attackerDisbanded = true;
                    }
                    else
                    {
                        attacker.applyTroopLosses(casualtyModifiers[0]);
                    }

                    defender.applyTroopLosses(casualtyModifiers[1]);
                }

                // if is pillage, attacking (temporary) army always disbands after battle
                if (circumstance.Equals("pillage"))
                {
                    attackerDisbanded = true;
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
                            // do something to remove character
                        }
                    }
                }

                // check army leader
                characterDead = attackerLeader.calculateCombatInjury(casualtyModifiers[0]);

                // process death, if applicable
                if (characterDead)
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

                    // do something to remove character
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
                                // do something to remove character
                            }
                        }
                    }

                    // check army leader
                    characterDead = defenderLeader.calculateCombatInjury(casualtyModifiers[1]);

                    // process death, if applicable
                    if (characterDead)
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

                        // do something to remove character
                    }
                }

                // check for SIEGE RELIEF
                // attacker (relieving army) victory or defender (besieging army) retreat = relief
                if ((attackerVictorious) || (retreatDistances[1] > 0))
                {
                    // check to see if defender was besieging the fief keep
                    string thisSiegeID = defender.checkIfBesieger();
                    if (thisSiegeID != null)
                    {
                        // if so, dismantle the siege
                        Siege thisSiege = Globals_Server.siegeMasterList[thisSiegeID];
                        this.siegeEnd(thisSiege);
                    }
                }

                // DISBANDMENT
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
                System.Windows.Forms.MessageBox.Show(defender.armyID + " has refused battle.");
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
        public void processPillage(Fief f, Army a)
        {
            string toDisplay = "";
            double thisLoss = 0;
            double moneyPillagedTotal = 0;
            double moneyPillagedOwner = 0;
            double pillageMultiplier = 0;

            // get pillaging army owner (receives a proportion of total spoils)
            PlayerCharacter armyOwner = a.getOwner();

            // calculate pillageMultiplier (based on no. pillagers per 1000 population)
            pillageMultiplier = a.calcArmySize() / (f.population / 1000);

            // calculate days taken for pillage
            double daysTaken = Globals_Server.myRand.Next(7, 16);
            if (daysTaken > a.days)
            {
                daysTaken = a.days;
            }
            toDisplay += "Days taken: " + daysTaken + "\r\n";

            // % population loss
            thisLoss = (0.007 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply population loss
            toDisplay += "Population loss: " + Convert.ToUInt32((f.population * (thisLoss / 100))) + "\r\n";
            f.population -= Convert.ToUInt32((f.population * (thisLoss / 100)));

            // % treasury loss
            thisLoss = (0.2 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply treasury loss
            toDisplay += "Treasury loss: " + Convert.ToInt32((f.treasury * (thisLoss / 100))) + "\r\n";
            if (f.treasury > 0)
            {
                f.treasury -= Convert.ToInt32((f.treasury * (thisLoss / 100)));
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
            toDisplay += "Loyalty loss: " + (f.loyalty * (thisLoss / 100)) + "\r\n";
            f.loyalty -= (f.loyalty * (thisLoss / 100));

            // % fields loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply fields loss
            toDisplay += "Fields loss: " + (f.fields * (thisLoss / 100)) + "\r\n";
            f.fields -= (f.fields * (thisLoss / 100));

            // % industry loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is at least 1%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            // apply industry loss
            toDisplay += "Industry loss: " + (f.industry * (thisLoss / 100)) + "\r\n";
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
            toDisplay += "baseMoneyPillaged: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";

            // factor in no. days spent pillaging (get extra 5% per day > 7)
            int daysOver7 = Convert.ToInt32(daysTaken) - 7;
            for (int i = 0; i < daysOver7; i++)
            {
                moneyPillagedTotal += (baseMoneyPillaged * 0.05);
            }
            toDisplay += "- after days > 7: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";

            // check for jackpot
            // generate randomPercentage to see if hit the jackpot
            int myRandomPercent = Globals_Server.myRand.Next(101);
            if (myRandomPercent <= 30)
            {
                // generate random int to multiply amount pillaged
                int myRandomMultiplier = Globals_Server.myRand.Next(3, 11);
                moneyPillagedTotal = moneyPillagedTotal * myRandomMultiplier;
                toDisplay += "- after jackpot: " + Convert.ToInt32(moneyPillagedTotal) + "\r\n";
            }

            // check proportion of money pillaged goes to army owner (based on stature)
            double proportionForOwner = 0.05 * armyOwner.calculateStature();
            moneyPillagedOwner = (moneyPillagedTotal * proportionForOwner);
            toDisplay += "Money pillaged by player (with " + armyOwner.calculateStature() + " stature): " + Convert.ToInt32(moneyPillagedOwner) + "\r\n";

            // apply to army owner's home fief treasury
            armyOwner.getHomeFief().treasury += Convert.ToInt32(moneyPillagedOwner);

            // set isPillaged for fief
            f.isPillaged = true;

            System.Windows.Forms.MessageBox.Show(toDisplay);
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
                for (int i = 0; i < f.characters.Count; i++)
                {
                    if (f.characters[i] == f.bailiff)
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
            string thisNationality = f.owner.nationality.ToUpper();
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
            defender = new Army("Garrison" + Globals_Server.getNextArmyID(), armyLeaderID, f.owner.charID, armyLeaderDays, f.fiefID, trp: troopsForArmy);
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
                for (int i = 0; i < f.characters.Count; i++ )
                {
                    if (f.characters[i] == f.bailiff)
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
                    System.Windows.Forms.MessageBox.Show("The pillaging force has been forced to retreat by the fief's defenders!");
                }

                // check still have enough days left
                if (a.days < 7)
                {
                    System.Windows.Forms.MessageBox.Show("After giving battle, the pillaging army no longer has\r\nsufficient days for this operation.  Pillage cancelled.");
                    pillageCancelled = true;
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

            // check if is your own fief
            if (f.owner == a.getOwner())
            {
                proceed = false;
                if (circumstance == "pillage")
                {
                    System.Windows.Forms.MessageBox.Show("You cannot pillage your own fief!  Pillage cancelled.");
                }
                else if (circumstance == "siege")
                {
                    System.Windows.Forms.MessageBox.Show("You cannot besiege your own fief!  Siege cancelled.");
                }
            }

            // check if fief is under siege
            if ((f.siege != null) && (proceed))
            {
                proceed = false;
                if (circumstance == "pillage")
                {
                    System.Windows.Forms.MessageBox.Show("You cannot pillage a fief that is under siege.  Pillage cancelled.");
                }
                else if (circumstance == "siege")
                {
                    System.Windows.Forms.MessageBox.Show("This fief is already under siege.  Siege cancelled.");
                }
            }

            if (circumstance == "pillage")
            {
                // check isPillaged = false
                if ((f.isPillaged) && (proceed))
                {
                    proceed = false;
                    System.Windows.Forms.MessageBox.Show("This fief has already been pillaged during\r\nthe current season.  Pillage cancelled.");
                }
            }

            // check if your army has a leader
            if (a.getLeader() == null)
            {
                proceed = false;
                if (circumstance == "pillage")
                {
                    System.Windows.Forms.MessageBox.Show("This army has no leader.  Pillage cancelled.");
                }
                else if (circumstance == "siege")
                {
                    System.Windows.Forms.MessageBox.Show("This army has no leader.  Siege cancelled.");
                }
            }

            // check has min days required
            if (circumstance == "pillage")
            {
                // pillage = min 7
                if ((a.days < 7) && (proceed))
                {
                    proceed = false;
                    System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na pillage operation.  Pillage cancelled.");
                }
            }
            else if (circumstance == "siege")
            {
                // siege = 1 (to set up siege)
                if ((a.days < 1) && (proceed))
                {
                    proceed = false;
                    System.Windows.Forms.MessageBox.Show("This army has too few days remaining for\r\na siege operation.  Siege cancelled.");
                }
            }

            // check for presence of armies belonging to fief owner
            if (proceed)
            {
                // iterate through armies in fief
                for (int i = 0; i < f.armies.Count; i++)
                {
                    // get army
                    Army armyInFief = Globals_Server.armyMasterList[f.armies[i]];

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
                                if (circumstance == "pillage")
                                {
                                    System.Windows.Forms.MessageBox.Show("There is at least one defending army (" + armyInFief.armyID + ") that must be defeated\r\nbefore you can pillage this fief.  Pillage cancelled.");
                                }
                                else if (circumstance == "siege")
                                {
                                    System.Windows.Forms.MessageBox.Show("There is at least one defending army (" + armyInFief.armyID + ") that must be defeated\r\nbefore you can besiege this fief.  Siege cancelled.");
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
                System.Windows.Forms.MessageBox.Show("There are not enough days remaining for this\r\na siege operation.  Operation cancelled.");
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
                Army thisArmy = Globals_Server.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

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
                System.Windows.Forms.MessageBox.Show("No army selected!");
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
        public void siegeEnd(Siege s)
        {
            // get principle objects
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            Fief besiegedFief = s.getFief();
            PlayerCharacter besiegingPlayer = s.getBesiegingPlayer();
            PlayerCharacter defendingPlayer = s.getDefendingPlayer();
            Character besiegingArmyLeader = s.getBesiegingArmy().getLeader();

            // disband garrison
            this.disbandArmy(defenderGarrison);

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
            foreach (Character thisChar in besiegedFief.characters)
            {
                if (thisChar.inKeep)
                {
                    // reset character's days to reflect days spent in siege
                    thisChar.days = thisChar.getDaysAllowance() * daysProportion;
                }
            }

            // remove from master list
            Globals_Server.siegeMasterList.Remove(s.siegeID);

            // set to null
            s = null;
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
                    stormFailurePercent = stormFailurePercent + (stormFailurePercent * (0.08 * (1 / keepLvl - 1)));
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
        public void siegeStormRound(Siege s)
        {
            bool stormSuccess = false;
            Fief besiegedFief = s.getFief();
            Army besiegingArmy = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = s.getDefenderAdditional();
            PlayerCharacter attackingPlayer = s.getBesiegingPlayer();
            Character defenderLeader = defenderGarrison.getLeader();

            // process non-storm round
            this.siegeReductionRound(s);

            // calculate current keep level
            double keepLvl = this.calcStormKeepLevel(s);

            // calculate success chance based on keep level
            double successChance = this.calcStormSuccess(keepLvl);

            // generate random double 0-100 to see if storm a success
            double myRandomDouble = Globals_Server.GetRandomDouble(100);

            if (myRandomDouble <= successChance)
            {
                stormSuccess = true;
            }

            // KEEP DAMAGE
            // base damage to keep level (10%)
            double keepDamageModifier = 0.1;

            // calculate further damage, based on comparative battle values (up to extra 15%)
            uint [] battleValues = this.calculateBattleValue(besiegingArmy, defenderGarrison, Convert.ToInt32(keepLvl));
            // divide attackerBV by defenderBV to get extraDamageMultiplier
            double extraDamageMultiplier = battleValues[0] / battleValues[1];

            // ensure extraDamageMultiplier is max 10
            if (extraDamageMultiplier > 10)
            {
                extraDamageMultiplier = 10;
            }

            // generate random double 0-1 to see what proportion of extraDamageMultiplier will apply
            myRandomDouble = Globals_Server.GetRandomDouble(1);
            extraDamageMultiplier =  extraDamageMultiplier * myRandomDouble;

            keepDamageModifier += (0.015 * extraDamageMultiplier);
            keepDamageModifier = (1 - keepDamageModifier);

            // apply keep damage
            besiegedFief.keepLevel = besiegedFief.keepLevel * keepDamageModifier;

            // CASUALTIES, based on comparative battle values and keep level
            // 1. DEFENDER
            // defender base casualtyModifier
            double defenderCasualtyModifier = 0.01;
            defenderCasualtyModifier = defenderCasualtyModifier * (battleValues[0] / battleValues[1]);

            // apply casualties but not if storm was successful (defending army just disbands)
            if (!stormSuccess)
            {
                defenderGarrison.applyTroopLosses(defenderCasualtyModifier);
            }

            // 2. ATTACKER
            double attackerCasualtyModifier = 0.01;
            attackerCasualtyModifier = attackerCasualtyModifier * (battleValues[1] / battleValues[0]);
            // for attacker, add effects of keep level, modified by on storm success
            if (stormSuccess)
            {
                attackerCasualtyModifier += (0.001 * keepLvl);
            }
            else
            {
                attackerCasualtyModifier += (0.002 * keepLvl);
            }
            // apply casualties
            besiegingArmy.applyTroopLosses(attackerCasualtyModifier);

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
                            // do something to remove character
                        }
                    }
                }

                // check defenderLeader
                characterDead = defenderLeader.calculateCombatInjury(defenderCasualtyModifier);

                if (characterDead)
                {
                    // remove as leader
                    defenderGarrison.leader = null;

                    // do something to remove character
                }
            }

            if (stormSuccess)
            {
                // disband defending armies
                this.disbandArmy(defenderGarrison);
                if (defenderAdditional != null)
                {
                    this.disbandArmy(defenderAdditional);
                }

                // pillage fief
                this.processPillage(besiegedFief, besiegingArmy);

                // CAPTIVES
                // identify captives - fief owner, his family, and any PCs of enemy nationality
                List<Character> captives = new List<Character>();
                foreach (Character thisCharacter in besiegedFief.characters)
                {
                    if (thisCharacter.inKeep)
                    {
                        // fief owner and his family
                        if (thisCharacter.familyID.Equals(s.getDefendingPlayer().charID))
                        {
                            captives.Add(thisCharacter);
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
                }

                // change fief ownership
                besiegedFief.changeOwnership(attackingPlayer);
            }

        }

        /// <summary>
        /// Processes a single negotiation round of a siege
        /// </summary>
        /// <returns>bool indicating whether negotiation was successful</returns>
        /// <param name="s">The siege</param>
        public bool siegeNegotiationRound(Siege s)
        {
            bool negotiateSuccess = false;

            // process non-storm round
            this.siegeReductionRound(s);

            // calculate current keep level
            double keepLvl = this.calcStormKeepLevel(s);

            // calculate success chance based on keep level
            double successChance = (this.calcStormSuccess(keepLvl) / 2);

            // generate random double 0-100 to see if storm a success
            double myRandomDouble = Globals_Server.GetRandomDouble(100);

            if (myRandomDouble <= successChance)
            {
                negotiateSuccess = true;
            }

            return negotiateSuccess;
        }
        
        /// <summary>
        /// Processes a single reduction round of a siege
        /// </summary>
        /// <param name="s">The siege</param>
        public void siegeReductionRound(Siege s)
        {
            bool siegeRaised = false;
            Fief besiegedFief = s.getFief();
            Army besieger = s.getBesiegingArmy();
            Army defenderGarrison = s.getDefenderGarrison();
            Army defenderAdditional = null;
            Character defenderLeader = defenderGarrison.getLeader();

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
                System.Windows.Forms.MessageBox.Show("The defenders have successfully raised the siege!");
            }

            else
            {
                // process results of siege round
                // reduce keep level by 5%
                besiegedFief.keepLevel = (besiegedFief.keepLevel * 0.95);

                // apply combat losses to defenderGarrison
                // NOTE: attrition for both sides is calculated in siege.syncDays

                double combatLosses = 0.01;
                defenderGarrison.applyTroopLosses(combatLosses);

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
                                // do something to remove character
                            }
                        }
                    }

                    // check defenderLeader
                    characterDead = defenderLeader.calculateCombatInjury(combatLosses);

                    if (characterDead)
                    {
                        // remove as leader
                        defenderGarrison.leader = null;

                        // do something to remove character
                    }
                }

                // update total days (NOTE: siege.days will be updated in syncDays)
                s.totalDays += 10;

                // synchronise days
                s.syncDays(s.days - 10);
            }

            // refresh screen
            this.refreshCurrentScreen();
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
            uint thisYear = Globals_Server.clock.currentYear;

            switch (relativeSeason)
            {
                case (-1):
                    if (Globals_Server.clock.currentSeason == 0)
                    {
                        financialYear = thisYear - 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                case (1):
                    if (Globals_Server.clock.currentSeason == 4)
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
            byte thisSeason = Globals_Server.clock.currentSeason;

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
        public void besiegeFief(Army attacker, Fief target)
        {
            Army defenderGarrison = null;
            Army defenderAdditional = null;

            // create defending force
            defenderGarrison = this.createDefendingArmy(target);

            // check for existence of army in keep
            for (int i = 0; i < target.armies.Count; i++)
            {
                // get army
                Army armyInFief = Globals_Server.armyMasterList[target.armies[i]];

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
            Siege mySiege = new Siege(Globals_Server.getNextSiegeID(), Globals_Server.clock.currentYear, Globals_Server.clock.currentSeason, attacker.getOwner().charID, target.owner.charID, attacker.armyID, defenderGarrison.armyID, target.fiefID, minDays, target.keepLevel, defAdd: defAddID);

            // add to master list
            Globals_Server.siegeMasterList.Add(mySiege.siegeID, mySiege);

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
                Globals_Client.siegeToView = Globals_Server.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];
            }

            if (Globals_Client.siegeToView != null)
            {
                Army besiegingArmy = Globals_Client.siegeToView.getBesiegingArmy();
                PlayerCharacter besiegingPlayer = Globals_Client.siegeToView.getBesiegingPlayer();
                bool playerIsBesieger = (Globals_Client.myChar == besiegingPlayer);

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
                Army thisArmy = Globals_Server.armyMasterList[this.armyListView.SelectedItems[0].SubItems[0].Text];

                // get fief
                Fief thisFief = thisArmy.getLocation();

                // do various checks
                proceed = this.checksBeforePillageSiege(thisArmy, thisFief, "siege");

                // process siege
                if (proceed)
                {
                    this.besiegeFief(thisArmy, thisFief);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No army selected!");
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
                Siege thisSiege = Globals_Server.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

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
                System.Windows.Forms.MessageBox.Show("No siege selected!");
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
                Siege thisSiege = Globals_Server.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

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
                System.Windows.Forms.MessageBox.Show("No siege selected!");
            }
        }

        /// <summary>
        /// Responds to the click event of the siegeReduceBtn button
        /// processing a single siege reduction round
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void siegeReduceBtn_Click(object sender, EventArgs e)
        {
            if (this.siegeListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                // get siege
                Siege thisSiege = Globals_Server.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege);

                if (proceed)
                {
                    // process siege reduction round
                    this.siegeReductionRound(thisSiege);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No siege selected!");
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
                Siege thisSiege = Globals_Server.siegeMasterList[this.siegeListView.SelectedItems[0].SubItems[0].Text];

                // perform conditional checks here
                proceed = this.checksBeforeSiegeOperation(thisSiege, "end");

                if (proceed)
                {
                    // process siege reduction round
                    this.siegeEnd(thisSiege);

                    //refresh screen
                    this.refreshCurrentScreen();
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No siege selected!");
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
                NonPlayerCharacter wife = Globals_Server.npcMasterList[husband.spouse];

                // check to make sure is in same fief
                if (!(wife.location == husband.location))
                {
                    System.Windows.Forms.MessageBox.Show("You have to be in the same fief to do that!");
                    proceed = false;
                }

                else
                {
                    // make sure wife not already pregnant
                    if (wife.isPregnant)
                    {
                        System.Windows.Forms.MessageBox.Show(wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!", "PREGNANCY ATTEMPT CANCELLED");
                        proceed = false;
                    }

                    // check if are kept apart by siege
                    else
                    {
                        if ((husband.location.siege != null) && (husband.inKeep != wife.inKeep))
                        {
                            System.Windows.Forms.MessageBox.Show("I'm afraid the husband and wife are being separated by the ongoing siege.", "PREGNANCY ATTEMPT CANCELLED");
                            proceed = false;
                        }

                        else
                        {
                            // ensure player and spouse have at least 1 day remaining
                            double minDays = Math.Min(husband.days, wife.days);

                            if (minDays < 1)
                            {
                                System.Windows.Forms.MessageBox.Show("Sorry, you don't have enough time left for this in the current season.", "PREGNANCY ATTEMPT CANCELLED");
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
                System.Windows.Forms.MessageBox.Show("This man is not married.", "PREGNANCY ATTEMPT CANCELLED");
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
                System.Windows.Forms.MessageBox.Show("No character selected!");
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
            Fief homeFief = Globals_Client.myChar.getHomeFief();

            if (homeFief != null)
            {
                // display home fief
                this.refreshFiefContainer(homeFief);
            }

            // if have no home fief
            else
            {
                System.Windows.Forms.MessageBox.Show("You have no home fief!");
            }
        }

        /// <summary>
        /// Check to see if a JournalEvent is of interest to the player
        /// </summary>
        /// <returns>bool indicating whether the JournalEvent is of interest</returns>
        /// <param name="jEvent">The JournalEvent</param>
        public bool checkEventForInterest(JournalEvent jEvent)
        {
            bool isOfInterest = false;

            for (int i = 0; i < jEvent.personae.Length; i++)
            {
                string thisPersonae = jEvent.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');
                if (thisPersonaeSplit[0].Equals(Globals_Client.myChar.charID))
                {
                    isOfInterest = true;
                    break;
                }
            }

            return isOfInterest;
        }

        /// <summary>
        /// Adds a new JournalEvent to the myPastEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEvent to be added</param>
        public bool addMyPastEvent(JournalEvent jEvent)
        {
            bool success = false;

            success = Globals_Client.myPastEvents.addNewEvent(jEvent);

            // indicate unread items to player
            if (Globals_Client.myPastEvents.areNewItems)
            {
                this.journalToolStripMenuItem.BackColor = Color.GreenYellow;
            }

            return success;

        }

        /// <summary>
        /// Responds to the click event of the addTestJournalEventToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void addTestJournalEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create and add a past event
            string[] myEventPersonae = new string[] { Globals_Client.myChar.charID + "|father", Globals_Client.myChar.spouse + "|mother" };
            JournalEvent myEvent = new JournalEvent(Globals_Server.getNextJournalEventID(), 1280, 1, myEventPersonae, "birth");
            Globals_Server.addPastEvent(myEvent);
        }

        /// <summary>
        /// Updates appropriate components when data received from observable
        /// </summary>
        /// <param name="info">String containing data about component to update</param>
        public void update(String info)
        {
            // get update info
            string[] infoSplit = info.Split('|');
            switch (infoSplit[0])
            {
                case "newEvent":
                    // get jEvent ID and retrieve from Globals_Server
                    if (infoSplit[1] != null)
                    {
                        JournalEvent newJevent = Globals_Server.pastEvents.events[infoSplit[1]];

                        // check to see if is of interest to player
                        if (this.checkEventForInterest(newJevent))
                        {
                            this.addMyPastEvent(newJevent);
                            System.Windows.Forms.MessageBox.Show("There is a new event!");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }

}
