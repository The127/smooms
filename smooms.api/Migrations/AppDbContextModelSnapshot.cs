﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using smooms.api.Models;

#nullable disable

namespace smooms.api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("smooms.api.Models.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant>("LastAccessedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_accessed_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_session");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_session_user_id");

                    b.ToTable("session", (string)null);
                });

            modelBuilder.Entity("smooms.api.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<byte[]>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("hashed_password");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("salt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_user_email");

                    b.ToTable("user", null, t =>
                        {
                            t.HasCheckConstraint("CK_user_email_Length", "(\"email\" IS NULL OR LENGTH(\"email\") <= 320");

                            t.HasCheckConstraint("CK_user_email_Regex", "(\"email\" IS NULL OR \"email\" ~ '^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$'");

                            t.HasCheckConstraint("CK_user_user_name_Length", "LENGTH(\"user_name\") <= 1000");
                        });
                });

            modelBuilder.Entity("smooms.api.Models.Session", b =>
                {
                    b.HasOne("smooms.api.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_session_user_user_temp_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("smooms.api.Models.User", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
