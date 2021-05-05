using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary> 
    /// Represents a region of the world globe map and contains all the possible tourist destinations from that region
    /// </summary>
    public class TouristRegion : MonoBehaviour
    {
        public string regionName;
        public List<int> provinces = new List<int>();
        public List<string> landmarks = new List<string>();
        public List<int> countries = new List<int>();
        public List<TouristRegion> neighbouringRegions = new List<TouristRegion>();
    }
}
