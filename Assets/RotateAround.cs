/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 1.60
 * Created Date: 04/15/13 
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Used to rotate around a given object.
  * 
 *  Inputs:
 * 
 *      rotateAroundObject: Rotates around this GameObject.
 *      
 *      speed: Speed of rotation.
 *      
 *      axis: Axis of the rotation.
 * 
*******************************************************************************************/
using System.Collections;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    //Rotates around this GameObject 
    public GameObject rotateAroundObject;

    //Speed of rotation
    public float speed = 10f;

    //Axis of the rotation
    public Vector3 axis = Vector3.up;

	private Vector3 initPos;
	public Vector3 offset;
	public bool embedInFollowTransform = false;

	[Header("Following Something")]
	public bool followSomething =false;
	public Transform followThing;
	public float acceleration = 2f;
	public bool randomSpeed;
	private float randomTargetSpeed;
    //Use this for initialization
    void Start()
    {
		if (GetComponent<Collider>() != null){
			if (GetComponent<Rigidbody>() == null){
				gameObject.AddComponent<Rigidbody>().isKinematic = true;
			}
		}
		EnsureNoStaticColliders();

		initPos = this.gameObject.transform.position;
		if(embedInFollowTransform)
			transform.parent = followThing;
    }

    //Update is called once per frame
    void Update()
    {	
		if(rotateAroundObject==null)
			rotateAroundObject = this.gameObject;

		if(randomSpeed){
			if(Random.Range (0,1000)<80){
				speed = Random.Range (-80f, 80f);
			}
//			speed = Mathf.Lerp(speed, randomTargetSpeed, Time.deltaTime*0.5f);
		}

	    transform.RotateAround(rotateAroundObject.transform.position, axis, speed * Time.deltaTime);

    }

	void LateUpdate(){
		if(followSomething && followThing!=null){
			//this.transform.position = new Vector3(followThing.transform.position.x, initPos.y, followThing.transform.position.z);
			this.transform.position =  Vector3.Lerp(this.transform.position, 
			                                        followThing.position + offset,
			                                        Time.deltaTime*acceleration);

			//this.transform.rotation = followThing.transform.rotation;
		}
	}

	public void followThisThing(bool onOrOff, Transform target){
		followSomething = onOrOff;
		followThing = target;
	}

	public void setToOrigPos(){
		this.transform.position = initPos;
		followSomething = false;
	}


	void EnsureNoStaticColliders(){
		Collider[] colliders = GetComponentsInChildren<Collider>(true);
		foreach(var c in colliders){
			if (c.GetComponent<Rigidbody>() == null){
				c.gameObject.AddComponent<Rigidbody>().isKinematic = true;
			}
		}
	}
}