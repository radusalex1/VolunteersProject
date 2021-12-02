﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VolunteersProject.Data;

namespace VolunteersProject.Migrations
{
    [DbContext(typeof(VolunteersContext))]
    partial class VolunteersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VolunteersProject.Models.Contribution", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Credits")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FinishDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("VolunteerDeadlineConfirmation")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Contributions");
                });

            modelBuilder.Entity("VolunteersProject.Models.Enrollment", b =>
                {
                    b.Property<int>("EnrollmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("VolunteerID")
                        .HasColumnType("int");

                    b.Property<int>("VolunteerStatus")
                        .HasColumnType("int");

                    b.Property<int>("contributionId")
                        .HasColumnType("int");

                    b.HasKey("EnrollmentID");

                    b.HasIndex("VolunteerID");

                    b.HasIndex("contributionId");

                    b.ToTable("Enrollments");
                });

            modelBuilder.Entity("VolunteersProject.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VolunteersProject.Models.Volunteer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionContributionToHub")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FaceBookProfile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstagramProfile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinHubDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Volunteers");
                });

            modelBuilder.Entity("VolunteersProject.Models.Enrollment", b =>
                {
                    b.HasOne("VolunteersProject.Models.Volunteer", "volunteer")
                        .WithMany("Enrollments")
                        .HasForeignKey("VolunteerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VolunteersProject.Models.Contribution", "contribution")
                        .WithMany("Enrollments")
                        .HasForeignKey("contributionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("contribution");

                    b.Navigation("volunteer");
                });

            modelBuilder.Entity("VolunteersProject.Models.Contribution", b =>
                {
                    b.Navigation("Enrollments");
                });

            modelBuilder.Entity("VolunteersProject.Models.Volunteer", b =>
                {
                    b.Navigation("Enrollments");
                });
#pragma warning restore 612, 618
        }
    }
}
