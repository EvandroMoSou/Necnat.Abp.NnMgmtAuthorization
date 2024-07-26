using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security;
using Volo.Abp;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SimpleStateChecking;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public class RoleDefinition :
        IHasSimpleStateCheckers<RoleDefinition>
    {
        public string Name { get; }

        /// <summary>
        /// Parent of this Role if one exists.
        /// If set, this Role can be granted only if parent is granted.
        /// </summary>
        public RoleDefinition? Parent { get; private set; }

        /// <summary>
        /// MultiTenancy side.
        /// Default: <see cref="MultiTenancySides.Both"/>
        /// </summary>
        public MultiTenancySides MultiTenancySide { get; set; }

        /// <summary>
        /// A list of allowed providers to get/set value of this Role.
        /// An empty list indicates that all providers are allowed.
        /// </summary>
        public List<string> Providers { get; }

        public List<ISimpleStateChecker<RoleDefinition>> StateCheckers { get; }

        public ILocalizableString DisplayName
        {
            get => _displayName;
            set => _displayName = Check.NotNull(value, nameof(value));
        }
        private ILocalizableString _displayName = default!;

        public IReadOnlyList<RoleDefinition> Children => _children.ToImmutableList();
        private readonly List<RoleDefinition> _children;

        public IReadOnlyList<string> Permissions => _permissions.ToImmutableList();
        private readonly List<string> _permissions;

        /// <summary>
        /// Can be used to get/set custom properties for this Role definition.
        /// </summary>
        public Dictionary<string, object?> Properties { get; }

        /// <summary>
        /// Indicates whether this Role is enabled or disabled.
        /// A Role is normally enabled.
        /// A disabled Role can not be granted to anyone, but it is still
        /// will be available to check its value (while it will always be false).
        ///
        /// Disabling a Role would be helpful to hide a related application
        /// functionality from users/clients.
        ///
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets/sets a key-value on the <see cref="Properties"/>.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <returns>
        /// Returns the value in the <see cref="Properties"/> dictionary by given <paramref name="name"/>.
        /// Returns null if given <paramref name="name"/> is not present in the <see cref="Properties"/> dictionary.
        /// </returns>
        public object? this[string name]
        {
            get => Properties.GetOrDefault(name);
            set => Properties[name] = value;
        }

        protected internal RoleDefinition(
            string name,
            ILocalizableString? displayName = null,
            MultiTenancySides multiTenancySide = MultiTenancySides.Both,
            bool isEnabled = true)
        {
            Name = Check.NotNull(name, nameof(name));
            DisplayName = displayName ?? new FixedLocalizableString(name);
            MultiTenancySide = multiTenancySide;
            IsEnabled = isEnabled;

            Properties = new Dictionary<string, object?>();
            Providers = new List<string>();
            StateCheckers = new List<ISimpleStateChecker<RoleDefinition>>();
            _children = new List<RoleDefinition>();
            _permissions = new List<string>();
        }

        public virtual RoleDefinition AddChild(
            string name,
            ILocalizableString? displayName = null,
            MultiTenancySides multiTenancySide = MultiTenancySides.Both,
            bool isEnabled = true)
        {
            var child = new RoleDefinition(
                name,
                displayName,
                multiTenancySide,
                isEnabled)
            {
                Parent = this
            };

            _children.Add(child);

            return child;
        }

        public virtual void AddPermission(string permission)
        {
            _permissions.Add(permission);
        }

        /// <summary>
        /// Sets a property in the <see cref="Properties"/> dictionary.
        /// This is a shortcut for nested calls on this object.
        /// </summary>
        public virtual RoleDefinition WithProperty(string key, object value)
        {
            Properties[key] = value;
            return this;
        }

        /// <summary>
        /// Adds one or more providers to the <see cref="Providers"/> list.
        /// This is a shortcut for nested calls on this object.
        /// </summary>
        public virtual RoleDefinition WithProviders(params string[] providers)
        {
            if (!providers.IsNullOrEmpty())
            {
                Providers.AddIfNotContains(providers);
            }

            return this;
        }

        public override string ToString()
        {
            return $"[{nameof(RoleDefinition)} {Name}]";
        }
    }
}
