using System;

namespace Larnaca.Blueprints
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class MicroserviceAttribute : Attribute
    {
        public MicroserviceAttribute() { }
    }
}