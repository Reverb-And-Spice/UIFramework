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
	internal class Model
	{
        List<Mod> ModModelsList = new ();
        internal class Mod : MonoBehaviour
        {
            internal MelonMod Instance {get; set;}
            internal List<MelonPreferences_Category> Categories = new();
        }
        internal class PreferenceEntry
        {
            
        }
    }
}