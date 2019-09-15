#pragma strict

private var motor : CharacterMotor;
private var fly : FlyCam;
private var mouselook : MouseLook;



function Start () {

}

function Update () {

	if(Input.GetMouseButtonDown(1)){
		motor = GetComponent(CharacterMotor);
		fly = GetComponent(FlyCam);
		mouselook = GetComponent(MouseLook);
		
		motor.enabled = !motor.enabled;
		fly.enabled = !fly.enabled;
		
		if (motor.enabled == false){ mouselook.enabled = false;	}
		else{mouselook.enabled = true;}
		
		
	}
}
