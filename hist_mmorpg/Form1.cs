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
        /// Holds CharacterModel
        /// </summary>
        private CharacterModel charModel;
        /// <summary>
        /// Holds FiefModel
        /// </summary>
        private FiefModel fModel;
        /// <summary>
        /// Holds initial Character to view
        /// </summary>
        private Character initialCharacter;
        /// <summary>
        /// Holds initial Fief to view
        /// </summary>
        private Fief initialFief;

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
            this.initGameObjects();
            // inform models of initial game objects
            cm.changeCurrent(initialCharacter);
            fm.changeCurrent(initialFief);
        }

        public void initGameObjects()
        {
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

            Fief myFief1 = new Fief("ESX02", "Cuckfield", myProv, 6000, "100", "200", "4001", 3.0, 3.0, 50, 9.10, 1000, 1000, 2000, 2000, 9.10, 1000, 1000, 2000, 100000, 5.63, 5.20, 'U', 'P', fief1Chars, keep1BarChars, false, false);
            Fief myFief2 = new Fief("ESX03", "Pulborough", myProv, 10000, "100", "200", "4001", 3.50, 0.20, 50, 9.10, 1000, 1000, 2000, 2000, 9.10, 1000, 1000, 2000, 2000, 5.63, 5.20, 'U', 'P', fief2Chars, keep2BarChars, false, false);
            Army myArmy = new Army(0, 0, 0, 0, 100, 0, "101", "401", 90);

            // create QuickGraph undirected graph
            // 1. create graph
            var myHexMap = new HexMapGraph();
            // 2. Add edge and auto create vertices
            myHexMap.addHexesAndRoute(myFief1, myFief2, "W");

            myHexMap.addHexesAndRoute(myFief2, myFief1, "E");

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

            // set inital character to display
            this.initialCharacter = myChar1;

            // set inital fief to display
            this.initialFief = myChar1.location;
        }

        // TODO
        public void displayCharacter(Character ch)
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
            charText += "Virility: " + ch.virility + "\r\n";
            charText += "Current location: " + ch.location.name + " (" + ch.location.province.name + ")\r\n";
            charText += "Language: " + ch.language + "\r\n";
            charText += "Days remaining: " + ch.days + "\r\n";
            charText += "Stature: " + ch.stature + "\r\n";
            charText += "Skills:";
            for (int i = 0; i < ch.skills.Length; i++)
            {
                charText += " " + ch.skills[i].name;
                if (i < (ch.skills.Length - 1))
                {
                    charText += ",";
                }
                else
                {
                    charText += "\r\n";
                }
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
                charText += "Your wife's ID is: " + ch.spouseID + "\r\n";
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
            charText += "Father's ID: " + ch.father + "\r\n";
            charText += "Head of family's ID: " + ch.familyHead + "\r\n";

            this.characterTextBox.Text = charText;

            bool isPC = ch is PlayerCharacter;
            if (isPC)
            {
                this.displayPlayerCharacter((PlayerCharacter)ch);
            }
            else
            {
                this.displayNonPlayerCharacter((NonPlayerCharacter)ch);
            }
        }

        // TODO
        public void displayPlayerCharacter(PlayerCharacter ch)
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

            this.characterTextBox.Text += pcText;
        }

        // TODO
        public void displayNonPlayerCharacter(NonPlayerCharacter ch)
        {
            string npcText = "";

            npcText += "Hired by (ID): " + ch.hiredBy + "\r\n";
            npcText += "Go to (Fief ID): " + ch.goTo + "\r\n";
            npcText += "Salary: " + ch.wage + "\r\n";

            this.characterTextBox.Text += npcText;
        }

        // TODO
        public void displayFief(Fief f)
        {
            string fiefText = "";

            fiefText += "ID: " + f.fiefID + "\r\n";
            fiefText += "Name: " + f.name + " (Province: " + f.province.name + ")\r\n";
            fiefText += "Population: " + f.population + "\r\n";
            fiefText += "Owner (ID): " + f.owner + "\r\n";
            fiefText += "Ancestral owner (ID): " + f.ancestralOwner + "\r\n";
            fiefText += "Bailiff (ID): " + f.bailiff + "\r\n";
            fiefText += "Troops: " + f.troops + "\r\n";
            fiefText += "Loyalty: " + f.keepLevel + "\r\n";
            fiefText += "Status: " + f.keepLevel + "\r\n";
            fiefText += "Terrain: " + f.keepLevel + "\r\n";
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

            fiefText += "Fields level: " + f.fields + "\r\n";
            fiefText += "Industry level: " + f.industry + "\r\n";
            fiefText += "GDP: " + f.calcGDP("this") + "\r\n";
            fiefText += "Tax rate: " + f.taxRate + "\r\n";
            fiefText += "Officials expenditure: " + f.officialsSpend + "\r\n";
            fiefText += "Garrison expenditure: " + f.garrisonSpend + "\r\n";
            fiefText += "Infrastructure expenditure: " + f.infrastructureSpend + "\r\n";
            fiefText += "Keep expenditure: " + f.keepSpend + "\r\n";
            fiefText += "Keep level: " + f.keepLevel + "\r\n";
            fiefText += "Income: " + f.calcIncome("this") + "\r\n";
            fiefText += "Family expenses: 0\r\n";
            fiefText += "Total expenses: " + f.calcExpenses("this") + "\r\n";
            fiefText += "Bottom line: " + f.calcBottomLine("this") + "\r\n\r\n";

            fiefText += "========= Next season =========\r\n";
            fiefText += "==== (with current bailiff) ===\r\n\r\n";

            fiefText += "Fields level: " + f.calcFieldLevel() + "\r\n";
            fiefText += "Industry level: " + f.calcIndustryLevel() + "\r\n";
            fiefText += "GDP: " + f.calcGDP("next") + "\r\n";
            fiefText += "Tax rate: " + f.taxRateNext + "\r\n";
            fiefText += "Officials expenditure: " + f.officialsSpendNext + "\r\n";
            fiefText += "Garrison expenditure: " + f.garrisonSpendNext + "\r\n";
            fiefText += "Infrastructure expenditure: " + f.infrastructureSpendNext + "\r\n";
            fiefText += "Keep expenditure: " + f.keepSpendNext + "\r\n";
            fiefText += "Keep level: " + f.calcKeepLevel() + "\r\n";
            fiefText += "Income: " + f.calcIncome("next") + "\r\n";
            fiefText += "Family expenses: 0\r\n";
            fiefText += "Total expenses: " + f.calcExpenses("next") + "\r\n";
            fiefText += "Bottom line: " + f.calcBottomLine("next") + "\r\n\r\n";

            this.fiefTextBox.Text = fiefText;
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
                    this.displayCharacter(this.charModel.currentCharacter);
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
            if (this.fiefContainer.Visible)
            {
                this.fiefContainer.Visible = false;
            }

            if (!this.characterContainer.Visible)
            {
                this.characterContainer.Visible = true;
            }
        }

        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.characterContainer.Visible)
            {
                this.characterContainer.Visible = false;
            }

            if (! this.fiefContainer.Visible)
            {
                this.fiefContainer.Visible = true;
            }
        }

        private void checkDeath_Click(object sender, EventArgs e)
        {
            this.charModel.checkDeath();
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

    }
}
