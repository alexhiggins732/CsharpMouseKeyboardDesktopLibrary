using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseKeyNetwork
{
    class Program
    {
        public static void Main(string[] args)
        {
            var n3 = KeysEnumNetworkOfFloat.Create(Keys.None);
            var ns = n3.ToString();
            GenerateKeysEnumNetwork();
            var n = new KeysEnumNetworkOfFloat();
            var n2 = KeysEnumNetworkOfFloat.Create(Keys.Shift | Keys.A);
            var n4 = KeysEnumNetworkOfFloat.Create(Keys.Control | Keys.Alt | Keys.Delete);

        }
        public static void GenerateKeysEnumNetwork()
        {
            var className = $"KeysEnumNetwork";
          
            var projectFolder = GetProjectFolder();
            var filePath = Path.Combine(projectFolder.FullName, $"{className}.cs");
            var code = NetworkGenerator.GenerateNetwork<Keys>(className);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, code);
            }
                
        }

        private static DirectoryInfo GetProjectFolder()
        {
            var path = Path.GetFullPath(".");
            var dir = new DirectoryInfo(path);
            var projectFiles = dir.GetFiles("*.csproj");
            while (projectFiles.Length == 0)
            {
                dir = dir.Parent;
                projectFiles = dir.GetFiles("*.csproj");
            }
            return dir;

        }
    }
}
