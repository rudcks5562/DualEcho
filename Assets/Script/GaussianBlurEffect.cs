using UnityEngine;

public class GaussianBlurEffect : MonoBehaviour
{
    public Material blurMaterial;
    public RenderTexture sourceTexture; // ���� ī�޶��� ���� �ؽ�ó

    [Range(0, 10)]
    public float blurSize = 3.0f;

    // �� ��ũ��Ʈ�� ���� ǥ���� ī�޶� �߰�
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // �� ���� ����
        blurMaterial.SetFloat("_BlurSize", blurSize);

        // �ӽ� ���� �ؽ�ó
        RenderTexture temp = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

        // ���� �� �н�
        Graphics.Blit(sourceTexture, temp, blurMaterial, 0);

        // ���� �� �н�
        Graphics.Blit(temp, destination, blurMaterial, 1);

        // �ӽ� �ؽ�ó ����
        RenderTexture.ReleaseTemporary(temp);
    }
}
