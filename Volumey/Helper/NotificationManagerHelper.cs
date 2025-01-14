﻿using System;
using System.Drawing;
using log4net;
using Notification.Wpf;
using Notification.Wpf.Controls;
using Volumey.Controls;
using Volumey.DataProvider;
using Volumey.Model;
using Volumey.View.Controls;
using Volumey.ViewModel.Settings;
using Brush = System.Windows.Media.Brush;

namespace Volumey.Helper
{
	internal static class NotificationManagerHelper
	{
		private static NotificationManager _notificationManager;
		private static TimeSpan NotificationDisplayTime;

		private const string _exampleNotificationId = "E7D8A20F-94C6-4027-A53A-267938F9FD90";
		public const int MinIndent = 15;
		public const int MaxIndent = 100;

		private static ILog _logger;
		private static ILog Logger => _logger ??= LogManager.GetLogger(typeof(NotificationManagerHelper));

		static NotificationManagerHelper()
		{
			_notificationManager = new NotificationManager();
			_notificationManager.ChangeNotificationAreaPosition(SettingsProvider.NotificationsSettings.Position);
			_notificationManager.SetNotificationAreaHorizontalIndent(SettingsProvider.NotificationsSettings.HorizontalIndent);
			_notificationManager.SetNotificationAreaVerticalIndent(SettingsProvider.NotificationsSettings.VerticalIndent);
			_notificationManager.SetNotificationsMaxDisplayCount(3);
			
			NotificationDisplayTime = TimeSpan.FromSeconds(SettingsProvider.NotificationsSettings.DisplayTimeInSeconds);
		}

		internal static void ShowNotification(IManagedAudioSession session)
		{
			try
			{
				var content = new VolumeNotificationContent(session);
				_notificationManager.ShowNotification(content, session.Id, true, NotificationDisplayTime);
			}
			catch(ObjectDisposedException) { }
			catch(Exception e)
			{
				Logger.Error("Failed to display notification", e);
			}
		}

		internal static void CloseNotification(IManagedAudioSession session)
		{
			try
			{
				_notificationManager.CloseNotification(session.Id);
			}
			catch(ObjectDisposedException) { }
			catch(Exception e)
			{
				Logger.Error("Failed to close notification", e);
			}
		}

		internal static void ShowNotificationExample()
		{
			try
			{
				var dummySession = new DummyAudioSession(50, false, "Application", IconHelper.GenericExeIcon);
				var content = new VolumeNotificationContent(dummySession);
				_notificationManager.ShowNotification(content, _exampleNotificationId, false, TimeSpan.MaxValue);
			}
			catch(ObjectDisposedException) { }
			catch(Exception e)
			{
				Logger.Error("Failed to display example notification", e);
			}
		}

		internal static void CloseNotificationExample()
		{
			try
			{
				_notificationManager.CloseNotification(_exampleNotificationId);
			}
			catch(ObjectDisposedException) { }
			catch(Exception e)
			{
				Logger.Error("Failed to close example notification", e);
			}
		}

		internal static void ChangePosition(NotificationPosition newPosition)
		{
			_notificationManager.ChangeNotificationAreaPosition(newPosition);

		}

		internal static void SetVerticalIndent(int indent)
		{
			_notificationManager.SetNotificationAreaVerticalIndent(indent);
		}

		internal static void SetHorizontalIndent(int indent)
		{
			_notificationManager.SetNotificationAreaHorizontalIndent(indent);
		}

		internal static void UpdateColors(Brush background, Brush foreground, Brush hover)
		{
			_notificationManager.UpdateColors(background, foreground, hover);
		}

		internal static void SetNotificationDisplayTime(int timeInSeconds)
		{
			NotificationDisplayTime = TimeSpan.FromSeconds(timeInSeconds);
		}

		private class DummyAudioSession : BaseAudioSession
		{
			public DummyAudioSession(int volume, bool isMuted, string id, Icon icon, AudioSessionStateNotificationMediator audioSessionStateNotificationMediator = null) :
				base(volume, isMuted, id, icon, audioSessionStateNotificationMediator)
			{
				this.Name = id;
			}
			public override string Name { get; set; }

			public override bool SetVolumeHotkeys(HotKey volUp, HotKey volDown)
				=> false;

			public override void ResetVolumeHotkeys() { }
		}
	}
}