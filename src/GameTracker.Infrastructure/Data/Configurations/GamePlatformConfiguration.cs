using GameTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Infrastructure.Data.Configurations
{
    public class GamePlatformConfiguration : IEntityTypeConfiguration<GamePlatform>
    {
        public void Configure(EntityTypeBuilder<GamePlatform> builder)
        {
            builder.HasIndex(gp => gp.Name)
                .IsUnique();

            builder.Navigation(gp => gp.Games)
                .HasField("_games")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
