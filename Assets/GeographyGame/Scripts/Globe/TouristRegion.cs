using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TouristRegion : MonoBehaviour
    {
        public string regionName;
        public List<int> provinces = new List<int>();
        public List<string> landmarks = new List<string>();
        public List<int> countries = new List<int>();
        public List<TouristRegion> neighbouringRegions = new List<TouristRegion>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
