namespace Nest
{
	public interface IFileScriptTransform : IFileScript, IScriptTransform { }

	public class FileScriptTransform : FileScript, IFileScriptTransform
	{
		public FileScriptTransform(string file) : base(file)
		{
		}
	}

	public class FileScriptTransformDescriptor : FileScriptDescriptor, IFileScriptTransform
	{
		public FileScriptTransformDescriptor(string file) : base(file)
		{
		}

		public FileScriptTransformDescriptor()
		{
		}
	}
}
