using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Constraints;

namespace DataAccess.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<NoteModel> Notes { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteModel>().HasKey(x => x.ItemId);
            modelBuilder.Entity<NoteModel>().Property(x => x.ItemId).ValueGeneratedOnAdd();
            modelBuilder.Entity<NoteModel>().Property(x => x.Name).HasMaxLength(FieldLimits.ItemNameMax);
            modelBuilder.Entity<NoteModel>().Property(x => x.Text).HasMaxLength(FieldLimits.NoteTextMax);
            modelBuilder.Entity<NoteModel>()
                .HasOne<UserModel>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskModel>().HasKey(x => x.ItemId);
            modelBuilder.Entity<TaskModel>().Property(x => x.ItemId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TaskModel>().Property(x => x.Name).HasMaxLength(FieldLimits.ItemNameMax);
            modelBuilder.Entity<TaskModel>().Property(x => x.Description).HasMaxLength(FieldLimits.TaskDescriptionMax);
            modelBuilder.Entity<TaskModel>()
                .HasOne<UserModel>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>().HasKey(x => x.Id);
            modelBuilder.Entity<UserModel>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserModel>().Property(x => x.Name).HasMaxLength(FieldLimits.UserNameMax);
            modelBuilder.Entity<UserModel>().HasIndex(x => x.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}