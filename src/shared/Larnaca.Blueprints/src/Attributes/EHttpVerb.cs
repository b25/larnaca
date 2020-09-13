namespace Larnaca.Blueprints
{
    /// <summary>
    /// Compatible with System.Web.Mvc.HttpVerbs Enum, hence the non-intuitive values
    /// </summary>
    public enum EHttpVerb
    {
        /// <summary>
        /// Retrieves the information or entity that is identified by the URI of the request.
        /// </summary>
        Get = 1,
        /// <summary>
        /// Posts a new entity as an addition to a URI.
        /// </summary>
        Post = 2,
        /// <summary>
        /// Replaces an entity that is identified by a URI.
        /// </summary>
        Put = 4,
        /// <summary>
        /// Requests that a specified URI be deleted.
        /// </summary>
        Delete = 8,
        /// <summary>
        /// Requests that a set of changes described in the request entity be applied to the resource identified by the Request- URI.
        /// </summary>
        Patch = 32
    }
}