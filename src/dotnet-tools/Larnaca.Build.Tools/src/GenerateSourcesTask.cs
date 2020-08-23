using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Larnaca.Build.Tools
{
    public class GenerateSources : Task
    {

        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("GenerateSources task running", "Larnaca Framework 🌴", nameof(GenerateSources), MessageImportance.High));
            return true;

        }
    }
}
