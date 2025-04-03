using System;
using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light fluorescentLight;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    void Start()
    {
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime, maxWaitTime));
            fluorescentLight.enabled = !fluorescentLight.enabled;
        }
    }
}
