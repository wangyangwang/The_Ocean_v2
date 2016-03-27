using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class oscReceive : MonoBehaviour {


	private string UDPHost = "127.0.0.1";
	private int listenerPort = 7771;
	private int broadcastPort = 57131;
	private Osc oscHandler;


//	private string eventName = "";
//	private string eventData = "";
	public int inputData = 0;


	// Use this for initialization
	void Start () {
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(UDPHost,broadcastPort,listenerPort);
		oscHandler = GetComponent<Osc>();
		oscHandler.init(udp);

		oscHandler.SetAddressHandler("/meditation",getInput);
	}


	void Update(){

	}

	public void getInput(OscMessage oscMessage) {
		Osc.OscMessageToString(oscMessage);
		inputData =  Convert.ToInt32(oscMessage.Values[0]); // Int32.Parse(oscMessage.Values[0]);


	}

	public int getData(){
		return inputData;
	}

}
