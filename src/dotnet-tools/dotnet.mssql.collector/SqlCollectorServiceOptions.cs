namespace mssql.collector
{
    public class SqlCollectorServiceOptions
    {
        public string ConnectionString { get; set; }
        public string ProcedurePattern { get; set; } = "(?i)(^prc__?)(?!.*internal).*";
        public bool SkipOutputParams { get; set; }
        public string ResultFile { get; set; } = "result.json";
        public string PreviousResultFile { get; set; } = "result_prev.json";
    }
}