using UnityEngine;
using System.Collections;

public class OceanModifier : MonoBehaviour {

	float eegReading;
	public bool controllingWaves = true;
	//normal
	float normalOceanScale = 5.5f;
	float normalWaveSpeed = 0.7f;

	//max
	float maxWaveSpeed = 3.0f;
	float maxOceanScale = 18.0f;
	float maxVol = 1.5f;

	//current
	float oceanScale;//1-10
	float waveSpeed; //0-3


	float vol = 1.0f;


	public Ocean myOcean;

	// Use this for initialization
	void Start () {
		oceanScale = normalOceanScale;
		waveSpeed = normalWaveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		eegReading = GameObject.Find("osc").GetComponent<oscReceive>().inputData;

		oceanScale = (map(eegReading,100,0,normalOceanScale,maxOceanScale) - oceanScale) * 0.008f + oceanScale;
		waveSpeed = map(eegReading,0,100,normalWaveSpeed,maxWaveSpeed);
		vol = (map(eegReading,100,0,0.0f,1.0f) - vol) * 0.035f + vol;

		if(oceanScale>maxOceanScale)oceanScale = maxOceanScale;
		if(waveSpeed>maxWaveSpeed)waveSpeed = maxWaveSpeed;
		if(vol>maxVol)vol = maxVol;


		if(controllingWaves){
			myOcean.scale = oceanScale;
		}
		AudioListener.volume = vol;

	}

	float map(float value, 
		float istart, 
		float istop, 
		float ostart, 
		float ostop) {
		return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
	}


}
