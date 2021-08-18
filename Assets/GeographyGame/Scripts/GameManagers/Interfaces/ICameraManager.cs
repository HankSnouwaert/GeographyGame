using UnityEngine;

namespace WPM
{
    /// <summary> 
    /// Contains functionality for controlling game camera
    /// </summary>
    public interface ICameraManager
    {
        GameObject CameraGameObject { get; }

        /// <summary>
        /// Orients the camera to a given vector location
        /// </summary>
        /// <param name="vectorLocation">The location the camera is being oriented.</param>
        void OrientOnLocation(Vector3 vectorLocation);

        void LockCamera();

        void UnlockCamera();
    }
}