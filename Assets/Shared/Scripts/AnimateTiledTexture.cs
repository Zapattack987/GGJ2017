using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTiledTexture : MonoBehaviour {
	public float rows;
	public float cols;
	public float fps;
	public bool animateX;
	public bool animateY;
	private Renderer renderer;

	private int i = 0;

	void Start (){
		renderer = this.GetComponent<Renderer>();

		StartCoroutine (updateTiles ());
	
		Vector2 size = new Vector2 (1f / cols, 1f / rows);
		renderer.sharedMaterial.SetTextureScale ("_MainTex", size);
	}

	private IEnumerator updateTiles(){
		while (true) {
			i++;
			if (i >= rows * cols) {
				i = 0;
			}
			Vector2 offset = new Vector2 ((animateX ? (float)rows - (i / cols) : 0), (animateY ? (i / cols) / (float)rows : 0));

			renderer.sharedMaterial.SetTextureOffset ("_MainTex", offset);

			yield return new WaitForSeconds (1f / fps);
		}
	}

}
