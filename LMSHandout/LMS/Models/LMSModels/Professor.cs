using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Classes = new HashSet<Class>();
        }

        public DateOnly? Dob { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string UId { get; set; } = null!;
        public string WorksIn { get; set; }

        public virtual Department? WorksInNavigation { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
