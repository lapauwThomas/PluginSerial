using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSerialFormsGui
{
    internal abstract class ToastCallback
    {

        public abstract void Invoke(ToastNotificationActivatedEventArgsCompat notificationActivatedEventArgs);

    }
}
