using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace XOAProductions.Utility
{
    /// <summary>
    /// Contains some helper functions for non-monobehavior scripts to access.
    /// Please call init every time before using, the script destroys itself after each function to avoid
    /// clustering the hierachy with gameobjects.
    /// </summary>
    public  class HelperFunctions: MonoBehaviour
    {
        /// <summary>
        /// Please call init every time before using, the script destroys itself after each function to avoid
        /// clustering the hierachy with gameobjects.
        /// </summary>
        /// <returns>Instance of this</returns>
        public HelperFunctions init()
        {
            GameObject go = new GameObject();
            return go.AddComponent<HelperFunctions>();

        }

        /// <summary>
        /// Invoke an action
        /// </summary>
        /// <param name="theDelegate">the action to invoke</param>
        /// <param name="time">after how much time to invoke</param>
        public  void Invoke( Action theDelegate, float time)
        {
            this.StartCoroutine(ExecuteAfterTime(theDelegate, time));
        }


        private  IEnumerator ExecuteAfterTime(Action theDelegate, float delay)
        {
            yield return new WaitForSeconds(delay);
            theDelegate();
            Destroy(this.gameObject);
        }

    }
}
