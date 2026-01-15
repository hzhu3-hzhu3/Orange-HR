using HRManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Data;
public class HRManagementDbContext : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>
{
    public HRManagementDbContext(DbContextOptions<HRManagementDbContext> options)
        : base(options)
    {
    }
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
    public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Role)
                .IsRequired();
            entity.Property(e => e.Status)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.HasOne(e => e.Manager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(lr => lr.Id);
            entity.Property(lr => lr.Reason)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(lr => lr.RejectionReason)
                .HasMaxLength(500);
            entity.Property(lr => lr.StartDate)
                .IsRequired();
            entity.Property(lr => lr.EndDate)
                .IsRequired();
            entity.Property(lr => lr.Status)
                .IsRequired();
            entity.Property(lr => lr.SubmittedAt)
                .IsRequired();
            entity.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(lr => lr.IsReadOnly);
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasOne(t => t.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(al => al.Id);
            entity.HasIndex(al => al.Timestamp);
            entity.HasIndex(al => new { al.EntityType, al.EntityId });
            entity.Property(al => al.Action)
                .IsRequired();
            entity.Property(al => al.EntityType)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(al => al.Timestamp)
                .IsRequired();
            entity.Property(al => al.Details)
                .HasMaxLength(1000);
            entity.HasOne(al => al.Actor)
                .WithMany()
                .HasForeignKey(al => al.ActorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasKey(lb => lb.Id);
            entity.HasIndex(lb => new { lb.EmployeeId, lb.Year }).IsUnique();
            entity.Property(lb => lb.Year)
                .IsRequired();
            entity.Property(lb => lb.TotalDays)
                .IsRequired();
            entity.Property(lb => lb.UsedDays)
                .IsRequired();
            entity.Property(lb => lb.PendingDays)
                .IsRequired();
            entity.Property(lb => lb.CreatedAt)
                .IsRequired();
            entity.HasOne(lb => lb.Employee)
                .WithMany(e => e.LeaveBalances)
                .HasForeignKey(lb => lb.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(lb => lb.RemainingDays);
        });
    }
}
