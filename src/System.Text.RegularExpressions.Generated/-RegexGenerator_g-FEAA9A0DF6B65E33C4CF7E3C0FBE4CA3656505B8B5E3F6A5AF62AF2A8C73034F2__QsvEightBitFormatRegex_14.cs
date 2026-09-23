using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__QsvEightBitFormatRegex_14 : Regex
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
				if (num <= inputSpan.Length - 19)
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
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				num2 = num;
				if (readOnlySpan.IsEmpty)
				{
					UncaptureUntil(0);
					return false;
				}
				switch (readOnlySpan[0])
				{
				case 'V':
				case 'v':
					if ((uint)readOnlySpan.Length < 3u || (readOnlySpan[1] | 0x20) != 112 || (readOnlySpan[2] | 0x20) != 112)
					{
						UncaptureUntil(0);
						return false;
					}
					num += 3;
					readOnlySpan = inputSpan.Slice(num);
					break;
				case 'S':
				case 's':
					if ((uint)readOnlySpan.Length < 5u || !readOnlySpan.Slice(1).StartsWith("cale", StringComparison.OrdinalIgnoreCase))
					{
						UncaptureUntil(0);
						return false;
					}
					num += 5;
					readOnlySpan = inputSpan.Slice(num);
					break;
				default:
					UncaptureUntil(0);
					return false;
				}
				if ((uint)readOnlySpan.Length < 5u || !readOnlySpan.StartsWith("_qsv", StringComparison.OrdinalIgnoreCase) || readOnlySpan[4] != '=')
				{
					UncaptureUntil(0);
					return false;
				}
				num += 5;
				readOnlySpan = inputSpan.Slice(num);
				num4 = num;
				int num5;
				while (true)
				{
					num3 = Crawlpos();
					if ((uint)readOnlySpan.Length >= 7u && readOnlySpan.StartsWith("format", StringComparison.OrdinalIgnoreCase) && readOnlySpan[6] == '=')
					{
						num += 7;
						readOnlySpan = inputSpan.Slice(num);
						Capture(1, num2, num);
						if (!readOnlySpan.IsEmpty)
						{
							char c = readOnlySpan[0];
							if ((uint)c <= 89u)
							{
								if (c == 'N')
								{
									goto IL_0248;
								}
								if (c != 'Y')
								{
									goto IL_0133;
								}
							}
							else
							{
								if (c == 'n')
								{
									goto IL_0248;
								}
								if (c != 'y')
								{
									goto IL_0133;
								}
							}
							if ((uint)readOnlySpan.Length >= 7u && readOnlySpan.Slice(1).StartsWith("uv420p", StringComparison.OrdinalIgnoreCase))
							{
								num += 7;
								readOnlySpan = inputSpan.Slice(num);
								goto IL_02ba;
							}
						}
					}
					goto IL_0133;
					IL_02ba:
					num5 = num;
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					int num6 = num;
					uint num7;
					if (!readOnlySpan.IsEmpty && (int)((uint)(-1593311200 << (int)(short)(num7 = (ushort)(readOnlySpan[0] - 32))) & (num7 - 32)) < 0)
					{
						num++;
						readOnlySpan = inputSpan.Slice(num);
						break;
					}
					num = num6;
					readOnlySpan = inputSpan.Slice(num);
					if (num >= inputSpan.Length - 1 && ((uint)num >= (uint)inputSpan.Length || inputSpan[num] == '\n'))
					{
						break;
					}
					goto IL_0133;
					IL_0248:
					if ((uint)readOnlySpan.Length < 4u || !readOnlySpan.Slice(1).StartsWith("v12", StringComparison.OrdinalIgnoreCase))
					{
						goto IL_0133;
					}
					num += 4;
					readOnlySpan = inputSpan.Slice(num);
					goto IL_02ba;
					IL_0133:
					UncaptureUntil(num3);
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					num = num4;
					readOnlySpan = inputSpan.Slice(num);
					char c2;
					if (readOnlySpan.IsEmpty || (((c2 = readOnlySpan[0]) < '\u0080') ? (("쇿\uffff\uef7a\uffff\uffff\uffff\uffff\uffff"[(int)c2 >> 4] & (1 << (c2 & 0xF))) == 0) : (!RegexRunner.CharInClass(c2, "\u0001\u0006\u0001\"#'(,-d"))))
					{
						UncaptureUntil(0);
						return false;
					}
					num++;
					readOnlySpan = inputSpan.Slice(num);
					num4 = num;
				}
				num = num5;
				readOnlySpan = inputSpan.Slice(num);
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__QsvEightBitFormatRegex_14 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__QsvEightBitFormatRegex_14();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__QsvEightBitFormatRegex_14()
	{
		pattern = "((?:vpp|scale)_qsv=[^,\\s\"']*?format=)(?:nv12|yuv420p)(?=[: ,\"']|$)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
