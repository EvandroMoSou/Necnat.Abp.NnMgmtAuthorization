using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Extensions;
using Necnat.Abp.NnLibCommon.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public static class HierarchyValidator
    {
        public static List<string>? Validate(HierarchyDto dto, IStringLocalizer stringLocalizer)
        {
            var lError = new List<string>();

            lError.AddIfNotIsNullOrWhiteSpace(ValidateName(dto.Name, stringLocalizer));
            lError.AddIfNotIsNullOrWhiteSpace(ValidateIsActive(dto.IsActive, stringLocalizer));

            if (lError.Count > 0)
                return lError;

            return null;
        }

        public static string? ValidateName(string? value, IStringLocalizer stringLocalizer)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Format(stringLocalizer[ValidationMessages.Required], HierarchyConsts.NameDisplay);

            if (value!.Length > HierarchyConsts.MaxNameLength)
                return string.Format(stringLocalizer[ValidationMessages.MaxLength], HierarchyConsts.NameDisplay, HierarchyConsts.MaxNameLength);

            return null;
        }

        public static string? ValidateIsActive(bool? value, IStringLocalizer stringLocalizer)
        {
            if (value == null)
                return string.Format(stringLocalizer[ValidationMessages.Required], HierarchyConsts.IsActiveDisplay);

            return null;
        }

        public static List<string>? Validate(HierarchyResultRequestDto resultRequestDto, IStringLocalizer stringLocalizer)
        {
            var lError = new List<string>();

            if (lError.Count > 0)
                return lError;

            return null;
        }
    }
}
