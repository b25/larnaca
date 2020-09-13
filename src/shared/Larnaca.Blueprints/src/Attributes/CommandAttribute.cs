using System;

namespace Larnaca.Blueprints
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute() { }
    }
}