using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputLabelRegex_6 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 3)
				{
					ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
					int num2;
					for (num2 = 0; num2 < readOnlySpan.Length - 2; num2++)
					{
						int num3 = readOnlySpan.Slice(num2).IndexOf('[');
						if (num3 < 0)
						{
							break;
						}
						num2 += num3;
						if ((uint)(num2 + 1) >= (uint)readOnlySpan.Length)
						{
							break;
						}
						char c;
						if (((c = readOnlySpan[num2 + 1]) != '[') & (c != ']'))
						{
							runtextpos = num + num2;
							return true;
						}
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				num2 = num;
				if (readOnlySpan.IsEmpty || readOnlySpan[0] != '[')
				{
					UncaptureUntil(0);
					return false;
				}
				int num3 = readOnlySpan.Slice(1).IndexOfAny('[', ']');
				if (num3 < 0)
				{
					num3 = readOnlySpan.Length - 1;
				}
				if (num3 == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				readOnlySpan = readOnlySpan.Slice(num3);
				num += num3;
				if ((uint)readOnlySpan.Length < 2u || readOnlySpan[1] != ']')
				{
					UncaptureUntil(0);
					return false;
				}
				num += 2;
				readOnlySpan = inputSpan.Slice(num);
				Capture(1, num2, num);
				if (num < inputSpan.Length - 1 || ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n'))
				{
					UncaptureUntil(0);
					return false;
				}
				runtextpos = num;
				Capture(0, start, num);
				return true;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int capturePosition)
				{
					while (Crawlpos() > capturePosition)
					{
						Uncapture();
					}
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputLabelRegex_6 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputLabelRegex_6();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputLabelRegex_6()
	{
		pattern = "(\\[[^\\[\\]]+\\])$";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
