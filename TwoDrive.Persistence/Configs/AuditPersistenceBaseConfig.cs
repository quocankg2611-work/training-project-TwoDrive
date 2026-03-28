using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Configs
{
    internal abstract class AuditPersistenceBaseConfig<T> where T : AuditPersistenceBase
    {
        protected static void ConfigureAudit(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.CreatedByUserId)
                .IsRequired();

            builder.Property(x => x.CreatedByUserNameSnapshot)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.UpdatedByUserId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.UpdatedByUserNameSnapshot)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();
        }
    }
}
