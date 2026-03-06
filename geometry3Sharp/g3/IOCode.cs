using System;

namespace g3
{
	public enum IOCode
	{
		Ok,
		FileAccessError,
		UnknownFormatError,
		FormatNotSupportedError,
		InvalidFilenameError,
		FileParsingError = 100,
		GarbageDataError,
		GenericReaderError,
		GenericReaderWarning,
		WriterError = 200,
		ComputingInWorkerThread = 1000
	}
}
