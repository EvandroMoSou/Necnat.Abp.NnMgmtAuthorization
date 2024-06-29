using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Extensions;
using Necnat.Abp.NnLibCommon.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public static class HierarchyComponentGroupValidator
    {
        public static List<string>? Validate(HierarchyComponentGroupDto dto, IStringLocalizer stringLocalizer)
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
                return string.Format(stringLocalizer[ValidationMessages.Required], HierarchyComponentGroupConsts.NameDisplay);

            if (value!.Length > HierarchyComponentGroupConsts.MaxNameLength)
                return string.Format(stringLocalizer[ValidationMessages.MaxLength], HierarchyComponentGroupConsts.NameDisplay, HierarchyComponentGroupConsts.MaxNameLength);

            return null;
        }

        public static string? ValidateIsActive(bool? value, IStringLocalizer stringLocalizer)
        {
            if (value == null)
                return string.Format(stringLocalizer[ValidationMessages.Required], HierarchyComponentGroupConsts.IsActiveDisplay);

            return null;
        }

        public static List<string>? Validate(HierarchyComponentGroupResultRequestDto resultRequestDto, IStringLocalizer stringLocalizer)
        {
            var lError = new List<string>();

            if (lError.Count > 0)
                return lError;

            return null;
        }
    }
}
