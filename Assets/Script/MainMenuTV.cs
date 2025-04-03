using System;
using System.Collections;
using UnityEngine;

public class TVFlickerEffect : MonoBehaviour
{
    public Material tvMaterial; // TV 화면의 머티리얼
    public float minIntensity = 0.2f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            tvMaterial.SetColor("_EmissionColor", Color.white * intensity);
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
