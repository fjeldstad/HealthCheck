namespace HealthCheck.Core
{
    public class CheckResult
    {
        public string CheckerName { get; set; }
        public string SectionName { get; set; }
        public bool Passed { get; set; }
        public string Output { get; set; }   
    }
}
