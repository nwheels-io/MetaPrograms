using System;

namespace Example.WebUIModel
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, 
        AllowMultiple = false, 
        Inherited = true)]
    public class ProgrammingModelAttribute : Attribute
    {
    }
}
