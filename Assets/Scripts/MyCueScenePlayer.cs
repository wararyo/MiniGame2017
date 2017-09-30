using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wararyo.EclairCueMaker;

public class MyCueScenePlayer : CueScenePlayer {

	public CueEvent_MCBaloon MCBaloon;
	public bool controllable = true;

	protected override bool OnInvoking ()
	{
		if (MCBaloon == null)
			return true;
		
		if (MCBaloon.isAnimating) {//MCが喋ってる途中だったらスキップするやつ
			MCBaloon.Skip ();
			return false;
		} else {
			return true;
		}
	}

	protected override void Update ()
	{
		if (Input.GetButtonDown ("4") && controllable)
			Invoke ();
		
		base.Update ();
	}
}
