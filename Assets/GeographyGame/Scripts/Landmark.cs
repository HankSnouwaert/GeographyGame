using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class Landmark : MappableObject
    {
        public string landmarkName;
        public MountPoint mountPoint;
        public int cellIndex;
        public Cell cell;
    }

}