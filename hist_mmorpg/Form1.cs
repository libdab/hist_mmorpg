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
    public partial class Form1 : Form
    {

        /// <summary>
        /// Holds all NonPlayerCharacter objects
        /// </summary>
        public Dictionary<string, NonPlayerCharacter> npcMasterList = new Dictionary<string, NonPlayerCharacter>();
		/// <summary>
		/// Holds keys for NonPlayerCharacter objects
		/// </summary>
		List<String> npcKeys = new List<String> ();
        /// <summary>
        /// Holds all PlayerCharacter objects
        /// </summary>
        public Dictionary<string, PlayerCharacter> pcMasterList = new Dictionary<string, PlayerCharacter>();
		/// <summary>
		/// Holds keys for PlayerCharacter objects
		/// </summary>
		List<String> pcKeys = new List<String> ();
        /// <summary>
        /// Holds all Fief objects
        /// </summary>
        public Dictionary<string, Fief> fiefMasterList = new Dictionary<string, Fief>();
		/// <summary>
		/// Holds keys for Fief objects
		/// </summary>
		List<String> fiefKeys = new List<String> ();
		/// <summary>
		/// Holds all Province objects
		/// </summary>
		public Dictionary<string, Province> provinceMasterList = new Dictionary<string, Province>();
		/// <summary>
		/// Holds keys for Province objects
		/// </summary>
		List<String> provKeys = new List<String> ();
		/// <summary>
		/// Holds all Terrain objects
		/// </summary>
		public Dictionary<string, Terrain> terrainMasterList = new Dictionary<string, Terrain>();
		/// <summary>
		/// Holds keys for Terrain objects
		/// </summary>
		List<String> terrKeys = new List<String> ();
		/// <summary>
		/// Holds all Skill objects
		/// </summary>
		public Dictionary<string, Skill> skillMasterList = new Dictionary<string, Skill>();
		/// <summary>
		/// Holds keys for Skill objects
		/// </summary>
		List<String> skillKeys = new List<String> ();
		/// <summary>
		/// Holds Character_Riak objects with existing goTo queues (for initial load)
		/// </summary>
		List<Character_Riak> goToList = new List<Character_Riak> ();
        /// <summary>
        /// Holds main PlayerCharacter
        /// </summary>
        private PlayerCharacter myChar;
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        private Character charToView;
        /// <summary>
        /// Holds Fief to view in UI
        /// </summary>
        private Fief fiefToView;
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
            // initialise display
            InitializeComponent();

			// initialise Riak elements
			cluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
			client = (RiakClient)cluster.CreateClient();

            // create game objects
			this.myChar = this.initGameObjects("NOTdb");

			// this.ArrayFromCSV ("/home/libdab/Dissertation_data/11-07-14/hacked-player.csv", true, "testGame", "skeletonPlayers1194");

            this.fiefToView = this.myChar.location;
            this.setUpFiefsList();
            this.setUpCourtCharsList();
            this.charToView = this.myChar;
            this.refreshCharacterContainer();
        }

		public String[][] ArrayFromCSV (String csvFilename, bool writeToDB, String bucket = "", String key = "")
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

			if(writeToDB)
			{
				var arrayToDB = new RiakObject(bucket, key, outArray);
				var putArrayResult = client.Put(arrayToDB);

				if (! putArrayResult.IsSuccess)
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

		public static bool IsOdd(int value)
		{
			return value % 2 != 0;
		}

		/// <summary>
		/// Creates all game objects from code
		/// </summary>
		/// <param name="source">Where to get object data</param>
		public PlayerCharacter initGameObjects(String source)
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
				
			// set inital fief to display
			this.fiefToView = this.pcMasterList["101"].location;

			// return PC;
			return this.pcMasterList["101"];

        }

		/// <summary>
		/// Creates all game objects from code
		/// </summary>
		public void initialLoad()
		{

            // creat GameClock
			GameClock myGameClock = new GameClock("clock001", 1320);
			this.clock = myGameClock;

			// create skills
			// Dictionary to hold collection of skills
			// Dictionary<string, Skill> skillsCollection = new Dictionary<string, Skill>();

			// List to holds skill keys (for random selection)
			List<string> skillsKeys = new List<string>();

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
				skillsArray1[i] = this.skillMasterList[skillsKeysCopy[randChoice]];
				skillsKeysCopy.RemoveAt(randChoice);
			}

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

			// create province for fiefs
			Province myProv = new Province("ESX00", "Sussex, England", 6.2, "E1");
			this.provinceMasterList.Add (myProv.provinceID, myProv);
			Province myProv2 = new Province("ESR00", "Surrey, England", 6.2, "E1");
			this.provinceMasterList.Add (myProv2.provinceID, myProv2);

			Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', plains, fief1Chars, keep1BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief1.fiefID, myFief1);
			Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, 3.50, 0.20, 50, 10, 1000, 1000, 2000, 2000, 10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', hills, fief2Chars, keep2BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief2.fiefID, myFief2);
			Fief myFief3 = new Fief("ESX01", "Hastings", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', plains, fief3Chars, keep3BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief3.fiefID, myFief3);
			Fief myFief4 = new Fief("ESX04", "Eastbourne", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', plains, fief4Chars, keep4BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief4.fiefID, myFief4);
			Fief myFief5 = new Fief("ESX05", "Worthing", myProv, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', plains, fief5Chars, keep5BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief5.fiefID, myFief5);
			Fief myFief6 = new Fief("ESR03", "Reigate", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', plains, fief6Chars, keep6BarChars, false, false, this.clock);
			fiefMasterList.Add(myFief6.fiefID, myFief6);
			Fief myFief7 = new Fief("ESR04", "Guilford", myProv2, 6000, 3.0, 3.0, 50, 10, 12000, 42000, 2000, 2000, 10, 12000, 42000, 2000, 2000, 5.63, 5.5, 'C', forrest, fief7Chars, keep7BarChars, false, false, this.clock);
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
			myGoTo1.Enqueue (myFief2);
			myGoTo1.Enqueue (myFief7);

			// create entourages for PCs
			List<NonPlayerCharacter> myEmployees1 = new List<NonPlayerCharacter>();
			List<NonPlayerCharacter> myEmployees2 = new List<NonPlayerCharacter>();

			// create lists of fiefs owned by PCs and add some fiefs
			List<Fief> myFiefsOwned1 = new List<Fief>();
			List<Fief> myFiefsOwned2 = new List<Fief>();

			// create some characters
			PlayerCharacter myChar1 = new PlayerCharacter("101", "Dave Bond", 50, true, "Fr", 1.0, 8.50, 6.0, myGoTo1, "E1", 90, 4.0, 7.2, 6.1, skillsArray1, false, true, false, "200", "403", "", false, 13000, myEmployees1, myFiefsOwned1, cl: this.clock, loc: myFief1);
			pcMasterList.Add(myChar1.charID, myChar1);
			PlayerCharacter myChar2 = new PlayerCharacter("102", "Bave Dond", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo2, "E1", 90, 4.0, 5.0, 4.5, skillsArray1, true, false, false, "200", "", "", false, 13000, myEmployees2, myFiefsOwned2, cl: this.clock, loc: myFief1);
			pcMasterList.Add(myChar2.charID, myChar2);
			NonPlayerCharacter myNPC1 = new NonPlayerCharacter("401", "Jimmy Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo3, "E1", 90, 4.0, 3.3, 6.7, skillsArray1, false, false, false, "200", "", "", 0, false, cl: this.clock, loc: myFief1);
			npcMasterList.Add(myNPC1.charID, myNPC1);
			NonPlayerCharacter myNPC2 = new NonPlayerCharacter("402", "Johnny Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myGoTo4, "E1", 90, 4.0, 7.1, 5.2, skillsArray1, false, false, false, "200", "", "", 10000, true, mb: myChar1.charID, cl: this.clock, loc: myFief1);
			npcMasterList.Add(myNPC2.charID, myNPC2);
			NonPlayerCharacter myWife = new NonPlayerCharacter("403", "Molly Maguire", 50, false, "Eng", 1.0, 8.50, 6.0, myGoTo5, "E1", 90, 4.0, 4.0, 6.0, skillsArray1, false, true, true, "200", "", "", 0, false, cl: this.clock, loc: myFief1);
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

			// set fief bailiffs
			myFief1.bailiff = myNPC1;
			myFief2.bailiff = myNPC2;

			// add NPC to employees
			myChar1.hireNPC(myNPC2, 10000);
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
			// myFief1.barCharacter(myNPC2.charID);

			// try retrieving fief from masterlist using fiefID
			// Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");

		}

		/// <summary>
		/// Writes all objects for a particular game to database
		/// </summary>
		/// <param name="gameID">ID of game</param>
		public void writeToDB(String gameID)
		{
			// write clock
			this.writeClock (gameID, this.clock);

			// write skills
			if (this.skillKeys.Count > 0)
			{
				this.skillKeys.Clear ();
			}

			foreach (KeyValuePair<String, Skill> pair in this.skillMasterList)
			{
				bool success = this.writeSkill (gameID, pair.Value);
				if (success)
				{
					this.skillKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "skillKeys", this.skillKeys);

			// write NPCs
			if (this.npcKeys.Count > 0)
			{
				this.npcKeys.Clear ();
			}

			foreach (KeyValuePair<String, NonPlayerCharacter> pair in this.npcMasterList)
			{
				bool success = this.writeNPC (gameID, pair.Value);
				if (success)
				{
					this.npcKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "npcKeys", this.npcKeys);

			// write PCs
			if (this.pcKeys.Count > 0)
			{
				this.pcKeys.Clear ();
			}

			foreach (KeyValuePair<String, PlayerCharacter> pair in this.pcMasterList)
			{
				bool success = this.writePC (gameID, pair.Value);
				if (success)
				{
					this.pcKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "pcKeys", this.pcKeys);

			// write Provinces
			if (this.provKeys.Count > 0)
			{
				this.provKeys.Clear ();
			}

			foreach (KeyValuePair<String, Province> pair in this.provinceMasterList)
			{
				bool success = this.writeProvince (gameID, pair.Value);
				if (success)
				{
					this.provKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "provKeys", this.provKeys);

			// write Terrains
			if (this.terrKeys.Count > 0)
			{
				this.terrKeys.Clear ();
			}

			foreach (KeyValuePair<String, Terrain> pair in this.terrainMasterList)
			{
				bool success = this.writeTerrain (gameID, pair.Value);
				if (success)
				{
					this.terrKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "terrKeys", this.terrKeys);

			// write Fiefs
			if (this.fiefKeys.Count > 0)
			{
				this.fiefKeys.Clear ();
			}

			foreach (KeyValuePair<String, Fief> pair in this.fiefMasterList)
			{
				bool success = this.writeFief (gameID, pair.Value);
				if (success)
				{
					this.fiefKeys.Add (pair.Key);
				}
			}

			this.writeKeyList (gameID, "fiefKeys", this.fiefKeys);

			// write map (edges)
			this.writeMapEdges (gameID, this.gameMap);

		}

		/// <summary>
		/// Loads all objects for a particular game from database
		/// </summary>
		/// <param name="gameID">ID of game</param>
		public void initialDBload(String gameID)
		{

			// load key lists
			this.initialDBload_keyLists (gameID);

			// load clock
			this.clock = this.initialDBload_clock (gameID, "clock001");

			// load skills
			foreach (String element in this.skillKeys)
			{
				Skill skill = this.initialDBload_skill (gameID, element);
				this.skillMasterList.Add(skill.skillID, skill);
			}
				
			// load NPCs
			foreach (String element in this.npcKeys)
			{
				NonPlayerCharacter npc = this.initialDBload_NPC (gameID, element);
				this.npcMasterList.Add(npc.charID, npc);
			}

			// load PCs
			foreach (String element in this.pcKeys)
			{
				PlayerCharacter pc = this.initialDBload_PC (gameID, element);
				this.pcMasterList.Add(pc.charID, pc);
			}

			// load provinces
			foreach (String element in this.provKeys)
			{
				Province prov = this.initialDBload_Province (gameID, element);
				this.provinceMasterList.Add (prov.provinceID, prov);
			}

			// load terrains
			foreach (String element in this.terrKeys)
			{
				Terrain terr = this.initialDBload_terrain (gameID, element);
				this.terrainMasterList.Add (terr.terrainCode, terr);
			}

			// load fiefs
			foreach (String element in this.fiefKeys)
			{
				Fief f = this.initialDBload_Fief (gameID, element);
				this.fiefMasterList.Add (f.fiefID, f);
			}

			// process Character goTo queue
			if (this.goToList.Count > 0)
			{
				for (int i = 0; i < this.goToList.Count; i++)
				{
					this.populate_goTo (this.goToList[i]);
				}
				this.goToList.Clear();
			}

			// load map
			this.gameMap = this.initialDBload_map (gameID, "map001E");
		}

		/// <summary>
		/// Loads all Riak key lists for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which key lists to be retrieved</param>
		public void initialDBload_keyLists(String gameID)
		{
			// get Skill Keys
			var skillKeyResult = client.Get(gameID, "skillKeys");
			if (skillKeyResult.IsSuccess)
			{
				this.skillKeys = skillKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve skillKeys from database.");
			}

			// get NPC Keys
			var npcKeyResult = client.Get(gameID, "npcKeys");
			if (npcKeyResult.IsSuccess)
			{
				this.npcKeys = npcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve npcKeys from database.");
			}
				
			// get PC Keys
			var pcKeyResult = client.Get(gameID, "pcKeys");
			if (pcKeyResult.IsSuccess)
			{
				this.pcKeys = pcKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve pcKeys from database.");
			}

			// get Province Keys
			var provKeyResult = client.Get(gameID, "provKeys");
			if (provKeyResult.IsSuccess)
			{
				this.provKeys = provKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve provKeys from database.");
			}

			// get Terrain Keys
			var terrKeyResult = client.Get(gameID, "terrKeys");
			if (terrKeyResult.IsSuccess)
			{
				this.terrKeys = terrKeyResult.Value.GetObject<List<String>>();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("InitialDBload: Unable to retrieve terrKeys from database.");
			}

			// get Fief Keys
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
		/// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="npcID">ID of NPC to be retrieved</param>
		public NonPlayerCharacter initialDBload_NPC(String gameID, String npcID)
		{
			var npcResult = client.Get(gameID, npcID);
			var npcRiak = new NonPlayerCharacter_Riak();
			NonPlayerCharacter myNPC = new NonPlayerCharacter ();

			if (npcResult.IsSuccess)
			{
				npcRiak = npcResult.Value.GetObject<NonPlayerCharacter_Riak>();
				if (npcRiak.goTo.Count > 0)
				{
					goToList.Add (npcRiak);
				}
				myNPC = this.NPCfromRiakNPC (npcRiak);
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
		/// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="pcID">ID of PC to be retrieved</param>
		public PlayerCharacter initialDBload_PC(String gameID, String pcID)
		{
			var pcResult = client.Get(gameID, pcID);
			var pcRiak = new PlayerCharacter_Riak();
			PlayerCharacter myPC = new PlayerCharacter ();

			if (pcResult.IsSuccess)
			{
				pcRiak = pcResult.Value.GetObject<PlayerCharacter_Riak>();
				if (pcRiak.goTo.Count > 0)
				{
					goToList.Add (pcRiak);
				}
				myPC = this.PCfromRiakPC (pcRiak);
			}
			else
			{
				System.Windows.Forms.MessageBox.Show ("InitialDBload: Unable to retrieve PlayerCharacter " + pcID);
			}

			return myPC;
		}

		/// <summary>
		/// Loads a Province for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="provID">ID of Province to be retrieved</param>
		public Province initialDBload_Province(String gameID, String provID)
		{
			var provResult = client.Get(gameID, provID);
			var provRiak = new Province_Riak();
			Province myProv = new Province ();

			if (provResult.IsSuccess)
			{
				provRiak = provResult.Value.GetObject<Province_Riak>();
				myProv = this.ProvinceFromRiak (provRiak);
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
		/// Loads a Fief for a particular game from database
		/// </summary>
		/// <param name="gameID">Game for which skill to be retrieved</param>
		/// <param name="fiefID">ID of Fief to be retrieved</param>
		public Fief initialDBload_Fief(String gameID, String fiefID)
		{
			var fiefResult = client.Get(gameID, fiefID);
			var fiefRiak = new Fief_Riak();
			Fief myFief = new Fief ();

			if (fiefResult.IsSuccess)
			{
				fiefRiak = fiefResult.Value.GetObject<Fief_Riak>();
				myFief = this.FiefFromRiakFief (fiefRiak);
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

			// insert owner
			fOut.owner = this.pcMasterList[fr.owner];
			// Fief source = fiefMasterList.Find(x => x.fiefID == "ESX03");
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
					System.Windows.Forms.MessageBox.Show ("Unable to identify bailiff for Fief " + fOut.fiefID);
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
					else if (this.pcMasterList.ContainsKey (fr.characters[i]))
					{
						fOut.characters.Add(this.pcMasterList[fr.characters[i]]);
						this.pcMasterList[fr.characters[i]].location = fOut;
					}

				}
			}

			return fOut;
		}

		/// <summary>
		/// Converts PlayerCharacter object (containing nested objects) into suitable format for JSON serialisation
		/// </summary>
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
		/// <param name="pcr">PlayerCharacter_Riak object to be converted</param>
		public PlayerCharacter PCfromRiakPC(PlayerCharacter_Riak pcr)
		{
			PlayerCharacter pcOut = null;
			// create PlayerCharacter from PlayerCharacter_Riak
			pcOut = new PlayerCharacter (pcr);

			// insert game clock
			pcOut.clock = this.clock;

			// insert skills
			if (pcr.skills.Length > 0)
			{
				for (int i = 0; i < pcr.skills.Length; i++)
				{
					Skill skill = this.skillMasterList[pcr.skills[i]];
					pcOut.skills[i] = skill;
				}
			}

			// insert employees
			if (pcr.employees.Count > 0)
			{
				for (int i = 0; i < pcr.employees.Count; i++)
				{
					NonPlayerCharacter employee = npcMasterList[pcr.employees[i]];
					pcOut.employees.Add (employee);
				}
			}

			return pcOut;
		}

		/// <summary>
		/// Converts NonPlayerCharacter_Riak objects into NonPlayerCharacter game objects
		/// </summary>
		/// <param name="npcr">NonPlayerCharacter_Riak object to be converted</param>
		public NonPlayerCharacter NPCfromRiakNPC(NonPlayerCharacter_Riak npcr)
		{
			NonPlayerCharacter npcOut = null;
			// create PlayerCharacter from PlayerCharacter_Riak
			npcOut = new NonPlayerCharacter (npcr);

			// insert game clock
			npcOut.clock = this.clock;

			// insert skills
			if (npcr.skills.Length > 0)
			{
				for (int i = 0; i < npcr.skills.Length; i++)
				{
					Skill skill = this.skillMasterList[npcr.skills[i]];
					npcOut.skills[i] = skill;
				}
			}

			return npcOut;
		}

		/// <summary>
		/// Converts HexMapGraph edge collection into suitable format for JSON serialisation
		/// </summary>
		/// <param name="edgesIn">List<TaggedEdge<Fief, string>> to be converted</param>
		public List<TaggedEdge<String, string>> EdgeCollection_to_Riak(List<TaggedEdge<Fief, string>> edgesIn)
		{
			List<TaggedEdge<String, string>> edgesOut = new List<TaggedEdge<string, string>> ();

			foreach (TaggedEdge<Fief, string> element in edgesIn)
			{
				TaggedEdge<String, string> newElement = this.EdgeFief_to_EdgeString (element);
				edgesOut.Add (newElement);
			}

			return edgesOut;
		}

		/// <summary>
		/// Converts List<TaggedEdge<String, string>> into TaggedEdge<Fief, string>[]
		/// </summary>
		/// <param name="edgesIn">List<TaggedEdge<String, string>> to be converted</param>
		public TaggedEdge<Fief, string>[] EdgeCollection_from_Riak(List<TaggedEdge<String, string>> edgesIn)
		{
			TaggedEdge<Fief, string>[] edgesOut = new TaggedEdge<Fief, string>[edgesIn.Count];

			int i = 0;
			foreach (TaggedEdge<String, string> element in edgesIn)
			{
				TaggedEdge<Fief, string> newElement = this.EdgeString_to_EdgeFief (element);
				edgesOut [i] = newElement;
				i++;
			}

			return edgesOut;
		}

		/// <summary>
		/// Converts TaggedEdge<Fief, string> object into suitable format for JSON serialisation
		/// (i.e. TaggedEdge<String, string>)
		/// </summary>
		/// <param name="te">TaggedEdge<Fief, string> to be converted</param>
		public TaggedEdge<String, string> EdgeFief_to_EdgeString(TaggedEdge<Fief, string> te)
		{
			TaggedEdge<String, string> edgeOut = new TaggedEdge<String, string>(te.Source.fiefID, te.Target.fiefID, te.Tag);
			return edgeOut;
		}

		/// <summary>
		/// Converts TaggedEdge<String, string> object into TaggedEdge<Fief, string>
		/// </summary>
		/// <param name="te">TaggedEdge<String, string> to be converted</param>
		public TaggedEdge<Fief, string> EdgeString_to_EdgeFief(TaggedEdge<String, string> te)
		{
			TaggedEdge<Fief, string> edgeOut = new TaggedEdge<Fief, string>(this.fiefMasterList[te.Source], this.fiefMasterList[te.Target], te.Tag);
			return edgeOut;
		}

		/// <summary>
		/// Inserts Fiefs from a Character_Riak's goTo Queue into a Character's goTo Queue
		/// (used with load from database)
		/// </summary>
		/// <param name="cr">Character_Riak containing goTo Queue</param>
		public bool populate_goTo(Character_Riak cr)
		{
			bool success = false;
			Character myChar = null;

			if (cr is PlayerCharacter_Riak)
			{
				if (this.pcMasterList.ContainsKey(cr.charID))
				{
					myChar = this.pcMasterList [cr.charID];
					success = true;
				}
				else
				{
					System.Windows.Forms.MessageBox.Show ("PlayerCharacter not found: " + cr.charID);
				}
			}
			else if (cr is NonPlayerCharacter_Riak)
			{
				if (this.npcMasterList.ContainsKey(cr.charID))
				{
					myChar = this.npcMasterList [cr.charID];
					success = true;
				}
				else
				{
					System.Windows.Forms.MessageBox.Show ("NonPlayerCharacter not found: " + cr.charID);
				}
			}

			foreach (String value in cr.goTo)
			{
				myChar.goTo.Enqueue (this.fiefMasterList[value]);
			}

			return success;
		}

		/// <summary>
		/// Converts Province object into suitable format for JSON serialisation
		/// </summary>
		/// <param name="p">Province to be converted</param>
		public Province_Riak ProvinceToRiak(Province p)
		{
			Province_Riak oOut = null;
			oOut = new Province_Riak (p);
			return oOut;
		}

		/// <summary>
		/// Converts Province_Riak objects into Province game objects
		/// </summary>
		/// <param name="pr">Province_Riak to be converted</param>
		public Province ProvinceFromRiak(Province_Riak pr)
		{
			Province oOut = null;
			oOut = new Province (pr);
			if (pr.overlordID != null)
			{
				Character oLord = pcMasterList[pr.overlordID];
				oOut.overlord = oLord;
			}
			return oOut;
		}

		/// <summary>
		/// Writes key list (List object) to Riak
		/// </summary>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="key">key of key list</param>
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
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="gc">GameClock to write</param>
		public bool writeClock(String gameID, GameClock gc)
		{

			var rClock = new RiakObject(gameID, gc.clockID, gc);
			var putClockResult = client.Put(rClock);

			if (! putClockResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: GameClock " + rClock.Key + " to bucket " + rClock.Bucket);
			}

			return putClockResult.IsSuccess;
		}

		/// <summary>
		/// Writes Skill object to Riak
		/// </summary>
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
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="npc">NonPlayerCharacter to write</param>
		public bool writeNPC(String gameID, NonPlayerCharacter npc)
		{

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
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="pc">PlayerCharacter to write</param>
		public bool writePC(String gameID, PlayerCharacter pc)
		{

			PlayerCharacter_Riak riakPC = this.PCtoRiak (pc);
			var rPC = new RiakObject(gameID, riakPC.charID, riakPC);
			var putPCresult = client.Put(rPC);

			if (! putPCresult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: PC " + rPC.Key + " to bucket " + rPC.Bucket);
			}

			return putPCresult.IsSuccess;
		}

		/// <summary>
		/// Writes Province object to Riak
		/// </summary>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="p">Province to write</param>
		public bool writeProvince(String gameID, Province p)
		{

			Province_Riak riakProv = this.ProvinceToRiak (p);
			var rProv = new RiakObject(gameID, riakProv.provinceID, riakProv);
			var putProvResult = client.Put(rProv);

			if (! putProvResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Province " + rProv.Key + " to bucket " + rProv.Bucket);
			}

			return putProvResult.IsSuccess;
		}

		/// <summary>
		/// Writes Terrain object to Riak
		/// </summary>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="t">Terrain to write</param>
		public bool writeTerrain(String gameID, Terrain t)
		{

			var rTerrain = new RiakObject(gameID, t.terrainCode, t);
			var putTerrainResult = client.Put(rTerrain);

			if (! putTerrainResult.IsSuccess)
			{
				System.Windows.Forms.MessageBox.Show("Write failed: Terrain " + rTerrain.Key + " to bucket " + rTerrain.Bucket);
			}

			return putTerrainResult.IsSuccess;
		}

		/// <summary>
		/// Writes Fief object to Riak
		/// </summary>
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="f">Fief to write</param>
		public bool writeFief(String gameID, Fief f)
		{

			Fief_Riak riakFief = this.FieftoRiak (f);
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
		/// <param name="gameID">Game (bucket) to write to</param>
		/// <param name="map">HexMapGraph containing edges collection</param>
		public bool writeMapEdges(String gameID, HexMapGraph map)
		{

			List<TaggedEdge<String, string>> riakMapEdges = this.EdgeCollection_to_Riak (map.myMap.Edges.ToList());

			var rMapE = new RiakObject(gameID, map.mapID + "E", riakMapEdges);
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
			foreach (KeyValuePair<string, Fief> fief in this.fiefMasterList)
			{
				fief.Value.updateFief();
			}

			// player characters
			foreach (KeyValuePair<string, PlayerCharacter> pc in this.pcMasterList)
			{
				if (pc.Value.health != 0)
				{
					pc.Value.updateCharacter();
				}
			}

			// player characters
			foreach (KeyValuePair<string, NonPlayerCharacter> npc in this.npcMasterList)
			{
				if (npc.Value.health != 0)
				{
					this.seasonUpdateNPC(npc.Value);
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
                bool success = false;
                Fief target = this.gameMap.chooseRandomHex(npc.location);
                if (target != null)
                {
                    double travelCost = this.getTravelCost(npc.location, target);
                    success = npc.moveCharacter(target, travelCost);
                }
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

            ListViewItem[] fiefsOwned = new ListViewItem[this.myChar.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < this.myChar.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                fiefsOwned[i] = new ListViewItem(this.myChar.ownedFiefs[i].name);
                fiefsOwned[i].SubItems.Add(this.myChar.ownedFiefs[i].fiefID);
                if (this.myChar.ownedFiefs[i] == this.myChar.location)
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
            textToDisplay += this.clock.seasons[this.clock.currentSeason] + ", " + this.clock.currentYear + ".  Your days left: " + this.myChar.days + "\r\n\r\n";
            textToDisplay += "Fief: " + this.myChar.location.name + " (" + this.myChar.location.fiefID + ")  in " + this.myChar.location.province.name + "\r\n\r\n";
            textToDisplay += "Owner: " + this.myChar.location.owner.name + "\r\n";
            textToDisplay += "Overlord: " + this.myChar.location.province.overlord.name + "\r\n";

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

            ListViewItem[] charsInCourt = new ListViewItem[this.myChar.location.characters.Count];
            // iterates through characters
            for (int i = 0; i < this.myChar.location.characters.Count; i++)
            {
                if (this.myChar.location.characters[i].inKeep == ifInKeep)
                {
                    // don't show this PlayerCharacter
                    if (this.myChar.location.characters[i] != this.myChar)
                    {
                        // Create an item and subitems for each character
                        charsInCourt[i] = new ListViewItem(this.myChar.location.characters[i].name);
                        charsInCourt[i].SubItems.Add(this.myChar.location.characters[i].charID);
                        charsInCourt[i].SubItems.Add("A household");
                        if (this.myChar.location.characters[i].isMale)
                        {
                            charsInCourt[i].SubItems.Add("Male");
                        }
                        else
                        {
                            charsInCourt[i].SubItems.Add("Female");
                        }
                        charsInCourt[i].SubItems.Add("A type");

                        bool isCompanion = false;
                        for (int ii = 0; ii < this.myChar.employees.Count; ii++)
                        {
                            if (this.myChar.employees[ii] == this.myChar.location.characters[i])
                            {
                                if (this.myChar.employees[ii].inEntourage)
                                {
                                    isCompanion = true;
                                }
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
            if (ch.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + ch.goTo.Peek().fiefID + "\r\n";
            }
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
                charText += "Your spouse's ID is: " + ch.spouse + "\r\n";
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
            pcText += "Employees:\r\n";
            for (int i = 0; i < ch.employees.Count; i++)
            {
                pcText += "  - " + ch.employees[i].name;
                if (ch.employees[i].inEntourage)
                {
                    pcText += " (travelling companion)";
                }
                pcText += "\r\n";
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
                npcText += "Hired by (ID): " + ch.myBoss + "\r\n";
            }
            npcText += "Potential salary: " + ch.calcNPCwage() + "\r\n";
            npcText += "Last offer from this PC: ";
            if (ch.lastOffer.ContainsKey(this.myChar.charID))
            {
                npcText += ch.lastOffer[this.myChar.charID];
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

            fiefText += "Terrain: " + f.terrain.description + "\r\n";
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
            fiefText += "Characters barred from keep (IDs):\r\n";
            for (int i = 0; i < f.barredCharacters.Count; i++)
            {
                fiefText += " " + f.barredCharacters[i] + "\r\n";
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
            foreach (KeyValuePair<string, Fief> entry in this.fiefMasterList)
            {
                entry.Value.updateFief();
            }
            // this.fModel.updateFief();
            foreach (KeyValuePair<string, PlayerCharacter> entry in this.pcMasterList)
            {
                entry.Value.updateCharacter();
            }

            foreach (KeyValuePair<string, NonPlayerCharacter> entry in this.npcMasterList)
            {
                entry.Value.updateCharacter();
            }
            // this.charModel.updateCharacter();
            this.clock.advanceSeason();
        }

        public void refreshCharacterContainer()
        {
            string textToDisplay = "";
            textToDisplay += this.displayCharacter(this.charToView);
            this.characterTextBox.Text = textToDisplay;

            if (this.myChar != this.charToView)
			{
                if (!this.myChar.employees.Contains(this.charToView))
				{
					this.charMultiMoveBtn.Enabled = false;
				}
			}
            this.characterContainer.BringToFront();
        }
        
        private void personalCharacteristicsAndAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.charToView = this.myChar;
            this.refreshCharacterContainer();
        }

        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.displayFief(this.myChar.location);
            this.fiefContainer.BringToFront();
        }

        private void updateCharacter_Click(object sender, EventArgs e)
        {
            // something
        }

        private void adjustTaxButton_Click(object sender, EventArgs e)
        {
            this.fiefToView.adjustTaxRate(Convert.ToDouble(this.adjustTaxTextBox.Text));
            this.displayFief(this.fiefToView);
        }

        private void adjOffSpend_Click(object sender, EventArgs e)
        {
            this.fiefToView.adjustOfficialsSpend(Convert.ToUInt32(this.adjOffSpendTextBox.Text));
            this.displayFief(this.fiefToView);
        }

        private void adjGarrSpendBtn_Click(object sender, EventArgs e)
        {
            this.fiefToView.adjustGarrisonSpend(Convert.ToUInt32(this.adjGarrSpendTextBox.Text));
            this.displayFief(this.fiefToView);
        }

        private void adjInfrSpendBtn_Click(object sender, EventArgs e)
        {
            this.fiefToView.adjustInfraSpend(Convert.ToUInt32(this.adjInfrSpendTextBox.Text));
            this.displayFief(this.fiefToView);
        }

        private void adjustKeepSpendBtn_Click(object sender, EventArgs e)
        {
            this.fiefToView.adjustKeepSpend(Convert.ToUInt32(this.adjustKeepSpendTextBox.Text));
            this.displayFief(this.fiefToView);
        }

        private void updateFiefBtn_Click(object sender, EventArgs e)
        {
            // something
        }

        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshTravelContainer();
            this.travelContainer.BringToFront();
        }

        /// <summary>
        /// Gtes travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="f">Target fief</param>
        private double getTravelCost(Fief source, Fief target)
        {
            double cost = 0;
            cost = ((2 + source.calcTerrainTravMod() + target.calcTerrainTravMod()) / 2) * this.clock.calcSeasonTravMod();
            return cost;
        }

        private void refreshTravelContainer()
        {
            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + this.myChar.location.name + "\r\n(" + this.myChar.location.province.name + ")";

            // get text for directional buttons
            // NE
            Fief targetNE = this.gameMap.getFief(this.myChar.location, "NE");
            if (targetNE != null)
            {
                this.travel_NE_btn.Text = "NE FIEF:\r\n\r\n";
                this.travel_NE_btn.Text += targetNE.name + " (" + targetNE.fiefID + ")\r\n";
                this.travel_NE_btn.Text += "(" + targetNE.province.name + ")\r\n\r\n";
                this.travel_NE_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetNE);
            }
            else
            {
                this.travel_NE_btn.Text = "NE FIEF:\r\n\r\nNo fief present";
            }

            // E
            Fief targetE = this.gameMap.getFief(this.myChar.location, "E");
            if (targetE != null)
            {
                this.travel_E_btn.Text = "E FIEF:\r\n\r\n";
                this.travel_E_btn.Text += targetE.name + " (" + targetE.fiefID + ")\r\n";
                this.travel_E_btn.Text += "(" + targetE.province.name + ")\r\n\r\n";
                this.travel_E_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetE);
            }
            else
            {
                this.travel_E_btn.Text = "E FIEF:\r\n\r\nNo fief present";
            }

            // SE
            Fief targetSE = this.gameMap.getFief(this.myChar.location, "SE");
            if (targetSE != null)
            {
                this.travel_SE_btn.Text = "SE FIEF:\r\n\r\n";
                this.travel_SE_btn.Text += targetSE.name + " (" + targetSE.fiefID + ")\r\n";
                this.travel_SE_btn.Text += "(" + targetSE.province.name + ")\r\n\r\n";
                this.travel_SE_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetSE);
            }
            else
            {
                this.travel_SE_btn.Text = "SE FIEF:\r\n\r\nNo fief present";
            }

            // SW
            Fief targetSW = this.gameMap.getFief(this.myChar.location, "SW");
            if (targetSW != null)
            {
                this.travel_SW_btn.Text = "SW FIEF:\r\n\r\n";
                this.travel_SW_btn.Text += targetSW.name + " (" + targetSW.fiefID + ")\r\n";
                this.travel_SW_btn.Text += "(" + targetSW.province.name + ")\r\n\r\n";
                this.travel_SW_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetSW);
            }
            else
            {
                this.travel_SW_btn.Text = "SW FIEF:\r\n\r\nNo fief present";
            }

            // W
            Fief targetW = this.gameMap.getFief(this.myChar.location, "W");
            if (targetW != null)
            {
                this.travel_W_btn.Text = "W FIEF:\r\n\r\n";
                this.travel_W_btn.Text += targetW.name + " (" + targetW.fiefID + ")\r\n";
                this.travel_W_btn.Text += "(" + targetW.province.name + ")\r\n\r\n";
                this.travel_W_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetW);
            }
            else
            {
                this.travel_W_btn.Text = "W FIEF:\r\n\r\nNo fief present";
            }

            // NW
            Fief targetNW = this.gameMap.getFief(this.myChar.location, "NW");
            if (targetNW != null)
            {
                this.travel_NW_btn.Text = "NW FIEF:\r\n\r\n";
                this.travel_NW_btn.Text += targetNW.name + " (" + targetNW.fiefID + ")\r\n";
                this.travel_NW_btn.Text += "(" + targetNW.province.name + ")\r\n\r\n";
                this.travel_NW_btn.Text += "Cost: " + this.getTravelCost(this.myChar.location, targetNW);
            }
            else
            {
                this.travel_NW_btn.Text = "NW FIEF:\r\n\r\nNo fief present";
            }

            // set text for 'enter/exit keep' button
            if (this.myChar.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
            }

        }

        private void travelBtnClick(object sender, EventArgs e)
        {
            bool success = false;
            Button button = sender as Button;
            Fief targetFief = this.gameMap.getFief(this.myChar.location, button.Tag.ToString());
            if (targetFief != null)
            {
                double travelCost = this.getTravelCost(this.myChar.location, targetFief);
                success = this.myChar.moveCharacter(targetFief, travelCost);
                if (success)
                {
                    this.fiefToView = targetFief;
                    this.refreshTravelContainer();
                }
            }
        }

        private bool characterMultiMove(Character ch)
        {
            bool success = false;
            double travelCost = 0;
            int steps = ch.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                travelCost = this.getTravelCost(ch.location, ch.goTo.Peek());
                success = ch.moveCharacter(ch.goTo.Peek(), travelCost);
                if (success)
                {
                    ch.goTo.Dequeue();
                }
                else
                {
                    break;
                }
            }

            if (ch == this.myChar)
            {
                // if myChar has moved, refresh display
                if (ch.goTo.Count < steps)
                {
                    this.fiefToView = this.myChar.location;
                    this.refreshCharacterContainer();
                }
            }

            return success;

        }

        private void myFiefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshMyFiefs();
            this.fiefsOwnedContainer.BringToFront();
        }

        private void viewBailiffBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefToView.bailiff != null)
            {
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(this.fiefToView.bailiff);
                this.characterTextBox.Text = textToDisplay;
                this.characterContainer.BringToFront();
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

            for (int i = 0; i < this.myChar.ownedFiefs.Count; i++)
            {
                if (this.myChar.ownedFiefs[i].fiefID.Equals(this.fiefsListView.SelectedItems[0].SubItems[1].Text))
                {
                    fiefToDisplay = this.myChar.ownedFiefs[i];
                }

            }

            if (fiefToDisplay != null)
            {
                this.fiefToView = fiefToDisplay;
                this.displayFief(fiefToView);
                this.fiefContainer.BringToFront();
            }
        }

        private void enterKeepBtn_Click(object sender, EventArgs e)
        {
            if (this.myChar.inKeep)
            {
                this.myChar.exitKeep();
                this.enterKeepBtn.Text = "Enter Keep";
                this.refreshTravelContainer();
            }
            else
            {
                this.myChar.enterKeep();
                this.enterKeepBtn.Text = "Exit Keep";
                this.refreshTravelContainer();
            }
        }

        private void visitCourtBtn1_Click(object sender, EventArgs e)
        {
            if (!this.myChar.inKeep)
            {
                this.myChar.enterKeep();
            }

            this.refreshMeetingPlaceDisplay("court");
            this.meetingPlaceCharDisplayTextBox.Text = "";
            this.meetingPlaceContainer.BringToFront();
        }

        private void visitTavernBtn_Click_1(object sender, EventArgs e)
        {
            if (this.myChar.inKeep)
            {
                this.myChar.exitKeep();
            }
            this.refreshMeetingPlaceDisplay("tavern");
            this.meetingPlaceCharDisplayTextBox.Text = "";
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

                        // set text for hire/fire button
                        if (this.myChar.employees.Contains(this.fiefToView.characters[i]))
                        {
                            this.hireNPC_Btn.Text = "Fire NPC";
                            this.hireNPC_TextBox.Visible = false;
                        }
                        else
                        {
                            this.hireNPC_Btn.Text = "Hire NPC";
                            this.hireNPC_TextBox.Visible = true;
                        }
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
            if (!this.myChar.employees.Contains(charToView))
            {
                bool offerAccepted = this.myChar.processEmployOffer((NonPlayerCharacter)charToView, Convert.ToUInt32(this.hireNPC_TextBox.Text));
            }
            else
            {
                this.myChar.fireNPC((NonPlayerCharacter)charToView);
            }

            string textToDisplay = "";
            textToDisplay += this.displayCharacter(charToView);
            this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
        }

        private void charMultiMoveBtn_Click(object sender, EventArgs e)
        {
            bool success = false;
            Fief target = fiefMasterList[this.charMultiMoveTextBox.Text];
            this.charToView.goTo = this.gameMap.getShortestPath(this.charToView.location, target);
            if (this.charToView.goTo.Count > 0)
            {
                success = this.characterMultiMove(this.charToView);
            }
        }

    }

}
