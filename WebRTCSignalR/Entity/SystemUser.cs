using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("User")]
    public class SystemUser
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual int? TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Parent Parent { get; set; }
    }
}