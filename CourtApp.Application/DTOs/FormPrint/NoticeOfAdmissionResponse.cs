namespace CourtApp.Application.DTOs.FormPrint
{
    public class NoticeOfAdmissionResponse
    {
        /// <summary>
        /// When first title value is Applent
        /// </summary>
        public string Applent { get; set; }

        /// <summary>
        /// When Second title value is Respondent
        /// </summary>
        public string Respondent { get; set; }

        /// <summary>
        /// Case category either form Writ, Civil, or criminal
        /// </summary>
        public string CaseCategory { get; set; }

        /// <summary>
        /// Case Type of the user
        /// </summary>
        public string CaseType { get; set; }

        /// <summary>
        /// Combined value fo Case No & Year
        /// </summary>
        public string NoYear { get; set; }

        /// <summary>
        /// Against court if any
        /// </summary>
        public string AgainstCourt { get; set; }

        /// <summary>
        /// this property is bind for the civil origin case type
        /// </summary>
        public string CivilCaseType { get; set; }
        /// <summary>
        /// this property is bind for the civil origin case number
        /// </summary>
        public string CivilNoYear { get; set; }

        /// <summary>
        /// This is complete title of Respondent
        /// </summary>
        public string CompleteTitle { get; set; }
        /// <summary>
        /// This is complete title Number of Respondent
        /// </summary>
        public string TitleNo { get; set; }
        /// <summary>
        /// This is case bench
        /// </summary>
        public string Bench { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Date { get; set; }
    }
}
