﻿namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System;
	using System.Runtime.CompilerServices;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;

	public class Daten
	{
		private readonly string _callingMethodName;
		private readonly string _callingClassPath;
		private const string DoNotSet = "";

		private readonly IOsDirectory _osDirectory;
		private readonly IOsFile _osFile;

		private readonly Coordinator _coordinator;

		/// <summary>
		/// Retrieves data for the test that is currently executing.
		/// </summary>
		/// <param name="callingMethodName">Do not provide a value </param>
		/// <param name="callingClassPath">Do not provide a value.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public Daten([CallerMemberName] string callingMethodName = DoNotSet,
			[CallerFilePath] string callingClassPath = DoNotSet)
			: this(Lokator.Get(), callingMethodName, callingClassPath)
		{
			// nothing to do
		}

		internal Daten(
			Lokator lokator,
			[CallerMemberName] string callingMethodName = DoNotSet,
			[CallerFilePath] string callingClassPath = DoNotSet)
		{
			_callingMethodName = callingMethodName ?? throw new ArgumentNullException(nameof(callingMethodName));
			_callingClassPath = callingClassPath ?? throw new ArgumentNullException(nameof(callingClassPath));

			Lokator currentLokator = lokator ?? throw new ArgumentNullException(nameof(lokator));

			if (!lokator.IsSetup)
			{
				throw new InvalidOperationException("The test environment has not yet been initialized. Hint: Call Lokator.Setup().");
			}

			_osDirectory = currentLokator.OsDirectory;
			_osFile = currentLokator.OsFile;

			_coordinator = currentLokator.Coordinator;
		}

		private void ThrowIfFileMissing(string path)
		{
			if (!_osFile.Exists(path))
			{
				var sourceFile = System.IO.Path.GetFileName(path);
				var directoryPath = System.IO.Path.GetDirectoryName(path) + @"\";
				throw new System.IO.FileNotFoundException(
					$@"Unable to find the requested input file. Directory=`{directoryPath}`, File=`{sourceFile}`",
					path);
			}

			System.Console.WriteLine($"Source data has been selected. FileName=`{System.IO.Path.GetFileName(path)}`");
		}

		private string GetRegisteredPath(Using usingStrategy)
		{
			var defaultFilePath = string.Empty;

			switch (usingStrategy)
			{
				case Using.DefaultFileName:
					defaultFilePath = _coordinator.GetDefaultFilePath();
					break;

				default:
					throw new ArgumentOutOfRangeException(
						nameof(usingStrategy),
						$"This method expects the parameter to always be: {nameof(Using.DefaultFileName)}");
			}

			return defaultFilePath;
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsFilePath()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsFilePath(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="usingStrategy">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public string AsFilePath(Using usingStrategy)
		{
			var sourceFilePath = GetRegisteredPath(usingStrategy);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsString()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsString(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="usingStrategy">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public string AsString(Using usingStrategy)
		{
			var sourceFilePath = GetRegisteredPath(usingStrategy);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.Stream AsStream()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.Stream AsStream(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="usingStrategy">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public System.IO.Stream AsStream(Using usingStrategy)
		{
			var sourceFilePath = GetRegisteredPath(usingStrategy);

			ThrowIfFileMissing(sourceFilePath);

			return _osFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.StreamReader AsStreamReader()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_osFile.OpenRead(sourceFilePath));
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.StreamReader AsStreamReader(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_osFile.OpenRead(sourceFilePath));
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="usingStrategy">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public System.IO.StreamReader StreamReader(Using usingStrategy)
		{
			var sourceFilePath = GetRegisteredPath(usingStrategy);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_osFile.OpenRead(sourceFilePath));
		}
	}
}
