namespace Larnaca.Blueprints
{
    public interface ILogger
    {
        public void Log(string log, ESeverity severity);
        public void Debug(string log) => Log(log, ESeverity.Debug);
        public void Log(string log) => Log(log, ESeverity.Log);
        public void Fail(string log) => Log(log, ESeverity.Fail);
        public void Warn(string log) => Log(log, ESeverity.Warn);
        public void Error(string log) => Log(log, ESeverity.Error);
    }
}
