using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Configs
{
    internal class FolderConfiguration : AuditPersistenceBaseConfig<FolderPersistence>, IEntityTypeConfiguration<FolderPersistence>
    {
        public void Configure(EntityTypeBuilder<FolderPersistence> builder)
        {
            builder.ToTable("Folders");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Path)
                .HasMaxLength(4000)
                .IsRequired();

            ConfigureAudit(builder);

            builder.HasIndex(x => x.Path);
            builder.HasIndex(x => x.ParentFolderId);

            builder.HasOne(x => x.ParentFolder)
                .WithMany(x => x.ChildFolders)
                .HasForeignKey(x => x.ParentFolderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Files)
                .WithOne(x => x.Folder)
                .HasForeignKey(x => x.FolderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
