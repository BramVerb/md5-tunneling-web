#version 300 es
in highp vec2 position;
highp ivec2 pos;


out highp vec4 color;

#define USE_B1_Q4 1
#define USE_B1_Q9 1
#define USE_B1_Q10 1
#define USE_B1_Q13 1
#define USE_B1_Q14 1
#define USE_B1_Q20 1
#define USE_B2_Q9 1

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


uint a,b,c,d;
uint Hx[16];
uint IV1,IV2,IV3,IV4;

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
uint mix(uint a) {
   a = (a+0x7ed55d16u) + (a<<12);
   a = (a^0xc761c23cu) ^ (a>>19);
   a = (a+0x165667b1u) + (a<<5);
   a = (a+0xd3a2646cu) ^ (a<<9);
   a = (a+0xfd7046c5u) + (a<<3);
   a = (a^0xb55a4f09u) ^ (a>>16);
   return a;
}


//Random number generator. We will use an LCG pseudo random generator. Different options are possible
uint X;
uint rng() {
  X = (1664525u*X + 1013904223u) & 0xffffffffu;
  //X = (1103515245*X + 12345) & 0xffffffffu;
  return X;
}

const highp uint mask_bit[33] = uint[]( 0x0u,
        0x00000001u,0x00000002u,0x00000004u,0x00000008u,0x00000010u,0x00000020u,0x00000040u,0x00000080u,
        0x00000100u,0x00000200u,0x00000400u,0x00000800u,0x00001000u,0x00002000u,0x00004000u,0x00008000u,
        0x00010000u,0x00020000u,0x00040000u,0x00080000u,0x00100000u,0x00200000u,0x00400000u,0x00800000u,
        0x01000000u,0x02000000u,0x04000000u,0x08000000u,0x10000000u,0x20000000u,0x40000000u,0x80000000u );

uint bit(uint a, uint b) {
    if ((b==0u) || (b > 32u))
      return 0u;
    else
      return (a & mask_bit[b]) >> (b-1u);
}




/* uint * generate_mask(uint strength, uint * mask_bits) { */

/*         uint mask_cardinality = (int32_t) pow(2, strength); */

/*         uint * mask = calloc( mask_cardinality , sizeof(uint) ); */

/*         //If more or equal 32 bits tunneling is useless */
/*         if (strength < 32) { */

/*                 uint i,j; */

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



int Block1() {

  uint Q[65], x[16], QM0, QM1, QM2, QM3;
  uint sigma_Q19, sigma_Q20, sigma_Q23, sigma_Q35, sigma_Q62;
  uint i, itr_Q9, itr_Q4, itr_Q14, itr_Q13, itr_Q20, itr_Q10;
  uint tmp_q3, tmp_q4, tmp_q13, tmp_q14, tmp_q20, tmp_q21, tmp_q9, tmp_q10;
  uint tmp_x1, tmp_x15, tmp_x4;
  uint Q3_fix, Q4_fix, Q14_fix, const_masked, const_unmasked;
  uint AA0, BB0, CC0, DD0, AA1, BB1, CC1, DD1;


  //Mask generation for tunnel Q4 - 1 bit
  int Q4_mask_bits[] = int[]( 26 );
  int Q4_strength = 1;
  /* const uint * mask_Q4 = generate_mask(Q4_strength, Q4_mask_bits); */

  //Mask generation for tunnel Q9 - 3 bits
  int Q9_mask_bits[] = int[]( 22, 23, 24 );
  int Q9_strength = 3;
  /* const uint * mask_Q9 = generate_mask(Q9_strength, Q9_mask_bits); */ 

  //Mask generation for tunnel Q13 - 12 bits
  int Q13_mask_bits[] = int[]( 2,3,5,7,10,11,12,21,22,23,28,29 );
  int Q13_strength = 12;
  /* const uint * mask_Q13 = generate_mask(Q13_strength, Q13_mask_bits); */ 

  //Mask generation for tunnel Q20 - 6 bits
  int Q20_mask_bits[] = int[]( 1, 2, 10, 15, 22, 24 );
  int Q20_strength = 6;
  /* const uint * mask_Q20 = generate_mask(Q20_strength, Q20_mask_bits); */ 

  //Mask generation for tunnel Q10 - 3 bits
  int Q10_mask_bits[] = int[]( 11, 25, 27 );
  int Q10_strength = 3;
  /* const uint * mask_Q10 = generate_mask(Q10_strength, Q10_mask_bits); */ 

  //Mask generation for tunnel Q14 - 9 bits
  int Q14_mask_bits[] = int[]( 1, 2, 3, 5, 6, 7, 27, 28, 29 );
  int Q14_strength = 9;
  /* const uint * mask_Q14 = generate_mask(Q14_strength, Q14_mask_bits); */ 

  //Initialization vectors
  QM3 = IV1;  QM0 = IV2;
  QM1 = IV3;  QM2 = IV4;


  //Start block 1 generation. 
  //TO-DO: add a time limit for collision search.
  for( ; ; ) {

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
    Q[ 2] = Q[ 1] + RL( F(Q[ 1],QM0  ,QM1  ) + QM2   + x[1] + 0xe8c7b756u,12);

    // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[18] = Q[17] + RL( G(Q[17],Q[16],Q[15]) + Q[14] + x[6] + 0xc040b340u, 9);

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

    Q[19] = Q[18] + RL(sigma_Q19, 14);

    // Q[18] = ^.^. .... .... ..1. .... .... .... .... 
    // Q[19] = ^... .... .... ..0. .... .... .... .... 
    //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
    if ( ((Q[19] ^ Q[18]) & 0x80020000u) != 0x00020000u ) 
      continue;

    // Q[20] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Extra conditions: Σ20,30 ~ Σ20,32 not all 0
    // 0xe0000000u = 1110 0000 0000 0000 0000 0000 0000 0000
    sigma_Q20 = G(Q[19],Q[18],Q[17]) + Q[16] + x[0] + 0xe9b6c7aau;
    if ( (sigma_Q20  & 0xe0000000u) == 0 ) 
      continue;

    Q[20] = Q[19] + RL(sigma_Q20, 20);

    // Q[20] = ^... .... .... ..v. .... .... .... .... 
    if ( bit(Q[20],32) != bit(Q[15],32) ) 
      continue;

    // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5);   

    // Q[20] = ^... .... .... ..v. .... .... .... .... 
    // Q[21] = ^... .... .... ..^. .... .... .... ....
    //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
    if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0 ) 
      continue;

    // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9);

    // Q[22] = ^... .... .... .... .... .... .... ....
    if ( bit(Q[22],32) != bit(Q[15],32) ) 
      continue;

    // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Extra conditions: Σ23,18 = 0
    sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
    if ( bit(sigma_Q23,18) != 0 ) 
      continue;

    Q[23] = Q[22] + RL(sigma_Q23, 14);

    // Q[23] = 0... .... .... .... .... .... .... ....
    if ( bit(Q[23],32) != 0 ) 
      continue;

    // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

    // Q[24] = 1... .... .... .... .... .... .... ....
    if ( bit(Q[24],32) != 1) 
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
    for (itr_Q10 = 0; itr_Q10 < (USE_B1_Q10 ? pow(2,Q10_strength) : 1); itr_Q10++ ) {

      Q[9]  = tmp_q9;
      Q[10] = tmp_q10;  
      Q[13] = tmp_q13;
      Q[20] = tmp_q20;
      Q[21] = tmp_q21;

      x[4]  = tmp_x4;
      x[15] = tmp_x15;

      //Multi message modification - Q10 is modified according to its mask (bits 11,25,27)
      Q[10] = tmp_q10 ^ mask_Q10[USE_B1_Q10 ? itr_Q10 : 0];
      
      //x[10] is modified and related states are regenerated
      x[10] = RR(Q[11]-Q[10],17) - F(Q[10],Q[ 9],Q[ 8]) - Q[ 7] - 0xffff5bb1u; 

      //Q10 Tunnel - Verification of bit conditions on Q[22-24]
      
      // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9);

      // Q[22] = ^... .... .... .... .... .... .... ....
      if ( bit(Q[22],32) != bit(Q[15],32) ) 
        continue;
      
      // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ23,18 = 0
      sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
      if ( bit(sigma_Q23,18) != 0 ) 
        continue;

      Q[23] = Q[22] + RL(sigma_Q23, 14);

      // Q[23] = 0... .... .... .... .... .... .... ....
      if ( bit(Q[23],32) != 0 ) 
        continue;

      // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

      // Q[24] = 1... .... .... .... .... .... .... ....
      if ( bit(Q[24],32) != 1) 
        continue;

      ///////////////////////////////////////////////////////////////
      ///                       Tunnel Q20                         //
      ///////////////////////////////////////////////////////////////
      //Tunnel Q20 - 6 bits - Probabilistic. Modifications on Q[20] and free choice of Q[1] and Q[2] lead to change in x[0] and x[2..5]
      for (itr_Q20 = 0; itr_Q20 < (USE_B1_Q20 ? pow(2,Q20_strength) : 1); itr_Q20++) {

        Q[3]  = tmp_q3;
        Q[4]  = tmp_q4;

        x[1]  = tmp_x1;
        x[15] = tmp_x15;

        //Q20 is modified according to its mask (bits 1,2,10,15,22,24)
        Q[20] = tmp_q20 ^ mask_Q20[USE_B1_Q20 ? itr_Q20 : 0];

        x[ 0] = RR(Q[20] - Q[19],20) - G(Q[19],Q[18],Q[17]) - Q[16] - 0xe9b6c7aau;

        Q[ 1] = QM0  + RL(F( QM0, QM1, QM2) + QM3 + x[0] + 0xd76aa478u,  7);
        Q[ 2] = Q[1] + RL(F(Q[1], QM0, QM1) + QM2 + x[1] + 0xe8c7b756u, 12);

        x[ 4] = RR(Q[5] - Q[4],  7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;
        x[ 5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62au;

        // Tunnel Q20 - Verification of bit conditions on Q[21-24]
        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5);   

        // Q[20] = ^... .... .... ..v. .... .... .... .... 
        // Q[21] = ^... .... .... ..^. .... .... .... ....
        //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
        if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0 ) 
          continue;

        // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9);

        // Q[22] = ^... .... .... .... .... .... .... ....
        if ( bit(Q[22],32) != bit(Q[15],32) ) 
          continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ23,18 = 0
        sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
        if ( bit(sigma_Q23,18) != 0 ) 
          continue;

        Q[23] = Q[22] + RL(sigma_Q23, 14);

        // Q[23] = 0... .... .... .... .... .... .... ....
        if ( bit(Q[23],32) != 0 ) 
          continue;

        // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

        // Q[24] = 1... .... .... .... .... .... .... ....
        if ( bit(Q[24],32) != 1) 
          continue;

        ///////////////////////////////////////////////////////////////
        ///                       Tunnel Q13                         //
        ///////////////////////////////////////////////////////////////
        //Tunnel Q13 - 12 bits - Probabilistic. Modifications on Q[13] and free choice of Q[2] lead to change in x[1..5] and x[15]
        for(itr_Q13 = 0; itr_Q13 < (USE_B1_Q13 ? pow(2,Q13_strength) : 1); itr_Q13++ ) {
          
          Q[3]  = tmp_q3;
          Q[4]  = tmp_q4;
          Q[14] = tmp_q14;

          Q[13] = tmp_q13 ^ mask_Q13[USE_B1_Q13 ? itr_Q13 : 0];
          
          x[ 1] = RR(Q[17] - Q[16], 5) - G(Q[16], Q[15], Q[14]) - Q[13] - 0xf61e2562u;
          
          Q[ 2] = Q[ 1] + RL(F(Q[1 ], QM0, QM1) + QM2 + x[ 1] + 0xe8c7b756u, 12);
          
          x[ 4] = RR(Q[ 5] - Q[ 4], 7) - F(Q[ 4], Q[ 3], Q[ 2]) - Q[ 1] - 0xf57c0fafu;
          x[ 5] = RR(Q[ 6] - Q[ 5],12) - F(Q[ 5], Q[ 4], Q[ 3]) - Q[ 2] - 0x4787c62au;
          x[15] = RR(Q[16] - Q[15],22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821u;

          // Tunnel Q13 - Verification of bit conditions on Q[21-24]
          // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[21] = Q[20] + RL(G(Q[20],Q[19],Q[18]) + Q[17] + x[5] + 0xd62f105du, 5);   

          // Q[20] = ^... .... .... ..v. .... .... .... .... 
          // Q[21] = ^... .... .... ..^. .... .... .... ....
          //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000u 
          if ( ((Q[21] ^ Q[20]) & 0x80020000u) != 0 ) 
            continue;

          // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[22] = Q[21] + RL(G(Q[21],Q[20],Q[19]) + Q[18] + x[10] + 0x2441453u, 9);

          // Q[22] = ^... .... .... .... .... .... .... ....
          if ( bit(Q[22],32) != bit(Q[15],32) ) 
            continue;

          // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          // Extra conditions: Σ23,18 = 0
          sigma_Q23 = G(Q[22],Q[21],Q[20]) + Q[19] + x[15] + 0xd8a1e681u;
          if ( bit(sigma_Q23,18) != 0 ) 
            continue;

          Q[23] = Q[22] + RL(sigma_Q23, 14);

          // Q[23] = 0... .... .... .... .... .... .... ....
          if ( bit(Q[23],32) != 0 ) 
            continue;

          // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[24] = Q[23] + RL(G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);

          // Q[24] = 1... .... .... .... .... .... .... ....
          if ( bit(Q[24],32) != 1) 
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
          const_unmasked = RR(Q[7]-Q[6],17) - 0xa8304613u //= F(Q[ 6],Q[ 5],Q[ 4]) + Q[ 3] + x[ 6]
                          -RR(Q[18]-Q[17],9) + G(Q[17],Q[16],Q[15]) + 0xc040b340u //= -Q[14] -x[ 6]
                          -F(Q[6],Q[5],Q4_fix) - Q3_fix + Q14_fix;

          //So const_unmasked is the difference (F(Q[6],Q[5],Q[4]) - F(Q[6],Q[5],Q4_fix)) + (Q[3]-Q3_fix) - (Q[14]-Q14_fix)
          
          //From const_unmasked, that depends on the current value for Q[5], we'll get the new values for Q[3],Q[4] 
          //(i.e. the bits that we have to change to don't affect x[5])


          //Tunnel Q14 starts
          for(itr_Q14 = 0; itr_Q14 < (USE_B1_Q14 ? pow(2,Q14_strength) : 1); itr_Q14++ ) {

            //Q14 is modified according to its mask {1, 2, 3, 5, 6, 7, 27, 28, 29}
            //NOTE that const_unmasked consider carries. So operations are +,- and not XOR.
            const_masked = const_unmasked + mask_Q14[USE_B1_Q14 ? itr_Q14 : 0];
            
            //If the current value for Q[14] affects bits in const_masked that are outside 
            //0xfc00002fu = 1111  1100  0000  0000  0000  0000  0010  1111
            //this means that this modification cannot be compensated by Q[3]/Q[4] and then we need to continue
            //
            //0x03ffffd0u = 0000  0011  1111  1111  1111  1111  1101  0000
            if ((const_masked & 0x03ffffd0u) != 0)
              continue;

            //We recover the remaining bits of Q[3],Q[4] and Q[14] from the current const_masked
            Q[ 3] = Q3_fix + (const_masked & 0x88000025u);
            Q[ 4] = Q4_fix + (const_masked & 0x7400000au);
            Q[14] = Q14_fix + mask_Q14[itr_Q14];

            x[2] = RR(Q[3] - Q[2], 17) - F(Q[2], Q[1], QM0) - QM1 - 0x242070dbu;

            ///////////////////////////////////////////////////////////////
            ///                       Tunnel Q4                          //
            ///////////////////////////////////////////////////////////////
            //Tunnel Q4 - 1 bit - Probabilistic tunnel. Modification on Q[4][26] will probably affect Q[24][32] 
            for (itr_Q4 = 0; itr_Q4 < (USE_B1_Q4 ? pow(2,Q4_strength) : 1); itr_Q4++) {

              Q[4] = Q[4] ^ mask_Q4[USE_B1_Q4 ? itr_Q4 : 0];

              x[4] = RR(Q[5] - Q[4],  7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0fafu;

              // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
              Q[24] = Q[23] + RL( G(Q[23],Q[22],Q[21]) + Q[20] + x[4] + 0xe7d3fbc8u, 20);
                
              // Q[24] = 1... .... .... .... .... .... .... ....
              if (bit(Q[24],32) != 1) 
                continue;

              x[ 3] = RR(Q[ 4] - Q[ 3], 22) - F(Q[ 3], Q[ 2], Q[ 1]) -   QM0 - 0xc1bdceeeu;
              x[ 6] = RR(Q[ 7] - Q[ 6], 17) - F(Q[ 6], Q[ 5], Q[ 4]) - Q[ 3] - 0xa8304613u; 
              x[ 7] = RR(Q[ 8] - Q[ 7], 22) - F(Q[ 7], Q[ 6], Q[ 5]) - Q[ 4] - 0xfd469501u;
              x[13] = RR(Q[14] - Q[13], 12) - F(Q[13], Q[12], Q[11]) - Q[10] - 0xfd987193u; 
              x[14] = RR(Q[15] - Q[14], 17) - F(Q[14], Q[13], Q[12]) - Q[11] - 0xa679438eu; 

     
              ///////////////////////////////////////////////////////////////
              ///                       Tunnel Q9                          //
              ///////////////////////////////////////////////////////////////
              //Tunnel Q9 - 3 bits - Deterministic tunnel. If the i-th bit of Q[10] would be zero 
              //and the i-th bit of Q[11] would be one, an eventual change of the i-th 
              //bit of Q[9] shouldn't affect the equations for Q[11] and Q[12].
              for(itr_Q9 = 0; itr_Q9 < (USE_B1_Q9 ? pow(2,Q9_strength) : 1); itr_Q9++ ) {

                  Q[ 9] = tmp_q9 ^ mask_Q9[USE_B1_Q9 ? itr_Q9 : 0]; 

                  x[ 8] = RR(Q[ 9]-Q[ 8],  7) - F(Q[ 8], Q[ 7], Q[ 6]) - Q[5] - 0x698098d8u;
                  x[ 9] = RR(Q[10]-Q[ 9], 12) - F(Q[ 9], Q[ 8], Q[ 7]) - Q[6] - 0x8b44f7afu;    
                  x[12] = RR(Q[13]-Q[12],  7) - F(Q[12], Q[11], Q[10]) - Q[9] - 0x6b901122u;
                 
                  Q[25] = Q[24] + RL(G(Q[24], Q[23], Q[22]) + Q[21] + x[ 9] + 0x21e1cde6u,  5);
                  Q[26] = Q[25] + RL(G(Q[25], Q[24], Q[23]) + Q[22] + x[14] + 0xc33707d6u,  9);            
                  Q[27] = Q[26] + RL(G(Q[26], Q[25], Q[24]) + Q[23] + x[ 3] + 0xf4d50d87u, 14);
                  Q[28] = Q[27] + RL(G(Q[27], Q[26], Q[25]) + Q[24] + x[ 8] + 0x455a14edu, 20);
                  Q[29] = Q[28] + RL(G(Q[28], Q[27], Q[26]) + Q[25] + x[13] + 0xa9e3e905u,  5);
                  Q[30] = Q[29] + RL(G(Q[29], Q[28], Q[27]) + Q[26] + x[ 2] + 0xfcefa3f8u,  9);
                  Q[31] = Q[30] + RL(G(Q[30], Q[29], Q[28]) + Q[27] + x[ 7] + 0x676f02d9u, 14);
                  Q[32] = Q[31] + RL(G(Q[31], Q[30], Q[29]) + Q[28] + x[12] + 0x8d2a4c8au, 20);
                  Q[33] = Q[32] + RL(H(Q[32], Q[31], Q[30]) + Q[29] + x[ 5] + 0xfffa3942u,  4);          
                  Q[34] = Q[33] + RL(H(Q[33], Q[32], Q[31]) + Q[30] + x[ 8] + 0x8771f681u, 11);

                  // Extra conditions: Σ35,16 = 0
                  sigma_Q35 = H(Q[34],Q[33],Q[32]) + Q[31] + x[11] + 0x6d9d6122u;
                  if (bit(sigma_Q35,16) != 0)
                    continue; 

                  Q[35] = Q[34] + RL(sigma_Q35, 16);

                  Q[36] = Q[35] + RL(H(Q[35], Q[34], Q[33]) + Q[32] + x[14] + 0xfde5380cu, 23);
                  Q[37] = Q[36] + RL(H(Q[36], Q[35], Q[34]) + Q[33] + x[ 1] + 0xa4beea44u,  4);
                  Q[38] = Q[37] + RL(H(Q[37], Q[36], Q[35]) + Q[34] + x[ 4] + 0x4bdecfa9u, 11);
                  Q[39] = Q[38] + RL(H(Q[38], Q[37], Q[36]) + Q[35] + x[ 7] + 0xf6bb4b60u, 16);
                  Q[40] = Q[39] + RL(H(Q[39], Q[38], Q[37]) + Q[36] + x[10] + 0xbebfbc70u, 23);
                  Q[41] = Q[40] + RL(H(Q[40], Q[39], Q[38]) + Q[37] + x[13] + 0x289b7ec6u,  4);
                  Q[42] = Q[41] + RL(H(Q[41], Q[40], Q[39]) + Q[38] + x[ 0] + 0xeaa127fau, 11);
                  Q[43] = Q[42] + RL(H(Q[42], Q[41], Q[40]) + Q[39] + x[ 3] + 0xd4ef3085u, 16);
                  Q[44] = Q[43] + RL(H(Q[43], Q[42], Q[41]) + Q[40] + x[ 6] + 0x04881d05u, 23);
                  Q[45] = Q[44] + RL(H(Q[44], Q[43], Q[42]) + Q[41] + x[ 9] + 0xd9d4d039u,  4);
                  Q[46] = Q[45] + RL(H(Q[45], Q[44], Q[43]) + Q[42] + x[12] + 0xe6db99e5u, 11);
                  Q[47] = Q[46] + RL(H(Q[46], Q[45], Q[44]) + Q[43] + x[15] + 0x1fa27cf8u, 16);
                  Q[48] = Q[47] + RL(H(Q[47], Q[46], Q[45]) + Q[44] + x[ 2] + 0xc4ac5665u, 23);
                                  
                  //Sufficient conditions
                  if ( bit(Q[46], 32) != bit(Q[48], 32) ) 
                    continue; 

                  Q[49] = Q[48] + RL(I(Q[48], Q[47], Q[46]) + Q[45] + x[ 0] + 0xf4292244u,  6);
                              
                  if (bit(Q[47],32) != bit(Q[49],32)) 
                    continue;

                  Q[50] = Q[49] + RL(I(Q[49], Q[48], Q[47]) + Q[46] + x[ 7] + 0x432aff97u, 10);
              
                  if (bit(Q[50],32) != (bit(Q[48],32) ^ 1)) 
                    continue;

                  Q[51] = Q[50] + RL(I(Q[50], Q[49], Q[48]) + Q[47] + x[14] + 0xab9423a7u, 15);
                  
                  if (bit(Q[51],32) != bit(Q[49],32)) 
                    continue;  
                  
                  Q[52] = Q[51] + RL(I(Q[51], Q[50], Q[49]) + Q[48] + x[ 5] + 0xfc93a039u, 21);
                      
                  if (bit(Q[52],32) != bit(Q[50],32)) 
                    continue; 

                  Q[53] = Q[52] + RL(I(Q[52], Q[51], Q[50]) + Q[49] + x[12] + 0x655b59c3u, 6); 
                                
                  if (bit(Q[53],32) != bit(Q[51],32)) 
                    continue; 

                  Q[54] = Q[53] + RL(I(Q[53], Q[52], Q[51]) + Q[50] + x[ 3] + 0x8f0ccc92u, 10);    
                  
                  if (bit(Q[54],32) != bit(Q[52],32)) 
                    continue; 

                  Q[55] = Q[54] + RL(I(Q[54], Q[53], Q[52]) + Q[51] + x[10] + 0xffeff47du, 15);   
                  
                  if (bit(Q[55],32) != bit(Q[53],32)) 
                    continue; 

                  Q[56] = Q[55] + RL(I(Q[55], Q[54], Q[53]) + Q[52] + x[ 1] + 0x85845dd1u, 21);    
                  
                  if (bit(Q[56],32) != bit(Q[54],32)) 
                    continue; 

                  Q[57] = Q[56] + RL(I(Q[56], Q[55], Q[54]) + Q[53] + x[ 8] + 0x6fa87e4fu, 6);   
                  
                  if (bit(Q[57],32) != bit(Q[55],32)) 
                    continue; 

                  Q[58] = Q[57] + RL(I(Q[57], Q[56], Q[55]) + Q[54] + x[15] + 0xfe2ce6e0u, 10);   
                  
                  if (bit(Q[58],32) != bit(Q[56],32)) 
                    continue; 

                  Q[59] = Q[58] + RL(I(Q[58], Q[57], Q[56]) + Q[55] + x[ 6] + 0xa3014314u, 15);    
                  
                  if (bit(Q[59],32) != bit(Q[57],32)) 
                    continue; 

                  Q[60] = Q[59] + RL(I(Q[59], Q[58], Q[57]) + Q[56] + x[13] + 0x4e0811a1u, 21);   
                  
                  if (bit(Q[60],26) != 0) 
                    continue; 

                  if (bit(Q[60],32) != (bit(Q[58],32) ^ 1)) 
                    continue; 

                  Q[61] = Q[60] + RL(I(Q[60], Q[59], Q[58]) + Q[57] + x[ 4] + 0xf7537e82u,  6);   
                  
                  if (bit(Q[61],26) != 1) 
                    continue; 

                  if (bit(Q[61],32) != bit(Q[59],32)) 
                    continue; 

                  //Extra conditions: Σ62,16 ~ Σ62,22 not all ones
                  //0x003f8000u = 0000  0000  0011  1111  1000  0000  0000  0000
                  sigma_Q62 = I(Q[61],Q[60],Q[59]) + Q[58] + x[11] + 0xbd3af235u;
                  if ( (sigma_Q62 & 0x003f8000u) == 0x003f8000u )
                    continue;

                  Q[62] = Q[61] + RL(sigma_Q62 , 10); 

                  Q[63] = Q[62] + RL(I(Q[62], Q[61], Q[60]) + Q[59] + x[2] + 0x2ad7d2bbu, 15);    
                  Q[64] = Q[63] + RL(I(Q[63], Q[62], Q[61]) + Q[60] + x[9] + 0xeb86d391u, 21);    
                    
                  //We add the initial vector to obtain the Intermediate Hash Values of the current block
                  AA0 = IV1 + Q[61];  BB0 = IV2 + Q[64];
                  CC0 = IV3 + Q[63];  DD0 = IV4 + Q[62];
                  
                  //Last sufficient conditions  
                  if (bit(BB0,6) != 0) 
                    continue;

                  if (bit(BB0,26) != 0) 
                    continue;

                  if (bit(BB0,27) != 0) 
                    continue;

                  if (bit(CC0,26) != 1)
                    continue;

                  if (bit(CC0,27) != 0) 
                    continue;  

                  if (bit(DD0,26) != 0 ) 
                    continue;

                  if (bit(BB0,32) != bit(CC0,32)) 
                    continue;

                  if (bit(CC0,32) != bit(DD0,32)) 
                    continue;

                  //Message 1 block 1 computation completed. 
                  
                  //Now we see if the differential path is verified
                  //Note that message 1 block 1 is x = x[0]||...||x[15]
                  //While message 2 block 1 is Hx = x + C

                  //Message 2 block 1 hash computation
                  for(i = 0; i < 16; i++) 
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
                  for (i=0; i<16; i++) {
                    memcpy( &v1[4*i], &x[i],  4); 
                    memcpy( &v2[4*i], &Hx[i], 4);
                  }

                  return 0;

                } //End of Q9 Tunnel
              } //End of Q4 Tunnel
            } //End of Q14 Tunnel
          } //End of Q13 Tunnel
        } //End of Q20 Tunnel
      } //End of Q10 Tunnel
    } //End of general for
  return(-1); //Collision not found;
}



void main() {
    /* gl_FragColor = vec4(0.0, 1.0, 1.0, 1.0); */
    pos = ivec2(position * 256.0);
    uint x = uint(pos.x);
    uint y = uint(pos.y);
    if((x & 2u) == 2u) {
        color = vec4(position.x, position.y, 1.0, 1.0);
    } else {
        color = vec4(1.0, 1.0, position.x, 1.0);
    }
    /* color = vec4(position.x, position.y, 1.0, 1.0); */
    HMD5Tr();
}
