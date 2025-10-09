using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class CaseHistoryViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Court { get; set; }
        public string CaseNoYear { get; set; }
        public List<HistoryDetail> History { get; set; }
        public List<CaseDoc> Docs { get; set; }
        public List<ProcHistory> ProcHis { get; set; }
    }
    public class ProcHistory
    {
        public string Date { get; set; }
        public List<HistoryDetail> History { get; set; }

    }
    public class HistoryDetail
    {
        public string Type { get; set; }
        public string Date { get; set; }
        public string NextDate { get; set; }
        public string Stage { get; set; }
        public string Activity { get; set; }
        public List<CaseWorkDetail> WorkDetail { get; set; }
    }

    public class CaseDoc
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string DocName { get; set; }
        public string DocDate { get; set; }
        public string DocFilePath { get; set; }
        public string FIcon { get; set; }
        public string Reference { get; set; }
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
