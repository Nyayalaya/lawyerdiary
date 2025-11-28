using System;
using System.Collections.Generic;

namespace CourtApp.Application.DTOs.CaseDetails
{
    public class CaseHistoryResposnse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CourtType { get; set; }
        public string Court { get; set; }
        public string CaseNoYear { get; set; }
        public List<CaseHistoryData> History { get; set; }
        public List<CaseUploadedDocument> Docs { get; set; }
    }

    public class CaseHistoryData
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string NextDate { get; set; }
        public string Stage { get; set; }
        public string Activity { get; set; }
        public List<CaseWorkDetail> WorkDetail { get; set; }
    }
    public class CaseUploadedDocument
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string DocName { get; set; }
        public string DocFilePath { get; set; }
        public string DocDate { get; set; }
    }

    public class CaseWorkDetail
    {
        public string WorkingDate { get; set; }
        public List<CaseWork> Works { get; set; }

    }
    public class CaseWork
    {
        public string WorkType { get; set; }
        public string Work { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string AppliedOn { get; set; }
    }
}
