using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        int latlonIndex;
        float totalLength;
        float currentProgress = 0;
        private const float MOVE_SPEED = 0.009f;

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

            currentProgress = progress;  //This seems pointless

            Vector3 pos0 = Conversion.GetSpherePointFromLatLon(latLon[latlonIndex]);
            Vector3 pos1 = Conversion.GetSpherePointFromLatLon(latLon[latlonIndex + 1]);
            Vector3 pos = Vector3.Lerp(pos0, pos1, progress);
            pos = pos.normalized * 0.5f;
            map.AddMarker(gameObject, pos, playerCharacter.size, false);

            // Make it look towards destination
            Vector3 dir = (pos0 - pos1).normalized;
            Vector3 proj = Vector3.ProjectOnPlane(dir, pos0);
            transform.LookAt(map.transform.TransformPoint(proj + pos0), map.transform.transform.TransformDirection(pos0));
        }

        public void GenerateLatLon(List<int> pathIndices)
        {
            if (latLon != null)
            {
                latLon.Clear();
            }
            foreach (var hexIndex in pathIndices)
            {
                latLon.Add(map.cells[hexIndex].latlonCenter);
            }
        }

        public void Update()
        {
            if (auto)
            {
                MoveTo(currentProgress);
                currentProgress += MOVE_SPEED;
                if (currentProgress > 1f)
                {
                    latlonIndex++;
                    int newCell = map.GetCellIndex(latLon[latlonIndex]);
                    playerCharacter.UpdateLocation(newCell);
                    currentProgress = 0;
                    if(latlonIndex >= latLon.Count - 1)
                    {
                        latlonIndex = 0;
                        latLon.Clear();
                        auto = false;
                        playerCharacter.FinishedPathFinding();
                    }
                    
                }
            }

        }

    }
}