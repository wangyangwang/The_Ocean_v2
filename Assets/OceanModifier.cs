using UnityEngine;
using System.Collections;

public class OceanModifier : MonoBehaviour {

	float eegReading;
	public bool controllingWaves = true;
	//normal
	float minOceanScale = 4.0f;
	float normalWaveSpeed = 0.7f;

	//max
	float maxWaveSpeed = 3.0f;
	float maxOceanScale = 12.5f;
	float maxVol = 1.5f;


	float incrementArg = 0.01f;

	//current
	float oceanScale;//1-10
	float waveSpeed; //0-3


	float vol = 1.0f;


	public Ocean myOcean;

	// Use this for initialization
	void Start () {
		oceanScale = minOceanScale;
		waveSpeed = normalWaveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		eegReading = GameObject.Find("osc").GetComponent<oscReceive>().mellowValue;

		oceanScale = (CustomFunc.Map(eegReading,1,0,minOceanScale,maxOceanScale) - oceanScale) * incrementArg + oceanScale;
		waveSpeed = CustomFunc.Map(eegReading,1,0,normalWaveSpeed,maxWaveSpeed);
		vol = CustomFunc.Map (oceanScale, minOceanScale, maxOceanScale, 0.2f, 1.0f);

		AudioListener.volume = vol;
		if(oceanScale>maxOceanScale)oceanScale = maxOceanScale;
		if(waveSpeed>maxWaveSpeed)waveSpeed = maxWaveSpeed;

		if(controllingWaves){
			myOcean.scale = oceanScale;
		}


	}




}
