using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Configs
{
    internal class FileConfiguration : AuditPersistenceBaseConfig<FilePersistence>, IEntityTypeConfiguration<FilePersistence>
    {
        public void Configure(EntityTypeBuilder<FilePersistence> builder)
        {
            builder.ToTable("Files");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.FolderId)
                .IsRequired(false);

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Path)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(x => x.MimeType)
                .HasMaxLength(127)
                .IsRequired();

            builder.Property(x => x.SizeBytes)
                .IsRequired();

            builder.Property(x => x.StorageKey)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Checksum)
                .HasMaxLength(64)
                .IsRequired();

            ConfigureAudit(builder);

            builder.HasIndex(x => x.Path);
            builder.HasIndex(x => x.FolderId);
            builder.HasIndex(x => x.CreatedByUserId);

            builder.HasOne(x => x.Folder)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.FolderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
