using System;using Thinktecture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tracnghiem.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<ExamDAO> Exam { get; set; }
        public virtual DbSet<ExamHistoryDAO> ExamHistory { get; set; }
        public virtual DbSet<ExamLevelDAO> ExamLevel { get; set; }
        public virtual DbSet<ExamQuestionMappingDAO> ExamQuestionMapping { get; set; }
        public virtual DbSet<ExamStatusDAO> ExamStatus { get; set; }
        public virtual DbSet<GradeDAO> Grade { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<QuestionDAO> Question { get; set; }
        public virtual DbSet<QuestionContentDAO> QuestionContent { get; set; }
        public virtual DbSet<QuestionGroupDAO> QuestionGroup { get; set; }
        public virtual DbSet<QuestionTypeDAO> QuestionType { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<StatusDAO> Status { get; set; }
        public virtual DbSet<SubjectDAO> Subject { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=42.96.33.23,20651;initial catalog=Tracnghiem;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RefreshToken).HasMaxLength(4000);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_AppUser_Image");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_Role");
            });

            modelBuilder.Entity<ExamDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TotalMark).HasColumnType("decimal(20, 6)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_AppUser");

                entity.HasOne(d => d.ExamLevel)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.ExamLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_ExamLevel");

                entity.HasOne(d => d.ExamStatus)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.ExamStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_ExamStatus");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Grade");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Exam_Image");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Status");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Subject");
            });

            modelBuilder.Entity<ExamHistoryDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppUserId).ValueGeneratedOnAdd();

                entity.Property(e => e.ExamedAt).HasColumnType("datetime");

                entity.Property(e => e.Mark).HasColumnType("decimal(20, 6)");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.ExamHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamHistory_AppUser");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamHistories)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamHistory_Exam");
            });

            modelBuilder.Entity<ExamLevelDAO>(entity =>
            {
                entity.ToTable("ExamLevel", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ExamQuestionMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ExamId, e.QuestionId });

                entity.Property(e => e.Mark).HasColumnType("decimal(20, 6)");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamQuestionMappings)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamQuestionMapping_Exam");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamQuestionMappings)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamQuestionMapping_Question");
            });

            modelBuilder.Entity<ExamStatusDAO>(entity =>
            {
                entity.ToTable("ExamStatus", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<GradeDAO>(entity =>
            {
                entity.ToTable("Grade", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ImageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<QuestionDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Grade");

                entity.HasOne(d => d.QuestionGroup)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QuestionGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_QuestionGroup");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_QuestionType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Status");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Subject");
            });

            modelBuilder.Entity<QuestionContentDAO>(entity =>
            {
                entity.Property(e => e.AnswerContent)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionContents)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionContent_Question");
            });

            modelBuilder.Entity<QuestionGroupDAO>(entity =>
            {
                entity.ToTable("QuestionGroup", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<QuestionTypeDAO>(entity =>
            {
                entity.ToTable("QuestionType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.ToTable("Role", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<StatusDAO>(entity =>
            {
                entity.ToTable("Status", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<SubjectDAO>(entity =>
            {
                entity.ToTable("Subject", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
