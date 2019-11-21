using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AndroidApp.FilterUtils
{
    class TimeLock
    {

        static readonly string TAG = typeof(TimeLock).Name.ToString();

        static string LockedFormat(DateTime locked)
        {
            return "Locked until: " + locked.ToString() + ", Left: " +
                string.Format("{0:%d}days {0:%h}h {0:%m}m {0:%s}sec", (locked - DateTime.Now));
        }


        public static string ForceLockDate(DateTime date)
        {
            string result = "";
            try
            {
                string unlockPath = Filenames.LOCK_DATE.getAppPrivate();

                AndroidBridge.d(TAG, "(*) Locking until '" + date.ToString() + "'");
                if (File.Exists(unlockPath))
                    File.Delete(unlockPath);
                File.WriteAllText(unlockPath, date.ToString());

                result = "Sucess! Locked to " + date.ToString();
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG,ex);
                result = ex.ToString();
            }
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

        
        public static TaskResult isLocked()
        {
            // Fail - not locked

            TaskResult isLocked = TaskResult.Fail("No issue (init)"); // unlocked on error by default
            try
            {
                if (File.Exists(Filenames.LOCK_DATE.getAppPrivate()))
                {
                    DateTime unlockDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));
                    if (DateTime.TryParse(File.ReadAllText(Filenames.LOCK_DATE.getAppPrivate()), out unlockDate))
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

        public static async Task onlyUnlockedAsync(Func<Task> callback)
        {
            try
            {
                var isLocked = TimeLock.isLocked();
                if (isLocked)
                {
                    AndroidBridge.ToastIt(isLocked.eventReason);
                }
                else
                {
                    await callback();
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
            }
        }

        public static async Task onlyUnlocked(Action callback)
        {
            await onlyUnlockedAsync(async () => await Task.Run(callback));
        }
    }
}
