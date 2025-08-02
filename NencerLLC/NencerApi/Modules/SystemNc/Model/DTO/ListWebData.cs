namespace NencerApi.Modules.SystemNc.Model.DTO
{
    public class ListWebData
    {
        public List<ListWebDataDto>?  Relation { get; set; }
        public List<ListWebDataDto>?  JobTitle { get; set; }
        public List<ListWebDataDto>? Ethnic { get; set; }
        public List<ListWebDataDto>? Gender { get; set; }
        public List<ListWebDataDto>? ExamType { get; set; }
        public List<ListWebDataDto>? GuestStyle { get; set; }
        public List<ListWebDataDto>? Method { get; set; }
    }
    public class ListWebDataDto()
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
