using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    public class FiefModel : Mmorpg_Model
    {
        /// <summary>
        /// List of observers registered to observe this subject
        /// </summary>
        private List<Form1> registeredObservers = new List<Form1>();

        /// <summary>
        /// Holds current Fief
        /// </summary>
        internal Fief currentFief;



        /// <summary>
        /// Constructor for FiefModel
        /// </summary>
        public FiefModel()
        {
        }

        /// <summary>
        /// Performs any necessary initialisation for thread
        /// </summary>
        public void runThread()
        {
        }

        /// <summary>
        /// Changes current Fief
        /// </summary>
        /// <param name="f">Fief to set as current</param>
        public void changeCurrent(Fief f)
        {
            this.currentFief = f;
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Calculates fief population increase of current Fief
        /// </summary>
        /// <param name="ch">Character to check for death</param>
        public void calcNewPop()
        {
            uint newPop = this.currentFief.calcNewPop();
            this.currentFief.population = newPop;
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Calculates fief income of current Fief
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int calcIncome()
        {
            int fiefIncome = this.currentFief.calcIncome("this");
            return fiefIncome;
        }

        /// <summary>
        /// Calculates fief expenses of current Fief
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int calcExpenses()
        {
            int fiefExpenses = this.currentFief.calcExpenses("this");
            return fiefExpenses;
        }

        /// <summary>
        /// Calculates fief bottom line of current Fief
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int calcBottomLine()
        {
            int fiefBottomLine = this.currentFief.calcBottomLine("this");
            return fiefBottomLine;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// </summary>
        /// <param name="tx">double containing new tax rate</returns>
        public void adjustTx(double tx)
        {
            this.currentFief.adjustTaxRate(tx);
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</returns>
        public void adjustOffSpend(uint os)
        {
            this.currentFief.adjustOfficialsSpend(os);
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</returns>
        public void adjustInfSpend(uint infs)
        {
            this.currentFief.adjustInfraSpend(infs);
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</returns>
        public void adjustGarrSpend(uint gs)
        {
            this.currentFief.adjustGarrisonSpend(gs);
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</returns>
        public void adjustKpSpend(uint ks)
        {
            this.currentFief.adjustKeepSpend(ks);
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Updates fief data at the end/beginning of the season
        /// </summary>
        public void updateFief()
        {
            this.currentFief.updateFief();
            this.notifyObservers("refreshFief");
        }

        /*
        /// <summary>
        /// Sets new fief field level (from next season's spend)
        /// </summary>
        public void setNewFieldLvl()
        {
            this.currentFief.fields = this.currentFief.calcFieldLevel();
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Sets new fief industry level (from next season's spend)
        /// </summary>
        public void calcIndustryLvl()
        {
            this.currentFief.industry = this.currentFief.calcIndustryLevel();
            this.notifyObservers("refreshFief");
        }

        /// <summary>
        /// Sets new fief keep level (from next season's spend)
        /// </summary>
        public void calcKeepLvl()
        {
            this.currentFief.keepLevel = this.currentFief.calcKeepLevel();
            this.notifyObservers("refreshFief");
        }
        */

        /// <summary>
        /// Adds a view (Form1 object) to the list of registered views
        /// </summary>
        /// <param name="obs">Form1 object to be added</param>
        public void registerObserver(Form1 obs)
        {
            // add new view to list
            registeredObservers.Add(obs);
        }

        /// <summary>
        /// Removes a view (Form1 object) from the list of registered views
        /// </summary>
        /// <param name="obs">Form1 object to be removed</param>
        public void removeObserver(Form1 obs)
        {
            // remove view from list
            registeredObservers.Remove(obs);
        }

        /// <summary>
        /// Notifies all views (Form1 objects) in the list of registered views
        /// that a change has occured to data in the model
        /// </summary>
        /// <param name="info">String indicating the type of data that has been changed</param>
        public void notifyObservers(String info)
        {
            // iterate through list of views
            foreach (Form1 obs in registeredObservers)
            {
                // call view's update method to perform the required actions
                // based on the string passed
                obs.update(info);
            }
        }
    }
}
