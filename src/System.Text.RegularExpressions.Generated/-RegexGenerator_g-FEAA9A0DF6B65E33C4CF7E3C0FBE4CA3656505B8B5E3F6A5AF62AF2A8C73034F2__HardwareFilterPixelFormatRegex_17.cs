using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HardwareFilterPixelFormatRegex_17 : Regex
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
				if (num <= inputSpan.Length - 16)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfAnyStrings_OrdinalIgnoreCase_2788718B86A1104F4E4688CF1D292CB912DBFE7EF3197C4AEC3E5D375E82E3BD);
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
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				num4 = num;
				num3 = Crawlpos();
				char c;
				if ((uint)span.Length < 9u || !span.StartsWith("vpp_r", StringComparison.OrdinalIgnoreCase) || ((((c = span[5]) | 0x20) != 107) & (c != 'K')) || !span.Slice(6).StartsWith("rga", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_00a1;
				}
				num2 = 0;
				num += 9;
				span = inputSpan.Slice(num);
				goto IL_0361;
				IL_0245:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 7u || !span.StartsWith("vpp_qsv", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_028d;
				}
				num2 = 4;
				num += 7;
				span = inputSpan.Slice(num);
				goto IL_0361;
				IL_01aa:
				num2 = 1;
				goto IL_0361;
				IL_0176:
				if ((uint)span.Length >= 11u && span.Slice(7).StartsWith("aapi", StringComparison.OrdinalIgnoreCase))
				{
					num += 11;
					span = inputSpan.Slice(num);
					goto IL_01aa;
				}
				goto IL_01b1;
				IL_028d:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 10u || !span.StartsWith("scale_cuda", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_02d7;
				}
				num2 = 5;
				num += 10;
				span = inputSpan.Slice(num);
				goto IL_0361;
				IL_0361:
				while (true)
				{
					if (!span.IsEmpty && span[0] == '=')
					{
						num++;
						span = inputSpan.Slice(num);
						num7 = num;
						while (true)
						{
							num6 = Crawlpos();
							if ((uint)span.Length >= 7u && span.StartsWith("format", StringComparison.OrdinalIgnoreCase) && span[6] == '=')
							{
								num += 7;
								span = inputSpan.Slice(num);
								num5 = num;
								int num8 = span.IndexOfAnyExcept(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_nonAscii_9FA52D3BAECB644578472387D5284CC6F36F408FEB88A04BA674CE14F24D2386);
								if (num8 < 0)
								{
									num8 = span.Length;
								}
								if (num8 != 0)
								{
									span = span.Slice(num8);
									num += num8;
									Capture(1, num5, num);
									runtextpos = num;
									Capture(0, start, num);
									return true;
								}
							}
							UncaptureUntil(num6);
							if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
							{
								CheckTimeout();
							}
							num = num7;
							span = inputSpan.Slice(num);
							if (span.IsEmpty || (((c = span[0]) < '\u0080') ? (("쇿\uffff\uef7a\uffff\uffff\uffff\uffff\uffff"[(int)c >> 4] & (1 << (c & 0xF))) == 0) : (!RegexRunner.CharInClass(c, "\u0001\u0006\u0001\"#'(,-d"))))
							{
								break;
							}
							num++;
							span = inputSpan.Slice(num);
							num7 = num;
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
						goto IL_01b1;
					case 2:
						goto IL_01fb;
					case 3:
						goto IL_0245;
					case 4:
						goto IL_028d;
					case 5:
						goto IL_02d7;
					case 6:
						UncaptureUntil(0);
						return false;
					default:
						continue;
					}
					break;
				}
				goto IL_00a1;
				IL_01fb:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 9u || !span.StartsWith("scale_qsv", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_0245;
				}
				num2 = 3;
				num += 9;
				span = inputSpan.Slice(num);
				goto IL_0361;
				IL_011a:
				if ((uint)span.Length >= 11u && !((((c = span[7]) | 0x20) != 107) & (c != 'K')) && span.Slice(8).StartsWith("rga", StringComparison.OrdinalIgnoreCase))
				{
					num += 11;
					span = inputSpan.Slice(num);
					goto IL_01aa;
				}
				goto IL_01b1;
				IL_00a1:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length >= 6u && span.StartsWith("scale_", StringComparison.OrdinalIgnoreCase) && (uint)span.Length >= 7u)
				{
					char c2 = span[6];
					if ((uint)c2 <= 86u)
					{
						if (c2 == 'R')
						{
							goto IL_011a;
						}
						if (c2 == 'V')
						{
							goto IL_0176;
						}
					}
					else
					{
						if (c2 == 'r')
						{
							goto IL_011a;
						}
						if (c2 == 'v')
						{
							goto IL_0176;
						}
					}
				}
				goto IL_01b1;
				IL_01b1:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 13u || !span.StartsWith("tonemap_vaapi", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_01fb;
				}
				num2 = 2;
				num += 13;
				span = inputSpan.Slice(num);
				goto IL_0361;
				IL_02d7:
				num = num4;
				span = inputSpan.Slice(num);
				UncaptureUntil(num3);
				if ((uint)span.Length < 12u || !span.StartsWith("overlay_cuda", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num2 = 6;
				num += 12;
				span = inputSpan.Slice(num);
				goto IL_0361;
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HardwareFilterPixelFormatRegex_17 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HardwareFilterPixelFormatRegex_17();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HardwareFilterPixelFormatRegex_17()
	{
		pattern = "(?:vpp_rkrga|scale_rkrga|scale_vaapi|tonemap_vaapi|scale_qsv|vpp_qsv|scale_cuda|overlay_cuda)=[^,\\s\"']*?format=([a-z0-9_]+)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
