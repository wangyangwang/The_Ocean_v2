using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class oscReceive : MonoBehaviour {

	private string UDPHost = "127.0.0.1";
	private int listenerPort = 5001;
	private int broadcastPort = 57131;
	private Osc oscHandler; //mellow : float 
	private Osc oscHandler2; //touching forhead : int 

	public float inputData = 0;
	public int touchingForehead = 0;


	// Use this for initialization
	void Start () {
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		UDPPacketIO udp2 = GetComponent<UDPPacketIO>();

		udp.init(UDPHost,broadcastPort,listenerPort);
		udp2.init(UDPHost,broadcastPort,listenerPort);

		oscHandler = GetComponent<Osc>();
		oscHandler2 = GetComponent<Osc> ();

		oscHandler.init(udp);
		oscHandler2.init (udp2);

		oscHandler.SetAddressHandler("/muse/elements/experimental/mellow",getMellowInput);
		oscHandler2.SetAddressHandler("/muse/elements/touching_forehead",getConcentrationInput);
	}


	void Update(){
		
	}

	public void getMellowInput(OscMessage oscMessage) {
		Osc.OscMessageToString(oscMessage);
		inputData =  (float)(oscMessage.Values[0]);
//		Debug.Log("mellow: " + inputData );
	}

	public void getConcentrationInput(OscMessage oscMessage) {
		Osc.OscMessageToString(oscMessage);
		touchingForehead = (int)(oscMessage.Values [0]);
//		Debug.Log ("concentration: " + (int)(oscMessage.Values [0]));// Int32.Parse(oscMessage.Values[0]);
	}


}
