using UnityEngine;
using System.Collections;

public class Control : Photon.MonoBehaviour {

    public GUIStyle guiMarks;
    public GUIStyle normalStyle;

	private Player ply;
	public Transform ballPos;
	public Rigidbody plane;
	public float maxThr;
	public int thrStep;
	public float minThr;
	public float pitchCoef = 60f;
	public float yawCoef = 45f;
	private Vector3 fwd;
	private float xSpd;
	private float ySpd;
	private float thrust;

	private float seaLevel;
	private float alt;
	private float range;
	private float curSpd;
    public GameObject camObj;
    public Camera cam;
    private float camDefFOV;
    private Vector3 camDefPos;
    private Vector3 camPos;


    public Texture2D crosshairTex;
    private Rect crosshairPos;


    private GameObject selectedBall;
    private GameObject selectedPlayer;


    private readonly VectorPid angularVelocityController = new VectorPid(14.8f, 0, 0.27f);
    private readonly VectorPid headingController = new VectorPid(37f, 0, 0.08f);

    //private Vector3 vertVec;


	void Awake () {
		if (photonView.isMine) {
            cam = camObj.GetComponent<Camera>();
            camDefFOV = cam.fieldOfView;
            camDefPos = transform.position - camObj.transform.position;
			Screen.lockCursor = true;
			rigidbody.isKinematic = false;
		} else {
			rigidbody.isKinematic = true;
		}
		plane = this.gameObject.rigidbody;
		ply = this.gameObject.GetComponent<Player>();
		seaLevel = this.transform.position.y;
		fwd = transform.TransformDirection (Vector3.up);

        crosshairPos = new Rect((Screen.width - crosshairTex.width) / 2, (Screen.height - crosshairTex.height) / 2, crosshairTex.width, crosshairTex.height);

	}

	void OnGUI() {
		if (photonView.isMine) {
            

            GUI.DrawTexture(crosshairPos, crosshairTex);

			GUI.Label (new Rect (20, 20, 200, 20), "THR: " + thrust,normalStyle);
            GUI.Label(new Rect(20, 40, 200, 20), "SPD: " + Mathf.FloorToInt(curSpd), normalStyle);
            GUI.Label(new Rect(20, 60, 200, 20), "ALT: " + Mathf.FloorToInt(alt), normalStyle);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 20), "Range: " + Mathf.FloorToInt(range), normalStyle);

            GUI.Label(new Rect(20, 80, 200, 20), "Balls: " + ply.balls, normalStyle);
           // GUI.Label(new Rect(20, 80, 200, 20), "MouseX: " + xSpd);
           // GUI.Label(new Rect(20, 100, 200, 20), "MouseY: " + ySpd);
            //GUI.Label(new Rect(20, 80, 200, 20), "Angle: " + Vector3.Dot(transform.TransformDirection(Vector3.up),Vector3.up));


            if ((selectedBall != null)&&(selectedBall.renderer.isVisible))
            {
                Vector3 bPos = cam.WorldToScreenPoint(selectedBall.transform.position);
              //  GUI.Label(new Rect(Screen.width - bPos.x, bPos.y, 20, 20),"+");
                GUI.Label(new Rect(bPos.x - 21,Screen.height - bPos.y- 13, 30, 60), "<  >", guiMarks);
            }

		}
	}

	void FixedUpdate ()
	{	
		if (photonView.isMine) {
            xSpd = Input.GetAxis("Mouse X");
            ySpd = Input.GetAxis("Mouse Y");
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
			plane.AddRelativeTorque (0,  xSpd*yawCoef ,0);
			plane.AddRelativeTorque ( -ySpd*pitchCoef , 0,0);
           // plane.AddRelativeTorque(0, 0, (1 - Vector3.Dot(transform.TransformDirection(Vector3.up), Vector3.up)) * 50);

            //vertVec = 
          //  camObj.transform.position = transform.position - camDefPos +camObj.transform.TransformDirection(Vector3.back) * Mathf.Abs(curSpd * 0.1f);
         //   camObj.transform.LookAt(this.transform);


		}

	}
	

	void Update() {
		if (photonView.isMine) {
		
			if(Input.GetMouseButtonDown(0)){
				if(ply.balls>0){
				ply.photonView.RPC("ThrowBall",PhotonTargets.All);
				}
			}

        if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position + new Vector3(0,0.5f,0), 1.2f, transform.TransformDirection(Vector3.forward), out hit, 400.0f)) {
                    if (hit.collider.tag == "Ball")
                    {
                        selectedBall = hit.collider.gameObject;
                    }

                    if (hit.collider.tag == "Player")
                    {
                        selectedPlayer = hit.collider.gameObject;
                    }

                    Debug.Log(hit.transform.gameObject.name);
                }
               
            }


            if (Input.GetKey("r"))
             {
                Pursuit(selectedBall);
             }

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

			if (Input.GetKey (KeyCode.LeftShift) && (Input.GetKeyDown ("a"))) {
				plane.AddRelativeForce (-10000, 0, 0);
			} else if (Input.GetKey ("a")) {
				plane.AddRelativeTorque (0, 0, 90);
				//	dampRoll = false;
			}

			if(Input.GetKey(KeyCode.LeftShift)&&(Input.GetKeyDown("d"))){
				plane.AddRelativeForce (10000, 0, 0);
			} else if (Input.GetKey ("d")) {
				//dampRoll = false;
				plane.AddRelativeTorque (0, 0, -90);
			} 

			if(Input.GetKey("x")){
				thrust = 0;

			}

			if(Input.GetKeyDown(KeyCode.Space)){
				plane.AddRelativeForce (0, 80000, 0);
			}


		}
	}


    private void Pursuit(GameObject target)
    {
        if (target == null) { return; }

        var angularVelocityError = plane.angularVelocity * -1;
        var angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
       plane.AddTorque(angularVelocityCorrection);
        var desiredHeading = target.transform.position - transform.position;
        var currentHeading = transform.forward;
        var headingError = Vector3.Cross(currentHeading, desiredHeading);
        var headingCorrection = headingController.Update(headingError, Time.deltaTime);
        plane.AddTorque(headingCorrection);


    }

}
