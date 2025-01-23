﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PForum.API.Data;

#nullable disable

namespace PForum.API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("PForum.Domain.Entities.LanguageTopic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("LanguageDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LanguageLogoUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LanguageTopics");
                });

            modelBuilder.Entity("PForum.Domain.Entities.Topic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LanguageTopicId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TopicDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TopicName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LanguageTopicId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("PForum.Domain.Entities.TopicThread", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ThreadDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ThreadName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicThreads");
                });

            modelBuilder.Entity("PForum.Domain.Entities.TopicThreadMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TopicThreadId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TopicThreadId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicThreadMessages");
                });

            modelBuilder.Entity("PForum.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PForum.Domain.Entities.Topic", b =>
                {
                    b.HasOne("PForum.Domain.Entities.LanguageTopic", "LanguageTopic")
                        .WithMany("Topics")
                        .HasForeignKey("LanguageTopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LanguageTopic");
                });

            modelBuilder.Entity("PForum.Domain.Entities.TopicThread", b =>
                {
                    b.HasOne("PForum.Domain.Entities.Topic", "Topic")
                        .WithMany("TopicThreads")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PForum.Domain.Entities.User", "User")
                        .WithMany("CreatedThreads")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Topic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PForum.Domain.Entities.TopicThreadMessage", b =>
                {
                    b.HasOne("PForum.Domain.Entities.TopicThread", "TopicThread")
                        .WithMany("Messages")
                        .HasForeignKey("TopicThreadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PForum.Domain.Entities.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TopicThread");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PForum.Domain.Entities.LanguageTopic", b =>
                {
                    b.Navigation("Topics");
                });

            modelBuilder.Entity("PForum.Domain.Entities.Topic", b =>
                {
                    b.Navigation("TopicThreads");
                });

            modelBuilder.Entity("PForum.Domain.Entities.TopicThread", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("PForum.Domain.Entities.User", b =>
                {
                    b.Navigation("CreatedThreads");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
