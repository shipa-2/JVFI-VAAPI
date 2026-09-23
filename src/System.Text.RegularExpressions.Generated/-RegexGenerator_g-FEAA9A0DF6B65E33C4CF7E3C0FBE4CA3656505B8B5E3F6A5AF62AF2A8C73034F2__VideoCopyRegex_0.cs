using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCopyRegex_0 : Regex
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
				if (num <= inputSpan.Length - 9)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfString_FE9CC628257F93F75E4DF1EEB6C03E59185FD8BF19304994FD3E7E4D5C15EFF9);
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
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int pos = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if ((uint)span.Length < 2u || !span.StartsWith("-c", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num3 = num;
				if ((uint)span.Length >= 8u && span.Slice(2).StartsWith("odec:v", StringComparison.OrdinalIgnoreCase))
				{
					num += 8;
					span = inputSpan.Slice(num);
					num4 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
						num4++;
						if (!span.StartsWith(":0"))
						{
							break;
						}
						num += 2;
						span = inputSpan.Slice(num);
						if (num4 == 0)
						{
							continue;
						}
						goto IL_00d8;
					}
					goto IL_00b5;
				}
				goto IL_00df;
				IL_019d:
				while (true)
				{
					int i;
					for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
					{
					}
					if (i != 0)
					{
						span = span.Slice(i);
						num += i;
						if ((uint)span.Length >= 4u && span.StartsWith("copy", StringComparison.OrdinalIgnoreCase))
						{
							int num6 = num;
							if ((uint)span.Length >= 5u && char.IsWhiteSpace(span[4]))
							{
								num += 5;
								span = inputSpan.Slice(num);
							}
							else
							{
								num = num6;
								span = inputSpan.Slice(num);
								if (5 < span.Length || (4 < span.Length && span[4] != '\n'))
								{
									goto IL_0186;
								}
								num += 4;
								span = inputSpan.Slice(num);
							}
							runtextpos = num;
							Capture(0, start, num);
							return true;
						}
					}
					goto IL_0186;
					IL_0186:
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num2 == 0)
					{
						break;
					}
					if (num2 != 1)
					{
						continue;
					}
					goto IL_015d;
				}
				goto IL_00b5;
				IL_00b5:
				if (--num4 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_00d8;
				}
				goto IL_00df;
				IL_00d8:
				num2 = 0;
				goto IL_019d;
				IL_00df:
				num = num3;
				span = inputSpan.Slice(num);
				if ((uint)span.Length < 4u || !span.Slice(2).StartsWith(":v", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num += 4;
				span = inputSpan.Slice(num);
				num5 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
					num5++;
					if (!span.StartsWith(":0"))
					{
						break;
					}
					num += 2;
					span = inputSpan.Slice(num);
					if (num5 == 0)
					{
						continue;
					}
					goto IL_0182;
				}
				goto IL_015d;
				IL_015d:
				if (--num5 < 0)
				{
					return false;
				}
				num = runstack[--pos];
				span = inputSpan.Slice(num);
				goto IL_0182;
				IL_0182:
				num2 = 1;
				goto IL_019d;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCopyRegex_0 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCopyRegex_0();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCopyRegex_0()
	{
		pattern = "(?:-codec:v(?::0)?|-c:v(?::0)?)\\s+copy(?:\\s|$)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
