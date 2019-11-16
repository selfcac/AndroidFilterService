using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class TimeLock
    {
        static readonly string TAG = typeof(TimeLock).Name.ToString();

        bool isLocked()
        {
            string filename = "";
            string result ="No issue (init)"; // unlocked on error by default
            bool isLocked = true;

            try
            {
                if (File.Exists(filename))
                {
                    DateTime unlock = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));
                    if (DateTime.TryParse(File.ReadAllText(filename), out unlock))
                    {
                        if (unlock > DateTime.Now)
                        {
                            isLocked = true;
                        }
                        else
                        {
                            isLocked = false;
                            result = "Lock expired";
                        }
                    }
                }
                else
                {
                    isLocked = false;
                    result = "File doesn't exits";
                }
            }
            catch (Exception ex)
            {
                isLocked = false;
                result = ex.ToString();
            }

            return isLocked;
        }
    }
}
