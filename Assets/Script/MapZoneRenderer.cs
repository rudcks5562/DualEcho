using UnityEngine;
using System.Collections.Generic;
using LibTessDotNet;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapZoneRenderer : MonoBehaviour
{
    public MapZoneData zoneData;
    public Material zoneMaterial;
    public Material boundaryMaterial; // 테두리 강조용 머티리얼
    private HashSet<string> renderedSegments = new();
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
            if (zone.allPoints.Count < 3) continue;

            // 곡선 보간된 점 목록 (메쉬 생성용)
            List<Vector3> controlPoints = GetOrderedPoints(zone);
            List<Vector3> curvedPoints = GenerateSmoothCurve(controlPoints, 10);

            GameObject zoneObj = new GameObject(zone.zoneName);
            zoneObj.transform.SetParent(transform);

            var mf = zoneObj.AddComponent<MeshFilter>();
            var mr = zoneObj.AddComponent<MeshRenderer>();

            // 메쉬 생성
            Mesh mesh = CreateZoneMesh(curvedPoints, zoneIndex);
            mf.mesh = mesh;

            // ✅ 반투명한 URP Lit Material
            Material mat = new Material(zoneMaterial); 
            Color color = zone.zoneColor;
            color.a = 0.3f;
            mat.color = color;

            mr.material = mat; // 꼭 렌더러에 할당해줘야 함

            // 테두리 선 그리기
            //DrawBoundaryLine(curvedPoints, zoneIndex);
            DrawSmoothZoneBoundary(zone, zoneIndex);
            zoneIndex++;
        }

        // Debug.Log("zoneData.zones.Count + "개의 구역 생성됨");
    }
    string GetSegmentKey(Vector3 a, Vector3 b)
    {
        // 양 방향 모두 동일한 키가 되도록 정렬
        return a.GetHashCode() < b.GetHashCode()
            ? $"{a.x}_{a.y}_{b.x}_{b.y}"
            : $"{b.x}_{b.y}_{a.x}_{a.y}";
    }
    // zone의 allPoints 기반으로 실제 좌표들을 추출
    List<Vector3> GetOrderedPoints(MapZone zone)
    {
        List<Vector3> result = new();

        foreach (var pointRef in zone.allPoints)
        {
            if (pointRef.isShared)
            {
                if (pointRef.index >= 0 && pointRef.index < zoneData.sharedPoints.Count)
                    result.Add(zoneData.sharedPoints[pointRef.index]);
            }
            else
            {
                if (pointRef.index >= 0 && pointRef.index < zone.localPoints.Count)
                    result.Add(zone.localPoints[pointRef.index]);
            }
        }

        return result;
    }

    // 곡선 보간 함수
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

    // Catmull-Rom 보간
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2 * p1 +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
        );
    }

    // 메쉬 생성
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
            // Debug.LogWarning("⚠️ Tessellation 실패");
            return new Mesh();
        }
        
        Vector3[] vertices = new Vector3[tess.Vertices.Length];
        for (int i = 0; i < tess.Vertices.Length; i++)
        {
            var v = tess.Vertices[i].Position;
            vertices[i] = new Vector3((float)v.X, (float)v.Y, 0.01f * zoneIndex);
        }

        int[] indices = tess.Elements;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
    void RenderLine(List<Vector3> points, string name, int zoneIndex, bool loop = false)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = boundaryMaterial;
        lr.loop = loop;
        lr.positionCount = points.Count;

        float zOffset = 0.1f + 0.02f * zoneIndex;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            p.z += zOffset;
            lr.SetPosition(i, p);
        }

        lr.startWidth = 0.08f;
        lr.endWidth = 0.06f;
        lr.alignment = LineAlignment.TransformZ;
        obj.transform.forward = Camera.main.transform.forward;
    }
    // 테두리 선 그리기
    void DrawBoundaryLine(List<Vector3> points, int zoneIndex)
    {
        GameObject boundaryObj = new GameObject("Boundary_" + zoneIndex);
        boundaryObj.transform.SetParent(transform);


        LineRenderer lineRenderer = boundaryObj.AddComponent<LineRenderer>();
        lineRenderer.material = boundaryMaterial;
        lineRenderer.loop = true;

        lineRenderer.positionCount = points.Count;

        // 선 두께 설정
        lineRenderer.startWidth = 0.10f;
        lineRenderer.endWidth = 0.06f;

        lineRenderer.alignment = LineAlignment.TransformZ;
        boundaryObj.transform.forward = Camera.main.transform.forward;

        // 정점 설정 (Z값 올려주기)
        float zOffset = 0.1f + 0.02f * zoneIndex; // 충분히 큰 오프셋
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            p.z += zOffset;
            lineRenderer.SetPosition(i, p);
        }
    }
    Vector3 GetPointPosition(MapZonePointRef pointRef, MapZone zone)
    {
        return pointRef.isShared
            ? zoneData.sharedPoints[pointRef.index]
            : zone.localPoints[pointRef.index];
    }
    Vector3 GetSafePoint(List<MapZonePointRef> refs, int index, MapZone zone)
    {
        int count = refs.Count;
        int safeIndex = (index + count) % count;
        return GetPointPosition(refs[safeIndex], zone);
    }
    List<Vector3> ExtractSegmentFromCurve(List<Vector3> curve, int subdivisions)
    {
        if (curve.Count < subdivisions)
            return curve;

        return curve.GetRange(subdivisions, subdivisions); // p1~p2 구간
    }// 곡선 보간 부분 추출
    void DrawSmoothZoneBoundary(MapZone zone, int zoneIndex)
    {
        var refs = zone.allPoints;
        if (refs.Count < 2) return;

        for (int i = 0; i < refs.Count; i++)
        {
            var curr = refs[i];
            var next = refs[(i + 1) % refs.Count]; // 루프 처리

            Vector3 p1 = GetPointPosition(curr, zone);
            Vector3 p2 = GetPointPosition(next, zone);

            string key = GetSegmentKey(p1, p2);
            if (renderedSegments.Contains(key)) continue;

            renderedSegments.Add(key);

            // 곡선 보간 (2점만이라도 Catmull-Rom 적용 가능)
            //List<Vector3> curve = GenerateSmoothCurve(new List<Vector3> { p1, p2 }, 10);
            //RenderLine(curve, $"Curve_{zone.zoneName}_{i}", zoneIndex);

            List<Vector3> segmentPoints = new List<Vector3> {
                GetSafePoint(refs, i - 1, zone),
                p1,
                p2,
                GetSafePoint(refs, i + 2, zone) };

            if (curr.isShared && next.isShared)
            {
                // 공유 ↔ 공유: 직선 처리
                RenderLine(segmentPoints, $"Straight_{zone.zoneName}_{i}", zoneIndex);
            }
            else
            {
                // 로컬 포함: 곡선 보간
                List<Vector3> curve = GenerateSmoothCurve(segmentPoints, 10);

                List<Vector3> segment = ExtractSegmentFromCurve(curve, 10);
                RenderLine(segment, $"Curved_{zone.zoneName}_{i}", zoneIndex);
            }



        }
    }

}
