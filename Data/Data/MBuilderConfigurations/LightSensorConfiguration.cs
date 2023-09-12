using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Data.MBuilderConfigurations
{
    public class LightSensorConfiguration : IEntityTypeConfiguration<LightSensor>
    {
        public void Configure(EntityTypeBuilder<LightSensor> builder)
        {
            builder.HasOne<AppUser>()
                   .WithOne()
                   .HasForeignKey<LightSensor>(s => s.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.lightSensorTelemetries)
                   .WithOne(t => t.lightSensor)
                   .HasForeignKey(t => t.LightSensorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
