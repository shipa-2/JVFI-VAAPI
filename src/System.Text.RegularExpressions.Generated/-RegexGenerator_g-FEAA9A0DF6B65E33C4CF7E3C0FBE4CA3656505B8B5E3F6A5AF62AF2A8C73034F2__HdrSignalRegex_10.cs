using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HdrSignalRegex_10 : Regex
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
				if (num <= inputSpan.Length - 18)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfAnyStrings_OrdinalIgnoreCase_0645521195AA1C75CAB01A3966931FD3577643CCFDDABD5E77A3BA4CE1090D87);
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
				int pos = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				int num7 = pos;
				if (span.IsEmpty)
				{
					return false;
				}
				switch (span[0])
				{
				case '-':
				{
					if ((uint)span.Length < 9u || !span.Slice(1).StartsWith("color_tr", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					int num8 = pos;
					if ((uint)span.Length < 10u)
					{
						return false;
					}
					switch (span[9])
					{
					case 'C':
					case 'c':
						num += 10;
						span = inputSpan.Slice(num);
						num5 = 0;
						while (true)
						{
							_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
							num5++;
							if (!span.IsEmpty && span[0] == ':')
							{
								num++;
								span = inputSpan.Slice(num);
								int k;
								for (k = 0; (uint)k < (uint)span.Length && _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.IsWordChar(span[k]); k++)
								{
								}
								if (k != 0)
								{
									span = span.Slice(k);
									num += k;
									if (num5 == 0)
									{
										continue;
									}
									goto IL_017b;
								}
							}
							goto IL_0156;
							IL_017b:
							int l;
							for (l = 0; (uint)l < (uint)span.Length && char.IsWhiteSpace(span[l]); l++)
							{
							}
							if (l != 0)
							{
								span = span.Slice(l);
								num += l;
								if (!span.IsEmpty)
								{
									char c2 = span[0];
									if ((uint)c2 <= 83u)
									{
										if (c2 != 'A')
										{
											if (c2 != 'S')
											{
												goto IL_0156;
											}
											goto IL_01f6;
										}
									}
									else if (c2 != 'a')
									{
										if (c2 != 's')
										{
											goto IL_0156;
										}
										goto IL_01f6;
									}
									if ((uint)span.Length >= 12u && span.Slice(1).StartsWith("rib-std-b67", StringComparison.OrdinalIgnoreCase))
									{
										num += 12;
										span = inputSpan.Slice(num);
										break;
									}
								}
							}
							goto IL_0156;
							IL_01f6:
							if ((uint)span.Length >= 5u && span.Slice(1).StartsWith("mpte", StringComparison.OrdinalIgnoreCase) && (uint)span.Length >= 6u)
							{
								char c4 = span[5];
								if (c4 != '-')
								{
									if (c4 == '2' && span.Slice(6).StartsWith("084"))
									{
										num += 9;
										span = inputSpan.Slice(num);
										break;
									}
								}
								else if (span.Slice(6).StartsWith("2084"))
								{
									num += 10;
									span = inputSpan.Slice(num);
									break;
								}
							}
							goto IL_0156;
							IL_0156:
							if (--num5 < 0)
							{
								return false;
							}
							num = runstack[--pos];
							span = inputSpan.Slice(num);
							goto IL_017b;
						}
						break;
					case 'A':
					case 'a':
						if ((uint)span.Length < 15u || !span.Slice(10).StartsWith("nsfer", StringComparison.OrdinalIgnoreCase))
						{
							return false;
						}
						num += 15;
						span = inputSpan.Slice(num);
						num6 = 0;
						while (true)
						{
							_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
							num6++;
							if (!span.IsEmpty && span[0] == ':')
							{
								num++;
								span = inputSpan.Slice(num);
								int i;
								for (i = 0; (uint)i < (uint)span.Length && _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.IsWordChar(span[i]); i++)
								{
								}
								if (i != 0)
								{
									span = span.Slice(i);
									num += i;
									if (num6 == 0)
									{
										continue;
									}
									goto IL_03be;
								}
							}
							goto IL_0399;
							IL_03be:
							int j;
							for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span[j]); j++)
							{
							}
							if (j != 0)
							{
								span = span.Slice(j);
								num += j;
								if (!span.IsEmpty)
								{
									char c2 = span[0];
									if ((uint)c2 <= 83u)
									{
										if (c2 != 'A')
										{
											if (c2 != 'S')
											{
												goto IL_0399;
											}
											goto IL_0439;
										}
									}
									else if (c2 != 'a')
									{
										if (c2 != 's')
										{
											goto IL_0399;
										}
										goto IL_0439;
									}
									if ((uint)span.Length >= 12u && span.Slice(1).StartsWith("rib-std-b67", StringComparison.OrdinalIgnoreCase))
									{
										num += 12;
										span = inputSpan.Slice(num);
										break;
									}
								}
							}
							goto IL_0399;
							IL_0439:
							if ((uint)span.Length >= 5u && span.Slice(1).StartsWith("mpte", StringComparison.OrdinalIgnoreCase) && (uint)span.Length >= 6u)
							{
								char c4 = span[5];
								if (c4 != '-')
								{
									if (c4 == '2' && span.Slice(6).StartsWith("084"))
									{
										num += 9;
										span = inputSpan.Slice(num);
										break;
									}
								}
								else if (span.Slice(6).StartsWith("2084"))
								{
									num += 10;
									span = inputSpan.Slice(num);
									break;
								}
							}
							goto IL_0399;
							IL_0399:
							if (--num6 < 0)
							{
								return false;
							}
							num = runstack[--pos];
							span = inputSpan.Slice(num);
							goto IL_03be;
						}
						break;
					default:
						return false;
					}
					pos = num8;
					break;
				}
				case 'Z':
				case 'z':
					if ((uint)span.Length < 7u || !span.Slice(1).StartsWith("scale", StringComparison.OrdinalIgnoreCase) || span[6] != '=')
					{
						return false;
					}
					num += 7;
					span = inputSpan.Slice(num);
					num4 = num;
					while (true)
					{
						num3 = num;
						if ((uint)span.Length < 8u || !span.StartsWith("transfer", StringComparison.OrdinalIgnoreCase))
						{
							goto IL_0618;
						}
						num2 = 0;
						num += 8;
						span = inputSpan.Slice(num);
						goto IL_066d;
						IL_066d:
						while (true)
						{
							if (!span.IsEmpty && span[0] == '=' && (uint)span.Length >= 2u)
							{
								char c = span[1];
								if ((uint)c <= 83u)
								{
									if (c != 'A')
									{
										if (c != 'S')
										{
											goto IL_0656;
										}
										goto IL_06be;
									}
								}
								else if (c != 'a')
								{
									if (c != 's')
									{
										goto IL_0656;
									}
									goto IL_06be;
								}
								if ((uint)span.Length >= 13u && span.Slice(2).StartsWith("rib-std-b67", StringComparison.OrdinalIgnoreCase))
								{
									goto IL_0791;
								}
							}
							goto IL_0656;
							IL_0656:
							if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
							{
								CheckTimeout();
							}
							if (num2 != 0)
							{
								if (num2 != 1)
								{
									continue;
								}
								goto IL_0571;
							}
							goto IL_0618;
							IL_06be:
							if ((uint)span.Length >= 6u && span.Slice(2).StartsWith("mpte", StringComparison.OrdinalIgnoreCase) && (uint)span.Length >= 7u)
							{
								char c2 = span[6];
								if (c2 != '-')
								{
									if (c2 == '2' && span.Slice(7).StartsWith("084"))
									{
										break;
									}
								}
								else if (span.Slice(7).StartsWith("2084"))
								{
									goto IL_0755;
								}
							}
							goto IL_0656;
						}
						num += 10;
						span = inputSpan.Slice(num);
						break;
						IL_0571:
						if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						num = num4;
						span = inputSpan.Slice(num);
						char c3;
						if (span.IsEmpty || (((c3 = span[0]) < '\u0080') ? (("쇿\uffff\uef7a\uffff\uffff\uffff\uffff\uffff"[(int)c3 >> 4] & (1 << (c3 & 0xF))) == 0) : (!RegexRunner.CharInClass(c3, "\u0001\u0006\u0001\"#'(,-d"))))
						{
							return false;
						}
						num++;
						span = inputSpan.Slice(num);
						num4 = num;
						continue;
						IL_0618:
						num = num3;
						span = inputSpan.Slice(num);
						if (span.IsEmpty || (span[0] | 0x20) != 116)
						{
							goto IL_0571;
						}
						num2 = 1;
						num++;
						span = inputSpan.Slice(num);
						goto IL_066d;
						IL_0755:
						num += 11;
						span = inputSpan.Slice(num);
						break;
						IL_0791:
						num += 13;
						span = inputSpan.Slice(num);
						break;
					}
					break;
				default:
					return false;
				}
				pos = num7;
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HdrSignalRegex_10 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HdrSignalRegex_10();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HdrSignalRegex_10()
	{
		pattern = "(?:-color_trc(?::\\w+)?\\s+(?:smpte2084|smpte-2084|arib-std-b67)|-color_transfer(?::\\w+)?\\s+(?:smpte2084|smpte-2084|arib-std-b67)|zscale=[^,\\s\"']*?(?:transfer|t)=(?:smpte2084|smpte-2084|arib-std-b67))";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
