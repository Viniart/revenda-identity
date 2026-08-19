using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(customer => customer.Name)
            .HasColumnName("name")
            .HasMaxLength(Customer.MaxNameLength)
            .IsRequired();

        builder.Property(customer => customer.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(11)
            .HasConversion(cpf => cpf.Value, value => Cpf.Create(value))
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasColumnName("email")
            .HasMaxLength(Email.MaxLength)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .IsRequired();

        builder.Property(customer => customer.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(customer => customer.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(customer => customer.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(customer => customer.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(customer => customer.Email).IsUnique().HasDatabaseName("ix_customers_email");
        builder.HasIndex(customer => customer.Cpf).IsUnique().HasDatabaseName("ix_customers_cpf");
    }
}
