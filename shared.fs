#version 300 es
in highp vec2 position;
highp ivec2 pos;
out highp vec4 color;

#define u32 highp uint
#define pow2(a) (1u << a)

uniform highp uint seed;
uniform highp uint rngMask;

uniform highp uint A0;
uniform highp uint B0;
uniform highp uint C0;
uniform highp uint D0;

u32 a,b,c,d;
u32 Hx[16];
u32 IV1,IV2,IV3,IV4;
/* u32 A0,B0,C0,D0; */

u32 tunnel9;
u32 tunnel4;
u32 tunnel14;
u32 tunnel13;
u32 tunnel20;
u32 tunnel10;
u32 tunnel16;
u32 tunnelq1q2;

//Robert Jenkins' 32 bit integer hash function
//Used to generate a good seed for rng()
u32 mix(u32 a) {
   a = (a+0x7ed55d16u) + (a<<12);
   a = (a^0xc761c23cu) ^ (a>>19);
   a = (a+0x165667b1u) + (a<<5);
   a = (a+0xd3a2646cu) ^ (a<<9);
   a = (a+0xfd7046c5u) + (a<<3);
   a = (a^0xb55a4f09u) ^ (a>>16);
   return a;
}


#define USE_B1_Q4 true
#define USE_B1_Q9 true
#define USE_B1_Q10 true
#define USE_B1_Q13 true
#define USE_B1_Q14 true
#define USE_B1_Q20 true
#define USE_B2_Q9 true

#define RL(x, n) (((x) << (n)) | ((x) >> (32u-(n))))
#define RR(x, n) (((x) >> (n)) | ((x) << (32u-(n))))

#define F(x, y, z) (((x) & (y)) | ((~x) & (z)))
#define G(x, y, z) (((x) & (z)) | ((y) & (~z)))
#define H(x, y, z) ((x) ^ (y) ^ (z))
#define I(x, y, z) ((y) ^ ((x) | (~z)))

#define FFx(a, b, c, d, x, s, ac) { \
 (a) = F ((b), (c), (d)) + (a) + (x) + (ac); \
 (a) = RL ((a), (s)); \
 (a) += (b); \
 }

#define GGx(a, b, c, d, x, s, ac) { \
 (a) = G ((b), (c), (d)) + (a) + (x) + (ac); \
 (a) = RL ((a), (s)); \
 (a) += (b); \
 }

#define HHx(a, b, c, d, x, s, ac) { \
 (a) = H ((b), (c), (d)) + (a) + (x) + (ac); \
 (a) = RL ((a), (s)); \
 (a) += (b); \
 }

#define IIx(a, b, c, d, x, s, ac) { \
 (a) = I ((b), (c), (d)) + (a) + (x) + (ac); \
 (a) = RL ((a), (s)); \
 (a) += (b); \
 }

//Random number generator. We will use an LCG pseudo random generator. Different options are possible
u32 X;
u32 rng() {
  X = (1664525u*X + 1013904223u) & 0xffffffffu;
  /* X = (1103515245u*X + 12345u) & 0xffffffffu; */
  return X & rngMask;
}

void HMD5Tr() {

  FFx(a, b, c, d, Hx[ 0],  7u, 0xd76aa478u); /* 1  - a1 */
  FFx(d, a, b, c, Hx[ 1], 12u, 0xe8c7b756u); /* 2  - d1 */
  FFx(c, d, a, b, Hx[ 2], 17u, 0x242070dbu); /* 3  - c1 */
  FFx(b, c, d, a, Hx[ 3], 22u, 0xc1bdceeeu); /* 4  - b1 */
  FFx(a, b, c, d, Hx[ 4],  7u, 0xf57c0fafu); /* 5  - a2 */
  FFx(d, a, b, c, Hx[ 5], 12u, 0x4787c62au); /* 6  - d2 */
  FFx(c, d, a, b, Hx[ 6], 17u, 0xa8304613u); /* 7  - c2 */
  FFx(b, c, d, a, Hx[ 7], 22u, 0xfd469501u); /* 8  - b2 */
  FFx(a, b, c, d, Hx[ 8],  7u, 0x698098d8u); /* 9  - a3 */
  FFx(d, a, b, c, Hx[ 9], 12u, 0x8b44f7afu); /* 10 - d3 */
  FFx(c, d, a, b, Hx[10], 17u, 0xffff5bb1u); /* 11 - c3 */
  FFx(b, c, d, a, Hx[11], 22u, 0x895cd7beu); /* 12 - b3 */
  FFx(a, b, c, d, Hx[12],  7u, 0x6b901122u); /* 13 - a4 */
  FFx(d, a, b, c, Hx[13], 12u, 0xfd987193u); /* 14 - d4 */
  FFx(c, d, a, b, Hx[14], 17u, 0xa679438eu); /* 15 - c4 */
  FFx(b, c, d, a, Hx[15], 22u, 0x49b40821u); /* 16 - b4 */

  GGx(a, b, c, d, Hx[ 1],  5u, 0xf61e2562u); /* 17 - a5 */
  GGx(d, a, b, c, Hx[ 6],  9u, 0xc040b340u); /* 18 - d5 */
  GGx(c, d, a, b, Hx[11], 14u, 0x265e5a51u); /* 19 - c5 */
  GGx(b, c, d, a, Hx[ 0], 20u, 0xe9b6c7aau); /* 20 - b5 */
  GGx(a, b, c, d, Hx[ 5],  5u, 0xd62f105du); /* 21 - a6 */
  GGx(d, a, b, c, Hx[10],  9u,  0x2441453u); /* 22 - d6 */
  GGx(c, d, a, b, Hx[15], 14u, 0xd8a1e681u); /* 23 - c6 */
  GGx(b, c, d, a, Hx[ 4], 20u, 0xe7d3fbc8u); /* 24 - b6 */
  GGx(a, b, c, d, Hx[ 9],  5u, 0x21e1cde6u); /* 25 - a7 */
  GGx(d, a, b, c, Hx[14],  9u, 0xc33707d6u); /* 26 - d7 */
  GGx(c, d, a, b, Hx[ 3], 14u, 0xf4d50d87u); /* 27 - c7 */
  GGx(b, c, d, a, Hx[ 8], 20u, 0x455a14edu); /* 28 - b7 */
  GGx(a, b, c, d, Hx[13],  5u, 0xa9e3e905u); /* 29 - a8 */
  GGx(d, a, b, c, Hx[ 2],  9u, 0xfcefa3f8u); /* 30 - d8 */
  GGx(c, d, a, b, Hx[ 7], 14u, 0x676f02d9u); /* 31 - c8 */
  GGx(b, c, d, a, Hx[12], 20u, 0x8d2a4c8au); /* 32 - b8 */

  HHx(a, b, c, d, Hx[ 5],  4u, 0xfffa3942u); /* 33 - a9 */
  HHx(d, a, b, c, Hx[ 8], 11u, 0x8771f681u); /* 34 - d9 */
  HHx(c, d, a, b, Hx[11], 16u, 0x6d9d6122u); /* 35 - c9 */
  HHx(b, c, d, a, Hx[14], 23u, 0xfde5380cu); /* 36 - b9 */
  HHx(a, b, c, d, Hx[ 1],  4u, 0xa4beea44u); /* 37 - a10 */
  HHx(d, a, b, c, Hx[ 4], 11u, 0x4bdecfa9u); /* 38 - d10 */
  HHx(c, d, a, b, Hx[ 7], 16u, 0xf6bb4b60u); /* 39 - c10 */
  HHx(b, c, d, a, Hx[10], 23u, 0xbebfbc70u); /* 40 - b10 */
  HHx(a, b, c, d, Hx[13],  4u, 0x289b7ec6u); /* 41 - a11 */
  HHx(d, a, b, c, Hx[ 0], 11u, 0xeaa127fau); /* 42 - d11 */
  HHx(c, d, a, b, Hx[ 3], 16u, 0xd4ef3085u); /* 43 - c11 */
  HHx(b, c, d, a, Hx[ 6], 23u,  0x4881d05u); /* 44 - b11 */
  HHx(a, b, c, d, Hx[ 9],  4u, 0xd9d4d039u); /* 45 - a12 */
  HHx(d, a, b, c, Hx[12], 11u, 0xe6db99e5u); /* 46 - d12 */
  HHx(c, d, a, b, Hx[15], 16u, 0x1fa27cf8u); /* 47 - c12 */
  HHx(b, c, d, a, Hx[ 2], 23u, 0xc4ac5665u); /* 48 - b12 */

  IIx(a, b, c, d, Hx[ 0],  6u, 0xf4292244u); /* 49 - a13 */
  IIx(d, a, b, c, Hx[ 7], 10u, 0x432aff97u); /* 50 - d13 */
  IIx(c, d, a, b, Hx[14], 15u, 0xab9423a7u); /* 51 - c13 */
  IIx(b, c, d, a, Hx[ 5], 21u, 0xfc93a039u); /* 52 - b13 */
  IIx(a, b, c, d, Hx[12],  6u, 0x655b59c3u); /* 53 - a14 */
  IIx(d, a, b, c, Hx[ 3], 10u, 0x8f0ccc92u); /* 54 - d14 */
  IIx(c, d, a, b, Hx[10], 15u, 0xffeff47du); /* 55 - c14 */
  IIx(b, c, d, a, Hx[ 1], 21u, 0x85845dd1u); /* 56 - b14 */
  IIx(a, b, c, d, Hx[ 8],  6u, 0x6fa87e4fu); /* 57 - a15 */
  IIx(d, a, b, c, Hx[15], 10u, 0xfe2ce6e0u); /* 58 - d15 */
  IIx(c, d, a, b, Hx[ 6], 15u, 0xa3014314u); /* 59 - c15 */
  IIx(b, c, d, a, Hx[13], 21u, 0x4e0811a1u); /* 60 - b15 */
  IIx(a, b, c, d, Hx[ 4],  6u, 0xf7537e82u); /* 61 - a16 */
  IIx(d, a, b, c, Hx[11], 10u, 0xbd3af235u); /* 62 - d16 */
  IIx(c, d, a, b, Hx[ 2], 15u, 0x2ad7d2bbu); /* 63 - c16 */
  IIx(b, c, d, a, Hx[ 9], 21u, 0xeb86d391u); /* 64 - b16 */
}



const u32 mask_bit[33] = uint[]( 0x0u,
        0x00000001u,0x00000002u,0x00000004u,0x00000008u,0x00000010u,0x00000020u,0x00000040u,0x00000080u,
        0x00000100u,0x00000200u,0x00000400u,0x00000800u,0x00001000u,0x00002000u,0x00004000u,0x00008000u,
        0x00010000u,0x00020000u,0x00040000u,0x00080000u,0x00100000u,0x00200000u,0x00400000u,0x00800000u,
        0x01000000u,0x02000000u,0x04000000u,0x08000000u,0x10000000u,0x20000000u,0x40000000u,0x80000000u );

u32 bit(u32 a, u32 b) {
    if ((b==0u) || (b > 32u))
      return 0u;
    else
      return (a & mask_bit[b]) >> (b-1u);
}

u32 mask_Q(int strength, int[12] mask_bits, u32 i) {
    u32 mask = 0u;
    for (int j=0; j<strength; j++)
        mask = mask ^ (((i >> j) & 1u) << (mask_bits[j]-1));
    return mask;
}

highp vec4 return_vec(int r){
  highp float x = float((r >> 0) & 0xff) / 255.0;
  highp float y = float((r >> 8) & 0xff) / 255.0;
  highp float z = float((r >>16) & 0xff) / 255.0;
  return vec4(x, y, z, 1.0);
}

highp vec4 return_vec(uint r){
  highp float x = float((r >> 0) & 0xffu) / 255.0;
  highp float y = float((r >> 8) & 0xffu) / 255.0;
  highp float z = float((r >>16) & 0xffu) / 255.0;
  return vec4(x, y, z, 1.0);
}

u32 Q[65];
