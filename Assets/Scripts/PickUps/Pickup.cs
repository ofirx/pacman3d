using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	public float destroyTime = 10;
	public int scoreAmount;
	// Use this for initialization
	void Start () 
	{
		Destroy(gameObject, destroyTime);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			GameManager.instance.AddScore(scoreAmount);
			Destroy(gameObject);
		}
	}
}
