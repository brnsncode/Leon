using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Leon.Models;

public partial class Leon_Context : DbContext
{
    public Leon_Context()
    {
    }

    public Leon_Context(DbContextOptions<Leon_Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AppMetric> AppMetrics { get; set; }

    public virtual DbSet<AssignedTask> AssignedTasks { get; set; }

    public virtual DbSet<AssignedTaskNote> AssignedTaskNotes { get; set; }

    public virtual DbSet<DashboardStatus> DashboardStatuses { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<PlannerSyncHistory> PlannerSyncHistories { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<TicketTracker> TicketTrackers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=Leon_Dev");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppMetric>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.DateEntered)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EntryId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<AssignedTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Assigned__7C6949D1C759BF56");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.DateEntered)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsReminder).HasDefaultValue(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");
            entity.Property(e => e.SetReminderDate).HasColumnType("datetime");
            entity.Property(e => e.TaskName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Resource).WithMany(p => p.AssignedTasks)
                .HasForeignKey(d => d.ResourceId)
                .HasConstraintName("FK__AssignedT__Resou__44FF419A");
        });

        modelBuilder.Entity<AssignedTaskNote>(entity =>
        {
            entity.HasKey(e => e.NoteId);

            entity.Property(e => e.NoteId).HasColumnName("NoteID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Resource).WithMany(p => p.AssignedTaskNotes)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssignedTaskNotes_To_Resources");

            entity.HasOne(d => d.Task).WithMany(p => p.AssignedTaskNotes)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssignedTaskNotes_To_AssignedTasks");
        });

        modelBuilder.Entity<DashboardStatus>(entity =>
        {
            entity.HasKey(e => e.DashboardId).HasName("PK__Dashboar__C711E1D09FD95489");

            entity.ToTable("DashboardStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.OperationId).HasName("PK__Operatio__A4F5FC64F0119379");

            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.BusinessOwner)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OperationName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");

            entity.HasOne(d => d.Resource).WithMany(p => p.Operations)
                .HasForeignKey(d => d.ResourceId)
                .HasConstraintName("FK__Operation__Resou__46E78A0C");
        });

        modelBuilder.Entity<PlannerSyncHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PlannerSyncHistory");

            entity.Property(e => e.SyncDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SyncId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABED09E15C48A");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.BusinessOwner)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InitiativeId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("InitiativeID");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");

            entity.HasOne(d => d.Resource).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ResourceId)
                .HasConstraintName("FK__Projects__Resour__48CFD27E");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Resource__4ED1814FC28B085F");

            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");
            entity.Property(e => e.Adguid).HasColumnName("ADGuid");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TicketTracker>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TicketTracker");

            entity.Property(e => e.DateEntered)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TicketEntryId).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
