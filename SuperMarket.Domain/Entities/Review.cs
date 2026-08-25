using System;
using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities
{
    public class Review : AuditableEntity
    {
        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int MaxCommentLength = 2000;

        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public int Rating { get; private set; }
        public string Comment { get; private set; } = null!;

        private Review() { }

        public Review(Guid productId, Guid userId, int rating, string comment)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product id is required.", nameof(productId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required.", nameof(userId));

            ProductId = productId;
            UserId = userId;

            SetRating(rating);
            SetComment(comment);
        }

        public void Update(int rating, string comment, Guid modifiedBy)
        {
            EnsureNotDeleted();

            SetRating(rating);
            SetComment(comment);

            SetModified(modifiedBy);
        }

        private void SetRating(int rating)
        {
            if (rating < MinRating || rating > MaxRating)
                throw new ArgumentException($"Rating must be between {MinRating} and {MaxRating}.", nameof(rating));

            Rating = rating;
        }

        private void SetComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment is required.", nameof(comment));

            comment = comment.Trim();

            if (comment.Length > MaxCommentLength)
                throw new ArgumentException($"Comment cannot exceed {MaxCommentLength} characters.", nameof(comment));

            Comment = comment;
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted review.");
        }
    }
}
