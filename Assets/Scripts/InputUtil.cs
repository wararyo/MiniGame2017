using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtil : MonoBehaviour{

	private static float previousHorizontal;
	private static float previousVertical;
	private static float nowHorizontal;
	private static float nowVertical;

	static string horizontal = "Horizontal";
	static string vertical = "Vertical";

	static float threshold = 0.1f;

	public static int GetHorizontalDown(){
		if (Mathf.Abs(previousHorizontal) < threshold) {
			if (nowHorizontal > threshold)
				return 1;
			else if (nowHorizontal < -threshold)
				return -1;
		}
		return 0;
	}

	public static int GetVerticalDown(){
		if (Mathf.Abs(previousVertical) < threshold) {
			if (nowVertical > threshold)
				return 1;
			else if (nowVertical < -threshold)
				return -1;
		}
		return 0;
	}

	void Update(){
		previousVertical = nowVertical;
		previousHorizontal = nowHorizontal;
		nowVertical = Input.GetAxisRaw (vertical);
		nowHorizontal = Input.GetAxisRaw (horizontal);
	}
}
