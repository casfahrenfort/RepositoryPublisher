using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ThesisPrototype.Helpers
{
    public static class DirectoryHelper
    {
        public static void SetAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                SetAttributesNormal(subDir);
                subDir.Attributes = FileAttributes.Normal;
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
                file.IsReadOnly = false;
            }
        }
    }
}
