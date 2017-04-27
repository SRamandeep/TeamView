using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("Standard")]
    public class Standard
    {
        public virtual int Id { get; set; }

        public virtual string Code { get; set; }

        public virtual int Level { get; set; }

        public virtual decimal Floor { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}