﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskSharp;

#nullable disable

namespace TaskSharp.Migrations
{
    [DbContext(typeof(NotesContext))]
    partial class NotesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("TaskSharp.Classes.BaseNote", b =>
                {
                    b.Property<int>("BaseNoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Pinned")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserName")
                        .HasColumnType("INTEGER");

                    b.HasKey("BaseNoteId");

                    b.HasIndex("UserId");

                    b.ToTable("BaseNotes");
                });

            modelBuilder.Entity("TaskSharp.Classes.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TaskSharp.Classes.BaseNote", b =>
                {
                    b.HasOne("TaskSharp.Classes.User", "User")
                        .WithMany("TaskList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TaskSharp.Classes.User", b =>
                {
                    b.Navigation("TaskList");
                });
#pragma warning restore 612, 618
        }
    }
}
