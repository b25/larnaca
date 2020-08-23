namespace Larnaca.Project.Tools.Templating
{
    public enum ETemplateUpdateTemplateMode
    {
        /// <summary>
        /// Update to latest on each build
        /// </summary>
        Build,
        /// <summary>
        /// Never update this template
        /// </summary>
        None,
        /// <summary>
        /// Update template only to a newer version
        /// </summary>
        Newer,
    }
}
