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

    private void OnSceneGUI(SceneView sceneView)
    {
        if (zoneData == null || zoneData.zones.Count == 0) return;

        Event e = Event.current;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        MapZone currentZone = zoneData.zones[selectedZoneIndex];

        for (int zoneIdx = 0; zoneIdx < zoneData.zones.Count; zoneIdx++)
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
                    if (isPlacingSharedPoint && zoneIdx != selectedZoneIndex)
                    {
                        MapZonePointRef refClicked = zone.allPoints[i];
                        if (refClicked.isShared)
                        {
                            if (!currentZone.sharedPointIndices.Contains(refClicked.index))
                            {
                                currentZone.sharedPointIndices.Add(refClicked.index);
                                currentZone.allPoints.Add(new MapZonePointRef { isShared = true, index = refClicked.index });
                                EditorUtility.SetDirty(zoneData);
                                e.Use();
                                return;
                            }
                        }
                    }
                    else if (zoneIdx == selectedZoneIndex)
                    {
                        if (e.shift)
                        {
                            Undo.RecordObject(zoneData, "Remove Point");
                            RemovePoint(zone, i);
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

                if (zoneIdx == selectedZoneIndex && isDragging && draggingPointIndex == i)
                {
                    Undo.RecordObject(zoneData, "Move Point");
                    Vector3 newPos = Handles.PositionHandle(point, Quaternion.identity);
                    SetPoint(zone, zone.allPoints[i], newPos);
                    SceneView.RepaintAll();
                }
            }
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
        if (!pointRef.isShared)
        {
            zone.localPoints.RemoveAt(pointRef.index);
            for (int j = 0; j < zone.allPoints.Count; j++)
            {
                if (!zone.allPoints[j].isShared && zone.allPoints[j].index > pointRef.index)
                    zone.allPoints[j].index--;
            }
        }
        zone.allPoints.RemoveAt(i);
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