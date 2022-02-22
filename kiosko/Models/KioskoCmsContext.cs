using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace kiosko.Models
{
    public partial class KioskoCmsContext : DbContext
    {
        public KioskoCmsContext()
        {
        }

        public KioskoCmsContext(DbContextOptions<KioskoCmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Componente> Componentes { get; set; } = null!;
        public virtual DbSet<Modulo> Modulos { get; set; } = null!;
        public virtual DbSet<Submodulo> Submodulos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-ESMK0VC;Database=KioskoCms;Trusted_Connection=true;user=sa;password=123456789;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("componentes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgregarFondo).HasColumnName("agregar_fondo");

                entity.Property(e => e.BackgroundColor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("background_color");

                entity.Property(e => e.Descripcion)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Orden).HasColumnName("orden");

                entity.Property(e => e.Padre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("padre");

                entity.Property(e => e.Subtitulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("subtitulo");

                entity.Property(e => e.TipoComponente)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tipo_componente");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("titulo");

                entity.Property(e => e.Url)
                    .IsUnicode(false)
                    .HasColumnName("url");
            });

            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.ToTable("modulos");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccesoDirecto).HasColumnName("acceso_directo");

                entity.Property(e => e.Desplegable).HasColumnName("desplegable");

                entity.Property(e => e.IdModulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("id_modulo");

                entity.Property(e => e.Orden).HasColumnName("orden");

                entity.Property(e => e.Padre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("padre");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("titulo");
            });

            modelBuilder.Entity<Submodulo>(entity =>
            {
                entity.ToTable("submodulos");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccesoDirecto).HasColumnName("acceso_directo");

                entity.Property(e => e.Desplegable).HasColumnName("desplegable");

                entity.Property(e => e.IdModulo).HasColumnName("id_modulo");

                entity.Property(e => e.IdSubmodulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("id_submodulo");

                entity.Property(e => e.Nivel).HasColumnName("nivel");

                entity.Property(e => e.Padre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("padre");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("titulo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
