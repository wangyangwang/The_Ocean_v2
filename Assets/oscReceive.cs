using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class oscReceive : MonoBehaviour {


	public delegate IEnumerator BlinkHandler();
	public static event BlinkHandler Blinked;

	private string UDPHost = "127.0.0.1";
	private int listenerPort = 5001;
	private int broadcastPort = 57131;
	private Osc oscHandler; //mellow : float 
	private Osc oscHandler2; //touching forhead : int 
	private Osc oscHandler3; //battery percentage: int
	private Osc oscHandler4; //battery percentage: int

	public float mellowValue = 0;
	public int touchingForehead = 0;
	public float batteryPercentage;
	public int blink;

	// Use this for initialization
	void Start () {
		
		batteryPercentage = 0; //set to 0 by default

		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(UDPHost,broadcastPort,listenerPort);
		oscHandler = GetComponent<Osc>();
		oscHandler.init(udp);
		oscHandler.SetAllMessageHandler(getMellowInput);
	}


	void Update(){


//		if(blink==1){
//			if (Blinked != null) {
//				StartCoroutine (Blinked ());
//			}
//		}


		//temporary test
//
//		if (Input.GetKeyDown ("space")){
//			Debug.Log ("blinked!!");
//			if (Blinked != null) {
//				StartCoroutine (Blinked ());
//			}
//		}
		//...
		
	}

	public void getMellowInput(OscMessage oscMessage) {
		OscMessage m = oscMessage;

//		Debug.Log (m.Address);
		Osc.OscMessageToString(m);

		if (m.Address == "/muse/elements/experimental/mellow") {
			mellowValue = (float)(m.Values [0]);
		} 


		if (m.Address == "/muse/elements/touching_forehead") {
			touchingForehead = (int)(m.Values [0]);
		} 

		if (m.Address == "/muse/batt") {
			batteryPercentage = (int)(m.Values [0]);
			batteryPercentage = CustomFunc.Map (batteryPercentage, 0, 10000, 0, 100);
		}
			
		if (m.Address == "/muse/elements/blink") {
			blink = (int)m.Values [0];
		}

			
//		Osc.OscMessageToString(oscMessage);
//		mellowValue =  (float)(oscMessage.Values[0]);
//		Debug.Log("mellow: " + inputData );
	}

//	public void getTouchingForehead(OscMessage oscMessage) {
//		Osc.OscMessageToString(oscMessage);
//		touchingForehead = (int)(oscMessage.Values [0]);
////		Debug.Log ("touching forehead: " + (int)(oscMessage.Values [0]));// Int32.Parse(oscMessage.Values[0]);
//	}
//
//	public void getBatteryPer(OscMessage oscMessage){
//		batteryPercentage = (int)oscMessage.Values[0];
//		batteryPercentage = CustomFunc.Map (batteryPercentage, 0, 10000, 0, 100);
////		Debug.Log ("battery%: "+ batteryPercentage);
//	}
//
//	public void getBlink(OscMessage oscMessage){
//		blink = (Boolean)oscMessage.Values[0];
//		Debug.Log (blink);
//	}


}
