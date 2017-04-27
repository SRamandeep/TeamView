using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebRTCSignalR.Models;

namespace WebRTCSignalR.Entity
{
    [Table("Parent")]
    public class Parent 
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}