using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaEightBitFormatRegex_13 : Regex
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
				if (num <= inputSpan.Length - 21)
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
				char c;
				if ((uint)readOnlySpan.Length < 7u || !readOnlySpan.StartsWith("_r", StringComparison.OrdinalIgnoreCase) || ((((c = readOnlySpan[2]) | 0x20) != 107) & (c != 'K')) || !readOnlySpan.Slice(3).StartsWith("rga", StringComparison.OrdinalIgnoreCase) || readOnlySpan[6] != '=')
				{
					UncaptureUntil(0);
					return false;
				}
				num += 7;
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
							char c2 = readOnlySpan[0];
							if ((uint)c2 <= 89u)
							{
								if (c2 == 'N')
								{
									goto IL_0285;
								}
								if (c2 != 'Y')
								{
									goto IL_0170;
								}
							}
							else
							{
								if (c2 == 'n')
								{
									goto IL_0285;
								}
								if (c2 != 'y')
								{
									goto IL_0170;
								}
							}
							if ((uint)readOnlySpan.Length >= 7u && readOnlySpan.Slice(1).StartsWith("uv420p", StringComparison.OrdinalIgnoreCase))
							{
								num += 7;
								readOnlySpan = inputSpan.Slice(num);
								goto IL_02f7;
							}
						}
					}
					goto IL_0170;
					IL_02f7:
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
					goto IL_0170;
					IL_0285:
					if ((uint)readOnlySpan.Length < 4u || !readOnlySpan.Slice(1).StartsWith("v12", StringComparison.OrdinalIgnoreCase))
					{
						goto IL_0170;
					}
					num += 4;
					readOnlySpan = inputSpan.Slice(num);
					goto IL_02f7;
					IL_0170:
					UncaptureUntil(num3);
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					num = num4;
					readOnlySpan = inputSpan.Slice(num);
					if (readOnlySpan.IsEmpty || (((c = readOnlySpan[0]) < '\u0080') ? (("쇿\uffff\uef7a\uffff\uffff\uffff\uffff\uffff"[(int)c >> 4] & (1 << (c & 0xF))) == 0) : (!RegexRunner.CharInClass(c, "\u0001\u0006\u0001\"#'(,-d"))))
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaEightBitFormatRegex_13 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaEightBitFormatRegex_13();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaEightBitFormatRegex_13()
	{
		pattern = "((?:vpp|scale)_rkrga=[^,\\s\"']*?format=)(?:nv12|yuv420p)(?=[: ,\"']|$)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
