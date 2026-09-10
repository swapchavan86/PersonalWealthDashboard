using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalWealth.Domain.Identity;
namespace PersonalWealth.Infrastructure.Persistence.Configurations;
public sealed class TenantConfiguration:IEntityTypeConfiguration<Tenant>{public void Configure(EntityTypeBuilder<Tenant>b){b.ToTable("Tenants");b.HasKey(x=>x.Id);b.Property(x=>x.Name).HasMaxLength(200).IsRequired();b.HasIndex(x=>x.Name).IsUnique();}}
public sealed class UserIdentityConfiguration:IEntityTypeConfiguration<UserIdentity>{public void Configure(EntityTypeBuilder<UserIdentity>b){b.ToTable("UserIdentities");b.HasKey(x=>x.Id);b.Property(x=>x.Subject).HasMaxLength(200).IsRequired();b.Property(x=>x.Email).HasMaxLength(320).IsRequired();b.HasIndex(x=>new{x.TenantId,x.Subject}).IsUnique();b.HasIndex(x=>new{x.TenantId,x.Email});}}
