using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace CIV
{
    [MarkupExtensionReturnType(typeof(Array))]
    public class EnumValuesExtension : MarkupExtension
    {
        private CIVResourceManager resourceManager;

        public EnumValuesExtension()
        {
            resourceManager = new CIVResourceManager();
        }

        public EnumValuesExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //return Enum.GetValues(EnumType);

            Dictionary<Enum, String> ret = new Dictionary<Enum, string>();

            string text;

            foreach (Enum e in Enum.GetValues(EnumType))
            {
                text = CIV.strings.ResourceManager.GetString(e.ToString());
                if (String.IsNullOrEmpty(text))
                    ret.Add(e, e.ToString());
                else
                    ret.Add(e, text);
            }
            return ret;
        }
    }
}
