// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VIVE.OpenXR.CompositionLayer
{
	public struct XrSwapchain : IEquatable<ulong>
	{
		private readonly ulong value;

		public XrSwapchain(ulong u)
		{
			value = u;
		}

		public static implicit operator ulong(XrSwapchain xrBool)
		{
			return xrBool.value;
		}
		public static implicit operator XrSwapchain(ulong u)
		{
			return new XrSwapchain(u);
		}

		public bool Equals(XrSwapchain other)
		{
			return value == other.value;
		}
		public bool Equals(ulong other)
		{
			return value == other;
		}
		public override bool Equals(object obj)
		{
			return obj is XrSwapchain && Equals((XrSwapchain)obj);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(XrSwapchain a, XrSwapchain b) { return a.Equals(b); }
		public static bool operator !=(XrSwapchain a, XrSwapchain b) { return !a.Equals(b); }
		public static bool operator >=(XrSwapchain a, XrSwapchain b) { return a.value >= b.value; }
		public static bool operator <=(XrSwapchain a, XrSwapchain b) { return a.value <= b.value; }
		public static bool operator >(XrSwapchain a, XrSwapchain b) { return a.value > b.value; }
		public static bool operator <(XrSwapchain a, XrSwapchain b) { return a.value < b.value; }
		public static XrSwapchain operator +(XrSwapchain a, XrSwapchain b) { return a.value + b.value; }
		public static XrSwapchain operator -(XrSwapchain a, XrSwapchain b) { return a.value - b.value; }
		public static XrSwapchain operator *(XrSwapchain a, XrSwapchain b) { return a.value * b.value; }
		public static XrSwapchain operator /(XrSwapchain a, XrSwapchain b)
		{
			if (b.value == 0)
			{
				throw new DivideByZeroException();
			}
			return a.value / b.value;
		}

	}
	public struct XrCompositionLayerFlags : IEquatable<UInt64>
	{
		private readonly UInt64 value;

		public XrCompositionLayerFlags(UInt64 u)
		{
			value = u;
		}

		public static implicit operator UInt64(XrCompositionLayerFlags xrBool)
		{
			return xrBool.value;
		}
		public static implicit operator XrCompositionLayerFlags(UInt64 u)
		{
			return new XrCompositionLayerFlags(u);
		}

		public bool Equals(XrCompositionLayerFlags other)
		{
			return value == other.value;
		}
		public bool Equals(UInt64 other)
		{
			return value == other;
		}
		public override bool Equals(object obj)
		{
			return obj is XrCompositionLayerFlags && Equals((XrCompositionLayerFlags)obj);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.Equals(b); }
		public static bool operator !=(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return !a.Equals(b); }
		public static bool operator >=(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value >= b.value; }
		public static bool operator <=(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value <= b.value; }
		public static bool operator >(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value > b.value; }
		public static bool operator <(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value < b.value; }
		public static XrCompositionLayerFlags operator +(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value + b.value; }
		public static XrCompositionLayerFlags operator -(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value - b.value; }
		public static XrCompositionLayerFlags operator *(XrCompositionLayerFlags a, XrCompositionLayerFlags b) { return a.value * b.value; }
		public static XrCompositionLayerFlags operator /(XrCompositionLayerFlags a, XrCompositionLayerFlags b)
		{
			if (b.value == 0)
			{
				throw new DivideByZeroException();
			}
			return a.value / b.value;
		}
	}

	public struct XrSwapchainCreateFlags : IEquatable<UInt64>
	{
		private readonly UInt64 value;

		public XrSwapchainCreateFlags(UInt64 u)
		{
			value = u;
		}

		public static implicit operator UInt64(XrSwapchainCreateFlags xrBool)
		{
			return xrBool.value;
		}
		public static implicit operator XrSwapchainCreateFlags(UInt64 u)
		{
			return new XrSwapchainCreateFlags(u);
		}

		public bool Equals(XrSwapchainCreateFlags other)
		{
			return value == other.value;
		}
		public bool Equals(UInt64 other)
		{
			return value == other;
		}
		public override bool Equals(object obj)
		{
			return obj is XrSwapchainCreateFlags && Equals((XrSwapchainCreateFlags)obj);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.Equals(b); }
		public static bool operator !=(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return !a.Equals(b); }
		public static bool operator >=(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value >= b.value; }
		public static bool operator <=(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value <= b.value; }
		public static bool operator >(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value > b.value; }
		public static bool operator <(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value < b.value; }
		public static XrSwapchainCreateFlags operator +(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value + b.value; }
		public static XrSwapchainCreateFlags operator -(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value - b.value; }
		public static XrSwapchainCreateFlags operator *(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b) { return a.value * b.value; }
		public static XrSwapchainCreateFlags operator /(XrSwapchainCreateFlags a, XrSwapchainCreateFlags b)
		{
			if (b.value == 0)
			{
				throw new DivideByZeroException();
			}
			return a.value / b.value;
		}
	}

	public struct XrSwapchainUsageFlags : IEquatable<UInt64>
	{
		private readonly UInt64 value;

		public XrSwapchainUsageFlags(UInt64 u)
		{
			value = u;
		}

		public static implicit operator UInt64(XrSwapchainUsageFlags xrBool)
		{
			return xrBool.value;
		}
		public static implicit operator XrSwapchainUsageFlags(UInt64 u)
		{
			return new XrSwapchainUsageFlags(u);
		}

		public bool Equals(XrSwapchainUsageFlags other)
		{
			return value == other.value;
		}
		public bool Equals(UInt64 other)
		{
			return value == other;
		}
		public override bool Equals(object obj)
		{
			return obj is XrSwapchainUsageFlags && Equals((XrSwapchainUsageFlags)obj);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.Equals(b); }
		public static bool operator !=(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return !a.Equals(b); }
		public static bool operator >=(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value >= b.value; }
		public static bool operator <=(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value <= b.value; }
		public static bool operator >(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value > b.value; }
		public static bool operator <(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value < b.value; }
		public static XrSwapchainUsageFlags operator +(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value + b.value; }
		public static XrSwapchainUsageFlags operator -(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value - b.value; }
		public static XrSwapchainUsageFlags operator *(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b) { return a.value * b.value; }
		public static XrSwapchainUsageFlags operator /(XrSwapchainUsageFlags a, XrSwapchainUsageFlags b)
		{
			if (b.value == 0)
			{
				throw new DivideByZeroException();
			}
			return a.value / b.value;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct XrCompositionLayerQuad
	{
		public XrStructureType type;
		public IntPtr next;
		public XrCompositionLayerFlags layerFlags;
		public XrSpace space;
		public XrEyeVisibility eyeVisibility;
		public XrSwapchainSubImage subImage;
		public XrPosef pose;
		public XrExtent2Df size;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct XrCompositionLayerCylinderKHR
	{
		public XrStructureType type;
		public IntPtr next;
		public XrCompositionLayerFlags layerFlags;
		public XrSpace space;
		public XrEyeVisibility eyeVisibility;
		public XrSwapchainSubImage subImage;
		public XrPosef pose;
		public float radius;
		public float centralAngle;
		public float aspectRatio;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct XrSwapchainSubImage
	{
		public XrSwapchain swapchain;
		public XrRect2Di imageRect;
		public uint imageArrayIndex;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct XrCompositionLayerColorScaleBiasKHR
	{
		public XrStructureType type;
		public IntPtr next;
		public XrColor4f colorScale;
		public XrColor4f colorBias;
	}
	public enum GraphicsAPI
	{
		GLES3	= 1,
		Vulkan	= 2
	}
	public enum LayerType
	{
		///<summary> Overlays are composition layers rendered after the projection layer </summary>
		Overlay = 1,
		///<summary> Underlays are composition layers rendered before the projection layer </summary>
		Underlay = 2
	}

	/// <summary>
	/// An application can create an <see cref="XrPassthroughHTC">XrPassthroughHTC</see> handle by calling <see cref="ViveCompositionLayerHelper.xrCreatePassthroughHTC">xrCreatePassthroughHTC</see>. The returned passthrough handle can be subsequently used in API calls.
	/// </summary>
	public struct XrPassthroughHTC : IEquatable<UInt64>
	{
		private readonly UInt64 value;

		public XrPassthroughHTC(UInt64 u)
		{
			value = u;
		}

		public static implicit operator UInt64(XrPassthroughHTC equatable)
		{
			return equatable.value;
		}
		public static implicit operator XrPassthroughHTC(UInt64 u)
		{
			return new XrPassthroughHTC(u);
		}

		public bool Equals(XrPassthroughHTC other)
		{
			return value == other.value;
		}
		public bool Equals(UInt64 other)
		{
			return value == other;
		}
		public override bool Equals(object obj)
		{
			return obj is XrPassthroughHTC && Equals((XrPassthroughHTC)obj);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(XrPassthroughHTC a, XrPassthroughHTC b) { return a.Equals(b); }
		public static bool operator !=(XrPassthroughHTC a, XrPassthroughHTC b) { return !a.Equals(b); }
		public static bool operator >=(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value >= b.value; }
		public static bool operator <=(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value <= b.value; }
		public static bool operator >(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value > b.value; }
		public static bool operator <(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value < b.value; }
		public static XrPassthroughHTC operator +(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value + b.value; }
		public static XrPassthroughHTC operator -(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value - b.value; }
		public static XrPassthroughHTC operator *(XrPassthroughHTC a, XrPassthroughHTC b) { return a.value * b.value; }
		public static XrPassthroughHTC operator /(XrPassthroughHTC a, XrPassthroughHTC b)
		{
			if (b.value == 0)
			{
				throw new DivideByZeroException();
			}
			return a.value / b.value;
		}

	}

	/// <summary>
	/// The XrPassthroughFormHTC enumeration identifies the form of the passthrough, presenting the passthrough fill the full screen or project onto a specified mesh.
	/// </summary>
	public enum XrPassthroughFormHTC
	{
		/// <summary>
		/// Presents the passthrough with full of the entire screen..
		/// </summary>
		XR_PASSTHROUGH_FORM_PLANAR_HTC = 0,
		/// <summary>
		/// Presents the passthrough projecting onto a custom mesh.
		/// </summary>
		XR_PASSTHROUGH_FORM_PROJECTED_HTC = 1,
	};

	/// <summary>
	/// The XrPassthroughCreateInfoHTC structure describes the information to create an <see cref="XrPassthroughCreateInfoHTC">XrPassthroughCreateInfoHTC</see> handle.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct XrPassthroughCreateInfoHTC
	{
		/// <summary>
		/// The <see cref="XrStructureType">XrStructureType</see> of this structure.
		/// </summary>
		public XrStructureType type;
		/// <summary>
		/// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
		/// </summary>
		public IntPtr next;
		/// <summary>
		/// The form specifies the form of passthrough.
		/// </summary>
		public XrPassthroughFormHTC form;

		/// <param name="in_type">The <see cref="XrStructureType">XrStructureType</see> of this structure.</param>
		/// <param name="in_next">NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.</param>
		/// <param name="in_facialTrackingType">An XrFacialTrackingTypeHTC which describes which type of facial tracking should be used for this handle.</param>
		public XrPassthroughCreateInfoHTC(XrStructureType in_type, IntPtr in_next, XrPassthroughFormHTC in_form)
		{
			type = in_type;
			next = in_next;
			form = in_form;
		}
	};
	/// <summary>
	/// The XrCompositionLayerBaseHeader structure is not intended to be directly used, but forms a basis for defining current and future structures containing composition layer information. The XrFrameEndInfo structure contains an array of pointers to these polymorphic header structures. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct XrCompositionLayerBaseHeader
	{
		/// <summary>
		/// The XrStructureType of this structure.
		/// </summary>
		public XrStructureType type;
		/// <summary>
		/// Next is NULL or a pointer to the next structure in a structure chain, such as XrPassthroughMeshTransformInfoHTC.
		/// </summary>
		public IntPtr next;
		/// <summary>
		/// A bitmask of XrCompositionLayerFlagBits describing flags to apply to the layer.
		/// </summary>
		public XrCompositionLayerFlags layerFlags;
		/// <summary>
		/// The XrSpace in which the layer will be kept stable over time.
		/// </summary>
		public XrSpace space;
	};
	/// <summary>
	/// The application can specify the XrPassthroughColorHTC to adjust the alpha value of the passthrough. The range is between 0.0f and 1.0f, 1.0f means opaque.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct XrPassthroughColorHTC
	{
		/// <summary>
		/// The XrStructureType of this structure.
		/// </summary>
		public XrStructureType type;
		/// <summary>
		/// Next is NULL or a pointer to the next structure in a structure chain, such as XrPassthroughMeshTransformInfoHTC.
		/// </summary>
		public IntPtr next;
		/// <summary>
		/// The alpha value of the passthrough in the range [0, 1].
		/// </summary>
		public float alpha;
		public XrPassthroughColorHTC(XrStructureType in_type, IntPtr in_next, float in_alpha)
        {
			type = in_type;
			next = in_next;
			alpha = in_alpha;
		}
	};
	/// <summary>
	/// A pointer to XrCompositionLayerPassthroughHTC may be submitted in xrEndFrame as a pointer to the base structure XrCompositionLayerBaseHeader, in the desired layer order, to request the runtime to composite a passthrough layer into the final frame output.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct XrCompositionLayerPassthroughHTC
	{
		/// <summary>
		/// The XrStructureType of this structure.
		/// </summary>
		public XrStructureType type;
		/// <summary>
		/// Next is NULL or a pointer to the next structure in a structure chain, such as XrPassthroughMeshTransformInfoHTC.
		/// </summary>
		public IntPtr next;
		/// <summary>
		/// A bitmask of XrCompositionLayerFlagBits describing flags to apply to the layer.
		/// </summary>
		public XrCompositionLayerFlags layerFlags;
		/// <summary>
		/// The XrSpace that specifies the layer’s space - must be XR_NULL_HANDLE.
		/// </summary>
		public XrSpace space;
		/// <summary>
		/// The XrPassthroughHTC previously created by xrCreatePassthroughHTC.
		/// </summary>
		public XrPassthroughHTC passthrough;
		/// <summary>
		/// The XrPassthroughColorHTC describing the color information with the alpha value of the passthrough layer.
		/// </summary>
		public XrPassthroughColorHTC color;
		public XrCompositionLayerPassthroughHTC(XrStructureType in_type, IntPtr in_next, XrCompositionLayerFlags in_layerFlags,
			XrSpace in_space, XrPassthroughHTC in_passthrough, XrPassthroughColorHTC in_color)
		{
			type = in_type;
			next = in_next;
			layerFlags = in_layerFlags;
			space = in_space;
			passthrough = in_passthrough;
			color = in_color;
		}
	};

	/// <summary>
	/// The XrPassthroughMeshTransformInfoHTC structure describes the mesh and transformation.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct XrPassthroughMeshTransformInfoHTC
	{
		/// <summary>
		/// The XrStructureType of this structure.
		/// </summary>
		public XrStructureType type;
		/// <summary>
		/// Next is NULL or a pointer to the next structure in a structure chain.
		/// </summary>
		public IntPtr next;
		/// <summary>
		/// The count of vertices array in the mesh.
		/// </summary>
		public UInt32 vertexCount;
		/// <summary>
		/// An array of XrVector3f. The size of the array must be equal to vertexCount.
		/// </summary>
		public XrVector3f[] vertices;
		/// <summary>
		/// The count of indices array in the mesh.
		/// </summary>
		public UInt32 indexCount;
		/// <summary>
		/// An array of triangle indices. The size of the array must be equal to indexCount.
		/// </summary>
		public UInt32[] indices;
		/// <summary>
		/// The XrSpace that defines the projected passthrough's base space for transformations.
		/// </summary>
		public XrSpace baseSpace;
		/// <summary>
		/// The XrTime that defines the time at which the transform is applied.
		/// </summary>
		public XrTime time;
		/// <summary>
		/// The XrPosef that defines the pose of the mesh
		/// </summary>
		public XrPosef pose;
		/// <summary>
		/// The XrVector3f that defines the scale of the mesh
		/// </summary>
		public XrVector3f scale;
		public XrPassthroughMeshTransformInfoHTC(XrStructureType in_type, IntPtr in_next, UInt32 in_vertexCount,
			XrVector3f[] in_vertices, UInt32 in_indexCount, UInt32[] in_indices, XrSpace in_baseSpace, XrTime in_time,
			XrPosef in_pose, XrVector3f in_scale)
		{
			type = in_type;
			next = in_next;
			vertexCount = in_vertexCount;
			vertices = in_vertices;
			indexCount = in_indexCount;
			indices = in_indices;
			baseSpace = in_baseSpace;
			time = in_time;
			pose = in_pose;
			scale = in_scale;
		}
	};

	public static class ViveCompositionLayerHelper
	{
		/// <summary>
		/// The delegate function of <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreatePassthroughHTC">xrCreatePassthroughHTC</see>.
		/// </summary>
		/// <param name="session">An <see cref="XrSession">XrSession</see>  in which the passthrough will be active.</param>
		/// <param name="createInfo">createInfo is a pointer to an <see cref="XrPassthroughCreateInfoHTC">XrPassthroughCreateInfoHTC</see> structure containing information about how to create the passthrough.</param>
		/// <param name="passthrough">passthrough is a pointer to a handle in which the created <see cref="XrPassthroughHTC">XrPassthroughHTC</see> is returned.</param>
		/// <returns>XR_SUCCESS for success.</returns>
		public delegate XrResult xrCreatePassthroughHTCDelegate(
			XrSession session,
			XrPassthroughCreateInfoHTC createInfo,
			out XrPassthroughHTC passthrough);

		/// <summary>
		/// The delegate function of <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroyPassthroughHTC">xrDestroyFacialTrackerHTC</see>.
		/// </summary>
		/// <param name="passthrough">passthrough is the <see cref="XrPassthroughHTC">XrPassthroughHTC</see> to be destroyed..</param>
		/// <returns>XR_SUCCESS for success.</returns>
		public delegate XrResult xrDestroyPassthroughHTCDelegate(
			XrPassthroughHTC passthrough);
		// Flag bits for XrCompositionLayerFlags
		public static XrCompositionLayerFlags XR_COMPOSITION_LAYER_CORRECT_CHROMATIC_ABERRATION_BIT = 0x00000001;
		public static XrCompositionLayerFlags XR_COMPOSITION_LAYER_BLEND_TEXTURE_SOURCE_ALPHA_BIT = 0x00000002;
		public static XrCompositionLayerFlags XR_COMPOSITION_LAYER_UNPREMULTIPLIED_ALPHA_BIT = 0x00000004;

		// Flag bits for XrSwapchainCreateFlags
		public static XrSwapchainCreateFlags XR_SWAPCHAIN_CREATE_PROTECTED_CONTENT_BIT = 0x00000001;
		public static XrSwapchainCreateFlags XR_SWAPCHAIN_CREATE_STATIC_IMAGE_BIT = 0x00000002;

		// Flag bits for XrSwapchainUsageFlags
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_COLOR_ATTACHMENT_BIT = 0x00000001;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000002;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_UNORDERED_ACCESS_BIT = 0x00000004;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_TRANSFER_SRC_BIT = 0x00000008;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_TRANSFER_DST_BIT = 0x00000010;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_SAMPLED_BIT = 0x00000020;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_MUTABLE_FORMAT_BIT = 0x00000040;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_INPUT_ATTACHMENT_BIT_MND = 0x00000080;
		public static XrSwapchainUsageFlags XR_SWAPCHAIN_USAGE_INPUT_ATTACHMENT_BIT_KHR = 0x00000080;
	}
}
