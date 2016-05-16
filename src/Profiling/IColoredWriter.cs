using System;

namespace Profiling
{
	public interface IColoredWriter
	{
		void Write(ConsoleColor color, string value);
		void WriteLine(ConsoleColor color, string value);
	}
}
