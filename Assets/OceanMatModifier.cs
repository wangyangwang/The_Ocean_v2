using UnityEngine;
using System.Collections;

public class OceanMatModifier : MonoBehaviour {

	public Material oceanMat;
	public GameObject oscGO;

	oscReceive oscRe;

	Color touchingStateColor;
	Color nontouchingStateColor;

	Color finalColor;

	float t;
	float incrementArg = 0.006f;

	// Use this for initialization
	void Start () {
		oscRe = oscGO.GetComponent<oscReceive> ();
		finalColor = Color.black;

		touchingStateColor = Color.black;
		nontouchingStateColor = new Color (0.8f, 0.8f, 0.8f);
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




}
