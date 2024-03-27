using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WPFPersonalTracking.DB;

public partial class PersonaltrackingContext : DbContext
{
    public PersonaltrackingContext()
    {
    }

    public PersonaltrackingContext(DbContextOptions<PersonaltrackingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Permissionstate> Permissionstates { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Salary> Salaries { get; set; }

    public virtual DbSet<Salarymonth> Salarymonths { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Taskstate> Taskstates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=iOnur\\SQLEXPRESS; Database=PERSONALTRACKING; trusted_connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().ToTable(tb => tb.HasTrigger("TriggerName"));
        modelBuilder.Entity<Position>().ToTable(tb => tb.HasTrigger("TriggerName"));
        modelBuilder.Entity<Department>().ToTable(tb => tb.HasTrigger("TriggerName"));
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("DEPARTMENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("EMPLOYEE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Adress).IsUnicode(false);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.ImagePath).IsUnicode(false);
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EMPLOYEE_DEPARTMENT");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EMPLOYEE_POSITION");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("PERMISSION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISSION_EMPLOYEE");

            entity.HasOne(d => d.PermissionStateNavigation).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.PermissionState)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISSION_PERMISSIONSTATE");
        });

        modelBuilder.Entity<Permissionstate>(entity =>
        {
            entity.ToTable("PERMISSIONSTATE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.StateName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("POSITION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Positions)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_POSITION_DEPARTMENT");
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.ToTable("SALARY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.MonthId).HasColumnName("MonthID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Salaries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SALARY_EMPLOYEE");

            entity.HasOne(d => d.Month).WithMany(p => p.Salaries)
                .HasForeignKey(d => d.MonthId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SALARY_MONTHS");
        });

        modelBuilder.Entity<Salarymonth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MONTHS");

            entity.ToTable("SALARYMONTH");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MonthName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("TASK");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.TaskContent).IsUnicode(false);
            entity.Property(e => e.TaskTitle)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TASK_EMPLOYEE");

            entity.HasOne(d => d.TaskStateNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskState)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TASK_TASKSTATE");
        });

        modelBuilder.Entity<Taskstate>(entity =>
        {
            entity.ToTable("TASKSTATE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.StateName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
