using System;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#nullable disable
namespace HRManagement.Infrastructure.Migrations
{
    [DbContext(typeof(HRManagementDbContext))]
    [Migration("20260102204047_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);
            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
            modelBuilder.Entity("HRManagement.Domain.Entities.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<int>("Action")
                        .HasColumnType("int");
                    b.Property<Guid>("ActorId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("Details")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");
                    b.Property<Guid>("EntityId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");
                    b.HasKey("Id");
                    b.HasIndex("ActorId");
                    b.HasIndex("Timestamp");
                    b.HasIndex("EntityType", "EntityId");
                    b.ToTable("AuditLogs");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<Guid?>("ManagerId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<int>("Role")
                        .HasColumnType("int");
                    b.Property<int>("Status")
                        .HasColumnType("int");
                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");
                    b.HasKey("Id");
                    b.HasIndex("Email")
                        .IsUnique();
                    b.HasIndex("ManagerId");
                    b.HasIndex("TeamId");
                    b.ToTable("Employees");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.LeaveRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid?>("ApprovedByHRId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");
                    b.Property<DateTime?>("HRApprovedAt")
                        .HasColumnType("datetime2");
                    b.Property<DateTime?>("ManagerReviewedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");
                    b.Property<string>("RejectionReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");
                    b.Property<Guid?>("ReviewedByManagerId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");
                    b.Property<int>("Status")
                        .HasColumnType("int");
                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("datetime2");
                    b.HasKey("Id");
                    b.HasIndex("EmployeeId");
                    b.ToTable("LeaveRequests");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid?>("ManagerId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.HasKey("Id");
                    b.HasIndex("ManagerId");
                    b.ToTable("Teams");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.AuditLog", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", "Actor")
                        .WithMany()
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Actor");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Employee", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", "Manager")
                        .WithMany("DirectReports")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Restrict);
                    b.HasOne("HRManagement.Domain.Entities.Team", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.Navigation("Manager");
                    b.Navigation("Team");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.LeaveRequest", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", "Employee")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Employee");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Team", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.Navigation("Manager");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Employee", b =>
                {
                    b.Navigation("DirectReports");
                    b.Navigation("LeaveRequests");
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Team", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
