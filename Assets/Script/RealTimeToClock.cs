using UnityEngine;

public class RealTimeToClock : MonoBehaviour
{
    public Transform hourHand; // 시침
    public Transform minuteHand; // 분침

    void Update()
    {

    }
    void Start()
    {
        // 현재 시간과 분을 가져옵니다.
        System.DateTime currentTime = System.DateTime.Now;

        // 12시간제를 기준으로 시침 회전 계산 (시침은 360도 / 12시간 = 30도)
        float hourRotation = (currentTime.Hour % 12 + currentTime.Minute / 60f) * 30f;

        // 분침 회전 계산 (분침은 360도 / 60분 = 6도)
        float minuteRotation = currentTime.Minute * 6f;

        // 기존 회전값을 가져와서 Z축 회전만 수정합니다.
        if (hourHand != null)
        {
            Vector3 currentHourRotation = hourHand.rotation.eulerAngles;
            hourHand.rotation = Quaternion.Euler(currentHourRotation.x, currentHourRotation.y, currentHourRotation.z + hourRotation); // 기존 x, y값 유지하고 Z값만 수정
        }

        if (minuteHand != null)
        {
            Vector3 currentMinuteRotation = minuteHand.rotation.eulerAngles;
            minuteHand.rotation = Quaternion.Euler(currentMinuteRotation.x, currentMinuteRotation.y, currentMinuteRotation.z + minuteRotation); // 기존 x, y값 유지하고 Z값만 수정
        }
    }
}
