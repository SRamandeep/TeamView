using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("School")]
    public class School
    {
        public virtual int Id { get; set; }

        public virtual String Name { get; set; }

        public virtual string Address { get; set; }

        public virtual string City { get; set; }

        public virtual string State { get; set; }

        public virtual string Country { get; set; }

        public virtual ICollection<Standard> Standards { get; set; }
    }
}