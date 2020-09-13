using Mono.TextTemplating;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Larnaca.Project.Tools
{
    internal class TemplateLarnacaProperties
    {
        public ETemplateReplication Replication { get; set; }
        public ETemplateType Type { get; set; }
        public bool DoNotCompile { get; set; }

        internal static OperationResult<TemplateLarnacaProperties> Extract(ParsedTemplate pt)
        {
            if (pt is null)
            {
                throw new ArgumentNullException(nameof(pt));
            }
            OperationResult<TemplateLarnacaProperties> theReturn = new OperationResult<TemplateLarnacaProperties>()
            {
                StatusMessage = string.Empty,
                Data = new TemplateLarnacaProperties()
            };
            var templateDirective = pt.Directives.FirstOrDefault(d => d.Name.Equals("template", StringComparison.OrdinalIgnoreCase));


            if (!templateDirective.Attributes.TryGetValue(nameof(Type), out var templateTypeString))
            {
                theReturn.StatusCode = 1;
                theReturn.StatusMessage += $"Template missing '{nameof(Type)}' from Template directive";
            }
            else if (Enum.TryParse<ETemplateType>(templateTypeString, out var templateType))
            {
                theReturn.Data.Type = templateType;
            }
            else
            {
                theReturn.StatusCode = 1;
                theReturn.StatusMessage += $"Template '{nameof(Type)}'={templateTypeString} from Template directive could not be parsed";
            }

            if (!templateDirective.Attributes.TryGetValue(nameof(Replication), out var templateReplicationString))
            {
                theReturn.StatusCode = 1;
                theReturn.StatusMessage += $"Template missing '{nameof(Replication)}' from Template directive";
            }
            else if (Enum.TryParse<ETemplateReplication>(templateReplicationString, out var templateReplication))
            {
                theReturn.Data.Replication = templateReplication;
            }
            else
            {
                theReturn.StatusCode = 1;
                theReturn.StatusMessage += $"Template '{nameof(Replication)}'={templateReplicationString} from Template directive could not be parsed";
            }

            if (templateDirective.Attributes.TryGetValue(nameof(DoNotCompile), out var doNotCompileString))
            {
                if (!bool.TryParse(doNotCompileString, out var doNotCompile))
                {
                    theReturn.StatusCode = 1;
                    theReturn.StatusMessage += $"Template '{nameof(DoNotCompile)}'={doNotCompileString} from Template directive could not be parsed";
                }
                else
                {
                    theReturn.Data.DoNotCompile = doNotCompile;
                }
            }

            return theReturn;
        }
    }
}
