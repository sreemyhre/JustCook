using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeVault.Core.Entities;

namespace RecipeVault.Infrastructure.Data.Configurations;

public class PantryStapleConfiguration : IEntityTypeConfiguration<PantryStaple>
{
    public void Configure(EntityTypeBuilder<PantryStaple> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
