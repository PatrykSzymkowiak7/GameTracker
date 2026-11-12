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
    public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
    {
        public void Configure(EntityTypeBuilder<Developer> builder)
        {
            builder.HasIndex(d => d.Name)
                .IsUnique();

            builder.Navigation(d => d.Games)
                .HasField("_games")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
