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
	float maxOceanScale = 12.5f;
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

		oceanScale = (CustomFunc.Map(eegReading,1,0,normalOceanScale,maxOceanScale) - oceanScale) * 0.008f + oceanScale;
//		Debug.Log("Ocean Scale: "+ oceanScale);
		waveSpeed = CustomFunc.Map(eegReading,1,0,normalWaveSpeed,maxWaveSpeed);
		vol = CustomFunc.Map (oceanScale, normalOceanScale, maxOceanScale, 0.2f, 1.0f);

		AudioListener.volume = vol;
		if(oceanScale>maxOceanScale)oceanScale = maxOceanScale;
		if(waveSpeed>maxWaveSpeed)waveSpeed = maxWaveSpeed;

		if(controllingWaves){
			myOcean.scale = oceanScale;
		}


	}




}
