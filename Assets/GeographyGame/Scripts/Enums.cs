using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
        public enum AttributeType
        {
            political,
            terrain,
            climate
        }

        public enum MountPointType
        {
            start_point,
            cultural_landmark_point,
            natural_landmark_point
        }

        public enum ErrorState
        {
            no_error,
            close_window,
            restart_scene,
            close_application
        }
}
