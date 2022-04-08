using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace kiosko.Models
{
    public partial class KioskoCmsContext : IdentityDbContext
    {
        public KioskoCmsContext()
        {
        }

        public KioskoCmsContext(DbContextOptions<KioskoCmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Componente> Componentes { get; set; } = null!;
        public virtual DbSet<Desplazante> Desplazantes { get; set; } = null!;
        public virtual DbSet<Modulo> Modulos { get; set; } = null!;
        public virtual DbSet<Progreso> Progresos { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public object AspeNetUser { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("componentes");

                entity.HasIndex(e => e.IdModulo, "IX_componentes_id_modulo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgregarFondo).HasColumnName("agregar_fondo");

                entity.Property(e => e.BackgroundColor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("background_color");

                entity.Property(e => e.Descripcion)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.IdModulo).HasColumnName("id_modulo");

                entity.Property(e => e.Orden).HasColumnName("orden");

                entity.Property(e => e.Padre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("padre");

                entity.Property(e => e.Subtitulo)
                    .IsUnicode(false)
                    .HasColumnName("subtitulo");

                entity.Property(e => e.TipoComponente)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tipo_componente");

                entity.Property(e => e.Titulo)
                    .IsUnicode(false)
                    .HasColumnName("titulo");

                entity.Property(e => e.Url)
                    .IsUnicode(false)
                    .HasColumnName("url");

                entity.Property(e => e.UrlDos)
                    .IsUnicode(false)
                    .HasColumnName("url_dos");

                entity.Property(e => e.UrlTres)
                    .IsUnicode(false)
                    .HasColumnName("url_tres");

                entity.HasOne(d => d.IdModuloNavigation)
                    .WithMany(p => p.Componentes)
                    .HasForeignKey(d => d.IdModulo)
                    .HasConstraintName("FK_componentes_modulos");
            });

            modelBuilder.Entity<Desplazante>(entity =>
            {
                entity.ToTable("desplazantes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdComponente).HasColumnName("id_componente");

                entity.Property(e => e.Texto)
                    .IsUnicode(false)
                    .HasColumnName("texto");

                entity.Property(e => e.Titulo)
                    .IsUnicode(false)
                    .HasColumnName("titulo");

                entity.Property(e => e.Url)
                    .IsUnicode(false)
                    .HasColumnName("url");

                entity.HasOne(d => d.IdComponenteNavigation)
                    .WithMany(p => p.Desplazantes)
                    .HasForeignKey(d => d.IdComponente)
                    .HasConstraintName("FK_desplazantes_componentes");
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

                entity.Property(e => e.TiempoInactividad).HasColumnName("tiempo_inactividad");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("titulo");
            });

            modelBuilder.Entity<Progreso>(entity =>
            {
                entity.ToTable("progresos");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FechaActualizacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_actualizacion");

                entity.Property(e => e.FechaFin)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_fin");

                entity.Property(e => e.FechaInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_inicio");

                entity.Property(e => e.Finalizado)
                    .HasColumnName("finalizado")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IdModulo).HasColumnName("id_modulo");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.Porcentaje).HasColumnName("porcentaje");

                entity.HasOne(d => d.IdModuloNavigation)
                    .WithMany(p => p.Progresos)
                    .HasForeignKey(d => d.IdModulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_progresos_modulos");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Progresos)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK_progresos_usuarios");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Clave)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("clave");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nombre_usuario");

                entity.Property(e => e.Rol)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rol");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
