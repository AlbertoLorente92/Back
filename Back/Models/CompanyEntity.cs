namespace Back.Models
{
    public sealed class CompanyEntity : IEquatable<CompanyEntity>
    {
        public required Guid Guid { get; set; }
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string ComercialName { get; set; }
        public required string Vat { get; set; }
        public required bool Deleted { get; set; }
        public required DateTime CreationDate { get; set; }

        public override bool Equals(object? obj)
        {

            if (ReferenceEquals(this, obj)) return true;

            if (obj is CompanyEntity other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(CompanyEntity? other)
        {
            if (other is null) return false;

            return Guid.Equals(other.Guid) && Id.Equals(other.Id) && Vat.Equals(other.Vat);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Id);
        }
    }
}
