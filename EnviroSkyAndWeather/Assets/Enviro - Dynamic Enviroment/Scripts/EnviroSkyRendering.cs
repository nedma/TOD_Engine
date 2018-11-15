using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class EnviroSkyRendering : MonoBehaviour
{
	[HideInInspector]
	public Material material;

	private Camera myCam;

	private RenderTexture spCloudtex;
	private RenderTexture spSkytex;
	private RenderTexture spBGtex;

	private Camera spCloudsCam;
	private Camera spSkyCam;
	private Camera spBGCam;

	private void Start() 
	{ 

	}

	void CreateSinglePassCameras ()
	{
		var format = EnviroSky.instance.PlayerCamera.hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;

		if (spCloudsCam == null) {
			spCloudtex = new RenderTexture (Screen.currentResolution.width / EnviroSky.instance.cloudsSettings.cloudsRenderResolution, Screen.currentResolution.height / EnviroSky.instance.cloudsSettings.cloudsRenderResolution, 16, format);
			GameObject n = new GameObject ();
			n.name = "Enviro Clouds SinglePass Camera";
			n.hideFlags = HideFlags.HideAndDontSave;
			spCloudsCam = n.AddComponent<Camera> ();
			spCloudsCam.hdr = EnviroSky.instance.HDR;
			spCloudsCam.renderingPath = RenderingPath.Forward;
			spCloudsCam.enabled = false;
			spCloudsCam.cullingMask = (1 << EnviroSky.instance.skyRenderingLayer);
			spCloudsCam.targetTexture = spCloudtex;
			spCloudsCam.useOcclusionCulling = false;
		}
		if (spSkyCam == null) {
			spSkytex = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height, 16, format);
			GameObject s = new GameObject ();
			s.name = "Enviro Sky SinglePass Camera";
			s.hideFlags = HideFlags.HideAndDontSave;
			spSkyCam = s.AddComponent<Camera> ();
			spSkyCam.hdr = EnviroSky.instance.HDR;
			spSkyCam.renderingPath = RenderingPath.Forward;
			spSkyCam.enabled = false;
			spSkyCam.cullingMask = (1 << EnviroSky.instance.satelliteRenderingLayer);
			spSkyCam.targetTexture = spSkytex;
			spSkyCam.useOcclusionCulling = false;
		}
		if (EnviroSky.instance.backgroundSettings.backgroundRendering) {
			if (spBGCam == null) {
				spSkytex = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height, 16, format);
				GameObject b = new GameObject ();
				b.name = "Enviro BG SinglePass Camera";
				b.hideFlags = HideFlags.HideAndDontSave;
				spBGCam = b.AddComponent<Camera> ();
				spBGCam.hdr = EnviroSky.instance.HDR;
				spBGCam.renderingPath = RenderingPath.Forward;
				spBGCam.enabled = false;
				spBGCam.cullingMask = (1 << EnviroSky.instance.backgroundSettings.backgroundLayer);
				spBGCam.targetTexture = spBGtex;
				spBGCam.useOcclusionCulling = false;
			}
		}
	}

	public void Apply()
	{
		myCam = GetComponent<Camera> ();

		if (EnviroSky.instance.singlePassVR == true && UnityEngine.VR.VRSettings.enabled) {
			CreateSinglePassCameras ();
		}

		CommandBuffer cb = new CommandBuffer();
		cb.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget,material);
		cb.name = "Enviro Sky Rendering";

		if (myCam.actualRenderingPath == RenderingPath.DeferredShading) {
			CommandBuffer[] cbs;
			cbs = myCam.GetCommandBuffers (CameraEvent.BeforeGBuffer);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					myCam.RemoveCommandBuffer (CameraEvent.BeforeGBuffer, cbs [i]);
			}

			myCam.AddCommandBuffer (CameraEvent.BeforeGBuffer, cb);

		} else {

			CommandBuffer[] cbs;
			cbs = myCam.GetCommandBuffers (CameraEvent.BeforeForwardOpaque);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					myCam.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cbs [i]);
			}

			myCam.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		}
	}

	void OnPreRender ()
	{
		if (myCam != null) {
			switch (myCam.stereoActiveEye) {
			case Camera.MonoOrStereoscopicEye.Mono:
				if (EnviroSky.instance.cloudsCamera != null)
					RenderCamera (EnviroSky.instance.cloudsCamera, Camera.MonoOrStereoscopicEye.Mono);
				if (EnviroSky.instance.skyCamera != null)
					RenderCamera (EnviroSky.instance.skyCamera, Camera.MonoOrStereoscopicEye.Mono);
				if (EnviroSky.instance.bgCamera != null)
					RenderCamera (EnviroSky.instance.bgCamera, Camera.MonoOrStereoscopicEye.Mono);
				break;

			case Camera.MonoOrStereoscopicEye.Left:
				if (EnviroSky.instance.cloudsCamera != null)
					RenderCamera (EnviroSky.instance.cloudsCamera, Camera.MonoOrStereoscopicEye.Left);
				if (EnviroSky.instance.skyCamera != null)
					RenderCamera (EnviroSky.instance.skyCamera, Camera.MonoOrStereoscopicEye.Left);
				if (EnviroSky.instance.bgCamera != null)
					RenderCamera (EnviroSky.instance.bgCamera, Camera.MonoOrStereoscopicEye.Left);

				break;

			case Camera.MonoOrStereoscopicEye.Right:
				if (EnviroSky.instance.cloudsCamera != null)
					RenderCamera (EnviroSky.instance.cloudsCamera, Camera.MonoOrStereoscopicEye.Right);
				if (EnviroSky.instance.skyCamera != null)
					RenderCamera (EnviroSky.instance.skyCamera, Camera.MonoOrStereoscopicEye.Right);
				if (EnviroSky.instance.bgCamera != null)
					RenderCamera (EnviroSky.instance.bgCamera, Camera.MonoOrStereoscopicEye.Right);
				break;
			}

			if (EnviroSky.instance.cloudsCamera != null)
				material.SetTexture ("_Clouds", EnviroSky.instance.cloudsCamera.targetTexture);
			if (EnviroSky.instance.skyCamera != null)
				material.SetTexture ("_Sky", EnviroSky.instance.skyCamera.targetTexture);
			if (EnviroSky.instance.bgCamera != null)
				material.SetTexture("_Background",EnviroSky.instance.bgCamera.targetTexture);
		}
	} 

	void RenderCamera(Camera targetCam, Camera.MonoOrStereoscopicEye eye)
	{
		targetCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;	
		targetCam.aspect = EnviroSky.instance.PlayerCamera.aspect;

		switch (eye) 
		{
		case Camera.MonoOrStereoscopicEye.Mono:
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.worldToCameraMatrix;
			targetCam.Render ();
			break;

		case Camera.MonoOrStereoscopicEye.Left:
	
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Left);
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Left);
			targetCam.Render ();

			if (EnviroSky.instance.singlePassVR == true) 
			{
				if (targetCam == EnviroSky.instance.cloudsCamera && spCloudsCam != null) {

						spCloudsCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;	
						spCloudsCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
						spCloudsCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Right);
						spCloudsCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Right);
						spCloudsCam.Render ();
						material.SetTexture ("_CloudsSPSR", spCloudtex);
				} else if (targetCam == EnviroSky.instance.skyCamera && spSkyCam != null) {
					spSkyCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;	
					spSkyCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
					spSkyCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Right);
					spSkyCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Right);
					spSkyCam.Render ();
					material.SetTexture ("_SkySPSR", spSkytex);
				} else if (targetCam == EnviroSky.instance.bgCamera && spBGCam != null) {
					spBGCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;	
					spBGCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
					spBGCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Right);
					spBGCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Right);
					spBGCam.Render ();
					material.SetTexture ("_BackgroundSPSR", spBGtex);
				}
			}
			break;

		case Camera.MonoOrStereoscopicEye.Right:
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Right);
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Right);
			targetCam.Render ();
			break;
		}
	}
}