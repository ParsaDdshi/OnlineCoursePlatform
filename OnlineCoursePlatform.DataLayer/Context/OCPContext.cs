using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineCoursePlatform.DataLayer.Entities.Course;
using OnlineCoursePlatform.DataLayer.Entities.Order;
using OnlineCoursePlatform.DataLayer.Entities.Permission;
using OnlineCoursePlatform.DataLayer.Entities.Question;
using OnlineCoursePlatform.DataLayer.Entities.User;
using OnlineCoursePlatform.DataLayer.Entities.Wallet;

namespace OnlineCoursePlatform.DataLayer.Context;

//OCP = Online Courser Platform
public class OCPContext : DbContext
{
    public OCPContext(DbContextOptions<OCPContext> options) : base(options) { }

    #region User

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UsersRoles { get; set; }
    public DbSet<UserDiscountCode> UserDiscountCodes { get; set; }

    #endregion

    #region Wallet

    public DbSet<WalletType> WalletTypes { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

    #endregion

    #region Permission

    public DbSet<Permission> Permission { get; set; }
    public DbSet<RolePermission> RolePermission { get; set; }

    #endregion

    #region Course

    public DbSet<CourseGroup> CourseGroups { get; set; }
    public DbSet<CourseLevel> CourseLevels { get; set; }
    public DbSet<CourseStatus> CourseStatuses { get; set; }
    public DbSet<CourseEpisode> CourseEpisodes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<CourseComment> CourseComments { get; set; }
    public DbSet<CourseVote> CourseVotes { get; set; }

    #endregion

    #region Order

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    #endregion

    #region Question

    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDelete);

        modelBuilder.Entity<Role>()
            .HasQueryFilter(r => !r.IsDelete);

        modelBuilder.Entity<CourseGroup>()
            .HasQueryFilter(g => !g.IsDelete);

        modelBuilder.Entity<Course>()
            .HasQueryFilter(c => !c.IsDelete);

        modelBuilder.Entity<Discount>()
            .HasQueryFilter(d => !d.IsDelete);

        modelBuilder.Entity<Course>()
            .HasOne<CourseGroup>(c => c.CourseGroup)
            .WithMany(c => c.Courses)
            .HasForeignKey(g => g.GroupId);

        modelBuilder.Entity<Course>()
            .HasOne<CourseGroup>(g => g.Group)
            .WithMany(g => g.SubGroup)
            .HasForeignKey(g => g.SubGroup);

        modelBuilder.Entity<Permission>().HasData(
           new Permission()
           {
               PermissionId = 1,
               PermissionTitle = "پنل مدیریت"
           },
           new Permission()
           {
               PermissionId = 2,
               PermissionTitle = "مدیریت کاربران",
               ParentId = 1
           },
           new Permission()
           {
               PermissionId = 3,
               PermissionTitle = "افزودن کاربر",
               ParentId = 2
           },
           new Permission()
           {
               PermissionId = 4,
               PermissionTitle = "ویرایش کاربر",
               ParentId = 2
           },
           new Permission()
           {
               PermissionId = 5,
               PermissionTitle = "حذف کاربر",
               ParentId = 2
           },
           new Permission()
           {
               PermissionId = 6,
               PermissionTitle = "لیست کاربرهای حذف شده",
               ParentId = 2
           },
           new Permission()
           {
               PermissionId = 7,
               PermissionTitle = "مدیریت نفش ها",
               ParentId = 1
           },
           new Permission()
           {
               PermissionId = 8,
               PermissionTitle = "ایجاد نفش",
               ParentId = 7
           },
           new Permission()
           {
               PermissionId = 9,
               PermissionTitle = "ویرایش نفش",
               ParentId = 7
           },
           new Permission()
           {
               PermissionId = 10,
               PermissionTitle = "حذف نفش",
               ParentId = 7
           },
           new Permission()
           {
               PermissionId = 11,
               PermissionTitle = "لیست نقش های حذف شده",
               ParentId = 7
           },
           new Permission()
           {
               PermissionId = 12,
               PermissionTitle = "مدیریت گروه ها",
               ParentId = 1 
           },
           new Permission()
           {
               PermissionId = 13,
               PermissionTitle = "افزودن گروه",
               ParentId = 12
           },
           new Permission()
           {
               PermissionId = 14,
               PermissionTitle = "ویرایش گروه",
               ParentId = 12
           },
           new Permission()
           {
               PermissionId = 15,
               PermissionTitle = "حذف گروه",
               ParentId = 12
           },
           new Permission()
           {
               PermissionId = 16,
               PermissionTitle = "گروه های حذف شده",
               ParentId = 12
           },
           new Permission()
           {
               PermissionId = 17,
               PermissionTitle = "مدیریت دوره ها",
               ParentId = 1
           },
           new Permission()
           {
               PermissionId = 18,
               PermissionTitle = "افزودن دوره",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 19,
               PermissionTitle = "ویرایش دوره",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 20,
               PermissionTitle = "حذف دوره",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 21,
               PermissionTitle = "لیست قسمت ها",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 22,
               PermissionTitle = "افزودن قسمت",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 23,
               PermissionTitle = "ویرایش قسمت",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 24,
               PermissionTitle = "حذف قسمت",
               ParentId = 17
           },
           new Permission()
           {
               PermissionId = 25,
               PermissionTitle = "مدیریت تخفیف ها",
               ParentId = 1
           },
           new Permission()
           {
               PermissionId = 26,
               PermissionTitle = "افزودن تخفیف",
               ParentId = 25
           },
           new Permission()
           {
               PermissionId = 27,
               PermissionTitle = "ویرایش تخفیف",
               ParentId = 25
           },
           new Permission()
           {
               PermissionId = 28,
               PermissionTitle = "حذف تخفیف",
               ParentId = 25
           },
           new Permission()
           {
               PermissionId = 29,
               PermissionTitle = "لیست تخفیف های حذف شده",
               ParentId = 25
           }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role()
            {
                RoleId = 1,
                RoleTitle = "مدیرسایت"
            },
            new Role()
            {
                RoleId = 2,
                RoleTitle = "استاد"
            },
            new Role()
            {
                RoleId = 3,
                RoleTitle = "کاربر"
            }
        );

        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission() { RP_Id = 1, RoleId = 1, PermissionId = 1 },
            new RolePermission() { RP_Id = 2, RoleId = 1, PermissionId = 2 },
            new RolePermission() { RP_Id = 3, RoleId = 1, PermissionId = 3 },
            new RolePermission() { RP_Id = 4, RoleId = 1, PermissionId = 4 },
            new RolePermission() { RP_Id = 5, RoleId = 1, PermissionId = 5 },
            new RolePermission() { RP_Id = 6, RoleId = 1, PermissionId = 6 },
            new RolePermission() { RP_Id = 7, RoleId = 1, PermissionId = 7 },
            new RolePermission() { RP_Id = 8, RoleId = 1, PermissionId = 8 },
            new RolePermission() { RP_Id = 9, RoleId = 1, PermissionId = 9 },
            new RolePermission() { RP_Id = 10, RoleId = 1, PermissionId = 10 },
            new RolePermission() { RP_Id = 11, RoleId = 1, PermissionId = 11 },
            new RolePermission() { RP_Id = 12, RoleId = 1, PermissionId = 12 },
            new RolePermission() { RP_Id = 13, RoleId = 1, PermissionId = 13 },
            new RolePermission() { RP_Id = 14, RoleId = 1, PermissionId = 14 },
            new RolePermission() { RP_Id = 15, RoleId = 1, PermissionId = 15 },
            new RolePermission() { RP_Id = 16, RoleId = 1, PermissionId = 16 },
            new RolePermission() { RP_Id = 17, RoleId = 1, PermissionId = 17 },
            new RolePermission() { RP_Id = 18, RoleId = 1, PermissionId = 18 },
            new RolePermission() { RP_Id = 19, RoleId = 1, PermissionId = 19 },
            new RolePermission() { RP_Id = 20, RoleId = 1, PermissionId = 20 },
            new RolePermission() { RP_Id = 21, RoleId = 1, PermissionId = 21 },
            new RolePermission() { RP_Id = 22, RoleId = 1, PermissionId = 22 },
            new RolePermission() { RP_Id = 23, RoleId = 1, PermissionId = 23 },
            new RolePermission() { RP_Id = 24, RoleId = 1, PermissionId = 24 },
            new RolePermission() { RP_Id = 25, RoleId = 1, PermissionId = 25 },
            new RolePermission() { RP_Id = 26, RoleId = 1, PermissionId = 26 },
            new RolePermission() { RP_Id = 27, RoleId = 1, PermissionId = 27 },
            new RolePermission() { RP_Id = 28, RoleId = 1, PermissionId = 28 },
            new RolePermission() { RP_Id = 29, RoleId = 1, PermissionId = 29 },
            new RolePermission() { RP_Id = 30, RoleId = 2, PermissionId = 1 },
            new RolePermission() { RP_Id = 31, RoleId = 2, PermissionId = 17 },
            new RolePermission() { RP_Id = 32, RoleId = 2, PermissionId = 18 },
            new RolePermission() { RP_Id = 33, RoleId = 2, PermissionId = 19 },
            new RolePermission() { RP_Id = 34, RoleId = 2, PermissionId = 21 },
            new RolePermission() { RP_Id = 35, RoleId = 2, PermissionId = 22 },
            new RolePermission() { RP_Id = 36, RoleId = 2, PermissionId = 23 },
            new RolePermission() { RP_Id = 37, RoleId = 2, PermissionId = 24 }
        );

        modelBuilder.Entity<User>().HasData(
            new User()
            {
                UserId = 1,
                UserName = "admin",
                Email = "admin@gmail.com",
                Password = "21-23-2F-29-7A-57-A5-A7-43-89-4A-0E-4A-80-1F-C3", //admin
                ActiveCode = "admin",
                IsActive = true,
                RegisterDate = DateTime.Now,
                UserAvatar = "DefaultAvatar.png"
            },
            new User()
            {
                UserId = 2,
                UserName = "پارسا داداشی",
                Email = "parsaddshi2002@gmail.com",
                Password = "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70", //123
                ActiveCode = "123",
                IsActive = true,
                RegisterDate = DateTime.Now,
                UserAvatar = "DefaultAvatar.png"
            }
        );

        modelBuilder.Entity<UserRole>().HasData(
           new UserRole()
           {
               UR_Id = 1,
               UserId = 1,
               RoleId = 1
           },
           new UserRole()
           {
               UR_Id = 2,
               UserId = 1,
               RoleId = 2
           },
           new UserRole()
           {
               UR_Id = 3,
               UserId = 1,
               RoleId = 3
           },
           new UserRole()
           {
               UR_Id = 4,
               UserId = 2,
               RoleId = 2
           },
           new UserRole()
           {
               UR_Id = 5,
               UserId = 2,
               RoleId = 3
           }
        );

        modelBuilder.Entity<WalletType>().HasData(
            new WalletType()
            {
                TypeId = 1,
                TypeTitle = "واریز"
            },
            new WalletType()
            {
                TypeId = 2,
                TypeTitle = "برداشت"
            }
        );

        modelBuilder.Entity<CourseGroup>().HasData(
            new CourseGroup()
            {
                GroupId = 1,
                GroupTitle = "برنامه نویسی موبایل",
            },
            new CourseGroup()
            {
                GroupId = 2,
                GroupTitle = "زامارین",
                ParentId = 1
            },
            new CourseGroup()
            {
                GroupId = 3,
                GroupTitle = "React Native",
                ParentId = 1
            },
            new CourseGroup()
            {
                GroupId = 4,
                GroupTitle = "برنامه نویسی وب"
            },
            new CourseGroup()
            {
                GroupId = 5,
                GroupTitle = ".Net Core",
                ParentId = 4
            },
            new CourseGroup()
            {
                GroupId = 6,
                GroupTitle = "Laravel",
                ParentId = 4
            },
            new CourseGroup()
            {
                GroupId = 7,
                GroupTitle = "Django",
                ParentId = 4
            },
            new CourseGroup()
            {
                GroupId = 8,
                GroupTitle = "برنامه نویسی تحت ویندوز"
            },
            new CourseGroup()
            {
                GroupId = 9,
                GroupTitle = "طراحی سایت"
            },
            new CourseGroup()
            {
                GroupId = 10,
                GroupTitle = "HTML/CSS",
                ParentId = 9
            },
            new CourseGroup()
            {
                GroupId = 11,
                GroupTitle = "بوت استرپ",
                ParentId = 9
            },
            new CourseGroup()
            {
                GroupId = 12,
                GroupTitle = "React",
                ParentId = 9
            },
            new CourseGroup()
            {
                GroupId = 13,
                GroupTitle = "بانک های اطلاعاتی",
            }
        );

        modelBuilder.Entity<CourseLevel>().HasData(
            new CourseLevel()
            {
                LevelId = 1,
                LevelTitle = "مقدماتی"
            },
            new CourseLevel()
            {
                LevelId = 2,
                LevelTitle = "متوسط"
            },
            new CourseLevel()
            {
                LevelId = 3,
                LevelTitle = "پیشرفته"
            }
        );

        modelBuilder.Entity<CourseStatus>().HasData(
            new CourseStatus()
            {
                StatusId = 1,
                StatusTitle = "در حال برگزاری"
            },
            new CourseStatus()
            {
                StatusId = 2,
                StatusTitle = "به اتمام رسیده"
            }
       );

        modelBuilder.Entity<Course>().HasData(
            new Course()
            {
                CourseId = 1,
                GroupId = 1,
                SubGroup = 2,
                TeacherId = 1,
                StatusId = 1,
                LevelId = 2,
                CourseTitle = "دوره متخصص زامارین اندروید",
                CourseDescription = "در این دوره آموزش جامع Xamarin Android ما از مقدمه تا پیشرفت و تکمیلی در کنار هم یاد میگیرم . \r\n\r\nزامارین یکی از بهتر محیط برنامه نویسی اندروید هست ما در دوره زامارین  به طور کامل این تکنولوژی رو بررسی میکنم .",
                CoursePrice = 250000,
                Tags = "زامارین - موبایل - اندروید",
                CourseImageName = "Xamarin.jpg",
                CreateDate = DateTime.Now,
                DemoFileName = "demo.mp4"
            },
            new Course()
            {
                CourseId = 2,
                GroupId = 4,
                SubGroup = 5,
                TeacherId = 2,
                StatusId = 1,
                LevelId = 3,
                CourseTitle = "آموزش Asp.Net Core پیشرفته",
                CourseDescription = "دوره Asp.Net Core  پیشرفته به همراه پروژه عملی ( سایت تاپ لرن )\r\n\r\n \r\n\r\nدر این دوره به صورت پروزه محور سایتی شبیه همین سایت تاپ لرن را پیاده سازی خواهیم کرد ",
                CoursePrice = 150000,
                Tags = "asp.net core - .NET - وب",
                CourseImageName = "Asp.netcore.jpg",
                CreateDate = DateTime.Now,
                DemoFileName = "demo.mp4"
            },
            new Course()
            {
                CourseId = 3,
                GroupId = 8,
                TeacherId = 1,
                StatusId = 2,
                LevelId = 1,
                CourseTitle = "دوره آموزش سی شارپ از مقدماتی تا پیشرفته",
                CourseDescription = "در این دوره شما زبان برنامه نویسی سی شارپ، که یکی از قدرتمند ترین زبان های برنامه نویسی شیء گرا می باشد را از 0 تا 100 آموزش خواهید دید و پس از یادگیری با کمک این زبان می توانید تکنولوژی مورد علاقه خود را (وب، ویندوز و موبایل) انتخاب کرده و هرچه سریع تر در آن حرفه ای شوید",
                CoursePrice = 0,
                Tags = "C# - ویندوز - برنامه نویسی",
                CourseImageName = "Cs.jpg",
                CreateDate = DateTime.Now,
                DemoFileName = "demo.mp4"
            },
            new Course()
            {
                CourseId = 4,
                GroupId = 9,
                SubGroup = 10,
                TeacherId = 2,
                StatusId = 1,
                LevelId = 1,
                CourseTitle = "آموزش HTML و CSS از مقدماتی ",
                CourseDescription = "در این دوره سعی شده تمام مباحث مهم و پرکاربرد در طراحی سایت به صورت کامل و عملی گفته شود . مهم ترین نقطه قوت در این دوره آموزشی ، مثال محور بودن این دوره میباشد که تمامی مطالب با مثال های فراوان و عملی از دنیای واقعی طراحی وبسایت برای شما عزیزان گردآوری شده است، امید است پس از گذراندن این دوره به راحتی به طراحی یک سایت کامل و جامع با متدهای روز تسلط کامل پیدا کنید",
                CoursePrice = 0,
                Tags = "HTML - CSS - طراحی سایت",
                CourseImageName = "Htmlcss.jpg",
                CreateDate = DateTime.Now,
                DemoFileName = "demo.mp4"
            },
            new Course()
            {
                CourseId = 5,
                GroupId = 13,
                TeacherId = 1,
                StatusId = 2,
                LevelId = 2,
                CourseTitle = "آموزش SQL Server 2022",
                CourseDescription = "ما در این دوره قرار با همدیگه به امید خدا با پایگاه داده SQL Server از صفر آشنا بشیم و با تمامی مواردی که نیاز داریم برای توسعه و طراحی یک دیتابیس آشنا بشیم و یادشون بگیریم، و موارد مختلفی رو با هم کار کنیم مثل ساخت انواع پروسیجر ها ، فانکشن ها ، تریگر ها ،  جداول  و ویوو ها و ... با انواع کوئری های ساده و پیچیده کار کنیم و آشنا بشیم.",   
                CoursePrice = 400000,
                Tags = "SQL - دیتابیس -  پایگاه داده",
                CourseImageName = "Sql.jpg",
                CreateDate = DateTime.Now,
                DemoFileName = "demo.mp4"
            }
        );

        modelBuilder.Entity<CourseEpisode>().HasData(
            new CourseEpisode()
            {
                EpisodeId = 1,
                CourseId = 1,
                EpisodeTitle = "مقدمه",
                EpisodeTime = new TimeSpan(0, 5, 0),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 2,
                CourseId = 1,
                EpisodeTitle = "قسمت اول",
                EpisodeTime = new TimeSpan(0, 16, 33),
                EpisodeFileName = "courseFile.rar",
            },
            new CourseEpisode()
            {
                EpisodeId = 3,
                CourseId = 2,
                EpisodeTitle = "مقدمه",
                EpisodeTime = new TimeSpan(0, 7, 33),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 4,
                CourseId = 2,
                EpisodeTitle = "پیش نیاز های دوره",
                EpisodeTime = new TimeSpan(0, 10, 15),
                EpisodeFileName = "courseFile.rar",
            },
            new CourseEpisode()
            {
                EpisodeId = 5,
                CourseId = 3,
                EpisodeTitle = "مقدمه",
                EpisodeTime = new TimeSpan(0, 4, 00),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 6,
                CourseId = 3,
                EpisodeTitle = "انواع متغیرها",
                EpisodeTime = new TimeSpan(0, 8, 00),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 7,
                CourseId = 4,
                EpisodeTitle = "مقدمه",
                EpisodeTime = new TimeSpan(0, 6, 2),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 8,
                CourseId = 4,
                EpisodeTitle = "انواع تگ ها",
                EpisodeTime = new TimeSpan(0, 15, 28),
                EpisodeFileName = "courseFile.rar",
                IsFree = true
            },
            new CourseEpisode()
            {
                EpisodeId = 9,
                CourseId = 5,
                EpisodeTitle = "مقدمه",
                EpisodeTime = new TimeSpan(0, 4, 22),
                EpisodeFileName = "courseFile.rar",
            },
            new CourseEpisode()
            {
                EpisodeId = 10,
                CourseId = 5,
                EpisodeTitle = "پیش نیاز های دوره",
                EpisodeTime = new TimeSpan(0, 18, 17),
                EpisodeFileName = "courseFile.rar",
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}

public class OCPContextFactory : IDesignTimeDbContextFactory<OCPContext>
{
    public OCPContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OCPContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("OCPConnection"));

        return new OCPContext(optionsBuilder.Options);
    }
}