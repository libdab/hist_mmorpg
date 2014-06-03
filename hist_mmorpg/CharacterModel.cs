using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    public class CharacterModel : Mmorpg_Model
    {

        /// <summary>
        /// List of observers registered to observe this subject
        /// </summary>
        private List<Form1> registeredObservers = new List<Form1>();

        /// <summary>
        /// Holds current Character
        /// </summary>
        internal Character currentCharacter;


        /// <summary>
        /// Constructor for CharacterModel
        /// </summary>
        public CharacterModel()
        {
        }

        /// <summary>
        /// Performs any necessary initialisation for thread
        /// </summary>
        public void runThread()
        {
        }

        /// <summary>
        /// Changes current Character
        /// </summary>
        /// <param name="ch">Character to set as current</param>
        public void changeCurrent(Character ch)
        {
            this.currentCharacter = ch;
            this.notifyObservers("refreshChar");
        }

        /// <summary>
        /// Checks for death of current Character
        /// </summary>
        /// <param name="ch">Character to check for death</param>
        public void checkDeath()
        {
            bool dead = this.currentCharacter.checkDeath();
            if (dead)
            {
                this.currentCharacter.health = 0;
                this.notifyObservers("refreshChar");
            }
        }

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
