namespace Larnaca.Blueprints
{
    public enum EJobIntervalMode
    {
        /// <summary>
        /// The jobs will be fired off at set intervals irrespective of when the previous iteration(s) completes
        /// Can result in multiple jobs executing in parallel
        /// </summary>
        Timer = 0,
        /// <summary>
        /// The job will be run when the specifed interval of time AFTER the previous iteration completes
        /// </summary>
        IntevalBetweenIterations = 1,
    }
}
