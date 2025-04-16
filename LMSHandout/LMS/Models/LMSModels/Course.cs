using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public Course(string subject, int number, string name)
        {
            Subject = subject;
            Number = number.ToString();
            Name = name;

            Classes = new HashSet<Class>();
        }

        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? Subject { get; set; }
        public int CourseId { get; set; }

        public virtual Department? SubjectNavigation { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
