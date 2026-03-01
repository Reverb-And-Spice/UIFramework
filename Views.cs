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
    /// Custom components that will serve as views in this MVP/MVC-ish pattern
    /// </summary>
	internal class View
	{
        [RegisterTypeInIl2Cpp]
        internal class Mod : MonoBehaviour
        {
            
        }
        [RegisterTypeInIl2Cpp]
        internal class Category : MonoBehaviour
        {

        }
        [RegisterTypeInIl2Cpp]
        internal class PreferenceEntry
        {
            
        }
    }
}