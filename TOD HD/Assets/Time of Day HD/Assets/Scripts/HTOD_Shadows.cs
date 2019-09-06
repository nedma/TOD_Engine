using UnityEngine;

/// Cloud shadow camera component.

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera Cloud Shadows")]
public class HTOD_Shadows : HTOD_ImageEffect
{
	public Shader ShadowShader = null;

	public Texture2D CloudTexture = null;

	[Header("Shadows")]

	[Range(0f, 1f)] public float Cutoff    = 0.0f;
	[Range(0f, 1f)] public float Fade      = 0.0f;
	[Range(0f, 1f)] public float Intensity = 0.5f;

	private Material shadowMaterial = null;

	protected void OnEnable()
	{
		if (!ShadowShader) ShadowShader = Shader.Find("Hidden/Time of Day/Cloud Shadows");

		shadowMaterial = CreateMaterial(ShadowShader);
	}

	protected void OnDisable()
	{
		if (shadowMaterial) DestroyImmediate(shadowMaterial);
	}

	[ImageEffectOpaque]
	protected void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckSupport(true))
		{
			Graphics.Blit(source, destination);
			return;
		}

		sky.Components.Shadows = this;

		shadowMaterial.SetMatrix("_FrustumCornersWS", FrustumCorners());

		Shader.SetGlobalTexture("HTOD_CloudTexture", CloudTexture);
		Shader.SetGlobalFloat("HTOD_CloudShadowCutoff", Cutoff);
		Shader.SetGlobalFloat("HTOD_CloudShadowFade", Fade);
		Shader.SetGlobalFloat("HTOD_CloudShadowIntensity", Intensity * Mathf.Clamp01(1 - sky.SunZenith / 90f));

		Graphics.Blit(source, destination, shadowMaterial);
	}
}
