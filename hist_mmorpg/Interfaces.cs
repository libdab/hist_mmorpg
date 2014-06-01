using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    public interface Mmorpg_View
    {
        // method to update form component
        void update(String info);
    }

    public interface Mmorpg_Model
    {
        // method to register observer
        void registerObserver(Form1 obs);
        // method to remove observer
        void removeObserver(Form1 obs);
        // method to notify observers of changes to subject
        void notifyObservers(String info);
    }
}
