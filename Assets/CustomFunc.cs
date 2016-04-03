using UnityEngine;
using System.Collections;

public static class CustomFunc {
	public static float Map(float value, 
		float istart, 
		float istop, 
		float ostart, 
		float ostop) {
		return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
	}
}
