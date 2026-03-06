using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Runtime.Shared")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Editor")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Runtime.Tests")]
[assembly: InternalsVisibleTo("Unity.GraphicTests.Performance.RPCore.Runtime")]
[assembly: InternalsVisibleTo("Unity.GraphicTests.Performance.Universal.Runtime")]
[assembly: InternalsVisibleTo("SRPSmoke.Runtime.Tests")]
[assembly: InternalsVisibleTo("SRPSmoke.Editor.Tests")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-testable")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Editor.Tests")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
