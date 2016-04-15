﻿using Brook.ZhiHuRiBao.Authorization;
using Brook.ZhiHuRiBao.Common;
using Brook.ZhiHuRiBao.Pages;
using Brook.ZhiHuRiBao.Utils;
using LLQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XPHttp;
using XPHttp.Serializer;

namespace Brook.ZhiHuRiBao
{
    sealed partial class App : Application
    {
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception.ToString());
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            InitConfig();
            InitHttpClient();
            InitTileScheduler();
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            Window.Current.Activate();
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = GetWindowFrame();
            if (rootFrame == null)
                return;

            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        public static Frame GetWindowFrame()
        {
            return Window.Current.Content as Frame;
        }

        public void InitConfig()
        {
            LLQNotifier.MainDispatcher = Window.Current.Dispatcher;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        void InitHttpClient()
        {
            XPHttpClient.DefaultClient.HttpConfig
                .SetBaseUrl(Urls.BaseUrl)
                .SetUseHttpCache(false)
                .ApplyConfig();
        }

        void InitTileScheduler()
        {
            if (StorageUtil.IsTileInited())
                return;

            try
            {
                const string baseTileUrl = "http://7xpj2g.com1.z0.glb.clouddn.com/Tile{0}.xml";
                StorageUtil.SetTileInited();
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

                var uris = new List<Uri>();
                for (int i = 1; i < 6; i++)
                {
                    uris.Add(new Uri(string.Format(baseTileUrl, i)));
                }
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(uris, PeriodicUpdateRecurrence.HalfHour);
            }
            catch { }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
