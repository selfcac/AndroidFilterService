using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Droid
{
    
    [Service(Exported = false, Name = "com.yoniwas.selfcac.MyForegroundService")]
    class MyForegroundService : Service
    {

        static readonly string TAG = typeof(MyForegroundService).Name.ToString();

        public const string MAIN_NOTIFICATION_CHANNEL_ID = "com.yoniwas.selfcac.MyForegroundService.NotificaitionChannel";

        public const string ACTION_START_SERVICE = "MyForegroundService.action.START_SERVICE";
        public const string ACTION_RESTART_SERVICE = "MyForegroundService.action.RESTART_SERVICE";
        public const string ACTION_STOP_SERVICE = "MyForegroundService.action.STOP_SERVICE";
        public const string ACTION_MAIN_ACTIVITY = "MyForegroundService.action.MAIN_ACTIVITY";

        public const string SERVICE_STARTED_KEY = "has_service_been_started";

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        // ==================================================================================================
        // ==================================================================================================

        public static void StartForegroundServiceCompat<T>(Context context, string ACTION, Bundle args = null) where T : Service
        {
            try
            {
                var intent = new Intent(context, typeof(T));
                intent.SetAction(ACTION);
                if (args != null)
                {
                    intent.PutExtras(args);
                }

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    context.StartForegroundService(intent);
                }
                else
                {
                    context.StartService(intent);
                }
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex.ToString());
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            MyLogger.d(TAG, "OnBind");
            return null;
        }

        public NotificationChannel createNotificationChannel()
        {
            string chanName = "Foreground Service";
            var importance = NotificationImportance.Min; // Hide in oreo+ (bottom of notif list)

            var channel = new NotificationChannel(MAIN_NOTIFICATION_CHANNEL_ID, chanName, importance);
            NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
            manager.CreateNotificationChannel(channel);

            return channel;
        }

        public override void OnCreate()
        {
            // Should be called once in lifetime of this service    
            try
            {
                AndroidBridge.OnForgroundServiceStart?.Invoke();
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex);   
            }
        }

        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        NotificationCompat.Action BuildAction(string ACTION,int drawableIcon, string caption)
        {
            var actionIntent = new Intent(this, GetType());
            actionIntent.SetAction(ACTION);
            var pendingIntent = PendingIntent.GetService(this, 0, actionIntent, 0);

            var builder = new NotificationCompat.Action.Builder(drawableIcon,caption,pendingIntent);
            return builder.Build();

        }

        private void StartMyForegroundService()
        {
            try
            {
                // Code not directly related to publishing the notification has been omitted for clarity.
                // Normally, this method would hold the code to be run when the service is started.
                var notifChannel = createNotificationChannel();

                var notification = new NotificationCompat.Builder(this)
                    .SetContentTitle("selfCAC")
                    .SetContentText("selfCAC Filtering Service is Running in the back.")
                    .SetSmallIcon(Resource.Drawable.ic_notfiy_icon)
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    .SetOngoing(true)
                    .AddAction(BuildAction(ACTION_RESTART_SERVICE, Android.Resource.Drawable.IcMediaPause, "Restart"))
                    .SetChannelId(notifChannel.Id)
                    .SetPriority((int)NotificationPriority.Min) // Hide in oreo+
                    .Build();

                // Enlist this instance of the service as a foreground service
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex.ToString());
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Called everytime someone try to start (even if already started) or stop us or custom even we set up.
            try
            {
                if (intent.Action != null)
                {
                    switch (intent.Action)
                    {
                        case ACTION_START_SERVICE:
                            // Will have no affect if already running:
                            //                      (onCreate will not called)
                            StartMyForegroundService(); 
                            break;

                        case ACTION_RESTART_SERVICE:
                            AndroidBridge.OnForgroundServiceStop?.Invoke();
                            AndroidBridge.OnForgroundServiceStart?.Invoke();
                            break;

                        case ACTION_STOP_SERVICE:
                            AndroidBridge.OnForgroundServiceStop?.Invoke();
                            StopForeground(true);
                            StopSelf();
                            break;

                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex.ToString());
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            try
            {
                AndroidBridge.OnForgroundServiceStop?.Invoke();

                // Remove the notification from the status bar.
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex.ToString());
            }
        }


    }
}