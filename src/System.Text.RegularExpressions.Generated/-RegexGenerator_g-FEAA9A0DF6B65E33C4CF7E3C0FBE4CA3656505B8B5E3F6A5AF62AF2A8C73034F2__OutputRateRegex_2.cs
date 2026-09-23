using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputRateRegex_2 : Regex
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
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_indexOfString_42D446FFF68032AC1330DB729FF69F5D8CDE58A278A4BFA46AA7079F14EB38BA);
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
				if ((uint)span.Length < 4u || !span.StartsWith("-r:v", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				num += 4;
				span = inputSpan.Slice(num);
				num4 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
					num4++;
					if (!span.StartsWith(":0"))
					{
						goto IL_0089;
					}
					num += 2;
					span = inputSpan.Slice(num);
					if (num4 == 0)
					{
						continue;
					}
					goto IL_00ae;
					IL_00ae:
					num2 = num;
					int i;
					for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
					{
					}
					if (i != 0)
					{
						span = span.Slice(i);
						num += i;
						num3 = num;
						num2++;
						while (true)
						{
							int j;
							for (j = 0; (uint)j < (uint)span.Length && !char.IsWhiteSpace(span[j]); j++)
							{
							}
							if (j == 0)
							{
								if (_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_hasTimeout)
								{
									CheckTimeout();
								}
								if (num2 >= num3)
								{
									break;
								}
								num = --num3;
								span = inputSpan.Slice(num);
								continue;
							}
							span = span.Slice(j);
							Capture(0, start, runtextpos = num + j);
							return true;
						}
					}
					goto IL_0089;
					IL_0089:
					if (--num4 < 0)
					{
						break;
					}
					num = runstack[--pos];
					span = inputSpan.Slice(num);
					goto IL_00ae;
				}
				return false;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputRateRegex_2 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputRateRegex_2();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputRateRegex_2()
	{
		pattern = "-r:v(?::0)?\\s+\\S+";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
