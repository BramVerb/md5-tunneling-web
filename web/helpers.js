function F(x, y, z) {
  return ((x & y) | (~x & z)) >>> 0;
}
function G(x, y, z) {
  return ((x & z) | (y & ~z)) >>> 0;
}
function H(x, y, z) {
  return (x ^ y ^ z) >>> 0;
}
function fI(x, y, z) {
  return (y ^ (x | ~z)) >>> 0;
}

function RL(x, n) {
  return ((x << n) | (x >>> (32 - n))) >>> 0;
}
function RR(x, n) {
  return ((x >>> n) | (x << (32 - n))) >>> 0;
}

function FFx(a, b, c, d, x, s, ac) {
  a = (F(b, c, d) + a + x + ac) >>> 0;
  a = RL(a, s);
  return (a + b) >>> 0;
}

function GGx(a, b, c, d, x, s, ac) {
  a = G(b, c, d) + a + x + ac;
  a = RL(a, s);
  return (a + b) >>> 0;
}

function HHx(a, b, c, d, x, s, ac) {
  a = H(b, c, d) + a + x + ac;
  a = RL(a, s);
  return (a + b) >>> 0;
}

function IIx(a, b, c, d, x, s, ac) {
  a = fI(b, c, d) + a + x + ac;
  a = RL(a, s);
  return (a + b) >>> 0;
}

function printf(...args) {
  console.log(...args);
}

function createMD5Object() {
  return {
    a: 0,
    b: 0,
    c: 0,
    d: 0,
    Hx: new Array(16).fill(0),
  };
}
function printHx(Hx) {
  console.log(
    "Hx",
    Hx.slice(0, 16).map((x) => x >>> 0)
  );
}

function HMD5Tr(m) {
  let { a, b, c, d, Hx } = m;
  a = FFx(a, b, c, d, Hx[0], 7, 0xd76aa478); /* 1  - a1 */
  d = FFx(d, a, b, c, Hx[1], 12, 0xe8c7b756); /* 2  - d1 */
  c = FFx(c, d, a, b, Hx[2], 17, 0x242070db); /* 3  - c1 */
  b = FFx(b, c, d, a, Hx[3], 22, 0xc1bdceee); /* 4  - b1 */
  a = FFx(a, b, c, d, Hx[4], 7, 0xf57c0faf); /* 5  - a2 */
  d = FFx(d, a, b, c, Hx[5], 12, 0x4787c62a); /* 6  - d2 */
  c = FFx(c, d, a, b, Hx[6], 17, 0xa8304613); /* 7  - c2 */
  b = FFx(b, c, d, a, Hx[7], 22, 0xfd469501); /* 8  - b2 */
  a = FFx(a, b, c, d, Hx[8], 7, 0x698098d8); /* 9  - a3 */
  d = FFx(d, a, b, c, Hx[9], 12, 0x8b44f7af); /* 10 - d3 */
  c = FFx(c, d, a, b, Hx[10], 17, 0xffff5bb1); /* 11 - c3 */
  b = FFx(b, c, d, a, Hx[11], 22, 0x895cd7be); /* 12 - b3 */
  a = FFx(a, b, c, d, Hx[12], 7, 0x6b901122); /* 13 - a4 */
  d = FFx(d, a, b, c, Hx[13], 12, 0xfd987193); /* 14 - d4 */
  c = FFx(c, d, a, b, Hx[14], 17, 0xa679438e); /* 15 - c4 */
  b = FFx(b, c, d, a, Hx[15], 22, 0x49b40821); /* 16 - b4 */

  a = GGx(a, b, c, d, Hx[1], 5, 0xf61e2562); /* 17 - a5 */
  d = GGx(d, a, b, c, Hx[6], 9, 0xc040b340); /* 18 - d5 */
  c = GGx(c, d, a, b, Hx[11], 14, 0x265e5a51); /* 19 - c5 */
  b = GGx(b, c, d, a, Hx[0], 20, 0xe9b6c7aa); /* 20 - b5 */
  a = GGx(a, b, c, d, Hx[5], 5, 0xd62f105d); /* 21 - a6 */
  d = GGx(d, a, b, c, Hx[10], 9, 0x2441453); /* 22 - d6 */
  c = GGx(c, d, a, b, Hx[15], 14, 0xd8a1e681); /* 23 - c6 */
  b = GGx(b, c, d, a, Hx[4], 20, 0xe7d3fbc8); /* 24 - b6 */
  a = GGx(a, b, c, d, Hx[9], 5, 0x21e1cde6); /* 25 - a7 */
  d = GGx(d, a, b, c, Hx[14], 9, 0xc33707d6); /* 26 - d7 */
  c = GGx(c, d, a, b, Hx[3], 14, 0xf4d50d87); /* 27 - c7 */
  b = GGx(b, c, d, a, Hx[8], 20, 0x455a14ed); /* 28 - b7 */
  a = GGx(a, b, c, d, Hx[13], 5, 0xa9e3e905); /* 29 - a8 */
  d = GGx(d, a, b, c, Hx[2], 9, 0xfcefa3f8); /* 30 - d8 */
  c = GGx(c, d, a, b, Hx[7], 14, 0x676f02d9); /* 31 - c8 */
  b = GGx(b, c, d, a, Hx[12], 20, 0x8d2a4c8a); /* 32 - b8 */

  a = HHx(a, b, c, d, Hx[5], 4, 0xfffa3942); /* 33 - a9 */
  d = HHx(d, a, b, c, Hx[8], 11, 0x8771f681); /* 34 - d9 */
  c = HHx(c, d, a, b, Hx[11], 16, 0x6d9d6122); /* 35 - c9 */
  b = HHx(b, c, d, a, Hx[14], 23, 0xfde5380c); /* 36 - b9 */
  a = HHx(a, b, c, d, Hx[1], 4, 0xa4beea44); /* 37 - a10 */
  d = HHx(d, a, b, c, Hx[4], 11, 0x4bdecfa9); /* 38 - d10 */
  c = HHx(c, d, a, b, Hx[7], 16, 0xf6bb4b60); /* 39 - c10 */
  b = HHx(b, c, d, a, Hx[10], 23, 0xbebfbc70); /* 40 - b10 */
  a = HHx(a, b, c, d, Hx[13], 4, 0x289b7ec6); /* 41 - a11 */
  d = HHx(d, a, b, c, Hx[0], 11, 0xeaa127fa); /* 42 - d11 */
  c = HHx(c, d, a, b, Hx[3], 16, 0xd4ef3085); /* 43 - c11 */
  b = HHx(b, c, d, a, Hx[6], 23, 0x4881d05); /* 44 - b11 */
  a = HHx(a, b, c, d, Hx[9], 4, 0xd9d4d039); /* 45 - a12 */
  d = HHx(d, a, b, c, Hx[12], 11, 0xe6db99e5); /* 46 - d12 */
  c = HHx(c, d, a, b, Hx[15], 16, 0x1fa27cf8); /* 47 - c12 */
  b = HHx(b, c, d, a, Hx[2], 23, 0xc4ac5665); /* 48 - b12 */

  a = IIx(a, b, c, d, Hx[0], 6, 0xf4292244); /* 49 - a13 */
  d = IIx(d, a, b, c, Hx[7], 10, 0x432aff97); /* 50 - d13 */
  c = IIx(c, d, a, b, Hx[14], 15, 0xab9423a7); /* 51 - c13 */
  b = IIx(b, c, d, a, Hx[5], 21, 0xfc93a039); /* 52 - b13 */
  a = IIx(a, b, c, d, Hx[12], 6, 0x655b59c3); /* 53 - a14 */
  d = IIx(d, a, b, c, Hx[3], 10, 0x8f0ccc92); /* 54 - d14 */
  c = IIx(c, d, a, b, Hx[10], 15, 0xffeff47d); /* 55 - c14 */
  b = IIx(b, c, d, a, Hx[1], 21, 0x85845dd1); /* 56 - b14 */
  a = IIx(a, b, c, d, Hx[8], 6, 0x6fa87e4f); /* 57 - a15 */
  d = IIx(d, a, b, c, Hx[15], 10, 0xfe2ce6e0); /* 58 - d15 */
  c = IIx(c, d, a, b, Hx[6], 15, 0xa3014314); /* 59 - c15 */
  b = IIx(b, c, d, a, Hx[13], 21, 0x4e0811a1); /* 60 - b15 */
  a = IIx(a, b, c, d, Hx[4], 6, 0xf7537e82); /* 61 - a16 */
  d = IIx(d, a, b, c, Hx[11], 10, 0xbd3af235); /* 62 - d16 */
  c = IIx(c, d, a, b, Hx[2], 15, 0x2ad7d2bb); /* 63 - c16 */
  b = IIx(b, c, d, a, Hx[9], 21, 0xeb86d391); /* 64 - b16 */
  a = a >>> 0;
  b = b >>> 0;
  c = c >>> 0;
  d = d >>> 0;
  return { a, b, c, d, Hx };
}

function generate_mask(strength, mask_bits) {
  const mask_cardinality = Math.pow(2, strength) >>> 0;

  const mask = new Array(mask_cardinality).fill(0);

  // If more or equal 32 bits tunneling is useless
  if (strength < 32) {
    for (let i = 0; i < mask_cardinality; i++)
      for (let j = 0; j < strength; j++)
        mask[i] = mask[i] ^ (((i >>> j) & 1) << (mask_bits[j] - 1));
    // console.log(`For strength: ${strength} mask_bits: ${mask_bits} mask: ${mask}`)
    return mask;
  } else {
    throw new Error("Uncorrect parameters in mask generation");
  }
}

// Robert Jenkins' 32 bit integer hash function
// Used to generate a good seed for rng()
function mix(a) {
  a = a + 0x7ed55d16 + (a << 12);
  a = a ^ 0xc761c23c ^ (a >>> 19);
  a = a + 0x165667b1 + (a << 5);
  a = (a + 0xd3a2646c) ^ (a << 9);
  a = a + 0xfd7046c5 + (a << 3);
  a = a ^ 0xb55a4f09 ^ (a >>> 16);
  return a >>> 0;
}

let X = 0 >>> 0;
function rng() {
  X = ((1664525 * X + 1013904223) & 0xffffffff) >>> 0;
  //X = (((((1103515245 >>> 0) * X >>>0) + 12345)>>>0) & 0xffffffff) >>> 0;
  return X;
}


const mask_bit = [
  0x0, 0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020,
  0x00000040, 0x00000080, 0x00000100, 0x00000200, 0x00000400, 0x00000800,
  0x00001000, 0x00002000, 0x00004000, 0x00008000, 0x00010000, 0x00020000,
  0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
  0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000,
  0x40000000, 0x80000000,
];

function bit(a, b) {
  if (b == 0 || b > 32) {
    throw new Error("NOT POSSIBLE");
  } else {
    return ((((a >>> 0) & mask_bit[b]) >>> 0) >>> (b - 1)) >>> 0;
  }
}

function newArray(length) {
  return new Array(length).fill(0);
}

function memcpy(a, ai, b, bi, l) {
  if (l !== 4) {
    throw new Error("length should be 4");
  }
  // figured out storing 32 bits as 8 bit numbers
  a[ai + 0] = (b[bi] >>> 0) & 0xff;
  a[ai + 1] = (b[bi] >>> 8) & 0xff;
  a[ai + 2] = (b[bi] >>> 16) & 0xff;
  a[ai + 3] = (b[bi] >>> 24) & 0xff;
}


