using UnityEngine;

using System.Collections.Generic;
using WPM.Poly2Tri;

namespace WPM {

    public partial class WorldMapGlobe : MonoBehaviour {

        const float REFINE_VERTICES_MIN_DISTANCE = 0.1f;

        List<Vector2> tempCoords = new List<Vector2>();
        List<int> tempIndices = new List<int>();
        List<Vector3> tempVertices = new List<Vector3>();
        List<Vector4> tempUVs = new List<Vector4>();

        Region tempRegion;

        void RefineCoordinates(Vector2[] latlon) {
            tempCoords.Clear();
            int count = latlon.Length + 1;
            Vector2 prev = latlon[latlon.Length - 1];
            for (int k = 0; k < count; k++) {
                Vector2 pos = k == count - 1 ? latlon[0] : latlon[k];
                float dist = Vector2.Distance(prev, pos);
                if (dist > REFINE_VERTICES_MIN_DISTANCE) {
                    int steps = (int)(dist / REFINE_VERTICES_MIN_DISTANCE) + 1;
                    for (int j=1;j<steps;j++) {
                        Vector2 nextPos = Vector2.Lerp(prev, pos, (float)j / steps);
                        tempCoords.Add(nextPos);
                    }
                }
                tempCoords.Add(pos);
                prev = pos;
            }
        }

        void TempCoordinatesToVertices() {
            tempVertices.Clear();
            tempIndices.Clear();
            int count = tempCoords.Count;

            for (int k=0;k<count;k++) {
                tempVertices.Add(Conversion.GetSpherePointFromLatLon(tempCoords[k]));
                tempIndices.Add(k);
            }
        }


        GameObject GeneratePolygonSurface(Region region, Material material) {

            // Triangulate to get the polygon vertex indices
            Poly2Tri.Polygon poly = new Poly2Tri.Polygon(region.latlon);

            const float step = 2f;
            if (steinerPoints == null) {
                steinerPoints = new List<TriangulationPoint>(1000);
            } else {
                steinerPoints.Clear();
            }
            float x0 = region.latlonRect2D.min.x + step / 2f;
            float x1 = region.latlonRect2D.max.x - step / 2f;
            float y0 = region.latlonRect2D.min.y + step / 2f;
            float y1 = region.latlonRect2D.max.y - step / 2f;
            for (float x = x0; x < x1; x += step) {
                for (float y = y0; y < y1; y += step) {
                    float xp = x + Random.Range(-0.0001f, 0.0001f);
                    float yp = y + Random.Range(-0.0001f, 0.0001f);
                    if (region.Contains(xp, yp)) {
                        steinerPoints.Add(new TriangulationPoint(xp, yp));
                    }
                }
            }
            if (steinerPoints.Count > 0) {
                poly.AddSteinerPoints(steinerPoints);
            }
            P2T.Triangulate(poly);

            int flip1, flip2;
            if (_earthInvertedMode) {
                flip1 = 2;
                flip2 = 1;
            } else {
                flip1 = 1;
                flip2 = 2;
            }
            int triCount = poly.Triangles.Count;
            Vector3[] revisedSurfPoints = new Vector3[triCount * 3];
            for (int k = 0; k < triCount; k++) {
                DelaunayTriangle dt = poly.Triangles[k];
                revisedSurfPoints[k * 3] = Conversion.GetSpherePointFromLatLon(dt.Points[0].X, dt.Points[0].Y);
                revisedSurfPoints[k * 3 + flip1] = Conversion.GetSpherePointFromLatLon(dt.Points[1].X, dt.Points[1].Y);
                revisedSurfPoints[k * 3 + flip2] = Conversion.GetSpherePointFromLatLon(dt.Points[2].X, dt.Points[2].Y);
            }

            int revIndex = revisedSurfPoints.Length - 1;

            // Generate surface mesh
            GameObject surf = Drawing.CreateSurface(SURFACE_GAMEOBJECT, revisedSurfPoints, revIndex, material);
            if (_earthInvertedMode) {
                surf.transform.localScale = Misc.Vector3one * 0.998f;
            }
            return surf;
        }

    }

}