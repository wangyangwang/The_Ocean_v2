using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MicInput))]
public class MicControlOcean : MonoBehaviour {

    [SerializeField]
    Ocean ocean;

    public float defaultScale = 4.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float loudness = MicInput.MicLoudness * 10000;
        float scale;
        Debug.Log(loudness);
        if(loudness > 1){
            scale = CustomFunc.Map(loudness, 1, 20, defaultScale, 12);
        }else{
            scale = defaultScale;
        }
        float yVelocity = 0.1F;
        ocean.scale = Mathf.SmoothDamp(ocean.scale, scale, ref yVelocity, 1f);
	}
}
