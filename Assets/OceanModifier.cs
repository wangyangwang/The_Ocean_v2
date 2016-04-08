using UnityEngine;
using System.Collections;

public class OceanModifier : MonoBehaviour {

	float eegReading;
	int touchingForehead = 0;
	public bool controllingWaves = true;

	//normal
	float minOceanScale = 4.0f;
	float minWaveSpeed = 0.3f;
	float minVol = 0.05f;

	//max
	float maxWaveSpeed = 1.65f;
	float maxOceanScale = 12.5f;
	float maxVol = 1.0f;

	float incrementArg = 0.01f;
	float waveSpeedIncrementArg = 0.0001f;

	//current
	float oceanScale;//1-10
	float waveSpeed; //0-3

	public float vol = 1.0f;
	public Ocean myOcean;
	oscReceive oscReceiver;

	// Use this for initialization
	void Start () {
		oceanScale = minOceanScale;
		waveSpeed = minWaveSpeed;
		oscReceiver = GameObject.Find ("osc").GetComponent<oscReceive> ();
	}
	
	// Update is called once per frame
	void Update () {

		eegReading = oscReceiver.mellowValue;
		touchingForehead = oscReceiver.touchingForehead;

		if (touchingForehead == 1) {
			oceanScale = (CustomFunc.Map (eegReading, 1, 0, minOceanScale, maxOceanScale) - oceanScale) * incrementArg + oceanScale;
			waveSpeed = (CustomFunc.Map (eegReading, 1, 0, minWaveSpeed, maxWaveSpeed) - waveSpeed) * waveSpeedIncrementArg + waveSpeed;
		} else {
			oceanScale = (minOceanScale - oceanScale) * incrementArg + oceanScale;
			waveSpeed = (minWaveSpeed - waveSpeed) * waveSpeedIncrementArg + waveSpeed;
		}

		oceanScale = Mathf.Clamp (oceanScale, minOceanScale, maxOceanScale);
		waveSpeed = Mathf.Clamp (waveSpeed, minWaveSpeed, maxWaveSpeed);
		vol = CustomFunc.Map (oceanScale, minOceanScale, maxOceanScale, minVol, maxVol);
		vol = Mathf.Clamp (vol, 0.05f, 1.0f);

		AudioListener.volume = vol;

		if(controllingWaves){
			myOcean.scale = oceanScale;
			myOcean.speed = waveSpeed;
		}


	}




}