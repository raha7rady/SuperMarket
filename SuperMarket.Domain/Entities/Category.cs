
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities
{
    public class Category : AuditableEntity
    {
        private const int MaxTitleLength = 200;
        private const int MaxImageUrlLength = 500;
        private const int MaxDescriptionLength = 1000;
        private const int MaxBadgeLength = 50;

        private readonly List<Product> _products = new();

        public string Title { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Description { get; private set; }
        public string? Badge { get; private set; }
        public IReadOnlyCollection<Product> Products => _products;

        private Category() { }

        public Category(string title, int displayOrder = 0)
        {
            SetTitle(title);
            SetDisplayOrder(displayOrder);
            IsActive = true;
        }

        public void Update(string title, int displayOrder, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            SetTitle(title);
            SetDisplayOrder(displayOrder);

            if (modifiedBy.HasValue)
                SetModified(modifiedBy.Value);
        }

        public void UpdateCatalogDetails(
            string? imageUrl,
            string? description,
            string? badge,
            Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            ImageUrl = NormalizeOptional(imageUrl, MaxImageUrlLength);
            Description = NormalizeOptional(description, MaxDescriptionLength);
            Badge = NormalizeOptional(badge, MaxBadgeLength);

            if (modifiedBy.HasValue)
                SetModified(modifiedBy.Value);
        }

        public void SetActive(bool active, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            if (IsActive == active) return;

            IsActive = active;

            if (modifiedBy.HasValue)
                SetModified(modifiedBy.Value);
        }

        public void AddProduct(Product product)
        {
            EnsureNotDeleted();

            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (_products.Any(p => p.Id == product.Id))
                return;

            _products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            EnsureNotDeleted();

            if (product is null)
                throw new ArgumentNullException(nameof(product));

            _products.RemoveAll(p => p.Id == product.Id);
        }

        public override void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;

            base.SoftDelete(deletedBy);

            foreach (var product in _products.Where(p => !p.IsDeleted))
                product.SoftDelete(deletedBy);
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            title = title.Trim();

            if (title.Length > MaxTitleLength)
                throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(title));

            Title = title;
            Slug = GenerateSlug(title);
        }

        private void SetDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
                throw new ArgumentException("DisplayOrder cannot be negative.", nameof(displayOrder));

            DisplayOrder = displayOrder;
        }

        private static string? NormalizeOptional(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (value.Length > maxLength)
                throw new ArgumentException($"Value cannot exceed {maxLength} characters.", nameof(value));

            return value;
        }

        private static string GenerateSlug(string title)
        {
            var slug = title.Trim().ToLowerInvariant();

            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return string.IsNullOrWhiteSpace(slug)
                ? Guid.NewGuid().ToString("N")[..8]
                : slug;
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted category.");
        }
    }
}
