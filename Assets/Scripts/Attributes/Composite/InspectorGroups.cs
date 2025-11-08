using System;
using Sirenix.OdinInspector;

namespace CTNOriginals.PlatformReplayer.Attributes.Composite {
	[IncludeMyAttributes]
	[FoldoutGroup("Config")]
	public class ConfigGroup : Attribute {}

	[IncludeMyAttributes]
	[FoldoutGroup("Runtime"), ReadOnly, HideInEditorMode]
	public class RuntimeGroup : Attribute {}

	[IncludeMyAttributes]
	[FoldoutGroup("References")]
	public class ReferenceGroup : Attribute {}
}