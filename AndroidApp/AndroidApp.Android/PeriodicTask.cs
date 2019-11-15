﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using JobSchedulerType = Android.App.Job.JobScheduler;

namespace AndroidApp.Droid
{
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    class PeriodicTask : JobService
    {
        static readonly string TAG = typeof(PeriodicTask).Name.ToString();

        private static int _job_counter = 0;
        private static object _job_counter_lock = new object();
        
        public static int getUniqueJobId()
        {
            var newId = -1;
            lock (_job_counter_lock)
            {
                 newId = _job_counter++;
            }
            return newId;
        }

        static Dictionary<int, JobCallbacks> allJobs = new Dictionary<int, JobCallbacks>();

        public class JobCallbacks
        {
            public Action<Action<bool>> onJob = null;
            public Func<bool> shouldContinue = null;

            public Action onJobRequirementAbort = null;
            public Func<bool> shouldRetryAfterAbort = null;
        }

        public static bool scheduleJob(Context ctx, JobCallbacks callbacks, TimeSpan minLatency, TimeSpan maxLatency, TimeSpan? interval = null,
            bool requireIdle = false, bool requireCharger = false, NetworkType requiredNet = NetworkType.Any,
            bool requireAboveLowBattery = false, bool requireAboveLowStorage = false)
        {
            int jobID = -1;
            bool success = false;

            jobID = getUniqueJobId();
            allJobs.Add(jobID, callbacks);

            try
            {
                ComponentName JobServiceComponent = new ComponentName(ctx, Java.Lang.Class.FromType(typeof(PeriodicTask)));

                var jobBuilder = new JobInfo
                    .Builder(jobID, JobServiceComponent)
                    .SetMinimumLatency((long)minLatency.TotalMilliseconds)
                    .SetOverrideDeadline((long)maxLatency.TotalMilliseconds)
                    .SetRequiresDeviceIdle(requireIdle)
                    .SetRequiresCharging(requireCharger)
                    .SetRequiredNetworkType(requiredNet)
                    .SetRequiresBatteryNotLow(requireAboveLowBattery)
                    .SetRequiresStorageNotLow(requireAboveLowStorage)
                    ;

                long intervalMillisecond = (long)(interval?.TotalMilliseconds ?? 0);
                if (intervalMillisecond > 0L)
                {
                    jobBuilder.SetPeriodic(intervalMillisecond);
                }

                JobInfo job = jobBuilder.Build();

                var manager = (JobSchedulerType)ctx.GetSystemService(Context.JobSchedulerService);
                var status = manager.Schedule(job);

                if (status != JobSchedulerType.ResultSuccess)
                {
                    AndroidLevelLogger.e(TAG, "Job schedule failed for id: " + jobID);
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                AndroidLevelLogger.e(TAG,ex);
            }

            if (!success && allJobs.ContainsKey(jobID))
            {
                allJobs.Remove(jobID);
            }

            return success;
        }

        public override StartCommandResult OnStartCommand(Intent intent, Android.App.StartCommandFlags flags, int startId)
        {
            return StartCommandResult.NotSticky;
        }

        public override bool OnStartJob(JobParameters @params)
        {
            bool shouldJobContinue = false; 

            int jobID = @params.JobId;
            bool hasUserFinished = false;
            bool invokeErrors = false;
            try
            {
                JobCallbacks callbacks = allJobs[jobID];
                Action<bool> jobFinishedProxy = 
                    (bool wantsReschedule) => {
                        hasUserFinished = true;
                        JobFinished(@params, wantsReschedule);
                    };

                callbacks.onJob?.Invoke(jobFinishedProxy);
                shouldJobContinue = callbacks.shouldContinue?.Invoke() ?? shouldJobContinue;

            }
            catch (Exception ex)
            {
                invokeErrors = true;
                AndroidLevelLogger.e(TAG, ex);
            }

            if (invokeErrors || (hasUserFinished && !shouldJobContinue))
            {
                shouldJobContinue = false;
                if (allJobs.ContainsKey(jobID)) allJobs.Remove(jobID);
            }

            return shouldJobContinue;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            // Called when a requierment not met any more (ex. idle became not-idle...)

            bool shouldJobContinue = false; // Once requirement met again
            bool invokeErrors = false;

            int jobID = @params.JobId;
            try
            {
                JobCallbacks callbacks = allJobs[jobID];

                callbacks.onJobRequirementAbort?.Invoke();
                shouldJobContinue = callbacks.shouldRetryAfterAbort?.Invoke() ?? shouldJobContinue;

                
            }
            catch (Exception ex)
            {
                invokeErrors = true;
                AndroidLevelLogger.e(TAG, ex);
            }

            if (invokeErrors || !shouldJobContinue)
            {
                shouldJobContinue = false;
                if (allJobs.ContainsKey(jobID)) allJobs.Remove(jobID);
            }

            return shouldJobContinue;
        }
    }
}