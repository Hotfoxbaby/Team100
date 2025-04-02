using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public string? Name { get; set; }
        public uint? Points { get; set; }
        public string? Contents { get; set; }
        public DateTime? Due { get; set; }
        public int? AcId { get; set; }
        public int AId { get; set; }

        public virtual AssignmentCategory? Ac { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
