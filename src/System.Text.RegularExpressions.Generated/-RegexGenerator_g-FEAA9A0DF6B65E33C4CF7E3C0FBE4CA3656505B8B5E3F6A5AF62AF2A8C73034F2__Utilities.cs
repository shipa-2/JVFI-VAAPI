using System.Buffers;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.14.36724")]
internal static class _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_0645521195AA1C75CAB01A3966931FD3577643CCFDDABD5E77A3BA4CE1090D87;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_1FB3A96AB46DC539F5A40167F476758299B0E25C98263B03FE8E04D9B0FBEEB7;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_2788718B86A1104F4E4688CF1D292CB912DBFE7EF3197C4AEC3E5D375E82E3BD;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_903F20CB91CC8C379144B939FA94B2F2EB20C03ED7B2578837BEFD9A7EF535F2;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_B5A80BD0D90005DE13CE507EB755A1CE7F26115C05CBCCFDAE14B70978DC262B;

	internal static readonly SearchValues<string> s_indexOfAnyStrings_OrdinalIgnoreCase_C54B5661665FE8B6C590E24093B38E4356A59458FE50240D8300FF3B140A2F6D;

	internal static readonly SearchValues<string> s_indexOfString_25D6F082883373102C2490B138C56E587A018D3FFCEBD1631DCB3FB5A97436CF;

	internal static readonly SearchValues<string> s_indexOfString_2B0262D9455A307F5D87FA4A74F9C2BF0EA081A59FF53977B67C91D8BF579F12;

	internal static readonly SearchValues<string> s_indexOfString_2DDC8227357669D296F342A57C08560F8489D0E671EB728C17B081DCA84DD2AC;

	internal static readonly SearchValues<string> s_indexOfString_42D446FFF68032AC1330DB729FF69F5D8CDE58A278A4BFA46AA7079F14EB38BA;

	internal static readonly SearchValues<string> s_indexOfString_6B04DE053411FD9D821E69CE12885816083B97A0E9939BF44E4B3A18BC25B290;

	internal static readonly SearchValues<string> s_indexOfString_A1FBC50E3481D6B349A26CEC44C0B75C53A3FB219B182FFD57F7F59D1F6618EC;

	internal static readonly SearchValues<string> s_indexOfString_FE9CC628257F93F75E4DF1EEB6C03E59185FD8BF19304994FD3E7E4D5C15EFF9;

	internal static readonly SearchValues<char> s_nonAscii_9FA52D3BAECB644578472387D5284CC6F36F408FEB88A04BA674CE14F24D2386;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsWordChar(char ch)
	{
		ReadOnlySpan<byte> readOnlySpan = new byte[16]
		{
			0, 0, 0, 0, 0, 0, 255, 3, 254, 255,
			255, 135, 254, 255, 255, 7
		};
		int num = (int)ch >> 3;
		if ((uint)num >= (uint)readOnlySpan.Length)
		{
			return (0x4013F & (1 << (int)CharUnicodeInfo.GetUnicodeCategory(ch))) != 0;
		}
		return (readOnlySpan[num] & (1 << (ch & 7))) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPop(int[] stack, ref int pos, out int arg0, out int arg1)
	{
		arg0 = stack[--pos];
		arg1 = stack[--pos];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPush(ref int[] stack, ref int pos, int arg0)
	{
		int[] array = stack;
		int num = pos;
		if ((uint)num < (uint)array.Length)
		{
			array[num] = arg0;
			pos++;
		}
		else
		{
			WithResize(ref stack, ref pos, arg0);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void WithResize(ref int[] reference, ref int reference2, int arg1)
		{
			Array.Resize(ref reference, reference2 * 2);
			StackPush(ref reference, ref reference2, arg1);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPush(ref int[] stack, ref int pos, int arg0, int arg1)
	{
		int[] array = stack;
		int num = pos;
		if ((uint)(num + 1) < (uint)array.Length)
		{
			array[num] = arg0;
			array[num + 1] = arg1;
			pos += 2;
		}
		else
		{
			WithResize(ref stack, ref pos, arg0, arg1);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void WithResize(ref int[] reference, ref int reference2, int arg2, int arg3)
		{
			Array.Resize(ref reference, (reference2 + 1) * 2);
			StackPush(ref reference, ref reference2, arg2, arg3);
		}
	}

	static _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Utilities()
	{
		_003C_003Ey__InlineArray2<string> buffer = default;
		buffer[0] = "-color_t";
		buffer[1] = "zscale";
		s_indexOfAnyStrings_OrdinalIgnoreCase_0645521195AA1C75CAB01A3966931FD3577643CCFDDABD5E77A3BA4CE1090D87 = SearchValues.Create(buffer, StringComparison.OrdinalIgnoreCase);
		buffer = default;
		buffer[0] = "vpp";
		buffer[1] = "scale";
		s_indexOfAnyStrings_OrdinalIgnoreCase_1FB3A96AB46DC539F5A40167F476758299B0E25C98263B03FE8E04D9B0FBEEB7 = SearchValues.Create(buffer, StringComparison.OrdinalIgnoreCase);
		_003C_003Ey__InlineArray8<string> buffer2 = default;
		buffer2[0] = "vpp_r";
		buffer2[1] = "scale_r";
		buffer2[2] = "scale_vaapi";
		buffer2[3] = "tonemap_";
		buffer2[4] = "scale_qs";
		buffer2[5] = "vpp_qsv";
		buffer2[6] = "scale_cu";
		buffer2[7] = "overlay_";
		s_indexOfAnyStrings_OrdinalIgnoreCase_2788718B86A1104F4E4688CF1D292CB912DBFE7EF3197C4AEC3E5D375E82E3BD = SearchValues.Create(buffer2, StringComparison.OrdinalIgnoreCase);
		buffer = default;
		buffer[0] = "p010";
		buffer[1] = "yuv420p10";
		s_indexOfAnyStrings_OrdinalIgnoreCase_903F20CB91CC8C379144B939FA94B2F2EB20C03ED7B2578837BEFD9A7EF535F2 = SearchValues.Create(buffer, StringComparison.OrdinalIgnoreCase);
		buffer = default;
		buffer[0] = "scale";
		buffer[1] = "tonemap";
		s_indexOfAnyStrings_OrdinalIgnoreCase_B5A80BD0D90005DE13CE507EB755A1CE7F26115C05CBCCFDAE14B70978DC262B = SearchValues.Create(buffer, StringComparison.OrdinalIgnoreCase);
		buffer = default;
		buffer[0] = "scale";
		buffer[1] = "overlay";
		s_indexOfAnyStrings_OrdinalIgnoreCase_C54B5661665FE8B6C590E24093B38E4356A59458FE50240D8300FF3B140A2F6D = SearchValues.Create(buffer, StringComparison.OrdinalIgnoreCase);
		s_indexOfString_25D6F082883373102C2490B138C56E587A018D3FFCEBD1631DCB3FB5A97436CF = SearchValues.Create(new ReadOnlySpan<string>("-pix_fmt"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_2B0262D9455A307F5D87FA4A74F9C2BF0EA081A59FF53977B67C91D8BF579F12 = SearchValues.Create(new ReadOnlySpan<string>("-maxrate"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_2DDC8227357669D296F342A57C08560F8489D0E671EB728C17B081DCA84DD2AC = SearchValues.Create(new ReadOnlySpan<string>("-profile:v"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_42D446FFF68032AC1330DB729FF69F5D8CDE58A278A4BFA46AA7079F14EB38BA = SearchValues.Create(new ReadOnlySpan<string>("-r:v"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_6B04DE053411FD9D821E69CE12885816083B97A0E9939BF44E4B3A18BC25B290 = SearchValues.Create(new ReadOnlySpan<string>("-b:v"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_A1FBC50E3481D6B349A26CEC44C0B75C53A3FB219B182FFD57F7F59D1F6618EC = SearchValues.Create(new ReadOnlySpan<string>("-bufsize"), StringComparison.OrdinalIgnoreCase);
		s_indexOfString_FE9CC628257F93F75E4DF1EEB6C03E59185FD8BF19304994FD3E7E4D5C15EFF9 = SearchValues.Create(new ReadOnlySpan<string>("-c"), StringComparison.OrdinalIgnoreCase);
		s_nonAscii_9FA52D3BAECB644578472387D5284CC6F36F408FEB88A04BA674CE14F24D2386 = SearchValues.Create("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyzK");
	}
}
