using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("Subject")]
    public class Subject
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
    }
}