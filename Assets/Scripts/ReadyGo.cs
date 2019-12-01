using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyGo : MonoBehaviour {

	public void JustGo()
	{
		GameManager.instance.StartRound();
		gameObject.SetActive(false);
	}
}
