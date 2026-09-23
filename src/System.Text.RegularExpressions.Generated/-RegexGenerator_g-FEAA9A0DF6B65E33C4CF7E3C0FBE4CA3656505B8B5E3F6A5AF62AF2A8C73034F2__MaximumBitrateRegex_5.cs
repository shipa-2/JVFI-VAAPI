using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__MaximumBitrateRegex_5 : Regex
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
				if (num <= inputSpan.Length - 10)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfString_2B0262D9455A307F5D87FA4A74F9C2BF0EA081A59FF53977B67C91D8BF579F12);
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
				int pos = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				span = inputSpan.Slice(num);
				int num5 = num;
				if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
				{
					CheckTimeout();
				}
				if ((uint)(num - 1) < inputSpan.Length && !char.IsWhiteSpace(inputSpan[num - 1]))
				{
					num--;
					UncaptureUntil(0);
					return false;
				}
				num = num5;
				span = inputSpan.Slice(num);
				if ((uint)span.Length < 8u || !span.StartsWith("-maxrate", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 8;
				span = inputSpan.Slice(num);
				num3 = 0;
				int j;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, Crawlpos(), num);
					num3++;
					if ((uint)span.Length >= 2u && span.StartsWith(":v", StringComparison.OrdinalIgnoreCase))
					{
						num += 2;
						span = inputSpan.Slice(num);
						num4 = 0;
						while (true)
						{
							_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, Crawlpos(), num);
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
							goto IL_016d;
						}
						goto IL_0136;
					}
					goto IL_01a5;
					IL_016d:
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num4);
					if (num3 == 0)
					{
						continue;
					}
					goto IL_01fe;
					IL_0136:
					if (--num4 >= 0)
					{
						num = runstack[--pos];
						UncaptureUntil(runstack[--pos]);
						span = inputSpan.Slice(num);
						goto IL_016d;
					}
					goto IL_01a5;
					IL_01fe:
					int i;
					for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
					{
					}
					if (i != 0)
					{
						span = span.Slice(i);
						num += i;
						num2 = num;
						for (j = 0; (uint)j < (uint)span.Length && char.IsDigit(span[j]); j++)
						{
						}
						if (j != 0)
						{
							break;
						}
					}
					if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num3 != 0)
					{
						num4 = runstack[--pos];
						if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						goto IL_0136;
					}
					UncaptureUntil(0);
					return false;
					IL_01a5:
					if (--num3 < 0)
					{
						UncaptureUntil(0);
						return false;
					}
					num = runstack[--pos];
					UncaptureUntil(runstack[--pos]);
					span = inputSpan.Slice(num);
					goto IL_01fe;
				}
				span = span.Slice(j);
				num += j;
				Capture(1, num2, num);
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__MaximumBitrateRegex_5 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__MaximumBitrateRegex_5();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__MaximumBitrateRegex_5()
	{
		pattern = "(?<!\\S)-maxrate(?::v(?::0)?)?\\s+(\\d+)";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
