using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TitanCommerce.Domain.Identity.Entities;

namespace TitanCommerce.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens", SchemaNames.Identity);

		builder.HasKey(rt => rt.Id);

		builder.Property(rt => rt.Token)
			.IsRequired()
			.HasMaxLength(256);

		// দ্রুত ভ্যালিডেশনের জন্য টোকেনে ইনডেক্স
		builder.HasIndex(rt => rt.Token)
			.IsUnique();

		builder.HasOne(rt => rt.User)
			.WithMany(u => u.RefreshTokens)
			.HasForeignKey(rt => rt.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}