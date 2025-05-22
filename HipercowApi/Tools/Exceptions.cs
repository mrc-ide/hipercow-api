// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools.Exceptions
{
    /// <summary>
    /// Exception when LDAP user/password is incorrect.
    /// </summary>
    /// <param name="user">Username that failed to authenticate.</param>
    public class LdapAuthFailure(string user) : Exception($"Couldn't authenticate user {user}")
    {
    }

    /// <summary>
    /// Exception when LDAP user/password is correct, but no permission on any clusters.
    /// </summary>
    /// <param name="user">Username that had no permissions.</param>
    public class LdapNoClusterPermissions(string user) : Exception($"User {user} does not have permission to use any clusters.")
    {
    }

    /// <summary>
    /// Exception when user/password not provided.
    /// </summary>
    public class LdapEmptyUsernamePassword() : Exception("User and password for login cannot be empty.")
    {
    }
}