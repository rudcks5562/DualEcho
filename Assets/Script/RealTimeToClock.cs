using UnityEngine;

public class RealTimeToClock : MonoBehaviour
{
    public Transform hourHand; // ��ħ
    public Transform minuteHand; // ��ħ

    void Update()
    {

    }
    void Start()
    {
        // ���� �ð��� ���� �����ɴϴ�.
        System.DateTime currentTime = System.DateTime.Now;

        // 12�ð����� �������� ��ħ ȸ�� ��� (��ħ�� 360�� / 12�ð� = 30��)
        float hourRotation = (currentTime.Hour % 12 + currentTime.Minute / 60f) * 30f;

        // ��ħ ȸ�� ��� (��ħ�� 360�� / 60�� = 6��)
        float minuteRotation = currentTime.Minute * 6f;

        // ���� ȸ������ �����ͼ� Z�� ȸ���� �����մϴ�.
        if (hourHand != null)
        {
            Vector3 currentHourRotation = hourHand.rotation.eulerAngles;
            hourHand.rotation = Quaternion.Euler(currentHourRotation.x, currentHourRotation.y, currentHourRotation.z + hourRotation); // ���� x, y�� �����ϰ� Z���� ����
        }

        if (minuteHand != null)
        {
            Vector3 currentMinuteRotation = minuteHand.rotation.eulerAngles;
            minuteHand.rotation = Quaternion.Euler(currentMinuteRotation.x, currentMinuteRotation.y, currentMinuteRotation.z + minuteRotation); // ���� x, y�� �����ϰ� Z���� ����
        }
    }
}
