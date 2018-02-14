using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PIRSensor))]
public class GlitchOcean : MonoBehaviour
{


    Kino.AnalogGlitch glitch;
    PIRSensor sensor;


    public AudioSource oceanSound;


    // Use this for initialization
    void Start()
    {
        sensor = GetComponent<PIRSensor>();
        sensor.MotionDetected += CreateGlitch;

        glitch = Camera.main.GetComponent<Kino.AnalogGlitch>();
    }

    void CreateGlitch()
    {
        if (glitch != null)
        {
            glitch.scanLineJitter = Random.Range(0.2f, 1);
            glitch.verticalJump = Random.Range(0.2f, 1);
            glitch.horizontalShake = Random.Range(0.2f, 1);
            glitch.colorDrift = Random.Range(0.2f, 1);

            StartCoroutine(ShiftingPitch());

            Invoke("GlitchOver", Random.Range(0.2f, 1f));
        }

    }

    IEnumerator ShiftingPitch(){
        while(true)
        {
            oceanSound.pitch = Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void GlitchOver()
    {
        glitch.scanLineJitter = 0;
        glitch.verticalJump = 0;
        glitch.horizontalShake = 0;
        glitch.colorDrift = 0;

        oceanSound.pitch = 0.8f;
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
