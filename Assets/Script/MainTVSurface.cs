using System;
using System.Collections;
using UnityEngine;

public class TVStaticEffect : MonoBehaviour
{
    public Material tvMaterial;
    public Texture2D[] staticTextures; // 전파 노이즈 이미지들
    public float changeInterval = 0.05f;

    void Start()
    {
        StartCoroutine(StaticEffect());
    }

    IEnumerator StaticEffect()
    {
        while (true)
        {
            int index = UnityEngine.Random.Range(0, staticTextures.Length);
            tvMaterial.SetTexture("_MainTex", staticTextures[index]);
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
