using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebRTCSignalR.Entity;

namespace WebRTCSignalR.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual string ConnectionId { get; set; }
        public virtual bool InCall { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual int? TeacherId { get; set; }

        //Navigation properties
        public virtual Parent Parent { get; set; }
        public virtual Teacher Teacher { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<School> School { get; set; }
        public virtual DbSet<Standard> Standard { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Parent> Parent { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }
        public virtual DbSet<SubjectTeacherMap> SubjectTeacherMap { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        
    }
}