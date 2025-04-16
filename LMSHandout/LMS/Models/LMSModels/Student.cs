using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolleds = new HashSet<Enrolled>();
            Submissions = new HashSet<Submission>();
        }

        public DateOnly? Dob { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string UId { get; set; } = null!;
        public string Major { get; set; } = null!;

        public virtual Department? MajorNavigation { get; set; }
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
