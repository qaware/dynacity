using System.Collections.Generic;
using EasyCurvedLine;
using UnityEngine;
using Material = SoftwareCities.figures.Material;

namespace SoftwareCities.unityadapter
{
    public class DependencyDrawer : MonoBehaviour
    {
        public static GameObject DrawLine(Vector3 startPos, Vector3 endPos, List<Vector3> attractionPoints,
            Material material)
        {
            GameObject curvedLine = new GameObject("CurvedLine");
            curvedLine.transform.position = new Vector3(startPos.x + 1, startPos.y + startPos.y / 2 - 3, startPos.z);
            CurvedLineRenderer curvedLineRenderer = curvedLine.AddComponent<CurvedLineRenderer>();
            curvedLineRenderer.showGizmos = false;
            curvedLineRenderer.lineWidth = 0.3F;
            GameObject linePointStart = new GameObject("LinePointStart");
            linePointStart.AddComponent<CurvedLinePoint>();
            linePointStart.transform.SetParent(curvedLine.transform, false);
            foreach (Vector3 attractionPoint in attractionPoints)
            {
                GameObject linePointMiddle = new GameObject("LinePointMiddle");
                linePointMiddle.AddComponent<CurvedLinePoint>();
                linePointMiddle.transform.position = attractionPoint;
                linePointMiddle.transform.SetParent(curvedLine.transform, false);
            }

            GameObject linePointEnd = new GameObject("LinePointEnd");
            linePointEnd.AddComponent<CurvedLinePoint>();
            linePointEnd.transform.position =
                new Vector3(endPos.x - startPos.x, endPos.y - startPos.y, endPos.z - startPos.z);
            linePointEnd.transform.SetParent(curvedLine.transform, false);
            curvedLine.GetComponent<Renderer>().material =
                Resources.Load("Materials/" + material, typeof(UnityEngine.Material)) as
                    UnityEngine.Material;
            return curvedLine;
        }
    }
}