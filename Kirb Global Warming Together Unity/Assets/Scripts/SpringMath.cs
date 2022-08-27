// Referenced from Game Math Series on https://allenchou.net/game-math-series/
// GameMaker video example https://youtu.be/omU_G5TV4YA

using UnityEngine;

public class SpringMath
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <param name="velocity"> if used on an angle, use the Mathf.DeltaAngle between current and  </param>
	/// <param name="targetValue"></param>
	/// <param name="dampingRatio">Damping of the oscillations. 0 (no damping) to 1 (no springing).</param>
	/// <param name="angularFreq">How much it bounces. Oscillations per second. More oscillations, smaller bounce. </param>
	/// <param name="timeStep">Put Time.deltaTime here</param>
	public static void Lerp(ref float value, ref float velocity, float targetValue, float dampingRatio, float angularFreq, float timeStep)
	{
		float delta, delta_x, delta_v,
			q = 1 + 2 * timeStep * dampingRatio * angularFreq,
			r = Mathf.Pow(angularFreq, 2),
			s = Mathf.Pow(timeStep, 2);

		delta = q + s * r;

		delta_x = q * value
				+ timeStep * velocity
				+ s * r * targetValue;

		delta_v = velocity + timeStep * r * (targetValue - value);

		velocity = delta_v / delta;
		value = delta_x / delta;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <param name="velocity"></param>
	/// <param name="target"></param>
	/// <param name="dampingRatio"></param>
	/// <param name="angularFreq"></param>
	/// <param name="timeStep"></param>
	public static void Lerp(ref Vector3 value, ref Vector3 velocity, Vector3 target, float dampingRatio, float angularFreq, float timeStep)
	{
		Lerp(ref value.x, ref velocity.x, target.x, dampingRatio, angularFreq, timeStep);
		Lerp(ref value.y, ref velocity.y, target.y, dampingRatio, angularFreq, timeStep);
		Lerp(ref value.z, ref velocity.z, target.z, dampingRatio, angularFreq, timeStep);
	}

	public static void LerpAngle(ref float angle, ref float velocity, float targetAngle, float dampingRatio, float angularFreq, float timeStep)
	{
		// get the shortest direction to the target angle
		targetAngle = angle - Mathf.DeltaAngle(angle, targetAngle);


		float delta, delta_x, delta_v;

		delta = (1 + 2 * timeStep * dampingRatio * angularFreq) + Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2);

		delta_x = (1 + 2 * timeStep * dampingRatio * angularFreq) * angle
				+ timeStep * velocity
				+ Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2) * targetAngle;

		delta_v = velocity + timeStep * Mathf.Pow(angularFreq, 2) * (targetAngle - angle);

		angle = delta_x / delta;
		velocity = delta_v / delta;
	}
}