using CourtApp.Web.Areas.LawyerDiary.Models;
using System.Collections.Generic;

namespace CourtApp.Web.Models
{
    public class DataTableRequest
    {
        //public int Draw { get; set; }
        //public int PageSize { get; set; }
        //public int PageNumber { get; set; }
        //public string SearchValue { get; set; }
        //public string SortColumn { get; set; }
        //public string SortDirection { get; set; }

        public int draw { get; set; }
        public int start { get; set; }         // Starting record index
        public int length { get; set; }        // Page size
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public List<Column> columns { get; set; }
    }
    public class Search
    {
        public string value { get; set; }
        public bool regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class DataTableResponse
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<ProceedingSubHeadViewModel> data { get; set; }
    }
}
