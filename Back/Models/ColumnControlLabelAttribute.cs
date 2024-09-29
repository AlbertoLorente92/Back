using Back.Enums;

namespace Back.Models
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ColumnControlLabelAttribute : Attribute
    {
        public ColumnProperty Label { get; }

        public ColumnControlLabelAttribute(ColumnProperty label)
        {
            Label = label;
        }
    }
}
