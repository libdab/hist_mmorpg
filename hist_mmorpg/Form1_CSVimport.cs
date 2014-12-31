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
    /// Partial class for Form1, containing functionality specific to importing from CSV file
    /// </summary>
    partial class Form1
    {

        /// <summary>
        /// Creates game objects using data imported from a CSV file and writes them to the database
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        /// <param name="resynch">bool indicating whether or not to resynch the game objects</param>
        public bool ImportFromCSV(string filename, string bucketID, bool resynch = false)
        {
            bool inputFileError = false;
            string lineIn;
            string[] lineParts;
            int lineNum = 0;
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
            Dictionary<string, Fief_Serialised> fiefMasterList = new Dictionary<string, Fief_Serialised>();
            Dictionary<string, PlayerCharacter_Serialised> pcMasterList = new Dictionary<string, PlayerCharacter_Serialised>();
            Dictionary<string, NonPlayerCharacter_Serialised> npcMasterList = new Dictionary<string, NonPlayerCharacter_Serialised>();
            Dictionary<string, Province_Serialised> provinceMasterList = new Dictionary<string, Province_Serialised>();
            Dictionary<string, Kingdom_Serialised> kingdomMasterList = new Dictionary<string, Kingdom_Serialised>();
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
                // increment lineNum
                lineNum++;

                // put the contents of the line into lineParts array, splitting on (char)9 (TAB)
                lineParts = lineIn.Split(',');

                try
                {

                    if (lineParts[0].Equals("fief"))
                    {
                        Fief_Serialised thisFiefSer = null;

                        thisFiefSer = this.importFromCSV_Fief(lineParts, lineNum);

                        if (thisFiefSer != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                fiefMasterList.Add(thisFiefSer.id, thisFiefSer);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_Fief(bucketID, fs: thisFiefSer);
                            }

                            // add fief id to keylist
                            fiefKeyList.Add(thisFiefSer.id);
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
                        Province_Serialised thisProvSer = null;
                        thisProvSer = this.importFromCSV_Prov(lineParts, lineNum);

                        if (thisProvSer != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                provinceMasterList.Add(thisProvSer.id, thisProvSer);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_Province(bucketID, ps: thisProvSer);
                            }

                            // add province id to keylist
                            provKeyList.Add(thisProvSer.id);
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

                    else if (lineParts[0].Equals("kingdom"))
                    {
                        Kingdom_Serialised thisKingSer = null;
                        thisKingSer = this.importFromCSV_Kingdom(lineParts, lineNum);

                        if (thisKingSer != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                kingdomMasterList.Add(thisKingSer.id, thisKingSer);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_Kingdom(bucketID, ks: thisKingSer);
                            }

                            // add kingdom id to keylist
                            kingKeyList.Add(thisKingSer.id);
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

                    else if (lineParts[0].Equals("pc"))
                    {
                        PlayerCharacter_Serialised thisPcSer = null;

                        thisPcSer = this.importFromCSV_PC(lineParts, lineNum);

                        if (thisPcSer != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                pcMasterList.Add(thisPcSer.charID, thisPcSer);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_PC(bucketID, pcs: thisPcSer);
                            }

                            // add id to keylist
                            pcKeyList.Add(thisPcSer.charID);
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
                        NonPlayerCharacter_Serialised thisNpcSer = null;
                        thisNpcSer = this.importFromCSV_NPC(lineParts, lineNum);

                        if (thisNpcSer != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                npcMasterList.Add(thisNpcSer.charID, thisNpcSer);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_NPC(bucketID, npcs: thisNpcSer);
                            }

                            // add id to keylist
                            npcKeyList.Add(thisNpcSer.charID);
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

                        thisSkill = this.importFromCSV_Skill(lineParts, lineNum);

                        if (thisSkill != null)
                        {
                            // save to database
                            this.databaseWrite_Skill(bucketID, thisSkill);

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
                        thisArmy = this.importFromCSV_Army(lineParts, lineNum);

                        if (thisArmy != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                armyMasterList.Add(thisArmy.armyID, thisArmy);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_Army(bucketID, thisArmy);
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

                    else if (lineParts[0].Equals("language"))
                    {
                        Language_Serialised thisLangSer = null;
                        thisLangSer = this.importFromCSV_Language(lineParts, lineNum);

                        if (thisLangSer != null)
                        {
                            // save to database
                            this.databaseWrite_Language(bucketID, ls: thisLangSer);

                            // add id to keylist
                            langKeyList.Add(thisLangSer.id);
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

                    else if (lineParts[0].Equals("baseLanguage"))
                    {
                        BaseLanguage thisBaseLang = null;
                        thisBaseLang = this.importFromCSV_BaseLanguage(lineParts, lineNum);

                        if (thisBaseLang != null)
                        {
                            // save to database
                            this.databaseWrite_BaseLanguage(bucketID, thisBaseLang);

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

                    else if (lineParts[0].Equals("nationality"))
                    {
                        Nationality thisNat = null;
                        thisNat = this.importFromCSV_Nationality(lineParts, lineNum);

                        if (thisNat != null)
                        {
                            // save to database
                            this.databaseWrite_Nationality(bucketID, thisNat);

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

                    else if (lineParts[0].Equals("rank"))
                    {
                        Rank thisRank = null;

                        thisRank = this.importFromCSV_Rank(lineParts, lineNum);

                        if (thisRank != null)
                        {
                            // save to database
                            //this.databaseWrite_Rank(bucketID, thisRank);

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
                        Position_Serialised thisPosSer = null;
                        thisPosSer = this.importFromCSV_Position(lineParts, lineNum);

                        if (thisPosSer != null)
                        {
                            // save to database
                            this.databaseWrite_Position(bucketID, ps: thisPosSer);                            

                            // add id to keylist
                            posKeyList.Add(thisPosSer.id);
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
                        thisSiege = this.importFromCSV_Siege(lineParts, lineNum);

                        if (thisSiege != null)
                        {
                            // check for resynching
                            if (resynch)
                            {
                                // add to masterList
                                siegeMasterList.Add(thisSiege.siegeID, thisSiege);
                            }

                            else
                            {
                                // save to database
                                this.databaseWrite_Siege(bucketID, thisSiege);
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

                    else if (lineParts[0].Equals("terrain"))
                    {
                        Terrain thisTerr = null;
                        thisTerr = this.importFromCSV_Terrain(lineParts, lineNum);

                        if (thisTerr != null)
                        {
                            // save to database
                            this.databaseWrite_Terrain(bucketID, thisTerr);

                            // add id to keylist
                            terrKeyList.Add(thisTerr.id);
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

                    // non-recognised object prefix
                    else
                    {
                        throw new InvalidDataException("Object prefix not recognised");
                    }
                }
                // catch exception that could be thrown by an invalid object prefix
                catch (InvalidDataException ide)
                {
                    // create and add sysAdmin JournalEntry
                    JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                    if (importErrorEntry != null)
                    {
                        importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                        Globals_Game.addPastEvent(importErrorEntry);
                    }

                    if (Globals_Client.showDebugMessages)
                    {
                        MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                    }
                }
            }

            // perform resynch if appropriate
            if (resynch)
            {
                // pass objects for resynching/writing
                this.SynchGameObjectCollections(fiefMasterList, pcMasterList, npcMasterList, provinceMasterList,
                    kingdomMasterList, siegeMasterList, armyMasterList, bucketID);
            }

            // save keyLists to database
            // fiefs
            if (fiefKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "fiefKeys", fiefKeyList);
            }

            // provinces
            if (provKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "provKeys", provKeyList);
            }

            // kingdoms
            if (kingKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "kingKeys", kingKeyList);
            }

            // PCs
            if (pcKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "pcKeys", pcKeyList);
            }

            // NPCs
            if (npcKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "npcKeys", npcKeyList);
            }

            // skills
            if (skillKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "skillKeys", skillKeyList);
            }

            // armies
            if (armyKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "armyKeys", armyKeyList);
            }

            // languages
            if (langKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "langKeys", langKeyList);
            }

            // baseLanguages
            if (baseLangKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "baseLangKeys", baseLangKeyList);
            }

            // nationalities
            if (natKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "nationalityKeys", natKeyList);
            }

            // ranks
            if (rankKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "rankKeys", rankKeyList);
            }

            // positions
            if (posKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "positionKeys", posKeyList);
            }

            // sieges
            if (siegeKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "siegeKeys", siegeKeyList);
            }

            // terrains
            if (terrKeyList.Count > 0)
            {
                // save keylist to database
                this.databaseWrite_KeyList(bucketID, "terrKeys", terrKeyList);
            }

            return inputFileError;
        }

        /// <summary>
        /// Creates a Fief_Serialised object using data in a string array
        /// </summary>
        /// <returns>Fief_Serialised object</returns>
        /// <param name="fiefData">string[] holding source data</param>
        public Fief_Serialised importFromCSV_Fief(string[] fiefData, int lineNum)
        {
            Fief_Serialised thisFiefSer = null;

            try
            {
                // create empty lists for variable length collections
                // (characters, barredChars, armies)
                List<string> characters = new List<string>();
                List<string> barredChars = new List<string>();
                List<string> barredNats = new List<string>();
                List<string> armies = new List<string>();

                // check to see if any data present for variable length collections
                if (fiefData.Length > 57)
                {
                    // create variables to hold start/end index positions
                    int chStart, chEnd, barChStart, barChEnd, barNatStart, barNatEnd, arStart, arEnd;
                    chStart = chEnd = barChStart = barChEnd = barNatStart = barNatEnd = arStart = arEnd = -1;

                    // iterate through main list STORING START/END INDEX POSITIONS
                    for (int i = 57; i < fiefData.Length; i++)
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
                        else if (fiefData[i].Equals("barNatStart"))
                        {
                            barNatStart = i;
                        }
                        else if (fiefData[i].Equals("barNatEnd"))
                        {
                            barNatEnd = i;
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

                    // barredNats
                    if ((barNatStart > -1) && (barNatEnd > -1))
                    {
                        for (int i = barNatStart + 1; i < barNatEnd; i++)
                        {
                            barredNats.Add(fiefData[i]);
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

                if (!String.IsNullOrWhiteSpace(fiefData[52]))
                {
                    tiHo = fiefData[52];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[53]))
                {
                    own = fiefData[53];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[54]))
                {
                    ancOwn = fiefData[54];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[55]))
                {
                    bail = fiefData[55];
                }
                if (!String.IsNullOrWhiteSpace(fiefData[56]))
                {
                    sge = fiefData[56];
                }

                // create Fife_Serialised object
                thisFiefSer = new Fief_Serialised(fiefData[1], fiefData[2], fiefData[3], Convert.ToInt32(fiefData[4]),
                    Convert.ToDouble(fiefData[5]), Convert.ToDouble(fiefData[6]), Convert.ToUInt32(fiefData[7]),
                    Convert.ToDouble(fiefData[8]), Convert.ToDouble(fiefData[9]), Convert.ToUInt32(fiefData[10]),
                    Convert.ToUInt32(fiefData[11]), Convert.ToUInt32(fiefData[12]), Convert.ToUInt32(fiefData[13]),
                    finCurr, finPrev, Convert.ToDouble(fiefData[42]), Convert.ToDouble(fiefData[43]),
                    Convert.ToChar(fiefData[44]), fiefData[45], fiefData[46], characters, barredChars,
                    barredNats, Convert.ToDouble(fiefData[47]), Convert.ToInt32(fiefData[48]), armies,
                    Convert.ToBoolean(fiefData[49]), new Dictionary<string, string[]>(), Convert.ToBoolean(fiefData[50]),
                    Convert.ToByte(fiefData[51]), tiHo: tiHo, own: own, ancOwn: ancOwn, bail: bail, sge: sge);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisFiefSer;
        }

        /// <summary>
        /// Creates a Province_Serialised object using data in a string array
        /// </summary>
        /// <returns>Province_Serialised object</returns>
        /// <param name="provData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Province_Serialised importFromCSV_Prov(string[] provData, int lineNum)
        {
            Province_Serialised thisProvSer = null;

            try
            {
                if (provData.Length != 8)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Province object.");
                }

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

                // create Province_Serialised object
                thisProvSer = new Province_Serialised(provData[1], provData[2], Convert.ToByte(provData[3]),
                    Convert.ToDouble(provData[4]), tiHo, own, kingdom);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisProvSer;
        }

        /// <summary>
        /// Creates a Kingdom_Serialised object using data in a string array
        /// </summary>
        /// <returns>Kingdom_Serialised object</returns>
        /// <param name="kingData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Kingdom_Serialised importFromCSV_Kingdom(string[] kingData, int lineNum)
        {
            Kingdom_Serialised thisKingSer = null;

            try
            {
                if (kingData.Length != 7)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Kingdom object.");
                }

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

                // create Kingdom_Serialised object
                thisKingSer = new Kingdom_Serialised(kingData[1], kingData[2], Convert.ToByte(kingData[3]), kingData[4],
                    tiHo, own);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisKingSer;
        }

        /// <summary>
        /// Creates a PlayerCharacter_Serialised object using data in a string array
        /// </summary>
        /// <returns>PlayerCharacter_Serialised object</returns>
        /// <param name="pcData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public PlayerCharacter_Serialised importFromCSV_PC(string[] pcData, int lineNum)
        {
            PlayerCharacter_Serialised thisPcSer = null;

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
                        if (Utility_Methods.IsOdd(skStart + skEnd))
                        {
                            for (int i = skStart + 1; i < skEnd; i = i + 2)
                            {
                                Tuple<string, int> thisSkill = new Tuple<string, int>(pcData[i], Convert.ToInt32(pcData[i + 1]));
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
                    Tuple<Skill, int>[] generatedSkills = Utility_Methods.generateSkillSet();

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
                Tuple<uint, byte> dob = new Tuple<uint, byte>(Convert.ToUInt32(pcData[4]), Convert.ToByte(pcData[5]));

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

                // create PlayerCharacter_Serialised object
                thisPcSer = new PlayerCharacter_Serialised(pcData[1], pcData[2], pcData[3], dob, Convert.ToBoolean(pcData[6]),
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
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisPcSer;
        }

        /// <summary>
        /// Creates a NonPlayerCharacter_Serialised object using data in a string array
        /// </summary>
        /// <returns>NonPlayerCharacter_Serialised object</returns>
        /// <param name="npcData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public NonPlayerCharacter_Serialised importFromCSV_NPC(string[] npcData, int lineNum)
        {
            NonPlayerCharacter_Serialised thisNpcSer = null;

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
                        if (Utility_Methods.IsOdd(skStart + skEnd))
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
                    Tuple<Skill, int>[] generatedSkills = Utility_Methods.generateSkillSet();

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

                // create NonPlayerCharacter_Serialised object
                thisNpcSer = new NonPlayerCharacter_Serialised(npcData[1], npcData[2], npcData[3], dob, Convert.ToBoolean(npcData[6]),
                    npcData[7], Convert.ToBoolean(npcData[8]), Convert.ToDouble(npcData[9]), Convert.ToDouble(npcData[10]),
                    new List<string>(), npcData[11], Convert.ToDouble(npcData[12]), Convert.ToDouble(npcData[13]),
                    Convert.ToDouble(npcData[14]), Convert.ToDouble(npcData[15]), skills, Convert.ToBoolean(npcData[16]),
                    Convert.ToBoolean(npcData[17]), npcData[18], npcData[19], npcData[20], npcData[21], myTitles, npcData[22],
                    Convert.ToUInt32(npcData[23]), Convert.ToBoolean(npcData[24]), Convert.ToBoolean(npcData[25]),
                    ails: new Dictionary<string, Ailment>(), loc: loc, aID: aID, empl: boss);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisNpcSer;
        }

        /// <summary>
        /// Creates a Skill object using data in a string array
        /// </summary>
        /// <returns>Skill object</returns>
        /// <param name="skillData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Skill importFromCSV_Skill(string[] skillData, int lineNum)
        {
            Skill thisSkill = null;

            try
            {
                // create empty lists for variable length collections
                // (effects)
                Dictionary<string, double> effects = new Dictionary<string, double>();

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
                        if (Utility_Methods.IsOdd(effStart + effEnd))
                        {
                            for (int i = effStart + 1; i < effEnd; i = i + 2)
                            {
                                effects.Add(skillData[i], Convert.ToDouble(skillData[i + 1]));
                            }
                        }
                    }
                }

                // create Skill object
                if (effects.Count > 0)
                {
                    thisSkill = new Skill(skillData[1], skillData[2], effects);
                }
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisSkill;
        }

        /// <summary>
        /// Creates a Army object using data in a string array
        /// </summary>
        /// <returns>Army object</returns>
        /// <param name="armyData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Army importFromCSV_Army(string[] armyData, int lineNum)
        {
            Army thisArmy = null;

            try
            {
                if (armyData.Length != 16)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Army object.");
                }

                // create troops array
                uint[] troops = new uint[] { Convert.ToUInt32(armyData[9]), Convert.ToUInt32(armyData[10]),
                    Convert.ToUInt32(armyData[11]), Convert.ToUInt32(armyData[12]), Convert.ToUInt32(armyData[13]),
                    Convert.ToUInt32(armyData[14]), Convert.ToUInt32(armyData[15])};

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
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisArmy;
        }

        /// <summary>
        /// Creates a Language_Serialised object using data in a string array
        /// </summary>
        /// <returns>Language_Serialised object</returns>
        /// <param name="langData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Language_Serialised importFromCSV_Language(string[] langData, int lineNum)
        {
            Language_Serialised thisLangSer = null;

            try
            {
                if (langData.Length != 4)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Language object.");
                }

                // create Language_Serialised object
                thisLangSer = new Language_Serialised(langData[1], langData[2], Convert.ToInt32(langData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisLangSer;
        }

        /// <summary>
        /// Creates a BaseLanguage object using data in a string array
        /// </summary>
        /// <returns>BaseLanguage object</returns>
        /// <param name="baseLangData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public BaseLanguage importFromCSV_BaseLanguage(string[] baseLangData, int lineNum)
        {
            BaseLanguage thisBaseLang = null;

            try
            {
                if (baseLangData.Length != 3)
                {
                    throw new InvalidDataException("Incorrect number of data parts for BaseLanguage object.");
                }

                // create BaseLanguage object
                thisBaseLang = new BaseLanguage(baseLangData[1], baseLangData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisBaseLang;
        }

        /// <summary>
        /// Creates a Nationality object using data in a string array
        /// </summary>
        /// <returns>Nationality object</returns>
        /// <param name="natData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Nationality importFromCSV_Nationality(string[] natData, int lineNum)
        {
            Nationality thisNat = null;

            try
            {
                if (natData.Length != 3)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Nationality object.");
                }

                // create Nationality object
                thisNat = new Nationality(natData[1], natData[2]);
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisNat;
        }

        /// <summary>
        /// Creates a Rank object using data in a string array
        /// </summary>
        /// <returns>Rank object</returns>
        /// <param name="rankData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Rank importFromCSV_Rank(string[] rankData, int lineNum)
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
                    if (Utility_Methods.IsOdd(tiStart + tiEnd))
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
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisRank;
        }

        /// <summary>
        /// Creates a Position_Serialised object using data in a string array
        /// </summary>
        /// <returns>Position_Serialised object</returns>
        /// <param name="posData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Position_Serialised importFromCSV_Position(string[] posData, int lineNum)
        {
            Position_Serialised thisPosSer = null;

            try
            {
                // create empty lists for variable length collections
                // (title)
                TitleName[] title = null;

                // create variables to hold start/end index positions
                int tiStart, tiEnd;
                tiStart = tiEnd = -1;

                // iterate through main list STORING START/END INDEX POSITIONS
                for (int i = 5; i < posData.Length; i++)
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
                    if (Utility_Methods.IsOdd(tiStart + tiEnd))
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
                    thisPosSer = new Position_Serialised(Convert.ToByte(posData[1]), title, Convert.ToByte(posData[2]), posData[3], posData[4]);
                }

            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisPosSer;
        }

        /// <summary>
        /// Creates a Siege object using data in a string array
        /// </summary>
        /// <returns>Siege object</returns>
        /// <param name="siegeData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Siege importFromCSV_Siege(string[] siegeData, int lineNum)
        {
            Siege thisSiege = null;

            try
            {
                if (siegeData.Length != 16)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Siege object.");
                }

                // check for presence of conditional values
                string totAttCas, totDefCas, totDays, defenderAdd, siegeEnd;
                totAttCas = totDefCas = totDays = defenderAdd = siegeEnd = null;

                if (!String.IsNullOrWhiteSpace(siegeData[11]))
                {
                    totAttCas = siegeData[11];
                }
                else
                {
                    totAttCas = "0";
                }
                if (!String.IsNullOrWhiteSpace(siegeData[12]))
                {
                    totDefCas = siegeData[12];
                }
                else
                {
                    totDefCas = "0";
                }
                if (!String.IsNullOrWhiteSpace(siegeData[13]))
                {
                    totDays = siegeData[13];
                }
                else
                {
                    totDays = "0";
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
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisSiege;
        }

        /// <summary>
        /// Creates a Terrain object using data in a string array
        /// </summary>
        /// <returns>Terrain object</returns>
        /// <param name="terrData">string[] holding source data</param>
        /// <param name="lineNum">Line number in source file</param>
        public Terrain importFromCSV_Terrain(string[] terrData, int lineNum)
        {
            Terrain thisTerr = null;

            try
            {
                if (terrData.Length != 4)
                {
                    throw new InvalidDataException("Incorrect number of data parts for Terrain object.");
                }
                
                // create Terrain object
                thisTerr = new Terrain(terrData[1], terrData[2], Convert.ToDouble(terrData[3]));
            }
            // catch exception that could result from incorrect conversion of string to numeric 
            catch (FormatException fe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + fe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + fe.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (ArgumentOutOfRangeException aoore)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + aoore.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + aoore.Message);
                }
            }
            // catch exception that could be thrown by several checks in the Fief constructor
            catch (InvalidDataException ide)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + ide.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + ide.Message);
                }
            }
            // catch exception that could result from incorrect numeric values
            catch (OverflowException oe)
            {
                // create and add sysAdmin JournalEntry
                JournalEntry importErrorEntry = Utility_Methods.createSysAdminJentry();
                if (importErrorEntry != null)
                {
                    importErrorEntry.description = "Line " + lineNum + ": " + oe.Message;
                    Globals_Game.addPastEvent(importErrorEntry);
                }

                if (Globals_Client.showDebugMessages)
                {
                    MessageBox.Show("Line " + lineNum + ": " + oe.Message);
                }
            }

            return thisTerr;
        }

        /// <summary>
        /// Uses individual game objects to populate variable-length collections within other game objects
        /// </summary>
        /// <param name="fiefMasterList">Fief_Serialised objects</param>
        /// <param name="pcMasterList">PlayerCharacter_Serialised objects</param>
        /// <param name="npcMasterList">NonPlayerCharacter_Serialised objects</param>
        /// <param name="provinceMasterList">Province_Serialised objects</param>
        /// <param name="kingdomMasterList">Kingdom_Serialised objects</param>
        /// <param name="siegeMasterList">Siege objects</param>
        /// <param name="armyMasterList">Army objects</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        public void SynchGameObjectCollections(Dictionary<string, Fief_Serialised> fiefMasterList, Dictionary<string,
            PlayerCharacter_Serialised> pcMasterList, Dictionary<string, NonPlayerCharacter_Serialised> npcMasterList,
            Dictionary<string, Province_Serialised> provinceMasterList, Dictionary<string, Kingdom_Serialised> kingdomMasterList,
            Dictionary<string, Siege> siegeMasterList, Dictionary<string, Army> armyMasterList, string bucketID)
        {

            // iterate through FIEFS
            foreach (KeyValuePair<string, Fief_Serialised> fiefEntry in fiefMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(fiefEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
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
                if (!String.IsNullOrWhiteSpace(fiefEntry.Value.owner))
                {
                    PlayerCharacter_Serialised thisOwner = null;
                    if (pcMasterList.ContainsKey(fiefEntry.Value.owner))
                    {
                        thisOwner = pcMasterList[fiefEntry.Value.owner];
                    }

					// put fief in owner's ownedFiefs
					if (thisOwner != null)
					{
						if (!thisOwner.ownedFiefs.Contains(fiefEntry.Key))
						{
							thisOwner.ownedFiefs.Add(fiefEntry.Key);
						}
					}
                }
            }

            // iterate through PROVINCES
            foreach (KeyValuePair<string, Province_Serialised> provEntry in provinceMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(provEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
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
                if (!String.IsNullOrWhiteSpace(provEntry.Value.owner))
                {
                    PlayerCharacter_Serialised thisOwner = null;
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
            foreach (KeyValuePair<string, Kingdom_Serialised> kingEntry in kingdomMasterList)
            {
                // get titleHolder
                if (!String.IsNullOrWhiteSpace(kingEntry.Value.titleHolder))
                {
                    Character_Serialised thisTiHo = null;
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
            foreach (KeyValuePair<string, PlayerCharacter_Serialised> pcEntry in pcMasterList)
            {
                if (pcEntry.Value.isAlive)
                {
                    // get location
                    if (!String.IsNullOrWhiteSpace(pcEntry.Value.location))
                    {
                        Fief_Serialised thisFief = null;
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
            foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> npcEntry in npcMasterList)
            {
                if (npcEntry.Value.isAlive)
                {
                    // get location
                    if (!String.IsNullOrWhiteSpace(npcEntry.Value.location))
                    {
                        Fief_Serialised thisFief = null;
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
                    if (!String.IsNullOrWhiteSpace(npcEntry.Value.employer))
                    {
                        PlayerCharacter_Serialised thisBoss = null;
                        if (pcMasterList.ContainsKey(npcEntry.Value.employer))
                        {
                            thisBoss = pcMasterList[npcEntry.Value.employer];
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
                        PlayerCharacter_Serialised thisHeadOfFamily = null;
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
                        PlayerCharacter_Serialised attacker = null;
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
                        PlayerCharacter_Serialised defender = null;
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
                        Fief_Serialised besiegedFief = null;
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
                    PlayerCharacter_Serialised owner = null;
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
                    Character_Serialised leader = null;
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
                    Fief_Serialised thisFief = null;
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
            foreach (KeyValuePair<string, Fief_Serialised> fiefEntry in fiefMasterList)
            {
                // save to database
                this.databaseWrite_Fief(bucketID, fs: fiefEntry.Value);
            }
            foreach (KeyValuePair<string, Province_Serialised> provEntry in provinceMasterList)
            {
                // save to database
                this.databaseWrite_Province(bucketID, ps: provEntry.Value);
            }
            foreach (KeyValuePair<string, Kingdom_Serialised> kingEntry in kingdomMasterList)
            {
                // save to database
                this.databaseWrite_Kingdom(bucketID, ks: kingEntry.Value);
            }
            foreach (KeyValuePair<string, PlayerCharacter_Serialised> pcEntry in pcMasterList)
            {
                // save to database
                this.databaseWrite_PC(bucketID, pcs: pcEntry.Value);
            }
            foreach (KeyValuePair<string, NonPlayerCharacter_Serialised> npcEntry in npcMasterList)
            {
                // save to database
                this.databaseWrite_NPC(bucketID, npcs: npcEntry.Value);
            }
            foreach (KeyValuePair<string, Siege> siegeEntry in siegeMasterList)
            {
                // save to database
                this.databaseWrite_Siege(bucketID, siegeEntry.Value);
            }
            foreach (KeyValuePair<string, Army> armyEntry in armyMasterList)
            {
                // save to database
                this.databaseWrite_Army(bucketID, armyEntry.Value);
            }

        }

        /// <summary>
        /// Creates game map using data imported from a CSV file and writes it to the database
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
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
                    for (int i = 0; i < mapHexes.GetLength(1); i++)
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
            this.databaseWrite_MapEdges(bucketID, edges: mapEdges);

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
                            if (!String.IsNullOrWhiteSpace(mapArray[i, j - 1]))
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
                            if (!((!Utility_Methods.IsOdd(i)) && (j == 0)))
                            {
                                // target correct column (above left is different for odd/even numbered rows)
                                if (Utility_Methods.IsOdd(i))
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
                            if (!((Utility_Methods.IsOdd(i)) && (j == mapArray.GetLength(1) - 1)))
                            {
                                // target correct column (above right is different for odd/even numbered rows)
                                if (Utility_Methods.IsOdd(i))
                                {
                                    col = j + 1;
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
