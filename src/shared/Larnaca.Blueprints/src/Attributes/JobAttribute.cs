using System;

namespace Larnaca.Blueprints
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    sealed class JobAttribute : Attribute
    {
        public uint Interval { get; private set; }
        public EJobIntervalMode JobIntervalMode { get; private set; }
        public JobAttribute(uint interval, EJobIntervalMode jobIntervalMode = EJobIntervalMode.Timer)
        {
            Interval = interval;
            JobIntervalMode = jobIntervalMode;
        }
    }
}
