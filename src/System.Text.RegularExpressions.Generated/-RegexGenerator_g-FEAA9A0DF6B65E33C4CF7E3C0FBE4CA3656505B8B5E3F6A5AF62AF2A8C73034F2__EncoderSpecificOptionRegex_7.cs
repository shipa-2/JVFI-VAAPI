using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__EncoderSpecificOptionRegex_7 : Regex
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
					while (true)
					{
						ReadOnlySpan<char> span = inputSpan.Slice(num);
						int num2 = span.IndexOf('-');
						if (num2 < 0)
						{
							break;
						}
						int num3 = num2 - 1;
						while ((uint)num3 < (uint)span.Length && char.IsWhiteSpace(span[num3]))
						{
							num3--;
						}
						if (num2 - num3 - 1 < 1)
						{
							num += num2 + 1;
							continue;
						}
						runtextpos = num + num3 + 1;
						runtrackpos = num + num2;
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
				int arg = 0;
				int arg2 = 0;
				int arg3 = 0;
				int arg4 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int pos = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				num = runtrackpos;
				span = inputSpan.Slice(num);
				if (runtextpos < num)
				{
					runtextpos = num;
				}
				if (span.IsEmpty || span[0] != '-')
				{
					return false;
				}
				num4 = num;
				if ((uint)span.Length >= 14u && span.Slice(1).StartsWith("svtav1-params", StringComparison.OrdinalIgnoreCase))
				{
					num += 14;
					span = inputSpan.Slice(num);
					num8 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
						num8++;
						if (span.IsEmpty || span[0] != ':')
						{
							break;
						}
						num++;
						span = inputSpan.Slice(num);
						int i;
						for (i = 0; (uint)i < (uint)span.Length && char.IsDigit(span[i]); i++)
						{
						}
						if (i == 0)
						{
							break;
						}
						span = span.Slice(i);
						num += i;
						if (num8 == 0)
						{
							continue;
						}
						goto IL_015a;
					}
					goto IL_0137;
				}
				goto IL_0161;
				IL_0619:
				if (--num13 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_0654;
				}
				goto IL_068d;
				IL_0654:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num13);
				if (num12 == 0)
				{
					goto IL_04f9;
				}
				goto IL_06c7;
				IL_02dc:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, arg, arg2);
				if (num10 == 0)
				{
					goto IL_021f;
				}
				goto IL_0331;
				IL_021f:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
				num10++;
				if (!span.IsEmpty && span[0] == ':')
				{
					num++;
					span = inputSpan.Slice(num);
					arg = num;
					int j;
					for (j = 0; (uint)j < (uint)span.Length && char.IsDigit(span[j]); j++)
					{
					}
					if (j != 0)
					{
						span = span.Slice(j);
						num += j;
						arg2 = num;
						arg++;
						goto IL_02dc;
					}
				}
				goto IL_02f6;
				IL_036a:
				if (--num9 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_03a2;
				}
				goto IL_03aa;
				IL_0542:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
				num13++;
				if (!span.IsEmpty && span[0] == ':')
				{
					num++;
					span = inputSpan.Slice(num);
					arg3 = num;
					int k;
					for (k = 0; (uint)k < (uint)span.Length && char.IsDigit(span[k]); k++)
					{
					}
					if (k != 0)
					{
						span = span.Slice(k);
						num += k;
						arg4 = num;
						arg3++;
						goto IL_05ff;
					}
				}
				goto IL_0619;
				IL_068d:
				if (--num12 < 0)
				{
					return false;
				}
				num = runstack[--pos];
				span = inputSpan.Slice(num);
				goto IL_06c7;
				IL_06b4:
				if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
				{
					CheckTimeout();
				}
				if (num12 != 0)
				{
					num13 = runstack[--pos];
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num13 != 0)
					{
						_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPop(runstack, ref pos, out arg4, out arg3);
						if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						if (arg3 < arg4)
						{
							num = --arg4;
							span = inputSpan.Slice(num);
							goto IL_05ff;
						}
						goto IL_0619;
					}
					goto IL_068d;
				}
				return false;
				IL_04ae:
				num2 = 1;
				goto IL_06ea;
				IL_0490:
				num3 = 1;
				goto IL_04ae;
				IL_03a2:
				num3 = 0;
				goto IL_04ae;
				IL_0495:
				if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
				{
					CheckTimeout();
				}
				if (num3 == 0)
				{
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num9 != 0)
					{
						num10 = runstack[--pos];
						if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						if (num10 != 0)
						{
							_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPop(runstack, ref pos, out arg2, out arg);
							if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
							{
								CheckTimeout();
							}
							if (arg < arg2)
							{
								num = --arg2;
								span = inputSpan.Slice(num);
								goto IL_02dc;
							}
							goto IL_02f6;
						}
						goto IL_036a;
					}
					goto IL_03aa;
				}
				if (num3 == 1)
				{
					goto IL_046d;
				}
				goto IL_04ae;
				IL_0137:
				if (--num8 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_015a;
				}
				goto IL_0161;
				IL_06c7:
				num2 = 2;
				goto IL_06ea;
				IL_015a:
				num2 = 0;
				goto IL_06ea;
				IL_06ea:
				while (true)
				{
					num6 = num;
					int l;
					for (l = 0; (uint)l < (uint)span.Length && char.IsWhiteSpace(span[l]); l++)
					{
					}
					if (l != 0)
					{
						span = span.Slice(l);
						num += l;
						num7 = num;
						num6++;
						while (true)
						{
							int m;
							for (m = 0; (uint)m < (uint)span.Length; m++)
							{
								char c;
								if (!(((c = span[m]) < '\u0080') ? (("쇿\uffff\ufffa\uffff\uffff\uffff\uffff\uffff"[(int)c >> 4] & (1 << (c & 0xF))) != 0) : RegexRunner.CharInClass(c, "\u0001\u0002\u0001\"#d")))
								{
									break;
								}
							}
							if (m == 0)
							{
								if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
								{
									CheckTimeout();
								}
								if (num6 >= num7)
								{
									break;
								}
								num = --num7;
								span = inputSpan.Slice(num);
								continue;
							}
							span = span.Slice(m);
							Capture(0, start, runtextpos = num + m);
							return true;
						}
					}
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					switch (num2)
					{
					case 0:
						break;
					case 1:
						goto IL_0495;
					case 2:
						goto IL_06b4;
					default:
						continue;
					}
					break;
				}
				goto IL_0137;
				IL_05ff:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, arg3, arg4);
				if (num13 == 0)
				{
					goto IL_0542;
				}
				goto IL_0654;
				IL_01d6:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
				num9++;
				if ((uint)span.Length >= 2u && span.StartsWith(":v", StringComparison.OrdinalIgnoreCase))
				{
					num += 2;
					span = inputSpan.Slice(num);
					num10 = 0;
					goto IL_021f;
				}
				goto IL_036a;
				IL_0331:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num10);
				if (num9 == 0)
				{
					goto IL_01d6;
				}
				goto IL_03a2;
				IL_02f6:
				if (--num10 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_0331;
				}
				goto IL_036a;
				IL_046d:
				if (--num11 >= 0)
				{
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_0490;
				}
				goto IL_04b5;
				IL_03aa:
				num = num5;
				span = inputSpan.Slice(num);
				if ((uint)span.Length >= 10u && span.Slice(3).StartsWith("ofile:v", StringComparison.OrdinalIgnoreCase))
				{
					num += 10;
					span = inputSpan.Slice(num);
					num11 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
						num11++;
						if (span.IsEmpty || span[0] != ':')
						{
							break;
						}
						num++;
						span = inputSpan.Slice(num);
						int n;
						for (n = 0; (uint)n < (uint)span.Length && char.IsDigit(span[n]); n++)
						{
						}
						if (n == 0)
						{
							break;
						}
						span = span.Slice(n);
						num += n;
						if (num11 == 0)
						{
							continue;
						}
						goto IL_0490;
					}
					goto IL_046d;
				}
				goto IL_04b5;
				IL_0161:
				num = num4;
				span = inputSpan.Slice(num);
				if ((uint)span.Length >= 3u && span.Slice(1).StartsWith("pr", StringComparison.OrdinalIgnoreCase))
				{
					num5 = num;
					if ((uint)span.Length >= 7u && span.Slice(3).StartsWith("eset", StringComparison.OrdinalIgnoreCase))
					{
						num += 7;
						span = inputSpan.Slice(num);
						num9 = 0;
						goto IL_01d6;
					}
					goto IL_03aa;
				}
				goto IL_04b5;
				IL_04f9:
				_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
				num12++;
				if ((uint)span.Length >= 2u && span.StartsWith(":v", StringComparison.OrdinalIgnoreCase))
				{
					num += 2;
					span = inputSpan.Slice(num);
					num13 = 0;
					goto IL_0542;
				}
				goto IL_068d;
				IL_04b5:
				num = num4;
				span = inputSpan.Slice(num);
				if ((uint)span.Length < 6u || !span.Slice(1).StartsWith("level", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num += 6;
				span = inputSpan.Slice(num);
				num12 = 0;
				goto IL_04f9;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__EncoderSpecificOptionRegex_7 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__EncoderSpecificOptionRegex_7();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__EncoderSpecificOptionRegex_7()
	{
		pattern = "\\s+-(?:svtav1-params(?::\\d+)?|preset(?::v(?::\\d+)?)?|profile:v(?::\\d+)?|level(?::v(?::\\d+)?)?)\\s+[^\\s\"]+";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
