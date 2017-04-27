using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("Teacher")]
    public class Teacher
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Degree { get; set; }
    }
}