using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class oscReceive : MonoBehaviour {


	private string UDPHost = "127.0.0.1";
	private int listenerPort = 5001;
	private int broadcastPort = 57131;
	private Osc oscHandler;


//	private string eventName = "";
//	private string eventData = "";
	public float inputData = 0;


	// Use this for initialization
	void Start () {
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(UDPHost,broadcastPort,listenerPort);
		oscHandler = GetComponent<Osc>();
		oscHandler.init(udp);

		oscHandler.SetAddressHandler("/muse/elements/experimental/mellow",getInput);
	}


	void Update(){
		
	}

	public void getInput(OscMessage oscMessage) {
		Osc.OscMessageToString(oscMessage);
		inputData =  (float)(oscMessage.Values[0]); // Int32.Parse(oscMessage.Values[0]);


	}


}
