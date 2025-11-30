namespace Seven.Boundless.Utility;

using System;
using System.Runtime.CompilerServices;

public static class Mathfs {
	public const float Pi = (float)Math.PI;
	public const double PiDouble = Math.PI;

	public const float Epsilon = 1E-06f;
	public const double EpsilonDouble = 1E-14;

	extension (float value) {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Clamp01() {
			return Math.Clamp(value, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEqualApprox(float other) {
			if (value == other) return true;

			float tolerance = Epsilon * Math.Abs(value);
			if (tolerance < Epsilon)
			{
				tolerance = Epsilon;
			}

			return Math.Abs(value - other) < (tolerance < Epsilon ? Epsilon : tolerance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsZeroApprox() {
			return Math.Abs(value) < Epsilon;
		}
		public float MoveToward(float to, float delta) {
			if (Math.Abs(to - value) <= delta) return to;

			return value + Math.Sign(to - value) * delta;
		}
		public float Lerp(float to, float amount) {
			return value + (to - value) * amount;
		}
		public float ClampedLerp(float to, float amount) {
			float res = value + (to - value) * amount;
			if (amount > Epsilon && value.IsEqualApprox(res)) {
				return to;
			}
			return res;
		}

		public float SmoothDamp(float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
			smoothTime = Math.Max(0.0001f, smoothTime);
			float num1 = 2.0f / smoothTime;
			float num2 = num1 * deltaTime;
			float num3 = 1.0f / (1.0f + num2 + 0.479999989271164f * num2 * num2 + 0.234999999403954f * num2 * num2 * num2);
			float num4 = value - target;
			float num5 = target;
			float max = maxSpeed * smoothTime;
			float num6 = Math.Max(-max, Math.Min(max, num4));
			target = value - num6;
			float num7 = (currentVelocity + num1 * num6) * deltaTime;
			currentVelocity = (currentVelocity - num1 * num7) * num3;
			float num8 = target + (num6 + num7) * num3;
			if ((num5 - value > 0.0f) == (num8 > num5)) {
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}
			return num8;
		}
	}

	extension (double value) {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Clamp01() {
			return Math.Clamp(value, 0, 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEqualApprox(double other) {
			if (value == other) return true;

			double tolerance = EpsilonDouble * Math.Abs(value);
			if (tolerance < EpsilonDouble)
			{
				tolerance = EpsilonDouble;
			}

			return Math.Abs(value - other) < (tolerance < EpsilonDouble ? EpsilonDouble : tolerance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsZeroApprox() {
			return Math.Abs(value) < EpsilonDouble;
		}


		public double MoveToward(double to, double delta) {
			if (Math.Abs(to - value) <= delta) return to;

			return value + Math.Sign(to - value) * delta;
		}


		public double Lerp(double to, double amount) {
			return value + (to - value) * amount;
		}
		public double ClampedLerp(double to, double amount) {
			double res = value + (to - value) * amount;
			if (amount > EpsilonDouble && value.IsEqualApprox(res)) {
				return to;
			}
			return res;
		}

		public double SmoothDamp(double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime) {
			smoothTime = Math.Max(0.0001, smoothTime);
			double num1 = 2.0 / smoothTime;
			double num2 = num1 * deltaTime;
			double num3 = 1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2);
			double num4 = value - target;
			double num5 = target;
			double max = maxSpeed * smoothTime;
			double num6 = Math.Max(-max, Math.Min(max, num4));
			target = value - num6;
			double num7 = (currentVelocity + num1 * num6) * deltaTime;
			currentVelocity = (currentVelocity - num1 * num7) * num3;
			double num8 = target + (num6 + num7) * num3;
			if ((num5 - value > 0.0) == (num8 > num5)) {
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}
			return num8;
		}
	}
}