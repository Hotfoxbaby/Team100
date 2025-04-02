using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrator
    {
        public DateOnly? Dob { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string UId { get; set; } = null!;
    }
}
