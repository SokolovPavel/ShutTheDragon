using UnityEngine;
using System.Collections;

public class Ball : Photon.MonoBehaviour {
	
	private Vector3 fwd;
	private float time;
	public float triggerDist;
	public float throwTime = 2.0f;
	public int turnChance = 400;
	public float thrust = 5;
	public float maxThrust =10.0f;
	public float minThrust =4.0f;
	public BallState state;
	public enum BallState {
	FreeFlight,
	Pass,
	Thrown
	
	}
	
	
	// Use this for initialization
	void Awake () {
			if (photonView.isMine) {
			Screen.lockCursor = true;
			rigidbody.isKinematic = false;
			
		} else {
			rigidbody.isKinematic = true;
		}

		fwd = transform.TransformDirection (Vector3.up);
		state = BallState.FreeFlight;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	if (photonView.isMine) {
		if(state == BallState.FreeFlight) {
			rigidbody.AddForce(0,rigidbody.mass * 9.84f,0);
			rigidbody.AddRelativeForce(Vector3.forward * thrust);
			
			if(Random.Range(0, turnChance)==15) {
				rigidbody.AddRelativeTorque(Random.Range(0, 50),Random.Range(0, 50),Random.Range(0, 50));
				thrust = Random.Range(minThrust,maxThrust);
			}

		}
		
		if(state == BallState.Thrown) {
			time += Time.fixedDeltaTime;
			if (time>throwTime){
				state = BallState.FreeFlight;
			}
		}
	}
	}
	
		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);

		} else {
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
	
		private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		}
	}
	
	void DistCheck() {
	
	
	}
}
