using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework
{
    public static class Utils
    {
        public static string GetRealTypeName(this Type type)
        {
            string typeName = type.Name;
            int tildeIndex = typeName.IndexOf('`');

            if (tildeIndex == -1) return typeName;

            StringBuilder builder = new StringBuilder();
            builder.Append(typeName.Substring(0, tildeIndex));
            builder.Append("<");
            builder.Append(string.Join(", ", type.GetGenericArguments().Select(GetRealTypeName)));
            builder.Append(">");

            return builder.ToString();
        }
    }
}