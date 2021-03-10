using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing_Control : MonoBehaviour
{
    public static bool Grounded;
    [HideInInspector]
    public static int Jumps;
    [HideInInspector]
	public static int Jumpcap = 1;

    void Start(){
        Jumps = Jumpcap;
    }
	void OnTriggerStay(){
		Grounded = true;
        if (Jumps < Jumpcap)
		    Jumps = Jumpcap;
	}
	void OnTriggerExit(){
		Grounded = false;
		if (Jumps == Jumpcap)
		    Invoke ("JumpDeduct", 0.1f);
	}
	void JumpDeduct(){
		Jumps--;
	}
}
