using System;
using System.Collections.Generic;
using System.Text;
using a4crypt;

namespace GUI
{
    internal static class W
    {
        public static List<string> selectedFilePathList = [];
        public static bool IsSelectedFilesEncrypted = false;
        public static G.KeyStrengths SelectedKeyStrength = G.KeyStrengths.High;
        public static G.KeyTypes SelectedKeyType = G.KeyTypes.Argon2id;
    }
}
