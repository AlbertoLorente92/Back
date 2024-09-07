using Back.Models;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Resources;

namespace UnitTest.Common
{
    public class ResourceManagerStringLocalizer : IStringLocalizer<SharedResource>
    {
        private readonly ResourceManager _resourceManager;

        public ResourceManagerStringLocalizer(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var value = _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
                var formattedValue = string.Format(value ?? name, arguments);
                return new LocalizedString(name, formattedValue, resourceNotFound: value == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            yield break;
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}
