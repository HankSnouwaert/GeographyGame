using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WPM
{
    public class GeoPosAnimator : MonoBehaviour
    {

        public bool auto;

        /// <summary>
        /// Array with latitude/longitude positions
        /// </summary>
        public List<Vector2> latLon;
        WorldMapGlobe map;
        PlayerCharacter playerCharacter;
        float[] stepLengths;
        float totalLength;
        float currentProgress = 0;

        void Awake()
        {
            map = WorldMapGlobe.instance;
            playerCharacter = gameObject.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
        }

        public void ComputePath()
        {
            // Compute path length
            int steps = latLon.Count;
            stepLengths = new float[steps];

            // Calculate total travel length
            totalLength = 0;
            for (int k = 0; k < steps - 1; k++)
            {
                stepLengths[k] = map.calc.Distance(latLon[k], latLon[k + 1]);
                totalLength += stepLengths[k];
            }

            Debug.Log("Total path length = " + totalLength / 1000 + " km.");
        }

        /// <summary>
        /// Moves the gameobject obj onto the globe at the path given by latlon array and progress factor.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="progress">Progress expressed in 0..1.</param>
        public void MoveTo(float progress)
        {
            currentProgress = progress;

            // Iterate again until we reach progress
            int steps = latLon.Count;
            float acum = 0, acumPrev = 0;
            for (int k = 0; k < steps - 1; k++)
            {
                acumPrev = acum;
                acum += stepLengths[k] / totalLength;
                if (acum > progress)
                {
                    // This is the step where "progress" is contained.
                    if (k > 0)
                    {
                        progress = (progress - acumPrev) / (acum - acumPrev);
                    }
                    Vector3 pos0 = Conversion.GetSpherePointFromLatLon(latLon[k]);
                    Vector3 pos1 = Conversion.GetSpherePointFromLatLon(latLon[k + 1]);
                    Vector3 pos = Vector3.Lerp(pos0, pos1, progress);
                    pos = pos.normalized * 0.5f;
                    map.AddMarker(gameObject, pos, 0.01f, false);

                    // Make it look towards destination
                    Vector3 dir = (pos1 - pos0).normalized;
                    Vector3 proj = Vector3.ProjectOnPlane(dir, pos0);
                    transform.LookAt(map.transform.TransformPoint(proj + pos0), map.transform.transform.TransformDirection(pos0));

                    // Follow object
                    map.FlyToLocation(pos, 0f);
                    break;
                }
            }
        }

        public void Update()
        {
            if (auto)
            {
                MoveTo(currentProgress);
                currentProgress += Time.deltaTime * 0.1f;
                if (currentProgress > 1f)
                {
                    currentProgress = 0;
                    auto = false;
                    playerCharacter.FinishedPathFinding();
                }
            }

        }

    }
}