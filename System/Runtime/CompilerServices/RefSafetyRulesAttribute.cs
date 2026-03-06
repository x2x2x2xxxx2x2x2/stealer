using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute([In] int obj0)
		{
			Version = obj0;
		}
	}
}