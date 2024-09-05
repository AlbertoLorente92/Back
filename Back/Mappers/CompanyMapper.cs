﻿using Back.Enums;
using Back.Models;

namespace Back.Mappers
{
    public static class CompanyMapper
    {
        public static CompanyEntity MapToEntity(CreateCompanyRequest request, Guid guid, int id)
        {
            return new CompanyEntity
            {
                Guid = guid,
                Id = id,
                Name = request.Name,
                ComercialName = request.ComercialName,
                Vat = request.Vat,
                Deleted = false,
                CreationDate = DateTime.UtcNow
            };
        }

        public static ErrorCodes Map(SaveCompanyCode source)
        {
            return source switch
            {
                SaveCompanyCode.VatAlreadyExists => ErrorCodes.VatAlreadyExists,
                SaveCompanyCode.UnknownError => ErrorCodes.UnknownError,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        public static ErrorCodes Map(UpdateCompanyCode source)
        {
            return source switch
            {
                UpdateCompanyCode.VatAlreadyExists => ErrorCodes.VatAlreadyExists,
                UpdateCompanyCode.UnknownError => ErrorCodes.UnknownError,
                UpdateCompanyCode.CompanyDoesNotExist => ErrorCodes.CompanyDoesNotExist,
                UpdateCompanyCode.UnmodifiableProperty => ErrorCodes.UnmodifiableProperty,
                UpdateCompanyCode.UniqueProperty => ErrorCodes.UnmodifiableProperty,
                UpdateCompanyCode.NonExistentProperty => ErrorCodes.NonExistentProperty,
                UpdateCompanyCode.PropertyCastingError => ErrorCodes.PropertyCastingError,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }
}
