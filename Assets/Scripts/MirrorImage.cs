using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component will be automatically added on the mirror camera!
/// You can write your own post processing code here!
/// </summary>
public class MirrorImage : MonoBehaviour {
	Material mat;
	Camera cam;
	void Awake(){
		cam = GetComponent<Camera> ();
		mat = new Material (Shader.Find("Hidden/Mirror-Blur"));
	}
	int ivpID;
	void OnRenderImage(RenderTexture src, RenderTexture dest){
		Graphics.Blit (src, dest, mat);
		Graphics.Blit (dest, src, mat);
		Graphics.Blit (src, dest, mat);
	}
}
