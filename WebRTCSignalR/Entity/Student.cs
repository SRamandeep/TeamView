using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("Student")]
    public class Student
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual Int32 RollNo { get; set; }
        public virtual int StandardId { get; set; }

        //Navigation Properties
        public virtual Standard Standard { get; set; }

        public virtual ICollection<SubjectTeacherMap> SubjectTeacherMapping { get; set; }
    }
}