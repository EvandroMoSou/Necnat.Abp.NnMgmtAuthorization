using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Extensions;
using Necnat.Abp.NnLibCommon.Validators;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public static class AuthEndpointValidator
    {
        public static List<string>? Validate(AuthEndpointDto dto, IStringLocalizer stringLocalizer)
        {
            var lError = new List<string>();

            lError.AddIfNotIsNullOrWhiteSpace(ValidateDisplayName(dto.DisplayName, stringLocalizer));
            lError.AddIfNotIsNullOrWhiteSpace(ValidateEndpoint(dto.Endpoint, stringLocalizer));

            if (lError.Count > 0)
                return lError;

            return null;
        }

        public static string? ValidateDisplayName(string? value, IStringLocalizer stringLocalizer)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Format(stringLocalizer[ValidationMessages.Required], AuthEndpointConsts.DisplayNameDisplay);

            if (value!.Length > AuthEndpointConsts.MaxDisplayNameLength)
                return string.Format(stringLocalizer[ValidationMessages.MaxLength], AuthEndpointConsts.DisplayNameDisplay, AuthEndpointConsts.MaxDisplayNameLength);

            return null;
        }

        public static string? ValidateEndpoint(string? value, IStringLocalizer stringLocalizer)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Format(stringLocalizer[ValidationMessages.Required], AuthEndpointConsts.EndpointDisplay);

            if (value!.Length > AuthEndpointConsts.MaxEndpointLength)
                return string.Format(stringLocalizer[ValidationMessages.MaxLength], AuthEndpointConsts.EndpointDisplay, AuthEndpointConsts.MaxEndpointLength);

            return null;
        }

        public static string? ValidateIsAuthentication(bool? value, IStringLocalizer stringLocalizer)
        {
            if (value == null)
                return string.Format(stringLocalizer[ValidationMessages.Required], AuthEndpointConsts.IsAuthenticationDisplay);

            return null;
        }

        public static string? ValidateIsActive(bool? value, IStringLocalizer stringLocalizer)
        {
            if (value == null)
                return string.Format(stringLocalizer[ValidationMessages.Required], AuthEndpointConsts.IsActiveDisplay);

            return null;
        }

        public static List<string>? Validate(AuthEndpointResultRequestDto resultRequestDto, IStringLocalizer stringLocalizer)
        {
            var lError = new List<string>();

            if (lError.Count > 0)
                return lError;

            return null;
        }
    }
}
