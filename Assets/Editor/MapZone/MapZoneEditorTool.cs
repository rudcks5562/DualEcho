using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
public class MapZoneEditorTool : EditorWindow
{
    private static MapZoneData zoneData;
    private static int selectedZoneIndex = 0;
    private static bool isDragging = false;
    private static int draggingPointIndex = -1;

    private static bool isPlacingSharedPoint = false;
    private static int selectedSharedPointIndex = -1;

    [MenuItem("Tools/Map Zone Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapZoneEditorTool>("Map Zone Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        zoneData = (MapZoneData)EditorGUILayout.ObjectField("Zone Data", zoneData, typeof(MapZoneData), false);

        if (zoneData == null)
        {
            EditorGUILayout.HelpBox("Zone 데이터를 선택해주세요.", MessageType.Info);
            return;
        }

        if (GUILayout.Button("+ 새 Zone 추가"))
        {
            MapZone newZone = new MapZone
            {
                zoneName = $"Zone {zoneData.zones.Count + 1}",
                zoneColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f, 0.3f, 0.6f),
                sharedPointIndices = new List<int>(),
                localPoints = new List<Vector3>(),
                allPoints = new List<MapZonePointRef>()
            };
            zoneData.zones.Add(newZone);
            selectedZoneIndex = zoneData.zones.Count - 1;
        }

        if (zoneData.zones.Count == 0)
        {
            EditorGUILayout.HelpBox("존이 없습니다. 새 존을 추가하세요.", MessageType.Info);
            return;
        }

        string[] zoneNames = zoneData.zones.ConvertAll(z => z.zoneName).ToArray();
        selectedZoneIndex = EditorGUILayout.Popup("Zone 선택", selectedZoneIndex, zoneNames);

        MapZone currentZone = zoneData.zones[selectedZoneIndex];

        currentZone.zoneName = EditorGUILayout.TextField("Zone 이름", currentZone.zoneName);
        currentZone.zoneColor = EditorGUILayout.ColorField("Zone 색상", currentZone.zoneColor);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("- 이 Zone 삭제"))
        {
            zoneData.zones.RemoveAt(selectedZoneIndex);
            selectedZoneIndex = Mathf.Clamp(selectedZoneIndex - 1, 0, zoneData.zones.Count - 1);
            return;
        }

        if (GUILayout.Button("저장"))
        {
            EditorUtility.SetDirty(zoneData);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        isPlacingSharedPoint = GUILayout.Toggle(isPlacingSharedPoint, "공용 포인트 추가 모드 (Space 키 전환 가능)");
    }
    private void DrawZone(int zoneIdx, Event e, bool isSelected = false)
    {
        MapZone zone = zoneData.zones[zoneIdx];
        Handles.color = zone.zoneColor;
        List<Vector3> resolvedPoints = ResolveAllPoints(zone);
        if (resolvedPoints.Count >= 2)
        {
            DrawCatmullRomOpen(resolvedPoints.ToArray(), 10f, zone.zoneColor);
        }

        for (int i = 0; i < zone.allPoints.Count; i++)
        {
            Vector3 point = GetPoint(zone, zone.allPoints[i]);
            float size = HandleUtility.GetHandleSize(point) * 0.1f;

            if (Handles.Button(point, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                if (isPlacingSharedPoint)
                {
                    // 기존 공유 포인트 추가 로직 유지
                    if (!isSelected)
                    {
                        var clickedRef = zone.allPoints[i];
                        if (clickedRef.isShared)
                        {
                            if (!zoneData.zones[selectedZoneIndex].sharedPointIndices.Contains(clickedRef.index))
                            {
                                zoneData.zones[selectedZoneIndex].sharedPointIndices.Add(clickedRef.index);
                                zoneData.zones[selectedZoneIndex].allPoints.Add(new MapZonePointRef
                                {
                                    isShared = true,
                                    index = clickedRef.index
                                });
                                EditorUtility.SetDirty(zoneData);
                                e.Use();
                                return;
                            }
                        }
                        else
                        {
                            ConvertLocalToShared(zone, i, zoneData.zones[selectedZoneIndex]);
                            e.Use();
                            return;
                        }
                    }
                }
                else if (isSelected)
                {
                    if (e.shift)
                    {
                        Undo.RecordObject(zoneData, "Remove Point");

                        if (e.control)
                        {
                            // Shift+Ctrl+클릭: 모든 영역에서 삭제
                            RemovePointFromAllZones(zone, i);
                        }
                        else
                        {
                            // Shift+클릭: 현재 영역에서만 삭제
                            RemovePointFromCurrentZone(zone, i);
                        }

                        e.Use();
                        return;
                    }
                    else
                    {
                        draggingPointIndex = i;
                        isDragging = true;
                    }
                }
            }

            if (isSelected && isDragging && draggingPointIndex == i)
            {
                Undo.RecordObject(zoneData, "Move Point");
                Vector3 newPos = Handles.PositionHandle(point, Quaternion.identity);
                SetPoint(zone, zone.allPoints[i], newPos);
                SceneView.RepaintAll();
            }
        }
    }

    // 현재 영역에서만 포인트 삭제
    private void RemovePointFromCurrentZone(MapZone zone, int pointIndex)
    {
        var pointRef = zone.allPoints[pointIndex];

        if (!pointRef.isShared)
        {
            // 로컬 포인트 삭제 로직 (기존과 같음)
            zone.localPoints.RemoveAt(pointRef.index);
            for (int j = 0; j < zone.allPoints.Count; j++)
            {
                if (!zone.allPoints[j].isShared && zone.allPoints[j].index > pointRef.index)
                    zone.allPoints[j].index--;
            }
            zone.allPoints.RemoveAt(pointIndex);
        }
        else
        {
            // 공유 포인트는 현재 존에서만 참조 제거
            int sharedIndex = pointRef.index;
            zone.sharedPointIndices.Remove(sharedIndex);
            zone.allPoints.RemoveAt(pointIndex);

            // 이 공유점이 더 이상 어떤 존에서도 사용되지 않는지 확인
            bool isUsedElsewhere = false;
            foreach (var z in zoneData.zones)
            {
                if (z.sharedPointIndices.Contains(sharedIndex))
                {
                    isUsedElsewhere = true;
                    break;
                }
            }

            // 사용되지 않는 공유점이면 완전히 제거
            if (!isUsedElsewhere)
            {
                RemoveSharedPointCompletely(sharedIndex);
            }
        }

        EditorUtility.SetDirty(zoneData);
    }

    // 모든 영역에서 공유 포인트 삭제
    private void RemovePointFromAllZones(MapZone zone, int pointIndex)
    {
        var pointRef = zone.allPoints[pointIndex];

        if (!pointRef.isShared)
        {
            // 공유점이 아닌 경우 일반 삭제 수행
            RemovePointFromCurrentZone(zone, pointIndex);
            return;
        }

        // 공유점인 경우 모든 영역에서 제거
        int sharedIndex = pointRef.index;
        RemoveSharedPointCompletely(sharedIndex);

        EditorUtility.SetDirty(zoneData);
    }

    // 공유점 완전히 제거 및 인덱스 재정렬
    private void RemoveSharedPointCompletely(int sharedIndex)
    {
        // 공유점 제거
        zoneData.sharedPoints.RemoveAt(sharedIndex);

        // 모든 존에서 해당 공유점 참조 제거 및 인덱스 조정
        foreach (var zone in zoneData.zones)
        {
            // 이 존의 공유점 인덱스 목록에서 제거
            zone.sharedPointIndices.Remove(sharedIndex);

            // 인덱스 재조정
            for (int i = 0; i < zone.sharedPointIndices.Count; i++)
            {
                if (zone.sharedPointIndices[i] > sharedIndex)
                    zone.sharedPointIndices[i]--;
            }

            // 존의 allPoints 목록에서 해당 공유점 참조 제거 및 인덱스 조정
            for (int i = zone.allPoints.Count - 1; i >= 0; i--)
            {
                var pt = zone.allPoints[i];
                if (pt.isShared)
                {
                    if (pt.index == sharedIndex)
                    {
                        zone.allPoints.RemoveAt(i);
                    }
                    else if (pt.index > sharedIndex)
                    {
                        pt.index--;
                        zone.allPoints[i] = pt;
                    }
                }
            }
        }
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        if (zoneData == null || zoneData.zones.Count == 0) return;

        Event e = Event.current;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        MapZone currentZone = zoneData.zones[selectedZoneIndex];

        for (int zoneIdx = 0; zoneIdx < zoneData.zones.Count; zoneIdx++)
        {
            if (zoneIdx == selectedZoneIndex) continue;
            DrawZone(zoneIdx, e);
        }
        if (selectedZoneIndex >= 0 && selectedZoneIndex < zoneData.zones.Count)
        {
            DrawZone(selectedZoneIndex, e, true);
        }
        if (e.type == EventType.MouseUp)
        {
            isDragging = false;
            draggingPointIndex = -1;
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
        {
            isPlacingSharedPoint = !isPlacingSharedPoint;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.control && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 point = ray.origin + ray.direction * 10;
            point.z = 0;
            Undo.RecordObject(zoneData, "Add Point");

            if (isPlacingSharedPoint)
            {
                zoneData.sharedPoints.Add(point);
                int idx = zoneData.sharedPoints.Count - 1;
                currentZone.sharedPointIndices.Add(idx);
                currentZone.allPoints.Add(new MapZonePointRef { isShared = true, index = idx });
            }
            else
            {
                currentZone.localPoints.Add(point);
                int idx = currentZone.localPoints.Count - 1;
                currentZone.allPoints.Add(new MapZonePointRef { isShared = false, index = idx });
            }
            EditorUtility.SetDirty(zoneData);
            e.Use();
        }
    }
    private Vector3 GetPoint(MapZone zone, MapZonePointRef pointRef)
    {
        return pointRef.isShared ? zoneData.sharedPoints[pointRef.index] : zone.localPoints[pointRef.index];
    }

    private void SetPoint(MapZone zone, MapZonePointRef pointRef, Vector3 newPos)
    {
        if (pointRef.isShared)
        {
            zoneData.sharedPoints[pointRef.index] = newPos;
        }
        else
        {
            zone.localPoints[pointRef.index] = newPos;
        }
    }

    private void RemovePoint(MapZone zone, int i)
    {
        var pointRef = zone.allPoints[i];

        if (pointRef.isShared)
        {
            // 공유점 제거 로직
            int sharedIndex = pointRef.index;// 
            zone.sharedPointIndices.Remove(sharedIndex);
            zone.allPoints.RemoveAt(i);

            // 공유점을 참조하는 모든 존 검사
            int usageCount = -1;//반드시 하나 이상의 존과 연관되어 있을것..
            foreach (var z in zoneData.zones)
            {
                if (z.sharedPointIndices.Contains(sharedIndex))
                    usageCount++;
            }

            if (usageCount == 0)
            {
                // 해당 공유점 제거 및 인덱스 재정렬
                zoneData.sharedPoints.RemoveAt(sharedIndex);// 공유점보관소: sharedPoints

                foreach (var z in zoneData.zones)
                {
                    // sharedPointIndices 업데이트
                    for (int j = 0; j < z.sharedPointIndices.Count; j++)
                    {
                        if (z.sharedPointIndices[j] > sharedIndex)
                            z.sharedPointIndices[j]--;
                    }

                    // allPoints 인덱스 업데이트
                    for (int j = 0; j < z.allPoints.Count; j++)
                    {
                        var refPt = z.allPoints[j];
                        if (refPt.isShared && refPt.index > sharedIndex)
                            refPt.index--;
                    }
                }
            }
        }
        else
        {
            zone.localPoints.RemoveAt(pointRef.index);
            for (int j = 0; j < zone.allPoints.Count; j++)
            {
                if (!zone.allPoints[j].isShared && zone.allPoints[j].index > pointRef.index)
                    zone.allPoints[j].index--;
            }
            zone.allPoints.RemoveAt(i);
        }
    }
    private void ConvertLocalToShared(MapZone fromZone, int fromIndex, MapZone toZone)
    {
        Vector3 localPoint = fromZone.localPoints[fromZone.allPoints[fromIndex].index];
        zoneData.sharedPoints.Add(localPoint);
        int newSharedIndex = zoneData.sharedPoints.Count - 1;

        // fromZone 업데이트
        int oldLocalIdx = fromZone.allPoints[fromIndex].index;
        fromZone.localPoints.RemoveAt(oldLocalIdx);
        for (int j = 0; j < fromZone.allPoints.Count; j++)
        {
            if (!fromZone.allPoints[j].isShared && fromZone.allPoints[j].index > oldLocalIdx)
                fromZone.allPoints[j].index--;
        }
        fromZone.sharedPointIndices.Add(newSharedIndex);
        fromZone.allPoints[fromIndex] = new MapZonePointRef { isShared = true, index = newSharedIndex };

        // toZone 업데이트
        toZone.sharedPointIndices.Add(newSharedIndex);
        toZone.allPoints.Add(new MapZonePointRef { isShared = true, index = newSharedIndex });

        EditorUtility.SetDirty(zoneData);
    }
    private List<Vector3> ResolveAllPoints(MapZone zone)
    {
        List<Vector3> result = new();
        foreach (var p in zone.allPoints)
        {
            result.Add(GetPoint(zone, p));
        }
        return result;
    }

    private void DrawCatmullRomOpen(Vector3[] points, float thickness, Color color)
    {
        if (points.Length < 2) return;

        List<Vector3> curvePoints = new();
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = (i + 2 < points.Length) ? points[i + 2] : points[i + 1];

            for (int j = 0; j < 10; j++)
            {
                float t = j / 10f;
                Vector3 point = 0.5f * (
                    2f * p1 +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
                );
                curvePoints.Add(point);
            }
        }

        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            Handles.DrawAAPolyLine(thickness, curvePoints[i], curvePoints[i + 1]);
        }
    }
}
// 공유점 추가 삭제 및 연결에 대한 작업 필요.