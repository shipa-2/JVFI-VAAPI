using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCodecRegex_1 : Regex
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
				if (num <= inputSpan.Length - 6)
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
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int pos = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if ((uint)span.Length < 2u || !span.StartsWith("-c", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num4 = num;
				num3 = Crawlpos();
				if ((uint)span.Length >= 8u && span.Slice(2).StartsWith("odec:v", StringComparison.OrdinalIgnoreCase))
				{
					num += 8;
					span = inputSpan.Slice(num);
					num9 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, Crawlpos(), num);
						num9++;
						if (!span.StartsWith(":0"))
						{
							break;
						}
						num += 2;
						span = inputSpan.Slice(num);
						if (num9 == 0)
						{
							continue;
						}
						goto IL_0115;
					}
					goto IL_00de;
				}
				goto IL_011c;
				IL_020b:
				while (true)
				{
					num7 = num;
					int i;
					for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
					{
					}
					if (i != 0)
					{
						span = span.Slice(i);
						num += i;
						num8 = num;
						num7++;
						while (true)
						{
							num6 = Crawlpos();
							num5 = num;
							int j;
							for (j = 0; (uint)j < (uint)span.Length; j++)
							{
								char c;
								if (!(((c = span[j]) < '\u0080') ? (("쇿\uffff\ufffa\uffff\uffff\uffff\uffff\uffff"[(int)c >> 4] & (1 << (c & 0xF))) != 0) : RegexRunner.CharInClass(c, "\u0001\u0002\u0001\"#d")))
								{
									break;
								}
							}
							if (j == 0)
							{
								UncaptureUntil(num6);
								if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
								{
									CheckTimeout();
								}
								if (num7 >= num8)
								{
									break;
								}
								num = --num8;
								span = inputSpan.Slice(num);
								continue;
							}
							span = span.Slice(j);
							num += j;
							Capture(1, num5, num);
							runtextpos = num;
							Capture(0, start, num);
							return true;
						}
					}
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
					goto IL_01b0;
				}
				goto IL_00de;
				IL_011c:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 4u || !span.Slice(2).StartsWith(":v", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 4;
				span = inputSpan.Slice(num);
				num10 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, Crawlpos(), num);
					num10++;
					if (!span.StartsWith(":0"))
					{
						break;
					}
					num += 2;
					span = inputSpan.Slice(num);
					if (num10 == 0)
					{
						continue;
					}
					goto IL_01f0;
				}
				goto IL_01b0;
				IL_0115:
				num2 = 0;
				goto IL_020b;
				IL_01b0:
				if (--num10 < 0)
				{
					UncaptureUntil(0);
					return false;
				}
				num = runstack[--pos];
				UncaptureUntil(runstack[--pos]);
				span = inputSpan.Slice(num);
				goto IL_01f0;
				IL_00de:
				if (--num9 >= 0)
				{
					num = runstack[--pos];
					UncaptureUntil(runstack[--pos]);
					span = inputSpan.Slice(num);
					goto IL_0115;
				}
				goto IL_011c;
				IL_01f0:
				num2 = 1;
				goto IL_020b;
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCodecRegex_1 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCodecRegex_1();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCodecRegex_1()
	{
		pattern = "(?:-codec:v(?::0)?|-c:v(?::0)?)\\s+([^\\s\\\"]+)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
