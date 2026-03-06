using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: InternalsVisibleTo("Universal2DGraphicsTests")]
[assembly: InternalsVisibleTo("Universal2DEditorTests")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Universal.Editor")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Universal.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Universal.Runtime.Tests")]
[assembly: InternalsVisibleTo("Unity.GraphicTests.Performance.Universal.Runtime")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
