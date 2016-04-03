using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Gamestrap{

    [ExecuteInEditMode]
    public class CameraViewsData : MonoBehaviour {

        public static CameraViewsData Instance;

        public int slotCount = 5;
        public int columnCount = 5;
        public List<CameraState> slots;

        public void UpdateSlotCount()
        {
            while (slots.Count < slotCount)
            {
                CameraState a = new CameraState();
                a.name = (slots.Count + 1) + "";
                slots.Add(a);
            }
        }

        /// <summary>
        /// Removes any excess in the list based on the slotCount
        /// Only does this when you close the editor window
        /// </summary>
        public void CleanSlotCount()
        {
            while (slots.Count > slotCount)
            {
                slots.RemoveAt(slots.Count - 1);
            }
        }
    }
}