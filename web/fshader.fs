#version 300 es
in highp vec2 position;
highp ivec2 pos;

uniform highp uint seed;

out highp vec4 color;

#define u32 highp uint

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


u32 a,b,c,d;
u32 Hx[16];
u32 IV1,IV2,IV3,IV4;
u32 A0,B0,C0,D0, A1,B1,C1,D1;

u32 tunnel9;
u32 tunnel4;
u32 tunnel14;
u32 tunnel13;
u32 tunnel20;
u32 tunnel10;

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

u32 pow2(u32 a) {
    return 1u << a;
}
u32 pow2(int a) {
    return 1u << a;
}


//Random number generator. We will use an LCG pseudo random generator. Different options are possible
u32 X;
u32 rng() {
  X = (1664525u*X + 1013904223u) & 0xffffffffu;
  //X = (1103515245*X + 12345) & 0xffffffffu;
  return X;
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




/* u32 * generate_mask(u32 strength, u32 * mask_bits) { */

/*         u32 mask_cardinality = (int32_t) pow2( strength); */

/*         u32 * mask = calloc( mask_cardinality , sizeof(u32) ); */

/*         //If more or equal 32 bits tunneling is useless */
/*         if (strength < 32) { */

/*                 u32 i,j; */

/*                 for (i=0; i<mask_cardinality; i++) */
/*                         for (j=0; j<strength; j++) */
/*                                 mask[i] = mask[i] ^ (((i >> j) & 1) << (mask_bits[j]-1)); */
/*                 return mask; */
/*         } */

/*         else { */
/*                 printf("Uncorrect parameters in mask generation.\n"); */
/*                 return NULL; */
/*         } */

/* } */


u32 mask_Q(int strength, int[12] mask_bits, u32 i) {
    u32 mask = 0u;
    for (int j=0; j<strength; j++)
        mask = mask ^ (((i >> j) & 1u) << (mask_bits[j]-1));
    return mask;
}
u32 Q[65];
int Block1(uint id) {

  u32 x[16];
  u32 sigma_Q19, sigma_Q20, sigma_Q23, sigma_Q35, sigma_Q62;
  u32 i, itr_Q9, itr_Q4, itr_Q14, itr_Q13, itr_Q20, itr_Q10;
  u32 tmp_q3, tmp_q4, tmp_q13, tmp_q14, tmp_q20, tmp_q21, tmp_q9, tmp_q10;
  u32 tmp_x1, tmp_x15, tmp_x4;
  u32 Q3_fix, Q4_fix, Q14_fix, const_masked, const_unmasked;
  u32 AA0, BB0, CC0, DD0, AA1, BB1, CC1, DD1;

  //Mask generation for tunnel Q4 - 1 bit
  int Q4_mask_bits[] = int[]( 26 );
  int Q4_strength = 1;
  u32 mask_Q4[] = uint[](0u,33554432u);
  /* const u32 * mask_Q4 = generate_mask(Q4_strength, Q4_mask_bits); */
  u32 startQ4 = id & ((1u << Q4_strength) - 1u);
  u32 endQ4 = startQ4 + 1u; // (USE_B1_Q4 ? pow2(Q4_strength) : 1u);
  id = id >> Q4_strength;

  //Mask generation for tunnel Q9 - 3 bits
  int Q9_mask_bits[] = int[]( 22, 23, 24 );
  int Q9_strength = 3;
  /* const u32 * mask_Q9 = generate_mask(Q9_strength, Q9_mask_bits); */ 
  u32 mask_Q9[] = uint[](0u,2097152u,4194304u,6291456u,8388608u,10485760u,12582912u,14680064u);
  u32 startQ9 = id & ((1u << Q9_strength) - 1u);
  u32 endQ9 = startQ9 + 1u; // (USE_B1_Q9 ? pow2(Q9_strength) : 1u);
  id = id >> Q9_strength;

  //Mask generation for tunnel Q13 - 12 bits
  int Q13_mask_bits[] = int[]( 2,3,5,7,10,11,12,21,22,23,28,29 );
  int Q13_strength = 12;
  /* const u32 * mask_Q13 = generate_mask(Q13_strength, Q13_mask_bits); */ 
  u32 startQ13 = id & ((1u << Q13_strength) - 1u);
  u32 endQ13 = startQ13 + 1u; // (USE_B1_Q13 ? pow2(Q13_strength) : 1u);
  id = id >> Q13_strength;

  //Mask generation for tunnel Q20 - 6 bits
  int Q20_mask_bits[] = int[]( 1, 2, 10, 15, 22, 24 );
  int Q20_strength = 6;
  /* const u32 * mask_Q20 = generate_mask(Q20_strength, Q20_mask_bits); */ 
  u32 mask_Q20[] = uint[]( 0u,1u,2u,3u,512u,513u,514u,515u,16384u,16385u,16386u,16387u,16896u,16897u,16898u,16899u,2097152u,2097153u,2097154u,2097155u,2097664u,2097665u,2097666u,2097667u,2113536u,2113537u,2113538u,2113539u,2114048u,2114049u,2114050u,2114051u,8388608u,8388609u,8388610u,8388611u,8389120u,8389121u,8389122u,8389123u,8404992u,8404993u,8404994u,8404995u,8405504u,8405505u,8405506u,8405507u,10485760u,10485761u,10485762u,10485763u,10486272u,10486273u,10486274u,10486275u,10502144u,10502145u,10502146u,10502147u,10502656u,10502657u,10502658u,10502659u);
  /* u32 startQ20 = id & ((1u << Q20_strength) - 1u); */
  /* u32 endQ20 = startQ20 + 1u; // (USE_B1_Q20 ? pow2(Q20_strength) : 1u); */
  /* id = id >> Q20_strength; */

  //Mask generation for tunnel Q10 - 3 bits
  int Q10_mask_bits[] = int[]( 11, 25, 27 );
  int Q10_strength = 3;
  /* const u32 * mask_Q10 = generate_mask(Q10_strength, Q10_mask_bits); */ 
  u32 mask_Q10[] = uint[](0u,1024u,16777216u,16778240u,67108864u,67109888u,83886080u,83887104u);
  /* u32 startQ10 = id & ((1u << Q10_strength) - 1u); */
  /* u32 endQ10 = startQ10 + 1u; // (USE_B1_Q10 ? pow2(Q10_strength) : 1u); */
  /* id = id >> Q10_strength; */

  //Mask generation for tunnel Q14 - 9 bits
  int Q14_mask_bits[] = int[]( 1, 2, 3, 5, 6, 7, 27, 28, 29 );
  int Q14_strength = 9;
  /* const u32 * mask_Q14 = generate_mask(Q14_strength, Q14_mask_bits); */ 
  u32 mask_Q14[] = uint[](0u,1u,2u,3u,4u,5u,6u,7u,16u,17u,18u,19u,20u,21u,22u,23u,32u,33u,34u,35u,36u,37u,38u,39u,48u,49u,50u,51u,52u,53u,54u,55u,64u,65u,66u,67u,68u,69u,70u,71u,80u,81u,82u,83u,84u,85u,86u,87u,96u,97u,98u,99u,100u,101u,102u,103u,112u,113u,114u,115u,116u,117u,118u,119u,67108864u,67108865u,67108866u,67108867u,67108868u,67108869u,67108870u,67108871u,67108880u,67108881u,67108882u,67108883u,67108884u,67108885u,67108886u,67108887u,67108896u,67108897u,67108898u,67108899u,67108900u,67108901u,67108902u,67108903u,67108912u,67108913u,67108914u,67108915u,67108916u,67108917u,67108918u,67108919u,67108928u,67108929u,67108930u,67108931u,67108932u,67108933u,67108934u,67108935u,67108944u,67108945u,67108946u,67108947u,67108948u,67108949u,67108950u,67108951u,67108960u,67108961u,67108962u,67108963u,67108964u,67108965u,67108966u,67108967u,67108976u,67108977u,67108978u,67108979u,67108980u,67108981u,67108982u,67108983u,134217728u,134217729u,134217730u,134217731u,134217732u,134217733u,134217734u,134217735u,134217744u,134217745u,134217746u,134217747u,134217748u,134217749u,134217750u,134217751u,134217760u,134217761u,134217762u,134217763u,134217764u,134217765u,134217766u,134217767u,134217776u,134217777u,134217778u,134217779u,134217780u,134217781u,134217782u,134217783u,134217792u,134217793u,134217794u,134217795u,134217796u,134217797u,134217798u,134217799u,134217808u,134217809u,134217810u,134217811u,134217812u,134217813u,134217814u,134217815u,134217824u,134217825u,134217826u,134217827u,134217828u,134217829u,134217830u,134217831u,134217840u,134217841u,134217842u,134217843u,134217844u,134217845u,134217846u,134217847u,201326592u,201326593u,201326594u,201326595u,201326596u,201326597u,201326598u,201326599u,201326608u,201326609u,201326610u,201326611u,201326612u,201326613u,201326614u,201326615u,201326624u,201326625u,201326626u,201326627u,201326628u,201326629u,201326630u,201326631u,201326640u,201326641u,201326642u,201326643u,201326644u,201326645u,201326646u,201326647u,201326656u,201326657u,201326658u,201326659u,201326660u,201326661u,201326662u,201326663u,201326672u,201326673u,201326674u,201326675u,201326676u,201326677u,201326678u,201326679u,201326688u,201326689u,201326690u,201326691u,201326692u,201326693u,201326694u,201326695u,201326704u,201326705u,201326706u,201326707u,201326708u,201326709u,201326710u,201326711u,268435456u,268435457u,268435458u,268435459u,268435460u,268435461u,268435462u,268435463u,268435472u,268435473u,268435474u,268435475u,268435476u,268435477u,268435478u,268435479u,268435488u,268435489u,268435490u,268435491u,268435492u,268435493u,268435494u,268435495u,268435504u,268435505u,268435506u,268435507u,268435508u,268435509u,268435510u,268435511u,268435520u,268435521u,268435522u,268435523u,268435524u,268435525u,268435526u,268435527u,268435536u,268435537u,268435538u,268435539u,268435540u,268435541u,268435542u,268435543u,268435552u,268435553u,268435554u,268435555u,268435556u,268435557u,268435558u,268435559u,268435568u,268435569u,268435570u,268435571u,268435572u,268435573u,268435574u,268435575u,335544320u,335544321u,335544322u,335544323u,335544324u,335544325u,335544326u,335544327u,335544336u,335544337u,335544338u,335544339u,335544340u,335544341u,335544342u,335544343u,335544352u,335544353u,335544354u,335544355u,335544356u,335544357u,335544358u,335544359u,335544368u,335544369u,335544370u,335544371u,335544372u,335544373u,335544374u,335544375u,335544384u,335544385u,335544386u,335544387u,335544388u,335544389u,335544390u,335544391u,335544400u,335544401u,335544402u,335544403u,335544404u,335544405u,335544406u,335544407u,335544416u,335544417u,335544418u,335544419u,335544420u,335544421u,335544422u,335544423u,335544432u,335544433u,335544434u,335544435u,335544436u,335544437u,335544438u,335544439u,402653184u,402653185u,402653186u,402653187u,402653188u,402653189u,402653190u,402653191u,402653200u,402653201u,402653202u,402653203u,402653204u,402653205u,402653206u,402653207u,402653216u,402653217u,402653218u,402653219u,402653220u,402653221u,402653222u,402653223u,402653232u,402653233u,402653234u,402653235u,402653236u,402653237u,402653238u,402653239u,402653248u,402653249u,402653250u,402653251u,402653252u,402653253u,402653254u,402653255u,402653264u,402653265u,402653266u,402653267u,402653268u,402653269u,402653270u,402653271u,402653280u,402653281u,402653282u,402653283u,402653284u,402653285u,402653286u,402653287u,402653296u,402653297u,402653298u,402653299u,402653300u,402653301u,402653302u,402653303u,469762048u,469762049u,469762050u,469762051u,469762052u,469762053u,469762054u,469762055u,469762064u,469762065u,469762066u,469762067u,469762068u,469762069u,469762070u,469762071u,469762080u,469762081u,469762082u,469762083u,469762084u,469762085u,469762086u,469762087u,469762096u,469762097u,469762098u,469762099u,469762100u,469762101u,469762102u,469762103u,469762112u,469762113u,469762114u,469762115u,469762116u,469762117u,469762118u,469762119u,469762128u,469762129u,469762130u,469762131u,469762132u,469762133u,469762134u,469762135u,469762144u,469762145u,469762146u,469762147u,469762148u,469762149u,469762150u,469762151u,469762160u,469762161u,469762162u,469762163u,469762164u,469762165u,469762166u,469762167u);

  //Initialization vectors
  u32 QM3 = IV1;  
  u32 QM0 = IV2;
  u32 QM1 = IV3;  
  u32 QM2 = IV4;


  //Start block 1 generation. 
  //TO-DO: add a time limit for collision search.
  for(int it = 0; it < 2; it++) {
    /* return 0; */

    // Q[1]  = .... .... .... .... .... .... .... .... 
    // RNG   = **** **** **** **** **** **** **** ****  0xffffffff
    // 0     = .... .... .... .... .... .... .... ....  0x00000000
    // 1     = .... .... .... .... .... .... .... ....  0x00000000
    Q[1]  = rng();

    // Q[2] will be generated from x[1] using Q[14..17]

    // Q[3]  = .... .... .vvv 0vvv vvvv 0vvv v0.. .... 
    // RNG   = **** **** **** .*** **** .*** *.** ****  0xfff7f7bf
    // 0     = .... .... .... *... .... *... .*.. ....  0x00080840u   
    // 1     = .... .... .... .... .... .... .... ....  0x00000000
    Q[3]  = rng() & 0xfff7f7bfu;

    // Q[4]  = 1... .... 0^^^ 1^^^ ^^^^ 1^^^ ^011 .... 
    // RNG   = .*** **** .... .... .... .... .... ****  0x7f00000f
    // 0     = .... .... *... .... .... .... .*.. ....  0x00800040
    // 1     = *... .... .... *... .... *... ..** ....  0x80080830
    // Q[3]  = .... .... .*** .*** **** .*** *... ....  0x0077f780
    Q[4]  = (rng() & 0x7f00000fu) + 0x80080830u + (Q[3] & 0x0077f780u);

    // I set bit 2 and 4 to zero, not necessary for Q14 tunnel
    // Q[5]  = 1000 100v 0100 0000 0000 0000 0010 0101 
    // RNG   = .... ...* .... .... .... .... .... ....  0x01000000
    // 0     = .*** .**. *.** **** **** **** **.* *.*.  0x76bfffda
    // 1     = *... *... .*.. .... .... .... ..*. .*.*  0x88400025
    Q[5]  = (rng() & 0x01000000u) + 0x88400025u;

    // I set bit 2 and 4 to zero, not necessary for Q14 tunnel
    // Q[6]  = 0000 001^ 0111 1111 1011 1100 0100 0001 
    // RNG   = .... .... .... .... .... .... .... ....  0x00000000
    // 0     = **** **.. *... .... .*.. ..** *.** ***.  0xfc8043be
    // 1     = .... ..*. .*** **** *.** **.. .*.. ...*  0x027fbc41
    // Q[5]  = .... ...* .... .... .... .... .... ....  0x01000000
    Q[6]  = 0x027fbc41u + (Q[ 5] & 0x01000000u);

    // Q[7]  = 0000 0011 1111 1110 1111 1000 0010 0000 
    // RNG   = .... .... .... .... .... .... .... ....  0x00000000
    // 0     = **** **.. .... ...* .... .*** **.* ****  0xfc0107df
    // 1     = .... ..** **** ***. **** *... ..*. ....  0x03fef820
    Q[7]  = 0x03fef820u;

    // Q[8]  = 0000 0001 1..1 0001 0.0v 0101 0100 0000 
    // RNG   = .... .... .**. .... .*.* .... .... ....  0x00605000
    // 0     = **** ***. .... ***. *.*. *.*. *.** ****  0xfe0eaabf
    // 1     = .... ...* *..* ...* .... .*.* .*.. ....  0x01910540
    Q[8]  = (rng() & 0x00605000u) + 0x01910540u;

    // Q[9]  = 1111 1011 ...1 0000 0.1^ 1111 0011 1101 
    // RNG   = .... .... ***. .... .*.. .... .... ....  0x00e04000
    // 0     = .... .*.. .... **** *... .... **.. ..*.  0x040f80c2
    // 1     = **** *.** ...* .... ..*. **** ..** **.*  0xfb102f3d
    // Q[8]  = .... .... .... .... ...* .... .... ....  0x00001000
    Q[9]  = (rng() & 0x00e04000u) + 0xfb102f3du + (Q[ 8] & 0x00001000u);

    // Q[10] = 0111 .... 0001 1111 1v01 ...0 01.. ..00 
    // RNG   = .... **** .... .... .*.. ***. ..** **..  0x0f004e3c
    // 0     = *... .... ***. .... ..*. ...* *... ..**  0x80e02183
    // 1     = .*** .... ...* **** *..* .... .*.. ....  0x701f9040
    Q[10] = (rng() & 0x0f004e3cu) + 0x701f9040u;

    // Q[11] = 0010 .0v0 111. 0001 1^00 .0.0 11.. ..10 
    // RNG   = .... *.*. ...* .... .... *.*. ..** **..  0x0a100a3c
    // 0     = **.* .*.* .... ***. ..** .*.* .... ...*  0xd50e3501u   
    // 1     = ..*. .... ***. ...* *... .... **.. ..*.  0x20e180c2
    // Q[10] = .... .... .... .... .*.. .... .... ....  0x00004000
    Q[11] = (rng() & 0x0a100a3cu) + 0x20e180c2u + (Q[10] & 0x00004000u);

    // Q[12] = 000. ..^^ .... 1000 0001 ...1 0... .... 
    // RNG   = ...* **.. **** .... .... ***. .*** ****  0x1cf00e7f
    // 0     = ***. .... .... .*** ***. .... *... ....  0xe007e080
    // 1     = .... .... .... *... ...* ...* .... ....  0x00081100
    // Q[11] = .... ..** .... .... .... .... .... ....  0x03000000
    Q[12] = (rng() & 0x1cf00e7fu) + 0x00081100u + (Q[11] & 0x03000000u);

    // Q[13] = 01.. ..01 .... 1111 111. ...0 0... 1... 
    // RNG   = ..** **.. **** .... ...* ***. .*** .***  0x3cf01e77
    // 0     = *... ..*. .... .... .... ...* *... ....  0x82000180
    // 1     = .*.. ...* .... **** ***. .... .... *...  0x410fe008
    Q[13] = (rng() & 0x3cf01e77u) + 0x410fe008u;

    // Q[14] = 000. ..00 .... 1011 111. ...1 1... 1... 
    // RNG   = ...* **.. **** .... ...* ***. .*** .***  0x1cf01e77
    // 0     = ***. ..** .... .*.. .... .... .... ....  0xe3040000
    // 1     = .... .... .... *.** ***. ...* *... *...  0x000be188
    Q[14] = (rng() & 0x1cf01e77u) + 0x000be188u;

    // Q[15] = v110 0001 ..V. .... 10.. .... .000 0000 
    // RNG   = *... .... **** **** ..** **** *... ....  0x80ff3f80
    // 0     = ...* ***. .... .... .*.. .... .*** ****  0x1e00407f
    // 1     = .**. ...* .... .... *... .... .... ....  0x61008000
    Q[15] = (rng() & 0x80ff3f80u) + 0x61008000u;

    // Q[16] = ^010 00.. ..A. .... v... .... .000 v000 
    // RNG   = .... ..** **.* **** **** **** *... *...  0x03dfff88
    // 0     = .*.* **.. .... .... .... .... .*** .***  0x5c000077
    // 1     = ..*. .... .... .... .... .... .... ....  0x20000000
    // Q[15] = *... .... .... .... .... .... .... ....  0x80000000
    // ~Q[15]= .... .... ..*. .... .... .... .... ....  0x00200000
    Q[16] = (rng() & 0x03dfff88u) + 0x20000000u + (Q[15] & 0x80000000u) + ((~Q[15]) & 0x00200000u);

    // Q[17] = ^1v. .... .... ..0. ^... .... .... ^... 
    // RNG   = ..** **** **** **.* .*** **** **** .***  0x3ffd7ff7
    // 0     = .... .... .... ..*. .... .... .... ....  0x00020000
    // 1     = .*.. .... .... .... .... .... .... ....  0x40000000
    // Q[16] = *... .... .... .... *... .... .... *...  0x80008008
    Q[17] = (rng() & 0x3ffd7ff7u) + 0x40000000u + (Q[16] & 0x80008008u);


    //Start message creation
    x[ 0] = RR(Q[ 1] - QM0  ,  7u) - F(QM0  , QM1  , QM2  ) - QM3   - 0xd76aa478u; 
    x[ 1] = RR(Q[17] - Q[16],  5u) - G(Q[16], Q[15], Q[14]) - Q[13] - 0xf61e2562u;
    x[ 4] = RR(Q[ 5] - Q[ 4],  7u) - F(Q[ 4], Q[ 3], Q[ 2]) - Q[ 1] - 0xf57c0fafu;
    x[ 5] = RR(Q[ 6] - Q[ 5], 12u) - F(Q[ 5], Q[ 4], Q[ 3]) - Q[ 2] - 0x4787c62au;
    x[ 6] = RR(Q[ 7] - Q[ 6], 17u) - F(Q[ 6], Q[ 5], Q[ 4]) - Q[ 3] - 0xa8304613u; 
    x[10] = RR(Q[11] - Q[10], 17u) - F(Q[10], Q[ 9], Q[ 8]) - Q[ 7] - 0xffff5bb1u; 
    x[11] = RR(Q[12] - Q[11], 22u) - F(Q[11], Q[10], Q[ 9]) - Q[ 8] - 0x895cd7beu; 
    x[15] = RR(Q[16] - Q[15], 22u) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821u; 


    // Q[2] = .... .... .... .... .... .... .... .... 
    Q[ 2] = Q[ 1] + RL( F(Q[ 1],QM0  ,QM1  ) + QM2   + x[1] + 0xe8c7b756u,12u);

    // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[18] = Q[17] + RL( G(Q[17],Q[16],Q[15]) + Q[14] + x[6] + 0xc040b340u, 9u);

    // Q[17] = ^1v. .... .... ..0. ^... .... .... ^... 
    // Q[18] = ^.^. .... .... ..1. .... .... .... .... 
    //         1010 0000 0000 0010 0000 0000 0000 0000  0xa0020000
    if ( ((Q[18] ^ Q[17]) & 0xa0020000u) != 0x00020000u ) 
      continue;

    // Q[19] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Extra conditions: Σ19,4 ~ Σ19,18 not all 1
    // 0x0003fff8u = 0000 0000 0000 0011 1111 1111 1111 1000
    sigma_Q19 = G(Q[18],Q[17],Q[16]) + Q[15] + x[11] + 0x265e5a51u;
    if ( (sigma_Q19 & 0x0003fff8u) == 0x0003fff8u ) 
      continue;

    Q[19] = Q[18] + RL(sigma_Q19, 14u);

    // Q[18] = ^.^. .... .... ..1. .... .... .... .... 
    // Q[19] = ^... .... .... ..0. .... .... .... .... 
    //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
    if ( ((Q[19] ^ Q[18]) & 0x80020000u) != 0x00020000u ) 
      continue;

    // Q[20] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Extra conditions: Σ20,30 ~ Σ20,32 not all 0
    // 0xe0000000u = 1110 0000 0000 0000 0000 0000 0000 0000
    sigma_Q20 = G(Q[19],Q[18],Q[17]) + Q[16] + x[0] + 0xe9b6c7aau;
    if ( (sigma_Q20  & 0xe0000000u) == 0u ) 
      continue;

    Q[20] = Q[19] + RL(sigma_Q20, 20u);

    // Q[20] = ^... .... .... ..v. .... .... .... .... 
    if ( bit(Q[20],32u) != bit(Q[15],32u) ) 
      continue;

    // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5u);   

    // Q[20] = ^... .... .... ..v. .... .... .... .... 
    // Q[21] = ^... .... .... ..^. .... .... .... ....
    //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
    if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0u ) 
      continue;

    // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9u);

    // Q[22] = ^... .... .... .... .... .... .... ....
    if ( bit(Q[22],32u) != bit(Q[15],32u) ) 
      continue;

    // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Extra conditions: Σ23,18 = 0
    sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
    if ( bit(sigma_Q23,18u) != 0u ) 
      continue;

    Q[23] = Q[22] + RL(sigma_Q23, 14u);

    // Q[23] = 0... .... .... .... .... .... .... ....
    if ( bit(Q[23],32u) != 0u ) 
      continue;

    // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20u);

    // Q[24] = 1... .... .... .... .... .... .... ....
    if ( bit(Q[24],32u) != 1u) 
      continue;

    //Every bit condition in Q[1]..Q[24] is now satisfied. We proceed with tunnelling.


    //Temporary variables to perform Multiple Messages modifications.
    tmp_x1  = x[1];
    tmp_x4  = x[4];
    tmp_x15 = x[15];

    tmp_q3  = Q[3];
    tmp_q4  = Q[4];
    tmp_q9  = Q[9];
    tmp_q10 = Q[10];
    tmp_q13 = Q[13];
    tmp_q14 = Q[14];
    tmp_q20 = Q[20];
    tmp_q21 = Q[21];

    ///////////////////////////////////////////////////////////////
    ///                       Tunnel Q10                         //
    ///////////////////////////////////////////////////////////////
    //Tunnel Q10 - 3 bits - Probabilistic. Modifications on x[10] disturb probabilistically conditions for Q[22-24]
    for (itr_Q10 = 0u; itr_Q10 < (USE_B1_Q10 ? pow2(Q10_strength) : 1u); itr_Q10++ ) {

      Q[9]  = tmp_q9;
      Q[10] = tmp_q10;  
      Q[13] = tmp_q13;
      Q[20] = tmp_q20;
      Q[21] = tmp_q21;

      x[4]  = tmp_x4;
      x[15] = tmp_x15;

      //Multi message modification - Q10 is modified according to its mask (bits 11,25,27)
      Q[10] = tmp_q10 ^ mask_Q10[USE_B1_Q10 ? itr_Q10 : 0u];
      
      //x[10] is modified and related states are regenerated
      x[10] = RR(Q[11]-Q[10],17u) - F(Q[10],Q[ 9],Q[ 8]) - Q[ 7] - 0xffff5bb1u; 

      //Q10 Tunnel - Verification of bit conditions on Q[22-24]
      
      // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9u);

      // Q[22] = ^... .... .... .... .... .... .... ....
      if ( bit(Q[22],32u) != bit(Q[15],32u) ) 
        continue;
      
      // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ23,18 = 0
      sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
      if ( bit(sigma_Q23,18u) != 0u ) 
        continue;

      Q[23] = Q[22] + RL(sigma_Q23, 14u);

      // Q[23] = 0... .... .... .... .... .... .... ....
      if ( bit(Q[23],32u) != 0u ) 
        continue;

      // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20u);

      // Q[24] = 1... .... .... .... .... .... .... ....
      if ( bit(Q[24],32u) != 1u) 
        continue;

      ///////////////////////////////////////////////////////////////
      ///                       Tunnel Q20                         //
      ///////////////////////////////////////////////////////////////
      //Tunnel Q20 - 6 bits - Probabilistic. Modifications on Q[20] and free choice of Q[1] and Q[2] lead to change in x[0] and x[2..5]
      for (itr_Q20 = 0u; itr_Q20 < (USE_B1_Q20 ? pow2(Q20_strength) : 1u); itr_Q20++) {

        Q[3]  = tmp_q3;
        Q[4]  = tmp_q4;

        x[1]  = tmp_x1;
        x[15] = tmp_x15;

        //Q20 is modified according to its mask (bits 1,2,10,15,22,24)
        Q[20] = tmp_q20 ^ mask_Q20[USE_B1_Q20 ? itr_Q20 : 0u];

        x[ 0] = RR(Q[20] - Q[19],20u) - G(Q[19],Q[18],Q[17]) - Q[16] - 0xe9b6c7aau;

        Q[ 1] = QM0  + RL(F( QM0, QM1, QM2) + QM3 + x[0] + 0xd76aa478u,  7u);
        Q[ 2] = Q[1] + RL(F(Q[1], QM0, QM1) + QM2 + x[1] + 0xe8c7b756u, 12u);

        x[ 4] = RR(Q[5] - Q[4],  7u) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;
        x[ 5] = RR(Q[6] - Q[5], 12u) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62au;

        // Tunnel Q20 - Verification of bit conditions on Q[21-24]
        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5u);   

        // Q[20] = ^... .... .... ..v. .... .... .... .... 
        // Q[21] = ^... .... .... ..^. .... .... .... ....
        //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
        if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0u) 
          continue;

        // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9u);

        // Q[22] = ^... .... .... .... .... .... .... ....
        if ( bit(Q[22],32u) != bit(Q[15],32u) ) 
          continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ23,18 = 0
        sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
        if ( bit(sigma_Q23,18u) != 0u ) 
          continue;

        Q[23] = Q[22] + RL(sigma_Q23, 14u);

        // Q[23] = 0... .... .... .... .... .... .... ....
        if ( bit(Q[23],32u) != 0u) 
          continue;

        // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20u);

        // Q[24] = 1... .... .... .... .... .... .... ....
        if ( bit(Q[24],32u) != 1u)
          continue;

        ///////////////////////////////////////////////////////////////
        ///                       Tunnel Q13                         //
        ///////////////////////////////////////////////////////////////
        //Tunnel Q13 - 12 bits - Probabilistic. Modifications on Q[13] and free choice of Q[2] lead to change in x[1..5] and x[15]
        for(itr_Q13 = startQ13; itr_Q13 < endQ13; itr_Q13++ ) {
          
          Q[3]  = tmp_q3;
          Q[4]  = tmp_q4;
          Q[14] = tmp_q14;

          /* Q[13] = tmp_q13 ^ mask_Q13[USE_B1_Q13 ? itr_Q13 : 0u]; */
          Q[13] = tmp_q13 ^ (USE_B1_Q13? mask_Q(Q13_strength, Q13_mask_bits, itr_Q13) : 0u);
          
          x[ 1] = RR(Q[17] - Q[16], 5u) - G(Q[16], Q[15], Q[14]) - Q[13] - 0xf61e2562u;
          
          Q[ 2] = Q[ 1] + RL(F(Q[1 ], QM0, QM1) + QM2 + x[ 1] + 0xe8c7b756u, 12u);
          
          x[ 4] = RR(Q[ 5] - Q[ 4], 7u) - F(Q[ 4], Q[ 3], Q[ 2]) - Q[ 1] - 0xf57c0fafu;
          x[ 5] = RR(Q[ 6] - Q[ 5],12u) - F(Q[ 5], Q[ 4], Q[ 3]) - Q[ 2] - 0x4787c62au;
          x[15] = RR(Q[16] - Q[15],22u) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821u;

          // Tunnel Q13 - Verification of bit conditions on Q[21-24]
          // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5u);   

          // Q[20] = ^... .... .... ..v. .... .... .... .... 
          // Q[21] = ^... .... .... ..^. .... .... .... ....
          //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
          if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0u) 
            continue;

          // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9u);

          // Q[22] = ^... .... .... .... .... .... .... ....
          if ( bit(Q[22],32u) != bit(Q[15],32u) ) 
            continue;

          // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          // Extra conditions: Σ23,18 = 0
          sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
          if ( bit(sigma_Q23,18u) != 0u) 
            continue;

          Q[23] = Q[22] + RL(sigma_Q23, 14u);

          // Q[23] = 0... .... .... .... .... .... .... ....
          if ( bit(Q[23],32u) != 0u) 
            continue;

          // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20u);

          // Q[24] = 1... .... .... .... .... .... .... ....
          if ( bit(Q[24],32u) != 1u) 
            continue;

          ///////////////////////////////////////////////////////////////
          ///                       Tunnel Q14                         //
          ///////////////////////////////////////////////////////////////
          //Tunnel Q14 - 9 bits - Dynamic tunnel. We will find bit positions i where we can change Q[3][i] 
          //and/or Q[4][i] such that doesn't affect x[5] in the equation for Q[6]
          //In particular v = F(Q[5][i],Q[4][i],Q[5][i]) has to remain unchanged.
          //Is dynamic because, according to the value of Q[5][i] we will decide which one of the bits
          //of Q[4][i],Q[5][i] will be changed to maintain v unchanged.
          //If we change both bits Q[4][i],Q[5][i] v will change. Thus bit positions where
          //we have sufficient conditions Q[3][i] = Q[4][i] are useless (we want to change them to have MMM)

          //Bits for Q[3]
          // The ones that has to remain unchanged or are useless
          // 0x77ffffdau = 0111  0111  1111  1111  1111  1111  1101  1010 
          // The ones that can be changed
          // 0x88000025u = 1000  1000  0000  0000  0000  0000  0010  0101

          //Bits for Q[4]
          // The ones that has to remain unchanged or are useless
          // 0x8bfffff5u = 1000  1011  1111  1111  1111  1111  1111  0101
          // The ones that can be changed
          // 0x7400000au = 0111  0100  0000  0000  0000  0000  0000  1010

          //Bits for Q[14]
          // The ones that has to remain unchanged or are useless. (the not-in-mask bits)
          // 0xe3ffff88u = 1110  0011  1111  1111  1111  1111  1000  1000
          // The ones that can be changed (mask bits)
          // 0x7400000au = 0001  1100  0000  0000  0000  0000  0111  0111

          //Summarizing, bits that can be changed in Q[3]/Q[4] are the XOR
          // 0x88000025u = 1000  1000  0000  0000  0000  0000  0010  0101
          //    XOR
          // 0x7400000au = 0111  0100  0000  0000  0000  0000  0000  1010
          // -----------------------------------------------------------
          // 0xfc00002fu = 1111  1100  0000  0000  0000  0000  0010  1111
          //              \______/                              |___\__/
          //                (1)                                    (2)
          //
          //Change in bits 1,2,3,5,6,7 for Q[14] will affect bits in (1)
          //Change in bits 27,28,29 for Q[14] will affect bits in (2)

          //Unchanged bits of Q[3],Q[4],Q[14]
          Q3_fix  = Q[ 3] & 0x77ffffdau;
          Q4_fix  = Q[ 4] & 0x8bfffff5u;
          Q14_fix = Q[14] & 0xe3ffff88u;

          //Relation for Q[18], Q[7]:
          // Q[18] = Q[17]+RL(G(Q[17],Q[16],Q[15])+Q[14]+x[ 6]+0xc040b340u, 9);
          // Q[ 7] = Q[ 6]+RL(F(Q[ 6],Q[ 5],Q[ 4])+Q[ 3]+x[ 6]+0xa8304613u,17); 

          //We eliminate x[6] from our equations. We will work on const_unmasked to compensate variations on bits
          const_unmasked = RR(Q[7]-Q[6],17u) - 0xa8304613u //= F(Q[ 6],Q[ 5],Q[ 4]) + Q[ 3] + x[ 6]
                          -RR(Q[18]-Q[17],9u) + G(Q[17],Q[16],Q[15]) + 0xc040b340u //= -Q[14] -x[ 6]
                          -F(Q[6],Q[5],Q4_fix) - Q3_fix + Q14_fix;

          //So const_unmasked is the difference (F(Q[6],Q[5],Q[4]) - F(Q[6],Q[5],Q4_fix)) + (Q[3]-Q3_fix) - (Q[14]-Q14_fix)
          
          //From const_unmasked, that depends on the current value for Q[5], we'll get the new values for Q[3],Q[4] 
          //(i.e. the bits that we have to change to don't affect x[5])


          //Tunnel Q14 starts
          for(itr_Q14 = 0u; itr_Q14 < (USE_B1_Q14 ? pow2(Q14_strength) : 1u); itr_Q14++ ) {

            //Q14 is modified according to its mask {1, 2, 3, 5, 6, 7, 27, 28, 29}
            //NOTE that const_unmasked consider carries. So operations are +,- and not XOR.
            const_masked = const_unmasked + mask_Q14[USE_B1_Q14 ? itr_Q14 : 0u];
            
            //If the current value for Q[14] affects bits in const_masked that are outside 
            //0xfc00002fu = 1111  1100  0000  0000  0000  0000  0010  1111
            //this means that this modification cannot be compensated by Q[3]/Q[4] and then we need to continue
            //
            //0x03ffffd0u = 0000  0011  1111  1111  1111  1111  1101  0000
            if ((const_masked & 0x03ffffd0u) != 0u)
              continue;

            //We recover the remaining bits of Q[3],Q[4] and Q[14] from the current const_masked
            Q[ 3] = Q3_fix + (const_masked & 0x88000025u);
            Q[ 4] = Q4_fix + (const_masked & 0x7400000au);
            Q[14] = Q14_fix + mask_Q14[itr_Q14];

            x[2] = RR(Q[3] - Q[2], 17u) - F(Q[2], Q[1], QM0) - QM1 - 0x242070dbu;

            ///////////////////////////////////////////////////////////////
            ///                       Tunnel Q4                          //
            ///////////////////////////////////////////////////////////////
            //Tunnel Q4 - 1 bit - Probabilistic tunnel. Modification on Q[4][26] will probably affect Q[24][32] 
            for (itr_Q4 = startQ4; itr_Q4 < endQ4; itr_Q4++) {

              Q[4] = Q[4] ^ mask_Q4[USE_B1_Q4 ? itr_Q4 : 0u];

              x[4] = RR(Q[5] - Q[4],  7u) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;

              // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
              Q[24] = Q[23] + RL( G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20u);
                
              // Q[24] = 1... .... .... .... .... .... .... ....
              if (bit(Q[24],32u) != 1u) 
                continue;

              x[ 3] = RR(Q[ 4] - Q[ 3], 22u) - F(Q[ 3], Q[ 2], Q[ 1]) -   QM0 - 0xc1bdceeeu;
              x[ 6] = RR(Q[ 7] - Q[ 6], 17u) - F(Q[ 6], Q[ 5], Q[ 4]) - Q[ 3] - 0xa8304613u; 
              x[ 7] = RR(Q[ 8] - Q[ 7], 22u) - F(Q[ 7], Q[ 6], Q[ 5]) - Q[ 4] - 0xfd469501u;
              x[13] = RR(Q[14] - Q[13], 12u) - F(Q[13], Q[12], Q[11]) - Q[10] - 0xfd987193u; 
              x[14] = RR(Q[15] - Q[14], 17u) - F(Q[14], Q[13], Q[12]) - Q[11] - 0xa679438eu; 

     
              ///////////////////////////////////////////////////////////////
              ///                       Tunnel Q9                          //
              ///////////////////////////////////////////////////////////////
              //Tunnel Q9 - 3 bits - Deterministic tunnel. If the i-th bit of Q[10] would be zero 
              //and the i-th bit of Q[11] would be one, an eventual change of the i-th 
              //bit of Q[9] shouldn't affect the equations for Q[11] and Q[12].
              for(itr_Q9 = startQ9; itr_Q9 < endQ9; itr_Q9++ ) {

                  Q[ 9] = tmp_q9 ^ mask_Q9[USE_B1_Q9 ? itr_Q9 : 0u]; 

                  x[ 8] = RR(Q[ 9]-Q[ 8],  7u) - F(Q[ 8], Q[ 7], Q[ 6]) - Q[5] - 0x698098d8u;
                  x[ 9] = RR(Q[10]-Q[ 9], 12u) - F(Q[ 9], Q[ 8], Q[ 7]) - Q[6] - 0x8b44f7afu;    
                  x[12] = RR(Q[13]-Q[12],  7u) - F(Q[12], Q[11], Q[10]) - Q[9] - 0x6b901122u;
                 
                  Q[25] = Q[24] + RL(G(Q[24], Q[23], Q[22]) + Q[21] + x[ 9] + 0x21e1cde6u,  5u);
                  Q[26] = Q[25] + RL(G(Q[25], Q[24], Q[23]) + Q[22] + x[14] + 0xc33707d6u,  9u);            
                  Q[27] = Q[26] + RL(G(Q[26], Q[25], Q[24]) + Q[23] + x[ 3] + 0xf4d50d87u, 14u);
                  Q[28] = Q[27] + RL(G(Q[27], Q[26], Q[25]) + Q[24] + x[ 8] + 0x455a14edu, 20u);
                  Q[29] = Q[28] + RL(G(Q[28], Q[27], Q[26]) + Q[25] + x[13] + 0xa9e3e905u,  5u);
                  Q[30] = Q[29] + RL(G(Q[29], Q[28], Q[27]) + Q[26] + x[ 2] + 0xfcefa3f8u,  9u);
                  Q[31] = Q[30] + RL(G(Q[30], Q[29], Q[28]) + Q[27] + x[ 7] + 0x676f02d9u, 14u);
                  Q[32] = Q[31] + RL(G(Q[31], Q[30], Q[29]) + Q[28] + x[12] + 0x8d2a4c8au, 20u);
                  Q[33] = Q[32] + RL(H(Q[32], Q[31], Q[30]) + Q[29] + x[ 5] + 0xfffa3942u,  4u);          
                  Q[34] = Q[33] + RL(H(Q[33], Q[32], Q[31]) + Q[30] + x[ 8] + 0x8771f681u, 11u);

                  // Extra conditions: Σ35,16 = 0
                  sigma_Q35 = H(Q[34],Q[33],Q[32]) + Q[31] + x[11] + 0x6d9d6122u;
                  if (bit(sigma_Q35,16u) != 0u)
                    continue; 

                  Q[35] = Q[34] + RL(sigma_Q35, 16u);

                  Q[36] = Q[35] + RL(H(Q[35], Q[34], Q[33]) + Q[32] + x[14] + 0xfde5380cu, 23u);
                  Q[37] = Q[36] + RL(H(Q[36], Q[35], Q[34]) + Q[33] + x[ 1] + 0xa4beea44u,  4u);
                  Q[38] = Q[37] + RL(H(Q[37], Q[36], Q[35]) + Q[34] + x[ 4] + 0x4bdecfa9u, 11u);
                  Q[39] = Q[38] + RL(H(Q[38], Q[37], Q[36]) + Q[35] + x[ 7] + 0xf6bb4b60u, 16u);
                  Q[40] = Q[39] + RL(H(Q[39], Q[38], Q[37]) + Q[36] + x[10] + 0xbebfbc70u, 23u);
                  Q[41] = Q[40] + RL(H(Q[40], Q[39], Q[38]) + Q[37] + x[13] + 0x289b7ec6u,  4u);
                  Q[42] = Q[41] + RL(H(Q[41], Q[40], Q[39]) + Q[38] + x[ 0] + 0xeaa127fau, 11u);
                  Q[43] = Q[42] + RL(H(Q[42], Q[41], Q[40]) + Q[39] + x[ 3] + 0xd4ef3085u, 16u);
                  Q[44] = Q[43] + RL(H(Q[43], Q[42], Q[41]) + Q[40] + x[ 6] + 0x04881d05u, 23u);
                  Q[45] = Q[44] + RL(H(Q[44], Q[43], Q[42]) + Q[41] + x[ 9] + 0xd9d4d039u,  4u);
                  Q[46] = Q[45] + RL(H(Q[45], Q[44], Q[43]) + Q[42] + x[12] + 0xe6db99e5u, 11u);
                  Q[47] = Q[46] + RL(H(Q[46], Q[45], Q[44]) + Q[43] + x[15] + 0x1fa27cf8u, 16u);
                  Q[48] = Q[47] + RL(H(Q[47], Q[46], Q[45]) + Q[44] + x[ 2] + 0xc4ac5665u, 23u);
                                  
                  //Sufficient conditions
                  if ( bit(Q[46], 32u) != bit(Q[48], 32u) ) 
                    continue; 

                  Q[49] = Q[48] + RL(I(Q[48], Q[47], Q[46]) + Q[45] + x[ 0] + 0xf4292244u,  6u);
                              
                  if (bit(Q[47],32u) != bit(Q[49],32u)) 
                    continue;

                  Q[50] = Q[49] + RL(I(Q[49], Q[48], Q[47]) + Q[46] + x[ 7] + 0x432aff97u, 10u);
              
                  if (bit(Q[50],32u) != (bit(Q[48],32u) ^ 1u)) 
                    continue;

                  Q[51] = Q[50] + RL(I(Q[50], Q[49], Q[48]) + Q[47] + x[14] + 0xab9423a7u, 15u);
                  
                  if (bit(Q[51],32u) != bit(Q[49],32u)) 
                    continue;  
                  
                  Q[52] = Q[51] + RL(I(Q[51], Q[50], Q[49]) + Q[48] + x[ 5] + 0xfc93a039u, 21u);
                      
                  if (bit(Q[52],32u) != bit(Q[50],32u)) 
                    continue; 

                  Q[53] = Q[52] + RL(I(Q[52], Q[51], Q[50]) + Q[49] + x[12] + 0x655b59c3u, 6u); 
                                
                  if (bit(Q[53],32u) != bit(Q[51],32u)) 
                    continue; 

                  Q[54] = Q[53] + RL(I(Q[53], Q[52], Q[51]) + Q[50] + x[ 3] + 0x8f0ccc92u, 10u);    
                  
                  if (bit(Q[54],32u) != bit(Q[52],32u)) 
                    continue; 

                  Q[55] = Q[54] + RL(I(Q[54], Q[53], Q[52]) + Q[51] + x[10] + 0xffeff47du, 15u);   
                  
                  if (bit(Q[55],32u) != bit(Q[53],32u)) 
                    continue; 

                  Q[56] = Q[55] + RL(I(Q[55], Q[54], Q[53]) + Q[52] + x[ 1] + 0x85845dd1u, 21u);    
                  
                  if (bit(Q[56],32u) != bit(Q[54],32u)) 
                    continue; 

                  Q[57] = Q[56] + RL(I(Q[56], Q[55], Q[54]) + Q[53] + x[ 8] + 0x6fa87e4fu, 6u);   
                  
                  if (bit(Q[57],32u) != bit(Q[55],32u)) 
                    continue; 

                  Q[58] = Q[57] + RL(I(Q[57], Q[56], Q[55]) + Q[54] + x[15] + 0xfe2ce6e0u, 10u);   
                  
                  if (bit(Q[58],32u) != bit(Q[56],32u)) 
                    continue; 

                  Q[59] = Q[58] + RL(I(Q[58], Q[57], Q[56]) + Q[55] + x[ 6] + 0xa3014314u, 15u);    
                  
                  if (bit(Q[59],32u) != bit(Q[57],32u)) 
                    continue; 

                  Q[60] = Q[59] + RL(I(Q[59], Q[58], Q[57]) + Q[56] + x[13] + 0x4e0811a1u, 21u);   
                  
                  if (bit(Q[60],26u) != 0u) 
                    continue; 

                  if (bit(Q[60],32u) != (bit(Q[58],32u) ^ 1u)) 
                    continue; 

                  Q[61] = Q[60] + RL(I(Q[60], Q[59], Q[58]) + Q[57] + x[ 4] + 0xf7537e82u,  6u);   
                  
                  if (bit(Q[61],26u) != 1u) 
                    continue; 

                  if (bit(Q[61],32u) != bit(Q[59],32u)) 
                    continue; 

                  //Extra conditions: Σ62,16 ~ Σ62,22 not all ones
                  //0x003f8000u = 0000  0000  0011  1111  1000  0000  0000  0000
                  sigma_Q62 = I(Q[61],Q[60],Q[59]) + Q[58] + x[11] + 0xbd3af235u;
                  if ( (sigma_Q62 & 0x003f8000u) == 0x003f8000u )
                    continue;

                  Q[62] = Q[61] + RL(sigma_Q62 , 10u); 

                  Q[63] = Q[62] + RL(I(Q[62], Q[61], Q[60]) + Q[59] + x[2] + 0x2ad7d2bbu, 15u);    
                  Q[64] = Q[63] + RL(I(Q[63], Q[62], Q[61]) + Q[60] + x[9] + 0xeb86d391u, 21u);    
                    
                  //We add the initial vector to obtain the Intermediate Hash Values of the current block
                  AA0 = IV1 + Q[61];  BB0 = IV2 + Q[64];
                  CC0 = IV3 + Q[63];  DD0 = IV4 + Q[62];
                  
                  //Last sufficient conditions  
                  if (bit(BB0,6u) != 0u) 
                    continue;

                  if (bit(BB0,26u) != 0u) 
                    continue;

                  if (bit(BB0,27u) != 0u) 
                    continue;

                  if (bit(CC0,26u) != 1u)
                    continue;

                  if (bit(CC0,27u) != 0u) 
                    continue;  

                  if (bit(DD0,26u) != 0u) 
                    continue;

                  if (bit(BB0,32u) != bit(CC0,32u)) 
                    continue;

                  if (bit(CC0,32u) != bit(DD0,32u)) 
                    continue;

                  //Message 1 block 1 computation completed. 
                  
                  //Now we see if the differential path is verified
                  //Note that message 1 block 1 is x = x[0]||...||x[15]
                  //While message 2 block 1 is Hx = x + C

                  //Message 2 block 1 hash computation
                  for(i = 0u; i < 16u; i++) 
                    Hx[i] = x[i];

                  Hx[ 4] = x[ 4] + 0x80000000u;
                  Hx[11] = x[11] + 0x00008000u;
                  Hx[14] = x[14] + 0x80000000u;

                  //We set the IV, hash Hx and get the Intermediate Hash Value
                  a = IV1;  b = IV2;
                  c = IV3;  d = IV4; 

                  HMD5Tr();
                  
                  AA1 = IV1 + a;  BB1 = IV2 + b;
                  CC1 = IV3 + c;  DD1 = IV4 + d;
                  
                  //We see if the Differential Path is verified,
                  if ( ((AA1-AA0) != 0x80000000u) || 
                       ((BB1-BB0) != 0x82000000u) || 
                       ((CC1-CC0) != 0x82000000u) || 
                       ((DD1-DD0) != 0x82000000u)  )
                    continue;
                  
                  //We store the intermediate hash values
                  A0=AA0; B0=BB0; C0=CC0; D0=DD0;
                  A1=AA1; B1=BB1; C1=CC1; D1=DD1;

                  //We store both first blocks
                  /* for (i=0u; i<16u; i++) { */
                  /*   memcpy( &v1[4*i], &x[i],  4); */ 
                  /*   memcpy( &v2[4*i], &Hx[i], 4); */
                  /* } */
                  tunnel9 = itr_Q9;
                  tunnel4 = itr_Q4;
                  tunnel14 = itr_Q14;
                  tunnel13 = itr_Q13;
                  tunnel20 = itr_Q20;
                  tunnel10 = itr_Q10;
                  return it;

                } //End of Q9 Tunnel
              } //End of Q4 Tunnel
            } //End of Q14 Tunnel
          } //End of Q13 Tunnel
        } //End of Q20 Tunnel
      } //End of Q10 Tunnel
    } //End of general for
  return(-1); //Collision not found;
  /* return 0; */
}


int Block2() {

  /* u32 Q[65]; */
  u32 x[16];
  u32 QM0, QM1, QM2, QM3;
  u32 i, itr_q16, itr_q1q2, itr_q9, itr_q4, tmp_q1, tmp_q2, tmp_q4, tmp_q9;
  u32 I, not_I;
  u32 sigma_Q17, sigma_Q19, sigma_Q20, sigma_Q23, sigma_Q35, sigma_Q62;
  u32 Q1_fix, Q2_fix, mask_Q1Q2, Q1Q2_strength;
  u32 AA0, BB0, CC0, DD0, AA1, BB1, CC1, DD1;

  QM3 = A0;
  QM0 = B0;
  QM1 = C0;
  QM2 = D0;

  // Mask generation for tunnel Q9 - 8 bits
  int Q9_mask_bits[] = int[]( 3, 4, 5, 11, 19, 21, 22, 23 );
  int Q9_strength = 8;
  /* const u32 *mask_Q9 = generate_mask(Q9_strength, Q9_mask_bits); */
  const u32 mask_Q9[] = uint[](0u,4u,8u,12u,16u,20u,24u,28u,1024u,1028u,1032u,1036u,1040u,1044u,1048u,1052u,262144u,262148u,262152u,262156u,262160u,262164u,262168u,262172u,263168u,263172u,263176u,263180u,263184u,263188u,263192u,263196u,1048576u,1048580u,1048584u,1048588u,1048592u,1048596u,1048600u,1048604u,1049600u,1049604u,1049608u,1049612u,1049616u,1049620u,1049624u,1049628u,1310720u,1310724u,1310728u,1310732u,1310736u,1310740u,1310744u,1310748u,1311744u,1311748u,1311752u,1311756u,1311760u,1311764u,1311768u,1311772u,2097152u,2097156u,2097160u,2097164u,2097168u,2097172u,2097176u,2097180u,2098176u,2098180u,2098184u,2098188u,2098192u,2098196u,2098200u,2098204u,2359296u,2359300u,2359304u,2359308u,2359312u,2359316u,2359320u,2359324u,2360320u,2360324u,2360328u,2360332u,2360336u,2360340u,2360344u,2360348u,3145728u,3145732u,3145736u,3145740u,3145744u,3145748u,3145752u,3145756u,3146752u,3146756u,3146760u,3146764u,3146768u,3146772u,3146776u,3146780u,3407872u,3407876u,3407880u,3407884u,3407888u,3407892u,3407896u,3407900u,3408896u,3408900u,3408904u,3408908u,3408912u,3408916u,3408920u,3408924u,4194304u,4194308u,4194312u,4194316u,4194320u,4194324u,4194328u,4194332u,4195328u,4195332u,4195336u,4195340u,4195344u,4195348u,4195352u,4195356u,4456448u,4456452u,4456456u,4456460u,4456464u,4456468u,4456472u,4456476u,4457472u,4457476u,4457480u,4457484u,4457488u,4457492u,4457496u,4457500u,5242880u,5242884u,5242888u,5242892u,5242896u,5242900u,5242904u,5242908u,5243904u,5243908u,5243912u,5243916u,5243920u,5243924u,5243928u,5243932u,5505024u,5505028u,5505032u,5505036u,5505040u,5505044u,5505048u,5505052u,5506048u,5506052u,5506056u,5506060u,5506064u,5506068u,5506072u,5506076u,6291456u,6291460u,6291464u,6291468u,6291472u,6291476u,6291480u,6291484u,6292480u,6292484u,6292488u,6292492u,6292496u,6292500u,6292504u,6292508u,6553600u,6553604u,6553608u,6553612u,6553616u,6553620u,6553624u,6553628u,6554624u,6554628u,6554632u,6554636u,6554640u,6554644u,6554648u,6554652u,7340032u,7340036u,7340040u,7340044u,7340048u,7340052u,7340056u,7340060u,7341056u,7341060u,7341064u,7341068u,7341072u,7341076u,7341080u,7341084u,7602176u,7602180u,7602184u,7602188u,7602192u,7602196u,7602200u,7602204u,7603200u,7603204u,7603208u,7603212u,7603216u,7603220u,7603224u,7603228u);

  // Mask generation for MMMM Q4 - 6 bits
  int Q4_mask_bits[] = int[]( 14, 15, 16, 23, 24, 25 );
  int Q4_strength = 6;
  /* const u32 *mask_Q4 = generate_mask(Q4_strength, Q4_mask_bits); */
  const u32 mask_Q4[] = uint[](0u,8192u,16384u,24576u,32768u,40960u,49152u,57344u,4194304u,4202496u,4210688u,4218880u,4227072u,4235264u,4243456u,4251648u,8388608u,8396800u,8404992u,8413184u,8421376u,8429568u,8437760u,8445952u,12582912u,12591104u,12599296u,12607488u,12615680u,12623872u,12632064u,12640256u,16777216u,16785408u,16793600u,16801792u,16809984u,16818176u,16826368u,16834560u,20971520u,20979712u,20987904u,20996096u,21004288u,21012480u,21020672u,21028864u,25165824u,25174016u,25182208u,25190400u,25198592u,25206784u,25214976u,25223168u,29360128u,29368320u,29376512u,29384704u,29392896u,29401088u,29409280u,29417472u);

  // We extract the 32th bit of B0 and its
  I = QM0 & 0x80000000u;
  not_I = (~QM0) & 0x80000000u;

  // Start block 2 generation.
  // TO-DO: add a time limit for collision search.
  for (int it = 0; it < 2; it++) {

    // Q[ 1] = ~Ivvv  010v  vv1v  vvv1  .vvv  0vvv  vv0.  ...v
    // RNG   =  .***  ...*  **.*  ***.  ****  .***  **.*  ****  0x71def7dfu
    // 0     =  ....  *.*.  ....  ....  ....  *...  ..*.  ....  0x0a000820u
    // 1     =  ....  .*..  ..*.  ...*  ....  ....  ....  ....  0x04210000u
    Q[1] = (rng() & 0x71def7dfu) + 0x04210000u + not_I;

    // Multi message modif. meth. (MMMM) Q1Q2, Klima
    // Q[ 2] = ~I^^^  110^  ^^0^  ^^^1  0^^^  1^^^  ^^0v  v00^
    // RNG   =  ....  ....  ....  ....  ....  ....  ...*  *...  0x00000018u
    // 0     =  ....  ..*.  ..*.  ....  *...  ....  ..*.  .**.  0x02208026u
    // 1     =  ....  **..  ....  ...*  ....  *...  ....  ....  0x0c010800u
    // Q[ 1] =  .***  ...*  **.*  ***.  .***  .***  **..  ...*  0x71de77c1u
    Q[2] = (rng() & 0x00000018u) + 0x0c010800u + (Q[1] & 0x71de77c1u) + not_I;

    // Q[ 3] = ~I011  111.  ..01  1111  1..0  1vv1  011^  ^111
    // RNG   =  ....  ...*  **..  ....  .**.  .**.  ....  ....  0x01c06600u
    // 0     =  .*..  ....  ..*.  ....  ...*  ....  *...  ....  0x40201080u
    // 1     =  ..**  ***.  ...*  ****  *...  *..*  .**.  .***  0x3e1f8967u
    // Q[ 2] =  ....  ....  ....  ....  ....  ....  ...*  *...  0x00000018u
    Q[3] = (rng() & 0x01c06600u) + 0x3e1f8967u + (Q[2] & 0x00000018u) + not_I;

    // Q[ 4] = ~I011  101.  ..00  0100  ...0  0^^0  0001  0001
    // RNG   =  ....  ...*  **..  ....  ***.  ....  ....  ....  0x01c0e000u
    // 0     =  .*..  .*..  ..**  *.**  ...*  *..*  ***.  ***.  0x443b19eeu
    // 1     =  ..**  *.*.  ....  .*..  ....  ....  ...*  ...*  0x3a040011u
    // Q[ 3] =  ....  ....  ....  ....  ....  .**.  ....  ....  0x00000600u
    Q[4] = (rng() & 0x01c0e000u) + 0x3a040011u + (Q[3] & 0x00000600u) + not_I;

    // Q4 tunnel, Klima, bits 25-23,16-14
    // Q[ 5] =  I100  10.0  0010  1111  0000  1110  0101  0000
    // RNG   =  ....  ..*.  ....  ....  ....  ....  ....  ....  0x02000000u
    // 0     =  ..**  .*.*  **.*  ....  ****  ...*  *.*.  ****  0x35d0f1afu
    // 1     =  .*..  *...  ..*.  ****  ....  ***.  .*.*  ....  0x482f0e50u
    Q[5] = (rng() & 0x02000000u) + 0x482f0e50u + I;

    // Q4 tunnel, Klima, bits 25-23,16-14
    // Q[ 6] =  I..0  0101  1110  ..10  1110  1100  0101  0110
    // RNG   =  .**.  ....  ....  **..  ....  ....  ....  ....  0x600c0000u
    // 0     =  ...*  *.*.  ...*  ...*  ...*  ..**  *.*.  *..*  0x1a1113a9u
    // 1     =  ....  .*.*  ***.  ..*.  ***.  **..  .*.*  .**.  0x05e2ec56u
    Q[6] = (rng() & 0x600c0000u) + 0x05e2ec56u + I;

    // Q[ 7] = ~I..1  0111  1.00  ..01  10.1  1110  00..  ..v1
    // RNG   =  .**.  ....  .*..  **..  ..*.  ....  ..**  ***.  0x604c203eu
    // 0     =  ....  *...  ..**  ..*.  .*..  ...*  **..  ....  0x083241c0u
    // 1     =  ...*  .***  *...  ...*  *..*  ***.  ....  ...*  0x17819e01u
    Q[7] = (rng() & 0x604c203eu) + 0x17819e01u + not_I;

    // Q[ 8] = ~I..0  0100  0.11  ..10  1..v  ..11  111.  ..^0
    // RNG   =  .**.  ....  .*..  **..  .***  **..  ...*  **..  0x604c7c1cu
    // 0     =  ...*  *.**  *...  ...*  ....  ....  ....  ...*  0x1b810001u
    // 1     =  ....  .*..  ..**  ..*.  *...  ..**  ***.  ....  0x043283e0u
    // Q[ 7] =  ....  ....  ....  ....  ....  ....  ....  ..*.  0x00000002u
    Q[8] = (rng() & 0x604c7c1cu) + 0x043283e0u + (Q[7] & 0x00000002u) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima, prepared, not programmed
    // Q[ 9] = ~Ivv1  1100  0xxx  .x01  0..^  .x01  110x  xx01
    // RNG   =  .**.  ....  .***  **..  .**.  **..  ...*  **..  0x607c6c1cu
    // 0     =  ....  ..**  *...  ..*.  *...  ..*.  ..*.  ..*.  0x03828222u
    // 1     =  ...*  **..  ....  ...*  ....  ...*  **..  ...*  0x1c0101c1u
    // Q[ 8] =  ....  ....  ....  ....  ...*  ....  ....  ....  0x00001000u
    Q[9] = (rng() & 0x607c6c1cu) + 0x1c0101c1u + (Q[8] & 0x00001000u) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima
    // Q[10] = ~I^^1  1111  1000  v011  1vv0  1011  1100  0000
    // RNG   =  ....  ....  ....  *...  .**.  ....  ....  ....  0x00086000u
    // 0     =  ....  ....  .***  .*..  ...*  .*..  ..**  ****  0x0074143fu
    // 1     =  ...*  ****  *...  ..**  *...  *.**  **..  ....  0x1f838bc0u
    // Q[ 9] =  .**.  ....  ....  ....  ....  ....  ....  ....  0x60000000u
    Q[10] = (rng() & 0x00086000u) + 0x1f838bc0u + (Q[9] & 0x60000000u) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima
    // Q[11] = ~Ivvv  vvvv  .111  ^101  1^^0  0111  11v1  1111
    // RNG   =  .***  ****  *...  ....  ....  ....  ..*.  ....  0x7f800020u
    // 0     =  ....  ....  ....  ..*.  ...*  *...  ....  ....  0x00021800u
    // 1     =  ....  ....  .***  .*.*  *...  .***  **.*  ****  0x007587dfu
    // Q[10] =  ....  ....  ....  *...  .**.  ....  ....  ....  0x00086000u
    Q[11] = (rng() & 0x7f800020u) + 0x007587dfu + (Q[10] & 0x00086000u) + not_I;

    // MMMM-Q12Q11, Klima
    // Q[12] = ~I^^^  ^^^^  ....  1000  0001  ....  1.^.  ....
    // RNG   =  ....  ....  ****  ....  ....  ****  .*.*  ****  0x00f00f5fu
    // 0     =  ....  ....  ....  .***  ***.  ....  ....  ....  0x0007e000u
    // 1     =  ....  ....  ....  *...  ...*  ....  *...  ....  0x00081080u
    // Q[11] =  .***  ****  ....  ....  ....  ....  ..*.  ....  0x7f000020u
    Q[12] = (rng() & 0x00f00f5fu) + 0x00081080u + (Q[11] & 0x7f000020u) + not_I;

    // Q[13] =  I011  1111  0...  1111  111.  ....  0...  1...
    // RNG   =  ....  ....  .***  ....  ...*  ****  .***  .***  0x00701f77u
    // 0     =  .*..  ....  *...  ....  ....  ....  *...  ....  0x40800080u
    // 1     =  ..**  ****  ....  ****  ***.  ....  ....  *...  0x3f0fe008u
    Q[13] = (rng() & 0x00701f77u) + 0x3f0fe008u + I;

    // Q[14] =  I100  0000  1...  1011  111.  ....  1...  1...
    // RNG   =  ....  ....  .***  ....  ...*  ****  .***  .***  0x00701f77u
    // 0     =  ..**  ****  ....  .*..  ....  ....  ....  ....  0x3f040000u
    // 1     =  .*..  ....  *...  *.**  ***.  ....  *...  *...  0x408be088u
    Q[14] = (rng() & 0x00701f77u) + 0x408be088u + I;

    // Next sufficient conditions until Q[24]:
    // Q[15] =  0111  1101  ....  ..10  00..  ....  ....  0...
    // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
    // Q[17] =  ^.v.  ....  ....  ..0.  1...  ....  ....  1...
    // Q[18] =  ^.^.  ....  ....  ..1.  ....  ....  ....  ....
    // Q[19] =  ^...  ....  ....  ..0.  ....  ....  ....  ....
    // Q[20] =  ^...  ....  ....  ..v.  ....  ....  ....  ....
    // Q[21] =  ^...  ....  ....  ..^.  ....  ....  ....  ....
    // Q[22] =  ^...  ....  ....  ....  ....  ....  ....  ....
    // Q[23] =  0...  ....  ....  ....  ....  ....  ....  ....
    // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....

    // In MMMM-Q[1]/Q[2] we want to change the value of x[0] without updating
    // the value of x[1], as updating x[1] will cause conditions on Q[17] not
    // hold. According to F, if QM0[i] = QM1[i] randomly choose the value where
    // Q[1][i] = Q[2][i] will not change the value of F(Q[1],QM0,QM1). We select
    // these bits
    //  Q[ 1] = ~Ivvv  010v  vv1v  vvv1  .vvv  0vvv  vv0.  ...v
    //  Q[ 2] = ~I^^^  110^  ^^0^  ^^^1  0^^^  1^^^  ^^0v  v00^
    //           0111  0001  1101  1110  0111  0111  1100  0001 = 0x71de77c1u
    //
    // Note that (~(QM0 ^ QM1)) are all the bits where QM0[i] = QM1[i] and so
    // mask_Q1Q2 are all the bits where QM0[i] = QM1[i] and where Q[1][i] =
    // Q[2][i]. These bits will be changed.
    mask_Q1Q2 = (~(QM0 ^ QM1)) & 0x71de77c1u;
    Q1Q2_strength = 0;
    for (i = 1; i < 33; i++)
      Q1Q2_strength += bit(mask_Q1Q2, i);

    Q1_fix = Q[1] & ~mask_Q1Q2;
    Q2_fix = Q[2] & ~mask_Q1Q2;

    tmp_q1 = Q[1];
    tmp_q2 = Q[2];
    tmp_q4 = Q[4];
    tmp_q9 = Q[9];

    ///////////////////////////////////////////////////////////////
    ///                        MMMM Q16                          //
    ///////////////////////////////////////////////////////////////
    // MMMM Q16 - 25 bits
    for (itr_q16 = 0; itr_q16 < pow(2, 25); itr_q16++) {

      Q[1] = tmp_q1;
      Q[2] = tmp_q2;
      Q[4] = tmp_q4;
      Q[9] = tmp_q9;

      // Conditions by Liang-Lai says: Q[15] = (rng() & 0x80fc3ff7u) +
      // 0x7d020000u, Q[15] =  0111  1101  ....  ..10  00..  ....  ....  0... RNG
      // =  ....  ....  ****  **..  ..**  ****  ****  .***  0x80fc3ff7u 0     =
      // *...  ..*.  ....  ...*  **..  ....  ....  *...  0x0201c008u 1     = .***
      // **.*  ....  ..*.  ....  ....  ....  ....  0x7d020000u
      Q[15] = (rng() & 0x00fc3ff7u) + 0x7d020000u;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // RNG   =  .*..  ****  ****  **..  .***  ****  ****  .***  0x4ffc7ff7u
      // 0     =  ...*  ....  ....  ..*.  ....  ....  ....  ....  0x10020000u
      // 1     =  ..*.  ....  ....  ...*  *...  ....  ....  *...  0x20018008u
      // Q[15] =  *.... ..... ..... .... ..... ..... ...... ....  0x80000000u
      Q[16] = (rng() & 0x4ffc7ff7u) + 0x20018008u + (Q[15] & 0x80000000u);

      x[1] = RR(Q[2] - Q[1], 12) - F(Q[1], QM0, QM1) - QM2 - 0xe8c7b756u;
      x[6] = RR(Q[7] - Q[6], 17) - F(Q[6], Q[5], Q[4]) - Q[3] - 0xa8304613u;
      x[11] = RR(Q[12] - Q[11], 22) - F(Q[11], Q[10], Q[9]) - Q[8] - 0x895cd7beu;

      // Q[17] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ17,25 ~ Σ17,27 not all 1
      // 0x07000000u =  0000 0111 0000 0000 0000 0000 0000 0000
      sigma_Q17 = G(Q[16], Q[15], Q[14]) + Q[13] + x[1] + 0xf61e2562u;
      if ((sigma_Q17 & 0x07000000u) == 0x07000000u)
        continue;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // Q[17] =  ^.v.  ....  ....  ..0.  1...  ....  ....  1...
      //          1000  0000  0000  0010  1000  0000  0000  1000 0x80028008u
      Q[17] = Q[16] + RL(sigma_Q17, 5);

      if ((Q[17] & 0x80028008u) != (Q[16] & 0x80028008u))
        continue;

      // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[18] = Q[17] + RL(G(Q[17], Q[16], Q[15]) + Q[14] + x[6] + 0xc040b340u, 9);

      // Q[18] =  ^.^.  ....  ....  ..1.  ....  ....  ....  ....
      if (bit(Q[18], 18) != 1)
        continue;

      if ((Q[18] & 0xa0000000u) != (Q[17] & 0xa0000000u))
        continue;

      // Q[19] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ19,4 ~ Σ19,18 not all 1
      // 0x0003fff8u =  0000 0000 0000 0011 1111 1111 1111 1000
      sigma_Q19 = G(Q[18], Q[17], Q[16]) + Q[15] + x[11] + 0x265e5a51u;
      if ((sigma_Q19 & 0x0003fff8u) == 0x0003fff8u)
        continue;

      Q[19] = Q[18] + RL(sigma_Q19, 14);

      // Q[19] =  ^...  ....  ....  ..0.  ....  ....  ....  ....
      if (bit(Q[19], 18) != 0)
        continue;

      if (bit(Q[19], 32) != bit(Q[18], 32))
        continue;

      x[10] = RR(Q[11] - Q[10], 17) - F(Q[10], Q[9], Q[8]) - Q[7] - 0xffff5bb1u;
      x[15] =
          RR(Q[16] - Q[15], 22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821u;

      ///////////////////////////////////////////////////////////////
      ///                      MMMM Q1/Q2                          //
      ///////////////////////////////////////////////////////////////
      // MMMM Q1/Q2 - variable bits
      for (itr_q1q2 = 0; itr_q1q2 < pow(2, Q1Q2_strength); itr_q1q2++) {

        Q[4] = tmp_q4;
        Q[9] = tmp_q9;

        // We randomly change the mask bits where QM0[i] = QM1[i] and where
        // Q[1][i] = Q[2][i]
        Q[1] = (rng() & mask_Q1Q2) + Q1_fix;
        Q[2] = (Q[1] & mask_Q1Q2) + Q2_fix;

        x[0] = RR(Q[1] - QM0, 7) - F(QM0, QM1, QM2) - QM3 - 0xd76aa478u;

        // Q[20] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ20,30 ~ Σ20,32 not all 0
        // 0xe0000000u =  1110 0000 0000 0000 0000 0000 0000 0000
        sigma_Q20 = G(Q[19], Q[18], Q[17]) + Q[16] + x[0] + 0xe9b6c7aau;
        if ((sigma_Q20 & 0xe0000000u) == 0)
          continue;

        Q[20] = Q[19] + RL(sigma_Q20, 20);

        // Q[20] =  ^...  ....  ....  ..v.  ....  ....  ....  ....
        if (bit(Q[20], 32) != bit(Q[19], 32))
          continue;

        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        x[5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62au;

        Q[21] =
            Q[20] + RL(G(Q[20], Q[19], Q[18]) + Q[17] + x[5] + 0xd62f105du, 5);

        // Q[21] =  ^...  ....  ....  ..^.  ....  ....  ....  ....
        //          1000  0000  0000  0010  0000  0000  0000  0000 = 0x80020000u
        if ((Q[21] & 0x80020000u) != (Q[20] & 0x80020000u))
          continue;

        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[22] =
            Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453u, 9);

        // Q[22] =  ^...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[22], 32) != bit(Q[21], 32))
          continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ23,18 = 0
        sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
        if (bit(sigma_Q23, 18) != 0)
          continue;

        Q[23] = Q[22] + RL(sigma_Q23, 14);

        // Q[23] =  0...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[23], 32) != 0)
          continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;

        Q[24] =
            Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

        // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[24], 32) != 1)
          continue;

        x[2] = RR(Q[3] - Q[2], 17) - F(Q[2], Q[1], QM0) - QM1 - 0x242070dbu;
        x[13] =
            RR(Q[14] - Q[13], 12) - F(Q[13], Q[12], Q[11]) - Q[10] - 0xfd987193u;
        x[14] =
            RR(Q[15] - Q[14], 17) - F(Q[14], Q[13], Q[12]) - Q[11] - 0xa679438eu;

        ///////////////////////////////////////////////////////////////
        ///                         MMMM Q4                          //
        ///////////////////////////////////////////////////////////////
        // MMMM Q4 - 6 bits
        for (itr_q4 = 0; itr_q4 < pow(2, 6); itr_q4++) {

          Q[4] = tmp_q4 ^ mask_Q4[itr_q4];

          x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;

          Q[24] = Q[23] +
                  RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

          // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....
          if (bit(Q[24], 32) != 1)
            continue;

          x[3] = RR(Q[4] - Q[3], 22) - F(Q[3], Q[2], Q[1]) - QM0 - 0xc1bdceeeu;
          x[7] = RR(Q[8] - Q[7], 22) - F(Q[7], Q[6], Q[5]) - Q[4] - 0xfd469501u;

          ///////////////////////////////////////////////////////////////
          ///                       Tunnel Q9                          //
          ///////////////////////////////////////////////////////////////
          // Tunnel Q9 - 8 bits
          for (itr_q9 = 0; itr_q9 < (USE_B2_Q9 ? pow(2, Q9_strength) : 1);
               itr_q9++) {

            Q[9] = tmp_q9 ^ mask_Q9[USE_B2_Q9 ? itr_q9 : 0];

            x[8] = RR(Q[9] - Q[8], 7) - F(Q[8], Q[7], Q[6]) - Q[5] - 0x698098d8u;
            x[9] =
                RR(Q[10] - Q[9], 12) - F(Q[9], Q[8], Q[7]) - Q[6] - 0x8b44f7afu;
            x[12] = RR(Q[13] - Q[12], 7) - F(Q[12], Q[11], Q[10]) - Q[9] -
                    0x6b901122u;

            Q[25] = Q[24] +
                    RL(G(Q[24], Q[23], Q[22]) + Q[21] + x[9] + 0x21e1cde6u, 5);
            Q[26] = Q[25] +
                    RL(G(Q[25], Q[24], Q[23]) + Q[22] + x[14] + 0xc33707d6u, 9);
            Q[27] = Q[26] +
                    RL(G(Q[26], Q[25], Q[24]) + Q[23] + x[3] + 0xf4d50d87u, 14);
            Q[28] = Q[27] +
                    RL(G(Q[27], Q[26], Q[25]) + Q[24] + x[8] + 0x455a14edu, 20);
            Q[29] = Q[28] +
                    RL(G(Q[28], Q[27], Q[26]) + Q[25] + x[13] + 0xa9e3e905u, 5);
            Q[30] = Q[29] +
                    RL(G(Q[29], Q[28], Q[27]) + Q[26] + x[2] + 0xfcefa3f8u, 9);
            Q[31] = Q[30] +
                    RL(G(Q[30], Q[29], Q[28]) + Q[27] + x[7] + 0x676f02d9u, 14);
            Q[32] = Q[31] +
                    RL(G(Q[31], Q[30], Q[29]) + Q[28] + x[12] + 0x8d2a4c8au, 20);
            Q[33] = Q[32] +
                    RL(H(Q[32], Q[31], Q[30]) + Q[29] + x[5] + 0xfffa3942u, 4);
            Q[34] = Q[33] +
                    RL(H(Q[33], Q[32], Q[31]) + Q[30] + x[8] + 0x8771f681u, 11);

            // Extra conditions: Σ35,16 = 1
            sigma_Q35 = H(Q[34], Q[33], Q[32]) + Q[31] + x[11] + 0x6d9d6122u;
            if (bit(sigma_Q35, 16) != 1)
              continue;

            Q[35] = Q[34] + RL(sigma_Q35, 16);

            Q[36] = Q[35] +
                    RL(H(Q[35], Q[34], Q[33]) + Q[32] + x[14] + 0xfde5380cu, 23);
            Q[37] = Q[36] +
                    RL(H(Q[36], Q[35], Q[34]) + Q[33] + x[1] + 0xa4beea44u, 4);
            Q[38] = Q[37] +
                    RL(H(Q[37], Q[36], Q[35]) + Q[34] + x[4] + 0x4bdecfa9u, 11);
            Q[39] = Q[38] +
                    RL(H(Q[38], Q[37], Q[36]) + Q[35] + x[7] + 0xf6bb4b60u, 16);
            Q[40] = Q[39] +
                    RL(H(Q[39], Q[38], Q[37]) + Q[36] + x[10] + 0xbebfbc70u, 23);
            Q[41] = Q[40] +
                    RL(H(Q[40], Q[39], Q[38]) + Q[37] + x[13] + 0x289b7ec6u, 4);
            Q[42] = Q[41] +
                    RL(H(Q[41], Q[40], Q[39]) + Q[38] + x[0] + 0xeaa127fau, 11);
            Q[43] = Q[42] +
                    RL(H(Q[42], Q[41], Q[40]) + Q[39] + x[3] + 0xd4ef3085u, 16);
            Q[44] = Q[43] +
                    RL(H(Q[43], Q[42], Q[41]) + Q[40] + x[6] + 0x04881d05u, 23);
            Q[45] = Q[44] +
                    RL(H(Q[44], Q[43], Q[42]) + Q[41] + x[9] + 0xd9d4d039u, 4);
            Q[46] = Q[45] +
                    RL(H(Q[45], Q[44], Q[43]) + Q[42] + x[12] + 0xe6db99e5u, 11);
            Q[47] = Q[46] +
                    RL(H(Q[46], Q[45], Q[44]) + Q[43] + x[15] + 0x1fa27cf8u, 16);
            Q[48] = Q[47] +
                    RL(H(Q[47], Q[46], Q[45]) + Q[44] + x[2] + 0xc4ac5665u, 23);

            // Last sufficient conditions
            if (bit(Q[48], 32) != bit(Q[46], 32))
              continue;

            Q[49] = Q[48] +
                    RL(I(Q[48], Q[47], Q[46]) + Q[45] + x[0] + 0xf4292244u, 6);

            if (bit(Q[49], 32) != bit(Q[47], 32))
              continue;

            Q[50] = Q[49] +
                    RL(I(Q[49], Q[48], Q[47]) + Q[46] + x[7] + 0x432aff97u, 10);

            if (bit(Q[50], 32) != (bit(Q[48], 32) ^ 1))
              continue;

            Q[51] = Q[50] +
                    RL(I(Q[50], Q[49], Q[48]) + Q[47] + x[14] + 0xab9423a7u, 15);

            if (bit(Q[51], 32) != bit(Q[49], 32))
              continue;

            Q[52] = Q[51] +
                    RL(I(Q[51], Q[50], Q[49]) + Q[48] + x[5] + 0xfc93a039u, 21);

            if (bit(Q[52], 32) != bit(Q[50], 32))
              continue;

            Q[53] = Q[52] +
                    RL(I(Q[52], Q[51], Q[50]) + Q[49] + x[12] + 0x655b59c3u, 6);

            if (bit(Q[53], 32) != bit(Q[51], 32))
              continue;

            Q[54] = Q[53] +
                    RL(I(Q[53], Q[52], Q[51]) + Q[50] + x[3] + 0x8f0ccc92u, 10);

            if (bit(Q[54], 32) != bit(Q[52], 32))
              continue;

            Q[55] = Q[54] +
                    RL(I(Q[54], Q[53], Q[52]) + Q[51] + x[10] + 0xffeff47du, 15);

            if (bit(Q[55], 32) != bit(Q[53], 32))
              continue;

            Q[56] = Q[55] +
                    RL(I(Q[55], Q[54], Q[53]) + Q[52] + x[1] + 0x85845dd1u, 21);

            if (bit(Q[56], 32) != bit(Q[54], 32))
              continue;

            Q[57] = Q[56] +
                    RL(I(Q[56], Q[55], Q[54]) + Q[53] + x[8] + 0x6fa87e4fu, 6);

            if (bit(Q[57], 32) != bit(Q[55], 32))
              continue;

            Q[58] = Q[57] +
                    RL(I(Q[57], Q[56], Q[55]) + Q[54] + x[15] + 0xfe2ce6e0u, 10);

            if (bit(Q[58], 32) != bit(Q[56], 32))
              continue;

            Q[59] = Q[58] +
                    RL(I(Q[58], Q[57], Q[56]) + Q[55] + x[6] + 0xa3014314u, 15);

            if (bit(Q[59], 32) != bit(Q[57], 32))
              continue;

            Q[60] = Q[59] +
                    RL(I(Q[59], Q[58], Q[57]) + Q[56] + x[13] + 0x4e0811a1u, 21);

            if (bit(Q[60], 26) != 0)
              continue;

            if (bit(Q[60], 32) != (bit(Q[58], 32) ^ 1))
              continue;

            Q[61] = Q[60] +
                    RL(I(Q[60], Q[59], Q[58]) + Q[57] + x[4] + 0xf7537e82u, 6);

            if (bit(Q[61], 26) != 1)
              continue;

            if (bit(Q[61], 32) != bit(Q[59], 32))
              continue;

            // Extra conditions: Σ62,16 ~ Σ62,22 not all 0
            // 0x003f8000u =  0000 0000 0011 1111 1000 0000 0000 0000
            sigma_Q62 = I(Q[61], Q[60], Q[59]) + Q[58] + x[11] + 0xbd3af235u;
            if ((sigma_Q62 & 0x003f8000u) == 0)
              continue;

            Q[62] = Q[61] + RL(sigma_Q62, 10);

            if (bit(Q[62], 26) != 1)
              continue;

            if (bit(Q[62], 32) != bit(Q[60], 32))
              continue;

            Q[63] = Q[62] +
                    RL(I(Q[62], Q[61], Q[60]) + Q[59] + x[2] + 0x2ad7d2bbu, 15);

            if (bit(Q[63], 26) != 1)
              continue;

            if (bit(Q[63], 32) != bit(Q[61], 32))
              continue;

            Q[64] = Q[63] +
                    RL(I(Q[63], Q[62], Q[61]) + Q[60] + x[9] + 0xeb86d391u, 21);

            // Condition not necessary (Sasaki), try to remove
            if (bit(Q[64], 26) != 1)
              continue;

            // Block 2 is now completed. We verify if the differential path is
            // reached.

            // Message 1 intermediate hash
            AA0 = A0 + Q[61];
            BB0 = B0 + Q[64];
            CC0 = C0 + Q[63];
            DD0 = D0 + Q[62];

            // Message 2 intermediate hash computation
            for (i = 0; i < 16; i++)
              Hx[i] = x[i];

            Hx[4] = x[4] - 0x80000000u;
            Hx[11] = x[11] - 0x00008000u;
            Hx[14] = x[14] - 0x80000000u;

            a = A1;
            b = B1;
            c = C1;
            d = D1;

            HMD5Tr();

            AA1 = A1 + a;
            BB1 = B1 + b;
            CC1 = C1 + c;
            DD1 = D1 + d;

            if (((AA1 - AA0) != 0) || ((BB1 - BB0) != 0) ||
                ((CC1 - CC0) != 0) || ((DD1 - DD0) != 0))
              continue;

            // We have now found a collision!!

            // I save the last intermediate hash for final hash computation
            A0 = AA0;
            B0 = BB0;
            C0 = CC0;
            D0 = DD0;

            printf("A0 %u \n", A0);
            printf("Took %d  and %d iterations \n", it, itr_q9);
            // I save both second blocks
            for (i = 0; i < 16; i++) {
              memcpy(&v1[64 + (i * 4)], &x[i], 4);
              memcpy(&v2[64 + (i * 4)], &Hx[i], 4);
              /* for (int j = 0; j < 4; j++) { */
              /*   printf("%u, ", v1[64 + 4 * i + j]); */
              /* } */
              /* printf("\n"); */
            }

            return (0);

          }    // End of Tunnel Q9
        }      // End of MMMM Q4
      }        // End of MMMM Q1/12
    }          // End of MMMM Q16
  }            // End of general for
  return (-1); // Collision not found
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

u32 create_return_from_tunnels() {
  u32 res = 0u;
  res = res + tunnel20;
  res = res << 3;
  res = res + tunnel10;
  res = res << 9;
  res = res + tunnel14;
  return res;
}

void main() {
    pos = ivec2(position * 256.0);
    u32 x = uint(pos.x);
    u32 y = uint(pos.y);
    X = seed;
    IV1 = 0x67452301u;
    IV2 = 0xefcdab89u;
    IV3 = 0x98badcfeu;
    IV4 = 0x10325476u;
    u32 id = x + y * 256u;
    int it = Block1(id);
    if (it >= 0) {
      color = return_vec(create_return_from_tunnels());
    } else {
      discard;
    }
}
