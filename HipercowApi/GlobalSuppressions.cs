// Copyright (c) Imperial College London. All rights reserved.

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Multiple exceptions in one file", Scope = "type", Target = "~T:Hipercow_api.Tools.Exceptions.LdapAuthFailure")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Multiple exceptions in one file", Scope = "type", Target = "~T:Hipercow_api.Tools.Exceptions.LdapNoClusterPermissions")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Multiple exceptions in one file", Scope = "type", Target = "~T:Hipercow_api.Tools.Exceptions.LdapEmptyUsernamePassword")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needs to be mockable", Scope = "member", Target = "~M:LdapManager.DoBind(System.DirectoryServices.Protocols.LdapConnection,System.Net.NetworkCredential)")]
