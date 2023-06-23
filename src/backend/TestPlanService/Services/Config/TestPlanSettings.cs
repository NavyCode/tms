namespace TestPlanService.Services.Config
{
    public class TestPlanSettings
    {
        public string PgConnectionString { get; set; }
        public string MsSqlConnectionString { get; set; }
        public bool IsClearDb { get; set; }
    }
}