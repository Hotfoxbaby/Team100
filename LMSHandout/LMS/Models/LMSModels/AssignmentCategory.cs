using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public uint? Weight { get; set; }
        public string? Name { get; set; }
        public int? ClassId { get; set; }
        public int AcId { get; set; }

        public virtual Class? Class { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
