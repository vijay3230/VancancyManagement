using System.Collections.Generic;

namespace JobsApplication.Models
{
    public class JobListResponse
    {
        public int Total { get; set; }
        public List<Job> JobList { get; set; }
    }
}
