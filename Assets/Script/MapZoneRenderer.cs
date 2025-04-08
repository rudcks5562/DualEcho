using UnityEngine;
using System.Collections.Generic;
using LibTessDotNet;
using System.Diagnostics;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapZoneRenderer : MonoBehaviour
{
    public MapZoneData zoneData;
    public Material zoneMaterial;

    void Start()
    {
        if (zoneData == null || zoneData.zones.Count == 0)
        {
           // Debug.LogWarning("MapZoneData가 비어있습니다.");
            return;
        }

        int zoneIndex = 0;

        foreach (var zone in zoneData.zones)
        {
            if (zone.controlPoints.Count < 3) continue;

            // 곡선 보간된 점 목록
            List<Vector3> curvedPoints = GenerateSmoothCurve(zone.controlPoints, 10);

            GameObject zoneObj = new GameObject(zone.zoneName);
            zoneObj.transform.SetParent(transform);

            var mf = zoneObj.AddComponent<MeshFilter>();
            var mr = zoneObj.AddComponent<MeshRenderer>();

            Mesh mesh = CreateZoneMesh(curvedPoints, zoneIndex);
            mf.mesh = mesh;

            // ✅ 반투명한 URP Lit Material
            Material mat = new Material(zoneMaterial); 
            Color color = zone.zoneColor;
            color.a = 0.3f;
            mat.color = color;

            mr.material = mat; // 꼭 렌더러에 할당해줘야 함


            zoneIndex++;
        }

       // Debug.Log("✅ 시작 설정 완료: " + zoneData.zones.Count + "개의 존 생성됨");
    }

    List<Vector3> GenerateSmoothCurve(List<Vector3> points, int subdivisions)
    {
        List<Vector3> result = new();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p0 = points[(i - 1 + points.Count) % points.Count];
            Vector3 p1 = points[i];
            Vector3 p2 = points[(i + 1) % points.Count];
            Vector3 p3 = points[(i + 2) % points.Count];

            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                Vector3 interpolated = CatmullRom(p0, p1, p2, p3, t);
                result.Add(interpolated);
            }
        }

        return result;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2 * p1 +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
        );
    }

    Mesh CreateZoneMesh(List<Vector3> points, int zoneIndex)
    {
        Tess tess = new Tess();

        ContourVertex[] contour = new ContourVertex[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            contour[i].Position = new Vec3(p.x, p.y, 0); // XY 평면 기준
        }

        tess.AddContour(contour, ContourOrientation.CounterClockwise); // 시계 ↔ 반시계로 변경

        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

        if (tess.Vertices.Length == 0 || tess.Elements.Length == 0)
        {
          //  Debug.LogWarning("⚠️ Tessellation 실패");
            return new Mesh();
        }

        Vector3[] vertices = new Vector3[tess.Vertices.Length];
        for (int i = 0; i < tess.Vertices.Length; i++)
        {
            var v = tess.Vertices[i].Position;
            vertices[i] = new Vector3((float)v.X, (float)v.Y, -0.01f * zoneIndex); // Z를 약간 낮게
        }

        int[] indices = tess.Elements;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
