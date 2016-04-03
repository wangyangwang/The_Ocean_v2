using UnityEngine;
using System.Collections;

public class OceanMatModifier : MonoBehaviour {


	public Material oceanMat;
	public GameObject oscGO;

	oscReceive oscRe;

	Color touchingStateColor;
	Color nontouchingStateColor;

	Color finalColor;

	public Color[] glitchColors;

	float t;
	float incrementArg = 0.006f;


	void OnEnable(){
		oscReceive.Blinked += glitch;
	}

	void OnDisable(){
		oscReceive.Blinked -= glitch;
	}


	// Use this for initialization
	void Start () {
		oscRe = oscGO.GetComponent<oscReceive> ();
		finalColor = Color.black;

		touchingStateColor = Color.black;
		nontouchingStateColor = new Color (0.5f, 0.5f, 0.5f);
		t = oscRe.touchingForehead;
	}
	
	// Update is called once per frame
	void Update () {

		t += (oscRe.touchingForehead - t) * incrementArg;
		finalColor = Color.Lerp (nontouchingStateColor, touchingStateColor, t);

		if (oceanMat.GetColor ("_EmissionColor") != finalColor) {
			oceanMat.SetColor ("_EmissionColor", finalColor);
		}
	}

	IEnumerator glitch() {
		
		for (int i=0;i<glitchColors.Length;i++) {
			
			if (i < glitchColors.Length) {
				oceanMat.SetColor ("_EmissionColor", glitchColors [i]);
			}else{
				oceanMat.SetColor ("_EmissionColor", touchingStateColor);
			}

			yield return new WaitForSeconds(0.001f);
		}
	}




}
