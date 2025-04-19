using UnityEngine;

public class GaussianBlurEffect : MonoBehaviour
{
    public Material blurMaterial;
    public RenderTexture sourceTexture; // 블러용 카메라의 렌더 텍스처

    [Range(0, 10)]
    public float blurSize = 3.0f;

    // 이 스크립트를 블러를 표시할 카메라에 추가
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 블러 강도 설정
        blurMaterial.SetFloat("_BlurSize", blurSize);

        // 임시 렌더 텍스처
        RenderTexture temp = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

        // 수평 블러 패스
        Graphics.Blit(sourceTexture, temp, blurMaterial, 0);

        // 수직 블러 패스
        Graphics.Blit(temp, destination, blurMaterial, 1);

        // 임시 텍스처 해제
        RenderTexture.ReleaseTemporary(temp);
    }
}
