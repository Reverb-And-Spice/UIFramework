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
    /// 
    /// </summary>
	internal class UIFModel
	{
        internal List<Mod> ModModelsList = new ();
        internal Dictionary<MelonMod; Mod> ModModelsDict = new();

        internal bool AddToDict(MelonMod modInstance, Mod model)
        {

        }
        internal class Mod 
        {
            internal MelonMod Instance {get; set;}
            internal List<MelonPreferences_Category> new();
            


            
        }

        internal class Categories 
        {
            internal MelonPreferences_Category category;
            

        }
        internal class PreferenceEntry
        {
            
        }
    }
}