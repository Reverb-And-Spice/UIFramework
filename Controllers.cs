using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// </summary>
	internal class UIFController
	{
        internal UIFModel MainModel = new();

        internal void DisplayMods()
        {
            foreach(kvp modToCat in MainModel.ModModelsDict)
            {
                
            }
        }
        internal class Mod 
        {
            internal MelonMod Instance;
            internal GameObject TabButton;

            public Mod(MelonMod instance, GameObject tabButton)
            {
                Instance = instance;
                TabButton = tabButton;
            }
        }
        internal class Category
        {

        }
        internal class PreferenceEntry
        {
            
        }
    }
}