using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaFilterRegex_18 : Regex
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
				if (num <= inputSpan.Length - 11)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfAnyStrings_OrdinalIgnoreCase_1FB3A96AB46DC539F5A40167F476758299B0E25C98263B03FE8E04D9B0FBEEB7);
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (span.IsEmpty)
				{
					return false;
				}
				switch (span[0])
				{
				case 'V':
				case 'v':
					if ((uint)span.Length < 3u || (span[1] | 0x20) != 112 || (span[2] | 0x20) != 112)
					{
						return false;
					}
					num += 3;
					span = inputSpan.Slice(num);
					break;
				case 'S':
				case 's':
					if ((uint)span.Length < 5u || !span.Slice(1).StartsWith("cale", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					num += 5;
					span = inputSpan.Slice(num);
					break;
				default:
					return false;
				}
				char c;
				if ((uint)span.Length < 7u || !span.StartsWith("_r", StringComparison.OrdinalIgnoreCase) || ((((c = span[2]) | 0x20) != 107) & (c != 'K')) || !span.Slice(3).StartsWith("rga", StringComparison.OrdinalIgnoreCase) || span[6] != '=')
				{
					return false;
				}
				num += 7;
				span = inputSpan.Slice(num);
				int i;
				for (i = 0; (uint)i < (uint)span.Length; i++)
				{
					if (!(((c = span[i]) < '\u0080') ? (("쇿\uffff\uef7a\uffff\uffff\uffff\uffff\uffff"[(int)c >> 4] & (1 << (c & 0xF))) != 0) : RegexRunner.CharInClass(c, "\u0001\u0006\u0001\"#'(,-d")))
					{
						break;
					}
				}
				if (i == 0)
				{
					return false;
				}
				span = span.Slice(i);
				Capture(0, start, runtextpos = num + i);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaFilterRegex_18 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaFilterRegex_18();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaFilterRegex_18()
	{
		pattern = "(?:vpp|scale)_rkrga=[^,\\s\"']+";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
