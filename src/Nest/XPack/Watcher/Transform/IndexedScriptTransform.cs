namespace Nest
{
	public interface IIndexedScriptTransform : IIndexedScript, IScriptTransform { }

	public class IndexedScriptTransform : IndexedScript, IIndexedScriptTransform
	{
		public IndexedScriptTransform(string id) : base(id)
		{
		}
	}

	public class IndexedScriptTransformDescriptor : IndexedScriptDescriptor, IIndexedScriptTransform
	{
		public IndexedScriptTransformDescriptor(string id) : base(id)
		{
		}

		public IndexedScriptTransformDescriptor()
		{
		}
	}
}
