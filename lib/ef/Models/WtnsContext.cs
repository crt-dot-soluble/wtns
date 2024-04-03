using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace lib.ef.Models;

public partial class WTNSContext : DbContext
{
    public WTNSContext() { }

    public WTNSContext(DbContextOptions<WTNSContext> options)
        : base(options) { }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
        optionsBuilder.UseMySql(
            "server=localhost;database=WTNS;user id=root;password=root",
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.3.0-mysql")
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci").HasCharSet("utf8mb4");

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PRIMARY");

            entity.ToTable("posts");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PostContent).HasMaxLength(255);
            entity
                .Property(e => e.PostDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.UserName, "UserName").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Active).HasDefaultValueSql("b'1'").HasColumnType("bit(1)");
            entity.Property(e => e.Bio).HasMaxLength(512);
            entity.Property(e => e.DisplayName).HasMaxLength(32);
            entity.Property(e => e.Hash).HasMaxLength(60).IsFixedLength();
            entity.Property(e => e.Salt).HasMaxLength(32).IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(32);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
