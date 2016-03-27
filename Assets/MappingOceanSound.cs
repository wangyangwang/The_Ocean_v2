using UnityEngine;
using System.Collections;

public class MappingOceanSound : MonoBehaviour {

	oscReceive osc_game_object;
	int osc_reading;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	float map(float value, 
		float istart, 
		float istop, 
		float ostart, 
		float ostop) {
		return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
	}


}
