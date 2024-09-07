using Back.Enums;

namespace Back.Models
{
    public sealed class CompanyEntity : IEquatable<CompanyEntity>
    {
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
        public required Guid Guid { get; set; }
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
        public required int Id { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required string Name { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public string ComercialName { get; set; } = string.Empty;
        [ColumnControlLabel(ColumnProperty.Unique)]
        public required string Vat { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required bool Deleted { get; set; }
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
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

            return Guid.Equals(other.Guid) && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Id);
        }

        public override string ToString()
        {
            return 
                $"Guid:          {Guid}\n" +
                $"Id:            {Id}\n" +
                $"Name:          {Name}\n" +
                $"ComercialName: {ComercialName}\n" +
                $"Vat:           {Vat}\n" +
                $"Deleted:       {Deleted}\n" +
                $"CreationDate:  {CreationDate}";
        }
    }
}
