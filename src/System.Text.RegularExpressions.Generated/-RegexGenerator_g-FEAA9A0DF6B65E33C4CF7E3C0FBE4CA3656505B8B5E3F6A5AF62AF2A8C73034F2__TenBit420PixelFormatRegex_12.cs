using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__TenBit420PixelFormatRegex_12 : Regex
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
				if (num <= inputSpan.Length - 4)
				{
					if (num < inputSpan.Length - 12)
					{
						num = inputSpan.Length - 12;
					}
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfAnyStrings_OrdinalIgnoreCase_903F20CB91CC8C379144B939FA94B2F2EB20C03ED7B2578837BEFD9A7EF535F2);
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
				int num2 = 0;
				int pos = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				if (readOnlySpan.IsEmpty)
				{
					return false;
				}
				switch (readOnlySpan[0])
				{
				case 'P':
				case 'p':
					if (!readOnlySpan.Slice(1).StartsWith("010"))
					{
						return false;
					}
					num += 4;
					readOnlySpan = inputSpan.Slice(num);
					break;
				case 'Y':
				case 'y':
					if ((uint)readOnlySpan.Length < 9u || !readOnlySpan.Slice(1).StartsWith("uv420p10", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					num += 9;
					readOnlySpan = inputSpan.Slice(num);
					break;
				default:
					return false;
				}
				num2 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
					num2++;
					if (!readOnlySpan.IsEmpty)
					{
						switch (readOnlySpan[0])
						{
						case 'L':
						case 'l':
							if ((uint)readOnlySpan.Length < 2u || (readOnlySpan[1] | 0x20) != 101)
							{
								break;
							}
							num += 2;
							readOnlySpan = inputSpan.Slice(num);
							goto IL_0151;
						case 'B':
						case 'b':
							if ((uint)readOnlySpan.Length < 2u || (readOnlySpan[1] | 0x20) != 101)
							{
								break;
							}
							num += 2;
							readOnlySpan = inputSpan.Slice(num);
							goto IL_0151;
						}
					}
					goto IL_0159;
					IL_017a:
					if (num >= inputSpan.Length - 1 && ((uint)num >= (uint)inputSpan.Length || inputSpan[num] == '\n'))
					{
						break;
					}
					goto IL_0159;
					IL_0151:
					if (num2 == 0)
					{
						continue;
					}
					goto IL_017a;
					IL_0159:
					if (--num2 < 0)
					{
						return false;
					}
					num = runstack[--pos];
					readOnlySpan = inputSpan.Slice(num);
					goto IL_017a;
				}
				runtextpos = num;
				Capture(0, start, num);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__TenBit420PixelFormatRegex_12 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__TenBit420PixelFormatRegex_12();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__TenBit420PixelFormatRegex_12()
	{
		pattern = "(?:p010|yuv420p10)(?:le|be)?$";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
