using UnityEngine;
using System.Collections;

public class Gate : Photon.MonoBehaviour {
	public AudioSource snd;
	
	void OnTriggerEnter(Collider col ) {
		if(col.gameObject.tag == "Ball") {
		 snd.Play();
		}
	
	}
	
}
