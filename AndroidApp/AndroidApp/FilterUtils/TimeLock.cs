using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class TimeLock
    {

        static readonly string TAG = typeof(TimeLock).Name.ToString();
      

        public static string ForceLockDate(DateTime date)
        {
            string result;
            string unlockPath = Filenames.LOCK_DATE;

            AndroidBridge.d(TAG, "(*) Locking until '" + date.ToString() + "'");
            if (File.Exists(unlockPath))
                File.Delete(unlockPath);
            File.WriteAllText(unlockPath, date.ToString());

            result = "Sucess! Locked to " + date.ToString();
            return result;
        }

        public static string TrySetLockDate(DateTime date)
        {
            string result = "Unkown lock result";
            TaskResult unlockedStatus = isLocked();
            // Lock it!
            try
            {
                if (!unlockedStatus) // Only if not already locked!
                {
                    if (date > DateTime.Now)
                    {
                        result = ForceLockDate(date);
                    }
                    else
                    {
                        result = "Please choose *future* time";
                    }
                }
                else
                {
                    result = unlockedStatus.eventReason;
                }
            }
            catch (Exception ex)
            {
                result = "Failed lock: " + ex.Message;
            }
            return result;
        }

        static string LockedFormat(DateTime locked)
        {
            return "Locked until: " + locked.ToString() + ", Left: " +
                string.Format("{0:%d}days {0:%h}h {0:%m}m {0:%s}sec", (locked - DateTime.Now));
        }

        public static TaskResult isLocked()
        {
            // Fail - not locked

            TaskResult isLocked = TaskResult.Fail("No issue (init)"); // unlocked on error by default
            try
            {
                if (File.Exists(Filenames.LOCK_DATE))
                {
                    DateTime unlockDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));
                    if (DateTime.TryParse(File.ReadAllText(Filenames.LOCK_DATE), out unlockDate))
                    {
                        if (unlockDate > DateTime.Now)
                        {
                            isLocked = TaskResult.Success(LockedFormat(unlockDate));
                        }
                        else
                        {
                            isLocked = TaskResult.Fail(reason: "Lock expired");
                        }
                    }
                }
                else
                {
                    isLocked = TaskResult.Fail(reason: "No file exist");
                }
            }
            catch (Exception ex)
            {
                isLocked = TaskResult.Fail(reason: ex.ToString());
            }
            return isLocked;
        }


    }
}
