using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class TaskResult
    {
        public bool success;
        public string eventReason;

        public static TaskResult Fail(string reason)
        {
            return new TaskResult()
            {
                success = false,
                eventReason = reason
            };
        }

        public static TaskResult Success(string reason)
        {
            return new TaskResult()
            {
                success = true,
                eventReason = reason
            };
        }

        public static implicit operator bool(TaskResult e)
        {
            return e.success;
        }
    }

    class TaskResultExtra<T> : TaskResult
    {
        public T result;

        public static TaskResultExtra<T> setResult(T resultData)
        {
            return new TaskResultExtra<T>()
            {
                success = true,
                result = resultData
            };
        }

        public static implicit operator T(TaskResultExtra<T> o)
        {
            return o.result;
        }
    }




}
