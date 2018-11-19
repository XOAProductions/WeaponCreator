using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOAProductions.WeaponDesigner
{
    /// <summary>
    /// Base Class for every user-made weapon in the game
    /// </summary>
    public class Weapon : MonoBehaviour
    {

        private string Name;
        private string Id;

        private WeaponStructure Structure;
       
       /// <summary>
       /// initializes the weapon
       /// </summary>
       /// <param name="_name">the name of the weapon</param>
       /// <param name="_structure">the structure of the weapon</param>
        public Weapon(string _name, WeaponStructure _structure)
        {
            Name = _name;
            Structure = _structure;
            
            Id = System.Guid.NewGuid().ToString();
        }

      }
}
