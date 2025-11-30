namespace Seven.Boundless.Utility;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

public static class NodeExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsEnabled(this Node node) => node.ProcessMode == Node.ProcessModeEnum.Inherit || node.ProcessMode == Node.ProcessModeEnum.Always;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Enabled<T>(this T node, Node.ProcessModeEnum mode = Node.ProcessModeEnum.Inherit) where T : Node {
		if (node.ProcessMode == Node.ProcessModeEnum.Disabled) {
			node.ProcessMode = mode;
		}
		return node;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Disabled<T>(this T node) where T : Node {
		node.ProcessMode = Node.ProcessModeEnum.Disabled;
		return node;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SetEnabled<T>(this T node, bool enabled, Node.ProcessModeEnum mode = Node.ProcessModeEnum.Inherit) where T : Node {
		if (enabled) return node.Enabled(mode);
		else return node.Disabled();
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ToggleEnabled<T>(this T node) where T : Node {
		if (node.IsEnabled()) return node.Disabled();
		else return node.Enabled();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T RenameTo<T>(this T node, StringName name) where T : Node {
		node.Name = name;
		return node;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ReparentTo<T>(this T child, Node parent, bool forceReadableName = false) where T : Node {
		parent.AddChild(child, forceReadableName);
		return child;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SafeReparentTo<T>(this T child, Node? newParent, bool keepGlobalTransform = true) where T : Node {
		if (child.GetParent() == newParent) return child;

		if (!child.IsInsideTree()) {
			child.Unparent();
		}

		if (child.GetParent() is null) {
			newParent?.AddChild(child);
		}
		else if (newParent is not null) {
			child.Reparent(newParent, keepGlobalTransform);
		}

		return child;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SafeReparentRecursive<T>(this T child, Node? newParent, bool keepGlobalTransform = true) where T : Node {
		child.SafeReparentTo(newParent, keepGlobalTransform);

		if (!Engine.IsEditorHint()) return child;
		if (child.Owner == newParent?.Owner) return child;

		ReownRecursive(child, newParent?.Owner ?? newParent);

		static void ReownRecursive(Node childNode, Node? newOwner) {
			childNode.Reown(newOwner);
			foreach (Node child in childNode.GetChildren()) {
				ReownRecursive(child, newOwner);
			}
		}

		return child;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SetSiblingIndex<T>(this T node, Node parent, int index) where T : Node {
		parent.MoveChild(node, index);
		return node;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unparent<T>(this T child) where T : Node {
		child.Owner = null;
		child.GetParent()?.RemoveChild(child);
		return child;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void UnparentAndQueueFree(this Node obj) {
		obj.QueueFree();
		obj.GetParent()?.RemoveChild(obj);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ReparentToSceneInstance<T>(this T child, Node parent, bool forceReadableName = false) where T : Node {
		parent.AddSceneInstanceChild(child, forceReadableName);
		return child;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AddSceneInstanceChild<T>(this T parent, Node child, bool forceReadableName = false) where T : Node {
		parent.AddChild(child, forceReadableName);
		child.Owner = parent.SceneFilePath.Length != 0 ? parent : parent.Owner ?? parent;
		return parent;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Reown<T>(this T node, Node? newOwner) where T : Node {
		node.Owner = newOwner;
		return node;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ShareOwnerWith<T>(this T node, Node? newParent) where T : Node =>
		node.Reown(newParent?.Owner ?? newParent);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetNodeByTypeName<T>(this Node obj) where T : Node {
		return obj.GetNodeOrNull<T>(typeof(T).Name);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryGetNode<T>(this Node obj, NodePath nodePath, out T node) where T : class {
		node = default!;
		if (!obj.HasNode(nodePath))
			return false;

		if (obj.GetNodeOrNull(nodePath) is T tNode) {
			node = tNode;
			return true;
		}

		return false;
	}

	public static void ReconnectSignal<T>(this T node, StringName signalName, Callable method, GodotObject.ConnectFlags flags) where T : Node {
		if (node is null) return;

		if (node.IsConnected(signalName, method)) {
			node.Disconnect(signalName, method);
		}

		node?.Connect(signalName, method, (uint)flags);
	}

	public static void SwapSignalEmitter<T>(ref T? emitter, T? newEmitter, StringName signalName, Callable method, GodotObject.ConnectFlags flags = 0) where T : Node {
		if (emitter is not null && emitter.IsConnected(signalName, method)) {
			emitter.Disconnect(signalName, method);
		}

		emitter = newEmitter;

		if (emitter is not null && !emitter.IsConnected(signalName, method)) { // IsConnected() sometimes does not work, not my fault https://github.com/godotengine/godot/issues/76690


			emitter.Connect(signalName, method, (uint)flags);
		}
	}

	public static void PropagateActionToChildren<T>(this Node parent, Action<T>? action, bool parentFirst = false) {
		if (parentFirst && parent is T tParent1) {
			action?.Invoke(tParent1);
		}

		foreach (T child in parent.GetChildren().OfType<T>()) {
			action?.Invoke(child);
		}

		if (!parentFirst && parent is T tParent2) {
			action?.Invoke(tParent2);
		}
	}

	public static void PropagateAction<T>(this Node parent, Action<T>? action, bool parentFirst = false) where T : class {
		T? tParent = parent as T;
		if (parentFirst && tParent is not null) {
			action?.Invoke(tParent);
		}

		foreach (Node child in parent.GetChildren()) {
			child.PropagateAction(action, parentFirst);
		}

		if (!parentFirst && tParent is not null) {
			action?.Invoke(tParent);
		}
	}


	public static void MakeLocal(this Node node, Node owner) {
		node.SceneFilePath = string.Empty;
		node.Owner = owner;
		foreach (Node childNode in node.GetChildren()) {
			MakeLocal(childNode, owner);
		}
	}
}

public static class Node3DExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Visible<T>(this T node) where T : Node3D {
		node.Visible = true;
		return node;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Invisible<T>(this T node) where T : Node3D {
		node.Visible = false;
		return node;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SetVisible<T>(this T node, bool visible) where T : Node3D {
		if (visible) return node.Visible();
		else return node.Invisible();
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ToggleVisible<T>(this T node) where T : Node3D {
		if (node.Visible) return node.Invisible();
		else return node.Visible();
	}
}

public static class CanvasItemExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Visible<T>(this T node) where T : CanvasItem {
		node.Visible = true;
		return node;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Invisible<T>(this T node) where T : CanvasItem {
		node.Visible = false;
		return node;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SetVisible<T>(this T node, bool visible) where T : CanvasItem {
		if (visible) return node.Visible();
		else return node.Invisible();
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ToggleVisible<T>(this T node) where T : CanvasItem {
		if (node.Visible) return node.Invisible();
		else return node.Visible();
	}
}