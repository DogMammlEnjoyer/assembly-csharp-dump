"""
float_splitter.py — Convert an 8-byte target value into two Lua doubles
that, when cast to (float), reproduce the exact 4-byte bit patterns.

Usage:
    python float_splitter.py [hex_value]

The C code does: *(float*)addr = (float)luaL_checknumber(L, 3)
luaL_checknumber returns a double (Lua number). The (float) cast converts
double -> float using IEEE 754 round-to-nearest-even.

Key fact: every float value is exactly representable as a double.
So if we reinterpret our 4 target bytes as a float, then store that
float's value as a double, the (float)double cast is lossless.

Exception: signaling NaNs (sNaN). On ARM64, float<->double conversion
of sNaN may quiet it (set bit 22). We detect and warn about these.
"""

import struct
import sys


def is_snan_float(pattern: int) -> bool:
    """Check if a 32-bit pattern is a signaling NaN (float).
    Float NaN: exponent = 0xFF (bits 30-23 all set), mantissa != 0.
    Signaling NaN: bit 22 = 0 (quiet bit not set), remaining mantissa != 0.
    """
    exponent = (pattern >> 23) & 0xFF
    mantissa = pattern & 0x7FFFFF
    if exponent != 0xFF or mantissa == 0:
        return False
    # Bit 22 = 0 means signaling NaN
    quiet_bit = (pattern >> 22) & 1
    return quiet_bit == 0


def pattern_to_double(pattern: int) -> float:
    """Convert a 32-bit pattern to the double value that, when cast to
    (float), reproduces that exact bit pattern.

    Returns the double value. Raises ValueError for sNaN patterns.
    """
    # Reinterpret 4 bytes as IEEE 754 float
    float_bytes = struct.pack('<I', pattern)
    float_val = struct.unpack('<f', float_bytes)[0]

    # Convert to double (this is what Python does natively -- float is
    # actually double precision in Python, but struct gives us the
    # single-precision interpretation)

    # Verify round-trip: double -> float should give back same bits
    # For most values this is guaranteed. Check anyway.
    recovered = struct.pack('<f', float_val)
    recovered_pattern = struct.unpack('<I', recovered)[0]

    if recovered_pattern != pattern:
        # This happens with sNaN: Python/hardware quiets it
        return None  # Caller must handle

    return float_val


def split_pointer(target: int):
    """Split an 8-byte value into two doubles for the float-write primitive.

    Returns: (lower_double, upper_double, warnings)
    lower_double: write to addr+0, produces lower 4 bytes
    upper_double: write to addr+4, produces upper 4 bytes
    """
    lower_32 = target & 0xFFFFFFFF
    upper_32 = (target >> 32) & 0xFFFFFFFF

    warnings = []
    results = {}

    for name, pattern in [("lower", lower_32), ("upper", upper_32)]:
        if is_snan_float(pattern):
            # sNaN: ARM64 will quiet it during double->float conversion.
            # The quiet version has bit 22 set. We CANNOT produce this
            # exact pattern through (float)double on ARM64.
            quiet_pattern = pattern | (1 << 22)
            warnings.append(
                f"WARNING: {name} half 0x{pattern:08x} is a signaling NaN! "
                f"ARM64 will quiet it to 0x{quiet_pattern:08x}. "
                f"This pattern CANNOT be written via (float)double cast."
            )
            # Return the closest we can get (qNaN version)
            results[name] = pattern_to_double(quiet_pattern)
        else:
            val = pattern_to_double(pattern)
            if val is None:
                warnings.append(
                    f"WARNING: {name} half 0x{pattern:08x} round-trip failed!"
                )
                results[name] = None
            else:
                results[name] = val

    return results.get("lower"), results.get("upper"), warnings


def double_to_hex(val: float) -> str:
    """Show the 8-byte hex representation of a double."""
    return '0x' + struct.pack('<d', val).hex()


def verify_double(val: float, expected_pattern: int) -> bool:
    """Verify that (float)val produces the expected 32-bit pattern."""
    if val is None:
        return False
    float_bytes = struct.pack('<f', val)
    actual = struct.unpack('<I', float_bytes)[0]
    return actual == expected_pattern


def format_lua_number(val: float) -> str:
    """Format a double as a Lua-friendly decimal literal.
    Uses enough precision to survive Lua's number parsing round-trip.
    """
    if val is None:
        return "IMPOSSIBLE (sNaN)"

    # Check special cases
    if val == 0.0:
        # Distinguish +0 and -0
        sign_byte = struct.pack('<d', val)[7]
        if sign_byte & 0x80:
            return "-0.0"  # Lua: -0.0 or -(0)
        return "0.0"

    import math
    if math.isinf(val):
        return "math.huge" if val > 0 else "-math.huge"
    if math.isnan(val):
        return "0/0"  # Lua's way to get NaN

    # Use repr for full precision (Python gives 17 significant digits)
    s = repr(val)

    # Verify the string round-trips correctly
    recovered = float(s)
    if struct.pack('<d', recovered) != struct.pack('<d', val):
        # Use hex float as fallback (Lua 5.3+ supports this)
        s = val.hex()

    return s


def main():
    test_pointers = [
        0xb4000071c32d5128,
        0xb4000071c3922548,
        0x0000000000000000,
        0x0000002c00000000,
    ]

    if len(sys.argv) > 1:
        test_pointers = [int(x, 16) if x.startswith('0x') else int(x)
                         for x in sys.argv[1:]]

    print("=" * 72)
    print("Float Write Primitive — Pointer Splitter")
    print("=" * 72)
    print()
    print("C code: *(float*)(base + offset) = (float)lua_double_value;")
    print("Two writes needed: offset+0 (lower 32 bits), offset+4 (upper 32 bits)")
    print()

    for ptr in test_pointers:
        print("-" * 72)
        print(f"Target: 0x{ptr:016x}")

        lower_32 = ptr & 0xFFFFFFFF
        upper_32 = (ptr >> 32) & 0xFFFFFFFF
        print(f"  Lower 32 bits: 0x{lower_32:08x}")
        print(f"  Upper 32 bits: 0x{upper_32:08x}")

        lower_dbl, upper_dbl, warnings = split_pointer(ptr)

        for w in warnings:
            print(f"  *** {w}")

        print()
        print(f"  Write #1 (offset+0): {format_lua_number(lower_dbl)}")
        if lower_dbl is not None:
            print(f"    double hex:  {double_to_hex(lower_dbl)}")
            print(f"    float bits:  0x{lower_32:08x}")
            ok = verify_double(lower_dbl, lower_32)
            print(f"    round-trip:  {'PASS' if ok else 'FAIL'}")
            # Show what the float value "means"
            float_val = struct.unpack('<f', struct.pack('<I', lower_32))[0]
            print(f"    float value: {float_val}")

        print()
        print(f"  Write #2 (offset+4): {format_lua_number(upper_dbl)}")
        if upper_dbl is not None:
            print(f"    double hex:  {double_to_hex(upper_dbl)}")
            print(f"    float bits:  0x{upper_32:08x}")
            ok = verify_double(upper_dbl, upper_32)
            print(f"    round-trip:  {'PASS' if ok else 'FAIL'}")
            float_val = struct.unpack('<f', struct.pack('<I', upper_32))[0]
            print(f"    float value: {float_val}")

        print()

    # Analysis of impossible patterns
    print("=" * 72)
    print("IMPOSSIBLE PATTERNS (cannot produce via (float)double on ARM64)")
    print("=" * 72)
    print()
    print("Signaling NaN (sNaN) float patterns:")
    print("  Exponent = 0xFF, bit 22 = 0, remaining mantissa != 0")
    print("  Pattern range: 0x7F800001 - 0x7FBFFFFF (positive sNaN)")
    print("                 0xFF800001 - 0xFFBFFFFF (negative sNaN)")
    print()
    print("  Count: 2 * (2^22 - 1) = 8,388,606 patterns out of 2^32")
    print("  That's 0.195% of all 32-bit patterns.")
    print()
    print("  ARM64 behavior: double->float conversion of sNaN sets bit 22")
    print("  (quiets the NaN), so 0x7F800001 becomes 0x7FC00001.")
    print()
    print("  For heap pointers (0xb4xxxxxxxx): upper half 0xb4000071")
    print("  is NOT a NaN (exponent field = 0x68, not 0xFF). SAFE.")
    print()

    # Check if common Quest heap pointer patterns are safe
    print("Common Quest pointer pattern analysis:")
    common_uppers = [0xb4000071, 0xb4000072, 0x00000071, 0x0000002c]
    for pat in common_uppers:
        exp = (pat >> 23) & 0xFF
        is_nan = is_snan_float(pat)
        print(f"  0x{pat:08x}: exponent=0x{exp:02x}, sNaN={is_nan} — {'BLOCKED' if is_nan else 'OK'}")

    print()
    print("ALL other patterns (99.8%) work perfectly via (float)double cast.")
    print("Normal floats, denormals, zeros, infinities, and quiet NaNs all survive.")


if __name__ == "__main__":
    main()
