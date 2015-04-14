namespace HealthCheck.Core
{
    public class CheckResult
    {
        public string Checker { get; set; }
        public bool Passed { get; set; }
        public string Output { get; set; }
    }
}
