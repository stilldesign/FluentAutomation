﻿using FluentAutomation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace FluentAutomation
{
	public class FluentSettings
	{
		private static ConcurrentDictionary<int, FluentSettings> current = new ConcurrentDictionary<int, FluentSettings>();

		public static FluentSettings Current
		{
			get
			{
				if (!current.ContainsKey(Thread.CurrentThread.ManagedThreadId))
					current.TryAdd(Thread.CurrentThread.ManagedThreadId, new FluentSettings());

				return current[Thread.CurrentThread.ManagedThreadId];
			}
			set
			{
				//if (!current.ContainsKey(Thread.CurrentThread.ManagedThreadId))
				//	current.TryAdd(Thread.CurrentThread.ManagedThreadId, new FluentSettings());

				current[Thread.CurrentThread.ManagedThreadId] = value;
			}
		}

		public FluentSettings()
		{
			// Toggle features on/off
			this.WaitOnAllExpects = false;
			this.WaitOnAllAsserts = true;
			this.WaitOnAllActions = true;
			this.MinimizeAllWindowsOnTestStart = false;
			this.ScreenshotOnFailedExpect = false;
			this.ScreenshotOnFailedAssert = false;
			this.ScreenshotOnFailedAction = false;
			this.ExpectIsAssert = false; // determine if Expects are treated as Asserts (v2.x behavior)

			// browser size
			this.WindowHeight = null;
			this.WindowWidth = null;
			this.WindowMaximized = false;

			// timeouts
			this.WaitTimeout = TimeSpan.FromSeconds(1);
			this.WaitUntilTimeout = TimeSpan.FromSeconds(5);
			this.WaitUntilInterval = TimeSpan.FromMilliseconds(100);

			// paths
			this.UserTempDirectory = System.IO.Path.GetTempPath();
			this.ScreenshotPath = this.UserTempDirectory;

			// IoC registration
			this.ContainerRegistration = (c) => { };

			// events
			this.OnExpectFailed = (ex, state) =>
			{
				var fluentException = ex.InnerException as FluentException;
				if (fluentException != null)
					System.Diagnostics.Trace.WriteLine("[EXPECT FAIL] " + fluentException.Message);
				else
					System.Diagnostics.Trace.WriteLine("[EXPECT FAIL] " + ex.Message);
			};

			this.OnAssertFailed = (ex, state) =>
			{
				var fluentException = ex.InnerException as FluentException;
				if (fluentException != null)
					System.Diagnostics.Trace.WriteLine("[ASSERT FAIL] " + fluentException.Message);
				else
					System.Diagnostics.Trace.WriteLine("[ASSERT FAIL] " + ex.Message);
			};
		}

		internal FluentSettings Clone()
		{
			return (FluentSettings)this.MemberwiseClone();
		}

		public bool WaitOnAllExpects { get; set; }
		public bool WaitOnAllAsserts { get; set; }
		public bool WaitOnAllActions { get; set; }
		public bool MinimizeAllWindowsOnTestStart { get; set; }
		public bool ExpectIsAssert { get; set; }
		public bool ScreenshotOnFailedExpect { get; set; }
		public bool ScreenshotOnFailedAssert { get; set; }
		public bool ScreenshotOnFailedAction { get; set; }
		public int? WindowHeight { get; set; }
		public int? WindowWidth { get; set; }
		public bool WindowMaximized { get; set; }
		public TimeSpan WaitTimeout { get; set; }
		public TimeSpan WaitUntilTimeout { get; set; }
		public TimeSpan WaitUntilInterval { get; set; }
		public string ScreenshotPath { get; set; }
		public string ScreenshotPrefix { get; set; }
		public string UserTempDirectory { get; set; }
		public Action<TinyIoC.TinyIoCContainer> ContainerRegistration { get; set; }
		public Action<FluentExpectFailedException, WindowState> OnExpectFailed { get; set; }
		public Action<FluentAssertFailedException, WindowState> OnAssertFailed { get; set; }
	}
}
