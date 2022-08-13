﻿namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	public sealed class Domain
	{
		private static readonly IFileManagementStrategy DefaultFileManagementStrategy;
		private static readonly ITestNamingStrategy DefaultTestNamingStrategy;

		internal static readonly DomainSettings Settings;

        private readonly IOsDirectory _directory;
        private readonly IOsFile _file;

		private IFileManagementStrategy _fileManagementStrategy;
		private ITestNamingStrategy _testNamingStrategy;

		static Domain()
		{
			DefaultFileManagementStrategy = new SimpleFileManagementStrategy();
			DefaultTestNamingStrategy = new AssertActArrangeStrategy();

			Settings = new DomainSettings
			{
				DefaultFile = string.Empty,
				FileManager = new FileManager(DefaultFileManagementStrategy, DefaultTestNamingStrategy),
			};
		}

		public Domain()
		{
			_directory = new OsDirectory();
			_file = new OsFile();

			_fileManagementStrategy = DefaultFileManagementStrategy;
			_testNamingStrategy = DefaultTestNamingStrategy;
		}

		public static Domain Get()
        {
            return new Domain();
        }

        public Domain UsingDefaultFile(string path)
        {
            Settings.DefaultFile = path;
            return this;
        }

        public Domain UsingTestNamingConvention(ITestNamingStrategy strategy)
        {
	        _testNamingStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

        public Domain UsingFileManager(IFileManagementStrategy strategy)
        {
	        _fileManagementStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

		public void Setup()
        {
	        Settings.FileManager = new FileManager(_fileManagementStrategy, _testNamingStrategy);

			Settings.FileManager.Setup(
				_directory, 
				_file, 
				ConfigurationManager.AppSettings,
				AssemblyHelper.ExecutingDirectory);
        }

		public void TearDown()
        {
	        Settings.FileManager.TearDown();
		}
    }
}
