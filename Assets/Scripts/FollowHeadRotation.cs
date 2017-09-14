using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeadRotation : MonoBehaviour
{
    public GameObject objectToFollow;
    public Vector3 offset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    this.transform.eulerAngles = objectToFollow.transform.eulerAngles + offset;
	}
}
