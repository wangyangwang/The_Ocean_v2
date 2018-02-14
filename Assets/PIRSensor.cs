using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class PIRSensor : MonoBehaviour
{

    SerialPort stream = new SerialPort("/dev/cu.usbmodem1421", 9600);

    // Use this for initialization
    void Start()
    {
        stream.ReadTimeout = 10;
        stream.Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (stream.IsOpen)
        {
            try
            {
                string input = stream.ReadLine();
                Debug.Log(input);
            }
            catch (System.Exception e)
            {
             
            }


        }


    }
}
