// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Attributes
{
    using System;

    /// <summary>
    /// Allow the [RequireJwt] attribute for endpoints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireJwtAttribute : Attribute
    {
    }
}
