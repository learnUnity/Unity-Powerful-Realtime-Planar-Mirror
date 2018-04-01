using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffects : MonoBehaviour {
	public CustomRenderTexture[] customRT;
	// Use this for initialization
	public Shader generateRTShader;
	[Range(0.1f, 100f)]
	public float dispearSpeed = 0.8f;

	int rainProcessID;
	CustomRenderTextureUpdateZone zone0;
	CustomRenderTextureUpdateZone zone1;
	CustomRenderTextureUpdateZone zone2;
	CustomRenderTextureUpdateZone zone3;
	CustomRenderTextureUpdateZone zone4;
	CustomRenderTextureUpdateZone[] zones;
	CustomRenderTextureUpdateZone[] onezone;

	void OnEnable(){
		rainProcessID = Shader.PropertyToID ("_RainVariable");

		foreach (var v in customRT) {
			v.material.SetTexture ("_Tex", v);
			v.Initialize ();
		}
		zone0 = new CustomRenderTextureUpdateZone ();
		zone0.passIndex = 0;
		zone0.updateZoneCenter = new Vector3(0.5f,0.5f);
		zone0.updateZoneSize = new Vector3 (1, 1);
		zone1 = new CustomRenderTextureUpdateZone ();
		zone1.passIndex = 1;
		zone2 = zone1;
		zone2.passIndex = 2;
		zone3 = zone1;
		zone3.passIndex = 3;
		zone4 = zone1;
		zone4.passIndex = 4;
		onezone = new CustomRenderTextureUpdateZone[]{ zone0 };
		zones = new CustomRenderTextureUpdateZone[]{ zone0,  zone1,zone2,zone3,zone4};
		foreach(var v in customRT)
			v.SetUpdateZones (zones);
		StartCoroutine (UpdateCT ());
	}

	IEnumerator UpdateCT(){
		int g = 0;
		while (true) {
			Shader.SetGlobalFloat (rainProcessID, dispearSpeed * 0.02f);
			/*for (int i = 0; i < 4; ++i) {
				if (g == i) {
					
					zone1.updateZoneCenter = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f)); 
					float randomRange = Random.Range (0.1f, 0.3f);
					zone1.updateZoneSize = new Vector3 (randomRange, randomRange);
					zones [1] = zone1;
					customRT [i].SetUpdateZones (zones);
					customRT [i].Update ();
				} else {
					customRT [i].SetUpdateZones (onezone);
					customRT [i].Update ();

				}
			}
			if (++g >= 4)
				g = 0;*/
			foreach(var v in customRT) {
				zone1.updateZoneCenter = new Vector3 (Random.Range (0.025f, 0.475f), Random.Range (0.025f, 0.475f)); 
				float randomRange = Random.Range (0.03f, 0.15f);
				zone1.updateZoneSize = new Vector3 (randomRange, randomRange);
				zones [1] = zone1;


				zone2.updateZoneCenter = new Vector3 (Random.Range (0.025f, 0.475f), Random.Range (0.525f, 0.975f)); 
				randomRange = Random.Range (0.03f, 0.15f);
				zone2.updateZoneSize = new Vector3 (randomRange, randomRange);
				zones [2] = zone2;

				zone3.updateZoneCenter = new Vector3 (Random.Range (0.525f, 0.975f), Random.Range (0.025f, 0.475f)); 
				randomRange = Random.Range (0.03f, 0.15f);
				zone3.updateZoneSize = new Vector3 (randomRange, randomRange);
				zones [3] = zone3;


				zone4.updateZoneCenter = new Vector3 (Random.Range (0.525f, 0.975f), Random.Range (0.525f, 0.975f)); 
				randomRange = Random.Range (0.03f, 0.15f);
				zone4.updateZoneSize = new Vector3 (randomRange, randomRange);
				zones [4] = zone4;
				v.SetUpdateZones (zones);
				v.Update ();
			}
			yield return new WaitForSeconds (0.02f);
		}
	}
}
