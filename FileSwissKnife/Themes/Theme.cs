using System;
using System.Windows;

namespace FileSwissKnife.Themes
{
    public class Theme
    {
        public Theme(string name, ResourceDictionary resourceDictionary)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ResourceDictionary = resourceDictionary ?? throw new ArgumentNullException(nameof(resourceDictionary));
        }

        public string Name { get; }

        public ResourceDictionary ResourceDictionary { get; }
    }
}