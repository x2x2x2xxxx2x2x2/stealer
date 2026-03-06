using Costura;
using System;

internal static class ModuleInitializer
{
	static ModuleInitializer()
	{
		AssemblyLoader.Attach(true);
	}
}