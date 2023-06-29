using About.Interface;  
using About.Models;
using Microsoft.EntityFrameworkCore;

namespace About.Data
{
    public class AccountContext:DbContext
    {
        private readonly IHashService _hashService;
        public AccountContext(DbContextOptions<AccountContext>options, IHashService hashService) :base(options)
        {
            _hashService = hashService;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Embedding> Embedding { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //使用Entity Framework的Fluent API，通過使用HasKey方法將UserId和RoleId屬性標記為複合主鍵
            modelBuilder.Entity<UserRoles>().HasKey(ur => new { ur.UserId, ur.RoleId });

            //Password未加密
            //modelBuilder.Entity<User>().HasData(
            //    new User { Id = "U001", Name = "Kevin@gmail.com", Email = "kevinxi@gmail.com", Password = "123456", PasswordConfirmed = "123456" },
            //    new User { Id = "U002", Name = "Mary@gmail.com", Email = "marylee@gmail.com", Password = "123456", PasswordConfirmed = "123456" },
            //    new User { Id = "U003", Name = "John@gmail.com", Email = "johnwei@gmail.com", Password = "123456", PasswordConfirmed = "123456" }
            //    );

            //Password以MD5加密

            modelBuilder.Entity<User>().HasData(
                new User { Id = "U001", Name = "Kevin@gmail.com", Email = "kevinxi@gmail.com",Password =_hashService.MD5Hash("123456"), PasswordConfirmed = _hashService.MD5Hash("123456") },
                new User { Id = "U002", Name = "Mary@gmail.com", Email = "marylee@gmail.com", Password =_hashService.MD5Hash("123456"), PasswordConfirmed = _hashService.MD5Hash("123456") },
                new User { Id = "U003", Name = "John@gmail.com", Email = "johnwei@gmail.com", Password =_hashService.MD5Hash("123456"), PasswordConfirmed = _hashService.MD5Hash("123456") }
                );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = "R001", Name = "User" },
                new Role { Id = "R002", Name = "Member" }
                );



            modelBuilder.Entity<UserRoles>()
                .HasData(
                new UserRoles { UserId = "U001", RoleId = "R001" },
                new UserRoles { UserId = "U002", RoleId = "R001" },
                new UserRoles { UserId = "U003", RoleId = "R001" },
                new UserRoles { UserId = "U003", RoleId = "R002" }
                );

            //modelBuilder.Entity<Embedding>().HasData(
            //    new Embedding { EmbeddingID = "E0000", EmbeddingQuestion = null, EmbeddingAnswer = null, QA = "QA", EmbeddingVectors = "[123]" }

            //    );

        }

    }
}
