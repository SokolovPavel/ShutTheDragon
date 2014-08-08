using UnityEngine;
using System.Collections;

public class Control : Photon.MonoBehaviour {

	public Rigidbody plane;
	public float maxThr;
	public int thrStep;
	public float minThr;

	private Vector3 fwd;
	private float xSpd;
	private float ySpd;
	private float thrust;

	private float seaLevel;
	private float alt;
	private float range;
	private float curSpd;



	void Awake () {
		if (photonView.isMine) {
			Screen.lockCursor = true;
			rigidbody.isKinematic = false;
		} else {
			rigidbody.isKinematic = true;
		}
		plane = this.gameObject.rigidbody;

		seaLevel = this.transform.position.y;
		fwd = transform.TransformDirection (Vector3.up);

	}

	void OnGUI() {
		if (photonView.isMine) {
			GUI.Label (new Rect (20, 20, 200, 20), "THR: " + thrust);
			GUI.Label (new Rect (20, 40, 200, 20), "SPD: " + Mathf.FloorToInt (curSpd));
			GUI.Label (new Rect (20, 60, 200, 20), "ALT: " + Mathf.FloorToInt (alt));
			GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 20), "Range: " + Mathf.FloorToInt (range));
		}
	}

	void FixedUpdate ()
	{	
		if (photonView.isMine) {
			curSpd = plane.velocity.magnitude;

			Screen.lockCursor = true;
			alt = transform.position.y - seaLevel;

			fwd = transform.TransformDirection (Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, fwd, out hit,800.0f)) {
				Debug.DrawRay (this.transform.position, fwd * 10);
				range = hit.distance;
			}

			plane.AddForce (Vector3.up* plane.mass * 9.85f );
			plane.AddRelativeForce (Vector3.forward* thrust);
			plane.AddRelativeTorque (0,  xSpd*35.0f,0);
			plane.AddRelativeTorque ( -ySpd*35.0f, 0,0);
		}

	}
	

	void Update() {
		if (photonView.isMine) {

			xSpd = Input.GetAxis ("Mouse X");
			ySpd = Input.GetAxis ("Mouse Y");

			if(Input.GetKey("s")){
				thrust -= thrStep;
				if (thrust < -maxThr) {
					thrust = -maxThr;
				}
			}

			if(Input.GetKey("w")){
				thrust += thrStep;
				if (thrust > maxThr) {
					thrust = maxThr;
				}
			}

			if (Input.GetKey (KeyCode.LeftShift) && (Input.GetKey ("a"))) {
				plane.AddRelativeForce (-10000, 0, 0);
			} else if (Input.GetKey ("a")) {
				plane.AddRelativeTorque (0, 0, 20);
				//	dampRoll = false;
			}

			if(Input.GetKey(KeyCode.LeftShift)&&(Input.GetKey("d"))){
				plane.AddRelativeForce (10000, 0, 0);
			} else if (Input.GetKey ("d")) {
				//dampRoll = false;
				plane.AddRelativeTorque (0, 0, -20);
			} 

			if(Input.GetKey("x")){
				thrust = 0;

			}

			if(Input.GetKeyDown(KeyCode.Space)){
				plane.AddRelativeForce (0, 80000, 0);
			}


		}
	}
}
