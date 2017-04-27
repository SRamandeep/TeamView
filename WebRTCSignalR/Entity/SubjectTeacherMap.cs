using System.ComponentModel.DataAnnotations.Schema;

namespace WebRTCSignalR.Entity
{
    [Table("SubjectTeacherMap")]
    public class SubjectTeacherMap
    {
        public virtual int Id { get; set; }
        public virtual int SubjectId { get; set; }
        public virtual int TeacherId { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Subject Subject { get; set; }
    }
}