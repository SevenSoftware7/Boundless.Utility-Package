namespace Seven.Boundless.Utility;

using Godot;

public static class PhysicsExtensions {

	public static Aabb CreateOrientedBoxAabb(Vector3 center, Vector3 size, Basis basis) {
		Aabb aabb = new();
		Vector3 half = size * 0.5f;
		Basis absoluteBasis = new(
			basis.Column0.Abs(),
			basis.Column1.Abs(),
			basis.Column2.Abs()
		);
		Vector3 worldHalf = absoluteBasis * half;

		aabb.Expand(center + new Vector3(worldHalf.X, worldHalf.Y, worldHalf.Z));
		aabb.Expand(center + new Vector3(worldHalf.X, worldHalf.Y, -worldHalf.Z));
		aabb.Expand(center + new Vector3(worldHalf.X, -worldHalf.Y, worldHalf.Z));
		aabb.Expand(center + new Vector3(worldHalf.X, -worldHalf.Y, -worldHalf.Z));
		aabb.Expand(center + new Vector3(-worldHalf.X, worldHalf.Y, worldHalf.Z));
		aabb.Expand(center + new Vector3(-worldHalf.X, worldHalf.Y, -worldHalf.Z));
		aabb.Expand(center + new Vector3(-worldHalf.X, -worldHalf.Y, worldHalf.Z));
		aabb.Expand(center + new Vector3(-worldHalf.X, -worldHalf.Y, -worldHalf.Z));

		return aabb;
	}

	public static Aabb ComputeAabb(Shape3D shape, Transform3D? transform = null) {
		Aabb result = new();
		Transform3D transformVal = transform ?? Transform3D.Identity;

		if (shape is ConcavePolygonShape3D concaveShape) {
			foreach (Vector3 face in concaveShape.GetFaces()) {
				result.Expand(transformVal * face);
			}
		}
		else if (shape is ConvexPolygonShape3D convexShape) {
			foreach (Vector3 point in convexShape.GetPoints()) {
				result.Expand(transformVal * point);
			}
		}
		else if (shape is BoxShape3D boxShape) {
			Vector3 size = boxShape.Size;
			Basis basis = transformVal.Basis;
			Vector3 origin = transformVal.Origin;

			Aabb obb = CreateOrientedBoxAabb(origin, size, basis);
			result = result.Merge(obb);
		}
		else if (shape is SphereShape3D sphereShape) {
			float radius = sphereShape.Radius;
			Vector3 center = transformVal.Origin;
			Vector3 unit = new(radius, radius, radius);
			result.Expand(center + unit);
			result.Expand(center - unit);
		}
		else if (shape is CapsuleShape3D capsuleShape) {
			float radius = capsuleShape.Radius;
			Basis basis = transformVal.Basis;
			Vector3 size = new(radius * 2, capsuleShape.Height + radius, radius * 2);
			Vector3 center = transformVal.Origin;

			Aabb obb = CreateOrientedBoxAabb(center, size, basis);
			result = result.Merge(obb);
		}
		else if (shape is CylinderShape3D cylinderShape) {
			float radius = cylinderShape.Radius;
			Vector3 size = new(radius * 2, cylinderShape.Height, radius * 2);
			Basis basis = transformVal.Basis;
			Vector3 center = transformVal.Origin;

			Aabb obb = CreateOrientedBoxAabb(center, size, basis);
			result = result.Merge(obb);
		}
		else if (shape is HeightMapShape3D heightMapShape) {
			float height = heightMapShape.GetMaxHeight() - heightMapShape.GetMinHeight();
			Vector3 size = new (heightMapShape.MapWidth, height, heightMapShape.MapDepth);
			Basis basis = transformVal.Basis;
			Vector3 origin = transformVal.Origin;

			Aabb obb = CreateOrientedBoxAabb(origin, size, basis);
			result = result.Merge(obb);
		}

		return result;
	}

	public static Aabb ComputeAabb(this CollisionObject3D collision) {
		Aabb aabb = new(collision.GlobalPosition, Vector3.Zero);
		foreach (int ownerId in collision.GetShapeOwners()) {
			uint ownerIdUint = (uint)ownerId;
			int shapeCount = collision.ShapeOwnerGetShapeCount(ownerIdUint);
			for (int i = 0; i < shapeCount; i++) {
				Transform3D transform = collision.ShapeOwnerGetTransform(ownerIdUint);
				Shape3D? shape = collision.ShapeOwnerGetShape(ownerIdUint, i);
				aabb = aabb.Merge(ComputeAabb(shape, transform));

			}
		}
		return aabb;
	}

	public static bool GetSurfaceInDirection(this Area3D area, Vector3 location, Vector3 direction, out Collisions.IntersectRay3DResult result) {
		if (direction == Vector3.Zero) direction = Vector3.Up;
		direction = direction.IsNormalized() ? direction : direction.Normalized();

		Aabb aabb = area.ComputeAabb();

		float depth = Mathf.Abs((aabb.Size * 0.5f).Dot(direction));

		Vector3 inside = location + (area.GlobalPosition - location).SlideOnFace(-direction).Project(-direction);
		Vector3 outside = location + direction * (depth + Mathf.Epsilon);

		return area.GetWorld3D().IntersectRay3DExclusive(area, outside, inside, out result, area.CollisionLayer, collideWithBodies: false);
	}
}