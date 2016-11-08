using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<ChainTransform>))]
	public interface IChainTransform : ITransform
	{
		ICollection<TransformContainer> Transforms { get; set; }
	}

	public class ChainTransform : TransformBase, IChainTransform
	{
		public ChainTransform() { }

		public ChainTransform(IEnumerable<TransformContainer> transforms)
		{
			this.Transforms = transforms?.ToList();
		}

		public ICollection<TransformContainer> Transforms { get; set; }

		internal override void WrapInContainer(ITransformContainer container) => container.Chain = this;
	}

	public class ChainTransformDescriptor : DescriptorBase<ChainTransformDescriptor, IChainTransform>, IChainTransform
	{
		public ChainTransformDescriptor() { }

		public ChainTransformDescriptor(ICollection<TransformContainer> transforms)
		{
			Self.Transforms = transforms;
		}

		ICollection<TransformContainer> IChainTransform.Transforms { get; set; }

		/// <inheritdoc />
		public ChainTransformDescriptor Transform(Func<TransformDescriptor, TransformContainer> selector)
		{
			if (Self.Transforms == null) Self.Transforms = new List<TransformContainer>();
			Self.Transforms.Add(selector.InvokeOrDefault(new TransformDescriptor()));
			return this;
		}
	}
}
