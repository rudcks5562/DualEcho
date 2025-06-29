using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRaycast : MonoBehaviour
{
    //public ZoneContentManager contentManager;
    //public ZoneUIManager uiManager;

    public Camera mainCamera;            // 월드 카메라
    public LayerMask mapZoneLayer;       // MapZone만 포함된 레이어 마스크

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // EventSystem 상에서 UI가 클릭된 경우 무시
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // UI는 무시할지 말지는 선택. 아래 코드에서 실제 레이캐스트 처리.
                UnityEngine.Debug.Log("UI clicked, but attempting world click anyway.");
                LogUIRaycastHits();
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // MapZone 레이어에 대해서만 레이캐스트
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mapZoneLayer))
            {
                GameObject hitObject = hit.collider.gameObject;
                UnityEngine.Debug.Log("Clicked: " + hitObject.name);

                // 이 이름을 기반으로 ScriptableObject 정보 출력 가능
            }
            else
            {
                UnityEngine.Debug.Log("No hit.");
            }
        }
    }
    private void LogUIRaycastHits()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        UnityEngine.Debug.Log($"UI Raycast hit count: {results.Count}");
        foreach (var r in results)
        {
            UnityEngine.Debug.Log($"UI Hit: {r.gameObject.name}");
        }
    }


}