using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal sealed class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HighBitDepthPixelFormatRegex_11 : Regex
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
				if (num <= inputSpan.Length - 2)
				{
					if (num < inputSpan.Length - 5)
					{
						num = inputSpan.Length - 5;
					}
					ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
					int num2;
					for (num2 = 0; num2 < readOnlySpan.Length - 1; num2++)
					{
						int num3 = readOnlySpan.Slice(num2).IndexOf('1');
						if (num3 < 0)
						{
							break;
						}
						num2 += num3;
						if ((uint)(num2 + 1) >= (uint)readOnlySpan.Length)
						{
							break;
						}
						uint num4;
						if ((int)((uint)(-1442840576 << (int)(short)(num4 = (ushort)(readOnlySpan[num2 + 1] - 48))) & (num4 - 32)) < 0)
						{
							runtextpos = num + num2;
							return true;
						}
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
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				uint num3;
				if ((uint)readOnlySpan.Length < 2u || readOnlySpan[0] != '1' || (int)((uint)(-1442840576 << (int)(short)(num3 = (ushort)(readOnlySpan[1] - 48))) & (num3 - 32)) >= 0)
				{
					return false;
				}
				num += 2;
				readOnlySpan = inputSpan.Slice(num);
				num2 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.StackPush(ref runstack, ref pos, num);
					num2++;
					if (!readOnlySpan.IsEmpty)
					{
						switch (readOnlySpan[0])
						{
						case 'L':
						case 'l':
							if ((uint)readOnlySpan.Length < 2u || (readOnlySpan[1] | 0x20) != 101)
							{
								break;
							}
							num += 2;
							readOnlySpan = inputSpan.Slice(num);
							goto IL_00fe;
						case 'B':
						case 'b':
							if ((uint)readOnlySpan.Length < 2u || (readOnlySpan[1] | 0x20) != 101)
							{
								break;
							}
							num += 2;
							readOnlySpan = inputSpan.Slice(num);
							goto IL_00fe;
						}
					}
					goto IL_0106;
					IL_0127:
					if (num >= inputSpan.Length - 1 && ((uint)num >= (uint)inputSpan.Length || inputSpan[num] == '\n'))
					{
						break;
					}
					goto IL_0106;
					IL_00fe:
					if (num2 == 0)
					{
						continue;
					}
					goto IL_0127;
					IL_0106:
					if (--num2 < 0)
					{
						return false;
					}
					num = runstack[--pos];
					readOnlySpan = inputSpan.Slice(num);
					goto IL_0127;
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

	internal static readonly _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HighBitDepthPixelFormatRegex_11 Instance = new _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HighBitDepthPixelFormatRegex_11();

	private _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HighBitDepthPixelFormatRegex_11()
	{
		pattern = "(?:10|12|14|16)(?:le|be)?$";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
