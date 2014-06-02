using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;


namespace hist_mmorpg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            // create CharacterModel
            CharacterModel cm = new CharacterModel();
            // start CharacterModel in thread
            Thread cmThread = new Thread(new ThreadStart(cm.runThread));
            cmThread.Start();

            // create FiefModel
            FiefModel fm = new FiefModel();
            // start FiefModel in thread
            Thread fmThread = new Thread(new ThreadStart(fm.runThread));
            fmThread.Start();

            // prepare main application thread to run
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // create and run observer (Form1 object)
            Application.Run(new Form1(cm, fm));

            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */ 
        }
    }
}
