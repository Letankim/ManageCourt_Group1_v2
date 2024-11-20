using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DataAccess;

public partial class ManageCourtContext : DbContext
{
    public ManageCourtContext()
    {
    }

    public ManageCourtContext(DbContextOptions<ManageCourtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accessory> Accessories { get; set; }

    public virtual DbSet<BadmintonCourt> BadmintonCourts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingAccessory> BookingAccessories { get; set; }

    public virtual DbSet<CourtImage> CourtImages { get; set; }

    public virtual DbSet<CourtSchedule> CourtSchedules { get; set; }

    public virtual DbSet<EnableLog> EnableLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOtp> UserOtps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ManageCourt;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accessory>(entity =>
        {
            entity.HasKey(e => e.AccessoryId).HasName("PK__Accessor__09C3F09BCA67DAA5");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<BadmintonCourt>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PK__Badminto__C3A67C9AD66E022B");

            entity.Property(e => e.CourtName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.MapsLink).HasMaxLength(255);
            entity.Property(e => e.PricePerHour).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Owner).WithMany(p => p.BadmintonCourts)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Badminton__Owner__286302EC");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED9B051D16");

            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed");
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TimeSlot).HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Court).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__Bookings__CourtI__33D4B598");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Bookings__UserId__32E0915F");
        });

        modelBuilder.Entity<BookingAccessory>(entity =>
        {
            entity.HasKey(e => e.BookingAccessoryId).HasName("PK__BookingA__F48BC5817B04FC4E");
            entity.Property(e => e.BookingAccessoryId).ValueGeneratedOnAdd();
            entity.HasOne(d => d.Accessory).WithMany(p => p.BookingAccessories)
                .HasForeignKey(d => d.AccessoryId)
                .HasConstraintName("FK__BookingAc__Acces__3E52440B");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingAccessories)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingAc__Booki__3D5E1FD2");
        });

        modelBuilder.Entity<CourtImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__CourtIma__7516F70C61163F69");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Court).WithMany(p => p.CourtImages)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__CourtImag__Court__2C3393D0");
        });

        modelBuilder.Entity<CourtSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__CourtSch__9C8A5B496CDDC4EA");

            entity.ToTable("CourtSchedule");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.TimeSlot).HasMaxLength(20);

            entity.HasOne(d => d.Court).WithMany(p => p.CourtSchedules)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK__CourtSche__Court__2F10007B");
        });

        modelBuilder.Entity<EnableLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__EnableLo__5E548648AA57FE90");

            entity.ToTable("EnableLog");

            entity.Property(e => e.ChangedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EntityType).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C60B2A606");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.HasKey(e => e.UserOtpid).HasName("PK__UserOTP__47DF0256F57A3397");

            entity.ToTable("UserOTP");

            entity.Property(e => e.UserOtpid).HasColumnName("UserOTPId");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Otpcode)
                .HasMaxLength(6)
                .HasColumnName("OTPCode");

            entity.HasOne(d => d.User).WithMany(p => p.UserOtps)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserOTP__UserId__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
