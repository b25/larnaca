namespace Larnaca.Project.Tools.Templating
{
    public enum ETemplateGenerateSourceMode
    {
        /// <summary>
        ///  Generate sources on each build
        /// </summary>
        Build,
        /// <summary>
        ///  Never generate sources based on this template
        /// </summary>
        None,
        /// <summary>
        ///  Generate sources when the template is newer
        /// </summary>
        Newer,
        /// <summary>
        ///  Generate sources when the template is updated
        /// </summary>
        Update,
    }
}
