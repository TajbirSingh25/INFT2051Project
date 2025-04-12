using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mauiapp1.Services;
using System;
using System.IO;

namespace Mauiapp1
{
    public partial class App : Application
    {
        public static string DatabasePath { get; private set; }

        public App()
        {
            InitializeComponent();

            // Make sure your database path is valid
            DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "products.db3");

            // Register database service with the proper path
            var dbService = new DatabaseService(DatabasePath);

            // Register service in DependencyService
            DependencyService.RegisterSingleton<IDatabaseService>(dbService);

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}