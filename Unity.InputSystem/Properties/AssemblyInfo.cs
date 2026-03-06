using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyVersion("1.14.2.0")]
[assembly: InternalsVisibleTo("Unity.InputSystem.TestFramework")]
[assembly: InternalsVisibleTo("Unity.InputSystem.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.InputSystem.Tests")]
[assembly: InternalsVisibleTo("Unity.InputSystem.IntegrationTests")]
[assembly: InternalsVisibleTo("Unity.InputSystem.ForUI")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
