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
                // 알파 범위를 0.3~0.6 사이로 지정
                zoneColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f, 0.3f, 0.6f),
                controlPoints = new List<Vector3>()
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
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (zoneData == null) return;

        Event e = Event.current;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        for (int zoneIdx = 0; zoneIdx < zoneData.zones.Count; zoneIdx++)
        {
            MapZone zone = zoneData.zones[zoneIdx];
            Handles.color = zone.zoneColor;

            if (zone.controlPoints.Count >= 2)
            {
                DrawCatmullRomOpen(zone.controlPoints.ToArray(), 10f, zone.zoneColor);

            }

            for (int i = 0; i < zone.controlPoints.Count; i++)
            {
                Vector3 point = zone.controlPoints[i];
                float size = HandleUtility.GetHandleSize(point) * 0.1f;

                if (Handles.Button(point, Quaternion.identity, size, size, Handles.SphereHandleCap))
                {
                    if (zoneIdx == selectedZoneIndex)
                    {
                        if (e.shift)
                        {
                            Undo.RecordObject(zoneData, "Remove Point");
                            zone.controlPoints.RemoveAt(i);
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
                    zone.controlPoints[i] = Handles.PositionHandle(zone.controlPoints[i], Quaternion.identity);
                    SceneView.RepaintAll();
                }
            }

            if (zoneIdx == selectedZoneIndex && e.type == EventType.MouseUp)
            {
                isDragging = false;
                draggingPointIndex = -1;
            }

            // Ctrl + 클릭으로 점 추가
            if (zoneIdx == selectedZoneIndex && e.type == EventType.MouseDown && e.control && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AddPointToZone(zone, hit.point);
                }
                else
                {
                    Vector3 point = ray.origin + ray.direction * 10;
                    point.z = 0;
                    AddPointToZone(zone, point);
                }
                e.Use();
            }
        }
    }

    private void AddPointToZone(MapZone zone, Vector3 point)
    {
        Undo.RecordObject(zoneData, "Add Point");
        zone.controlPoints.Add(point);
        EditorUtility.SetDirty(zoneData);
        SceneView.RepaintAll();
    }

    private void DrawCatmullRomOpen(Vector3[] points, float thickness, Color color)
    {
        int count = points.Length;
        if (count < 2) return;

        List<Vector3> curvePoints = new();
        for (int i = 0; i < count - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = (i + 2 < count) ? points[i + 2] : points[i + 1]; // 끝점 반복

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
