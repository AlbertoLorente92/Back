using Back.Enums;

namespace Back.Models
{
    public sealed class UserEntity : IEquatable<UserEntity>
    {
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
        public required Guid Guid { get; set; }
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
        public required int Id { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required string Name { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required string LastName { get; set; }
        [ColumnControlLabel(ColumnProperty.Unique)]
        public required string Email { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required string Password { get; set; }
        [ColumnControlLabel(ColumnProperty.NotEmpty)]
        public required string Salt { get; set; }
        public required bool Deleted { get; set; }
        [ColumnControlLabel(ColumnProperty.Unmodifiable)]
        public required DateTime CreationDate { get; set; }

        public override bool Equals(object? obj)
        {

            if (ReferenceEquals(this, obj)) return true;

            if (obj is UserEntity other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(UserEntity? other)
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
                $"Guid:         {Guid}\n" +
                $"Id:           {Id}\n" +
                $"Name:         {Name}\n" +
                $"LastName:     {LastName}\n" +
                $"Email:        {Email}\n" +
                $"Password:     {Password}\n" +
                $"Salt:         {Salt}\n" +
                $"Deleted:      {Deleted}\n" +
                $"CreationDate: {CreationDate}";
        }
    }
}
