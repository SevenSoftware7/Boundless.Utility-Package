namespace Seven.Boundless.Utility;

using Godot;

public static class Transform3Ds {
	extension (Transform3D transform) {
		public Vector3 Right => -transform.Basis.X;
		public Vector3 Up => transform.Basis.Y;
		public Vector3 Forward => -transform.Basis.Z;
		public Vector3 Left => transform.Basis.X;
		public Vector3 Down => -transform.Basis.Y;
		public Vector3 Back => transform.Basis.Z;
	}
}