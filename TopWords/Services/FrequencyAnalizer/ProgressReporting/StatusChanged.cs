namespace TopWords.Services.FrequencyAnalizer.ProgressReporting
{
    internal class StatusChanged
    {
        public string Status { get; set; }

        public string Details { get; set; }

        public StatusChanged(string status, string details)
        {
            Status = status;
            Details = details;
        }

        public StatusChanged(string status)
        {
            Status = status;
        }
    }
}
