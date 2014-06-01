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
        public Form1()
        {
            InitializeComponent();
            TestProg();
        }

        // to be placed in controller class
        public Fief moveCharacter(Fief f, string whereTo)
        {
            Fief current = f;

            return current;
        }

        public void TestProg()
        {

            string toDisplay = "";

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

            // create chars lists for fiefs
            List<Character> fief1Chars = new List<Character>();
            List<Character> fief2Chars = new List<Character>();

            // create province for fiefs
            Province myProv = new Province("ESX00", "Sussex, England", 100, 6.2, "E1");

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, "100", "200", "4001", 3.0, 3.0, 50, 9.10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', 'P', fief1Chars, keep1BarChars, false, false);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, "100", "200", "4001", 3.50, 0.20, 50, 9.10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', 'P', fief2Chars, keep2BarChars, false, false);
            Army myArmy = new Army(0, 0, 0, 0, 100, 0, "101", "401", 90);

            // create QuickGraph undirected graph
            // 1. create graph
            var myHexMap = new HexMapGraph();
            // 2. Add edge and auto create vertices
            myHexMap.addHexesAndRoute(myFief1, myFief2, "W");


            toDisplay += "myHexMap edges from myFief1: " + "\r\n";
            foreach (var e in myHexMap.myMap.Edges)
                if (e.Source == myFief1)
                {
                    toDisplay += e.Tag + " " + e.Source.fiefID + " -> " + e.Target.fiefID + "\r\n";
                }

            myHexMap.addHexesAndRoute(myFief2, myFief1, "E");

            toDisplay += "myHexMap edges from myFief2: " + "\r\n";
            foreach (var e in myHexMap.myMap.Edges)
                if (e.Source == myFief2)
                {
                    toDisplay += e.Tag + " " + e.Source.fiefID + " -> " + e.Target.fiefID + "\r\n";
                }

            // create entourages for PCs
            List<Character> myEnt1 = new List<Character>();
            List<Character> myEnt2 = new List<Character>();

            // create some characters
            PlayerCharacter myChar1 = new PlayerCharacter("101", "Dave Bond", 50, true, "Fr", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, skillsArray1, false, true, "101", false, "99", "98", false, 13000, myEnt1);
            PlayerCharacter myChar2 = new PlayerCharacter("102", "Bave Dond", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, skillsArray1, true, false, "NA", false, "99", "98", false, 13000, myEnt2);
            NonPlayerCharacter myNPC1 = new NonPlayerCharacter("401", "Jimmy Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, skillsArray1, false, false, "NA", false, "99", "98", "100", "ESX05", 10000);
            NonPlayerCharacter myNPC2 = new NonPlayerCharacter("402", "Johnny Servant", 50, true, "Eng", 1.0, 8.50, 6.0, myFief1, "E1", 0, 4.0, skillsArray1, false, false, "NA", false, "99", "98", "100", "ESX05", 10000);

            // add NPC to entourage
            myChar1.addToEntourage(myNPC2);
            
            // add some characters to myFief1
            myFief1.addCharacter(myChar1);
            myFief1.addCharacter(myChar2);
            myFief1.addCharacter(myNPC1);
            myFief1.addCharacter(myNPC2);

            // bar a character from the myFief1 keep
            myFief1.barCharacter(myNPC2.charID);

            toDisplay += "myFief1 Province: " + myFief1.province.name + "\r\n";
            toDisplay += "myFief1 GDP: " + myFief1.calcGDP() + "\r\n";
            toDisplay += "myFief2 Province: " + myFief2.province.name + "\r\n\r\n";

            myProv.name = "Suffolk, England";

            toDisplay += "myFief1 Province: " + myFief1.province.name + "\r\n";
            toDisplay += "myFief2 Province: " + myFief2.province.name + "\r\n\r\n";

            myFief2.province.name = "Norfolk, England";

            toDisplay += "myProv name: " + myProv.name + "\r\n\r\n";
            
            Boolean dead = myChar1.checkDeath();
            if (dead)
            {
                toDisplay += "myChar1 is dead!\r\n\r\n";
            }
            else
            {
                toDisplay += "Gordon's alive!\r\n\r\n";
            }

            int totModifier = 0;
            for (int i = 0; i < myChar1.skills.Length; i++)
            {
                toDisplay += "myChar1 Skill " + i + " = " + myChar1.skills[i].name + "\r\n";
                foreach (KeyValuePair<string, int> entry in myChar1.skills[i].effects)
                {
                    if (entry.Key.Equals("death"))
                    {
                        totModifier += entry.Value;
                    }
                }
            }
            toDisplay += "myChar1 Total 'death' modifier = " + totModifier + "\r\n\r\n";

            bool enteredKeep = myChar1.enterKeep();
            toDisplay += "myChar1's attempt to enter the keep was ";
            if (enteredKeep)
            {
                toDisplay += "successful.";
            }
            else
            {
                toDisplay += "unsuccessful (barred).";
            }

            toDisplay += "\r\n\r\n";

            toDisplay += "Characters in myFief1 (keep): ";
            for (int i = 0; i < myFief1.characters.Count; i++)
            {
                if (myFief1.characters[i].inKeep) 
                {
                    toDisplay += myFief1.characters[i].charID + " ";
                }
            }
            toDisplay += "\r\n\r\n";

            toDisplay += "Characters in myChar1's entourage: ";
            for (int i = 0; i < myChar1.entourage.Count; i++)
            {
                toDisplay += myChar1.entourage[i].charID + " ";
            }
            toDisplay += "\r\n\r\n";

            toDisplay += "Characters in myFief1 (outside keep): ";
            for (int i = 0; i < myFief1.characters.Count; i++)
            {
                if (! myFief1.characters[i].inKeep)
                {
                    toDisplay += myFief1.characters[i].charID + " ";
                }
            }
            toDisplay += "\r\n\r\n";

            toDisplay += "myChar2 location: " + myChar2.location.fiefID + "\r\n\r\n";

            myHexMap.moveCharacter(myChar1,myFief1, "W");
            myHexMap.moveCharacter(myNPC1, myFief1, "W");

            // toDisplay += "myChar2 location: " + myChar2.location.data.fiefID + "\r\n\r\n";

            toDisplay += "All characters in myFief1: ";
            for (int i = 0; i < myFief1.characters.Count; i++)
            {
                toDisplay += myFief1.characters[i].charID + " ";
            }
            toDisplay += "\r\n";
            toDisplay += "All characters in myFief2: ";
            for (int i = 0; i < myFief2.characters.Count; i++)
            {
                toDisplay += myFief2.characters[i].charID + " ";
            }
            toDisplay += "\r\n";
            toDisplay += "myChar2 location: " + myChar2.location.fiefID + "\r\n\r\n";

            toDisplay += "Effects in skill command: ";
            foreach (KeyValuePair<string, int> entry in command.effects)
            {
                toDisplay += " " + entry.Key;
            }

            this.textBox1.Text = toDisplay;


        }

        /// <summary>
        /// Updates appropriate display elements when data received from model
        /// </summary>
        /// <param name="info">String containing data about display element to update</param>
        public void update(String info)
        {
            // update UI components
        }

    }
}
