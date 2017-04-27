namespace WebRTCSignalR.Migrations
{
    using Entity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebRTCSignalR.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        protected override void Seed(WebRTCSignalR.Models.ApplicationDbContext context)
        {
            if (context.Set<School>().Find(1) == null)
            {
                context.Set<School>().Add(new School
                {
                    Name = "Sample Public School"
                });
            }


            //Subject
            if (context.Set<Subject>().Find(1) == null)
            {
                context.Set<Subject>().Add(new Subject
                {
                    Name = "English"
                });

                context.Set<Subject>().Add(new Subject
                {
                    Name = "Science"
                });
            }

            //Standard
            if (context.Set<Standard>().Find(1) == null)
            {
                context.Set<Standard>().Add(new Standard
                {
                    Level = 10,
                });
            }

            //Save Changes
            context.SaveChanges();

            var passwordHash = new PasswordHasher();
            string password = "Password@123";
            if (!(context.Users.Any(u => u.UserName == "admin@gmail.com")))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                ApplicationUser userToInsert = new ApplicationUser
                {
                    UserName = "parent1@gmail.com",
                    PhoneNumber = "0797697898",
                    Parent = new Entity.Parent()
                    {
                        Name = "Mr. Sunil",
                        Students = new List<Student>() {
                            new Student
                                {
                                    Name = "Raman Pratap Singh",
                                    StandardId = context.Set<Standard>().Find(1).Id,
                                    DateOfBirth = DateTime.Now.AddYears(24),
                                    RollNo = 11100021
                                }
                        }
                    }
                };
                userManager.Create(userToInsert, password);

                userToInsert = new ApplicationUser
                {
                    UserName = "Parent2@gmail.com",
                    PhoneNumber = "0797697898",
                    Parent = new Entity.Parent()
                    {
                        Name = "Mr. Kapoor",
                        Students = new List<Student>() {
                            new Student
                                {
                                    Name = "Saloni Sharma",
                                    StandardId = context.Set<Standard>().Find(1).Id,
                                    DateOfBirth = DateTime.Now.AddYears(24),
                                    RollNo = 11100022
                                }
                        }
                    }
                };
                userManager.Create(userToInsert, password);

                userToInsert = new ApplicationUser
                {
                    UserName = "Teacher1@gmail.com",
                    PhoneNumber = "0797697898",
                    Teacher = new Entity.Teacher()
                    {
                        Name = "Dr. R.K. Mishra"
                    }
                };
                userManager.Create(userToInsert, password);

                userToInsert = new ApplicationUser
                {
                    UserName = "Teacher2@gmail.com",
                    PhoneNumber = "0797697898",
                    Teacher = new Entity.Teacher()
                    {
                        Name = "Dr. Ravi Goyal"
                    }
                };
                userManager.Create(userToInsert, password);
            }

            //Student Teacher Map
            if (context.Set<SubjectTeacherMap>().Find(1) == null)
            {
                context.Set<SubjectTeacherMap>().Add(new SubjectTeacherMap()
                {
                    IsActive = true,
                    SubjectId = context.Set<Subject>().Find(1).Id,
                    TeacherId = context.Set<Teacher>().Find(1).Id
                });

                context.Set<SubjectTeacherMap>().Add(new SubjectTeacherMap()
                {
                    IsActive = true,
                    SubjectId = context.Set<Subject>().Find(2).Id,
                    TeacherId = context.Set<Teacher>().Find(1).Id
                });
            }

            context.SaveChanges();
        }
    }
}
