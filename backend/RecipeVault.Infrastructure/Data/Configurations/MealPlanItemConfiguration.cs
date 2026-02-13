using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeVault.Core.Entities;

namespace RecipeVault.Infrastructure.Data.Configurations;

public class MealPlanItemConfiguration : IEntityTypeConfiguration<MealPlanItem>
{
    public void Configure(EntityTypeBuilder<MealPlanItem> builder)
    {
        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.DayOfWeek)
            .IsRequired();

        builder.Property(mi => mi.MealType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(mi => mi.MealPlan)
            .WithMany(m => m.Items)
            .HasForeignKey(mi => mi.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mi => mi.Recipe)
            .WithMany()
            .HasForeignKey(mi => mi.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
