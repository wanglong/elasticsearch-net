using System;

namespace Nest
{
	public abstract class ActionsDescriptorBase<TDescriptor, TInterface>
		: DescriptorBase<TDescriptor, TInterface>, IAction
		where TDescriptor : DescriptorBase<TDescriptor, TInterface>, TInterface
		where TInterface : class, IAction
	{
		private readonly string _name;

		protected ActionsDescriptorBase(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (name.Length == 0) throw new ArgumentException("cannot be empty");

			this._name = name;
		}

		string IAction.Name => this._name;
		ActionType IAction.ActionType => this.ActionType;
		TransformContainer IAction.Transform { get; set; }
		string IAction.ThrottlePeriod { get; set; }

		protected abstract ActionType ActionType { get; }

		public TDescriptor Transform(Func<TransformDescriptor, TransformContainer> selector) =>
			Assign(a => a.Transform = selector.InvokeOrDefault(new TransformDescriptor()));

		public TDescriptor ThrottlePeriod(string throttlePeriod) => Assign(a => a.ThrottlePeriod = throttlePeriod);
	}
}