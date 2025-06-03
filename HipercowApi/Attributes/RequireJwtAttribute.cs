namespace HipercowApi.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireJwtAttribute : Attribute
    {
    }
}
