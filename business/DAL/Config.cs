using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace business.DAL
{
    public class Config
    {
        public static readonly string DataPath = Directory.GetCurrentDirectory() + @"\App_Data\bcr.db";
    }
}
