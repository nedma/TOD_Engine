using System;
using UnityEngine;

	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Enviro/Effects/Fog Image Effect")]
	public class EnviroFog : EnviroEffects
	{
	[HideInInspector]public bool  distanceFog = true;
	[HideInInspector]public bool  useRadialDistance = false;
	[HideInInspector]public bool  heightFog = true;
	[HideInInspector]public float height = 1.0f;
		[Range(0.001f,10.0f)]
	[HideInInspector]public float heightDensity = 2.0f;

	[HideInInspector]public float startDistance = 0.0f;

	[HideInInspector]public Shader fogShader = null;
	[HideInInspector]public Material fogMaterial = null;

	    private Texture3D _noiseTexture;

	public override bool CheckResources ()
	{
		CheckSupport (true);

		fogMaterial = CheckShaderAndCreateMaterial (fogShader, fogMaterial);

		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

		public void Start ()
		{
			LoadNoise3dTexture ();
		}

		//[ImageEffectOpaque]
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
		if (CheckResources()==false || (!distanceFog && !heightFog))
		{
			Graphics.Blit (source, destination);
			return;
		}
			
			Camera cam = GetComponent<Camera>();
			Transform camtr = cam.transform;
			float camNear = cam.nearClipPlane;
			float camFar = cam.farClipPlane;
			float camFov = cam.fieldOfView;
			float camAspect = cam.aspect;

			Matrix4x4 frustumCorners = Matrix4x4.identity;

			float fovWHalf = camFov * 0.5f;

			Vector3 toRight = camtr.right * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * camAspect;
			Vector3 toTop = camtr.up * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);

			Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
			float camScale = topLeft.magnitude * camFar/camNear;

			topLeft.Normalize();
			topLeft *= camScale;

			Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
			topRight.Normalize();
			topRight *= camScale;

			Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
			bottomRight.Normalize();
			bottomRight *= camScale;

			Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
			bottomLeft.Normalize();
			bottomLeft *= camScale;

			frustumCorners.SetRow (0, topLeft);
			frustumCorners.SetRow (1, topRight);
			frustumCorners.SetRow (2, bottomRight);
			frustumCorners.SetRow (3, bottomLeft);

			var camPos= camtr.position;
			float FdotC = camPos.y-height;
			float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
			fogMaterial.SetMatrix ("_FrustumCornersWS", frustumCorners);
			fogMaterial.SetVector ("_CameraWS", camPos);
			fogMaterial.SetVector ("_HeightParams", new Vector4 (height, FdotC, paramK, heightDensity * 0.5f));
			fogMaterial.SetVector ("_DistanceParams", new Vector4 (-Mathf.Max(startDistance,0.0f), 0, 0, 0));
		    fogMaterial.SetTexture("_NoiseTexture", _noiseTexture);

			var sceneMode= RenderSettings.fogMode;
			var sceneDensity= RenderSettings.fogDensity;
			var sceneStart= RenderSettings.fogStartDistance;
			var sceneEnd= RenderSettings.fogEndDistance;
			Vector4 sceneParams;
			bool  linear = (sceneMode == FogMode.Linear);
			float diff = linear ? sceneEnd - sceneStart : 0.0f;
			float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
			sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
			sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
			sceneParams.z = linear ? -invDiff : 0.0f;
			sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
			fogMaterial.SetVector ("_SceneFogParams", sceneParams);
			fogMaterial.SetVector ("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));

			int pass = 0;
			if (distanceFog && heightFog)
				pass = 0; // distance + height
			else if (distanceFog)
				pass = 1; // distance only
			else
				pass = 2; // height only
			CustomGraphicsBlit (source, destination, fogMaterial, pass);
		}

		static void CustomGraphicsBlit (RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
		{
			RenderTexture.active = dest;

			fxMaterial.SetTexture ("_MainTex", source);

			GL.PushMatrix ();
			GL.LoadOrtho ();

			fxMaterial.SetPass (passNr);

			GL.Begin (GL.QUADS);

			GL.MultiTexCoord2 (0, 0.0f, 0.0f);
			GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL

			GL.MultiTexCoord2 (0, 1.0f, 0.0f);
			GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR

			GL.MultiTexCoord2 (0, 1.0f, 1.0f);
			GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR

			GL.MultiTexCoord2 (0, 0.0f, 1.0f);
			GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL

			GL.End ();
			GL.PopMatrix ();
		}


	void LoadNoise3dTexture()
	{
		TextAsset data = Resources.Load("NoiseVolume") as TextAsset;

		byte[] bytes = data.bytes;

		uint height = BitConverter.ToUInt32(data.bytes, 12);
		uint width = BitConverter.ToUInt32(data.bytes, 16);
		uint pitch = BitConverter.ToUInt32(data.bytes, 20);
		uint depth = BitConverter.ToUInt32(data.bytes, 24);
		uint formatFlags = BitConverter.ToUInt32(data.bytes, 20 * 4);
		//uint fourCC = BitConverter.ToUInt32(data.bytes, 21 * 4);
		uint bitdepth = BitConverter.ToUInt32(data.bytes, 22 * 4);
		if (bitdepth == 0)
			bitdepth = pitch / width * 8;

		_noiseTexture = new Texture3D((int)width, (int)height, (int)depth, TextureFormat.RGBA32, false);
		_noiseTexture.name = "3D Noise";

		Color[] c = new Color[width * height * depth];

		uint index = 128;
		if (data.bytes[21 * 4] == 'D' && data.bytes[21 * 4 + 1] == 'X' && data.bytes[21 * 4 + 2] == '1' && data.bytes[21 * 4 + 3] == '0' &&
			(formatFlags & 0x4) != 0)
		{
			uint format = BitConverter.ToUInt32(data.bytes, (int)index);
			if (format >= 60 && format <= 65)
				bitdepth = 8;
			else if (format >= 48 && format <= 52)
				bitdepth = 16;
			else if (format >= 27 && format <= 32)
				bitdepth = 32;
			
			index += 20;
		}

		uint byteDepth = bitdepth / 8;
		pitch = (width * bitdepth + 7) / 8;

		for (int d = 0; d < depth; ++d)
		{
			for (int h = 0; h < height; ++h)
			{
				for (int w = 0; w < width; ++w)
				{
					float v = (bytes[index + w * byteDepth] / 255.0f);
					c[w + h * width + d * width * height] = new Color(v, v, v, v);
				}

				index += pitch;
			}
		}

		_noiseTexture.SetPixels(c);
		_noiseTexture.Apply();

	}

	}
