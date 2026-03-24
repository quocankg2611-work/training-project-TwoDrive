using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Configs
{
    internal class FileConfiguration : IEntityTypeConfiguration<FileModel>
    {
        public void Configure(EntityTypeBuilder<FileModel> builder)
        {
            builder.ToTable("Files");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.FolderId)
                .IsRequired();

            builder.Property(x => x.OwnerId)
                .IsRequired();

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

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.HasIndex(x => x.Path);
            builder.HasIndex(x => x.FolderId);
            builder.HasIndex(x => x.OwnerId);

            builder.HasOne(x => x.Folder)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.FolderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
