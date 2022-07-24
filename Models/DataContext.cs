using System;using Thinktecture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tracnghiem.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ActionDAO> Action { get; set; }
        public virtual DbSet<ActionPageMappingDAO> ActionPageMapping { get; set; }
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserRoleMappingDAO> AppUserRoleMapping { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<ExamDAO> Exam { get; set; }
        public virtual DbSet<ExamHistoryDAO> ExamHistory { get; set; }
        public virtual DbSet<ExamLevelDAO> ExamLevel { get; set; }
        public virtual DbSet<ExamQuestionMappingDAO> ExamQuestionMapping { get; set; }
        public virtual DbSet<ExamStatusDAO> ExamStatus { get; set; }
        public virtual DbSet<FieldDAO> Field { get; set; }
        public virtual DbSet<FieldTypeDAO> FieldType { get; set; }
        public virtual DbSet<GradeDAO> Grade { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<MenuDAO> Menu { get; set; }
        public virtual DbSet<PageDAO> Page { get; set; }
        public virtual DbSet<PermissionDAO> Permission { get; set; }
        public virtual DbSet<PermissionActionMappingDAO> PermissionActionMapping { get; set; }
        public virtual DbSet<PermissionContentDAO> PermissionContent { get; set; }
        public virtual DbSet<PermissionOperatorDAO> PermissionOperator { get; set; }
        public virtual DbSet<QuestionDAO> Question { get; set; }
        public virtual DbSet<QuestionContentDAO> QuestionContent { get; set; }
        public virtual DbSet<QuestionGroupDAO> QuestionGroup { get; set; }
        public virtual DbSet<QuestionTypeDAO> QuestionType { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<SiteDAO> Site { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }
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
            modelBuilder.ConfigureTempTable<long>();modelBuilder.ConfigureTempTable<Guid>();modelBuilder.Entity<ActionDAO>(entity =>
            {
                entity.ToTable("Action", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Action_Menu");
            });

            modelBuilder.Entity<ActionPageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PageId });

                entity.ToTable("ActionPageMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Action");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Page");
            });

            modelBuilder.Entity<AggregatedCounterDAO>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RefreshToken).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_AppUser_Image");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_Status");
            });

            modelBuilder.Entity<AppUserRoleMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.RoleId });

                entity.ToTable("AppUserRoleMapping", "PER");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_AppUser");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_Role");
            });

            modelBuilder.Entity<CounterDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
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

            modelBuilder.Entity<FieldDAO>(entity =>
            {
                entity.ToTable("Field", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Field_FieldType");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Field_Menu");
            });

            modelBuilder.Entity<FieldTypeDAO>(entity =>
            {
                entity.ToTable("FieldType", "PER");

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

            modelBuilder.Entity<HashDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
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

            modelBuilder.Entity<JobDAO>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameterDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueueDAO>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MenuDAO>(entity =>
            {
                entity.ToTable("Menu", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Menu_Site");
            });

            modelBuilder.Entity<PageDAO>(entity =>
            {
                entity.ToTable("Page", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<PermissionDAO>(entity =>
            {
                entity.ToTable("Permission", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Menu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Role");
            });

            modelBuilder.Entity<PermissionActionMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PermissionId });

                entity.ToTable("PermissionActionMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionActionMapping_Action");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionActionMapping_Permission");
            });

            modelBuilder.Entity<PermissionContentDAO>(entity =>
            {
                entity.ToTable("PermissionContent", "PER");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Field");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Permission");

                entity.HasOne(d => d.PermissionOperator)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionOperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_PermissionOperator");
            });

            modelBuilder.Entity<PermissionOperatorDAO>(entity =>
            {
                entity.ToTable("PermissionOperator", "PER");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.PermissionOperators)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionOperator_FieldType");
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
                entity.ToTable("Role", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Site");
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<ServerDAO>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SiteDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Value).HasMaxLength(500);
            });

            modelBuilder.Entity<StateDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
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
