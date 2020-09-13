using System.Threading;

namespace Larnaca.Blueprints
{
    public interface IContext : ILogger, IConfigger
    {
        public ContextDetails Details { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
