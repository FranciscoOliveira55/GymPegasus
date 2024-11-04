using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Server;
using System.ComponentModel;
using System.Globalization;

namespace BackEnd.Data
{
    public class GymContext : IdentityDbContext<User>
    {
        public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
            try
            {   //Usefull if i'm using docker
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    //If there is no database, creates it
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    //If there is no tables in the database, creates them
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Setting all db tables
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<GetInOut> GetInOuts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ExerciseTemplate> ExerciseTemplates { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanExercise> WorkoutPlanExercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<SubscriptionProduct> SubscriptionProducts { get; set; }
        public DbSet<SubscriptionProductPurchaseRecord> SubscriptionProductPurchaseRecords { get; set; }



        //Adding dummy data to the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Cliente", NormalizedName = "CLIENTE" },
                new IdentityRole { Id = "2", Name = "Employee", NormalizedName = "EMPLOYEE" }
                );

            //Creates Users with the "string" password
            var hasher = new PasswordHasher<User>();
            var users = new List<User> {
                new() { Id = "1", UserName = "user1", NormalizedUserName = "USER1", Email = "user1@gmail.com", NormalizedEmail = "USER1@GMAIL.COM", PhoneNumber = "916900179" },
                new() { Id = "2", UserName = "user2", NormalizedUserName = "USER2", Email = "user2@gmail.com", NormalizedEmail = "USER2@GMAIL.COM", PhoneNumber = "916900179"},
                new() { Id = "3", UserName = "admin1", NormalizedUserName = "ADMIN1", Email = "admin1@gmail.com", NormalizedEmail = "ADMIN1@GMAIL.COM", PhoneNumber = "916900179"},
                new() { Id = "4", UserName = "admin2", NormalizedUserName = "ADMIN2" ,Email = "admin2@gmail.com", NormalizedEmail = "ADMIN2@GMAIL.COM", PhoneNumber = "916900179"}
            };
            foreach (User user in users)
            {
                var hashedPassword = hasher.HashPassword(user, "string");
                user.PasswordHash = hashedPassword;
            }
            modelBuilder.Entity<User>().HasData(users);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "1", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "2", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "3", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "4", RoleId = "2" }
            );
            modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, ClienteName = "Francisco", Nif = "1234567890", UserId = "1", SubscriptionExpiration = DateTime.Now, Gender = Gender.Male, BirthDate = DateTime.ParseExact("29/08/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture), Motto = "Sweat now, shine later", ImageUrl = ("https://www.dropbox.com/scl/fi/8u9wo2gy8v29w33cubabj/cliente1m.jpg?rlkey=wu7dxi8fsgpetlhlonxd8kg7a&raw=1") },
            new Cliente { Id = 2, ClienteName = "Pedro", Nif = "2234567890", UserId = "2", SubscriptionExpiration = DateTime.Now, Gender = Gender.Male, BirthDate = DateTime.ParseExact("01/05/1995", "dd/MM/yyyy", CultureInfo.InvariantCulture), Motto = "Train insane or remain the same", ImageUrl = ("https://www.dropbox.com/scl/fi/94b5wofpvl5ue6csy0ffu/cliente2m.jpg?rlkey=twurkth16et7ft058gnsdy4bt&raw=1") }
                );
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, EmployeeName = "Alberto", Nif = "1234567890", UserId = "3", Position = EmployeePosition.Instructor, Gender = Gender.Male, BirthDate = DateTime.ParseExact("14/03/1992", "dd/MM/yyyy", CultureInfo.InvariantCulture), Motto = "Strength begins when you think you have none left", ImageUrl = ("https://www.dropbox.com/scl/fi/mzgubs7foislyfchbmi25/intructor1m.png?rlkey=pn8df4y8thygcyam62x9htejt&raw=1") },
                new Employee { Id = 2, EmployeeName = "Rita", Nif = "2234567890", UserId = "4", Position = EmployeePosition.Reception, Gender = Gender.Female, BirthDate = DateTime.ParseExact("25/09/1995", "dd/MM/yyyy", CultureInfo.InvariantCulture), Motto = "Work hard, train harder", ImageUrl = ("https://www.dropbox.com/scl/fi/pddx98h2nfs759bsvp931/instructor1f.png?rlkey=qlra7ye1q8lhnc2mb18fzx6mc&raw=1") }
                );

            modelBuilder.Entity<GetInOut>().HasData(
                new GetInOut { Id = 1, GetInOutType = GetInOutType.In, DateTime = DateTime.Now, ClienteId = 1 },
                new GetInOut { Id = 2, GetInOutType = GetInOutType.In, DateTime = DateTime.Now, ClienteId = 1 }
                );
            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, Classification = 4, ClienteId = 1, EmployeeId = 1, DateTime = DateTime.Now, Description = "Gostei Muito" },
                new Review { Id = 2, Classification = 5, ClienteId = 2, EmployeeId = 1, DateTime = DateTime.Now, Description = "O melhor Instrutor do Ginásio" }
                );
            modelBuilder.Entity<Event>().HasData(
                new Event { Id = 1, Name = "Hidroginástica ", Description = "Hidroginástica é uma excelente forma de promover saúde e mobilidade" },
                new Event { Id = 2, Name = "Indoor Cycling" }
                );
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = 1, Room = "J27", HourInit = DateTime.Parse("2024/06/1 16:00:00"), HourEnd = DateTime.Parse("2024/06/1 18:00:00"), EventId = 1, EmployeeID = 1, },
                new Schedule { Id = 2, Room = "J27", HourInit = DateTime.Parse("2024/06/2 16:00:00"), HourEnd = DateTime.Parse("2024/06/2 18:00:00"), EventId = 1, EmployeeID = 1, }
                );
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { ClienteId = 1, ScheduleId = 1 },
                new Ticket { ClienteId = 1, ScheduleId = 2 },
                new Ticket { ClienteId = 2, ScheduleId = 1 },
                new Ticket { ClienteId = 2, ScheduleId = 2 }
                );
            modelBuilder.Entity<ExerciseTemplate>().HasData(
                new ExerciseTemplate { Id = 1, Name = "Treadmill", EffortUnit = EffortUnit.Km, RepeatUnit = RepeatUnit.Min, Description = "Correr na passadeira" },
                new ExerciseTemplate { Id = 2, Name = "Bicep curl", EffortUnit = EffortUnit.Kg, RepeatUnit = RepeatUnit.Rep, Description = "Para fazer uma rosca bíceps com halteres, segure um haltere com a palma da mão voltada para cima. Enrole lentamente o peso dobrando o cotovelo, mantendo-o próximo ao corpo. Em seguida, abaixe lentamente o peso até a posição inicial. Você sentirá tensão nos músculos da parte frontal do braço." }
                );
            modelBuilder.Entity<WorkoutPlan>().HasData(
                new WorkoutPlan { Id = 1, Name = "Peito && Costas", ClienteId = 1, EmployeeId = 1, Description = "Plano de treino para treinar peito e costas" },
                new WorkoutPlan { Id = 2, Name = "Pernas", ClienteId = 1, EmployeeId = 1, Description = "Plano de treino para treinar pernas" }
                );
            modelBuilder.Entity<WorkoutPlanExercise>().HasData(
                new WorkoutPlanExercise { Id = 1, WorkoutPlanId = 1, ExerciseTemplateId = 2, Effort = 8, Repeat = 12 },
                new WorkoutPlanExercise { Id = 2, WorkoutPlanId = 1, ExerciseTemplateId = 2, Effort = 8, Repeat = 12 },
                new WorkoutPlanExercise { Id = 3, WorkoutPlanId = 1, ExerciseTemplateId = 2, Effort = 10, Repeat = 12 },
                new WorkoutPlanExercise { Id = 4, WorkoutPlanId = 1, ExerciseTemplateId = 2, Effort = 10, Repeat = 12 },
                new WorkoutPlanExercise { Id = 5, WorkoutPlanId = 2, ExerciseTemplateId = 1, Effort = 10 }
                );
            modelBuilder.Entity<Workout>().HasData(
                new Workout { Id = 1, ClienteId = 1, WorkoutPlanId = 1, HourInit = DateTime.Parse("2023/12/1 16:00:00"), HourEnd = DateTime.Parse("2023/12/1 18:00:00") },
                new Workout { Id = 2, ClienteId = 1, WorkoutPlanId = 2, HourInit = DateTime.Parse("2023/12/3 16:00:00"), HourEnd = DateTime.Parse("2023/12/3 18:00:00") }
                );
            modelBuilder.Entity<WorkoutExercise>().HasData(
                new WorkoutExercise { Id = 1, WorkoutId = 1, ExerciseTemplateId = 2, Effort = 8, Repeat = 12, Done = true },
                new WorkoutExercise { Id = 2, WorkoutId = 1, ExerciseTemplateId = 2, Effort = 8, Repeat = 12, Done = true },
                new WorkoutExercise { Id = 3, WorkoutId = 1, ExerciseTemplateId = 2, Effort = 10, Repeat = 12, Done = true },
                new WorkoutExercise { Id = 4, WorkoutId = 1, ExerciseTemplateId = 2, Effort = 10, Repeat = 12, Done = false },
                new WorkoutExercise { Id = 5, WorkoutId = 2, ExerciseTemplateId = 1, Effort = 10, Done = true }
                );
            modelBuilder.Entity<SubscriptionProduct>().HasData(
                new SubscriptionProduct { Id = 1, Title = "Subscription 1 month", Price = 3000, SubscriptionTimeTicks = TimeSpan.FromDays(30).Ticks, ImageUrl = ("https://www.dropbox.com/scl/fi/0caymcf0688nqh9x27nfj/1monthSub.png?rlkey=oxhdfp06ixwj6snc32l04943f&raw=1") },
                new SubscriptionProduct { Id = 2, Title = "Subscription 3 month", Price = 7500, SubscriptionTimeTicks = TimeSpan.FromDays(92).Ticks, ImageUrl = ("https://www.dropbox.com/scl/fi/mtk22xt3kkk7a0iylb6wp/3monthSub.png?rlkey=rpoacntas6qwgdv3bhfxx6rqo&raw=1") },
                new SubscriptionProduct { Id = 3, Title = "Subscription 6 month", Price = 12500, SubscriptionTimeTicks = TimeSpan.FromDays(183).Ticks, ImageUrl = ("https://www.dropbox.com/scl/fi/49gcj6x5tzfla3i29lrey/6monthSub.png?rlkey=6uhiadcem8t50vdv74uluxqyh&raw=1") },
                new SubscriptionProduct { Id = 4, Title = "Subscription 1 year", Price = 20000, SubscriptionTimeTicks = TimeSpan.FromDays(365).Ticks, ImageUrl = ("https://www.dropbox.com/scl/fi/4fibpru8yaqeodwrquo3a/1yearSub.png?rlkey=ph5i5menlgm93ia121lsfimoj&raw=1") }
                );
        }
    }
}
