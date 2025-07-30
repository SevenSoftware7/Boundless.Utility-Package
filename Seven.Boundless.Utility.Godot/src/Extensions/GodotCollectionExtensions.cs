namespace Seven.Boundless.Utility;

using System.Collections.Generic;
using Godot;
using Godot.Collections;

public static class GodotCollectionExtensions {
	public static void RemoveRange<[MustBeVariant] T>(this Array<T> collection, IEnumerable<T> toRemove) {
		foreach (T item in toRemove) {
			collection.Remove(item);
		}
	}
}