namespace Seven.Boundless.Utility;

using Godot;

public static class Vector2s {
	extension (Vector2 vector) {
		public Vector2 SafeSlerp(Vector2 to, float weight) {
			if (vector.IsEqualApprox(to)) {
				return vector;
			}

			// Avoid error on both vectors being inverses of each other, breaking a Cross Product operation in the Slerp method

			if ((vector + to) == Vector2.Zero) {
				return vector.Lerp(to, weight);
			}

			return vector.Slerp(to, weight);
		}

		public Vector2 ClampMagnitude(float maxLength) {
			if (vector.LengthSquared() > maxLength * maxLength) {
				return vector.Normalized() * maxLength;
			}
			return vector;
		}

		public Vector2 ClampedLerp(Vector2 to, float amount) {
			Vector2 res = vector + (to - vector) * amount;
			if (amount > Mathf.Epsilon && vector.IsEqualApprox(res)) {
				return to;
			}
			return res;
		}


		/// <summary>
		/// Returns the target vector after being "flattened" against a surface defined by the given normal.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public Vector2 SlideOnFace(Vector2 normal) =>
			vector + normal * Mathf.Max(vector.Project(-normal).Dot(-normal), 0);

		public Vector2 To(Vector2 to)
			=> new(to.X - vector.X, to.Y - vector.Y);
	}
}