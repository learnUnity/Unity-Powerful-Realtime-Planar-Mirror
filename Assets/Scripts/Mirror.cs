using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;
using System.Collections.Generic;

public class Mirror : MonoBehaviour
{

	public string ReflectionSample = "_ReflectionTex";
	public bool addPostProcessingComponent = false;
	public static bool s_InsideRendering = false;
	private int uniqueTextureID = -1;

	[HideInInspector]
	public int textureSize {
		get {
			return m_TextureSize;
		}
		set {
			m_TextureSize = Mathf.Clamp (value, 1, 2048);
			if (m_ReflectionTexture) {
				if (m_ReflectionTexture.IsCreated ()) {
					m_ReflectionTexture.Release ();
					m_ReflectionTexture.width = m_TextureSize;
					m_ReflectionTexture.height = m_TextureSize;
					m_ReflectionTexture.Create ();
				} else {
					m_ReflectionTexture.width = m_TextureSize;
					m_ReflectionTexture.height = m_TextureSize;
				}
			}
		}
	}

	[HideInInspector]
	public int m_TextureSize = 256;
	[HideInInspector]
	public float m_ClipPlaneOffset = 0.01f;
	[Tooltip ("With lots of small mirrors in the same plane position, you can add several SmallMirrors components and manage them with only one Mirror component to significantly save cost")]
	public SmallMirrors[] allMirrors = new SmallMirrors[0];

	public enum AntiAlias
	{
		X1 = 1,
		X2 = 2,
		X4 = 4,
		X8 = 8
	}

	[Tooltip ("MSAA anti alias")]
	public AntiAlias antiAlias = AntiAlias.X8;
	[Tooltip ("The normal transform(transform.up as normal)")]
	public Transform normalTrans;
	[Tooltip ("Mirror mask")]
	public LayerMask m_ReflectLayers = -1;

	public enum RenderQuality
	{
		Default,
		High,
		Medium,
		Low,
		VeryLow
	}

	[Tooltip ("Reflection Quality")]
	public RenderQuality renderQuality = RenderQuality.Default;
	private RenderTexture m_ReflectionTexture = null;
	[HideInInspector]
	public bool useDistanceCull = false;
	[HideInInspector]public float m_SqrMaxdistance = 2500f;
	[HideInInspector]public float m_maxDistance = 50f;

	public float maxDistance {
		get { 
			return m_maxDistance;
		}
		set { 
			m_maxDistance = value;
			m_SqrMaxdistance = value * value;
		}
	}


	public bool enableSelfCullingDistance = true;
	[HideInInspector]
	public float[] layerCullingDistances = new float[32];
	[HideInInspector]
	public Renderer render;
	Camera cam;
	Camera reflectionCamera;
	Transform refT;
	Transform camT;
	private List<Material> allMats = new List<Material> ();
	private Action postProcessAction;

	void Awake ()
	{
		uniqueTextureID = Shader.PropertyToID (ReflectionSample);
		if (!normalTrans) {
			normalTrans = new GameObject ("Normal Trans").transform;
			normalTrans.position = transform.position;
			normalTrans.rotation = transform.rotation;
			normalTrans.SetParent (transform);
		}
		render = GetComponent<Renderer> ();
		if (!render || !render.sharedMaterial) {
			Destroy (this);
		}
		for (int i = 0; i < allMirrors.Length; ++i) {
			allMirrors [i].manager = this;
		}
		for (int i = 0, length = render.sharedMaterials.Length; i < length; ++i) {
			Material m = render.sharedMaterials [i];
			if (!allMats.Contains (m))
				allMats.Add (m);
		}
		for (int i = 0; i < allMirrors.Length; ++i) {
			Renderer r = allMirrors [i].GetRenderer ();
			for (int a = 0, length = r.sharedMaterials.Length; a < length; ++a) {
				Material m = r.sharedMaterials [a];
				if (!allMats.Contains (m))
					allMats.Add (m);
			}
		}
		bool billboard;
		bool softVeg;
		bool softParticle;
		postProcessAction = () => {
		};
		AnisotropicFiltering ani;
		ShadowResolution shaR;
		ShadowQuality shadowQuality;
		switch (renderQuality) {
		case RenderQuality.Default:
			postProcessAction += ()=>reflectionCamera.Render ();
			break;
		case RenderQuality.High:
			postProcessAction += () => {
				billboard = QualitySettings.billboardsFaceCameraPosition;
				QualitySettings.billboardsFaceCameraPosition = false;
				softParticle = QualitySettings.softParticles;
				softVeg = QualitySettings.softVegetation;
				QualitySettings.softParticles = false;
				QualitySettings.softVegetation = false;
				ani = QualitySettings.anisotropicFiltering;
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				shaR = QualitySettings.shadowResolution;
				QualitySettings.shadowResolution = ShadowResolution.High;
				reflectionCamera.Render ();
				QualitySettings.softParticles = softParticle;
				QualitySettings.softVegetation = softVeg;
				QualitySettings.billboardsFaceCameraPosition = billboard;
				QualitySettings.anisotropicFiltering = ani;
				QualitySettings.shadowResolution = shaR;
			};
			break;
		case RenderQuality.Medium:
			postProcessAction += () => {
				softParticle = QualitySettings.softParticles;
				softVeg = QualitySettings.softVegetation;
				QualitySettings.softParticles = false;
				QualitySettings.softVegetation = false;
				billboard = QualitySettings.billboardsFaceCameraPosition;
				QualitySettings.billboardsFaceCameraPosition = false;
				shadowQuality = QualitySettings.shadows;
				QualitySettings.shadows = ShadowQuality.HardOnly;
				ani = QualitySettings.anisotropicFiltering;
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				shaR = QualitySettings.shadowResolution;
				QualitySettings.shadowResolution = ShadowResolution.Low;
				reflectionCamera.Render ();
				QualitySettings.softParticles = softParticle;
				QualitySettings.softVegetation = softVeg;
				QualitySettings.shadows = shadowQuality;
				QualitySettings.billboardsFaceCameraPosition = billboard;
				QualitySettings.anisotropicFiltering = ani;
				QualitySettings.shadowResolution = shaR;
			};
			break;
		case RenderQuality.Low:
			postProcessAction += () => {
				softParticle = QualitySettings.softParticles;
				softVeg = QualitySettings.softVegetation;
				QualitySettings.softParticles = false;
				QualitySettings.softVegetation = false;

				shadowQuality = QualitySettings.shadows;
				QualitySettings.shadows = ShadowQuality.Disable;

				ani = QualitySettings.anisotropicFiltering;
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				reflectionCamera.Render ();
				QualitySettings.softParticles = softParticle;
				QualitySettings.softVegetation = softVeg;

				QualitySettings.shadows = shadowQuality;
				QualitySettings.anisotropicFiltering = ani;
			};
			break;
		case RenderQuality.VeryLow:
			postProcessAction += () => {
				ani = QualitySettings.anisotropicFiltering;
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				reflectionCamera.Render ();
				QualitySettings.anisotropicFiltering = ani;
			};
			break;
		}

	}

	public void SetTexture (RenderTexture target)
	{
		for (int i = 0, length = allMats.Count; i < length; ++i) {
			try {
				Material m = allMats [i];
				m.SetTexture (uniqueTextureID, target);
			} catch {
				
			}
		}
	}

	void OnDestroy(){
		DestroyImmediate (m_ReflectionTexture);
		DestroyImmediate (reflectionCamera.gameObject);
	}

	void Start ()
	{
		m_SqrMaxdistance = m_maxDistance * m_maxDistance;
		m_ReflectionTexture = new RenderTexture (m_TextureSize, m_TextureSize, 16);
		m_ReflectionTexture.name = "__HighQualityReflection" + GetInstanceID ();
		m_ReflectionTexture.isPowerOfTwo = true;
		m_ReflectionTexture.filterMode = FilterMode.Trilinear;
		m_ReflectionTexture.antiAliasing = (int)antiAlias;
		GameObject go = new GameObject ("MirrorCam", typeof(Camera), typeof(FlareLayer));
		reflectionCamera = go.GetComponent<Camera> ();
		go.hideFlags = HideFlags.HideAndDontSave;

		//mysky = go.AddComponent<Skybox> ();
		go.transform.SetParent (normalTrans);
		reflectionCamera.enabled = false;
		reflectionCamera.targetTexture = m_ReflectionTexture;

		reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; 
		reflectionCamera.layerCullSpherical = enableSelfCullingDistance;
		refT = reflectionCamera.transform;
		if (!enableSelfCullingDistance) {
			for (int i = 0, length = layerCullingDistances.Length; i < length; ++i) {
				layerCullingDistances [i] = 0;
			}
		} else {
			reflectionCamera.layerCullDistances = layerCullingDistances; 
		}
		reflectionCamera.useOcclusionCulling = false;
	//	#if UNITY_EDITOR
	//	SetTexture (m_ReflectionTexture);
	//	#else
		if (addPostProcessingComponent){
			MirrorImage mi = go.AddComponent<MirrorImage> ();
		}
		SetTexture (m_ReflectionTexture);
	//	#endif

	}

	Vector3 pos;
	Vector3 normal;
	Vector4 reflectionPlane;
	Vector4 clipPlane;
	Matrix4x4 reflection = Matrix4x4.zero;
	Matrix4x4 ref_WorldToCam;
	[System.NonSerialized]
	public bool isBaked = false;

	IEnumerator WaitTo ()
	{
		yield return null;
		isBaked = false;
	}

	public void OnWillRenderObject ()
	{
		if (s_InsideRendering || !enabled || !render.enabled || isBaked)
			return;
		s_InsideRendering = true;
		isBaked = true;
		StartCoroutine (WaitTo ());
		if (cam != Camera.current) {
			cam = Camera.current;
			camT = cam.transform;
			reflectionCamera.renderingPath = (renderQuality == RenderQuality.VeryLow) ? RenderingPath.VertexLit : cam.renderingPath;
			reflectionCamera.fieldOfView = cam.fieldOfView;
			reflectionCamera.clearFlags = cam.clearFlags;
			reflectionCamera.backgroundColor = cam.backgroundColor;
			reflectionCamera.allowHDR = false;
			reflectionCamera.allowMSAA = true;
			reflectionCamera.orthographic = cam.orthographic;
			reflectionCamera.aspect = cam.aspect;
			reflectionCamera.orthographicSize = cam.orthographicSize;
			reflectionCamera.depthTextureMode = DepthTextureMode.None;


		}
		if (useDistanceCull && Vector3.SqrMagnitude (normalTrans.position - camT.position) > m_SqrMaxdistance) {
			s_InsideRendering = false;
			return;
		}

		Vector3 localPos = normalTrans.worldToLocalMatrix.MultiplyPoint3x4
(camT.position);
		Matrix4x4 localToWorldMatrix = normalTrans.localToWorldMatrix;
		if (localPos.y < 0) {
			s_InsideRendering = false;
			return;
			
		}

		localPos.y = -localPos.y;
		refT.position = localToWorldMatrix.MultiplyPoint3x4
(localPos);
		refT.eulerAngles = camT.eulerAngles;
		Vector3 localEuler = refT.localEulerAngles;
		localEuler.x *= -1;
		refT.localEulerAngles = localEuler;
		normal = normalTrans.up;
		pos = normalTrans.position;
		float d = -Vector3.Dot (normal, pos) - m_ClipPlaneOffset;
		reflectionPlane = new Vector4 (normal.x, normal.y, normal.z, d);
		CalculateReflectionMatrix (ref reflection, reflectionPlane);
		ref_WorldToCam = cam.worldToCameraMatrix * reflection;
		reflectionCamera.worldToCameraMatrix = ref_WorldToCam;
		clipPlane = CameraSpacePlane (ref_WorldToCam, pos, normal, 1.0f);       
		reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix (clipPlane);

		GL.invertCulling = true;

		#if UNITY_EDITOR
		if (renderQuality == RenderQuality.VeryLow) {
			if (reflectionCamera.renderingPath != RenderingPath.VertexLit)
				reflectionCamera.renderingPath = RenderingPath.VertexLit;
		} else if (reflectionCamera.renderingPath != cam.renderingPath) {
			reflectionCamera.renderingPath = cam.renderingPath;
		}
		#endif
		postProcessAction ();
		GL.invertCulling = false;
		s_InsideRendering = false;
	}

	private static float sgn (float a)
	{
		if (a > 0.0f)
			return 1.0f;
		if (a < 0.0f)
			return -1.0f;
		return 0.0f;
	}

	private Vector4 CameraSpacePlane (Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Vector3 cpos = worldToCameraMatrix.MultiplyPoint3x4
(offsetPos);
		Vector3 cnormal = worldToCameraMatrix.MultiplyVector (normal).normalized * sideSign;
		return new Vector4 (cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot (cpos, cnormal));
	}

	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint3x4
(offsetPos);
		Vector3 cnormal = m.MultiplyVector (normal).normalized * sideSign;
		return new Vector4 (cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot (cpos, cnormal));
	}

	private static void CalculateReflectionMatrix (ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = (1F - 2F * plane [0] * plane [0]);
		reflectionMat.m01 = (-2F * plane [0] * plane [1]);
		reflectionMat.m02 = (-2F * plane [0] * plane [2]);
		reflectionMat.m03 = (-2F * plane [3] * plane [0]);

		reflectionMat.m10 = (-2F * plane [1] * plane [0]);
		reflectionMat.m11 = (1F - 2F * plane [1] * plane [1]);
		reflectionMat.m12 = (-2F * plane [1] * plane [2]);
		reflectionMat.m13 = (-2F * plane [3] * plane [1]);

		reflectionMat.m20 = (-2F * plane [2] * plane [0]);
		reflectionMat.m21 = (-2F * plane [2] * plane [1]);
		reflectionMat.m22 = (1F - 2F * plane [2] * plane [2]);
		reflectionMat.m23 = (-2F * plane [3] * plane [2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}
}
