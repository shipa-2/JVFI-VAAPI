using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Main10ProfileRegex_9 : Regex
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
				if (num <= inputSpan.Length - 15)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfString_2DDC8227357669D296F342A57C08560F8489D0E671EB728C17B081DCA84DD2AC);
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
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				span = inputSpan.Slice(num);
				int num3 = num;
				if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
				{
					CheckTimeout();
				}
				if ((uint)(num - 1) < inputSpan.Length && !char.IsWhiteSpace(inputSpan[num - 1]))
				{
					num--;
					return false;
				}
				num = num3;
				span = inputSpan.Slice(num);
				if ((uint)span.Length < 10u || !span.StartsWith("-profile:v", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num += 10;
				span = inputSpan.Slice(num);
				num2 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
					num2++;
					if (!span.StartsWith(":0"))
					{
						goto IL_00ce;
					}
					num += 2;
					span = inputSpan.Slice(num);
					if (num2 == 0)
					{
						continue;
					}
					goto IL_00ef;
					IL_0166:
					if ((uint)span.Length >= 6u && span.Slice(1).StartsWith("ain1", StringComparison.OrdinalIgnoreCase) && (span[5] | 2) == 50)
					{
						num += 6;
						span = inputSpan.Slice(num);
						break;
					}
					goto IL_00ce;
					IL_00ce:
					if (--num2 < 0)
					{
						return false;
					}
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_00ef;
					IL_00ef:
					int i;
					for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
					{
					}
					if (i != 0)
					{
						span = span.Slice(i);
						num += i;
						if (!span.IsEmpty)
						{
							char c = span[0];
							if ((uint)c <= 82u)
							{
								if (c == 'M')
								{
									goto IL_0166;
								}
								if (c != 'R')
								{
									goto IL_00ce;
								}
							}
							else
							{
								if (c == 'm')
								{
									goto IL_0166;
								}
								if (c != 'r')
								{
									goto IL_00ce;
								}
							}
							if ((uint)span.Length >= 4u && span.Slice(1).StartsWith("ext", StringComparison.OrdinalIgnoreCase))
							{
								num += 4;
								span = inputSpan.Slice(num);
								break;
							}
						}
					}
					goto IL_00ce;
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Main10ProfileRegex_9 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Main10ProfileRegex_9();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Main10ProfileRegex_9()
	{
		pattern = "(?<!\\S)-profile:v(?::0)?\\s+(?:main10|main12|rext)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
