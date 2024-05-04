using System;

namespace JobsApplication.Models
{
    public class UpdateJobRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime ClosingDate { get; set; }
    }
}
