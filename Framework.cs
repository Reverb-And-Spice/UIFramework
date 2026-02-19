using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;

namespace UIFramework
{
	public class UIFramework
    {

        static List<ModEntry> RegisteredMods = new();

        public static void Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
        {
            
        }
    }

    public class ModEntry
    {
        public MelonMod Mod {get; set;}
        public List<MelonPreferences_Category> Categories {get; set;}


        public ModEntry(MelonMod modInstance, List<MelonPreferences_Category> categories)
        {
            Mod = modInstance;
            Categories = categories;

        }


        public class PrefCategory
        {

            public class PrefEntry
            {

            }
        }
        
    }
}