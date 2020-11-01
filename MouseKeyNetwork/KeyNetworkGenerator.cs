using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
namespace MouseKeyNetwork
{
    public class NetworkGenerator
    {
        public static string GenerateNetwork<T>(string className)
            where T : Enum
        {
            const string tab = "    ";
            var keyType = typeof(T);
            var typeName = keyType.Name;
            var names = Enum.GetNames(keyType);
            var values = Enum.GetValues(keyType);
            var d = new Dictionary<string, T>();
            foreach (var name in names)
            {
                var value = (T)Enum.Parse(keyType, name);
                d.Add(name, value);
            }
            var sb = new StringBuilder();
            var t = typeof(NetworkGenerator);

            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using {keyType.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {t.Namespace}");
            sb.AppendLine($"{{");
            sb.AppendLine($"{tab}public class {className}<TNumeric>");
            sb.AppendLine($"{tab}{{");
            sb.AppendLine(@"        public override string ToString()
        {{
            return string.Join("" | "",
                this.GetType()
                .GetFields()
                .Where(x => x.FieldType == typeof(TNumeric) && ToInt((TNumeric)x.GetValue(this)) != 0)
                .Select(x=> x.Name));
        }}");
            sb.AppendLine(@"        public virtual int ToInt(TNumeric numeric) => Convert.ToInt32(numeric);");
            foreach (var kvp in d)
            {
                sb.AppendLine($"{ tab}{tab}public TNumeric {kvp.Key};");
            }
            sb.AppendLine($"{tab}}}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"{tab}public class {className}OfFloat: {className}<float>");
            sb.AppendLine($"{tab}{{");

            string typeVarName = $"{char.ToLower(typeName[0])}{typeName.Substring(1)}";
            sb.AppendLine($"{tab}{tab}public static {className}OfFloat Create({typeName} {typeVarName})");
            sb.AppendLine($"{tab}{tab}{{");

            sb.AppendLine(string.Format(@"            var thisType = typeof({0}OfFloat);
            var type = typeof({1});
            var result = new {0}OfFloat();
            if ((int)keys == 0)
            {{
                if (Enum.IsDefined(type, 0))
                {{
                    var name = Enum.GetName(type, 0);
                    var field = thisType.GetField(name);
                    field.SetValue(result, 1.0f);
                }}
            }} 
            else
            {{
                var names = Enum.GetNames(type);
                foreach (var name in names)
                {{
                    var value = ({1})Enum.Parse(type, name);
                    if ((int)value == 0) continue;
                    if (({2} & value) == value)
                    {{
                        var field = thisType.GetField(name);
                        field.SetValue(result, 1.0f);
                    }}
                }}
            }}
            return result;",


            className, typeName, typeVarName));

            sb.AppendLine($"{tab}{tab}}}");
            sb.AppendLine($"{tab}}}");
            sb.AppendLine($"}}");
            var code = sb.ToString();
            return code;
        }
    }
}
