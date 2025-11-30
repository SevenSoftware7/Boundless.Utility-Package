namespace Seven.Boundless.Utility;

using System;
using Godot;
using Godot.Collections;
using System.Runtime.InteropServices;

public static class CompositorExtensions {
	public static (uint xGroups, uint yGroups) GetGroups(Vector2I renderSize, uint range) {
		uint xGroups = (uint)renderSize.X / range + 1;
		uint yGroups = (uint)renderSize.Y / range + 1;

		return (xGroups, yGroups);
	}

	extension(RenderSceneBuffersRD sceneBuffers) {
		public (Vector2I renderSize, uint xGroups, uint yGroups) GetRenderSize(uint range = 8u) {
			Vector2I renderSize = sceneBuffers.GetInternalSize();
			if (renderSize.X == 0 && renderSize.Y == 0) {
				throw new ArgumentException("Render size is incorrect");
			}

			(uint xGroups, uint yGroups) = GetGroups(renderSize, range);

			return (renderSize, xGroups, yGroups);
		}
	}

	extension(RDUniform uniform) {
		public RDUniform SetIds(ReadOnlySpan<Rid> ids) {
			uniform._Ids = [.. ids];

			return uniform;
		}
	}

	extension(RenderingDevice renderingDevice) {
		public Rid IndexBufferCreate(ReadOnlySpan<uint> indices, uint shapeVertices = 3u) {
			if (indices.Length % shapeVertices != 0u) throw new ArgumentException($"Invalid number of values in the index buffer, there should be {shapeVertices} vertices in a shape. Total count : {indices.Length}", nameof(indices));
			return renderingDevice.IndexBufferCreate((uint)indices.Length, RenderingDevice.IndexBufferFormat.Uint32, MemoryMarshal.AsBytes(indices), false, 0L);
		}
		public Rid IndexBufferCreate(ReadOnlySpan<ushort> indices, uint shapeVertices = 3u) {
			if (indices.Length % shapeVertices != 0u) throw new ArgumentException($"Invalid number of values in the index buffer, there should be {shapeVertices} vertices in a shape. Total count : {indices.Length}", nameof(indices));
			return renderingDevice.IndexBufferCreate((uint)indices.Length, RenderingDevice.IndexBufferFormat.Uint16, MemoryMarshal.AsBytes(indices), false, 0L);
		}


		public (Rid indexBuffer, Rid indexArray) IndexArrayCreate(ReadOnlySpan<uint> indices, uint shapeVertices = 3u, uint indexOffset = 0u) {
			Rid indexBuffer = renderingDevice.IndexBufferCreate(indices, shapeVertices);
			if (!indexBuffer.IsValid) {
				throw new ArgumentException("Index Buffer is Invalid");
			}
			Rid indexArray = renderingDevice.IndexArrayCreate(indexBuffer, indexOffset, (uint)indices.Length);
			if (!indexArray.IsValid) {
				throw new ArgumentException("Index Array is Invalid");
			}

			return (indexBuffer, indexArray);
		}
		public (Rid indexBuffer, Rid indexArray) IndexArrayCreate(ReadOnlySpan<ushort> indices, uint shapeVertices = 3u, uint indexOffset = 0u) {
			Rid indexBuffer = renderingDevice.IndexBufferCreate(indices, shapeVertices);
			if (!indexBuffer.IsValid) {
				throw new ArgumentException("Index Buffer is Invalid");
			}
			Rid indexArray = renderingDevice.IndexArrayCreate(indexBuffer, indexOffset, (uint)indices.Length);
			if (!indexArray.IsValid) {
				throw new ArgumentException("Index Array is Invalid");
			}

			return (indexBuffer, indexArray);
		}

		public Rid VertexBufferCreate(ReadOnlySpan<float> vertices, uint vertexLength = 3u) {
			if (vertices.Length % vertexLength != 0) throw new ArgumentException($"Invalid number of values in the points buffer, there should be {vertexLength} float values per point. Total count : {vertices.Length}", nameof(vertices));
			var byteVertices = MemoryMarshal.AsBytes(vertices);

			return renderingDevice.VertexBufferCreate((uint)byteVertices.Length, byteVertices, 0L);
		}
		public Rid VertexBufferCreate(ReadOnlySpan<double> vertices, uint vertexLength = 3u) {
			if (vertices.Length % vertexLength != 0) throw new ArgumentException($"Invalid number of values in the points buffer, there should be {vertexLength} float values per point. Total count : {vertices.Length}", nameof(vertices));
			var byteVertices = MemoryMarshal.AsBytes(vertices);

			return renderingDevice.VertexBufferCreate((uint)byteVertices.Length, byteVertices, 0L);
		}

		public (Rid vertexBuffer, Rid vertexArray) VertexArrayCreate(ReadOnlySpan<float> points, long vertexFormat, uint vertexLength = 3u) {
			Rid vertexBuffer = renderingDevice.VertexBufferCreate(points);
			if (!vertexBuffer.IsValid) {
				throw new ArgumentException("Vertex Buffer is Invalid");
			}
			Rid vertexArray = renderingDevice.VertexArrayCreate((uint)(points.Length / vertexLength), vertexFormat, [vertexBuffer]);
			if (!vertexArray.IsValid) {
				throw new ArgumentException("Vertex Array is Invalid");
			}

			return (vertexBuffer, vertexArray);
		}
		public (Rid vertexBuffer, Rid vertexArray) VertexArrayCreate(ReadOnlySpan<double> points, long vertexFormat, uint vertexLength = 3u) {
			Rid vertexBuffer = renderingDevice.VertexBufferCreate(points);
			if (!vertexBuffer.IsValid) {
				throw new ArgumentException("Vertex Buffer is Invalid");
			}
			Rid vertexArray = renderingDevice.VertexArrayCreate((uint)(points.Length / vertexLength), vertexFormat, [vertexBuffer]);
			if (!vertexArray.IsValid) {
				throw new ArgumentException("Vertex Array is Invalid");
			}

			return (vertexBuffer, vertexArray);
		}


		public (Rid framebufferTexture, Rid framebuffer) FramebufferCreate(RDTextureFormat textureFormat, RDTextureView textureView, RenderingDevice.TextureSamples textureSamples = RenderingDevice.TextureSamples.Samples1) {
			Rid frameBufferTexture = renderingDevice.TextureCreate(textureFormat, textureView);
			if (!frameBufferTexture.IsValid) {
				throw new ArgumentException("Frame Buffer Texture is Invalid");
			}

			Array<RDAttachmentFormat> attachments = [
				new RDAttachmentFormat() {
					Format = textureFormat.Format,
					Samples = textureSamples,
					UsageFlags = (uint)textureFormat.UsageBits
				}
			];
			long frameBufferFormat = renderingDevice.FramebufferFormatCreate(attachments);
			Rid frameBuffer = renderingDevice.FramebufferCreate([frameBufferTexture], frameBufferFormat);
			if (!frameBuffer.IsValid) {
				throw new ArgumentException("Frame Buffer is Invalid");
			}

			return (frameBufferTexture, frameBuffer);
		}

		public void ComputeListBind(long computeList, Rid shaderRid, ReadOnlySpan<Rid> ids, RenderingDevice.UniformType uniformType, uint setIndex, int binding = 0) {
			RDUniform uniform = new RDUniform() {
				UniformType = uniformType,
				Binding = binding,
			}.SetIds(ids);

			renderingDevice.ComputeListBindUniform(computeList, uniform, shaderRid, setIndex);
		}

		public void ComputeListBindImage(long computeList, Rid shaderRid, Rid image, uint setIndex, int binding = 0) =>
			renderingDevice.ComputeListBind(computeList, shaderRid, [image], RenderingDevice.UniformType.Image, setIndex, binding);

		public void ComputeListBindSampler(long computeList, Rid shaderRid, Rid image, Rid sampler, uint setIndex, int binding = 0) =>
			renderingDevice.ComputeListBind(computeList, shaderRid, [sampler, image], RenderingDevice.UniformType.SamplerWithTexture, setIndex, binding);
		public void ComputeListBindColor(long computeList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, uint setIndex, int binding = 0) =>
			renderingDevice.ComputeListBindImage(computeList, shaderRid, sceneBuffers.GetColorLayer(view), setIndex, binding);

		public void ComputeListBindDepth(long computeList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, Rid sampler, uint setIndex, int binding = 0) =>
			renderingDevice.ComputeListBindSampler(computeList, shaderRid, sceneBuffers.GetDepthLayer(view), sampler, setIndex, binding);

		public void ComputeListBindStorageBuffer(long computeList, Rid shaderRid, Rid buffer, uint setIndex, int binding = 0) =>
			renderingDevice.ComputeListBind(computeList, shaderRid, [buffer], RenderingDevice.UniformType.StorageBuffer, setIndex, binding);

		public void ComputeListBindUniform(long computeList, RDUniform uniform, Rid shaderRid, uint setIndex) {
			Rid set = UniformSetCacheRD.GetCache(shaderRid, setIndex, [uniform]);

			renderingDevice.ComputeListBindUniformSet(computeList, set, setIndex);
		}


		public void DrawListBind(long drawList, Rid shaderRid, ReadOnlySpan<Rid> ids, RenderingDevice.UniformType uniformType, uint setIndex, int binding = 0) {
			RDUniform uniform = new RDUniform() {
				UniformType = uniformType,
				Binding = binding,
			}.SetIds(ids);

			renderingDevice.DrawListBindUniform(drawList, uniform, shaderRid, setIndex);
		}

		public void DrawListBindImage(long drawList, Rid shaderRid, Rid image, uint setIndex, int binding = 0) =>
			renderingDevice.DrawListBind(drawList, shaderRid, [image], RenderingDevice.UniformType.Image, setIndex, binding);
		public void DrawListBindSampler(long drawList, Rid shaderRid, Rid image, Rid sampler, uint setIndex, int binding = 0) =>
			renderingDevice.DrawListBind(drawList, shaderRid, [sampler, image], RenderingDevice.UniformType.SamplerWithTexture, setIndex, binding);

		public void DrawListBindColor(long drawList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, uint setIndex, int binding = 0) =>
			renderingDevice.DrawListBindImage(drawList, shaderRid, sceneBuffers.GetColorLayer(view), setIndex, binding);

		public void DrawListBindDepth(long drawList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, Rid sampler, uint setIndex, int binding = 0) =>
			renderingDevice.DrawListBindSampler(drawList, shaderRid, sceneBuffers.GetDepthLayer(view), sampler, setIndex, binding);
		public void DrawListBindStorageBuffer(long drawList, Rid shaderRid, Rid buffer, uint setIndex, int binding = 0) =>
			renderingDevice.DrawListBind(drawList, shaderRid, [buffer], RenderingDevice.UniformType.StorageBuffer, setIndex, binding);


		public void DrawListBindUniform(long drawList, RDUniform uniform, Rid shaderRid, uint setIndex) {
			Rid set = UniformSetCacheRD.GetCache(shaderRid, setIndex, [uniform]);

			renderingDevice.DrawListBindUniformSet(drawList, set, setIndex);
		}
	}
}