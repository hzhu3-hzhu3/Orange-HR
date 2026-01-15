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
    [Migration("20260109165105_AddLeaveBalanceSystem")]
    partial class AddLeaveBalanceSystem
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
                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");
                    b.Property<int>("AnnualLeaveQuota")
                        .HasColumnType("int");
                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");
                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");
                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");
                    b.Property<Guid?>("ManagerId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");
                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");
                    b.Property<int>("Role")
                        .HasColumnType("int");
                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");
                    b.Property<int>("Status")
                        .HasColumnType("int");
                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");
                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.HasKey("Id");
                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");
                    b.HasIndex("ManagerId");
                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");
                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");
                    b.HasIndex("TeamId");
                    b.ToTable("AspNetUsers", (string)null);
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.LeaveBalance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<int>("PendingDays")
                        .HasColumnType("int");
                    b.Property<int>("TotalDays")
                        .HasColumnType("int");
                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");
                    b.Property<int>("UsedDays")
                        .HasColumnType("int");
                    b.Property<int>("Year")
                        .HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("EmployeeId", "Year")
                        .IsUnique();
                    b.ToTable("LeaveBalances");
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
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.HasKey("Id");
                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");
                    b.ToTable("AspNetRoles", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("RoleId");
                    b.ToTable("AspNetRoleClaims", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("UserId");
                    b.ToTable("AspNetUserClaims", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("LoginProvider", "ProviderKey");
                    b.HasIndex("UserId");
                    b.ToTable("AspNetUserLogins", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("UserId", "RoleId");
                    b.HasIndex("RoleId");
                    b.ToTable("AspNetUserRoles", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");
                    b.HasKey("UserId", "LoginProvider", "Name");
                    b.ToTable("AspNetUserTokens", (string)null);
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
            modelBuilder.Entity("HRManagement.Domain.Entities.LeaveBalance", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", "Employee")
                        .WithMany("LeaveBalances")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Employee");
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
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("HRManagement.Domain.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("HRManagement.Domain.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("HRManagement.Domain.Entities.Employee", b =>
                {
                    b.Navigation("DirectReports");
                    b.Navigation("LeaveBalances");
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
