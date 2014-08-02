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
    public partial class Form1 : Form
    {
        /// <summary>
        /// Holds all NonPlayerCharacter objects
        /// </summary>
        public Dictionary<string, NonPlayerCharacter> npcMasterList = new Dictionary<string, NonPlayerCharacter>();
		/// <summary>
		/// Holds keys for NonPlayerCharacter objects (used when retrieving from database)
		/// </summary>
		List<String> npcKeys = new List<String> ();
        /// <summary>
        /// Holds all PlayerCharacter objects
        /// </summary>
        public Dictionary<string, PlayerCharacter> pcMasterList = new Dictionary<string, PlayerCharacter>();
		/// <summary>
        /// Holds keys for PlayerCharacter objects (used when retrieving from database)
		/// </summary>
		List<String> pcKeys = new List<String> ();
        /// <summary>
        /// Holds all Fief objects
        /// </summary>
        public Dictionary<string, Fief> fiefMasterList = new Dictionary<string, Fief>();
		/// <summary>
        /// Holds keys for Fief objects (used when retrieving from database)
		/// </summary>
		List<String> fiefKeys = new List<String> ();
		/// <summary>
		/// Holds all Kingdom objects
		/// </summary>
        public Dictionary<string, Kingdom> kingdomMasterList = new Dictionary<string, Kingdom>();
		/// <summary>
        /// Holds keys for Kingdom objects (used when retrieving from database)
		/// </summary>
		List<String> kingKeys = new List<String> ();
        /// <summary>
        /// Holds all Province objects
        /// </summary>
        public Dictionary<string, Province> provinceMasterList = new Dictionary<string, Province>();
        /// <summary>
        /// Holds keys for Province objects (used when retrieving from database)
        /// </summary>
        List<String> provKeys = new List<String>();
        /// <summary>
		/// Holds all Language objects
		/// </summary>
        public Dictionary<string, Language> languageMasterList = new Dictionary<string, Language>();
		/// <summary>
        /// Holds keys for Language objects (used when retrieving from database)
		/// </summary>
		List<String> langKeys = new List<String> ();
        /// <summary>
        /// Holds all Rank objects
        /// </summary>
        public Dictionary<string, Rank> rankMasterList = new Dictionary<string, Rank>();
        /// <summary>
        /// Holds keys for Rank objects (used when retrieving from database)
        /// </summary>
        List<String> rankKeys = new List<String>();
        /// <summary>
        /// Holds all Terrain objects
        /// </summary>
        public Dictionary<string, Terrain> terrainMasterList = new Dictionary<string, Terrain>();
        /// <summary>
        /// Holds keys for Terrain objects (used when retrieving from database)
        /// </summary>
        List<String> terrKeys = new List<String>();
        /// <summary>
		/// Holds all Skill objects
		/// </summary>
		public Dictionary<string, Skill> skillMasterList = new Dictionary<string, Skill>();
		/// <summary>
        /// Holds keys for Skill objects (used when retrieving from database)
		/// </summary>
		List<String> skillKeys = new List<String> ();
		/// <summary>
		/// Holds Character_Riak objects with existing goTo queues (used during initial load)
		/// </summary>
		List<Character_Riak> goToList = new List<Character_Riak> ();
        /// <summary>
        /// Holds main PlayerCharacter
        /// </summary>
        public PlayerCharacter myChar;
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        private Character charToView;
        /// <summary>
        /// Holds Fief to view in UI
        /// </summary>
        public Fief fiefToView;
        /// <summary>
        /// Holds HexMapGraph for this game
        /// </summary>
        private HexMapGraph gameMap;
        /// <summary>
        /// Holds GameClock for this game
        /// </summary>
        public GameClock clock { get; set; }
		/// <summary>
		/// Holds target RiakCluster 
		/// </summary>
		RiakCluster cluster;
		/// <summary>
        /// Holds RiakClient to communicate with RiakCluster
		/// </summary>
		RiakClient client;

        /// <summary>
        /// Constructor for Form1
        /// </summary>
        public Form1()
        {
            // initialise form elements
            InitializeComponent();

			// initialise Riak elements
			cluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
			client = (RiakClient)cluster.CreateClient();

            // initialise game objects
			this.initGameObjects("NOTdb", "101");

			// this.ArrayFromCSV ("/home/libdab/Dissertation_data/11-07-14/hacked-player.csv", true, "testGame", "skeletonPlayers1194");

			// write game objects to Riak
			//this.writeToDB ("testGame");
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
            this.myChar = this.pcMasterList[pc];
                
            // set inital fief to display
            this.fiefToView = this.myChar.location;

            // initialise list elements in UI
            this.setUpFiefsList();
            this.setUpMeetingPLaceCharsList();
            this.setUpHouseholdCharsList();

            // initialise character display in UI
            this.charToView = this.myChar;
            this.refreshCharacterContainer();

        }

		/// <summary>
		/// Creates some game objects from code (temporary)
		/// </summary>
		public void initialLoad()
		{

            // creat GameClock
			GameClock myGameClock = new GameClock("clock001", 1320);
			this.clock = myGameClock;

			// create skills
			// Dictionary of skill effects
			Dictionary<string, int> effectsCommand = new Dictionary<string, int>();
			effectsCommand.Add("battle", 40);
			effectsCommand.Add("siege", 40);
			effectsCommand.Add("npcHire", 20);
			// create skill
			Skill command = new Skill("sk001", "Command", effectsCommand);
			// add to skillsCollection
			this.skillMasterList.Add(command.skillID, command);

			Dictionary<string, int> effectsChivalry = new Dictionary<string, int>();
			effectsChivalry.Add("famExpense", 20);
			effectsChivalry.Add("fiefExpense", 10);
			effectsChivalry.Add("fiefLoy", 20);
			effectsChivalry.Add("npcHire", 10);
			effectsChivalry.Add("siege", 10);
			Skill chivalry = new Skill("sk002", "Chivalry", effectsChivalry);
			this.skillMasterList.Add(chivalry.skillID, chivalry);

			Dictionary<string, int> effectsAbrasiveness = new Dictionary<string, int>();
			effectsAbrasiveness.Add("battle", 15);
			effectsAbrasiveness.Add("death", 5);
			effectsAbrasiveness.Add("fiefExpense", -5);
			effectsAbrasiveness.Add("famExpense", 5);
			effectsAbrasiveness.Add("time", 5);
			effectsAbrasiveness.Add("siege", -10);
			Skill abrasiveness = new Skill("sk003", "Abrasiveness", effectsAbrasiveness);
			this.skillMasterList.Add(abrasiveness.skillID, abrasiveness);

			Dictionary<string, int> effectsAccountancy = new Dictionary<string, int>();
			effectsAccountancy.Add("time", 10);
			effectsAccountancy.Add("fiefExpense", -20);
			effectsAccountancy.Add("famExpense", -20);
			effectsAccountancy.Add("fiefLoy", -5);
			Skill accountancy = new Skill("sk004", "Accountancy", effectsAccountancy);
			this.skillMasterList.Add(accountancy.skillID, accountancy);

			Dictionary<string, int> effectsStupidity = new Dictionary<string, int>();
			effectsStupidity.Add("battle", -40);
			effectsStupidity.Add("death", 5);
			effectsStupidity.Add("fiefExpense", 20);
			effectsStupidity.Add("famExpense", 20);
			effectsStupidity.Add("fiefLoy", -10);
			effectsStupidity.Add("npcHire", -10);
			effectsStupidity.Add("time", -10);
			effectsStupidity.Add("siege", -40);
			Skill stupidity = new Skill("sk005", "Stupidity", effectsStupidity);
			this.skillMasterList.Add(stupidity.skillID, stupidity);

			Dictionary<string, int> effectsRobust = new Dictionary<string, int>();
			effectsRobust.Add("virility", 20);
			effectsRobust.Add("npcHire", 5);
			effectsRobust.Add("fiefLoy", 5);
			effectsRobust.Add("death", -20);
			Skill robust = new Skill("sk006", "Robust", effectsRobust);
			this.skillMasterList.Add(robust.skillID, robust);

			Dictionary<string, int> effectsPious = new Dictionary<string, int>();
			effectsPious.Add("virility", -20);
			effectsPious.Add("npcHire", 10);
			effectsPious.Add("fiefLoy", 10);
			effectsPious.Add("time", -10);
			Skill pious = new Skill("sk007", "Pious", effectsPious);
			this.skillMasterList.Add(pious.skillID, pious);

			// add each skillsCollection key to skillsKeys
			foreach (KeyValuePair<string, Skill> entry in this.skillMasterList)
			{
				this.skillKeys.Add(entry.Key);
			}

            // create Language objects
            Language c = new Language("langC", "Celtic");
			this.languageMasterList.Add (c.languageID, c);
            Language f = new Language("langF", "French");
            this.languageMasterList.Add(f.languageID, f);
            // create languages for Fiefs
            Tuple<Language, int> myLang1 = new Tuple<Language, int>(c, 1);
            Tuple<Language, int> myLang2 = new Tuple<Language, int>(c, 2);
            Tuple<Language, int> myLang3 = new Tuple<Language, int>(f, 1);

			// create terrain objects
			Terrain plains = new Terrain("P", "Plains", 1);
			this.terrainMasterList.Add (plains.terrainCode, plains);
			Terrain hills = new Terrain("H", "Hills", 1.5);
			this.terrainMasterList.Add (hills.terrainCode, hills);
			Terrain forrest = new Terrain("F", "Forrest", 1.5);
			this.terrainMasterList.Add (forrest.terrainCode, forrest);
			Terrain mountains = new Terrain("M", "Mountains", 90);
			this.terrainMasterList.Add (mountains.terrainCode, mountains);

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
            this.rankMasterList.Add(myRank03.rankID, myRank03);

            Tuple<String, String>[] myTitle09 = new Tuple<string, string>[3];
            myTitle09[0] = new Tuple<string, string>("langC", "Prince");
            myTitle09[1] = new Tuple<string, string>("E", "Prince");
            myTitle09[2] = new Tuple<string, string>("langF", "Prince");
            Rank myRank09 = new Rank("09", myTitle09, 4);
            this.rankMasterList.Add(myRank09.rankID, myRank09);

            Tuple<String, String>[] myTitle11 = new Tuple<string, string>[3];
            myTitle11[0] = new Tuple<string, string>("langC", "Earl");
            myTitle11[1] = new Tuple<string, string>("E", "Earl");
            myTitle11[2] = new Tuple<string, string>("langF", "Comte");
            Rank myRank11 = new Rank("11", myTitle11, 4);
            this.rankMasterList.Add(myRank11.rankID, myRank11);

            Tuple<String, String>[] myTitle15 = new Tuple<string, string>[3];
            myTitle15[0] = new Tuple<string, string>("langC", "Baron");
            myTitle15[1] = new Tuple<string, string>("E", "Baron");
            myTitle15[2] = new Tuple<string, string>("langF", "Baron");
            Rank myRank15 = new Rank("15", myTitle15, 2);
            this.rankMasterList.Add(myRank15.rankID, myRank15);

            Tuple<String, String>[] myTitle17 = new Tuple<string, string>[3];
            myTitle17[0] = new Tuple<string, string>("langC", "Lord");
            myTitle17[1] = new Tuple<string, string>("E", "Lord");
            myTitle17[2] = new Tuple<string, string>("langF", "Sire");
            Rank myRank17 = new Rank("17", myTitle17, 1);
            this.rankMasterList.Add(myRank17.rankID, myRank17);

            // create kingdoms for provinces
            Kingdom myKingdom1 = new Kingdom("E0000", "England", r: myRank03);
            this.kingdomMasterList.Add(myKingdom1.kingdomID, myKingdom1);
            Kingdom myKingdom2 = new Kingdom("B0000", "Boogiboogiland", r: myRank03);
            this.kingdomMasterList.Add(myKingdom2.kingdomID, myKingdom2);

            // create provinces for fiefs
            Province myProv = new Province("ESX00", "Sussex", 6.2, king: myKingdom1, ra: myRank11);
			this.provinceMasterList.Add (myProv.provinceID, myProv);
            Province myProv2 = new Province("ESR00", "Surrey", 6.2, king: myKingdom2, ra: myRank11);
			this.provinceMasterList.Add (myProv2.provinceID, myProv2);

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang1, plains, fief1Chars, keep1BarChars, false, false, this.clock, 0, ra: myRank17);
			fiefMasterList.Add(myFief1.fiefID, myFief1);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 1000, 1000, 2000, 2000, 10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', myLang1, hills, fief2Chars, keep2BarChars, false, false, this.clock, 0, ra: myRank15);
			fiefMasterList.Add(myFief2.fiefID, myFief2);
            Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang1, plains, fief3Chars, keep3BarChars, false, false, this.clock, 0, ra: myRank17);
			fiefMasterList.Add(myFief3.fiefID, myFief3);
            Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang1, plains, fief4Chars, keep4BarChars, false, false, this.clock, 0, ra: myRank17);
			fiefMasterList.Add(myFief4.fiefID, myFief4);
            Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang1, plains, fief5Chars, keep5BarChars, false, false, this.clock, 0, ra: myRank15);
			fiefMasterList.Add(myFief5.fiefID, myFief5);
            Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang3, plains, fief6Chars, keep6BarChars, false, false, this.clock, 0, ra: myRank17);
			fiefMasterList.Add(myFief6.fiefID, myFief6);
            Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', myLang3, forrest, fief7Chars, keep7BarChars, false, false, this.clock, 0, ra: myRank15);
			fiefMasterList.Add(myFief7.fiefID, myFief7);
			Army myArmy = new Army("army001", 0, 0, 0, 0, 100, 0, "101", "401", 90, this.clock);

			// create QuickGraph undirected graph
			// 1. create graph
			var myHexMap = new HexMapGraph("map001");
			this.gameMap = myHexMap;
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

			// add some goTo entries for myChar1
			//myGoTo1.Enqueue (myFief2);
			//myGoTo1.Enqueue (myFief7);

			// create entourages for PCs
			List<NonPlayerCharacter> myEmployees1 = new List<NonPlayerCharacter>();
			List<NonPlayerCharacter> myEmployees2 = new List<NonPlayerCharacter>();

			// create lists of fiefs owned by PCs and add some fiefs
			List<Fief> myFiefsOwned1 = new List<Fief>();
			List<Fief> myFiefsOwned2 = new List<Fief>();

            // create Random for use with generating skill sets for Characters
            Random myRand = new Random();
            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("101", "Dave Bond", 50, true, "Fr", 1.0, 8.50, 6.0, myGoTo1, myLang1, 90, 4.0, 7.2, 6.1, generateSkillSet(myRand), false, true, false, "200", "403", "", false, 13000, myEmployees1, myFiefsOwned1, cl: this.clock, loc: myFief1);
			pcMasterList.Add(myChar1.charID, myChar1);
            PlayerCharacter myChar2 = new PlayerCharacter("102", "Bave Dond", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo2, myLang1, 90, 4.0, 5.0, 4.5, generateSkillSet(myRand), false, false, false, "200", "", "", false, 13000, myEmployees2, myFiefsOwned2, cl: this.clock, loc: myFief1);
			pcMasterList.Add(myChar2.charID, myChar2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("401", "Jimmy Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo3, myLang1, 90, 4.0, 3.3, 6.7, generateSkillSet(myRand), false, false, false, "200", "", "", 0, false, cl: this.clock, loc: myFief1);
			npcMasterList.Add(myNPC1.charID, myNPC1);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("402", "Johnny Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo4, myLang1, 90, 4.0, 7.1, 5.2, generateSkillSet(myRand), false, false, false, "200", "", "", 10000, true, mb: myChar1.charID, cl: this.clock, loc: myFief1);
			npcMasterList.Add(myNPC2.charID, myNPC2);
            NonPlayerCharacter myWife = new NonPlayerCharacter("403", "Molly Maguire", 50, false, "Eng", 1.0, 8.50, 6.0, myGoTo5, myLang2, 90, 4.0, 4.0, 6.0, generateSkillSet(myRand), false, true, true, "200", "", "", 0, false, cl: this.clock, loc: myFief1);
			npcMasterList.Add(myWife.charID, myWife);

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

            // set kings
            myKingdom1.king = myChar1;
            myKingdom2.king = myChar2;

            // set fief bailiffs
            myFief1.bailiff = myChar1;
			myFief2.bailiff = myNPC2;

			// add NPC to employees
            myChar1.hireNPC(myNPC2, 12000);
			// set employee as travelling companion
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
			myFief2.barCharacter(myNPC1.charID);
            myFief2.barCharacter(myChar2.charID);
            myFief2.barCharacter(myWife.charID);

			// try retrieving fief from masterlist using fiefID
			// Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");

		}

		/// <summary>
		/// Generates a random skill set for a Character
		/// </summary>
        /// <returns>Tuple<Skill, int>[] for use with a Character object</returns>
        /// <param name="myRand">Random to use for integer generation</param>
        public Tuple<Skill, int>[] generateSkillSet(Random myRand)
        {
            // create new random for generating skills for Character
            Random rndSkills = myRand;

            // create array of skills between 2-3 in length
            Tuple<Skill, int>[] skillSet = new Tuple<Skill, int>[rndSkills.Next(2, 4)];

            // populate array of skills with randomly chosen skills
            // 1) make temporary copy of skillKeys
            List<string> skillKeysCopy = new List<string>(this.skillKeys);
            // 2) choose random skill, removing entry from keys list to ensure no duplication
            // Also assign random skill level
            for (int i = 0; i < skillSet.Length; i++)
            {
                // choose random skill
                int randSkill = rndSkills.Next(0, skillKeysCopy.Count - 1);
                // assign random skill level
                int randSkillLevel = rndSkills.Next(1, 10);
                // create Skill tuple
                skillSet[i] = new Tuple<Skill, int>(this.skillMasterList[skillKeysCopy[randSkill]], randSkillLevel);
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
			// write clock
			this.writeClock (gameID, this.clock);

			// write skills
            // clear existing key list
			if (this.skillKeys.Count > 0)
			{
				this.skillKeys.Clear ();
			}

            // write each object in skillMasterList, whilst also repopulating key list
			foreach (KeyValuePair<String, Skill> pair in this.skillMasterList)
			{
				bool success = this.writeSkill (gameID, pair.Value);
				if (success)
				{
					this.skillKeys.Add (pair.Key);
				}
			}

            // write key list to database
			this.writeKeyList (gameID, "skillKeys", this.skillKeys);

            // write Languages
            // clear existing key list
            if (this.langKeys.Count > 0)
            {
                this.langKeys.Clear();
            }

            // write each object in languageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Language> pair in this.languageMasterList)
            {
                bool success = this.writeLanguage(gameID, pair.Value);
                if (success)
                {
                    this.langKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "langKeys", this.langKeys);

            // write Ranks
            // clear existing key list
            if (this.rankKeys.Count > 0)
            {
                this.rankKeys.Clear();
            }

            // write each object in rankMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Rank> pair in this.rankMasterList)
            {
                bool success = this.writeRank(gameID, pair.Value);
                if (success)
                {
                    this.rankKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "rankKeys", this.rankKeys);

            // write NPCs
            // clear existing key list
            if (this.npcKeys.Count > 0)
			{
				this.npcKeys.Clear ();
			}

            // write each object in npcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, NonPlayerCharacter> pair in this.npcMasterList)
			{
				bool success = this.writeNPC (gameID, pair.Value);
				if (success)
				{
					this.npcKeys.Add (pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "npcKeys", this.npcKeys);

			// write PCs
            // clear existing key list
            if (this.pcKeys.Count > 0)
			{
				this.pcKeys.Clear ();
			}

            // write each object in pcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, PlayerCharacter> pair in this.pcMasterList)
			{
				bool success = this.writePC (gameID, pair.Value);
				if (success)
				{
					this.pcKeys.Add (pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "pcKeys", this.pcKeys);

            // write Kingdoms
            // clear existing key list
            if (this.kingKeys.Count > 0)
            {
                this.kingKeys.Clear();
            }

            // write each object in kingdomMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Kingdom> pair in this.kingdomMasterList)
            {
                bool success = this.writeKingdom(gameID, pair.Value);
                if (success)
                {
                    this.kingKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.writeKeyList(gameID, "kingKeys", this.kingKeys);

            // write Provinces
            // clear existing key list
            if (this.provKeys.Count > 0)
			{
				this.provKeys.Clear ();
			}

            // write each object in provinceMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Province> pair in this.provinceMasterList)
			{
				bool success = this.writeProvince (gameID, pair.Value);
				if (success)
				{
					this.provKeys.Add (pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "provKeys", this.provKeys);

			// write Terrains
            // clear existing key list
            if (this.terrKeys.Count > 0)
			{
				this.terrKeys.Clear ();
			}

            // write each object in terrainMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Terrain> pair in this.terrainMasterList)
			{
				bool success = this.writeTerrain (gameID, pair.Value);
				if (success)
				{
					this.terrKeys.Add (pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "terrKeys", this.terrKeys);

			// write Fiefs
            // clear existing key list
            if (this.fiefKeys.Count > 0)
			{
				this.fiefKeys.Clear ();
			}

            // write each object in fiefMasterList, whilst also repopulating key list
            foreach (KeyValuePair<String, Fief> pair in this.fiefMasterList)
			{
				bool success = this.writeFief (gameID, pair.Value);
				if (success)
				{
					this.fiefKeys.Add (pair.Key);
				}
			}

            // write key list to database
            this.writeKeyList(gameID, "fiefKeys", this.fiefKeys);

			// write map (edges collection)
			this.writeMapEdges (gameID, this.gameMap);

		}

		/// <summary>
		/// Loads all objects for a particular game from database
		/// </summary>
        /// <param name="gameID">ID of game (Riak bucket)</param>
		public void initialDBload(String gameID)
		{

			// load key lists (to ensure efficient retrieval of specific game objects)
			this.initialDBload_keyLists (gameID);

			// load clock
			this.clock = this.initialDBload_clock (gameID, "gameClock");

			// load skills
			foreach (String element in this.skillKeys)
			{
				Skill skill = this.initialDBload_skill (gameID, element);
                // add Skill to skillMasterList
				this.skillMasterList.Add(skill.skillID, skill);
			}
				
            // load Languages
            foreach (String element in this.langKeys)
            {
                Language lang = this.initialDBload_language(gameID, element);
                // add Language to languageMasterList
                this.languageMasterList.Add(lang.languageID, lang);
            }

            // load Ranks
            foreach (String element in this.rankKeys)
            {
                Rank rank = this.initialDBload_rank(gameID, element);
                // add Rank to rankMasterList
                this.rankMasterList.Add(rank.rankID, rank);
            }

            // load NPCs
			foreach (String element in this.npcKeys)
			{
				NonPlayerCharacter npc = this.initialDBload_NPC (gameID, element);
                // add NPC to npcMasterList
                this.npcMasterList.Add(npc.charID, npc);
			}

			// load PCs
			foreach (String element in this.pcKeys)
			{
				PlayerCharacter pc = this.initialDBload_PC (gameID, element);
                // add PC to pcMasterList
                this.pcMasterList.Add(pc.charID, pc);
			}

            // load kingdoms
            foreach (String element in this.kingKeys)
            {
                Kingdom king = this.initialDBload_Kingdom(gameID, element);
                // add Kingdom to kingdomMasterList
                this.kingdomMasterList.Add(king.kingdomID, king);
            }

            // load provinces
			foreach (String element in this.provKeys)
			{
				Province prov = this.initialDBload_Province (gameID, element);
                // add Province to provinceMasterList
                this.provinceMasterList.Add(prov.provinceID, prov);
			}

			// load terrains
			foreach (String element in this.terrKeys)
			{
				Terrain terr = this.initialDBload_terrain (gameID, element);
                // add Terrain to terrainMasterList
                this.terrainMasterList.Add(terr.terrainCode, terr);
			}

			// load fiefs
			foreach (String element in this.fiefKeys)
			{
				Fief f = this.initialDBload_Fief (gameID, element);
                // add Fief to fiefMasterList
                this.fiefMasterList.Add(f.fiefID, f);
			}

			// process any Character goTo queues containing entries
			if (this.goToList.Count > 0)
			{
				for (int i = 0; i < this.goToList.Count; i++)
				{
					this.populate_goTo (this.goToList[i]);
				}
				this.goToList.Clear();
			}

			// load map
			this.gameMap = this.initialDBload_map (gameID, "mapEdges");
		}

		/// <summary>
		/// Loads all Riak key lists for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which key lists to be retrieved</param>
		public void initialDBload_keyLists(String gameID)
		{
            // populate skillKeys
			var skillKeyResult = client.Get(gameID, "skillKeys");
			if (skillKeyResult.IsSuccess)
			{
				this.skillKeys = skillKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skillKeys from database.");
			}

			// populate langKeys
			var langKeyResult = client.Get(gameID, "langKeys");
			if (langKeyResult.IsSuccess)
			{
				this.langKeys = langKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve langKeys from database.");
			}

            // populate rankKeys
            var rankKeyResult = client.Get(gameID, "rankKeys");
            if (rankKeyResult.IsSuccess)
            {
                this.rankKeys = rankKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve rankKeys from database.");
            }

            // populate npcKeys
            var npcKeyResult = client.Get(gameID, "npcKeys");
			if (npcKeyResult.IsSuccess)
			{
				this.npcKeys = npcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve npcKeys from database.");
			}

            // populate pcKeys
            var pcKeyResult = client.Get(gameID, "pcKeys");
			if (pcKeyResult.IsSuccess)
			{
				this.pcKeys = pcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve pcKeys from database.");
			}

            // populate kingKeys
            var kingKeyResult = client.Get(gameID, "kingKeys");
            if (kingKeyResult.IsSuccess)
            {
                this.kingKeys = kingKeyResult.Value.GetObject<List<String>>();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve kingKeys from database.");
            }

            // populate provKeys
            var provKeyResult = client.Get(gameID, "provKeys");
			if (provKeyResult.IsSuccess)
			{
				this.provKeys = provKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve provKeys from database.");
			}

            // populate terrKeys
            var terrKeyResult = client.Get(gameID, "terrKeys");
			if (terrKeyResult.IsSuccess)
			{
				this.terrKeys = terrKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve terrKeys from database.");
			}

            // populate fiefKeys
            var fiefKeyResult = client.Get(gameID, "fiefKeys");
			if (fiefKeyResult.IsSuccess)
			{
				this.fiefKeys = fiefKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve fiefKeys from database.");
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
			var clockResult = client.Get(gameID, clockID);
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
		/// Loads a skill for a particular game from database
		/// </summary>
        /// <returns>Skill object</returns>
        /// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="skillID">ID of skill to be retrieved</param>
		public Skill initialDBload_skill(String gameID, String skillID)
		{
			var skillResult = client.Get(gameID, skillID);
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
			var npcResult = client.Get(gameID, npcID);
			var npcRiak = new NonPlayerCharacter_Riak();
			NonPlayerCharacter myNPC = new NonPlayerCharacter ();

			if (npcResult.IsSuccess)
			{
                // extract NonPlayerCharacter_Riak object
				npcRiak = npcResult.Value.GetObject<NonPlayerCharacter_Riak>();
                // if NonPlayerCharacter_Riak goTo queue contains entries, store for later processing
				if (npcRiak.goTo.Count > 0)
				{
					goToList.Add (npcRiak);
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
			var pcResult = client.Get(gameID, pcID);
			var pcRiak = new PlayerCharacter_Riak();
			PlayerCharacter myPC = new PlayerCharacter ();

			if (pcResult.IsSuccess)
			{
                // extract PlayerCharacter_Riak object
                pcRiak = pcResult.Value.GetObject<PlayerCharacter_Riak>();
                // if PlayerCharacter_Riak goTo queue contains entries, store for later processing
                if (pcRiak.goTo.Count > 0)
				{
					goToList.Add (pcRiak);
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
            var kingResult = client.Get(gameID, kingID);
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
			var provResult = client.Get(gameID, provID);
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
			var terrainResult = client.Get(gameID, terrID);
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
            var languageResult = client.Get(gameID, langID);
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
            var rankResult = client.Get(gameID, rankID);
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
			var fiefResult = client.Get(gameID, fiefID);
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
		/// Creates HexMapGraph for a particular game,
		/// using map edges collection retrieved from database
		/// </summary>
        /// <returns>HexMapGraph object</returns>
        /// <param name="gameID">Game for which map to be created</param>
		/// <param name="mapEdgesID">ID of map edges collection to be retrieved</param>
		public HexMapGraph initialDBload_map(String gameID, String mapEdgesID)
		{
			var mapResult = client.Get(gameID, mapEdgesID);
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

			// insert game clock
			fOut.clock = this.clock;

			// insert province
			fOut.province = this.provinceMasterList[fr.province];

            // insert language
            fOut.language = new Tuple<Language,int>(this.languageMasterList[fr.language.Item1], fr.language.Item2);

            // insert owner
			fOut.owner = this.pcMasterList[fr.owner];
			// check if fief is in owner's list of fiefs owned
			bool fiefInList = fOut.owner.ownedFiefs.Any(item => item.fiefID == fOut.fiefID);
			// if not, add it
			if(! fiefInList)
			{
				fOut.owner.ownedFiefs.Add(fOut);
			}

			// insert ancestral owner
			fOut.ancestralOwner = this.pcMasterList[fr.ancestralOwner];

			// insert bailiff (PC or NPC)
			if (fr.bailiff != null)
			{
				if (this.npcMasterList.ContainsKey (fr.bailiff)) {
					fOut.bailiff = this.npcMasterList [fr.bailiff];
				} else if (this.pcMasterList.ContainsKey (fr.bailiff)) {
					fOut.bailiff = this.pcMasterList [fr.bailiff];
				} else {
					fOut.bailiff = null;
					System.Windows.Forms.MessageBox.Show ("Unable to identify bailiff (" + fr.bailiff + ") for Fief " + fOut.fiefID);
				}
			}
				
			//insert terrain
			fOut.terrain = this.terrainMasterList[fr.terrain];

			// insert characters
			if (fr.characters.Count > 0)
			{
				for (int i = 0; i < fr.characters.Count; i++)
				{
					if (this.npcMasterList.ContainsKey (fr.characters[i]))
					{
						fOut.characters.Add(this.npcMasterList[fr.characters[i]]);
						this.npcMasterList[fr.characters[i]].location = fOut;
					}
                    else if (this.pcMasterList.ContainsKey(fr.characters[i]))
                    {
                        fOut.characters.Add(this.pcMasterList[fr.characters[i]]);
                        this.pcMasterList[fr.characters[i]].location = fOut;
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
                if (this.rankMasterList.ContainsKey(fr.rankID))
                {
                    fOut.rank = rankMasterList[fr.rankID];
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

			// insert game clock
			pcOut.clock = this.clock;

            // insert language
            pcOut.language = new Tuple<Language, int>(this.languageMasterList[pcr.language.Item1], pcr.language.Item2);

            // insert skills
			if (pcr.skills.Length > 0)
			{
				for (int i = 0; i < pcr.skills.Length; i++)
				{
                    pcOut.skills[i] = new Tuple<Skill, int>(this.skillMasterList[pcr.skills[i].Item1], pcr.skills[i].Item2);
				}
			}

			// insert employees
			if (pcr.employees.Count > 0)
			{
				for (int i = 0; i < pcr.employees.Count; i++)
				{
                    pcOut.employees.Add (npcMasterList[pcr.employees[i]]);
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

			// insert game clock
			npcOut.clock = this.clock;

            // insert language
            npcOut.language = new Tuple<Language, int>(this.languageMasterList[npcr.language.Item1], npcr.language.Item2);
            
            // insert skills
			if (npcr.skills.Length > 0)
			{
				for (int i = 0; i < npcr.skills.Length; i++)
				{
                    npcOut.skills[i] = new Tuple<Skill, int>(this.skillMasterList[npcr.skills[i].Item1], npcr.skills[i].Item2);
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
			TaggedEdge<Fief, string> edgeOut = new TaggedEdge<Fief, string>(this.fiefMasterList[te.Source], this.fiefMasterList[te.Target], te.Tag);
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
				if (this.pcMasterList.ContainsKey(cr.charID))
				{
					myCh = this.pcMasterList [cr.charID];
					success = true;
				}
			}
			else if (cr is NonPlayerCharacter_Riak)
			{
				if (this.npcMasterList.ContainsKey(cr.charID))
				{
					myCh = this.npcMasterList [cr.charID];
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
                    myCh.goTo.Enqueue(this.fiefMasterList[value]);
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
                if (this.pcMasterList.ContainsKey(kr.kingID))
                {
                    oOut.king = pcMasterList[kr.kingID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Kingdom " + kr.kingdomID + ": King not found (" + kr.kingID + ")");
                }
            }

            // insert rank
            if (kr.rankID != null)
            {
                if (this.rankMasterList.ContainsKey(kr.rankID))
                {
                    oOut.rank = rankMasterList[kr.rankID];
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
                if (this.pcMasterList.ContainsKey(pr.overlordID))
                {
                    oOut.overlord = pcMasterList[pr.overlordID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Province " + pr.provinceID + ": Overlord not found (" + pr.overlordID + ")");
                }
            }

            // insert kingdom using kingdomID
            if (pr.kingdomID != null)
            {
                if (this.kingdomMasterList.ContainsKey(pr.kingdomID))
                {
                    oOut.kingdom = kingdomMasterList[pr.kingdomID];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Province " + pr.provinceID + ": Kingdom not found (" + pr.kingdomID + ")");
                }
            }

            // insert rank using rankID
            if (pr.rankID != null)
            {
                if (this.rankMasterList.ContainsKey(pr.rankID))
                {
                    oOut.rank = rankMasterList[pr.rankID];
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
			var putListResult = client.Put(rList);

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
			var putClockResult = client.Put(rClock);

			if (! putClockResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: GameClock to bucket " + rClock.Bucket);
			}

			return putClockResult.IsSuccess;
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
			var putSkillResult = client.Put(rSkill);

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
			var putNPCresult = client.Put(rNPC);

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
			var putPCresult = client.Put(rPC);

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
			var putKingResult = client.Put(rKing);

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
            var putProvResult = client.Put(rProv);

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
			var putLanguageResult = client.Put(rLanguage);

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
            var putRankResult = client.Put(rRank);

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
            var putTerrainResult = client.Put(rTerrain);

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
			var putFiefResult = client.Put(rFief);

			if (! putFiefResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Fief " + rFief.Key + " to bucket " + rFief.Bucket);
			}

			return putFiefResult.IsSuccess;
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
			var putMapResultE = client.Put(rMapE);

			if (! putMapResultE.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Map edges collection " + rMapE.Key + " to bucket " + rMapE.Bucket);
			}

			return putMapResultE.IsSuccess;
		}

        /// <summary>
		/// Updates game objects at end/start of season
		/// </summary>
		public void seasonUpdate()
		{
			// fiefs
			foreach (KeyValuePair<string, Fief> fiefEntry in this.fiefMasterList)
			{
				fiefEntry.Value.updateFief();
			}

            // PlayerCharacters
			foreach (KeyValuePair<string, PlayerCharacter> pcEntry in this.pcMasterList)
			{
				if (pcEntry.Value.health != 0)
				{
					pcEntry.Value.updateCharacter();
				}
			}

            // NonPlayerCharacters
			foreach (KeyValuePair<string, NonPlayerCharacter> npcEntry in this.npcMasterList)
			{
				if (npcEntry.Value.health != 0)
				{
                    npcEntry.Value.updateCharacter();
                    // NPC-specific
                    this.seasonUpdateNPC(npcEntry.Value);
				}
			}

            // GameClock (advance season)
            this.clock.advanceSeason();
        }

        /// <summary>
        /// End/start of season updates specific to NPC objects
        /// </summary>
        /// <param name="npc">NPC to update</param>
        public void seasonUpdateNPC(NonPlayerCharacter npc)
        {
            // random move if has no boss
            this.randomMoveNPC(npc);
        }

        /// <summary>
        /// Moves an NPC without a boss one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="npc">NPC to move</param>
        public bool randomMoveNPC(NonPlayerCharacter npc)
        {
            bool success = false;

            if (npc.myBoss == null)
            {
                Fief target = this.gameMap.chooseRandomHex(npc.location);
                if (target != null)
                {
                    double travelCost = this.getTravelCost(npc.location, target);
                    success = npc.moveCharacter(target, travelCost);
                }
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
            this.meetingPlaceCharsListView.Columns.Add("Household", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Sex", -2, HorizontalAlignment.Left);
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
        /// Refreshes display of PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void refreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            ListViewItem[] fiefsOwned = new ListViewItem[this.myChar.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < this.myChar.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                fiefsOwned[i] = new ListViewItem(this.myChar.ownedFiefs[i].name);
                fiefsOwned[i].SubItems.Add(this.myChar.ownedFiefs[i].fiefID);
                // indicate if fief is current location
                if (this.myChar.ownedFiefs[i] == this.myChar.location)
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
            // set label
            switch (place)
            {
                case "tavern":
                    this.meetingPlaceLabel.Text = "Ye Olde Tavern";
                    break;
                case "court":
                    this.meetingPlaceLabel.Text = "The Esteemed Court Of " + this.fiefToView.name;
                    break;
                case "outsideKeep":
                    this.meetingPlaceLabel.Text = "Hangin' Outside The Keep";
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
            textToDisplay += this.clock.seasons[this.clock.currentSeason] + ", " + this.clock.currentYear + ".  Your days left: " + this.myChar.days + "\r\n\r\n";
            // Fief name/ID and province name
            textToDisplay += "Fief: " + this.myChar.location.name + " (" + this.myChar.location.fiefID + ")  in " + this.myChar.location.province.name + ", " + this.myChar.location.province.kingdom.name + "\r\n\r\n";
            // Fief owner
            textToDisplay += "Owner: " + this.myChar.location.owner.name + "\r\n";
            // Fief overlord
            textToDisplay += "Overlord: " + this.myChar.location.province.overlord.name + "\r\n";

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
            for (int i = 0; i < this.myChar.location.characters.Count; i++)
            {
                ListViewItem charsInCourt = null;

                // only display characters in relevant location (in keep, or not)
                if (this.myChar.location.characters[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (this.myChar.location.characters[i] != this.myChar)
                    {

                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (this.myChar.location.characters[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((this.myChar.location.characters[i] as NonPlayerCharacter).wage == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsInCourt = this.createMeetingPlaceListItem(this.myChar.location.characters[i]);
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsInCourt = this.createMeetingPlaceListItem(this.myChar.location.characters[i]);
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
            ListViewItem myItem = new ListViewItem(ch.name);

            // charID
            myItem.SubItems.Add(ch.charID);

            // TODO: household
            myItem.SubItems.Add("A household");

            // sex
            if (ch.isMale)
            {
                myItem.SubItems.Add("Male");
            }
            else
            {
                myItem.SubItems.Add("Female");
            }

            // TODO: Type (e.g. family or not)
            myItem.SubItems.Add("A type");

            // show whether is in player's entourage
            bool isCompanion = false;

            if (ch is NonPlayerCharacter)
            {
                // iterate through employees checking for character
                for (int i = 0; i < this.myChar.employees.Count; i++)
                {
                    if (this.myChar.employees[i] == ch)
                    {
                        if (this.myChar.employees[i].inEntourage)
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
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        public void refreshHouseholdDisplay(NonPlayerCharacter npc = null)
        {
            // remove any previously displayed characters
            this.houseCharTextBox.ReadOnly = true;
            this.houseCharTextBox.Text = "";

            // ensure unable to use controls whilst no NPC selected in ListView
            this.houseCampBtn.Enabled = false;
            this.houseCampDaysTextBox.Text = "";
            this.houseCampDaysTextBox.Enabled = false;

            // clear existing items in characters list
            this.houseCharListView.Items.Clear();

            // iterates through characters
            for (int i = 0; i < this.myChar.employees.Count; i++)
            {
                ListViewItem houseChar = null;

                // name
                houseChar = new ListViewItem(this.myChar.employees[i].name);

                // charID
                houseChar.SubItems.Add(this.myChar.employees[i].charID);

                // TODO: Function (e.g. NPC, Son, etc.)
                houseChar.SubItems.Add("A function");

                // location
                houseChar.SubItems.Add(this.myChar.employees[i].location.fiefID + " (" + this.myChar.employees[i].location.name + ")");

                // show whether is in player's entourage
                if (this.myChar.employees[i].inEntourage)
                {
                    houseChar.SubItems.Add("Yes");
                }

                if (houseChar != null)
                {
                    // if NPC passed in as parameter, show as selected
                    if (this.myChar.employees[i] == npc)
                    {
                        houseChar.Selected = true;
                    }

                    // add item to fiefsListView
                    this.houseCharListView.Items.Add(houseChar);
                }

            }

            this.houseCharListView.HideSelection = false;
            this.houseCharListView.Focus();
        }

        /// <summary>
        /// Retrieves information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="ch">Character whose information is to be displayed</param>
        public string displayCharacter(Character ch)
        {
            string charText = "";

            // ID
            charText += "ID: " + ch.charID + "\r\n";

            // name
            charText += "Name: " + ch.name + "\r\n";

            // age
            charText += "Age: " + ch.age + "\r\n";

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

            // health (& max. health)
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

            // any death modifiers (from skills)
            charText += "  (Death modifier from skills: " + ch.calcSkillEffect("death") + ")\r\n";

            // virility
            charText += "Virility: " + ch.virility + "\r\n";

            // location
            charText += "Current location: " + ch.location.name + " (" + ch.location.province.name + ")\r\n";
            
            // if in process of auto-moving, display next hex
            if (ch.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + ch.goTo.Peek().fiefID + "\r\n";
            }

            // language
            charText += "Language: " + ch.language.Item1.name + " (dialect " + ch.language.Item2 + ")\r\n";

            // days left
            charText += "Days remaining: " + ch.days + "\r\n";

            // stature
            charText += "Stature: " + ch.stature + "\r\n";

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
            if (ch.married)
            {
                charText += "happily married";
            }
            else
            {
                charText += "single and lonely";
            }
            charText += "\r\n";

            // spouse ID
            if (ch.married)
            {
                charText += "Your spouse's ID is: " + ch.spouse + "\r\n";
            }

            // if pregnant
            if (!ch.isMale)
            {
                charText += "You are ";
                if (!ch.pregnant)
                {
                    charText += "not ";
                }
                charText += "pregnant\r\n";
            }

            // if spouse pregnant
            else
            {
                if (ch.married)
                {
                    NonPlayerCharacter thisSpouse = npcMasterList[ch.spouse];
                    if (thisSpouse.pregnant)
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
            if (ch.familyHead != null)
            {
                charText += ch.familyHead;
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
                charText += this.displayPlayerCharacter((PlayerCharacter)ch);
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
            for (int i = 0; i < pc.employees.Count; i++)
            {
                pcText += "  - " + pc.employees[i].name;
                if (pc.employees[i].inEntourage)
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
                foreach (KeyValuePair<string, Kingdom> entry in this.kingdomMasterList)
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
                foreach (KeyValuePair<string, Province> entry in this.provinceMasterList)
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

                // owned fiefs
                for (int i = 0; i < pc.ownedFiefs.Count; i++ )
                {
                    for (int j = 0; j < pc.ownedFiefs[i].rank.title.Length; j++)
                    {
                        if (pc.ownedFiefs[i].rank.title[j].Item1 == pc.ownedFiefs[i].language.Item1.languageID)
                        {
                            pcText += pc.ownedFiefs[i].rank.title[j].Item2 + " (rank " + pc.ownedFiefs[i].rank.rankID + ") of ";
                            break;
                        }
                    }
                    // get fief details
                    pcText += pc.ownedFiefs[i].name + " (" + pc.ownedFiefs[i].fiefID + ")\r\n";
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
            npcText += "Potential salary: " + npc.calcWage(this.myChar) + "\r\n";

            // most recent salary offer from player (if any)
            npcText += "Last offer from this PC: ";
            if (npc.lastOffer.ContainsKey(this.myChar.charID))
            {
                npcText += npc.lastOffer[this.myChar.charID];
            }
            else
            {
                npcText += "N/A";
            }
            npcText += "\r\n";

            // current salary
            npcText += "Current salary: " + npc.wage + "\r\n";

            return npcText;
        }

        /// <summary>
        /// Retrieves information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief whose information is to be displayed</param>
        public string displayFief(Fief f)
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

            // no. of troops
            fiefText += "Troops: " + f.troops + "\r\n";

            // fief status
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

            // language
            fiefText += "Language: " + f.language.Item1.name + " (dialect " + f.language.Item2 + ")\r\n";

            // terrain type
            fiefText += "Terrain: " + f.terrain.description + "\r\n";

            // characters present
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

            // fief management section
            fiefText += "========= Management ==========\r\n\r\n";

            // loyalty
            fiefText += "Loyalty: " + (f.loyalty + (f.loyalty * f.calcBlfLoyAdjusted(f.bailiffDaysInFief >= 30))) + "\r\n";
            // loyalty modifier for officials spend
            fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod("this") + ")\r\n";
            // loyalty modifier for garrison spend
            fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod("this") + ")\r\n";
            // loyalty modifier for bailiff
            fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted(f.bailiffDaysInFief >= 30) + ")\r\n";
            // loyalty modifier for bailiff skills
            fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod(f.bailiffDaysInFief >= 30) + ")\r\n";
            
            // fields
            fiefText += "Fields level: " + f.fields + "\r\n";

            // industry
            fiefText += "Industry level: " + f.industry + "\r\n";

            // GDP
            fiefText += "GDP: " + f.calcGDP("this") + "\r\n";

            // tax rate
            fiefText += "Tax rate: " + f.taxRate + "\r\n";

            // officials spend
            fiefText += "Officials expenditure: " + f.officialsSpend + " (modifier: " + f.calcOffIncMod("this") + ")\r\n";

            // garrison spend
            fiefText += "Garrison expenditure: " + f.garrisonSpend + "\r\n";

            // infrastructure spend
            fiefText += "Infrastructure expenditure: " + f.infrastructureSpend + "\r\n";

            // keep spend
            fiefText += "Keep expenditure: " + f.keepSpend + "\r\n";

            // keep level
            fiefText += "Keep level: " + f.keepLevel + "\r\n";

            // income
            fiefText += "Income: " + (f.calcIncome("this") * f.calcStatusIncmMod()) + "\r\n";
            // income modifier for bailiff
            fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod(f.bailiffDaysInFief >= 30) + ")\r\n";
            // income modifier for officials spend
            fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod("this") + ")\r\n";
            // income modifier for fief status
            fiefText += "  (including fief status income modifier: " + f.calcStatusIncmMod() + ")\r\n";

            // family expenses
            fiefText += "Family expenses: 0 (not yet implemented)\r\n";

            // total expenses
            fiefText += "Total expenses: " + f.calcExpenses("this") + "\r\n";
            // expenses modifier for bailiff
            fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif(f.bailiffDaysInFief >= 30) + ")\r\n";
            
            // overlord taxes
            fiefText += "Overlord taxes: " + f.calcOlordTaxes("this") + "\r\n";

            // surplus
            fiefText += "Bottom line: " + f.calcBottomLine("this") + "\r\n\r\n";

            // projected figures for next season (as above)
            fiefText += "========= Next season =========\r\n";
            fiefText += "(with current bailiff & oLord tax)\r\n";
            fiefText += " (NOT including effects of status)\r\n\r\n";

            fiefText += "Loyalty: " + f.calcNewLoyalty() + "\r\n";
            fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod("next") + ")\r\n";
            fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod("next") + ")\r\n";
            fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted(f.bailiffDaysInFief >= 30) + ")\r\n";
            fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod(f.bailiffDaysInFief >= 30) + ")\r\n";
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
            fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod(f.bailiffDaysInFief >= 30) + ")\r\n";
            fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod("next") + ")\r\n";
            fiefText += "Family expenses: 0 (not yet implemented)\r\n";
            fiefText += "Total expenses: " + f.calcExpenses("next") + "\r\n";
            fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif(f.bailiffDaysInFief >= 30) + ")\r\n";
            fiefText += "Overlord taxes: " + f.calcOlordTaxes("next") + "\r\n";
            fiefText += "Bottom line: " + f.calcBottomLine("next") + "\r\n\r\n";

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
                ch = this.myChar;
            }

            string textToDisplay = "";

            // get information to display
            textToDisplay += this.displayCharacter(ch);

            // refresh Character display TextBox
            this.characterTextBox.ReadOnly = true;
            this.characterTextBox.Text = textToDisplay;

            // clear previous entries in Camp TextBox
            this.travelCampDaysTextBox.Text = "";

            // multimove button only enabled if is player or an employee
            if (ch != this.myChar)
			{
                if (!this.myChar.employees.Contains(ch))
				{
					this.travelMultiMoveBtn.Enabled = false;
				}
			}

            this.characterContainer.BringToFront();
        }

        /// <summary>
        /// Responds to the click event of the personalCharacteristicsAndAffairsToolStripMenuItem
        /// which displays main Character information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void personalCharacteristicsAndAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.charToView = this.myChar;
            this.refreshCharacterContainer(this.charToView);
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
                f = this.myChar.location;
            }

            this.fiefToView = f;

            string textToDisplay = "";

            // set label text
            this.fiefLabel.Text = this.fiefToView.name + " (" + this.fiefToView.fiefID + ")";

            // get information to display
            textToDisplay += this.displayFief(this.fiefToView);

            // refresh Character display TextBox
            this.fiefTextBox.Text = textToDisplay;
            this.fiefTextBox.ReadOnly = true;

            // if fief is NOT owned by player, disable fief management buttons and TextBoxes 
            if (!myChar.ownedFiefs.Contains(this.fiefToView))
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
                this.updateFiefBtn.Enabled = false;
                this.lockoutBtn.Enabled = false;
                this.selfBailiffBtn.Enabled = false;
                this.setBailiffBtn.Enabled = false;
                this.removeBaliffBtn.Enabled = false;

                // set TextBoxes to nowt
                this.adjGarrSpendTextBox.Text = "";
                this.adjInfrSpendTextBox.Text = "";
                this.adjOffSpendTextBox.Text = "";
                this.adjustKeepSpendTextBox.Text = "";
                this.adjustTaxTextBox.Text = "";
            }

            // if fief IS owned by player, enable fief management buttons and TextBoxes 
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
                this.updateFiefBtn.Enabled = true;
                this.lockoutBtn.Enabled = true;
                this.selfBailiffBtn.Enabled = true;
                this.setBailiffBtn.Enabled = true;
                this.removeBaliffBtn.Enabled = true;

                // set TextBoxes to show next year's figures
                this.adjGarrSpendTextBox.Text = Convert.ToString(this.fiefToView.garrisonSpendNext);
                this.adjInfrSpendTextBox.Text = Convert.ToString(this.fiefToView.infrastructureSpendNext);
                this.adjOffSpendTextBox.Text = Convert.ToString(this.fiefToView.officialsSpendNext);
                this.adjustKeepSpendTextBox.Text = Convert.ToString(this.fiefToView.keepSpendNext);
                this.adjustTaxTextBox.Text = Convert.ToString(this.fiefToView.taxRateNext);
            }

            this.fiefContainer.BringToFront();
        }

        /// <summary>
        /// Responds to the click event of the fiefManagementToolStripMenuItem
        /// which displays main Fief information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshFiefContainer(this.myChar.location);
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
            try
            {
                Double newTax = Convert.ToDouble(this.adjustTaxTextBox.Text);
                // check if amount/rate changed
                if (newTax != this.fiefToView.taxRateNext)
                {
                    // adjust tax rate
                    this.fiefToView.adjustTaxRate(newTax);
                }

                UInt32 newOff = Convert.ToUInt32(this.adjOffSpendTextBox.Text);
                // check if amount/rate changed
                if (newOff != this.fiefToView.officialsSpendNext)
                {
                    // adjust officials spend
                    this.fiefToView.adjustOfficialsSpend(Convert.ToUInt32(this.adjOffSpendTextBox.Text));
                }

                UInt32 newGarr = Convert.ToUInt32(this.adjGarrSpendTextBox.Text);
                // check if amount/rate changed
                if (newGarr != this.fiefToView.garrisonSpendNext)
                {
                    // adjust garrison spend
                    this.fiefToView.adjustGarrisonSpend(Convert.ToUInt32(this.adjGarrSpendTextBox.Text));
                }

                UInt32 newInfra = Convert.ToUInt32(this.adjInfrSpendTextBox.Text);
                // check if amount/rate changed
                if (newInfra != this.fiefToView.infrastructureSpendNext)
                {
                    // adjust infrastructure spend
                    this.fiefToView.adjustInfraSpend(Convert.ToUInt32(this.adjInfrSpendTextBox.Text));
                }

                UInt32 newKeep = Convert.ToUInt32(this.adjustKeepSpendTextBox.Text);
                // check if amount/rate changed
                if (newKeep != this.fiefToView.keepSpendNext)
                {
                    // adjust keep spend
                    this.fiefToView.adjustKeepSpend(Convert.ToUInt32(this.adjustKeepSpendTextBox.Text));
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
                // refresh display
                this.refreshFiefContainer(this.fiefToView);
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
            // something
        }

        /// <summary>
        /// Responds to the click event of the navigateToolStripMenuItem
        /// which refreshes and displays the navigation screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // refresh navigation data
            this.fiefToView = this.myChar.location;
            this.refreshTravelContainer();
            // show navigation screen
            this.travelContainer.BringToFront();
        }

        /// <summary>
        /// Gets travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="f">Target fief</param>
        private double getTravelCost(Fief source, Fief target)
        {
            double cost = 0;
            // calculate travel cost based on terrain for both fiefs
            cost = ((source.terrain.travelCost + target.terrain.travelCost) / 2) * this.clock.calcSeasonTravMod();
            return cost;
        }

        /// <summary>
        /// Refreshes travel display screen
        /// </summary>
        private void refreshTravelContainer()
        {
            // clear existing data in TextBoxex
            this.travelMultiMoveTextBox.Text = "";
            this.travelCampDaysTextBox.Text = "";

            // string[] to hold direction text
            string[] directions = new string[] { "NE", "E", "SE", "SW", "W", "NW" };
            // Button[] to hold corresponding travel buttons
            Button[] travelBtns = new Button[] { travel_NE_btn, travel_E_btn, travel_SE_btn, travel_SW_btn, travel_W_btn, travel_NW_btn };

            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + this.myChar.location.name + " (" + this.myChar.location.fiefID + ")" + "\r\n" + this.myChar.location.province.name + ", " + this.myChar.location.province.kingdom.name;

            for (int i = 0; i < directions.Length; i++ )
            {
                // retrieve target fief for that direction
                Fief target = this.gameMap.getFief(this.myChar.location, directions[i]);
                // display fief details and travel cost
                if (target != null)
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\n";
                    travelBtns[i].Text += target.name + " (" + target.fiefID + ")\r\n";
                    travelBtns[i].Text += target.province.name + ", " + target.province.kingdom.name + "\r\n\r\n";
                    travelBtns[i].Text += "Cost: " + this.getTravelCost(this.myChar.location, target);
                }
                else
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\nNo fief present";
                }
            }

            // set text for 'enter/exit keep' button, depending on whether player in/out of keep
            if (this.myChar.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
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
            Fief targetFief = this.gameMap.getFief(this.myChar.location, button.Tag.ToString());

            if (targetFief != null)
            {
                // get travel cost
                double travelCost = this.getTravelCost(this.myChar.location, targetFief);
                // attempt to move player to target fief
                success = this.myChar.moveCharacter(targetFief, travelCost);
                // if move successfull, refresh travel display
                if (success)
                {
                    this.fiefToView = targetFief;
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
            this.refreshMyFiefs();
            this.fiefsOwnedContainer.BringToFront();
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
            if (this.fiefToView.bailiff != null)
            {
                // if player is bailiff, show in personal characteristics screen
                if (this.fiefToView.bailiff == this.myChar)
                {
                    this.charToView = this.myChar;
                    this.refreshCharacterContainer(this.charToView);
                }

                // if NPC is bailiff, show in household affairs screen
                else if (this.fiefToView.bailiff is NonPlayerCharacter)
                {
                    this.charToView = this.fiefToView.bailiff;
                    // refresh household affairs screen 
                    this.refreshHouseholdDisplay(this.charToView as NonPlayerCharacter);
                    // display household affairs screen
                    this.houseContainer.BringToFront();
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
            for (int i = 0; i < this.myChar.ownedFiefs.Count; i++)
            {
                if (this.myChar.ownedFiefs[i].fiefID.Equals(this.fiefsListView.SelectedItems[0].SubItems[1].Text))
                {
                    fiefToDisplay = this.myChar.ownedFiefs[i];
                }

            }

            // display fief details
            if (fiefToDisplay != null)
            {
                this.refreshFiefContainer(fiefToDisplay);
                //this.fiefTextBox.Text = this.displayFief(fiefToView);
                this.fiefContainer.BringToFront();
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
            if (this.myChar.inKeep)
            {
                // exit keep
                this.myChar.exitKeep();
                // change button text
                this.enterKeepBtn.Text = "Enter Keep";
                // refresh display
                this.refreshTravelContainer();
            }
            // if player not in keep
            else
            {
                // attempt to enter keep
                bool entered = this.myChar.enterKeep();
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
            if (!this.myChar.inKeep)
            {
                // attempt to enter keep
                enteredKeep = this.myChar.enterKeep();
            }
            else
            {
                enteredKeep = true;
            }

            // if in keep
            if (enteredKeep)
            {
                // set hireNPC_Btn tag to reflect which meeting place
                this.hireNPC_Btn.Tag = place;
                // refresh court screen 
                this.refreshMeetingPlaceDisplay(place);
                // display court screen
                this.meetingPlaceContainer.BringToFront();
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
            if (this.myChar.inKeep)
            {
                this.myChar.exitKeep();
            }
            // set hireNPC_Btn tag to reflect which meeting place
            this.hireNPC_Btn.Tag = place;
            // refresh tavern screen 
            this.refreshMeetingPlaceDisplay(place);
            // display tavern screen
            this.meetingPlaceContainer.BringToFront();
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
            for (int i = 0; i < this.fiefToView.characters.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (this.fiefToView.characters[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = this.fiefToView.characters[i];

                        // set appropriate text for hire/fire button, 
                        // depending on whether is existing employee
                        if (this.myChar.employees.Contains(this.fiefToView.characters[i]))
                        {
                            this.hireNPC_Btn.Text = "Fire NPC";
                            // if already employee, disable 'salary offer' text box
                            this.hireNPC_TextBox.Visible = false;
                        }
                        else
                        {
                            this.hireNPC_Btn.Text = "Hire NPC";
                            this.hireNPC_TextBox.Visible = true;
                            this.hireNPC_TextBox.Enabled = true;
                        }
                        this.hireNPC_Btn.Enabled = true;
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                this.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(charToDisplay);
                this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
                this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Responds to the click event of the hireNPC_Btn button
        /// which allows the player to either invoke processEmployOffer to attempt to hire an NPC
        /// or invoke fireNPC to fire one
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void hireNPC_Btn_Click(object sender, EventArgs e)
        {
            bool isHired = false;

            // get hireNPC_Btn tag (shows which meeting place are in)
            String place = Convert.ToString(((Button)sender).Tag);

            // if selected NPC is not a current employee
            if (!this.myChar.employees.Contains(charToView))
            {
                try
                {
                    // get offer amount
                    UInt32 newOffer = Convert.ToUInt32(this.hireNPC_TextBox.Text);
                    // submit offer
                    isHired = this.myChar.processEmployOffer((NonPlayerCharacter)charToView, newOffer);

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
                this.myChar.fireNPC((NonPlayerCharacter)charToView);
            }

            // if in the tavern
            if (place.Equals("tavern"))
            {
                // if NPC is hired, refresh whole screen (NPC removed from list)
                if (isHired)
                {
                    this.refreshMeetingPlaceDisplay(place);
                }
                // if NPC not hired, just refresh the character display
                else
                {
                    this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(charToView);
                }
            }
            // if not in tavern, just refresh the character display
            else
            {
                this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(charToView);
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
                travelCost = this.getTravelCost(ch.location, ch.goTo.Peek());
                // attempt to move character
                success = ch.moveCharacter(ch.goTo.Peek(), travelCost);
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

            if (ch == this.myChar)
            {
                // if player has moved, refresh display
                if (ch.goTo.Count < steps)
                {
                    this.fiefToView = this.myChar.location;
                    this.refreshTravelContainer();
                }
            }

            return success;

        }

        /// <summary>
        /// Responds to the click event of the travelMultiMoveBtn button
        /// which finds the shortest path to the target fief (specified in text box),
        /// populates the character's goTo queue, and invokes characterMultiMove to
        /// perform the move
        /// </summary>
        /// <returns>bool indicating whether odd</returns>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelMultiMoveBtn_Click(object sender, EventArgs e)
        {
            // check for existence of fief
            if (this.fiefMasterList.ContainsKey(this.travelMultiMoveTextBox.Text))
            {
                // retrieves target fief
                Fief target = fiefMasterList[this.travelMultiMoveTextBox.Text];
                // obtains goTo queue for shortest path to target
                this.charToView.goTo = this.gameMap.getShortestPath(this.charToView.location, target);
                // if valid, perform move
                if (this.charToView.goTo.Count > 0)
                {
                    this.characterMultiMove(this.charToView);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Target fief ID not found.  Please ensure fiefID is valid.");
            }

        }

        private void setBailiffBtn_Click(object sender, EventArgs e)
        {
            SelectionForm chooseBailiff = new SelectionForm(this, "bailiff");
            chooseBailiff.Show();
        }

        private void lockoutBtn_Click(object sender, EventArgs e)
        {
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
                var putArrayResult = client.Put(arrayToDB);

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
            if (this.fiefToView.bailiff != null)
            {
                // relieve him of his duties
                this.fiefToView.bailiff = null;
                // refresh fief display
                this.refreshFiefContainer(this.fiefToView);
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
                if (this.fiefToView.bailiff != null)
                {
                    // relieve him of his duties
                    this.fiefToView.bailiff = null;
                }

                // set player as bailiff
                this.fiefToView.bailiff = this.myChar;
            }

            // refresh fief display
            this.refreshFiefContainer(this.fiefToView);
        }

        private void characterTitlesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.refreshCharacterContainer(this.charToView);
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
            if (this.myChar.inKeep)
            {
                this.myChar.exitKeep();
            }
            // set hireNPC_Btn tag to reflect which meeting place
            this.hireNPC_Btn.Tag = place;
            // refresh outside keep screen 
            this.refreshMeetingPlaceDisplay(place);
            // display tavern screen
            this.meetingPlaceContainer.BringToFront();
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
            // refresh household affairs screen 
            this.refreshHouseholdDisplay();
            // display household affairs screen
            this.houseContainer.BringToFront();
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
            for (int i = 0; i < this.myChar.employees.Count; i++)
            {
                if (this.houseCharListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (this.myChar.employees[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = this.myChar.employees[i];
                        break;
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                this.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(charToDisplay);
                this.houseCharTextBox.ReadOnly = true;
                this.houseCharTextBox.Text = textToDisplay;
                // re-enable controls
                this.houseCampBtn.Enabled = true;
                this.houseCampDaysTextBox.Enabled = true;
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
            this.campWaitHere(this.charToView);
        }

        /// <summary>
        /// Responds to the click event of the travelCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void travelCampBtn_Click(object sender, EventArgs e)
        {
            this.campWaitHere(this.charToView);
        }

        /// <summary>
        /// Allows the character to remain in their current location for the specified
        /// number of days, incrementing bailiffDaysInFief if appropriate
        /// </summary>
        /// <param name="ch">The Character who wishes to camp (wait here)</param>
        public void campWaitHere(Character ch)
        {
            bool proceed = true;
            byte campDays = 0;

            try
            {
                // get number of days to camp
                // if player, get from travel screen
                if (ch == this.myChar)
                {
                    campDays = Convert.ToByte(this.travelCampDaysTextBox.Text);
                }
                // if not player, get from household screen
                else
                {
                    campDays = Convert.ToByte(this.houseCampDaysTextBox.Text);
                }

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
                    if (ch == this.myChar)
                    {
                        entourageCamp = true;
                    }

                    // if character NOT player
                    else
                    {
                        // if is in entourage, give player chance to remove prior to camping
                        if ((ch as NonPlayerCharacter).inEntourage)
                        {
                            DialogResult dialogResult = MessageBox.Show("Do you wish to remove this character from  your entourage before he camps?", "Remove character from entourage?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No)
                            {
                                entourageCamp = true;
                            }
                            else
                            {
                                (ch as NonPlayerCharacter).inEntourage = false;
                            }
                        }
                    }

                    // adjust character's days
                    ch.days = ch.days - campDays;
                    System.Windows.Forms.MessageBox.Show("You remain in " + ch.location.name + " for " + campDays + " days.");

                    // camp entourage (and player) if necessary
                    if (entourageCamp)
                    {
                        // iterate through employees to locate entourage
                        for (int i = 0; i < this.myChar.employees.Count; i++ )
                        {
                            if (this.myChar.employees[i].inEntourage)
                            {
                                if (this.myChar.employees[i] != ch)
                                {
                                    this.myChar.employees[i].days = this.myChar.employees[i].days - campDays;
                                }
                            }
                        }

                        // camp player, if necessary
                        if (ch != this.myChar)
                        {
                            this.myChar.days = this.myChar.days - campDays;
                        }
                    }

                    // keep track of bailiffDaysInFief before any possible increment
                    byte bailiffDaysBefore = ch.location.bailiffDaysInFief;

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
                        if (this.myChar == ch.location.bailiff)
                        {
                            myBailiff = this.myChar;
                        }
                        // if not, check for bailiff in entourage
                        else
                        {
                            for (int i = 0; i < this.myChar.employees.Count; i++)
                            {
                                if (this.myChar.employees[i].inEntourage)
                                {
                                    if (this.myChar.employees[i] != ch)
                                    {
                                        if (this.myChar.employees[i] == ch.location.bailiff)
                                        {
                                            myBailiff = this.myChar.employees[i];
                                        }
                                    }
                                }
                            }
                        }

                    }

                    // if bailiff identified as someone who camped
                    if (myBailiff != null)
                    {
                        // incremenet bailiffDaysInFief
                        ch.location.bailiffDaysInFief += campDays;
                        // if necessary, display message to player
                        if (ch.location.bailiffDaysInFief >= 30)
                        {
                            // don't display this message if min bailiffDaysInFief was already achieved
                            if (!(bailiffDaysBefore >= 30))
                            {
                                System.Windows.Forms.MessageBox.Show(myBailiff.name + " has fulfilled his bailiff duties in " + ch.location.name + ".");
                            }
                        }
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
                // refresh display
                if (proceed)
                {
                    if (ch == this.myChar)
                    {
                        this.refreshTravelContainer();
                    }
                    else
                    {
                        this.refreshHouseholdDisplay((this.charToView as NonPlayerCharacter));
                    }
                }
            }

        }

    }

}
